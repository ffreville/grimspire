using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class AdventurerParty
{
    public enum PartyFormation
    {
        Balanced,       // Front: Tank, Back: DPS/Support
        Offensive,      // Focus on damage dealers
        Defensive,      // Focus on tanks and healers
        Stealth,        // Focus on rogues and scouts
        Magic,          // Focus on mages and magical classes
        Custom          // Player-defined formation
    }

    [Header("Party Information")]
    [SerializeField] private int partyId;
    [SerializeField] private string partyName;
    [SerializeField] private PartyFormation formation;
    [SerializeField] private List<Adventurer> members;
    [SerializeField] private Adventurer leader;
    [SerializeField] private bool isActive;
    [SerializeField] private bool isOnMission;
    
    [Header("Party Stats")]
    [SerializeField] private int maxSize;
    [SerializeField] private float cohesion;
    [SerializeField] private float morale;
    [SerializeField] private int totalLevel;
    [SerializeField] private DateTime creationDate;
    [SerializeField] private int missionsCompleted;
    [SerializeField] private float successRate;
    
    [Header("Synergies")]
    [SerializeField] private List<PartySynergy> activeSynergies;
    [SerializeField] private Dictionary<string, float> combinedBonuses;

    // Properties
    public int PartyId => partyId;
    public string PartyName => partyName;
    public PartyFormation Formation => formation;
    public List<Adventurer> Members => new List<Adventurer>(members ?? new List<Adventurer>());
    public Adventurer Leader => leader;
    public bool IsActive => isActive;
    public bool IsOnMission => isOnMission;
    public int MaxSize => maxSize;
    public int CurrentSize => members?.Count ?? 0;
    public bool IsFull => CurrentSize >= maxSize;
    public bool IsEmpty => CurrentSize == 0;
    public float Cohesion => cohesion;
    public float Morale => morale;
    public int TotalLevel => totalLevel;
    public DateTime CreationDate => creationDate;
    public int MissionsCompleted => missionsCompleted;
    public float SuccessRate => successRate;
    public List<PartySynergy> ActiveSynergies => new List<PartySynergy>(activeSynergies ?? new List<PartySynergy>());
    public Dictionary<string, float> CombinedBonuses => new Dictionary<string, float>(combinedBonuses ?? new Dictionary<string, float>());
    
    // Calculated Properties
    public int AverageLevel => CurrentSize > 0 ? totalLevel / CurrentSize : 0;
    public float CombatPower => CalculateCombatPower();
    public float OverallHealth => CalculateOverallHealth();
    public bool HasLeader => leader != null;
    public bool CanAddMember => CurrentSize < maxSize;

    public AdventurerParty(int id, string name, int maximumSize = 4)
    {
        partyId = id;
        partyName = name;
        maxSize = Mathf.Clamp(maximumSize, 1, 6);
        formation = PartyFormation.Balanced;
        members = new List<Adventurer>();
        activeSynergies = new List<PartySynergy>();
        combinedBonuses = new Dictionary<string, float>();
        isActive = true;
        isOnMission = false;
        cohesion = 50f;
        morale = 50f;
        creationDate = DateTime.Now;
        missionsCompleted = 0;
        successRate = 0f;
        
        RecalculateStats();
    }

    public bool AddMember(Adventurer adventurer)
    {
        if (adventurer == null || !CanAddMember || members.Contains(adventurer))
            return false;
        
        if (!adventurer.IsAvailable)
            return false;
        
        members.Add(adventurer);
        adventurer.SetPartyId(partyId);
        
        // Set leader if none exists
        if (leader == null)
        {
            SetLeader(adventurer);
        }
        
        RecalculateStats();
        RecalculateSynergies();
        
        return true;
    }

    public bool RemoveMember(Adventurer adventurer)
    {
        if (adventurer == null || !members.Contains(adventurer))
            return false;
        
        members.Remove(adventurer);
        adventurer.SetPartyId(-1);
        
        // If removed adventurer was leader, assign new leader
        if (leader == adventurer)
        {
            leader = members.Count > 0 ? GetBestLeaderCandidate() : null;
        }
        
        RecalculateStats();
        RecalculateSynergies();
        
        return true;
    }

    public bool SetLeader(Adventurer adventurer)
    {
        if (adventurer == null || !members.Contains(adventurer))
            return false;
        
        leader = adventurer;
        RecalculateStats();
        
        return true;
    }

    public void SetFormation(PartyFormation newFormation)
    {
        formation = newFormation;
        RecalculateSynergies();
        RecalculateStats();
    }

    public void SetPartyName(string newName)
    {
        if (!string.IsNullOrEmpty(newName))
        {
            partyName = newName;
        }
    }

    private void RecalculateStats()
    {
        if (members == null || members.Count == 0)
        {
            totalLevel = 0;
            cohesion = 50f;
            morale = 50f;
            return;
        }
        
        // Calculate total level
        totalLevel = members.Sum(m => m.Level);
        
        // Calculate cohesion based on class diversity and time together
        CalculateCohesion();
        
        // Calculate morale based on success rate and member happiness
        CalculateMorale();
        
        // Update combined bonuses
        CalculateCombinedBonuses();
    }

    private void CalculateCohesion()
    {
        if (members.Count <= 1)
        {
            cohesion = 100f;
            return;
        }
        
        float baseCohesion = 50f;
        
        // Bonus for balanced party composition
        var classDistribution = GetClassDistribution();
        if (classDistribution.Count >= 3) // Diverse party
        {
            baseCohesion += 20f;
        }
        else if (classDistribution.Count == 1) // All same class
        {
            baseCohesion -= 10f;
        }
        
        // Bonus for having a leader
        if (HasLeader)
        {
            baseCohesion += leader.Charisma * 2f;
        }
        
        // Bonus based on missions completed together
        if (missionsCompleted > 0)
        {
            baseCohesion += Mathf.Min(missionsCompleted * 2f, 30f);
        }
        
        cohesion = Mathf.Clamp(baseCohesion, 0f, 100f);
    }

    private void CalculateMorale()
    {
        if (members.Count == 0)
        {
            morale = 50f;
            return;
        }
        
        float averageLoyalty = members.Average(m => m.LoyaltyLevel);
        float baseMorale = averageLoyalty;
        
        // Bonus for success rate
        if (successRate > 0.7f)
        {
            baseMorale += 20f;
        }
        else if (successRate < 0.3f)
        {
            baseMorale -= 20f;
        }
        
        // Bonus for good equipment
        float averageEquipmentQuality = CalculateAverageEquipmentQuality();
        baseMorale += averageEquipmentQuality * 10f;
        
        morale = Mathf.Clamp(baseMorale, 0f, 100f);
    }

    private void RecalculateSynergies()
    {
        activeSynergies.Clear();
        
        if (members.Count < 2) return;
        
        // Check for class synergies
        CheckClassSynergies();
        
        // Check for formation synergies
        CheckFormationSynergies();
        
        // Check for trait synergies
        CheckTraitSynergies();
    }

    private void CheckClassSynergies()
    {
        var classCount = GetClassDistribution();
        
        // Tank + Healer synergy
        if (classCount.ContainsKey(Adventurer.AdventurerClass.Warrior) && 
            classCount.ContainsKey(Adventurer.AdventurerClass.Cleric))
        {
            activeSynergies.Add(new PartySynergy("Tank & Heal", "Improved survivability", 
                new Dictionary<string, float> { ["Defense"] = 15f, ["HealthRegen"] = 10f }));
        }
        
        // Magic synergy (2+ mages)
        if (classCount.GetValueOrDefault(Adventurer.AdventurerClass.Mage, 0) >= 2)
        {
            activeSynergies.Add(new PartySynergy("Arcane Focus", "Enhanced magical power", 
                new Dictionary<string, float> { ["MagicDamage"] = 25f, ["ManaRegen"] = 15f }));
        }
        
        // Stealth synergy (2+ rogues)
        if (classCount.GetValueOrDefault(Adventurer.AdventurerClass.Rogue, 0) >= 2)
        {
            activeSynergies.Add(new PartySynergy("Shadow Strike", "Increased critical chance", 
                new Dictionary<string, float> { ["CriticalChance"] = 20f, ["Initiative"] = 10f }));
        }
        
        // Balanced party synergy (all 4+ different classes)
        if (classCount.Count >= 4)
        {
            activeSynergies.Add(new PartySynergy("Perfect Balance", "All stats boosted", 
                new Dictionary<string, float> { ["AllStats"] = 10f }));
        }
    }

    private void CheckFormationSynergies()
    {
        switch (formation)
        {
            case PartyFormation.Offensive:
                if (GetDamageDealerCount() >= 3)
                {
                    activeSynergies.Add(new PartySynergy("All-Out Attack", "Maximum damage output", 
                        new Dictionary<string, float> { ["Damage"] = 30f, ["AttackSpeed"] = 15f }));
                }
                break;
                
            case PartyFormation.Defensive:
                if (GetTankAndHealerCount() >= 2)
                {
                    activeSynergies.Add(new PartySynergy("Fortress", "Maximum defense", 
                        new Dictionary<string, float> { ["Defense"] = 25f, ["HealthRegen"] = 20f }));
                }
                break;
                
            case PartyFormation.Magic:
                if (GetMagicUserCount() >= 3)
                {
                    activeSynergies.Add(new PartySynergy("Magical Circle", "Enhanced spellcasting", 
                        new Dictionary<string, float> { ["MagicPower"] = 35f, ["ManaEfficiency"] = 20f }));
                }
                break;
        }
    }

    private void CheckTraitSynergies()
    {
        // This would check for complementary traits between party members
        // Implementation depends on the trait system
    }

    private void CalculateCombinedBonuses()
    {
        combinedBonuses.Clear();
        
        foreach (var synergy in activeSynergies)
        {
            foreach (var bonus in synergy.Bonuses)
            {
                if (!combinedBonuses.ContainsKey(bonus.Key))
                    combinedBonuses[bonus.Key] = 0f;
                
                combinedBonuses[bonus.Key] += bonus.Value;
            }
        }
        
        // Apply cohesion and morale modifiers
        float cohesionModifier = (cohesion - 50f) / 100f; // -0.5 to +0.5
        float moraleModifier = (morale - 50f) / 100f;
        
        var keys = new List<string>(combinedBonuses.Keys);
        foreach (var key in keys)
        {
            combinedBonuses[key] *= (1f + cohesionModifier * 0.2f + moraleModifier * 0.15f);
        }
    }

    private Dictionary<Adventurer.AdventurerClass, int> GetClassDistribution()
    {
        var distribution = new Dictionary<Adventurer.AdventurerClass, int>();
        
        foreach (var member in members)
        {
            if (!distribution.ContainsKey(member.Class))
                distribution[member.Class] = 0;
            distribution[member.Class]++;
        }
        
        return distribution;
    }

    private Adventurer GetBestLeaderCandidate()
    {
        if (members.Count == 0) return null;
        
        return members.OrderByDescending(m => m.Charisma + m.Level).First();
    }

    private float CalculateCombatPower()
    {
        if (members.Count == 0) return 0f;
        
        float basePower = members.Sum(m => m.GetCombatPower());
        
        // Apply synergy bonuses
        float synergyMultiplier = 1f;
        if (combinedBonuses.ContainsKey("AllStats"))
            synergyMultiplier += combinedBonuses["AllStats"] / 100f;
        if (combinedBonuses.ContainsKey("Damage"))
            synergyMultiplier += combinedBonuses["Damage"] / 100f;
        
        // Apply cohesion and morale
        float cohesionBonus = (cohesion - 50f) / 100f * 0.3f;
        float moraleBonus = (morale - 50f) / 100f * 0.2f;
        
        return basePower * synergyMultiplier * (1f + cohesionBonus + moraleBonus);
    }

    private float CalculateOverallHealth()
    {
        if (members.Count == 0) return 0f;
        
        return members.Average(m => m.HealthPercentage);
    }

    private float CalculateAverageEquipmentQuality()
    {
        if (members.Count == 0) return 0f;
        
        float totalQuality = 0f;
        int equipmentCount = 0;
        
        foreach (var member in members)
        {
            if (member.Weapon != null) { totalQuality += GetEquipmentQuality(member.Weapon); equipmentCount++; }
            if (member.Armor != null) { totalQuality += GetEquipmentQuality(member.Armor); equipmentCount++; }
            if (member.Helmet != null) { totalQuality += GetEquipmentQuality(member.Helmet); equipmentCount++; }
            if (member.Boots != null) { totalQuality += GetEquipmentQuality(member.Boots); equipmentCount++; }
            if (member.Accessory != null) { totalQuality += GetEquipmentQuality(member.Accessory); equipmentCount++; }
        }
        
        return equipmentCount > 0 ? totalQuality / equipmentCount : 0f;
    }

    private float GetEquipmentQuality(Equipment equipment)
    {
        if (equipment == null) return 0f;
        
        switch (equipment.Rarity)
        {
            case Equipment.EquipmentRarity.Common: return 0.2f;
            case Equipment.EquipmentRarity.Uncommon: return 0.4f;
            case Equipment.EquipmentRarity.Rare: return 0.6f;
            case Equipment.EquipmentRarity.Epic: return 0.8f;
            case Equipment.EquipmentRarity.Legendary: return 1.0f;
            case Equipment.EquipmentRarity.Artifact: return 1.2f;
            default: return 0f;
        }
    }

    private int GetDamageDealerCount()
    {
        return members.Count(m => m.Class == Adventurer.AdventurerClass.Warrior || 
                                 m.Class == Adventurer.AdventurerClass.Mage || 
                                 m.Class == Adventurer.AdventurerClass.Rogue ||
                                 m.Class == Adventurer.AdventurerClass.Ranger);
    }

    private int GetTankAndHealerCount()
    {
        return members.Count(m => m.Class == Adventurer.AdventurerClass.Warrior || 
                                 m.Class == Adventurer.AdventurerClass.Cleric);
    }

    private int GetMagicUserCount()
    {
        return members.Count(m => m.Class == Adventurer.AdventurerClass.Mage || 
                                 m.Class == Adventurer.AdventurerClass.Cleric);
    }

    public void CompleteMission(bool successful)
    {
        missionsCompleted++;
        
        if (successful)
        {
            // Increase cohesion and morale
            cohesion = Mathf.Min(100f, cohesion + 2f);
            morale = Mathf.Min(100f, morale + 3f);
        }
        else
        {
            // Decrease morale
            morale = Mathf.Max(0f, morale - 5f);
        }
        
        // Recalculate success rate
        int successfulMissions = Mathf.RoundToInt(successRate * (missionsCompleted - 1));
        if (successful) successfulMissions++;
        successRate = (float)successfulMissions / missionsCompleted;
        
        RecalculateStats();
    }

    public bool IsValidFormation()
    {
        switch (formation)
        {
            case PartyFormation.Balanced:
                return members.Count >= 2;
            case PartyFormation.Offensive:
                return GetDamageDealerCount() >= 2;
            case PartyFormation.Defensive:
                return GetTankAndHealerCount() >= 1;
            case PartyFormation.Magic:
                return GetMagicUserCount() >= 2;
            case PartyFormation.Stealth:
                return members.Count(m => m.Class == Adventurer.AdventurerClass.Rogue) >= 1;
            default:
                return true;
        }
    }

    public string GetFormationDescription()
    {
        switch (formation)
        {
            case PartyFormation.Balanced: return "Formation équilibrée pour toutes situations";
            case PartyFormation.Offensive: return "Formation axée sur les dégâts maximum";
            case PartyFormation.Defensive: return "Formation défensive pour la survie";
            case PartyFormation.Magic: return "Formation magique pour les sorts puissants";
            case PartyFormation.Stealth: return "Formation furtive pour l'infiltration";
            case PartyFormation.Custom: return "Formation personnalisée";
            default: return "Formation inconnue";
        }
    }

    public string GetPartySummary()
    {
        string summary = $"=== {partyName} ===\n";
        summary += $"Membres: {CurrentSize}/{maxSize}\n";
        summary += $"Niveau moyen: {AverageLevel}\n";
        summary += $"Puissance de combat: {CombatPower:F0}\n";
        summary += $"Cohésion: {cohesion:F1}%\n";
        summary += $"Moral: {morale:F1}%\n";
        summary += $"Missions réussies: {successRate:P1}\n";
        summary += $"Formation: {GetFormationDescription()}\n";
        
        if (activeSynergies.Count > 0)
        {
            summary += "\nSynergies actives:\n";
            foreach (var synergy in activeSynergies)
            {
                summary += $"  • {synergy.Name}\n";
            }
        }
        
        return summary;
    }

    public void SetMissionStatus(bool onMission)
    {
        isOnMission = onMission;
        
        // Update member statuses
        foreach (var member in members)
        {
            if (onMission)
            {
                member.SetStatus(Adventurer.AdventurerStatus.OnMission);
            }
            else if (member.Status == Adventurer.AdventurerStatus.OnMission)
            {
                member.SetStatus(Adventurer.AdventurerStatus.Available);
            }
        }
    }

    public void SetActive(bool active)
    {
        isActive = active;
    }

    public override string ToString()
    {
        return $"{partyName} ({CurrentSize}/{maxSize}) - {formation}";
    }
}

[System.Serializable]
public class PartySynergy
{
    [SerializeField] private string name;
    [SerializeField] private string description;
    [SerializeField] private Dictionary<string, float> bonuses;
    
    public string Name => name;
    public string Description => description;
    public Dictionary<string, float> Bonuses => new Dictionary<string, float>(bonuses ?? new Dictionary<string, float>());
    
    public PartySynergy(string synergyName, string synergyDescription, Dictionary<string, float> synergyBonuses)
    {
        name = synergyName;
        description = synergyDescription;
        bonuses = new Dictionary<string, float>(synergyBonuses ?? new Dictionary<string, float>());
    }
    
    public override string ToString()
    {
        return $"{name}: {description}";
    }
}