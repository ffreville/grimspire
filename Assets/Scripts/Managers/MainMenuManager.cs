using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    private static MainMenuManager instance;
    public static MainMenuManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<MainMenuManager>();
            }
            return instance;
        }
    }

    [Header("Main Menu Buttons")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button creditsButton;

    [Header("New Game Panel")]
    [SerializeField] private GameObject newGamePanel;
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private TMP_InputField cityNameInput;
    [SerializeField] private Button startNewGameButton;
    [SerializeField] private Button cancelNewGameButton;

    [Header("Load Game Panel")]
    [SerializeField] private GameObject loadGamePanel;
    [SerializeField] private Transform saveFilesList;
    [SerializeField] private GameObject saveFileItemPrefab;
    [SerializeField] private Button cancelLoadGameButton;

    [Header("Settings Panel")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Button closeSettingsButton;

    [Header("Version Info")]
    [SerializeField] private TextMeshProUGUI versionText;

    public System.Action OnNewGameStarted;
    public System.Action OnGameLoaded;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeMainMenu();
    }

    private void InitializeMainMenu()
    {
        // Auto-find UI elements if not assigned
        AutoFindUIElements();
        
        // Setup button listeners
        if (newGameButton != null)
            newGameButton.onClick.AddListener(ShowNewGamePanel);
        
        if (loadGameButton != null)
            loadGameButton.onClick.AddListener(ShowLoadGamePanel);
        
        if (settingsButton != null)
            settingsButton.onClick.AddListener(ShowSettingsPanel);
        
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
        
        if (creditsButton != null)
            creditsButton.onClick.AddListener(ShowCredits);

        // Setup new game panel
        if (startNewGameButton != null)
            startNewGameButton.onClick.AddListener(StartNewGame);
        
        if (cancelNewGameButton != null)
            cancelNewGameButton.onClick.AddListener(HideNewGamePanel);

        // Setup load game panel
        if (cancelLoadGameButton != null)
            cancelLoadGameButton.onClick.AddListener(HideLoadGamePanel);

        // Setup settings panel
        if (closeSettingsButton != null)
            closeSettingsButton.onClick.AddListener(HideSettingsPanel);

        // Initialize panels as hidden
        HideAllPanels();

        // Setup version info
        if (versionText != null)
            versionText.text = $"Version Phase 2.4 - {System.DateTime.Now.Year}";

        // Check if load game should be enabled
        UpdateLoadGameButton();

        Debug.Log("Main menu initialized");
    }

    private void AutoFindUIElements()
    {
        // Find buttons by name if not assigned
        if (newGameButton == null)
            newGameButton = GameObject.Find("NewGameButton")?.GetComponent<Button>();
        
        if (loadGameButton == null)
            loadGameButton = GameObject.Find("LoadGameButton")?.GetComponent<Button>();
        
        if (settingsButton == null)
            settingsButton = GameObject.Find("SettingsButton")?.GetComponent<Button>();
        
        if (quitButton == null)
            quitButton = GameObject.Find("QuitButton")?.GetComponent<Button>();
        
        if (creditsButton == null)
            creditsButton = GameObject.Find("CreditsButton")?.GetComponent<Button>();

        // Find panels by name if not assigned
        if (newGamePanel == null)
            newGamePanel = GameObject.Find("NewGamePanel");
        
        if (loadGamePanel == null)
            loadGamePanel = GameObject.Find("LoadGamePanel");
        
        if (settingsPanel == null)
            settingsPanel = GameObject.Find("SettingsPanel");

        // Find new game UI elements
        if (startNewGameButton == null)
            startNewGameButton = GameObject.Find("StartNewGameButton")?.GetComponent<Button>();
        
        if (cancelNewGameButton == null)
            cancelNewGameButton = GameObject.Find("CancelNewGameButton")?.GetComponent<Button>();
        
        if (cancelLoadGameButton == null)
            cancelLoadGameButton = GameObject.Find("CancelLoadGameButton")?.GetComponent<Button>();
        
        if (closeSettingsButton == null)
            closeSettingsButton = GameObject.Find("CloseSettingsButton")?.GetComponent<Button>();

        // Find input fields
        if (playerNameInput == null)
            playerNameInput = GameObject.Find("PlayerNameInput")?.GetComponent<TMP_InputField>();
        
        if (cityNameInput == null)
            cityNameInput = GameObject.Find("CityNameInput")?.GetComponent<TMP_InputField>();

        // Find text elements
        if (versionText == null)
            versionText = GameObject.Find("VersionText")?.GetComponent<TextMeshProUGUI>();

        // Find save files list
        if (saveFilesList == null)
            saveFilesList = GameObject.Find("SaveFilesList")?.transform;

        // Log what was found
        Debug.Log($"Main Menu UI Elements Found:");
        Debug.Log($"  New Game Button: {(newGameButton != null ? "✓" : "✗")}");
        Debug.Log($"  Load Game Button: {(loadGameButton != null ? "✓" : "✗")}");
        Debug.Log($"  Settings Button: {(settingsButton != null ? "✓" : "✗")}");
        Debug.Log($"  Quit Button: {(quitButton != null ? "✓" : "✗")}");
        Debug.Log($"  New Game Panel: {(newGamePanel != null ? "✓" : "✗")}");
        Debug.Log($"  Start New Game Button: {(startNewGameButton != null ? "✓" : "✗")}");
    }

    private void ShowNewGamePanel()
    {
        HideAllPanels();
        
        if (newGamePanel != null)
        {
            newGamePanel.SetActive(true);
            
            // Set default values
            if (playerNameInput != null)
                playerNameInput.text = NewGameManager.Instance.GetPlayerName();
            
            if (cityNameInput != null)
                cityNameInput.text = NewGameManager.Instance.GetCityName();
            
            // Focus on player name input
            if (playerNameInput != null)
                playerNameInput.Select();
        }
    }

    private void HideNewGamePanel()
    {
        if (newGamePanel != null)
            newGamePanel.SetActive(false);
    }

    private void StartNewGame()
    {
        // Get input values
        string playerName = playerNameInput?.text ?? "Seigneur";
        string cityName = cityNameInput?.text ?? "Grimspire";

        // Validate inputs
        if (string.IsNullOrEmpty(playerName.Trim()))
        {
            playerName = "Seigneur";
        }

        if (string.IsNullOrEmpty(cityName.Trim()))
        {
            cityName = "Grimspire";
        }

        // Set names in NewGameManager
        NewGameManager.Instance.SetPlayerName(playerName);
        NewGameManager.Instance.SetCityName(cityName);

        // Start new game
        NewGameManager.Instance.StartNewGame();
        
        // Fire event
        OnNewGameStarted?.Invoke();
        
        Debug.Log($"Starting new game - Player: {playerName}, City: {cityName}");
    }

    private void ShowLoadGamePanel()
    {
        HideAllPanels();
        
        if (loadGamePanel != null)
        {
            loadGamePanel.SetActive(true);
            RefreshSaveFilesList();
        }
    }

    private void HideLoadGamePanel()
    {
        if (loadGamePanel != null)
            loadGamePanel.SetActive(false);
    }

    private void RefreshSaveFilesList()
    {
        if (saveFilesList == null) return;

        // Clear existing items
        foreach (Transform child in saveFilesList)
        {
            Destroy(child.gameObject);
        }

        // Get save files
        var saveFiles = NewGameManager.Instance.GetSaveFiles();
        
        if (saveFiles.Count == 0)
        {
            // Show "No save files" message
            CreateNoSaveFilesMessage();
        }
        else
        {
            // Create save file items
            foreach (string saveFile in saveFiles)
            {
                CreateSaveFileItem(saveFile);
            }
        }
    }

    private void CreateNoSaveFilesMessage()
    {
        GameObject messageObject = new GameObject("NoSaveFilesMessage");
        messageObject.transform.SetParent(saveFilesList);
        
        var text = messageObject.AddComponent<TextMeshProUGUI>();
        text.text = "Aucune sauvegarde trouvée";
        text.color = Color.gray;
        text.fontSize = 18;
        text.alignment = TextAlignmentOptions.Center;
        
        var rectTransform = messageObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(400, 50);
    }

    private void CreateSaveFileItem(string saveFileName)
    {
        GameObject saveItem;
        
        if (saveFileItemPrefab != null)
        {
            saveItem = Instantiate(saveFileItemPrefab, saveFilesList);
        }
        else
        {
            saveItem = new GameObject($"SaveFile_{saveFileName}");
            saveItem.transform.SetParent(saveFilesList);
            
            // Add basic UI components
            var rectTransform = saveItem.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(400, 60);
            
            var button = saveItem.AddComponent<Button>();
            var image = saveItem.AddComponent<Image>();
            image.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            
            // Add text
            var textObject = new GameObject("Text");
            textObject.transform.SetParent(saveItem.transform);
            var text = textObject.AddComponent<TextMeshProUGUI>();
            text.text = saveFileName;
            text.color = Color.white;
            text.fontSize = 16;
            text.alignment = TextAlignmentOptions.Center;
            
            var textRect = textObject.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.anchoredPosition = Vector2.zero;
            
            // Setup button
            button.onClick.AddListener(() => LoadGame(saveFileName));
        }
    }

    private void LoadGame(string saveFileName)
    {
        NewGameManager.Instance.LoadGame(saveFileName);
        OnGameLoaded?.Invoke();
        
        Debug.Log($"Loading game: {saveFileName}");
    }

    private void ShowSettingsPanel()
    {
        HideAllPanels();
        
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }

    private void HideSettingsPanel()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    private void ShowCredits()
    {
        Debug.Log("Showing credits - Grimspire © 2024 Frédéric Fréville");
        // TODO: Implement credits screen
    }

    private void QuitGame()
    {
        Debug.Log("Quitting game...");
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    private void HideAllPanels()
    {
        if (newGamePanel != null)
            newGamePanel.SetActive(false);
        
        if (loadGamePanel != null)
            loadGamePanel.SetActive(false);
        
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    private void UpdateLoadGameButton()
    {
        if (loadGameButton != null)
        {
            loadGameButton.interactable = NewGameManager.Instance.HasSaveFiles();
        }
    }

    public void ShowMainMenu()
    {
        HideAllPanels();
        gameObject.SetActive(true);
    }

    public void HideMainMenu()
    {
        gameObject.SetActive(false);
    }

    public void RefreshMenu()
    {
        UpdateLoadGameButton();
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}