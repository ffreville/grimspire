/**
 * Grimspire - Application Principale
 * Phase 1.2 - Système de données et menu de jeu avec onglets
 */

class GrimspireApp {
    constructor() {
        this.gameManager = new GameManager();
        this.currentScreen = 'menu';
        this.initializeEventListeners();
    }

    initializeEventListeners() {
        // Menu principal
        this.initializeMainMenu();
        
        // Interface de jeu
        this.initializeGameInterface();
        
        // Navigation onglets
        this.initializeTabNavigation();
    }

    initializeMainMenu() {
        // Bouton Nouvelle Partie
        const newGameBtn = document.getElementById('new-game-btn');
        if (newGameBtn) {
            newGameBtn.addEventListener('click', this.startNewGame.bind(this));
        }

        // Bouton Charger Partie
        const loadGameBtn = document.getElementById('load-game-btn');
        if (loadGameBtn) {
            if (this.gameManager.hasSaveData()) {
                loadGameBtn.disabled = false;
                loadGameBtn.addEventListener('click', this.loadGame.bind(this));
            } else {
                loadGameBtn.addEventListener('click', () => this.showNotImplemented('Charger Partie'));
            }
        }
        
        // Autres boutons
        const settingsBtn = document.getElementById('settings-btn');
        const quitBtn = document.getElementById('quit-btn');

        if (settingsBtn) {
            settingsBtn.addEventListener('click', () => this.showNotImplemented('Options'));
        }
        
        if (quitBtn) {
            quitBtn.addEventListener('click', () => this.showNotImplemented('Quitter'));
        }
    }

    initializeGameInterface() {
        // Contrôles de jeu
        const nextPhaseBtn = document.getElementById('next-phase-btn');
        const saveGameBtn = document.getElementById('save-game-btn');
        const returnMenuBtn = document.getElementById('return-menu-btn');

        if (nextPhaseBtn) {
            nextPhaseBtn.addEventListener('click', this.nextPhase.bind(this));
        }

        if (saveGameBtn) {
            saveGameBtn.addEventListener('click', this.saveGame.bind(this));
        }

        if (returnMenuBtn) {
            returnMenuBtn.addEventListener('click', this.returnToMainMenu.bind(this));
        }

        // Callbacks du GameManager
        this.gameManager.setStateChangeCallback(this.updateGameInterface.bind(this));
        this.gameManager.setTabChangeCallback(this.updateActiveTab.bind(this));
        this.gameManager.setResourcesChangeCallback(this.updateResources.bind(this));
    }

    initializeTabNavigation() {
        const tabButtons = document.querySelectorAll('.tab-btn');
        tabButtons.forEach(button => {
            button.addEventListener('click', (e) => {
                const tabName = e.target.getAttribute('data-tab');
                this.switchTab(tabName);
            });
        });
    }

    startNewGame() {
        console.log('Démarrage d\'une nouvelle partie...');
        
        const gameState = this.gameManager.startNewGame();
        if (gameState) {
            this.fadeTransition(() => {
                this.switchToGameScreen();
                this.updateGameInterface(gameState);
                this.renderBuildings();
            });
        }
    }

    loadGame() {
        console.log('Chargement de la partie...');
        
        if (this.gameManager.loadGame()) {
            this.fadeTransition(() => {
                this.switchToGameScreen();
                this.updateGameInterface(this.gameManager.getCurrentGameState());
                this.renderBuildings();
            });
        } else {
            alert('Erreur lors du chargement de la sauvegarde');
        }
    }

    switchToGameScreen() {
        const mainMenu = document.getElementById('main-menu');
        const gameScreen = document.getElementById('game-screen');
        
        if (mainMenu && gameScreen) {
            mainMenu.classList.remove('active');
            gameScreen.classList.add('active');
            this.currentScreen = 'game';
        }
    }

    returnToMainMenu() {
        const mainMenu = document.getElementById('main-menu');
        const gameScreen = document.getElementById('game-screen');
        
        if (mainMenu && gameScreen) {
            gameScreen.classList.remove('active');
            mainMenu.classList.add('active');
            this.currentScreen = 'menu';
        }
    }

    switchTab(tabName) {
        this.gameManager.switchTab(tabName);
        this.updateActiveTab(tabName);
        
        // Mettre à jour le contenu selon l'onglet
        if (tabName === 'batiments') {
            this.renderBuildings();
        }
    }

    updateActiveTab(tabName) {
        // Mettre à jour les boutons d'onglets
        document.querySelectorAll('.tab-btn').forEach(btn => {
            btn.classList.remove('active');
        });
        document.querySelector(`[data-tab="${tabName}"]`)?.classList.add('active');

        // Mettre à jour les panneaux
        document.querySelectorAll('.tab-panel').forEach(panel => {
            panel.classList.remove('active');
        });
        document.getElementById(`tab-${tabName}`)?.classList.add('active');
    }

    updateGameInterface(gameState) {
        if (!gameState) return;

        // Mettre à jour les informations de la ville
        document.getElementById('city-name').textContent = gameState.name;
        document.getElementById('day-counter').textContent = `Jour ${gameState.day}`;
        document.getElementById('phase-indicator').textContent = gameState.isNight ? 'Nuit' : 'Jour';
        document.getElementById('action-points').textContent = `${gameState.currentActionPoints}/${gameState.maxActionPoints} PA`;

        // Mettre à jour le bouton de phase
        const nextPhaseBtn = document.getElementById('next-phase-btn');
        if (nextPhaseBtn) {
            nextPhaseBtn.textContent = gameState.isNight ? 'Passer au Jour' : 'Passer à la Nuit';
        }

        this.updateResources(gameState.resources);
    }

    updateResources(resources) {
        if (!resources) return;

        document.getElementById('gold-amount').textContent = resources.gold;
        document.getElementById('population-amount').textContent = resources.population;
        document.getElementById('materials-amount').textContent = resources.materials;
        document.getElementById('magic-amount').textContent = resources.magic;
        document.getElementById('reputation-amount').textContent = resources.reputation;
    }

    renderBuildings() {
        const buildingsContainer = document.getElementById('buildings-list');
        if (!buildingsContainer) return;

        const buildings = this.gameManager.getBuildingsInfo();
        
        buildingsContainer.innerHTML = '';
        
        buildings.forEach(building => {
            const buildingCard = this.createBuildingCard(building);
            buildingsContainer.appendChild(buildingCard);
        });
    }

    createBuildingCard(building) {
        const card = document.createElement('div');
        card.className = `building-card ${building.built ? 'built' : ''}`;
        
        const effects = this.formatEffects(building.effects);
        const cost = this.formatCost(building.upgradeCost);
        
        card.innerHTML = `
            <div class="building-header">
                <h4 class="building-name">${building.name}</h4>
                <span class="building-level">Niv. ${building.level}/${building.maxLevel}</span>
            </div>
            <div class="building-district">District: ${building.district}</div>
            
            <div class="building-effects">
                <h4>Effets:</h4>
                <div class="effects-list">
                    ${effects}
                </div>
            </div>
            
            <div class="building-cost">
                <strong>${building.built ? 'Coût amélioration:' : 'Coût construction:'}</strong>
                <div class="cost-list">
                    ${cost}
                </div>
            </div>
            
            <div class="building-actions">
                ${this.createBuildingActions(building)}
            </div>
        `;
        
        return card;
    }

    formatEffects(effects) {
        if (!effects || Object.keys(effects).length === 0) {
            return '<span class="effect-item">Aucun effet</span>';
        }
        
        return Object.entries(effects)
            .map(([key, value]) => {
                let effectName = key;
                let effectValue = value > 0 ? `+${value}` : `${value}`;
                
                // Traduire les noms d'effets
                const translations = {
                    'population': 'Population',
                    'goldPerTurn': 'Or/tour',
                    'materialsPerTurn': 'Matériaux/tour',
                    'magicPerTurn': 'Magie/tour',
                    'reputation': 'Réputation'
                };
                
                effectName = translations[key] || key;
                
                return `<span class="effect-item">${effectName}: ${effectValue}</span>`;
            })
            .join('');
    }

    formatCost(cost) {
        if (!cost || Object.keys(cost).length === 0) {
            return 'Gratuit';
        }
        
        return Object.entries(cost)
            .map(([resource, amount]) => {
                const icons = {
                    'gold': '💰',
                    'population': '👥',
                    'materials': '🔨',
                    'magic': '✨'
                };
                
                return `${icons[resource] || resource}: ${amount}`;
            })
            .join(' | ');
    }

    createBuildingActions(building) {
        const resources = this.gameManager.getResourcesInfo();
        const canAfford = resources && this.canAffordCost(resources, building.upgradeCost);
        const hasActionPoints = this.gameManager.city && this.gameManager.city.canPerformAction(building.built ? 1 : 2);
        
        if (!building.built) {
            return `
                <button class="building-btn primary" 
                        onclick="app.buildBuilding('${building.id}')"
                        ${!canAfford || !hasActionPoints ? 'disabled' : ''}>
                    Construire
                </button>
            `;
        } else if (building.level < building.maxLevel) {
            return `
                <button class="building-btn" 
                        onclick="app.upgradeBuilding('${building.id}')"
                        ${!canAfford || !hasActionPoints ? 'disabled' : ''}>
                    Améliorer
                </button>
            `;
        } else {
            return `<button class="building-btn" disabled>Niveau Maximum</button>`;
        }
    }

    canAffordCost(resources, cost) {
        return Object.entries(cost).every(([resource, amount]) => {
            return resources[resource] >= amount;
        });
    }

    buildBuilding(buildingId) {
        const result = this.gameManager.performBuildingAction(buildingId, 'build');
        this.showActionResult(result);
        
        if (result.success) {
            this.renderBuildings();
        }
    }

    upgradeBuilding(buildingId) {
        const result = this.gameManager.performBuildingAction(buildingId, 'upgrade');
        this.showActionResult(result);
        
        if (result.success) {
            this.renderBuildings();
        }
    }

    nextPhase() {
        this.gameManager.switchTimePhase();
        this.renderBuildings(); // Actualise les boutons d'action
    }

    saveGame() {
        if (this.gameManager.saveGame()) {
            this.showActionResult({ success: true, message: 'Partie sauvegardée' });
        } else {
            this.showActionResult({ success: false, message: 'Erreur de sauvegarde' });
        }
    }

    showActionResult(result) {
        const message = document.createElement('div');
        message.style.cssText = `
            position: fixed;
            top: 20px;
            right: 20px;
            background: ${result.success ? 'rgba(74, 124, 89, 0.9)' : 'rgba(139, 90, 60, 0.9)'};
            color: #e8e6e3;
            padding: 15px 20px;
            border-radius: 8px;
            border: 2px solid ${result.success ? '#4a7c59' : '#8b5a3c'};
            z-index: 1000;
            font-size: 0.9rem;
            max-width: 300px;
        `;
        
        message.textContent = result.message;
        document.body.appendChild(message);
        
        setTimeout(() => {
            if (message.parentElement) {
                message.remove();
            }
        }, 3000);
    }

    fadeTransition(callback) {
        const app = document.getElementById('app');
        
        app.style.transition = 'opacity 0.3s ease-in-out';
        app.style.opacity = '0';
        
        setTimeout(() => {
            callback();
            app.style.opacity = '1';
        }, 300);
    }

    showNotImplemented(featureName) {
        this.showActionResult({
            success: false,
            message: `${featureName} - Fonctionnalité non implémentée (prochaines phases)`
        });
    }
}

// Instance globale de l'application
let app;

document.addEventListener('DOMContentLoaded', () => {
    console.log('Grimspire - Phase 1.2 initialisée');
    app = new GrimspireApp();
});

// Gestion des erreurs globales
window.addEventListener('error', (event) => {
    console.error('Erreur JavaScript:', event.error);
});

// Export pour utilisation dans d'autres modules
if (typeof module !== 'undefined' && module.exports) {
    module.exports = { GrimspireApp };
}