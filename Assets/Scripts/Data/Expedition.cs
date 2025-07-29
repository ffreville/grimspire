using System;
using System.Collections.Generic;

[System.Serializable]
public class Expedition
{
    public string id;
    public string name;
    public string description;
    public ExpeditionStatus status;
    public ExpeditionDifficulty difficulty;
    public int duration; // en heures
    public int minAdventurers;
    public int maxAdventurers;
    public List<string> assignedAdventurerIds;
    public DateTime startTime;
    public DateTime endTime;
    public List<Resource> rewards;
    public int experienceReward;
    public string location;

    public Expedition()
    {
        id = Guid.NewGuid().ToString();
        assignedAdventurerIds = new List<string>();
        rewards = new List<Resource>();
        status = ExpeditionStatus.Available;
    }

    public Expedition(string expeditionName, string expeditionDescription, ExpeditionDifficulty expeditionDifficulty, 
                     int expeditionDuration, int minAdv, int maxAdv, string expeditionLocation)
    {
        id = Guid.NewGuid().ToString();
        name = expeditionName;
        description = expeditionDescription;
        difficulty = expeditionDifficulty;
        duration = expeditionDuration;
        minAdventurers = minAdv;
        maxAdventurers = maxAdv;
        location = expeditionLocation;
        assignedAdventurerIds = new List<string>();
        rewards = new List<Resource>();
        status = ExpeditionStatus.Available;
        experienceReward = (int)difficulty * 100;
    }

    public bool CanStartExpedition()
    {
        return status == ExpeditionStatus.Available && 
               assignedAdventurerIds.Count >= minAdventurers &&
               assignedAdventurerIds.Count <= maxAdventurers;
    }

    public void StartExpedition()
    {
        if (CanStartExpedition())
        {
            status = ExpeditionStatus.InProgress;
            startTime = DateTime.Now;
            endTime = startTime.AddHours(duration);
        }
    }

    public bool IsCompleted()
    {
        return status == ExpeditionStatus.InProgress && DateTime.Now >= endTime;
    }

    public void CompleteExpedition(bool success = true)
    {
        status = success ? ExpeditionStatus.Completed : ExpeditionStatus.Failed;
    }

    public float GetProgressPercentage()
    {
        if (status != ExpeditionStatus.InProgress) return 0f;
        
        var totalDuration = endTime - startTime;
        var elapsed = DateTime.Now - startTime;
        
        return Math.Min(1f, (float)(elapsed.TotalSeconds / totalDuration.TotalSeconds));
    }

    public string GetFormattedTimeRemaining()
    {
        if (status != ExpeditionStatus.InProgress) return "";
        
        var remaining = endTime - DateTime.Now;
        if (remaining.TotalSeconds <= 0) return "Terminé";
        
        if (remaining.TotalHours >= 1)
            return $"{remaining.Hours}h {remaining.Minutes}m";
        else
            return $"{remaining.Minutes}m {remaining.Seconds}s";
    }
}