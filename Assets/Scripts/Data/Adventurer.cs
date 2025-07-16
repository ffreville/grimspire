using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Adventurer
{
    public enum AdventurerClass
    {
        Warrior,
        Mage,
        Rogue,
        Cleric,
        Ranger
    }

    public enum AdventurerStatus
    {
        Available,
        OnMission,
        Injured,
        Dead,
        Resting,
        Training
    }

    public enum AdventurerRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

    [Header("Basic Information")]
    [SerializeField] private string name;
    [SerializeField] private AdventurerClass adventurerClass;
    [SerializeField] private AdventurerStatus status;
    [SerializeField] private AdventurerRarity rarity;
    [SerializeField] private int recruitmentCost;
    [SerializeField] private string biography;
    
    [Header("Level and Experience")]
    [SerializeField] private int level;
    [SerializeField] private int experience;
    [SerializeField] private int experienceToNext;
    [SerializeField] private int skillPoints;
    
    [Header("Core Stats")]
    [SerializeField] private int strength;
    [SerializeField] private int intelligence;
    [SerializeField] private int agility;
    [SerializeField] private int charisma;
    [SerializeField] private int luck;
    [SerializeField] private int constitution;
    
    [Header("Health and Mana")]
    [SerializeField] private int health;
    [SerializeField] private int maxHealth;
    [SerializeField] private int mana;
    [SerializeField] private int maxMana;
    [SerializeField] private float healthRegenRate;
    [SerializeField] private float manaRegenRate;
    
    [Header("Equipment")]
    [SerializeField] private Equipment weapon;
    [SerializeField] private Equipment armor;
    [SerializeField] private Equipment accessory;
    [SerializeField] private Equipment boots;
    [SerializeField] private Equipment helmet;
    
    [Header("Mission History")]
    [SerializeField] private int missionsCompleted;
    [SerializeField] private int missionsSuccessful;
    [SerializeField] private int missionsFailed;
    [SerializeField] private int totalDamageDealt;
    [SerializeField] private int totalDamageTaken;
    [SerializeField] private int enemiesKilled;
    
    [Header("Skills and Traits")]
    [SerializeField] private List<string> specialties;
    [SerializeField] private List<AdventurerTrait> traits;
    [SerializeField] private List<AdventurerSkill> skills;
    [SerializeField] private int currentPartyId = -1;
    
    [Header("Economic")]
    [SerializeField] private int dailySalary;
    [SerializeField] private float loyaltyLevel;
    [SerializeField] private int daysInService;
    [SerializeField] private DateTime recruitmentDate;

    // Properties
    public string Name => name;
    public AdventurerClass Class => adventurerClass;
    public AdventurerStatus Status => status;
    public AdventurerRarity Rarity => rarity;
    public string Biography => biography;
    public int Level => level;
    public int Experience => experience;
    public int ExperienceToNext => experienceToNext;
    public int SkillPoints => skillPoints;
    
    // Core Stats
    public int Strength => strength;
    public int Intelligence => intelligence;
    public int Agility => agility;
    public int Charisma => charisma;
    public int Luck => luck;
    public int Constitution => constitution;
    
    // Health and Mana
    public int Health => health;
    public int MaxHealth => maxHealth;
    public int Mana => mana;
    public int MaxMana => maxMana;
    public float HealthRegenRate => healthRegenRate;
    public float ManaRegenRate => manaRegenRate;
    
    // Equipment
    public Equipment Weapon => weapon;
    public Equipment Armor => armor;
    public Equipment Accessory => accessory;
    public Equipment Boots => boots;
    public Equipment Helmet => helmet;
    
    // Mission Stats
    public int MissionsCompleted => missionsCompleted;
    public int MissionsSuccessful => missionsSuccessful;
    public int MissionsFailed => missionsFailed;
    public int TotalDamageDealt => totalDamageDealt;
    public int TotalDamageTaken => totalDamageTaken;
    public int EnemiesKilled => enemiesKilled;
    
    // Skills and Traits
    public List<string> Specialties => new List<string>(specialties ?? new List<string>());
    public List<AdventurerTrait> Traits => new List<AdventurerTrait>(traits ?? new List<AdventurerTrait>());
    public List<AdventurerSkill> Skills => new List<AdventurerSkill>(skills ?? new List<AdventurerSkill>());
    public int CurrentPartyId => currentPartyId;
    
    // Economic
    public int DailySalary => dailySalary;
    public float LoyaltyLevel => loyaltyLevel;
    public int DaysInService => daysInService;
    public DateTime RecruitmentDate => recruitmentDate;
    public int RecruitmentCost => recruitmentCost;
    
    // Calculated Properties
    public float SuccessRate => missionsCompleted > 0 ? (float)missionsSuccessful / missionsCompleted : 0f;
    public float FailureRate => missionsCompleted > 0 ? (float)missionsFailed / missionsCompleted : 0f;
    public bool IsAlive => status != AdventurerStatus.Dead;
    public bool IsAvailable => status == AdventurerStatus.Available;
    public bool IsInjured => status == AdventurerStatus.Injured;
    public bool IsOnMission => status == AdventurerStatus.OnMission;
    public bool IsInParty => currentPartyId >= 0;
    public float HealthPercentage => maxHealth > 0 ? (float)health / maxHealth : 0f;
    public float ManaPercentage => maxMana > 0 ? (float)mana / maxMana : 0f;

    public Adventurer(string adventurerName, AdventurerClass adventurerClass, AdventurerRarity adventurerRarity = AdventurerRarity.Common)
    {
        this.name = adventurerName;
        this.adventurerClass = adventurerClass;
        this.rarity = adventurerRarity;
        this.status = AdventurerStatus.Available;
        this.level = 1;
        this.experience = 0;
        this.experienceToNext = 100;
        this.skillPoints = 0;
        this.currentPartyId = -1;
        
        // Initialize collections
        this.specialties = new List<string>();
        this.traits = new List<AdventurerTrait>();
        this.skills = new List<AdventurerSkill>();
        
        // Initialize mission stats
        this.missionsCompleted = 0;
        this.missionsSuccessful = 0;
        this.missionsFailed = 0;
        this.totalDamageDealt = 0;
        this.totalDamageTaken = 0;
        this.enemiesKilled = 0;
        
        // Initialize economic data
        this.loyaltyLevel = 50f;
        this.daysInService = 0;
        this.recruitmentDate = DateTime.Now;
        
        GenerateBaseStats();
        GenerateStartingEquipment();
        GenerateStartingTraits();
        GenerateClassSkills();
        RecalculateStats();
        
        this.health = this.maxHealth;
        this.mana = this.maxMana;
        this.dailySalary = CalculateBaseSalary();
        this.recruitmentCost = CalculateRecruitmentCost();
        this.biography = GenerateBiography();
    }

    private void GenerateBaseStats()
    {
        // Apply rarity multiplier
        float rarityBonus = GetRarityStatBonus();
        
        switch (adventurerClass)
        {
            case AdventurerClass.Warrior:
                strength = Mathf.RoundToInt(UnityEngine.Random.Range(15, 20) * rarityBonus);
                intelligence = Mathf.RoundToInt(UnityEngine.Random.Range(8, 12) * rarityBonus);
                agility = Mathf.RoundToInt(UnityEngine.Random.Range(10, 15) * rarityBonus);
                charisma = Mathf.RoundToInt(UnityEngine.Random.Range(8, 12) * rarityBonus);
                luck = Mathf.RoundToInt(UnityEngine.Random.Range(8, 12) * rarityBonus);
                constitution = Mathf.RoundToInt(UnityEngine.Random.Range(12, 18) * rarityBonus);
                maxHealth = Mathf.RoundToInt(100 * rarityBonus);
                maxMana = Mathf.RoundToInt(30 * rarityBonus);
                break;
                
            case AdventurerClass.Mage:
                strength = Mathf.RoundToInt(UnityEngine.Random.Range(6, 10) * rarityBonus);
                intelligence = Mathf.RoundToInt(UnityEngine.Random.Range(16, 20) * rarityBonus);
                agility = Mathf.RoundToInt(UnityEngine.Random.Range(8, 12) * rarityBonus);
                charisma = Mathf.RoundToInt(UnityEngine.Random.Range(10, 15) * rarityBonus);
                luck = Mathf.RoundToInt(UnityEngine.Random.Range(10, 15) * rarityBonus);
                constitution = Mathf.RoundToInt(UnityEngine.Random.Range(6, 10) * rarityBonus);
                maxHealth = Mathf.RoundToInt(60 * rarityBonus);
                maxMana = Mathf.RoundToInt(120 * rarityBonus);
                break;
                
            case AdventurerClass.Rogue:
                strength = Mathf.RoundToInt(UnityEngine.Random.Range(10, 15) * rarityBonus);
                intelligence = Mathf.RoundToInt(UnityEngine.Random.Range(10, 15) * rarityBonus);
                agility = Mathf.RoundToInt(UnityEngine.Random.Range(16, 20) * rarityBonus);
                charisma = Mathf.RoundToInt(UnityEngine.Random.Range(8, 12) * rarityBonus);
                luck = Mathf.RoundToInt(UnityEngine.Random.Range(12, 16) * rarityBonus);
                constitution = Mathf.RoundToInt(UnityEngine.Random.Range(10, 14) * rarityBonus);
                maxHealth = Mathf.RoundToInt(80 * rarityBonus);
                maxMana = Mathf.RoundToInt(50 * rarityBonus);
                break;
                
            case AdventurerClass.Cleric:
                strength = Mathf.RoundToInt(UnityEngine.Random.Range(8, 12) * rarityBonus);
                intelligence = Mathf.RoundToInt(UnityEngine.Random.Range(14, 18) * rarityBonus);
                agility = Mathf.RoundToInt(UnityEngine.Random.Range(8, 12) * rarityBonus);
                charisma = Mathf.RoundToInt(UnityEngine.Random.Range(14, 18) * rarityBonus);
                luck = Mathf.RoundToInt(UnityEngine.Random.Range(10, 15) * rarityBonus);
                constitution = Mathf.RoundToInt(UnityEngine.Random.Range(10, 14) * rarityBonus);
                maxHealth = Mathf.RoundToInt(80 * rarityBonus);
                maxMana = Mathf.RoundToInt(90 * rarityBonus);
                break;
                
            case AdventurerClass.Ranger:
                strength = Mathf.RoundToInt(UnityEngine.Random.Range(12, 16) * rarityBonus);
                intelligence = Mathf.RoundToInt(UnityEngine.Random.Range(10, 14) * rarityBonus);
                agility = Mathf.RoundToInt(UnityEngine.Random.Range(14, 18) * rarityBonus);
                charisma = Mathf.RoundToInt(UnityEngine.Random.Range(10, 14) * rarityBonus);
                luck = Mathf.RoundToInt(UnityEngine.Random.Range(12, 16) * rarityBonus);
                constitution = Mathf.RoundToInt(UnityEngine.Random.Range(12, 16) * rarityBonus);
                maxHealth = Mathf.RoundToInt(90 * rarityBonus);
                maxMana = Mathf.RoundToInt(60 * rarityBonus);
                break;
        }
        
        healthRegenRate = 1f + (constitution * 0.1f);
        manaRegenRate = 1f + (intelligence * 0.1f);
    }

    public void SetStatus(AdventurerStatus newStatus)
    {
        status = newStatus;
    }

    public void AddExperience(int amount)
    {
        experience += amount;
        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        while (experience >= experienceToNext && level < 20)
        {
            experience -= experienceToNext;
            level++;
            LevelUp();
            experienceToNext = level * 100;
        }
    }

    private void LevelUp()
    {
        strength += UnityEngine.Random.Range(1, 3);
        intelligence += UnityEngine.Random.Range(1, 3);
        agility += UnityEngine.Random.Range(1, 3);
        charisma += UnityEngine.Random.Range(1, 3);
        luck += UnityEngine.Random.Range(1, 3);
        maxHealth += UnityEngine.Random.Range(5, 15);
        health = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        health = Mathf.Max(0, health - damage);
        if (health <= 0)
        {
            status = AdventurerStatus.Dead;
        }
        else if (health < maxHealth * 0.3f)
        {
            status = AdventurerStatus.Injured;
        }
    }

    public void Heal(int amount)
    {
        health = Mathf.Min(maxHealth, health + amount);
        if (health > maxHealth * 0.5f && status == AdventurerStatus.Injured)
        {
            status = AdventurerStatus.Available;
        }
    }

    public bool EquipWeapon(Equipment newWeapon)
    {
        if (newWeapon == null || newWeapon.Type != Equipment.EquipmentType.Weapon) return false;
        if (!newWeapon.CanBeEquippedBy(this)) return false;
        
        weapon = newWeapon;
        RecalculateStats();
        return true;
    }

    public bool EquipArmor(Equipment newArmor)
    {
        if (newArmor == null || newArmor.Type != Equipment.EquipmentType.Armor) return false;
        if (!newArmor.CanBeEquippedBy(this)) return false;
        
        armor = newArmor;
        RecalculateStats();
        return true;
    }

    public bool EquipHelmet(Equipment newHelmet)
    {
        if (newHelmet == null || newHelmet.Type != Equipment.EquipmentType.Helmet) return false;
        if (!newHelmet.CanBeEquippedBy(this)) return false;
        
        helmet = newHelmet;
        RecalculateStats();
        return true;
    }

    public bool EquipBoots(Equipment newBoots)
    {
        if (newBoots == null || newBoots.Type != Equipment.EquipmentType.Boots) return false;
        if (!newBoots.CanBeEquippedBy(this)) return false;
        
        boots = newBoots;
        RecalculateStats();
        return true;
    }

    public bool EquipAccessory(Equipment newAccessory)
    {
        if (newAccessory == null || newAccessory.Type != Equipment.EquipmentType.Accessory) return false;
        if (!newAccessory.CanBeEquippedBy(this)) return false;
        
        accessory = newAccessory;
        RecalculateStats();
        return true;
    }

    public Equipment UnequipWeapon()
    {
        Equipment oldWeapon = weapon;
        weapon = null;
        RecalculateStats();
        return oldWeapon;
    }

    public Equipment UnequipArmor()
    {
        Equipment oldArmor = armor;
        armor = null;
        RecalculateStats();
        return oldArmor;
    }

    public Equipment UnequipHelmet()
    {
        Equipment oldHelmet = helmet;
        helmet = null;
        RecalculateStats();
        return oldHelmet;
    }

    public Equipment UnequipBoots()
    {
        Equipment oldBoots = boots;
        boots = null;
        RecalculateStats();
        return oldBoots;
    }

    public Equipment UnequipAccessory()
    {
        Equipment oldAccessory = accessory;
        accessory = null;
        RecalculateStats();
        return oldAccessory;
    }

    public void CompleteMission(bool successful)
    {
        missionsCompleted++;
        if (successful)
        {
            missionsSuccessful++;
            AddExperience(50);
        }
        else
        {
            AddExperience(10);
        }
    }

    public int GetCombatPower()
    {
        int basePower = strength + agility + (intelligence / 2) + (luck / 2);
        return basePower * level;
    }

    private float GetRarityStatBonus()
    {
        switch (rarity)
        {
            case AdventurerRarity.Common: return 1.0f;
            case AdventurerRarity.Uncommon: return 1.2f;
            case AdventurerRarity.Rare: return 1.4f;
            case AdventurerRarity.Epic: return 1.7f;
            case AdventurerRarity.Legendary: return 2.0f;
            default: return 1.0f;
        }
    }

    private void GenerateStartingEquipment()
    {
        // Generate basic equipment based on class and rarity
        Equipment.EquipmentRarity equipRarity = ConvertAdventurerRarityToEquipmentRarity();
        
        switch (adventurerClass)
        {
            case AdventurerClass.Warrior:
                weapon = new Equipment("Basic Sword", Equipment.EquipmentType.Weapon, equipRarity);
                armor = new Equipment("Leather Armor", Equipment.EquipmentType.Armor, equipRarity);
                break;
                
            case AdventurerClass.Mage:
                weapon = new Equipment("Wooden Staff", Equipment.EquipmentType.Weapon, equipRarity);
                armor = new Equipment("Cloth Robes", Equipment.EquipmentType.Armor, equipRarity);
                break;
                
            case AdventurerClass.Rogue:
                weapon = new Equipment("Iron Dagger", Equipment.EquipmentType.Weapon, equipRarity);
                armor = new Equipment("Leather Vest", Equipment.EquipmentType.Armor, equipRarity);
                break;
                
            case AdventurerClass.Cleric:
                weapon = new Equipment("Holy Mace", Equipment.EquipmentType.Weapon, equipRarity);
                armor = new Equipment("Chain Mail", Equipment.EquipmentType.Armor, equipRarity);
                break;
                
            case AdventurerClass.Ranger:
                weapon = new Equipment("Hunter's Bow", Equipment.EquipmentType.Weapon, equipRarity);
                armor = new Equipment("Ranger Cloak", Equipment.EquipmentType.Armor, equipRarity);
                break;
        }
    }

    private Equipment.EquipmentRarity ConvertAdventurerRarityToEquipmentRarity()
    {
        switch (rarity)
        {
            case AdventurerRarity.Common: return Equipment.EquipmentRarity.Common;
            case AdventurerRarity.Uncommon: return Equipment.EquipmentRarity.Uncommon;
            case AdventurerRarity.Rare: return Equipment.EquipmentRarity.Rare;
            case AdventurerRarity.Epic: return Equipment.EquipmentRarity.Epic;
            case AdventurerRarity.Legendary: return Equipment.EquipmentRarity.Legendary;
            default: return Equipment.EquipmentRarity.Common;
        }
    }

    private void GenerateStartingTraits()
    {
        // Generate 1-3 random traits based on rarity
        int traitCount = rarity switch
        {
            AdventurerRarity.Common => UnityEngine.Random.Range(0, 2),
            AdventurerRarity.Uncommon => UnityEngine.Random.Range(1, 3),
            AdventurerRarity.Rare => UnityEngine.Random.Range(2, 4),
            AdventurerRarity.Epic => UnityEngine.Random.Range(2, 4),
            AdventurerRarity.Legendary => UnityEngine.Random.Range(3, 5),
            _ => 1
        };

        string[] availableTraits = GetAvailableTraitsForClass();
        
        for (int i = 0; i < traitCount && i < availableTraits.Length; i++)
        {
            string traitName = availableTraits[UnityEngine.Random.Range(0, availableTraits.Length)];
            if (!traits.Any(t => t.Name == traitName))
            {
                traits.Add(CreateTrait(traitName));
            }
        }
    }

    private string[] GetAvailableTraitsForClass()
    {
        switch (adventurerClass)
        {
            case AdventurerClass.Warrior:
                return new[] { "Berserker", "Shield Master", "Weapon Expert", "Tough", "Intimidating" };
            case AdventurerClass.Mage:
                return new[] { "Arcane Focus", "Spell Mastery", "Mana Efficient", "Scholar", "Elemental Affinity" };
            case AdventurerClass.Rogue:
                return new[] { "Stealth Master", "Lucky", "Quick Reflexes", "Backstab", "Trap Detection" };
            case AdventurerClass.Cleric:
                return new[] { "Divine Favor", "Healer", "Turn Undead", "Blessed", "Righteous" };
            case AdventurerClass.Ranger:
                return new[] { "Eagle Eye", "Beast Friend", "Tracker", "Survivalist", "Dual Wield" };
            default:
                return new[] { "Determined", "Lucky", "Hardy" };
        }
    }

    private AdventurerTrait CreateTrait(string traitName)
    {
        var trait = new AdventurerTrait(traitName, GetTraitDescription(traitName), true);
        
        switch (traitName)
        {
            case "Berserker":
                trait.AddStatModifier("Strength", 0.2f);
                trait.AddStatModifier("Defense", -0.1f);
                break;
            case "Lucky":
                trait.AddStatModifier("Luck", 0.3f);
                break;
            case "Tough":
                trait.AddStatModifier("Constitution", 0.2f);
                break;
            case "Scholar":
                trait.AddStatModifier("Intelligence", 0.2f);
                break;
            case "Quick Reflexes":
                trait.AddStatModifier("Agility", 0.2f);
                break;
            default:
                trait.AddStatModifier("AllStats", 0.05f);
                break;
        }
        
        return trait;
    }

    private string GetTraitDescription(string traitName)
    {
        return traitName switch
        {
            "Berserker" => "Increased damage but reduced defense",
            "Lucky" => "Higher chance for critical hits and finds",
            "Tough" => "Increased health and resistance",
            "Scholar" => "Faster experience gain and magic power",
            "Quick Reflexes" => "Increased dodge chance and initiative",
            _ => $"Special trait: {traitName}"
        };
    }

    private void GenerateClassSkills()
    {
        // Add class-specific starting skills
        switch (adventurerClass)
        {
            case AdventurerClass.Warrior:
                skills.Add(new AdventurerSkill("Sword Fighting", "Mastery of sword combat", adventurerClass));
                skills.Add(new AdventurerSkill("Shield Defense", "Defensive techniques with shields", adventurerClass));
                break;
            case AdventurerClass.Mage:
                skills.Add(new AdventurerSkill("Arcane Magic", "Knowledge of arcane spells", adventurerClass));
                skills.Add(new AdventurerSkill("Mana Control", "Efficient use of magical energy", adventurerClass));
                break;
            case AdventurerClass.Rogue:
                skills.Add(new AdventurerSkill("Stealth", "Moving unseen and unheard", adventurerClass));
                skills.Add(new AdventurerSkill("Lockpicking", "Opening locks and traps", adventurerClass));
                break;
            case AdventurerClass.Cleric:
                skills.Add(new AdventurerSkill("Divine Magic", "Channeling divine power", adventurerClass));
                skills.Add(new AdventurerSkill("Healing", "Restoring health and vitality", adventurerClass));
                break;
            case AdventurerClass.Ranger:
                skills.Add(new AdventurerSkill("Tracking", "Following trails and signs", adventurerClass));
                skills.Add(new AdventurerSkill("Archery", "Precision with ranged weapons", adventurerClass));
                break;
        }
    }

    private void RecalculateStats()
    {
        // This method recalculates derived stats based on equipment and traits
        // Called after equipment changes or trait modifications
    }

    private int CalculateBaseSalary()
    {
        int baseSalary = 10; // Base daily salary
        
        // Rarity multiplier
        baseSalary = Mathf.RoundToInt(baseSalary * GetRarityStatBonus());
        
        // Level multiplier
        baseSalary += level * 2;
        
        return baseSalary;
    }

    private int CalculateRecruitmentCost()
    {
        int baseCost = 50;
        
        // Rarity multiplier
        baseCost = Mathf.RoundToInt(baseCost * GetRarityStatBonus() * 2f);
        
        // Level multiplier
        baseCost += level * 10;
        
        return baseCost;
    }

    private string GenerateBiography()
    {
        string[] backgrounds = adventurerClass switch
        {
            AdventurerClass.Warrior => new[] { 
                "A veteran of many battles, seeking new challenges.",
                "Former guard who left to seek fortune and glory.",
                "Trained in the arts of war from a young age."
            },
            AdventurerClass.Mage => new[] {
                "A scholar of the arcane arts with mysterious origins.",
                "Self-taught mage with a hunger for magical knowledge.",
                "Former apprentice seeking to prove their worth."
            },
            AdventurerClass.Rogue => new[] {
                "A shadowy figure with a questionable past.",
                "Former thief turned legitimate adventurer.",
                "Master of stealth and subterfuge."
            },
            AdventurerClass.Cleric => new[] {
                "Devoted servant of the divine seeking to do good.",
                "Healer who travels to help those in need.",
                "Former temple priest called to adventure."
            },
            AdventurerClass.Ranger => new[] {
                "Wilderness expert comfortable in nature.",
                "Hunter and tracker from the frontier.",
                "Guardian of the wild places."
            },
            _ => new[] { "An adventurer of unknown background." }
        };
        
        return backgrounds[UnityEngine.Random.Range(0, backgrounds.Length)];
    }

    public void SetPartyId(int partyId)
    {
        currentPartyId = partyId;
    }

    public int GetTotalAttack()
    {
        int attack = strength;
        if (weapon != null) attack += weapon.GetStatBonus("Attack");
        return attack;
    }

    public int GetTotalDefense()
    {
        int defense = constitution / 2;
        if (armor != null) defense += armor.GetStatBonus("Defense");
        if (helmet != null) defense += helmet.GetStatBonus("Defense");
        if (boots != null) defense += boots.GetStatBonus("Defense");
        return defense;
    }

    public string GetDetailedStats()
    {
        string stats = $"=== {name} ===\\n";
        stats += $"Class: {adventurerClass} | Rarity: {rarity} | Level: {level}\\n";
        stats += $"Health: {health}/{maxHealth} | Mana: {mana}/{maxMana}\\n";
        stats += $"STR: {strength} | INT: {intelligence} | AGI: {agility}\\n";
        stats += $"CHA: {charisma} | LUC: {luck} | CON: {constitution}\\n";
        stats += $"Attack: {GetTotalAttack()} | Defense: {GetTotalDefense()}\\n";
        stats += $"Success Rate: {SuccessRate:P1} | Loyalty: {loyaltyLevel:F0}%\\n";
        
        if (traits.Count > 0)
        {
            stats += $"Traits: {string.Join(", ", traits.Select(t => t.Name))}\\n";
        }
        
        return stats;
    }

    public override string ToString()
    {
        return $"{name} ({adventurerClass}) - Level {level} - {status}";
    }
}