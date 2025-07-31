/**
 * Classe CityUpgradeManager - Gestion des améliorations de ville
 */
class CityUpgradeManager {
    constructor(city) {
        this.city = city;
        this.upgrades = [];
        this.initializeUpgrades();
    }

    initializeUpgrades() {
        // Créer toutes les améliorations disponibles
        this.upgrades = [
            this.createGuildUpgrade(),
            this.createBankUpgrade(),
            this.createAlchemistUpgrade(),
            this.createEnchanterUpgrade(),
            this.createPrisonUpgrade()
        ];
    }

    createGuildUpgrade() {
        const upgrade = new CityUpgrade(
            'guild_unlock',
            'Débloquer la guilde des aventuriers',
            'Permet la construction du bâtiment Guilde des Aventuriers. Nécessaire pour recruter et gérer des aventuriers.',
            { gold: 100 }
        );
        upgrade.icon = '⚔️';
        upgrade.category = 'guild';
        return upgrade;
    }

    createBankUpgrade() {
        const upgrade = new CityUpgrade(
            'bank_unlock',
            'Débloquer les banques',
            'Permet la construction de bâtiments Banque. Augmente la génération d\'or et permet des transactions avancées.',
            { gold: 100 }
        );
        upgrade.icon = '🏦';
        upgrade.category = 'finance';
        return upgrade;
    }

    createAlchemistUpgrade() {
        const upgrade = new CityUpgrade(
            'alchemist_unlock',
            'Débloquer les alchimistes',
            'Permet la construction de bâtiments Alchimiste. Nécessaire pour créer des potions et objets magiques.',
            { gold: 100 }
        );
        upgrade.icon = '🧪';
        upgrade.category = 'magic';
        return upgrade;
    }

    createEnchanterUpgrade() {
        const upgrade = new CityUpgrade(
            'enchanter_unlock',
            'Débloquer les enchanteurs',
            'Permet la construction de bâtiments Enchanteur. Nécessaire pour enchanter les équipements des aventuriers.',
            { gold: 100 }
        );
        upgrade.icon = '✨';
        upgrade.category = 'magic';
        return upgrade;
    }

    createPrisonUpgrade() {
        const upgrade = new CityUpgrade(
            'prison_unlock',
            'Débloquer la prison',
            'Permet la construction du bâtiment Prison. Améliore la sécurité et l\'ordre dans la ville.',
            { gold: 100 }
        );
        upgrade.icon = '🔒';
        upgrade.category = 'security';
        return upgrade;
    }

    getUpgradeById(upgradeId) {
        return this.upgrades.find(u => u.id === upgradeId);
    }

    canUnlockUpgrade(upgradeId) {
        const upgrade = this.getUpgradeById(upgradeId);
        if (!upgrade) {
            return { canUnlock: false, reason: 'Amélioration introuvable' };
        }

        // Vérifier qu'il n'y a pas déjà une recherche en cours
        const currentDevelopments = this.getCurrentDevelopments();
        if (currentDevelopments.length > 0) {
            const currentResearch = currentDevelopments[0];
            return { 
                canUnlock: false, 
                reason: `Une recherche est déjà en cours : ${currentResearch.name}` 
            };
        }

        return upgrade.canUnlock(this.upgrades, this.city.resources);
    }

    unlockUpgrade(upgradeId) {
        const upgrade = this.getUpgradeById(upgradeId);
        if (!upgrade) {
            return { success: false, message: 'Amélioration introuvable' };
        }

        const canUnlock = this.canUnlockUpgrade(upgradeId);
        if (!canUnlock.canUnlock) {
            return { success: false, message: canUnlock.reason };
        }

        // Vérifier les points d'action
        if (!this.city.canPerformAction(1)) {
            return { success: false, message: 'Points d\'action insuffisants' };
        }

        // Payer le coût
        this.city.resources.spend(upgrade.cost);
        this.city.performAction(1);

        // Démarrer le développement
        const started = upgrade.startDevelopment();
        if (started) {
            return {
                success: true,
                message: `Recherche de ${upgrade.name} commencée ! Durée : ${upgrade.constructionTime}h`,
                upgrade: upgrade.getDisplayInfo()
            };
        } else {
            // Fallback sur l'ancien système si le nouveau échoue
            const result = upgrade.unlock();
            this.applyUpgradeEffects(upgrade);
            return result;
        }
    }

    applyUpgradeEffects(upgrade) {
        // Ici on peut ajouter des effets spéciaux selon l'amélioration
        switch (upgrade.id) {
            case 'guild_unlock':
                // L'effet sera géré dans le système de bâtiments
                console.log('Guilde des aventuriers débloquée');
                break;
            case 'bank_unlock':
                console.log('Banques débloquées');
                break;
            case 'alchemist_unlock':
                console.log('Alchimistes débloqués');
                break;
            case 'enchanter_unlock':
                console.log('Enchanteurs débloqués');
                break;
            case 'prison_unlock':
                console.log('Prison débloquée');
                break;
        }
    }

    isUpgradeUnlocked(upgradeId) {
        const upgrade = this.getUpgradeById(upgradeId);
        return upgrade ? upgrade.unlocked : false;
    }

    getUnlockedUpgrades() {
        return this.upgrades.filter(u => u.unlocked).map(u => u.getDisplayInfo());
    }

    getAvailableUpgrades() {
        return this.upgrades.filter(u => !u.unlocked).map(u => u.getDisplayInfo());
    }

    getAllUpgrades() {
        return this.upgrades.map(u => u.getDisplayInfo());
    }

    getUpgradeStats() {
        const total = this.upgrades.length;
        const unlocked = this.upgrades.filter(u => u.unlocked).length;
        const available = total - unlocked;

        return {
            total,
            unlocked,
            available,
            progress: Math.round((unlocked / total) * 100)
        };
    }

    // Obtenir les améliorations en cours de développement
    getCurrentDevelopments() {
        return this.upgrades.filter(u => u.isUnderDevelopment);
    }

    // Traitement temporel pour faire avancer les recherches
    processTimeProgress(gameMinutesElapsed) {
        const completedUpgrades = [];

        this.upgrades.forEach(upgrade => {
            const result = upgrade.advanceProgress(gameMinutesElapsed);
            if (result && result.completed) {
                completedUpgrades.push(upgrade);
                // Appliquer les effets de l'amélioration nouvellement débloquée
                this.applyUpgradeEffects(upgrade);
            }
        });

        return {
            completedUpgrades,
            currentDevelopments: this.getCurrentDevelopments().length
        };
    }

    // Gestion du changement de tour (si nécessaire)
    processTurnChange() {
        // Pour l'instant, aucun traitement spécial nécessaire
        // Mais on garde la méthode pour une éventuelle évolution
    }

    // Sérialisation pour la sauvegarde
    toJSON() {
        return {
            upgrades: this.upgrades.map(u => u.toJSON())
        };
    }

    static fromJSON(data, city) {
        const manager = new CityUpgradeManager(city);
        
        if (data.upgrades) {
            manager.upgrades = data.upgrades.map(upgradeData => 
                CityUpgrade.fromJSON(upgradeData)
            );
        }
        
        return manager;
    }
}

if (typeof module !== 'undefined' && module.exports) {
    module.exports = CityUpgradeManager;
}