using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class EnhancementMaterial
{
    public string name;
    public string description;
    public Equipment.EquipmentRarity rarity;
    public Dictionary<string, int> statBonuses;
    public float successChance;
    public int quantity;

    public EnhancementMaterial()
    {
        statBonuses = new Dictionary<string, int>();
        successChance = 1.0f;
        quantity = 0;
    }
}

[System.Serializable]
public class EnhancementLevel
{
    public int level;
    public int cost;
    public Dictionary<string, int> requiredMaterials;
    public float successChance;
    public float destructionChance;
    public Dictionary<string, int> statBonuses;

    public EnhancementLevel()
    {
        requiredMaterials = new Dictionary<string, int>();
        statBonuses = new Dictionary<string, int>();
        successChance = 1.0f;
        destructionChance = 0.0f;
    }
}

[System.Serializable]
public class GemSocket
{
    public string socketType; // "Red", "Blue", "Green", "Universal"
    public Gem insertedGem;
    public bool isActive;

    public GemSocket(string type)
    {
        socketType = type;
        insertedGem = null;
        isActive = true;
    }
}

[System.Serializable]
public class Gem
{
    public string name;
    public string description;
    public string socketType;
    public Equipment.EquipmentRarity rarity;
    public Dictionary<string, int> statBonuses;
    public Dictionary<string, float> statMultipliers;
    public List<string> specialEffects;
    public int level;
    public UnityEngine.Sprite icon;

    public UnityEngine.Sprite Icon => icon;

    public Gem()
    {
        statBonuses = new Dictionary<string, int>();
        statMultipliers = new Dictionary<string, float>();
        specialEffects = new List<string>();
        level = 1;
    }

    public bool CanFitInSocket(GemSocket socket)
    {
        return socket.socketType == "Universal" || socket.socketType == socketType;
    }
}

[System.Serializable]
public class EquipmentSet
{
    public string name;
    public string description;
    public List<string> requiredItems;
    public Dictionary<int, Dictionary<string, int>> setBonuses; // pieces -> stat bonuses
    public Dictionary<int, List<string>> setEffects; // pieces -> special effects

    public EquipmentSet()
    {
        requiredItems = new List<string>();
        setBonuses = new Dictionary<int, Dictionary<string, int>>();
        setEffects = new Dictionary<int, List<string>>();
    }
}

public class EquipmentEnhancementSystem : MonoBehaviour
{
    private static EquipmentEnhancementSystem instance;
    public static EquipmentEnhancementSystem Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<EquipmentEnhancementSystem>();
                if (instance == null)
                {
                    GameObject go = new GameObject("EquipmentEnhancementSystem");
                    instance = go.AddComponent<EquipmentEnhancementSystem>();
                    DontDestroyOnLoad(go);
                }
            }
            return instance;
        }
    }

    [Header("Enhancement Settings")]
    [SerializeField] private int maxEnhancementLevel = 15;
    [SerializeField] private float baseSuccessChance = 0.8f;
    [SerializeField] private float successDecayRate = 0.1f;
    [SerializeField] private float baseDestructionChance = 0.05f;
    [SerializeField] private float destructionIncreaseRate = 0.02f;

    [Header("Enhancement Materials")]
    [SerializeField] private Dictionary<string, EnhancementMaterial> enhancementMaterials;
    
    [Header("Enhancement Levels")]
    [SerializeField] private Dictionary<int, EnhancementLevel> enhancementLevels;
    
    [Header("Gems")]
    [SerializeField] private List<Gem> availableGems;
    [SerializeField] private Dictionary<string, Gem> gemInventory;
    
    [Header("Equipment Sets")]
    [SerializeField] private List<EquipmentSet> equipmentSets;

    public event Action<Equipment, int> OnEquipmentEnhanced;
    public event Action<Equipment> OnEquipmentDestroyed;
    public event Action<Equipment, Gem> OnGemInserted;
    public event Action<Equipment, Gem> OnGemRemoved;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeEnhancementSystem();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void InitializeEnhancementSystem()
    {
        enhancementMaterials = new Dictionary<string, EnhancementMaterial>();
        enhancementLevels = new Dictionary<int, EnhancementLevel>();
        availableGems = new List<Gem>();
        gemInventory = new Dictionary<string, Gem>();
        equipmentSets = new List<EquipmentSet>();
        
        InitializeEnhancementMaterials();
        InitializeEnhancementLevels();
        InitializeGems();
        InitializeEquipmentSets();
    }

    private void InitializeEnhancementMaterials()
    {
        // Basic Enhancement Materials
        var ironIngot = new EnhancementMaterial
        {
            name = "Lingot de Fer",
            description = "Matériau de base pour l'amélioration d'équipement",
            rarity = Equipment.EquipmentRarity.Common,
            statBonuses = new Dictionary<string, int> { {"Attack", 2}, {"Defense", 1} },
            successChance = 0.95f,
            quantity = 0
        };
        enhancementMaterials["iron_ingot"] = ironIngot;

        var steelIngot = new EnhancementMaterial
        {
            name = "Lingot d'Acier",
            description = "Matériau amélioré pour l'amélioration d'équipement",
            rarity = Equipment.EquipmentRarity.Uncommon,
            statBonuses = new Dictionary<string, int> { {"Attack", 4}, {"Defense", 3} },
            successChance = 0.9f,
            quantity = 0
        };
        enhancementMaterials["steel_ingot"] = steelIngot;

        var mithrilIngot = new EnhancementMaterial
        {
            name = "Lingot de Mithril",
            description = "Matériau rare aux propriétés magiques",
            rarity = Equipment.EquipmentRarity.Rare,
            statBonuses = new Dictionary<string, int> { {"Attack", 6}, {"Defense", 5}, {"Intelligence", 3} },
            successChance = 0.85f,
            quantity = 0
        };
        enhancementMaterials["mithril_ingot"] = mithrilIngot;

        var enchantedOil = new EnhancementMaterial
        {
            name = "Huile Enchantée",
            description = "Augmente les chances de succès d'amélioration",
            rarity = Equipment.EquipmentRarity.Uncommon,
            statBonuses = new Dictionary<string, int>(),
            successChance = 1.1f, // +10% success chance
            quantity = 0
        };
        enhancementMaterials["enchanted_oil"] = enchantedOil;

        var protectionCharm = new EnhancementMaterial
        {
            name = "Charme de Protection",
            description = "Réduit les chances de destruction",
            rarity = Equipment.EquipmentRarity.Rare,
            statBonuses = new Dictionary<string, int>(),
            successChance = 1.0f,
            quantity = 0
        };
        enhancementMaterials["protection_charm"] = protectionCharm;
    }

    private void InitializeEnhancementLevels()
    {
        for (int i = 1; i <= maxEnhancementLevel; i++)
        {
            var level = new EnhancementLevel
            {
                level = i,
                cost = 100 * i * i, // Exponential cost increase
                successChance = Mathf.Max(0.1f, baseSuccessChance - (i - 1) * successDecayRate),
                destructionChance = Mathf.Min(0.5f, baseDestructionChance + (i - 1) * destructionIncreaseRate),
                statBonuses = new Dictionary<string, int>
                {
                    {"Attack", i * 2},
                    {"Defense", i * 2},
                    {"Strength", i},
                    {"Intelligence", i},
                    {"Agility", i}
                }
            };

            // Material requirements based on level
            if (i <= 5)
            {
                level.requiredMaterials["iron_ingot"] = i;
            }
            else if (i <= 10)
            {
                level.requiredMaterials["steel_ingot"] = i - 5;
                level.requiredMaterials["iron_ingot"] = 5;
            }
            else
            {
                level.requiredMaterials["mithril_ingot"] = i - 10;
                level.requiredMaterials["steel_ingot"] = 5;
                level.requiredMaterials["iron_ingot"] = 5;
            }

            enhancementLevels[i] = level;
        }
    }

    private void InitializeGems()
    {
        // Red Gems (Attack focused)
        var rubyGem = new Gem
        {
            name = "Rubis",
            description = "Augmente les dégâts d'attaque",
            socketType = "Red",
            rarity = Equipment.EquipmentRarity.Uncommon,
            statBonuses = new Dictionary<string, int> { {"Attack", 8}, {"Strength", 4} },
            level = 1
        };
        availableGems.Add(rubyGem);

        var fireGem = new Gem
        {
            name = "Gemme de Feu",
            description = "Ajoute des dégâts de feu",
            socketType = "Red",
            rarity = Equipment.EquipmentRarity.Rare,
            statBonuses = new Dictionary<string, int> { {"Attack", 12}, {"Intelligence", 6} },
            specialEffects = new List<string> { "Fire_Damage" },
            level = 1
        };
        availableGems.Add(fireGem);

        // Blue Gems (Defense focused)
        var sapphireGem = new Gem
        {
            name = "Saphir",
            description = "Augmente la défense",
            socketType = "Blue",
            rarity = Equipment.EquipmentRarity.Uncommon,
            statBonuses = new Dictionary<string, int> { {"Defense", 10}, {"Constitution", 5} },
            level = 1
        };
        availableGems.Add(sapphireGem);

        var iceGem = new Gem
        {
            name = "Gemme de Glace",
            description = "Ajoute une défense glaciale",
            socketType = "Blue",
            rarity = Equipment.EquipmentRarity.Rare,
            statBonuses = new Dictionary<string, int> { {"Defense", 15}, {"Intelligence", 4} },
            specialEffects = new List<string> { "Ice_Armor" },
            level = 1
        };
        availableGems.Add(iceGem);

        // Green Gems (Utility focused)
        var emeraldGem = new Gem
        {
            name = "Émeraude",
            description = "Améliore la chance et l'agilité",
            socketType = "Green",
            rarity = Equipment.EquipmentRarity.Uncommon,
            statBonuses = new Dictionary<string, int> { {"Luck", 8}, {"Agility", 6} },
            level = 1
        };
        availableGems.Add(emeraldGem);

        var natureGem = new Gem
        {
            name = "Gemme de Nature",
            description = "Connexion avec la nature",
            socketType = "Green",
            rarity = Equipment.EquipmentRarity.Rare,
            statBonuses = new Dictionary<string, int> { {"Constitution", 8}, {"Charisma", 6} },
            specialEffects = new List<string> { "Nature_Healing" },
            level = 1
        };
        availableGems.Add(natureGem);

        // Universal Gems
        var diamondGem = new Gem
        {
            name = "Diamant",
            description = "Gemme universelle puissante",
            socketType = "Universal",
            rarity = Equipment.EquipmentRarity.Epic,
            statBonuses = new Dictionary<string, int> { {"Attack", 6}, {"Defense", 6}, {"Intelligence", 6} },
            statMultipliers = new Dictionary<string, float> { {"All_Stats", 0.05f} },
            level = 1
        };
        availableGems.Add(diamondGem);
    }

    private void InitializeEquipmentSets()
    {
        // Warrior Set
        var dragonSlayerSet = new EquipmentSet
        {
            name = "Tueur de Dragons",
            description = "Équipement légendaire des chasseurs de dragons",
            requiredItems = new List<string> { "Épée du Tueur de Dragons", "Armure du Tueur de Dragons", "Casque du Tueur de Dragons", "Bottes du Tueur de Dragons" }
        };
        
        dragonSlayerSet.setBonuses[2] = new Dictionary<string, int> { {"Attack", 15}, {"Defense", 10} };
        dragonSlayerSet.setBonuses[4] = new Dictionary<string, int> { {"Attack", 30}, {"Defense", 25}, {"Strength", 10} };
        dragonSlayerSet.setEffects[2] = new List<string> { "Dragon_Resistance" };
        dragonSlayerSet.setEffects[4] = new List<string> { "Dragon_Slayer", "Fire_Immunity" };
        
        equipmentSets.Add(dragonSlayerSet);

        // Mage Set
        var arcaneScholarSet = new EquipmentSet
        {
            name = "Érudit Arcanique",
            description = "Ensemble magique des grands mages",
            requiredItems = new List<string> { "Bâton de l'Érudit", "Robe de l'Érudit", "Couronne de l'Érudit", "Sandales de l'Érudit" }
        };
        
        arcaneScholarSet.setBonuses[2] = new Dictionary<string, int> { {"Intelligence", 20}, {"MaxMana", 50} };
        arcaneScholarSet.setBonuses[4] = new Dictionary<string, int> { {"Intelligence", 40}, {"MaxMana", 100}, {"Charisma", 15} };
        arcaneScholarSet.setEffects[2] = new List<string> { "Mana_Regeneration" };
        arcaneScholarSet.setEffects[4] = new List<string> { "Spell_Mastery", "Arcane_Shield" };
        
        equipmentSets.Add(arcaneScholarSet);
    }

    public EnhancementResult EnhanceEquipment(Equipment equipment, List<string> materialIds, bool useProtection = false)
    {
        if (equipment == null) return new EnhancementResult { success = false, message = "Équipement invalide" };
        
        int currentLevel = GetEnhancementLevel(equipment);
        if (currentLevel >= maxEnhancementLevel) 
            return new EnhancementResult { success = false, message = "Niveau d'amélioration maximum atteint" };

        int targetLevel = currentLevel + 1;
        if (!enhancementLevels.ContainsKey(targetLevel))
            return new EnhancementResult { success = false, message = "Niveau d'amélioration invalide" };

        var enhancementLevel = enhancementLevels[targetLevel];
        
        // Check materials
        if (!HasRequiredMaterials(enhancementLevel.requiredMaterials, materialIds))
            return new EnhancementResult { success = false, message = "Matériaux insuffisants" };

        // Check gold
        if (!ResourceManager.Instance.HasResource(Resource.ResourceType.Gold, enhancementLevel.cost))
            return new EnhancementResult { success = false, message = "Or insuffisant" };

        // Calculate success chance
        float successChance = enhancementLevel.successChance;
        float destructionChance = enhancementLevel.destructionChance;
        
        // Apply material bonuses
        foreach (string materialId in materialIds)
        {
            if (enhancementMaterials.ContainsKey(materialId))
            {
                var material = enhancementMaterials[materialId];
                successChance *= material.successChance;
                
                if (materialId == "protection_charm")
                {
                    destructionChance *= 0.5f; // 50% reduction
                }
            }
        }

        if (useProtection)
        {
            destructionChance = 0f; // No destruction with protection
            successChance *= 0.8f; // But lower success chance
        }

        // Consume materials and gold
        ConsumeMaterials(enhancementLevel.requiredMaterials, materialIds);
        ResourceManager.Instance.RemoveResource(Resource.ResourceType.Gold, enhancementLevel.cost);

        // Roll for success
        float roll = UnityEngine.Random.Range(0f, 1f);
        
        if (roll <= successChance)
        {
            // Success!
            ApplyEnhancement(equipment, targetLevel);
            OnEquipmentEnhanced?.Invoke(equipment, targetLevel);
            return new EnhancementResult 
            { 
                success = true, 
                message = $"Amélioration réussie! Niveau {targetLevel}",
                newLevel = targetLevel
            };
        }
        else if (roll <= successChance + destructionChance)
        {
            // Destruction!
            OnEquipmentDestroyed?.Invoke(equipment);
            return new EnhancementResult 
            { 
                success = false, 
                message = "Équipement détruit lors de l'amélioration!",
                destroyed = true
            };
        }
        else
        {
            // Failure but equipment survives
            return new EnhancementResult 
            { 
                success = false, 
                message = "Amélioration échouée, équipement intact"
            };
        }
    }

    public bool InsertGem(Equipment equipment, Gem gem, int socketIndex)
    {
        var sockets = GetEquipmentSockets(equipment);
        if (socketIndex < 0 || socketIndex >= sockets.Count) return false;
        
        var socket = sockets[socketIndex];
        if (socket.insertedGem != null) return false; // Socket already occupied
        if (!gem.CanFitInSocket(socket)) return false; // Gem doesn't fit
        
        socket.insertedGem = gem;
        ApplyGemBonuses(equipment, gem);
        OnGemInserted?.Invoke(equipment, gem);
        return true;
    }

    public bool RemoveGem(Equipment equipment, int socketIndex)
    {
        var sockets = GetEquipmentSockets(equipment);
        if (socketIndex < 0 || socketIndex >= sockets.Count) return false;
        
        var socket = sockets[socketIndex];
        if (socket.insertedGem == null) return false; // No gem to remove
        
        var gem = socket.insertedGem;
        socket.insertedGem = null;
        RemoveGemBonuses(equipment, gem);
        OnGemRemoved?.Invoke(equipment, gem);
        return true;
    }

    private void ApplyEnhancement(Equipment equipment, int level)
    {
        var enhancementLevel = enhancementLevels[level];
        foreach (var bonus in enhancementLevel.statBonuses)
        {
            if (equipment.StatBonuses.ContainsKey(bonus.Key))
                equipment.StatBonuses[bonus.Key] += bonus.Value;
            else
                equipment.StatBonuses[bonus.Key] = bonus.Value;
        }
        
        // Update equipment name to show enhancement level
        // This would require making the name field settable in Equipment class
        // equipment.name = $"{equipment.name} +{level}";
    }

    private void ApplyGemBonuses(Equipment equipment, Gem gem)
    {
        foreach (var bonus in gem.statBonuses)
        {
            if (equipment.StatBonuses.ContainsKey(bonus.Key))
                equipment.StatBonuses[bonus.Key] += bonus.Value;
            else
                equipment.StatBonuses[bonus.Key] = bonus.Value;
        }
    }

    private void RemoveGemBonuses(Equipment equipment, Gem gem)
    {
        foreach (var bonus in gem.statBonuses)
        {
            if (equipment.StatBonuses.ContainsKey(bonus.Key))
            {
                equipment.StatBonuses[bonus.Key] -= bonus.Value;
                if (equipment.StatBonuses[bonus.Key] <= 0)
                    equipment.StatBonuses.Remove(bonus.Key);
            }
        }
    }

    private bool HasRequiredMaterials(Dictionary<string, int> required, List<string> available)
    {
        foreach (var requirement in required)
        {
            if (!enhancementMaterials.ContainsKey(requirement.Key)) return false;
            if (enhancementMaterials[requirement.Key].quantity < requirement.Value) return false;
        }
        return true;
    }

    private void ConsumeMaterials(Dictionary<string, int> required, List<string> materialIds)
    {
        foreach (var requirement in required)
        {
            if (enhancementMaterials.ContainsKey(requirement.Key))
            {
                enhancementMaterials[requirement.Key].quantity -= requirement.Value;
            }
        }
    }

    public int GetEnhancementLevel(Equipment equipment)
    {
        // This would need to be stored in the Equipment class
        // For now, return 0 as placeholder
        return 0;
    }

    public List<GemSocket> GetEquipmentSockets(Equipment equipment)
    {
        // This would need to be stored in the Equipment class
        // For now, return empty list as placeholder
        return new List<GemSocket>();
    }

    public void AddEnhancementMaterial(string materialId, int quantity)
    {
        if (enhancementMaterials.ContainsKey(materialId))
        {
            enhancementMaterials[materialId].quantity += quantity;
        }
    }

    public void AddGem(Gem gem)
    {
        string gemId = $"{gem.name}_{gem.level}";
        gemInventory[gemId] = gem;
    }

    public Dictionary<string, int> GetSetBonuses(List<Equipment> equippedItems)
    {
        var totalBonuses = new Dictionary<string, int>();
        
        foreach (var set in equipmentSets)
        {
            int equippedPieces = 0;
            foreach (string requiredItem in set.requiredItems)
            {
                if (equippedItems.Any(item => item.Name == requiredItem))
                    equippedPieces++;
            }
            
            if (equippedPieces >= 2)
            {
                foreach (var setPiece in set.setBonuses)
                {
                    if (equippedPieces >= setPiece.Key)
                    {
                        foreach (var bonus in setPiece.Value)
                        {
                            if (totalBonuses.ContainsKey(bonus.Key))
                                totalBonuses[bonus.Key] += bonus.Value;
                            else
                                totalBonuses[bonus.Key] = bonus.Value;
                        }
                    }
                }
            }
        }
        
        return totalBonuses;
    }

    public Dictionary<string, EnhancementMaterial> GetEnhancementMaterials()
    {
        return new Dictionary<string, EnhancementMaterial>(enhancementMaterials);
    }

    public List<Gem> GetAvailableGems()
    {
        return new List<Gem>(availableGems);
    }

    public Dictionary<string, Gem> GetGemInventory()
    {
        return new Dictionary<string, Gem>(gemInventory);
    }

    public int GetEnhancementCost(int level)
    {
        if (enhancementLevels.ContainsKey(level))
        {
            return enhancementLevels[level].cost;
        }
        return 0;
    }
}

[System.Serializable]
public class EnhancementResult
{
    public bool success;
    public string message;
    public int newLevel;
    public bool destroyed;
}