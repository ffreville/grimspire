using UnityEngine;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    [Header("Tab Navigation")]
    [SerializeField] private Button[] tabButtons;
    [SerializeField] private GameObject[] tabContents;
    [SerializeField] private int activeTabIndex = 0;
    
    [Header("Tab Colors")]
    [SerializeField] private Color activeTabColor = new(0.4f, 0.4f, 0.4f, 1f);
    [SerializeField] private Color inactiveTabColor = new(0.2f, 0.2f, 0.2f, 1f);
    
    [Header("Game Data")]
    [SerializeField] private City currentCity;

    private void Awake()
    {
        SetupTabButtons();
        InitializeCity();
    }

    private void Start()
    {
        SetActiveTab(0);
    }

    private void SetupTabButtons()
    {
        for (int i = 0; i < tabButtons.Length; i++)
        {
            int tabIndex = i; // Capture pour la closure
            tabButtons[i]?.onClick.AddListener(() => SetActiveTab(tabIndex));
        }
    }

    private void InitializeCity()
    {
        if (currentCity == null)
        {
            currentCity = new City();
        }
        
        // Distribute city reference to all relevant menus
        DistributeCityReference();
    }

    private void DistributeCityReference()
    {
        // Find and setup BuildingMenu
        BuildingMenu buildingMenu = GetComponentInChildren<BuildingMenu>();
        if (buildingMenu != null)
        {
            buildingMenu.SetCity(currentCity);
        }
        
        // Add other menus here when they're created
        // PopulationMenu, CommerceMenu, etc.
    }

    public void SetActiveTab(int tabIndex)
    {
        if (tabIndex < 0 || tabIndex >= tabButtons.Length) return;

        // Update tab button colors
        for (int i = 0; i < tabButtons.Length; i++)
        {
            if (tabButtons[i] != null)
            {
                Image tabImage = tabButtons[i].GetComponent<Image>();
                if (tabImage != null)
                {
                    tabImage.color = i == tabIndex ? activeTabColor : inactiveTabColor;
                }
            }
        }

        // Show/hide tab contents
        for (int i = 0; i < tabContents.Length; i++)
        {
            if (tabContents[i] != null)
            {
                tabContents[i].SetActive(i == tabIndex);
            }
        }

        activeTabIndex = tabIndex;
        
        // Notify the active tab that it's been selected
        OnTabSelected(tabIndex);
    }

    private void OnTabSelected(int tabIndex)
    {
        // Refresh the active tab content
        switch (tabIndex)
        {
            case 0: // Bâtiments
                BuildingMenu buildingMenu = GetComponentInChildren<BuildingMenu>();
                if (buildingMenu != null && buildingMenu.gameObject.activeInHierarchy)
                {
                    Debug.Log("On passe par là");
                    buildingMenu.RefreshBuildingList();
                }
                break;
                
            case 1: // Population
                // TODO: Refresh population menu
                break;
                
            case 2: // Commerce
                // TODO: Refresh commerce menu
                break;
                
            // Add other tabs as needed
        }
    }

    public City GetCurrentCity()
    {
        return currentCity;
    }

    public void SetCity(City city)
    {
        currentCity = city;
        DistributeCityReference();
    }

    public int GetActiveTabIndex()
    {
        return activeTabIndex;
    }

    // Called when day/night cycle changes
    public void OnDayNightChanged()
    {
        if (currentCity != null)
        {
            // Process any day/night specific updates
            DistributeCityReference();
            
            // Refresh the currently active tab
            OnTabSelected(activeTabIndex);
        }
    }

    // Called when an action is performed (building, recruiting, etc.)
    public void OnActionPerformed()
    {
        // Refresh the currently active tab to reflect changes
        OnTabSelected(activeTabIndex);
    }
}
