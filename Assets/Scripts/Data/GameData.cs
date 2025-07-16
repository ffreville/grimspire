using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    [Header("Meta Information")]
    public string gameVersion;
    public long lastSaveTime;
    public float totalPlayTime;
    
    [Header("City Information")]
    public string cityName;
    public int cityLevel;
    public float dayTime;
    public bool isNightPhase;
    
    [Header("Resources")]
    public SerializableResource[] resources;
    
    [Header("Buildings")]
    public SerializableBuildingData[] buildings;
    
    [Header("Adventurers")]
    public SerializableAdventurerData[] adventurers;
    
    [Header("Game Settings")]
    public bool autosaveEnabled = true;
    public float autosaveInterval = 300f; // 5 minutes
    public int difficulty = 1;

    public GameData()
    {
        gameVersion = "1.0.0";
        lastSaveTime = DateTime.Now.ToBinary();
        totalPlayTime = 0f;
        cityName = "Grimspire";
        cityLevel = 1;
        dayTime = 0f;
        isNightPhase = false;
        
        InitializeDefaultResources();
        buildings = new SerializableBuildingData[0];
        adventurers = new SerializableAdventurerData[0];
    }
    
    public void InitializeGameVersion()
    {
        gameVersion = Application.version;
    }

    private void InitializeDefaultResources()
    {
        resources = new SerializableResource[]
        {
            new SerializableResource(Resource.ResourceType.Gold, 1000, 10000),
            new SerializableResource(Resource.ResourceType.Population, 50, 500),
            new SerializableResource(Resource.ResourceType.Stone, 100, 1000),
            new SerializableResource(Resource.ResourceType.Wood, 100, 1000),
            new SerializableResource(Resource.ResourceType.Iron, 50, 500),
            new SerializableResource(Resource.ResourceType.MagicCrystal, 10, 100),
            new SerializableResource(Resource.ResourceType.Reputation, 10, 100)
        };
    }

    public DateTime GetLastSaveDateTime()
    {
        return DateTime.FromBinary(lastSaveTime);
    }

    public void UpdateSaveTime()
    {
        lastSaveTime = DateTime.Now.ToBinary();
    }
}

[System.Serializable]
public class SerializableResource
{
    public Resource.ResourceType type;
    public int amount;
    public int capacity;

    public SerializableResource(Resource.ResourceType resourceType, int resourceAmount, int resourceCapacity)
    {
        type = resourceType;
        amount = resourceAmount;
        capacity = resourceCapacity;
    }

    public Resource ToResource()
    {
        return new Resource(type, amount, capacity);
    }
}

[System.Serializable]
public class SerializableBuildingData
{
    public Building.BuildingType type;
    public string name;
    public Building.BuildingCategory category;
    public int level;
    public bool isBuilt;

    public SerializableBuildingData(Building building)
    {
        type = building.Type;
        name = building.Name;
        category = building.Category;
        level = building.Level;
        isBuilt = building.IsBuilt;
    }

    public Building ToBuilding()
    {
        Building building = new Building(type, name, category);
        if (isBuilt)
        {
            building.Build();
        }
        for (int i = 1; i < level; i++)
        {
            building.Upgrade();
        }
        return building;
    }
}

[System.Serializable]
public class SerializableAdventurerData
{
    public string name;
    public Adventurer.AdventurerClass adventurerClass;
    public Adventurer.AdventurerStatus status;
    public int level;
    public int experience;
    public int strength;
    public int intelligence;
    public int agility;
    public int charisma;
    public int luck;
    public int health;
    public int maxHealth;
    public string weapon;
    public string armor;
    public string accessory;
    public int missionsCompleted;
    public int missionsSuccessful;
    public string[] specialties;

    public SerializableAdventurerData(Adventurer adventurer)
    {
        name = adventurer.Name;
        adventurerClass = adventurer.Class;
        status = adventurer.Status;
        level = adventurer.Level;
        experience = adventurer.Experience;
        strength = adventurer.Strength;
        intelligence = adventurer.Intelligence;
        agility = adventurer.Agility;
        charisma = adventurer.Charisma;
        luck = adventurer.Luck;
        health = adventurer.Health;
        maxHealth = adventurer.MaxHealth;
        weapon = adventurer.Weapon?.Name ?? "";
        armor = adventurer.Armor?.Name ?? "";
        accessory = adventurer.Accessory?.Name ?? "";
        missionsCompleted = adventurer.MissionsCompleted;
        missionsSuccessful = adventurer.MissionsSuccessful;
        specialties = adventurer.Specialties.ToArray();
    }

    public Adventurer ToAdventurer()
    {
        Adventurer adventurer = new Adventurer(name, adventurerClass);
        
        // Note: This would require reflection or additional constructors
        // For now, we'll create a basic adventurer and set available properties
        // In a full implementation, you'd need to add setters or use reflection
        
        return adventurer;
    }
}