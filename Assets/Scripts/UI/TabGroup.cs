using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TabGroup : MonoBehaviour
{
    [Header("Tab Settings")]
    [SerializeField] private List<TabButton> tabButtons;
    [SerializeField] private Color selectedColor = Color.white;
    [SerializeField] private Color unselectedColor = Color.gray;
    [SerializeField] private Color hoveredColor = new Color(0.8f, 0.8f, 0.8f); // Light gray
    
    private TabButton selectedTab;
    
    public System.Action<int> OnTabSelected;
    
    private void Start()
    {
        // Find all tab buttons if not assigned
        if (tabButtons == null || tabButtons.Count == 0)
        {
            tabButtons = new List<TabButton>();
            TabButton[] buttons = GetComponentsInChildren<TabButton>();
            tabButtons.AddRange(buttons);
        }
        
        // Initialize tabs
        foreach (var tab in tabButtons)
        {
            tab.SetTabGroup(this);
        }
        
        // Select first tab by default
        if (tabButtons.Count > 0)
        {
            SelectTab(0);
        }
    }
    
    public void SelectTab(int index)
    {
        if (index < 0 || index >= tabButtons.Count) return;
        
        SelectTab(tabButtons[index]);
    }
    
    public void SelectTab(TabButton tab)
    {
        if (selectedTab != null)
        {
            selectedTab.SetSelected(false);
        }
        
        selectedTab = tab;
        tab.SetSelected(true);
        
        // Update all tab colors
        UpdateTabColors();
        
        // Notify listeners
        int tabIndex = tabButtons.IndexOf(tab);
        OnTabSelected?.Invoke(tabIndex);
    }
    
    public void OnTabHovered(TabButton tab)
    {
        if (selectedTab != tab)
        {
            tab.SetColor(hoveredColor);
        }
    }
    
    public void OnTabExited(TabButton tab)
    {
        if (selectedTab != tab)
        {
            tab.SetColor(unselectedColor);
        }
    }
    
    private void UpdateTabColors()
    {
        foreach (var tab in tabButtons)
        {
            if (tab == selectedTab)
            {
                tab.SetColor(selectedColor);
            }
            else
            {
                tab.SetColor(unselectedColor);
            }
        }
    }
    
    public void AddTab(TabButton tab)
    {
        tabButtons.Add(tab);
        tab.SetTabGroup(this);
        UpdateTabColors();
    }
    
    public void RemoveTab(TabButton tab)
    {
        tabButtons.Remove(tab);
        if (selectedTab == tab)
        {
            selectedTab = null;
            if (tabButtons.Count > 0)
            {
                SelectTab(0);
            }
        }
        UpdateTabColors();
    }
    
    public TabButton GetSelectedTab()
    {
        return selectedTab;
    }
    
    public int GetSelectedTabIndex()
    {
        return selectedTab != null ? tabButtons.IndexOf(selectedTab) : -1;
    }
}

public class TabButton : MonoBehaviour, UnityEngine.EventSystems.IPointerClickHandler, UnityEngine.EventSystems.IPointerEnterHandler, UnityEngine.EventSystems.IPointerExitHandler
{
    [Header("Tab Components")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI tabText;
    [SerializeField] private Image tabIcon;
    [SerializeField] private GameObject selectedIndicator;
    [SerializeField] private GameObject notificationBadge;
    [SerializeField] private TextMeshProUGUI notificationText;
    
    [Header("Tab Settings")]
    [SerializeField] private string tabName;
    [SerializeField] private Sprite tabSprite;
    [SerializeField] private bool interactable = true;
    
    private TabGroup tabGroup;
    private bool isSelected = false;
    
    public void SetTabGroup(TabGroup group)
    {
        tabGroup = group;
    }
    
    public void OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
    {
        if (interactable && tabGroup != null)
        {
            tabGroup.SelectTab(this);
        }
    }
    
    public void OnPointerEnter(UnityEngine.EventSystems.PointerEventData eventData)
    {
        if (interactable && tabGroup != null)
        {
            tabGroup.OnTabHovered(this);
        }
    }
    
    public void OnPointerExit(UnityEngine.EventSystems.PointerEventData eventData)
    {
        if (interactable && tabGroup != null)
        {
            tabGroup.OnTabExited(this);
        }
    }
    
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        
        if (selectedIndicator != null)
        {
            selectedIndicator.SetActive(selected);
        }
    }
    
    public void SetColor(Color color)
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = color;
        }
    }
    
    public void SetTabName(string name)
    {
        tabName = name;
        if (tabText != null)
        {
            tabText.text = name;
        }
    }
    
    public void SetTabIcon(Sprite sprite)
    {
        tabSprite = sprite;
        if (tabIcon != null)
        {
            tabIcon.sprite = sprite;
        }
    }
    
    public void SetInteractable(bool interactable)
    {
        this.interactable = interactable;
        
        // Visual feedback for non-interactable tabs
        if (backgroundImage != null)
        {
            backgroundImage.color = interactable ? Color.white : Color.gray;
        }
    }
    
    public void SetNotification(int count)
    {
        if (notificationBadge != null)
        {
            notificationBadge.SetActive(count > 0);
            if (notificationText != null)
            {
                notificationText.text = count > 99 ? "99+" : count.ToString();
            }
        }
    }
    
    public void ClearNotification()
    {
        SetNotification(0);
    }
    
    public string GetTabName()
    {
        return tabName;
    }
    
    public bool IsSelected()
    {
        return isSelected;
    }
    
    public bool IsInteractable()
    {
        return interactable;
    }
    
    private void Start()
    {
        // Initialize tab display
        if (tabText != null && !string.IsNullOrEmpty(tabName))
        {
            tabText.text = tabName;
        }
        
        if (tabIcon != null && tabSprite != null)
        {
            tabIcon.sprite = tabSprite;
        }
        
        // Hide notification badge initially
        if (notificationBadge != null)
        {
            notificationBadge.SetActive(false);
        }
    }
}