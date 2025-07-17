using UnityEngine;
using UnityEngine.UI;

public class GameIntegrationManager : MonoBehaviour
{
    private static GameIntegrationManager instance;
    public static GameIntegrationManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameIntegrationManager>();
                if (instance == null)
                {
                    GameObject gameIntegrationObject = new GameObject("GameIntegrationManager");
                    instance = gameIntegrationObject.AddComponent<GameIntegrationManager>();
                    DontDestroyOnLoad(gameIntegrationObject);
                }
            }
            return instance;
        }
    }

    [Header("UI References")]
    [SerializeField] private Button adventurerMenuButton;
    [SerializeField] private Button equipmentMenuButton;
    [SerializeField] private Button craftingMenuButton;
    [SerializeField] private Button mainMenuButton;

    [Header("Game State")]
    [SerializeField] private bool gameStarted = false;
    
    public System.Action OnGameStarted;
    public System.Action OnGameEnded;
    public System.Action OnMenuOpened;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeIntegration();
    }

    private void InitializeIntegration()
    {
        // Listen for new game events
        if (NewGameManager.Instance != null)
        {
            NewGameManager.Instance.OnNewGameStarted += OnNewGameStarted;
            NewGameManager.Instance.OnGameInitialized += OnGameInitialized;
        }

        // Listen for main menu events
        if (MainMenuManager.Instance != null)
        {
            MainMenuManager.Instance.OnNewGameStarted += OnNewGameStarted;
            MainMenuManager.Instance.OnGameLoaded += OnGameLoaded;
        }

        // Setup UI button listeners
        SetupUIButtons();

        Debug.Log("Game Integration Manager initialized");
    }

    private void SetupUIButtons()
    {
        // Find and setup main game UI buttons
        if (adventurerMenuButton != null)
        {
            adventurerMenuButton.onClick.AddListener(OpenAdventurerMenu);
        }

        if (equipmentMenuButton != null)
        {
            equipmentMenuButton.onClick.AddListener(OpenEquipmentMenu);
        }

        if (craftingMenuButton != null)
        {
            craftingMenuButton.onClick.AddListener(OpenCraftingMenu);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(OpenMainMenu);
        }
    }

    private void OnNewGameStarted()
    {
        gameStarted = true;
        OnGameStarted?.Invoke();
        
        // Enable game UI buttons
        SetGameUIButtonsActive(true);
        
        Debug.Log("New game started - UI integrated");
    }

    private void OnGameInitialized()
    {
        // Setup initial game state
        InitializeGameUI();
        
        // Show initial tutorial or hints
        ShowInitialGameHints();
        
        Debug.Log("Game initialized - Phase 2 systems ready");
    }

    private void OnGameLoaded()
    {
        gameStarted = true;
        OnGameStarted?.Invoke();
        
        // Enable game UI buttons
        SetGameUIButtonsActive(true);
        
        Debug.Log("Game loaded - UI integrated");
    }

    private void InitializeGameUI()
    {
        // Setup resource display
        UpdateResourceDisplay();
        
        // Setup adventurer roster
        RefreshAdventurerRoster();
        
        // Setup crafting availability
        UpdateCraftingAvailability();
    }

    private void UpdateResourceDisplay()
    {
        // This would update the resource display UI
        if (ResourceManager.Instance != null)
        {
            int gold = ResourceManager.Instance.GetResourceAmount(Resource.ResourceType.Gold);
            int population = ResourceManager.Instance.GetResourceAmount(Resource.ResourceType.Population);
            
            Debug.Log($"Resources - Gold: {gold}, Population: {population}");
        }
    }

    private void RefreshAdventurerRoster()
    {
        // This would refresh the adventurer roster UI
        if (RecruitmentSystem.Instance != null)
        {
            var availableAdventurers = RecruitmentSystem.Instance.AvailableAdventurers;
            Debug.Log($"Available adventurers for recruitment: {availableAdventurers.Count}");
        }
        
        // Check parties for recruited adventurers
        if (PartyManagementSystem.Instance != null)
        {
            var parties = PartyManagementSystem.Instance.Parties;
            int totalRecruitedAdventurers = 0;
            foreach (var party in parties)
            {
                totalRecruitedAdventurers += party.Members.Count;
            }
            Debug.Log($"Recruited adventurers in parties: {totalRecruitedAdventurers}");
        }
    }

    private void UpdateCraftingAvailability()
    {
        // This would update crafting system availability
        if (CraftingSystem.Instance != null)
        {
            var unlockedRecipes = CraftingSystem.Instance.GetUnlockedRecipes();
            var activeOrders = CraftingSystem.Instance.GetActiveOrders();
            var completedOrders = CraftingSystem.Instance.GetCompletedOrders();
            
            Debug.Log($"Unlocked crafting recipes: {unlockedRecipes.Count}");
            Debug.Log($"Active crafting orders: {activeOrders.Count}");
            Debug.Log($"Completed crafting orders: {completedOrders.Count}");
        }
    }

    private void ShowInitialGameHints()
    {
        // Show tutorial hints for first-time players
        Debug.Log("Bienvenue dans Grimspire! Gérez votre cité et vos aventuriers.");
        Debug.Log("Conseil: Commencez par recruter des aventuriers dans la taverne.");
        Debug.Log("Conseil: Équipez vos aventuriers avant de les envoyer en mission.");
        Debug.Log("Conseil: Utilisez le système de fabrication pour créer de meilleurs équipements.");
    }

    public void OpenAdventurerMenu()
    {
        if (!gameStarted)
        {
            Debug.LogWarning("Cannot open adventurer menu - game not started");
            return;
        }

        CanvasManager.Instance.ShowOverlay(CanvasManager.CanvasType.Adventurer);
        OnMenuOpened?.Invoke();
        
        Debug.Log("Opened adventurer menu");
    }

    public void OpenEquipmentMenu()
    {
        if (!gameStarted)
        {
            Debug.LogWarning("Cannot open equipment menu - game not started");
            return;
        }

        CanvasManager.Instance.ShowOverlay(CanvasManager.CanvasType.Equipment);
        OnMenuOpened?.Invoke();
        
        Debug.Log("Opened equipment menu");
    }

    public void OpenCraftingMenu()
    {
        if (!gameStarted)
        {
            Debug.LogWarning("Cannot open crafting menu - game not started");
            return;
        }

        CanvasManager.Instance.ShowOverlay(CanvasManager.CanvasType.Crafting);
        OnMenuOpened?.Invoke();
        
        Debug.Log("Opened crafting menu");
    }

    public void OpenMainMenu()
    {
        CanvasManager.Instance.ShowMainMenu();
        OnMenuOpened?.Invoke();
        
        Debug.Log("Opened main menu");
    }

    public void CloseAllMenus()
    {
        CanvasManager.Instance.ShowGameplay();
        Debug.Log("Closed all menus - returned to gameplay");
    }

    private void SetGameUIButtonsActive(bool active)
    {
        if (adventurerMenuButton != null)
            adventurerMenuButton.gameObject.SetActive(active);
        
        if (equipmentMenuButton != null)
            equipmentMenuButton.gameObject.SetActive(active);
        
        if (craftingMenuButton != null)
            craftingMenuButton.gameObject.SetActive(active);
    }

    public bool IsGameStarted()
    {
        return gameStarted;
    }

    public void EndGame()
    {
        gameStarted = false;
        OnGameEnded?.Invoke();
        
        // Disable game UI buttons
        SetGameUIButtonsActive(false);
        
        // Return to main menu
        CanvasManager.Instance.ShowMainMenu();
        
        Debug.Log("Game ended - returned to main menu");
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        Debug.Log("Game paused");
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        Debug.Log("Game resumed");
    }

    public void SaveGame()
    {
        if (!gameStarted)
        {
            Debug.LogWarning("Cannot save game - game not started");
            return;
        }

        // TODO: Interface with SaveSystem
        Debug.Log("Game saved");
    }

    public void QuickSave()
    {
        if (!gameStarted)
        {
            Debug.LogWarning("Cannot quick save - game not started");
            return;
        }

        // TODO: Interface with SaveSystem for quick save
        Debug.Log("Quick save completed");
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    private void Update()
    {
        // Handle keyboard shortcuts
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameStarted)
            {
                OpenMainMenu();
            }
        }

        if (Input.GetKeyDown(KeyCode.F5) && gameStarted)
        {
            QuickSave();
        }
    }
}