using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class EconomicUI : MonoBehaviour
{
    [Header("Economic Display")]
    [SerializeField] private Text taxRateText;
    [SerializeField] private Slider taxRateSlider;
    [SerializeField] private Text dailyIncomeText;
    [SerializeField] private Text dailyExpensesText;
    [SerializeField] private Text netFlowText;
    [SerializeField] private Text economicSummaryText;
    
    [Header("Market Display")]
    [SerializeField] private Transform marketPriceContainer;
    [SerializeField] private GameObject marketPriceItemPrefab;
    
    [Header("Controls")]
    [SerializeField] private Button processDailyEconomicsButton;
    [SerializeField] private Button toggleRealTimeButton;
    [SerializeField] private Text realTimeStatusText;
    
    [Header("Colors")]
    [SerializeField] private Color positiveColor = Color.green;
    [SerializeField] private Color negativeColor = Color.red;
    [SerializeField] private Color neutralColor = Color.white;
    
    private Dictionary<Resource.ResourceType, Text> marketPriceTexts;
    private bool isInitialized = false;
    
    private void Start()
    {
        InitializeEconomicUI();
    }

    private void OnEnable()
    {
        SubscribeToEvents();
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    private void InitializeEconomicUI()
    {
        SetupTaxRateControls();
        SetupButtons();
        SetupMarketDisplay();
        UpdateDisplay();
        isInitialized = true;
    }

    private void SetupTaxRateControls()
    {
        if (taxRateSlider != null)
        {
            taxRateSlider.minValue = 0.05f;
            taxRateSlider.maxValue = 0.5f;
            taxRateSlider.value = 0.1f;
            taxRateSlider.onValueChanged.AddListener(OnTaxRateChanged);
        }
    }

    private void SetupButtons()
    {
        if (processDailyEconomicsButton != null)
        {
            processDailyEconomicsButton.onClick.AddListener(ProcessDailyEconomics);
        }
        
        if (toggleRealTimeButton != null)
        {
            toggleRealTimeButton.onClick.AddListener(ToggleRealTimeEconomics);
        }
    }

    private void SetupMarketDisplay()
    {
        if (marketPriceContainer == null) return;
        
        marketPriceTexts = new Dictionary<Resource.ResourceType, Text>();
        
        // Create market price displays for tradeable resources
        Resource.ResourceType[] tradeableResources = {
            Resource.ResourceType.Stone,
            Resource.ResourceType.Wood,
            Resource.ResourceType.Iron,
            Resource.ResourceType.MagicCrystal
        };
        
        foreach (Resource.ResourceType resourceType in tradeableResources)
        {
            CreateMarketPriceDisplay(resourceType);
        }
    }

    private void CreateMarketPriceDisplay(Resource.ResourceType resourceType)
    {
        GameObject displayObject;
        
        if (marketPriceItemPrefab != null)
        {
            displayObject = Instantiate(marketPriceItemPrefab, marketPriceContainer);
        }
        else
        {
            displayObject = CreateDefaultMarketPriceDisplay(resourceType);
        }
        
        Text priceText = displayObject.GetComponentInChildren<Text>();
        if (priceText != null)
        {
            marketPriceTexts[resourceType] = priceText;
            UpdateMarketPriceDisplay(resourceType);
        }
    }

    private GameObject CreateDefaultMarketPriceDisplay(Resource.ResourceType resourceType)
    {
        GameObject displayObject = new GameObject($"MarketPrice_{resourceType}");
        displayObject.transform.SetParent(marketPriceContainer, false);
        
        RectTransform rectTransform = displayObject.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(200, 30);
        
        Text textComponent = displayObject.AddComponent<Text>();
        textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textComponent.fontSize = 12;
        textComponent.color = neutralColor;
        textComponent.text = $"{resourceType}: 0.00g";
        
        return displayObject;
    }

    private void UpdateDisplay()
    {
        if (EconomicSystem.Instance == null) return;
        
        UpdateTaxRateDisplay();
        UpdateIncomeExpenseDisplay();
        UpdateRealTimeStatus();
        UpdateMarketPrices();
        UpdateEconomicSummary();
    }

    private void UpdateTaxRateDisplay()
    {
        if (taxRateText != null && EconomicSystem.Instance != null)
        {
            float currentTaxRate = EconomicSystem.Instance.CurrentTaxRate;
            taxRateText.text = $"Taux d'imposition: {currentTaxRate:P1}";
        }
        
        if (taxRateSlider != null && EconomicSystem.Instance != null)
        {
            taxRateSlider.value = EconomicSystem.Instance.BaseTaxRate;
        }
    }

    private void UpdateIncomeExpenseDisplay()
    {
        if (EconomicSystem.Instance == null) return;
        
        var dailyIncome = EconomicSystem.Instance.DailyIncome;
        var dailyExpenses = EconomicSystem.Instance.DailyExpenses;
        var netFlow = EconomicSystem.Instance.NetFlow;
        
        // Update daily income
        if (dailyIncomeText != null)
        {
            string incomeText = "REVENUS QUOTIDIENS:\n";
            foreach (var income in dailyIncome)
            {
                if (income.Value > 0)
                {
                    incomeText += $"  {income.Key}: +{income.Value}\n";
                }
            }
            dailyIncomeText.text = incomeText;
            dailyIncomeText.color = positiveColor;
        }
        
        // Update daily expenses
        if (dailyExpensesText != null)
        {
            string expenseText = "DÉPENSES QUOTIDIENNES:\n";
            foreach (var expense in dailyExpenses)
            {
                if (expense.Value > 0)
                {
                    expenseText += $"  {expense.Key}: -{expense.Value}\n";
                }
            }
            dailyExpensesText.text = expenseText;
            dailyExpensesText.color = negativeColor;
        }
        
        // Update net flow
        if (netFlowText != null)
        {
            string flowText = "FLUX NET:\n";
            foreach (var flow in netFlow)
            {
                string prefix = flow.Value >= 0 ? "+" : "";
                Color flowColor = flow.Value >= 0 ? positiveColor : negativeColor;
                flowText += $"  {flow.Key}: {prefix}{flow.Value}\n";
            }
            netFlowText.text = flowText;
        }
    }

    private void UpdateRealTimeStatus()
    {
        if (realTimeStatusText != null && EconomicSystem.Instance != null)
        {
            bool isRealTime = EconomicSystem.Instance.enabled; // Assuming this controls real-time economics
            realTimeStatusText.text = isRealTime ? "Temps réel: ACTIVÉ" : "Temps réel: DÉSACTIVÉ";
            realTimeStatusText.color = isRealTime ? positiveColor : negativeColor;
        }
    }

    private void UpdateMarketPrices()
    {
        if (EconomicSystem.Instance == null) return;
        
        foreach (var kvp in marketPriceTexts)
        {
            UpdateMarketPriceDisplay(kvp.Key);
        }
    }

    private void UpdateMarketPriceDisplay(Resource.ResourceType resourceType)
    {
        if (!marketPriceTexts.ContainsKey(resourceType)) return;
        
        Text priceText = marketPriceTexts[resourceType];
        if (priceText != null && EconomicSystem.Instance != null)
        {
            float price = EconomicSystem.Instance.GetMarketPrice(resourceType);
            priceText.text = $"{resourceType}: {price:F2}g";
        }
    }

    private void UpdateEconomicSummary()
    {
        if (economicSummaryText != null && EconomicSystem.Instance != null)
        {
            economicSummaryText.text = EconomicSystem.Instance.GetEconomicSummary();
        }
    }

    private void SubscribeToEvents()
    {
        if (EconomicSystem.Instance != null)
        {
            EconomicSystem.OnDailyIncomeCalculated += OnDailyIncomeCalculated;
            EconomicSystem.OnDailyExpensesCalculated += OnDailyExpensesCalculated;
            EconomicSystem.OnNetFlowCalculated += OnNetFlowCalculated;
            EconomicSystem.OnTaxRateChanged += OnTaxRateChanged;
            EconomicSystem.OnMarketPriceChanged += OnMarketPriceChanged;
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (EconomicSystem.Instance != null)
        {
            EconomicSystem.OnDailyIncomeCalculated -= OnDailyIncomeCalculated;
            EconomicSystem.OnDailyExpensesCalculated -= OnDailyExpensesCalculated;
            EconomicSystem.OnNetFlowCalculated -= OnNetFlowCalculated;
            EconomicSystem.OnTaxRateChanged -= OnTaxRateChanged;
            EconomicSystem.OnMarketPriceChanged -= OnMarketPriceChanged;
        }
    }

    private void OnDailyIncomeCalculated(Dictionary<Resource.ResourceType, int> income)
    {
        UpdateIncomeExpenseDisplay();
    }

    private void OnDailyExpensesCalculated(Dictionary<Resource.ResourceType, int> expenses)
    {
        UpdateIncomeExpenseDisplay();
    }

    private void OnNetFlowCalculated(Dictionary<Resource.ResourceType, int> netFlow)
    {
        UpdateIncomeExpenseDisplay();
    }

    private void OnTaxRateChanged(float newRate)
    {
        if (EconomicSystem.Instance != null)
        {
            EconomicSystem.Instance.SetTaxRate(newRate);
        }
        UpdateTaxRateDisplay();
    }

    private void OnMarketPriceChanged(Resource.ResourceType resourceType, float newPrice)
    {
        UpdateMarketPriceDisplay(resourceType);
    }

    private void ProcessDailyEconomics()
    {
        if (EconomicSystem.Instance != null && GameManager.Instance != null)
        {
            City currentCity = GameManager.Instance.CurrentCity;
            if (currentCity != null)
            {
                EconomicSystem.Instance.ProcessDailyEconomics(currentCity);
            }
        }
    }

    private void ToggleRealTimeEconomics()
    {
        if (EconomicSystem.Instance != null)
        {
            EconomicSystem.Instance.enabled = !EconomicSystem.Instance.enabled;
            UpdateRealTimeStatus();
        }
    }

    public void RefreshDisplay()
    {
        UpdateDisplay();
    }

    private void OnDestroy()
    {
        if (taxRateSlider != null)
        {
            taxRateSlider.onValueChanged.RemoveListener(OnTaxRateChanged);
        }
        
        if (processDailyEconomicsButton != null)
        {
            processDailyEconomicsButton.onClick.RemoveListener(ProcessDailyEconomics);
        }
        
        if (toggleRealTimeButton != null)
        {
            toggleRealTimeButton.onClick.RemoveListener(ToggleRealTimeEconomics);
        }
    }
}