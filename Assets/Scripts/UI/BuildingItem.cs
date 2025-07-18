using UnityEngine;
using UnityEngine.UI;

public class BuildingItem : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Text nameText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Text costText;
    [SerializeField] private Text statusText;
    [SerializeField] private Button actionButton;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image iconImage;

    [Header("Colors")]
    [SerializeField] private Color builtColor = new(0.2f, 0.6f, 0.2f, 1f);
    [SerializeField] private Color constructionColor = new(0.6f, 0.6f, 0.2f, 1f);
    [SerializeField] private Color availableColor = new(0.3f, 0.3f, 0.3f, 1f);
    [SerializeField] private Color unavailableColor = new(0.5f, 0.2f, 0.2f, 1f);

    private Building building;
    private bool isBuilt;
    private BuildingMenu parentMenu;

    public void Setup(Building buildingData, bool built, BuildingMenu menu)
    {
        building = buildingData;
        isBuilt = built;
        parentMenu = menu;
        
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (building == null) return;

        // Update name
        if (nameText != null)
            nameText.text = building.name;

        // Update description
        if (descriptionText != null)
            descriptionText.text = building.description;

        // Update cost
        if (costText != null)
            UpdateCostText();

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

    private void UpdateCostText()
    {
        if (building.isBuilt && building.CanUpgrade(parentMenu.CurrentCity))
        {
            costText.text = $"Amélioration: {building.upgradeGoldCost} or";
            if (building.upgradeWoodCost > 0) costText.text += $", {building.upgradeWoodCost} bois";
            if (building.upgradeStoneCost > 0) costText.text += $", {building.upgradeStoneCost} pierre";
            if (building.upgradeIronCost > 0) costText.text += $", {building.upgradeIronCost} fer";
            if (building.upgradeMagicCost > 0) costText.text += $", {building.upgradeMagicCost} cristaux";
        }
        else if (!building.isBuilt && !building.isUnderConstruction)
        {
            costText.text = $"Construction: {building.goldCost} or";
            if (building.woodCost > 0) costText.text += $", {building.woodCost} bois";
            if (building.stoneCost > 0) costText.text += $", {building.stoneCost} pierre";
            if (building.ironCost > 0) costText.text += $", {building.ironCost} fer";
            if (building.magicCost > 0) costText.text += $", {building.magicCost} cristaux";
        }
        else
        {
            costText.text = "";
        }
    }

    private void UpdateStatusText()
    {
        if (building.isBuilt)
        {
            statusText.text = $"Niveau {building.level}/{building.maxLevel} - Construit";
        }
        else if (building.isUnderConstruction)
        {
            statusText.text = $"Construction: {building.constructionTimeRemaining} jour(s)";
        }
        else
        {
            statusText.text = "Disponible à la construction";
        }
    }

    private void UpdateActionButton()
    {
        if (building.isBuilt)
        {
            actionButton.GetComponentInChildren<Text>().text = building.CanUpgrade(parentMenu.CurrentCity) ? "Améliorer" : "Détails";
            actionButton.interactable = true;
        }
        else if (building.isUnderConstruction)
        {
            actionButton.GetComponentInChildren<Text>().text = "En construction";
            actionButton.interactable = false;
        }
        else
        {
            actionButton.GetComponentInChildren<Text>().text = "Construire";
            actionButton.interactable = building.CanBuild(parentMenu.CurrentCity);
        }

        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(OnActionButtonClicked);
    }

    private void UpdateBackgroundColor()
    {
        if (building.isBuilt)
        {
            backgroundImage.color = builtColor;
        }
        else if (building.isUnderConstruction)
        {
            backgroundImage.color = constructionColor;
        }
        else if (building.CanBuild(parentMenu.CurrentCity))
        {
            backgroundImage.color = availableColor;
        }
        else
        {
            backgroundImage.color = unavailableColor;
        }
    }

    private void OnActionButtonClicked()
    {
        if (parentMenu == null || building == null) return;

        if (building.isBuilt && building.CanUpgrade(parentMenu.CurrentCity))
        {
            parentMenu.OnUpgradeClicked(building);
        }
        else
        {
            parentMenu.OnBuildingClicked(building);
        }
    }
}
