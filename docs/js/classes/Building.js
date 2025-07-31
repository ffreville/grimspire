/**
 * Classe Building - Représente une instance construite d'un bâtiment dans la ville
 */
class Building {
    constructor(id, customName, buildingType, level = 1, startConstruction = false) {
        this.id = id; // ID unique de cette instance
        this.customName = customName; // Nom personnalisé donné par le joueur
        this.buildingType = buildingType; // Type de bâtiment (BuildingType)
        this.level = level;
        this.built = !startConstruction; // Si startConstruction = true, le bâtiment n'est pas encore construit
        this.constructionDate = Date.now();
        
        // Système de construction
        this.isUnderConstruction = startConstruction;
        this.constructionStartTime = startConstruction ? Date.now() : null; // Timestamp du début de construction
        this.constructionDuration = startConstruction ? buildingType.getConstructionTimeAtLevel(level) * 60 : 0; // Durée en minutes de jeu
        this.constructionProgress = 0; // Progression en minutes de jeu écoulées
        
        // Système d'amélioration
        this.isUpgrading = false;
        this.upgradeStartTime = null;
        this.upgradeDuration = 0;
        this.upgradeProgress = 0;
        this.upgradeTargetLevel = level;
        
        // Calculer les effets et coûts selon le type et le niveau
        this.updateStats();
    }

    updateStats() {
        this.effects = this.buildingType.getEffectsAtLevel(this.level);
        this.upgradeCost = this.buildingType.getCostAtLevel(this.level + 1);
        this.maxLevel = this.buildingType.maxLevel;
    }

    canUpgrade() {
        return this.level < this.maxLevel && this.built && !this.isUnderConstruction && !this.isUpgrading;
    }

    startUpgrade() {
        if (this.canUpgrade()) {
            this.isUpgrading = true;
            this.upgradeStartTime = Date.now();
            this.upgradeTargetLevel = this.level + 1;
            this.upgradeDuration = this.buildingType.getUpgradeTimeToLevel(this.upgradeTargetLevel) * 60; // En minutes de jeu
            this.upgradeProgress = 0;
            return true;
        }
        return false;
    }

    upgrade() {
        // Méthode d'amélioration instantanée (pour compatibilité)
        if (this.canUpgrade()) {
            this.level++;
            this.updateStats();
            return true;
        }
        return false;
    }

    // Avancer la progression de la construction/amélioration
    advanceProgress(gameMinutesElapsed) {
        if (this.isUnderConstruction) {
            this.constructionProgress += gameMinutesElapsed;
            if (this.constructionProgress >= this.constructionDuration) {
                this.completeConstruction();
                return { type: 'construction', completed: true };
            }
            return { type: 'construction', completed: false, progress: this.getConstructionPercentage() };
        }
        
        if (this.isUpgrading) {
            this.upgradeProgress += gameMinutesElapsed;
            if (this.upgradeProgress >= this.upgradeDuration) {
                this.completeUpgrade();
                return { type: 'upgrade', completed: true };
            }
            return { type: 'upgrade', completed: false, progress: this.getUpgradePercentage() };
        }
        
        return null;
    }

    completeConstruction() {
        this.isUnderConstruction = false;
        this.built = true;
        this.constructionProgress = this.constructionDuration;
        this.constructionDate = Date.now();
    }

    completeUpgrade() {
        this.isUpgrading = false;
        this.level = this.upgradeTargetLevel;
        this.upgradeProgress = this.upgradeDuration;
        this.updateStats();
    }

    getConstructionPercentage() {
        if (!this.isUnderConstruction) return 100;
        return Math.min(100, Math.round((this.constructionProgress / this.constructionDuration) * 100));
    }

    getUpgradePercentage() {
        if (!this.isUpgrading) return 100;
        return Math.min(100, Math.round((this.upgradeProgress / this.upgradeDuration) * 100));
    }

    getRemainingConstructionTime() {
        if (!this.isUnderConstruction) return 0;
        return Math.max(0, this.constructionDuration - this.constructionProgress);
    }

    getRemainingUpgradeTime() {
        if (!this.isUpgrading) return 0;
        return Math.max(0, this.upgradeDuration - this.upgradeProgress);
    }

    getFormattedRemainingTime() {
        const remaining = this.isUnderConstruction ? this.getRemainingConstructionTime() : this.getRemainingUpgradeTime();
        const hours = Math.floor(remaining / 60);
        const minutes = remaining % 60;
        return `${hours}h${minutes.toString().padStart(2, '0')}m`;
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
            constructionDate: this.constructionDate,
            // Informations de construction
            isUnderConstruction: this.isUnderConstruction,
            constructionProgress: this.getConstructionPercentage(),
            remainingConstructionTime: this.getFormattedRemainingTime(),
            // Informations d'amélioration
            isUpgrading: this.isUpgrading,
            upgradeProgress: this.getUpgradePercentage(),
            upgradeTargetLevel: this.upgradeTargetLevel,
            remainingUpgradeTime: this.getFormattedRemainingTime(),
            // Info d'onglet débloqué
            unlocksTab: this.buildingType.unlocksTab
        };
    }

    toJSON() {
        return {
            id: this.id,
            customName: this.customName,
            buildingTypeId: this.buildingType.id,
            level: this.level,
            built: this.built,
            constructionDate: this.constructionDate,
            // Données de construction
            isUnderConstruction: this.isUnderConstruction,
            constructionStartTime: this.constructionStartTime,
            constructionDuration: this.constructionDuration,
            constructionProgress: this.constructionProgress,
            // Données d'amélioration
            isUpgrading: this.isUpgrading,
            upgradeStartTime: this.upgradeStartTime,
            upgradeDuration: this.upgradeDuration,
            upgradeProgress: this.upgradeProgress,
            upgradeTargetLevel: this.upgradeTargetLevel
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
            data.level || 1,
            false // Ne pas démarrer la construction automatiquement
        );
        
        // Restaurer l'état du bâtiment
        building.built = data.built !== undefined ? data.built : true;
        building.constructionDate = data.constructionDate || Date.now();
        
        // Restaurer les données de construction
        building.isUnderConstruction = data.isUnderConstruction || false;
        building.constructionStartTime = data.constructionStartTime || null;
        building.constructionDuration = data.constructionDuration || 0;
        building.constructionProgress = data.constructionProgress || 0;
        
        // Restaurer les données d'amélioration
        building.isUpgrading = data.isUpgrading || false;
        building.upgradeStartTime = data.upgradeStartTime || null;
        building.upgradeDuration = data.upgradeDuration || 0;
        building.upgradeProgress = data.upgradeProgress || 0;
        building.upgradeTargetLevel = data.upgradeTargetLevel || building.level;
        
        return building;
    }
}

if (typeof module !== 'undefined' && module.exports) {
    module.exports = Building;
}