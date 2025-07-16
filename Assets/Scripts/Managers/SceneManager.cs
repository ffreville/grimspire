using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    public enum GameScene
    {
        MainMenu,
        CityManagement,
        AdventurerMenu,
        MissionBoard,
        Settings,
        LoadGame
    }

    public static SceneManager Instance { get; private set; }
    
    [Header("Scene References")]
    [SerializeField] private string mainMenuScene = "MainMenu";
    [SerializeField] private string cityManagementScene = "CityManagement";
    [SerializeField] private string adventurerMenuScene = "AdventurerMenu";
    [SerializeField] private string missionBoardScene = "MissionBoard";
    [SerializeField] private string settingsScene = "Settings";
    [SerializeField] private string loadGameScene = "LoadGame";
    
    [Header("Loading")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private UnityEngine.UI.Slider loadingBar;
    
    private GameScene currentScene;
    private bool isLoading = false;

    public GameScene CurrentScene => currentScene;
    public bool IsLoading => isLoading;

    public static event Action<GameScene> OnSceneChanged;
    public static event Action<float> OnLoadingProgress;

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
        UpdateCurrentScene();
    }

    public void LoadScene(GameScene scene)
    {
        if (isLoading) return;
        
        StartCoroutine(LoadSceneAsync(scene));
    }

    private IEnumerator LoadSceneAsync(GameScene scene)
    {
        isLoading = true;
        
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true);
        }
        
        string sceneName = GetSceneName(scene);
        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        
        while (!asyncLoad.isDone)
        {
            float progress = asyncLoad.progress / 0.9f;
            
            if (loadingBar != null)
            {
                loadingBar.value = progress;
            }
            
            OnLoadingProgress?.Invoke(progress);
            yield return null;
        }
        
        currentScene = scene;
        OnSceneChanged?.Invoke(scene);
        
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(false);
        }
        
        isLoading = false;
    }

    private string GetSceneName(GameScene scene)
    {
        switch (scene)
        {
            case GameScene.MainMenu:
                return mainMenuScene;
            case GameScene.CityManagement:
                return cityManagementScene;
            case GameScene.AdventurerMenu:
                return adventurerMenuScene;
            case GameScene.MissionBoard:
                return missionBoardScene;
            case GameScene.Settings:
                return settingsScene;
            case GameScene.LoadGame:
                return loadGameScene;
            default:
                return mainMenuScene;
        }
    }

    private void UpdateCurrentScene()
    {
        string activeSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        
        if (activeSceneName == mainMenuScene)
            currentScene = GameScene.MainMenu;
        else if (activeSceneName == cityManagementScene)
            currentScene = GameScene.CityManagement;
        else if (activeSceneName == adventurerMenuScene)
            currentScene = GameScene.AdventurerMenu;
        else if (activeSceneName == missionBoardScene)
            currentScene = GameScene.MissionBoard;
        else if (activeSceneName == settingsScene)
            currentScene = GameScene.Settings;
        else if (activeSceneName == loadGameScene)
            currentScene = GameScene.LoadGame;
        else
            currentScene = GameScene.MainMenu;
    }

    public void ReturnToMainMenu()
    {
        LoadScene(GameScene.MainMenu);
    }

    public void LoadCityManagement()
    {
        LoadScene(GameScene.CityManagement);
    }

    public void LoadAdventurerMenu()
    {
        LoadScene(GameScene.AdventurerMenu);
    }

    public void LoadMissionBoard()
    {
        LoadScene(GameScene.MissionBoard);
    }

    public void LoadSettings()
    {
        LoadScene(GameScene.Settings);
    }

    public void LoadGameMenu()
    {
        LoadScene(GameScene.LoadGame);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}