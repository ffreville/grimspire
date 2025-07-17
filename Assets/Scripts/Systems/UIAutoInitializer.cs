using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIAutoInitializer : MonoBehaviour
{
    [Header("Auto-initialization Settings")]
    [SerializeField] private bool initializeOnStart = true;
    [SerializeField] private bool debugMode = true;

    private void Start()
    {
        if (initializeOnStart)
        {
            InitializeUI();
        }
    }

    [ContextMenu("Initialize UI")]
    public void InitializeUI()
    {
        Debug.Log("=== Starting UI Auto-Initialization ===");
        
        // Initialize managers
        InitializeManagers();
        
        // Initialize main menu
        InitializeMainMenu();
        
        // Initialize canvas manager
        InitializeCanvasManager();
        
        Debug.Log("=== UI Auto-Initialization Complete ===");
    }

    private void InitializeManagers()
    {
        // Ensure CanvasManager exists and is properly initialized
        if (CanvasManager.Instance == null)
        {
            GameObject canvasManagerObj = GameObject.Find("CanvasManager");
            if (canvasManagerObj == null)
            {
                canvasManagerObj = new GameObject("CanvasManager");
                canvasManagerObj.AddComponent<CanvasManager>();
            }
            else if (canvasManagerObj.GetComponent<CanvasManager>() == null)
            {
                canvasManagerObj.AddComponent<CanvasManager>();
            }
        }

        // Ensure NewGameManager exists
        if (NewGameManager.Instance == null)
        {
            GameObject newGameManagerObj = GameObject.Find("NewGameManager");
            if (newGameManagerObj == null)
            {
                newGameManagerObj = new GameObject("NewGameManager");
                newGameManagerObj.AddComponent<NewGameManager>();
            }
            else if (newGameManagerObj.GetComponent<NewGameManager>() == null)
            {
                newGameManagerObj.AddComponent<NewGameManager>();
            }
        }

        // Ensure MainMenuManager exists
        if (MainMenuManager.Instance == null)
        {
            GameObject mainMenuManagerObj = GameObject.Find("MainMenuManager");
            if (mainMenuManagerObj == null)
            {
                mainMenuManagerObj = new GameObject("MainMenuManager");
                mainMenuManagerObj.AddComponent<MainMenuManager>();
            }
            else if (mainMenuManagerObj.GetComponent<MainMenuManager>() == null)
            {
                mainMenuManagerObj.AddComponent<MainMenuManager>();
            }
        }

        // Ensure GameIntegrationManager exists
        if (GameIntegrationManager.Instance == null)
        {
            GameObject gameIntegrationObj = GameObject.Find("GameIntegrationManager");
            if (gameIntegrationObj == null)
            {
                gameIntegrationObj = new GameObject("GameIntegrationManager");
                gameIntegrationObj.AddComponent<GameIntegrationManager>();
            }
            else if (gameIntegrationObj.GetComponent<GameIntegrationManager>() == null)
            {
                gameIntegrationObj.AddComponent<GameIntegrationManager>();
            }
        }

        if (debugMode)
        {
            Debug.Log("✓ Managers initialized");
        }
    }

    private void InitializeMainMenu()
    {
        // Find and setup main menu buttons
        SetupButton("NewGameButton", "Nouvelle Partie");
        SetupButton("LoadGameButton", "Charger Partie");
        SetupButton("SettingsButton", "Paramètres");
        SetupButton("QuitButton", "Quitter");
        
        // Setup new game panel buttons
        SetupButton("StartNewGameButton", "Commencer");
        SetupButton("CancelNewGameButton", "Annuler");

        // Setup input fields
        SetupInputField("PlayerNameInput", "Seigneur");
        SetupInputField("CityNameInput", "Grimspire");

        // Setup panels
        SetupPanel("NewGamePanel", false);
        SetupPanel("LoadGamePanel", false);
        SetupPanel("SettingsPanel", false);

        if (debugMode)
        {
            Debug.Log("✓ Main menu UI elements initialized");
        }
    }

    private void InitializeCanvasManager()
    {
        // Make sure main menu canvas is properly registered
        GameObject mainMenuCanvas = GameObject.Find("MainMenuCanvas");
        if (mainMenuCanvas != null && CanvasManager.Instance != null)
        {
            Canvas canvas = mainMenuCanvas.GetComponent<Canvas>();
            if (canvas != null)
            {
                CanvasManager.Instance.RegisterCustomCanvas("MainMenu", canvas);
            }
        }

        if (debugMode)
        {
            Debug.Log("✓ Canvas manager initialized");
        }
    }

    private void SetupButton(string buttonName, string buttonText)
    {
        GameObject buttonObj = GameObject.Find(buttonName);
        if (buttonObj != null)
        {
            Button button = buttonObj.GetComponent<Button>();
            if (button == null)
            {
                button = buttonObj.AddComponent<Button>();
            }

            // Setup button image if missing
            Image buttonImage = buttonObj.GetComponent<Image>();
            if (buttonImage == null)
            {
                buttonImage = buttonObj.AddComponent<Image>();
                buttonImage.color = new Color(0.2f, 0.3f, 0.5f, 1f);
            }

            // Find or create text component
            TextMeshProUGUI buttonTextComponent = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonTextComponent == null)
            {
                GameObject textObj = new GameObject("Text");
                textObj.transform.SetParent(buttonObj.transform);
                
                RectTransform textRect = textObj.AddComponent<RectTransform>();
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.sizeDelta = Vector2.zero;
                textRect.anchoredPosition = Vector2.zero;
                
                buttonTextComponent = textObj.AddComponent<TextMeshProUGUI>();
                buttonTextComponent.color = Color.white;
                buttonTextComponent.alignment = TextAlignmentOptions.Center;
                buttonTextComponent.fontSize = 18;
            }

            if (buttonTextComponent != null)
            {
                buttonTextComponent.text = buttonText;
            }

            if (debugMode)
            {
                Debug.Log($"✓ Setup button: {buttonName}");
            }
        }
        else if (debugMode)
        {
            Debug.LogWarning($"✗ Button not found: {buttonName}");
        }
    }

    private void SetupInputField(string inputFieldName, string placeholder)
    {
        GameObject inputFieldObj = GameObject.Find(inputFieldName);
        if (inputFieldObj != null)
        {
            TMP_InputField inputField = inputFieldObj.GetComponent<TMP_InputField>();
            if (inputField == null)
            {
                inputField = inputFieldObj.AddComponent<TMP_InputField>();
            }

            // Setup input field image if missing
            Image inputFieldImage = inputFieldObj.GetComponent<Image>();
            if (inputFieldImage == null)
            {
                inputFieldImage = inputFieldObj.AddComponent<Image>();
                inputFieldImage.color = new Color(0.9f, 0.9f, 0.9f, 1f);
            }

            // Setup placeholder text
            Transform placeholderTransform = inputFieldObj.transform.Find("Placeholder");
            if (placeholderTransform == null)
            {
                GameObject placeholderObj = new GameObject("Placeholder");
                placeholderObj.transform.SetParent(inputFieldObj.transform);
                
                RectTransform placeholderRect = placeholderObj.AddComponent<RectTransform>();
                placeholderRect.anchorMin = Vector2.zero;
                placeholderRect.anchorMax = Vector2.one;
                placeholderRect.sizeDelta = Vector2.zero;
                placeholderRect.anchoredPosition = Vector2.zero;
                
                TextMeshProUGUI placeholderText = placeholderObj.AddComponent<TextMeshProUGUI>();
                placeholderText.text = placeholder;
                placeholderText.color = Color.gray;
                placeholderText.fontSize = 14;
                
                inputField.placeholder = placeholderText;
            }

            if (debugMode)
            {
                Debug.Log($"✓ Setup input field: {inputFieldName}");
            }
        }
        else if (debugMode)
        {
            Debug.LogWarning($"✗ Input field not found: {inputFieldName}");
        }
    }

    private void SetupPanel(string panelName, bool startActive)
    {
        GameObject panelObj = GameObject.Find(panelName);
        if (panelObj != null)
        {
            panelObj.SetActive(startActive);
            
            if (debugMode)
            {
                Debug.Log($"✓ Setup panel: {panelName} (active: {startActive})");
            }
        }
        else if (debugMode)
        {
            Debug.LogWarning($"✗ Panel not found: {panelName}");
        }
    }

    [ContextMenu("Test New Game Flow")]
    public void TestNewGameFlow()
    {
        Debug.Log("=== Testing New Game Flow ===");
        
        // Test button click
        GameObject newGameButton = GameObject.Find("NewGameButton");
        if (newGameButton != null)
        {
            Button button = newGameButton.GetComponent<Button>();
            if (button != null)
            {
                Debug.Log("Simulating new game button click...");
                button.onClick.Invoke();
            }
        }
        
        // Test new game manager
        if (NewGameManager.Instance != null)
        {
            Debug.Log("Testing new game creation...");
            NewGameManager.Instance.CreateQuickStartGame();
        }
        
        Debug.Log("=== New Game Flow Test Complete ===");
    }
}