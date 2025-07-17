using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillItemUI : MonoBehaviour
{
    [Header("Visual Components")]
    [SerializeField] private TextMeshProUGUI skillNameText;
    [SerializeField] private TextMeshProUGUI skillLevelText;
    [SerializeField] private TextMeshProUGUI skillDescriptionText;
    [SerializeField] private Slider experienceBar;
    [SerializeField] private TextMeshProUGUI experienceText;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Image skillIcon;
    [SerializeField] private Image maxLevelIndicator;
    
    [Header("Colors")]
    [SerializeField] private Color maxLevelColor = new Color(1f, 0.84f, 0f); // Gold
    [SerializeField] private Color normalColor = Color.white;
    
    private AdventurerSkill skill;
    
    public void SetSkill(AdventurerSkill skill)
    {
        this.skill = skill;
        
        if (skill == null)
        {
            gameObject.SetActive(false);
            return;
        }
        
        UpdateDisplay();
        gameObject.SetActive(true);
    }
    
    private void UpdateDisplay()
    {
        // Basic info
        skillNameText.text = skill.Name;
        skillLevelText.text = $"Niveau {skill.Level}";
        skillDescriptionText.text = skill.Description;
        
        // Experience
        if (skill.IsMaxLevel)
        {
            experienceBar.value = 1f;
            experienceText.text = "MAX";
            experienceBar.fillRect.GetComponent<Image>().color = maxLevelColor;
            maxLevelIndicator.gameObject.SetActive(true);
        }
        else
        {
            experienceBar.value = (float)skill.Experience / skill.ExperienceToNext;
            experienceText.text = $"{skill.Experience}/{skill.ExperienceToNext}";
            experienceBar.fillRect.GetComponent<Image>().color = normalColor;
            maxLevelIndicator.gameObject.SetActive(false);
        }
        
        // Upgrade button
        upgradeButton.gameObject.SetActive(!skill.IsMaxLevel);
        upgradeButton.interactable = skill.Experience >= skill.ExperienceToNext;
        
        // Skill icon (if available)
        // skillIcon.sprite = GetSkillIcon(skill.Name);
    }
    
    private void Start()
    {
        if (upgradeButton != null)
        {
            upgradeButton.onClick.AddListener(UpgradeSkill);
        }
    }
    
    private void UpgradeSkill()
    {
        if (skill != null && !skill.IsMaxLevel)
        {
            // This would interface with the skill upgrade system
            Debug.Log($"Upgrading skill: {skill.Name}");
            UpdateDisplay();
        }
    }
    
    public AdventurerSkill GetSkill()
    {
        return skill;
    }
}

public class TraitItemUI : MonoBehaviour
{
    [Header("Visual Components")]
    [SerializeField] private TextMeshProUGUI traitNameText;
    [SerializeField] private TextMeshProUGUI traitDescriptionText;
    [SerializeField] private TextMeshProUGUI traitEffectsText;
    [SerializeField] private Image traitIcon;
    [SerializeField] private Image backgroundImage;
    
    [Header("Colors")]
    [SerializeField] private Color positiveColor = Color.green;
    [SerializeField] private Color negativeColor = Color.red;
    [SerializeField] private Color neutralColor = Color.gray;
    
    private AdventurerTrait trait;
    
    public void SetTrait(AdventurerTrait trait)
    {
        this.trait = trait;
        
        if (trait == null)
        {
            gameObject.SetActive(false);
            return;
        }
        
        UpdateDisplay();
        gameObject.SetActive(true);
    }
    
    private void UpdateDisplay()
    {
        // Basic info
        traitNameText.text = trait.Name;
        traitDescriptionText.text = trait.Description;
        
        // Effects
        string effectsText = "";
        foreach (var modifier in trait.StatModifiers)
        {
            string sign = modifier.Value > 0 ? "+" : "";
            effectsText += $"{sign}{modifier.Value * 100:F0}% {modifier.Key}\n";
        }
        traitEffectsText.text = effectsText;
        
        // Color coding
        Color backgroundColor = trait.IsPositive ? positiveColor : negativeColor;
        backgroundImage.color = new Color(backgroundColor.r, backgroundColor.g, backgroundColor.b, 0.3f);
        
        // Trait icon (if available)
        // traitIcon.sprite = GetTraitIcon(trait.Name);
    }
    
    public AdventurerTrait GetTrait()
    {
        return trait;
    }
}

public class HistoryEntryUI : MonoBehaviour
{
    [Header("Visual Components")]
    [SerializeField] private TextMeshProUGUI dateText;
    [SerializeField] private TextMeshProUGUI missionNameText;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI experienceText;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private Image resultIcon;
    
    [Header("Colors")]
    [SerializeField] private Color successColor = Color.green;
    [SerializeField] private Color failureColor = Color.red;
    [SerializeField] private Color neutralColor = Color.gray;
    
    private MissionHistoryEntry historyEntry;
    
    public void SetHistoryEntry(MissionHistoryEntry entry)
    {
        this.historyEntry = entry;
        
        if (entry == null)
        {
            gameObject.SetActive(false);
            return;
        }
        
        UpdateDisplay();
        gameObject.SetActive(true);
    }
    
    private void UpdateDisplay()
    {
        dateText.text = historyEntry.date.ToString("dd/MM/yyyy");
        missionNameText.text = historyEntry.missionName;
        resultText.text = historyEntry.result;
        experienceText.text = $"+{historyEntry.experienceGained} Exp";
        goldText.text = $"+{historyEntry.goldEarned} Or";
        
        // Color coding based on result
        Color resultColor = historyEntry.result.ToLower().Contains("succès") ? successColor : failureColor;
        resultText.color = resultColor;
        resultIcon.color = resultColor;
    }
    
    public MissionHistoryEntry GetHistoryEntry()
    {
        return historyEntry;
    }
}

public class MasteryItemUI : MonoBehaviour
{
    [Header("Visual Components")]
    [SerializeField] private TextMeshProUGUI masteryNameText;
    [SerializeField] private TextMeshProUGUI masteryLevelText;
    [SerializeField] private Slider masteryBar;
    [SerializeField] private TextMeshProUGUI masteryEffectsText;
    [SerializeField] private Image masteryIcon;
    
    private MasteryData mastery;
    
    public void SetMastery(MasteryData mastery)
    {
        this.mastery = mastery;
        
        if (mastery == null)
        {
            gameObject.SetActive(false);
            return;
        }
        
        UpdateDisplay();
        gameObject.SetActive(true);
    }
    
    private void UpdateDisplay()
    {
        masteryNameText.text = mastery.name;
        masteryLevelText.text = $"Niveau {mastery.level}/{mastery.maxLevel}";
        masteryBar.value = (float)mastery.level / mastery.maxLevel;
        
        // Effects
        string effectsText = "";
        foreach (var bonus in mastery.bonuses)
        {
            float totalBonus = bonus.Value * mastery.level;
            effectsText += $"+{totalBonus:F1}% {bonus.Key}\n";
        }
        masteryEffectsText.text = effectsText;
        
        // Mastery icon (if available)
        // masteryIcon.sprite = GetMasteryIcon(mastery.name);
    }
    
    public MasteryData GetMastery()
    {
        return mastery;
    }
}