using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExpeditionMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform availableExpeditionsContent;
    [SerializeField] private Transform activeExpeditionsContent;
    [SerializeField] private Transform completedExpeditionsContent;
    [SerializeField] private GameObject expeditionItemPrefab;
    
    [Header("Selected Expedition Panel")]
    [SerializeField] private GameObject selectedExpeditionPanel;
    [SerializeField] private TextMeshProUGUI selectedExpeditionName;
    [SerializeField] private TextMeshProUGUI selectedExpeditionDescription;
    [SerializeField] private TextMeshProUGUI selectedExpeditionDetails;
    [SerializeField] private Transform availableAdventurersContent;
    [SerializeField] private Transform assignedAdventurersContent;
    [SerializeField] private GameObject adventurerItemPrefab;
    [SerializeField] private Button startExpeditionButton;
    [SerializeField] private Button cancelSelectionButton;
    
    [Header("Controls")]
    [SerializeField] private Button generateNewExpeditionsButton;
    [SerializeField] private TextMeshProUGUI cityResourcesText;

    private City currentCity;
    private ExpeditionSystem expeditionSystem;
    private Expedition selectedExpedition;

    private void Awake()
    {
        expeditionSystem = new ExpeditionSystem();
        SetupButtons();
    }

    private void SetupButtons()
    {
        if (generateNewExpeditionsButton != null)
        {
            generateNewExpeditionsButton.onClick.AddListener(() => {
                expeditionSystem.GenerateNewExpeditions(2);
                RefreshExpeditionLists();
            });
        }

        if (startExpeditionButton != null)
        {
            startExpeditionButton.onClick.AddListener(StartSelectedExpedition);
        }

        if (cancelSelectionButton != null)
        {
            cancelSelectionButton.onClick.AddListener(ClearSelection);
        }
    }

    public void SetCity(City city)
    {
        currentCity = city;
        RefreshExpeditionLists();
        UpdateResourcesDisplay();
    }

    public void RefreshExpeditionLists()
    {
        if (currentCity == null) return;

        RefreshExpeditionList(availableExpeditionsContent, expeditionSystem.GetAvailableExpeditions());
        RefreshExpeditionList(activeExpeditionsContent, expeditionSystem.GetActiveExpeditions());
        RefreshExpeditionList(completedExpeditionsContent, expeditionSystem.GetCompletedExpeditions());
        
        UpdateResourcesDisplay();
    }

    private void RefreshExpeditionList(Transform container, List<Expedition> expeditions)
    {
        if (container == null) return;

        // Nettoyer les éléments existants
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        // Créer les nouveaux éléments
        foreach (var expedition in expeditions)
        {
            CreateExpeditionItem(expedition, container);
        }
    }

    private void CreateExpeditionItem(Expedition expedition, Transform parent)
    {
        if (expeditionItemPrefab == null) return;

        GameObject itemObj = Instantiate(expeditionItemPrefab, parent);
        ExpeditionItem expeditionItem = itemObj.GetComponent<ExpeditionItem>();
        
        if (expeditionItem != null)
        {
            expeditionItem.Initialize(expedition, this);
        }
    }

    public void SelectExpedition(Expedition expedition)
    {
        selectedExpedition = expedition;
        ShowExpeditionDetails();
    }

    private void ShowExpeditionDetails()
    {
        if (selectedExpedition == null || selectedExpeditionPanel == null) return;

        selectedExpeditionPanel.SetActive(true);
        
        selectedExpeditionName.text = selectedExpedition.name;
        selectedExpeditionDescription.text = selectedExpedition.description;
        
        string details = $"Difficulté: {selectedExpedition.difficulty}\n" +
                        $"Durée: {selectedExpedition.duration}h\n" +
                        $"Aventuriers requis: {selectedExpedition.minAdventurers}-{selectedExpedition.maxAdventurers}\n" +
                        $"Lieu: {selectedExpedition.location}\n" +
                        $"Expérience: {selectedExpedition.experienceReward} XP";
        selectedExpeditionDetails.text = details;

        RefreshAdventurerLists();
        UpdateStartButton();
    }

    private void RefreshAdventurerLists()
    {
        if (currentCity == null || selectedExpedition == null) return;

        // Aventuriers disponibles
        RefreshAvailableAdventurersList();
        
        // Aventuriers assignés
        RefreshAssignedAdventurersList();
    }

    private void RefreshAvailableAdventurersList()
    {
        if (availableAdventurersContent == null) return;

        // Nettoyer
        foreach (Transform child in availableAdventurersContent)
        {
            Destroy(child.gameObject);
        }

        var availableAdventurers = expeditionSystem.GetAvailableAdventurers(currentCity.GetAdventurers())
            .Where(a => !selectedExpedition.assignedAdventurerIds.Contains(a.id)).ToList();

        foreach (var adventurer in availableAdventurers)
        {
            CreateAdventurerItem(adventurer, availableAdventurersContent, true);
        }
    }

    private void RefreshAssignedAdventurersList()
    {
        if (assignedAdventurersContent == null) return;

        // Nettoyer
        foreach (Transform child in assignedAdventurersContent)
        {
            Destroy(child.gameObject);
        }

        var assignedAdventurers = currentCity.GetAdventurers()
            .Where(a => selectedExpedition.assignedAdventurerIds.Contains(a.id)).ToList();

        foreach (var adventurer in assignedAdventurers)
        {
            CreateAdventurerItem(adventurer, assignedAdventurersContent, false);
        }
    }

    private void CreateAdventurerItem(Adventurer adventurer, Transform parent, bool isAvailable)
    {
        if (adventurerItemPrefab == null) return;

        GameObject itemObj = Instantiate(adventurerItemPrefab, parent);
        AdventurerItem adventurerItem = itemObj.GetComponent<AdventurerItem>();
        
        if (adventurerItem != null)
        {
            adventurerItem.Setup(adventurer, true, null); // true car l'aventurier est recruté
            
            // Ajouter un bouton pour assigner/retirer
            Button actionButton = itemObj.GetComponentInChildren<Button>();
            if (actionButton != null)
            {
                actionButton.onClick.RemoveAllListeners();
                
                if (isAvailable)
                {
                    actionButton.GetComponentInChildren<TextMeshProUGUI>().text = "Assigner";
                    actionButton.onClick.AddListener(() => AssignAdventurer(adventurer.id));
                }
                else
                {
                    actionButton.GetComponentInChildren<TextMeshProUGUI>().text = "Retirer";
                    actionButton.onClick.AddListener(() => RemoveAdventurer(adventurer.id));
                }
            }
        }
    }

    private void AssignAdventurer(string adventurerId)
    {
        if (selectedExpedition != null && currentCity != null)
        {
            bool success = expeditionSystem.AssignAdventurerToExpedition(
                selectedExpedition.id, adventurerId, currentCity.GetAdventurers());
            
            if (success)
            {
                RefreshAdventurerLists();
                UpdateStartButton();
            }
        }
    }

    private void RemoveAdventurer(string adventurerId)
    {
        if (selectedExpedition != null)
        {
            bool success = expeditionSystem.RemoveAdventurerFromExpedition(
                selectedExpedition.id, adventurerId);
            
            if (success)
            {
                RefreshAdventurerLists();
                UpdateStartButton();
            }
        }
    }

    private void UpdateStartButton()
    {
        if (startExpeditionButton == null || selectedExpedition == null) return;

        bool canStart = selectedExpedition.CanStartExpedition();
        startExpeditionButton.interactable = canStart;
        
        TextMeshProUGUI buttonText = startExpeditionButton.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            if (canStart)
            {
                buttonText.text = "Lancer l'Expédition";
            }
            else
            {
                int assigned = selectedExpedition.assignedAdventurerIds.Count;
                int min = selectedExpedition.minAdventurers;
                buttonText.text = $"Besoin de {min - assigned} aventurier(s) de plus";
            }
        }
    }

    private void StartSelectedExpedition()
    {
        if (selectedExpedition != null)
        {
            bool success = expeditionSystem.StartExpedition(selectedExpedition.id);
            
            if (success)
            {
                Debug.Log($"Expédition '{selectedExpedition.name}' lancée!");
                ClearSelection();
                RefreshExpeditionLists();
            }
        }
    }

    private void ClearSelection()
    {
        selectedExpedition = null;
        if (selectedExpeditionPanel != null)
        {
            selectedExpeditionPanel.SetActive(false);
        }
    }

    public void CollectRewards(Expedition expedition)
    {
        if (currentCity != null && expedition.status == ExpeditionStatus.Completed)
        {
            expeditionSystem.CollectExpeditionRewards(expedition.id, currentCity);
            RefreshExpeditionLists();
            UpdateResourcesDisplay();
        }
    }

    private void UpdateResourcesDisplay()
    {
        if (cityResourcesText != null && currentCity != null)
        {
            string resourcesText = $"Or: {currentCity.GetResourceAmount(ResourceType.Gold)} | " +
                                 $"Pop: {currentCity.GetResourceAmount(ResourceType.Population)} | " +
                                 $"Mat: {currentCity.GetResourceAmount(ResourceType.Materials)} | " +
                                 $"Cristaux: {currentCity.GetResourceAmount(ResourceType.MagicCrystals)}";
            cityResourcesText.text = resourcesText;
        }
    }

    private void Update()
    {
        // Mettre à jour régulièrement les listes d'expéditions
        if (Time.frameCount % 60 == 0) // Chaque seconde environ
        {
            RefreshExpeditionLists();
        }
    }
}