using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class ResourceUI : MonoBehaviour
{
    [Header("Resource Display")]
    [SerializeField] private GameObject resourceDisplayPrefab;
    [SerializeField] private Transform resourceContainer;
    [SerializeField] private bool updateInRealTime = true;
    [SerializeField] private float updateInterval = 0.5f;
    
    [Header("Resource Icons")]
    [SerializeField] private Sprite goldIcon;
    [SerializeField] private Sprite populationIcon;
    [SerializeField] private Sprite stoneIcon;
    [SerializeField] private Sprite woodIcon;
    [SerializeField] private Sprite ironIcon;
    [SerializeField] private Sprite magicCrystalIcon;
    [SerializeField] private Sprite reputationIcon;
    
    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color lowResourceColor = Color.yellow;
    [SerializeField] private Color criticalResourceColor = Color.red;
    [SerializeField] private Color fullResourceColor = Color.green;
    
    private Dictionary<Resource.ResourceType, ResourceDisplayItem> resourceDisplays;
    private float lastUpdateTime;
    private bool isInitialized = false;
    
    private void Start()
    {
        InitializeResourceUI();
    }

    private void OnEnable()
    {
        SubscribeToEvents();
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    private void Update()
    {
        if (updateInRealTime && isInitialized && Time.time - lastUpdateTime >= updateInterval)
        {
            UpdateAllResourceDisplays();
            lastUpdateTime = Time.time;
        }
    }

    private void InitializeResourceUI()
    {
        resourceDisplays = new Dictionary<Resource.ResourceType, ResourceDisplayItem>();
        
        if (resourceContainer == null)
        {
            Debug.LogError("ResourceUI: Resource container not assigned!");
            return;
        }
        
        CreateResourceDisplays();
        UpdateAllResourceDisplays();
        isInitialized = true;
    }

    private void CreateResourceDisplays()
    {
        // Create displays for each resource type
        Resource.ResourceType[] resourceTypes = (Resource.ResourceType[])Enum.GetValues(typeof(Resource.ResourceType));
        
        foreach (Resource.ResourceType resourceType in resourceTypes)
        {
            GameObject displayObject = CreateResourceDisplay(resourceType);
            if (displayObject != null)
            {
                ResourceDisplayItem displayItem = displayObject.GetComponent<ResourceDisplayItem>();
                if (displayItem != null)
                {
                    resourceDisplays[resourceType] = displayItem;
                }
            }
        }
    }

    private GameObject CreateResourceDisplay(Resource.ResourceType resourceType)
    {
        GameObject displayObject;
        
        if (resourceDisplayPrefab != null)
        {
            displayObject = Instantiate(resourceDisplayPrefab, resourceContainer);
        }
        else
        {
            displayObject = CreateDefaultResourceDisplay(resourceType);
        }
        
        // Configure the display
        ResourceDisplayItem displayItem = displayObject.GetComponent<ResourceDisplayItem>();
        if (displayItem == null)
        {
            displayItem = displayObject.AddComponent<ResourceDisplayItem>();
        }
        
        displayItem.Initialize(resourceType, GetResourceIcon(resourceType));
        
        return displayObject;
    }

    private GameObject CreateDefaultResourceDisplay(Resource.ResourceType resourceType)
    {
        GameObject displayObject = new GameObject($"Resource_{resourceType}");
        displayObject.transform.SetParent(resourceContainer, false);
        
        // Add RectTransform
        RectTransform rectTransform = displayObject.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(200, 40);
        
        // Add horizontal layout
        HorizontalLayoutGroup layout = displayObject.AddComponent<HorizontalLayoutGroup>();
        layout.childAlignment = TextAnchor.MiddleLeft;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        layout.spacing = 10;
        
        // Create icon
        GameObject iconObject = new GameObject("Icon");
        iconObject.transform.SetParent(displayObject.transform, false);
        RectTransform iconRect = iconObject.AddComponent<RectTransform>();
        iconRect.sizeDelta = new Vector2(32, 32);
        Image iconImage = iconObject.AddComponent<Image>();
        iconImage.sprite = GetResourceIcon(resourceType);
        
        // Create text
        GameObject textObject = new GameObject("Text");
        textObject.transform.SetParent(displayObject.transform, false);
        RectTransform textRect = textObject.AddComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(150, 32);
        Text textComponent = textObject.AddComponent<Text>();
        textComponent.text = $"{resourceType}: 0/0";
        textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textComponent.fontSize = 14;
        textComponent.color = normalColor;
        
        return displayObject;
    }

    private Sprite GetResourceIcon(Resource.ResourceType resourceType)
    {
        switch (resourceType)
        {
            case Resource.ResourceType.Gold:
                return goldIcon;
            case Resource.ResourceType.Population:
                return populationIcon;
            case Resource.ResourceType.Stone:
                return stoneIcon;
            case Resource.ResourceType.Wood:
                return woodIcon;
            case Resource.ResourceType.Iron:
                return ironIcon;
            case Resource.ResourceType.MagicCrystal:
                return magicCrystalIcon;
            case Resource.ResourceType.Reputation:
                return reputationIcon;
            default:
                return null;
        }
    }

    private void UpdateAllResourceDisplays()
    {
        if (ResourceManager.Instance == null) return;
        
        foreach (var kvp in resourceDisplays)
        {
            UpdateResourceDisplay(kvp.Key, kvp.Value);
        }
    }

    private void UpdateResourceDisplay(Resource.ResourceType resourceType, ResourceDisplayItem displayItem)
    {
        if (ResourceManager.Instance == null || displayItem == null) return;
        
        Resource resource = ResourceManager.Instance.GetResource(resourceType);
        if (resource == null) return;
        
        // Update text
        string displayText = $"{resourceType}: {resource.Amount}/{resource.Capacity}";
        displayItem.UpdateText(displayText);
        
        // Update color based on resource level
        Color textColor = GetResourceColor(resource);
        displayItem.UpdateColor(textColor);
        
        // Update fill bar if available
        displayItem.UpdateFillBar(resource.FillPercentage);
    }

    private Color GetResourceColor(Resource resource)
    {
        if (resource.IsFull)
        {
            return fullResourceColor;
        }
        else if (resource.FillPercentage < 0.1f)
        {
            return criticalResourceColor;
        }
        else if (resource.FillPercentage < 0.3f)
        {
            return lowResourceColor;
        }
        else
        {
            return normalColor;
        }
    }

    private void SubscribeToEvents()
    {
        if (ResourceManager.Instance != null)
        {
            ResourceManager.OnResourceChanged += OnResourceChanged;
            ResourceManager.OnResourceEmpty += OnResourceEmpty;
            ResourceManager.OnResourceFull += OnResourceFull;
            ResourceManager.OnMultipleResourcesChanged += OnMultipleResourcesChanged;
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (ResourceManager.Instance != null)
        {
            ResourceManager.OnResourceChanged -= OnResourceChanged;
            ResourceManager.OnResourceEmpty -= OnResourceEmpty;
            ResourceManager.OnResourceFull -= OnResourceFull;
            ResourceManager.OnMultipleResourcesChanged -= OnMultipleResourcesChanged;
        }
    }

    private void OnResourceChanged(Resource.ResourceType resourceType, int previousAmount, int newAmount)
    {
        if (resourceDisplays.ContainsKey(resourceType))
        {
            UpdateResourceDisplay(resourceType, resourceDisplays[resourceType]);
        }
    }

    private void OnResourceEmpty(Resource.ResourceType resourceType)
    {
        if (resourceDisplays.ContainsKey(resourceType))
        {
            ResourceDisplayItem displayItem = resourceDisplays[resourceType];
            displayItem.PlayEmptyAnimation();
        }
    }

    private void OnResourceFull(Resource.ResourceType resourceType)
    {
        if (resourceDisplays.ContainsKey(resourceType))
        {
            ResourceDisplayItem displayItem = resourceDisplays[resourceType];
            displayItem.PlayFullAnimation();
        }
    }

    private void OnMultipleResourcesChanged(Dictionary<Resource.ResourceType, int> changedResources)
    {
        foreach (var kvp in changedResources)
        {
            if (resourceDisplays.ContainsKey(kvp.Key))
            {
                UpdateResourceDisplay(kvp.Key, resourceDisplays[kvp.Key]);
            }
        }
    }

    public void SetUpdateInterval(float interval)
    {
        updateInterval = Mathf.Max(0.1f, interval);
    }

    public void SetRealTimeUpdates(bool enabled)
    {
        updateInRealTime = enabled;
    }

    public void RefreshDisplay()
    {
        UpdateAllResourceDisplays();
    }
}

// Helper class for individual resource display items
public class ResourceDisplayItem : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Text textComponent;
    [SerializeField] private Image fillBar;
    [SerializeField] private Animator animator;
    
    private Resource.ResourceType resourceType;
    private bool isInitialized = false;
    
    public void Initialize(Resource.ResourceType type, Sprite icon)
    {
        resourceType = type;
        
        // Find or create components
        if (iconImage == null)
            iconImage = GetComponentInChildren<Image>();
        
        if (textComponent == null)
            textComponent = GetComponentInChildren<Text>();
        
        if (fillBar == null)
        {
            // Try to find a fill bar in children
            Transform fillTransform = transform.Find("FillBar");
            if (fillTransform != null)
            {
                fillBar = fillTransform.GetComponent<Image>();
            }
        }
        
        if (animator == null)
            animator = GetComponent<Animator>();
        
        // Set icon
        if (iconImage != null && icon != null)
        {
            iconImage.sprite = icon;
        }
        
        isInitialized = true;
    }
    
    public void UpdateText(string text)
    {
        if (textComponent != null)
        {
            textComponent.text = text;
        }
    }
    
    public void UpdateColor(Color color)
    {
        if (textComponent != null)
        {
            textComponent.color = color;
        }
    }
    
    public void UpdateFillBar(float fillPercentage)
    {
        if (fillBar != null)
        {
            fillBar.fillAmount = fillPercentage;
        }
    }
    
    public void PlayEmptyAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Empty");
        }
    }
    
    public void PlayFullAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Full");
        }
    }
}