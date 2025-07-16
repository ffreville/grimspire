using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Building
{
    public enum BuildingType
    {
        // Residential
        House,
        Inn,
        Tavern,
        
        // Commercial
        Market,
        Shop,
        Bank,
        
        // Industrial
        Forge,
        Alchemist,
        Enchanter,
        
        // Administrative
        TownHall,
        Barracks,
        Prison,
        Court
    }

    public enum BuildingCategory
    {
        Residential,
        Commercial,
        Industrial,
        Administrative
    }

    [SerializeField] private BuildingType type;
    [SerializeField] private string name;
    [SerializeField] private BuildingCategory category;
    [SerializeField] private int level;
    [SerializeField] private bool isBuilt;
    [SerializeField] private Dictionary<Resource.ResourceType, int> buildCosts;
    [SerializeField] private Dictionary<Resource.ResourceType, int> maintenanceCosts;
    [SerializeField] private Dictionary<Resource.ResourceType, int> production;

    public BuildingType Type => type;
    public string Name => name;
    public BuildingCategory Category => category;
    public int Level => level;
    public bool IsBuilt => isBuilt;
    public Dictionary<Resource.ResourceType, int> BuildCosts => new Dictionary<Resource.ResourceType, int>(buildCosts);
    public Dictionary<Resource.ResourceType, int> MaintenanceCosts => new Dictionary<Resource.ResourceType, int>(maintenanceCosts);
    public Dictionary<Resource.ResourceType, int> Production => new Dictionary<Resource.ResourceType, int>(production);

    public Building(BuildingType type, string name, BuildingCategory category)
    {
        this.type = type;
        this.name = name;
        this.category = category;
        this.level = 1;
        this.isBuilt = false;
        this.buildCosts = new Dictionary<Resource.ResourceType, int>();
        this.maintenanceCosts = new Dictionary<Resource.ResourceType, int>();
        this.production = new Dictionary<Resource.ResourceType, int>();
        
        InitializeBuildingData();
    }

    private void InitializeBuildingData()
    {
        switch (type)
        {
            case BuildingType.House:
                buildCosts[Resource.ResourceType.Gold] = 100;
                buildCosts[Resource.ResourceType.Wood] = 50;
                production[Resource.ResourceType.Population] = 4;
                break;
                
            case BuildingType.Inn:
                buildCosts[Resource.ResourceType.Gold] = 200;
                buildCosts[Resource.ResourceType.Wood] = 75;
                production[Resource.ResourceType.Gold] = 10;
                production[Resource.ResourceType.Reputation] = 1;
                break;
                
            case BuildingType.Market:
                buildCosts[Resource.ResourceType.Gold] = 300;
                buildCosts[Resource.ResourceType.Wood] = 100;
                production[Resource.ResourceType.Gold] = 20;
                break;
                
            case BuildingType.Forge:
                buildCosts[Resource.ResourceType.Gold] = 400;
                buildCosts[Resource.ResourceType.Stone] = 100;
                buildCosts[Resource.ResourceType.Iron] = 50;
                maintenanceCosts[Resource.ResourceType.Gold] = 5;
                break;
                
            case BuildingType.TownHall:
                buildCosts[Resource.ResourceType.Gold] = 500;
                buildCosts[Resource.ResourceType.Stone] = 200;
                production[Resource.ResourceType.Reputation] = 5;
                break;
        }
    }

    public bool CanBuild(Dictionary<Resource.ResourceType, Resource> availableResources)
    {
        foreach (var cost in buildCosts)
        {
            if (!availableResources.ContainsKey(cost.Key) || 
                !availableResources[cost.Key].CanRemove(cost.Value))
            {
                return false;
            }
        }
        return true;
    }

    public void Build()
    {
        if (!isBuilt)
        {
            isBuilt = true;
        }
    }

    public void Upgrade()
    {
        if (isBuilt && level < 5)
        {
            level++;
        }
    }

    public Dictionary<Resource.ResourceType, int> GetUpgradeCosts()
    {
        var upgradeCosts = new Dictionary<Resource.ResourceType, int>();
        foreach (var cost in buildCosts)
        {
            upgradeCosts[cost.Key] = cost.Value * level;
        }
        return upgradeCosts;
    }

    public Dictionary<Resource.ResourceType, int> GetCurrentProduction()
    {
        if (!isBuilt) return new Dictionary<Resource.ResourceType, int>();
        
        var currentProduction = new Dictionary<Resource.ResourceType, int>();
        foreach (var prod in production)
        {
            currentProduction[prod.Key] = prod.Value * level;
        }
        return currentProduction;
    }

    public override string ToString()
    {
        return $"{name} (Level {level}) - {(isBuilt ? "Built" : "Not Built")}";
    }
}