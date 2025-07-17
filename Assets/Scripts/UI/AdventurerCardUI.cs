using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class AdventurerCardUI : MonoBehaviour, IPointerClickHandler
{
    [Header("Visual Components")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image portraitImage;
    [SerializeField] private Image classIcon;
    [SerializeField] private Image rarityBorder;
    [SerializeField] private Image statusIcon;
    [SerializeField] private GameObject selectionHighlight;
    
    [Header("Text Components")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI classText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI experienceText;
    [SerializeField] private TextMeshProUGUI loyaltyText;
    [SerializeField] private TextMeshProUGUI salaryText;
    
    [Header("Stats Display")]
    [SerializeField] private TextMeshProUGUI strengthText;
    [SerializeField] private TextMeshProUGUI intelligenceText;
    [SerializeField] private TextMeshProUGUI agilityText;
    [SerializeField] private TextMeshProUGUI constitutionText;
    [SerializeField] private TextMeshProUGUI charismaText;
    [SerializeField] private TextMeshProUGUI luckText;
    
    [Header("Health and Mana")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider manaBar;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI manaText;
    
    [Header("Mission Stats")]
    [SerializeField] private TextMeshProUGUI missionsCompletedText;
    [SerializeField] private TextMeshProUGUI successRateText;
    [SerializeField] private GameObject missionStatsPanel;
    
    [Header("Party Info")]
    [SerializeField] private GameObject partyIndicator;
    [SerializeField] private TextMeshProUGUI partyNameText;
    [SerializeField] private Image partyRoleIcon;
    
    [Header("Equipment Preview")]
    [SerializeField] private Transform equipmentPreviewContainer;
    [SerializeField] private GameObject equipmentSlotPrefab;
    
    [Header("Color Scheme")]
    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color availableColor = Color.green;
    [SerializeField] private Color busyColor = new Color(1f, 0.5f, 0f); // Orange
    [SerializeField] private Color injuredColor = Color.red;
    [SerializeField] private Color deadColor = Color.black;
    
    private Adventurer adventurer;
    private bool isSelected = false;
    private float lastClickTime = 0f;
    private const float doubleClickTime = 0.5f;
    
    public event Action<Adventurer> OnCardClicked;
    public event Action<Adventurer> OnCardDoubleClicked;
    public event Action<Adventurer> OnCardRightClicked;
    
    public void SetAdventurer(Adventurer adventurer)
    {
        this.adventurer = adventurer;
        
        if (adventurer == null)
        {
            gameObject.SetActive(false);
            return;
        }
        
        UpdateCardDisplay();
        gameObject.SetActive(true);
    }
    
    private void UpdateCardDisplay()
    {
        if (adventurer == null) return;
        
        // Basic info
        nameText.text = adventurer.Name;
        levelText.text = $"Niv. {adventurer.Level}";
        classText.text = GetClassDisplayName(adventurer.Class);
        statusText.text = GetStatusDisplayName(adventurer.Status);
        
        // Experience and loyalty
        experienceText.text = $"Exp: {adventurer.Experience}/{adventurer.ExperienceToNext}";
        loyaltyText.text = $"Loyauté: {adventurer.LoyaltyLevel:F0}%";
        salaryText.text = $"Salaire: {adventurer.DailySalary}or/jour";
        
        // Stats
        strengthText.text = adventurer.Strength.ToString();
        intelligenceText.text = adventurer.Intelligence.ToString();
        agilityText.text = adventurer.Agility.ToString();
        constitutionText.text = adventurer.Constitution.ToString();
        charismaText.text = adventurer.Charisma.ToString();
        luckText.text = adventurer.Luck.ToString();
        
        // Health and mana
        healthBar.value = adventurer.HealthPercentage;
        manaBar.value = adventurer.ManaPercentage;
        healthText.text = $"{adventurer.Health}/{adventurer.MaxHealth}";
        manaText.text = $"{adventurer.Mana}/{adventurer.MaxMana}";
        
        // Mission stats
        missionsCompletedText.text = $"Missions: {adventurer.MissionsCompleted}";
        successRateText.text = $"Succès: {adventurer.SuccessRate * 100:F1}%";
        
        // Visual styling
        UpdateVisualStyling();
        
        // Party info
        UpdatePartyInfo();
        
        // Equipment preview
        UpdateEquipmentPreview();
    }
    
    private void UpdateVisualStyling()
    {
        // Rarity border
        Color rarityColor = GetRarityColor(adventurer.Rarity);
        rarityBorder.color = rarityColor;
        
        // Status-based background color
        Color statusColor = GetStatusColor(adventurer.Status);
        backgroundImage.color = statusColor;
        
        // Class icon (if available)
        // classIcon.sprite = GetClassIcon(adventurer.Class);
        
        // Status icon
        // statusIcon.sprite = GetStatusIcon(adventurer.Status);
        statusIcon.color = statusColor;
        
        // Health bar color
        if (adventurer.HealthPercentage < 0.25f)
            healthBar.fillRect.GetComponent<Image>().color = Color.red;
        else if (adventurer.HealthPercentage < 0.5f)
            healthBar.fillRect.GetComponent<Image>().color = Color.yellow;
        else
            healthBar.fillRect.GetComponent<Image>().color = Color.green;
        
        // Mana bar color
        manaBar.fillRect.GetComponent<Image>().color = Color.blue;
    }
    
    private void UpdatePartyInfo()
    {
        if (adventurer.IsInParty)
        {
            partyIndicator.SetActive(true);
            partyNameText.text = $"Groupe {adventurer.CurrentPartyId}";
            // partyRoleIcon.sprite = GetPartyRoleIcon(adventurer);
        }
        else
        {
            partyIndicator.SetActive(false);
        }
    }
    
    private void UpdateEquipmentPreview()
    {
        // Clear existing equipment displays
        foreach (Transform child in equipmentPreviewContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Create equipment slot previews
        CreateEquipmentSlotPreview(adventurer.Weapon);
        CreateEquipmentSlotPreview(adventurer.Armor);
        CreateEquipmentSlotPreview(adventurer.Helmet);
        CreateEquipmentSlotPreview(adventurer.Boots);
        CreateEquipmentSlotPreview(adventurer.Accessory);
    }
    
    private void CreateEquipmentSlotPreview(Equipment equipment)
    {
        var slotObject = Instantiate(equipmentSlotPrefab, equipmentPreviewContainer);
        var slotImage = slotObject.GetComponent<Image>();
        
        if (equipment != null)
        {
            if (equipment.Icon != null)
            {
                slotImage.sprite = equipment.Icon;
                slotImage.color = equipment.GetRarityColor();
            }
            else
            {
                slotImage.color = equipment.GetRarityColor();
            }
        }
        else
        {
            slotImage.color = Color.gray;
        }
    }
    
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        selectionHighlight.SetActive(selected);
        
        if (selected)
        {
            backgroundImage.color = selectedColor;
        }
        else
        {
            UpdateVisualStyling();
        }
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            float timeSinceLastClick = Time.time - lastClickTime;
            
            if (timeSinceLastClick < doubleClickTime)
            {
                // Double click
                OnCardDoubleClicked?.Invoke(adventurer);
            }
            else
            {
                // Single click
                OnCardClicked?.Invoke(adventurer);
            }
            
            lastClickTime = Time.time;
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnCardRightClicked?.Invoke(adventurer);
        }
    }
    
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
    
    private Color GetRarityColor(Adventurer.AdventurerRarity rarity)
    {
        switch (rarity)
        {
            case Adventurer.AdventurerRarity.Common: return Color.white;
            case Adventurer.AdventurerRarity.Uncommon: return Color.green;
            case Adventurer.AdventurerRarity.Rare: return Color.blue;
            case Adventurer.AdventurerRarity.Epic: return new Color(0.5f, 0f, 1f); // Purple
            case Adventurer.AdventurerRarity.Legendary: return new Color(1f, 0.5f, 0f); // Orange
            default: return Color.white;
        }
    }
    
    private Color GetStatusColor(Adventurer.AdventurerStatus status)
    {
        switch (status)
        {
            case Adventurer.AdventurerStatus.Available: return availableColor;
            case Adventurer.AdventurerStatus.OnMission: return busyColor;
            case Adventurer.AdventurerStatus.Injured: return injuredColor;
            case Adventurer.AdventurerStatus.Dead: return deadColor;
            case Adventurer.AdventurerStatus.Resting: return busyColor;
            case Adventurer.AdventurerStatus.Training: return busyColor;
            default: return normalColor;
        }
    }
    
    public Adventurer GetAdventurer()
    {
        return adventurer;
    }
    
    public bool IsSelected()
    {
        return isSelected;
    }
    
    public void RefreshDisplay()
    {
        UpdateCardDisplay();
    }
    
    public void SetCompactMode(bool compact)
    {
        // Toggle between compact and detailed view
        missionStatsPanel.SetActive(!compact);
        // Could hide/show other detailed elements
    }
}