using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class City
{
    [SerializeField] private string name;
    [SerializeField] private int cityLevel;
    [SerializeField] private Dictionary<Resource.ResourceType, Resource> resources;
    [SerializeField] private List<Building> buildings;
    [SerializeField] private List<Adventurer> adventurers;
    [SerializeField] private float dayTime;
    [SerializeField] private bool isNightPhase;

    public string Name => name;
    public int CityLevel => cityLevel;
    public Dictionary<Resource.ResourceType, Resource> Resources => resources;
    public List<Building> Buildings => buildings;
    public List<Adventurer> Adventurers => adventurers;
    public float DayTime => dayTime;
    public bool IsNightPhase => isNightPhase;

    public City(string cityName)
    {
        name = cityName;
        cityLevel = 1;
        dayTime = 0f;
        isNightPhase = false;
        
        InitializeResources();
        buildings = new List<Building>();
        adventurers = new List<Adventurer>();
    }

    private void InitializeResources()
    {
        resources = new Dictionary<Resource.ResourceType, Resource>
        {
            [Resource.ResourceType.Gold] = new Resource(Resource.ResourceType.Gold, 1000, 10000),
            [Resource.ResourceType.Population] = new Resource(Resource.ResourceType.Population, 50, 500),
            [Resource.ResourceType.Stone] = new Resource(Resource.ResourceType.Stone, 100, 1000),
            [Resource.ResourceType.Wood] = new Resource(Resource.ResourceType.Wood, 100, 1000),
            [Resource.ResourceType.Iron] = new Resource(Resource.ResourceType.Iron, 50, 500),
            [Resource.ResourceType.MagicCrystal] = new Resource(Resource.ResourceType.MagicCrystal, 10, 100),
            [Resource.ResourceType.Reputation] = new Resource(Resource.ResourceType.Reputation, 10, 100)
        };
    }

    public Resource GetResource(Resource.ResourceType type)
    {
        return resources.ContainsKey(type) ? resources[type] : null;
    }

    public bool CanAfford(Dictionary<Resource.ResourceType, int> costs)
    {
        foreach (var cost in costs)
        {
            if (!resources.ContainsKey(cost.Key) || 
                !resources[cost.Key].CanRemove(cost.Value))
            {
                return false;
            }
        }
        return true;
    }

    public bool SpendResources(Dictionary<Resource.ResourceType, int> costs)
    {
        if (!CanAfford(costs)) return false;
        
        foreach (var cost in costs)
        {
            resources[cost.Key].Remove(cost.Value);
        }
        return true;
    }

    public void AddResources(Dictionary<Resource.ResourceType, int> gains)
    {
        foreach (var gain in gains)
        {
            if (resources.ContainsKey(gain.Key))
            {
                resources[gain.Key].Add(gain.Value);
            }
        }
    }

    public bool BuildBuilding(Building.BuildingType buildingType)
    {
        string buildingName = buildingType.ToString();
        Building.BuildingCategory category = GetBuildingCategory(buildingType);
        
        Building newBuilding = new Building(buildingType, buildingName, category);
        
        if (newBuilding.CanBuild(resources))
        {
            if (SpendResources(newBuilding.BuildCosts))
            {
                newBuilding.Build();
                buildings.Add(newBuilding);
                return true;
            }
        }
        return false;
    }

    private Building.BuildingCategory GetBuildingCategory(Building.BuildingType type)
    {
        switch (type)
        {
            case Building.BuildingType.House:
            case Building.BuildingType.Inn:
            case Building.BuildingType.Tavern:
                return Building.BuildingCategory.Residential;
                
            case Building.BuildingType.Market:
            case Building.BuildingType.Shop:
            case Building.BuildingType.Bank:
                return Building.BuildingCategory.Commercial;
                
            case Building.BuildingType.Forge:
            case Building.BuildingType.Alchemist:
            case Building.BuildingType.Enchanter:
                return Building.BuildingCategory.Industrial;
                
            case Building.BuildingType.TownHall:
            case Building.BuildingType.Barracks:
            case Building.BuildingType.Prison:
            case Building.BuildingType.Court:
                return Building.BuildingCategory.Administrative;
                
            default:
                return Building.BuildingCategory.Residential;
        }
    }

    public void AddAdventurer(Adventurer adventurer)
    {
        adventurers.Add(adventurer);
    }

    public void RemoveAdventurer(Adventurer adventurer)
    {
        adventurers.Remove(adventurer);
    }

    public List<Adventurer> GetAvailableAdventurers()
    {
        return adventurers.FindAll(a => a.IsAvailable);
    }

    public List<Adventurer> GetInjuredAdventurers()
    {
        return adventurers.FindAll(a => a.IsInjured);
    }

    public void ProcessDailyProduction()
    {
        var totalProduction = new Dictionary<Resource.ResourceType, int>();
        var totalMaintenance = new Dictionary<Resource.ResourceType, int>();
        
        foreach (var building in buildings)
        {
            if (building.IsBuilt)
            {
                var production = building.GetCurrentProduction();
                foreach (var prod in production)
                {
                    if (!totalProduction.ContainsKey(prod.Key))
                        totalProduction[prod.Key] = 0;
                    totalProduction[prod.Key] += prod.Value;
                }
                
                foreach (var maint in building.MaintenanceCosts)
                {
                    if (!totalMaintenance.ContainsKey(maint.Key))
                        totalMaintenance[maint.Key] = 0;
                    totalMaintenance[maint.Key] += maint.Value;
                }
            }
        }
        
        // Apply maintenance costs first
        foreach (var maint in totalMaintenance)
        {
            if (resources.ContainsKey(maint.Key))
            {
                resources[maint.Key].Remove(maint.Value);
            }
        }
        
        // Apply production
        AddResources(totalProduction);
    }

    public void AdvanceTime(float deltaTime)
    {
        dayTime += deltaTime;
        
        // Day cycle: 0-12 = day, 12-24 = night
        if (dayTime >= 24f)
        {
            dayTime = 0f;
            ProcessDailyProduction();
        }
        
        isNightPhase = dayTime >= 12f;
    }

    public void LevelUp()
    {
        if (cityLevel < 10)
        {
            cityLevel++;
            
            // Increase resource capacities
            foreach (var resource in resources.Values)
            {
                resource.SetCapacity(resource.Capacity + 1000);
            }
        }
    }

    public int GetBuildingCount(Building.BuildingCategory category)
    {
        return buildings.FindAll(b => b.Category == category && b.IsBuilt).Count;
    }

    public int GetTotalPopulation()
    {
        return resources[Resource.ResourceType.Population].Amount;
    }

    public override string ToString()
    {
        return $"{name} - Level {cityLevel} - Population: {GetTotalPopulation()}";
    }
}