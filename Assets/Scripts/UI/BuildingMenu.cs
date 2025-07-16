using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class BuildingMenu : BaseMenu
{
    [Header("Building Menu")]
    [SerializeField] private Transform categoryButtonContainer;
    [SerializeField] private Transform buildingListContainer;
    [SerializeField] private Transform constructionQueueContainer;
    [SerializeField] private ScrollRect buildingScrollRect;
    
    [Header("Building Item Prefabs")]
    [SerializeField] private GameObject buildingItemPrefab;
    [SerializeField] private GameObject constructionItemPrefab;
    [SerializeField] private GameObject categoryButtonPrefab;
    
    [Header("Building Details Panel")]
    [SerializeField] private GameObject buildingDetailsPanel;
    [SerializeField] private Image buildingIcon;
    [SerializeField] private Text buildingNameText;
    [SerializeField] private Text buildingDescriptionText;
    [SerializeField] private Text buildingCostText;
    [SerializeField] private Text buildingEffectsText;
    [SerializeField] private Button constructButton;
    [SerializeField] private Button upgradeButton;
    
    [Header("Filter Options")]
    [SerializeField] private Toggle showAvailableOnlyToggle;
    [SerializeField] private Toggle showAffordableOnlyToggle;
    
    private Building.BuildingCategory currentCategory = Building.BuildingCategory.Residential;
    private BuildingData selectedBuildingData;
    private Building selectedBuilding;
    private Dictionary<Building.BuildingCategory, Button> categoryButtons;
    private List<GameObject> currentBuildingItems;
    private List<GameObject> currentConstructionItems;

    protected override void Awake()
    {
        base.Awake();
        categoryButtons = new Dictionary<Building.BuildingCategory, Button>();
        currentBuildingItems = new List<GameObject>();
        currentConstructionItems = new List<GameObject>();
    }

    protected override void OnMenuShown()
    {
        base.OnMenuShown();
        InitializeBuildingMenu();
        RefreshBuildingList();
        RefreshConstructionQueue();
    }

    private void OnEnable()
    {
        SubscribeToEvents();
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        if (ConstructionSystem.Instance != null)
        {
            ConstructionSystem.OnConstructionStarted += OnConstructionStarted;
            ConstructionSystem.OnConstructionCompleted += OnConstructionCompleted;
            ConstructionSystem.OnConstructionCancelled += OnConstructionCancelled;
            ConstructionSystem.OnConstructionProgress += OnConstructionProgress;
        }
        
        if (ResourceManager.Instance != null)
        {
            ResourceManager.OnResourceChanged += OnResourceChanged;
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (ConstructionSystem.Instance != null)
        {
            ConstructionSystem.OnConstructionStarted -= OnConstructionStarted;
            ConstructionSystem.OnConstructionCompleted -= OnConstructionCompleted;
            ConstructionSystem.OnConstructionCancelled -= OnConstructionCancelled;
            ConstructionSystem.OnConstructionProgress -= OnConstructionProgress;
        }
        
        if (ResourceManager.Instance != null)
        {
            ResourceManager.OnResourceChanged -= OnResourceChanged;
        }
    }

    private void InitializeBuildingMenu()
    {
        CreateCategoryButtons();
        SetupFilterToggles();
        SetupDetailsPanelButtons();
        
        if (buildingDetailsPanel != null)
        {
            buildingDetailsPanel.SetActive(false);
        }
    }

    private void CreateCategoryButtons()
    {
        if (categoryButtonContainer == null) return;
        
        // Clear existing buttons
        foreach (Transform child in categoryButtonContainer)
        {
            Destroy(child.gameObject);
        }
        categoryButtons.Clear();
        
        // Create category buttons
        var categories = System.Enum.GetValues(typeof(Building.BuildingCategory));
        foreach (Building.BuildingCategory category in categories)
        {
            CreateCategoryButton(category);
        }
        
        // Select first category
        if (categoryButtons.Count > 0)
        {
            SelectCategory(currentCategory);
        }
    }

    private void CreateCategoryButton(Building.BuildingCategory category)
    {
        GameObject buttonObject;
        
        if (categoryButtonPrefab != null)
        {
            buttonObject = Instantiate(categoryButtonPrefab, categoryButtonContainer);
        }
        else
        {
            buttonObject = CreateDefaultCategoryButton(category);
        }
        
        Button button = buttonObject.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => SelectCategory(category));
            categoryButtons[category] = button;
            
            // Update button text
            Text buttonText = button.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = GetCategoryDisplayName(category);
            }
        }
    }

    private GameObject CreateDefaultCategoryButton(Building.BuildingCategory category)
    {
        GameObject buttonObject = new GameObject($"Category_{category}");
        buttonObject.transform.SetParent(categoryButtonContainer, false);
        
        RectTransform rect = buttonObject.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(120, 40);
        
        Button button = buttonObject.AddComponent<Button>();
        Image buttonImage = buttonObject.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.3f, 0.5f, 1f);
        
        GameObject textObject = new GameObject("Text");
        textObject.transform.SetParent(buttonObject.transform, false);
        RectTransform textRect = textObject.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        
        Text text = textObject.AddComponent<Text>();
        text.text = GetCategoryDisplayName(category);
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 14;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        
        button.targetGraphic = buttonImage;
        
        return buttonObject;
    }

    private void SetupFilterToggles()
    {
        if (showAvailableOnlyToggle != null)
        {
            showAvailableOnlyToggle.onValueChanged.AddListener(OnFilterChanged);
        }
        
        if (showAffordableOnlyToggle != null)
        {
            showAffordableOnlyToggle.onValueChanged.AddListener(OnFilterChanged);
        }
    }

    private void SetupDetailsPanelButtons()
    {
        if (constructButton != null)
        {
            constructButton.onClick.AddListener(OnConstructButtonPressed);
        }
        
        if (upgradeButton != null)
        {
            upgradeButton.onClick.AddListener(OnUpgradeButtonPressed);
        }
    }

    private void SelectCategory(Building.BuildingCategory category)
    {
        currentCategory = category;
        
        // Update button states
        foreach (var kvp in categoryButtons)
        {
            if (kvp.Value != null)
            {
                ColorBlock colors = kvp.Value.colors;
                colors.normalColor = kvp.Key == category ? Color.yellow : Color.white;
                kvp.Value.colors = colors;
            }
        }
        
        RefreshBuildingList();
    }

    private void RefreshBuildingList()
    {
        ClearBuildingList();
        
        if (BuildingDB.Database == null) return;
        
        BuildingData[] categoryBuildings = BuildingDB.GetBuildingsByCategory(currentCategory);
        
        foreach (var buildingData in categoryBuildings)
        {
            if (ShouldShowBuilding(buildingData))
            {
                CreateBuildingListItem(buildingData);
            }
        }
    }

    private void ClearBuildingList()
    {
        foreach (var item in currentBuildingItems)
        {
            if (item != null) Destroy(item);
        }
        currentBuildingItems.Clear();
    }

    private bool ShouldShowBuilding(BuildingData buildingData)
    {
        if (buildingData == null) return false;
        
        // Check available only filter
        if (showAvailableOnlyToggle != null && showAvailableOnlyToggle.isOn)
        {
            if (GameManager.Instance != null && GameManager.Instance.CurrentCity != null)
            {
                int cityLevel = GameManager.Instance.CurrentCity.CityLevel;
                if (!BuildingDB.Database.IsBuildingUnlocked(buildingData.buildingType, cityLevel, null))
                {
                    return false;
                }
            }
        }
        
        // Check affordable only filter
        if (showAffordableOnlyToggle != null && showAffordableOnlyToggle.isOn)
        {
            if (!BuildingDB.CanAffordBuilding(buildingData.buildingType))
            {
                return false;
            }
        }
        
        return true;
    }

    private void CreateBuildingListItem(BuildingData buildingData)
    {
        GameObject itemObject;
        
        if (buildingItemPrefab != null)
        {
            itemObject = Instantiate(buildingItemPrefab, buildingListContainer);
        }
        else
        {
            itemObject = CreateDefaultBuildingItem(buildingData);
        }
        
        currentBuildingItems.Add(itemObject);
        
        // Setup item click handler
        Button itemButton = itemObject.GetComponent<Button>();
        if (itemButton != null)
        {
            itemButton.onClick.AddListener(() => SelectBuilding(buildingData));
        }
        
        // Update item display
        UpdateBuildingItemDisplay(itemObject, buildingData);
    }

    private GameObject CreateDefaultBuildingItem(BuildingData buildingData)
    {
        GameObject itemObject = new GameObject($"BuildingItem_{buildingData.buildingType}");
        itemObject.transform.SetParent(buildingListContainer, false);
        
        RectTransform rect = itemObject.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(300, 80);
        
        Button button = itemObject.AddComponent<Button>();
        Image backgroundImage = itemObject.AddComponent<Image>();
        backgroundImage.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        
        // Add horizontal layout
        HorizontalLayoutGroup layout = itemObject.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 10;
        layout.padding = new RectOffset(10, 10, 10, 10);
        layout.childAlignment = TextAnchor.MiddleLeft;
        
        // Create icon
        GameObject iconObject = new GameObject("Icon");
        iconObject.transform.SetParent(itemObject.transform, false);
        RectTransform iconRect = iconObject.AddComponent<RectTransform>();
        iconRect.sizeDelta = new Vector2(60, 60);
        Image iconImage = iconObject.AddComponent<Image>();
        iconImage.sprite = buildingData.icon;
        
        // Create text panel
        GameObject textPanel = new GameObject("TextPanel");
        textPanel.transform.SetParent(itemObject.transform, false);
        RectTransform textRect = textPanel.AddComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(200, 60);
        
        VerticalLayoutGroup textLayout = textPanel.AddComponent<VerticalLayoutGroup>();
        textLayout.childAlignment = TextAnchor.UpperLeft;
        
        // Create name text
        GameObject nameObject = new GameObject("Name");
        nameObject.transform.SetParent(textPanel.transform, false);
        Text nameText = nameObject.AddComponent<Text>();
        nameText.text = buildingData.displayName;
        nameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        nameText.fontSize = 16;
        nameText.color = Color.white;
        
        // Create cost text
        GameObject costObject = new GameObject("Cost");
        costObject.transform.SetParent(textPanel.transform, false);
        Text costText = costObject.AddComponent<Text>();
        costText.text = GetBuildingCostText(buildingData);
        costText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        costText.fontSize = 12;
        costText.color = Color.gray;
        
        button.targetGraphic = backgroundImage;
        
        return itemObject;
    }

    private void UpdateBuildingItemDisplay(GameObject itemObject, BuildingData buildingData)
    {
        // Update affordability color
        bool canAfford = BuildingDB.CanAffordBuilding(buildingData.buildingType);
        Image backgroundImage = itemObject.GetComponent<Image>();
        if (backgroundImage != null)
        {
            backgroundImage.color = canAfford ? new Color(0.1f, 0.3f, 0.1f, 0.8f) : new Color(0.3f, 0.1f, 0.1f, 0.8f);
        }
    }

    private void SelectBuilding(BuildingData buildingData)
    {
        selectedBuildingData = buildingData;
        selectedBuilding = null;
        
        if (ConstructionSystem.Instance != null)
        {
            selectedBuilding = ConstructionSystem.Instance.GetBuilding(buildingData.buildingType);
        }
        
        ShowBuildingDetails();
    }

    private void ShowBuildingDetails()
    {
        if (buildingDetailsPanel == null || selectedBuildingData == null) return;
        
        buildingDetailsPanel.SetActive(true);
        
        // Update building info
        if (buildingIcon != null)
            buildingIcon.sprite = selectedBuildingData.icon;
        
        if (buildingNameText != null)
            buildingNameText.text = selectedBuildingData.displayName;
        
        if (buildingDescriptionText != null)
            buildingDescriptionText.text = selectedBuildingData.description;
        
        if (buildingCostText != null)
            buildingCostText.text = GetBuildingCostText(selectedBuildingData);
        
        if (buildingEffectsText != null)
            buildingEffectsText.text = GetBuildingEffectsText(selectedBuildingData);
        
        // Update buttons
        UpdateDetailsButtons();
    }

    private void UpdateDetailsButtons()
    {
        if (constructButton != null)
        {
            string reason = "";
            bool canConstruct = selectedBuilding == null && ConstructionSystem.Instance != null &&
                               ConstructionSystem.Instance.CanConstructBuilding(selectedBuildingData.buildingType, out reason);
            constructButton.interactable = canConstruct;
            
            Text buttonText = constructButton.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = canConstruct ? "CONSTRUIRE" : reason;
            }
        }
        
        if (upgradeButton != null)
        {
            bool canUpgrade = selectedBuilding != null && selectedBuilding.Level < selectedBuildingData.maxLevel &&
                             selectedBuildingData.canUpgrade;
            upgradeButton.interactable = canUpgrade;
            upgradeButton.gameObject.SetActive(selectedBuilding != null);
        }
    }

    private void RefreshConstructionQueue()
    {
        ClearConstructionQueue();
        
        if (ConstructionSystem.Instance == null) return;
        
        foreach (var project in ConstructionSystem.Instance.ActiveProjects)
        {
            CreateConstructionQueueItem(project);
        }
    }

    private void ClearConstructionQueue()
    {
        foreach (var item in currentConstructionItems)
        {
            if (item != null) Destroy(item);
        }
        currentConstructionItems.Clear();
    }

    private void CreateConstructionQueueItem(ConstructionProject project)
    {
        GameObject itemObject;
        
        if (constructionItemPrefab != null)
        {
            itemObject = Instantiate(constructionItemPrefab, constructionQueueContainer);
        }
        else
        {
            itemObject = CreateDefaultConstructionItem(project);
        }
        
        currentConstructionItems.Add(itemObject);
        UpdateConstructionItemDisplay(itemObject, project);
    }

    private GameObject CreateDefaultConstructionItem(ConstructionProject project)
    {
        GameObject itemObject = new GameObject($"ConstructionItem_{project.BuildingType}");
        itemObject.transform.SetParent(constructionQueueContainer, false);
        
        RectTransform rect = itemObject.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(250, 60);
        
        Image backgroundImage = itemObject.AddComponent<Image>();
        backgroundImage.color = new Color(0.2f, 0.2f, 0.4f, 0.8f);
        
        // Create progress bar
        GameObject progressBarObject = new GameObject("ProgressBar");
        progressBarObject.transform.SetParent(itemObject.transform, false);
        RectTransform progressRect = progressBarObject.AddComponent<RectTransform>();
        progressRect.anchorMin = new Vector2(0, 0);
        progressRect.anchorMax = new Vector2(1, 0.3f);
        progressRect.sizeDelta = Vector2.zero;
        
        Slider progressBar = progressBarObject.AddComponent<Slider>();
        progressBar.value = project.Progress;
        
        // Create text
        GameObject textObject = new GameObject("Text");
        textObject.transform.SetParent(itemObject.transform, false);
        RectTransform textRect = textObject.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0, 0.3f);
        textRect.anchorMax = new Vector2(1, 1);
        textRect.sizeDelta = Vector2.zero;
        
        Text text = textObject.AddComponent<Text>();
        text.text = $"{project.BuildingType}\n{project.Progress:P0}";
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 12;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        
        return itemObject;
    }

    private void UpdateConstructionItemDisplay(GameObject itemObject, ConstructionProject project)
    {
        Slider progressBar = itemObject.GetComponentInChildren<Slider>();
        if (progressBar != null)
        {
            progressBar.value = project.Progress;
        }
        
        Text text = itemObject.GetComponentInChildren<Text>();
        if (text != null)
        {
            text.text = $"{project.BuildingType}\n{project.Progress:P0}";
        }
    }

    private string GetBuildingCostText(BuildingData buildingData)
    {
        if (buildingData.buildCosts == null || buildingData.buildCosts.Length == 0)
            return "Gratuit";
        
        string costText = "Coût: ";
        for (int i = 0; i < buildingData.buildCosts.Length; i++)
        {
            var cost = buildingData.buildCosts[i];
            if (i > 0) costText += ", ";
            costText += $"{cost.amount} {cost.resourceType}";
        }
        
        return costText;
    }

    private string GetBuildingEffectsText(BuildingData buildingData)
    {
        if (buildingData.buildingEffects == null || buildingData.buildingEffects.Length == 0)
            return "Aucun effet spécial";
        
        string effectsText = "Effets:\n";
        foreach (var effect in buildingData.buildingEffects)
        {
            effectsText += $"• {effect.description}\n";
        }
        
        return effectsText.TrimEnd('\n');
    }

    private string GetCategoryDisplayName(Building.BuildingCategory category)
    {
        switch (category)
        {
            case Building.BuildingCategory.Residential: return "Résidentiel";
            case Building.BuildingCategory.Commercial: return "Commercial";
            case Building.BuildingCategory.Industrial: return "Industriel";
            case Building.BuildingCategory.Administrative: return "Administratif";
            default: return category.ToString();
        }
    }

    private void OnFilterChanged(bool value)
    {
        RefreshBuildingList();
    }

    private void OnConstructButtonPressed()
    {
        if (selectedBuildingData != null && ConstructionSystem.Instance != null)
        {
            ConstructionSystem.Instance.StartConstruction(selectedBuildingData.buildingType);
        }
    }

    private void OnUpgradeButtonPressed()
    {
        if (selectedBuilding != null && ConstructionSystem.Instance != null)
        {
            ConstructionSystem.Instance.UpgradeBuilding(selectedBuilding);
        }
    }

    private void OnConstructionStarted(Building.BuildingType buildingType)
    {
        RefreshConstructionQueue();
        RefreshBuildingList();
        UpdateDetailsButtons();
    }

    private void OnConstructionCompleted(Building.BuildingType buildingType)
    {
        RefreshConstructionQueue();
        RefreshBuildingList();
        UpdateDetailsButtons();
    }

    private void OnConstructionCancelled(Building.BuildingType buildingType)
    {
        RefreshConstructionQueue();
        RefreshBuildingList();
        UpdateDetailsButtons();
    }

    private void OnConstructionProgress(Building.BuildingType buildingType, float progress)
    {
        // Update specific construction item progress
        // Find and update the corresponding construction item
    }

    private void OnResourceChanged(Resource.ResourceType resourceType, int previousAmount, int newAmount)
    {
        RefreshBuildingList();
        UpdateDetailsButtons();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        
        if (showAvailableOnlyToggle != null)
            showAvailableOnlyToggle.onValueChanged.RemoveListener(OnFilterChanged);
        
        if (showAffordableOnlyToggle != null)
            showAffordableOnlyToggle.onValueChanged.RemoveListener(OnFilterChanged);
        
        if (constructButton != null)
            constructButton.onClick.RemoveListener(OnConstructButtonPressed);
        
        if (upgradeButton != null)
            upgradeButton.onClick.RemoveListener(OnUpgradeButtonPressed);
    }
}