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
        this.isNight = false;
        this.actionPoints = { day: 3, night: 2 };
        this.currentActionPoints = this.actionPoints.day;
        
        // Initialiser les bâtiments de base
        this.initializeStartingBuildings();
    }

    initializeStartingBuildings() {
        // Bâtiments de départ
        const startingBuildings = [
            new Building('house_1', 'Maison 1', 'maison', 'residentiel'),
            new Building('house_2', 'Maison 2', 'maison', 'residentiel'),
            new Building('tavern_1', 'La Chope Dorée', 'taverne', 'residentiel'),
            new Building('market_1', 'Marché Central', 'marche', 'commercial'),
            new Building('forge_1', 'Forge du Marteau', 'forge', 'industriel'),
            new Building('town_hall', 'Mairie', 'mairie', 'administratif')
        ];
        
        // Construire quelques bâtiments de base
        startingBuildings.forEach((building, index) => {
            if (index < 3) { // Les 3 premiers sont construits
                building.build();
            }
            this.buildings.push(building);
        });
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
        return this.currentActionPoints >= cost;
    }

    performAction(cost = 1) {
        if (this.canPerformAction(cost)) {
            this.currentActionPoints -= cost;
            return true;
        }
        return false;
    }

    switchTimePhase() {
        this.isNight = !this.isNight;
        
        if (this.isNight) {
            // Passage au soir
            this.currentActionPoints = this.actionPoints.night;
        } else {
            // Passage au jour (nouveau jour)
            this.day++;
            this.currentActionPoints = this.actionPoints.day;
            this.processDaily();
        }
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
        
        // Ajouter un minimum de base
        dailyGain.gold += 50; // Revenu de base
        
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
        
        if (!this.performAction(1)) {
            return { success: false, message: 'Points d\'action insuffisants' };
        }
        
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
        
        if (!this.performAction(2)) {
            return { success: false, message: 'Points d\'action insuffisants' };
        }
        
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
            isNight: this.isNight,
            currentActionPoints: this.currentActionPoints,
            maxActionPoints: this.isNight ? this.actionPoints.night : this.actionPoints.day
        };
    }

    toJSON() {
        return {
            name: this.name,
            resources: this.resources.toJSON(),
            buildings: this.buildings.map(b => b.toJSON()),
            adventurers: this.adventurers.map(a => a.toJSON()),
            day: this.day,
            isNight: this.isNight,
            actionPoints: this.actionPoints,
            currentActionPoints: this.currentActionPoints
        };
    }

    static fromJSON(data) {
        const city = new City(data.name);
        city.resources = Resource.fromJSON(data.resources);
        city.buildings = data.buildings.map(b => Building.fromJSON(b));
        city.adventurers = data.adventurers.map(a => Adventurer.fromJSON(a));
        city.day = data.day || 1;
        city.isNight = data.isNight || false;
        city.actionPoints = data.actionPoints || { day: 3, night: 2 };
        city.currentActionPoints = data.currentActionPoints || city.actionPoints.day;
        return city;
    }
}

if (typeof module !== 'undefined' && module.exports) {
    module.exports = City;
}