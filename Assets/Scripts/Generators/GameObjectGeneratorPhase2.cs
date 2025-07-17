using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameObjectGeneratorPhase2 : MonoBehaviour
{
    [Header("Generation Settings")]
    [SerializeField] private bool generateOnStart = false;
    [SerializeField] private bool generateSampleData = true;
    [SerializeField] private bool clearExistingObjects = true;
    [SerializeField] private Transform canvasTransform;
    [SerializeField] private Canvas mainCanvas;
    
    [Header("UI Prefabs (Optional)")]
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private GameObject panelPrefab;
    [SerializeField] private GameObject scrollViewPrefab;
    [SerializeField] private GameObject inputFieldPrefab;
    [SerializeField] private GameObject dropdownPrefab;
    [SerializeField] private GameObject sliderPrefab;
    [SerializeField] private GameObject textPrefab;
    
    [Header("Generated Objects")]
    [SerializeField] private List<GameObject> generatedObjects = new List<GameObject>();
    
    private void Start()
    {
        if (generateOnStart)
        {
            GenerateAllPhase2Systems();
        }
    }
    
    [ContextMenu("Generate All Phase 2 Systems")]
    public void GenerateAllPhase2Systems()
    {
        Debug.Log("Starting Phase 2 GameObject Generation...");
        
        // Clear previous generation
        if (clearExistingObjects)
        {
            ClearGeneratedObjects();
        }
        
        // Find or create canvas
        SetupCanvas();
        
        // Generate Phase 2.1 - Adventurer System
        GeneratePhase2_1_AdventurerSystem();
        
        // Generate Phase 2.2 - Equipment and Progression
        GeneratePhase2_2_EquipmentProgression();
        
        // Generate Phase 2.3 - Adventurer Interface
        GeneratePhase2_3_AdventurerInterface();
        
        // Generate Phase 2.4 - First Playable Version
        GeneratePhase2_4_FirstPlayableVersion();
        
        // Generate sample data if requested
        if (generateSampleData)
        {
            GenerateSampleData();
        }
        
        Debug.Log($"Phase 2 Generation Complete! Created {generatedObjects.Count} GameObjects");
    }
    
    private void SetupCanvas()
    {
        if (mainCanvas == null)
        {
            mainCanvas = FindObjectOfType<Canvas>();
        }
        
        if (mainCanvas == null)
        {
            // Create main canvas
            GameObject canvasObj = new GameObject("Main Canvas");
            mainCanvas = canvasObj.AddComponent<Canvas>();
            mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            generatedObjects.Add(canvasObj);
        }
        
        canvasTransform = mainCanvas.transform;
    }
    
    #region Phase 2.1 - Adventurer System
    private void GeneratePhase2_1_AdventurerSystem()
    {
        Debug.Log("Generating Phase 2.1 - Adventurer System...");
        
        // Create Adventurer Management System
        CreateAdventurerManagementSystem();
        
        // Create Recruitment System
        CreateRecruitmentSystem();
        
        // Create Party Management System
        CreatePartyManagementSystem();
        
        // Create Adventurer Data Components
        CreateAdventurerDataComponents();
    }
    
    private void CreateAdventurerManagementSystem()
    {
        GameObject adventurerManager = new GameObject("AdventurerManager");
        adventurerManager.AddComponent<RecruitmentSystem>();
        adventurerManager.AddComponent<PartyManagementSystem>();
        if (!generatedObjects.Contains(adventurerManager))
        {
            generatedObjects.Add(adventurerManager);
        }
        
        Debug.Log("✓ Created Adventurer Management System");
    }
    
    private void CreateRecruitmentSystem()
    {
        GameObject recruitmentPanel = CreatePanel("RecruitmentPanel", canvasTransform);
        
        // Recruitment UI
        GameObject recruitmentTitle = CreateText("RecruitmentTitle", recruitmentPanel.transform);
        recruitmentTitle.GetComponent<TextMeshProUGUI>().text = "Recrutement d'Aventuriers";
        
        GameObject availableAdventurers = CreateScrollView("AvailableAdventurers", recruitmentPanel.transform);
        GameObject recruitButton = CreateButton("RecruitButton", recruitmentPanel.transform);
        recruitButton.GetComponentInChildren<TextMeshProUGUI>().text = "Recruter";
        
        GameObject refreshButton = CreateButton("RefreshButton", recruitmentPanel.transform);
        refreshButton.GetComponentInChildren<TextMeshProUGUI>().text = "Actualiser";
        
        // Cost display
        GameObject costText = CreateText("CostText", recruitmentPanel.transform);
        costText.GetComponent<TextMeshProUGUI>().text = "Coût: 0 or";
        
        recruitmentPanel.SetActive(false);
        generatedObjects.Add(recruitmentPanel);
        
        Debug.Log("✓ Created Recruitment System UI");
    }
    
    private void CreatePartyManagementSystem()
    {
        GameObject partyPanel = CreatePanel("PartyManagementPanel", canvasTransform);
        
        // Party list
        GameObject partyList = CreateScrollView("PartyList", partyPanel.transform);
        GameObject createPartyButton = CreateButton("CreatePartyButton", partyPanel.transform);
        createPartyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Créer Groupe";
        
        GameObject disbandPartyButton = CreateButton("DisbandPartyButton", partyPanel.transform);
        disbandPartyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Dissoudre Groupe";
        
        // Formation selection
        GameObject formationDropdown = CreateDropdown("FormationDropdown", partyPanel.transform);
        
        partyPanel.SetActive(false);
        generatedObjects.Add(partyPanel);
        
        Debug.Log("✓ Created Party Management System UI");
    }
    
    private void CreateAdventurerDataComponents()
    {
        GameObject dataManager = new GameObject("AdventurerDataManager");
        // Add data management components if needed
        generatedObjects.Add(dataManager);
        
        Debug.Log("✓ Created Adventurer Data Components");
    }
    #endregion
    
    #region Phase 2.2 - Equipment and Progression
    private void GeneratePhase2_2_EquipmentProgression()
    {
        Debug.Log("Generating Phase 2.2 - Equipment and Progression...");
        
        // Create Item Database System
        CreateItemDatabaseSystem();
        
        // Create Crafting System
        CreateCraftingSystem();
        
        // Create Progression System
        CreateProgressionSystem();
        
        // Create Equipment Enhancement System
        CreateEquipmentEnhancementSystem();
    }
    
    private void CreateItemDatabaseSystem()
    {
        GameObject itemDatabase = new GameObject("ItemDatabase");
        itemDatabase.AddComponent<ItemDatabase>();
        generatedObjects.Add(itemDatabase);
        
        Debug.Log("✓ Created Item Database System");
    }
    
    private void CreateCraftingSystem()
    {
        GameObject craftingManager = new GameObject("CraftingSystem");
        craftingManager.AddComponent<CraftingSystem>();
        
        // Crafting UI
        GameObject craftingPanel = CreatePanel("CraftingPanel", canvasTransform);
        
        // Recipe list
        GameObject recipeList = CreateScrollView("RecipeList", craftingPanel.transform);
        GameObject recipeTitle = CreateText("RecipeTitle", craftingPanel.transform);
        recipeTitle.GetComponent<TextMeshProUGUI>().text = "Recettes de Fabrication";
        
        // Materials display
        GameObject materialsPanel = CreatePanel("MaterialsPanel", craftingPanel.transform);
        GameObject materialsTitle = CreateText("MaterialsTitle", materialsPanel.transform);
        materialsTitle.GetComponent<TextMeshProUGUI>().text = "Matériaux";
        
        GameObject materialsList = CreateScrollView("MaterialsList", materialsPanel.transform);
        
        // Crafting actions
        GameObject craftButton = CreateButton("CraftButton", craftingPanel.transform);
        craftButton.GetComponentInChildren<TextMeshProUGUI>().text = "Fabriquer";
        
        GameObject craftingQueue = CreateScrollView("CraftingQueue", craftingPanel.transform);
        GameObject queueTitle = CreateText("QueueTitle", craftingPanel.transform);
        queueTitle.GetComponent<TextMeshProUGUI>().text = "File d'attente";
        
        craftingPanel.SetActive(false);
        generatedObjects.Add(craftingManager);
        generatedObjects.Add(craftingPanel);
        
        Debug.Log("✓ Created Crafting System");
    }
    
    private void CreateProgressionSystem()
    {
        GameObject progressionManager = new GameObject("ProgressionSystem");
        progressionManager.AddComponent<ProgressionSystem>();
        
        // Progression UI will be created in Phase 2.3
        generatedObjects.Add(progressionManager);
        
        Debug.Log("✓ Created Progression System");
    }
    
    private void CreateEquipmentEnhancementSystem()
    {
        GameObject enhancementManager = new GameObject("EquipmentEnhancementSystem");
        enhancementManager.AddComponent<EquipmentEnhancementSystem>();
        
        // Enhancement UI
        GameObject enhancementPanel = CreatePanel("EnhancementPanel", canvasTransform);
        
        GameObject enhancementTitle = CreateText("EnhancementTitle", enhancementPanel.transform);
        enhancementTitle.GetComponent<TextMeshProUGUI>().text = "Amélioration d'Équipement";
        
        // Equipment slot for enhancement
        GameObject equipmentSlot = CreatePanel("EquipmentSlot", enhancementPanel.transform);
        
        // Enhancement materials
        GameObject materialsGrid = CreatePanel("MaterialsGrid", enhancementPanel.transform);
        GameObject enhanceButton = CreateButton("EnhanceButton", enhancementPanel.transform);
        enhanceButton.GetComponentInChildren<TextMeshProUGUI>().text = "Améliorer";
        
        // Success chance display
        GameObject successChanceText = CreateText("SuccessChanceText", enhancementPanel.transform);
        successChanceText.GetComponent<TextMeshProUGUI>().text = "Chance de succès: 80%";
        
        enhancementPanel.SetActive(false);
        generatedObjects.Add(enhancementManager);
        generatedObjects.Add(enhancementPanel);
        
        Debug.Log("✓ Created Equipment Enhancement System");
    }
    #endregion
    
    #region Phase 2.3 - Adventurer Interface
    private void GeneratePhase2_3_AdventurerInterface()
    {
        Debug.Log("Generating Phase 2.3 - Adventurer Interface...");
        
        // Create Roster Management UI
        CreateRosterManagementUI();
        
        // Create Character Sheet UI
        CreateCharacterSheetUI();
        
        // Create Equipment Interface
        CreateEquipmentInterface();
        
        // Create Talent Tree UI
        CreateTalentTreeUI();
    }
    
    private void CreateRosterManagementUI()
    {
        GameObject rosterPanel = CreatePanel("AdventurerRosterPanel", canvasTransform);
        GameObject rosterUI = new GameObject("AdventurerRosterUI");
        rosterUI.transform.SetParent(rosterPanel.transform);
        rosterUI.AddComponent<AdventurerRosterUI>();
        
        // Roster header
        GameObject rosterHeader = CreatePanel("RosterHeader", rosterPanel.transform);
        GameObject rosterTitle = CreateText("RosterTitle", rosterHeader.transform);
        rosterTitle.GetComponent<TextMeshProUGUI>().text = "Roster des Aventuriers";
        
        // Filters
        GameObject filterPanel = CreatePanel("FilterPanel", rosterPanel.transform);
        GameObject classFilter = CreateDropdown("ClassFilter", filterPanel.transform);
        GameObject statusFilter = CreateDropdown("StatusFilter", filterPanel.transform);
        GameObject levelFilter = CreateSlider("LevelFilter", filterPanel.transform);
        GameObject searchInput = CreateInputField("SearchInput", filterPanel.transform);
        
        // Sort controls
        GameObject sortPanel = CreatePanel("SortPanel", rosterPanel.transform);
        GameObject sortDropdown = CreateDropdown("SortDropdown", sortPanel.transform);
        GameObject sortOrderButton = CreateButton("SortOrderButton", sortPanel.transform);
        
        // Adventurer list
        GameObject adventurerList = CreateScrollView("AdventurerList", rosterPanel.transform);
        
        // Multi-selection controls
        GameObject selectionPanel = CreatePanel("SelectionPanel", rosterPanel.transform);
        GameObject selectAllButton = CreateButton("SelectAllButton", selectionPanel.transform);
        GameObject createPartyButton = CreateButton("CreatePartyButton", selectionPanel.transform);
        
        rosterPanel.SetActive(false);
        generatedObjects.Add(rosterPanel);
        
        Debug.Log("✓ Created Roster Management UI");
    }
    
    private void CreateCharacterSheetUI()
    {
        GameObject characterSheetPanel = CreatePanel("CharacterSheetPanel", canvasTransform);
        GameObject characterSheetUI = new GameObject("AdventurerCharacterSheetUI");
        characterSheetUI.transform.SetParent(characterSheetPanel.transform);
        characterSheetUI.AddComponent<AdventurerCharacterSheetUI>();
        
        // Tab system
        GameObject tabGroup = new GameObject("TabGroup");
        tabGroup.transform.SetParent(characterSheetPanel.transform);
        tabGroup.AddComponent<TabGroup>();
        
        // Create tabs
        CreateCharacterSheetTab("GeneralTab", tabGroup.transform, "Général");
        CreateCharacterSheetTab("StatsTab", tabGroup.transform, "Statistiques");
        CreateCharacterSheetTab("SkillsTab", tabGroup.transform, "Compétences");
        CreateCharacterSheetTab("EquipmentTab", tabGroup.transform, "Équipement");
        CreateCharacterSheetTab("HistoryTab", tabGroup.transform, "Historique");
        CreateCharacterSheetTab("ProgressionTab", tabGroup.transform, "Progression");
        
        // Tab content panels
        GameObject generalTab = CreatePanel("GeneralTabContent", characterSheetPanel.transform);
        GameObject statsTab = CreatePanel("StatsTabContent", characterSheetPanel.transform);
        GameObject skillsTab = CreatePanel("SkillsTabContent", characterSheetPanel.transform);
        GameObject equipmentTab = CreatePanel("EquipmentTabContent", characterSheetPanel.transform);
        GameObject historyTab = CreatePanel("HistoryTabContent", characterSheetPanel.transform);
        GameObject progressionTab = CreatePanel("ProgressionTabContent", characterSheetPanel.transform);
        
        // Action buttons
        GameObject actionPanel = CreatePanel("ActionPanel", characterSheetPanel.transform);
        GameObject healButton = CreateButton("HealButton", actionPanel.transform);
        GameObject restButton = CreateButton("RestButton", actionPanel.transform);
        GameObject dismissButton = CreateButton("DismissButton", actionPanel.transform);
        
        characterSheetPanel.SetActive(false);
        generatedObjects.Add(characterSheetPanel);
        
        Debug.Log("✓ Created Character Sheet UI");
    }
    
    private void CreateCharacterSheetTab(string tabName, Transform parent, string displayName)
    {
        GameObject tab = CreateButton(tabName, parent);
        GameObject tabButton = new GameObject("TabButton");
        tabButton.transform.SetParent(tab.transform);
        tabButton.AddComponent<TabButton>();
        
        tab.GetComponentInChildren<TextMeshProUGUI>().text = displayName;
    }
    
    private void CreateEquipmentInterface()
    {
        GameObject equipmentPanel = CreatePanel("EquipmentInterfacePanel", canvasTransform);
        GameObject equipmentUI = new GameObject("AdventurerEquipmentUI");
        equipmentUI.transform.SetParent(equipmentPanel.transform);
        equipmentUI.AddComponent<AdventurerEquipmentUI>();
        
        // Equipment slots
        GameObject slotsPanel = CreatePanel("EquipmentSlotsPanel", equipmentPanel.transform);
        CreateEquipmentSlot("WeaponSlot", slotsPanel.transform);
        CreateEquipmentSlot("ArmorSlot", slotsPanel.transform);
        CreateEquipmentSlot("HelmetSlot", slotsPanel.transform);
        CreateEquipmentSlot("BootsSlot", slotsPanel.transform);
        CreateEquipmentSlot("AccessorySlot", slotsPanel.transform);
        
        // Equipment details
        GameObject detailsPanel = CreatePanel("EquipmentDetailsPanel", equipmentPanel.transform);
        GameObject equipmentName = CreateText("EquipmentName", detailsPanel.transform);
        GameObject equipmentStats = CreateText("EquipmentStats", detailsPanel.transform);
        GameObject equipmentDescription = CreateText("EquipmentDescription", detailsPanel.transform);
        
        // Equipment inventory
        GameObject inventoryPanel = CreatePanel("EquipmentInventoryPanel", equipmentPanel.transform);
        GameObject inventoryTitle = CreateText("InventoryTitle", inventoryPanel.transform);
        inventoryTitle.GetComponent<TextMeshProUGUI>().text = "Inventaire";
        
        GameObject inventoryGrid = CreateScrollView("InventoryGrid", inventoryPanel.transform);
        
        // Equipment filter
        GameObject filterUI = new GameObject("EquipmentFilterUI");
        filterUI.transform.SetParent(equipmentPanel.transform);
        filterUI.AddComponent<EquipmentFilterUI>();
        
        equipmentPanel.SetActive(false);
        generatedObjects.Add(equipmentPanel);
        
        Debug.Log("✓ Created Equipment Interface");
    }
    
    private void CreateEquipmentSlot(string slotName, Transform parent)
    {
        GameObject slot = CreatePanel(slotName, parent);
        GameObject slotUI = new GameObject("EquipmentSlotUI");
        slotUI.transform.SetParent(slot.transform);
        slotUI.AddComponent<EquipmentSlotUI>();
        
        // Add visual components
        GameObject slotIcon = new GameObject("SlotIcon");
        slotIcon.transform.SetParent(slot.transform);
        slotIcon.AddComponent<Image>();
    }
    
    private void CreateTalentTreeUI()
    {
        GameObject talentTreePanel = CreatePanel("TalentTreePanel", canvasTransform);
        GameObject talentTreeUI = new GameObject("TalentTreeUI");
        talentTreeUI.transform.SetParent(talentTreePanel.transform);
        talentTreeUI.AddComponent<TalentTreeUI>();
        
        // Talent tree container
        GameObject talentContainer = CreateScrollView("TalentContainer", talentTreePanel.transform);
        
        // Talent details panel
        GameObject talentDetails = CreatePanel("TalentDetailsPanel", talentTreePanel.transform);
        GameObject talentName = CreateText("TalentName", talentDetails.transform);
        GameObject talentDescription = CreateText("TalentDescription", talentDetails.transform);
        GameObject unlockButton = CreateButton("UnlockTalentButton", talentDetails.transform);
        
        // Class selection
        GameObject classPanel = CreatePanel("ClassPanel", talentTreePanel.transform);
        GameObject classDropdown = CreateDropdown("ClassDropdown", classPanel.transform);
        GameObject skillPointsText = CreateText("SkillPointsText", classPanel.transform);
        
        // Specialization panel
        GameObject specializationPanel = CreatePanel("SpecializationPanel", talentTreePanel.transform);
        GameObject specializationList = CreateScrollView("SpecializationList", specializationPanel.transform);
        
        talentTreePanel.SetActive(false);
        generatedObjects.Add(talentTreePanel);
        
        Debug.Log("✓ Created Talent Tree UI");
    }
    #endregion
    
    #region Phase 2.4 - First Playable Version
    private void GeneratePhase2_4_FirstPlayableVersion()
    {
        Debug.Log("Generating Phase 2.4 - First Playable Version...");
        
        // Create Canvas Manager
        CreateCanvasManager();
        
        // Create New Game Manager
        CreateNewGameManager();
        
        // Create Main Menu Manager
        CreateMainMenuManager();
        
        // Create Main Menu UI
        CreateMainMenuUI();
        
        // Create New Game UI
        CreateNewGameUI();
        
        // Create Game Integration Manager
        CreateGameIntegrationManager();
        
        // Create UI Auto-Initializer
        CreateUIAutoInitializer();
        
        // Create UI Event Configurator
        CreateUIEventConfigurator();
        
        // Create Script Attacher
        CreateScriptAttacher();
    }
    
    private void CreateCanvasManager()
    {
        GameObject canvasManager = new GameObject("CanvasManager");
        // CanvasManager component will be added at runtime when the script is available
        generatedObjects.Add(canvasManager);
        
        Debug.Log("✓ Created Canvas Manager GameObject");
    }
    
    private void CreateNewGameManager()
    {
        GameObject newGameManager = new GameObject("NewGameManager");
        // NewGameManager component will be added at runtime when the script is available
        generatedObjects.Add(newGameManager);
        
        Debug.Log("✓ Created New Game Manager GameObject");
    }
    
    private void CreateMainMenuManager()
    {
        GameObject mainMenuManager = new GameObject("MainMenuManager");
        // MainMenuManager component will be added at runtime when the script is available
        generatedObjects.Add(mainMenuManager);
        
        Debug.Log("✓ Created Main Menu Manager GameObject");
    }
    
    private void CreateMainMenuUI()
    {
        GameObject mainMenuCanvas = CreateCanvas("MainMenuCanvas", 0).gameObject;
        
        // Main menu panel
        GameObject mainMenuPanel = CreatePanel("MainMenuPanel", mainMenuCanvas.transform);
        
        // Title
        GameObject titleText = CreateText("GameTitle", mainMenuPanel.transform);
        titleText.GetComponent<TextMeshProUGUI>().text = "GRIMSPIRE";
        titleText.GetComponent<TextMeshProUGUI>().fontSize = 72;
        
        // Subtitle
        GameObject subtitleText = CreateText("GameSubtitle", mainMenuPanel.transform);
        subtitleText.GetComponent<TextMeshProUGUI>().text = "Gestionnaire de Cité Aventurière";
        subtitleText.GetComponent<TextMeshProUGUI>().fontSize = 24;
        
        // Main menu buttons
        GameObject buttonPanel = CreatePanel("MainMenuButtons", mainMenuPanel.transform);
        
        GameObject newGameButton = CreateButton("NewGameButton", buttonPanel.transform);
        newGameButton.GetComponentInChildren<TextMeshProUGUI>().text = "Nouvelle Partie";
        
        GameObject loadGameButton = CreateButton("LoadGameButton", buttonPanel.transform);
        loadGameButton.GetComponentInChildren<TextMeshProUGUI>().text = "Charger Partie";
        
        GameObject settingsButton = CreateButton("SettingsButton", buttonPanel.transform);
        settingsButton.GetComponentInChildren<TextMeshProUGUI>().text = "Paramètres";
        
        GameObject quitButton = CreateButton("QuitButton", buttonPanel.transform);
        quitButton.GetComponentInChildren<TextMeshProUGUI>().text = "Quitter";
        
        // Version info
        GameObject versionText = CreateText("VersionText", mainMenuPanel.transform);
        versionText.GetComponent<TextMeshProUGUI>().text = "Version Phase 2.4";
        versionText.GetComponent<TextMeshProUGUI>().fontSize = 14;
        
        mainMenuCanvas.SetActive(false);
        generatedObjects.Add(mainMenuCanvas);
        
        Debug.Log("✓ Created Main Menu UI");
    }
    
    private void CreateNewGameUI()
    {
        // Ensure we have a canvas to work with
        if (canvasTransform == null)
        {
            SetupCanvas();
        }
        
        GameObject newGamePanel = CreatePanel("NewGamePanel", canvasTransform);
        
        // Title
        GameObject titleText = CreateText("NewGameTitle", newGamePanel.transform);
        titleText.GetComponent<TextMeshProUGUI>().text = "Nouvelle Partie";
        titleText.GetComponent<TextMeshProUGUI>().fontSize = 36;
        
        // Player name input
        GameObject playerNameLabel = CreateText("PlayerNameLabel", newGamePanel.transform);
        playerNameLabel.GetComponent<TextMeshProUGUI>().text = "Nom du Seigneur:";
        
        GameObject playerNameInput = CreateInputField("PlayerNameInput", newGamePanel.transform);
        SetInputFieldPlaceholder(playerNameInput, "Seigneur");
        
        // City name input
        GameObject cityNameLabel = CreateText("CityNameLabel", newGamePanel.transform);
        cityNameLabel.GetComponent<TextMeshProUGUI>().text = "Nom de la Cité:";
        
        GameObject cityNameInput = CreateInputField("CityNameInput", newGamePanel.transform);
        SetInputFieldPlaceholder(cityNameInput, "Grimspire");
        
        // Buttons
        GameObject buttonPanel = CreatePanel("NewGameButtons", newGamePanel.transform);
        
        GameObject startButton = CreateButton("StartNewGameButton", buttonPanel.transform);
        startButton.GetComponentInChildren<TextMeshProUGUI>().text = "Commencer";
        
        GameObject cancelButton = CreateButton("CancelNewGameButton", buttonPanel.transform);
        cancelButton.GetComponentInChildren<TextMeshProUGUI>().text = "Annuler";
        
        newGamePanel.SetActive(false);
        generatedObjects.Add(newGamePanel);
        
        Debug.Log("✓ Created New Game UI");
    }
    
    private void CreateGameIntegrationManager()
    {
        GameObject gameIntegration = new GameObject("GameIntegrationManager");
        // GameIntegrationManager component will be added at runtime when the script is available
        generatedObjects.Add(gameIntegration);
        
        Debug.Log("✓ Created Game Integration Manager");
    }
    
    private void CreateUIAutoInitializer()
    {
        GameObject uiInitializer = new GameObject("UIAutoInitializer");
        // UIAutoInitializer component will be added at runtime when the script is available
        generatedObjects.Add(uiInitializer);
        
        Debug.Log("✓ Created UI Auto-Initializer GameObject");
    }
    
    private void CreateUIEventConfigurator()
    {
        GameObject uiEventConfigurator = new GameObject("UIEventConfigurator");
        // UIEventConfigurator component will be added at runtime when the script is available
        generatedObjects.Add(uiEventConfigurator);
        
        Debug.Log("✓ Created UI Event Configurator GameObject");
    }
    
    private void CreateScriptAttacher()
    {
        GameObject scriptAttacher = new GameObject("ScriptAttacher");
        // ScriptAttacher component will be added at runtime when the script is available
        generatedObjects.Add(scriptAttacher);
        
        Debug.Log("✓ Created Script Attacher GameObject");
    }
    
    private Canvas CreateCanvas(string name, int sortingOrder)
    {
        GameObject canvasObject = new GameObject(name);
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = sortingOrder;
        
        CanvasScaler canvasScaler = canvasObject.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920, 1080);
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        canvasScaler.matchWidthOrHeight = 0.5f;
        
        canvasObject.AddComponent<GraphicRaycaster>();
        
        CanvasGroup canvasGroup = canvasObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        
        canvas.gameObject.SetActive(false);
        
        return canvas;
    }
    #endregion
    
    #region UI Helper Methods
    private GameObject CreatePanel(string name, Transform parent)
    {
        GameObject panel = panelPrefab != null ? Instantiate(panelPrefab, parent) : new GameObject(name);
        panel.name = name;
        panel.transform.SetParent(parent);
        
        if (panelPrefab == null)
        {
            panel.AddComponent<RectTransform>();
            panel.AddComponent<Image>();
        }
        
        return panel;
    }
    
    private GameObject CreateButton(string name, Transform parent)
    {
        GameObject button = buttonPrefab != null ? Instantiate(buttonPrefab, parent) : new GameObject(name);
        button.name = name;
        button.transform.SetParent(parent);
        
        if (buttonPrefab == null)
        {
            button.AddComponent<RectTransform>();
            button.AddComponent<Image>();
            button.AddComponent<Button>();
            
            GameObject buttonText = new GameObject("Text");
            buttonText.transform.SetParent(button.transform);
            buttonText.AddComponent<RectTransform>();
            buttonText.AddComponent<TextMeshProUGUI>();
            buttonText.GetComponent<TextMeshProUGUI>().text = name;
        }
        
        return button;
    }
    
    private GameObject CreateText(string name, Transform parent)
    {
        GameObject text = textPrefab != null ? Instantiate(textPrefab, parent) : new GameObject(name);
        text.name = name;
        text.transform.SetParent(parent);
        
        if (textPrefab == null)
        {
            text.AddComponent<RectTransform>();
            text.AddComponent<TextMeshProUGUI>();
            text.GetComponent<TextMeshProUGUI>().text = name;
        }
        
        return text;
    }
    
    private GameObject CreateScrollView(string name, Transform parent)
    {
        GameObject scrollView = scrollViewPrefab != null ? Instantiate(scrollViewPrefab, parent) : new GameObject(name);
        scrollView.name = name;
        scrollView.transform.SetParent(parent);
        
        if (scrollViewPrefab == null)
        {
            scrollView.AddComponent<RectTransform>();
            scrollView.AddComponent<ScrollRect>();
            scrollView.AddComponent<Image>();
            
            GameObject viewport = new GameObject("Viewport");
            viewport.transform.SetParent(scrollView.transform);
            viewport.AddComponent<RectTransform>();
            viewport.AddComponent<Image>();
            viewport.AddComponent<Mask>();
            
            GameObject content = new GameObject("Content");
            content.transform.SetParent(viewport.transform);
            content.AddComponent<RectTransform>();
        }
        
        return scrollView;
    }
    
    private GameObject CreateInputField(string name, Transform parent)
    {
        GameObject inputField = inputFieldPrefab != null ? Instantiate(inputFieldPrefab, parent) : new GameObject(name);
        inputField.name = name;
        inputField.transform.SetParent(parent);
        
        if (inputFieldPrefab == null)
        {
            inputField.AddComponent<RectTransform>();
            inputField.AddComponent<Image>();
            TMP_InputField inputFieldComponent = inputField.AddComponent<TMP_InputField>();
            
            GameObject placeholder = new GameObject("Placeholder");
            placeholder.transform.SetParent(inputField.transform);
            RectTransform placeholderRect = placeholder.AddComponent<RectTransform>();
            placeholderRect.anchorMin = Vector2.zero;
            placeholderRect.anchorMax = Vector2.one;
            placeholderRect.sizeDelta = Vector2.zero;
            placeholderRect.anchoredPosition = Vector2.zero;
            TextMeshProUGUI placeholderText = placeholder.AddComponent<TextMeshProUGUI>();
            placeholderText.color = Color.gray;
            placeholderText.fontSize = 14;
            
            GameObject text = new GameObject("Text");
            text.transform.SetParent(inputField.transform);
            RectTransform textRect = text.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.anchoredPosition = Vector2.zero;
            TextMeshProUGUI textComponent = text.AddComponent<TextMeshProUGUI>();
            textComponent.color = Color.black;
            textComponent.fontSize = 14;
            
            // Configure the TMP_InputField to use these components
            inputFieldComponent.placeholder = placeholderText;
            inputFieldComponent.textComponent = textComponent;
        }
        
        return inputField;
    }
    
    private void SetInputFieldPlaceholder(GameObject inputFieldObject, string placeholderText)
    {
        TMP_InputField inputField = inputFieldObject.GetComponent<TMP_InputField>();
        if (inputField != null && inputField.placeholder != null)
        {
            ((TextMeshProUGUI)inputField.placeholder).text = placeholderText;
        }
    }
    
    private GameObject CreateDropdown(string name, Transform parent)
    {
        GameObject dropdown = dropdownPrefab != null ? Instantiate(dropdownPrefab, parent) : new GameObject(name);
        dropdown.name = name;
        dropdown.transform.SetParent(parent);
        
        if (dropdownPrefab == null)
        {
            dropdown.AddComponent<RectTransform>();
            dropdown.AddComponent<Image>();
            dropdown.AddComponent<TMP_Dropdown>();
            
            GameObject label = new GameObject("Label");
            label.transform.SetParent(dropdown.transform);
            label.AddComponent<RectTransform>();
            label.AddComponent<TextMeshProUGUI>();
            
            GameObject arrow = new GameObject("Arrow");
            arrow.transform.SetParent(dropdown.transform);
            arrow.AddComponent<RectTransform>();
            arrow.AddComponent<Image>();
            
            GameObject template = new GameObject("Template");
            template.transform.SetParent(dropdown.transform);
            template.AddComponent<RectTransform>();
            template.AddComponent<Image>();
        }
        
        return dropdown;
    }
    
    private GameObject CreateSlider(string name, Transform parent)
    {
        GameObject slider = sliderPrefab != null ? Instantiate(sliderPrefab, parent) : new GameObject(name);
        slider.name = name;
        slider.transform.SetParent(parent);
        
        if (sliderPrefab == null)
        {
            slider.AddComponent<RectTransform>();
            slider.AddComponent<Slider>();
            
            GameObject background = new GameObject("Background");
            background.transform.SetParent(slider.transform);
            background.AddComponent<RectTransform>();
            background.AddComponent<Image>();
            
            GameObject fillArea = new GameObject("Fill Area");
            fillArea.transform.SetParent(slider.transform);
            fillArea.AddComponent<RectTransform>();
            
            GameObject fill = new GameObject("Fill");
            fill.transform.SetParent(fillArea.transform);
            fill.AddComponent<RectTransform>();
            fill.AddComponent<Image>();
            
            GameObject handleArea = new GameObject("Handle Slide Area");
            handleArea.transform.SetParent(slider.transform);
            handleArea.AddComponent<RectTransform>();
            
            GameObject handle = new GameObject("Handle");
            handle.transform.SetParent(handleArea.transform);
            handle.AddComponent<RectTransform>();
            handle.AddComponent<Image>();
        }
        
        return slider;
    }
    #endregion
    
    #region Sample Data Generation
    private void GenerateSampleData()
    {
        Debug.Log("Generating Sample Data...");
        
        // Create sample adventurers
        CreateSampleAdventurers();
        
        // Create sample equipment
        CreateSampleEquipment();
        
        // Create sample materials
        CreateSampleMaterials();
        
        Debug.Log("✓ Sample Data Generated");
    }
    
    private void CreateSampleAdventurers()
    {
        // This would create sample adventurer data
        // In a real implementation, this would interface with the RecruitmentSystem
        Debug.Log("Creating sample adventurers...");
        
        string[] adventurerNames = { "Gareth", "Lyanna", "Thorin", "Elara", "Brom" };
        Adventurer.AdventurerClass[] classes = { 
            Adventurer.AdventurerClass.Warrior, 
            Adventurer.AdventurerClass.Mage, 
            Adventurer.AdventurerClass.Rogue, 
            Adventurer.AdventurerClass.Cleric, 
            Adventurer.AdventurerClass.Ranger 
        };
        
        for (int i = 0; i < adventurerNames.Length; i++)
        {
            Debug.Log($"  - {adventurerNames[i]} ({classes[i]})");
        }
    }
    
    private void CreateSampleEquipment()
    {
        Debug.Log("Creating sample equipment...");
        
        string[] weaponNames = { "Épée de Fer", "Bâton de Mage", "Dague d'Acier", "Masse Sacrée", "Arc Elfique" };
        string[] armorNames = { "Armure de Mailles", "Robe de Mage", "Armure de Cuir", "Robe Bénie", "Armure Légère" };
        
        foreach (string weapon in weaponNames)
        {
            Debug.Log($"  - {weapon}");
        }
        
        foreach (string armor in armorNames)
        {
            Debug.Log($"  - {armor}");
        }
    }
    
    private void CreateSampleMaterials()
    {
        Debug.Log("Creating sample materials...");
        
        string[] materialNames = { "Fer", "Cuir", "Bois", "Tissu", "Acier", "Mithril", "Cristal de Mana" };
        
        foreach (string material in materialNames)
        {
            Debug.Log($"  - {material}");
        }
    }
    #endregion
    
    #region Utility Methods
    [ContextMenu("Clear Generated Objects")]
    public void ClearGeneratedObjects()
    {
        int clearedCount = 0;
        for (int i = generatedObjects.Count - 1; i >= 0; i--)
        {
            if (generatedObjects[i] != null)
            {
                #if UNITY_EDITOR
                if (Application.isPlaying)
                {
                    Destroy(generatedObjects[i]);
                }
                else
                {
                    DestroyImmediate(generatedObjects[i]);
                }
                #else
                Destroy(generatedObjects[i]);
                #endif
                clearedCount++;
            }
        }
        generatedObjects.Clear();
        Debug.Log($"Cleared {clearedCount} generated objects");
    }
    
    [ContextMenu("Show Generated Objects Info")]
    public void ShowGeneratedObjectsInfo()
    {
        Debug.Log($"Generated Objects Count: {generatedObjects.Count}");
        
        foreach (GameObject obj in generatedObjects)
        {
            if (obj != null)
            {
                Debug.Log($"  - {obj.name} ({obj.GetComponents<Component>().Length} components)");
            }
        }
    }
    
    [ContextMenu("Organize Generated Objects")]
    public void OrganizeGeneratedObjects()
    {
        // Create organization hierarchy
        GameObject phase2Root = new GameObject("Phase 2 Systems");
        GameObject phase2_1 = new GameObject("Phase 2.1 - Adventurer System");
        GameObject phase2_2 = new GameObject("Phase 2.2 - Equipment & Progression");
        GameObject phase2_3 = new GameObject("Phase 2.3 - Adventurer Interface");
        
        phase2_1.transform.SetParent(phase2Root.transform);
        phase2_2.transform.SetParent(phase2Root.transform);
        phase2_3.transform.SetParent(phase2Root.transform);
        
        // Move objects to appropriate categories
        foreach (GameObject obj in generatedObjects)
        {
            if (obj != null)
            {
                if (obj.name.Contains("Adventurer") || obj.name.Contains("Recruitment") || obj.name.Contains("Party"))
                {
                    obj.transform.SetParent(phase2_1.transform);
                }
                else if (obj.name.Contains("Crafting") || obj.name.Contains("Item") || obj.name.Contains("Enhancement") || obj.name.Contains("Progression"))
                {
                    obj.transform.SetParent(phase2_2.transform);
                }
                else if (obj.name.Contains("Roster") || obj.name.Contains("Character") || obj.name.Contains("Equipment") || obj.name.Contains("Talent"))
                {
                    obj.transform.SetParent(phase2_3.transform);
                }
            }
        }
        
        generatedObjects.Add(phase2Root);
        Debug.Log("Organized generated objects into hierarchy");
    }
    
    public List<GameObject> GetGeneratedObjects()
    {
        return new List<GameObject>(generatedObjects);
    }
    
    public int GetGeneratedObjectCount()
    {
        return generatedObjects.Count;
    }
    
    #if UNITY_EDITOR
    [MenuItem("Grimspire/Generate Phase 2 Systems %#2")] // Ctrl+Shift+2
    public static void GeneratePhase2SystemsFromMenu()
    {
        // Check if generator already exists
        GameObjectGeneratorPhase2 existingGenerator = FindObjectOfType<GameObjectGeneratorPhase2>();
        if (existingGenerator != null)
        {
            existingGenerator.GenerateAllPhase2Systems();
            Debug.Log("Phase 2 Systems regenerated using existing generator.");
        }
        else
        {
            GameObject generatorObject = new GameObject("GameObjectGeneratorPhase2");
            GameObjectGeneratorPhase2 generator = generatorObject.AddComponent<GameObjectGeneratorPhase2>();
            generator.GenerateAllPhase2Systems();
            Debug.Log("Phase 2 Systems generated from menu. Generator object kept for organization.");
        }
    }
    
    [MenuItem("Grimspire/Clear Phase 2 Systems")]
    public static void ClearPhase2SystemsFromMenu()
    {
        GameObjectGeneratorPhase2 generator = FindObjectOfType<GameObjectGeneratorPhase2>();
        if (generator != null)
        {
            generator.ClearGeneratedObjects();
            Debug.Log("Phase 2 Systems cleared from menu.");
        }
        else
        {
            Debug.LogWarning("No GameObjectGeneratorPhase2 found to clear objects.");
        }
    }
    
    [MenuItem("Grimspire/Organize Phase 2 Systems")]
    public static void OrganizePhase2SystemsFromMenu()
    {
        GameObjectGeneratorPhase2 generator = FindObjectOfType<GameObjectGeneratorPhase2>();
        if (generator != null)
        {
            generator.OrganizeGeneratedObjects();
            Debug.Log("Phase 2 Systems organized from menu.");
        }
        else
        {
            Debug.LogWarning("No GameObjectGeneratorPhase2 found to organize objects.");
        }
    }
    #endif
    #endregion
}