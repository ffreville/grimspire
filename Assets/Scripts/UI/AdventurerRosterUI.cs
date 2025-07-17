using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AdventurerRosterUI : MonoBehaviour
{
    [Header("Roster Display")]
    [SerializeField] private Transform rosterContainer;
    [SerializeField] private GameObject adventurerCardPrefab;
    [SerializeField] private ScrollRect rosterScrollRect;
    [SerializeField] private TextMeshProUGUI rosterCountText;
    [SerializeField] private TextMeshProUGUI capacityText;
    
    [Header("Sorting Controls")]
    [SerializeField] private TMP_Dropdown sortDropdown;
    [SerializeField] private Button sortAscendingButton;
    [SerializeField] private Button sortDescendingButton;
    [SerializeField] private TextMeshProUGUI sortOrderText;
    
    [Header("Filtering Controls")]
    [SerializeField] private TMP_Dropdown classFilter;
    [SerializeField] private TMP_Dropdown statusFilter;
    [SerializeField] private TMP_Dropdown rarityFilter;
    [SerializeField] private Slider minLevelSlider;
    [SerializeField] private Slider maxLevelSlider;
    [SerializeField] private TMP_InputField searchInput;
    [SerializeField] private Button resetFiltersButton;
    
    [Header("Filter Display")]
    [SerializeField] private TextMeshProUGUI minLevelText;
    [SerializeField] private TextMeshProUGUI maxLevelText;
    [SerializeField] private TextMeshProUGUI activeFiltersText;
    
    [Header("Multi-Selection")]
    [SerializeField] private Button selectAllButton;
    [SerializeField] private Button deselectAllButton;
    [SerializeField] private Button createPartyButton;
    [SerializeField] private Button dismissSelectedButton;
    [SerializeField] private TextMeshProUGUI selectedCountText;
    
    [Header("Quick Actions")]
    [SerializeField] private Button healAllButton;
    [SerializeField] private Button restAllButton;
    [SerializeField] private Button equipBestButton;
    [SerializeField] private Button autoAssignButton;
    
    private List<Adventurer> allAdventurers;
    private List<Adventurer> filteredAdventurers;
    private List<AdventurerCardUI> adventurerCards;
    private List<Adventurer> selectedAdventurers;
    private AdventurerFilterData currentFilter;
    private AdventurerSortData currentSort;
    private bool isAscending = true;
    
    public System.Action<Adventurer> OnAdventurerSelected;
    public System.Action<List<Adventurer>> OnMultipleAdventurersSelected;
    public System.Action<Adventurer> OnAdventurerDoubleClicked;
    
    private void Start()
    {
        InitializeRosterUI();
        ResetFilters();
        LoadAdventurers();
    }
    
    private void InitializeRosterUI()
    {
        allAdventurers = new List<Adventurer>();
        filteredAdventurers = new List<Adventurer>();
        adventurerCards = new List<AdventurerCardUI>();
        selectedAdventurers = new List<Adventurer>();
        currentFilter = new AdventurerFilterData();
        currentSort = new AdventurerSortData();
        
        // Setup sorting dropdown
        SetupSortingDropdown();
        
        // Setup filtering dropdowns
        SetupFilteringDropdowns();
        
        // Setup level sliders
        SetupLevelSliders();
        
        // Setup button events
        SetupButtonEvents();
        
        // Initial display update
        UpdateRosterDisplay();
    }
    
    private void SetupSortingDropdown()
    {
        sortDropdown.options.Clear();
        sortDropdown.options.Add(new TMP_Dropdown.OptionData("Nom"));
        sortDropdown.options.Add(new TMP_Dropdown.OptionData("Niveau"));
        sortDropdown.options.Add(new TMP_Dropdown.OptionData("Classe"));
        sortDropdown.options.Add(new TMP_Dropdown.OptionData("Rareté"));
        sortDropdown.options.Add(new TMP_Dropdown.OptionData("Statut"));
        sortDropdown.options.Add(new TMP_Dropdown.OptionData("Expérience"));
        sortDropdown.options.Add(new TMP_Dropdown.OptionData("Missions Réussies"));
        sortDropdown.options.Add(new TMP_Dropdown.OptionData("Loyauté"));
        sortDropdown.options.Add(new TMP_Dropdown.OptionData("Salaire"));
        sortDropdown.value = 0;
        
        sortDropdown.onValueChanged.AddListener(OnSortChanged);
    }
    
    private void SetupFilteringDropdowns()
    {
        // Class filter
        classFilter.options.Clear();
        classFilter.options.Add(new TMP_Dropdown.OptionData("Toutes Classes"));
        foreach (Adventurer.AdventurerClass adventurerClass in System.Enum.GetValues(typeof(Adventurer.AdventurerClass)))
        {
            classFilter.options.Add(new TMP_Dropdown.OptionData(GetClassDisplayName(adventurerClass)));
        }
        classFilter.onValueChanged.AddListener(OnClassFilterChanged);
        
        // Status filter
        statusFilter.options.Clear();
        statusFilter.options.Add(new TMP_Dropdown.OptionData("Tous Statuts"));
        foreach (Adventurer.AdventurerStatus status in System.Enum.GetValues(typeof(Adventurer.AdventurerStatus)))
        {
            statusFilter.options.Add(new TMP_Dropdown.OptionData(GetStatusDisplayName(status)));
        }
        statusFilter.onValueChanged.AddListener(OnStatusFilterChanged);
        
        // Rarity filter
        rarityFilter.options.Clear();
        rarityFilter.options.Add(new TMP_Dropdown.OptionData("Toutes Raretés"));
        foreach (Adventurer.AdventurerRarity rarity in System.Enum.GetValues(typeof(Adventurer.AdventurerRarity)))
        {
            rarityFilter.options.Add(new TMP_Dropdown.OptionData(GetRarityDisplayName(rarity)));
        }
        rarityFilter.onValueChanged.AddListener(OnRarityFilterChanged);
    }
    
    private void SetupLevelSliders()
    {
        minLevelSlider.minValue = 1;
        minLevelSlider.maxValue = 50;
        minLevelSlider.value = 1;
        minLevelSlider.onValueChanged.AddListener(OnMinLevelChanged);
        
        maxLevelSlider.minValue = 1;
        maxLevelSlider.maxValue = 50;
        maxLevelSlider.value = 50;
        maxLevelSlider.onValueChanged.AddListener(OnMaxLevelChanged);
        
        UpdateLevelDisplay();
    }
    
    private void SetupButtonEvents()
    {
        sortAscendingButton.onClick.AddListener(() => SetSortOrder(true));
        sortDescendingButton.onClick.AddListener(() => SetSortOrder(false));
        
        searchInput.onValueChanged.AddListener(OnSearchTextChanged);
        resetFiltersButton.onClick.AddListener(ResetFilters);
        
        selectAllButton.onClick.AddListener(SelectAllVisible);
        deselectAllButton.onClick.AddListener(DeselectAll);
        createPartyButton.onClick.AddListener(CreatePartyFromSelected);
        dismissSelectedButton.onClick.AddListener(DismissSelected);
        
        healAllButton.onClick.AddListener(HealAllAdventurers);
        restAllButton.onClick.AddListener(RestAllAdventurers);
        equipBestButton.onClick.AddListener(EquipBestGear);
        autoAssignButton.onClick.AddListener(AutoAssignParties);
    }
    
    private void LoadAdventurers()
    {
        // Load adventurers from game manager or city
        // This would typically get adventurers from the game state
        allAdventurers.Clear();
        
        // For now, we'll assume they're loaded from elsewhere
        // allAdventurers = GameManager.Instance.GetAdventurers();
        
        ApplyFiltersAndSort();
    }
    
    private void UpdateRosterDisplay()
    {
        // Clear existing cards
        foreach (var card in adventurerCards)
        {
            if (card != null) Destroy(card.gameObject);
        }
        adventurerCards.Clear();
        
        // Create new cards for filtered adventurers
        foreach (var adventurer in filteredAdventurers)
        {
            CreateAdventurerCard(adventurer);
        }
        
        // Update count displays
        UpdateCountDisplays();
        UpdateSelectedCountDisplay();
    }
    
    private void CreateAdventurerCard(Adventurer adventurer)
    {
        var cardObject = Instantiate(adventurerCardPrefab, rosterContainer);
        var cardUI = cardObject.GetComponent<AdventurerCardUI>();
        
        if (cardUI != null)
        {
            cardUI.SetAdventurer(adventurer);
            cardUI.OnCardClicked += OnAdventurerCardClicked;
            cardUI.OnCardDoubleClicked += OnAdventurerCardDoubleClicked;
            cardUI.OnCardRightClicked += OnAdventurerCardRightClicked;
            cardUI.SetSelected(selectedAdventurers.Contains(adventurer));
            adventurerCards.Add(cardUI);
        }
    }
    
    private void OnAdventurerCardClicked(Adventurer adventurer)
    {
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            // Multi-selection with Ctrl
            ToggleAdventurerSelection(adventurer);
        }
        else
        {
            // Single selection
            SelectSingleAdventurer(adventurer);
        }
        
        OnAdventurerSelected?.Invoke(adventurer);
    }
    
    private void OnAdventurerCardDoubleClicked(Adventurer adventurer)
    {
        OnAdventurerDoubleClicked?.Invoke(adventurer);
    }
    
    private void OnAdventurerCardRightClicked(Adventurer adventurer)
    {
        // Show context menu
        // This would open a context menu with quick actions
    }
    
    private void ToggleAdventurerSelection(Adventurer adventurer)
    {
        if (selectedAdventurers.Contains(adventurer))
        {
            selectedAdventurers.Remove(adventurer);
        }
        else
        {
            selectedAdventurers.Add(adventurer);
        }
        
        UpdateCardSelectionDisplay();
        UpdateSelectedCountDisplay();
        OnMultipleAdventurersSelected?.Invoke(selectedAdventurers);
    }
    
    private void SelectSingleAdventurer(Adventurer adventurer)
    {
        selectedAdventurers.Clear();
        selectedAdventurers.Add(adventurer);
        
        UpdateCardSelectionDisplay();
        UpdateSelectedCountDisplay();
        OnMultipleAdventurersSelected?.Invoke(selectedAdventurers);
    }
    
    private void UpdateCardSelectionDisplay()
    {
        foreach (var card in adventurerCards)
        {
            card.SetSelected(selectedAdventurers.Contains(card.GetAdventurer()));
        }
    }
    
    private void ApplyFiltersAndSort()
    {
        // Apply filters
        filteredAdventurers = allAdventurers.Where(FilterAdventurer).ToList();
        
        // Apply sorting
        filteredAdventurers = SortAdventurers(filteredAdventurers).ToList();
        
        UpdateRosterDisplay();
        UpdateActiveFiltersDisplay();
    }
    
    private bool FilterAdventurer(Adventurer adventurer)
    {
        // Class filter
        if (currentFilter.classFilter != Adventurer.AdventurerClass.Warrior && 
            adventurer.Class != currentFilter.classFilter)
        {
            return false;
        }
        
        // Status filter
        if (currentFilter.statusFilter != Adventurer.AdventurerStatus.Available && 
            adventurer.Status != currentFilter.statusFilter)
        {
            return false;
        }
        
        // Rarity filter
        if (currentFilter.rarityFilter != Adventurer.AdventurerRarity.Common && 
            adventurer.Rarity != currentFilter.rarityFilter)
        {
            return false;
        }
        
        // Level filter
        if (adventurer.Level < currentFilter.minLevel || 
            adventurer.Level > currentFilter.maxLevel)
        {
            return false;
        }
        
        // Search filter
        if (!string.IsNullOrEmpty(currentFilter.searchText))
        {
            string searchLower = currentFilter.searchText.ToLower();
            if (!adventurer.Name.ToLower().Contains(searchLower) &&
                !adventurer.Class.ToString().ToLower().Contains(searchLower))
            {
                return false;
            }
        }
        
        return true;
    }
    
    private IEnumerable<Adventurer> SortAdventurers(IEnumerable<Adventurer> adventurers)
    {
        switch (currentSort.sortType)
        {
            case AdventurerSortType.Name:
                return isAscending ? adventurers.OrderBy(a => a.Name) : adventurers.OrderByDescending(a => a.Name);
            case AdventurerSortType.Level:
                return isAscending ? adventurers.OrderBy(a => a.Level) : adventurers.OrderByDescending(a => a.Level);
            case AdventurerSortType.Class:
                return isAscending ? adventurers.OrderBy(a => a.Class) : adventurers.OrderByDescending(a => a.Class);
            case AdventurerSortType.Rarity:
                return isAscending ? adventurers.OrderBy(a => a.Rarity) : adventurers.OrderByDescending(a => a.Rarity);
            case AdventurerSortType.Status:
                return isAscending ? adventurers.OrderBy(a => a.Status) : adventurers.OrderByDescending(a => a.Status);
            case AdventurerSortType.Experience:
                return isAscending ? adventurers.OrderBy(a => a.Experience) : adventurers.OrderByDescending(a => a.Experience);
            case AdventurerSortType.SuccessRate:
                return isAscending ? adventurers.OrderBy(a => a.SuccessRate) : adventurers.OrderByDescending(a => a.SuccessRate);
            case AdventurerSortType.Loyalty:
                return isAscending ? adventurers.OrderBy(a => a.LoyaltyLevel) : adventurers.OrderByDescending(a => a.LoyaltyLevel);
            case AdventurerSortType.Salary:
                return isAscending ? adventurers.OrderBy(a => a.DailySalary) : adventurers.OrderByDescending(a => a.DailySalary);
            default:
                return adventurers;
        }
    }
    
    // Event handlers
    private void OnSortChanged(int value)
    {
        currentSort.sortType = (AdventurerSortType)value;
        ApplyFiltersAndSort();
        UpdateSortDisplay();
    }
    
    private void SetSortOrder(bool ascending)
    {
        isAscending = ascending;
        ApplyFiltersAndSort();
        UpdateSortDisplay();
    }
    
    private void OnClassFilterChanged(int value)
    {
        currentFilter.classFilter = value == 0 ? Adventurer.AdventurerClass.Warrior : (Adventurer.AdventurerClass)(value - 1);
        ApplyFiltersAndSort();
    }
    
    private void OnStatusFilterChanged(int value)
    {
        currentFilter.statusFilter = value == 0 ? Adventurer.AdventurerStatus.Available : (Adventurer.AdventurerStatus)(value - 1);
        ApplyFiltersAndSort();
    }
    
    private void OnRarityFilterChanged(int value)
    {
        currentFilter.rarityFilter = value == 0 ? Adventurer.AdventurerRarity.Common : (Adventurer.AdventurerRarity)(value - 1);
        ApplyFiltersAndSort();
    }
    
    private void OnMinLevelChanged(float value)
    {
        currentFilter.minLevel = (int)value;
        if (currentFilter.minLevel > currentFilter.maxLevel)
        {
            maxLevelSlider.value = currentFilter.minLevel;
            currentFilter.maxLevel = currentFilter.minLevel;
        }
        UpdateLevelDisplay();
        ApplyFiltersAndSort();
    }
    
    private void OnMaxLevelChanged(float value)
    {
        currentFilter.maxLevel = (int)value;
        if (currentFilter.maxLevel < currentFilter.minLevel)
        {
            minLevelSlider.value = currentFilter.maxLevel;
            currentFilter.minLevel = currentFilter.maxLevel;
        }
        UpdateLevelDisplay();
        ApplyFiltersAndSort();
    }
    
    private void OnSearchTextChanged(string text)
    {
        currentFilter.searchText = text;
        ApplyFiltersAndSort();
    }
    
    private void ResetFilters()
    {
        currentFilter = new AdventurerFilterData();
        classFilter.value = 0;
        statusFilter.value = 0;
        rarityFilter.value = 0;
        minLevelSlider.value = 1;
        maxLevelSlider.value = 50;
        searchInput.text = "";
        UpdateLevelDisplay();
        ApplyFiltersAndSort();
    }
    
    // Multi-selection actions
    private void SelectAllVisible()
    {
        selectedAdventurers.Clear();
        selectedAdventurers.AddRange(filteredAdventurers);
        UpdateCardSelectionDisplay();
        UpdateSelectedCountDisplay();
        OnMultipleAdventurersSelected?.Invoke(selectedAdventurers);
    }
    
    private void DeselectAll()
    {
        selectedAdventurers.Clear();
        UpdateCardSelectionDisplay();
        UpdateSelectedCountDisplay();
        OnMultipleAdventurersSelected?.Invoke(selectedAdventurers);
    }
    
    private void CreatePartyFromSelected()
    {
        if (selectedAdventurers.Count > 0)
        {
            // Create party from selected adventurers
            // This would interface with the party management system
            Debug.Log($"Creating party from {selectedAdventurers.Count} adventurers");
        }
    }
    
    private void DismissSelected()
    {
        if (selectedAdventurers.Count > 0)
        {
            // Dismiss selected adventurers
            // This would remove them from the roster
            Debug.Log($"Dismissing {selectedAdventurers.Count} adventurers");
        }
    }
    
    // Quick actions
    private void HealAllAdventurers()
    {
        foreach (var adventurer in allAdventurers)
        {
            // Heal adventurer
            Debug.Log($"Healing {adventurer.Name}");
        }
    }
    
    private void RestAllAdventurers()
    {
        foreach (var adventurer in allAdventurers)
        {
            // Rest adventurer
            Debug.Log($"Resting {adventurer.Name}");
        }
    }
    
    private void EquipBestGear()
    {
        foreach (var adventurer in selectedAdventurers)
        {
            // Auto-equip best available gear
            Debug.Log($"Equipping best gear for {adventurer.Name}");
        }
    }
    
    private void AutoAssignParties()
    {
        // Auto-assign adventurers to parties
        Debug.Log("Auto-assigning parties");
    }
    
    // Display updates
    private void UpdateCountDisplays()
    {
        rosterCountText.text = $"Aventuriers: {filteredAdventurers.Count}/{allAdventurers.Count}";
        capacityText.text = $"Capacité: {allAdventurers.Count}/100"; // Assuming max capacity of 100
    }
    
    private void UpdateSelectedCountDisplay()
    {
        selectedCountText.text = $"Sélectionnés: {selectedAdventurers.Count}";
        
        // Enable/disable multi-selection buttons
        createPartyButton.interactable = selectedAdventurers.Count > 0;
        dismissSelectedButton.interactable = selectedAdventurers.Count > 0;
        equipBestButton.interactable = selectedAdventurers.Count > 0;
    }
    
    private void UpdateSortDisplay()
    {
        sortOrderText.text = isAscending ? "Croissant" : "Décroissant";
    }
    
    private void UpdateLevelDisplay()
    {
        minLevelText.text = $"Niv. Min: {currentFilter.minLevel}";
        maxLevelText.text = $"Niv. Max: {currentFilter.maxLevel}";
    }
    
    private void UpdateActiveFiltersDisplay()
    {
        string filtersText = "Filtres: ";
        bool hasFilters = false;
        
        if (classFilter.value != 0)
        {
            filtersText += GetClassDisplayName(currentFilter.classFilter) + " ";
            hasFilters = true;
        }
        
        if (statusFilter.value != 0)
        {
            filtersText += GetStatusDisplayName(currentFilter.statusFilter) + " ";
            hasFilters = true;
        }
        
        if (rarityFilter.value != 0)
        {
            filtersText += GetRarityDisplayName(currentFilter.rarityFilter) + " ";
            hasFilters = true;
        }
        
        if (currentFilter.minLevel > 1 || currentFilter.maxLevel < 50)
        {
            filtersText += $"Niv.{currentFilter.minLevel}-{currentFilter.maxLevel} ";
            hasFilters = true;
        }
        
        if (!string.IsNullOrEmpty(currentFilter.searchText))
        {
            filtersText += $"'{currentFilter.searchText}' ";
            hasFilters = true;
        }
        
        if (!hasFilters)
        {
            filtersText = "Aucun filtre";
        }
        
        activeFiltersText.text = filtersText;
    }
    
    // Helper methods
    private string GetClassDisplayName(Adventurer.AdventurerClass adventurerClass)
    {
        switch (adventurerClass)
        {
            case Adventurer.AdventurerClass.Warrior: return "Guerrier";
            case Adventurer.AdventurerClass.Mage: return "Mage";
            case Adventurer.AdventurerClass.Rogue: return "Voleur";
            case Adventurer.AdventurerClass.Cleric: return "Clerc";
            case Adventurer.AdventurerClass.Ranger: return "Rôdeur";
            default: return adventurerClass.ToString();
        }
    }
    
    private string GetStatusDisplayName(Adventurer.AdventurerStatus status)
    {
        switch (status)
        {
            case Adventurer.AdventurerStatus.Available: return "Disponible";
            case Adventurer.AdventurerStatus.OnMission: return "En Mission";
            case Adventurer.AdventurerStatus.Injured: return "Blessé";
            case Adventurer.AdventurerStatus.Dead: return "Mort";
            case Adventurer.AdventurerStatus.Resting: return "Repos";
            case Adventurer.AdventurerStatus.Training: return "Entraînement";
            default: return status.ToString();
        }
    }
    
    private string GetRarityDisplayName(Adventurer.AdventurerRarity rarity)
    {
        switch (rarity)
        {
            case Adventurer.AdventurerRarity.Common: return "Commun";
            case Adventurer.AdventurerRarity.Uncommon: return "Inhabituel";
            case Adventurer.AdventurerRarity.Rare: return "Rare";
            case Adventurer.AdventurerRarity.Epic: return "Épique";
            case Adventurer.AdventurerRarity.Legendary: return "Légendaire";
            default: return rarity.ToString();
        }
    }
    
    public void RefreshRoster()
    {
        LoadAdventurers();
    }
    
    public void AddAdventurer(Adventurer adventurer)
    {
        allAdventurers.Add(adventurer);
        ApplyFiltersAndSort();
    }
    
    public void RemoveAdventurer(Adventurer adventurer)
    {
        allAdventurers.Remove(adventurer);
        selectedAdventurers.Remove(adventurer);
        ApplyFiltersAndSort();
    }
    
    public List<Adventurer> GetSelectedAdventurers()
    {
        return new List<Adventurer>(selectedAdventurers);
    }
}

// Data structures for filtering and sorting
[System.Serializable]
public class AdventurerFilterData
{
    public Adventurer.AdventurerClass classFilter = Adventurer.AdventurerClass.Warrior;
    public Adventurer.AdventurerStatus statusFilter = Adventurer.AdventurerStatus.Available;
    public Adventurer.AdventurerRarity rarityFilter = Adventurer.AdventurerRarity.Common;
    public int minLevel = 1;
    public int maxLevel = 50;
    public string searchText = "";
}

[System.Serializable]
public class AdventurerSortData
{
    public AdventurerSortType sortType = AdventurerSortType.Name;
}

public enum AdventurerSortType
{
    Name,
    Level,
    Class,
    Rarity,
    Status,
    Experience,
    SuccessRate,
    Loyalty,
    Salary
}