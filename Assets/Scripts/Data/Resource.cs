using UnityEngine;

[System.Serializable]
public class Resource
{
    [Header("Basic Information")]
    public string name;
    public string description;
    public ResourceType resourceType;
    public int amount;
    public int maxAmount;
    
    [Header("Value")]
    public int baseValue;
    public int currentMarketValue;
    public bool isTradeGood;
    
    [Header("Production")]
    public int dailyProduction;
    public int dailyConsumption;
    public bool isRenewable;

    public Resource()
    {
        name = "Ressource";
        description = "Description de la ressource";
        resourceType = ResourceType.Gold;
        amount = 0;
        maxAmount = 1000;
        
        baseValue = 1;
        currentMarketValue = 1;
        isTradeGood = true;
        
        dailyProduction = 0;
        dailyConsumption = 0;
        isRenewable = false;
    }

    public Resource(ResourceType type, int initialAmount = 0)
    {
        resourceType = type;
        amount = initialAmount;
        SetupResourceData(type);
    }

    private void SetupResourceData(ResourceType type)
    {
        switch (type)
        {
            case ResourceType.Gold:
                name = "Or";
                description = "Monnaie principale du royaume";
                maxAmount = 999999;
                baseValue = 1;
                currentMarketValue = 1;
                isTradeGood = true;
                isRenewable = true;
                break;
                
            case ResourceType.Wood:
                name = "Bois";
                description = "Matériau de construction de base";
                maxAmount = 500;
                baseValue = 2;
                currentMarketValue = 2;
                isTradeGood = true;
                isRenewable = true;
                break;
                
            case ResourceType.Stone:
                name = "Pierre";
                description = "Matériau de construction solide";
                maxAmount = 300;
                baseValue = 5;
                currentMarketValue = 5;
                isTradeGood = true;
                isRenewable = false;
                break;
                
            case ResourceType.Iron:
                name = "Fer";
                description = "Métal pour armes et outils";
                maxAmount = 200;
                baseValue = 10;
                currentMarketValue = 10;
                isTradeGood = true;
                isRenewable = false;
                break;
                
            case ResourceType.MagicCrystals:
                name = "Cristaux Magiques";
                description = "Énergie mystique rare";
                maxAmount = 100;
                baseValue = 50;
                currentMarketValue = 50;
                isTradeGood = true;
                isRenewable = false;
                break;
                
            case ResourceType.Food:
                name = "Nourriture";
                description = "Nécessaire à la survie de la population";
                maxAmount = 200;
                baseValue = 3;
                currentMarketValue = 3;
                isTradeGood = true;
                isRenewable = true;
                dailyConsumption = 1;
                break;
                
            case ResourceType.Leather:
                name = "Cuir";
                description = "Matériau pour armures légères";
                maxAmount = 150;
                baseValue = 8;
                currentMarketValue = 8;
                isTradeGood = true;
                isRenewable = true;
                break;
                
            case ResourceType.Gems:
                name = "Gemmes";
                description = "Pierres précieuses pour enchantements";
                maxAmount = 50;
                baseValue = 100;
                currentMarketValue = 100;
                isTradeGood = true;
                isRenewable = false;
                break;
                
            case ResourceType.Population:
                name = "Population";
                description = "Habitants de la cité";
                maxAmount = 1000;
                baseValue = 0;
                currentMarketValue = 0;
                isTradeGood = false;
                isRenewable = true;
                dailyProduction = 1;
                break;
                
            case ResourceType.Materials:
                name = "Matériaux";
                description = "Matériaux de construction combinés";
                maxAmount = 1000;
                baseValue = 5;
                currentMarketValue = 5;
                isTradeGood = true;
                isRenewable = false;
                break;
        }
    }

    public bool CanAdd(int addAmount)
    {
        return amount + addAmount <= maxAmount;
    }

    public bool CanRemove(int removeAmount)
    {
        return amount >= removeAmount;
    }

    public bool Add(int addAmount)
    {
        if (CanAdd(addAmount))
        {
            amount += addAmount;
            return true;
        }
        return false;
    }

    public bool Remove(int removeAmount)
    {
        if (CanRemove(removeAmount))
        {
            amount -= removeAmount;
            return true;
        }
        return false;
    }

    public int GetAvailableSpace()
    {
        return maxAmount - amount;
    }

    public float GetFillPercentage()
    {
        return (float)amount / maxAmount;
    }

    public int GetTotalValue()
    {
        return amount * currentMarketValue;
    }

    public void UpdateMarketValue(float marketModifier)
    {
        currentMarketValue = Mathf.RoundToInt(baseValue * marketModifier);
        currentMarketValue = Mathf.Max(1, currentMarketValue);
    }

    public void ProcessDailyProduction()
    {
        if (isRenewable && dailyProduction > 0)
        {
            Add(dailyProduction);
        }
    }

    public void ProcessDailyConsumption()
    {
        if (dailyConsumption > 0)
        {
            Remove(dailyConsumption);
        }
    }

    public string GetStatusText()
    {
        string status = $"{name}: {amount}/{maxAmount}";
        
        if (dailyProduction > 0)
            status += $" (+{dailyProduction}/jour)";
            
        if (dailyConsumption > 0)
            status += $" (-{dailyConsumption}/jour)";
            
        return status;
    }

    public Color GetStatusColor()
    {
        float fillPercentage = GetFillPercentage();
        
        if (fillPercentage < 0.2f)
            return Color.red;
        else if (fillPercentage < 0.5f)
            return Color.yellow;
        else
            return Color.green;
    }
}