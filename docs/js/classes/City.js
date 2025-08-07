/**
 * Classe City - Représente l'état de la ville de Grimspire
 */
class City {
    constructor(name = "Grimspire") {
        this.name = name;
        this.resources = new Resource();
        this.buildings = [];
        this.adventurers = [];
        this.day = 1;
        this.currentTime = 0; // Heure actuelle en minutes (0-1439, soit 0:00-23:59)
        this.isPaused = false;
        
        // Actions du marché
        this.marketActions = {
            negotiator: {
                isActive: false,
                startTime: null,
                duration: 8 * 60 // 8 heures en minutes (8h * 60min)
            },
            emissary: {
                isActive: false,
                startTime: null,
                duration: 8 * 60 // 8 heures en minutes (8h * 60min)
            }
        };
        
        // Actions des artisans
        this.artisanActions = {
            nightWork: {
                isActive: false,
                startTime: null,
                duration: 2 * 24 * 60, // 2 jours en minutes (2 jours * 24h * 60min)
                effectStartTime: null,
                effectDuration: 1 * 24 * 60 // 1 jour d'effet en minutes
            },
            clearance: {
                isActive: false,
                startTime: null,
                duration: 2 * 24 * 60, // 2 jours en minutes (2 jours * 24h * 60min)
                effectStartTime: null,
                effectDuration: 1 * 24 * 60 // 1 jour d'effet en minutes
            }
        };
        
        // Initialiser les gestionnaires
        this.achievementManager = new AchievementManager(this);
        this.seasonManager = new Season();
        
        // Initialiser les bâtiments de base
        this.initializeStartingBuildings();
    }

    initializeStartingBuildings() {
        // Le nouveau système n'a plus de bâtiments prédéfinis
        // Les bâtiments sont créés à la demande lors de la construction
        // Cette méthode est conservée pour la compatibilité mais ne fait rien
    }

    addBuilding(building) {
        this.buildings.push(building);
    }

    removeBuilding(buildingId) {
        const index = this.buildings.findIndex(b => b.id === buildingId);
        if (index !== -1) {
            this.buildings.splice(index, 1);
            return true;
        }
        return false;
    }

    getBuildingById(buildingId) {
        return this.buildings.find(b => b.id === buildingId);
    }

    getBuildingsByDistrict(district) {
        return this.buildings.filter(b => b.district === district);
    }

    getBuiltBuildings() {
        return this.buildings.filter(b => b.built);
    }

    addAdventurer(adventurer) {
        this.adventurers.push(adventurer);
    }

    removeAdventurer(adventurerId) {
        const index = this.adventurers.findIndex(a => a.id === adventurerId);
        if (index !== -1) {
            this.adventurers.splice(index, 1);
            return true;
        }
        return false;
    }

    getAdventurerById(adventurerId) {
        return this.adventurers.find(a => a.id === adventurerId);
    }

    getAvailableAdventurers() {
        return this.adventurers.filter(a => !a.isOnMission && a.isAlive());
    }

    canPerformAction(cost = 1) {
        // Plus de système de points d'action - toujours autorisé
        return true;
    }

    performAction(cost = 1) {
        // Plus de système de points d'action - toujours réussi
        return true;
    }

    advanceTime(minutes = 1) {
        if (this.isPaused) return;
        
        const previousHour = Math.floor(this.currentTime / 60);
        this.currentTime += minutes;
        
        // Si on dépasse 23:59, nouveau jour
        if (this.currentTime >= 1440) { // 24 * 60 = 1440 minutes
            this.currentTime = 0;
            this.day++;
            this.processDaily();
            
            // Notifier qu'un nouveau jour a commencé
            if (this.onNewDay) {
                this.onNewDay();
            }
        }
        
        const currentHour = Math.floor(this.currentTime / 60);
        
        // Traitement par heure si nécessaire
        if (currentHour !== previousHour) {
            this.processHourly();
        }
    }

    setNewDayCallback(callback) {
        this.onNewDay = callback;
    }

    setMarketActionCallback(callback) {
        this.onMarketActionCompleted = callback;
    }

    setArtisanActionCallback(callback) {
        this.onArtisanActionCompleted = callback;
    }

    pauseGame() {
        this.isPaused = true;
    }

    resumeGame() {
        this.isPaused = false;
    }

    togglePause() {
        this.isPaused = !this.isPaused;
    }

    getFormattedTime() {
        const hours = Math.floor(this.currentTime / 60);
        const minutes = this.currentTime % 60;
        return `${hours.toString().padStart(2, '0')}:${minutes.toString().padStart(2, '0')}`;
    }

    processHourly() {
        // Traitement par heure : génération de ressources et progression des constructions
        this.generateHourlyResources();
        this.processBuildingProgress();
        
        // Vérifier les actions du marché à chaque heure
        const completedMarketActions = this.processMarketActionCompletion();
        
        // Notifier le GameManager des actions du marché terminées (sera géré par le callback)
        if (completedMarketActions.length > 0 && this.onMarketActionCompleted) {
            completedMarketActions.forEach(action => {
                this.onMarketActionCompleted(action);
            });
        }

        // Vérifier les actions des artisans à chaque heure
        const completedArtisanActions = this.processArtisanActionCompletion();
        
        // Notifier le GameManager des actions des artisans terminées (sera géré par le callback)
        if (completedArtisanActions.length > 0 && this.onArtisanActionCompleted) {
            completedArtisanActions.forEach(action => {
                this.onArtisanActionCompleted(action);
            });
        }
    }

    // Nouveau : traitement des constructions/améliorations
    processBuildingProgress() {
        // Cette méthode sera appelée par le GameManager qui a accès au BuildingManager
        // Elle est définie ici pour maintenir la structure mais sera déléguée
    }

    processDaily() {
        // Traitement quotidien : guérison des aventuriers
        this.healAllAdventurers();
    }

    generateHourlyResources() {
        const builtBuildings = this.getBuiltBuildings();
        let hourlyGain = { gold: 0, population: 0, materials: 0, magic: 0, reputation: 0 };
        
        builtBuildings.forEach(building => {
            const effects = building.effects;
            
            if (effects.goldPerHour) hourlyGain.gold += effects.goldPerHour;
            if (effects.populationPerHour) hourlyGain.population += effects.populationPerHour;
            if (effects.materialsPerHour) hourlyGain.materials += effects.materialsPerHour;
            if (effects.magicPerHour) hourlyGain.magic += effects.magicPerHour;
            if (effects.reputationPerHour) hourlyGain.reputation += effects.reputationPerHour;
        });
        
        // Appliquer les effets des actions d'artisans actives
        const artisanEffects = this.getActiveArtisanEffects();
        if (artisanEffects.doubleMaterials) {
            hourlyGain.materials *= 2;
        }
        if (artisanEffects.doubleGold) {
            hourlyGain.gold *= 2;
        }
        
        this.resources.gain(hourlyGain);
        
        return hourlyGain;
    }

    healAllAdventurers() {
        this.adventurers.forEach(adventurer => {
            if (!adventurer.isOnMission) {
                adventurer.heal(20); // Guérison partielle quotidienne
            }
        });
    }

    upgradeBuilding(buildingId) {
        const building = this.getBuildingById(buildingId);
        if (!building) return { success: false, message: 'Bâtiment introuvable' };
        
        if (!building.canUpgrade()) {
            return { success: false, message: 'Amélioration impossible' };
        }
        
        if (!this.resources.canAfford(building.upgradeCost)) {
            return { success: false, message: 'Ressources insuffisantes' };
        }
        
        // Plus de vérification de points d'action
        
        this.resources.spend(building.upgradeCost);
        building.upgrade();
        
        return { success: true, message: `${building.name} amélioré au niveau ${building.level}` };
    }

    buildBuilding(buildingId) {
        const building = this.getBuildingById(buildingId);
        if (!building) return { success: false, message: 'Bâtiment introuvable' };
        
        if (building.built) {
            return { success: false, message: 'Bâtiment déjà construit' };
        }
        
        if (!this.resources.canAfford(building.upgradeCost)) {
            return { success: false, message: 'Ressources insuffisantes' };
        }
        
        // Plus de vérification de points d'action
        
        this.resources.spend(building.upgradeCost);
        building.build();
        
        return { success: true, message: `${building.name} construit avec succès` };
    }

    getSeasonInfo() {
        return this.seasonManager.getSeasonDisplay(this.day);
    }

    // === ACTIONS DU MARCHÉ ===

    startMarketAction(actionType) {
        if (!['negotiator', 'emissary'].includes(actionType)) {
            return { success: false, message: 'Action inconnue' };
        }

        const action = this.marketActions[actionType];
        if (action.isActive) {
            return { success: false, message: 'Cette action est déjà en cours' };
        }

        // Vérifier qu'aucune autre action du marché n'est en cours
        const otherAction = actionType === 'negotiator' ? this.marketActions.emissary : this.marketActions.negotiator;
        if (otherAction.isActive) {
            const otherActionName = actionType === 'negotiator' ? 'émissaire' : 'négociateur';
            return { success: false, message: `Un ${otherActionName} est déjà en mission. Attendez son retour.` };
        }

        // Démarrer l'action
        action.isActive = true;
        action.startTime = this.day * 1440 + this.currentTime; // Convertir en temps absolu en minutes

        return { success: true, message: `${actionType === 'negotiator' ? 'Négociateur' : 'Émissaire'} envoyé avec succès` };
    }

    getMarketActionStatus(actionType) {
        if (!['negotiator', 'emissary'].includes(actionType)) {
            return null;
        }

        const action = this.marketActions[actionType];
        if (!action.isActive) {
            return { isActive: false };
        }

        const currentTime = this.day * 1440 + this.currentTime; // Temps absolu actuel en minutes
        const elapsedMinutes = currentTime - action.startTime;
        const remainingMinutes = Math.max(0, action.duration - elapsedMinutes);
        const isCompleted = elapsedMinutes >= action.duration;

        // Convertir en heures et minutes pour l'affichage
        const remainingHours = Math.floor(remainingMinutes / 60);
        const remainingMins = remainingMinutes % 60;

        return {
            isActive: true,
            elapsedMinutes,
            remainingMinutes,
            remainingHours,
            remainingMins,
            isCompleted,
            startTime: action.startTime
        };
    }

    processMarketActionCompletion() {
        const completedActions = [];

        // Vérifier le négociateur
        const negotiatorStatus = this.getMarketActionStatus('negotiator');
        if (negotiatorStatus && negotiatorStatus.isCompleted && this.marketActions.negotiator.isActive) {
            // Action terminée - donner les récompenses
            const reward = { materials: 100 }; // Gain de matériaux
            this.resources.gain(reward);
            
            // Réinitialiser l'action
            this.marketActions.negotiator.isActive = false;
            this.marketActions.negotiator.startTime = null;
            
            completedActions.push({
                type: 'negotiator',
                reward,
                message: 'Le négociateur est revenu avec de nouveaux matériaux'
            });
        }

        // Vérifier l'émissaire
        const emissaryStatus = this.getMarketActionStatus('emissary');
        if (emissaryStatus && emissaryStatus.isCompleted && this.marketActions.emissary.isActive) {
            // Action terminée - donner les récompenses
            const reward = { gold: 200 }; // Gain d'or
            this.resources.gain(reward);
            
            // Réinitialiser l'action
            this.marketActions.emissary.isActive = false;
            this.marketActions.emissary.startTime = null;
            
            completedActions.push({
                type: 'emissary',
                reward,
                message: 'L\'émissaire a attiré de nouveaux clients vers vos marchés'
            });
        }

        return completedActions;
    }

    // === ACTIONS DES ARTISANS ===

    startArtisanAction(actionType) {
        if (!['nightWork', 'clearance'].includes(actionType)) {
            return { success: false, message: 'Action inconnue' };
        }

        const action = this.artisanActions[actionType];
        if (action.isActive) {
            return { success: false, message: 'Cette action est déjà en cours' };
        }

        // Vérifier qu'aucune autre action des artisans n'est en cours (exclusivité)
        const otherActionType = actionType === 'nightWork' ? 'clearance' : 'nightWork';
        const otherAction = this.artisanActions[otherActionType];
        if (otherAction.isActive) {
            const otherActionName = actionType === 'nightWork' ? 'soldes' : 'travail de nuit';
            return { success: false, message: `Les ${otherActionName} sont déjà en cours. Attendez leur fin.` };
        }

        // Démarrer l'action
        action.isActive = true;
        action.startTime = this.day * 1440 + this.currentTime; // Convertir en temps absolu en minutes
        // Lancer l'effet immédiatement
        action.effectStartTime = this.day * 1440 + this.currentTime;

        return { success: true, message: `${actionType === 'nightWork' ? 'Travail de nuit' : 'Soldes'} lancé avec succès` };
    }

    getArtisanActionStatus(actionType) {
        if (!['nightWork', 'clearance'].includes(actionType)) {
            return null;
        }

        const action = this.artisanActions[actionType];
        if (!action.isActive) {
            return { isActive: false };
        }

        const currentTime = this.day * 1440 + this.currentTime; // Temps absolu actuel en minutes
        const elapsedMinutes = currentTime - action.startTime;
        const remainingMinutes = Math.max(0, action.duration - elapsedMinutes);
        const isCompleted = elapsedMinutes >= action.duration;

        // Convertir en jours et heures pour l'affichage
        const remainingDays = Math.floor(remainingMinutes / (24 * 60));
        const remainingHours = Math.floor((remainingMinutes % (24 * 60)) / 60);

        return {
            isActive: true,
            elapsedMinutes,
            remainingMinutes,
            remainingDays,
            remainingHours,
            isCompleted,
            startTime: action.startTime,
            effectStartTime: action.effectStartTime,
            isEffectActive: this.isArtisanEffectActive(actionType)
        };
    }

    isArtisanEffectActive(actionType) {
        if (!['nightWork', 'clearance'].includes(actionType)) {
            return false;
        }

        const action = this.artisanActions[actionType];
        if (!action.effectStartTime) {
            return false;
        }

        const currentTime = this.day * 1440 + this.currentTime;
        const effectElapsed = currentTime - action.effectStartTime;
        
        return effectElapsed < action.effectDuration;
    }

    getActiveArtisanEffects() {
        return {
            doubleMaterials: this.isArtisanEffectActive('nightWork'),
            doubleGold: this.isArtisanEffectActive('clearance')
        };
    }

    processArtisanActionCompletion() {
        const completedActions = [];

        // Vérifier le travail de nuit
        const nightWorkStatus = this.getArtisanActionStatus('nightWork');
        if (nightWorkStatus && this.artisanActions.nightWork.isActive) {
            // Vérifier si l'effet est terminé
            if (!this.isArtisanEffectActive('nightWork')) {
                // Réinitialiser l'action
                this.artisanActions.nightWork.isActive = false;
                this.artisanActions.nightWork.startTime = null;
                this.artisanActions.nightWork.effectStartTime = null;
            }
        }

        // Vérifier les soldes
        const clearanceStatus = this.getArtisanActionStatus('clearance');
        if (clearanceStatus && this.artisanActions.clearance.isActive) {
            // Vérifier si l'effet est terminé
            if (!this.isArtisanEffectActive('clearance')) {
                // Réinitialiser l'action
                this.artisanActions.clearance.isActive = false;
                this.artisanActions.clearance.startTime = null;
                this.artisanActions.clearance.effectStartTime = null;
            }
        }

        return completedActions;
    }

    getGameState() {
        return {
            name: this.name,
            resources: this.resources.toJSON(),
            buildings: this.buildings.map(b => b.getDisplayInfo()),
            adventurers: this.adventurers.map(a => a.getDisplayInfo()),
            day: this.day,
            currentTime: this.currentTime,
            formattedTime: this.getFormattedTime(),
            isPaused: this.isPaused,
            season: this.getSeasonInfo()
        };
    }

    toJSON() {
        return {
            name: this.name,
            resources: this.resources.toJSON(),
            buildings: this.buildings.map(b => b.toJSON()),
            adventurers: this.adventurers.map(a => a.toJSON()),
            day: this.day,
            currentTime: this.currentTime,
            isPaused: this.isPaused,
            marketActions: this.marketActions,
            artisanActions: this.artisanActions,
            seasonManager: this.seasonManager.toJSON()
        };
    }

    static fromJSON(data, buildingTypes = null) {
        const city = new City(data.name);
        city.resources = Resource.fromJSON(data.resources);
        
        // Pour la compatibilité, supporter l'ancien et le nouveau format de bâtiments
        if (data.buildings && buildingTypes) {
            city.buildings = data.buildings.map(b => {
                if (b.buildingTypeId) {
                    // Nouveau format
                    return Building.fromJSON(b, buildingTypes);
                } else {
                    // Ancien format - convertir ou ignorer
                    console.warn('Format de bâtiment obsolète détecté, ignoré');
                    return null;
                }
            }).filter(b => b !== null);
        } else {
            city.buildings = [];
        }
        
        city.adventurers = data.adventurers ? data.adventurers.map(a => Adventurer.fromJSON(a)) : [];
        city.day = data.day || 1;
        city.currentTime = data.currentTime || 0;
        city.isPaused = data.isPaused || false;
        
        // Restaurer les actions du marché
        if (data.marketActions) {
            city.marketActions = data.marketActions;
        }
        
        // Restaurer les actions des artisans
        if (data.artisanActions) {
            city.artisanActions = data.artisanActions;
        }
        
        // Restaurer le gestionnaire de saisons
        if (data.seasonManager) {
            city.seasonManager = Season.fromJSON(data.seasonManager);
        }
        
        return city;
    }
}

if (typeof module !== 'undefined' && module.exports) {
    module.exports = City;
}