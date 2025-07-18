using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class City
{
    [Header("City Information")]
    public string cityName;
    public int population;
    public int maxPopulation;
    public int reputation;
    
    [Header("Resources")]
    public int gold;
    public int wood;
    public int stone;
    public int iron;
    public int magicCrystals;
    
    [Header("Buildings")]
    public List<Building> buildings;
    
    [Header("Adventurers")]
    public List<Adventurer> adventurers;
    public int maxAdventurers;
    
    [Header("Game State")]
    public bool isDay;
    public int currentDay;
    public int actionPointsDay;
    public int actionPointsNight;
    public int maxActionPointsDay;
    public int maxActionPointsNight;

    public City()
    {
        cityName = "Grimspire";
        population = 100;
        maxPopulation = 150;
        reputation = 0;
        
        gold = 1000;
        wood = 50;
        stone = 30;
        iron = 10;
        magicCrystals = 5;
        
        buildings = new List<Building>();
        adventurers = new List<Adventurer>();
        maxAdventurers = 5;
        
        isDay = true;
        currentDay = 1;
        actionPointsDay = 3;
        actionPointsNight = 2;
        maxActionPointsDay = 3;
        maxActionPointsNight = 2;
    }

    public bool HasEnoughResources(int requiredGold, int requiredWood = 0, int requiredStone = 0, int requiredIron = 0, int requiredMagic = 0)
    {
        return gold >= requiredGold && 
               wood >= requiredWood && 
               stone >= requiredStone && 
               iron >= requiredIron && 
               magicCrystals >= requiredMagic;
    }

    public void SpendResources(int spentGold, int spentWood = 0, int spentStone = 0, int spentIron = 0, int spentMagic = 0)
    {
        gold -= spentGold;
        wood -= spentWood;
        stone -= spentStone;
        iron -= spentIron;
        magicCrystals -= spentMagic;
    }

    public void AddResources(int addedGold, int addedWood = 0, int addedStone = 0, int addedIron = 0, int addedMagic = 0)
    {
        gold += addedGold;
        wood += addedWood;
        stone += addedStone;
        iron += addedIron;
        magicCrystals += addedMagic;
    }

    public bool CanPerformAction()
    {
        return isDay ? actionPointsDay > 0 : actionPointsNight > 0;
    }

    public void SpendActionPoint()
    {
        if (isDay && actionPointsDay > 0)
            actionPointsDay--;
        else if (!isDay && actionPointsNight > 0)
            actionPointsNight--;
    }

    public void SwitchDayNight()
    {
        isDay = !isDay;
        if (isDay)
        {
            currentDay++;
            actionPointsDay = maxActionPointsDay;
        }
        else
        {
            actionPointsNight = maxActionPointsNight;
        }
    }
}