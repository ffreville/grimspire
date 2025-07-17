using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    [Header("Bootstrap Settings")]
    [SerializeField] private bool autoInitializeOnStart = true;
    [SerializeField] private bool showMainMenuOnStart = true;
    [SerializeField] private bool debugMode = true;

    private void Awake()
    {
        // Ensure this is the first thing that runs
        if (autoInitializeOnStart)
        {
            InitializeGame();
        }
    }

    private void Start()
    {
        if (showMainMenuOnStart)
        {
            ShowMainMenu();
        }
    }

    [ContextMenu("Initialize Game")]
    public void InitializeGame()
    {
        if (debugMode)
        {
            Debug.Log("=== Game Bootstrap Starting ===");
        }

        // Initialize all core systems
        InitializeCoreSystems();
        
        // Initialize UI
        InitializeUI();
        
        // Configure UI events
        ConfigureUIEvents();
        
        if (debugMode)
        {
            Debug.Log("=== Game Bootstrap Complete ===");
        }
    }

    private void InitializeCoreSystems()
    {
        // Initialize ResourceManager
        if (ResourceManager.Instance == null)
        {
            GameObject resourceManagerObj = new GameObject("ResourceManager");
            resourceManagerObj.AddComponent<ResourceManager>();
            DontDestroyOnLoad(resourceManagerObj);
        }

        // Initialize GameManager
        if (GameManager.Instance == null)
        {
            GameObject gameManagerObj = new GameObject("GameManager");
            gameManagerObj.AddComponent<GameManager>();
            DontDestroyOnLoad(gameManagerObj);
        }

        // Initialize other systems
        EnsureSystemExists<RecruitmentSystem>("RecruitmentSystem");
        EnsureSystemExists<PartyManagementSystem>("PartyManagementSystem");
        EnsureSystemExists<CraftingSystem>("CraftingSystem");
        EnsureSystemExists<ProgressionSystem>("ProgressionSystem");
        EnsureSystemExists<EquipmentEnhancementSystem>("EquipmentEnhancementSystem");
        EnsureSystemExists<ItemDatabase>("ItemDatabase");

        if (debugMode)
        {
            Debug.Log("✓ Core systems initialized");
        }
    }

    private void EnsureSystemExists<T>(string objectName) where T : MonoBehaviour
    {
        if (FindObjectOfType<T>() == null)
        {
            GameObject systemObject = new GameObject(objectName);
            systemObject.AddComponent<T>();
            DontDestroyOnLoad(systemObject);
        }
    }

    private void InitializeUI()
    {
        // Initialize Canvas Manager
        if (CanvasManager.Instance == null)
        {
            GameObject canvasManagerObj = new GameObject("CanvasManager");
            canvasManagerObj.AddComponent<CanvasManager>();
            DontDestroyOnLoad(canvasManagerObj);
        }

        // Initialize New Game Manager
        if (NewGameManager.Instance == null)
        {
            GameObject newGameManagerObj = new GameObject("NewGameManager");
            newGameManagerObj.AddComponent<NewGameManager>();
            DontDestroyOnLoad(newGameManagerObj);
        }

        // Initialize Main Menu Manager
        if (MainMenuManager.Instance == null)
        {
            GameObject mainMenuManagerObj = new GameObject("MainMenuManager");
            mainMenuManagerObj.AddComponent<MainMenuManager>();
            DontDestroyOnLoad(mainMenuManagerObj);
        }

        // Initialize Game Integration Manager
        if (GameIntegrationManager.Instance == null)
        {
            GameObject gameIntegrationObj = new GameObject("GameIntegrationManager");
            gameIntegrationObj.AddComponent<GameIntegrationManager>();
            DontDestroyOnLoad(gameIntegrationObj);
        }

        if (debugMode)
        {
            Debug.Log("✓ UI managers initialized");
        }
    }

    private void ConfigureUIEvents()
    {
        // Ensure UIEventConfigurator exists
        UIEventConfigurator eventConfigurator = FindObjectOfType<UIEventConfigurator>();
        if (eventConfigurator == null)
        {
            GameObject eventConfiguratorObj = new GameObject("UIEventConfigurator");
            eventConfigurator = eventConfiguratorObj.AddComponent<UIEventConfigurator>();
            DontDestroyOnLoad(eventConfiguratorObj);
        }

        // Configure all UI events
        eventConfigurator.ConfigureAllUIEvents();

        if (debugMode)
        {
            Debug.Log("✓ UI events configured");
        }
    }

    private void ShowMainMenu()
    {
        if (CanvasManager.Instance != null)
        {
            CanvasManager.Instance.ShowMainMenu();
            
            if (debugMode)
            {
                Debug.Log("✓ Main menu shown");
            }
        }
        else
        {
            Debug.LogWarning("CanvasManager not found - cannot show main menu");
        }
    }

    [ContextMenu("Test New Game")]
    public void TestNewGame()
    {
        if (debugMode)
        {
            Debug.Log("=== Testing New Game ===");
        }

        if (NewGameManager.Instance != null)
        {
            NewGameManager.Instance.SetPlayerName("Test Player");
            NewGameManager.Instance.SetCityName("Test City");
            NewGameManager.Instance.StartNewGame();
            
            if (debugMode)
            {
                Debug.Log("✓ New game started");
            }
        }
        else
        {
            Debug.LogWarning("NewGameManager not found - cannot start new game");
        }
    }

    [ContextMenu("Debug System Status")]
    public void DebugSystemStatus()
    {
        Debug.Log("=== System Status ===");
        Debug.Log($"ResourceManager: {(ResourceManager.Instance != null ? "✓" : "✗")}");
        Debug.Log($"GameManager: {(GameManager.Instance != null ? "✓" : "✗")}");
        Debug.Log($"CanvasManager: {(CanvasManager.Instance != null ? "✓" : "✗")}");
        Debug.Log($"NewGameManager: {(NewGameManager.Instance != null ? "✓" : "✗")}");
        Debug.Log($"MainMenuManager: {(MainMenuManager.Instance != null ? "✓" : "✗")}");
        Debug.Log($"GameIntegrationManager: {(GameIntegrationManager.Instance != null ? "✓" : "✗")}");
        Debug.Log($"RecruitmentSystem: {(RecruitmentSystem.Instance != null ? "✓" : "✗")}");
        Debug.Log($"CraftingSystem: {(CraftingSystem.Instance != null ? "✓" : "✗")}");
        Debug.Log($"ProgressionSystem: {(ProgressionSystem.Instance != null ? "✓" : "✗")}");
        Debug.Log("=== End System Status ===");
    }
}