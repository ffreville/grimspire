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
        // Traitement par heure : progression des constructions
        this.processBuildingProgress();
    }

    // Nouveau : traitement des constructions/améliorations
    processBuildingProgress() {
        // Cette méthode sera appelée par le GameManager qui a accès au BuildingManager
        // Elle est définie ici pour maintenir la structure mais sera déléguée
    }

    processDaily() {
        // Traitement quotidien : génération de ressources
        this.generateDailyResources();
        this.healAllAdventurers();
    }

    generateDailyResources() {
        const builtBuildings = this.getBuiltBuildings();
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
        
        this.resources.gain(dailyGain);
        
        return dailyGain;
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

    getGameState() {
        return {
            name: this.name,
            resources: this.resources.toJSON(),
            buildings: this.buildings.map(b => b.getDisplayInfo()),
            adventurers: this.adventurers.map(a => a.getDisplayInfo()),
            day: this.day,
            currentTime: this.currentTime,
            formattedTime: this.getFormattedTime(),
            isPaused: this.isPaused
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
            isPaused: this.isPaused
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
        return city;
    }
}

if (typeof module !== 'undefined' && module.exports) {
    module.exports = City;
}