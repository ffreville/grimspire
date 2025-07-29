using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class ExpeditionItem : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI difficultyText;
    [SerializeField] private TextMeshProUGUI durationText;
    [SerializeField] private TextMeshProUGUI adventurersText;
    [SerializeField] private TextMeshProUGUI locationText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Button actionButton;
    [SerializeField] private Image difficultyIcon;
    [SerializeField] private Slider progressSlider;

    [Header("Difficulty Colors")]
    [SerializeField] private Color easyColor = Color.green;
    [SerializeField] private Color mediumColor = Color.yellow;
    [SerializeField] private Color hardColor = Color.red;
    [SerializeField] private Color extremeColor = Color.magenta;

    private Expedition expedition;
    private ExpeditionMenu parentMenu;

    public void Initialize(Expedition exp, ExpeditionMenu menu)
    {
        expedition = exp;
        parentMenu = menu;
        RefreshDisplay();
        SetupButton();
    }

    private void RefreshDisplay()
    {
        if (expedition == null) return;

        nameText.text = expedition.name;
        descriptionText.text = expedition.description;
        difficultyText.text = expedition.difficulty.ToString();
        durationText.text = $"{expedition.duration}h";
        locationText.text = expedition.location;

        // Couleur de difficulté
        Color diffColor = GetDifficultyColor(expedition.difficulty);
        difficultyText.color = diffColor;
        if (difficultyIcon != null)
            difficultyIcon.color = diffColor;

        // Information sur les aventuriers
        string adventurerInfo = expedition.status == ExpeditionStatus.Available 
            ? $"{expedition.assignedAdventurerIds.Count}/{expedition.minAdventurers}-{expedition.maxAdventurers}"
            : $"{expedition.assignedAdventurerIds.Count} aventuriers";
        adventurersText.text = adventurerInfo;

        // Statut et progression
        UpdateStatusAndProgress();
    }

    private void UpdateStatusAndProgress()
    {
        switch (expedition.status)
        {
            case ExpeditionStatus.Available:
                statusText.text = "Disponible";
                statusText.color = Color.white;
                if (progressSlider != null) progressSlider.gameObject.SetActive(false);
                if (progressText != null) progressText.gameObject.SetActive(false);
                break;

            case ExpeditionStatus.InProgress:
                statusText.text = "En cours";
                statusText.color = Color.cyan;
                if (progressSlider != null)
                {
                    progressSlider.gameObject.SetActive(true);
                    progressSlider.value = expedition.GetProgressPercentage();
                }
                if (progressText != null)
                {
                    progressText.gameObject.SetActive(true);
                    progressText.text = expedition.GetFormattedTimeRemaining();
                }
                break;

            case ExpeditionStatus.Completed:
                statusText.text = "Terminée";
                statusText.color = Color.green;
                if (progressSlider != null) progressSlider.gameObject.SetActive(false);
                if (progressText != null)
                {
                    progressText.gameObject.SetActive(true);
                    progressText.text = "Récompenses disponibles";
                }
                break;

            case ExpeditionStatus.Failed:
                statusText.text = "Échouée";
                statusText.color = Color.red;
                if (progressSlider != null) progressSlider.gameObject.SetActive(false);
                if (progressText != null) progressText.gameObject.SetActive(false);
                break;
        }
    }

    private Color GetDifficultyColor(ExpeditionDifficulty difficulty)
    {
        return difficulty switch
        {
            ExpeditionDifficulty.Easy => easyColor,
            ExpeditionDifficulty.Medium => mediumColor,
            ExpeditionDifficulty.Hard => hardColor,
            ExpeditionDifficulty.Extreme => extremeColor,
            _ => Color.white
        };
    }

    private void SetupButton()
    {
        if (actionButton == null) return;

        actionButton.onClick.RemoveAllListeners();

        switch (expedition.status)
        {
            case ExpeditionStatus.Available:
                actionButton.GetComponentInChildren<TextMeshProUGUI>().text = "Sélectionner";
                actionButton.onClick.AddListener(() => parentMenu.SelectExpedition(expedition));
                actionButton.interactable = true;
                break;

            case ExpeditionStatus.InProgress:
                actionButton.GetComponentInChildren<TextMeshProUGUI>().text = "En cours...";
                actionButton.interactable = false;
                break;

            case ExpeditionStatus.Completed:
                actionButton.GetComponentInChildren<TextMeshProUGUI>().text = "Collecter";
                actionButton.onClick.AddListener(() => parentMenu.CollectRewards(expedition));
                actionButton.interactable = true;
                break;

            case ExpeditionStatus.Failed:
                actionButton.GetComponentInChildren<TextMeshProUGUI>().text = "Échouée";
                actionButton.interactable = false;
                break;
        }
    }

    private void Update()
    {
        // Mettre à jour la progression pour les expéditions en cours
        if (expedition != null && expedition.status == ExpeditionStatus.InProgress)
        {
            UpdateStatusAndProgress();
        }
    }

    public Expedition GetExpedition()
    {
        return expedition;
    }
}