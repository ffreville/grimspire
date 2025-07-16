using System;
using System.Collections.Generic;
using UnityEngine;

public class EconomicSystem : MonoBehaviour
{
    public static EconomicSystem Instance { get; private set; }
    
    [Header("Economic Settings")]
    [SerializeField] private float dailyCalculationTime = 24f; // Game hours for daily calculations
    [SerializeField] private bool enableRealTimeCalculations = true;
    [SerializeField] private float realTimeInterval = 1f; // Real seconds between calculations
    
    [Header("Tax Settings")]
    [SerializeField] private float baseTaxRate = 0.1f; // 10% base tax rate
    [SerializeField] private float taxRatePerPopulation = 0.001f; // Tax efficiency increases with population
    [SerializeField] private float maxTaxRate = 0.25f; // Maximum 25% tax rate
    
    [Header("Economic Modifiers")]
    [SerializeField] private float reputationBonusMultiplier = 0.02f; // 2% bonus per reputation point
    [SerializeField] private float happinessMultiplier = 1.0f; // Population happiness affects productivity
    [SerializeField] private float seasonalMultiplier = 1.0f; // Seasonal economic effects
    
    [Header("Market Settings")]
    [SerializeField] private Dictionary<Resource.ResourceType, float> resourceMarketPrices;
    [SerializeField] private float priceVolatility = 0.1f; // 10% price variation
    
    private float lastCalculationTime;
    private float accumulatedTime;
    private Dictionary<Resource.ResourceType, int> dailyIncome;
    private Dictionary<Resource.ResourceType, int> dailyExpenses;
    private Dictionary<Resource.ResourceType, int> netFlow;
    
    public float BaseTaxRate => baseTaxRate;
    public float CurrentTaxRate => GetCurrentTaxRate();
    public Dictionary<Resource.ResourceType, int> DailyIncome => new Dictionary<Resource.ResourceType, int>(dailyIncome);
    public Dictionary<Resource.ResourceType, int> DailyExpenses => new Dictionary<Resource.ResourceType, int>(dailyExpenses);
    public Dictionary<Resource.ResourceType, int> NetFlow => new Dictionary<Resource.ResourceType, int>(netFlow);
    
    // Events
    public static event Action<Dictionary<Resource.ResourceType, int>> OnDailyIncomeCalculated;
    public static event Action<Dictionary<Resource.ResourceType, int>> OnDailyExpensesCalculated;
    public static event Action<Dictionary<Resource.ResourceType, int>> OnNetFlowCalculated;
    public static event Action<float> OnTaxRateChanged;
    public static event Action<Resource.ResourceType, float> OnMarketPriceChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeEconomicSystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeEconomicSystem()
    {
        dailyIncome = new Dictionary<Resource.ResourceType, int>();
        dailyExpenses = new Dictionary<Resource.ResourceType, int>();
        netFlow = new Dictionary<Resource.ResourceType, int>();
        
        InitializeResourceMarketPrices();
        ResetDailyCalculations();
        
        lastCalculationTime = Time.time;
    }

    private void InitializeResourceMarketPrices()
    {
        resourceMarketPrices = new Dictionary<Resource.ResourceType, float>
        {
            [Resource.ResourceType.Gold] = 1.0f,
            [Resource.ResourceType.Stone] = 2.0f,
            [Resource.ResourceType.Wood] = 1.5f,
            [Resource.ResourceType.Iron] = 5.0f,
            [Resource.ResourceType.MagicCrystal] = 20.0f
        };
    }

    private void Update()
    {
        if (enableRealTimeCalculations)
        {
            accumulatedTime += Time.deltaTime;
            
            if (accumulatedTime >= realTimeInterval)
            {
                ProcessEconomicTick();
                accumulatedTime = 0f;
            }
        }
    }

    private void ProcessEconomicTick()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameInitialized)
        {
            City currentCity = GameManager.Instance.CurrentCity;
            if (currentCity != null)
            {
                CalculateRealTimeIncome(currentCity);
                CalculateRealTimeExpenses(currentCity);
                ApplyEconomicEffects();
            }
        }
    }

    public void ProcessDailyEconomics(City city)
    {
        if (city == null) return;
        
        CalculateDailyIncome(city);
        CalculateDailyExpenses(city);
        CalculateNetFlow();
        ApplyDailyResults();
        
        OnDailyIncomeCalculated?.Invoke(dailyIncome);
        OnDailyExpensesCalculated?.Invoke(dailyExpenses);
        OnNetFlowCalculated?.Invoke(netFlow);
    }

    private void CalculateDailyIncome(City city)
    {
        ResetDailyCalculations();
        
        // Tax income from population
        int populationTaxIncome = CalculatePopulationTaxIncome(city);
        dailyIncome[Resource.ResourceType.Gold] += populationTaxIncome;
        
        // Building production income
        foreach (var building in city.Buildings)
        {
            if (building.IsBuilt)
            {
                var production = building.GetCurrentProduction();
                foreach (var prod in production)
                {
                    if (!dailyIncome.ContainsKey(prod.Key))
                        dailyIncome[prod.Key] = 0;
                    dailyIncome[prod.Key] += prod.Value;
                }
            }
        }
        
        // Apply economic modifiers
        ApplyEconomicModifiers(city, dailyIncome);
    }

    private void CalculateDailyExpenses(City city)
    {
        // Building maintenance costs
        foreach (var building in city.Buildings)
        {
            if (building.IsBuilt)
            {
                foreach (var maintenance in building.MaintenanceCosts)
                {
                    if (!dailyExpenses.ContainsKey(maintenance.Key))
                        dailyExpenses[maintenance.Key] = 0;
                    dailyExpenses[maintenance.Key] += maintenance.Value;
                }
            }
        }
        
        // Adventurer salaries and costs
        int adventurerCosts = CalculateAdventurerCosts(city);
        dailyExpenses[Resource.ResourceType.Gold] += adventurerCosts;
        
        // Administrative costs based on city size
        int adminCosts = CalculateAdministrativeCosts(city);
        dailyExpenses[Resource.ResourceType.Gold] += adminCosts;
    }

    private void CalculateRealTimeIncome(City city)
    {
        float timeMultiplier = realTimeInterval / dailyCalculationTime;
        
        // Calculate small incremental income
        int goldIncome = Mathf.FloorToInt(CalculatePopulationTaxIncome(city) * timeMultiplier);
        
        if (goldIncome > 0 && ResourceManager.Instance != null)
        {
            ResourceManager.Instance.AddResource(Resource.ResourceType.Gold, goldIncome);
        }
    }

    private void CalculateRealTimeExpenses(City city)
    {
        float timeMultiplier = realTimeInterval / dailyCalculationTime;
        
        // Calculate small incremental expenses
        int goldExpense = Mathf.FloorToInt(CalculateAdministrativeCosts(city) * timeMultiplier);
        
        if (goldExpense > 0 && ResourceManager.Instance != null)
        {
            ResourceManager.Instance.RemoveResource(Resource.ResourceType.Gold, goldExpense);
        }
    }

    private int CalculatePopulationTaxIncome(City city)
    {
        int population = city.GetTotalPopulation();
        float currentTaxRate = GetCurrentTaxRate();
        
        // Base income per population unit
        int baseIncomePerPop = 5;
        
        return Mathf.FloorToInt(population * baseIncomePerPop * currentTaxRate);
    }

    private int CalculateAdventurerCosts(City city)
    {
        int totalCost = 0;
        
        foreach (var adventurer in city.Adventurers)
        {
            if (adventurer.IsAlive)
            {
                // Base salary increases with level
                int salary = 10 + (adventurer.Level * 5);
                totalCost += salary;
                
                // Injured adventurers cost more (medical expenses)
                if (adventurer.IsInjured)
                {
                    totalCost += 20;
                }
            }
        }
        
        return totalCost;
    }

    private int CalculateAdministrativeCosts(City city)
    {
        int baseCost = 50; // Base administrative cost
        int populationCost = city.GetTotalPopulation() / 10; // 1 gold per 10 population
        int buildingCost = city.Buildings.Count * 2; // 2 gold per building
        
        return baseCost + populationCost + buildingCost;
    }

    private float GetCurrentTaxRate()
    {
        if (ResourceManager.Instance == null) return baseTaxRate;
        
        int population = ResourceManager.Instance.GetResourceAmount(Resource.ResourceType.Population);
        float dynamicRate = baseTaxRate + (population * taxRatePerPopulation);
        
        return Mathf.Clamp(dynamicRate, baseTaxRate, maxTaxRate);
    }

    private void ApplyEconomicModifiers(City city, Dictionary<Resource.ResourceType, int> income)
    {
        if (ResourceManager.Instance == null) return;
        
        // Reputation bonus
        int reputation = ResourceManager.Instance.GetResourceAmount(Resource.ResourceType.Reputation);
        float reputationBonus = 1.0f + (reputation * reputationBonusMultiplier);
        
        // Apply bonuses to gold income
        if (income.ContainsKey(Resource.ResourceType.Gold))
        {
            income[Resource.ResourceType.Gold] = Mathf.FloorToInt(income[Resource.ResourceType.Gold] * reputationBonus * happinessMultiplier * seasonalMultiplier);
        }
    }

    private void CalculateNetFlow()
    {
        netFlow.Clear();
        
        // Calculate net flow for each resource
        var allResourceTypes = new HashSet<Resource.ResourceType>();
        foreach (var key in dailyIncome.Keys) allResourceTypes.Add(key);
        foreach (var key in dailyExpenses.Keys) allResourceTypes.Add(key);
        
        foreach (var resourceType in allResourceTypes)
        {
            int income = dailyIncome.ContainsKey(resourceType) ? dailyIncome[resourceType] : 0;
            int expenses = dailyExpenses.ContainsKey(resourceType) ? dailyExpenses[resourceType] : 0;
            netFlow[resourceType] = income - expenses;
        }
    }

    private void ApplyDailyResults()
    {
        if (ResourceManager.Instance == null) return;
        
        // Apply net flow to resources
        foreach (var flow in netFlow)
        {
            if (flow.Value > 0)
            {
                ResourceManager.Instance.AddResource(flow.Key, flow.Value);
            }
            else if (flow.Value < 0)
            {
                ResourceManager.Instance.RemoveResource(flow.Key, Math.Abs(flow.Value));
            }
        }
    }

    private void ApplyEconomicEffects()
    {
        UpdateMarketPrices();
    }

    private void UpdateMarketPrices()
    {
        foreach (var kvp in resourceMarketPrices)
        {
            float priceChange = UnityEngine.Random.Range(-priceVolatility, priceVolatility);
            float newPrice = kvp.Value * (1.0f + priceChange);
            resourceMarketPrices[kvp.Key] = Mathf.Max(0.1f, newPrice); // Minimum price of 0.1
            
            OnMarketPriceChanged?.Invoke(kvp.Key, newPrice);
        }
    }

    private void ResetDailyCalculations()
    {
        dailyIncome.Clear();
        dailyExpenses.Clear();
        netFlow.Clear();
        
        // Initialize with 0 values for all resource types
        foreach (Resource.ResourceType resourceType in Enum.GetValues(typeof(Resource.ResourceType)))
        {
            dailyIncome[resourceType] = 0;
            dailyExpenses[resourceType] = 0;
            netFlow[resourceType] = 0;
        }
    }

    public void SetTaxRate(float newRate)
    {
        baseTaxRate = Mathf.Clamp(newRate, 0.05f, 0.5f);
        OnTaxRateChanged?.Invoke(baseTaxRate);
    }

    public void SetHappinessMultiplier(float multiplier)
    {
        happinessMultiplier = Mathf.Clamp(multiplier, 0.1f, 2.0f);
    }

    public void SetSeasonalMultiplier(float multiplier)
    {
        seasonalMultiplier = Mathf.Clamp(multiplier, 0.5f, 1.5f);
    }

    public float GetMarketPrice(Resource.ResourceType resourceType)
    {
        return resourceMarketPrices.ContainsKey(resourceType) ? resourceMarketPrices[resourceType] : 1.0f;
    }

    public int CalculateResourceValue(Resource.ResourceType resourceType, int amount)
    {
        float price = GetMarketPrice(resourceType);
        return Mathf.FloorToInt(amount * price);
    }

    public string GetEconomicSummary()
    {
        string summary = "=== ECONOMIC SUMMARY ===\n";
        summary += $"Tax Rate: {CurrentTaxRate:P1}\n";
        summary += $"Happiness Multiplier: {happinessMultiplier:F2}\n";
        summary += $"Seasonal Multiplier: {seasonalMultiplier:F2}\n\n";
        
        summary += "DAILY INCOME:\n";
        foreach (var income in dailyIncome)
        {
            if (income.Value > 0)
                summary += $"  {income.Key}: +{income.Value}\n";
        }
        
        summary += "\nDAILY EXPENSES:\n";
        foreach (var expense in dailyExpenses)
        {
            if (expense.Value > 0)
                summary += $"  {expense.Key}: -{expense.Value}\n";
        }
        
        summary += "\nNET FLOW:\n";
        foreach (var flow in netFlow)
        {
            string prefix = flow.Value >= 0 ? "+" : "";
            summary += $"  {flow.Key}: {prefix}{flow.Value}\n";
        }
        
        return summary;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // Debug methods
    #if UNITY_EDITOR
    [ContextMenu("Debug: Print Economic Summary")]
    private void DebugPrintEconomicSummary()
    {
        Debug.Log(GetEconomicSummary());
    }

    [ContextMenu("Debug: Process Daily Economics")]
    private void DebugProcessDailyEconomics()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentCity != null)
        {
            ProcessDailyEconomics(GameManager.Instance.CurrentCity);
        }
    }
    #endif
}