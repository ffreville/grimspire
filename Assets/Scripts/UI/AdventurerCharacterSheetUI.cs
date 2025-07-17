using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AdventurerCharacterSheetUI : MonoBehaviour
{
    [Header("Main Display")]
    [SerializeField] private GameObject characterSheetPanel;
    [SerializeField] private Button closeButton;
    [SerializeField] private TabGroup tabGroup;
    
    [Header("General Tab")]
    [SerializeField] private GameObject generalTab;
    [SerializeField] private Image portraitImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI classText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI rarityText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI biographyText;
    [SerializeField] private TextMeshProUGUI recruitmentDateText;
    [SerializeField] private TextMeshProUGUI daysInServiceText;
    [SerializeField] private TextMeshProUGUI loyaltyText;
    [SerializeField] private TextMeshProUGUI salaryText;
    [SerializeField] private Slider loyaltySlider;
    
    [Header("Stats Tab")]
    [SerializeField] private GameObject statsTab;
    [SerializeField] private TextMeshProUGUI strengthText;
    [SerializeField] private TextMeshProUGUI intelligenceText;
    [SerializeField] private TextMeshProUGUI agilityText;
    [SerializeField] private TextMeshProUGUI constitutionText;
    [SerializeField] private TextMeshProUGUI charismaText;
    [SerializeField] private TextMeshProUGUI luckText;
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI defenseText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI manaText;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider manaBar;
    [SerializeField] private Slider experienceBar;
    [SerializeField] private TextMeshProUGUI experienceText;
    [SerializeField] private TextMeshProUGUI skillPointsText;
    
    [Header("Skills Tab")]
    [SerializeField] private GameObject skillsTab;
    [SerializeField] private Transform skillsContainer;
    [SerializeField] private GameObject skillItemPrefab;
    [SerializeField] private Transform traitsContainer;
    [SerializeField] private GameObject traitItemPrefab;
    [SerializeField] private TextMeshProUGUI specialtiesText;
    
    [Header("Equipment Tab")]
    [SerializeField] private GameObject equipmentTab;
    [SerializeField] private AdventurerEquipmentUI equipmentUI;
    
    [Header("History Tab")]
    [SerializeField] private GameObject historyTab;
    [SerializeField] private Transform historyContainer;
    [SerializeField] private GameObject historyEntryPrefab;
    [SerializeField] private TextMeshProUGUI missionsCompletedText;
    [SerializeField] private TextMeshProUGUI successRateText;
    [SerializeField] private TextMeshProUGUI totalDamageDealtText;
    [SerializeField] private TextMeshProUGUI totalDamageTakenText;
    [SerializeField] private TextMeshProUGUI enemiesKilledText;
    [SerializeField] private ScrollRect historyScrollRect;
    
    [Header("Progression Tab")]
    [SerializeField] private GameObject progressionTab;
    [SerializeField] private TalentTreeUI talentTreeUI;
    [SerializeField] private Transform masteryContainer;
    [SerializeField] private GameObject masteryItemPrefab;
    [SerializeField] private Button levelUpButton;
    [SerializeField] private TextMeshProUGUI levelUpCostText;
    
    [Header("Actions")]
    [SerializeField] private Button healButton;
    [SerializeField] private Button restButton;
    [SerializeField] private Button trainButton;
    [SerializeField] private Button dismissButton;
    [SerializeField] private Button renameButton;
    [SerializeField] private Button assignPartyButton;
    
    private Adventurer currentAdventurer;
    private List<SkillItemUI> skillItems;
    private List<TraitItemUI> traitItems;
    private List<HistoryEntryUI> historyEntries;
    private List<MasteryItemUI> masteryItems;
    
    public System.Action<Adventurer> OnAdventurerChanged;
    public System.Action OnCloseRequested;
    
    private void Start()
    {
        InitializeCharacterSheet();
        characterSheetPanel.SetActive(false);
    }
    
    private void InitializeCharacterSheet()
    {
        skillItems = new List<SkillItemUI>();
        traitItems = new List<TraitItemUI>();
        historyEntries = new List<HistoryEntryUI>();
        masteryItems = new List<MasteryItemUI>();
        
        // Setup tab system
        tabGroup.OnTabSelected += OnTabSelected;
        
        // Setup button events
        closeButton.onClick.AddListener(CloseCharacterSheet);
        healButton.onClick.AddListener(HealAdventurer);
        restButton.onClick.AddListener(RestAdventurer);
        trainButton.onClick.AddListener(TrainAdventurer);
        dismissButton.onClick.AddListener(DismissAdventurer);
        renameButton.onClick.AddListener(RenameAdventurer);
        assignPartyButton.onClick.AddListener(AssignToParty);
        levelUpButton.onClick.AddListener(LevelUpAdventurer);
        
        // Initialize equipment UI
        if (equipmentUI != null)
        {
            equipmentUI.gameObject.SetActive(false);
        }
        
        // Initialize talent tree UI
        if (talentTreeUI != null)
        {
            talentTreeUI.gameObject.SetActive(false);
        }
    }
    
    public void ShowCharacterSheet(Adventurer adventurer)
    {
        if (adventurer == null) return;
        
        currentAdventurer = adventurer;
        characterSheetPanel.SetActive(true);
        
        // Set default tab
        tabGroup.SelectTab(0);
        
        UpdateAllTabs();
    }
    
    public void CloseCharacterSheet()
    {
        characterSheetPanel.SetActive(false);
        OnCloseRequested?.Invoke();
    }
    
    private void OnTabSelected(int tabIndex)
    {
        // Hide all tabs
        generalTab.SetActive(false);
        statsTab.SetActive(false);
        skillsTab.SetActive(false);
        equipmentTab.SetActive(false);
        historyTab.SetActive(false);
        progressionTab.SetActive(false);
        
        // Show selected tab
        switch (tabIndex)
        {
            case 0:
                generalTab.SetActive(true);
                UpdateGeneralTab();
                break;
            case 1:
                statsTab.SetActive(true);
                UpdateStatsTab();
                break;
            case 2:
                skillsTab.SetActive(true);
                UpdateSkillsTab();
                break;
            case 3:
                equipmentTab.SetActive(true);
                UpdateEquipmentTab();
                break;
            case 4:
                historyTab.SetActive(true);
                UpdateHistoryTab();
                break;
            case 5:
                progressionTab.SetActive(true);
                UpdateProgressionTab();
                break;
        }
    }
    
    private void UpdateAllTabs()
    {
        UpdateGeneralTab();
        UpdateStatsTab();
        UpdateSkillsTab();
        UpdateEquipmentTab();
        UpdateHistoryTab();
        UpdateProgressionTab();
    }
    
    private void UpdateGeneralTab()
    {
        if (currentAdventurer == null) return;
        
        // Basic information
        nameText.text = currentAdventurer.Name;
        classText.text = GetClassDisplayName(currentAdventurer.Class);
        levelText.text = $"Niveau {currentAdventurer.Level}";
        rarityText.text = GetRarityDisplayName(currentAdventurer.Rarity);
        rarityText.color = GetRarityColor(currentAdventurer.Rarity);
        statusText.text = GetStatusDisplayName(currentAdventurer.Status);
        statusText.color = GetStatusColor(currentAdventurer.Status);
        
        // Biography and background
        biographyText.text = currentAdventurer.Biography;
        recruitmentDateText.text = $"Recruté le: {currentAdventurer.RecruitmentDate:dd/MM/yyyy}";
        daysInServiceText.text = $"Jours de service: {currentAdventurer.DaysInService}";
        
        // Loyalty and salary
        loyaltyText.text = $"Loyauté: {currentAdventurer.LoyaltyLevel:F1}%";
        loyaltySlider.value = currentAdventurer.LoyaltyLevel / 100f;
        salaryText.text = $"Salaire: {currentAdventurer.DailySalary} or/jour";
        
        // Portrait (if available)
        // portraitImage.sprite = currentAdventurer.Portrait;
    }
    
    private void UpdateStatsTab()
    {
        if (currentAdventurer == null) return;
        
        // Primary stats
        strengthText.text = $"Force: {currentAdventurer.Strength}";
        intelligenceText.text = $"Intelligence: {currentAdventurer.Intelligence}";
        agilityText.text = $"Agilité: {currentAdventurer.Agility}";
        constitutionText.text = $"Constitution: {currentAdventurer.Constitution}";
        charismaText.text = $"Charisme: {currentAdventurer.Charisma}";
        luckText.text = $"Chance: {currentAdventurer.Luck}";
        
        // Derived stats
        attackText.text = $"Attaque: {CalculateAttack(currentAdventurer)}";
        defenseText.text = $"Défense: {CalculateDefense(currentAdventurer)}";
        
        // Health and mana
        healthText.text = $"Santé: {currentAdventurer.Health}/{currentAdventurer.MaxHealth}";
        manaText.text = $"Mana: {currentAdventurer.Mana}/{currentAdventurer.MaxMana}";
        healthBar.value = currentAdventurer.HealthPercentage;
        manaBar.value = currentAdventurer.ManaPercentage;
        
        // Experience
        experienceText.text = $"Expérience: {currentAdventurer.Experience}/{currentAdventurer.ExperienceToNext}";
        experienceBar.value = (float)currentAdventurer.Experience / currentAdventurer.ExperienceToNext;
        skillPointsText.text = $"Points de compétence: {currentAdventurer.SkillPoints}";
    }
    
    private void UpdateSkillsTab()
    {
        if (currentAdventurer == null) return;
        
        // Clear existing items
        foreach (var item in skillItems)
        {
            if (item != null) Destroy(item.gameObject);
        }
        skillItems.Clear();
        
        foreach (var item in traitItems)
        {
            if (item != null) Destroy(item.gameObject);
        }
        traitItems.Clear();
        
        // Create skill items
        foreach (var skill in currentAdventurer.Skills)
        {
            CreateSkillItem(skill);
        }
        
        // Create trait items
        foreach (var trait in currentAdventurer.Traits)
        {
            CreateTraitItem(trait);
        }
        
        // Update specialties
        specialtiesText.text = "Spécialités: " + string.Join(", ", currentAdventurer.Specialties);
    }
    
    private void CreateSkillItem(AdventurerSkill skill)
    {
        var skillObject = Instantiate(skillItemPrefab, skillsContainer);
        var skillUI = skillObject.GetComponent<SkillItemUI>();
        
        if (skillUI != null)
        {
            skillUI.SetSkill(skill);
            skillItems.Add(skillUI);
        }
    }
    
    private void CreateTraitItem(AdventurerTrait trait)
    {
        var traitObject = Instantiate(traitItemPrefab, traitsContainer);
        var traitUI = traitObject.GetComponent<TraitItemUI>();
        
        if (traitUI != null)
        {
            traitUI.SetTrait(trait);
            traitItems.Add(traitUI);
        }
    }
    
    private void UpdateEquipmentTab()
    {
        if (currentAdventurer == null || equipmentUI == null) return;
        
        equipmentUI.SetAdventurer(currentAdventurer);
    }
    
    private void UpdateHistoryTab()
    {
        if (currentAdventurer == null) return;
        
        // Mission statistics
        missionsCompletedText.text = $"Missions accomplies: {currentAdventurer.MissionsCompleted}";
        successRateText.text = $"Taux de réussite: {currentAdventurer.SuccessRate * 100:F1}%";
        totalDamageDealtText.text = $"Dégâts infligés: {currentAdventurer.TotalDamageDealt}";
        totalDamageTakenText.text = $"Dégâts subis: {currentAdventurer.TotalDamageTaken}";
        enemiesKilledText.text = $"Ennemis tués: {currentAdventurer.EnemiesKilled}";
        
        // Clear existing history entries
        foreach (var entry in historyEntries)
        {
            if (entry != null) Destroy(entry.gameObject);
        }
        historyEntries.Clear();
        
        // Create history entries
        // This would typically load from a mission history system
        CreateHistoryEntries();
    }
    
    private void CreateHistoryEntries()
    {
        // Placeholder for mission history
        // In a real implementation, this would load from a MissionHistory system
        var sampleEntry = Instantiate(historyEntryPrefab, historyContainer);
        var entryUI = sampleEntry.GetComponent<HistoryEntryUI>();
        
        if (entryUI != null)
        {
            entryUI.SetHistoryEntry(new MissionHistoryEntry
            {
                missionName = "Mission d'exemple",
                date = System.DateTime.Now,
                result = "Succès",
                experienceGained = 100,
                goldEarned = 50
            });
            historyEntries.Add(entryUI);
        }
    }
    
    private void UpdateProgressionTab()
    {
        if (currentAdventurer == null) return;
        
        // Update talent tree
        if (talentTreeUI != null)
        {
            talentTreeUI.SetAdventurer(currentAdventurer);
        }
        
        // Clear existing mastery items
        foreach (var item in masteryItems)
        {
            if (item != null) Destroy(item.gameObject);
        }
        masteryItems.Clear();
        
        // Create mastery items
        CreateMasteryItems();
        
        // Update level up button
        bool canLevelUp = currentAdventurer.Experience >= currentAdventurer.ExperienceToNext;
        levelUpButton.interactable = canLevelUp;
        levelUpCostText.text = canLevelUp ? "Disponible!" : $"Exp. requis: {currentAdventurer.ExperienceToNext - currentAdventurer.Experience}";
    }
    
    private void CreateMasteryItems()
    {
        // Placeholder for mastery system
        // In a real implementation, this would show weapon/skill masteries
        var masteryTemplates = ProgressionSystem.Instance.GetMasteryTemplates();
        
        foreach (var template in masteryTemplates)
        {
            var masteryObject = Instantiate(masteryItemPrefab, masteryContainer);
            var masteryUI = masteryObject.GetComponent<MasteryItemUI>();
            
            if (masteryUI != null)
            {
                masteryUI.SetMastery(template.Value);
                masteryItems.Add(masteryUI);
            }
        }
    }
    
    // Action handlers
    private void HealAdventurer()
    {
        if (currentAdventurer == null) return;
        
        // Heal the adventurer
        // This would interface with a healing system
        Debug.Log($"Healing {currentAdventurer.Name}");
        UpdateStatsTab();
    }
    
    private void RestAdventurer()
    {
        if (currentAdventurer == null) return;
        
        // Set adventurer to rest
        Debug.Log($"Setting {currentAdventurer.Name} to rest");
        UpdateGeneralTab();
    }
    
    private void TrainAdventurer()
    {
        if (currentAdventurer == null) return;
        
        // Start training
        Debug.Log($"Starting training for {currentAdventurer.Name}");
        UpdateGeneralTab();
    }
    
    private void DismissAdventurer()
    {
        if (currentAdventurer == null) return;
        
        // Dismiss the adventurer
        Debug.Log($"Dismissing {currentAdventurer.Name}");
        CloseCharacterSheet();
    }
    
    private void RenameAdventurer()
    {
        if (currentAdventurer == null) return;
        
        // Open rename dialog
        Debug.Log($"Renaming {currentAdventurer.Name}");
    }
    
    private void AssignToParty()
    {
        if (currentAdventurer == null) return;
        
        // Open party assignment UI
        Debug.Log($"Assigning {currentAdventurer.Name} to party");
    }
    
    private void LevelUpAdventurer()
    {
        if (currentAdventurer == null) return;
        
        // Level up the adventurer
        Debug.Log($"Leveling up {currentAdventurer.Name}");
        UpdateAllTabs();
    }
    
    // Helper methods
    private int CalculateAttack(Adventurer adventurer)
    {
        int baseAttack = adventurer.Strength;
        if (adventurer.Weapon != null)
        {
            baseAttack += adventurer.Weapon.GetStatBonus("Attack");
        }
        return baseAttack;
    }
    
    private int CalculateDefense(Adventurer adventurer)
    {
        int baseDefense = adventurer.Constitution;
        if (adventurer.Armor != null)
        {
            baseDefense += adventurer.Armor.GetStatBonus("Defense");
        }
        return baseDefense;
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
            case Adventurer.AdventurerRarity.Epic: return new Color(0.5f, 0f, 1f);
            case Adventurer.AdventurerRarity.Legendary: return new Color(1f, 0.5f, 0f);
            default: return Color.white;
        }
    }
    
    private Color GetStatusColor(Adventurer.AdventurerStatus status)
    {
        switch (status)
        {
            case Adventurer.AdventurerStatus.Available: return Color.green;
            case Adventurer.AdventurerStatus.OnMission: return Color.yellow;
            case Adventurer.AdventurerStatus.Injured: return Color.red;
            case Adventurer.AdventurerStatus.Dead: return Color.black;
            case Adventurer.AdventurerStatus.Resting: return Color.cyan;
            case Adventurer.AdventurerStatus.Training: return Color.magenta;
            default: return Color.white;
        }
    }
    
    public void RefreshCharacterSheet()
    {
        if (currentAdventurer != null)
        {
            UpdateAllTabs();
        }
    }
    
    public Adventurer GetCurrentAdventurer()
    {
        return currentAdventurer;
    }
}

// Data structures for history system
[System.Serializable]
public class MissionHistoryEntry
{
    public string missionName;
    public System.DateTime date;
    public string result;
    public int experienceGained;
    public int goldEarned;
}