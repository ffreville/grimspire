/**
 * Classe BuildingType - Définit les types de bâtiments disponibles
 */
class BuildingType {
    constructor(id, name, district, baseEffects = {}, baseCost = {}, unlockRequirement = null) {
        this.id = id;
        this.name = name;
        this.district = district;
        this.baseEffects = baseEffects;
        this.baseCost = baseCost;
        this.unlockRequirement = unlockRequirement; // ID de l'amélioration requise
        this.icon = '🏗️'; // Icône par défaut
        this.description = '';
        this.maxLevel = 5; // Niveau maximum par défaut
    }

    isUnlocked(cityUpgradeManager) {
        if (!this.unlockRequirement) {
            return true; // Pas de prérequis, toujours débloqué
        }
        
        return cityUpgradeManager.isUpgradeUnlocked(this.unlockRequirement);
    }

    getEffectsAtLevel(level) {
        const effects = {};
        Object.entries(this.baseEffects).forEach(([key, baseValue]) => {
            effects[key] = Math.floor(baseValue * level * 1.2); // Progression par niveau
        });
        return effects;
    }

    getCostAtLevel(level) {
        const cost = {};
        Object.entries(this.baseCost).forEach(([resource, baseAmount]) => {
            cost[resource] = Math.floor(baseAmount * Math.pow(1.5, level - 1)); // Coût croissant
        });
        return cost;
    }

    getDisplayInfo(cityUpgradeManager = null) {
        return {
            id: this.id,
            name: this.name,
            district: this.district,
            baseEffects: this.baseEffects,
            baseCost: this.baseCost,
            unlockRequirement: this.unlockRequirement,
            icon: this.icon,
            description: this.description,
            maxLevel: this.maxLevel,
            unlocked: cityUpgradeManager ? this.isUnlocked(cityUpgradeManager) : true
        };
    }

    toJSON() {
        return {
            id: this.id,
            name: this.name,
            district: this.district,
            baseEffects: this.baseEffects,
            baseCost: this.baseCost,
            unlockRequirement: this.unlockRequirement,
            icon: this.icon,
            description: this.description,
            maxLevel: this.maxLevel
        };
    }

    static fromJSON(data) {
        const buildingType = new BuildingType(
            data.id,
            data.name,
            data.district,
            data.baseEffects,
            data.baseCost,
            data.unlockRequirement
        );
        
        buildingType.icon = data.icon || '🏗️';
        buildingType.description = data.description || '';
        buildingType.maxLevel = data.maxLevel || 5;
        
        return buildingType;
    }

    // Méthodes statiques pour créer les types de bâtiments
    static createMaison() {
        const type = new BuildingType(
            'maison',
            'Maison',
            'residentiel',
            { population: 10, goldPerTurn: 9 },
            { gold: 200, materials: 50 }
        );
        type.icon = '🏠';
        type.description = 'Logement pour les habitants de votre cité';
        return type;
    }

    static createTaverne() {
        const type = new BuildingType(
            'taverne',
            'Taverne',
            'residentiel',
            { population: 5, goldPerTurn: 19, reputation: 2 },
            { gold: 300, materials: 100 }
        );
        type.icon = '🍺';
        type.description = 'Lieu de rencontre qui attire habitants et voyageurs';
        return type;
    }

    static createMarche() {
        const type = new BuildingType(
            'marche',
            'Marché',
            'commercial',
            { goldPerTurn: 29, materialsPerTurn: 5 },
            { gold: 400, materials: 150 }
        );
        type.icon = '🏪';
        type.description = 'Centre commercial pour le négoce de marchandises';
        return type;
    }

    static createEchoppeArtisan() {
        const type = new BuildingType(
            'echoppe_artisan',
            "Échoppe d'Artisan",
            'commercial',
            { materialsPerTurn: 10, goldPerTurn: 14 },
            { gold: 350, materials: 200 }
        );
        type.icon = '🔨';
        type.description = "Atelier d'artisan produisant des biens de qualité";
        return type;
    }

    static createBanque() {
        const type = new BuildingType(
            'banque',
            'Banque',
            'commercial',
            { goldPerTurn: 54, reputation: 5 },
            { gold: 800, materials: 300 },
            'bank_unlock'
        );
        type.icon = '🏦';
        type.description = "Institution financière générant de l'or et de la réputation";
        return type;
    }

    static createMairie() {
        const type = new BuildingType(
            'mairie',
            'Mairie',
            'administratif',
            { reputation: 10, goldPerTurn: 24 },
            { gold: 1000, materials: 500 }
        );
        type.icon = '🏛️';
        type.description = 'Centre administratif de votre cité';
        return type;
    }

    static createForge() {
        const type = new BuildingType(
            'forge',
            'Forge',
            'industriel',
            { materialsPerTurn: 15, magicPerTurn: 2 },
            { gold: 500, materials: 300 }
        );
        type.icon = '⚒️';
        type.description = "Forge pour la production d'équipements et d'outils";
        return type;
    }

    static createAlchimiste() {
        const type = new BuildingType(
            'alchimiste',
            'Alchimiste',
            'industriel',
            { magicPerTurn: 10, materialsPerTurn: 5 },
            { gold: 600, materials: 200, magic: 50 },
            'alchemist_unlock'
        );
        type.icon = '🧪';
        type.description = 'Laboratoire alchimique produisant des substances magiques';
        return type;
    }

    static createEnchanteur() {
        const type = new BuildingType(
            'enchanteur',
            'Enchanteur',
            'industriel',
            { magicPerTurn: 15, reputation: 3 },
            { gold: 800, materials: 300, magic: 100 },
            'enchanter_unlock'
        );
        type.icon = '✨';
        type.description = "Atelier d'enchantement pour améliorer les équipements";
        return type;
    }

    static createGuildeAventuriers() {
        const type = new BuildingType(
            'guilde_aventuriers',
            'Guilde des Aventuriers',
            'administratif',
            { reputation: 15, goldPerTurn: 34 },
            { gold: 1200, materials: 600 },
            'guild_unlock'
        );
        type.icon = '⚔️';
        type.description = 'Quartier général pour recruter et gérer les aventuriers';
        type.maxLevel = 3; // Moins de niveaux car c'est un bâtiment unique
        return type;
    }

    static createPrison() {
        const type = new BuildingType(
            'prison',
            'Prison',
            'administratif',
            { reputation: 8, goldPerTurn: 14 },
            { gold: 700, materials: 400 },
            'prison_unlock'
        );
        type.icon = '🔒';
        type.description = "Prison pour maintenir l'ordre et la sécurité";
        return type;
    }
}

if (typeof module !== 'undefined' && module.exports) {
    module.exports = BuildingType;
}