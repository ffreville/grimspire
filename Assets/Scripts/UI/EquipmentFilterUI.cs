using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EquipmentFilterUI : MonoBehaviour
{
    [Header("Filter Controls")]
    [SerializeField] private TMP_Dropdown typeDropdown;
    [SerializeField] private TMP_Dropdown rarityDropdown;
    [SerializeField] private Slider minLevelSlider;
    [SerializeField] private Slider maxLevelSlider;
    [SerializeField] private TMP_InputField searchInput;
    [SerializeField] private Toggle onlyUsableToggle;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button applyButton;
    
    [Header("Filter Display")]
    [SerializeField] private TextMeshProUGUI minLevelText;
    [SerializeField] private TextMeshProUGUI maxLevelText;
    [SerializeField] private TextMeshProUGUI activeFiltersText;
    
    private EquipmentFilterData currentFilter;
    
    public event Action<EquipmentFilterData> OnFilterChanged;
    
    private void Start()
    {
        InitializeFilterUI();
        ResetFilters();
    }
    
    private void InitializeFilterUI()
    {
        // Setup type dropdown
        typeDropdown.options.Clear();
        typeDropdown.options.Add(new TMP_Dropdown.OptionData("Tous"));
        foreach (Equipment.EquipmentType type in Enum.GetValues(typeof(Equipment.EquipmentType)))
        {
            typeDropdown.options.Add(new TMP_Dropdown.OptionData(GetTypeDisplayName(type)));
        }
        typeDropdown.value = 0;
        
        // Setup rarity dropdown
        rarityDropdown.options.Clear();
        rarityDropdown.options.Add(new TMP_Dropdown.OptionData("Toutes"));
        foreach (Equipment.EquipmentRarity rarity in Enum.GetValues(typeof(Equipment.EquipmentRarity)))
        {
            rarityDropdown.options.Add(new TMP_Dropdown.OptionData(GetRarityDisplayName(rarity)));
        }
        rarityDropdown.value = 0;
        
        // Setup level sliders
        minLevelSlider.minValue = 1;
        minLevelSlider.maxValue = 50;
        minLevelSlider.value = 1;
        
        maxLevelSlider.minValue = 1;
        maxLevelSlider.maxValue = 50;
        maxLevelSlider.value = 50;
        
        // Setup events
        typeDropdown.onValueChanged.AddListener(OnTypeFilterChanged);
        rarityDropdown.onValueChanged.AddListener(OnRarityFilterChanged);
        minLevelSlider.onValueChanged.AddListener(OnMinLevelChanged);
        maxLevelSlider.onValueChanged.AddListener(OnMaxLevelChanged);
        searchInput.onValueChanged.AddListener(OnSearchTextChanged);
        onlyUsableToggle.onValueChanged.AddListener(OnUsableToggleChanged);
        resetButton.onClick.AddListener(ResetFilters);
        applyButton.onClick.AddListener(ApplyFilters);
        
        // Update display
        UpdateLevelDisplay();
    }
    
    private string GetTypeDisplayName(Equipment.EquipmentType type)
    {
        switch (type)
        {
            case Equipment.EquipmentType.Weapon: return "Armes";
            case Equipment.EquipmentType.Armor: return "Armures";
            case Equipment.EquipmentType.Helmet: return "Casques";
            case Equipment.EquipmentType.Boots: return "Bottes";
            case Equipment.EquipmentType.Accessory: return "Accessoires";
            case Equipment.EquipmentType.Shield: return "Boucliers";
            default: return type.ToString();
        }
    }
    
    private string GetRarityDisplayName(Equipment.EquipmentRarity rarity)
    {
        switch (rarity)
        {
            case Equipment.EquipmentRarity.Common: return "Commun";
            case Equipment.EquipmentRarity.Uncommon: return "Inhabituel";
            case Equipment.EquipmentRarity.Rare: return "Rare";
            case Equipment.EquipmentRarity.Epic: return "Épique";
            case Equipment.EquipmentRarity.Legendary: return "Légendaire";
            case Equipment.EquipmentRarity.Artifact: return "Artefact";
            default: return rarity.ToString();
        }
    }
    
    private void OnTypeFilterChanged(int value)
    {
        if (value == 0)
        {
            currentFilter.typeFilter = Equipment.EquipmentType.Weapon; // Default, will be ignored
        }
        else
        {
            currentFilter.typeFilter = (Equipment.EquipmentType)(value - 1);
        }
        
        UpdateActiveFiltersDisplay();
        OnFilterChanged?.Invoke(currentFilter);
    }
    
    private void OnRarityFilterChanged(int value)
    {
        if (value == 0)
        {
            currentFilter.rarityFilter = Equipment.EquipmentRarity.Common; // Default, will be ignored
        }
        else
        {
            currentFilter.rarityFilter = (Equipment.EquipmentRarity)(value - 1);
        }
        
        UpdateActiveFiltersDisplay();
        OnFilterChanged?.Invoke(currentFilter);
    }
    
    private void OnMinLevelChanged(float value)
    {
        currentFilter.minLevel = (int)value;
        
        // Ensure min level doesn't exceed max level
        if (currentFilter.minLevel > currentFilter.maxLevel)
        {
            maxLevelSlider.value = currentFilter.minLevel;
            currentFilter.maxLevel = currentFilter.minLevel;
        }
        
        UpdateLevelDisplay();
        UpdateActiveFiltersDisplay();
        OnFilterChanged?.Invoke(currentFilter);
    }
    
    private void OnMaxLevelChanged(float value)
    {
        currentFilter.maxLevel = (int)value;
        
        // Ensure max level doesn't go below min level
        if (currentFilter.maxLevel < currentFilter.minLevel)
        {
            minLevelSlider.value = currentFilter.maxLevel;
            currentFilter.minLevel = currentFilter.maxLevel;
        }
        
        UpdateLevelDisplay();
        UpdateActiveFiltersDisplay();
        OnFilterChanged?.Invoke(currentFilter);
    }
    
    private void OnSearchTextChanged(string text)
    {
        currentFilter.searchText = text;
        UpdateActiveFiltersDisplay();
        OnFilterChanged?.Invoke(currentFilter);
    }
    
    private void OnUsableToggleChanged(bool value)
    {
        currentFilter.onlyUsable = value;
        UpdateActiveFiltersDisplay();
        OnFilterChanged?.Invoke(currentFilter);
    }
    
    private void UpdateLevelDisplay()
    {
        minLevelText.text = $"Niv. Min: {currentFilter.minLevel}";
        maxLevelText.text = $"Niv. Max: {currentFilter.maxLevel}";
    }
    
    private void UpdateActiveFiltersDisplay()
    {
        string filtersText = "Filtres actifs: ";
        bool hasFilters = false;
        
        if (typeDropdown.value != 0)
        {
            filtersText += GetTypeDisplayName(currentFilter.typeFilter) + " ";
            hasFilters = true;
        }
        
        if (rarityDropdown.value != 0)
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
            filtersText += $"Recherche: '{currentFilter.searchText}' ";
            hasFilters = true;
        }
        
        if (currentFilter.onlyUsable)
        {
            filtersText += "Utilisable ";
            hasFilters = true;
        }
        
        if (!hasFilters)
        {
            filtersText = "Aucun filtre actif";
        }
        
        activeFiltersText.text = filtersText;
    }
    
    private void ResetFilters()
    {
        currentFilter = new EquipmentFilterData();
        
        typeDropdown.value = 0;
        rarityDropdown.value = 0;
        minLevelSlider.value = 1;
        maxLevelSlider.value = 50;
        searchInput.text = "";
        onlyUsableToggle.isOn = false;
        
        UpdateLevelDisplay();
        UpdateActiveFiltersDisplay();
        OnFilterChanged?.Invoke(currentFilter);
    }
    
    private void ApplyFilters()
    {
        OnFilterChanged?.Invoke(currentFilter);
    }
    
    public void SetAdventurer(Adventurer adventurer)
    {
        // This allows the filter to check equipment compatibility
        // The adventurer reference would be passed to the filter logic
    }
    
    public EquipmentFilterData GetCurrentFilter()
    {
        return currentFilter;
    }
}