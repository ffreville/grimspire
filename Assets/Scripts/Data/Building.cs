using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Building
{
    [Header("Basic Information")]
    public string name;
    public string description;
    public BuildingType buildingType;
    public BuildingCategory category;
    public int level;
    public int maxLevel;
    
    [Header("Construction")]
    public bool isBuilt;
    public bool isUnderConstruction;
    public int constructionTimeRemaining;
    
    [Header("Costs")]
    public int goldCost;
    public int woodCost;
    public int stoneCost;
    public int ironCost;
    public int magicCost;
    
    [Header("Upgrade Costs")]
    public int upgradeGoldCost;
    public int upgradeWoodCost;
    public int upgradeStoneCost;
    public int upgradeIronCost;
    public int upgradeMagicCost;
    
    [Header("Effects")]
    public int populationBonus;
    public int maxPopulationBonus;
    public int goldPerDay;
    public int resourcePerDay;
    public int actionPointsBonus;
    public int reputationBonus;
    
    [Header("Requirements")]
    public int populationRequired;
    public int reputationRequired;
    public List<string> prerequisiteBuildings;

    public Building()
    {
        name = "Bâtiment";
        description = "Description du bâtiment";
        buildingType = BuildingType.House;
        category = BuildingCategory.Residential;
        level = 1;
        maxLevel = 5;
        
        isBuilt = false;
        isUnderConstruction = false;
        constructionTimeRemaining = 0;
        
        goldCost = 100;
        woodCost = 10;
        stoneCost = 5;
        ironCost = 0;
        magicCost = 0;
        
        upgradeGoldCost = 200;
        upgradeWoodCost = 20;
        upgradeStoneCost = 10;
        upgradeIronCost = 5;
        upgradeMagicCost = 0;
        
        populationBonus = 0;
        maxPopulationBonus = 0;
        goldPerDay = 0;
        resourcePerDay = 0;
        actionPointsBonus = 0;
        reputationBonus = 0;
        
        populationRequired = 0;
        reputationRequired = 0;
        prerequisiteBuildings = new List<string>();
    }

    public Building(BuildingType type)
    {
        buildingType = type;
        category = GetCategoryForType(type);
        level = 1;
        maxLevel = 5;
        
        isBuilt = false;
        isUnderConstruction = false;
        constructionTimeRemaining = 0;
        
        prerequisiteBuildings = new List<string>();
        
        SetupBuildingData(type);
    }

    private BuildingCategory GetCategoryForType(BuildingType type)
    {
        switch (type)
        {
            case BuildingType.House:
            case BuildingType.Inn:
            case BuildingType.Tavern:
                return BuildingCategory.Residential;
                
            case BuildingType.Market:
            case BuildingType.Shop:
            case BuildingType.Bank:
                return BuildingCategory.Commercial;
                
            case BuildingType.Forge:
            case BuildingType.Alchemist:
            case BuildingType.Enchanter:
                return BuildingCategory.Industrial;
                
            case BuildingType.TownHall:
            case BuildingType.Prison:
                return BuildingCategory.Administrative;
                
            default:
                return BuildingCategory.Residential;
        }
    }

    private void SetupBuildingData(BuildingType type)
    {
        switch (type)
        {
            case BuildingType.House:
                name = "Maison";
                description = "Logement pour les citoyens";
                goldCost = 100;
                woodCost = 15;
                stoneCost = 5;
                populationBonus = 5;
                maxPopulationBonus = 5;
                break;

            case BuildingType.Inn:
                name = "Auberge";
                description = "Attire les voyageurs et aventuriers";
                goldCost = 300;
                woodCost = 25;
                stoneCost = 15;
                populationBonus = 3;
                reputationBonus = 2;
                goldPerDay = 20;
                break;

            case BuildingType.Tavern:
                name = "Taverne";
                description = "Lieu de rassemblement, améliore le moral";
                goldCost = 250;
                woodCost = 20;
                stoneCost = 10;
                populationBonus = 2;
                reputationBonus = 3;
                goldPerDay = 15;
                break;

            case BuildingType.Market:
                name = "Marché";
                description = "Centre commercial, génère de l'or";
                goldCost = 500;
                woodCost = 30;
                stoneCost = 20;
                ironCost = 5;
                goldPerDay = 50;
                reputationBonus = 1;
                populationRequired = 50;
                break;

            case BuildingType.Shop:
                name = "Magasin";
                description = "Magasin génère un peu d'or";
                goldCost = 500;
                woodCost = 30;
                stoneCost = 20;
                ironCost = 5;
                goldPerDay = 50;
                reputationBonus = 1;
                populationRequired = 50;
                break;

            case BuildingType.Bank:
                name = "Banque";
                description = "A voir ce que ça fait";
                goldCost = 500;
                woodCost = 30;
                stoneCost = 20;
                ironCost = 5;
                goldPerDay = 50;
                reputationBonus = 1;
                populationRequired = 50;
                break;

            case BuildingType.Forge:
                name = "Forge";
                description = "Produit des armes et armures";
                goldCost = 800;
                woodCost = 20;
                stoneCost = 40;
                ironCost = 20;
                resourcePerDay = 5;
                populationRequired = 30;
                break;


            case BuildingType.Alchemist:
                name = "Alchémie";
                description = "Produit des potions ";
                goldCost = 800;
                woodCost = 20;
                stoneCost = 40;
                ironCost = 20;
                resourcePerDay = 5;
                populationRequired = 30;
                break;

            case BuildingType.Enchanter:
                name = "Enchanteur";
                description = "Produit des parchemins ";
                goldCost = 800;
                woodCost = 20;
                stoneCost = 40;
                ironCost = 20;
                resourcePerDay = 5;
                populationRequired = 30;
                break;

            case BuildingType.TownHall:
                name = "Hôtel de Ville";
                description = "Centre administratif, augmente les points d'action";
                goldCost = 1000;
                woodCost = 50;
                stoneCost = 50;
                ironCost = 20;
                actionPointsBonus = 1;
                reputationBonus = 5;
                populationRequired = 100;
                break;

            case BuildingType.Prison:
                name = "Prison";
                description = "Prison";
                goldCost = 1000;
                woodCost = 50;
                stoneCost = 50;
                ironCost = 20;
                actionPointsBonus = 1;
                reputationBonus = 5;
                populationRequired = 100;
                break;
        }
        
        SetupUpgradeCosts();
    }

    private void SetupUpgradeCosts()
    {
        upgradeGoldCost = Mathf.RoundToInt(goldCost * 1.5f);
        upgradeWoodCost = Mathf.RoundToInt(woodCost * 1.5f);
        upgradeStoneCost = Mathf.RoundToInt(stoneCost * 1.5f);
        upgradeIronCost = Mathf.RoundToInt(ironCost * 1.5f);
        upgradeMagicCost = Mathf.RoundToInt(magicCost * 1.5f);
    }

    public bool CanBuild(City city)
    {
        /*if (isBuilt || isUnderConstruction) return false;
        
        if (!city.HasEnoughResources(goldCost, woodCost, stoneCost, ironCost, magicCost))
            return false;
            
        if (city.population < populationRequired)
            return false;
            
        if (city.reputation < reputationRequired)
            return false;
            
        foreach (string prerequisite in prerequisiteBuildings)
        {
            bool hasPrerequisite = city.buildings.Exists(b => b.name == prerequisite && b.isBuilt);
            if (!hasPrerequisite) return false;
        }
        */
        return true;
    }

    public bool CanUpgrade(City city)
    {
        if (!isBuilt || isUnderConstruction || level >= maxLevel) return false;
        
        return city.HasEnoughResources(upgradeGoldCost, upgradeWoodCost, upgradeStoneCost, upgradeIronCost, upgradeMagicCost);
    }

    public void StartConstruction()
    {
        isUnderConstruction = true;
        constructionTimeRemaining = 1; // 1 jour de construction
    }

    public void CompleteConstruction()
    {
        isBuilt = true;
        isUnderConstruction = false;
        constructionTimeRemaining = 0;
    }

    public void ProcessConstruction()
    {
        if (isUnderConstruction && constructionTimeRemaining > 0)
        {
            constructionTimeRemaining--;
            if (constructionTimeRemaining == 0)
            {
                CompleteConstruction();
            }
        }
    }

    public void Upgrade()
    {
        if (level < maxLevel)
        {
            level++;
            
            // Amélioration des effets
            populationBonus = Mathf.RoundToInt(populationBonus * 1.2f);
            maxPopulationBonus = Mathf.RoundToInt(maxPopulationBonus * 1.2f);
            goldPerDay = Mathf.RoundToInt(goldPerDay * 1.3f);
            resourcePerDay = Mathf.RoundToInt(resourcePerDay * 1.3f);
            reputationBonus = Mathf.RoundToInt(reputationBonus * 1.1f);
            
            SetupUpgradeCosts();
        }
    }

    public int GetTotalEffect(string effectType)
    {
        if (!isBuilt) return 0;
        
        float levelMultiplier = 1f + (level - 1) * 0.2f;
        
        switch (effectType)
        {
            case "population":
                return Mathf.RoundToInt(populationBonus * levelMultiplier);
            case "maxPopulation":
                return Mathf.RoundToInt(maxPopulationBonus * levelMultiplier);
            case "gold":
                return Mathf.RoundToInt(goldPerDay * levelMultiplier);
            case "resource":
                return Mathf.RoundToInt(resourcePerDay * levelMultiplier);
            case "reputation":
                return Mathf.RoundToInt(reputationBonus * levelMultiplier);
            case "actionPoints":
                return actionPointsBonus;
            default:
                return 0;
        }
    }
}
