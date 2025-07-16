using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BuildingEffectsSystem : MonoBehaviour
{
    public static BuildingEffectsSystem Instance { get; private set; }
    
    [Header("Effect Settings")]
    [SerializeField] private bool applyEffectsInRealTime = true;
    [SerializeField] private float effectUpdateInterval = 1.0f;
    [SerializeField] private bool debugMode = false;
    
    private Dictionary<Building.BuildingType, List<BuildingEffect>> activeBuildingEffects;
    private Dictionary<Resource.ResourceType, int> capacityBonuses;
    private Dictionary<Resource.ResourceType, float> productionBonuses;
    private float lastUpdateTime;
    
    // Cached effect values
    private int totalPopulationCapacityBonus;
    private int totalAdventurerCapacityBonus;
    private float totalTaxIncomeBonus;
    private float totalConstructionSpeedBonus;
    private float totalMaintenanceCostReduction;
    private int totalReputationBonus;
    
    public Dictionary<Resource.ResourceType, int> CapacityBonuses => new Dictionary<Resource.ResourceType, int>(capacityBonuses);
    public Dictionary<Resource.ResourceType, float> ProductionBonuses => new Dictionary<Resource.ResourceType, float>(productionBonuses);
    public int PopulationCapacityBonus => totalPopulationCapacityBonus;
    public int AdventurerCapacityBonus => totalAdventurerCapacityBonus;
    public float TaxIncomeBonus => totalTaxIncomeBonus;
    public float ConstructionSpeedBonus => totalConstructionSpeedBonus;
    public float MaintenanceCostReduction => totalMaintenanceCostReduction;
    public int ReputationBonus => totalReputationBonus;
    
    // Events
    public static event Action OnEffectsRecalculated;
    public static event Action<Building.BuildingType, BuildingEffect[]> OnBuildingEffectsApplied;
    public static event Action<Building.BuildingType> OnBuildingEffectsRemoved;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeEffectsSystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeEffectsSystem()
    {
        activeBuildingEffects = new Dictionary<Building.BuildingType, List<BuildingEffect>>();
        capacityBonuses = new Dictionary<Resource.ResourceType, int>();
        productionBonuses = new Dictionary<Resource.ResourceType, float>();
        
        ResetEffectValues();
        
        if (debugMode)
        {
            Debug.Log("BuildingEffectsSystem initialized");
        }
    }

    private void Start()
    {
        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
        
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Update()
    {
        if (applyEffectsInRealTime && Time.time - lastUpdateTime >= effectUpdateInterval)
        {
            UpdateEffects();
            lastUpdateTime = Time.time;
        }
    }

    private void SubscribeToEvents()
    {
        if (ConstructionSystem.Instance != null)
        {
            ConstructionSystem.OnConstructionCompleted += OnBuildingConstructed;
            ConstructionSystem.OnBuildingUpgraded += OnBuildingUpgraded;
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (ConstructionSystem.Instance != null)
        {
            ConstructionSystem.OnConstructionCompleted -= OnBuildingConstructed;
            ConstructionSystem.OnBuildingUpgraded -= OnBuildingUpgraded;
        }
    }

    public void RecalculateAllEffects()
    {
        ResetEffectValues();
        activeBuildingEffects.Clear();
        
        if (ConstructionSystem.Instance != null)
        {
            foreach (var building in ConstructionSystem.Instance.ConstructedBuildings)
            {
                if (building.IsBuilt)
                {
                    ApplyBuildingEffects(building);
                }
            }
        }
        
        ApplyEffectsToSystems();
        OnEffectsRecalculated?.Invoke();
        
        if (debugMode)
        {
            Debug.Log("All building effects recalculated");
        }
    }

    private void UpdateEffects()
    {
        RecalculateAllEffects();
    }

    private void OnBuildingConstructed(Building.BuildingType buildingType)
    {
        if (ConstructionSystem.Instance != null)
        {
            Building building = ConstructionSystem.Instance.GetBuilding(buildingType);
            if (building != null)
            {
                ApplyBuildingEffects(building);
                ApplyEffectsToSystems();
                OnEffectsRecalculated?.Invoke();
            }
        }
    }

    private void OnBuildingUpgraded(Building building)
    {
        if (building != null)
        {
            // Remove old effects and apply new ones
            RemoveBuildingEffects(building.Type);
            ApplyBuildingEffects(building);
            ApplyEffectsToSystems();
            OnEffectsRecalculated?.Invoke();
        }
    }

    private void ApplyBuildingEffects(Building building)
    {
        BuildingData data = BuildingDB.GetBuilding(building.Type);
        if (data == null) return;
        
        BuildingEffect[] effects = data.GetEffects(building.Level);
        if (effects == null || effects.Length == 0) return;
        
        activeBuildingEffects[building.Type] = new List<BuildingEffect>(effects);
        
        foreach (var effect in effects)
        {
            ApplyIndividualEffect(effect);
        }
        
        OnBuildingEffectsApplied?.Invoke(building.Type, effects);
        
        if (debugMode)
        {
            Debug.Log($"Applied {effects.Length} effects from {building.Type} (Level {building.Level})");
        }
    }

    private void RemoveBuildingEffects(Building.BuildingType buildingType)
    {
        if (activeBuildingEffects.ContainsKey(buildingType))
        {
            activeBuildingEffects.Remove(buildingType);
            OnBuildingEffectsRemoved?.Invoke(buildingType);
            
            if (debugMode)
            {
                Debug.Log($"Removed effects from {buildingType}");
            }
        }
    }

    private void ApplyIndividualEffect(BuildingEffect effect)
    {
        switch (effect.effectType)
        {
            case BuildingEffect.EffectType.ResourceCapacityIncrease:
                if (!capacityBonuses.ContainsKey(effect.targetResource))
                    capacityBonuses[effect.targetResource] = 0;
                capacityBonuses[effect.targetResource] += Mathf.RoundToInt(effect.effectValue);
                break;
                
            case BuildingEffect.EffectType.PopulationCapacityIncrease:
                totalPopulationCapacityBonus += Mathf.RoundToInt(effect.effectValue);
                break;
                
            case BuildingEffect.EffectType.ResourceProductionBonus:
                if (!productionBonuses.ContainsKey(effect.targetResource))
                    productionBonuses[effect.targetResource] = 0f;
                productionBonuses[effect.targetResource] += effect.effectValue;
                break;
                
            case BuildingEffect.EffectType.TaxIncomeBonus:
                totalTaxIncomeBonus += effect.effectValue;
                break;
                
            case BuildingEffect.EffectType.AdventurerCapacityIncrease:
                totalAdventurerCapacityBonus += Mathf.RoundToInt(effect.effectValue);
                break;
                
            case BuildingEffect.EffectType.ConstructionSpeedBonus:
                totalConstructionSpeedBonus += effect.effectValue;
                break;
                
            case BuildingEffect.EffectType.MaintenanceCostReduction:
                totalMaintenanceCostReduction += effect.effectValue;
                break;
                
            case BuildingEffect.EffectType.ReputationBonus:
                totalReputationBonus += Mathf.RoundToInt(effect.effectValue);
                break;
        }
    }

    private void ApplyEffectsToSystems()
    {
        ApplyResourceCapacityEffects();
        ApplyProductionEffects();
        ApplyConstructionEffects();
        ApplyReputationEffects();
    }

    private void ApplyResourceCapacityEffects()
    {
        if (ResourceManager.Instance == null) return;
        
        foreach (var bonus in capacityBonuses)
        {
            var resource = ResourceManager.Instance.GetResource(bonus.Key);
            if (resource != null)
            {
                // Note: This would require modifying ResourceManager to support dynamic capacity bonuses
                // For now, we'll store the bonuses for external systems to use
            }
        }
        
        // Apply population capacity bonus
        if (totalPopulationCapacityBonus > 0)
        {
            var populationResource = ResourceManager.Instance.GetResource(Resource.ResourceType.Population);
            if (populationResource != null)
            {
                // Add bonus capacity
                populationResource.SetCapacity(populationResource.Capacity + totalPopulationCapacityBonus);
            }
        }
    }

    private void ApplyProductionEffects()
    {
        // Production bonuses are applied through the EconomicSystem
        // The EconomicSystem should query this system for production bonuses
    }

    private void ApplyConstructionEffects()
    {
        if (ConstructionSystem.Instance != null && totalConstructionSpeedBonus > 0)
        {
            float speedMultiplier = 1.0f + totalConstructionSpeedBonus;
            ConstructionSystem.Instance.SetConstructionSpeed(speedMultiplier);
        }
    }

    private void ApplyReputationEffects()
    {
        if (ResourceManager.Instance != null && totalReputationBonus > 0)
        {
            ResourceManager.Instance.AddResource(Resource.ResourceType.Reputation, totalReputationBonus);
        }
    }

    private void ResetEffectValues()
    {
        capacityBonuses.Clear();
        productionBonuses.Clear();
        
        totalPopulationCapacityBonus = 0;
        totalAdventurerCapacityBonus = 0;
        totalTaxIncomeBonus = 0f;
        totalConstructionSpeedBonus = 0f;
        totalMaintenanceCostReduction = 0f;
        totalReputationBonus = 0;
    }

    public int GetResourceCapacityBonus(Resource.ResourceType resourceType)
    {
        return capacityBonuses.ContainsKey(resourceType) ? capacityBonuses[resourceType] : 0;
    }

    public float GetResourceProductionBonus(Resource.ResourceType resourceType)
    {
        return productionBonuses.ContainsKey(resourceType) ? productionBonuses[resourceType] : 0f;
    }

    public BuildingEffect[] GetBuildingEffects(Building.BuildingType buildingType)
    {
        if (activeBuildingEffects.ContainsKey(buildingType))
        {
            return activeBuildingEffects[buildingType].ToArray();
        }
        return new BuildingEffect[0];
    }

    public int GetTotalEffectValue(BuildingEffect.EffectType effectType, Resource.ResourceType targetResource = Resource.ResourceType.Gold)
    {
        int total = 0;
        
        foreach (var buildingEffects in activeBuildingEffects.Values)
        {
            foreach (var effect in buildingEffects)
            {
                if (effect.effectType == effectType && 
                    (targetResource == Resource.ResourceType.Gold || effect.targetResource == targetResource))
                {
                    total += Mathf.RoundToInt(effect.effectValue);
                }
            }
        }
        
        return total;
    }

    public string GetEffectsSummary()
    {
        string summary = "=== BUILDING EFFECTS SUMMARY ===\n";
        
        summary += "RESOURCE CAPACITY BONUSES:\n";
        foreach (var bonus in capacityBonuses)
        {
            summary += $"  {bonus.Key}: +{bonus.Value}\n";
        }
        
        summary += "\nPRODUCTION BONUSES:\n";
        foreach (var bonus in productionBonuses)
        {
            summary += $"  {bonus.Key}: +{bonus.Value:F1}%\n";
        }
        
        summary += "\nOTHER EFFECTS:\n";
        summary += $"  Population Capacity: +{totalPopulationCapacityBonus}\n";
        summary += $"  Adventurer Capacity: +{totalAdventurerCapacityBonus}\n";
        summary += $"  Tax Income Bonus: +{totalTaxIncomeBonus:F1}%\n";
        summary += $"  Construction Speed: +{totalConstructionSpeedBonus:F1}%\n";
        summary += $"  Maintenance Reduction: -{totalMaintenanceCostReduction:F1}%\n";
        summary += $"  Reputation Bonus: +{totalReputationBonus}\n";
        
        return summary;
    }

    // Debug methods
    #if UNITY_EDITOR
    [ContextMenu("Debug: Recalculate All Effects")]
    private void DebugRecalculateEffects()
    {
        RecalculateAllEffects();
    }

    [ContextMenu("Debug: Print Effects Summary")]
    private void DebugPrintEffectsSummary()
    {
        Debug.Log(GetEffectsSummary());
    }

    [ContextMenu("Debug: List Active Building Effects")]
    private void DebugListActiveEffects()
    {
        Debug.Log($"Active Building Effects ({activeBuildingEffects.Count} buildings):");
        foreach (var kvp in activeBuildingEffects)
        {
            Debug.Log($"  {kvp.Key}: {kvp.Value.Count} effects");
            foreach (var effect in kvp.Value)
            {
                Debug.Log($"    - {effect.effectType}: {effect.effectValue} ({effect.description})");
            }
        }
    }
    #endif
}