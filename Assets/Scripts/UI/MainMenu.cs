using UnityEngine;
using UnityEngine.UI;

public class MainMenu : BaseMenu
{
    [Header("Main Menu Buttons")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;
    
    [Header("Main Menu Panels")]
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private GameObject buttonPanel;

    protected override void SetupEventListeners()
    {
        base.SetupEventListeners();
        
        if (newGameButton != null)
            newGameButton.onClick.AddListener(OnNewGamePressed);
        
        if (loadGameButton != null)
            loadGameButton.onClick.AddListener(OnLoadGamePressed);
        
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettingsPressed);
        
        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitPressed);
    }

    protected override void OnMenuShown()
    {
        base.OnMenuShown();
        
        // Update load game button availability
        UpdateLoadGameButton();
    }

    private void UpdateLoadGameButton()
    {
        if (loadGameButton != null && SaveSystem.Instance != null)
        {
            loadGameButton.interactable = SaveSystem.Instance.HasSaveFile();
        }
    }

    private void OnNewGamePressed()
    {
        Debug.Log("New Game pressed");
        StartNewGame();
    }

    private void OnLoadGamePressed()
    {
        Debug.Log("Load Game pressed");
        LoadGame();
    }

    private void OnSettingsPressed()
    {
        Debug.Log("Settings pressed");
        OpenSettings();
    }

    private void OnQuitPressed()
    {
        Debug.Log("Quit pressed");
        QuitGame();
    }

    private void StartNewGame()
    {
        // Create new game data
        GameData newGameData = new GameData();
        newGameData.InitializeGameVersion();
        
        // Save the new game
        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.SaveGame(newGameData);
        }
        
        // Initialize game manager with new data
        if (GameManager.Instance != null)
        {
            GameManager.Instance.InitializeGame(newGameData);
        }
        
        // Load city management scene
        if (SceneManager.Instance != null)
        {
            SceneManager.Instance.LoadCityManagement();
        }
    }

    private void LoadGame()
    {
        if (SaveSystem.Instance != null)
        {
            GameData loadedData = SaveSystem.Instance.LoadGame();
            
            if (loadedData != null)
            {
                // Initialize game manager with loaded data
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.InitializeGame(loadedData);
                }
                
                // Load city management scene
                if (SceneManager.Instance != null)
                {
                    SceneManager.Instance.LoadCityManagement();
                }
            }
        }
    }

    private void OpenSettings()
    {
        if (SceneManager.Instance != null)
        {
            SceneManager.Instance.LoadSettings();
        }
    }

    private void QuitGame()
    {
        if (SceneManager.Instance != null)
        {
            SceneManager.Instance.QuitGame();
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        
        if (newGameButton != null)
            newGameButton.onClick.RemoveListener(OnNewGamePressed);
        
        if (loadGameButton != null)
            loadGameButton.onClick.RemoveListener(OnLoadGamePressed);
        
        if (settingsButton != null)
            settingsButton.onClick.RemoveListener(OnSettingsPressed);
        
        if (quitButton != null)
            quitButton.onClick.RemoveListener(OnQuitPressed);
    }
}