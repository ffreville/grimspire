using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }
    
    [Header("Resource Management")]
    [SerializeField] private Dictionary<Resource.ResourceType, Resource> resources;
    [SerializeField] private bool debugMode = false;
    
    [Header("Resource Limits")]
    [SerializeField] private int defaultGoldCapacity = 10000;
    [SerializeField] private int defaultPopulationCapacity = 1000;
    [SerializeField] private int defaultMaterialCapacity = 1000;
    [SerializeField] private int defaultMagicCapacity = 100;
    [SerializeField] private int defaultReputationCapacity = 100;
    
    public Dictionary<Resource.ResourceType, Resource> Resources => resources;
    
    // Events
    public static event Action<Resource.ResourceType, int, int> OnResourceChanged;
    public static event Action<Resource.ResourceType> OnResourceEmpty;
    public static event Action<Resource.ResourceType> OnResourceFull;
    public static event Action<Resource.ResourceType, int> OnResourceAdded;
    public static event Action<Resource.ResourceType, int> OnResourceRemoved;
    public static event Action<Dictionary<Resource.ResourceType, int>> OnMultipleResourcesChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeResources();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeResources()
    {
        resources = new Dictionary<Resource.ResourceType, Resource>
        {
            [Resource.ResourceType.Gold] = new Resource(Resource.ResourceType.Gold, 1000, defaultGoldCapacity),
            [Resource.ResourceType.Population] = new Resource(Resource.ResourceType.Population, 50, defaultPopulationCapacity),
            [Resource.ResourceType.Stone] = new Resource(Resource.ResourceType.Stone, 100, defaultMaterialCapacity),
            [Resource.ResourceType.Wood] = new Resource(Resource.ResourceType.Wood, 100, defaultMaterialCapacity),
            [Resource.ResourceType.Iron] = new Resource(Resource.ResourceType.Iron, 50, defaultMaterialCapacity),
            [Resource.ResourceType.MagicCrystal] = new Resource(Resource.ResourceType.MagicCrystal, 10, defaultMagicCapacity),
            [Resource.ResourceType.Reputation] = new Resource(Resource.ResourceType.Reputation, 10, defaultReputationCapacity)
        };
        
        if (debugMode)
        {
            Debug.Log("ResourceManager initialized with default resources");
        }
    }

    public void LoadResourcesFromGameData(SerializableResource[] resourceData)
    {
        if (resourceData == null) return;
        
        foreach (var data in resourceData)
        {
            if (resources.ContainsKey(data.type))
            {
                resources[data.type].SetAmount(data.amount);
                resources[data.type].SetCapacity(data.capacity);
            }
        }
        
        if (debugMode)
        {
            Debug.Log("Resources loaded from game data");
        }
    }

    public Resource GetResource(Resource.ResourceType type)
    {
        return resources.ContainsKey(type) ? resources[type] : null;
    }

    public int GetResourceAmount(Resource.ResourceType type)
    {
        return resources.ContainsKey(type) ? resources[type].Amount : 0;
    }

    public int GetResourceCapacity(Resource.ResourceType type)
    {
        return resources.ContainsKey(type) ? resources[type].Capacity : 0;
    }

    public float GetResourcePercentage(Resource.ResourceType type)
    {
        return resources.ContainsKey(type) ? resources[type].FillPercentage : 0f;
    }

    public bool HasResource(Resource.ResourceType type, int amount)
    {
        return resources.ContainsKey(type) && resources[type].Amount >= amount;
    }

    public bool CanAddResource(Resource.ResourceType type, int amount)
    {
        return resources.ContainsKey(type) && resources[type].CanAdd(amount);
    }

    public bool AddResource(Resource.ResourceType type, int amount)
    {
        if (!resources.ContainsKey(type) || amount <= 0) return false;
        
        Resource resource = resources[type];
        int previousAmount = resource.Amount;
        
        if (resource.Add(amount))
        {
            OnResourceChanged?.Invoke(type, previousAmount, resource.Amount);
            OnResourceAdded?.Invoke(type, amount);
            
            if (resource.IsFull)
            {
                OnResourceFull?.Invoke(type);
            }
            
            if (debugMode)
            {
                Debug.Log($"Added {amount} {type}. Total: {resource.Amount}/{resource.Capacity}");
            }
            
            return true;
        }
        
        return false;
    }

    public bool RemoveResource(Resource.ResourceType type, int amount)
    {
        if (!resources.ContainsKey(type) || amount <= 0) return false;
        
        Resource resource = resources[type];
        int previousAmount = resource.Amount;
        
        if (resource.Remove(amount))
        {
            OnResourceChanged?.Invoke(type, previousAmount, resource.Amount);
            OnResourceRemoved?.Invoke(type, amount);
            
            if (resource.IsEmpty)
            {
                OnResourceEmpty?.Invoke(type);
            }
            
            if (debugMode)
            {
                Debug.Log($"Removed {amount} {type}. Total: {resource.Amount}/{resource.Capacity}");
            }
            
            return true;
        }
        
        return false;
    }

    public bool CanAfford(Dictionary<Resource.ResourceType, int> costs)
    {
        foreach (var cost in costs)
        {
            if (!HasResource(cost.Key, cost.Value))
            {
                return false;
            }
        }
        return true;
    }

    public bool SpendResources(Dictionary<Resource.ResourceType, int> costs)
    {
        if (!CanAfford(costs)) return false;
        
        var changedResources = new Dictionary<Resource.ResourceType, int>();
        
        foreach (var cost in costs)
        {
            if (RemoveResource(cost.Key, cost.Value))
            {
                changedResources[cost.Key] = cost.Value;
            }
        }
        
        if (changedResources.Count > 0)
        {
            OnMultipleResourcesChanged?.Invoke(changedResources);
        }
        
        return true;
    }

    public void AddResources(Dictionary<Resource.ResourceType, int> gains)
    {
        var changedResources = new Dictionary<Resource.ResourceType, int>();
        
        foreach (var gain in gains)
        {
            if (AddResource(gain.Key, gain.Value))
            {
                changedResources[gain.Key] = gain.Value;
            }
        }
        
        if (changedResources.Count > 0)
        {
            OnMultipleResourcesChanged?.Invoke(changedResources);
        }
    }

    public void SetResourceCapacity(Resource.ResourceType type, int newCapacity)
    {
        if (resources.ContainsKey(type))
        {
            int previousAmount = resources[type].Amount;
            resources[type].SetCapacity(newCapacity);
            
            if (resources[type].Amount != previousAmount)
            {
                OnResourceChanged?.Invoke(type, previousAmount, resources[type].Amount);
            }
            
            if (debugMode)
            {
                Debug.Log($"Set {type} capacity to {newCapacity}");
            }
        }
    }

    public void UpgradeResourceCapacity(Resource.ResourceType type, int increase)
    {
        if (resources.ContainsKey(type))
        {
            int newCapacity = resources[type].Capacity + increase;
            SetResourceCapacity(type, newCapacity);
        }
    }

    public Dictionary<Resource.ResourceType, int> GetAllResourceAmounts()
    {
        var amounts = new Dictionary<Resource.ResourceType, int>();
        foreach (var kvp in resources)
        {
            amounts[kvp.Key] = kvp.Value.Amount;
        }
        return amounts;
    }

    public Dictionary<Resource.ResourceType, int> GetAllResourceCapacities()
    {
        var capacities = new Dictionary<Resource.ResourceType, int>();
        foreach (var kvp in resources)
        {
            capacities[kvp.Key] = kvp.Value.Capacity;
        }
        return capacities;
    }

    public void ResetResources()
    {
        foreach (var resource in resources.Values)
        {
            resource.SetAmount(0);
        }
        
        if (debugMode)
        {
            Debug.Log("All resources reset to 0");
        }
    }

    public void SetDebugMode(bool enabled)
    {
        debugMode = enabled;
    }

    public string GetResourceSummary()
    {
        string summary = "=== RESOURCE SUMMARY ===\n";
        foreach (var kvp in resources)
        {
            Resource resource = kvp.Value;
            summary += $"{kvp.Key}: {resource.Amount}/{resource.Capacity} ({resource.FillPercentage:P1})\n";
        }
        return summary;
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
    [ContextMenu("Debug: Add 100 Gold")]
    private void DebugAddGold()
    {
        AddResource(Resource.ResourceType.Gold, 100);
    }

    [ContextMenu("Debug: Print Resource Summary")]
    private void DebugPrintSummary()
    {
        Debug.Log(GetResourceSummary());
    }

    [ContextMenu("Debug: Fill All Resources")]
    private void DebugFillAll()
    {
        foreach (var kvp in resources)
        {
            kvp.Value.SetAmount(kvp.Value.Capacity);
        }
    }
    #endif
}