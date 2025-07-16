using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Equipment
{
    public enum EquipmentType
    {
        Weapon,
        Armor,
        Helmet,
        Boots,
        Accessory,
        Shield
    }

    public enum EquipmentRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary,
        Artifact
    }

    [Header("Basic Information")]
    [SerializeField] private string name;
    [SerializeField] private EquipmentType type;
    [SerializeField] private EquipmentRarity rarity;
    [SerializeField] private string description;
    [SerializeField] private Sprite icon;
    
    [Header("Stats")]
    [SerializeField] private Dictionary<string, int> statBonuses;
    [SerializeField] private int durability;
    [SerializeField] private int maxDurability;
    [SerializeField] private int level;
    [SerializeField] private int sellValue;
    
    [Header("Requirements")]
    [SerializeField] private int requiredLevel;
    [SerializeField] private List<Adventurer.AdventurerClass> allowedClasses;
    
    public string Name => name;
    public EquipmentType Type => type;
    public EquipmentRarity Rarity => rarity;
    public string Description => description;
    public Sprite Icon => icon;
    public Dictionary<string, int> StatBonuses => new Dictionary<string, int>(statBonuses ?? new Dictionary<string, int>());
    public int Durability => durability;
    public int MaxDurability => maxDurability;
    public int Level => level;
    public int SellValue => sellValue;
    public int RequiredLevel => requiredLevel;
    public List<Adventurer.AdventurerClass> AllowedClasses => new List<Adventurer.AdventurerClass>(allowedClasses ?? new List<Adventurer.AdventurerClass>());
    public bool IsBroken => durability <= 0;
    public float DurabilityPercentage => maxDurability > 0 ? (float)durability / maxDurability : 0f;

    public Equipment()
    {
        statBonuses = new Dictionary<string, int>();
        allowedClasses = new List<Adventurer.AdventurerClass>();
        durability = 100;
        maxDurability = 100;
    }

    public Equipment(string equipmentName, EquipmentType equipmentType, EquipmentRarity equipmentRarity)
    {
        name = equipmentName;
        type = equipmentType;
        rarity = equipmentRarity;
        statBonuses = new Dictionary<string, int>();
        allowedClasses = new List<Adventurer.AdventurerClass>();
        durability = 100;
        maxDurability = 100;
        level = 1;
        
        GenerateBasicStats();
    }

    private void GenerateBasicStats()
    {
        statBonuses = new Dictionary<string, int>();
        
        switch (type)
        {
            case EquipmentType.Weapon:
                statBonuses["Attack"] = GetRarityBaseStat() * 2;
                statBonuses["Strength"] = GetRarityBaseStat();
                break;
                
            case EquipmentType.Armor:
                statBonuses["Defense"] = GetRarityBaseStat() * 2;
                statBonuses["Constitution"] = GetRarityBaseStat();
                break;
                
            case EquipmentType.Helmet:
                statBonuses["Defense"] = GetRarityBaseStat();
                statBonuses["Intelligence"] = GetRarityBaseStat();
                break;
                
            case EquipmentType.Boots:
                statBonuses["Defense"] = GetRarityBaseStat();
                statBonuses["Agility"] = GetRarityBaseStat();
                break;
                
            case EquipmentType.Accessory:
                statBonuses["Luck"] = GetRarityBaseStat();
                statBonuses["Charisma"] = GetRarityBaseStat();
                break;
        }
        
        sellValue = GetRarityBaseStat() * 10;
    }

    private int GetRarityBaseStat()
    {
        switch (rarity)
        {
            case EquipmentRarity.Common: return UnityEngine.Random.Range(1, 4);
            case EquipmentRarity.Uncommon: return UnityEngine.Random.Range(3, 7);
            case EquipmentRarity.Rare: return UnityEngine.Random.Range(6, 11);
            case EquipmentRarity.Epic: return UnityEngine.Random.Range(10, 16);
            case EquipmentRarity.Legendary: return UnityEngine.Random.Range(15, 21);
            case EquipmentRarity.Artifact: return UnityEngine.Random.Range(20, 31);
            default: return 1;
        }
    }

    public bool CanBeEquippedBy(Adventurer adventurer)
    {
        if (adventurer.Level < requiredLevel) return false;
        if (allowedClasses.Count > 0 && !allowedClasses.Contains(adventurer.Class)) return false;
        return true;
    }

    public int GetStatBonus(string statName)
    {
        return statBonuses.ContainsKey(statName) ? statBonuses[statName] : 0;
    }

    public void TakeDamage(int damage)
    {
        durability = Mathf.Max(0, durability - damage);
    }

    public void Repair(int amount)
    {
        durability = Mathf.Min(maxDurability, durability + amount);
    }

    public void FullRepair()
    {
        durability = maxDurability;
    }

    public Color GetRarityColor()
    {
        switch (rarity)
        {
            case EquipmentRarity.Common: return Color.white;
            case EquipmentRarity.Uncommon: return Color.green;
            case EquipmentRarity.Rare: return Color.blue;
            case EquipmentRarity.Epic: return new Color(0.5f, 0f, 1f); // Purple
            case EquipmentRarity.Legendary: return new Color(1f, 0.5f, 0f); // Orange
            case EquipmentRarity.Artifact: return Color.red;
            default: return Color.white;
        }
    }

    public string GetStatsDescription()
    {
        string desc = "";
        foreach (var stat in statBonuses)
        {
            if (!string.IsNullOrEmpty(desc)) desc += "\n";
            desc += $"+{stat.Value} {stat.Key}";
        }
        return desc;
    }

    public override string ToString()
    {
        return $"{name} ({rarity} {type})";
    }
}

[System.Serializable]
public class AdventurerTrait
{
    [SerializeField] private string name;
    [SerializeField] private string description;
    [SerializeField] private Dictionary<string, float> statModifiers;
    [SerializeField] private bool isPositive;

    public string Name => name;
    public string Description => description;
    public Dictionary<string, float> StatModifiers => new Dictionary<string, float>(statModifiers ?? new Dictionary<string, float>());
    public bool IsPositive => isPositive;

    public AdventurerTrait(string traitName, string traitDescription, bool positive = true)
    {
        name = traitName;
        description = traitDescription;
        isPositive = positive;
        statModifiers = new Dictionary<string, float>();
    }

    public void AddStatModifier(string statName, float modifier)
    {
        if (statModifiers == null)
            statModifiers = new Dictionary<string, float>();
        
        statModifiers[statName] = modifier;
    }

    public float GetStatModifier(string statName)
    {
        return statModifiers?.ContainsKey(statName) == true ? statModifiers[statName] : 0f;
    }
}

[System.Serializable]
public class AdventurerSkill
{
    [SerializeField] private string name;
    [SerializeField] private string description;
    [SerializeField] private int level;
    [SerializeField] private int maxLevel;
    [SerializeField] private int experience;
    [SerializeField] private int experienceToNext;
    [SerializeField] private Adventurer.AdventurerClass associatedClass;

    public string Name => name;
    public string Description => description;
    public int Level => level;
    public int MaxLevel => maxLevel;
    public int Experience => experience;
    public int ExperienceToNext => experienceToNext;
    public Adventurer.AdventurerClass AssociatedClass => associatedClass;
    public bool IsMaxLevel => level >= maxLevel;

    public AdventurerSkill(string skillName, string skillDescription, Adventurer.AdventurerClass skillClass, int maxSkillLevel = 10)
    {
        name = skillName;
        description = skillDescription;
        associatedClass = skillClass;
        level = 1;
        maxLevel = maxSkillLevel;
        experience = 0;
        experienceToNext = 100;
    }

    public bool AddExperience(int amount)
    {
        if (IsMaxLevel) return false;
        
        experience += amount;
        bool leveledUp = false;
        
        while (experience >= experienceToNext && !IsMaxLevel)
        {
            experience -= experienceToNext;
            level++;
            experienceToNext = level * 100;
            leveledUp = true;
        }
        
        return leveledUp;
    }

    public float GetEffectiveness()
    {
        return 1.0f + (level - 1) * 0.1f; // 10% increase per level
    }
}