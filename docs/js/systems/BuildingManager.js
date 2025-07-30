/**
 * Classe BuildingManager - Gestion de la construction et des bâtiments
 */
class BuildingManager {
    constructor(city, cityUpgradeManager) {
        this.city = city;
        this.cityUpgradeManager = cityUpgradeManager;
        this.buildingTypes = [];
        this.initializeBuildingTypes();
    }

    initializeBuildingTypes() {
        // Créer tous les types de bâtiments disponibles
        this.buildingTypes = [
            BuildingType.createMaison(),
            BuildingType.createTaverne(),
            BuildingType.createMarche(),
            BuildingType.createEchoppeArtisan(),
            BuildingType.createBanque(),
            BuildingType.createMairie(),
            BuildingType.createForge(),
            BuildingType.createAlchimiste(),
            BuildingType.createEnchanteur(),
            BuildingType.createGuildeAventuriers(),
            BuildingType.createPrison()
        ];
    }

    getBuildingTypeById(typeId) {
        return this.buildingTypes.find(type => type.id === typeId);
    }

    generateDefaultBuildingName(buildingType) {
        // Compter les bâtiments existants du même type
        const existingCount = this.city.buildings.filter(b => 
            b.buildingType.id === buildingType.id
        ).length;
        
        // Générer le nom avec numéro
        return `${buildingType.name} ${existingCount + 1}`;
    }

    getAvailableBuildingTypes() {
        return this.buildingTypes.filter(type => 
            type.isUnlocked(this.cityUpgradeManager)
        ).map(type => type.getDisplayInfo(this.cityUpgradeManager));
    }

    getLockedBuildingTypes() {
        return this.buildingTypes.filter(type => 
            !type.isUnlocked(this.cityUpgradeManager)
        ).map(type => type.getDisplayInfo(this.cityUpgradeManager));
    }

    getAllBuildingTypes() {
        return this.buildingTypes.map(type => type.getDisplayInfo(this.cityUpgradeManager));
    }

    canConstructBuilding(typeId, customName) {
        const buildingType = this.getBuildingTypeById(typeId);
        if (!buildingType) {
            return { canConstruct: false, reason: 'Type de bâtiment introuvable' };
        }

        // Vérifier si le type est débloqué
        if (!buildingType.isUnlocked(this.cityUpgradeManager)) {
            return { canConstruct: false, reason: 'Type de bâtiment non débloqué' };
        }

        // Vérifier le nom personnalisé s'il est fourni
        if (customName && customName.trim().length > 30) {
            return { canConstruct: false, reason: 'Nom trop long (max 30 caractères)' };
        }

        // Vérifier si le nom n'est pas déjà utilisé (seulement si un nom est fourni)
        if (customName && customName.trim().length > 0) {
            const existingBuilding = this.city.buildings.find(b => 
                b.customName.toLowerCase() === customName.trim().toLowerCase()
            );
            if (existingBuilding) {
                return { canConstruct: false, reason: 'Ce nom est déjà utilisé' };
            }
        }

        // Vérifier les ressources
        const constructionCost = buildingType.getCostAtLevel(1);
        if (!this.city.resources.canAfford(constructionCost)) {
            return { canConstruct: false, reason: 'Ressources insuffisantes' };
        }

        // Vérifier les points d'action
        if (!this.city.canPerformAction(2)) {
            return { canConstruct: false, reason: 'Points d\'action insuffisants (2 requis)' };
        }

        // Vérifications spéciales pour certains bâtiments uniques
        if (typeId === 'guilde_aventuriers' || typeId === 'mairie') {
            const existingUnique = this.city.buildings.find(b => b.buildingType.id === typeId);
            if (existingUnique) {
                return { canConstruct: false, reason: 'Ce type de bâtiment ne peut être construit qu\'une fois' };
            }
        }

        return { canConstruct: true, buildingType, constructionCost };
    }

    constructBuilding(typeId, customName) {
        const canConstruct = this.canConstructBuilding(typeId, customName);
        if (!canConstruct.canConstruct) {
            return { success: false, message: canConstruct.reason };
        }

        const { buildingType, constructionCost } = canConstruct;

        // Payer le coût
        this.city.resources.spend(constructionCost);
        this.city.performAction(2);

        // Déterminer le nom final du bâtiment
        const finalName = (customName && customName.trim().length > 0) 
            ? customName.trim() 
            : this.generateDefaultBuildingName(buildingType);

        // Créer le bâtiment
        const buildingId = `building_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`;
        const building = new Building(buildingId, finalName, buildingType, 1);

        // Ajouter à la ville
        this.city.addBuilding(building);
        
        // Appliquer immédiatement les effets du bâtiment
        this.applyBuildingEffects(building);

        return {
            success: true,
            message: `${finalName} construit avec succès !`,
            building: building.getDisplayInfo()
        };
    }

    canUpgradeBuilding(buildingId) {
        const building = this.city.getBuildingById(buildingId);
        if (!building) {
            return { canUpgrade: false, reason: 'Bâtiment introuvable' };
        }

        if (!building.canUpgrade()) {
            return { canUpgrade: false, reason: 'Niveau maximum atteint' };
        }

        // Vérifier les ressources
        if (!this.city.resources.canAfford(building.upgradeCost)) {
            return { canUpgrade: false, reason: 'Ressources insuffisantes' };
        }

        // Vérifier les points d'action
        if (!this.city.canPerformAction(1)) {
            return { canUpgrade: false, reason: 'Points d\'action insuffisants' };
        }

        return { canUpgrade: true, building };
    }

    upgradeBuilding(buildingId) {
        const canUpgrade = this.canUpgradeBuilding(buildingId);
        if (!canUpgrade.canUpgrade) {
            return { success: false, message: canUpgrade.reason };
        }

        const building = canUpgrade.building;
        const upgradeCost = { ...building.upgradeCost };

        // Payer le coût
        this.city.resources.spend(upgradeCost);
        this.city.performAction(1);

        // Améliorer le bâtiment
        const oldLevel = building.level;
        const oldEffects = { ...building.effects };
        building.upgrade();
        
        // Appliquer la différence d'effets entre ancien et nouveau niveau
        this.applyBuildingEffectsDifference(oldEffects, building.effects);

        return {
            success: true,
            message: `${building.customName} amélioré au niveau ${building.level} !`,
            building: building.getDisplayInfo(),
            oldLevel,
            newLevel: building.level
        };
    }

    canDemolishBuilding(buildingId) {
        const building = this.city.getBuildingById(buildingId);
        if (!building) {
            return { canDemolish: false, reason: 'Bâtiment introuvable' };
        }

        // Vérifier les points d'action
        if (!this.city.canPerformAction(1)) {
            return { canDemolish: false, reason: 'Points d\'action insuffisants' };
        }

        // Certains bâtiments ne peuvent pas être détruits (mairie, etc.)
        const protectedBuildings = ['mairie'];
        if (protectedBuildings.includes(building.buildingType.id)) {
            return { canDemolish: false, reason: 'Ce bâtiment ne peut pas être détruit' };
        }

        return { canDemolish: true, building };
    }

    demolishBuilding(buildingId) {
        const canDemolish = this.canDemolishBuilding(buildingId);
        if (!canDemolish.canDemolish) {
            return { success: false, message: canDemolish.reason };
        }

        const building = canDemolish.building;

        // Récupérer une partie des ressources (30%)
        const refundCost = {};
        const constructionCost = building.buildingType.getCostAtLevel(1);
        Object.entries(constructionCost).forEach(([resource, amount]) => {
            refundCost[resource] = Math.floor(amount * 0.3);
        });

        // Appliquer le remboursement
        this.city.resources.gain(refundCost);
        this.city.performAction(1);

        // Retirer le bâtiment
        this.city.removeBuilding(buildingId);

        return {
            success: true,
            message: `${building.customName} détruit. Remboursement partiel accordé.`,
            refund: refundCost
        };
    }

    getConstructedBuildings() {
        return this.city.buildings.map(b => b.getDisplayInfo());
    }

    getBuildingsByDistrict(district) {
        return this.city.buildings
            .filter(b => b.buildingType.district === district)
            .map(b => b.getDisplayInfo());
    }

    getBuildingStats() {
        const buildings = this.city.buildings;
        const districts = {};
        
        // Compter par district
        buildings.forEach(building => {
            const district = building.buildingType.district;
            if (!districts[district]) {
                districts[district] = { count: 0, totalLevels: 0 };
            }
            districts[district].count++;
            districts[district].totalLevels += building.level;
        });

        return {
            total: buildings.length,
            districts,
            totalLevels: buildings.reduce((sum, b) => sum + b.level, 0),
            averageLevel: buildings.length > 0 ? 
                Math.round((buildings.reduce((sum, b) => sum + b.level, 0) / buildings.length) * 10) / 10 : 0
        };
    }

    // Appliquer les effets d'un bâtiment aux ressources de la ville
    applyBuildingEffects(building) {
        const effects = building.effects;
        
        // Appliquer les effets immédiats (non-par-tour)
        if (effects.population) {
            this.city.resources.gain({ population: effects.population });
        }
        if (effects.maxPopulation) {
            // Les effets de population maximale sont appliqués passivement
        }
    }
    
    // Appliquer la différence d'effets entre deux niveaux de bâtiment
    applyBuildingEffectsDifference(oldEffects, newEffects) {
        const effectsDiff = {};
        
        // Calculer la différence pour chaque type d'effet
        const effectTypes = ['population', 'maxPopulation'];
        
        effectTypes.forEach(effectType => {
            const oldValue = oldEffects[effectType] || 0;
            const newValue = newEffects[effectType] || 0;
            const diff = newValue - oldValue;
            
            if (diff !== 0) {
                effectsDiff[effectType] = diff;
            }
        });
        
        // Appliquer les différences
        if (effectsDiff.population) {
            this.city.resources.gain({ population: effectsDiff.population });
        }
    }

    // Sérialisation pour la sauvegarde
    toJSON() {
        return {
            buildingTypes: this.buildingTypes.map(type => type.toJSON())
        };
    }

    static fromJSON(data, city, cityUpgradeManager) {
        const manager = new BuildingManager(city, cityUpgradeManager);
        
        if (data.buildingTypes && data.buildingTypes.length > 0) {
            manager.buildingTypes = data.buildingTypes.map(typeData => 
                BuildingType.fromJSON(typeData)
            );
        }
        
        return manager;
    }
}

if (typeof module !== 'undefined' && module.exports) {
    module.exports = BuildingManager;
}