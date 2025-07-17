using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class GemSocketUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Socket Visual")]
    [SerializeField] private Image socketBackground;
    [SerializeField] private Image gemIcon;
    [SerializeField] private Image socketTypeIndicator;
    [SerializeField] private GameObject emptySocketOverlay;
    [SerializeField] private GameObject gemTooltip;
    [SerializeField] private TextMeshProUGUI gemTooltipText;
    
    [Header("Socket Colors")]
    [SerializeField] private Color redSocketColor = Color.red;
    [SerializeField] private Color blueSocketColor = Color.blue;
    [SerializeField] private Color greenSocketColor = Color.green;
    [SerializeField] private Color universalSocketColor = Color.white;
    [SerializeField] private Color emptySocketColor = Color.gray;
    
    private GemSocket gemSocket;
    private bool isHovered = false;
    
    public event Action<GemSocket> OnSocketClicked;
    public event Action<GemSocket> OnSocketHovered;
    public event Action<GemSocket> OnSocketExited;
    
    public void SetSocket(GemSocket socket)
    {
        gemSocket = socket;
        
        if (socket == null)
        {
            gameObject.SetActive(false);
            return;
        }
        
        UpdateSocketDisplay();
        gameObject.SetActive(true);
    }
    
    private void UpdateSocketDisplay()
    {
        if (gemSocket == null) return;
        
        // Set socket type color
        Color socketColor = GetSocketTypeColor(gemSocket.socketType);
        socketTypeIndicator.color = socketColor;
        
        // Update gem display
        if (gemSocket.insertedGem != null)
        {
            ShowInsertedGem(gemSocket.insertedGem);
        }
        else
        {
            ShowEmptySocket();
        }
        
        // Disable socket if not active
        if (!gemSocket.isActive)
        {
            socketBackground.color = Color.gray;
            gemIcon.color = Color.gray;
        }
    }
    
    private void ShowInsertedGem(Gem gem)
    {
        emptySocketOverlay.SetActive(false);
        
        if (gem.Icon != null)
        {
            gemIcon.sprite = gem.Icon;
        }
        
        Color gemColor = GetGemRarityColor(gem.rarity);
        gemIcon.color = gemColor;
        socketBackground.color = new Color(gemColor.r, gemColor.g, gemColor.b, 0.3f);
        
        // Update tooltip
        UpdateGemTooltip(gem);
    }
    
    private void ShowEmptySocket()
    {
        emptySocketOverlay.SetActive(true);
        gemIcon.sprite = null;
        gemIcon.color = emptySocketColor;
        socketBackground.color = emptySocketColor;
        
        // Hide tooltip
        if (gemTooltip != null)
        {
            gemTooltip.SetActive(false);
        }
    }
    
    private void UpdateGemTooltip(Gem gem)
    {
        if (gemTooltipText == null) return;
        
        string tooltipText = $"<b>{gem.name}</b>\n";
        tooltipText += $"<color=gray>{gem.description}</color>\n\n";
        
        tooltipText += "<b>Bonus:</b>\n";
        foreach (var bonus in gem.statBonuses)
        {
            tooltipText += $"+{bonus.Value} {bonus.Key}\n";
        }
        
        foreach (var multiplier in gem.statMultipliers)
        {
            tooltipText += $"+{multiplier.Value * 100:F1}% {multiplier.Key}\n";
        }
        
        if (gem.specialEffects.Count > 0)
        {
            tooltipText += "\n<b>Effets spéciaux:</b>\n";
            foreach (var effect in gem.specialEffects)
            {
                tooltipText += $"• {effect}\n";
            }
        }
        
        gemTooltipText.text = tooltipText;
    }
    
    private Color GetSocketTypeColor(string socketType)
    {
        switch (socketType.ToLower())
        {
            case "red": return redSocketColor;
            case "blue": return blueSocketColor;
            case "green": return greenSocketColor;
            case "universal": return universalSocketColor;
            default: return emptySocketColor;
        }
    }
    
    private Color GetGemRarityColor(Equipment.EquipmentRarity rarity)
    {
        switch (rarity)
        {
            case Equipment.EquipmentRarity.Common: return Color.white;
            case Equipment.EquipmentRarity.Uncommon: return Color.green;
            case Equipment.EquipmentRarity.Rare: return Color.blue;
            case Equipment.EquipmentRarity.Epic: return new Color(0.5f, 0f, 1f);
            case Equipment.EquipmentRarity.Legendary: return new Color(1f, 0.5f, 0f);
            case Equipment.EquipmentRarity.Artifact: return Color.red;
            default: return Color.white;
        }
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (gemSocket != null && gemSocket.isActive)
        {
            OnSocketClicked?.Invoke(gemSocket);
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        
        if (gemSocket != null && gemSocket.insertedGem != null && gemTooltip != null)
        {
            gemTooltip.SetActive(true);
        }
        
        OnSocketHovered?.Invoke(gemSocket);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        
        if (gemTooltip != null)
        {
            gemTooltip.SetActive(false);
        }
        
        OnSocketExited?.Invoke(gemSocket);
    }
    
    public GemSocket GetSocket()
    {
        return gemSocket;
    }
    
    public bool IsHovered()
    {
        return isHovered;
    }
    
    public void SetHighlight(bool highlight)
    {
        if (socketBackground != null)
        {
            socketBackground.color = highlight ? Color.yellow : GetSocketTypeColor(gemSocket?.socketType ?? "");
        }
    }
}