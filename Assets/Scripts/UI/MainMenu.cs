using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("UI Elements")]
    public Button newGameButton;
    public Button loadGameButton;
    public Button settingsButton;
    public Button quitButton;

    public GameObject gameMenu;

    private void Awake()
    {
        SetupButtons();
    }

    private void SetupButtons()
    {
        newGameButton?.onClick.AddListener(StartNewGame);
        loadGameButton?.onClick.AddListener(LoadGame);
        settingsButton?.onClick.AddListener(OpenSettings);
        quitButton?.onClick.AddListener(QuitGame);
    }

    private void StartNewGame()
    {
        Debug.Log("Starting new game...");
        
        // Hide main menu
        gameObject.SetActive(false);
        
        // Find and show existing game menu
        if (gameMenu != null)
        {
            gameMenu.SetActive(true);
            
            // Initialize the game menu if needed
            var gameMenuScript = gameMenu.GetComponent<GameMenu>();
            gameMenuScript?.SetActiveTab(0); // Start with Buildings tab
        }
        else
        {
            Debug.LogWarning("GameMenu not found in scene! Please add it to the scene first.");
        }
    }

    private void LoadGame()
    {
        Debug.Log("Loading game...");
        // TODO: Implement load game functionality
    }

    private void OpenSettings()
    {
        Debug.Log("Opening settings...");
        // TODO: Implement settings menu
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
}
