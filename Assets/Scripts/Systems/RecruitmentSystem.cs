using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RecruitmentSystem : MonoBehaviour
{
    public static RecruitmentSystem Instance { get; private set; }
    
    [Header("Recruitment Settings")]
    [SerializeField] private int maxAvailableAdventurers = 6;
    [SerializeField] private float recruitmentRefreshTime = 24f; // Game hours
    [SerializeField] private int baseRecruitmentCost = 100;
    [SerializeField] private float rarityMultiplier = 2f;
    
    [Header("Generation Probabilities")]
    [SerializeField] private float commonProbability = 0.5f;
    [SerializeField] private float uncommonProbability = 0.3f;
    [SerializeField] private float rareProbability = 0.15f;
    [SerializeField] private float epicProbability = 0.04f;
    [SerializeField] private float legendaryProbability = 0.01f;
    
    [Header("Name Generation")]
    [SerializeField] private string[] firstNames;
    [SerializeField] private string[] lastNames;
    [SerializeField] private string[] titles;
    
    [Header("Debug")]
    [SerializeField] private bool debugMode = false;
    
    private List<Adventurer> availableAdventurers;
    private float lastRefreshTime;
    private int totalAdventurersGenerated;
    
    public List<Adventurer> AvailableAdventurers => new List<Adventurer>(availableAdventurers);
    public int AvailableCount => availableAdventurers.Count;
    public bool CanRefresh => Time.time - lastRefreshTime >= recruitmentRefreshTime;
    public float TimeUntilRefresh => Mathf.Max(0, recruitmentRefreshTime - (Time.time - lastRefreshTime));
    
    // Events
    public static event Action<List<Adventurer>> OnRecruitmentRefreshed;
    public static event Action<Adventurer> OnAdventurerRecruited;
    public static event Action<Adventurer> OnAdventurerDismissed;
    public static event Action<string> OnRecruitmentError;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeRecruitmentSystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeRecruitmentSystem()
    {
        availableAdventurers = new List<Adventurer>();
        lastRefreshTime = 0f;
        totalAdventurersGenerated = 0;
        
        InitializeNameDatabase();
        GenerateInitialAdventurers();
        
        if (debugMode)
        {
            Debug.Log("RecruitmentSystem initialized");
        }
    }

    private void InitializeNameDatabase()
    {
        if (firstNames == null || firstNames.Length == 0)
        {
            firstNames = new string[]
            {
                "Aiden", "Lyra", "Gareth", "Sera", "Thane", "Mira", "Kael", "Zara",
                "Bjorn", "Elara", "Darius", "Nova", "Orion", "Luna", "Ragnar", "Iris",
                "Caden", "Vera", "Draven", "Aria", "Magnus", "Nyx", "Atlas", "Echo"
            };
        }
        
        if (lastNames == null || lastNames.Length == 0)
        {
            lastNames = new string[]
            {
                "Ironforge", "Stormwind", "Shadowbane", "Lightbringer", "Frostborn",
                "Emberheart", "Starweaver", "Nightfall", "Dawnbreaker", "Stonefist",
                "Swiftarrow", "Goldshield", "Darkblade", "Firewalker", "Iceborn"
            };
        }
        
        if (titles == null || titles.Length == 0)
        {
            titles = new string[]
            {
                "the Brave", "the Wise", "the Swift", "the Bold", "the Cunning",
                "the Strong", "the Noble", "the Silent", "the Fierce", "the Just",
                "the Wanderer", "the Guardian", "the Hunter", "the Scholar", "the Healer"
            };
        }
    }

    private void GenerateInitialAdventurers()
    {
        RefreshRecruitment();
    }

    public void RefreshRecruitment()
    {
        availableAdventurers.Clear();
        
        for (int i = 0; i < maxAvailableAdventurers; i++)
        {
            Adventurer newAdventurer = GenerateRandomAdventurer();
            availableAdventurers.Add(newAdventurer);
        }
        
        lastRefreshTime = Time.time;
        OnRecruitmentRefreshed?.Invoke(availableAdventurers);
        
        if (debugMode)
        {
            Debug.Log($"Recruitment refreshed with {availableAdventurers.Count} adventurers");
        }
    }

    public Adventurer GenerateRandomAdventurer()
    {
        // Generate rarity based on probabilities
        Adventurer.AdventurerRarity rarity = GenerateRarity();
        
        // Generate class
        Adventurer.AdventurerClass adventurerClass = GenerateRandomClass();
        
        // Generate name
        string adventurerName = GenerateRandomName();
        
        // Create adventurer
        Adventurer adventurer = new Adventurer(adventurerName, adventurerClass, rarity);
        
        totalAdventurersGenerated++;
        
        if (debugMode)
        {
            Debug.Log($"Generated {rarity} {adventurerClass}: {adventurerName}");
        }
        
        return adventurer;
    }

    public Adventurer GenerateSpecificAdventurer(Adventurer.AdventurerClass adventurerClass, Adventurer.AdventurerRarity rarity)
    {
        string adventurerName = GenerateRandomName();
        Adventurer adventurer = new Adventurer(adventurerName, adventurerClass, rarity);
        
        totalAdventurersGenerated++;
        return adventurer;
    }

    private Adventurer.AdventurerRarity GenerateRarity()
    {
        float roll = UnityEngine.Random.Range(0f, 1f);
        
        if (roll <= legendaryProbability)
            return Adventurer.AdventurerRarity.Legendary;
        else if (roll <= legendaryProbability + epicProbability)
            return Adventurer.AdventurerRarity.Epic;
        else if (roll <= legendaryProbability + epicProbability + rareProbability)
            return Adventurer.AdventurerRarity.Rare;
        else if (roll <= legendaryProbability + epicProbability + rareProbability + uncommonProbability)
            return Adventurer.AdventurerRarity.Uncommon;
        else
            return Adventurer.AdventurerRarity.Common;
    }

    private Adventurer.AdventurerClass GenerateRandomClass()
    {
        var classes = System.Enum.GetValues(typeof(Adventurer.AdventurerClass));
        return (Adventurer.AdventurerClass)classes.GetValue(UnityEngine.Random.Range(0, classes.Length));
    }

    private string GenerateRandomName()
    {
        string firstName = firstNames[UnityEngine.Random.Range(0, firstNames.Length)];
        string lastName = lastNames[UnityEngine.Random.Range(0, lastNames.Length)];
        
        // 30% chance to have a title
        if (UnityEngine.Random.Range(0f, 1f) < 0.3f)
        {
            string title = titles[UnityEngine.Random.Range(0, titles.Length)];
            return $"{firstName} {lastName} {title}";
        }
        
        return $"{firstName} {lastName}";
    }

    public bool CanRecruitAdventurer(Adventurer adventurer, out string reason)
    {
        reason = "";
        
        if (adventurer == null)
        {
            reason = "Aventurier invalide";
            return false;
        }
        
        if (!availableAdventurers.Contains(adventurer))
        {
            reason = "Aventurier non disponible";
            return false;
        }
        
        if (ResourceManager.Instance == null)
        {
            reason = "Gestionnaire de ressources non disponible";
            return false;
        }
        
        if (!ResourceManager.Instance.HasResource(Resource.ResourceType.Gold, adventurer.RecruitmentCost))
        {
            reason = $"Or insuffisant (requis: {adventurer.RecruitmentCost})";
            return false;
        }
        
        // Check adventurer capacity
        if (GameManager.Instance?.CurrentCity != null)
        {
            int currentAdventurers = GameManager.Instance.CurrentCity.Adventurers.Count;
            int maxAdventurers = GetMaxAdventurerCapacity();
            
            if (currentAdventurers >= maxAdventurers)
            {
                reason = $"Capacité maximale atteinte ({maxAdventurers})";
                return false;
            }
        }
        
        return true;
    }

    public bool RecruitAdventurer(Adventurer adventurer)
    {
        if (!CanRecruitAdventurer(adventurer, out string reason))
        {
            OnRecruitmentError?.Invoke(reason);
            return false;
        }
        
        // Spend gold
        if (!ResourceManager.Instance.RemoveResource(Resource.ResourceType.Gold, adventurer.RecruitmentCost))
        {
            OnRecruitmentError?.Invoke("Échec du paiement");
            return false;
        }
        
        // Add to city
        if (GameManager.Instance?.CurrentCity != null)
        {
            GameManager.Instance.CurrentCity.AddAdventurer(adventurer);
        }
        
        // Remove from available list
        availableAdventurers.Remove(adventurer);
        
        OnAdventurerRecruited?.Invoke(adventurer);
        
        if (debugMode)
        {
            Debug.Log($"Recruited {adventurer.Name} for {adventurer.RecruitmentCost} gold");
        }
        
        return true;
    }

    public bool DismissAdventurer(Adventurer adventurer)
    {
        if (adventurer == null) return false;
        
        if (GameManager.Instance?.CurrentCity != null)
        {
            GameManager.Instance.CurrentCity.RemoveAdventurer(adventurer);
        }
        
        // Refund partial cost (25%)
        int refund = adventurer.RecruitmentCost / 4;
        if (ResourceManager.Instance != null && refund > 0)
        {
            ResourceManager.Instance.AddResource(Resource.ResourceType.Gold, refund);
        }
        
        OnAdventurerDismissed?.Invoke(adventurer);
        
        if (debugMode)
        {
            Debug.Log($"Dismissed {adventurer.Name}, refunded {refund} gold");
        }
        
        return true;
    }

    public void ForceRefresh()
    {
        RefreshRecruitment();
    }

    public void SetMaxAvailableAdventurers(int max)
    {
        maxAvailableAdventurers = Mathf.Clamp(max, 1, 20);
    }

    public void SetRefreshTime(float hours)
    {
        recruitmentRefreshTime = Mathf.Max(1f, hours);
    }

    public void SetRarityProbabilities(float common, float uncommon, float rare, float epic, float legendary)
    {
        float total = common + uncommon + rare + epic + legendary;
        if (total > 1f)
        {
            // Normalize
            commonProbability = common / total;
            uncommonProbability = uncommon / total;
            rareProbability = rare / total;
            epicProbability = epic / total;
            legendaryProbability = legendary / total;
        }
        else
        {
            commonProbability = common;
            uncommonProbability = uncommon;
            rareProbability = rare;
            epicProbability = epic;
            legendaryProbability = legendary;
        }
    }

    private int GetMaxAdventurerCapacity()
    {
        int baseCapacity = 10;
        
        // Add capacity from buildings
        if (BuildingEffectsSystem.Instance != null)
        {
            baseCapacity += BuildingEffectsSystem.Instance.AdventurerCapacityBonus;
        }
        
        return baseCapacity;
    }

    public Dictionary<Adventurer.AdventurerRarity, int> GetRarityDistribution()
    {
        var distribution = new Dictionary<Adventurer.AdventurerRarity, int>();
        
        foreach (var rarity in System.Enum.GetValues(typeof(Adventurer.AdventurerRarity)))
        {
            distribution[(Adventurer.AdventurerRarity)rarity] = 0;
        }
        
        foreach (var adventurer in availableAdventurers)
        {
            distribution[adventurer.Rarity]++;
        }
        
        return distribution;
    }

    public Dictionary<Adventurer.AdventurerClass, int> GetClassDistribution()
    {
        var distribution = new Dictionary<Adventurer.AdventurerClass, int>();
        
        foreach (var adventurerClass in System.Enum.GetValues(typeof(Adventurer.AdventurerClass)))
        {
            distribution[(Adventurer.AdventurerClass)adventurerClass] = 0;
        }
        
        foreach (var adventurer in availableAdventurers)
        {
            distribution[adventurer.Class]++;
        }
        
        return distribution;
    }

    public string GetRecruitmentSummary()
    {
        string summary = "=== RECRUITMENT SUMMARY ===\n";
        summary += $"Available Adventurers: {availableAdventurers.Count}/{maxAvailableAdventurers}\n";
        summary += $"Total Generated: {totalAdventurersGenerated}\n";
        summary += $"Time Until Refresh: {TimeUntilRefresh:F1}h\n\n";
        
        var rarityDist = GetRarityDistribution();
        summary += "RARITY DISTRIBUTION:\n";
        foreach (var kvp in rarityDist)
        {
            if (kvp.Value > 0)
                summary += $"  {kvp.Key}: {kvp.Value}\n";
        }
        
        var classDist = GetClassDistribution();
        summary += "\nCLASS DISTRIBUTION:\n";
        foreach (var kvp in classDist)
        {
            if (kvp.Value > 0)
                summary += $"  {kvp.Key}: {kvp.Value}\n";
        }
        
        return summary;
    }

    private void Update()
    {
        // Auto-refresh if enough time has passed
        if (CanRefresh && availableAdventurers.Count < maxAvailableAdventurers)
        {
            RefreshRecruitment();
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // Debug methods
    #if UNITY_EDITOR
    [ContextMenu("Debug: Force Refresh Recruitment")]
    private void DebugForceRefresh()
    {
        ForceRefresh();
    }

    [ContextMenu("Debug: Generate Legendary Adventurer")]
    private void DebugGenerateLegendary()
    {
        var legendary = GenerateSpecificAdventurer(
            (Adventurer.AdventurerClass)UnityEngine.Random.Range(0, 5),
            Adventurer.AdventurerRarity.Legendary
        );
        availableAdventurers.Add(legendary);
        Debug.Log($"Generated legendary: {legendary.Name}");
    }

    [ContextMenu("Debug: Print Recruitment Summary")]
    private void DebugPrintSummary()
    {
        Debug.Log(GetRecruitmentSummary());
    }
    #endif
}