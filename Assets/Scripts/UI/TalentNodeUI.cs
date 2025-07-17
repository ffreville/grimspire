using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class TalentNodeUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Visual Components")]
    [SerializeField] private Image nodeBackground;
    [SerializeField] private Image nodeIcon;
    [SerializeField] private Image nodeBorder;
    [SerializeField] private TextMeshProUGUI nodeRankText;
    [SerializeField] private GameObject lockOverlay;
    [SerializeField] private GameObject maxRankIndicator;
    [SerializeField] private GameObject availableGlow;
    
    [Header("Node States")]
    [SerializeField] private Color lockedColor = Color.gray;
    [SerializeField] private Color availableColor = Color.white;
    [SerializeField] private Color unlockedColor = Color.green;
    [SerializeField] private Color maxRankColor = new Color(1f, 0.84f, 0f); // Gold
    [SerializeField] private Color selectedColor = Color.yellow;
    
    private TalentNode talent;
    private Adventurer adventurer;
    private bool isSelected = false;
    private bool isHovered = false;
    
    public event Action<TalentNode> OnNodeClicked;
    public event Action<TalentNode> OnNodeHovered;
    public event Action<TalentNode> OnNodeExited;
    
    public void SetTalent(TalentNode talent)
    {
        this.talent = talent;
        
        if (talent == null)
        {
            gameObject.SetActive(false);
            return;
        }
        
        UpdateNodeDisplay();
        gameObject.SetActive(true);
    }
    
    public void SetAdventurer(Adventurer adventurer)
    {
        this.adventurer = adventurer;
        UpdateNodeDisplay();
    }
    
    private void UpdateNodeDisplay()
    {
        if (talent == null) return;
        
        // Update rank display
        if (talent.maxRank > 1)
        {
            nodeRankText.text = $"{talent.currentRank}/{talent.maxRank}";
            nodeRankText.gameObject.SetActive(true);
        }
        else
        {
            nodeRankText.gameObject.SetActive(false);
        }
        
        // Update visual state
        UpdateNodeState();
        
        // Update icon
        // nodeIcon.sprite = GetTalentIcon(talent.id);
    }
    
    private void UpdateNodeState()
    {
        if (talent == null) return;
        
        bool canUnlock = adventurer != null && talent.CanUnlock(adventurer, GetAllTalentsForClass());
        bool isMaxRank = talent.currentRank >= talent.maxRank;
        
        // Determine node state
        if (!talent.unlocked)
        {
            if (canUnlock)
            {
                // Available to unlock
                SetNodeState(NodeState.Available);
            }
            else
            {
                // Locked
                SetNodeState(NodeState.Locked);
            }
        }
        else
        {
            if (isMaxRank)
            {
                // Max rank reached
                SetNodeState(NodeState.MaxRank);
            }
            else
            {
                // Unlocked but can be upgraded
                SetNodeState(NodeState.Unlocked);
            }
        }
    }
    
    private void SetNodeState(NodeState state)
    {
        switch (state)
        {
            case NodeState.Locked:
                nodeBackground.color = lockedColor;
                nodeBorder.color = lockedColor;
                lockOverlay.SetActive(true);
                availableGlow.SetActive(false);
                maxRankIndicator.SetActive(false);
                break;
                
            case NodeState.Available:
                nodeBackground.color = availableColor;
                nodeBorder.color = availableColor;
                lockOverlay.SetActive(false);
                availableGlow.SetActive(true);
                maxRankIndicator.SetActive(false);
                break;
                
            case NodeState.Unlocked:
                nodeBackground.color = unlockedColor;
                nodeBorder.color = unlockedColor;
                lockOverlay.SetActive(false);
                availableGlow.SetActive(false);
                maxRankIndicator.SetActive(false);
                break;
                
            case NodeState.MaxRank:
                nodeBackground.color = maxRankColor;
                nodeBorder.color = maxRankColor;
                lockOverlay.SetActive(false);
                availableGlow.SetActive(false);
                maxRankIndicator.SetActive(true);
                break;
        }
        
        // Apply selection highlight
        if (isSelected)
        {
            nodeBorder.color = selectedColor;
        }
    }
    
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateNodeState();
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        OnNodeClicked?.Invoke(talent);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        
        // Add hover effect
        float hoverBrightness = 1.2f;
        Color currentColor = nodeBackground.color;
        nodeBackground.color = new Color(
            currentColor.r * hoverBrightness,
            currentColor.g * hoverBrightness,
            currentColor.b * hoverBrightness,
            currentColor.a
        );
        
        OnNodeHovered?.Invoke(talent);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        
        // Remove hover effect
        UpdateNodeState();
        
        OnNodeExited?.Invoke(talent);
    }
    
    private System.Collections.Generic.List<TalentNode> GetAllTalentsForClass()
    {
        // This should return all talents for the current class
        // For now, return empty list
        return new System.Collections.Generic.List<TalentNode>();
    }
    
    public TalentNode GetTalent()
    {
        return talent;
    }
    
    public bool IsSelected()
    {
        return isSelected;
    }
    
    public bool IsHovered()
    {
        return isHovered;
    }
    
    private enum NodeState
    {
        Locked,
        Available,
        Unlocked,
        MaxRank
    }
}

public class SpecializationItemUI : MonoBehaviour
{
    [Header("Visual Components")]
    [SerializeField] private Image specializationIcon;
    [SerializeField] private TextMeshProUGUI specializationNameText;
    [SerializeField] private TextMeshProUGUI specializationDescriptionText;
    [SerializeField] private TextMeshProUGUI requirementsText;
    [SerializeField] private TextMeshProUGUI bonusesText;
    [SerializeField] private Button unlockButton;
    [SerializeField] private GameObject unlockedIndicator;
    
    [Header("Colors")]
    [SerializeField] private Color availableColor = Color.white;
    [SerializeField] private Color unavailableColor = Color.gray;
    [SerializeField] private Color unlockedColor = Color.green;
    
    private SpecializationPath specialization;
    
    public event Action<SpecializationPath> OnSpecializationClicked;
    
    public void SetSpecialization(SpecializationPath specialization)
    {
        this.specialization = specialization;
        
        if (specialization == null)
        {
            gameObject.SetActive(false);
            return;
        }
        
        UpdateSpecializationDisplay();
        gameObject.SetActive(true);
    }
    
    private void UpdateSpecializationDisplay()
    {
        if (specialization == null) return;
        
        // Basic info
        specializationNameText.text = specialization.name;
        specializationDescriptionText.text = specialization.description;
        
        // Requirements
        string reqText = $"Niveau requis: {specialization.requiredLevel}\n";
        reqText += $"Classe: {GetClassDisplayName(specialization.baseClass)}\n";
        
        if (specialization.requiredTalents.Count > 0)
        {
            reqText += "Talents requis:\n";
            foreach (string talentId in specialization.requiredTalents)
            {
                reqText += $"• {talentId}\n"; // Would need to get talent name
            }
        }
        
        requirementsText.text = reqText;
        
        // Bonuses
        string bonusText = "Bonus:\n";
        foreach (var bonus in specialization.statBonuses)
        {
            bonusText += $"+{bonus.Value} {bonus.Key}\n";
        }
        
        if (specialization.uniqueAbilities.Count > 0)
        {
            bonusText += "Capacités uniques:\n";
            foreach (string ability in specialization.uniqueAbilities)
            {
                bonusText += $"• {ability}\n";
            }
        }
        
        bonusesText.text = bonusText;
        
        // State
        if (specialization.unlocked)
        {
            unlockButton.gameObject.SetActive(false);
            unlockedIndicator.SetActive(true);
            specializationIcon.color = unlockedColor;
        }
        else
        {
            unlockButton.gameObject.SetActive(true);
            unlockedIndicator.SetActive(false);
            specializationIcon.color = availableColor;
        }
        
        // Specialization icon
        // specializationIcon.sprite = GetSpecializationIcon(specialization.id);
    }
    
    private void Start()
    {
        if (unlockButton != null)
        {
            unlockButton.onClick.AddListener(OnUnlockClicked);
        }
    }
    
    private void OnUnlockClicked()
    {
        OnSpecializationClicked?.Invoke(specialization);
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
    
    public SpecializationPath GetSpecialization()
    {
        return specialization;
    }
}