using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Game State")]
    [SerializeField] private GameData currentGameData;
    [SerializeField] private City currentCity;
    [SerializeField] private bool isGameInitialized = false;
    
    [Header("Auto Save")]
    [SerializeField] private bool autoSaveEnabled = true;
    [SerializeField] private float autoSaveInterval = 300f; // 5 minutes
    private float lastAutoSaveTime;
    
    public GameData CurrentGameData => currentGameData;
    public City CurrentCity => currentCity;
    public bool IsGameInitialized => isGameInitialized;
    
    public static event Action<GameData> OnGameInitialized;
    public static event Action<City> OnCityChanged;
    public static event Action OnGameSaved;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeManagers();
    }

    private void InitializeManagers()
    {
        // Initialize save system
        if (SaveSystem.Instance == null)
        {
            GameObject saveSystemObject = new GameObject("SaveSystem");
            saveSystemObject.AddComponent<SaveSystem>();
            DontDestroyOnLoad(saveSystemObject);
        }
        
        // Initialize scene manager
        if (SceneManager.Instance == null)
        {
            GameObject sceneManagerObject = new GameObject("SceneManager");
            sceneManagerObject.AddComponent<SceneManager>();
            DontDestroyOnLoad(sceneManagerObject);
        }
        
        // Initialize menu manager
        if (MenuManager.Instance == null)
        {
            GameObject menuManagerObject = new GameObject("MenuManager");
            menuManagerObject.AddComponent<MenuManager>();
            DontDestroyOnLoad(menuManagerObject);
        }
        
        // Initialize resource manager
        if (ResourceManager.Instance == null)
        {
            GameObject resourceManagerObject = new GameObject("ResourceManager");
            resourceManagerObject.AddComponent<ResourceManager>();
            DontDestroyOnLoad(resourceManagerObject);
        }
        
        // Initialize economic system
        if (EconomicSystem.Instance == null)
        {
            GameObject economicSystemObject = new GameObject("EconomicSystem");
            economicSystemObject.AddComponent<EconomicSystem>();
            DontDestroyOnLoad(economicSystemObject);
        }
        
        // Initialize construction system
        if (ConstructionSystem.Instance == null)
        {
            GameObject constructionSystemObject = new GameObject("ConstructionSystem");
            constructionSystemObject.AddComponent<ConstructionSystem>();
            DontDestroyOnLoad(constructionSystemObject);
        }
        
        // Initialize building effects system
        if (BuildingEffectsSystem.Instance == null)
        {
            GameObject buildingEffectsObject = new GameObject("BuildingEffectsSystem");
            buildingEffectsObject.AddComponent<BuildingEffectsSystem>();
            DontDestroyOnLoad(buildingEffectsObject);
        }
        
        // Initialize recruitment system
        if (RecruitmentSystem.Instance == null)
        {
            GameObject recruitmentSystemObject = new GameObject("RecruitmentSystem");
            recruitmentSystemObject.AddComponent<RecruitmentSystem>();
            DontDestroyOnLoad(recruitmentSystemObject);
        }
        
        // Initialize party management system
        if (PartyManagementSystem.Instance == null)
        {
            GameObject partyManagementObject = new GameObject("PartyManagementSystem");
            partyManagementObject.AddComponent<PartyManagementSystem>();
            DontDestroyOnLoad(partyManagementObject);
        }
    }

    public void InitializeGame(GameData gameData)
    {
        currentGameData = gameData;
        
        // Create city from game data
        currentCity = new City(gameData.cityName);
        LoadCityFromGameData();
        
        isGameInitialized = true;
        lastAutoSaveTime = Time.time;
        
        OnGameInitialized?.Invoke(currentGameData);
        OnCityChanged?.Invoke(currentCity);
        
        Debug.Log($"Game initialized with city: {currentCity.Name}");
    }

    private void LoadCityFromGameData()
    {
        // Load resources into ResourceManager
        if (ResourceManager.Instance != null && currentGameData.resources != null)
        {
            ResourceManager.Instance.LoadResourcesFromGameData(currentGameData.resources);
        }
        
        // Load resources into City (for backward compatibility)
        if (currentGameData.resources != null)
        {
            foreach (var resourceData in currentGameData.resources)
            {
                var resource = currentCity.GetResource(resourceData.type);
                if (resource != null)
                {
                    resource.SetAmount(resourceData.amount);
                    resource.SetCapacity(resourceData.capacity);
                }
            }
        }
        
        // Load buildings
        if (currentGameData.buildings != null)
        {
            var buildings = new List<Building>();
            foreach (var buildingData in currentGameData.buildings)
            {
                Building building = buildingData.ToBuilding();
                currentCity.Buildings.Add(building);
                buildings.Add(building);
            }
            
            // Load buildings into ConstructionSystem
            if (ConstructionSystem.Instance != null)
            {
                ConstructionSystem.Instance.LoadConstructedBuildings(buildings);
            }
        }
        
        // Load adventurers
        if (currentGameData.adventurers != null)
        {
            foreach (var adventurerData in currentGameData.adventurers)
            {
                Adventurer adventurer = adventurerData.ToAdventurer();
                currentCity.AddAdventurer(adventurer);
            }
        }
        
        // Set city properties
        currentCity.AdvanceTime(currentGameData.dayTime);
    }

    private void Update()
    {
        if (!isGameInitialized) return;
        
        // Update game time
        UpdateGameTime();
        
        // Handle auto save
        HandleAutoSave();
    }

    private void UpdateGameTime()
    {
        if (currentCity != null)
        {
            currentCity.AdvanceTime(Time.deltaTime);
        }
    }

    private void HandleAutoSave()
    {
        if (autoSaveEnabled && currentGameData.autosaveEnabled)
        {
            if (Time.time - lastAutoSaveTime >= autoSaveInterval)
            {
                AutoSave();
                lastAutoSaveTime = Time.time;
            }
        }
    }

    public void SaveGame()
    {
        if (!isGameInitialized) return;
        
        UpdateGameDataFromCity();
        
        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.SaveGame(currentGameData);
            OnGameSaved?.Invoke();
        }
    }

    public void AutoSave()
    {
        if (!isGameInitialized) return;
        
        UpdateGameDataFromCity();
        
        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.AutoSave(currentGameData);
        }
    }

    private void UpdateGameDataFromCity()
    {
        if (currentCity == null) return;
        
        // Update basic city info
        currentGameData.cityName = currentCity.Name;
        currentGameData.dayTime = currentCity.DayTime;
        currentGameData.isNightPhase = currentCity.IsNightPhase;
        currentGameData.totalPlayTime += Time.deltaTime;
        currentGameData.UpdateSaveTime();
        
        // Update resources from ResourceManager if available
        var resourcesList = new System.Collections.Generic.List<SerializableResource>();
        if (ResourceManager.Instance != null)
        {
            foreach (var kvp in ResourceManager.Instance.Resources)
            {
                resourcesList.Add(new SerializableResource(kvp.Key, kvp.Value.Amount, kvp.Value.Capacity));
            }
        }
        else
        {
            // Fallback to city resources
            foreach (var kvp in currentCity.Resources)
            {
                resourcesList.Add(new SerializableResource(kvp.Key, kvp.Value.Amount, kvp.Value.Capacity));
            }
        }
        currentGameData.resources = resourcesList.ToArray();
        
        // Update buildings
        var buildingsList = new System.Collections.Generic.List<SerializableBuildingData>();
        foreach (var building in currentCity.Buildings)
        {
            buildingsList.Add(new SerializableBuildingData(building));
        }
        currentGameData.buildings = buildingsList.ToArray();
        
        // Update adventurers
        var adventurersList = new System.Collections.Generic.List<SerializableAdventurerData>();
        foreach (var adventurer in currentCity.Adventurers)
        {
            adventurersList.Add(new SerializableAdventurerData(adventurer));
        }
        currentGameData.adventurers = adventurersList.ToArray();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus && isGameInitialized)
        {
            AutoSave();
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && isGameInitialized)
        {
            AutoSave();
        }
    }
}
