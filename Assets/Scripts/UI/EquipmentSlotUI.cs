using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquipmentSlotUI : MonoBehaviour, IPointerClickHandler
{
    [Header("Visual Components")]
    [SerializeField] private Image slotIcon;
    [SerializeField] private Image equipmentIcon;
    [SerializeField] private Image rarityBorder;
    [SerializeField] private Image enhancementGlow;
    [SerializeField] private TMPro.TextMeshProUGUI enhancementLevelText;
    [SerializeField] private GameObject emptySlotOverlay;
    
    [Header("Slot Settings")]
    [SerializeField] private Equipment.EquipmentType slotType;
    [SerializeField] private Sprite defaultSlotIcon;
    [SerializeField] private Color defaultSlotColor = Color.gray;
    
    private Equipment currentEquipment;
    private Action<Equipment> onSlotClicked;
    
    public void Initialize(Equipment.EquipmentType type, Action<Equipment> onClicked)
    {
        slotType = type;
        onSlotClicked = onClicked;
        
        // Set default slot appearance
        slotIcon.sprite = defaultSlotIcon;
        slotIcon.color = defaultSlotColor;
        
        // Hide equipment-specific elements
        SetEmpty();
    }
    
    public void SetEquipment(Equipment equipment)
    {
        currentEquipment = equipment;
        
        if (equipment == null)
        {
            SetEmpty();
            return;
        }
        
        // Show equipment icon
        if (equipment.Icon != null)
        {
            equipmentIcon.sprite = equipment.Icon;
            equipmentIcon.color = Color.white;
        }
        else
        {
            equipmentIcon.sprite = defaultSlotIcon;
            equipmentIcon.color = Color.white;
        }
        
        // Set rarity border color
        rarityBorder.color = equipment.GetRarityColor();
        
        // Show enhancement level if enhanced
        int enhancementLevel = GetEnhancementLevel(equipment);
        if (enhancementLevel > 0)
        {
            enhancementLevelText.text = $"+{enhancementLevel}";
            enhancementLevelText.gameObject.SetActive(true);
            enhancementGlow.gameObject.SetActive(true);
            enhancementGlow.color = GetEnhancementGlowColor(enhancementLevel);
        }
        else
        {
            enhancementLevelText.gameObject.SetActive(false);
            enhancementGlow.gameObject.SetActive(false);
        }
        
        // Show durability warning if low
        if (equipment.DurabilityPercentage < 0.25f)
        {
            rarityBorder.color = Color.red;
        }
        
        // Hide empty overlay
        emptySlotOverlay.SetActive(false);
        equipmentIcon.gameObject.SetActive(true);
        rarityBorder.gameObject.SetActive(true);
    }
    
    private void SetEmpty()
    {
        currentEquipment = null;
        
        // Show empty slot overlay
        emptySlotOverlay.SetActive(true);
        
        // Hide equipment-specific elements
        equipmentIcon.gameObject.SetActive(false);
        rarityBorder.gameObject.SetActive(false);
        enhancementLevelText.gameObject.SetActive(false);
        enhancementGlow.gameObject.SetActive(false);
    }
    
    private int GetEnhancementLevel(Equipment equipment)
    {
        // This would need to be implemented in Equipment class
        // For now, return 0 as placeholder
        return 0;
    }
    
    private Color GetEnhancementGlowColor(int level)
    {
        if (level < 5) return Color.cyan;
        if (level < 10) return Color.yellow;
        if (level < 15) return Color.magenta;
        return Color.red;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        onSlotClicked?.Invoke(currentEquipment);
    }
    
    public Equipment GetEquipment()
    {
        return currentEquipment;
    }
    
    public Equipment.EquipmentType GetSlotType()
    {
        return slotType;
    }
}