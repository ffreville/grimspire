/**
 * Classe AchievementManager - Gestionnaire des succès/achievements
 */
class AchievementManager {
    constructor(city) {
        this.city = city;
        this.achievements = [];
        this.initializeAchievements();
    }

    initializeAchievements() {
        // Créer tous les succès disponibles
        this.achievements = [
            this.createBuildingAchievements(),
            this.createAdventurerAchievements(),
            this.createResourceAchievements(),
            this.createProgressionAchievements(),
            this.createSpecialAchievements()
        ].flat();
    }

    createBuildingAchievements() {
        const achievements = [];
        
        // Premier bâtiment
        const firstBuilding = new Achievement(
            'first_building',
            'Premier pas',
            'Construisez votre premier bâtiment dans la cité',
            'Construire 1 bâtiment',
            'construction'
        );
        firstBuilding.icon = '🏗️';
        firstBuilding.rewards = { reputation: 5 };
        achievements.push(firstBuilding);
        
        // Mairie
        const cityHall = new Achievement(
            'city_hall',
            'Fondateur',
            'Construisez la mairie pour officialiser votre cité',
            'Construire une mairie',
            'construction'
        );
        cityHall.icon = '🏛️';
        cityHall.rewards = { reputation: 10 };
        achievements.push(cityHall);
        
        // Constructeur expérimenté
        const builder = new Achievement(
            'builder',
            'Architecte',
            'Construisez 5 bâtiments dans votre cité',
            'Construire 5 bâtiments',
            'construction'
        );
        builder.icon = '🏘️';
        builder.rewards = { gold: 200, reputation: 15 };
        achievements.push(builder);
        
        return achievements;
    }

    createAdventurerAchievements() {
        const achievements = [];
        
        // Premier aventurier
        const firstAdventurer = new Achievement(
            'first_adventurer',
            'Recruteur',
            'Recrutez votre premier aventurier',
            'Recruter 1 aventurier',
            'management'
        );
        firstAdventurer.icon = '⚔️';
        firstAdventurer.rewards = { reputation: 5 };
        achievements.push(firstAdventurer);
        
        // Maître de guilde
        const guildMaster = new Achievement(
            'guild_master',
            'Maître de Guilde',
            'Dirigez une guilde de 10 aventuriers',
            'Recruter 10 aventuriers',
            'management'
        );
        guildMaster.icon = '👥';
        guildMaster.rewards = { gold: 500, reputation: 25 };
        achievements.push(guildMaster);
        
        // Première mission
        const firstMission = new Achievement(
            'first_mission',
            'Explorateur',
            'Terminez votre première mission d\'exploration',
            'Terminer 1 mission',
            'exploration'
        );
        firstMission.icon = '🗺️';
        firstMission.rewards = { materials: 50, reputation: 10 };
        achievements.push(firstMission);
        
        return achievements;
    }

    createResourceAchievements() {
        const achievements = [];
        
        // Accumulation d'or
        const goldHoarder = new Achievement(
            'gold_hoarder',
            'Coffres pleins',
            'Accumulez 1000 pièces d\'or',
            'Posséder 1000 or',
            'economy'
        );
        goldHoarder.icon = '💰';
        goldHoarder.rewards = { reputation: 15 };
        achievements.push(goldHoarder);
        
        // Population
        const population100 = new Achievement(
            'population_100',
            'Ville prospère',
            'Atteignez une population de 100 habitants',
            'Atteindre 100 de population',
            'management'
        );
        population100.icon = '👥';
        population100.rewards = { gold: 300, reputation: 20 };
        achievements.push(population100);
        
        return achievements;
    }

    createProgressionAchievements() {
        const achievements = [];
        
        // Survivant des premiers jours
        const daySurvivor = new Achievement(
            'day_survivor',
            'Survivant',
            'Survivez pendant 7 jours dans votre cité',
            'Atteindre le jour 7',
            'general'
        );
        daySurvivor.icon = '📅';
        daySurvivor.rewards = { gold: 100, reputation: 10 };
        achievements.push(daySurvivor);
        
        // Chercheur
        const researcher = new Achievement(
            'researcher',
            'Érudit',
            'Terminez votre première recherche d\'amélioration',
            'Terminer 1 recherche',
            'general'
        );
        researcher.icon = '🔬';
        researcher.rewards = { magic: 25, reputation: 15 };
        achievements.push(researcher);
        
        return achievements;
    }

    createSpecialAchievements() {
        const achievements = [];
        
        // Succès secret
        const secretAchievement = new Achievement(
            'secret_discoverer',
            '???',
            'Un mystère attend d\'être découvert...',
            'Découvrir le secret',
            'special'
        );
        secretAchievement.icon = '❓';
        secretAchievement.isSecret = true;
        secretAchievement.rewards = { magic: 100, reputation: 50 };
        achievements.push(secretAchievement);
        
        return achievements;
    }

    // Vérifier tous les succès et débloquer ceux qui sont éligibles
    checkAchievements() {
        const gameState = this.getGameState();
        const newlyUnlocked = [];
        
        this.achievements.forEach(achievement => {
            if (achievement.checkUnlock(gameState)) {
                newlyUnlocked.push(achievement);
                
                // Créer un événement pour le nouveau succès
                if (this.city.eventManager) {
                    this.city.eventManager.createEvent(
                        'achievement_unlocked',
                        `Succès débloqué : ${achievement.name}`,
                        `Félicitations ! Vous avez débloqué le succès "${achievement.name}". ${achievement.description}`,
                        {
                            achievementId: achievement.id,
                            rewards: achievement.rewards
                        }
                    );
                }
                
                // Appliquer les récompenses
                this.applyRewards(achievement.rewards);
            }
        });
        
        return newlyUnlocked;
    }

    // Obtenir l'état actuel du jeu pour vérification des succès
    getGameState() {
        if (!this.city) return {};
        
        return {
            buildings: this.city.buildings || [],
            adventurers: this.city.adventurers || [],
            resources: this.city.resources || {},
            day: this.city.day || 1,
            completedMissions: this.city.missionManager ? this.city.missionManager.getCompletedMissionsCount() : 0,
            unlockedUpgrades: this.city.cityUpgradeManager ? this.city.cityUpgradeManager.getUnlockedUpgrades() : []
        };
    }

    // Appliquer les récompenses d'un succès
    applyRewards(rewards) {
        if (!rewards || !this.city.resources) return;
        
        Object.entries(rewards).forEach(([resource, amount]) => {
            if (this.city.resources[resource] !== undefined) {
                this.city.resources[resource] += amount;
            }
        });
    }

    // Obtenir un succès par ID
    getAchievementById(achievementId) {
        return this.achievements.find(a => a.id === achievementId);
    }

    // Obtenir tous les succès débloqués
    getUnlockedAchievements() {
        return this.achievements.filter(a => a.unlocked).map(a => a.getDisplayInfo());
    }

    // Obtenir tous les succès verrouillés
    getLockedAchievements() {
        return this.achievements.filter(a => !a.unlocked).map(a => a.getDisplayInfo());
    }

    // Obtenir tous les succès
    getAllAchievements() {
        return this.achievements.map(a => a.getDisplayInfo());
    }

    // Obtenir les statistiques des succès
    getAchievementStats() {
        const total = this.achievements.length;
        const unlocked = this.achievements.filter(a => a.unlocked).length;
        const progress = total > 0 ? Math.round((unlocked / total) * 100) : 0;
        
        const lastUnlock = this.achievements
            .filter(a => a.unlocked && a.unlockedDate)
            .sort((a, b) => b.unlockedDate - a.unlockedDate)[0];
        
        return {
            total,
            unlocked,
            locked: total - unlocked,
            progress,
            lastUnlock: lastUnlock ? lastUnlock.name : '-'
        };
    }

    // Débloquer manuellement un succès (pour les tests)
    unlockAchievement(achievementId) {
        const achievement = this.getAchievementById(achievementId);
        if (achievement && !achievement.unlocked) {
            achievement.unlock();
            this.applyRewards(achievement.rewards);
            return true;
        }
        return false;
    }

    // Sérialisation pour la sauvegarde
    toJSON() {
        return {
            achievements: this.achievements.map(a => a.toJSON())
        };
    }

    static fromJSON(data, city) {
        const manager = new AchievementManager(city);
        
        if (data.achievements) {
            manager.achievements = data.achievements.map(achievementData => 
                Achievement.fromJSON(achievementData)
            );
        }
        
        return manager;
    }
}

if (typeof module !== 'undefined' && module.exports) {
    module.exports = AchievementManager;
}