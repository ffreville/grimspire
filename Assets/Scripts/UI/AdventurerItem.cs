using UnityEngine;
using UnityEngine.UI;

public class AdventurerItem : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Text nameText;
    [SerializeField] private Text classText;
    [SerializeField] private Text levelText;
    [SerializeField] private Text statsText;
    [SerializeField] private Text statusText;
    [SerializeField] private Button actionButton;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image classIcon;

    [Header("Colors")]
    [SerializeField] private Color recruitedColor = new(0.2f, 0.5f, 0.2f, 1f);
    [SerializeField] private Color availableColor = new(0.3f, 0.3f, 0.3f, 1f);
    [SerializeField] private Color injuredColor = new(0.6f, 0.2f, 0.2f, 1f);
    [SerializeField] private Color onMissionColor = new(0.2f, 0.2f, 0.6f, 1f);

    private Adventurer adventurer;
    private bool isRecruited;
    private AdventurerMenu parentMenu;

    public void Setup(Adventurer adventurerData, bool recruited, AdventurerMenu menu)
    {
        adventurer = adventurerData;
        isRecruited = recruited;
        parentMenu = menu;
        
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (adventurer == null) return;

        // Update name
        if (nameText != null)
            nameText.text = adventurer.name;

        // Update class
        if (classText != null)
            classText.text = GetClassDisplayName(adventurer.adventurerClass);

        // Update level
        if (levelText != null)
            levelText.text = $"Niv. {adventurer.level}";

        // Update stats
        if (statsText != null)
            UpdateStatsText();

        // Update status
        if (statusText != null)
            UpdateStatusText();

        // Update button
        if (actionButton != null)
            UpdateActionButton();

        // Update background color
        if (backgroundImage != null)
            UpdateBackgroundColor();
    }

    private void UpdateStatsText()
    {
        string stats = $"FOR: {adventurer.strength} | INT: {adventurer.intelligence} | AGI: {adventurer.agility}\n";
        stats += $"CHA: {adventurer.charisma} | CHC: {adventurer.luck}";
        
        if (isRecruited)
        {
            stats += $"\nPV: {adventurer.currentHealth}/{adventurer.maxHealth}";
            stats += $"\nPuissance: {adventurer.GetTotalPower()}";
        }
        
        statsText.text = stats;
    }

    private void UpdateStatusText()
    {
        if (isRecruited)
        {
            if (adventurer.isOnMission)
            {
                statusText.text = "En mission";
                statusText.color = Color.cyan;
            }
            else if (adventurer.isInjured)
            {
                statusText.text = $"Blessé ({adventurer.recoveryDays}j)";
                statusText.color = Color.red;
            }
            else if (adventurer.isAvailable)
            {
                statusText.text = "Disponible";
                statusText.color = Color.green;
            }
            else
            {
                statusText.text = "Indisponible";
                statusText.color = Color.yellow;
            }
        }
        else
        {
            // Show recruitment cost
            if (parentMenu != null && parentMenu.CurrentCity != null)
            {
                AdventurerSystem system = FindObjectOfType<AdventurerSystem>();
                if (system != null)
                {
                    int cost = system.GetRecruitmentCost(adventurer);
                    statusText.text = $"Coût: {cost} or";
                    statusText.color = Color.white;
                }
            }
        }
    }

    private void UpdateActionButton()
    {
        if (actionButton == null) return;

        Text buttonText = actionButton.GetComponentInChildren<Text>();
        if (buttonText != null)
        {
            if (isRecruited)
            {
                buttonText.text = "Détails";
                actionButton.interactable = true;
            }
            else
            {
                buttonText.text = "Recruter";
                
                // Check if can recruit
                bool canRecruit = parentMenu != null && 
                                parentMenu.CurrentCity != null && 
                                parentMenu.CurrentCity.CanPerformAction();
                
                if (canRecruit && parentMenu.CurrentCity.adventurers.Count >= parentMenu.CurrentCity.maxAdventurers)
                {
                    canRecruit = false;
                    buttonText.text = "Plein";
                }
                Debug.Log("Can recruit :" + canRecruit);  
                actionButton.interactable = canRecruit;
            }
        }

        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(OnActionButtonClicked);
    }

    private void UpdateBackgroundColor()
    {
        if (isRecruited)
        {
            if (adventurer.isOnMission)
            {
                backgroundImage.color = onMissionColor;
            }
            else if (adventurer.isInjured)
            {
                backgroundImage.color = injuredColor;
            }
            else
            {
                backgroundImage.color = recruitedColor;
            }
        }
        else
        {
            backgroundImage.color = availableColor;
        }
    }

    private string GetClassDisplayName(AdventurerClass adventurerClass)
    {
        switch (adventurerClass)
        {
            case AdventurerClass.Warrior:
                return "Guerrier";
            case AdventurerClass.Mage:
                return "Mage";
            case AdventurerClass.Rogue:
                return "Voleur";
            case AdventurerClass.Cleric:
                return "Clerc";
            case AdventurerClass.Ranger:
                return "Rôdeur";
            default:
                return adventurerClass.ToString();
        }
    }

    private void OnActionButtonClicked()
    {
        if (parentMenu != null && adventurer != null)
        {
            parentMenu.OnAdventurerClicked(adventurer, isRecruited);
        }
    }
}
