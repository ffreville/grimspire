using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AdventurerEquipmentUI : MonoBehaviour
{
    [Header("Equipment Slots")]
    [SerializeField] private EquipmentSlotUI weaponSlot;
    [SerializeField] private EquipmentSlotUI armorSlot;
    [SerializeField] private EquipmentSlotUI helmetSlot;
    [SerializeField] private EquipmentSlotUI bootsSlot;
    [SerializeField] private EquipmentSlotUI accessorySlot;
    [SerializeField] private EquipmentSlotUI shieldSlot;

    [Header("Equipment Details")]
    [SerializeField] private GameObject equipmentDetailsPanel;
    [SerializeField] private TextMeshProUGUI equipmentNameText;
    [SerializeField] private TextMeshProUGUI equipmentTypeText;
    [SerializeField] private TextMeshProUGUI equipmentRarityText;
    [SerializeField] private TextMeshProUGUI equipmentDescriptionText;
    [SerializeField] private TextMeshProUGUI equipmentStatsText;
    [SerializeField] private TextMeshProUGUI equipmentDurabilityText;
    [SerializeField] private Image equipmentIcon;
    [SerializeField] private Image equipmentRarityBorder;

    [Header("Equipment Comparison")]
    [SerializeField] private GameObject comparisonPanel;
    [SerializeField] private TextMeshProUGUI comparisonStatsText;
    [SerializeField] private Button equipButton;
    [SerializeField] private Button unequipButton;

    [Header("Enhancement")]
    [SerializeField] private GameObject enhancementPanel;
    [SerializeField] private Button enhanceButton;
    [SerializeField] private TextMeshProUGUI enhancementLevelText;
    [SerializeField] private Slider enhancementProgressBar;
    [SerializeField] private TextMeshProUGUI enhancementCostText;

    [Header("Gem Sockets")]
    [SerializeField] private Transform gemSocketsContainer;
    [SerializeField] private GameObject gemSocketPrefab;
    [SerializeField] private List<GemSocketUI> gemSockets;

    [Header("Inventory")]
    [SerializeField] private Transform inventoryContainer;
    [SerializeField] private GameObject inventoryItemPrefab;
    [SerializeField] private ScrollRect inventoryScrollRect;
    [SerializeField] private EquipmentFilterUI equipmentFilter;

    private Adventurer currentAdventurer;
    private Equipment selectedEquipment;
    private List<Equipment> inventory;
    private List<EquipmentItemUI> inventoryItems;

    private void Start()
    {
        InitializeUI();
        inventory = new List<Equipment>();
        inventoryItems = new List<EquipmentItemUI>();
        gemSockets = new List<GemSocketUI>();
        
        // Hide panels initially
        equipmentDetailsPanel.SetActive(false);
        comparisonPanel.SetActive(false);
        enhancementPanel.SetActive(false);
        
        // Setup button events
        equipButton.onClick.AddListener(EquipSelectedItem);
        unequipButton.onClick.AddListener(UnequipSelectedItem);
        enhanceButton.onClick.AddListener(OpenEnhancementPanel);
        
        // Setup equipment filter
        equipmentFilter.OnFilterChanged += FilterInventory;
    }

    private void InitializeUI()
    {
        // Initialize equipment slots
        weaponSlot.Initialize(Equipment.EquipmentType.Weapon, OnEquipmentSlotClicked);
        armorSlot.Initialize(Equipment.EquipmentType.Armor, OnEquipmentSlotClicked);
        helmetSlot.Initialize(Equipment.EquipmentType.Helmet, OnEquipmentSlotClicked);
        bootsSlot.Initialize(Equipment.EquipmentType.Boots, OnEquipmentSlotClicked);
        accessorySlot.Initialize(Equipment.EquipmentType.Accessory, OnEquipmentSlotClicked);
        shieldSlot.Initialize(Equipment.EquipmentType.Shield, OnEquipmentSlotClicked);
    }

    public void SetAdventurer(Adventurer adventurer)
    {
        currentAdventurer = adventurer;
        UpdateEquipmentSlots();
        UpdateInventoryDisplay();
        UpdateAdventurerStats();
    }

    private void UpdateEquipmentSlots()
    {
        if (currentAdventurer == null) return;

        weaponSlot.SetEquipment(currentAdventurer.Weapon);
        armorSlot.SetEquipment(currentAdventurer.Armor);
        helmetSlot.SetEquipment(currentAdventurer.Helmet);
        bootsSlot.SetEquipment(currentAdventurer.Boots);
        accessorySlot.SetEquipment(currentAdventurer.Accessory);
        shieldSlot.SetEquipment(null); // Shield not implemented in Adventurer class
        
        // Update enhancement levels
        UpdateEnhancementDisplay();
    }

    private void UpdateInventoryDisplay()
    {
        // Clear existing items
        foreach (var item in inventoryItems)
        {
            if (item != null) Destroy(item.gameObject);
        }
        inventoryItems.Clear();

        // Add inventory items
        foreach (var equipment in inventory)
        {
            CreateInventoryItem(equipment);
        }
    }

    private void CreateInventoryItem(Equipment equipment)
    {
        var itemObject = Instantiate(inventoryItemPrefab, inventoryContainer);
        var itemUI = itemObject.GetComponent<EquipmentItemUI>();
        
        if (itemUI != null)
        {
            itemUI.SetEquipment(equipment);
            itemUI.OnItemClicked += OnInventoryItemClicked;
            itemUI.OnItemRightClicked += OnInventoryItemRightClicked;
            inventoryItems.Add(itemUI);
        }
    }

    private void OnEquipmentSlotClicked(Equipment equipment)
    {
        selectedEquipment = equipment;
        if (equipment != null)
        {
            ShowEquipmentDetails(equipment);
            unequipButton.gameObject.SetActive(true);
            equipButton.gameObject.SetActive(false);
        }
        else
        {
            HideEquipmentDetails();
        }
    }

    private void OnInventoryItemClicked(Equipment equipment)
    {
        selectedEquipment = equipment;
        ShowEquipmentDetails(equipment);
        
        // Show comparison if adventurer has equipment in same slot
        ShowEquipmentComparison(equipment);
        
        // Show equip button if compatible
        bool canEquip = equipment.CanBeEquippedBy(currentAdventurer);
        equipButton.gameObject.SetActive(canEquip);
        unequipButton.gameObject.SetActive(false);
    }

    private void OnInventoryItemRightClicked(Equipment equipment)
    {
        // Quick equip on right click
        if (equipment.CanBeEquippedBy(currentAdventurer))
        {
            EquipItem(equipment);
        }
    }

    private void ShowEquipmentDetails(Equipment equipment)
    {
        equipmentDetailsPanel.SetActive(true);
        
        equipmentNameText.text = equipment.Name;
        equipmentTypeText.text = equipment.Type.ToString();
        equipmentRarityText.text = equipment.Rarity.ToString();
        equipmentRarityText.color = equipment.GetRarityColor();
        equipmentDescriptionText.text = equipment.Description;
        equipmentStatsText.text = equipment.GetStatsDescription();
        
        // Durability display
        float durabilityPercent = equipment.DurabilityPercentage;
        equipmentDurabilityText.text = $"Durabilité: {equipment.Durability}/{equipment.MaxDurability}";
        equipmentDurabilityText.color = durabilityPercent > 0.5f ? Color.green : 
                                       durabilityPercent > 0.25f ? Color.yellow : Color.red;
        
        // Icon and border
        equipmentIcon.sprite = equipment.Icon;
        equipmentRarityBorder.color = equipment.GetRarityColor();
        
        // Update gem sockets
        UpdateGemSocketsDisplay(equipment);
    }

    private void ShowEquipmentComparison(Equipment newEquipment)
    {
        Equipment currentEquipment = GetCurrentEquipmentForType(newEquipment.Type);
        
        if (currentEquipment != null)
        {
            comparisonPanel.SetActive(true);
            
            string comparisonText = "Comparaison:\n";
            var newStats = newEquipment.StatBonuses;
            var currentStats = currentEquipment.StatBonuses;
            
            HashSet<string> allStats = new HashSet<string>();
            foreach (var stat in newStats.Keys) allStats.Add(stat);
            foreach (var stat in currentStats.Keys) allStats.Add(stat);
            
            foreach (string stat in allStats)
            {
                int newValue = newStats.ContainsKey(stat) ? newStats[stat] : 0;
                int currentValue = currentStats.ContainsKey(stat) ? currentStats[stat] : 0;
                int difference = newValue - currentValue;
                
                if (difference != 0)
                {
                    string color = difference > 0 ? "green" : "red";
                    string sign = difference > 0 ? "+" : "";
                    comparisonText += $"{stat}: <color={color}>{sign}{difference}</color>\n";
                }
            }
            
            comparisonStatsText.text = comparisonText;
        }
        else
        {
            comparisonPanel.SetActive(false);
        }
    }

    private void UpdateGemSocketsDisplay(Equipment equipment)
    {
        // Clear existing sockets
        foreach (var socket in gemSockets)
        {
            if (socket != null) Destroy(socket.gameObject);
        }
        gemSockets.Clear();
        
        // Get equipment sockets (this would need to be implemented in Equipment class)
        var equipmentSockets = EquipmentEnhancementSystem.Instance.GetEquipmentSockets(equipment);
        
        foreach (var socket in equipmentSockets)
        {
            var socketObject = Instantiate(gemSocketPrefab, gemSocketsContainer);
            var socketUI = socketObject.GetComponent<GemSocketUI>();
            
            if (socketUI != null)
            {
                socketUI.SetSocket(socket);
                socketUI.OnSocketClicked += OnGemSocketClicked;
                gemSockets.Add(socketUI);
            }
        }
    }

    private void OnGemSocketClicked(GemSocket socket)
    {
        // Open gem selection UI
        // This would open a gem inventory panel
    }

    private void UpdateEnhancementDisplay()
    {
        if (selectedEquipment == null) return;
        
        // Get enhancement level (would need to be implemented in Equipment class)
        int enhancementLevel = EquipmentEnhancementSystem.Instance.GetEnhancementLevel(selectedEquipment);
        
        enhancementLevelText.text = $"Niveau +{enhancementLevel}";
        
        // Update progress bar for next level
        float progress = (float)enhancementLevel / 15f; // Max level 15
        enhancementProgressBar.value = progress;
        
        // Update cost text
        if (enhancementLevel < 15)
        {
            var nextLevelCost = EquipmentEnhancementSystem.Instance.GetEnhancementCost(enhancementLevel + 1);
            enhancementCostText.text = $"Coût: {nextLevelCost} or";
        }
        else
        {
            enhancementCostText.text = "Niveau Maximum";
        }
    }

    private Equipment GetCurrentEquipmentForType(Equipment.EquipmentType type)
    {
        if (currentAdventurer == null) return null;
        
        switch (type)
        {
            case Equipment.EquipmentType.Weapon: return currentAdventurer.Weapon;
            case Equipment.EquipmentType.Armor: return currentAdventurer.Armor;
            case Equipment.EquipmentType.Helmet: return currentAdventurer.Helmet;
            case Equipment.EquipmentType.Boots: return currentAdventurer.Boots;
            case Equipment.EquipmentType.Accessory: return currentAdventurer.Accessory;
            case Equipment.EquipmentType.Shield: return null; // Shield not implemented in Adventurer class
            default: return null;
        }
    }

    private void EquipSelectedItem()
    {
        if (selectedEquipment != null && currentAdventurer != null)
        {
            EquipItem(selectedEquipment);
        }
    }

    private void UnequipSelectedItem()
    {
        if (selectedEquipment != null && currentAdventurer != null)
        {
            UnequipItem(selectedEquipment);
        }
    }

    private void EquipItem(Equipment equipment)
    {
        if (equipment == null || currentAdventurer == null) return;
        
        // Unequip current item of same type
        Equipment currentEquipment = GetCurrentEquipmentForType(equipment.Type);
        if (currentEquipment != null)
        {
            inventory.Add(currentEquipment);
        }
        
        // Equip new item
        SetAdventurerEquipment(equipment);
        inventory.Remove(equipment);
        
        // Update displays
        UpdateEquipmentSlots();
        UpdateInventoryDisplay();
        UpdateAdventurerStats();
        
        // Hide comparison panel
        comparisonPanel.SetActive(false);
    }

    private void UnequipItem(Equipment equipment)
    {
        if (equipment == null || currentAdventurer == null) return;
        
        // Add to inventory
        inventory.Add(equipment);
        
        // Remove from adventurer
        SetAdventurerEquipment(null, equipment.Type);
        
        // Update displays
        UpdateEquipmentSlots();
        UpdateInventoryDisplay();
        UpdateAdventurerStats();
        
        // Clear selection
        selectedEquipment = null;
        HideEquipmentDetails();
    }

    private void SetAdventurerEquipment(Equipment equipment, Equipment.EquipmentType? typeOverride = null)
    {
        Equipment.EquipmentType type = typeOverride ?? equipment?.Type ?? Equipment.EquipmentType.Weapon;
        
        // This would need to be implemented in Adventurer class
        // For now, just log the action
        Debug.Log($"Setting {type} equipment for {currentAdventurer.Name}: {equipment?.Name ?? "None"}");
    }

    private void UpdateAdventurerStats()
    {
        if (currentAdventurer == null) return;
        
        // Trigger stat recalculation
        // This would need to be implemented in Adventurer class
        // currentAdventurer.RecalculateStats();
    }

    private void FilterInventory(EquipmentFilterData filterData)
    {
        foreach (var itemUI in inventoryItems)
        {
            bool shouldShow = true;
            var equipment = itemUI.GetEquipment();
            
            // Filter by type
            if (filterData.typeFilter != Equipment.EquipmentType.Weapon && 
                equipment.Type != filterData.typeFilter)
            {
                shouldShow = false;
            }
            
            // Filter by rarity
            if (filterData.rarityFilter != Equipment.EquipmentRarity.Common && 
                equipment.Rarity != filterData.rarityFilter)
            {
                shouldShow = false;
            }
            
            // Filter by level
            if (equipment.RequiredLevel < filterData.minLevel || 
                equipment.RequiredLevel > filterData.maxLevel)
            {
                shouldShow = false;
            }
            
            // Filter by usability
            if (filterData.onlyUsable && !equipment.CanBeEquippedBy(currentAdventurer))
            {
                shouldShow = false;
            }
            
            itemUI.gameObject.SetActive(shouldShow);
        }
    }

    private void OpenEnhancementPanel()
    {
        enhancementPanel.SetActive(true);
        // This would open the enhancement UI with material selection
    }

    private void HideEquipmentDetails()
    {
        equipmentDetailsPanel.SetActive(false);
        comparisonPanel.SetActive(false);
        enhancementPanel.SetActive(false);
    }

    public void AddToInventory(Equipment equipment)
    {
        inventory.Add(equipment);
        UpdateInventoryDisplay();
    }

    public void RemoveFromInventory(Equipment equipment)
    {
        inventory.Remove(equipment);
        UpdateInventoryDisplay();
    }

    public List<Equipment> GetInventory()
    {
        return new List<Equipment>(inventory);
    }
}

[System.Serializable]
public class EquipmentFilterData
{
    public Equipment.EquipmentType typeFilter = Equipment.EquipmentType.Weapon;
    public Equipment.EquipmentRarity rarityFilter = Equipment.EquipmentRarity.Common;
    public int minLevel = 1;
    public int maxLevel = 50;
    public bool onlyUsable = false;
    public string searchText = "";
}