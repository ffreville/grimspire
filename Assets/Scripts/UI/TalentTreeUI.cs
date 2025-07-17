using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TalentTreeUI : MonoBehaviour
{
    [Header("Talent Tree Display")]
    [SerializeField] private Transform talentNodesContainer;
    [SerializeField] private GameObject talentNodePrefab;
    [SerializeField] private LineRenderer connectionLinePrefab;
    [SerializeField] private ScrollRect talentScrollRect;
    
    [Header("Talent Details")]
    [SerializeField] private GameObject talentDetailsPanel;
    [SerializeField] private TextMeshProUGUI talentNameText;
    [SerializeField] private TextMeshProUGUI talentDescriptionText;
    [SerializeField] private TextMeshProUGUI talentCostText;
    [SerializeField] private TextMeshProUGUI talentRequirementsText;
    [SerializeField] private TextMeshProUGUI talentEffectsText;
    [SerializeField] private Button unlockTalentButton;
    [SerializeField] private Button upgradeTalentButton;
    [SerializeField] private Image talentIcon;
    
    [Header("Class Selection")]
    [SerializeField] private TMP_Dropdown classDropdown;
    [SerializeField] private Button resetTalentsButton;
    [SerializeField] private TextMeshProUGUI skillPointsText;
    [SerializeField] private TextMeshProUGUI unlockedTalentsText;
    
    [Header("Specialization Panel")]
    [SerializeField] private GameObject specializationPanel;
    [SerializeField] private Transform specializationContainer;
    [SerializeField] private GameObject specializationItemPrefab;
    [SerializeField] private Button specializationButton;
    
    [Header("Visual Settings")]
    [SerializeField] private Color availableColor = Color.white;
    [SerializeField] private Color unlockedColor = Color.green;
    [SerializeField] private Color lockedColor = Color.red;
    [SerializeField] private Color maxRankColor = new Color(1f, 0.84f, 0f); // Gold
    [SerializeField] private Color connectionColor = Color.cyan;
    
    private Adventurer currentAdventurer;
    private List<TalentNodeUI> talentNodes;
    private List<LineRenderer> connectionLines;
    private TalentNode selectedTalent;
    private Adventurer.AdventurerClass currentClass;
    
    public System.Action<TalentNode> OnTalentSelected;
    public System.Action<TalentNode> OnTalentUnlocked;
    
    private void Start()
    {
        InitializeTalentTree();
        talentDetailsPanel.SetActive(false);
        specializationPanel.SetActive(false);
    }
    
    private void InitializeTalentTree()
    {
        talentNodes = new List<TalentNodeUI>();
        connectionLines = new List<LineRenderer>();
        
        // Setup class dropdown
        SetupClassDropdown();
        
        // Setup button events
        unlockTalentButton.onClick.AddListener(UnlockSelectedTalent);
        upgradeTalentButton.onClick.AddListener(UpgradeSelectedTalent);
        resetTalentsButton.onClick.AddListener(ResetTalents);
        specializationButton.onClick.AddListener(ToggleSpecializationPanel);
        
        classDropdown.onValueChanged.AddListener(OnClassChanged);
    }
    
    private void SetupClassDropdown()
    {
        classDropdown.options.Clear();
        foreach (Adventurer.AdventurerClass adventurerClass in System.Enum.GetValues(typeof(Adventurer.AdventurerClass)))
        {
            classDropdown.options.Add(new TMP_Dropdown.OptionData(GetClassDisplayName(adventurerClass)));
        }
    }
    
    public void SetAdventurer(Adventurer adventurer)
    {
        currentAdventurer = adventurer;
        
        if (adventurer == null)
        {
            gameObject.SetActive(false);
            return;
        }
        
        currentClass = adventurer.Class;
        classDropdown.value = (int)currentClass;
        
        UpdateTalentTree();
        UpdateSkillPointsDisplay();
        UpdateSpecializationPanel();
        
        gameObject.SetActive(true);
    }
    
    private void UpdateTalentTree()
    {
        // Clear existing nodes and connections
        ClearTalentTree();
        
        // Get talents for current class
        var talents = ProgressionSystem.Instance.GetAvailableTalents(currentAdventurer);
        
        // Create talent nodes
        foreach (var talent in talents)
        {
            CreateTalentNode(talent);
        }
        
        // Create connections
        CreateTalentConnections();
    }
    
    private void ClearTalentTree()
    {
        foreach (var node in talentNodes)
        {
            if (node != null) Destroy(node.gameObject);
        }
        talentNodes.Clear();
        
        foreach (var line in connectionLines)
        {
            if (line != null) Destroy(line.gameObject);
        }
        connectionLines.Clear();
    }
    
    private void CreateTalentNode(TalentNode talent)
    {
        var nodeObject = Instantiate(talentNodePrefab, talentNodesContainer);
        var nodeUI = nodeObject.GetComponent<TalentNodeUI>();
        
        if (nodeUI != null)
        {
            nodeUI.SetTalent(talent);
            nodeUI.OnNodeClicked += OnTalentNodeClicked;
            nodeUI.OnNodeHovered += OnTalentNodeHovered;
            nodeUI.SetAdventurer(currentAdventurer);
            
            // Position node based on talent requirements and level
            PositionTalentNode(nodeUI, talent);
            
            talentNodes.Add(nodeUI);
        }
    }
    
    private void PositionTalentNode(TalentNodeUI nodeUI, TalentNode talent)
    {
        // Calculate position based on talent tier and class
        float xPosition = talent.requiredLevel * 100f; // Horizontal spacing by level requirement
        float yPosition = talent.id.GetHashCode() % 5 * 80f; // Vertical spacing
        
        nodeUI.transform.localPosition = new Vector3(xPosition, yPosition, 0);
    }
    
    private void CreateTalentConnections()
    {
        foreach (var node in talentNodes)
        {
            var talent = node.GetTalent();
            
            // Create lines to prerequisite talents
            foreach (string prereqId in talent.prerequisites)
            {
                var prereqNode = talentNodes.Find(n => n.GetTalent().id == prereqId);
                if (prereqNode != null)
                {
                    CreateConnectionLine(prereqNode, node);
                }
            }
        }
    }
    
    private void CreateConnectionLine(TalentNodeUI fromNode, TalentNodeUI toNode)
    {
        var line = Instantiate(connectionLinePrefab, talentNodesContainer);
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.material.color = connectionColor;
        line.startWidth = 2f;
        line.endWidth = 2f;
        line.positionCount = 2;
        
        Vector3 startPos = fromNode.transform.localPosition;
        Vector3 endPos = toNode.transform.localPosition;
        
        line.SetPosition(0, startPos);
        line.SetPosition(1, endPos);
        
        connectionLines.Add(line);
    }
    
    private void OnTalentNodeClicked(TalentNode talent)
    {
        selectedTalent = talent;
        ShowTalentDetails(talent);
        OnTalentSelected?.Invoke(talent);
    }
    
    private void OnTalentNodeHovered(TalentNode talent)
    {
        // Show quick tooltip or highlight connections
        HighlightTalentConnections(talent);
    }
    
    private void ShowTalentDetails(TalentNode talent)
    {
        if (talent == null) return;
        
        talentDetailsPanel.SetActive(true);
        
        // Basic info
        talentNameText.text = talent.name;
        talentDescriptionText.text = talent.description;
        talentCostText.text = $"Coût: {talent.GetTotalCost()} points";
        
        // Requirements
        string requirementsText = $"Niveau requis: {talent.requiredLevel}\n";
        requirementsText += $"Classe: {GetClassDisplayName(talent.requiredClass)}\n";
        
        if (talent.prerequisites.Count > 0)
        {
            requirementsText += "Prérequis: ";
            foreach (string prereqId in talent.prerequisites)
            {
                var prereqTalent = talentNodes.Find(n => n.GetTalent().id == prereqId)?.GetTalent();
                if (prereqTalent != null)
                {
                    requirementsText += $"{prereqTalent.name} ";
                }
            }
            requirementsText += "\n";
        }
        
        talentRequirementsText.text = requirementsText;
        
        // Effects
        string effectsText = "";
        
        if (talent.statBonuses.Count > 0)
        {
            effectsText += "Bonus de stats:\n";
            foreach (var bonus in talent.statBonuses)
            {
                effectsText += $"+{bonus.Value} {bonus.Key}\n";
            }
        }
        
        if (talent.statMultipliers.Count > 0)
        {
            effectsText += "Multiplicateurs:\n";
            foreach (var multiplier in talent.statMultipliers)
            {
                effectsText += $"+{multiplier.Value * 100:F1}% {multiplier.Key}\n";
            }
        }
        
        if (talent.specialAbilities.Count > 0)
        {
            effectsText += "Capacités spéciales:\n";
            foreach (var ability in talent.specialAbilities)
            {
                effectsText += $"• {ability}\n";
            }
        }
        
        talentEffectsText.text = effectsText;
        
        // Buttons
        bool canUnlock = talent.CanUnlock(currentAdventurer, GetAllTalentsForClass(currentClass));
        bool canUpgrade = talent.unlocked && talent.currentRank < talent.maxRank;
        
        unlockTalentButton.gameObject.SetActive(!talent.unlocked && canUnlock);
        upgradeTalentButton.gameObject.SetActive(canUpgrade);
        
        unlockTalentButton.interactable = canUnlock;
        upgradeTalentButton.interactable = canUpgrade;
        
        // Talent icon
        // talentIcon.sprite = GetTalentIcon(talent.id);
    }
    
    private void HighlightTalentConnections(TalentNode talent)
    {
        // Highlight prerequisite connections
        foreach (var line in connectionLines)
        {
            line.material = new Material(Shader.Find("Sprites/Default"));
        line.material.color = connectionColor;
        }
        
        // Highlight connections related to this talent
        // This would involve checking which lines connect to this talent
    }
    
    private void UnlockSelectedTalent()
    {
        if (selectedTalent != null && currentAdventurer != null)
        {
            bool success = ProgressionSystem.Instance.UnlockTalent(currentAdventurer, selectedTalent.id);
            
            if (success)
            {
                OnTalentUnlocked?.Invoke(selectedTalent);
                UpdateTalentTree();
                UpdateSkillPointsDisplay();
                ShowTalentDetails(selectedTalent); // Refresh details
            }
        }
    }
    
    private void UpgradeSelectedTalent()
    {
        if (selectedTalent != null && currentAdventurer != null)
        {
            bool success = ProgressionSystem.Instance.UnlockTalent(currentAdventurer, selectedTalent.id);
            
            if (success)
            {
                OnTalentUnlocked?.Invoke(selectedTalent);
                UpdateTalentTree();
                UpdateSkillPointsDisplay();
                ShowTalentDetails(selectedTalent); // Refresh details
            }
        }
    }
    
    private void ResetTalents()
    {
        // Reset all talents for current adventurer
        // This would interface with the progression system
        Debug.Log($"Resetting talents for {currentAdventurer.Name}");
        UpdateTalentTree();
        UpdateSkillPointsDisplay();
    }
    
    private void OnClassChanged(int classIndex)
    {
        currentClass = (Adventurer.AdventurerClass)classIndex;
        UpdateTalentTree();
    }
    
    private void UpdateSkillPointsDisplay()
    {
        if (currentAdventurer == null) return;
        
        skillPointsText.text = $"Points de compétence: {currentAdventurer.SkillPoints}";
        
        // Count unlocked talents
        int unlockedCount = 0;
        var talents = GetAllTalentsForClass(currentClass);
        foreach (var talent in talents)
        {
            if (talent.unlocked) unlockedCount++;
        }
        
        unlockedTalentsText.text = $"Talents débloqués: {unlockedCount}/{talents.Count}";
    }
    
    private void UpdateSpecializationPanel()
    {
        if (currentAdventurer == null) return;
        
        var availableSpecs = ProgressionSystem.Instance.GetAvailableSpecializations(currentAdventurer);
        
        // Clear existing specialization items
        foreach (Transform child in specializationContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Create specialization items
        foreach (var spec in availableSpecs)
        {
            CreateSpecializationItem(spec);
        }
        
        // Show/hide specialization button
        specializationButton.gameObject.SetActive(availableSpecs.Count > 0);
    }
    
    private void CreateSpecializationItem(SpecializationPath specialization)
    {
        var specObject = Instantiate(specializationItemPrefab, specializationContainer);
        var specUI = specObject.GetComponent<SpecializationItemUI>();
        
        if (specUI != null)
        {
            specUI.SetSpecialization(specialization);
            specUI.OnSpecializationClicked += OnSpecializationClicked;
        }
    }
    
    private void OnSpecializationClicked(SpecializationPath specialization)
    {
        bool success = ProgressionSystem.Instance.UnlockSpecialization(currentAdventurer, specialization.id);
        
        if (success)
        {
            Debug.Log($"Unlocked specialization: {specialization.name}");
            UpdateSpecializationPanel();
        }
    }
    
    private void ToggleSpecializationPanel()
    {
        specializationPanel.SetActive(!specializationPanel.activeSelf);
    }
    
    private List<TalentNode> GetAllTalentsForClass(Adventurer.AdventurerClass adventurerClass)
    {
        // This would get all talents for the class from ProgressionSystem
        // For now, return empty list
        return new List<TalentNode>();
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
    
    public void RefreshTalentTree()
    {
        UpdateTalentTree();
        UpdateSkillPointsDisplay();
        UpdateSpecializationPanel();
    }
    
    public TalentNode GetSelectedTalent()
    {
        return selectedTalent;
    }
    
    public void HideTalentDetails()
    {
        talentDetailsPanel.SetActive(false);
        selectedTalent = null;
    }
}