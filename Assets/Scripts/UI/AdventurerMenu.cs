using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AdventurerMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject recruitedListContainer;
    [SerializeField] private GameObject availableListContainer;
    [SerializeField] private GameObject adventurerItemPrefab;
    [SerializeField] private Button searchButton;
    [SerializeField] private ScrollRect recruitedScrollRect;
    [SerializeField] private ScrollRect availableScrollRect;
    
    [Header("Current Data")]
    [SerializeField] private City currentCity;
    [SerializeField] private List<Adventurer> availableAdventurers;
    [SerializeField] private List<GameObject> recruitedUIItems;
    [SerializeField] private List<GameObject> availableUIItems;
    
    [Header("Search Settings")]
    [SerializeField] private int maxSearchResults = 10;
    [SerializeField] private int searchCost = 50;

    public City CurrentCity => currentCity;
    private AdventurerSystem adventurerSystem;

    private void Awake()
    {
        availableAdventurers = new List<Adventurer>();
        recruitedUIItems = new List<GameObject>();
        availableUIItems = new List<GameObject>();
        
        SetupSearchButton();
    }

    private void OnEnable()
    {
        // Get city from parent GameMenu if not set
        if (currentCity == null)
        {
            GameMenu gameMenu = GetComponentInParent<GameMenu>();
            if (gameMenu != null)
            {
                currentCity = gameMenu.GetCurrentCity();
            }
        }
        
        // Get adventurer system
        if (adventurerSystem == null)
        {
            adventurerSystem = FindObjectOfType<AdventurerSystem>();
        }
        
        RefreshAdventurerLists();
    }

    private void SetupSearchButton()
    {
        if (searchButton != null)
        {
            searchButton.onClick.AddListener(OnSearchButtonClicked);
        }
    }

    public void SetCity(City city)
    {
        currentCity = city;
        RefreshAdventurerLists();
    }

    public void RefreshAdventurerLists()
    {
        RefreshRecruitedList();
        RefreshAvailableList();
        UpdateSearchButton();
    }

    private void RefreshRecruitedList()
    {
        ClearRecruitedList();
        
        if (currentCity == null) return;

        Debug.Log($"Refreshing recruited list. City has {currentCity.adventurers.Count} adventurers");

        foreach (Adventurer adventurer in currentCity.adventurers)
        {
            CreateAdventurerItem(adventurer, true, recruitedListContainer);
        }
    }

    private void RefreshAvailableList()
    {
        ClearAvailableList();
        
        Debug.Log($"Refreshing available list. {availableAdventurers.Count} adventurers available");

        foreach (Adventurer adventurer in availableAdventurers)
        {
            CreateAdventurerItem(adventurer, false, availableListContainer);
        }
    }

    private void ClearRecruitedList()
    {
        foreach (GameObject item in recruitedUIItems)
        {
            if (item != null)
            {
                DestroyImmediate(item);
            }
        }
        recruitedUIItems.Clear();
    }

    private void ClearAvailableList()
    {
        foreach (GameObject item in availableUIItems)
        {
            if (item != null)
            {
                DestroyImmediate(item);
            }
        }
        availableUIItems.Clear();
    }

    private void CreateAdventurerItem(Adventurer adventurer, bool isRecruited, GameObject container)
    {
        if (adventurerItemPrefab == null || container == null) return;

        GameObject itemGO = Instantiate(adventurerItemPrefab, container.transform);
        itemGO.SetActive(true);
        
        if (isRecruited)
        {
            recruitedUIItems.Add(itemGO);
        }
        else
        {
            availableUIItems.Add(itemGO);
        }

        AdventurerItem adventurerItem = itemGO.GetComponent<AdventurerItem>();
        if (adventurerItem != null)
        {
            adventurerItem.Setup(adventurer, isRecruited, this);
        }
    }

    private void UpdateSearchButton()
    {
        if (searchButton == null) return;

        bool canSearch = currentCity != null && 
                        currentCity.gold >= searchCost && 
                        currentCity.CanPerformAction();

        searchButton.interactable = canSearch;
        
        Text buttonText = searchButton.GetComponentInChildren<Text>();
        if (buttonText != null)
        {
            buttonText.text = $"Rechercher ({searchCost} or)";
        }
    }

    private void OnSearchButtonClicked()
    {
        if (currentCity == null || adventurerSystem == null) return;

        if (!currentCity.CanPerformAction())
        {
            Debug.Log("Pas assez de points d'action pour rechercher!");
            return;
        }

        if (currentCity.gold < searchCost)
        {
            Debug.Log("Pas assez d'or pour rechercher!");
            return;
        }

        // Use adventurer system to search
        List<Adventurer> newAdventurers = adventurerSystem.SearchForAdventurers(maxSearchResults);
        
        // Spend resources
        currentCity.SpendResources(searchCost);
        currentCity.SpendActionPoint();
        
        // Update available list
        availableAdventurers.Clear();
        availableAdventurers.AddRange(newAdventurers);
        
        Debug.Log($"Recherche terminée! {newAdventurers.Count} aventuriers trouvés.");
        
        RefreshAvailableList();
        UpdateSearchButton();
    }

    public void OnRecruitAdventurer(Adventurer adventurer)
    {
        if (currentCity == null || adventurerSystem == null) return;

        if (currentCity.adventurers.Count >= currentCity.maxAdventurers)
        {
            Debug.Log("Capacité maximale d'aventuriers atteinte!");
            return;
        }

        int recruitCost = adventurerSystem.GetRecruitmentCost(adventurer);
        
        if (currentCity.gold < recruitCost)
        {
            Debug.Log($"Pas assez d'or pour recruter {adventurer.name}! Coût: {recruitCost}");
            return;
        }

        if (!currentCity.CanPerformAction())
        {
            Debug.Log("Pas assez de points d'action pour recruter!");
            return;
        }

        // Recruit the adventurer
        bool success = adventurerSystem.RecruitAdventurer(adventurer, currentCity);
        
        if (success)
        {
            currentCity.SpendResources(recruitCost);
            currentCity.SpendActionPoint();
            
            // Remove from available list
            availableAdventurers.Remove(adventurer);
            
            Debug.Log($"{adventurer.name} recruté avec succès!");
            
            RefreshAdventurerLists();
        }
    }

    public void OnAdventurerClicked(Adventurer adventurer, bool isRecruited)
    {
        if (isRecruited)
        {
            ShowAdventurerDetails(adventurer);
        }
        else
        {
            OnRecruitAdventurer(adventurer);
        }
    }

    private void ShowAdventurerDetails(Adventurer adventurer)
    {
        string details = $"=== {adventurer.name} ===\n";
        details += $"Classe: {adventurer.adventurerClass}\n";
        details += $"Niveau: {adventurer.level}\n";
        details += $"Expérience: {adventurer.experience}/{adventurer.experienceToNextLevel}\n\n";
        
        details += $"=== Statistiques ===\n";
        details += $"Force: {adventurer.strength}\n";
        details += $"Intelligence: {adventurer.intelligence}\n";
        details += $"Agilité: {adventurer.agility}\n";
        details += $"Charisme: {adventurer.charisma}\n";
        details += $"Chance: {adventurer.luck}\n\n";
        
        details += $"=== Santé ===\n";
        details += $"PV: {adventurer.currentHealth}/{adventurer.maxHealth}\n";
        details += $"Statut: {(adventurer.isAvailable ? "Disponible" : "Indisponible")}\n";
        
        if (adventurer.isInjured)
        {
            details += $"Blessé - Récupération: {adventurer.recoveryDays} jours\n";
        }
        
        details += $"\n=== Équipement ===\n";
        details += $"Arme: {(adventurer.weapon?.name ?? "Aucune")}\n";
        details += $"Armure: {(adventurer.armor?.name ?? "Aucune")}\n";
        details += $"Accessoire: {(adventurer.accessory?.name ?? "Aucun")}\n";
        
        details += $"\nPuissance totale: {adventurer.GetTotalPower()}";
        
        Debug.Log(details);
    }
}
