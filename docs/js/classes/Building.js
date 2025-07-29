/**
 * Classe Building - Représente un bâtiment de la ville
 */
class Building {
    constructor(id, name, type, district, level = 1) {
        this.id = id;
        this.name = name;
        this.type = type;
        this.district = district;
        this.level = level;
        this.maxLevel = 5;
        this.built = false;
        this.effects = this.calculateEffects();
        this.upgradeCost = this.calculateUpgradeCost();
    }

    calculateEffects() {
        const baseEffects = this.getBaseEffects();
        const multiplier = this.level;
        
        const effects = {};
        for (const [key, value] of Object.entries(baseEffects)) {
            effects[key] = Math.floor(value * multiplier);
        }
        
        return effects;
    }

    getBaseEffects() {
        const effects = {
            // Quartier Résidentiel
            'maison': { population: 10, goldPerTurn: 5 },
            'auberge': { population: 5, reputation: 2, goldPerTurn: 8 },
            'taverne': { population: 3, reputation: 3, goldPerTurn: 6 },
            
            // District Commercial
            'marche': { goldPerTurn: 15, reputation: 1 },
            'echoppe_artisan': { goldPerTurn: 10, materialsPerTurn: 2 },
            'banque': { goldPerTurn: 20, population: -2 },
            
            // Zone Industrielle
            'forge': { materialsPerTurn: 5, goldPerTurn: -3 },
            'alchimiste': { magicPerTurn: 3, goldPerTurn: -2 },
            'enchanteur': { magicPerTurn: 2, reputation: 2, goldPerTurn: -1 },
            
            // Quartier Administratif
            'mairie': { reputation: 5, goldPerTurn: -5 },
            'caserne': { reputation: 3, population: -3, goldPerTurn: -8 },
            'prison': { reputation: -1, population: -1, goldPerTurn: -3 },
            'tribunal': { reputation: 4, goldPerTurn: -4 }
        };
        
        return effects[this.type] || {};
    }

    calculateUpgradeCost() {
        const baseCosts = {
            'maison': { gold: 100, materials: 20 },
            'auberge': { gold: 200, materials: 30, magic: 5 },
            'taverne': { gold: 150, materials: 25 },
            'marche': { gold: 300, materials: 40, population: 5 },
            'echoppe_artisan': { gold: 250, materials: 35, magic: 10 },
            'banque': { gold: 500, materials: 50, magic: 20 },
            'forge': { gold: 400, materials: 60, magic: 15 },
            'alchimiste': { gold: 600, materials: 40, magic: 40 },
            'enchanteur': { gold: 800, materials: 50, magic: 60 },
            'mairie': { gold: 1000, materials: 100, magic: 50 },
            'caserne': { gold: 800, materials: 120, magic: 30 },
            'prison': { gold: 300, materials: 80, magic: 10 },
            'tribunal': { gold: 600, materials: 90, magic: 40 }
        };
        
        const baseCost = baseCosts[this.type] || { gold: 100, materials: 10 };
        const multiplier = Math.pow(1.5, this.level);
        
        const cost = {};
        for (const [key, value] of Object.entries(baseCost)) {
            cost[key] = Math.floor(value * multiplier);
        }
        
        return cost;
    }

    canUpgrade() {
        return this.built && this.level < this.maxLevel;
    }

    upgrade() {
        if (this.canUpgrade()) {
            this.level++;
            this.effects = this.calculateEffects();
            this.upgradeCost = this.calculateUpgradeCost();
            return true;
        }
        return false;
    }

    build() {
        this.built = true;
    }

    getDisplayInfo() {
        return {
            id: this.id,
            name: this.name,
            type: this.type,
            district: this.district,
            level: this.level,
            maxLevel: this.maxLevel,
            built: this.built,
            effects: this.effects,
            upgradeCost: this.upgradeCost
        };
    }

    toJSON() {
        return {
            id: this.id,
            name: this.name,
            type: this.type,
            district: this.district,
            level: this.level,
            built: this.built
        };
    }

    static fromJSON(data) {
        const building = new Building(data.id, data.name, data.type, data.district, data.level);
        building.built = data.built || false;
        return building;
    }
}

if (typeof module !== 'undefined' && module.exports) {
    module.exports = Building;
}