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

        // Contrôles de la guilde
        const searchAdventurersBtn = document.getElementById('search-adventurers-btn');
        if (searchAdventurersBtn) {
            searchAdventurersBtn.addEventListener('click', this.searchForAdventurers.bind(this));
        }

        // Contrôles des expéditions
        const refreshMissionsBtn = document.getElementById('refresh-missions-btn');
        if (refreshMissionsBtn) {
            refreshMissionsBtn.addEventListener('click', this.refreshMissions.bind(this));
        }

        // Modal de sélection d'aventuriers
        const closeModalBtn = document.getElementById('close-modal-btn');
        const cancelMissionBtn = document.getElementById('cancel-mission-btn');
        const startMissionBtn = document.getElementById('start-mission-btn');

        if (closeModalBtn) {
            closeModalBtn.addEventListener('click', this.closeAdventurerModal.bind(this));
        }
        if (cancelMissionBtn) {
            cancelMissionBtn.addEventListener('click', this.closeAdventurerModal.bind(this));
        }
        if (startMissionBtn) {
            startMissionBtn.addEventListener('click', this.confirmStartMission.bind(this));
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
                // Affichage initial selon l'onglet actuel
                if (this.gameManager.currentTab === 'guilde') {
                    this.renderGuild();
                } else if (this.gameManager.currentTab === 'expedition') {
                    this.renderExpeditions();
                }
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
                // Affichage initial selon l'onglet actuel
                if (this.gameManager.currentTab === 'guilde') {
                    this.renderGuild();
                } else if (this.gameManager.currentTab === 'expedition') {
                    this.renderExpeditions();
                }
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
        } else if (tabName === 'guilde') {
            this.renderGuild();
        } else if (tabName === 'expedition') {
            this.renderExpeditions();
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

    // === MÉTHODES POUR L'ONGLET GUILDE ===

    renderGuild() {
        const guildInfo = this.gameManager.getGuildInfo();
        if (!guildInfo) return;

        this.updateGuildStats(guildInfo.stats);
        this.updateSearchInfo(guildInfo.searchInfo);
        this.renderRecruitedAdventurers(guildInfo.recruited);
        this.renderAvailableAdventurers(guildInfo.available);
    }

    updateGuildStats(stats) {
        document.getElementById('guild-total-count').textContent = stats.totalRecruited;
        document.getElementById('guild-available-count').textContent = stats.availableForMission;
        document.getElementById('guild-mission-count').textContent = stats.onMission;
        document.getElementById('guild-avg-level').textContent = stats.averageLevel;
        document.getElementById('recruited-count').textContent = `${stats.totalRecruited} aventurier${stats.totalRecruited > 1 ? 's' : ''}`;
    }

    updateSearchInfo(searchInfo) {
        const searchBtn = document.getElementById('search-adventurers-btn');
        const searchInfoElement = document.getElementById('search-info');
        
        if (searchBtn && searchInfoElement) {
            searchBtn.disabled = !searchInfo.canSearch;
            
            if (searchInfo.canSearch) {
                searchInfoElement.textContent = `Coût: ${searchInfo.cost.gold}💰`;
            } else {
                searchInfoElement.textContent = searchInfo.reason;
            }
        }
    }

    renderRecruitedAdventurers(adventurers) {
        const container = document.getElementById('recruited-adventurers');
        if (!container) return;

        container.innerHTML = '';

        if (adventurers.length === 0) {
            container.innerHTML = `
                <div style="text-align: center; padding: 40px; color: #999;">
                    <p>Aucun aventurier recruté</p>
                    <p style="font-size: 0.9rem;">Utilisez la recherche pour trouver des aventuriers à recruter</p>
                </div>
            `;
            return;
        }

        adventurers.forEach(adventurer => {
            const card = this.createAdventurerCard(adventurer, false);
            container.appendChild(card);
        });
    }

    renderAvailableAdventurers(adventurers) {
        const container = document.getElementById('available-adventurers');
        if (!container) return;

        container.innerHTML = '';

        if (adventurers.length === 0) {
            container.innerHTML = `
                <div style="text-align: center; padding: 40px; color: #999;">
                    <p>Aucun aventurier disponible</p>
                    <p style="font-size: 0.9rem;">Utilisez le bouton "Rechercher" pour trouver de nouveaux aventuriers</p>
                </div>
            `;
            return;
        }

        adventurers.forEach(adventurer => {
            const card = this.createAdventurerCard(adventurer, true);
            container.appendChild(card);
        });
    }

    createAdventurerCard(adventurer, isRecruit = false) {
        const card = document.createElement('div');
        card.className = `adventurer-card ${isRecruit ? 'recruit' : ''} ${adventurer.isOnMission ? 'on-mission' : ''}`;

        const healthPercent = Math.round((adventurer.health / adventurer.maxHealth) * 100);
        const healthClass = healthPercent < 25 ? 'low' : healthPercent < 60 ? 'medium' : '';

        // Créer la liste des statistiques
        const statsHtml = Object.entries(adventurer.stats).map(([stat, value]) => {
            const statNames = {
                'force': 'Force',
                'intelligence': 'Intelligence',
                'agilite': 'Agilité',
                'charisme': 'Charisme',
                'chance': 'Chance'
            };
            return `
                <div class="stat-item">
                    <span class="stat-name">${statNames[stat] || stat}</span>
                    <span class="stat-value">${value}</span>
                </div>
            `;
        }).join('');

        // Créer les spécialisations
        const specializationsHtml = adventurer.specializations && adventurer.specializations.length > 0 ? `
            <div class="specializations">
                <div class="specializations-label">Spécialisations:</div>
                <div class="specializations-list">
                    ${adventurer.specializations.map(spec => 
                        `<span class="specialization-tag">${spec}</span>`
                    ).join('')}
                </div>
            </div>
        ` : '';

        // Créer le coût de recrutement
        const costHtml = isRecruit && adventurer.recruitmentCost ? `
            <div class="adventurer-cost">
                <div class="cost-label">Coût de recrutement:</div>
                <div class="cost-list">
                    ${this.formatCost(adventurer.recruitmentCost)}
                </div>
            </div>
        ` : '';

        card.innerHTML = `
            ${adventurer.isOnMission ? '<div class="mission-indicator">En Mission</div>' : ''}
            <div class="adventurer-header">
                <h4 class="adventurer-name">${adventurer.name}</h4>
                <span class="adventurer-level">Niv. ${adventurer.level}</span>
            </div>
            <div class="adventurer-class">${adventurer.class}</div>
            
            <div class="adventurer-stats">
                ${statsHtml}
            </div>
            
            <div class="adventurer-health">
                <div class="health-text">
                    <span>Santé</span>
                    <span>${adventurer.health}/${adventurer.maxHealth}</span>
                </div>
                <div class="health-bar">
                    <div class="health-fill ${healthClass}" style="width: ${healthPercent}%"></div>
                </div>
            </div>
            
            ${costHtml}
            ${specializationsHtml}
            
            <div class="adventurer-actions">
                ${this.createAdventurerActions(adventurer, isRecruit)}
            </div>
        `;

        return card;
    }

    createAdventurerActions(adventurer, isRecruit) {
        if (isRecruit) {
            const resources = this.gameManager.getResourcesInfo();
            const canAfford = resources && this.canAffordCost(resources, adventurer.recruitmentCost);
            const hasActionPoints = this.gameManager.city && this.gameManager.city.canPerformAction(1);

            return `
                <button class="adventurer-btn recruit" 
                        onclick="app.recruitAdventurer('${adventurer.id}')"
                        ${!canAfford || !hasActionPoints ? 'disabled' : ''}>
                    Recruter
                </button>
            `;
        } else {
            if (adventurer.isOnMission) {
                return `<button class="adventurer-btn" disabled>En Mission</button>`;
            } else {
                return `
                    <button class="adventurer-btn dismiss" 
                            onclick="app.dismissAdventurer('${adventurer.id}')">
                        Renvoyer
                    </button>
                `;
            }
        }
    }

    // Actions pour l'onglet Guilde
    searchForAdventurers() {
        const result = this.gameManager.searchForAdventurers();
        this.showActionResult(result);
        
        if (result.success) {
            this.renderGuild();
        }
    }

    recruitAdventurer(adventurerId) {
        const result = this.gameManager.recruitAdventurer(adventurerId);
        this.showActionResult(result);
        
        if (result.success) {
            this.renderGuild();
        }
    }

    dismissAdventurer(adventurerId) {
        // Demander confirmation
        if (!confirm('Êtes-vous sûr de vouloir renvoyer cet aventurier ?')) {
            return;
        }
        
        const result = this.gameManager.dismissAdventurer(adventurerId);
        this.showActionResult(result);
        
        if (result.success) {
            this.renderGuild();
        }
    }

    // === MÉTHODES POUR L'ONGLET EXPÉDITIONS ===

    renderExpeditions() {
        const missionInfo = this.gameManager.getMissionInfo();
        if (!missionInfo) return;

        this.updateMissionStats(missionInfo.stats);
        this.updateRefreshInfo(missionInfo.refreshInfo);
        this.renderActiveMissions(missionInfo.active);
        this.renderAvailableMissions(missionInfo.available);
        this.renderCompletedMissions(missionInfo.completed);
    }

    updateMissionStats(stats) {
        document.getElementById('mission-active-count').textContent = stats.activeMissions;
        document.getElementById('mission-success-count').textContent = stats.successfulMissions;
        document.getElementById('mission-success-rate').textContent = `${stats.successRate}%`;
        document.getElementById('mission-available-count').textContent = stats.availableMissions;
        document.getElementById('active-missions-count').textContent = `${stats.activeMissions} mission(s)`;
    }

    updateRefreshInfo(refreshInfo) {
        const refreshBtn = document.getElementById('refresh-missions-btn');
        const refreshInfoElement = document.getElementById('refresh-info');
        
        if (refreshBtn && refreshInfoElement) {
            refreshBtn.disabled = !refreshInfo.canRefresh;
            
            if (refreshInfo.canRefresh) {
                refreshInfoElement.textContent = `Coût: ${refreshInfo.cost.gold}💰`;
            } else {
                refreshInfoElement.textContent = refreshInfo.reason;
            }
        }
    }

    renderActiveMissions(missions) {
        const container = document.getElementById('active-missions');
        if (!container) return;

        container.innerHTML = '';

        if (missions.length === 0) {
            container.innerHTML = `
                <div style="text-align: center; padding: 40px; color: #999;">
                    <p>Aucune mission en cours</p>
                    <p style="font-size: 0.9rem;">Sélectionnez une mission disponible pour commencer</p>
                </div>
            `;
            return;
        }

        missions.forEach(mission => {
            const card = this.createMissionCard(mission, 'active');
            container.appendChild(card);
        });
    }

    renderAvailableMissions(missions) {
        const container = document.getElementById('available-missions');
        if (!container) return;

        container.innerHTML = '';

        if (missions.length === 0) {
            container.innerHTML = `
                <div style="text-align: center; padding: 40px; color: #999;">
                    <p>Aucune mission disponible</p>
                    <p style="font-size: 0.9rem;">Utilisez le bouton "Actualiser" pour générer de nouvelles missions</p>
                </div>
            `;
            return;
        }

        missions.forEach(mission => {
            const card = this.createMissionCard(mission, 'available');
            container.appendChild(card);
        });
    }

    renderCompletedMissions(missions) {
        const container = document.getElementById('completed-missions');
        if (!container) return;

        container.innerHTML = '';

        if (missions.length === 0) {
            container.innerHTML = `
                <div style="text-align: center; padding: 40px; color: #999;">
                    <p>Aucune mission terminée récemment</p>
                </div>
            `;
            return;
        }

        missions.forEach(mission => {
            const card = this.createMissionCard(mission, 'completed');
            container.appendChild(card);
        });
    }

    createMissionCard(mission, status) {
        const card = document.createElement('div');
        const successClass = mission.results && mission.results.success ? 'success' : 'failed';
        card.className = `mission-card ${status} ${status === 'completed' ? successClass : ''}`;

        let progressHtml = '';
        let adventurersHtml = '';
        let actionsHtml = '';

        // Contenu spécifique selon le statut
        if (status === 'active') {
            progressHtml = `
                <div class="mission-progress">
                    <div class="progress-text">
                        <span>Progression</span>
                        <span>${mission.formattedRemainingTime || '0h0m'} restant</span>
                    </div>
                    <div class="progress-bar">
                        <div class="progress-fill" style="width: ${mission.progress}%"></div>
                    </div>
                </div>
            `;
            
            adventurersHtml = this.createMissionAdventurersHtml(mission.adventurers);
        } else if (status === 'available') {
            actionsHtml = `
                <div class="mission-actions">
                    <button class="mission-btn start" onclick="app.openAdventurerSelection('${mission.id}')">
                        Lancer Mission
                    </button>
                </div>
            `;
        } else if (status === 'completed') {
            adventurersHtml = this.createMissionAdventurersHtml(mission.adventurers);
            
            if (mission.results) {
                progressHtml = `
                    <div class="mission-results">
                        <div style="color: ${mission.results.success ? '#4a7c59' : '#c94a4a'}; font-weight: 500; margin-bottom: 8px;">
                            ${mission.results.success ? '✅ Mission Réussie' : '❌ Mission Échouée'}
                        </div>
                        <div style="font-size: 0.85rem; color: #c9c9c9; line-height: 1.3;">
                            ${mission.results.message}
                        </div>
                    </div>
                `;
            }
        }

        const rewardsHtml = this.createMissionRewardsHtml(mission.rewards);

        card.innerHTML = `
            <div class="mission-header">
                <div class="mission-title">
                    <span class="mission-type-icon">${mission.typeIcon}</span>
                    <h4 class="mission-name">${mission.name}</h4>
                </div>
                <span class="mission-difficulty">${mission.difficultyStars}</span>
            </div>
            
            <div class="mission-description">${mission.description}</div>
            
            <div class="mission-details">
                <div class="mission-detail">
                    <span>Durée</span>
                    <span class="detail-value">${mission.formattedDuration}</span>
                </div>
                <div class="mission-detail">
                    <span>Type</span>
                    <span class="detail-value">${mission.type}</span>
                </div>
            </div>
            
            <div class="mission-party-size">
                <span>Aventuriers requis: </span>
                <span class="party-size-value">${mission.requiredPartySize.min}-${mission.requiredPartySize.max}</span>
                <span style="margin-left: 10px; color: #999;">(recommandé: ${mission.requiredPartySize.recommended})</span>
            </div>
            
            ${rewardsHtml}
            ${progressHtml}
            ${adventurersHtml}
            ${actionsHtml}
        `;

        return card;
    }

    createMissionRewardsHtml(rewards) {
        if (!rewards || Object.keys(rewards).length === 0) {
            return '<div class="mission-rewards"><h5>Récompenses:</h5><span>Aucune</span></div>';
        }

        const rewardItems = Object.entries(rewards).map(([resource, amount]) => {
            const icons = {
                'gold': '💰',
                'experience': '⭐',
                'materials': '🔨',
                'magic': '✨',
                'reputation': '🏆'
            };
            
            return `<span class="reward-item">${icons[resource] || resource}: ${amount}</span>`;
        }).join('');

        return `
            <div class="mission-rewards">
                <h5>Récompenses:</h5>
                <div class="rewards-list">
                    ${rewardItems}
                </div>
            </div>
        `;
    }

    createMissionAdventurersHtml(adventurerIds) {
        if (!adventurerIds || adventurerIds.length === 0) {
            return '';
        }

        const adventurerNames = adventurerIds.map(id => {
            const adventurer = this.gameManager.city.getAdventurerById(id);
            return adventurer ? adventurer.name : 'Inconnu';
        });

        const adventurerTags = adventurerNames.map(name => 
            `<span class="adventurer-tag">${name}</span>`
        ).join('');

        return `
            <div class="mission-adventurers">
                <h5>Équipe:</h5>
                <div class="adventurers-list">
                    ${adventurerTags}
                </div>
            </div>
        `;
    }

    // Actions pour l'onglet Expéditions
    refreshMissions() {
        const result = this.gameManager.refreshMissions();
        this.showActionResult(result);
        
        if (result.success) {
            this.renderExpeditions();
        }
    }

    openAdventurerSelection(missionId) {
        this.currentMissionId = missionId;
        const missionInfo = this.gameManager.getMissionInfo();
        const mission = missionInfo.available.find(m => m.id === missionId);
        
        if (!mission) {
            this.showActionResult({ success: false, message: 'Mission introuvable' });
            return;
        }

        // Remplir la modal avec les informations de la mission
        document.getElementById('modal-mission-name').textContent = mission.name;
        document.getElementById('modal-mission-description').textContent = mission.description;
        document.getElementById('modal-party-size').textContent = `${mission.requiredPartySize.min}-${mission.requiredPartySize.max}`;
        document.getElementById('modal-difficulty').textContent = mission.difficultyStars;

        // Remplir la liste des aventuriers disponibles
        this.renderModalAdventurers();

        // Réinitialiser la sélection
        this.selectedAdventurers = [];
        this.updateSelectedAdventurersList();

        // Afficher la modal
        document.getElementById('adventurer-selection-modal').classList.add('active');
    }

    renderModalAdventurers() {
        const container = document.getElementById('modal-adventurers-list');
        if (!container) return;

        const availableAdventurers = this.gameManager.adventurerManager.getAvailableAdventurers();
        container.innerHTML = '';

        if (availableAdventurers.length === 0) {
            container.innerHTML = `
                <div style="text-align: center; padding: 20px; color: #999;">
                    <p>Aucun aventurier disponible</p>
                </div>
            `;
            return;
        }

        availableAdventurers.forEach(adventurer => {
            const card = document.createElement('div');
            card.className = 'modal-adventurer-card';
            card.onclick = () => this.toggleAdventurerSelection(adventurer.id);

            card.innerHTML = `
                <div class="modal-adventurer-name">${adventurer.name}</div>
                <div class="modal-adventurer-class">${adventurer.class}</div>
                <div class="modal-adventurer-level">Niveau ${adventurer.level} - Puissance: ${adventurer.combatPower}</div>
            `;

            container.appendChild(card);
        });
    }

    toggleAdventurerSelection(adventurerId) {
        const index = this.selectedAdventurers.indexOf(adventurerId);
        
        if (index === -1) {
            // Ajouter à la sélection
            this.selectedAdventurers.push(adventurerId);
        } else {
            // Retirer de la sélection
            this.selectedAdventurers.splice(index, 1);
        }

        this.updateModalAdventurerCards();
        this.updateSelectedAdventurersList();
        this.updateStartMissionButton();
    }

    updateModalAdventurerCards() {
        const cards = document.querySelectorAll('.modal-adventurer-card');
        cards.forEach((card, index) => {
            const adventurerId = this.gameManager.adventurerManager.getAvailableAdventurers()[index]?.id;
            if (adventurerId) {
                if (this.selectedAdventurers.includes(adventurerId)) {
                    card.classList.add('selected');
                } else {
                    card.classList.remove('selected');
                }
            }
        });
    }

    updateSelectedAdventurersList() {
        const container = document.getElementById('selected-adventurers-list');
        const countElement = document.getElementById('selected-count');
        
        if (!container || !countElement) return;

        countElement.textContent = this.selectedAdventurers.length;
        container.innerHTML = '';

        if (this.selectedAdventurers.length === 0) {
            container.innerHTML = '<div style="text-align: center; color: #999; padding: 10px;">Aucun aventurier sélectionné</div>';
            return;
        }

        this.selectedAdventurers.forEach(adventurerId => {
            const adventurer = this.gameManager.city.getAdventurerById(adventurerId);
            if (adventurer) {
                const tag = document.createElement('div');
                tag.className = 'selected-adventurer-tag';
                tag.innerHTML = `
                    ${adventurer.name}
                    <button class="remove-adventurer" onclick="app.toggleAdventurerSelection('${adventurerId}')">×</button>
                `;
                container.appendChild(tag);
            }
        });
    }

    updateStartMissionButton() {
        const button = document.getElementById('start-mission-btn');
        if (!button) return;

        const missionInfo = this.gameManager.getMissionInfo();
        const mission = missionInfo.available.find(m => m.id === this.currentMissionId);
        
        if (!mission) {
            button.disabled = true;
            return;
        }

        const isValidPartySize = this.selectedAdventurers.length >= mission.requiredPartySize.min && 
                                this.selectedAdventurers.length <= mission.requiredPartySize.max;
        
        button.disabled = !isValidPartySize;
    }

    confirmStartMission() {
        if (!this.currentMissionId || this.selectedAdventurers.length === 0) {
            return;
        }

        const result = this.gameManager.startMission(this.currentMissionId, this.selectedAdventurers);
        this.showActionResult(result);

        if (result.success) {
            this.closeAdventurerModal();
            this.renderExpeditions();
            // Mettre à jour aussi l'onglet guilde car les aventuriers sont maintenant en mission
            if (this.gameManager.currentTab === 'expedition') {
                // Si on doit aussi rafraîchir la guilde, on peut le faire ici
            }
        }
    }

    closeAdventurerModal() {
        document.getElementById('adventurer-selection-modal').classList.remove('active');
        this.currentMissionId = null;
        this.selectedAdventurers = [];
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