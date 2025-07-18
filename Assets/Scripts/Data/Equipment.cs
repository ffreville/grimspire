using UnityEngine;

[System.Serializable]
public class Equipment
{
    [Header("Basic Information")]
    public string name;
    public string description;
    public EquipmentType equipmentType;
    public EquipmentRarity rarity;
    public int level;
    
    [Header("Stats")]
    public int power;
    public int strengthBonus;
    public int intelligenceBonus;
    public int agilityBonus;
    public int charismaBonus;
    public int luckBonus;
    
    [Header("Requirements")]
    public AdventurerClass requiredClass;
    public int requiredLevel;
    
    [Header("Crafting")]
    public int craftingCost;
    public int ironRequired;
    public int leatherRequired;
    public int gemsRequired;
    public int magicCrystalRequired;

    public Equipment()
    {
        name = "Équipement";
        description = "Description de l'équipement";
        equipmentType = EquipmentType.Weapon;
        rarity = EquipmentRarity.Common;
        level = 1;
        
        power = 10;
        strengthBonus = 0;
        intelligenceBonus = 0;
        agilityBonus = 0;
        charismaBonus = 0;
        luckBonus = 0;
        
        requiredClass = AdventurerClass.Warrior;
        requiredLevel = 1;
        
        craftingCost = 100;
        ironRequired = 5;
        leatherRequired = 0;
        gemsRequired = 0;
        magicCrystalRequired = 0;
    }

    public Equipment(string itemName, EquipmentType type, EquipmentRarity itemRarity, int itemLevel)
    {
        name = itemName;
        equipmentType = type;
        rarity = itemRarity;
        level = itemLevel;
        
        SetupEquipmentStats();
        SetupCraftingRequirements();
    }

    private void SetupEquipmentStats()
    {
        int basePower = 10 + (level * 5);
        float rarityMultiplier = GetRarityMultiplier();
        
        power = Mathf.RoundToInt(basePower * rarityMultiplier);
        
        switch (equipmentType)
        {
            case EquipmentType.Weapon:
                SetupWeaponStats();
                break;
            case EquipmentType.Armor:
                SetupArmorStats();
                break;
            case EquipmentType.Accessory:
                SetupAccessoryStats();
                break;
        }
        
        ApplyRarityBonuses();
    }

    private void SetupWeaponStats()
    {
        strengthBonus = Mathf.RoundToInt(level * 2 * GetRarityMultiplier());
        agilityBonus = Mathf.RoundToInt(level * 1 * GetRarityMultiplier());
        
        if (name.Contains("Bâton") || name.Contains("Grimoire"))
        {
            intelligenceBonus = Mathf.RoundToInt(level * 3 * GetRarityMultiplier());
            strengthBonus = Mathf.RoundToInt(level * 0.5f * GetRarityMultiplier());
        }
        else if (name.Contains("Dague") || name.Contains("Arc"))
        {
            agilityBonus = Mathf.RoundToInt(level * 3 * GetRarityMultiplier());
            strengthBonus = Mathf.RoundToInt(level * 1.5f * GetRarityMultiplier());
        }
    }

    private void SetupArmorStats()
    {
        strengthBonus = Mathf.RoundToInt(level * 1 * GetRarityMultiplier());
        agilityBonus = Mathf.RoundToInt(level * 0.5f * GetRarityMultiplier());
        
        if (name.Contains("Robe"))
        {
            intelligenceBonus = Mathf.RoundToInt(level * 2 * GetRarityMultiplier());
            charismaBonus = Mathf.RoundToInt(level * 1 * GetRarityMultiplier());
        }
        else if (name.Contains("Cuir"))
        {
            agilityBonus = Mathf.RoundToInt(level * 2 * GetRarityMultiplier());
        }
    }

    private void SetupAccessoryStats()
    {
        luckBonus = Mathf.RoundToInt(level * 1.5f * GetRarityMultiplier());
        
        switch (Random.Range(0, 4))
        {
            case 0:
                strengthBonus = Mathf.RoundToInt(level * 1 * GetRarityMultiplier());
                break;
            case 1:
                intelligenceBonus = Mathf.RoundToInt(level * 1 * GetRarityMultiplier());
                break;
            case 2:
                agilityBonus = Mathf.RoundToInt(level * 1 * GetRarityMultiplier());
                break;
            case 3:
                charismaBonus = Mathf.RoundToInt(level * 1 * GetRarityMultiplier());
                break;
        }
    }

    private float GetRarityMultiplier()
    {
        switch (rarity)
        {
            case EquipmentRarity.Common:
                return 1f;
            case EquipmentRarity.Uncommon:
                return 1.3f;
            case EquipmentRarity.Rare:
                return 1.6f;
            case EquipmentRarity.Epic:
                return 2f;
            case EquipmentRarity.Legendary:
                return 2.5f;
            default:
                return 1f;
        }
    }

    private void ApplyRarityBonuses()
    {
        switch (rarity)
        {
            case EquipmentRarity.Uncommon:
                AddRandomBonus(1);
                break;
            case EquipmentRarity.Rare:
                AddRandomBonus(2);
                break;
            case EquipmentRarity.Epic:
                AddRandomBonus(3);
                break;
            case EquipmentRarity.Legendary:
                AddRandomBonus(4);
                break;
        }
    }

    private void AddRandomBonus(int bonusCount)
    {
        for (int i = 0; i < bonusCount; i++)
        {
            switch (Random.Range(0, 5))
            {
                case 0:
                    strengthBonus += Random.Range(1, 4);
                    break;
                case 1:
                    intelligenceBonus += Random.Range(1, 4);
                    break;
                case 2:
                    agilityBonus += Random.Range(1, 4);
                    break;
                case 3:
                    charismaBonus += Random.Range(1, 4);
                    break;
                case 4:
                    luckBonus += Random.Range(1, 4);
                    break;
            }
        }
    }

    private void SetupCraftingRequirements()
    {
        craftingCost = Mathf.RoundToInt(50 * level * GetRarityMultiplier());
        
        switch (equipmentType)
        {
            case EquipmentType.Weapon:
                ironRequired = Mathf.RoundToInt(3 * level * GetRarityMultiplier());
                leatherRequired = Mathf.RoundToInt(1 * level);
                break;
            case EquipmentType.Armor:
                ironRequired = Mathf.RoundToInt(2 * level * GetRarityMultiplier());
                leatherRequired = Mathf.RoundToInt(3 * level * GetRarityMultiplier());
                break;
            case EquipmentType.Accessory:
                gemsRequired = Mathf.RoundToInt(1 * level);
                magicCrystalRequired = Mathf.RoundToInt(1 * level);
                break;
        }
        
        if (rarity >= EquipmentRarity.Rare)
        {
            gemsRequired += Random.Range(1, 3);
        }
        
        if (rarity >= EquipmentRarity.Epic)
        {
            magicCrystalRequired += Random.Range(1, 3);
        }
    }

    public bool CanEquip(Adventurer adventurer)
    {
        if (adventurer.level < requiredLevel)
            return false;
            
        if (requiredClass != AdventurerClass.Warrior && adventurer.adventurerClass != requiredClass)
            return false;
            
        return true;
    }

    public int GetTotalStatBonus()
    {
        return strengthBonus + intelligenceBonus + agilityBonus + charismaBonus + luckBonus;
    }

    public string GetRarityText()
    {
        switch (rarity)
        {
            case EquipmentRarity.Common:
                return "Commun";
            case EquipmentRarity.Uncommon:
                return "Peu Commun";
            case EquipmentRarity.Rare:
                return "Rare";
            case EquipmentRarity.Epic:
                return "Épique";
            case EquipmentRarity.Legendary:
                return "Légendaire";
            default:
                return "Commun";
        }
    }

    public Color GetRarityColor()
    {
        switch (rarity)
        {
            case EquipmentRarity.Common:
                return Color.gray;
            case EquipmentRarity.Uncommon:
                return Color.green;
            case EquipmentRarity.Rare:
                return Color.blue;
            case EquipmentRarity.Epic:
                return Color.magenta;
            case EquipmentRarity.Legendary:
                return Color.yellow;
            default:
                return Color.gray;
        }
    }

    public string GetDetailedDescription()
    {
        string details = $"{name} - {GetRarityText()}\n";
        details += $"Niveau: {level}\n";
        details += $"Puissance: {power}\n\n";
        
        if (strengthBonus > 0) details += $"Force: +{strengthBonus}\n";
        if (intelligenceBonus > 0) details += $"Intelligence: +{intelligenceBonus}\n";
        if (agilityBonus > 0) details += $"Agilité: +{agilityBonus}\n";
        if (charismaBonus > 0) details += $"Charisme: +{charismaBonus}\n";
        if (luckBonus > 0) details += $"Chance: +{luckBonus}\n";
        
        details += $"\nRequis: Niveau {requiredLevel}";
        if (requiredClass != AdventurerClass.Warrior)
            details += $", {requiredClass}";
            
        return details;
    }
}