using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExpeditionSystem
{
    private List<Expedition> availableExpeditions;
    private List<Expedition> activeExpeditions;
    private List<Expedition> completedExpeditions;

    public ExpeditionSystem()
    {
        availableExpeditions = new List<Expedition>();
        activeExpeditions = new List<Expedition>();
        completedExpeditions = new List<Expedition>();
        GenerateInitialExpeditions();
    }

    private void GenerateInitialExpeditions()
    {
        // Créer quelques expéditions de base
        availableExpeditions.Add(new Expedition(
            "Exploration de la Forêt Sombre",
            "Une mission simple d'exploration dans les bois environnants.",
            ExpeditionDifficulty.Easy,
            2, // 2 heures
            1, 3,
            "Forêt Sombre"
        ));

        availableExpeditions.Add(new Expedition(
            "Ruines Antiques",
            "Explorez des ruines mystérieuses pour trouver des trésors anciens.",
            ExpeditionDifficulty.Medium,
            4, // 4 heures
            2, 4,
            "Ruines du Temple Oublié"
        ));

        availableExpeditions.Add(new Expedition(
            "Antre du Dragon",
            "Mission extrêmement dangereuse dans l'antre d'un dragon.",
            ExpeditionDifficulty.Extreme,
            8, // 8 heures
            3, 5,
            "Mont Dracorium"
        ));

        availableExpeditions.Add(new Expedition(
            "Patrouille des Frontières",
            "Patrouiller les frontières de la cité contre les bandits.",
            ExpeditionDifficulty.Easy,
            3, // 3 heures
            2, 3,
            "Frontières Nord"
        ));

        availableExpeditions.Add(new Expedition(
            "Cavernes Maudites",
            "Explorer des cavernes infestées de créatures maléfiques.",
            ExpeditionDifficulty.Hard,
            6, // 6 heures
            3, 4,
            "Cavernes de l'Ombre"
        ));
    }

    public List<Expedition> GetAvailableExpeditions()
    {
        return availableExpeditions;
    }

    public List<Expedition> GetActiveExpeditions()
    {
        UpdateActiveExpeditions();
        return activeExpeditions;
    }

    public List<Expedition> GetCompletedExpeditions()
    {
        return completedExpeditions;
    }

    public bool CanAssignAdventurer(string expeditionId, string adventurerId, List<Adventurer> allAdventurers)
    {
        var expedition = availableExpeditions.FirstOrDefault(e => e.id == expeditionId);
        if (expedition == null) return false;

        var adventurer = allAdventurers.FirstOrDefault(a => a.id == adventurerId);
        if (adventurer == null) return false;

        // Vérifier si l'aventurier n'est pas déjà en expédition
        bool isAlreadyOnExpedition = activeExpeditions.Any(e => e.assignedAdventurerIds.Contains(adventurerId));
        if (isAlreadyOnExpedition) return false;

        // Vérifier si l'expédition n'est pas pleine
        return expedition.assignedAdventurerIds.Count < expedition.maxAdventurers;
    }

    public bool AssignAdventurerToExpedition(string expeditionId, string adventurerId, List<Adventurer> allAdventurers)
    {
        if (!CanAssignAdventurer(expeditionId, adventurerId, allAdventurers)) return false;

        var expedition = availableExpeditions.FirstOrDefault(e => e.id == expeditionId);
        if (expedition != null && !expedition.assignedAdventurerIds.Contains(adventurerId))
        {
            expedition.assignedAdventurerIds.Add(adventurerId);
            return true;
        }
        return false;
    }

    public bool RemoveAdventurerFromExpedition(string expeditionId, string adventurerId)
    {
        var expedition = availableExpeditions.FirstOrDefault(e => e.id == expeditionId);
        if (expedition != null)
        {
            return expedition.assignedAdventurerIds.Remove(adventurerId);
        }
        return false;
    }

    public bool StartExpedition(string expeditionId)
    {
        var expedition = availableExpeditions.FirstOrDefault(e => e.id == expeditionId);
        if (expedition != null && expedition.CanStartExpedition())
        {
            expedition.StartExpedition();
            availableExpeditions.Remove(expedition);
            activeExpeditions.Add(expedition);
            return true;
        }
        return false;
    }

    public List<Adventurer> GetAvailableAdventurers(List<Adventurer> allAdventurers)
    {
        var busyAdventurerIds = activeExpeditions.SelectMany(e => e.assignedAdventurerIds).ToHashSet();
        return allAdventurers.Where(a => !busyAdventurerIds.Contains(a.id)).ToList();
    }

    private void UpdateActiveExpeditions()
    {
        var completedExpeditionsThisUpdate = new List<Expedition>();

        foreach (var expedition in activeExpeditions.ToList())
        {
            if (expedition.IsCompleted())
            {
                // Simuler le succès/échec (ici simplifié à 80% de succès)
                bool success = UnityEngine.Random.Range(0f, 1f) > 0.2f;
                expedition.CompleteExpedition(success);
                
                completedExpeditionsThisUpdate.Add(expedition);
                activeExpeditions.Remove(expedition);
                completedExpeditions.Add(expedition);

                // Générer des récompenses
                if (success)
                {
                    GenerateExpeditionRewards(expedition);
                }

                Debug.Log($"Expédition '{expedition.name}' terminée avec {(success ? "succès" : "échec")}");
            }
        }
    }

    private void GenerateExpeditionRewards(Expedition expedition)
    {
        // Générer des récompenses basées sur la difficulté
        int baseGold = (int)expedition.difficulty * 50;
        int goldReward = UnityEngine.Random.Range(baseGold, baseGold * 2);
        
        expedition.rewards.Add(new Resource(ResourceType.Gold, goldReward));

        // Chance d'obtenir des matériaux rares
        if (UnityEngine.Random.Range(0f, 1f) < 0.3f)
        {
            int materialsReward = UnityEngine.Random.Range(1, (int)expedition.difficulty * 3);
            expedition.rewards.Add(new Resource(ResourceType.Materials, materialsReward));
        }

        // Chance d'obtenir des cristaux magiques pour les expéditions difficiles
        if (expedition.difficulty >= ExpeditionDifficulty.Hard && UnityEngine.Random.Range(0f, 1f) < 0.2f)
        {
            int crystalsReward = UnityEngine.Random.Range(1, 3);
            expedition.rewards.Add(new Resource(ResourceType.MagicCrystals, crystalsReward));
        }
    }

    public void GenerateNewExpeditions(int count = 1)
    {
        string[] expeditionNames = {
            "Chasse aux Gobelins", "Sauvetage de Caravane", "Exploration de Donjon",
            "Mission Diplomatique", "Chasse au Trésor", "Élimination de Bandits",
            "Collecte d'Herbes Rares", "Escorte de Marchand", "Investigation Mystérieuse"
        };

        string[] locations = {
            "Marais Brumeux", "Collines Rocheuses", "Vallée Perdue", "Pic Enneigé",
            "Plaines Venteuses", "Lac Cristallin", "Désert Rouge", "Forêt Enchantée"
        };

        for (int i = 0; i < count; i++)
        {
            string name = expeditionNames[UnityEngine.Random.Range(0, expeditionNames.Length)];
            string location = locations[UnityEngine.Random.Range(0, locations.Length)];
            ExpeditionDifficulty difficulty = (ExpeditionDifficulty)UnityEngine.Random.Range(1, 5);
            int duration = UnityEngine.Random.Range(1, 8);
            int minAdv = UnityEngine.Random.Range(1, 3);
            int maxAdv = minAdv + UnityEngine.Random.Range(1, 4);

            var newExpedition = new Expedition(
                name,
                $"Une mission {difficulty.ToString().ToLower()} à {location}.",
                difficulty,
                duration,
                minAdv,
                maxAdv,
                location
            );

            availableExpeditions.Add(newExpedition);
        }
    }

    public void CollectExpeditionRewards(string expeditionId, City city)
    {
        var expedition = completedExpeditions.FirstOrDefault(e => e.id == expeditionId);
        if (expedition != null && expedition.status == ExpeditionStatus.Completed)
        {
            foreach (var reward in expedition.rewards)
            {
                city.AddResource(reward.resourceType, reward.amount);
            }
            
            Debug.Log($"Récompenses collectées pour l'expédition '{expedition.name}'");
        }
    }
}