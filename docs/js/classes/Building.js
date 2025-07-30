/**
 * Classe Building - Représente une instance construite d'un bâtiment dans la ville
 */
class Building {
    constructor(id, customName, buildingType, level = 1) {
        this.id = id; // ID unique de cette instance
        this.customName = customName; // Nom personnalisé donné par le joueur
        this.buildingType = buildingType; // Type de bâtiment (BuildingType)
        this.level = level;
        this.built = true; // Un Building est toujours construit
        this.constructionDate = Date.now();
        
        // Calculer les effets et coûts selon le type et le niveau
        this.updateStats();
    }

    updateStats() {
        this.effects = this.buildingType.getEffectsAtLevel(this.level);
        this.upgradeCost = this.buildingType.getCostAtLevel(this.level + 1);
        this.maxLevel = this.buildingType.maxLevel;
    }

    canUpgrade() {
        return this.level < this.maxLevel;
    }

    upgrade() {
        if (this.canUpgrade()) {
            this.level++;
            this.updateStats();
            return true;
        }
        return false;
    }

    getDisplayInfo() {
        return {
            id: this.id,
            customName: this.customName,
            typeName: this.buildingType.name,
            typeId: this.buildingType.id,
            district: this.buildingType.district,
            level: this.level,
            maxLevel: this.maxLevel,
            built: this.built,
            effects: this.effects,
            upgradeCost: this.upgradeCost,
            icon: this.buildingType.icon,
            constructionDate: this.constructionDate
        };
    }

    toJSON() {
        return {
            id: this.id,
            customName: this.customName,
            buildingTypeId: this.buildingType.id,
            level: this.level,
            constructionDate: this.constructionDate
        };
    }

    static fromJSON(data, buildingTypes) {
        // Trouver le type de bâtiment correspondant
        const buildingType = buildingTypes.find(type => type.id === data.buildingTypeId);
        if (!buildingType) {
            console.error(`Type de bâtiment introuvable: ${data.buildingTypeId}`);
            return null;
        }

        const building = new Building(
            data.id,
            data.customName,
            buildingType,
            data.level || 1
        );
        
        building.constructionDate = data.constructionDate || Date.now();
        
        return building;
    }
}

if (typeof module !== 'undefined' && module.exports) {
    module.exports = Building;
}