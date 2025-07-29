using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Adventurer
{
    [Header("Basic Information")]
    public string id;
    public string name;
    public AdventurerClass adventurerClass;
    public int level;
    public int experience;
    public int experienceToNextLevel;
    
    [Header("Statistics")]
    public int strength;
    public int intelligence;
    public int agility;
    public int charisma;
    public int luck;
    
    [Header("Health & Status")]
    public int currentHealth;
    public int maxHealth;
    public bool isInjured;
    public bool isAvailable;
    public int recoveryDays;
    
    [Header("Equipment")]
    public Equipment weapon;
    public Equipment armor;
    public Equipment accessory;
    
    [Header("Mission Info")]
    public bool isOnMission;
    public string currentMissionId;
    
    public Adventurer()
    {
        id = System.Guid.NewGuid().ToString();
        name = "Aventurier";
        adventurerClass = AdventurerClass.Warrior;
        level = 1;
        experience = 0;
        experienceToNextLevel = 100;
        
        strength = 10;
        intelligence = 10;
        agility = 10;
        charisma = 10;
        luck = 10;
        
        maxHealth = 100;
        currentHealth = maxHealth;
        isInjured = false;
        isAvailable = true;
        recoveryDays = 0;
        
        weapon = null;
        armor = null;
        accessory = null;
        
        isOnMission = false;
        currentMissionId = "";
    }

    public Adventurer(string adventurerName, AdventurerClass classType)
    {
        id = System.Guid.NewGuid().ToString();
        name = adventurerName;
        adventurerClass = classType;
        level = 1;
        experience = 0;
        experienceToNextLevel = 100;
        
        GenerateBaseStats(classType);
        
        maxHealth = 80 + (strength * 2);
        currentHealth = maxHealth;
        isInjured = false;
        isAvailable = true;
        recoveryDays = 0;
        
        weapon = null;
        armor = null;
        accessory = null;
        
        isOnMission = false;
        currentMissionId = "";
    }

    private void GenerateBaseStats(AdventurerClass classType)
    {
        switch (classType)
        {
            case AdventurerClass.Warrior:
                strength = Random.Range(12, 16);
                intelligence = Random.Range(6, 10);
                agility = Random.Range(8, 12);
                charisma = Random.Range(6, 10);
                luck = Random.Range(8, 12);
                break;
                
            case AdventurerClass.Mage:
                strength = Random.Range(6, 10);
                intelligence = Random.Range(12, 16);
                agility = Random.Range(6, 10);
                charisma = Random.Range(8, 12);
                luck = Random.Range(8, 12);
                break;
                
            case AdventurerClass.Rogue:
                strength = Random.Range(8, 12);
                intelligence = Random.Range(8, 12);
                agility = Random.Range(12, 16);
                charisma = Random.Range(6, 10);
                luck = Random.Range(10, 14);
                break;
                
            case AdventurerClass.Cleric:
                strength = Random.Range(8, 12);
                intelligence = Random.Range(10, 14);
                agility = Random.Range(6, 10);
                charisma = Random.Range(12, 16);
                luck = Random.Range(8, 12);
                break;
                
            case AdventurerClass.Ranger:
                strength = Random.Range(10, 14);
                intelligence = Random.Range(8, 12);
                agility = Random.Range(10, 14);
                charisma = Random.Range(8, 12);
                luck = Random.Range(10, 14);
                break;
        }
    }

    public int GetTotalPower()
    {
        int basePower = (strength + intelligence + agility + charisma + luck) * level;
        int equipmentPower = 0;
        
        if (weapon != null) equipmentPower += weapon.power;
        if (armor != null) equipmentPower += armor.power;
        if (accessory != null) equipmentPower += accessory.power;
        
        return basePower + equipmentPower;
    }

    public void GainExperience(int exp)
    {
        experience += exp;
        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        while (experience >= experienceToNextLevel)
        {
            experience -= experienceToNextLevel;
            level++;
            LevelUp();
            experienceToNextLevel = Mathf.RoundToInt(experienceToNextLevel * 1.2f);
        }
    }

    private void LevelUp()
    {
        strength += Random.Range(1, 4);
        intelligence += Random.Range(1, 4);
        agility += Random.Range(1, 4);
        charisma += Random.Range(1, 4);
        luck += Random.Range(1, 4);
        
        int healthIncrease = Random.Range(5, 15);
        maxHealth += healthIncrease;
        currentHealth = maxHealth;
        
        Debug.Log($"{name} a atteint le niveau {level}!");
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isInjured = true;
            isAvailable = false;
            recoveryDays = Random.Range(3, 8);
        }
        else if (currentHealth < maxHealth * 0.3f)
        {
            isInjured = true;
            isAvailable = false;
            recoveryDays = Random.Range(1, 4);
        }
    }

    public void Heal(int healAmount)
    {
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
        if (currentHealth > maxHealth * 0.3f)
        {
            isInjured = false;
            isAvailable = true;
            recoveryDays = 0;
        }
    }

    public void ProcessRecovery()
    {
        if (recoveryDays > 0)
        {
            recoveryDays--;
            if (recoveryDays == 0)
            {
                isInjured = false;
                isAvailable = true;
                currentHealth = Mathf.Max(currentHealth, Mathf.RoundToInt(maxHealth * 0.5f));
            }
        }
    }

    public bool CanGoOnMission()
    {
        return isAvailable && !isOnMission && !isInjured && currentHealth > 0;
    }
}