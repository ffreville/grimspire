/**
 * Classe CityUpgrade - Représente une amélioration de ville
 */
class CityUpgrade {
    constructor(id, name, description, cost = { gold: 100 }, requirements = []) {
        this.id = id;
        this.name = name;
        this.description = description;
        this.cost = cost;
        this.requirements = requirements; // Autres améliorations requises
        this.unlocked = false;
        this.category = 'city'; // Type d'amélioration
        this.icon = '🏗️'; // Icône par défaut
        this.effects = {}; // Effets de l'amélioration (si nécessaire)
        
        // Système de temps de construction
        this.constructionTime = 1; // Temps de recherche en heures de jeu (par défaut)
        this.isUnderDevelopment = false;
        this.developmentStartTime = null;
        this.developmentProgress = 0; // En minutes de jeu
    }

    canUnlock(cityUpgrades, cityResources) {
        // Vérifier si déjà débloqué ou en cours de développement
        if (this.unlocked) {
            return { canUnlock: false, reason: 'Déjà débloqué' };
        }
        
        if (this.isUnderDevelopment) {
            return { canUnlock: false, reason: 'Recherche en cours' };
        }

        // Vérifier les ressources
        if (!cityResources.canAfford(this.cost)) {
            return { canUnlock: false, reason: 'Ressources insuffisantes' };
        }

        // Vérifier les prérequis
        for (const reqId of this.requirements) {
            const requirement = cityUpgrades.find(u => u.id === reqId);
            if (!requirement || !requirement.unlocked) {
                return { canUnlock: false, reason: `Nécessite: ${requirement ? requirement.name : reqId}` };
            }
        }

        return { canUnlock: true };
    }

    startDevelopment() {
        if (!this.isUnderDevelopment && !this.unlocked) {
            this.isUnderDevelopment = true;
            this.developmentStartTime = Date.now();
            this.developmentProgress = 0;
            return true;
        }
        return false;
    }

    unlock() {
        this.unlocked = true;
        this.isUnderDevelopment = false;
        this.developmentProgress = this.constructionTime * 60; // Marquer comme terminé
        return {
            success: true,
            message: `${this.name} débloqué !`,
            upgrade: this.getDisplayInfo()
        };
    }

    // Méthodes de progression temporelle
    advanceProgress(gameMinutesElapsed) {
        if (this.isUnderDevelopment && !this.unlocked) {
            this.developmentProgress += gameMinutesElapsed;
            const totalDuration = this.constructionTime * 60; // Convertir heures en minutes
            
            if (this.developmentProgress >= totalDuration) {
                this.completeDevelopment();
                return { completed: true };
            }
            return { completed: false, progress: this.getProgressPercentage() };
        }
        return null;
    }

    completeDevelopment() {
        this.isUnderDevelopment = false;
        this.unlocked = true;
        this.developmentProgress = this.constructionTime * 60;
    }

    getProgressPercentage() {
        if (!this.isUnderDevelopment) return this.unlocked ? 100 : 0;
        const totalDuration = this.constructionTime * 60;
        return Math.min(100, Math.round((this.developmentProgress / totalDuration) * 100));
    }

    getRemainingTime() {
        if (!this.isUnderDevelopment) return 0;
        const totalDuration = this.constructionTime * 60;
        return Math.max(0, totalDuration - this.developmentProgress);
    }

    getFormattedRemainingTime() {
        const remaining = this.getRemainingTime();
        const hours = Math.floor(remaining / 60);
        const minutes = remaining % 60;
        return `${hours}h${minutes.toString().padStart(2, '0')}m`;
    }

    getDisplayInfo() {
        return {
            id: this.id,
            name: this.name,
            description: this.description,
            cost: this.cost,
            requirements: this.requirements,
            unlocked: this.unlocked,
            category: this.category,
            icon: this.icon,
            effects: this.effects,
            constructionTime: this.constructionTime,
            isUnderDevelopment: this.isUnderDevelopment,
            developmentProgress: this.getProgressPercentage(),
            remainingTime: this.getFormattedRemainingTime()
        };
    }

    toJSON() {
        return {
            id: this.id,
            name: this.name,
            description: this.description,
            cost: this.cost,
            requirements: this.requirements,
            unlocked: this.unlocked,
            category: this.category,
            icon: this.icon,
            effects: this.effects,
            constructionTime: this.constructionTime,
            isUnderDevelopment: this.isUnderDevelopment,
            developmentStartTime: this.developmentStartTime,
            developmentProgress: this.developmentProgress
        };
    }

    static fromJSON(data) {
        const upgrade = new CityUpgrade(
            data.id,
            data.name,
            data.description,
            data.cost,
            data.requirements
        );
        
        upgrade.unlocked = data.unlocked || false;
        upgrade.category = data.category || 'city';
        upgrade.icon = data.icon || '🏗️';
        upgrade.effects = data.effects || {};
        upgrade.constructionTime = data.constructionTime || 6;
        upgrade.isUnderDevelopment = data.isUnderDevelopment || false;
        upgrade.developmentStartTime = data.developmentStartTime || null;
        upgrade.developmentProgress = data.developmentProgress || 0;
        
        return upgrade;
    }

    // Méthodes statiques pour créer les améliorations prédéfinies
    static createGuildUpgrade() {
        return new CityUpgrade(
            'guild_unlock',
            'Débloquer la guilde des aventuriers',
            'Permet la construction du bâtiment Guilde des Aventuriers',
            { gold: 100 },
            [],
            'guild'
        );
    }

    static createBankUpgrade() {
        return new CityUpgrade(
            'bank_unlock',
            'Débloquer les banques',
            'Permet la construction de bâtiments Banque',
            { gold: 100 },
            [],
            'finance'
        );
    }

    static createAlchemistUpgrade() {
        return new CityUpgrade(
            'alchemist_unlock',
            'Débloquer les alchimistes',
            'Permet la construction de bâtiments Alchimiste',
            { gold: 100 },
            [],
            'magic'
        );
    }

    static createEnchanterUpgrade() {
        return new CityUpgrade(
            'enchanter_unlock',
            'Débloquer les enchanteurs',
            'Permet la construction de bâtiments Enchanteur',
            { gold: 100 },
            [],
            'magic'
        );
    }

    static createPrisonUpgrade() {
        return new CityUpgrade(
            'prison_unlock',
            'Débloquer la prison',
            'Permet la construction du bâtiment Prison',
            { gold: 100 },
            [],
            'security'
        );
    }
}

if (typeof module !== 'undefined' && module.exports) {
    module.exports = CityUpgrade;
}