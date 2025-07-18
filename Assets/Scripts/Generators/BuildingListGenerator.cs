using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BuildingListGenerator : MonoBehaviour
{
    [Header("Target Container")]
    [SerializeField] private GameObject targetContainer;
    [SerializeField] private readonly Font uiFont;
    
    [Header("Building Item Configuration")]
    [SerializeField] private Color itemBackgroundColor = new(0.2f, 0.2f, 0.2f, 1f);
    [SerializeField] private Color buttonColor = new(0.4f, 0.4f, 0.4f, 1f);
    [SerializeField] private Color textColor = Color.white;
    [SerializeField] private int itemHeight = 120;
    [SerializeField] private int itemSpacing = 5;

#if UNITY_EDITOR
    [MenuItem("Grimspire/Generate Building List UI")]
    public static void GenerateBuildingListFromEditor()
    {
        BuildingListGenerator generator = FindObjectOfType<BuildingListGenerator>();
        if (generator == null)
        {
            GameObject generatorGO = new("BuildingListGenerator");
            generator = generatorGO.AddComponent<BuildingListGenerator>();
        }
        
        generator.GenerateBuildingListUI();
        
        EditorUtility.SetDirty(generator);
        Debug.Log("Building list UI generated successfully!");
    }
#endif

    public void GenerateBuildingListUI()
    {
        if (targetContainer == null)
        {
            Debug.LogError("Target container is not assigned!");
            return;
        }

        SetupContainer();
        CreateBuildingItemPrefab();
        SetupBuildingMenu();
    }

    private void SetupContainer()
    {
        // Add ScrollRect if not present
        ScrollRect scrollRect = targetContainer.GetComponent<ScrollRect>();
        if (scrollRect == null)
        {
            scrollRect = targetContainer.AddComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
        }

        // Create viewport if not present
        GameObject viewport = targetContainer.transform.Find("Viewport")?.gameObject;
        if (viewport == null)
        {
            viewport = new("Viewport");
            viewport.transform.SetParent(targetContainer.transform, false);
            
            RectTransform viewportRect = viewport.AddComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.offsetMin = Vector2.zero;
            viewportRect.offsetMax = Vector2.zero;
            
            Mask mask = viewport.AddComponent<Mask>();
            mask.showMaskGraphic = false;
            
            Image maskImage = viewport.AddComponent<Image>();
            maskImage.color = new(0, 0, 0, 0);
            
            scrollRect.viewport = viewportRect;
        }

        // Create content if not present
        GameObject content = viewport.transform.Find("Content")?.gameObject;
        if (content == null)
        {
            content = new("Content");
            content.transform.SetParent(viewport.transform, false);
            
            RectTransform contentRect = content.AddComponent<RectTransform>();
            contentRect.anchorMin = new(0, 1);
            contentRect.anchorMax = new(1, 1);
            contentRect.pivot = new(0.5f, 1);
            contentRect.sizeDelta = new(0, 0);
            
            VerticalLayoutGroup layoutGroup = content.AddComponent<VerticalLayoutGroup>();
            layoutGroup.childForceExpandWidth = true;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.spacing = itemSpacing;
            layoutGroup.padding = new RectOffset(10, 10, 10, 10);
            
            ContentSizeFitter sizeFitter = content.AddComponent<ContentSizeFitter>();
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            
            scrollRect.content = contentRect;
        }
    }

    private void CreateBuildingItemPrefab()
    {
        GameObject prefab = new("BuildingItemPrefab");
        prefab.transform.SetParent(targetContainer.transform, false);
        
        // Add RectTransform first
        RectTransform prefabRect = prefab.AddComponent<RectTransform>();
        prefabRect.sizeDelta = new(0, itemHeight);
        
        // Add BuildingItem component
        BuildingItem buildingItem = prefab.AddComponent<BuildingItem>();
        
        // Background
        Image backgroundImage = prefab.AddComponent<Image>();
        backgroundImage.color = itemBackgroundColor;
        
        // Create UI elements
        CreateNameText(prefab);
        CreateDescriptionText(prefab);
        CreateCostText(prefab);
        CreateStatusText(prefab);
        CreateActionButton(prefab);
        
        // Assign references to BuildingItem
        AssignBuildingItemReferences(buildingItem, prefab);
        
        // Make it a prefab by disabling it
        prefab.SetActive(false);
    }

    private void CreateNameText(GameObject parent)
    {
        GameObject nameGO = new("NameText");
        nameGO.transform.SetParent(parent.transform, false);
        
        Text nameText = nameGO.AddComponent<Text>();
        nameText.text = "Nom du Bâtiment";
        nameText.font = uiFont ?? Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        nameText.fontSize = 18;
        nameText.fontStyle = FontStyle.Bold;
        nameText.color = textColor;
        nameText.alignment = TextAnchor.MiddleLeft;
        
        RectTransform nameRect = nameGO.GetComponent<RectTransform>();
        nameRect.anchorMin = new(0, 0.7f);
        nameRect.anchorMax = new(0.7f, 1f);
        nameRect.offsetMin = new(10, 0);
        nameRect.offsetMax = new(-10, -5);
    }

    private void CreateDescriptionText(GameObject parent)
    {
        GameObject descGO = new("DescriptionText");
        descGO.transform.SetParent(parent.transform, false);
        
        Text descText = descGO.AddComponent<Text>();
        descText.text = "Description du bâtiment";
        descText.font = uiFont ?? Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        descText.fontSize = 12;
        descText.color = new(0.8f, 0.8f, 0.8f, 1f);
        descText.alignment = TextAnchor.UpperLeft;
        
        RectTransform descRect = descGO.GetComponent<RectTransform>();
        descRect.anchorMin = new(0, 0.4f);
        descRect.anchorMax = new(0.7f, 0.7f);
        descRect.offsetMin = new(10, 0);
        descRect.offsetMax = new(-10, 0);
    }

    private void CreateCostText(GameObject parent)
    {
        GameObject costGO = new("CostText");
        costGO.transform.SetParent(parent.transform, false);
        
        Text costText = costGO.AddComponent<Text>();
        costText.text = "Coût: 100 or, 10 bois";
        costText.font = uiFont ?? Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        costText.fontSize = 11;
        costText.color = new(1f, 0.9f, 0.3f, 1f);
        costText.alignment = TextAnchor.MiddleLeft;
        
        RectTransform costRect = costGO.GetComponent<RectTransform>();
        costRect.anchorMin = new(0, 0.2f);
        costRect.anchorMax = new(0.7f, 0.4f);
        costRect.offsetMin = new(10, 0);
        costRect.offsetMax = new(-10, 0);
    }

    private void CreateStatusText(GameObject parent)
    {
        GameObject statusGO = new("StatusText");
        statusGO.transform.SetParent(parent.transform, false);
        
        Text statusText = statusGO.AddComponent<Text>();
        statusText.text = "Disponible";
        statusText.font = uiFont ?? Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        statusText.fontSize = 10;
        statusText.color = Color.green;
        statusText.alignment = TextAnchor.MiddleLeft;
        
        RectTransform statusRect = statusGO.GetComponent<RectTransform>();
        statusRect.anchorMin = new(0, 0);
        statusRect.anchorMax = new(0.7f, 0.2f);
        statusRect.offsetMin = new(10, 2);
        statusRect.offsetMax = new(-10, -2);
    }

    private void CreateActionButton(GameObject parent)
    {
        GameObject buttonGO = new("ActionButton");
        buttonGO.transform.SetParent(parent.transform, false);
        
        Button button = buttonGO.AddComponent<Button>();
        Image buttonImage = buttonGO.AddComponent<Image>();
        buttonImage.color = buttonColor;
        button.targetGraphic = buttonImage;
        
        ColorBlock colors = button.colors;
        colors.normalColor = buttonColor;
        colors.highlightedColor = new(buttonColor.r + 0.1f, buttonColor.g + 0.1f, buttonColor.b + 0.1f, 1f);
        colors.pressedColor = new(buttonColor.r - 0.1f, buttonColor.g - 0.1f, buttonColor.b - 0.1f, 1f);
        button.colors = colors;
        
        RectTransform buttonRect = buttonGO.GetComponent<RectTransform>();
        buttonRect.anchorMin = new(0.72f, 0.2f);
        buttonRect.anchorMax = new(0.95f, 0.8f);
        buttonRect.offsetMin = Vector2.zero;
        buttonRect.offsetMax = Vector2.zero;
        
        // Button text
        GameObject textGO = new("Text");
        textGO.transform.SetParent(buttonGO.transform, false);
        
        Text buttonText = textGO.AddComponent<Text>();
        buttonText.text = "Construire";
        buttonText.font = uiFont ?? Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        buttonText.fontSize = 14;
        buttonText.color = textColor;
        buttonText.alignment = TextAnchor.MiddleCenter;
        
        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
    }

    private void AssignBuildingItemReferences(BuildingItem buildingItem, GameObject prefab)
    {
        // Use reflection to assign the private fields
        var nameField = typeof(BuildingItem).GetField("nameText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var descField = typeof(BuildingItem).GetField("descriptionText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var costField = typeof(BuildingItem).GetField("costText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var statusField = typeof(BuildingItem).GetField("statusText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var buttonField = typeof(BuildingItem).GetField("actionButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var backgroundField = typeof(BuildingItem).GetField("backgroundImage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        nameField?.SetValue(buildingItem, prefab.transform.Find("NameText").GetComponent<Text>());
        descField?.SetValue(buildingItem, prefab.transform.Find("DescriptionText").GetComponent<Text>());
        costField?.SetValue(buildingItem, prefab.transform.Find("CostText").GetComponent<Text>());
        statusField?.SetValue(buildingItem, prefab.transform.Find("StatusText").GetComponent<Text>());
        buttonField?.SetValue(buildingItem, prefab.transform.Find("ActionButton").GetComponent<Button>());
        backgroundField?.SetValue(buildingItem, prefab.GetComponent<Image>());
    }

    private void SetupBuildingMenu()
    {
        // Find the Content_Bâtiments container
        GameObject contentBatiments = GameObject.Find("Content_Bâtiments");
        if (contentBatiments == null)
        {
            Debug.LogWarning("Content_Bâtiments not found! Make sure the GameMenuGenerator has been run first.");
            return;
        }

        // Add BuildingMenu component if not present
        BuildingMenu buildingMenu = contentBatiments.GetComponent<BuildingMenu>();
        if (buildingMenu == null)
        {
            buildingMenu = contentBatiments.AddComponent<BuildingMenu>();
        }

        // Assign references
        GameObject content = targetContainer.transform.Find("Viewport/Content")?.gameObject;
        GameObject prefab = targetContainer.transform.Find("BuildingItemPrefab")?.gameObject;
        
        var containerField = typeof(BuildingMenu).GetField("buildingListContainer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var prefabField = typeof(BuildingMenu).GetField("buildingItemPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var scrollField = typeof(BuildingMenu).GetField("scrollRect", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        containerField?.SetValue(buildingMenu, content);
        prefabField?.SetValue(buildingMenu, prefab);
        scrollField?.SetValue(buildingMenu, targetContainer.GetComponent<ScrollRect>());
        
        Debug.Log("BuildingMenu setup completed!");
    }
}