using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PartyManagementSystem : MonoBehaviour
{
    public static PartyManagementSystem Instance { get; private set; }
    
    [Header("Party Management Settings")]
    [SerializeField] private int maxParties = 5;
    [SerializeField] private int defaultPartySize = 4;
    [SerializeField] private bool autoAssignLeaders = true;
    
    [Header("Debug")]
    [SerializeField] private bool debugMode = false;
    
    private List<AdventurerParty> parties;
    private int nextPartyId = 1;
    
    public List<AdventurerParty> Parties => new List<AdventurerParty>(parties);
    public int ActivePartyCount => parties.Count(p => p.IsActive && !p.IsEmpty);
    public int MaxParties => maxParties;
    public bool CanCreateNewParty => parties.Count < maxParties;
    
    // Events
    public static event Action<AdventurerParty> OnPartyCreated;
    public static event Action<AdventurerParty> OnPartyDisbanded;
    public static event Action<AdventurerParty, Adventurer> OnAdventurerAddedToParty;
    public static event Action<AdventurerParty, Adventurer> OnAdventurerRemovedFromParty;
    public static event Action<AdventurerParty, Adventurer> OnPartyLeaderChanged;
    public static event Action<AdventurerParty, AdventurerParty.PartyFormation> OnPartyFormationChanged;
    public static event Action<string> OnPartyManagementError;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            if (Application.isPlaying)
            {
                DontDestroyOnLoad(gameObject);
            }
            InitializePartySystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializePartySystem()
    {
        parties = new List<AdventurerParty>();
        
        if (debugMode)
        {
            Debug.Log("PartyManagementSystem initialized");
        }
    }

    public AdventurerParty CreateParty(string partyName = null, int partySize = -1)
    {
        if (!CanCreateNewParty)
        {
            OnPartyManagementError?.Invoke("Maximum number of parties reached");
            return null;
        }
        
        if (string.IsNullOrEmpty(partyName))
        {
            partyName = $"Party {nextPartyId}";
        }
        
        if (partySize <= 0)
        {
            partySize = defaultPartySize;
        }
        
        AdventurerParty newParty = new AdventurerParty(nextPartyId, partyName, partySize);
        parties.Add(newParty);
        nextPartyId++;
        
        OnPartyCreated?.Invoke(newParty);
        
        if (debugMode)
        {
            Debug.Log($"Created party: {newParty.PartyName} (ID: {newParty.PartyId})");
        }
        
        return newParty;
    }

    public bool DisbandParty(AdventurerParty party)
    {
        if (party == null || !parties.Contains(party))
        {
            OnPartyManagementError?.Invoke("Party not found");
            return false;
        }
        
        if (party.IsOnMission)
        {
            OnPartyManagementError?.Invoke("Cannot disband party while on mission");
            return false;
        }
        
        // Remove all adventurers from party
        var members = party.Members.ToList();
        foreach (var adventurer in members)
        {
            RemoveAdventurerFromParty(party, adventurer);
        }
        
        parties.Remove(party);
        OnPartyDisbanded?.Invoke(party);
        
        if (debugMode)
        {
            Debug.Log($"Disbanded party: {party.PartyName}");
        }
        
        return true;
    }

    public bool DisbandParty(int partyId)
    {
        var party = GetPartyById(partyId);
        return DisbandParty(party);
    }

    public bool AddAdventurerToParty(AdventurerParty party, Adventurer adventurer)
    {
        if (party == null || adventurer == null)
        {
            OnPartyManagementError?.Invoke("Invalid party or adventurer");
            return false;
        }
        
        if (!parties.Contains(party))
        {
            OnPartyManagementError?.Invoke("Party not managed by this system");
            return false;
        }
        
        if (!adventurer.IsAvailable)
        {
            OnPartyManagementError?.Invoke($"{adventurer.Name} is not available");
            return false;
        }
        
        if (adventurer.IsInParty)
        {
            OnPartyManagementError?.Invoke($"{adventurer.Name} is already in a party");
            return false;
        }
        
        if (party.AddMember(adventurer))
        {
            OnAdventurerAddedToParty?.Invoke(party, adventurer);
            
            if (debugMode)
            {
                Debug.Log($"Added {adventurer.Name} to {party.PartyName}");
            }
            
            return true;
        }
        
        OnPartyManagementError?.Invoke("Failed to add adventurer to party");
        return false;
    }

    public bool RemoveAdventurerFromParty(AdventurerParty party, Adventurer adventurer)
    {
        if (party == null || adventurer == null)
        {
            OnPartyManagementError?.Invoke("Invalid party or adventurer");
            return false;
        }
        
        if (!parties.Contains(party))
        {
            OnPartyManagementError?.Invoke("Party not managed by this system");
            return false;
        }
        
        if (party.IsOnMission)
        {
            OnPartyManagementError?.Invoke("Cannot remove adventurer while party is on mission");
            return false;
        }
        
        if (party.RemoveMember(adventurer))
        {
            OnAdventurerRemovedFromParty?.Invoke(party, adventurer);
            
            if (debugMode)
            {
                Debug.Log($"Removed {adventurer.Name} from {party.PartyName}");
            }
            
            return true;
        }
        
        OnPartyManagementError?.Invoke("Failed to remove adventurer from party");
        return false;
    }

    public bool TransferAdventurer(AdventurerParty fromParty, AdventurerParty toParty, Adventurer adventurer)
    {
        if (fromParty == null || toParty == null || adventurer == null)
        {
            OnPartyManagementError?.Invoke("Invalid parties or adventurer");
            return false;
        }
        
        if (fromParty.IsOnMission || toParty.IsOnMission)
        {
            OnPartyManagementError?.Invoke("Cannot transfer adventurer while parties are on mission");
            return false;
        }
        
        if (!toParty.CanAddMember)
        {
            OnPartyManagementError?.Invoke("Target party is full");
            return false;
        }
        
        if (RemoveAdventurerFromParty(fromParty, adventurer))
        {
            if (AddAdventurerToParty(toParty, adventurer))
            {
                if (debugMode)
                {
                    Debug.Log($"Transferred {adventurer.Name} from {fromParty.PartyName} to {toParty.PartyName}");
                }
                return true;
            }
            else
            {
                // Re-add to original party if transfer failed
                AddAdventurerToParty(fromParty, adventurer);
            }
        }
        
        return false;
    }

    public bool SetPartyLeader(AdventurerParty party, Adventurer newLeader)
    {
        if (party == null || newLeader == null)
        {
            OnPartyManagementError?.Invoke("Invalid party or adventurer");
            return false;
        }
        
        if (!parties.Contains(party))
        {
            OnPartyManagementError?.Invoke("Party not managed by this system");
            return false;
        }
        
        if (party.SetLeader(newLeader))
        {
            OnPartyLeaderChanged?.Invoke(party, newLeader);
            
            if (debugMode)
            {
                Debug.Log($"Set {newLeader.Name} as leader of {party.PartyName}");
            }
            
            return true;
        }
        
        OnPartyManagementError?.Invoke("Failed to set party leader");
        return false;
    }

    public bool SetPartyFormation(AdventurerParty party, AdventurerParty.PartyFormation formation)
    {
        if (party == null)
        {
            OnPartyManagementError?.Invoke("Invalid party");
            return false;
        }
        
        if (!parties.Contains(party))
        {
            OnPartyManagementError?.Invoke("Party not managed by this system");
            return false;
        }
        
        party.SetFormation(formation);
        OnPartyFormationChanged?.Invoke(party, formation);
        
        if (debugMode)
        {
            Debug.Log($"Set {party.PartyName} formation to {formation}");
        }
        
        return true;
    }

    public AdventurerParty GetPartyById(int partyId)
    {
        return parties.FirstOrDefault(p => p.PartyId == partyId);
    }

    public AdventurerParty GetPartyByName(string partyName)
    {
        return parties.FirstOrDefault(p => p.PartyName.Equals(partyName, StringComparison.OrdinalIgnoreCase));
    }

    public AdventurerParty GetAdventurerParty(Adventurer adventurer)
    {
        if (adventurer == null || !adventurer.IsInParty) return null;
        
        return parties.FirstOrDefault(p => p.Members.Contains(adventurer));
    }

    public List<AdventurerParty> GetAvailableParties()
    {
        return parties.Where(p => p.IsActive && !p.IsOnMission).ToList();
    }

    public List<AdventurerParty> GetPartiesOnMission()
    {
        return parties.Where(p => p.IsOnMission).ToList();
    }

    public List<Adventurer> GetUnassignedAdventurers()
    {
        if (GameManager.Instance?.CurrentCity == null) return new List<Adventurer>();
        
        return GameManager.Instance.CurrentCity.Adventurers
            .Where(a => a.IsAvailable && !a.IsInParty).ToList();
    }

    public bool AutoAssignToParty(Adventurer adventurer)
    {
        if (adventurer == null || !adventurer.IsAvailable || adventurer.IsInParty)
            return false;
        
        // Find party with space that would benefit from this adventurer
        var availableParties = parties.Where(p => p.CanAddMember && !p.IsOnMission).ToList();
        
        if (availableParties.Count == 0)
        {
            // Create new party if possible
            if (CanCreateNewParty)
            {
                var newParty = CreateParty();
                return AddAdventurerToParty(newParty, adventurer);
            }
            return false;
        }
        
        // Choose best party based on class synergy
        var bestParty = FindBestPartyForAdventurer(availableParties, adventurer);
        return AddAdventurerToParty(bestParty, adventurer);
    }

    private AdventurerParty FindBestPartyForAdventurer(List<AdventurerParty> availableParties, Adventurer adventurer)
    {
        AdventurerParty bestParty = null;
        float bestScore = -1f;
        
        foreach (var party in availableParties)
        {
            float score = CalculatePartySynergy(party, adventurer);
            if (score > bestScore)
            {
                bestScore = score;
                bestParty = party;
            }
        }
        
        return bestParty ?? availableParties.First();
    }

    private float CalculatePartySynergy(AdventurerParty party, Adventurer adventurer)
    {
        if (party.IsEmpty) return 1f; // Empty party is always good
        
        float score = 0f;
        var classDistribution = party.Members.GroupBy(m => m.Class).ToDictionary(g => g.Key, g => g.Count());
        
        // Prefer diversity
        if (!classDistribution.ContainsKey(adventurer.Class))
        {
            score += 2f; // New class adds diversity
        }
        else
        {
            score -= classDistribution[adventurer.Class] * 0.5f; // Penalize duplicates
        }
        
        // Formation compatibility
        switch (party.Formation)
        {
            case AdventurerParty.PartyFormation.Offensive:
                if (adventurer.Class == Adventurer.AdventurerClass.Warrior ||
                    adventurer.Class == Adventurer.AdventurerClass.Mage ||
                    adventurer.Class == Adventurer.AdventurerClass.Rogue)
                    score += 1f;
                break;
                
            case AdventurerParty.PartyFormation.Defensive:
                if (adventurer.Class == Adventurer.AdventurerClass.Warrior ||
                    adventurer.Class == Adventurer.AdventurerClass.Cleric)
                    score += 1f;
                break;
                
            case AdventurerParty.PartyFormation.Magic:
                if (adventurer.Class == Adventurer.AdventurerClass.Mage ||
                    adventurer.Class == Adventurer.AdventurerClass.Cleric)
                    score += 1f;
                break;
        }
        
        return score;
    }

    public string GetSystemSummary()
    {
        string summary = "=== PARTY MANAGEMENT SUMMARY ===\n";
        summary += $"Active Parties: {ActivePartyCount}/{maxParties}\n";
        summary += $"Unassigned Adventurers: {GetUnassignedAdventurers().Count}\n\n";
        
        foreach (var party in parties.Where(p => p.IsActive))
        {
            summary += $"{party.GetPartySummary()}\n\n";
        }
        
        return summary;
    }

    public void SetMaxParties(int max)
    {
        maxParties = Mathf.Max(1, max);
    }

    public void SetDefaultPartySize(int size)
    {
        defaultPartySize = Mathf.Clamp(size, 1, 6);
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
    [ContextMenu("Debug: Print System Summary")]
    private void DebugPrintSummary()
    {
        Debug.Log(GetSystemSummary());
    }

    [ContextMenu("Debug: Create Test Party")]
    private void DebugCreateTestParty()
    {
        var party = CreateParty("Test Party");
        Debug.Log($"Created test party: {party?.PartyName}");
    }

    [ContextMenu("Debug: Auto-assign All Unassigned")]
    private void DebugAutoAssignAll()
    {
        var unassigned = GetUnassignedAdventurers();
        foreach (var adventurer in unassigned)
        {
            AutoAssignToParty(adventurer);
        }
        Debug.Log($"Auto-assigned {unassigned.Count} adventurers");
    }
    #endif
}