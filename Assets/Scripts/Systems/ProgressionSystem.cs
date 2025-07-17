using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class TalentNode
{
    public string id;
    public string name;
    public string description;
    public Adventurer.AdventurerClass requiredClass;
    public int requiredLevel;
    public int skillPointCost;
    public List<string> prerequisites; // IDs of required talents
    public Dictionary<string, int> statBonuses;
    public Dictionary<string, float> statMultipliers;
    public List<string> specialAbilities;
    public bool unlocked;
    public int currentRank;
    public int maxRank;

    public TalentNode()
    {
        prerequisites = new List<string>();
        statBonuses = new Dictionary<string, int>();
        statMultipliers = new Dictionary<string, float>();
        specialAbilities = new List<string>();
        unlocked = false;
        currentRank = 0;
        maxRank = 1;
    }

    public bool CanUnlock(Adventurer adventurer, List<TalentNode> availableTalents)
    {
        if (unlocked && currentRank >= maxRank) return false;
        if (adventurer.Class != requiredClass && requiredClass != Adventurer.AdventurerClass.Warrior) return false; // Warrior as "Any"
        if (adventurer.Level < requiredLevel) return false;
        if (adventurer.SkillPoints < skillPointCost) return false;

        // Check prerequisites
        foreach (string prereqId in prerequisites)
        {
            var prereq = availableTalents.FirstOrDefault(t => t.id == prereqId);
            if (prereq == null || !prereq.unlocked) return false;
        }

        return true;
    }

    public int GetTotalCost()
    {
        return skillPointCost * (currentRank + 1);
    }
}

[System.Serializable]
public class SpecializationPath
{
    public string id;
    public string name;
    public string description;
    public Adventurer.AdventurerClass baseClass;
    public int requiredLevel;
    public List<string> requiredTalents;
    public Dictionary<string, int> statBonuses;
    public List<string> uniqueAbilities;
    public bool unlocked;

    public SpecializationPath()
    {
        requiredTalents = new List<string>();
        statBonuses = new Dictionary<string, int>();
        uniqueAbilities = new List<string>();
        unlocked = false;
    }
}

[System.Serializable]
public class MasteryData
{
    public string name;
    public int level;
    public int experience;
    public int experienceToNext;
    public int maxLevel;
    public Dictionary<string, float> bonuses;

    public MasteryData(string masteryName, int maxMasteryLevel = 20)
    {
        name = masteryName;
        level = 1;
        experience = 0;
        experienceToNext = 100;
        maxLevel = maxMasteryLevel;
        bonuses = new Dictionary<string, float>();
    }

    public bool AddExperience(int amount)
    {
        if (level >= maxLevel) return false;
        
        experience += amount;
        bool leveledUp = false;
        
        while (experience >= experienceToNext && level < maxLevel)
        {
            experience -= experienceToNext;
            level++;
            experienceToNext = level * 50;
            leveledUp = true;
        }
        
        return leveledUp;
    }

    public float GetBonus(string bonusType)
    {
        return bonuses.ContainsKey(bonusType) ? bonuses[bonusType] * level : 0f;
    }
}

public class ProgressionSystem : MonoBehaviour
{
    private static ProgressionSystem instance;
    public static ProgressionSystem Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ProgressionSystem>();
                if (instance == null)
                {
                    GameObject go = new GameObject("ProgressionSystem");
                    instance = go.AddComponent<ProgressionSystem>();
                    DontDestroyOnLoad(go);
                }
            }
            return instance;
        }
    }

    [Header("Talent Trees")]
    [SerializeField] private Dictionary<Adventurer.AdventurerClass, List<TalentNode>> talentTrees;
    
    [Header("Specialization Paths")]
    [SerializeField] private List<SpecializationPath> specializationPaths;
    
    [Header("Masteries")]
    [SerializeField] private Dictionary<string, MasteryData> masteryTemplates;
    
    [Header("Progression Settings")]
    [SerializeField] private int baseExperienceToLevel = 100;
    [SerializeField] private float experienceMultiplier = 1.5f;
    [SerializeField] private int skillPointsPerLevel = 2;
    [SerializeField] private int maxLevel = 50;

    public event Action<Adventurer, TalentNode> OnTalentUnlocked;
    public event Action<Adventurer, SpecializationPath> OnSpecializationUnlocked;
    public event Action<Adventurer, MasteryData> OnMasteryLevelUp;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeProgressionSystem();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void InitializeProgressionSystem()
    {
        talentTrees = new Dictionary<Adventurer.AdventurerClass, List<TalentNode>>();
        specializationPaths = new List<SpecializationPath>();
        masteryTemplates = new Dictionary<string, MasteryData>();
        
        InitializeTalentTrees();
        InitializeSpecializationPaths();
        InitializeMasteryTemplates();
    }

    private void InitializeTalentTrees()
    {
        InitializeWarriorTalents();
        InitializeMageTalents();
        InitializeRogueTalents();
        InitializeClericTalents();
        InitializeRangerTalents();
    }

    private void InitializeWarriorTalents()
    {
        var warriorTalents = new List<TalentNode>();

        // Tier 1 Talents
        var powerStrike = new TalentNode
        {
            id = "warrior_power_strike",
            name = "Frappe Puissante",
            description = "Augmente les dégâts d'attaque de 20%",
            requiredClass = Adventurer.AdventurerClass.Warrior,
            requiredLevel = 3,
            skillPointCost = 1,
            maxRank = 3,
            statMultipliers = new Dictionary<string, float> { {"Attack", 0.2f} }
        };
        warriorTalents.Add(powerStrike);

        var ironSkin = new TalentNode
        {
            id = "warrior_iron_skin",
            name = "Peau de Fer",
            description = "Augmente la défense de 15%",
            requiredClass = Adventurer.AdventurerClass.Warrior,
            requiredLevel = 3,
            skillPointCost = 1,
            maxRank = 3,
            statMultipliers = new Dictionary<string, float> { {"Defense", 0.15f} }
        };
        warriorTalents.Add(ironSkin);

        var weaponMaster = new TalentNode
        {
            id = "warrior_weapon_master",
            name = "Maître d'Armes",
            description = "Maîtrise avancée des armes",
            requiredClass = Adventurer.AdventurerClass.Warrior,
            requiredLevel = 8,
            skillPointCost = 2,
            prerequisites = new List<string> { "warrior_power_strike" },
            statBonuses = new Dictionary<string, int> { {"Strength", 5}, {"Attack", 10} },
            specialAbilities = new List<string> { "Weapon_Expertise" }
        };
        warriorTalents.Add(weaponMaster);

        var berserker = new TalentNode
        {
            id = "warrior_berserker",
            name = "Berserker",
            description = "Rage de combat - plus de dégâts mais moins de défense",
            requiredClass = Adventurer.AdventurerClass.Warrior,
            requiredLevel = 12,
            skillPointCost = 3,
            prerequisites = new List<string> { "warrior_weapon_master" },
            statMultipliers = new Dictionary<string, float> { {"Attack", 0.4f}, {"Defense", -0.2f} },
            specialAbilities = new List<string> { "Berserker_Rage" }
        };
        warriorTalents.Add(berserker);

        talentTrees[Adventurer.AdventurerClass.Warrior] = warriorTalents;
    }

    private void InitializeMageTalents()
    {
        var mageTalents = new List<TalentNode>();

        var arcaneKnowledge = new TalentNode
        {
            id = "mage_arcane_knowledge",
            name = "Savoir Arcanique",
            description = "Augmente l'intelligence et le mana",
            requiredClass = Adventurer.AdventurerClass.Mage,
            requiredLevel = 3,
            skillPointCost = 1,
            maxRank = 3,
            statBonuses = new Dictionary<string, int> { {"Intelligence", 3}, {"MaxMana", 20} }
        };
        mageTalents.Add(arcaneKnowledge);

        var elementalMastery = new TalentNode
        {
            id = "mage_elemental_mastery",
            name = "Maîtrise Élémentaire",
            description = "Contrôle des éléments",
            requiredClass = Adventurer.AdventurerClass.Mage,
            requiredLevel = 6,
            skillPointCost = 2,
            prerequisites = new List<string> { "mage_arcane_knowledge" },
            statBonuses = new Dictionary<string, int> { {"Intelligence", 5}, {"Attack", 8} },
            specialAbilities = new List<string> { "Elemental_Bolt", "Elemental_Shield" }
        };
        mageTalents.Add(elementalMastery);

        var archmage = new TalentNode
        {
            id = "mage_archmage",
            name = "Archimage",
            description = "Maîtrise suprême de la magie",
            requiredClass = Adventurer.AdventurerClass.Mage,
            requiredLevel = 15,
            skillPointCost = 4,
            prerequisites = new List<string> { "mage_elemental_mastery" },
            statBonuses = new Dictionary<string, int> { {"Intelligence", 10}, {"MaxMana", 50} },
            specialAbilities = new List<string> { "Meteor", "Time_Stop", "Teleport" }
        };
        mageTalents.Add(archmage);

        talentTrees[Adventurer.AdventurerClass.Mage] = mageTalents;
    }

    private void InitializeRogueTalents()
    {
        var rogueTalents = new List<TalentNode>();

        var stealth = new TalentNode
        {
            id = "rogue_stealth",
            name = "Furtivité",
            description = "Améliore la capacité à éviter les attaques",
            requiredClass = Adventurer.AdventurerClass.Rogue,
            requiredLevel = 3,
            skillPointCost = 1,
            maxRank = 3,
            statBonuses = new Dictionary<string, int> { {"Agility", 4}, {"Luck", 3} },
            specialAbilities = new List<string> { "Stealth" }
        };
        rogueTalents.Add(stealth);

        var backstab = new TalentNode
        {
            id = "rogue_backstab",
            name = "Attaque Sournoise",
            description = "Dégâts critiques augmentés",
            requiredClass = Adventurer.AdventurerClass.Rogue,
            requiredLevel = 6,
            skillPointCost = 2,
            prerequisites = new List<string> { "rogue_stealth" },
            statBonuses = new Dictionary<string, int> { {"Attack", 6}, {"Luck", 5} },
            specialAbilities = new List<string> { "Backstab", "Critical_Strike" }
        };
        rogueTalents.Add(backstab);

        var shadowMaster = new TalentNode
        {
            id = "rogue_shadow_master",
            name = "Maître des Ombres",
            description = "Maîtrise ultime de la furtivité",
            requiredClass = Adventurer.AdventurerClass.Rogue,
            requiredLevel = 15,
            skillPointCost = 4,
            prerequisites = new List<string> { "rogue_backstab" },
            statBonuses = new Dictionary<string, int> { {"Agility", 8}, {"Luck", 8}, {"Attack", 10} },
            specialAbilities = new List<string> { "Shadow_Clone", "Assassination", "Vanish" }
        };
        rogueTalents.Add(shadowMaster);

        talentTrees[Adventurer.AdventurerClass.Rogue] = rogueTalents;
    }

    private void InitializeClericTalents()
    {
        var clericTalents = new List<TalentNode>();

        var divineBlessing = new TalentNode
        {
            id = "cleric_divine_blessing",
            name = "Bénédiction Divine",
            description = "Améliore les capacités de soin",
            requiredClass = Adventurer.AdventurerClass.Cleric,
            requiredLevel = 3,
            skillPointCost = 1,
            maxRank = 3,
            statBonuses = new Dictionary<string, int> { {"Intelligence", 3}, {"Charisma", 3} },
            specialAbilities = new List<string> { "Heal", "Blessing" }
        };
        clericTalents.Add(divineBlessing);

        var holyWarrior = new TalentNode
        {
            id = "cleric_holy_warrior",
            name = "Guerrier Sacré",
            description = "Combat au service du divin",
            requiredClass = Adventurer.AdventurerClass.Cleric,
            requiredLevel = 6,
            skillPointCost = 2,
            prerequisites = new List<string> { "cleric_divine_blessing" },
            statBonuses = new Dictionary<string, int> { {"Attack", 8}, {"Defense", 6} },
            specialAbilities = new List<string> { "Holy_Strike", "Divine_Shield" }
        };
        clericTalents.Add(holyWarrior);

        var highPriest = new TalentNode
        {
            id = "cleric_high_priest",
            name = "Grand Prêtre",
            description = "Pouvoir divin ultime",
            requiredClass = Adventurer.AdventurerClass.Cleric,
            requiredLevel = 15,
            skillPointCost = 4,
            prerequisites = new List<string> { "cleric_holy_warrior" },
            statBonuses = new Dictionary<string, int> { {"Intelligence", 8}, {"Charisma", 10} },
            specialAbilities = new List<string> { "Resurrection", "Divine_Intervention", "Mass_Heal" }
        };
        clericTalents.Add(highPriest);

        talentTrees[Adventurer.AdventurerClass.Cleric] = clericTalents;
    }

    private void InitializeRangerTalents()
    {
        var rangerTalents = new List<TalentNode>();

        var tracking = new TalentNode
        {
            id = "ranger_tracking",
            name = "Pistage",
            description = "Améliore la recherche et l'exploration",
            requiredClass = Adventurer.AdventurerClass.Ranger,
            requiredLevel = 3,
            skillPointCost = 1,
            maxRank = 3,
            statBonuses = new Dictionary<string, int> { {"Agility", 3}, {"Luck", 4} },
            specialAbilities = new List<string> { "Track", "Hunt" }
        };
        rangerTalents.Add(tracking);

        var beastMaster = new TalentNode
        {
            id = "ranger_beast_master",
            name = "Maître des Bêtes",
            description = "Communion avec les animaux",
            requiredClass = Adventurer.AdventurerClass.Ranger,
            requiredLevel = 8,
            skillPointCost = 2,
            prerequisites = new List<string> { "ranger_tracking" },
            statBonuses = new Dictionary<string, int> { {"Charisma", 6}, {"Constitution", 4} },
            specialAbilities = new List<string> { "Animal_Companion", "Beast_Speech" }
        };
        rangerTalents.Add(beastMaster);

        var lordOfWild = new TalentNode
        {
            id = "ranger_lord_of_wild",
            name = "Seigneur Sauvage",
            description = "Maîtrise absolue de la nature",
            requiredClass = Adventurer.AdventurerClass.Ranger,
            requiredLevel = 15,
            skillPointCost = 4,
            prerequisites = new List<string> { "ranger_beast_master" },
            statBonuses = new Dictionary<string, int> { {"Agility", 10}, {"Constitution", 8} },
            specialAbilities = new List<string> { "Nature_Control", "Pack_Leader", "Wild_Form" }
        };
        rangerTalents.Add(lordOfWild);

        talentTrees[Adventurer.AdventurerClass.Ranger] = rangerTalents;
    }

    private void InitializeSpecializationPaths()
    {
        // Warrior Specializations
        var paladin = new SpecializationPath
        {
            id = "warrior_paladin",
            name = "Paladin",
            description = "Guerrier saint alliant force et magie divine",
            baseClass = Adventurer.AdventurerClass.Warrior,
            requiredLevel = 10,
            requiredTalents = new List<string> { "warrior_iron_skin", "warrior_weapon_master" },
            statBonuses = new Dictionary<string, int> { {"Charisma", 8}, {"Intelligence", 5} },
            uniqueAbilities = new List<string> { "Lay_On_Hands", "Smite_Evil", "Aura_Of_Protection" }
        };
        specializationPaths.Add(paladin);

        var berserker = new SpecializationPath
        {
            id = "warrior_berserker_path",
            name = "Berserker",
            description = "Guerrier sauvage sacrifiant défense pour attaque",
            baseClass = Adventurer.AdventurerClass.Warrior,
            requiredLevel = 12,
            requiredTalents = new List<string> { "warrior_berserker" },
            statBonuses = new Dictionary<string, int> { {"Strength", 10}, {"Constitution", 8} },
            uniqueAbilities = new List<string> { "Unstoppable_Rage", "Intimidating_Shout", "Bloodlust" }
        };
        specializationPaths.Add(berserker);

        // Mage Specializations
        var battlemage = new SpecializationPath
        {
            id = "mage_battlemage",
            name = "Mage de Guerre",
            description = "Mage combattant alliant magie et combat",
            baseClass = Adventurer.AdventurerClass.Mage,
            requiredLevel = 10,
            requiredTalents = new List<string> { "mage_elemental_mastery" },
            statBonuses = new Dictionary<string, int> { {"Strength", 6}, {"Constitution", 6} },
            uniqueAbilities = new List<string> { "Spell_Sword", "Mage_Armor", "Counterspell" }
        };
        specializationPaths.Add(battlemage);

        // Rogue Specializations
        var assassin = new SpecializationPath
        {
            id = "rogue_assassin",
            name = "Assassin",
            description = "Tueur silencieux spécialisé dans l'élimination",
            baseClass = Adventurer.AdventurerClass.Rogue,
            requiredLevel = 12,
            requiredTalents = new List<string> { "rogue_shadow_master" },
            statBonuses = new Dictionary<string, int> { {"Agility", 8}, {"Luck", 10} },
            uniqueAbilities = new List<string> { "Death_Strike", "Poison_Blade", "Shadow_Step" }
        };
        specializationPaths.Add(assassin);
    }

    private void InitializeMasteryTemplates()
    {
        // Weapon Masteries
        var swordMastery = new MasteryData("Maîtrise des Épées", 25);
        swordMastery.bonuses["Attack"] = 0.05f; // 5% per level
        swordMastery.bonuses["Critical_Chance"] = 0.02f; // 2% per level
        masteryTemplates["sword_mastery"] = swordMastery;

        var bowMastery = new MasteryData("Maîtrise des Arcs", 25);
        bowMastery.bonuses["Attack"] = 0.04f;
        bowMastery.bonuses["Accuracy"] = 0.03f;
        masteryTemplates["bow_mastery"] = bowMastery;

        var staffMastery = new MasteryData("Maîtrise des Bâtons", 25);
        staffMastery.bonuses["Intelligence"] = 0.05f;
        staffMastery.bonuses["Mana_Cost_Reduction"] = 0.02f;
        masteryTemplates["staff_mastery"] = staffMastery;

        // Combat Masteries
        var combatMastery = new MasteryData("Maîtrise du Combat", 30);
        combatMastery.bonuses["Attack"] = 0.03f;
        combatMastery.bonuses["Defense"] = 0.02f;
        masteryTemplates["combat_mastery"] = combatMastery;

        var magicMastery = new MasteryData("Maîtrise de la Magie", 30);
        magicMastery.bonuses["Intelligence"] = 0.04f;
        magicMastery.bonuses["Spell_Power"] = 0.05f;
        masteryTemplates["magic_mastery"] = magicMastery;

        var stealthMastery = new MasteryData("Maîtrise de la Furtivité", 25);
        stealthMastery.bonuses["Agility"] = 0.04f;
        stealthMastery.bonuses["Stealth_Effectiveness"] = 0.06f;
        masteryTemplates["stealth_mastery"] = stealthMastery;
    }

    public bool UnlockTalent(Adventurer adventurer, string talentId)
    {
        if (!talentTrees.ContainsKey(adventurer.Class)) return false;
        
        var talent = talentTrees[adventurer.Class].FirstOrDefault(t => t.id == talentId);
        if (talent == null) return false;
        
        if (!talent.CanUnlock(adventurer, talentTrees[adventurer.Class])) return false;
        
        // Spend skill points
        // Note: This would require adding a method to spend skill points in Adventurer class
        
        if (!talent.unlocked)
        {
            talent.unlocked = true;
            talent.currentRank = 1;
        }
        else
        {
            talent.currentRank++;
        }
        
        OnTalentUnlocked?.Invoke(adventurer, talent);
        return true;
    }

    public bool UnlockSpecialization(Adventurer adventurer, string specializationId)
    {
        var specialization = specializationPaths.FirstOrDefault(s => s.id == specializationId);
        if (specialization == null) return false;
        
        if (adventurer.Class != specialization.baseClass) return false;
        if (adventurer.Level < specialization.requiredLevel) return false;
        
        // Check required talents
        var adventurerTalents = talentTrees[adventurer.Class];
        foreach (string requiredTalent in specialization.requiredTalents)
        {
            var talent = adventurerTalents.FirstOrDefault(t => t.id == requiredTalent);
            if (talent == null || !talent.unlocked) return false;
        }
        
        specialization.unlocked = true;
        OnSpecializationUnlocked?.Invoke(adventurer, specialization);
        return true;
    }

    public List<TalentNode> GetAvailableTalents(Adventurer adventurer)
    {
        if (!talentTrees.ContainsKey(adventurer.Class)) return new List<TalentNode>();
        
        return talentTrees[adventurer.Class]
            .Where(t => t.CanUnlock(adventurer, talentTrees[adventurer.Class]))
            .ToList();
    }

    public List<SpecializationPath> GetAvailableSpecializations(Adventurer adventurer)
    {
        return specializationPaths
            .Where(s => s.baseClass == adventurer.Class && !s.unlocked)
            .ToList();
    }

    public int CalculateExperienceToNextLevel(int currentLevel)
    {
        return Mathf.RoundToInt(baseExperienceToLevel * Mathf.Pow(experienceMultiplier, currentLevel - 1));
    }

    public int CalculateSkillPointsEarned(int level)
    {
        return (level - 1) * skillPointsPerLevel;
    }

    public Dictionary<string, MasteryData> GetMasteryTemplates()
    {
        return new Dictionary<string, MasteryData>(masteryTemplates);
    }
}