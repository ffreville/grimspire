/**
 * Classe GameManager - Gestionnaire principal du jeu
 */
class GameManager {
    constructor() {
        this.city = null;
        this.adventurerManager = null;
        this.missionManager = null;
        this.cityUpgradeManager = null;
        this.buildingManager = null;
        this.gameState = 'menu'; // 'menu', 'playing', 'paused'
        this.currentTab = 'batiments';
        this.saveKey = 'grimspire_save';
        this.gameTimer = null;
        
        // Callbacks pour les mises à jour UI
        this.onStateChange = null;
        this.onTabChange = null;
        this.onResourcesChange = null;
    }

    startNewGame() {
        console.log('Démarrage d\'une nouvelle partie...');
        
        // Créer une nouvelle ville
        this.city = new City();
        
        // Ajouter quelques aventuriers de départ
        this.addStartingAdventurers();
        
        // Créer le gestionnaire d'aventuriers
        this.adventurerManager = new AdventurerManager(this.city);
        
        // Créer le gestionnaire de missions
        this.missionManager = new MissionManager(this.city);
        
        // Créer le gestionnaire d'améliorations de ville
        this.cityUpgradeManager = new CityUpgradeManager(this.city);
        
        // Créer le gestionnaire de bâtiments
        this.buildingManager = new BuildingManager(this.city, this.cityUpgradeManager);
        
        // Configurer les callbacks
        this.city.setNewDayCallback(this.processNewDay.bind(this));
        
        // Changer l'état du jeu
        this.gameState = 'playing';
        
        // Sauvegarder automatiquement
        this.autoSave();
        
        // Démarrer le timer de jeu
        this.startGameTimer();
        
        // Notifier les changements
        this.notifyStateChange();
        
        return this.city.getGameState();
    }

    addStartingAdventurers() {
        // Aucun aventurier de départ - ils doivent être recrutés
        // La méthode est conservée pour la compatibilité mais ne fait rien
    }

    loadGame() {
        try {
            const saveData = localStorage.getItem(this.saveKey);
            if (saveData) {
                const parsedData = JSON.parse(saveData);
                // Pour charger les bâtiments, on a besoin des types disponibles
                const tempUpgradeManager = new CityUpgradeManager(null);
                const tempBuildingManager = new BuildingManager(null, tempUpgradeManager);
                const buildingTypes = tempBuildingManager.buildingTypes;
                
                this.city = City.fromJSON(parsedData.city, buildingTypes);
                this.gameState = parsedData.gameState || 'playing';
                this.currentTab = parsedData.currentTab || 'batiments';
                
                // Recréer le gestionnaire d'aventuriers
                if (parsedData.adventurerManager) {
                    this.adventurerManager = AdventurerManager.fromJSON(parsedData.adventurerManager, this.city);
                } else {
                    this.adventurerManager = new AdventurerManager(this.city);
                }
                
                // Recréer le gestionnaire de missions
                if (parsedData.missionManager) {
                    this.missionManager = MissionManager.fromJSON(parsedData.missionManager, this.city);
                } else {
                    this.missionManager = new MissionManager(this.city);
                }
                
                // Recréer le gestionnaire d'améliorations de ville
                if (parsedData.cityUpgradeManager) {
                    this.cityUpgradeManager = CityUpgradeManager.fromJSON(parsedData.cityUpgradeManager, this.city);
                } else {
                    this.cityUpgradeManager = new CityUpgradeManager(this.city);
                }
                
                // Recréer le gestionnaire de bâtiments
                if (parsedData.buildingManager) {
                    this.buildingManager = BuildingManager.fromJSON(parsedData.buildingManager, this.city, this.cityUpgradeManager);
                } else {
                    this.buildingManager = new BuildingManager(this.city, this.cityUpgradeManager);
                }
                
                // Configurer les callbacks
                this.city.setNewDayCallback(this.processNewDay.bind(this));
                
                // Démarrer le timer de jeu si pas déjà démarré
                this.startGameTimer();
                
                this.notifyStateChange();
                return true;
            }
        } catch (error) {
            console.error('Erreur lors du chargement de la sauvegarde:', error);
        }
        return false;
    }

    saveGame() {
        if (!this.city) return false;
        
        try {
            const saveData = {
                city: this.city.toJSON(),
                adventurerManager: this.adventurerManager ? this.adventurerManager.toJSON() : null,
                missionManager: this.missionManager ? this.missionManager.toJSON() : null,
                cityUpgradeManager: this.cityUpgradeManager ? this.cityUpgradeManager.toJSON() : null,
                buildingManager: this.buildingManager ? this.buildingManager.toJSON() : null,
                gameState: this.gameState,
                currentTab: this.currentTab,
                timestamp: Date.now()
            };
            
            localStorage.setItem(this.saveKey, JSON.stringify(saveData));
            return true;
        } catch (error) {
            console.error('Erreur lors de la sauvegarde:', error);
            return false;
        }
    }

    autoSave() {
        this.saveGame();
    }

    hasSaveData() {
        return localStorage.getItem(this.saveKey) !== null;
    }

    startGameTimer() {
        if (this.gameTimer) {
            clearInterval(this.gameTimer);
        }
        
        // 24h jeu (1 jour) = 5min réel = 300s réel
        // Donc 15min jeu = 300/96 = 3.125s réel
        // L'horloge se met à jour par incréments de 15 minutes
        const gameQuarterHourInMs = (5 * 60 * 1000) / 96; // ≈ 3125ms = 3.125s
        
        this.gameTimer = setInterval(() => {
            if (this.city && !this.city.isPaused) {
                // Avancer le temps de 15 minutes
                this.city.advanceTime(15);
                
                // Avancer la progression des constructions et améliorations (15 min par 15 min)
                if (this.buildingManager) {
                    const buildingProgressResult = this.buildingManager.processTimeProgress(15);
                    
                    // Notifier les constructions/améliorations terminées
                    if (buildingProgressResult.completedBuildings.length > 0 || buildingProgressResult.completedUpgrades.length > 0) {
                        this.handleCompletedConstructions(buildingProgressResult);
                    }
                }

                // Avancer la progression des améliorations de ville (15 min par 15 min)
                if (this.cityUpgradeManager) {
                    const upgradeProgressResult = this.cityUpgradeManager.processTimeProgress(15);
                    
                    // Notifier les améliorations de ville terminées
                    if (upgradeProgressResult.completedUpgrades.length > 0) {
                        this.handleCompletedUpgrades(upgradeProgressResult);
                    }
                }
                
                this.notifyStateChange();
                
                // Sauvegarde automatique toutes les 2 heures de jeu (120 minutes)
                if (this.city.currentTime % 120 === 0) {
                    this.autoSave();
                }
            }
        }, gameQuarterHourInMs);
    }

    stopGameTimer() {
        if (this.gameTimer) {
            clearInterval(this.gameTimer);
            this.gameTimer = null;
        }
    }

    pauseGame() {
        if (this.city) {
            this.city.pauseGame();
            this.notifyStateChange();
        }
    }

    resumeGame() {
        if (this.city) {
            this.city.resumeGame();
            this.notifyStateChange();
        }
    }

    toggleGamePause() {
        if (this.city) {
            this.city.togglePause();
            this.notifyStateChange();
            return this.city.isPaused;
        }
        return false;
    }

    switchTab(tabName) {
        this.currentTab = tabName;
        this.notifyTabChange();
        this.autoSave();
    }

    getCurrentGameState() {
        if (!this.city) return null;
        return this.city.getGameState();
    }

    // Anciennes méthodes de bâtiments pour compatibilité (désormais déléguées au BuildingManager)
    performBuildingAction(buildingId, action) {
        if (!this.buildingManager) return { success: false, message: 'Gestionnaire de bâtiments non initialisé' };
        
        let result;
        switch (action) {
            case 'upgrade':
                result = this.buildingManager.upgradeBuilding(buildingId);
                break;
            default:
                return { success: false, message: 'Action non supportée dans le nouveau système' };
        }
        
        if (result.success) {
            this.notifyResourcesChange();
            this.autoSave();
        }
        
        return result;
    }

    // Nouvelles méthodes pour le système de bâtiments
    constructBuilding(typeId, customName) {
        if (!this.buildingManager) return { success: false, message: 'Gestionnaire de bâtiments non initialisé' };
        
        const result = this.buildingManager.constructBuilding(typeId, customName);
        if (result.success) {
            this.notifyResourcesChange();
            this.notifyStateChange();
            this.autoSave();
        }
        return result;
    }

    upgradeBuilding(buildingId) {
        if (!this.buildingManager) return { success: false, message: 'Gestionnaire de bâtiments non initialisé' };
        
        const result = this.buildingManager.upgradeBuilding(buildingId);
        if (result.success) {
            this.notifyResourcesChange();
            this.notifyStateChange();
            this.autoSave();
        }
        return result;
    }

    demolishBuilding(buildingId) {
        if (!this.buildingManager) return { success: false, message: 'Gestionnaire de bâtiments non initialisé' };
        
        const result = this.buildingManager.demolishBuilding(buildingId);
        if (result.success) {
            this.notifyResourcesChange();
            this.notifyStateChange();
            this.autoSave();
        }
        return result;
    }

    // Méthode appelée quand un nouveau jour commence (à minuit)
    processNewDay() {
        if (!this.city) return;
        
        // Traiter le changement de jour pour les gestionnaires
        if (this.adventurerManager) {
            this.adventurerManager.processTurnChange();
        }
        
        if (this.missionManager) {
            this.missionManager.processTurnChange();
        }
        
        if (this.cityUpgradeManager) {
            this.cityUpgradeManager.processTurnChange();
        }
        
        this.notifyStateChange();
        this.autoSave();
    }

    // Gérer les constructions et améliorations terminées
    handleCompletedConstructions(progressResult) {
        const messages = [];
        
        // Constructions terminées
        progressResult.completedBuildings.forEach(building => {
            messages.push(`🏗️ ${building.customName} construit avec succès !`);
            
            // Vérifier si le bâtiment débloque un onglet
            if (building.buildingType.unlocksTab) {
                messages.push(`🎉 Nouvel onglet débloqué : ${building.buildingType.unlocksTab}`);
            }
        });
        
        // Améliorations terminées
        progressResult.completedUpgrades.forEach(building => {
            messages.push(`⬆️ ${building.customName} amélioré au niveau ${building.level} !`);
        });
        
        // Ici on pourrait déclencher des notifications dans l'interface
        // Pour l'instant on log juste dans la console
        messages.forEach(msg => console.log(msg));
    }

    // Gérer les améliorations de ville terminées
    handleCompletedUpgrades(progressResult) {
        const messages = [];
        
        // Améliorations de ville terminées
        progressResult.completedUpgrades.forEach(upgrade => {
            messages.push(`🔬 Recherche terminée : ${upgrade.name} débloqué !`);
        });
        
        // Ici on pourrait déclencher des notifications dans l'interface
        // Pour l'instant on log juste dans la console
        messages.forEach(msg => console.log(msg));
    }

    addRandomAdventurer() {
        if (!this.city) return null;
        
        const names = ['Marcus', 'Elena', 'Thorin', 'Aria', 'Cedric', 'Luna', 'Ragnar', 'Iris'];
        const classes = ['guerrier', 'mage', 'voleur', 'clerc', 'ranger'];
        
        const randomName = names[Math.floor(Math.random() * names.length)];
        const randomClass = classes[Math.floor(Math.random() * classes.length)];
        const adventurerId = `adv_${Date.now()}`;
        
        const newAdventurer = new Adventurer(adventurerId, randomName, randomClass);
        this.city.addAdventurer(newAdventurer);
        
        this.autoSave();
        return newAdventurer.getDisplayInfo();
    }

    getResourcesInfo() {
        if (!this.city) return null;
        return this.city.resources.toJSON();
    }

    getBuildingsInfo() {
        if (!this.buildingManager) return [];
        return this.buildingManager.getConstructedBuildings();
    }

    getBuildingTypesInfo() {
        if (!this.buildingManager) return { available: [], locked: [], all: [] };
        
        return {
            available: this.buildingManager.getAvailableBuildingTypes(),
            locked: this.buildingManager.getLockedBuildingTypes(),
            all: this.buildingManager.getAllBuildingTypes(),
            stats: this.buildingManager.getBuildingStats()
        };
    }

    hasCityHall() {
        if (!this.city) return false;
        return this.city.buildings.some(building => building.buildingType.id === 'mairie');
    }

    hasCommercialBuildings() {
        if (!this.city) return { hasAny: false, marche: false, artisan: false, banque: false };
        
        const marche = this.city.buildings.some(building => building.buildingType.id === 'marche');
        const artisan = this.city.buildings.some(building => building.buildingType.id === 'echoppe_artisan');
        const banque = this.city.buildings.some(building => building.buildingType.id === 'banque');
        
        return {
            hasAny: marche || artisan || banque,
            marche,
            artisan,
            banque
        };
    }

    hasIndustrialBuildings() {
        if (!this.city) return { hasAny: false, forge: false, alchimiste: false, enchanteur: false };
        
        const forge = this.city.buildings.some(building => building.buildingType.id === 'forge');
        const alchimiste = this.city.buildings.some(building => building.buildingType.id === 'alchimiste');
        const enchanteur = this.city.buildings.some(building => building.buildingType.id === 'enchanteur');
        
        return {
            hasAny: forge || alchimiste || enchanteur,
            forge,
            alchimiste,
            enchanteur
        };
    }

    getDailyGains() {
        if (!this.city) return { gold: 0, population: 0, materials: 0, magic: 0, reputation: 0 };
        
        const builtBuildings = this.city.getBuiltBuildings();
        let dailyGain = { gold: 0, population: 0, materials: 0, magic: 0, reputation: 0 };
        
        builtBuildings.forEach(building => {
            const effects = building.effects;
            
            if (effects.goldPerTurn) dailyGain.gold += effects.goldPerTurn;
            if (effects.populationPerTurn) dailyGain.population += effects.populationPerTurn;
            if (effects.materialsPerTurn) dailyGain.materials += effects.materialsPerTurn;
            if (effects.magicPerTurn) dailyGain.magic += effects.magicPerTurn;
            if (effects.reputationPerTurn) dailyGain.reputation += effects.reputationPerTurn;
        });
        
        // Plus de revenu de base - chaque bâtiment génère ses propres ressources
        
        return dailyGain;
    }

    getAdventurersInfo() {
        if (!this.city) return [];
        return this.city.adventurers.map(a => a.getDisplayInfo());
    }

    // Système de callbacks pour les mises à jour UI
    setStateChangeCallback(callback) {
        this.onStateChange = callback;
    }

    setTabChangeCallback(callback) {
        this.onTabChange = callback;
    }

    setResourcesChangeCallback(callback) {
        this.onResourcesChange = callback;
    }

    notifyStateChange() {
        if (this.onStateChange) {
            this.onStateChange(this.getCurrentGameState());
        }
    }

    notifyTabChange() {
        if (this.onTabChange) {
            this.onTabChange(this.currentTab);
        }
    }

    notifyResourcesChange() {
        if (this.onResourcesChange) {
            this.onResourcesChange(this.getResourcesInfo());
        }
    }

    // Méthodes utilitaires pour les statistiques
    getGameStats() {
        if (!this.city) return null;
        
        const builtBuildings = this.city.getBuiltBuildings();
        const availableAdventurers = this.city.getAvailableAdventurers();
        
        return {
            day: this.city.day,
            currentTime: this.city.currentTime,
            formattedTime: this.city.getFormattedTime(),
            isPaused: this.city.isPaused,
            totalBuildings: this.city.buildings.length,
            builtBuildings: builtBuildings.length,
            totalAdventurers: this.city.adventurers.length,
            availableAdventurers: availableAdventurers.length
        };
    }

    // Méthodes pour l'onglet Guilde
    searchForAdventurers() {
        if (!this.adventurerManager) return { success: false, message: 'Gestionnaire d\'aventuriers non initialisé' };
        
        const result = this.adventurerManager.searchForAdventurers();
        if (result.success) {
            this.notifyResourcesChange();
            this.notifyStateChange();
            this.autoSave();
        }
        return result;
    }

    recruitAdventurer(adventurerId) {
        if (!this.adventurerManager) return { success: false, message: 'Gestionnaire d\'aventuriers non initialisé' };
        
        const result = this.adventurerManager.recruitAdventurer(adventurerId);
        if (result.success) {
            this.notifyResourcesChange();
            this.notifyStateChange();
            this.autoSave();
        }
        return result;
    }

    dismissAdventurer(adventurerId) {
        if (!this.adventurerManager) return { success: false, message: 'Gestionnaire d\'aventuriers non initialisé' };
        
        const result = this.adventurerManager.dismissAdventurer(adventurerId);
        if (result.success) {
            this.notifyResourcesChange();
            this.autoSave();
        }
        return result;
    }

    getGuildInfo() {
        if (!this.adventurerManager) return null;
        
        return {
            stats: this.adventurerManager.getGuildStats(),
            recruited: this.adventurerManager.getRecruitedAdventurers(),
            available: this.adventurerManager.getRecruitableAdventurers(),
            searchInfo: this.adventurerManager.getSearchInfo()
        };
    }

    // Méthodes pour l'onglet Expéditions
    refreshMissions() {
        if (!this.missionManager) return { success: false, message: 'Gestionnaire de missions non initialisé' };
        
        const result = this.missionManager.refreshMissions();
        if (result.success) {
            this.notifyResourcesChange();
            this.notifyStateChange();
            this.autoSave();
        }
        return result;
    }

    startMission(missionId, selectedAdventurerIds) {
        if (!this.missionManager) return { success: false, message: 'Gestionnaire de missions non initialisé' };
        
        const result = this.missionManager.startMission(missionId, selectedAdventurerIds);
        if (result.success) {
            this.notifyStateChange();
            this.autoSave();
        }
        return result;
    }

    getMissionInfo() {
        if (!this.missionManager) return null;
        
        return {
            stats: this.missionManager.getMissionStats(),
            available: this.missionManager.getAvailableMissions(),
            active: this.missionManager.getActiveMissions(),
            completed: this.missionManager.getCompletedMissions(),
            refreshInfo: this.missionManager.getRefreshInfo()
        };
    }

    // Méthodes pour l'onglet Administration
    unlockUpgrade(upgradeId) {
        if (!this.cityUpgradeManager) return { success: false, message: 'Gestionnaire d\'améliorations non initialisé' };
        
        const result = this.cityUpgradeManager.unlockUpgrade(upgradeId);
        if (result.success) {
            this.notifyResourcesChange();
            this.notifyStateChange();
            this.autoSave();
        }
        return result;
    }

    getUpgradeInfo() {
        if (!this.cityUpgradeManager) return null;
        
        return {
            stats: this.cityUpgradeManager.getUpgradeStats(),
            available: this.cityUpgradeManager.getAvailableUpgrades(),
            unlocked: this.cityUpgradeManager.getUnlockedUpgrades(),
            all: this.cityUpgradeManager.getAllUpgrades()
        };
    }

    isUpgradeUnlocked(upgradeId) {
        if (!this.cityUpgradeManager) return false;
        return this.cityUpgradeManager.isUpgradeUnlocked(upgradeId);
    }

    resetGame() {
        this.stopGameTimer();
        this.city = null;
        this.adventurerManager = null;
        this.missionManager = null;
        this.cityUpgradeManager = null;
        this.buildingManager = null;
        this.gameState = 'menu';
        this.currentTab = 'batiments';
        localStorage.removeItem(this.saveKey);
    }
}

if (typeof module !== 'undefined' && module.exports) {
    module.exports = GameManager;
}