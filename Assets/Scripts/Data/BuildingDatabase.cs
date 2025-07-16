using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingDatabase", menuName = "Grimspire/Building Database")]
public class BuildingDatabase : ScriptableObject
{
    [SerializeField] private BuildingData[] buildings;
    
    private Dictionary<Building.BuildingType, BuildingData> buildingLookup;
    
    public BuildingData[] AllBuildings => buildings;
    
    private void OnEnable()
    {
        InitializeLookup();
    }
    
    private void InitializeLookup()
    {
        buildingLookup = new Dictionary<Building.BuildingType, BuildingData>();
        
        if (buildings != null)
        {
            foreach (var building in buildings)
            {
                if (building != null)
                {
                    buildingLookup[building.buildingType] = building;
                }
            }
        }
    }
    
    public BuildingData GetBuildingData(Building.BuildingType buildingType)
    {
        if (buildingLookup == null)
        {
            InitializeLookup();
        }
        
        return buildingLookup.ContainsKey(buildingType) ? buildingLookup[buildingType] : null;
    }
    
    public BuildingData[] GetBuildingsByCategory(Building.BuildingCategory category)
    {
        if (buildings == null) return new BuildingData[0];
        
        var results = new List<BuildingData>();
        foreach (var building in buildings)
        {
            if (building != null && building.category == category)
            {
                results.Add(building);
            }
        }
        
        return results.ToArray();
    }
    
    public bool IsBuildingUnlocked(Building.BuildingType buildingType, int cityLevel, Dictionary<Resource.ResourceType, int> currentResources)
    {
        BuildingData data = GetBuildingData(buildingType);
        if (data == null) return false;
        
        // Check city level requirement
        if (cityLevel < data.requiredCityLevel) return false;
        
        // Check prerequisite buildings
        if (data.prerequisiteBuildings != null && data.prerequisiteBuildings.Length > 0)
        {
            // This would need integration with city building list
            // For now, assume prerequisites are met
        }
        
        return true;
    }
    
    public BuildingData[] GetAvailableBuildings(int cityLevel, Dictionary<Resource.ResourceType, int> currentResources)
    {
        if (buildings == null) return new BuildingData[0];
        
        var available = new List<BuildingData>();
        foreach (var building in buildings)
        {
            if (building != null && IsBuildingUnlocked(building.buildingType, cityLevel, currentResources))
            {
                available.Add(building);
            }
        }
        
        return available.ToArray();
    }
}

[System.Serializable]
public class BuildingData
{
    [Header("Basic Information")]
    public Building.BuildingType buildingType;
    public string displayName;
    public Building.BuildingCategory category;
    [TextArea(3, 5)]
    public string description;
    public Sprite icon;
    
    [Header("Requirements")]
    public int requiredCityLevel = 1;
    public Building.BuildingType[] prerequisiteBuildings;
    
    [Header("Construction Costs")]
    public ResourceCost[] buildCosts;
    public float constructionTime = 1.0f; // Game hours
    
    [Header("Maintenance")]
    public ResourceCost[] maintenanceCosts;
    public float maintenanceInterval = 24.0f; // Game hours
    
    [Header("Production")]
    public ResourceProduction[] resourceProduction;
    public float productionInterval = 24.0f; // Game hours
    
    [Header("Building Effects")]
    public BuildingEffect[] buildingEffects;
    
    [Header("Upgrade Information")]
    public bool canUpgrade = true;
    public int maxLevel = 5;
    public float upgradeCostMultiplier = 1.5f;
    public float upgradeEffectMultiplier = 1.2f;
    
    public Dictionary<Resource.ResourceType, int> GetBuildCosts(int level = 1)
    {
        var costs = new Dictionary<Resource.ResourceType, int>();
        
        if (buildCosts != null)
        {
            foreach (var cost in buildCosts)
            {
                float multiplier = level > 1 ? Mathf.Pow(upgradeCostMultiplier, level - 1) : 1.0f;
                costs[cost.resourceType] = Mathf.RoundToInt(cost.amount * multiplier);
            }
        }
        
        return costs;
    }
    
    public Dictionary<Resource.ResourceType, int> GetMaintenanceCosts(int level = 1)
    {
        var costs = new Dictionary<Resource.ResourceType, int>();
        
        if (maintenanceCosts != null)
        {
            foreach (var cost in maintenanceCosts)
            {
                float multiplier = level > 1 ? Mathf.Pow(upgradeEffectMultiplier, level - 1) : 1.0f;
                costs[cost.resourceType] = Mathf.RoundToInt(cost.amount * multiplier);
            }
        }
        
        return costs;
    }
    
    public Dictionary<Resource.ResourceType, int> GetProduction(int level = 1)
    {
        var production = new Dictionary<Resource.ResourceType, int>();
        
        if (resourceProduction != null)
        {
            foreach (var prod in resourceProduction)
            {
                float multiplier = level > 1 ? Mathf.Pow(upgradeEffectMultiplier, level - 1) : 1.0f;
                production[prod.resourceType] = Mathf.RoundToInt(prod.amount * multiplier);
            }
        }
        
        return production;
    }
    
    public BuildingEffect[] GetEffects(int level = 1)
    {
        if (buildingEffects == null) return new BuildingEffect[0];
        
        var scaledEffects = new BuildingEffect[buildingEffects.Length];
        for (int i = 0; i < buildingEffects.Length; i++)
        {
            scaledEffects[i] = buildingEffects[i].GetScaledEffect(level, upgradeEffectMultiplier);
        }
        
        return scaledEffects;
    }
}

[System.Serializable]
public class ResourceCost
{
    public Resource.ResourceType resourceType;
    public int amount;
    
    public ResourceCost(Resource.ResourceType type, int amt)
    {
        resourceType = type;
        amount = amt;
    }
}

[System.Serializable]
public class ResourceProduction
{
    public Resource.ResourceType resourceType;
    public int amount;
    
    public ResourceProduction(Resource.ResourceType type, int amt)
    {
        resourceType = type;
        amount = amt;
    }
}

[System.Serializable]
public class BuildingEffect
{
    public enum EffectType
    {
        ResourceCapacityIncrease,
        PopulationCapacityIncrease,
        ResourceProductionBonus,
        TaxIncomeBonus,
        AdventurerCapacityIncrease,
        AdventurerStatsBonus,
        ConstructionSpeedBonus,
        MaintenanceCostReduction,
        ReputationBonus,
        UnlockFeature
    }
    
    public EffectType effectType;
    public Resource.ResourceType targetResource;
    public float effectValue;
    public string description;
    
    public BuildingEffect GetScaledEffect(int level, float multiplier)
    {
        return new BuildingEffect
        {
            effectType = this.effectType,
            targetResource = this.targetResource,
            effectValue = this.effectValue * Mathf.Pow(multiplier, level - 1),
            description = this.description
        };
    }
}

// Static building database for easy access
public static class BuildingDB
{
    private static BuildingDatabase database;
    
    public static BuildingDatabase Database
    {
        get
        {
            if (database == null)
            {
                database = Resources.Load<BuildingDatabase>("BuildingDatabase");
                if (database == null)
                {
                    Debug.LogError("BuildingDatabase not found in Resources folder!");
                }
            }
            return database;
        }
    }
    
    public static BuildingData GetBuilding(Building.BuildingType buildingType)
    {
        return Database?.GetBuildingData(buildingType);
    }
    
    public static BuildingData[] GetBuildingsByCategory(Building.BuildingCategory category)
    {
        return Database?.GetBuildingsByCategory(category) ?? new BuildingData[0];
    }
    
    public static bool CanAffordBuilding(Building.BuildingType buildingType, int level = 1)
    {
        BuildingData data = GetBuilding(buildingType);
        if (data == null || ResourceManager.Instance == null) return false;
        
        var costs = data.GetBuildCosts(level);
        return ResourceManager.Instance.CanAfford(costs);
    }
}