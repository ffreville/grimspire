using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuGenerator : MonoBehaviour
{
    [Header("Menu Configuration")]
    [SerializeField] private Canvas menuCanvas;
    [SerializeField] private readonly Font uiFont;
    [SerializeField] private Color backgroundColor = new(0.1f, 0.1f, 0.1f, 0.8f);
    [SerializeField] private Color buttonColor = new(0.3f, 0.3f, 0.3f, 1f);
    [SerializeField] private Color buttonTextColor = Color.white;


#if UNITY_EDITOR
    [MenuItem("Grimspire/Generate Main Menu")]
    public static void GenerateMainMenuFromEditor()
    {
        MainMenuGenerator generator = FindObjectOfType<MainMenuGenerator>();
        if (generator == null)
        {
            GameObject generatorGO = new("MainMenuGenerator");
            generator = generatorGO.AddComponent<MainMenuGenerator>();
        }
        
        generator.GenerateMainMenu();
        
        EditorUtility.SetDirty(generator);
        Debug.Log("Main menu generated successfully!");
    }
#endif

    public void GenerateMainMenu()
    {
        if (menuCanvas == null)
        {
            CreateCanvas();
        }

        CreateBackground();
        CreateTitle();
        CreateMenuButtons();
    }

    private void CreateCanvas()
    {
        GameObject canvasGO = new("MainMenuCanvas");
        menuCanvas = canvasGO.AddComponent<Canvas>();
        menuCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        menuCanvas.sortingOrder = 10;

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasGO.AddComponent<GraphicRaycaster>();
    }

    private void CreateBackground()
    {
        GameObject backgroundGO = new("Background");
        backgroundGO.transform.SetParent(menuCanvas.transform, false);

        Image background = backgroundGO.AddComponent<Image>();
        background.color = backgroundColor;

        RectTransform bgRect = backgroundGO.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
    }

    private void CreateTitle()
    {
        GameObject titleGO = new("Title");
        titleGO.transform.SetParent(menuCanvas.transform, false);

        Text titleText = titleGO.AddComponent<Text>();
        titleText.text = "GRIMSPIRE";
        titleText.font = uiFont ?? Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleText.fontSize = 72;
        titleText.fontStyle = FontStyle.Bold;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = Color.white;

        RectTransform titleRect = titleGO.GetComponent<RectTransform>();
        titleRect.anchorMin = new(0.5f, 0.8f);
        titleRect.anchorMax = new(0.5f, 0.8f);
        titleRect.anchoredPosition = Vector2.zero;
        titleRect.sizeDelta = new(800, 100);
    }

    private void CreateMenuButtons()
    {
        string[] buttonLabels = { "Nouvelle Partie", "Charger", "Paramètres", "Quitter" };
        string[] buttonNames = { "NewGameButton", "LoadGameButton", "SettingsButton", "QuitButton" };

        for (int i = 0; i < buttonLabels.Length; i++)
        {
            CreateButton(buttonLabels[i], buttonNames[i], i);
        }
    }

    private void CreateButton(string label, string name, int index)
    {
        GameObject buttonGO = new(name);
        buttonGO.transform.SetParent(menuCanvas.transform, false);

        Button button = buttonGO.AddComponent<Button>();
        Image buttonImage = buttonGO.AddComponent<Image>();
        buttonImage.color = buttonColor;

        button.targetGraphic = buttonImage;

        ColorBlock colors = button.colors;
        colors.normalColor = buttonColor;
        colors.highlightedColor = new(buttonColor.r + 0.1f, buttonColor.g + 0.1f, buttonColor.b + 0.1f, 1f);
        colors.pressedColor = new(buttonColor.r - 0.1f, buttonColor.g - 0.1f, buttonColor.b - 0.1f, 1f);
        button.colors = colors;

        GameObject textGO = new("Text");
        textGO.transform.SetParent(buttonGO.transform, false);

        Text buttonText = textGO.AddComponent<Text>();
        buttonText.text = label;
        buttonText.font = uiFont ?? Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        buttonText.fontSize = 32;
        buttonText.alignment = TextAnchor.MiddleCenter;
        buttonText.color = buttonTextColor;

        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        RectTransform buttonRect = buttonGO.GetComponent<RectTransform>();
        buttonRect.anchorMin = new(0.5f, 0.5f);
        buttonRect.anchorMax = new(0.5f, 0.5f);
        buttonRect.anchoredPosition = new(0, 100 - (index * 80));
        buttonRect.sizeDelta = new(300, 60);

        AttachMainMenuComponent(button, name);
    }

    private void AttachMainMenuComponent(Button button, string buttonName)
    {
        MainMenu mainMenu = FindObjectOfType<MainMenu>();
        if (mainMenu == null)
        {
            GameObject mainMenuGO = new("MainMenuManager");
            mainMenu = mainMenuGO.AddComponent<MainMenu>();
        }

        switch (buttonName)
        {
            case "NewGameButton":
                SetButtonReference(mainMenu, "newGameButton", button);
                break;
            case "LoadGameButton":
                SetButtonReference(mainMenu, "loadGameButton", button);
                break;
            case "SettingsButton":
                SetButtonReference(mainMenu, "settingsButton", button);
                break;
            case "QuitButton":
                SetButtonReference(mainMenu, "quitButton", button);
                break;
        }
    }

    private void SetButtonReference(MainMenu mainMenu, string fieldName, Button button)
    {
        var field = typeof(MainMenu).GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(mainMenu, button);
    }
}