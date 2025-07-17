using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    private static CanvasManager instance;
    public static CanvasManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<CanvasManager>();
                if (instance == null)
                {
                    GameObject canvasManagerObject = new GameObject("CanvasManager");
                    instance = canvasManagerObject.AddComponent<CanvasManager>();
                    DontDestroyOnLoad(canvasManagerObject);
                }
            }
            return instance;
        }
    }

    [Header("Canvas References")]
    [SerializeField] private Canvas mainMenuCanvas;
    [SerializeField] private Canvas gameplayCanvas;
    [SerializeField] private Canvas adventurerCanvas;
    [SerializeField] private Canvas equipmentCanvas;
    [SerializeField] private Canvas craftingCanvas;
    [SerializeField] private Canvas settingsCanvas;
    [SerializeField] private Canvas loadingCanvas;

    [Header("Canvas Settings")]
    [SerializeField] private int baseCanvasSortOrder = 0;
    [SerializeField] private int overlayCanvasSortOrder = 100;
    [SerializeField] private int modalCanvasSortOrder = 200;
    [SerializeField] private int tooltipCanvasSortOrder = 300;

    [Header("Fade Settings")]
    [SerializeField] private float fadeInDuration = 0.3f;
    [SerializeField] private float fadeOutDuration = 0.3f;
    [SerializeField] private AnimationCurve fadeInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private AnimationCurve fadeOutCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    private Dictionary<string, Canvas> canvasDictionary;
    private Dictionary<string, CanvasGroup> canvasGroups;
    private Canvas currentActiveCanvas;
    private List<Canvas> activeOverlays;

    public enum CanvasType
    {
        MainMenu,
        Gameplay,
        Adventurer,
        Equipment,
        Crafting,
        Settings,
        Loading
    }

    public enum CanvasLayer
    {
        Base = 0,
        Overlay = 100,
        Modal = 200,
        Tooltip = 300
    }

    public System.Action<Canvas> OnCanvasChanged;
    public System.Action<Canvas> OnCanvasShown;
    public System.Action<Canvas> OnCanvasHidden;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeCanvasManager();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void InitializeCanvasManager()
    {
        canvasDictionary = new Dictionary<string, Canvas>();
        canvasGroups = new Dictionary<string, CanvasGroup>();
        activeOverlays = new List<Canvas>();

        // Initialize canvas dictionary
        RegisterCanvas("MainMenu", mainMenuCanvas);
        RegisterCanvas("Gameplay", gameplayCanvas);
        RegisterCanvas("Adventurer", adventurerCanvas);
        RegisterCanvas("Equipment", equipmentCanvas);
        RegisterCanvas("Crafting", craftingCanvas);
        RegisterCanvas("Settings", settingsCanvas);
        RegisterCanvas("Loading", loadingCanvas);

        // Create canvases if they don't exist
        CreateMissingCanvases();

        // Set initial state
        ShowCanvas("MainMenu");
    }

    private void RegisterCanvas(string name, Canvas canvas)
    {
        if (canvas != null)
        {
            canvasDictionary[name] = canvas;
            
            // Ensure canvas has a CanvasGroup
            CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = canvas.gameObject.AddComponent<CanvasGroup>();
            }
            canvasGroups[name] = canvasGroup;
        }
    }

    private void CreateMissingCanvases()
    {
        // Create main menu canvas if missing
        if (mainMenuCanvas == null)
        {
            mainMenuCanvas = CreateCanvas("MainMenuCanvas", CanvasLayer.Base);
            RegisterCanvas("MainMenu", mainMenuCanvas);
        }

        // Create gameplay canvas if missing
        if (gameplayCanvas == null)
        {
            gameplayCanvas = CreateCanvas("GameplayCanvas", CanvasLayer.Base);
            RegisterCanvas("Gameplay", gameplayCanvas);
        }

        // Create adventurer canvas if missing
        if (adventurerCanvas == null)
        {
            adventurerCanvas = CreateCanvas("AdventurerCanvas", CanvasLayer.Overlay);
            RegisterCanvas("Adventurer", adventurerCanvas);
        }

        // Create equipment canvas if missing
        if (equipmentCanvas == null)
        {
            equipmentCanvas = CreateCanvas("EquipmentCanvas", CanvasLayer.Overlay);
            RegisterCanvas("Equipment", equipmentCanvas);
        }

        // Create crafting canvas if missing
        if (craftingCanvas == null)
        {
            craftingCanvas = CreateCanvas("CraftingCanvas", CanvasLayer.Overlay);
            RegisterCanvas("Crafting", craftingCanvas);
        }

        // Create settings canvas if missing
        if (settingsCanvas == null)
        {
            settingsCanvas = CreateCanvas("SettingsCanvas", CanvasLayer.Modal);
            RegisterCanvas("Settings", settingsCanvas);
        }

        // Create loading canvas if missing
        if (loadingCanvas == null)
        {
            loadingCanvas = CreateCanvas("LoadingCanvas", CanvasLayer.Modal);
            RegisterCanvas("Loading", loadingCanvas);
        }
    }

    private Canvas CreateCanvas(string name, CanvasLayer layer)
    {
        GameObject canvasObject = new GameObject(name);
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = (int)layer;

        // Add CanvasScaler
        CanvasScaler canvasScaler = canvasObject.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920, 1080);
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        canvasScaler.matchWidthOrHeight = 0.5f;

        // Add GraphicRaycaster
        canvasObject.AddComponent<GraphicRaycaster>();

        // Add CanvasGroup
        CanvasGroup canvasGroup = canvasObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        // Initially hide the canvas
        canvas.gameObject.SetActive(false);

        return canvas;
    }

    public void ShowCanvas(string canvasName, bool hideOthers = true)
    {
        if (!canvasDictionary.ContainsKey(canvasName))
        {
            Debug.LogWarning($"Canvas '{canvasName}' not found!");
            return;
        }

        Canvas targetCanvas = canvasDictionary[canvasName];
        CanvasGroup targetCanvasGroup = canvasGroups[canvasName];

        // Hide other canvases if requested
        if (hideOthers)
        {
            HideAllCanvases();
        }

        // Show target canvas
        targetCanvas.gameObject.SetActive(true);
        targetCanvasGroup.alpha = 1f;
        targetCanvasGroup.interactable = true;
        targetCanvasGroup.blocksRaycasts = true;

        currentActiveCanvas = targetCanvas;
        OnCanvasShown?.Invoke(targetCanvas);
        OnCanvasChanged?.Invoke(targetCanvas);

        Debug.Log($"Canvas '{canvasName}' shown");
    }

    public void ShowCanvas(CanvasType canvasType, bool hideOthers = true)
    {
        ShowCanvas(canvasType.ToString(), hideOthers);
    }

    public void HideCanvas(string canvasName)
    {
        if (!canvasDictionary.ContainsKey(canvasName))
        {
            Debug.LogWarning($"Canvas '{canvasName}' not found!");
            return;
        }

        Canvas targetCanvas = canvasDictionary[canvasName];
        CanvasGroup targetCanvasGroup = canvasGroups[canvasName];

        // Hide canvas
        targetCanvasGroup.alpha = 0f;
        targetCanvasGroup.interactable = false;
        targetCanvasGroup.blocksRaycasts = false;
        targetCanvas.gameObject.SetActive(false);

        // Remove from active overlays
        if (activeOverlays.Contains(targetCanvas))
        {
            activeOverlays.Remove(targetCanvas);
        }

        OnCanvasHidden?.Invoke(targetCanvas);

        Debug.Log($"Canvas '{canvasName}' hidden");
    }

    public void HideCanvas(CanvasType canvasType)
    {
        HideCanvas(canvasType.ToString());
    }

    public void HideAllCanvases()
    {
        foreach (var kvp in canvasDictionary)
        {
            if (kvp.Value != null)
            {
                HideCanvas(kvp.Key);
            }
        }
        currentActiveCanvas = null;
        activeOverlays.Clear();
    }

    public void ShowOverlay(string canvasName)
    {
        if (!canvasDictionary.ContainsKey(canvasName))
        {
            Debug.LogWarning($"Canvas '{canvasName}' not found!");
            return;
        }

        Canvas targetCanvas = canvasDictionary[canvasName];
        CanvasGroup targetCanvasGroup = canvasGroups[canvasName];

        // Show overlay without hiding others
        targetCanvas.gameObject.SetActive(true);
        targetCanvasGroup.alpha = 1f;
        targetCanvasGroup.interactable = true;
        targetCanvasGroup.blocksRaycasts = true;

        if (!activeOverlays.Contains(targetCanvas))
        {
            activeOverlays.Add(targetCanvas);
        }

        OnCanvasShown?.Invoke(targetCanvas);

        Debug.Log($"Overlay '{canvasName}' shown");
    }

    public void ShowOverlay(CanvasType canvasType)
    {
        ShowOverlay(canvasType.ToString());
    }

    public void HideOverlay(string canvasName)
    {
        HideCanvas(canvasName);
    }

    public void HideOverlay(CanvasType canvasType)
    {
        HideCanvas(canvasType.ToString());
    }

    public void ToggleCanvas(string canvasName)
    {
        if (!canvasDictionary.ContainsKey(canvasName))
        {
            Debug.LogWarning($"Canvas '{canvasName}' not found!");
            return;
        }

        Canvas targetCanvas = canvasDictionary[canvasName];
        
        if (targetCanvas.gameObject.activeInHierarchy)
        {
            HideCanvas(canvasName);
        }
        else
        {
            ShowCanvas(canvasName, false);
        }
    }

    public void ToggleCanvas(CanvasType canvasType)
    {
        ToggleCanvas(canvasType.ToString());
    }

    public bool IsCanvasVisible(string canvasName)
    {
        if (!canvasDictionary.ContainsKey(canvasName))
        {
            return false;
        }

        Canvas canvas = canvasDictionary[canvasName];
        return canvas != null && canvas.gameObject.activeInHierarchy;
    }

    public bool IsCanvasVisible(CanvasType canvasType)
    {
        return IsCanvasVisible(canvasType.ToString());
    }

    public Canvas GetCanvas(string canvasName)
    {
        if (canvasDictionary.ContainsKey(canvasName))
        {
            return canvasDictionary[canvasName];
        }
        return null;
    }

    public Canvas GetCanvas(CanvasType canvasType)
    {
        return GetCanvas(canvasType.ToString());
    }

    public Canvas GetCurrentActiveCanvas()
    {
        return currentActiveCanvas;
    }

    public List<Canvas> GetActiveOverlays()
    {
        return new List<Canvas>(activeOverlays);
    }

    public void SetCanvasSortOrder(string canvasName, int sortOrder)
    {
        if (canvasDictionary.ContainsKey(canvasName))
        {
            Canvas canvas = canvasDictionary[canvasName];
            if (canvas != null)
            {
                canvas.sortingOrder = sortOrder;
            }
        }
    }

    public void SetCanvasSortOrder(CanvasType canvasType, int sortOrder)
    {
        SetCanvasSortOrder(canvasType.ToString(), sortOrder);
    }

    public void RegisterCustomCanvas(string name, Canvas canvas)
    {
        if (canvas != null)
        {
            canvasDictionary[name] = canvas;
            
            CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = canvas.gameObject.AddComponent<CanvasGroup>();
            }
            canvasGroups[name] = canvasGroup;
            
            Debug.Log($"Custom canvas '{name}' registered");
        }
    }

    public void UnregisterCanvas(string name)
    {
        if (canvasDictionary.ContainsKey(name))
        {
            canvasDictionary.Remove(name);
            canvasGroups.Remove(name);
            Debug.Log($"Canvas '{name}' unregistered");
        }
    }

    // Utility methods for common canvas operations
    public void ShowMainMenu()
    {
        ShowCanvas(CanvasType.MainMenu);
    }

    public void ShowGameplay()
    {
        ShowCanvas(CanvasType.Gameplay);
    }

    public void ShowAdventurerInterface()
    {
        ShowOverlay(CanvasType.Adventurer);
    }

    public void ShowEquipmentInterface()
    {
        ShowOverlay(CanvasType.Equipment);
    }

    public void ShowCraftingInterface()
    {
        ShowOverlay(CanvasType.Crafting);
    }

    public void ShowSettings()
    {
        ShowOverlay(CanvasType.Settings);
    }

    public void ShowLoading()
    {
        ShowOverlay(CanvasType.Loading);
    }

    public void HideLoading()
    {
        HideOverlay(CanvasType.Loading);
    }

    // Debug methods
    public void PrintCanvasStates()
    {
        Debug.Log("=== Canvas States ===");
        foreach (var kvp in canvasDictionary)
        {
            if (kvp.Value != null)
            {
                bool isActive = kvp.Value.gameObject.activeInHierarchy;
                float alpha = canvasGroups[kvp.Key].alpha;
                Debug.Log($"{kvp.Key}: Active={isActive}, Alpha={alpha:F2}");
            }
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}