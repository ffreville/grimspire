using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AdventurerMenuGenerator : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Grimspire/Setup Adventurer Menu")]
    public static void SetupAdventurerMenu()
    {
        // Find Content_Guilde
        GameObject contentGuilde = GameObject.Find("Content_Guilde");
        if (contentGuilde == null)
        {
            Debug.LogError("Content_Guilde not found! Create the game menu first.");
            return;
        }

        // Clear existing content
        for (int i = contentGuilde.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(contentGuilde.transform.GetChild(i).gameObject);
        }

        // Create main container
        GameObject mainContainer = new("AdventurerMenuContainer");
        mainContainer.transform.SetParent(contentGuilde.transform, false);
        
        RectTransform mainRect = mainContainer.AddComponent<RectTransform>();
        mainRect.anchorMin = Vector2.zero;
        mainRect.anchorMax = Vector2.one;
        mainRect.offsetMin = Vector2.zero;
        mainRect.offsetMax = Vector2.zero;

        // Create title
        CreateTitle(mainContainer);
        
        // Create search section
        GameObject searchSection = CreateSearchSection(mainContainer);
        
        // Create two-column layout for lists
        GameObject listsContainer = CreateListsContainer(mainContainer);
        
        // Create recruited list
        GameObject recruitedSection = CreateRecruitedSection(listsContainer);
        
        // Create available list
        GameObject availableSection = CreateAvailableSection(listsContainer);
        
        // Create simple adventurer item prefab
        GameObject adventurerPrefab = CreateAdventurerItemPrefab(mainContainer);
        
        // Setup AdventurerMenu component
        SetupAdventurerMenuComponent(contentGuilde, recruitedSection, availableSection, 
                                    adventurerPrefab, searchSection);
        
        Debug.Log("Adventurer menu setup completed!");
    }

    private static void CreateTitle(GameObject parent)
    {
        GameObject titleGO = new("Title");
        titleGO.transform.SetParent(parent.transform, false);
        
        Text titleText = titleGO.AddComponent<Text>();
        titleText.text = "Guilde des Aventuriers";
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleText.fontSize = 24;
        titleText.fontStyle = FontStyle.Bold;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = Color.white;
        
        RectTransform titleRect = titleGO.GetComponent<RectTransform>();
        titleRect.anchorMin = new(0, 0.9f);
        titleRect.anchorMax = new(1, 1);
        titleRect.offsetMin = new(10, 0);
        titleRect.offsetMax = new(-10, 0);
    }

    private static GameObject CreateSearchSection(GameObject parent)
    {
        GameObject searchSection = new("SearchSection");
        searchSection.transform.SetParent(parent.transform, false);
        
        RectTransform searchRect = searchSection.AddComponent<RectTransform>();
        searchRect.anchorMin = new(0, 0.8f);
        searchRect.anchorMax = new(1, 0.9f);
        searchRect.offsetMin = new(10, 0);
        searchRect.offsetMax = new(-10, 0);
        
        // Background
        Image searchBg = searchSection.AddComponent<Image>();
        searchBg.color = new(0.2f, 0.2f, 0.2f, 0.8f);
        
        // Search button
        GameObject searchButtonGO = new("SearchButton");
        searchButtonGO.transform.SetParent(searchSection.transform, false);
        
        Button searchButton = searchButtonGO.AddComponent<Button>();
        Image buttonImage = searchButtonGO.AddComponent<Image>();
        buttonImage.color = new(0.4f, 0.4f, 0.4f, 1f);
        searchButton.targetGraphic = buttonImage;
        
        RectTransform buttonRect = searchButtonGO.GetComponent<RectTransform>();
        buttonRect.anchorMin = new(0.4f, 0.2f);
        buttonRect.anchorMax = new(0.6f, 0.8f);
        buttonRect.offsetMin = Vector2.zero;
        buttonRect.offsetMax = Vector2.zero;
        
        // Button text
        GameObject buttonTextGO = new("Text");
        buttonTextGO.transform.SetParent(searchButtonGO.transform, false);
        
        Text buttonText = buttonTextGO.AddComponent<Text>();
        buttonText.text = "Rechercher (50 or)";
        buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        buttonText.fontSize = 16;
        buttonText.color = Color.white;
        buttonText.alignment = TextAnchor.MiddleCenter;
        
        RectTransform buttonTextRect = buttonTextGO.GetComponent<RectTransform>();
        buttonTextRect.anchorMin = Vector2.zero;
        buttonTextRect.anchorMax = Vector2.one;
        buttonTextRect.offsetMin = Vector2.zero;
        buttonTextRect.offsetMax = Vector2.zero;
        
        return searchSection;
    }

    private static GameObject CreateListsContainer(GameObject parent)
    {
        GameObject listsContainer = new("ListsContainer");
        listsContainer.transform.SetParent(parent.transform, false);
        
        RectTransform listsRect = listsContainer.AddComponent<RectTransform>();
        listsRect.anchorMin = new(0, 0);
        listsRect.anchorMax = new(1, 0.8f);
        listsRect.offsetMin = new(10, 10);
        listsRect.offsetMax = new(-10, 0);
        
        // Add horizontal layout
        HorizontalLayoutGroup layout = listsContainer.AddComponent<HorizontalLayoutGroup>();
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = true;
        layout.spacing = 10;
        
        return listsContainer;
    }

    private static GameObject CreateRecruitedSection(GameObject parent)
    {
        GameObject recruitedSection = new("RecruitedSection");
        recruitedSection.transform.SetParent(parent.transform, false);
        
        // Section title
        GameObject titleGO = new("Title");
        titleGO.transform.SetParent(recruitedSection.transform, false);
        
        Text titleText = titleGO.AddComponent<Text>();
        titleText.text = "Aventuriers Recrutés";
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleText.fontSize = 18;
        titleText.fontStyle = FontStyle.Bold;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = Color.green;
        
        RectTransform titleRect = titleGO.GetComponent<RectTransform>();
        titleRect.anchorMin = new(0, 0.9f);
        titleRect.anchorMax = new(1, 1);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;
        
        // Create scroll view
        GameObject scrollView = CreateScrollView(recruitedSection, "RecruitedScrollView");
        
        return scrollView;
    }

    private static GameObject CreateAvailableSection(GameObject parent)
    {
        GameObject availableSection = new("AvailableSection");
        availableSection.transform.SetParent(parent.transform, false);
        
        // Section title
        GameObject titleGO = new("Title");
        titleGO.transform.SetParent(availableSection.transform, false);
        
        Text titleText = titleGO.AddComponent<Text>();
        titleText.text = "Aventuriers Disponibles";
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleText.fontSize = 18;
        titleText.fontStyle = FontStyle.Bold;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = Color.cyan;
        
        RectTransform titleRect = titleGO.GetComponent<RectTransform>();
        titleRect.anchorMin = new(0, 0.9f);
        titleRect.anchorMax = new(1, 1);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;
        
        // Create scroll view
        GameObject scrollView = CreateScrollView(availableSection, "AvailableScrollView");
        
        return scrollView;
    }

    private static GameObject CreateScrollView(GameObject parent, string name)
    {
        GameObject scrollViewGO = new(name);
        scrollViewGO.transform.SetParent(parent.transform, false);
        
        RectTransform scrollRect = scrollViewGO.AddComponent<RectTransform>();
        scrollRect.anchorMin = new(0, 0);
        scrollRect.anchorMax = new(1, 0.9f);
        scrollRect.offsetMin = Vector2.zero;
        scrollRect.offsetMax = Vector2.zero;
        
        // Add ScrollRect component
        ScrollRect scroll = scrollViewGO.AddComponent<ScrollRect>();
        scroll.horizontal = false;
        scroll.vertical = true;
        
        // Create viewport
        GameObject viewport = new("Viewport");
        viewport.transform.SetParent(scrollViewGO.transform, false);
        
        RectTransform viewportRect = viewport.AddComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.offsetMin = Vector2.zero;
        viewportRect.offsetMax = Vector2.zero;
        
        viewport.AddComponent<Mask>();
        Image viewportImage = viewport.AddComponent<Image>();
        viewportImage.color = new(0.1f, 0.1f, 0.1f, 0.5f);
        
        scroll.viewport = viewportRect;
        
        // Create content
        GameObject content = new("Content");
        content.transform.SetParent(viewport.transform, false);
        
        RectTransform contentRect = content.AddComponent<RectTransform>();
        contentRect.anchorMin = new(0, 1);
        contentRect.anchorMax = new(1, 1);
        contentRect.pivot = new(0.5f, 1);
        contentRect.sizeDelta = new(0, 0);
        
        VerticalLayoutGroup layout = content.AddComponent<VerticalLayoutGroup>();
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;
        layout.spacing = 5;
        layout.padding = new RectOffset(5, 5, 5, 5);
        
        ContentSizeFitter fitter = content.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        scroll.content = contentRect;
        
        return content; // Return the content, not the scroll view
    }

    private static GameObject CreateAdventurerItemPrefab(GameObject parent)
    {
        GameObject prefab = new("AdventurerItemPrefab");
        prefab.transform.SetParent(parent.transform, false);
        
        // Add RectTransform
        RectTransform prefabRect = prefab.AddComponent<RectTransform>();
        prefabRect.sizeDelta = new(0, 120);
        
        // Background
        Image bg = prefab.AddComponent<Image>();
        bg.color = new(0.3f, 0.3f, 0.3f, 1f);
        
        // Name text
        GameObject nameGO = new("NameText");
        nameGO.transform.SetParent(prefab.transform, false);
        
        Text nameText = nameGO.AddComponent<Text>();
        nameText.text = "Nom Aventurier";
        nameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        nameText.fontSize = 16;
        nameText.fontStyle = FontStyle.Bold;
        nameText.color = Color.white;
        nameText.alignment = TextAnchor.MiddleLeft;
        
        RectTransform nameRect = nameGO.GetComponent<RectTransform>();
        nameRect.anchorMin = new(0, 0.8f);
        nameRect.anchorMax = new(0.7f, 1);
        nameRect.offsetMin = new(10, 0);
        nameRect.offsetMax = new(-10, 0);
        
        // Class text
        GameObject classGO = new("ClassText");
        classGO.transform.SetParent(prefab.transform, false);
        
        Text classText = classGO.AddComponent<Text>();
        classText.text = "Classe";
        classText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        classText.fontSize = 14;
        classText.color = Color.yellow;
        classText.alignment = TextAnchor.MiddleLeft;
        
        RectTransform classRect = classGO.GetComponent<RectTransform>();
        classRect.anchorMin = new(0, 0.6f);
        classRect.anchorMax = new(0.7f, 0.8f);
        classRect.offsetMin = new(10, 0);
        classRect.offsetMax = new(-10, 0);
        
        // Level text
        GameObject levelGO = new("LevelText");
        levelGO.transform.SetParent(prefab.transform, false);
        
        Text levelText = levelGO.AddComponent<Text>();
        levelText.text = "Niv. X";
        levelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        levelText.fontSize = 12;
        levelText.color = Color.white;
        levelText.alignment = TextAnchor.MiddleLeft;
        
        RectTransform levelRect = levelGO.GetComponent<RectTransform>();
        levelRect.anchorMin = new(0, 0.4f);
        levelRect.anchorMax = new(0.7f, 0.6f);
        levelRect.offsetMin = new(10, 0);
        levelRect.offsetMax = new(-10, 0);
        
        // Stats text
        GameObject statsGO = new("StatsText");
        statsGO.transform.SetParent(prefab.transform, false);
        
        Text statsText = statsGO.AddComponent<Text>();
        statsText.text = "FOR:10 INT:10 AGI:10";
        statsText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        statsText.fontSize = 10;
        statsText.color = Color.gray;
        statsText.alignment = TextAnchor.UpperLeft;
        
        RectTransform statsRect = statsGO.GetComponent<RectTransform>();
        statsRect.anchorMin = new(0, 0.2f);
        statsRect.anchorMax = new(0.7f, 0.4f);
        statsRect.offsetMin = new(10, 0);
        statsRect.offsetMax = new(-10, 0);
        
        // Status text
        GameObject statusGO = new("StatusText");
        statusGO.transform.SetParent(prefab.transform, false);
        
        Text statusText = statusGO.AddComponent<Text>();
        statusText.text = "Disponible";
        statusText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        statusText.fontSize = 10;
        statusText.color = Color.green;
        statusText.alignment = TextAnchor.MiddleLeft;
        
        RectTransform statusRect = statusGO.GetComponent<RectTransform>();
        statusRect.anchorMin = new(0, 0);
        statusRect.anchorMax = new(0.7f, 0.2f);
        statusRect.offsetMin = new(10, 0);
        statusRect.offsetMax = new(-10, 0);
        
        // Action button
        GameObject buttonGO = new("ActionButton");
        buttonGO.transform.SetParent(prefab.transform, false);
        
        Button button = buttonGO.AddComponent<Button>();
        Image buttonImage = buttonGO.AddComponent<Image>();
        buttonImage.color = new(0.5f, 0.5f, 0.5f, 1f);
        button.targetGraphic = buttonImage;
        
        RectTransform buttonRect = buttonGO.GetComponent<RectTransform>();
        buttonRect.anchorMin = new(0.75f, 0.3f);
        buttonRect.anchorMax = new(0.95f, 0.7f);
        buttonRect.offsetMin = Vector2.zero;
        buttonRect.offsetMax = Vector2.zero;
        
        GameObject buttonTextGO = new("Text");
        buttonTextGO.transform.SetParent(buttonGO.transform, false);
        
        Text buttonText = buttonTextGO.AddComponent<Text>();
        buttonText.text = "Action";
        buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        buttonText.fontSize = 12;
        buttonText.color = Color.white;
        buttonText.alignment = TextAnchor.MiddleCenter;
        
        RectTransform buttonTextRect = buttonTextGO.GetComponent<RectTransform>();
        buttonTextRect.anchorMin = Vector2.zero;
        buttonTextRect.anchorMax = Vector2.one;
        buttonTextRect.offsetMin = Vector2.zero;
        buttonTextRect.offsetMax = Vector2.zero;
        
        // Add AdventurerItem component
        AdventurerItem item = prefab.AddComponent<AdventurerItem>();
        
        // Assign references using reflection
        var nameField = typeof(AdventurerItem).GetField("nameText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var classField = typeof(AdventurerItem).GetField("classText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var levelField = typeof(AdventurerItem).GetField("levelText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var statsField = typeof(AdventurerItem).GetField("statsText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var statusField = typeof(AdventurerItem).GetField("statusText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var buttonField = typeof(AdventurerItem).GetField("actionButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var backgroundField = typeof(AdventurerItem).GetField("backgroundImage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        nameField?.SetValue(item, nameText);
        classField?.SetValue(item, classText);
        levelField?.SetValue(item, levelText);
        statsField?.SetValue(item, statsText);
        statusField?.SetValue(item, statusText);
        buttonField?.SetValue(item, button);
        backgroundField?.SetValue(item, bg);
        
        prefab.SetActive(false);
        
        return prefab;
    }

    private static void SetupAdventurerMenuComponent(GameObject parent, GameObject recruitedContent, 
                                                   GameObject availableContent, GameObject prefab, 
                                                   GameObject searchSection)
    {
        AdventurerMenu adventurerMenu = parent.GetComponent<AdventurerMenu>();
        if (adventurerMenu == null)
        {
            adventurerMenu = parent.AddComponent<AdventurerMenu>();
        }

        Button searchButton = searchSection.GetComponentInChildren<Button>();

        // Use reflection to set private fields
        var recruitedField = typeof(AdventurerMenu).GetField("recruitedListContainer", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var availableField = typeof(AdventurerMenu).GetField("availableListContainer", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var prefabField = typeof(AdventurerMenu).GetField("adventurerItemPrefab", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var searchField = typeof(AdventurerMenu).GetField("searchButton", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        recruitedField?.SetValue(adventurerMenu, recruitedContent);
        availableField?.SetValue(adventurerMenu, availableContent);
        prefabField?.SetValue(adventurerMenu, prefab);
        searchField?.SetValue(adventurerMenu, searchButton);

        EditorUtility.SetDirty(adventurerMenu);
    }
#endif
}