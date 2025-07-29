using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AdventurerSystem : MonoBehaviour
{
    [Header("Recruitment Settings")]
    [SerializeField] private int baseRecruitmentCost = 100;
    [SerializeField] private int costPerLevel = 50;
    [SerializeField] private int maxRecruitmentPool = 10;
    
    [Header("Generation Settings")]
    [SerializeField] private int minLevel = 1;
    [SerializeField] private int maxLevel = 5;
    [SerializeField] private float rareClassChance = 0.3f;
    
    [Header("Names")]
    [SerializeField] private string[] maleNames = { "Aldric", "Brom", "Gareth", "Kael", "Theron", "Vex", "Zander" };
    [SerializeField] private string[] femaleNames = { "Aria", "Brynn", "Elara", "Lyra", "Mira", "Nyx", "Zara" };
    [SerializeField] private string[] surnames = { "Forteflamme", "Ombreacier", "Ventargent", "Pierrelame", "Lunefer", "Solarbre", "Glacesang" };

    private List<Adventurer> recruitmentPool;
    private System.Random random;

    private void Awake()
    {
        recruitmentPool = new List<Adventurer>();
        random = new System.Random();
    }

    public List<Adventurer> SearchForAdventurers(int maxResults)
    {
        List<Adventurer> foundAdventurers = new List<Adventurer>();
        
        for (int i = 0; i < maxResults; i++)
        {
            Adventurer newAdventurer = GenerateRandomAdventurer();
            foundAdventurers.Add(newAdventurer);
        }
        
        Debug.Log($"AdventurerSystem: Generated {foundAdventurers.Count} new adventurers");
        return foundAdventurers;
    }

    private Adventurer GenerateRandomAdventurer()
    {
        // Generate random name
        bool isMale = random.NextDouble() < 0.5;
        string firstName = isMale ? maleNames[random.Next(maleNames.Length)] : femaleNames[random.Next(femaleNames.Length)];
        string lastName = surnames[random.Next(surnames.Length)];
        string fullName = $"{firstName} {lastName}";
        
        // Generate random class
        AdventurerClass[] classes = System.Enum.GetValues(typeof(AdventurerClass)).Cast<AdventurerClass>().ToArray();
        AdventurerClass randomClass = classes[random.Next(classes.Length)];
        
        // Generate random level
        int level = random.Next(minLevel, maxLevel + 1);
        
        // Create adventurer
        Adventurer adventurer = new Adventurer(fullName, randomClass);
        
        // Level up if needed
        for (int i = 1; i < level; i++)
        {
            adventurer.GainExperience(adventurer.experienceToNextLevel);
        }
        
        // Add some randomness to stats
        AddRandomBonusStats(adventurer);
        
        return adventurer;
    }

    private void AddRandomBonusStats(Adventurer adventurer)
    {
        // Add 1-3 random bonus points to stats
        int bonusPoints = random.Next(1, 4);
        
        for (int i = 0; i < bonusPoints; i++)
        {
            int statChoice = random.Next(5);
            switch (statChoice)
            {
                case 0:
                    adventurer.strength += 1;
                    break;
                case 1:
                    adventurer.intelligence += 1;
                    break;
                case 2:
                    adventurer.agility += 1;
                    break;
                case 3:
                    adventurer.charisma += 1;
                    break;
                case 4:
                    adventurer.luck += 1;
                    break;
            }
        }
    }

    public int GetRecruitmentCost(Adventurer adventurer)
    {
        if (adventurer == null) return baseRecruitmentCost;
        
        int cost = baseRecruitmentCost + (adventurer.level * costPerLevel);
        
        // Add cost based on total stats
        int totalStats = adventurer.strength + adventurer.intelligence + adventurer.agility + 
                        adventurer.charisma + adventurer.luck;
        cost += (totalStats - 50) * 2; // Base stats total around 50
        
        return Mathf.Max(cost, baseRecruitmentCost);
    }

    public bool RecruitAdventurer(Adventurer adventurer, City city)
    {
        if (adventurer == null || city == null) return false;
        
        // Check if city has space
        if (city.adventurers.Count >= city.maxAdventurers)
        {
            Debug.Log("City has reached maximum adventurer capacity!");
            return false;
        }
        
        // Check if city has enough gold
        int cost = GetRecruitmentCost(adventurer);
        if (city.gold < cost)
        {
            Debug.Log($"Not enough gold to recruit {adventurer.name}! Cost: {cost}, Available: {city.gold}");
            return false;
        }
        
        // Add to city
        city.adventurers.Add(adventurer);
        
        Debug.Log($"Successfully recruited {adventurer.name} for {cost} gold!");
        return true;
    }

    public bool DismissAdventurer(Adventurer adventurer, City city)
    {
        if (adventurer == null || city == null) return false;
        
        // Can't dismiss if on mission
        if (adventurer.isOnMission)
        {
            Debug.Log($"Cannot dismiss {adventurer.name} - currently on mission!");
            return false;
        }
        
        bool removed = city.adventurers.Remove(adventurer);
        if (removed)
        {
            Debug.Log($"Dismissed {adventurer.name} from the city.");
        }
        
        return removed;
    }

    public List<Adventurer> GetAvailableAdventurers(City city)
    {
        if (city == null) return new List<Adventurer>();
        
        return city.adventurers.Where(a => a.CanGoOnMission()).ToList();
    }

    public List<Adventurer> GetInjuredAdventurers(City city)
    {
        if (city == null) return new List<Adventurer>();
        
        return city.adventurers.Where(a => a.isInjured).ToList();
    }

    public List<Adventurer> GetAdventurersOnMission(City city)
    {
        if (city == null) return new List<Adventurer>();
        
        return city.adventurers.Where(a => a.isOnMission).ToList();
    }

    public void ProcessDailyRecovery(City city)
    {
        if (city == null) return;
        
        foreach (Adventurer adventurer in city.adventurers)
        {
            adventurer.ProcessRecovery();
        }
        
        Debug.Log("Processed daily recovery for all adventurers.");
    }

    public void HealAdventurer(Adventurer adventurer, int healAmount)
    {
        if (adventurer == null) return;
        
        adventurer.Heal(healAmount);
        Debug.Log($"Healed {adventurer.name} for {healAmount} HP.");
    }

    public void HealAllAdventurers(City city, int healAmount)
    {
        if (city == null) return;
        
        foreach (Adventurer adventurer in city.adventurers)
        {
            if (adventurer.currentHealth < adventurer.maxHealth)
            {
                adventurer.Heal(healAmount);
            }
        }
        
        Debug.Log($"Healed all adventurers for {healAmount} HP.");
    }

    public int GetTotalPartyPower(List<Adventurer> party)
    {
        if (party == null || party.Count == 0) return 0;
        
        return party.Sum(a => a.GetTotalPower());
    }

    public float GetAveragePartyLevel(List<Adventurer> party)
    {
        if (party == null || party.Count == 0) return 0f;
        
        return (float)party.Average(a => a.level);
    }

    public Dictionary<AdventurerClass, int> GetClassDistribution(City city)
    {
        Dictionary<AdventurerClass, int> distribution = new Dictionary<AdventurerClass, int>();
        
        if (city == null) return distribution;
        
        foreach (AdventurerClass adventurerClass in System.Enum.GetValues(typeof(AdventurerClass)))
        {
            distribution[adventurerClass] = 0;
        }
        
        foreach (Adventurer adventurer in city.adventurers)
        {
            distribution[adventurer.adventurerClass]++;
        }
        
        return distribution;
    }

    public List<Adventurer> GetRecommendedParty(City city, int maxSize = 4)
    {
        if (city == null) return new List<Adventurer>();
        
        List<Adventurer> available = GetAvailableAdventurers(city);
        if (available.Count == 0) return new List<Adventurer>();
        
        // Sort by power and take the strongest
        available.Sort((a, b) => b.GetTotalPower().CompareTo(a.GetTotalPower()));
        
        return available.Take(maxSize).ToList();
    }

    public string GetAdventurerSummary(City city)
    {
        if (city == null) return "Aucune ville assignée";
        
        int total = city.adventurers.Count;
        int available = GetAvailableAdventurers(city).Count;
        int injured = GetInjuredAdventurers(city).Count;
        int onMission = GetAdventurersOnMission(city).Count;
        
        return $"Total: {total}/{city.maxAdventurers} | Disponibles: {available} | Blessés: {injured} | En mission: {onMission}";
    }
}