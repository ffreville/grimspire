/**
 * Classe GameManager - Gestionnaire principal du jeu
 */
class GameManager {
    constructor() {
        this.city = null;
        this.gameState = 'menu'; // 'menu', 'playing', 'paused'
        this.currentTab = 'batiments';
        this.saveKey = 'grimspire_save';
        
        // Callbacks pour les mises à jour UI
        this.onStateChange = null;
        this.onTabChange = null;
        this.onResourcesChange = null;
    }

    startNewGame() {
        console.log('Démarrage d\'une nouvelle partie...');
        
        // Créer une nouvelle ville
        this.city = new City();
        
        // Ajouter quelques aventuriers de départ
        this.addStartingAdventurers();
        
        // Changer l'état du jeu
        this.gameState = 'playing';
        
        // Sauvegarder automatiquement
        this.autoSave();
        
        // Notifier les changements
        this.notifyStateChange();
        
        return this.city.getGameState();
    }

    addStartingAdventurers() {
        const startingAdventurers = [
            new Adventurer('adv_1', 'Sir Gareth', 'guerrier'),
            new Adventurer('adv_2', 'Lyra la Sage', 'mage'),
            new Adventurer('adv_3', 'Finn Doigts-Agiles', 'voleur')
        ];
        
        startingAdventurers.forEach(adventurer => {
            this.city.addAdventurer(adventurer);
        });
    }

    loadGame() {
        try {
            const saveData = localStorage.getItem(this.saveKey);
            if (saveData) {
                const parsedData = JSON.parse(saveData);
                this.city = City.fromJSON(parsedData.city);
                this.gameState = parsedData.gameState || 'playing';
                this.currentTab = parsedData.currentTab || 'batiments';
                
                this.notifyStateChange();
                return true;
            }
        } catch (error) {
            console.error('Erreur lors du chargement de la sauvegarde:', error);
        }
        return false;
    }

    saveGame() {
        if (!this.city) return false;
        
        try {
            const saveData = {
                city: this.city.toJSON(),
                gameState: this.gameState,
                currentTab: this.currentTab,
                timestamp: Date.now()
            };
            
            localStorage.setItem(this.saveKey, JSON.stringify(saveData));
            return true;
        } catch (error) {
            console.error('Erreur lors de la sauvegarde:', error);
            return false;
        }
    }

    autoSave() {
        this.saveGame();
    }

    hasSaveData() {
        return localStorage.getItem(this.saveKey) !== null;
    }

    switchTab(tabName) {
        this.currentTab = tabName;
        this.notifyTabChange();
        this.autoSave();
    }

    getCurrentGameState() {
        if (!this.city) return null;
        return this.city.getGameState();
    }

    performBuildingAction(buildingId, action) {
        if (!this.city) return { success: false, message: 'Aucune partie en cours' };
        
        let result;
        switch (action) {
            case 'build':
                result = this.city.buildBuilding(buildingId);
                break;
            case 'upgrade':
                result = this.city.upgradeBuilding(buildingId);
                break;
            default:
                return { success: false, message: 'Action inconnue' };
        }
        
        if (result.success) {
            this.notifyResourcesChange();
            this.autoSave();
        }
        
        return result;
    }

    switchTimePhase() {
        if (!this.city) return;
        
        this.city.switchTimePhase();
        this.notifyStateChange();
        this.autoSave();
    }

    addRandomAdventurer() {
        if (!this.city) return null;
        
        const names = ['Marcus', 'Elena', 'Thorin', 'Aria', 'Cedric', 'Luna', 'Ragnar', 'Iris'];
        const classes = ['guerrier', 'mage', 'voleur', 'clerc', 'ranger'];
        
        const randomName = names[Math.floor(Math.random() * names.length)];
        const randomClass = classes[Math.floor(Math.random() * classes.length)];
        const adventurerId = `adv_${Date.now()}`;
        
        const newAdventurer = new Adventurer(adventurerId, randomName, randomClass);
        this.city.addAdventurer(newAdventurer);
        
        this.autoSave();
        return newAdventurer.getDisplayInfo();
    }

    getResourcesInfo() {
        if (!this.city) return null;
        return this.city.resources.toJSON();
    }

    getBuildingsInfo() {
        if (!this.city) return [];
        return this.city.buildings.map(b => b.getDisplayInfo());
    }

    getAdventurersInfo() {
        if (!this.city) return [];
        return this.city.adventurers.map(a => a.getDisplayInfo());
    }

    // Système de callbacks pour les mises à jour UI
    setStateChangeCallback(callback) {
        this.onStateChange = callback;
    }

    setTabChangeCallback(callback) {
        this.onTabChange = callback;
    }

    setResourcesChangeCallback(callback) {
        this.onResourcesChange = callback;
    }

    notifyStateChange() {
        if (this.onStateChange) {
            this.onStateChange(this.getCurrentGameState());
        }
    }

    notifyTabChange() {
        if (this.onTabChange) {
            this.onTabChange(this.currentTab);
        }
    }

    notifyResourcesChange() {
        if (this.onResourcesChange) {
            this.onResourcesChange(this.getResourcesInfo());
        }
    }

    // Méthodes utilitaires pour les statistiques
    getGameStats() {
        if (!this.city) return null;
        
        const builtBuildings = this.city.getBuiltBuildings();
        const availableAdventurers = this.city.getAvailableAdventurers();
        
        return {
            day: this.city.day,
            isNight: this.city.isNight,
            currentActionPoints: this.city.currentActionPoints,
            maxActionPoints: this.city.isNight ? this.city.actionPoints.night : this.city.actionPoints.day,
            totalBuildings: this.city.buildings.length,
            builtBuildings: builtBuildings.length,
            totalAdventurers: this.city.adventurers.length,
            availableAdventurers: availableAdventurers.length
        };
    }

    resetGame() {
        this.city = null;
        this.gameState = 'menu';
        this.currentTab = 'batiments';
        localStorage.removeItem(this.saveKey);
    }
}

if (typeof module !== 'undefined' && module.exports) {
    module.exports = GameManager;
}