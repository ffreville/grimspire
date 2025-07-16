using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ConstructionSystem : MonoBehaviour
{
    public static ConstructionSystem Instance { get; private set; }
    
    [Header("Construction Settings")]
    [SerializeField] private bool instantConstruction = true;
    [SerializeField] private float constructionSpeedMultiplier = 1.0f;
    [SerializeField] private int maxConcurrentConstructions = 3;
    
    [Header("Debug")]
    [SerializeField] private bool debugMode = false;
    
    private List<ConstructionProject> activeProjects;
    private List<Building> constructedBuildings;
    
    public List<ConstructionProject> ActiveProjects => new List<ConstructionProject>(activeProjects);
    public List<Building> ConstructedBuildings => new List<Building>(constructedBuildings);
    public int ConcurrentConstructions => activeProjects.Count;
    public bool CanStartNewConstruction => activeProjects.Count < maxConcurrentConstructions;
    
    // Events
    public static event Action<Building.BuildingType> OnConstructionStarted;
    public static event Action<Building.BuildingType> OnConstructionCompleted;
    public static event Action<Building.BuildingType> OnConstructionCancelled;
    public static event Action<Building.BuildingType, float> OnConstructionProgress;
    public static event Action<Building> OnBuildingUpgraded;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeConstructionSystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeConstructionSystem()
    {
        activeProjects = new List<ConstructionProject>();
        constructedBuildings = new List<Building>();
        
        if (debugMode)
        {
            Debug.Log("ConstructionSystem initialized");
        }
    }

    private void Update()
    {
        if (!instantConstruction)
        {
            ProcessActiveConstructions();
        }
    }

    public bool CanConstructBuilding(Building.BuildingType buildingType, out string reason)
    {
        reason = "";
        
        // Check if building data exists
        BuildingData data = BuildingDB.GetBuilding(buildingType);
        if (data == null)
        {
            reason = "Données de bâtiment non trouvées";
            return false;
        }
        
        // Check city level requirement
        if (GameManager.Instance != null && GameManager.Instance.CurrentCity != null)
        {
            int cityLevel = GameManager.Instance.CurrentCity.CityLevel;
            if (cityLevel < data.requiredCityLevel)
            {
                reason = $"Niveau de cité requis: {data.requiredCityLevel}";
                return false;
            }
        }
        
        // Check if already under construction
        if (IsUnderConstruction(buildingType))
        {
            reason = "Déjà en construction";
            return false;
        }
        
        // Check construction queue capacity
        if (!CanStartNewConstruction)
        {
            reason = "File de construction pleine";
            return false;
        }
        
        // Check prerequisites
        if (!ArePrerequisitesMet(buildingType))
        {
            reason = "Prérequis non satisfaits";
            return false;
        }
        
        // Check resource availability
        if (!CanAffordConstruction(buildingType))
        {
            reason = "Ressources insuffisantes";
            return false;
        }
        
        return true;
    }

    public bool StartConstruction(Building.BuildingType buildingType)
    {
        if (!CanConstructBuilding(buildingType, out string reason))
        {
            if (debugMode)
            {
                Debug.LogWarning($"Cannot construct {buildingType}: {reason}");
            }
            return false;
        }
        
        BuildingData data = BuildingDB.GetBuilding(buildingType);
        var costs = data.GetBuildCosts();
        
        // Spend resources
        if (!ResourceManager.Instance.SpendResources(costs))
        {
            if (debugMode)
            {
                Debug.LogError($"Failed to spend resources for {buildingType}");
            }
            return false;
        }
        
        // Create construction project
        ConstructionProject project = new ConstructionProject(data, instantConstruction ? 0f : data.constructionTime * constructionSpeedMultiplier);
        activeProjects.Add(project);
        
        if (instantConstruction)
        {
            CompleteConstruction(project);
        }
        
        OnConstructionStarted?.Invoke(buildingType);
        
        if (debugMode)
        {
            Debug.Log($"Started construction of {buildingType}");
        }
        
        return true;
    }

    public bool CancelConstruction(Building.BuildingType buildingType)
    {
        ConstructionProject project = activeProjects.FirstOrDefault(p => p.BuildingType == buildingType);
        if (project == null) return false;
        
        activeProjects.Remove(project);
        
        // Refund partial resources (50% refund)
        BuildingData data = BuildingDB.GetBuilding(buildingType);
        if (data != null)
        {
            var costs = data.GetBuildCosts();
            var refund = new Dictionary<Resource.ResourceType, int>();
            
            foreach (var cost in costs)
            {
                refund[cost.Key] = cost.Value / 2;
            }
            
            ResourceManager.Instance.AddResources(refund);
        }
        
        OnConstructionCancelled?.Invoke(buildingType);
        
        if (debugMode)
        {
            Debug.Log($"Cancelled construction of {buildingType}");
        }
        
        return true;
    }

    public bool UpgradeBuilding(Building building)
    {
        if (building == null || !building.IsBuilt || building.Level >= 5)
        {
            return false;
        }
        
        BuildingData data = BuildingDB.GetBuilding(building.Type);
        if (data == null || !data.canUpgrade) return false;
        
        var upgradeCosts = data.GetBuildCosts(building.Level + 1);
        
        if (!ResourceManager.Instance.CanAfford(upgradeCosts))
        {
            return false;
        }
        
        if (!ResourceManager.Instance.SpendResources(upgradeCosts))
        {
            return false;
        }
        
        building.Upgrade();
        OnBuildingUpgraded?.Invoke(building);
        
        if (debugMode)
        {
            Debug.Log($"Upgraded {building.Type} to level {building.Level}");
        }
        
        return true;
    }

    private void ProcessActiveConstructions()
    {
        for (int i = activeProjects.Count - 1; i >= 0; i--)
        {
            ConstructionProject project = activeProjects[i];
            project.UpdateProgress(Time.deltaTime);
            
            OnConstructionProgress?.Invoke(project.BuildingType, project.Progress);
            
            if (project.IsCompleted)
            {
                CompleteConstruction(project);
                activeProjects.RemoveAt(i);
            }
        }
    }

    private void CompleteConstruction(ConstructionProject project)
    {
        // Create the actual building
        Building newBuilding = new Building(project.BuildingType, project.Data.displayName, project.Data.category);
        newBuilding.Build();
        
        constructedBuildings.Add(newBuilding);
        
        // Add building to city if GameManager is available
        if (GameManager.Instance != null && GameManager.Instance.CurrentCity != null)
        {
            GameManager.Instance.CurrentCity.Buildings.Add(newBuilding);
        }
        
        OnConstructionCompleted?.Invoke(project.BuildingType);
        
        if (debugMode)
        {
            Debug.Log($"Completed construction of {project.BuildingType}");
        }
    }

    private bool IsUnderConstruction(Building.BuildingType buildingType)
    {
        return activeProjects.Any(p => p.BuildingType == buildingType);
    }

    private bool ArePrerequisitesMet(Building.BuildingType buildingType)
    {
        BuildingData data = BuildingDB.GetBuilding(buildingType);
        if (data == null || data.prerequisiteBuildings == null || data.prerequisiteBuildings.Length == 0)
        {
            return true;
        }
        
        foreach (var prerequisite in data.prerequisiteBuildings)
        {
            if (!HasBuilding(prerequisite))
            {
                return false;
            }
        }
        
        return true;
    }

    private bool CanAffordConstruction(Building.BuildingType buildingType)
    {
        BuildingData data = BuildingDB.GetBuilding(buildingType);
        if (data == null) return false;
        
        var costs = data.GetBuildCosts();
        return ResourceManager.Instance != null && ResourceManager.Instance.CanAfford(costs);
    }

    public bool HasBuilding(Building.BuildingType buildingType)
    {
        return constructedBuildings.Any(b => b.Type == buildingType && b.IsBuilt);
    }

    public Building GetBuilding(Building.BuildingType buildingType)
    {
        return constructedBuildings.FirstOrDefault(b => b.Type == buildingType && b.IsBuilt);
    }

    public List<Building> GetBuildingsByCategory(Building.BuildingCategory category)
    {
        return constructedBuildings.Where(b => b.Category == category && b.IsBuilt).ToList();
    }

    public int GetBuildingCount(Building.BuildingType buildingType)
    {
        return constructedBuildings.Count(b => b.Type == buildingType && b.IsBuilt);
    }

    public int GetBuildingCountByCategory(Building.BuildingCategory category)
    {
        return constructedBuildings.Count(b => b.Category == category && b.IsBuilt);
    }

    public void LoadConstructedBuildings(List<Building> buildings)
    {
        constructedBuildings.Clear();
        if (buildings != null)
        {
            constructedBuildings.AddRange(buildings.Where(b => b.IsBuilt));
        }
        
        if (debugMode)
        {
            Debug.Log($"Loaded {constructedBuildings.Count} constructed buildings");
        }
    }

    public void SetConstructionSpeed(float speedMultiplier)
    {
        constructionSpeedMultiplier = Mathf.Max(0.1f, speedMultiplier);
    }

    public void SetMaxConcurrentConstructions(int maxCount)
    {
        maxConcurrentConstructions = Mathf.Max(1, maxCount);
    }

    public void SetInstantConstruction(bool instant)
    {
        instantConstruction = instant;
    }

    public float GetConstructionProgress(Building.BuildingType buildingType)
    {
        ConstructionProject project = activeProjects.FirstOrDefault(p => p.BuildingType == buildingType);
        return project?.Progress ?? 0f;
    }

    public float GetRemainingConstructionTime(Building.BuildingType buildingType)
    {
        ConstructionProject project = activeProjects.FirstOrDefault(p => p.BuildingType == buildingType);
        return project?.RemainingTime ?? 0f;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // Debug methods
    #if UNITY_EDITOR
    [ContextMenu("Debug: List Constructed Buildings")]
    private void DebugListBuildings()
    {
        Debug.Log($"Constructed Buildings ({constructedBuildings.Count}):");
        foreach (var building in constructedBuildings)
        {
            Debug.Log($"  - {building.Type} (Level {building.Level})");
        }
    }

    [ContextMenu("Debug: List Active Constructions")]
    private void DebugListConstructions()
    {
        Debug.Log($"Active Constructions ({activeProjects.Count}):");
        foreach (var project in activeProjects)
        {
            Debug.Log($"  - {project.BuildingType} ({project.Progress:P1})");
        }
    }
    #endif
}

[System.Serializable]
public class ConstructionProject
{
    public BuildingData Data { get; private set; }
    public Building.BuildingType BuildingType => Data.buildingType;
    public float TotalTime { get; private set; }
    public float ElapsedTime { get; private set; }
    public float Progress => TotalTime > 0 ? ElapsedTime / TotalTime : 1f;
    public float RemainingTime => TotalTime - ElapsedTime;
    public bool IsCompleted => ElapsedTime >= TotalTime;
    
    public ConstructionProject(BuildingData data, float constructionTime)
    {
        Data = data;
        TotalTime = constructionTime;
        ElapsedTime = 0f;
    }
    
    public void UpdateProgress(float deltaTime)
    {
        if (!IsCompleted)
        {
            ElapsedTime += deltaTime;
            ElapsedTime = Mathf.Min(ElapsedTime, TotalTime);
        }
    }
}