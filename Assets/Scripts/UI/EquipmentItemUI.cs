using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class EquipmentItemUI : MonoBehaviour, IPointerClickHandler
{
    [Header("Visual Components")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private Image rarityBorder;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemLevelText;
    [SerializeField] private TextMeshProUGUI enhancementLevelText;
    [SerializeField] private GameObject durabilityWarning;
    [SerializeField] private GameObject newItemIndicator;
    
    [Header("Selection")]
    [SerializeField] private GameObject selectionHighlight;
    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private Color normalColor = Color.white;
    
    private Equipment equipment;
    private bool isSelected = false;
    
    public event Action<Equipment> OnItemClicked;
    public event Action<Equipment> OnItemRightClicked;
    
    public void SetEquipment(Equipment equipment)
    {
        this.equipment = equipment;
        
        if (equipment == null)
        {
            gameObject.SetActive(false);
            return;
        }
        
        // Set item icon
        if (equipment.Icon != null)
        {
            itemIcon.sprite = equipment.Icon;
        }
        
        // Set rarity border and background
        Color rarityColor = equipment.GetRarityColor();
        rarityBorder.color = rarityColor;
        backgroundImage.color = new Color(rarityColor.r, rarityColor.g, rarityColor.b, 0.2f);
        
        // Set item name with rarity styling
        itemNameText.text = equipment.Name;
        itemNameText.color = rarityColor;
        
        // Set level requirement
        itemLevelText.text = $"Niv. {equipment.RequiredLevel}";
        
        // Show enhancement level
        int enhancementLevel = GetEnhancementLevel(equipment);
        if (enhancementLevel > 0)
        {
            enhancementLevelText.text = $"+{enhancementLevel}";
            enhancementLevelText.gameObject.SetActive(true);
            enhancementLevelText.color = GetEnhancementColor(enhancementLevel);
        }
        else
        {
            enhancementLevelText.gameObject.SetActive(false);
        }
        
        // Show durability warning if low
        durabilityWarning.SetActive(equipment.DurabilityPercentage < 0.25f);
        
        // Hide new item indicator by default
        newItemIndicator.SetActive(false);
        
        gameObject.SetActive(true);
    }
    
    private int GetEnhancementLevel(Equipment equipment)
    {
        // This would need to be implemented in Equipment class
        // For now, return 0 as placeholder
        return 0;
    }
    
    private Color GetEnhancementColor(int level)
    {
        if (level < 5) return Color.cyan;
        if (level < 10) return Color.yellow;
        if (level < 15) return Color.magenta;
        return Color.red;
    }
    
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        selectionHighlight.SetActive(selected);
        backgroundImage.color = selected ? selectedColor : normalColor;
    }
    
    public void SetNewItemIndicator(bool isNew)
    {
        newItemIndicator.SetActive(isNew);
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnItemClicked?.Invoke(equipment);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnItemRightClicked?.Invoke(equipment);
        }
    }
    
    public Equipment GetEquipment()
    {
        return equipment;
    }
    
    public bool IsSelected()
    {
        return isSelected;
    }
}