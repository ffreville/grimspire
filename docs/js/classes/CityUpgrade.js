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
    }

    canUnlock(cityUpgrades, cityResources) {
        // Vérifier si déjà débloqué
        if (this.unlocked) {
            return { canUnlock: false, reason: 'Déjà débloqué' };
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

    unlock() {
        this.unlocked = true;
        return {
            success: true,
            message: `${this.name} débloqué !`,
            upgrade: this.getDisplayInfo()
        };
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
            effects: this.effects
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
            effects: this.effects
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