using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIEventConfigurator : MonoBehaviour
{
    [Header("Configuration Settings")]
    [SerializeField] private bool configureOnStart = true;
    [SerializeField] private bool debugMode = true;
    [SerializeField] private float configurationDelay = 0.1f; // Délai pour s'assurer que tous les objets sont créés

    private void Start()
    {
        if (configureOnStart)
        {
            // Utiliser un délai pour s'assurer que tous les GameObjects sont créés
            Invoke(nameof(ConfigureAllUIEvents), configurationDelay);
        }
    }

    [ContextMenu("Configure All UI Events")]
    public void ConfigureAllUIEvents()
    {
        if (debugMode)
        {
            Debug.Log("=== Configuring UI Events ===");
        }

        // Configurer les événements du menu principal
        ConfigureMainMenuEvents();

        // Configurer les événements du nouveau jeu
        ConfigureNewGameEvents();

        // Configurer les événements de chargement
        ConfigureLoadGameEvents();

        // Configurer les événements des paramètres
        ConfigureSettingsEvents();

        if (debugMode)
        {
            Debug.Log("=== UI Events Configuration Complete ===");
        }
    }

    private void ConfigureMainMenuEvents()
    {
        // Configurer le bouton Nouvelle Partie
        Button newGameButton = FindButtonByName("NewGameButton");
        if (newGameButton != null)
        {
            newGameButton.onClick.RemoveAllListeners();
            newGameButton.onClick.AddListener(() => {
                if (debugMode) Debug.Log("New Game Button clicked!");
                ShowNewGamePanel();
            });
            
            if (debugMode) Debug.Log("✓ New Game Button configured");
        }
        else
        {
            Debug.LogWarning("✗ New Game Button not found");
        }

        // Configurer le bouton Charger Partie
        Button loadGameButton = FindButtonByName("LoadGameButton");
        if (loadGameButton != null)
        {
            loadGameButton.onClick.RemoveAllListeners();
            loadGameButton.onClick.AddListener(() => {
                if (debugMode) Debug.Log("Load Game Button clicked!");
                ShowLoadGamePanel();
            });
            
            if (debugMode) Debug.Log("✓ Load Game Button configured");
        }

        // Configurer le bouton Paramètres
        Button settingsButton = FindButtonByName("SettingsButton");
        if (settingsButton != null)
        {
            settingsButton.onClick.RemoveAllListeners();
            settingsButton.onClick.AddListener(() => {
                if (debugMode) Debug.Log("Settings Button clicked!");
                ShowSettingsPanel();
            });
            
            if (debugMode) Debug.Log("✓ Settings Button configured");
        }

        // Configurer le bouton Quitter
        Button quitButton = FindButtonByName("QuitButton");
        if (quitButton != null)
        {
            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(() => {
                if (debugMode) Debug.Log("Quit Button clicked!");
                QuitGame();
            });
            
            if (debugMode) Debug.Log("✓ Quit Button configured");
        }
    }

    private void ConfigureNewGameEvents()
    {
        // Configurer le bouton Commencer
        Button startNewGameButton = FindButtonByName("StartNewGameButton");
        if (startNewGameButton != null)
        {
            startNewGameButton.onClick.RemoveAllListeners();
            startNewGameButton.onClick.AddListener(() => {
                if (debugMode) Debug.Log("Start New Game Button clicked!");
                StartNewGame();
            });
            
            if (debugMode) Debug.Log("✓ Start New Game Button configured");
        }

        // Configurer le bouton Annuler
        Button cancelNewGameButton = FindButtonByName("CancelNewGameButton");
        if (cancelNewGameButton != null)
        {
            cancelNewGameButton.onClick.RemoveAllListeners();
            cancelNewGameButton.onClick.AddListener(() => {
                if (debugMode) Debug.Log("Cancel New Game Button clicked!");
                HideNewGamePanel();
            });
            
            if (debugMode) Debug.Log("✓ Cancel New Game Button configured");
        }
    }

    private void ConfigureLoadGameEvents()
    {
        // Configurer le bouton Annuler du chargement
        Button cancelLoadGameButton = FindButtonByName("CancelLoadGameButton");
        if (cancelLoadGameButton != null)
        {
            cancelLoadGameButton.onClick.RemoveAllListeners();
            cancelLoadGameButton.onClick.AddListener(() => {
                if (debugMode) Debug.Log("Cancel Load Game Button clicked!");
                HideLoadGamePanel();
            });
            
            if (debugMode) Debug.Log("✓ Cancel Load Game Button configured");
        }
    }

    private void ConfigureSettingsEvents()
    {
        // Configurer le bouton Fermer les paramètres
        Button closeSettingsButton = FindButtonByName("CloseSettingsButton");
        if (closeSettingsButton != null)
        {
            closeSettingsButton.onClick.RemoveAllListeners();
            closeSettingsButton.onClick.AddListener(() => {
                if (debugMode) Debug.Log("Close Settings Button clicked!");
                HideSettingsPanel();
            });
            
            if (debugMode) Debug.Log("✓ Close Settings Button configured");
        }
    }

    private Button FindButtonByName(string buttonName)
    {
        GameObject buttonObj = GameObject.Find(buttonName);
        if (buttonObj != null)
        {
            return buttonObj.GetComponent<Button>();
        }
        return null;
    }

    private GameObject FindPanelByName(string panelName)
    {
        return GameObject.Find(panelName);
    }

    // Méthodes d'action pour les boutons
    private void ShowNewGamePanel()
    {
        HideAllPanels();
        
        GameObject newGamePanel = FindPanelByName("NewGamePanel");
        if (newGamePanel != null)
        {
            newGamePanel.SetActive(true);
            
            // Configurer les valeurs par défaut
            TMP_InputField playerNameInput = GameObject.Find("PlayerNameInput")?.GetComponent<TMP_InputField>();
            if (playerNameInput != null && NewGameManager.Instance != null)
            {
                playerNameInput.text = NewGameManager.Instance.GetPlayerName();
            }
            
            TMP_InputField cityNameInput = GameObject.Find("CityNameInput")?.GetComponent<TMP_InputField>();
            if (cityNameInput != null && NewGameManager.Instance != null)
            {
                cityNameInput.text = NewGameManager.Instance.GetCityName();
            }
            
            if (debugMode) Debug.Log("New Game Panel shown");
        }
    }

    private void ShowLoadGamePanel()
    {
        HideAllPanels();
        
        GameObject loadGamePanel = FindPanelByName("LoadGamePanel");
        if (loadGamePanel != null)
        {
            loadGamePanel.SetActive(true);
            if (debugMode) Debug.Log("Load Game Panel shown");
        }
    }

    private void ShowSettingsPanel()
    {
        HideAllPanels();
        
        GameObject settingsPanel = FindPanelByName("SettingsPanel");
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            if (debugMode) Debug.Log("Settings Panel shown");
        }
    }

    private void HideNewGamePanel()
    {
        GameObject newGamePanel = FindPanelByName("NewGamePanel");
        if (newGamePanel != null)
        {
            newGamePanel.SetActive(false);
            if (debugMode) Debug.Log("New Game Panel hidden");
        }
    }

    private void HideLoadGamePanel()
    {
        GameObject loadGamePanel = FindPanelByName("LoadGamePanel");
        if (loadGamePanel != null)
        {
            loadGamePanel.SetActive(false);
            if (debugMode) Debug.Log("Load Game Panel hidden");
        }
    }

    private void HideSettingsPanel()
    {
        GameObject settingsPanel = FindPanelByName("SettingsPanel");
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
            if (debugMode) Debug.Log("Settings Panel hidden");
        }
    }

    private void HideAllPanels()
    {
        HideNewGamePanel();
        HideLoadGamePanel();
        HideSettingsPanel();
    }

    private void StartNewGame()
    {
        // Récupérer les valeurs des champs
        string playerName = "Seigneur";
        string cityName = "Grimspire";

        TMP_InputField playerNameInput = GameObject.Find("PlayerNameInput")?.GetComponent<TMP_InputField>();
        if (playerNameInput != null && !string.IsNullOrEmpty(playerNameInput.text))
        {
            playerName = playerNameInput.text;
        }

        TMP_InputField cityNameInput = GameObject.Find("CityNameInput")?.GetComponent<TMP_InputField>();
        if (cityNameInput != null && !string.IsNullOrEmpty(cityNameInput.text))
        {
            cityName = cityNameInput.text;
        }

        // Démarrer le nouveau jeu
        if (NewGameManager.Instance != null)
        {
            NewGameManager.Instance.SetPlayerName(playerName);
            NewGameManager.Instance.SetCityName(cityName);
            NewGameManager.Instance.StartNewGame();
            
            if (debugMode) Debug.Log($"New game started - Player: {playerName}, City: {cityName}");
        }
        else
        {
            Debug.LogError("NewGameManager not found!");
        }
    }

    private void QuitGame()
    {
        if (debugMode) Debug.Log("Quitting game...");
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    // Méthodes utilitaires pour les tests
    [ContextMenu("Test New Game Button")]
    public void TestNewGameButton()
    {
        Button newGameButton = FindButtonByName("NewGameButton");
        if (newGameButton != null)
        {
            Debug.Log("Testing New Game Button click...");
            newGameButton.onClick.Invoke();
        }
        else
        {
            Debug.LogError("New Game Button not found!");
        }
    }

    [ContextMenu("Test All Buttons")]
    public void TestAllButtons()
    {
        Debug.Log("=== Testing All Buttons ===");
        
        string[] buttonNames = { "NewGameButton", "LoadGameButton", "SettingsButton", "QuitButton" };
        
        foreach (string buttonName in buttonNames)
        {
            Button button = FindButtonByName(buttonName);
            if (button != null)
            {
                Debug.Log($"✓ {buttonName} found and has {button.onClick.GetPersistentEventCount()} listeners");
            }
            else
            {
                Debug.LogWarning($"✗ {buttonName} not found");
            }
        }
    }

    [ContextMenu("Debug UI Structure")]
    public void DebugUIStructure()
    {
        Debug.Log("=== UI Structure Debug ===");
        
        string[] objectNames = { 
            "NewGameButton", "LoadGameButton", "SettingsButton", "QuitButton",
            "NewGamePanel", "LoadGamePanel", "SettingsPanel",
            "StartNewGameButton", "CancelNewGameButton",
            "PlayerNameInput", "CityNameInput"
        };
        
        foreach (string objectName in objectNames)
        {
            GameObject obj = GameObject.Find(objectName);
            if (obj != null)
            {
                Debug.Log($"✓ {objectName} found - Active: {obj.activeInHierarchy}");
            }
            else
            {
                Debug.LogWarning($"✗ {objectName} not found");
            }
        }
    }
}