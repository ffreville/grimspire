using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BuildingMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject buildingListContainer;
    [SerializeField] private GameObject buildingItemPrefab;
    [SerializeField] private ScrollRect scrollRect;
    
    [Header("Current Data")]
    [SerializeField] private City currentCity;
    
    public City CurrentCity => currentCity;
    [SerializeField] private List<Building> availableBuildings;
    [SerializeField] private List<GameObject> buildingUIItems;

    private void Awake()
    {
        availableBuildings = new List<Building>();
        buildingUIItems = new List<GameObject>();
        InitializeAvailableBuildings();
    }

    private void OnEnable()
    {
        // Get city from parent GameMenu if not set
        if (currentCity == null)
        {
            GameMenu gameMenu = GetComponentInParent<GameMenu>();
            if (gameMenu != null)
            {
                currentCity = gameMenu.GetCurrentCity();
            }
        }
        
        //RefreshBuildingList();
    }

    private void InitializeAvailableBuildings()
    {
        availableBuildings.Clear();
        
        // Add all building types
        foreach (BuildingType buildingType in System.Enum.GetValues(typeof(BuildingType)))
        {
            Debug.Log("Creating : " + buildingType);
            Building building = new(buildingType);
            availableBuildings.Add(building);
        }
    }

    public void SetCity(City city)
    {
        currentCity = city;
    }

    public void RefreshBuildingList()
    {
        if (buildingListContainer == null) return;

        ClearBuildingList();
        
        if (currentCity == null) 
        {
            Debug.Log("CurrentCity is null in BuildingMenu");
            return;
        }

        Debug.Log($"Refreshing building list. City has {currentCity.buildings.Count} buildings");

        // Show built buildings first
        foreach (Building building in currentCity.buildings)
        {
            if (building.isBuilt)
            {
                Debug.Log($"Adding built building: {building.name}");
                CreateBuildingItem(building, true);
            }
        }

        // Show buildings under construction
        foreach (Building building in currentCity.buildings)
        {
            if (building.isUnderConstruction)
            {
                Debug.Log($"Adding building under construction: {building.name}");
                CreateBuildingItem(building, false);
            }
        }

        // Show available buildings to build (only if not already built or under construction)
        foreach (Building building in availableBuildings)
        {
        
            
            if ( building.CanBuild(currentCity))
            {
                Debug.Log($"Adding available building: {building.name}");
                CreateBuildingItem(building, false);
            }
        }
        
        Debug.Log($"Total building items created: {buildingUIItems.Count}");
    }

    private void ClearBuildingList()
    {
        foreach (GameObject item in buildingUIItems)
        {
            if (item != null)
            {
                DestroyImmediate(item);
            }
        }
        buildingUIItems.Clear();
    }

    private void CreateBuildingItem(Building building, bool isBuilt)
    {
        if (buildingItemPrefab == null) 
        {
            Debug.LogError("BuildingItemPrefab is null! Cannot create building item.");
            return;
        }

        if (buildingListContainer == null)
        {
            Debug.LogError("BuildingListContainer is null! Cannot create building item.");
            return;
        }

        GameObject itemGO = Instantiate(buildingItemPrefab, buildingListContainer.transform);
        itemGO.SetActive(true);
        buildingUIItems.Add(itemGO);

        BuildingItem buildingItem = itemGO.GetComponent<BuildingItem>();
        if (buildingItem != null)
        {
            buildingItem.Setup(building, isBuilt, this);
            Debug.Log($"Created building item for: {building.name}");
        }
        else
        {
            Debug.LogError($"BuildingItem component not found on prefab for building: {building.name}");
        }
    }

    public void OnBuildingClicked(Building building)
    {
        if (currentCity == null) return;

        if (building.isBuilt)
        {
            ShowBuildingDetails(building);
        }
        else if (building.isUnderConstruction)
        {
            ShowConstructionProgress(building);
        }
        else if (building.CanBuild(currentCity))
        {
            StartConstruction(building);
        }
    }

    private void ShowBuildingDetails(Building building)
    {
        string details = $"=== {building.name} ===\n";
        details += $"Niveau: {building.level}/{building.maxLevel}\n";
        details += $"Description: {building.description}\n\n";
        
        if (building.populationBonus > 0)
            details += $"Population: +{building.GetTotalEffect("population")}\n";
        if (building.maxPopulationBonus > 0)
            details += $"Population Max: +{building.GetTotalEffect("maxPopulation")}\n";
        if (building.goldPerDay > 0)
            details += $"Or par jour: +{building.GetTotalEffect("gold")}\n";
        if (building.reputationBonus > 0)
            details += $"Réputation: +{building.GetTotalEffect("reputation")}\n";
        if (building.actionPointsBonus > 0)
            details += $"Points d'action: +{building.actionPointsBonus}\n";

        if (building.CanUpgrade(currentCity))
        {
            details += $"\n=== Amélioration Disponible ===\n";
            details += $"Coût: {building.upgradeGoldCost} or";
            if (building.upgradeWoodCost > 0) details += $", {building.upgradeWoodCost} bois";
            if (building.upgradeStoneCost > 0) details += $", {building.upgradeStoneCost} pierre";
            if (building.upgradeIronCost > 0) details += $", {building.upgradeIronCost} fer";
            if (building.upgradeMagicCost > 0) details += $", {building.upgradeMagicCost} cristaux";
        }

        Debug.Log(details);
    }

    private void ShowConstructionProgress(Building building)
    {
        string progress = $"=== Construction en cours ===\n";
        progress += $"{building.name}\n";
        progress += $"Temps restant: {building.constructionTimeRemaining} jour(s)\n";
        
        Debug.Log(progress);
    }

    private void StartConstruction(Building building)
    {
        if (!currentCity.CanPerformAction())
        {
            Debug.Log("Pas assez de points d'action pour construire!");
            return;
        }

        if (!building.CanBuild(currentCity))
        {
            Debug.Log("Impossible de construire ce bâtiment!");
            return;
        }

        // Spend resources
        currentCity.SpendResources(building.goldCost, building.woodCost, building.stoneCost, 
                                 building.ironCost, building.magicCost);
        
        // Spend action point
        currentCity.SpendActionPoint();
        
        // Add building to city and start construction
        Building newBuilding = new(building.buildingType);
        newBuilding.StartConstruction();
        currentCity.buildings.Add(newBuilding);
        
        Debug.Log($"Construction de {building.name} commencée!");
        RefreshBuildingList();
    }

    public void OnUpgradeClicked(Building building)
    {
        if (currentCity == null || !building.CanUpgrade(currentCity)) return;

        if (!currentCity.CanPerformAction())
        {
            Debug.Log("Pas assez de points d'action pour améliorer!");
            return;
        }

        // Spend resources
        currentCity.SpendResources(building.upgradeGoldCost, building.upgradeWoodCost, 
                                 building.upgradeStoneCost, building.upgradeIronCost, 
                                 building.upgradeMagicCost);
        
        // Spend action point
        currentCity.SpendActionPoint();
        
        // Upgrade building
        building.Upgrade();
        
        Debug.Log($"{building.name} amélioré au niveau {building.level}!");
        RefreshBuildingList();
    }
}
