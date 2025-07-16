using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameObjectGenerator : MonoBehaviour
{
    [Header("Generation Settings")]
    [SerializeField] private bool generateOnStart = false;
    [SerializeField] private bool clearExistingObjects = true;
    
    [Header("UI Settings")]
    [SerializeField] private Canvas parentCanvas;
    [SerializeField] private Font defaultFont;
    [SerializeField] private Material defaultUIMaterial;

    private void Start()
    {
        if (generateOnStart)
        {
            GenerateAllGameObjects();
        }
    }

    [ContextMenu("Generate All GameObjects")]
    public void GenerateAllGameObjects()
    {
        if (clearExistingObjects)
        {
            ClearExistingObjects();
        }
        
        GenerateManagerObjects();
        GenerateUIObjects();
        GenerateEventSystem();
        
        Debug.Log("All GameObjects for Phase 1.1 generated successfully!");
    }

    private void ClearExistingObjects()
    {
        // Clear existing managers (except this one)
        var existingManagers = FindObjectsOfType<MonoBehaviour>();
        foreach (var manager in existingManagers)
        {
            if (manager != this && 
                (manager is GameManager || manager is SaveSystem || 
                 manager is SceneManager || manager is MenuManager))
            {
                DestroyImmediate(manager.gameObject);
            }
        }
    }

    private void GenerateManagerObjects()
    {
        // Generate GameManager
        GameObject gameManagerObject = new GameObject("GameManager");
        gameManagerObject.AddComponent<GameManager>();
        if (Application.isPlaying)
        {
            DontDestroyOnLoad(gameManagerObject);
        }
        
        // Generate SaveSystem
        GameObject saveSystemObject = new GameObject("SaveSystem");
        saveSystemObject.AddComponent<SaveSystem>();
        if (Application.isPlaying)
        {
            DontDestroyOnLoad(saveSystemObject);
        }
        
        // Generate SceneManager
        GameObject sceneManagerObject = new GameObject("SceneManager");
        sceneManagerObject.AddComponent<SceneManager>();
        if (Application.isPlaying)
        {
            DontDestroyOnLoad(sceneManagerObject);
        }
        
        // Generate MenuManager
        GameObject menuManagerObject = new GameObject("MenuManager");
        menuManagerObject.AddComponent<MenuManager>();
        if (Application.isPlaying)
        {
            DontDestroyOnLoad(menuManagerObject);
        }
        
        // Generate ResourceManager
        GameObject resourceManagerObject = new GameObject("ResourceManager");
        resourceManagerObject.AddComponent<ResourceManager>();
        if (Application.isPlaying)
        {
            DontDestroyOnLoad(resourceManagerObject);
        }
        
        // Generate EconomicSystem
        GameObject economicSystemObject = new GameObject("EconomicSystem");
        economicSystemObject.AddComponent<EconomicSystem>();
        if (Application.isPlaying)
        {
            DontDestroyOnLoad(economicSystemObject);
        }
        
        // Generate ConstructionSystem
        GameObject constructionSystemObject = new GameObject("ConstructionSystem");
        constructionSystemObject.AddComponent<ConstructionSystem>();
        if (Application.isPlaying)
        {
            DontDestroyOnLoad(constructionSystemObject);
        }
        
        // Generate BuildingEffectsSystem
        GameObject buildingEffectsObject = new GameObject("BuildingEffectsSystem");
        buildingEffectsObject.AddComponent<BuildingEffectsSystem>();
        if (Application.isPlaying)
        {
            DontDestroyOnLoad(buildingEffectsObject);
        }
        
        // Generate RecruitmentSystem
        GameObject recruitmentSystemObject = new GameObject("RecruitmentSystem");
        recruitmentSystemObject.AddComponent<RecruitmentSystem>();
        if (Application.isPlaying)
        {
            DontDestroyOnLoad(recruitmentSystemObject);
        }
        
        // Generate PartyManagementSystem
        GameObject partyManagementObject = new GameObject("PartyManagementSystem");
        partyManagementObject.AddComponent<PartyManagementSystem>();
        if (Application.isPlaying)
        {
            DontDestroyOnLoad(partyManagementObject);
        }
        
        Debug.Log("Manager objects generated successfully!");
    }

    private void GenerateUIObjects()
    {
        // Create main UI canvas if it doesn't exist
        if (parentCanvas == null)
        {
            parentCanvas = CreateMainCanvas();
        }
        
        // Generate Main Menu
        GameObject mainMenuObject = CreateMainMenu();
        
        // Generate Loading Screen
        GameObject loadingScreenObject = CreateLoadingScreen();
        
        Debug.Log("UI objects generated successfully!");
    }

    private Canvas CreateMainCanvas()
    {
        GameObject canvasObject = new GameObject("Main Canvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 0;
        
        CanvasScaler canvasScaler = canvasObject.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920, 1080);
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        canvasScaler.matchWidthOrHeight = 0.5f;
        
        canvasObject.AddComponent<GraphicRaycaster>();
        
        return canvas;
    }

    private GameObject CreateMainMenu()
    {
        GameObject mainMenuObject = new GameObject("MainMenu");
        mainMenuObject.transform.SetParent(parentCanvas.transform, false);
        
        // Add RectTransform and set to full screen
        RectTransform rectTransform = mainMenuObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.sizeDelta = Vector2.zero;
        rectTransform.anchoredPosition = Vector2.zero;
        
        // Add MainMenu component
        MainMenu mainMenu = mainMenuObject.AddComponent<MainMenu>();
        
        // Add Canvas and CanvasGroup
        Canvas canvas = mainMenuObject.AddComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = 1;
        
        CanvasGroup canvasGroup = mainMenuObject.AddComponent<CanvasGroup>();
        mainMenuObject.AddComponent<GraphicRaycaster>();
        
        // Create background
        GameObject backgroundObject = CreateUIBackground(mainMenuObject, "MenuBackground");
        
        // Create title panel
        GameObject titlePanel = CreateTitlePanel(mainMenuObject);
        
        // Create button panel
        GameObject buttonPanel = CreateButtonPanel(mainMenuObject);
        
        // Create loading panel
        GameObject loadingPanel = CreateLoadingPanel(mainMenuObject);
        loadingPanel.SetActive(false);
        
        return mainMenuObject;
    }

    private GameObject CreateUIBackground(GameObject parent, string name)
    {
        GameObject backgroundObject = new GameObject(name);
        backgroundObject.transform.SetParent(parent.transform, false);
        
        RectTransform rectTransform = backgroundObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.sizeDelta = Vector2.zero;
        rectTransform.anchoredPosition = Vector2.zero;
        
        Image backgroundImage = backgroundObject.AddComponent<Image>();
        backgroundImage.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        
        return backgroundObject;
    }

    private GameObject CreateTitlePanel(GameObject parent)
    {
        GameObject titlePanel = new GameObject("TitlePanel");
        titlePanel.transform.SetParent(parent.transform, false);
        
        RectTransform rectTransform = titlePanel.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.7f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.9f);
        rectTransform.sizeDelta = new Vector2(600, 200);
        rectTransform.anchoredPosition = Vector2.zero;
        
        // Create title text
        GameObject titleText = CreateText(titlePanel, "TitleText", "GRIMSPIRE", 72, TextAnchor.MiddleCenter);
        
        return titlePanel;
    }

    private GameObject CreateButtonPanel(GameObject parent)
    {
        GameObject buttonPanel = new GameObject("ButtonPanel");
        buttonPanel.transform.SetParent(parent.transform, false);
        
        RectTransform rectTransform = buttonPanel.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.2f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.6f);
        rectTransform.sizeDelta = new Vector2(400, 400);
        rectTransform.anchoredPosition = Vector2.zero;
        
        // Add vertical layout group
        VerticalLayoutGroup layoutGroup = buttonPanel.AddComponent<VerticalLayoutGroup>();
        layoutGroup.spacing = 20;
        layoutGroup.childAlignment = TextAnchor.MiddleCenter;
        layoutGroup.childControlWidth = true;
        layoutGroup.childControlHeight = false;
        layoutGroup.childForceExpandWidth = true;
        layoutGroup.childForceExpandHeight = false;
        
        // Add content size fitter
        ContentSizeFitter sizeFitter = buttonPanel.AddComponent<ContentSizeFitter>();
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        // Create buttons
        GameObject newGameButton = CreateButton(buttonPanel, "NewGameButton", "NOUVELLE PARTIE");
        GameObject loadGameButton = CreateButton(buttonPanel, "LoadGameButton", "CHARGER PARTIE");
        GameObject settingsButton = CreateButton(buttonPanel, "SettingsButton", "PARAMÈTRES");
        GameObject quitButton = CreateButton(buttonPanel, "QuitButton", "QUITTER");
        
        return buttonPanel;
    }

    private GameObject CreateLoadingPanel(GameObject parent)
    {
        GameObject loadingPanel = new GameObject("LoadingPanel");
        loadingPanel.transform.SetParent(parent.transform, false);
        
        RectTransform rectTransform = loadingPanel.AddComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.sizeDelta = Vector2.zero;
        rectTransform.anchoredPosition = Vector2.zero;
        
        // Add semi-transparent background
        Image backgroundImage = loadingPanel.AddComponent<Image>();
        backgroundImage.color = new Color(0, 0, 0, 0.7f);
        
        // Create loading text
        GameObject loadingText = CreateText(loadingPanel, "LoadingText", "Chargement...", 36, TextAnchor.MiddleCenter);
        RectTransform loadingTextRect = loadingText.GetComponent<RectTransform>();
        loadingTextRect.anchorMin = new Vector2(0.5f, 0.6f);
        loadingTextRect.anchorMax = new Vector2(0.5f, 0.6f);
        loadingTextRect.sizeDelta = new Vector2(400, 100);
        
        // Create loading bar
        GameObject loadingBar = CreateLoadingBar(loadingPanel);
        
        return loadingPanel;
    }

    private GameObject CreateLoadingBar(GameObject parent)
    {
        GameObject loadingBarObject = new GameObject("LoadingBar");
        loadingBarObject.transform.SetParent(parent.transform, false);
        
        RectTransform rectTransform = loadingBarObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.4f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.4f);
        rectTransform.sizeDelta = new Vector2(600, 20);
        rectTransform.anchoredPosition = Vector2.zero;
        
        // Add slider component
        Slider slider = loadingBarObject.AddComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 0f;
        
        // Create background
        GameObject background = new GameObject("Background");
        background.transform.SetParent(loadingBarObject.transform, false);
        RectTransform bgRect = background.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        bgRect.anchoredPosition = Vector2.zero;
        
        Image bgImage = background.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        
        // Create fill area
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(loadingBarObject.transform, false);
        RectTransform fillRect = fillArea.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.sizeDelta = Vector2.zero;
        fillRect.anchoredPosition = Vector2.zero;
        
        // Create fill
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        RectTransform fillTransform = fill.AddComponent<RectTransform>();
        fillTransform.anchorMin = Vector2.zero;
        fillTransform.anchorMax = Vector2.one;
        fillTransform.sizeDelta = Vector2.zero;
        fillTransform.anchoredPosition = Vector2.zero;
        
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = new Color(0.3f, 0.8f, 0.3f, 1f);
        
        // Set slider references
        slider.targetGraphic = fillImage;
        slider.fillRect = fillTransform;
        
        return loadingBarObject;
    }

    private GameObject CreateLoadingScreen()
    {
        GameObject loadingScreenObject = new GameObject("LoadingScreen");
        loadingScreenObject.transform.SetParent(parentCanvas.transform, false);
        
        RectTransform rectTransform = loadingScreenObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.sizeDelta = Vector2.zero;
        rectTransform.anchoredPosition = Vector2.zero;
        
        // Add background
        Image backgroundImage = loadingScreenObject.AddComponent<Image>();
        backgroundImage.color = new Color(0, 0, 0, 0.9f);
        
        // Create loading text
        GameObject loadingText = CreateText(loadingScreenObject, "LoadingText", "Chargement en cours...", 48, TextAnchor.MiddleCenter);
        
        // Create progress bar
        GameObject progressBar = CreateLoadingBar(loadingScreenObject);
        
        loadingScreenObject.SetActive(false);
        
        return loadingScreenObject;
    }

    private GameObject CreateButton(GameObject parent, string name, string text)
    {
        GameObject buttonObject = new GameObject(name);
        buttonObject.transform.SetParent(parent.transform, false);
        
        RectTransform rectTransform = buttonObject.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(350, 60);
        
        // Add button component
        Button button = buttonObject.AddComponent<Button>();
        
        // Add image component
        Image buttonImage = buttonObject.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.3f, 0.5f, 1f);
        
        // Set button colors
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.2f, 0.3f, 0.5f, 1f);
        colors.highlightedColor = new Color(0.3f, 0.4f, 0.6f, 1f);
        colors.pressedColor = new Color(0.15f, 0.2f, 0.4f, 1f);
        colors.disabledColor = new Color(0.1f, 0.1f, 0.1f, 0.5f);
        button.colors = colors;
        
        // Create button text
        GameObject buttonText = CreateText(buttonObject, "Text", text, 24, TextAnchor.MiddleCenter);
        RectTransform textRect = buttonText.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;
        
        // Set button target graphic
        button.targetGraphic = buttonImage;
        
        return buttonObject;
    }

    private GameObject CreateText(GameObject parent, string name, string text, int fontSize, TextAnchor alignment)
    {
        GameObject textObject = new GameObject(name);
        textObject.transform.SetParent(parent.transform, false);
        
        RectTransform rectTransform = textObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.sizeDelta = Vector2.zero;
        rectTransform.anchoredPosition = Vector2.zero;
        
        Text textComponent = textObject.AddComponent<Text>();
        textComponent.text = text;
        textComponent.font = defaultFont != null ? defaultFont : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textComponent.fontSize = fontSize;
        textComponent.alignment = alignment;
        textComponent.color = Color.white;
        
        return textObject;
    }

    private void GenerateEventSystem()
    {
        // Check if EventSystem already exists
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<StandaloneInputModule>();
            
            Debug.Log("EventSystem generated successfully!");
        }
    }

    #if UNITY_EDITOR
    [MenuItem("Grimspire/Generate Phase 1.1 GameObjects")]
    public static void GenerateGameObjectsFromMenu()
    {
        GameObject generatorObject = new GameObject("GameObjectGenerator");
        GameObjectGenerator generator = generatorObject.AddComponent<GameObjectGenerator>();
        generator.GenerateAllGameObjects();
        
        // Clean up the generator after use
        DestroyImmediate(generatorObject);
    }
    #endif
}