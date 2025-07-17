using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class CraftingRecipe
{
    public string name;
    public string description;
    public Equipment.EquipmentType outputType;
    public Equipment.EquipmentRarity outputRarity;
    public Dictionary<string, int> requiredMaterials;
    public int goldCost;
    public int craftingTime; // in hours
    public int requiredCrafterLevel;
    public List<Adventurer.AdventurerClass> preferredClasses;
    public bool unlocked;

    public CraftingRecipe()
    {
        requiredMaterials = new Dictionary<string, int>();
        preferredClasses = new List<Adventurer.AdventurerClass>();
        unlocked = false;
    }
}

[System.Serializable]
public class CraftingMaterial
{
    public string name;
    public string description;
    public int quantity;
    public int maxStack;
    public Equipment.EquipmentRarity rarity;
    public string source; // "Mine", "Monster", "Quest", etc.

    public CraftingMaterial(string materialName, string materialDescription, Equipment.EquipmentRarity materialRarity = Equipment.EquipmentRarity.Common)
    {
        name = materialName;
        description = materialDescription;
        rarity = materialRarity;
        quantity = 0;
        maxStack = 999;
        source = "Unknown";
    }
}

[System.Serializable]
public class CraftingOrder
{
    public string id;
    public CraftingRecipe recipe;
    public DateTime startTime;
    public DateTime endTime;
    public bool completed;
    public string crafterName;
    public Equipment resultEquipment;

    public CraftingOrder(CraftingRecipe craftingRecipe, string crafter)
    {
        id = Guid.NewGuid().ToString();
        recipe = craftingRecipe;
        crafterName = crafter;
        startTime = DateTime.Now;
        endTime = startTime.AddHours(recipe.craftingTime);
        completed = false;
    }

    public bool IsReady()
    {
        return DateTime.Now >= endTime;
    }

    public TimeSpan TimeRemaining()
    {
        if (completed) return TimeSpan.Zero;
        return endTime - DateTime.Now;
    }
}

public class CraftingSystem : MonoBehaviour
{
    private static CraftingSystem instance;
    public static CraftingSystem Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<CraftingSystem>();
                if (instance == null)
                {
                    GameObject go = new GameObject("CraftingSystem");
                    instance = go.AddComponent<CraftingSystem>();
                    DontDestroyOnLoad(go);
                }
            }
            return instance;
        }
    }

    [Header("Crafting Settings")]
    [SerializeField] private int maxConcurrentOrders = 3;
    [SerializeField] private int baseCraftingTime = 4; // hours
    [SerializeField] private float qualityBonusChance = 0.1f;

    [Header("Materials")]
    [SerializeField] private Dictionary<string, CraftingMaterial> materials;
    
    [Header("Recipes")]
    [SerializeField] private List<CraftingRecipe> availableRecipes;
    [SerializeField] private List<CraftingRecipe> unlockedRecipes;
    
    [Header("Active Orders")]
    [SerializeField] private List<CraftingOrder> activeOrders;
    [SerializeField] private List<CraftingOrder> completedOrders;

    public event Action<CraftingOrder> OnOrderCompleted;
    public event Action<CraftingMaterial> OnMaterialAdded;
    public event Action<CraftingRecipe> OnRecipeUnlocked;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeCraftingSystem();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Update crafting orders every minute
        InvokeRepeating(nameof(UpdateCraftingOrders), 60f, 60f);
    }

    private void InitializeCraftingSystem()
    {
        materials = new Dictionary<string, CraftingMaterial>();
        availableRecipes = new List<CraftingRecipe>();
        unlockedRecipes = new List<CraftingRecipe>();
        activeOrders = new List<CraftingOrder>();
        completedOrders = new List<CraftingOrder>();
        
        InitializeMaterials();
        InitializeRecipes();
    }

    private void InitializeMaterials()
    {
        // Basic materials
        AddMaterial(new CraftingMaterial("Fer", "Minerai de fer basique", Equipment.EquipmentRarity.Common));
        AddMaterial(new CraftingMaterial("Cuir", "Cuir de bête", Equipment.EquipmentRarity.Common));
        AddMaterial(new CraftingMaterial("Bois", "Bois dur", Equipment.EquipmentRarity.Common));
        AddMaterial(new CraftingMaterial("Tissu", "Tissu magique", Equipment.EquipmentRarity.Common));
        
        // Uncommon materials
        AddMaterial(new CraftingMaterial("Acier", "Acier forgé", Equipment.EquipmentRarity.Uncommon));
        AddMaterial(new CraftingMaterial("Cuir Renforcé", "Cuir traité et renforcé", Equipment.EquipmentRarity.Uncommon));
        AddMaterial(new CraftingMaterial("Bois Enchanté", "Bois imprégné de magie", Equipment.EquipmentRarity.Uncommon));
        
        // Rare materials
        AddMaterial(new CraftingMaterial("Mithril", "Métal précieux et léger", Equipment.EquipmentRarity.Rare));
        AddMaterial(new CraftingMaterial("Écaille de Dragon", "Écaille de dragon résistante", Equipment.EquipmentRarity.Rare));
        AddMaterial(new CraftingMaterial("Cristal de Mana", "Cristal saturé de magie", Equipment.EquipmentRarity.Rare));
        
        // Epic materials
        AddMaterial(new CraftingMaterial("Adamantine", "Métal indestructible", Equipment.EquipmentRarity.Epic));
        AddMaterial(new CraftingMaterial("Peau de Démon", "Peau de démon maudit", Equipment.EquipmentRarity.Epic));
        AddMaterial(new CraftingMaterial("Essence Élémentaire", "Essence d'élémental pur", Equipment.EquipmentRarity.Epic));
    }

    private void InitializeRecipes()
    {
        // Basic weapon recipes
        CreateWeaponRecipe("Épée de Fer", Equipment.EquipmentRarity.Common, 
            new Dictionary<string, int> { {"Fer", 5}, {"Bois", 2} }, 50, 2);
        CreateWeaponRecipe("Épée d'Acier", Equipment.EquipmentRarity.Uncommon, 
            new Dictionary<string, int> { {"Acier", 3}, {"Cuir", 2} }, 150, 4);
        CreateWeaponRecipe("Épée de Mithril", Equipment.EquipmentRarity.Rare, 
            new Dictionary<string, int> { {"Mithril", 2}, {"Cristal de Mana", 1} }, 500, 8);

        // Basic armor recipes
        CreateArmorRecipe("Armure de Cuir", Equipment.EquipmentRarity.Common, 
            new Dictionary<string, int> { {"Cuir", 8}, {"Fer", 2} }, 100, 3);
        CreateArmorRecipe("Armure de Mailles", Equipment.EquipmentRarity.Uncommon, 
            new Dictionary<string, int> { {"Acier", 6}, {"Cuir Renforcé", 4} }, 250, 6);
        CreateArmorRecipe("Armure de Mithril", Equipment.EquipmentRarity.Rare, 
            new Dictionary<string, int> { {"Mithril", 4}, {"Écaille de Dragon", 2} }, 800, 12);

        // Magic items
        CreateMagicRecipe("Bâton de Mage", Equipment.EquipmentRarity.Uncommon, 
            new Dictionary<string, int> { {"Bois Enchanté", 3}, {"Cristal de Mana", 2} }, 200, 5);
        CreateMagicRecipe("Orbe de Pouvoir", Equipment.EquipmentRarity.Rare, 
            new Dictionary<string, int> { {"Cristal de Mana", 3}, {"Essence Élémentaire", 1} }, 600, 10);

        // Unlock basic recipes
        UnlockRecipe("Épée de Fer");
        UnlockRecipe("Armure de Cuir");
        UnlockRecipe("Bâton de Mage");
    }

    private void CreateWeaponRecipe(string name, Equipment.EquipmentRarity rarity, Dictionary<string, int> materials, int cost, int time)
    {
        var recipe = new CraftingRecipe
        {
            name = name,
            description = $"Fabriquer une {name.ToLower()}",
            outputType = Equipment.EquipmentType.Weapon,
            outputRarity = rarity,
            requiredMaterials = materials,
            goldCost = cost,
            craftingTime = time,
            requiredCrafterLevel = GetRequiredLevel(rarity)
        };
        availableRecipes.Add(recipe);
    }

    private void CreateArmorRecipe(string name, Equipment.EquipmentRarity rarity, Dictionary<string, int> materials, int cost, int time)
    {
        var recipe = new CraftingRecipe
        {
            name = name,
            description = $"Fabriquer une {name.ToLower()}",
            outputType = Equipment.EquipmentType.Armor,
            outputRarity = rarity,
            requiredMaterials = materials,
            goldCost = cost,
            craftingTime = time,
            requiredCrafterLevel = GetRequiredLevel(rarity)
        };
        availableRecipes.Add(recipe);
    }

    private void CreateMagicRecipe(string name, Equipment.EquipmentRarity rarity, Dictionary<string, int> materials, int cost, int time)
    {
        var recipe = new CraftingRecipe
        {
            name = name,
            description = $"Fabriquer un {name.ToLower()}",
            outputType = Equipment.EquipmentType.Accessory,
            outputRarity = rarity,
            requiredMaterials = materials,
            goldCost = cost,
            craftingTime = time,
            requiredCrafterLevel = GetRequiredLevel(rarity)
        };
        availableRecipes.Add(recipe);
    }

    private int GetRequiredLevel(Equipment.EquipmentRarity rarity)
    {
        switch (rarity)
        {
            case Equipment.EquipmentRarity.Common: return 1;
            case Equipment.EquipmentRarity.Uncommon: return 3;
            case Equipment.EquipmentRarity.Rare: return 6;
            case Equipment.EquipmentRarity.Epic: return 10;
            case Equipment.EquipmentRarity.Legendary: return 15;
            case Equipment.EquipmentRarity.Artifact: return 20;
            default: return 1;
        }
    }

    public void AddMaterial(CraftingMaterial material)
    {
        if (materials.ContainsKey(material.name))
        {
            materials[material.name].quantity += material.quantity;
        }
        else
        {
            materials[material.name] = material;
        }
        
        OnMaterialAdded?.Invoke(material);
    }

    public void AddMaterial(string materialName, int quantity)
    {
        if (materials.ContainsKey(materialName))
        {
            materials[materialName].quantity += quantity;
            OnMaterialAdded?.Invoke(materials[materialName]);
        }
    }

    public int GetMaterialQuantity(string materialName)
    {
        return materials.ContainsKey(materialName) ? materials[materialName].quantity : 0;
    }

    public bool HasMaterials(Dictionary<string, int> requiredMaterials)
    {
        foreach (var requirement in requiredMaterials)
        {
            if (GetMaterialQuantity(requirement.Key) < requirement.Value)
                return false;
        }
        return true;
    }

    public bool CanCraftRecipe(CraftingRecipe recipe)
    {
        if (!recipe.unlocked) return false;
        if (activeOrders.Count >= maxConcurrentOrders) return false;
        if (!HasMaterials(recipe.requiredMaterials)) return false;
        if (!ResourceManager.Instance.HasResource(Resource.ResourceType.Gold, recipe.goldCost)) return false;
        
        return true;
    }

    public bool StartCrafting(CraftingRecipe recipe, string crafterName = "Artisan")
    {
        if (!CanCraftRecipe(recipe)) return false;

        // Consume materials
        foreach (var requirement in recipe.requiredMaterials)
        {
            materials[requirement.Key].quantity -= requirement.Value;
        }

        // Consume gold
        ResourceManager.Instance.RemoveResource(Resource.ResourceType.Gold, recipe.goldCost);

        // Create crafting order
        var order = new CraftingOrder(recipe, crafterName);
        activeOrders.Add(order);

        Debug.Log($"Crafting started: {recipe.name} by {crafterName}");
        return true;
    }

    public void UpdateCraftingOrders()
    {
        for (int i = activeOrders.Count - 1; i >= 0; i--)
        {
            var order = activeOrders[i];
            if (order.IsReady() && !order.completed)
            {
                CompleteCraftingOrder(order);
            }
        }
    }

    private void CompleteCraftingOrder(CraftingOrder order)
    {
        order.completed = true;
        
        // Generate the crafted equipment
        Equipment craftedItem = ItemDatabase.Instance.GenerateRandomEquipment(
            order.recipe.outputType, 
            order.recipe.outputRarity, 
            1
        );

        // Apply quality bonus chance
        if (UnityEngine.Random.Range(0f, 1f) < qualityBonusChance)
        {
            ApplyQualityBonus(craftedItem);
        }

        order.resultEquipment = craftedItem;
        
        // Move to completed orders
        activeOrders.Remove(order);
        completedOrders.Add(order);
        
        OnOrderCompleted?.Invoke(order);
        Debug.Log($"Crafting completed: {craftedItem.Name} by {order.crafterName}");
    }

    private void ApplyQualityBonus(Equipment equipment)
    {
        // Increase all stats by 10-20%
        float bonus = UnityEngine.Random.Range(0.1f, 0.2f);
        var statsToModify = new Dictionary<string, int>(equipment.StatBonuses);
        
        foreach (var stat in statsToModify)
        {
            int bonusValue = Mathf.RoundToInt(stat.Value * bonus);
            equipment.StatBonuses[stat.Key] = stat.Value + bonusValue;
        }
        
        Debug.Log($"Quality bonus applied to {equipment.Name}!");
    }

    public void UnlockRecipe(string recipeName)
    {
        var recipe = availableRecipes.FirstOrDefault(r => r.name == recipeName);
        if (recipe != null && !recipe.unlocked)
        {
            recipe.unlocked = true;
            unlockedRecipes.Add(recipe);
            OnRecipeUnlocked?.Invoke(recipe);
            Debug.Log($"Recipe unlocked: {recipeName}");
        }
    }

    public List<CraftingRecipe> GetUnlockedRecipes()
    {
        return unlockedRecipes.ToList();
    }

    public List<CraftingOrder> GetActiveOrders()
    {
        return activeOrders.ToList();
    }

    public List<CraftingOrder> GetCompletedOrders()
    {
        return completedOrders.ToList();
    }

    public Dictionary<string, CraftingMaterial> GetMaterials()
    {
        return new Dictionary<string, CraftingMaterial>(materials);
    }

    public void CollectCompletedItem(string orderId)
    {
        var order = completedOrders.FirstOrDefault(o => o.id == orderId);
        if (order != null && order.resultEquipment != null)
        {
            // Add to inventory (this would need an inventory system)
            // For now, we'll just log it
            Debug.Log($"Collected crafted item: {order.resultEquipment.Name}");
            completedOrders.Remove(order);
        }
    }

    // Methods for integration with mission system
    public void ProcessMissionLoot(List<string> lootMaterials)
    {
        foreach (string materialName in lootMaterials)
        {
            AddMaterial(materialName, UnityEngine.Random.Range(1, 4));
        }
    }

    public List<string> GetPossibleMissionLoot(Equipment.EquipmentRarity missionDifficulty)
    {
        var possibleLoot = new List<string>();
        
        foreach (var material in materials.Values)
        {
            if (material.rarity <= missionDifficulty)
            {
                possibleLoot.Add(material.name);
            }
        }
        
        return possibleLoot;
    }
}