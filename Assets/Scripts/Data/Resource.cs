using System;
using UnityEngine;

[System.Serializable]
public class Resource
{
    public enum ResourceType
    {
        Gold,
        Population,
        Stone,
        Wood,
        Iron,
        MagicCrystal,
        Reputation
    }

    [SerializeField] private ResourceType type;
    [SerializeField] private int amount;
    [SerializeField] private int capacity;

    public ResourceType Type => type;
    public int Amount => amount;
    public int Capacity => capacity;
    public bool IsFull => amount >= capacity;
    public bool IsEmpty => amount <= 0;
    public float FillPercentage => capacity > 0 ? (float)amount / capacity : 0f;

    public Resource(ResourceType type, int initialAmount = 0, int maxCapacity = int.MaxValue)
    {
        this.type = type;
        this.amount = Mathf.Clamp(initialAmount, 0, maxCapacity);
        this.capacity = maxCapacity;
    }

    public bool CanAdd(int value)
    {
        return amount + value <= capacity;
    }

    public bool CanRemove(int value)
    {
        return amount >= value;
    }

    public bool Add(int value)
    {
        if (value <= 0) return false;
        
        int newAmount = amount + value;
        if (newAmount > capacity) return false;
        
        amount = newAmount;
        return true;
    }

    public bool Remove(int value)
    {
        if (value <= 0) return false;
        if (amount < value) return false;
        
        amount -= value;
        return true;
    }

    public void SetAmount(int newAmount)
    {
        amount = Mathf.Clamp(newAmount, 0, capacity);
    }

    public void SetCapacity(int newCapacity)
    {
        capacity = Mathf.Max(0, newCapacity);
        amount = Mathf.Min(amount, capacity);
    }

    public override string ToString()
    {
        return $"{type}: {amount}/{capacity}";
    }
}