using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ItemTemplate
{
    public string baseName;
    public Equipment.EquipmentType type;
    public string description;
    public List<string> possiblePrefixes;
    public List<string> possibleSuffixes;
    public Dictionary<string, int> baseStats;
    public List<Adventurer.AdventurerClass> preferredClasses;
    public int baseValue;
    public int levelRequirement;

    public ItemTemplate()
    {
        possiblePrefixes = new List<string>();
        possibleSuffixes = new List<string>();
        baseStats = new Dictionary<string, int>();
        preferredClasses = new List<Adventurer.AdventurerClass>();
    }
}

[System.Serializable]
public class ItemAffix
{
    public string name;
    public string description;
    public Dictionary<string, int> statBonuses;
    public Equipment.EquipmentRarity minRarity;
    public bool isPrefix;
    public float weight;

    public ItemAffix()
    {
        statBonuses = new Dictionary<string, int>();
        weight = 1.0f;
    }
}

public class ItemDatabase : MonoBehaviour
{
    private static ItemDatabase instance;
    public static ItemDatabase Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ItemDatabase>();
                if (instance == null)
                {
                    GameObject go = new GameObject("ItemDatabase");
                    instance = go.AddComponent<ItemDatabase>();
                    DontDestroyOnLoad(go);
                }
            }
            return instance;
        }
    }

    [Header("Item Templates")]
    [SerializeField] private List<ItemTemplate> weaponTemplates;
    [SerializeField] private List<ItemTemplate> armorTemplates;
    [SerializeField] private List<ItemTemplate> helmetTemplates;
    [SerializeField] private List<ItemTemplate> bootsTemplates;
    [SerializeField] private List<ItemTemplate> accessoryTemplates;
    [SerializeField] private List<ItemTemplate> shieldTemplates;

    [Header("Item Affixes")]
    [SerializeField] private List<ItemAffix> prefixes;
    [SerializeField] private List<ItemAffix> suffixes;

    [Header("Generation Settings")]
    [SerializeField] private float affixChanceByRarity = 0.1f;
    [SerializeField] private int maxAffixesPerItem = 3;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDatabase();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void InitializeDatabase()
    {
        InitializeWeaponTemplates();
        InitializeArmorTemplates();
        InitializeHelmetTemplates();
        InitializeBootsTemplates();
        InitializeAccessoryTemplates();
        InitializeShieldTemplates();
        InitializeAffixes();
    }

    public Equipment GenerateRandomEquipment(Equipment.EquipmentType type, Equipment.EquipmentRarity rarity, int level = 1)
    {
        List<ItemTemplate> templates = GetTemplatesForType(type);
        if (templates.Count == 0) return null;

        ItemTemplate template = templates[UnityEngine.Random.Range(0, templates.Count)];
        Equipment equipment = CreateEquipmentFromTemplate(template, rarity, level);
        
        ApplyAffixes(equipment, rarity);
        
        return equipment;
    }

    public Equipment GenerateEquipmentForClass(Equipment.EquipmentType type, Adventurer.AdventurerClass adventurerClass, Equipment.EquipmentRarity rarity, int level = 1)
    {
        List<ItemTemplate> templates = GetTemplatesForType(type);
        ItemTemplate template = templates.FirstOrDefault(t => t.preferredClasses.Contains(adventurerClass));
        
        if (template == null)
            template = templates[UnityEngine.Random.Range(0, templates.Count)];

        Equipment equipment = CreateEquipmentFromTemplate(template, rarity, level);
        ApplyAffixes(equipment, rarity);
        
        return equipment;
    }

    private List<ItemTemplate> GetTemplatesForType(Equipment.EquipmentType type)
    {
        switch (type)
        {
            case Equipment.EquipmentType.Weapon: return weaponTemplates;
            case Equipment.EquipmentType.Armor: return armorTemplates;
            case Equipment.EquipmentType.Helmet: return helmetTemplates;
            case Equipment.EquipmentType.Boots: return bootsTemplates;
            case Equipment.EquipmentType.Accessory: return accessoryTemplates;
            case Equipment.EquipmentType.Shield: return shieldTemplates;
            default: return new List<ItemTemplate>();
        }
    }

    private Equipment CreateEquipmentFromTemplate(ItemTemplate template, Equipment.EquipmentRarity rarity, int level)
    {
        Equipment equipment = new Equipment(template.baseName, template.type, rarity);
        
        // Apply template base stats scaled by level and rarity
        float rarityMultiplier = GetRarityMultiplier(rarity);
        float levelMultiplier = 1.0f + (level - 1) * 0.1f;
        
        foreach (var stat in template.baseStats)
        {
            int finalValue = Mathf.RoundToInt(stat.Value * rarityMultiplier * levelMultiplier);
            equipment.StatBonuses[stat.Key] = finalValue;
        }
        
        return equipment;
    }

    private void ApplyAffixes(Equipment equipment, Equipment.EquipmentRarity rarity)
    {
        int affixCount = GetAffixCount(rarity);
        List<ItemAffix> availableAffixes = GetAvailableAffixes(rarity);
        
        for (int i = 0; i < affixCount; i++)
        {
            ItemAffix affix = SelectWeightedAffix(availableAffixes);
            if (affix != null)
            {
                ApplyAffixToEquipment(equipment, affix);
                availableAffixes.Remove(affix); // No duplicate affixes
            }
        }
    }

    private int GetAffixCount(Equipment.EquipmentRarity rarity)
    {
        switch (rarity)
        {
            case Equipment.EquipmentRarity.Common: return 0;
            case Equipment.EquipmentRarity.Uncommon: return UnityEngine.Random.Range(0, 2);
            case Equipment.EquipmentRarity.Rare: return UnityEngine.Random.Range(1, 3);
            case Equipment.EquipmentRarity.Epic: return UnityEngine.Random.Range(2, 4);
            case Equipment.EquipmentRarity.Legendary: return UnityEngine.Random.Range(3, 5);
            case Equipment.EquipmentRarity.Artifact: return UnityEngine.Random.Range(4, 6);
            default: return 0;
        }
    }

    private List<ItemAffix> GetAvailableAffixes(Equipment.EquipmentRarity rarity)
    {
        List<ItemAffix> available = new List<ItemAffix>();
        available.AddRange(prefixes.Where(p => p.minRarity <= rarity));
        available.AddRange(suffixes.Where(s => s.minRarity <= rarity));
        return available;
    }

    private ItemAffix SelectWeightedAffix(List<ItemAffix> affixes)
    {
        if (affixes.Count == 0) return null;
        
        float totalWeight = affixes.Sum(a => a.weight);
        float randomValue = UnityEngine.Random.Range(0f, totalWeight);
        
        float currentWeight = 0f;
        foreach (ItemAffix affix in affixes)
        {
            currentWeight += affix.weight;
            if (randomValue <= currentWeight)
                return affix;
        }
        
        return affixes[0];
    }

    private void ApplyAffixToEquipment(Equipment equipment, ItemAffix affix)
    {
        foreach (var stat in affix.statBonuses)
        {
            if (equipment.StatBonuses.ContainsKey(stat.Key))
                equipment.StatBonuses[stat.Key] += stat.Value;
            else
                equipment.StatBonuses[stat.Key] = stat.Value;
        }
        
        // Update equipment name with affix
        string currentName = equipment.Name;
        if (affix.isPrefix)
            currentName = $"{affix.name} {currentName}";
        else
            currentName = $"{currentName} {affix.name}";
        
        // Note: This would require making the name field settable in Equipment class
    }

    private float GetRarityMultiplier(Equipment.EquipmentRarity rarity)
    {
        switch (rarity)
        {
            case Equipment.EquipmentRarity.Common: return 1.0f;
            case Equipment.EquipmentRarity.Uncommon: return 1.3f;
            case Equipment.EquipmentRarity.Rare: return 1.7f;
            case Equipment.EquipmentRarity.Epic: return 2.2f;
            case Equipment.EquipmentRarity.Legendary: return 2.8f;
            case Equipment.EquipmentRarity.Artifact: return 3.5f;
            default: return 1.0f;
        }
    }

    #region Template Initialization
    private void InitializeWeaponTemplates()
    {
        weaponTemplates = new List<ItemTemplate>();
        
        // Swords
        var sword = new ItemTemplate
        {
            baseName = "Épée",
            type = Equipment.EquipmentType.Weapon,
            description = "Une épée bien équilibrée",
            baseStats = new Dictionary<string, int> { {"Attack", 15}, {"Strength", 5} },
            preferredClasses = new List<Adventurer.AdventurerClass> { Adventurer.AdventurerClass.Warrior },
            baseValue = 100,
            levelRequirement = 1
        };
        weaponTemplates.Add(sword);

        // Staff
        var staff = new ItemTemplate
        {
            baseName = "Bâton",
            type = Equipment.EquipmentType.Weapon,
            description = "Un bâton magique",
            baseStats = new Dictionary<string, int> { {"Attack", 8}, {"Intelligence", 12} },
            preferredClasses = new List<Adventurer.AdventurerClass> { Adventurer.AdventurerClass.Mage },
            baseValue = 120,
            levelRequirement = 1
        };
        weaponTemplates.Add(staff);

        // Dagger
        var dagger = new ItemTemplate
        {
            baseName = "Dague",
            type = Equipment.EquipmentType.Weapon,
            description = "Une lame rapide et précise",
            baseStats = new Dictionary<string, int> { {"Attack", 12}, {"Agility", 8} },
            preferredClasses = new List<Adventurer.AdventurerClass> { Adventurer.AdventurerClass.Rogue },
            baseValue = 80,
            levelRequirement = 1
        };
        weaponTemplates.Add(dagger);

        // Mace
        var mace = new ItemTemplate
        {
            baseName = "Masse",
            type = Equipment.EquipmentType.Weapon,
            description = "Une arme sacrée",
            baseStats = new Dictionary<string, int> { {"Attack", 13}, {"Constitution", 6} },
            preferredClasses = new List<Adventurer.AdventurerClass> { Adventurer.AdventurerClass.Cleric },
            baseValue = 110,
            levelRequirement = 1
        };
        weaponTemplates.Add(mace);

        // Bow
        var bow = new ItemTemplate
        {
            baseName = "Arc",
            type = Equipment.EquipmentType.Weapon,
            description = "Un arc de précision",
            baseStats = new Dictionary<string, int> { {"Attack", 14}, {"Agility", 7} },
            preferredClasses = new List<Adventurer.AdventurerClass> { Adventurer.AdventurerClass.Ranger },
            baseValue = 95,
            levelRequirement = 1
        };
        weaponTemplates.Add(bow);
    }

    private void InitializeArmorTemplates()
    {
        armorTemplates = new List<ItemTemplate>();
        
        var chainmail = new ItemTemplate
        {
            baseName = "Cotte de Mailles",
            type = Equipment.EquipmentType.Armor,
            description = "Une armure légère mais résistante",
            baseStats = new Dictionary<string, int> { {"Defense", 20}, {"Constitution", 8} },
            baseValue = 150,
            levelRequirement = 1
        };
        armorTemplates.Add(chainmail);

        var robe = new ItemTemplate
        {
            baseName = "Robe",
            type = Equipment.EquipmentType.Armor,
            description = "Une robe magique",
            baseStats = new Dictionary<string, int> { {"Defense", 10}, {"Intelligence", 15} },
            baseValue = 120,
            levelRequirement = 1
        };
        armorTemplates.Add(robe);

        var leather = new ItemTemplate
        {
            baseName = "Armure de Cuir",
            type = Equipment.EquipmentType.Armor,
            description = "Une armure souple pour la furtivité",
            baseStats = new Dictionary<string, int> { {"Defense", 15}, {"Agility", 10} },
            baseValue = 100,
            levelRequirement = 1
        };
        armorTemplates.Add(leather);
    }

    private void InitializeHelmetTemplates()
    {
        helmetTemplates = new List<ItemTemplate>();
        
        var helmet = new ItemTemplate
        {
            baseName = "Casque",
            type = Equipment.EquipmentType.Helmet,
            description = "Un casque protecteur",
            baseStats = new Dictionary<string, int> { {"Defense", 8}, {"Constitution", 5} },
            baseValue = 80,
            levelRequirement = 1
        };
        helmetTemplates.Add(helmet);

        var circlet = new ItemTemplate
        {
            baseName = "Diadème",
            type = Equipment.EquipmentType.Helmet,
            description = "Un diadème magique",
            baseStats = new Dictionary<string, int> { {"Defense", 4}, {"Intelligence", 10} },
            baseValue = 100,
            levelRequirement = 1
        };
        helmetTemplates.Add(circlet);
    }

    private void InitializeBootsTemplates()
    {
        bootsTemplates = new List<ItemTemplate>();
        
        var boots = new ItemTemplate
        {
            baseName = "Bottes",
            type = Equipment.EquipmentType.Boots,
            description = "Des bottes robustes",
            baseStats = new Dictionary<string, int> { {"Defense", 6}, {"Agility", 8} },
            baseValue = 60,
            levelRequirement = 1
        };
        bootsTemplates.Add(boots);
    }

    private void InitializeAccessoryTemplates()
    {
        accessoryTemplates = new List<ItemTemplate>();
        
        var ring = new ItemTemplate
        {
            baseName = "Anneau",
            type = Equipment.EquipmentType.Accessory,
            description = "Un anneau magique",
            baseStats = new Dictionary<string, int> { {"Luck", 10}, {"Charisma", 5} },
            baseValue = 150,
            levelRequirement = 1
        };
        accessoryTemplates.Add(ring);

        var amulet = new ItemTemplate
        {
            baseName = "Amulette",
            type = Equipment.EquipmentType.Accessory,
            description = "Une amulette protectrice",
            baseStats = new Dictionary<string, int> { {"Defense", 5}, {"Luck", 8} },
            baseValue = 120,
            levelRequirement = 1
        };
        accessoryTemplates.Add(amulet);
    }

    private void InitializeShieldTemplates()
    {
        shieldTemplates = new List<ItemTemplate>();
        
        var shield = new ItemTemplate
        {
            baseName = "Bouclier",
            type = Equipment.EquipmentType.Shield,
            description = "Un bouclier solide",
            baseStats = new Dictionary<string, int> { {"Defense", 15}, {"Constitution", 8} },
            baseValue = 100,
            levelRequirement = 1
        };
        shieldTemplates.Add(shield);
    }

    private void InitializeAffixes()
    {
        prefixes = new List<ItemAffix>();
        suffixes = new List<ItemAffix>();

        // Prefixes
        var sharp = new ItemAffix
        {
            name = "Acéré",
            isPrefix = true,
            statBonuses = new Dictionary<string, int> { {"Attack", 5} },
            minRarity = Equipment.EquipmentRarity.Uncommon,
            weight = 1.0f
        };
        prefixes.Add(sharp);

        var heavy = new ItemAffix
        {
            name = "Lourd",
            isPrefix = true,
            statBonuses = new Dictionary<string, int> { {"Defense", 8}, {"Agility", -3} },
            minRarity = Equipment.EquipmentRarity.Uncommon,
            weight = 0.8f
        };
        prefixes.Add(heavy);

        var blessed = new ItemAffix
        {
            name = "Béni",
            isPrefix = true,
            statBonuses = new Dictionary<string, int> { {"Luck", 10}, {"Charisma", 5} },
            minRarity = Equipment.EquipmentRarity.Rare,
            weight = 0.6f
        };
        prefixes.Add(blessed);

        // Suffixes
        var power = new ItemAffix
        {
            name = "de Puissance",
            isPrefix = false,
            statBonuses = new Dictionary<string, int> { {"Strength", 8} },
            minRarity = Equipment.EquipmentRarity.Uncommon,
            weight = 1.0f
        };
        suffixes.Add(power);

        var wisdom = new ItemAffix
        {
            name = "de Sagesse",
            isPrefix = false,
            statBonuses = new Dictionary<string, int> { {"Intelligence", 10} },
            minRarity = Equipment.EquipmentRarity.Uncommon,
            weight = 1.0f
        };
        suffixes.Add(wisdom);

        var swiftness = new ItemAffix
        {
            name = "de Rapidité",
            isPrefix = false,
            statBonuses = new Dictionary<string, int> { {"Agility", 12} },
            minRarity = Equipment.EquipmentRarity.Rare,
            weight = 0.8f
        };
        suffixes.Add(swiftness);

        var protection = new ItemAffix
        {
            name = "de Protection",
            isPrefix = false,
            statBonuses = new Dictionary<string, int> { {"Defense", 10}, {"Constitution", 5} },
            minRarity = Equipment.EquipmentRarity.Rare,
            weight = 0.7f
        };
        suffixes.Add(protection);
    }
    #endregion
}