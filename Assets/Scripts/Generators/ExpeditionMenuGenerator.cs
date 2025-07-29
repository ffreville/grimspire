using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ExpeditionMenuGenerator : MonoBehaviour
{
#if UNITY_EDITOR
    [Header("Colors")]
    [SerializeField] private Color backgroundColor = new(0.1f, 0.1f, 0.1f, 0.9f);
    [SerializeField] private Color panelColor = new(0.2f, 0.2f, 0.2f, 0.8f);
    [SerializeField] private Color buttonColor = new(0.3f, 0.5f, 0.7f, 1f);

    [MenuItem("Grimspire/Setup Expedition Menu")]
    public static void SetupExpeditionMenu()
    {
        // Find Content_Expeditions
        GameObject contentExpeditions = GameObject.Find("Content_Expeditions");
        if (contentExpeditions == null)
        {
            Debug.LogError("Content_Expeditions not found! Create the game menu first.");
            return;
        }

        // Clear existing content
        for (int i = contentExpeditions.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(contentExpeditions.transform.GetChild(i).gameObject);
        }

        var generator = new ExpeditionMenuGenerator();
        generator.GenerateMenuStructure(contentExpeditions);

        Debug.Log("Menu des Expéditions généré avec succès!");
    }

    [ContextMenu("Generate Expedition Menu")]
    public void GenerateExpeditionMenu()
    {
        GameObject targetGameObject = GameObject.Find("Content_Expeditions");
        if (targetGameObject == null)
        {
            Debug.LogError("Content_Expeditions not found!");
            return;
        }

        // Clear existing children
        for (int i = targetGameObject.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(targetGameObject.transform.GetChild(i).gameObject);
        }

        // Generate the menu structure
        GenerateMenuStructure(targetGameObject);

        Debug.Log("Menu des Expéditions généré avec succès!");
    }

    private void GenerateMenuStructure(GameObject targetGameObject)
    {
        // Créer le conteneur principal
        GameObject mainContainer = CreateMainContainer(targetGameObject);
        
        // Créer le header avec titre et boutons
        CreateHeader(mainContainer.transform);
        
        // Créer la zone des listes d'expéditions
        var expeditionLists = CreateExpeditionLists(mainContainer.transform);
        
        // Créer le panneau de détails d'expédition
        var detailsPanel = CreateExpeditionDetailsPanel(mainContainer.transform);

        // Créer un prefab d'expédition simple
        GameObject expeditionPrefab = CreateExpeditionItemPrefab();

        // Add the ExpeditionMenu component
        var expeditionMenu = targetGameObject.GetComponent<ExpeditionMenu>();
        if (expeditionMenu == null)
        {
            expeditionMenu = targetGameObject.AddComponent<ExpeditionMenu>();
        }

        // Set the references using reflection
        SetExpeditionMenuReferences(expeditionMenu, expeditionLists, detailsPanel, expeditionPrefab);
    }

    private GameObject CreateMainContainer(GameObject targetGameObject)
    {
        GameObject container = new GameObject("ExpeditionMenu");
        container.transform.SetParent(targetGameObject.transform, false);

        // Layout
        VerticalLayoutGroup layout = container.AddComponent<VerticalLayoutGroup>();
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = true;
        layout.spacing = 10f;
        layout.padding = new RectOffset(10, 10, 10, 10);

        // Background
        Image bgImage = container.AddComponent<Image>();
        bgImage.color = backgroundColor;

        // RectTransform
        RectTransform rectTransform = container.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        return container;
    }

    private void CreateHeader(Transform parent)
    {
        GameObject header = new GameObject("Header");
        header.transform.SetParent(parent, false);

        // Layout
        HorizontalLayoutGroup layout = header.AddComponent<HorizontalLayoutGroup>();
        layout.childAlignment = TextAnchor.MiddleLeft;
        layout.childControlWidth = false;
        layout.childControlHeight = true;
        layout.childForceExpandHeight = true;
        layout.spacing = 20f;

        // Size
        LayoutElement layoutElement = header.AddComponent<LayoutElement>();
        layoutElement.preferredHeight = 60f;

        // Titre
        GameObject title = CreateText("Title", "Expéditions", parent: header.transform);
        TextMeshProUGUI titleText = title.GetComponent<TextMeshProUGUI>();
        titleText.fontSize = 24f;
        titleText.fontStyle = FontStyles.Bold;

        // Spacer
        GameObject spacer = new GameObject("Spacer");
        spacer.transform.SetParent(header.transform, false);
        LayoutElement spacerLayout = spacer.AddComponent<LayoutElement>();
        spacerLayout.flexibleWidth = 1f;

        // Bouton générer nouvelles expéditions
        CreateButton("GenerateExpeditionsButton", "Nouvelles Expéditions", header.transform);

        // Ressources de la cité
        GameObject resourcesText = CreateText("ResourcesText", "Ressources: Chargement...", parent: header.transform);
        TextMeshProUGUI resourcesTextComponent = resourcesText.GetComponent<TextMeshProUGUI>();
        resourcesTextComponent.fontSize = 14f;
        resourcesTextComponent.alignment = TextAlignmentOptions.MidlineRight;
    }

    private ExpeditionListReferences CreateExpeditionLists(Transform parent)
    {
        GameObject listsContainer = new GameObject("ExpeditionLists");
        listsContainer.transform.SetParent(parent, false);

        // Layout
        HorizontalLayoutGroup layout = listsContainer.AddComponent<HorizontalLayoutGroup>();
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = true;
        layout.spacing = 10f;

        // Size
        LayoutElement layoutElement = listsContainer.AddComponent<LayoutElement>();
        layoutElement.flexibleHeight = 1f;

        // Créer les trois listes
        var availableContent = CreateExpeditionListPanel("AvailableExpeditions", "Expéditions Disponibles", listsContainer.transform);
        var activeContent = CreateExpeditionListPanel("ActiveExpeditions", "Expéditions en Cours", listsContainer.transform);
        var completedContent = CreateExpeditionListPanel("CompletedExpeditions", "Expéditions Terminées", listsContainer.transform);

        return new ExpeditionListReferences
        {
            availableExpeditionsContent = availableContent,
            activeExpeditionsContent = activeContent,
            completedExpeditionsContent = completedContent
        };
    }

    private class ExpeditionListReferences
    {
        public Transform availableExpeditionsContent;
        public Transform activeExpeditionsContent;
        public Transform completedExpeditionsContent;
    }

    private Transform CreateExpeditionListPanel(string name, string title, Transform parent)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);

        // Background
        Image bgImage = panel.AddComponent<Image>();
        bgImage.color = panelColor;

        // Layout
        VerticalLayoutGroup layout = panel.AddComponent<VerticalLayoutGroup>();
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = true;
        layout.spacing = 5f;
        layout.padding = new RectOffset(10, 10, 10, 10);

        // Size
        LayoutElement layoutElement = panel.AddComponent<LayoutElement>();
        layoutElement.flexibleWidth = 1f;

        // Titre du panneau
        GameObject titleObj = CreateText($"{name}Title", title, parent: panel.transform);
        TextMeshProUGUI titleText = titleObj.GetComponent<TextMeshProUGUI>();
        titleText.fontSize = 18f;
        titleText.fontStyle = FontStyles.Bold;
        titleText.alignment = TextAlignmentOptions.Center;

        LayoutElement titleLayout = titleObj.AddComponent<LayoutElement>();
        titleLayout.preferredHeight = 30f;

        // Zone de contenu avec scroll
        return CreateScrollableContent($"{name}Content", panel.transform);
    }

    private Transform CreateScrollableContent(string name, Transform parent)
    {
        GameObject scrollView = new GameObject(name + "ScrollView");
        scrollView.transform.SetParent(parent, false);

        // ScrollRect
        ScrollRect scrollRect = scrollView.AddComponent<ScrollRect>();
        scrollRect.horizontal = false;
        scrollRect.vertical = true;

        // Size
        LayoutElement layoutElement = scrollView.AddComponent<LayoutElement>();
        layoutElement.flexibleHeight = 1f;

        // Viewport
        GameObject viewport = new GameObject("Viewport");
        viewport.transform.SetParent(scrollView.transform, false);
        
        RectTransform viewportRect = viewport.AddComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.offsetMin = Vector2.zero;
        viewportRect.offsetMax = Vector2.zero;

        Image viewportImage = viewport.AddComponent<Image>();
        viewportImage.color = Color.clear;
        
        Mask mask = viewport.AddComponent<Mask>();
        mask.showMaskGraphic = false;

        scrollRect.viewport = viewportRect;

        // Content
        GameObject content = new GameObject(name);
        content.transform.SetParent(viewport.transform, false);

        RectTransform contentRect = content.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 1);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.pivot = new Vector2(0.5f, 1);
        contentRect.offsetMin = new Vector2(0, 0);
        contentRect.offsetMax = new Vector2(0, 0);

        VerticalLayoutGroup contentLayout = content.AddComponent<VerticalLayoutGroup>();
        contentLayout.childAlignment = TextAnchor.UpperCenter;
        contentLayout.childControlWidth = true;
        contentLayout.childControlHeight = false;
        contentLayout.childForceExpandWidth = true;
        contentLayout.spacing = 5f;

        ContentSizeFitter sizeFitter = content.AddComponent<ContentSizeFitter>();
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scrollRect.content = contentRect;

        // Scrollbar (optionnel)
        CreateScrollbar(scrollView.transform, scrollRect);

        return content.transform; // Return the content transform
    }

    private void CreateScrollbar(Transform parent, ScrollRect scrollRect)
    {
        GameObject scrollbar = new GameObject("Scrollbar");
        scrollbar.transform.SetParent(parent, false);

        RectTransform scrollbarRect = scrollbar.AddComponent<RectTransform>();
        scrollbarRect.anchorMin = new Vector2(1, 0);
        scrollbarRect.anchorMax = new Vector2(1, 1);
        scrollbarRect.pivot = new Vector2(1, 1);
        scrollbarRect.offsetMin = new Vector2(-20, 0);
        scrollbarRect.offsetMax = new Vector2(0, 0);

        Image scrollbarBg = scrollbar.AddComponent<Image>();
        scrollbarBg.color = new Color(0.1f, 0.1f, 0.1f, 0.5f);

        Scrollbar scrollbarComponent = scrollbar.AddComponent<Scrollbar>();
        scrollbarComponent.direction = Scrollbar.Direction.BottomToTop;

        // Handle
        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(scrollbar.transform, false);

        RectTransform handleRect = handle.AddComponent<RectTransform>();
        handleRect.anchorMin = Vector2.zero;
        handleRect.anchorMax = Vector2.one;
        handleRect.offsetMin = Vector2.zero;
        handleRect.offsetMax = Vector2.zero;

        Image handleImage = handle.AddComponent<Image>();
        handleImage.color = new Color(0.5f, 0.5f, 0.5f, 0.8f);

        scrollbarComponent.handleRect = handleRect;
        scrollbarComponent.targetGraphic = handleImage;

        scrollRect.verticalScrollbar = scrollbarComponent;
    }


    private GameObject CreateText(string name, string text, Transform parent)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);

        TextMeshProUGUI textComponent = textObj.AddComponent<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.fontSize = 14f;
        textComponent.color = Color.white;
        textComponent.alignment = TextAlignmentOptions.MidlineLeft;

        return textObj;
    }

    private GameObject CreateButton(string name, string text, Transform parent)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent, false);

        // Background
        Image bgImage = buttonObj.AddComponent<Image>();
        bgImage.color = buttonColor;

        // Button component
        Button button = buttonObj.AddComponent<Button>();
        button.targetGraphic = bgImage;

        // Size
        LayoutElement layoutElement = buttonObj.AddComponent<LayoutElement>();
        layoutElement.preferredHeight = 40f;

        // Text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        TextMeshProUGUI textComponent = textObj.AddComponent<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.fontSize = 14f;
        textComponent.color = Color.white;
        textComponent.alignment = TextAlignmentOptions.Center;

        return buttonObj;
    }

    private GameObject CreateExpeditionItemPrefab()
    {
        GameObject prefab = new GameObject("ExpeditionItemPrefab");
        
        // Add RectTransform
        RectTransform prefabRect = prefab.AddComponent<RectTransform>();
        prefabRect.sizeDelta = new Vector2(0, 100);
        
        // Background
        Image bg = prefab.AddComponent<Image>();
        bg.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        
        // Name text
        GameObject nameGO = new GameObject("NameText");
        nameGO.transform.SetParent(prefab.transform, false);
        
        TextMeshProUGUI nameText = nameGO.AddComponent<TextMeshProUGUI>();
        nameText.text = "Nom Expédition";
        nameText.fontSize = 16f;
        nameText.fontStyle = FontStyles.Bold;
        nameText.color = Color.white;
        nameText.alignment = TextAlignmentOptions.MidlineLeft;
        
        RectTransform nameRect = nameGO.GetComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0, 0.7f);
        nameRect.anchorMax = new Vector2(0.6f, 1f);
        nameRect.offsetMin = new Vector2(10, 0);
        nameRect.offsetMax = new Vector2(-10, 0);
        
        // Description text
        GameObject descGO = new GameObject("DescriptionText");
        descGO.transform.SetParent(prefab.transform, false);
        
        TextMeshProUGUI descText = descGO.AddComponent<TextMeshProUGUI>();
        descText.text = "Description";
        descText.fontSize = 12f;
        descText.color = Color.gray;
        descText.alignment = TextAlignmentOptions.MidlineLeft;
        
        RectTransform descRect = descGO.GetComponent<RectTransform>();
        descRect.anchorMin = new Vector2(0, 0.4f);
        descRect.anchorMax = new Vector2(0.6f, 0.7f);
        descRect.offsetMin = new Vector2(10, 0);
        descRect.offsetMax = new Vector2(-10, 0);
        
        // Difficulty text
        GameObject diffGO = new GameObject("DifficultyText");
        diffGO.transform.SetParent(prefab.transform, false);
        
        TextMeshProUGUI diffText = diffGO.AddComponent<TextMeshProUGUI>();
        diffText.text = "Facile";
        diffText.fontSize = 12f;
        diffText.color = Color.green;
        diffText.alignment = TextAlignmentOptions.MidlineLeft;
        
        RectTransform diffRect = diffGO.GetComponent<RectTransform>();
        diffRect.anchorMin = new Vector2(0, 0.2f);
        diffRect.anchorMax = new Vector2(0.3f, 0.4f);
        diffRect.offsetMin = new Vector2(10, 0);
        diffRect.offsetMax = new Vector2(-5, 0);
        
        // Duration text
        GameObject durGO = new GameObject("DurationText");
        durGO.transform.SetParent(prefab.transform, false);
        
        TextMeshProUGUI durText = durGO.AddComponent<TextMeshProUGUI>();
        durText.text = "2h";
        durText.fontSize = 12f;
        durText.color = Color.white;
        durText.alignment = TextAlignmentOptions.MidlineLeft;
        
        RectTransform durRect = durGO.GetComponent<RectTransform>();
        durRect.anchorMin = new Vector2(0.3f, 0.2f);
        durRect.anchorMax = new Vector2(0.6f, 0.4f);
        durRect.offsetMin = new Vector2(5, 0);
        durRect.offsetMax = new Vector2(-10, 0);
        
        // Status text
        GameObject statusGO = new GameObject("StatusText");
        statusGO.transform.SetParent(prefab.transform, false);
        
        TextMeshProUGUI statusText = statusGO.AddComponent<TextMeshProUGUI>();
        statusText.text = "Disponible";
        statusText.fontSize = 12f;
        statusText.color = Color.cyan;
        statusText.alignment = TextAlignmentOptions.MidlineLeft;
        
        RectTransform statusRect = statusGO.GetComponent<RectTransform>();
        statusRect.anchorMin = new Vector2(0, 0);
        statusRect.anchorMax = new Vector2(0.6f, 0.2f);
        statusRect.offsetMin = new Vector2(10, 0);
        statusRect.offsetMax = new Vector2(-10, 0);
        
        // Action button
        GameObject buttonGO = new GameObject("ActionButton");
        buttonGO.transform.SetParent(prefab.transform, false);
        
        Button button = buttonGO.AddComponent<Button>();
        Image buttonImage = buttonGO.AddComponent<Image>();
        buttonImage.color = buttonColor;
        button.targetGraphic = buttonImage;
        
        RectTransform buttonRect = buttonGO.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.65f, 0.3f);
        buttonRect.anchorMax = new Vector2(0.95f, 0.7f);
        buttonRect.offsetMin = Vector2.zero;
        buttonRect.offsetMax = Vector2.zero;
        
        GameObject buttonTextGO = new GameObject("Text");
        buttonTextGO.transform.SetParent(buttonGO.transform, false);
        
        TextMeshProUGUI buttonText = buttonTextGO.AddComponent<TextMeshProUGUI>();
        buttonText.text = "Sélectionner";
        buttonText.fontSize = 12f;
        buttonText.color = Color.white;
        buttonText.alignment = TextAlignmentOptions.Center;
        
        RectTransform buttonTextRect = buttonTextGO.GetComponent<RectTransform>();
        buttonTextRect.anchorMin = Vector2.zero;
        buttonTextRect.anchorMax = Vector2.one;
        buttonTextRect.offsetMin = Vector2.zero;
        buttonTextRect.offsetMax = Vector2.zero;
        
        // Add other missing text fields
        GameObject adventurersGO = new GameObject("AdventurersText");
        adventurersGO.transform.SetParent(prefab.transform, false);
        TextMeshProUGUI adventurersText = adventurersGO.AddComponent<TextMeshProUGUI>();
        adventurersText.text = "0/1-3";
        adventurersText.fontSize = 10f;
        adventurersText.color = Color.white;
        RectTransform advRect = adventurersGO.GetComponent<RectTransform>();
        advRect.anchorMin = new Vector2(0.65f, 0.7f);
        advRect.anchorMax = new Vector2(0.95f, 0.85f);
        advRect.offsetMin = Vector2.zero;
        advRect.offsetMax = Vector2.zero;
        
        GameObject locationGO = new GameObject("LocationText");
        locationGO.transform.SetParent(prefab.transform, false);
        TextMeshProUGUI locationText = locationGO.AddComponent<TextMeshProUGUI>();
        locationText.text = "Lieu";
        locationText.fontSize = 10f;
        locationText.color = Color.gray;
        RectTransform locRect = locationGO.GetComponent<RectTransform>();
        locRect.anchorMin = new Vector2(0.65f, 0.15f);
        locRect.anchorMax = new Vector2(0.95f, 0.3f);
        locRect.offsetMin = Vector2.zero;
        locRect.offsetMax = Vector2.zero;
        
        GameObject progressGO = new GameObject("ProgressText");
        progressGO.transform.SetParent(prefab.transform, false);
        TextMeshProUGUI progressText = progressGO.AddComponent<TextMeshProUGUI>();
        progressText.text = "";
        progressText.fontSize = 10f;
        progressText.color = Color.yellow;
        RectTransform progRect = progressGO.GetComponent<RectTransform>();
        progRect.anchorMin = new Vector2(0.65f, 0);
        progRect.anchorMax = new Vector2(0.95f, 0.15f);
        progRect.offsetMin = Vector2.zero;
        progRect.offsetMax = Vector2.zero;
        
        // Add ExpeditionItem component
        ExpeditionItem item = prefab.AddComponent<ExpeditionItem>();
        
        // Set references using reflection
        var nameField = typeof(ExpeditionItem).GetField("nameText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var descField = typeof(ExpeditionItem).GetField("descriptionText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var diffField = typeof(ExpeditionItem).GetField("difficultyText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var durField = typeof(ExpeditionItem).GetField("durationText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var advField = typeof(ExpeditionItem).GetField("adventurersText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var locField = typeof(ExpeditionItem).GetField("locationText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var statField = typeof(ExpeditionItem).GetField("statusText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var progField = typeof(ExpeditionItem).GetField("progressText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var buttonField = typeof(ExpeditionItem).GetField("actionButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        nameField?.SetValue(item, nameText);
        descField?.SetValue(item, descText);
        diffField?.SetValue(item, diffText);
        durField?.SetValue(item, durText);
        advField?.SetValue(item, adventurersText);
        locField?.SetValue(item, locationText);
        statField?.SetValue(item, statusText);
        progField?.SetValue(item, progressText);
        buttonField?.SetValue(item, button);
        
        prefab.SetActive(false);
        
        return prefab;
    }

    private ExpeditionDetailsReferences CreateExpeditionDetailsPanel(Transform parent)
    {
        GameObject detailsPanel = new GameObject("SelectedExpeditionPanel");
        detailsPanel.transform.SetParent(parent, false);
        detailsPanel.SetActive(false); // Caché par défaut

        // Background
        Image bgImage = detailsPanel.AddComponent<Image>();
        bgImage.color = panelColor;

        // Layout
        HorizontalLayoutGroup layout = detailsPanel.AddComponent<HorizontalLayoutGroup>();
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = true;
        layout.spacing = 20f;
        layout.padding = new RectOffset(20, 20, 20, 20);

        // Size
        LayoutElement layoutElement = detailsPanel.AddComponent<LayoutElement>();
        layoutElement.preferredHeight = 400f;

        // Panel des détails de l'expédition
        var infoRefs = CreateExpeditionInfoPanel(detailsPanel.transform);

        // Panel des aventuriers
        var advRefs = CreateAdventurerSelectionPanel(detailsPanel.transform);

        return new ExpeditionDetailsReferences
        {
            selectedExpeditionPanel = detailsPanel,
            selectedExpeditionName = infoRefs.nameText,
            selectedExpeditionDescription = infoRefs.descText,
            selectedExpeditionDetails = infoRefs.detailsText,
            startExpeditionButton = infoRefs.startButton,
            cancelSelectionButton = infoRefs.cancelButton,
            availableAdventurersContent = advRefs.availableContent,
            assignedAdventurersContent = advRefs.assignedContent
        };
    }

    private class ExpeditionDetailsReferences
    {
        public GameObject selectedExpeditionPanel;
        public TextMeshProUGUI selectedExpeditionName;
        public TextMeshProUGUI selectedExpeditionDescription;
        public TextMeshProUGUI selectedExpeditionDetails;
        public Button startExpeditionButton;
        public Button cancelSelectionButton;
        public Transform availableAdventurersContent;
        public Transform assignedAdventurersContent;
    }

    private ExpeditionInfoReferences CreateExpeditionInfoPanel(Transform parent)
    {
        GameObject infoPanel = new GameObject("ExpeditionInfo");
        infoPanel.transform.SetParent(parent, false);

        // Layout
        VerticalLayoutGroup layout = infoPanel.AddComponent<VerticalLayoutGroup>();
        layout.childAlignment = TextAnchor.UpperLeft;
        layout.childControlWidth = true;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = true;
        layout.spacing = 10f;

        // Size
        LayoutElement layoutElement = infoPanel.AddComponent<LayoutElement>();
        layoutElement.preferredWidth = 300f;

        // Nom de l'expédition
        GameObject nameText = CreateText("SelectedExpeditionName", "Nom de l'expédition", parent: infoPanel.transform);
        TextMeshProUGUI nameComponent = nameText.GetComponent<TextMeshProUGUI>();
        nameComponent.fontSize = 20f;
        nameComponent.fontStyle = FontStyles.Bold;

        // Description
        GameObject descText = CreateText("SelectedExpeditionDescription", "Description de l'expédition", parent: infoPanel.transform);
        TextMeshProUGUI descComponent = descText.GetComponent<TextMeshProUGUI>();
        descComponent.fontSize = 14f;

        // Détails
        GameObject detailsText = CreateText("SelectedExpeditionDetails", "Détails", parent: infoPanel.transform);
        TextMeshProUGUI detailsComponent = detailsText.GetComponent<TextMeshProUGUI>();
        detailsComponent.fontSize = 12f;

        // Spacer
        GameObject spacer = new GameObject("Spacer");
        spacer.transform.SetParent(infoPanel.transform, false);
        LayoutElement spacerLayout = spacer.AddComponent<LayoutElement>();
        spacerLayout.flexibleHeight = 1f;

        // Boutons
        GameObject startButton = CreateButton("StartExpeditionButton", "Lancer l'Expédition", infoPanel.transform);
        GameObject cancelButton = CreateButton("CancelSelectionButton", "Annuler", infoPanel.transform);

        return new ExpeditionInfoReferences
        {
            nameText = nameComponent,
            descText = descComponent,
            detailsText = detailsComponent,
            startButton = startButton.GetComponent<Button>(),
            cancelButton = cancelButton.GetComponent<Button>()
        };
    }

    private class ExpeditionInfoReferences
    {
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI descText;
        public TextMeshProUGUI detailsText;
        public Button startButton;
        public Button cancelButton;
    }

    private AdventurerSelectionReferences CreateAdventurerSelectionPanel(Transform parent)
    {
        GameObject adventurerPanel = new GameObject("AdventurerSelection");
        adventurerPanel.transform.SetParent(parent, false);

        // Layout
        HorizontalLayoutGroup layout = adventurerPanel.AddComponent<HorizontalLayoutGroup>();
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = true;
        layout.spacing = 10f;

        // Size
        LayoutElement layoutElement = adventurerPanel.AddComponent<LayoutElement>();
        layoutElement.flexibleWidth = 1f;

        // Aventuriers disponibles
        var availableContent = CreateAdventurerListPanel("AvailableAdventurers", "Aventuriers Disponibles", adventurerPanel.transform);

        // Aventuriers assignés
        var assignedContent = CreateAdventurerListPanel("AssignedAdventurers", "Aventuriers Assignés", adventurerPanel.transform);

        return new AdventurerSelectionReferences
        {
            availableContent = availableContent,
            assignedContent = assignedContent
        };
    }

    private class AdventurerSelectionReferences
    {
        public Transform availableContent;
        public Transform assignedContent;
    }

    private Transform CreateAdventurerListPanel(string name, string title, Transform parent)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);

        // Background
        Image bgImage = panel.AddComponent<Image>();
        bgImage.color = new Color(0.15f, 0.15f, 0.15f, 0.8f);

        // Layout
        VerticalLayoutGroup layout = panel.AddComponent<VerticalLayoutGroup>();
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = true;
        layout.spacing = 5f;
        layout.padding = new RectOffset(10, 10, 10, 10);

        // Size
        LayoutElement layoutElement = panel.AddComponent<LayoutElement>();
        layoutElement.flexibleWidth = 1f;

        // Titre
        GameObject titleObj = CreateText($"{name}Title", title, parent: panel.transform);
        TextMeshProUGUI titleText = titleObj.GetComponent<TextMeshProUGUI>();
        titleText.fontSize = 16f;
        titleText.fontStyle = FontStyles.Bold;
        titleText.alignment = TextAlignmentOptions.Center;

        LayoutElement titleLayout = titleObj.AddComponent<LayoutElement>();
        titleLayout.preferredHeight = 25f;

        // Contenu avec scroll
        return CreateScrollableContent($"{name}Content", panel.transform);
    }

    private void SetExpeditionMenuReferences(ExpeditionMenu expeditionMenu, ExpeditionListReferences lists, ExpeditionDetailsReferences details, GameObject expeditionPrefab)
    {
        // Use reflection to set private fields
        var availableField = typeof(ExpeditionMenu).GetField("availableExpeditionsContent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var activeField = typeof(ExpeditionMenu).GetField("activeExpeditionsContent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var completedField = typeof(ExpeditionMenu).GetField("completedExpeditionsContent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var prefabField = typeof(ExpeditionMenu).GetField("expeditionItemPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var panelField = typeof(ExpeditionMenu).GetField("selectedExpeditionPanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var nameField = typeof(ExpeditionMenu).GetField("selectedExpeditionName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var descField = typeof(ExpeditionMenu).GetField("selectedExpeditionDescription", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var detailsField = typeof(ExpeditionMenu).GetField("selectedExpeditionDetails", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var availableAdvField = typeof(ExpeditionMenu).GetField("availableAdventurersContent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var assignedAdvField = typeof(ExpeditionMenu).GetField("assignedAdventurersContent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var startBtnField = typeof(ExpeditionMenu).GetField("startExpeditionButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var cancelBtnField = typeof(ExpeditionMenu).GetField("cancelSelectionButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        availableField?.SetValue(expeditionMenu, lists.availableExpeditionsContent);
        activeField?.SetValue(expeditionMenu, lists.activeExpeditionsContent);
        completedField?.SetValue(expeditionMenu, lists.completedExpeditionsContent);
        prefabField?.SetValue(expeditionMenu, expeditionPrefab);
        panelField?.SetValue(expeditionMenu, details.selectedExpeditionPanel);
        nameField?.SetValue(expeditionMenu, details.selectedExpeditionName);
        descField?.SetValue(expeditionMenu, details.selectedExpeditionDescription);
        detailsField?.SetValue(expeditionMenu, details.selectedExpeditionDetails);
        availableAdvField?.SetValue(expeditionMenu, details.availableAdventurersContent);
        assignedAdvField?.SetValue(expeditionMenu, details.assignedAdventurersContent);
        startBtnField?.SetValue(expeditionMenu, details.startExpeditionButton);
        cancelBtnField?.SetValue(expeditionMenu, details.cancelSelectionButton);

        EditorUtility.SetDirty(expeditionMenu);
    }
#endif
}