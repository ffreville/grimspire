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
        
        // Navigation administration
        this.initializeAdminNavigation();
        
        // Navigation commerce
        this.initializeCommerceNavigation();
        
        // Contrôles des actions de marché
        this.initializeMarketActions();
        
        // Contrôles des actions d'artisans
        this.initializeArtisanActions();
        
        // Navigation industrie
        this.initializeIndustrieNavigation();
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
        const pauseGameBtn = document.getElementById('pause-game-btn');
        const saveGameBtn = document.getElementById('save-game-btn');
        const returnMenuBtn = document.getElementById('return-menu-btn');

        // Contrôles des bâtiments
        const closeBuildingModalBtn = document.getElementById('close-construction-modal-btn');
        const cancelConstructionBtn = document.getElementById('cancel-construction-btn');
        const confirmConstructionBtn = document.getElementById('confirm-construction-btn');
        
        if (closeBuildingModalBtn) {
            closeBuildingModalBtn.addEventListener('click', this.closeBuildingModal.bind(this));
        }
        if (cancelConstructionBtn) {
            cancelConstructionBtn.addEventListener('click', this.closeBuildingModal.bind(this));
        }
        if (confirmConstructionBtn) {
            confirmConstructionBtn.addEventListener('click', this.confirmBuildingConstruction.bind(this));
        }

        if (pauseGameBtn) {
            pauseGameBtn.addEventListener('click', this.togglePause.bind(this));
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

        // Contrôles des événements
        const markAllReadBtn = document.getElementById('mark-all-read-btn');
        const clearReadEventsBtn = document.getElementById('clear-read-events-btn');
        
        if (markAllReadBtn) {
            markAllReadBtn.addEventListener('click', this.markAllEventsAsRead.bind(this));
        }
        if (clearReadEventsBtn) {
            clearReadEventsBtn.addEventListener('click', this.clearReadEvents.bind(this));
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

    initializeAdminNavigation() {
        const adminMenuButtons = document.querySelectorAll('.admin-menu-btn');
        adminMenuButtons.forEach(button => {
            button.addEventListener('click', (e) => {
                if (!e.target.disabled) {
                    const adminTab = e.target.getAttribute('data-admin-tab');
                    this.switchAdminTab(adminTab);
                }
            });
        });
    }

    switchAdminTab(adminTabName) {
        // Mettre à jour les boutons du menu admin
        document.querySelectorAll('.admin-menu-btn').forEach(btn => {
            btn.classList.remove('active');
        });
        document.querySelector(`[data-admin-tab="${adminTabName}"]`)?.classList.add('active');

        // Mettre à jour les contenus admin
        document.querySelectorAll('.admin-content').forEach(content => {
            content.classList.remove('active');
        });
        document.getElementById(`admin-${adminTabName}`)?.classList.add('active');
        
        // Charger le contenu spécifique selon l'onglet admin
        if (adminTabName === 'upgrades') {
            this.renderCityUpgrades();
        }
    }

    initializeCommerceNavigation() {
        const commerceMenuButtons = document.querySelectorAll('.commerce-menu-btn');
        commerceMenuButtons.forEach(button => {
            button.addEventListener('click', (e) => {
                const commerceTab = e.target.getAttribute('data-commerce-tab');
                this.switchCommerceTab(commerceTab);
            });
        });
    }

    switchCommerceTab(commerceTabName) {
        // Mettre à jour les boutons du menu commerce
        document.querySelectorAll('.commerce-menu-btn').forEach(btn => {
            btn.classList.remove('active');
        });
        document.querySelector(`[data-commerce-tab="${commerceTabName}"]`)?.classList.add('active');

        // Mettre à jour les contenus commerce
        document.querySelectorAll('.commerce-content').forEach(content => {
            content.classList.remove('active');
        });
        document.getElementById(`commerce-${commerceTabName}`)?.classList.add('active');
        
        // Si on affiche l'onglet marché, mettre à jour les actions
        if (commerceTabName === 'marche') {
            this.renderMarketActions();
        } else if (commerceTabName === 'artisans') {
            this.renderArtisanActions();
        }
    }

    initializeMarketActions() {
        // Bouton pour envoyer un négociateur
        const sendNegotiatorBtn = document.getElementById('send-negotiator-btn');
        if (sendNegotiatorBtn) {
            sendNegotiatorBtn.addEventListener('click', () => this.sendNegotiator());
        }

        // Bouton pour envoyer un émissaire
        const sendEmissaryBtn = document.getElementById('send-emissary-btn');
        if (sendEmissaryBtn) {
            sendEmissaryBtn.addEventListener('click', () => this.sendEmissary());
        }
    }

    initializeArtisanActions() {
        // Bouton pour lancer le travail de nuit
        const startNightWorkBtn = document.getElementById('start-night-work-btn');
        if (startNightWorkBtn) {
            startNightWorkBtn.addEventListener('click', () => this.startNightWork());
        }

        // Bouton pour lancer les soldes
        const startClearanceBtn = document.getElementById('start-clearance-btn');
        if (startClearanceBtn) {
            startClearanceBtn.addEventListener('click', () => this.startClearance());
        }
    }

    initializeIndustrieNavigation() {
        const industrieMenuButtons = document.querySelectorAll('.industrie-menu-btn');
        industrieMenuButtons.forEach(button => {
            button.addEventListener('click', (e) => {
                const industrieTab = e.target.getAttribute('data-industrie-tab');
                this.switchIndustrieTab(industrieTab);
            });
        });
    }

    switchIndustrieTab(industrieTabName) {
        // Mettre à jour les boutons du menu industrie
        document.querySelectorAll('.industrie-menu-btn').forEach(btn => {
            btn.classList.remove('active');
        });
        document.querySelector(`[data-industrie-tab="${industrieTabName}"]`)?.classList.add('active');

        // Mettre à jour les contenus industrie
        document.querySelectorAll('.industrie-content').forEach(content => {
            content.classList.remove('active');
        });
        document.getElementById(`industrie-${industrieTabName}`)?.classList.add('active');
    }

    startNewGame() {
        console.log('Démarrage d\'une nouvelle partie...');
        
        const gameState = this.gameManager.startNewGame();
        if (gameState) {
            this.fadeTransition(() => {
                this.switchToGameScreen();
                this.updateGameInterface(gameState);
                this.renderBuildings();
                // Mettre à jour la disponibilité des onglets
                this.updateTabsAvailability();
                // Affichage initial selon l'onglet actuel
                if (this.gameManager.currentTab === 'guilde') {
                    this.renderGuild();
                } else if (this.gameManager.currentTab === 'expedition') {
                    this.renderExpeditions();
                } else if (this.gameManager.currentTab === 'administration') {
                    this.renderAdministration();
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
                // Mettre à jour la disponibilité des onglets
                this.updateTabsAvailability();
                // Affichage initial selon l'onglet actuel
                if (this.gameManager.currentTab === 'guilde') {
                    this.renderGuild();
                } else if (this.gameManager.currentTab === 'expedition') {
                    this.renderExpeditions();
                } else if (this.gameManager.currentTab === 'administration') {
                    this.renderAdministration();
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
        // Arrêter le timer de jeu
        this.gameManager.stopGameTimer();
        
        const mainMenu = document.getElementById('main-menu');
        const gameScreen = document.getElementById('game-screen');
        
        if (mainMenu && gameScreen) {
            gameScreen.classList.remove('active');
            mainMenu.classList.add('active');
            this.currentScreen = 'menu';
        }
    }

    switchTab(tabName) {
        // Vérifier si l'onglet est désactivé
        const tabButton = document.querySelector(`[data-tab="${tabName}"]`);
        if (tabButton && tabButton.disabled) {
            let requiredAction = '';
            if (tabName === 'guilde' || tabName === 'expedition') {
                const isGuildUnlocked = this.gameManager.isUpgradeUnlocked('guild_unlock');
                const hasGuildBuilding = this.gameManager.hasGuildBuilding();
                
                if (!isGuildUnlocked) {
                    requiredAction = 'Débloquer la guilde des aventuriers dans l\'onglet Administration';
                } else if (!hasGuildBuilding) {
                    requiredAction = 'Construire le bâtiment Guilde des Aventuriers';
                }
            } else if (tabName === 'administration') {
                requiredAction = 'Construire une Mairie';
            } else if (tabName === 'commerce') {
                requiredAction = 'Construire un bâtiment commercial (Marché, Échoppe d\'artisan, ou Banque)';
            } else if (tabName === 'industrie') {
                requiredAction = 'Construire un bâtiment industriel (Forge, Alchimiste, ou Enchanteur)';
            }
            
            this.showActionResult({
                success: false,
                message: `Onglet verrouillé. ${requiredAction}`
            });
            return;
        }
        
        this.gameManager.switchTab(tabName);
        this.updateActiveTab(tabName);
        
        // Mettre à jour le contenu selon l'onglet
        if (tabName === 'batiments') {
            this.renderBuildings();
        } else if (tabName === 'guilde') {
            this.renderGuild();
        } else if (tabName === 'expedition') {
            this.renderExpeditions();
        } else if (tabName === 'administration') {
            this.renderAdministration();
        } else if (tabName === 'commerce') {
            this.renderCommerce();
        } else if (tabName === 'evenements') {
            this.renderEvents();
        } else if (tabName === 'succes') {
            this.renderAchievements();
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
        document.getElementById('game-clock').textContent = gameState.formattedTime;
        
        // Mettre à jour l'affichage des saisons
        if (gameState.season) {
            this.updateSeasonDisplay(gameState.season);
        }

        // Mettre à jour le bouton de pause
        const pauseBtn = document.getElementById('pause-game-btn');
        if (pauseBtn) {
            pauseBtn.textContent = gameState.isPaused ? '▶️ Reprendre' : '⏸️ Pause';
        }

        // Mettre à jour l'indicateur visuel de pause
        const gameScreen = document.getElementById('game-screen');
        if (gameScreen) {
            if (gameState.isPaused) {
                gameScreen.classList.add('paused');
            } else {
                gameScreen.classList.remove('paused');
            }
        }

        this.updateResources(gameState.resources);
        
        // Mettre à jour l'affichage selon l'onglet actuel pour les barres de progression
        if (this.gameManager.currentTab === 'batiments') {
            this.renderBuildings();
        } else if (this.gameManager.currentTab === 'administration') {
            this.renderAdministration();
        } else if (this.gameManager.currentTab === 'commerce') {
            this.renderCommerce();
        } else if (this.gameManager.currentTab === 'evenements') {
            this.renderEvents();
        }
        
        // Mettre à jour les actions du marché et des artisans si on est dans l'onglet commerce
        if (this.gameManager.currentTab === 'commerce') {
            const activeCommerceBtn = document.querySelector('.commerce-menu-btn.active');
            if (activeCommerceBtn) {
                const commerceTab = activeCommerceBtn.getAttribute('data-commerce-tab');
                if (commerceTab === 'marche') {
                    this.renderMarketActions();
                } else if (commerceTab === 'artisans') {
                    this.renderArtisanActions();
                }
            }
        }
        
        // Mettre à jour la bulle de notification des événements
        this.updateEventsNotification();
    }

    updateResources(resources) {
        if (!resources) return;

        document.getElementById('gold-amount').textContent = resources.gold;
        document.getElementById('population-amount').textContent = resources.population;
        document.getElementById('materials-amount').textContent = resources.materials;
        document.getElementById('magic-amount').textContent = resources.magic;
        document.getElementById('reputation-amount').textContent = resources.reputation;
        
        // Mettre à jour les gains horaires
        this.updateHourlyGains();
    }

    updateHourlyGains() {
        const hourlyGains = this.gameManager.getHourlyGains();
        
        this.updateHourlyGainDisplay('gold', hourlyGains.gold);
        this.updateHourlyGainDisplay('population', hourlyGains.population);
        this.updateHourlyGainDisplay('materials', hourlyGains.materials);
        this.updateHourlyGainDisplay('magic', hourlyGains.magic);
        this.updateHourlyGainDisplay('reputation', hourlyGains.reputation);
    }

    updateHourlyGainDisplay(resourceType, gain) {
        const element = document.getElementById(`${resourceType}-hourly-gain`);
        if (!element) return;
        
        if (gain > 0) {
            element.textContent = `+${gain}/heure`;
            element.className = 'hourly-gain';
        } else {
            element.textContent = `+0/heure`;
            element.className = 'hourly-gain zero';
        }
    }

    updateSeasonDisplay(seasonInfo) {
        // Mettre à jour l'icône de saison
        const seasonIcon = document.getElementById('season-icon');
        if (seasonIcon) {
            seasonIcon.textContent = seasonInfo.icon;
        }
        
        // Mettre à jour le texte de saison
        const seasonText = document.getElementById('season-text');
        if (seasonText) {
            seasonText.textContent = seasonInfo.formattedText;
        }
        
        // Mettre à jour la couleur de la bordure de saison
        const seasonInfoElement = document.querySelector('.season-info');
        if (seasonInfoElement && seasonInfo.color) {
            seasonInfoElement.style.setProperty('--season-color', seasonInfo.color);
            seasonInfoElement.style.borderLeftColor = seasonInfo.color;
        }
    }

    renderBuildings() {
        const buildingInfo = this.gameManager.getBuildingTypesInfo();
        if (!buildingInfo) return;

        this.updateBuildingStats(buildingInfo.stats);
        this.renderConstructedBuildings(this.gameManager.getBuildingsInfo());
        this.renderAvailableBuildingTypes(buildingInfo.available);
        this.renderLockedBuildingTypes(buildingInfo.locked);
        
        // Mettre à jour les gains horaires après changement de bâtiments
        this.updateHourlyGains();
    }

    updateBuildingStats(stats) {
        document.getElementById('buildings-total-count').textContent = stats.total;
        document.getElementById('buildings-avg-level').textContent = stats.averageLevel;
        document.getElementById('building-types-available').textContent = Object.keys(stats.districts).length;
        document.getElementById('building-types-locked').textContent = this.gameManager.getBuildingTypesInfo().locked.length;
        document.getElementById('constructed-count').textContent = `${stats.total} bâtiment${stats.total > 1 ? 's' : ''}`;
    }

    renderConstructedBuildings(buildings) {
        const container = document.getElementById('constructed-buildings');
        if (!container) return;

        container.innerHTML = '';

        if (buildings.length === 0) {
            container.innerHTML = `
                <div style="text-align: center; padding: 40px; color: #999;">
                    <p>Aucun bâtiment construit</p>
                    <p style="font-size: 0.9rem;">Sélectionnez un type de bâtiment à droite pour commencer</p>
                </div>
            `;
            return;
        }

        buildings.forEach(building => {
            const card = this.createConstructedBuildingCard(building);
            container.appendChild(card);
        });
    }

    renderAvailableBuildingTypes(buildingTypes) {
        const container = document.getElementById('available-building-types');
        if (!container) return;

        container.innerHTML = '';

        if (buildingTypes.length === 0) {
            container.innerHTML = `
                <div style="text-align: center; padding: 40px; color: #999;">
                    <p>Aucun type de bâtiment disponible</p>
                    <p style="font-size: 0.9rem;">Débloquez des améliorations dans l'onglet Administration</p>
                </div>
            `;
            return;
        }

        buildingTypes.forEach(buildingType => {
            const card = this.createBuildingTypeCard(buildingType, false);
            container.appendChild(card);
        });
    }

    renderLockedBuildingTypes(buildingTypes) {
        const container = document.getElementById('locked-building-types');
        if (!container) return;

        container.innerHTML = '';
        document.getElementById('locked-types-count').textContent = `${buildingTypes.length} type${buildingTypes.length > 1 ? 's' : ''} verrouillé${buildingTypes.length > 1 ? 's' : ''}`;

        if (buildingTypes.length === 0) {
            container.innerHTML = `
                <div style="text-align: center; padding: 20px; color: #666;">
                    <p>Tous les types de bâtiments débloqués !</p>
                </div>
            `;
            return;
        }

        buildingTypes.forEach(buildingType => {
            const card = this.createBuildingTypeCard(buildingType, true);
            container.appendChild(card);
        });
    }

    createConstructedBuildingCard(building) {
        const card = document.createElement('div');
        card.className = `building-card ${building.isUnderConstruction ? 'under-construction' : ''} ${building.isUpgrading ? 'upgrading' : ''}`;
        
        const effects = this.formatEffects(building.effects);
        const cost = this.formatCost(building.upgradeCost);
        
        // Affichage du statut de construction/amélioration
        let statusHtml = '';
        if (building.isUnderConstruction) {
            statusHtml = `
                <div class="construction-status">
                    <div class="status-header">🏗️ En construction</div>
                    <div class="progress-info">
                        <div class="progress-bar">
                            <div class="progress-fill" style="width: ${building.constructionProgress}%"></div>
                        </div>
                        <span class="progress-text">${building.constructionProgress}% - ${building.remainingConstructionTime} restant</span>
                    </div>
                </div>
            `;
        } else if (building.isUpgrading) {
            statusHtml = `
                <div class="upgrade-status">
                    <div class="status-header">⬆️ Amélioration vers niveau ${building.upgradeTargetLevel}</div>
                    <div class="progress-info">
                        <div class="progress-bar">
                            <div class="progress-fill" style="width: ${building.upgradeProgress}%"></div>
                        </div>
                        <span class="progress-text">${building.upgradeProgress}% - ${building.remainingUpgradeTime} restant</span>
                    </div>
                </div>
            `;
        }
        
        card.innerHTML = `
            <div class="building-header">
                <div class="building-icon">${building.icon}</div>
                <div class="building-info">
                    <h4>${building.customName}</h4>
                    <div class="building-type">${building.typeName}</div>
                    ${building.unlocksTab ? `<div class="unlocks-tab">🎯 Débloque: ${building.unlocksTab}</div>` : ''}
                </div>
                <span class="building-level">Niv. ${building.level}/${building.maxLevel}</span>
            </div>
            <div class="building-district">District: ${building.district}</div>
            
            ${statusHtml}
            
            ${building.built ? `
                <div class="building-effects">
                    <h4>Effets actuels:</h4>
                    <div class="effects-list">
                        ${effects}
                    </div>
                </div>
            ` : ''}
            
            ${building.level < building.maxLevel && building.built && !building.isUpgrading ? `
                <div class="building-cost">
                    <strong>Coût amélioration:</strong>
                    <div class="cost-list">
                        ${cost}
                    </div>
                </div>
            ` : ''}
            
            <div class="building-actions">
                ${this.createConstructedBuildingActions(building)}
            </div>
        `;
        
        return card;
    }

    createBuildingTypeCard(buildingType, isLocked) {
        const card = document.createElement('div');
        card.className = `building-type-card ${isLocked ? 'locked' : ''}`;
        
        if (!isLocked) {
            card.onclick = () => this.openBuildingConstructionModal(buildingType.id);
        }
        
        // Récupérer l'instance complète du BuildingType pour accéder aux méthodes
        const fullBuildingType = this.gameManager.buildingManager.getBuildingTypeById(buildingType.id);
        const effectsAtLevel1 = this.formatEffects(fullBuildingType.getEffectsAtLevel(1));
        const baseCost = this.formatCost(buildingType.baseCost);
        const statusClass = isLocked ? 'locked' : 'available';
        const statusText = isLocked ? 'Verrouillé' : 'Disponible';
        
        
        card.innerHTML = `
            <div class="building-type-header">
                <div class="building-icon">${buildingType.icon}</div>
                <div class="building-type-info">
                    <h5>${buildingType.name}</h5>
                    <div class="unlock-status ${statusClass}">${statusText}</div>
                    ${buildingType.unlocksTab ? `<div class="unlocks-tab">🎯 Débloque: ${buildingType.unlocksTab}</div>` : ''}
                </div>
            </div>
            
            <div class="building-type-description">
                ${buildingType.description}
            </div>
            
            <div class="building-effects">
                <h4>Effets (Niv. 1):</h4>
                <div class="effects-list">
                    ${effectsAtLevel1}
                </div>
            </div>
            
            <div class="building-cost">
                <strong>Coût de base:</strong>
                <div class="cost-list">
                    ${baseCost}
                </div>
            </div>
            
            <div class="construction-time">
                <strong>Temps de construction:</strong>
                <span class="time-value">${buildingType.baseConstructionTime}h</span>
            </div>
            
            ${isLocked && buildingType.unlockRequirement ? `
                <div style="margin-top: 10px; padding: 8px; background: rgba(139, 90, 60, 0.2); border-radius: 4px; border: 1px solid #8b5a3c;">
                    <small style="color: #8b5a3c;">Requis: Amélioration correspondante</small>
                </div>
            ` : ''}
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
                    'goldPerHour': 'Or/heure',
                    'materialsPerHour': 'Matériaux/heure',
                    'magicPerHour': 'Magie/heure',
                    'reputationPerHour': 'Réputation/heure',
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

    createConstructedBuildingActions(building) {
        const resources = this.gameManager.getResourcesInfo();
        const canAfford = resources && this.canAffordCost(resources, building.upgradeCost);
        const isPaused = this.gameManager.city && this.gameManager.city.isPaused;
        
        const actions = [];
        
        // Si le bâtiment est en construction
        if (building.isUnderConstruction) {
            actions.push(`<button class="building-btn" disabled>En construction</button>`);
        }
        // Si le bâtiment est en amélioration
        else if (building.isUpgrading) {
            actions.push(`<button class="building-btn" disabled>Amélioration en cours</button>`);
        }
        // Si le bâtiment est construit et peut être amélioré
        else if (building.built && building.level < building.maxLevel) {
            actions.push(`
                <button class="building-btn primary" 
                        onclick="app.upgradeBuilding('${building.id}')"
                        ${!canAfford || isPaused ? 'disabled' : ''}>
                    ${isPaused ? 'Jeu en pause' : 'Améliorer'}
                </button>
            `);
        }
        // Si le bâtiment est au niveau maximum
        else if (building.built) {
            actions.push(`<button class="building-btn" disabled>Niveau Max</button>`);
        }
        
        // Bouton de démolition (sauf pour certains bâtiments protégés et en construction/amélioration)
        if (building.typeId !== 'mairie' && !building.isUnderConstruction && !building.isUpgrading) {
            actions.push(`
                <button class="building-btn danger" 
                        onclick="app.demolishBuilding('${building.id}')"
                        ${isPaused ? 'disabled' : ''}>
                    ${isPaused ? 'Jeu en pause' : 'Détruire'}
                </button>
            `);
        }
        
        return actions.join('');
    }

    canAffordCost(resources, cost) {
        return Object.entries(cost).every(([resource, amount]) => {
            return resources[resource] >= amount;
        });
    }

    // Nouvelles méthodes pour le système de bâtiments
    openBuildingConstructionModal(buildingTypeId) {
        // Vérifier si le jeu est en pause
        if (this.gameManager.city && this.gameManager.city.isPaused) {
            this.showActionResult({ success: false, message: 'Impossible de construire : jeu en pause' });
            return;
        }
        
        this.currentBuildingTypeId = buildingTypeId;
        const buildingTypesInfo = this.gameManager.getBuildingTypesInfo();
        const buildingType = buildingTypesInfo.all.find(type => type.id === buildingTypeId);
        
        if (!buildingType) {
            this.showActionResult({ success: false, message: 'Type de bâtiment introuvable' });
            return;
        }

        // Remplir la modal avec les informations du type de bâtiment
        document.getElementById('modal-building-type-name').textContent = `Construire: ${buildingType.name}`;
        document.getElementById('modal-building-icon').textContent = buildingType.icon;
        document.getElementById('modal-building-name').textContent = buildingType.name;
        document.getElementById('modal-building-description').textContent = buildingType.description;
        
        // Afficher le coût
        const costHtml = this.formatCost(buildingType.baseCost);
        document.getElementById('modal-construction-cost').innerHTML = costHtml;
        
        // Récupérer l'instance complète du BuildingType pour accéder aux méthodes
        const fullBuildingType = this.gameManager.buildingManager.getBuildingTypeById(buildingType.id);
        // Afficher les effets réels au niveau 1
        const effectsAtLevel1 = fullBuildingType.getEffectsAtLevel(1);
        const effectsHtml = this.formatEffects(effectsAtLevel1);
        document.getElementById('modal-building-effects').innerHTML = effectsHtml;
        
        // Réinitialiser le formulaire
        document.getElementById('building-custom-name').value = '';
        
        // Afficher la modal
        document.getElementById('building-construction-modal').classList.add('active');
    }

    closeBuildingModal() {
        document.getElementById('building-construction-modal').classList.remove('active');
        this.currentBuildingTypeId = null;
    }

    confirmBuildingConstruction() {
        if (!this.currentBuildingTypeId) return;
        
        const customName = document.getElementById('building-custom-name').value.trim();
        
        const result = this.gameManager.constructBuilding(this.currentBuildingTypeId, customName);
        this.showActionResult(result);
        
        if (result.success) {
            this.closeBuildingModal();
            this.renderBuildings();
            // Mettre à jour la disponibilité des onglets (en cas de construction d'une mairie)
            this.updateTabsAvailability();
        }
    }

    upgradeBuilding(buildingId) {
        if (this.gameManager.city && this.gameManager.city.isPaused) {
            this.showActionResult({ success: false, message: 'Impossible d\'améliorer : jeu en pause' });
            return;
        }
        
        const result = this.gameManager.upgradeBuilding(buildingId);
        this.showActionResult(result);
        
        if (result.success) {
            this.renderBuildings();
        }
    }

    demolishBuilding(buildingId) {
        if (this.gameManager.city && this.gameManager.city.isPaused) {
            this.showActionResult({ success: false, message: 'Impossible de détruire : jeu en pause' });
            return;
        }
        
        // Demander confirmation
        const building = this.gameManager.getBuildingsInfo().find(b => b.id === buildingId);
        if (!building) return;
        
        if (!confirm(`Êtes-vous sûr de vouloir détruire "${building.customName}" ?\n\nVous récupérerez 30% des ressources investîs.`)) {
            return;
        }
        
        const result = this.gameManager.demolishBuilding(buildingId);
        this.showActionResult(result);
        
        if (result.success) {
            this.renderBuildings();
        }
    }

    togglePause() {
        const isPaused = this.gameManager.toggleGamePause();
        // L'interface se met automatiquement à jour via le callback updateGameInterface
        
        // Actualiser l'affichage des boutons selon le nouvel état
        if (this.gameManager.currentTab === 'batiments') {
            this.renderBuildings();
        } else if (this.gameManager.currentTab === 'guilde') {
            this.renderGuild();
        } else if (this.gameManager.currentTab === 'expedition') {
            this.renderExpeditions();
        } else if (this.gameManager.currentTab === 'administration') {
            this.renderAdministration();
        } else if (this.gameManager.currentTab === 'commerce') {
            this.renderCommerce();
        }
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
            background: ${result.success ? 'rgba(74, 124, 89, 0.95)' : 'rgba(139, 90, 60, 0.95)'};
            color: #e8e6e3;
            padding: 15px 20px;
            border-radius: 8px;
            border: 2px solid ${result.success ? '#4a7c59' : '#8b5a3c'};
            z-index: 2500;
            font-size: 0.9rem;
            max-width: 300px;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.4);
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
        const isPaused = this.gameManager.city && this.gameManager.city.isPaused;
        
        if (searchBtn && searchInfoElement) {
            searchBtn.disabled = !searchInfo.canSearch || isPaused;
            
            if (isPaused) {
                searchInfoElement.textContent = 'Jeu en pause';
            } else if (searchInfo.canSearch) {
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
        const isPaused = this.gameManager.city && this.gameManager.city.isPaused;
        
        if (isRecruit) {
            const resources = this.gameManager.getResourcesInfo();
            const canAfford = resources && this.canAffordCost(resources, adventurer.recruitmentCost);

            return `
                <button class="adventurer-btn recruit" 
                        onclick="app.recruitAdventurer('${adventurer.id}')"
                        ${!canAfford || isPaused ? 'disabled' : ''}>
                    ${isPaused ? 'Jeu en pause' : 'Recruter'}
                </button>
            `;
        } else {
            if (adventurer.isOnMission) {
                return `<button class="adventurer-btn" disabled>En Mission</button>`;
            } else {
                return `
                    <button class="adventurer-btn dismiss" 
                            onclick="app.dismissAdventurer('${adventurer.id}')"
                            ${isPaused ? 'disabled' : ''}>
                        ${isPaused ? 'Jeu en pause' : 'Renvoyer'}
                    </button>
                `;
            }
        }
    }

    // Actions pour l'onglet Guilde
    searchForAdventurers() {
        if (this.gameManager.city && this.gameManager.city.isPaused) {
            this.showActionResult({ success: false, message: 'Impossible de rechercher : jeu en pause' });
            return;
        }
        
        const result = this.gameManager.searchForAdventurers();
        this.showActionResult(result);
        
        if (result.success) {
            this.renderGuild();
        }
    }

    recruitAdventurer(adventurerId) {
        if (this.gameManager.city && this.gameManager.city.isPaused) {
            this.showActionResult({ success: false, message: 'Impossible de recruter : jeu en pause' });
            return;
        }
        
        const result = this.gameManager.recruitAdventurer(adventurerId);
        this.showActionResult(result);
        
        if (result.success) {
            this.renderGuild();
        }
    }

    dismissAdventurer(adventurerId) {
        if (this.gameManager.city && this.gameManager.city.isPaused) {
            this.showActionResult({ success: false, message: 'Impossible de renvoyer : jeu en pause' });
            return;
        }
        
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
        const isPaused = this.gameManager.city && this.gameManager.city.isPaused;
        
        if (refreshBtn && refreshInfoElement) {
            refreshBtn.disabled = !refreshInfo.canRefresh || isPaused;
            
            if (isPaused) {
                refreshInfoElement.textContent = 'Jeu en pause';
            } else if (refreshInfo.canRefresh) {
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
            const isPaused = this.gameManager.city && this.gameManager.city.isPaused;
            actionsHtml = `
                <div class="mission-actions">
                    <button class="mission-btn start" 
                            onclick="app.openAdventurerSelection('${mission.id}')"
                            ${isPaused ? 'disabled' : ''}>
                        ${isPaused ? 'Jeu en pause' : 'Lancer Mission'}
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
        if (this.gameManager.city && this.gameManager.city.isPaused) {
            this.showActionResult({ success: false, message: 'Impossible d\'actualiser : jeu en pause' });
            return;
        }
        
        const result = this.gameManager.refreshMissions();
        this.showActionResult(result);
        
        if (result.success) {
            this.renderExpeditions();
        }
    }

    openAdventurerSelection(missionId) {
        // Vérifier si le jeu est en pause
        if (this.gameManager.city && this.gameManager.city.isPaused) {
            this.showActionResult({ success: false, message: 'Impossible de lancer une mission : jeu en pause' });
            return;
        }
        
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

    // === MÉTHODES POUR L'ONGLET ADMINISTRATION ===

    renderAdministration() {
        // Par défaut, afficher l'onglet des améliorations
        this.renderCityUpgrades();
    }

    renderCityUpgrades() {
        const upgradeInfo = this.gameManager.getUpgradeInfo();
        if (!upgradeInfo) return;

        this.renderUpgradesList(upgradeInfo.all);
    }

    renderUpgradesList(upgrades) {
        const container = document.getElementById('city-upgrades-list');
        if (!container) return;

        container.innerHTML = '';

        if (upgrades.length === 0) {
            container.innerHTML = `
                <div style="text-align: center; padding: 40px; color: #999;">
                    <p>Aucune amélioration disponible</p>
                </div>
            `;
            return;
        }

        upgrades.forEach(upgrade => {
            const card = this.createUpgradeCard(upgrade);
            container.appendChild(card);
        });
    }

    createUpgradeCard(upgrade) {
        const card = document.createElement('div');
        card.className = `upgrade-card ${upgrade.unlocked ? 'unlocked' : ''} ${upgrade.isUnderDevelopment ? 'under-development' : ''}`;

        const costHtml = this.formatCost(upgrade.cost);
        let statusClass, statusText;
        
        if (upgrade.unlocked) {
            statusClass = 'unlocked';
            statusText = 'Débloqué';
        } else if (upgrade.isUnderDevelopment) {
            statusClass = 'developing';
            statusText = 'En recherche';
        } else {
            statusClass = 'available';
            statusText = 'Disponible';
        }

        // Affichage du statut de recherche
        let statusHtml = '';
        if (upgrade.isUnderDevelopment) {
            statusHtml = `
                <div class="development-status">
                    <div class="status-header">🔬 Recherche en cours</div>
                    <div class="progress-info">
                        <div class="progress-bar">
                            <div class="progress-fill" style="width: ${upgrade.developmentProgress}%"></div>
                        </div>
                        <span class="progress-text">${upgrade.developmentProgress}% - ${upgrade.remainingTime} restant</span>
                    </div>
                </div>
            `;
        }

        card.innerHTML = `
            <div class="upgrade-header">
                <div class="upgrade-icon">${upgrade.icon}</div>
                <div class="upgrade-info">
                    <h5>${upgrade.name}</h5>
                    <div class="upgrade-status ${statusClass}">${statusText}</div>
                </div>
            </div>
            
            <div class="upgrade-description">
                ${upgrade.description}
            </div>
            
            ${statusHtml}
            
            <div class="upgrade-cost">
                <div class="upgrade-cost-label">Coût:</div>
                <div class="upgrade-cost-items">
                    ${costHtml}
                </div>
            </div>
            
            <div class="upgrade-time">
                <strong>Temps de recherche:</strong>
                <span class="time-value">${upgrade.constructionTime}h</span>
            </div>
            
            <div class="upgrade-actions">
                ${this.createUpgradeActions(upgrade)}
            </div>
        `;

        return card;
    }

    createUpgradeActions(upgrade) {
        if (upgrade.unlocked) {
            return `<button class="upgrade-btn unlocked" disabled>Débloqué</button>`;
        }

        if (upgrade.isUnderDevelopment) {
            return `<button class="upgrade-btn developing" disabled>Recherche en cours</button>`;
        }

        const resources = this.gameManager.getResourcesInfo();
        const canAfford = resources && this.canAffordCost(resources, upgrade.cost);
        const isPaused = this.gameManager.city && this.gameManager.city.isPaused;

        return `
            <button class="upgrade-btn" 
                    onclick="app.unlockUpgrade('${upgrade.id}')"
                    ${!canAfford || isPaused ? 'disabled' : ''}>
                ${isPaused ? 'Jeu en pause' : 'Rechercher'}
            </button>
        `;
    }

    unlockUpgrade(upgradeId) {
        if (this.gameManager.city && this.gameManager.city.isPaused) {
            this.showActionResult({ success: false, message: 'Impossible de débloquer : jeu en pause' });
            return;
        }
        
        const result = this.gameManager.unlockUpgrade(upgradeId);
        this.showActionResult(result);
        
        if (result.success) {
            this.renderCityUpgrades();
            // Mettre à jour les onglets qui pourraient être débloqués
            this.updateTabsAvailability();
        }
    }

    updateTabsAvailability() {
        const guildTab = document.querySelector('[data-tab="guilde"]');
        const expeditionTab = document.querySelector('[data-tab="expedition"]');
        const administrationTab = document.querySelector('[data-tab="administration"]');
        const commerceTab = document.querySelector('[data-tab="commerce"]');
        const industrieTab = document.querySelector('[data-tab="industrie"]');

        // Vérifier si la guilde est débloquée ET construite
        const isGuildUnlocked = this.gameManager.isUpgradeUnlocked('guild_unlock');
        const hasGuildBuilding = this.gameManager.hasGuildBuilding();
        
        // Vérifier si une mairie est construite
        const hasCityHall = this.gameManager.hasCityHall();
        
        // Vérifier les bâtiments commerciaux
        const commercialBuildings = this.gameManager.hasCommercialBuildings();
        
        // Vérifier les bâtiments industriels
        const industrialBuildings = this.gameManager.hasIndustrialBuildings();
        
        if (guildTab) {
            if (isGuildUnlocked && hasGuildBuilding) {
                guildTab.disabled = false;
                guildTab.style.opacity = '1';
                guildTab.style.cursor = 'pointer';
            } else {
                guildTab.disabled = true;
                guildTab.style.opacity = '0.5';
                guildTab.style.cursor = 'not-allowed';
            }
        }

        if (expeditionTab) {
            if (isGuildUnlocked && hasGuildBuilding) {
                expeditionTab.disabled = false;
                expeditionTab.style.opacity = '1';
                expeditionTab.style.cursor = 'pointer';
            } else {
                expeditionTab.disabled = true;
                expeditionTab.style.opacity = '0.5';
                expeditionTab.style.cursor = 'not-allowed';
            }
        }

        if (administrationTab) {
            if (hasCityHall) {
                administrationTab.disabled = false;
                administrationTab.style.opacity = '1';
                administrationTab.style.cursor = 'pointer';
            } else {
                administrationTab.disabled = true;
                administrationTab.style.opacity = '0.5';
                administrationTab.style.cursor = 'not-allowed';
            }
        }

        if (commerceTab) {
            if (commercialBuildings.hasAny) {
                commerceTab.disabled = false;
                commerceTab.style.opacity = '1';
                commerceTab.style.cursor = 'pointer';
            } else {
                commerceTab.disabled = true;
                commerceTab.style.opacity = '0.5';
                commerceTab.style.cursor = 'not-allowed';
            }
        }

        if (industrieTab) {
            if (industrialBuildings.hasAny) {
                industrieTab.disabled = false;
                industrieTab.style.opacity = '1';
                industrieTab.style.cursor = 'pointer';
            } else {
                industrieTab.disabled = true;
                industrieTab.style.opacity = '0.5';
                industrieTab.style.cursor = 'not-allowed';
            }
        }

        // Mettre à jour les sous-menus
        this.updateCommerceSubTabs(commercialBuildings);
        this.updateIndustrieSubTabs(industrialBuildings);
    }

    updateCommerceSubTabs(commercialBuildings) {
        const marcheBtn = document.querySelector('[data-commerce-tab="marche"]');
        const artisansBtn = document.querySelector('[data-commerce-tab="artisans"]');
        const banqueBtn = document.querySelector('[data-commerce-tab="banque"]');

        if (marcheBtn) {
            marcheBtn.disabled = false; // Toujours cliquable
            marcheBtn.textContent = '🏪 Marché';
            marcheBtn.classList.remove('locked');
        }

        if (artisansBtn) {
            artisansBtn.disabled = false; // Toujours cliquable
            artisansBtn.textContent = '🔨 Artisans';
            artisansBtn.classList.remove('locked');
        }

        if (banqueBtn) {
            banqueBtn.disabled = false; // Toujours cliquable
            banqueBtn.textContent = '🏦 Banque';
            banqueBtn.classList.remove('locked');
        }
    }

    updateIndustrieSubTabs(industrialBuildings) {
        const forgeBtn = document.querySelector('[data-industrie-tab="forge"]');
        const alchimistesBtn = document.querySelector('[data-industrie-tab="alchimistes"]');
        const enchanteursBtn = document.querySelector('[data-industrie-tab="enchanteurs"]');

        if (forgeBtn) {
            forgeBtn.disabled = false; // Toujours cliquable
            forgeBtn.textContent = '⚒️ Forge';
            forgeBtn.classList.remove('locked');
        }

        if (alchimistesBtn) {
            alchimistesBtn.disabled = false; // Toujours cliquable
            alchimistesBtn.textContent = '🧪 Alchimistes';
            alchimistesBtn.classList.remove('locked');
        }

        if (enchanteursBtn) {
            enchanteursBtn.disabled = false; // Toujours cliquable
            enchanteursBtn.textContent = '✨ Enchanteurs';
            enchanteursBtn.classList.remove('locked');
        }
    }

    // === MÉTHODES POUR L'ONGLET COMMERCE ===

    renderCommerce() {
        // Mettre à jour l'affichage selon le sous-onglet actuel
        const activeCommerceBtn = document.querySelector('.commerce-menu-btn.active');
        if (activeCommerceBtn) {
            const commerceTab = activeCommerceBtn.getAttribute('data-commerce-tab');
            if (commerceTab === 'marche') {
                this.renderMarketActions();
            } else if (commerceTab === 'artisans') {
                this.renderArtisanActions();
            }
        }
    }

    renderMarketActions() {
        const marketInfo = this.gameManager.getMarketInfo();
        if (!marketInfo) return;

        // Gérer l'affichage selon si un marché est construit ou non
        const lockedMessage = document.getElementById('marche-locked-message');
        const actionsSection = document.getElementById('marche-actions-section');

        if (marketInfo.hasMarket) {
            if (lockedMessage) lockedMessage.style.display = 'none';
            if (actionsSection) actionsSection.style.display = 'block';
            
            // Mettre à jour l'état des actions
            this.updateMarketActionStatus('negotiator', marketInfo.negotiatorStatus);
            this.updateMarketActionStatus('emissary', marketInfo.emissaryStatus);
        } else {
            if (lockedMessage) lockedMessage.style.display = 'block';
            if (actionsSection) actionsSection.style.display = 'none';
        }
    }

    updateMarketActionStatus(actionType, status) {
        const btnId = actionType === 'negotiator' ? 'send-negotiator-btn' : 'send-emissary-btn';
        const statusId = actionType === 'negotiator' ? 'negotiator-status' : 'emissary-status';
        
        const button = document.getElementById(btnId);
        const statusContainer = document.getElementById(statusId);
        
        if (!button || !statusContainer) return;

        const isPaused = this.gameManager.city && this.gameManager.city.isPaused;
        
        // Vérifier si l'autre action est en cours
        const marketInfo = this.gameManager.getMarketInfo();
        const otherActionType = actionType === 'negotiator' ? 'emissary' : 'negotiator';
        const otherStatus = actionType === 'negotiator' ? marketInfo.emissaryStatus : marketInfo.negotiatorStatus;
        const isOtherActionActive = otherStatus && otherStatus.isActive;

        if (status && status.isActive) {
            // Action en cours
            button.disabled = true;
            button.textContent = 'En cours';
            
            // Afficher le temps restant
            let remainingText = '';
            if (status.remainingHours > 0) {
                if (status.remainingMins > 0) {
                    remainingText = `${status.remainingHours}h ${status.remainingMins}m restant`;
                } else {
                    remainingText = `${status.remainingHours}h restant`;
                }
            } else if (status.remainingMins > 0) {
                remainingText = `${status.remainingMins}m restant`;
            } else {
                remainingText = 'Se termine bientôt';
            }
                
            statusContainer.innerHTML = `
                <div class="action-progress-text">Action en cours</div>
                <div class="action-time-remaining">${remainingText}</div>
            `;
            statusContainer.classList.add('action-in-progress');
        } else {
            // Action disponible ou bloquée
            let buttonText = '';
            let disabled = isPaused || isOtherActionActive;
            
            if (isPaused) {
                buttonText = 'Jeu en pause';
            } else if (isOtherActionActive) {
                buttonText = 'Autre action en cours';
            } else {
                buttonText = actionType === 'negotiator' ? 'Envoyer négociateur' : 'Envoyer émissaire';
            }
            
            button.disabled = disabled;
            button.textContent = buttonText;
            
            statusContainer.innerHTML = `
                <button id="${btnId}" class="action-btn primary" ${disabled ? 'disabled' : ''}>
                    ${buttonText}
                </button>
            `;
            statusContainer.classList.remove('action-in-progress');
            
            // Re-attacher l'événement click
            const newButton = document.getElementById(btnId);
            if (newButton) {
                newButton.addEventListener('click', () => {
                    if (actionType === 'negotiator') {
                        this.sendNegotiator();
                    } else {
                        this.sendEmissary();
                    }
                });
            }
        }
    }

    renderArtisanActions() {
        const artisanInfo = this.gameManager.getArtisanInfo();
        if (!artisanInfo) return;

        // Gérer l'affichage selon si une échoppe d'artisan est construite ou non
        const lockedMessage = document.getElementById('artisans-locked-message');
        const actionsSection = document.getElementById('artisans-actions-section');

        if (artisanInfo.hasArtisan) {
            if (lockedMessage) lockedMessage.style.display = 'none';
            if (actionsSection) actionsSection.style.display = 'block';
            
            // Mettre à jour l'état des actions
            this.updateArtisanActionStatus('nightWork', artisanInfo.nightWorkStatus);
            this.updateArtisanActionStatus('clearance', artisanInfo.clearanceStatus);
        } else {
            if (lockedMessage) lockedMessage.style.display = 'block';
            if (actionsSection) actionsSection.style.display = 'none';
        }
    }

    updateArtisanActionStatus(actionType, status) {
        const btnId = actionType === 'nightWork' ? 'start-night-work-btn' : 'start-clearance-btn';
        const statusId = actionType === 'nightWork' ? 'night-work-status' : 'clearance-status';
        
        const button = document.getElementById(btnId);
        const statusContainer = document.getElementById(statusId);
        
        if (!button || !statusContainer) return;

        const isPaused = this.gameManager.city && this.gameManager.city.isPaused;
        
        // Vérifier si l'autre action est en cours (exclusivité)
        const artisanInfo = this.gameManager.getArtisanInfo();
        const otherActionType = actionType === 'nightWork' ? 'clearance' : 'nightWork';
        const otherStatus = actionType === 'nightWork' ? artisanInfo.clearanceStatus : artisanInfo.nightWorkStatus;
        const isOtherActionActive = otherStatus && otherStatus.isActive;

        if (status && status.isActive) {
            // Action en cours
            button.disabled = true;
            button.textContent = 'En cours';
            
            // Afficher le temps restant
            let remainingText = '';
            if (status.remainingDays > 0) {
                if (status.remainingHours > 0) {
                    remainingText = `${status.remainingDays}j ${status.remainingHours}h restant`;
                } else {
                    remainingText = `${status.remainingDays}j restant`;
                }
            } else if (status.remainingHours > 0) {
                remainingText = `${status.remainingHours}h restant`;
            } else {
                remainingText = 'Se termine bientôt';
            }
            
            // Afficher l'état de l'effet
            let effectText = '';
            if (status.isEffectActive) {
                effectText = actionType === 'nightWork' ? 
                    '✨ Effet actif: matériaux x2' : 
                    '✨ Effet actif: or x2';
            }
            
            statusContainer.innerHTML = `
                <div class="action-progress-text">Action en cours</div>
                <div class="action-time-remaining">${remainingText}</div>
                ${effectText ? `<div class="action-effect-active" style="color: #4CAF50; font-size: 12px; margin-top: 5px;">${effectText}</div>` : ''}
            `;
            statusContainer.classList.add('action-in-progress');
        } else {
            // Action disponible ou bloquée
            let buttonText = '';
            let disabled = isPaused || isOtherActionActive;
            
            if (isPaused) {
                buttonText = 'Jeu en pause';
            } else if (isOtherActionActive) {
                buttonText = 'Autre action en cours';
            } else {
                buttonText = actionType === 'nightWork' ? 
                    'Lancer le travail de nuit' : 
                    'Lancer les soldes';
            }
            
            button.disabled = disabled;
            button.textContent = buttonText;
            
            statusContainer.innerHTML = `
                <button id="${btnId}" class="action-btn primary" ${disabled ? 'disabled' : ''}>
                    ${buttonText}
                </button>
            `;
            statusContainer.classList.remove('action-in-progress');
            
            // Re-attacher l'événement click
            const newButton = document.getElementById(btnId);
            if (newButton) {
                newButton.addEventListener('click', () => {
                    if (actionType === 'nightWork') {
                        this.startNightWork();
                    } else {
                        this.startClearance();
                    }
                });
            }
        }
    }

    sendNegotiator() {
        if (this.gameManager.city && this.gameManager.city.isPaused) {
            this.showActionResult({ success: false, message: 'Impossible d\'envoyer un négociateur : jeu en pause' });
            return;
        }
        
        const result = this.gameManager.startMarketAction('negotiator');
        this.showActionResult(result);
        
        if (result.success) {
            this.renderMarketActions();
        }
    }

    sendEmissary() {
        if (this.gameManager.city && this.gameManager.city.isPaused) {
            this.showActionResult({ success: false, message: 'Impossible d\'envoyer un émissaire : jeu en pause' });
            return;
        }
        
        const result = this.gameManager.startMarketAction('emissary');
        this.showActionResult(result);
        
        if (result.success) {
            this.renderMarketActions();
        }
    }

    startNightWork() {
        if (this.gameManager.city && this.gameManager.city.isPaused) {
            this.showActionResult({ success: false, message: 'Impossible de lancer le travail de nuit : jeu en pause' });
            return;
        }
        
        const result = this.gameManager.startArtisanAction('nightWork');
        this.showActionResult(result);
        
        if (result.success) {
            this.renderArtisanActions();
        }
    }

    startClearance() {
        if (this.gameManager.city && this.gameManager.city.isPaused) {
            this.showActionResult({ success: false, message: 'Impossible de lancer les soldes : jeu en pause' });
            return;
        }
        
        const result = this.gameManager.startArtisanAction('clearance');
        this.showActionResult(result);
        
        if (result.success) {
            this.renderArtisanActions();
        }
    }

    // === MÉTHODES POUR L'ONGLET SUCCÈS ===

    renderAchievements() {
        const achievementInfo = this.gameManager.getAchievementInfo();
        if (!achievementInfo) return;

        this.updateAchievementStats(achievementInfo.stats);
        this.renderAchievementsList(achievementInfo.achievements);
    }

    updateAchievementStats(stats) {
        document.getElementById('achievements-unlocked-count').textContent = stats.unlocked;
        document.getElementById('achievements-total-count').textContent = stats.total;
        document.getElementById('achievements-progress').textContent = `${stats.progress}%`;
        document.getElementById('achievements-last-unlock').textContent = stats.lastUnlock;
        document.getElementById('achievements-section-count').textContent = `${stats.total} succès`;
    }

    renderAchievementsList(achievements) {
        const container = document.getElementById('achievements-list');
        if (!container) return;

        container.innerHTML = '';

        if (achievements.length === 0) {
            container.innerHTML = `
                <div style="text-align: center; padding: 40px; color: #999;">
                    <p>🏆 Aucun succès disponible pour le moment</p>
                    <p style="font-size: 0.9rem;">Les succès apparaîtront au fur et à mesure de votre progression</p>
                </div>
            `;
            return;
        }

        achievements.forEach(achievement => {
            const card = this.createAchievementCard(achievement);
            container.appendChild(card);
        });
    }

    createAchievementCard(achievement) {
        const card = document.createElement('div');
        card.className = `achievement-card ${achievement.unlocked ? 'unlocked' : 'locked'}`;
        card.setAttribute('data-category', achievement.category);

        const rewardsHtml = this.createAchievementRewardsHtml(achievement.rewards);
        const progressHtml = achievement.maxProgress > 1 ? this.createAchievementProgressHtml(achievement) : '';

        card.innerHTML = `
            <div class="achievement-header">
                <div class="achievement-icon">${achievement.icon}</div>
                <div class="achievement-info">
                    <h4 class="achievement-name">${achievement.isSecret && !achievement.unlocked ? '???' : achievement.name}</h4>
                    <div class="achievement-description">
                        ${achievement.isSecret && !achievement.unlocked ? 'Succès secret non découvert' : achievement.description}
                    </div>
                    <div class="achievement-requirement">
                        ${achievement.isSecret && !achievement.unlocked ? '???' : achievement.requirement}
                    </div>
                </div>
                <div class="achievement-status ${achievement.unlocked ? 'unlocked' : 'locked'}">
                    ${achievement.unlocked ? 'Débloqué' : 'Verrouillé'}
                </div>
            </div>
            
            ${progressHtml}
            
            ${achievement.unlocked && achievement.formattedUnlockDate ? `
                <div class="achievement-unlock-date">
                    Débloqué le ${achievement.formattedUnlockDate}
                </div>
            ` : ''}
            
            ${rewardsHtml}
        `;

        return card;
    }

    createAchievementRewardsHtml(rewards) {
        if (!rewards || Object.keys(rewards).length === 0) {
            return '';
        }

        const rewardItems = Object.entries(rewards).map(([resource, amount]) => {
            const icons = {
                'gold': '💰',
                'reputation': '⭐',
                'materials': '🔨',
                'magic': '✨',
                'population': '👥'
            };
            
            return `<span class="achievement-reward-item">${icons[resource] || resource}: +${amount}</span>`;
        }).join('');

        return `
            <div class="achievement-rewards">
                <div class="achievement-rewards-label">Récompenses:</div>
                <div class="achievement-rewards-list">
                    ${rewardItems}
                </div>
            </div>
        `;
    }

    createAchievementProgressHtml(achievement) {
        return `
            <div class="achievement-progress">
                <div class="achievement-progress-text">
                    <span>Progression</span>
                    <span>${achievement.progress}/${achievement.maxProgress}</span>
                </div>
                <div class="achievement-progress-bar">
                    <div class="achievement-progress-fill" style="width: ${achievement.progressPercent}%"></div>
                </div>
            </div>
        `;
    }

    // === MÉTHODES POUR L'ONGLET ÉVÉNEMENTS ===

    renderEvents() {
        const eventInfo = this.gameManager.getEventInfo();
        if (!eventInfo) return;

        this.updateEventStats(eventInfo.stats);
        this.renderEventsList(eventInfo.events);
        this.updateEventsNotification();
        this.updateEventsControlButtons();
    }

    updateEventStats(stats) {
        document.getElementById('events-unread-count').textContent = stats.unreadCount;
        document.getElementById('events-total-count').textContent = stats.totalCount;
        document.getElementById('events-last-activity').textContent = stats.lastActivity;
        document.getElementById('events-section-count').textContent = `${stats.totalCount} événement${stats.totalCount > 1 ? 's' : ''}`;
    }

    updateEventsNotification() {
        const eventInfo = this.gameManager.getEventInfo();
        if (!eventInfo) return;

        const notificationBadge = document.getElementById('events-notification');
        if (notificationBadge) {
            const unreadCount = eventInfo.stats.unreadCount;
            if (unreadCount > 0) {
                notificationBadge.textContent = unreadCount;
                notificationBadge.style.display = 'inline-block';
            } else {
                notificationBadge.style.display = 'none';
            }
        }
    }

    updateEventsControlButtons() {
        const isPaused = this.gameManager.city && this.gameManager.city.isPaused;
        
        const markAllReadBtn = document.getElementById('mark-all-read-btn');
        const clearReadEventsBtn = document.getElementById('clear-read-events-btn');
        
        if (markAllReadBtn) {
            markAllReadBtn.disabled = isPaused;
        }
        
        if (clearReadEventsBtn) {
            clearReadEventsBtn.disabled = isPaused;
        }
    }

    renderEventsList(events) {
        const container = document.getElementById('events-list');
        if (!container) return;

        container.innerHTML = '';

        if (events.length === 0) {
            container.innerHTML = `
                <div style="text-align: center; padding: 40px; color: #999;">
                    <p>📰 Aucun événement pour le moment</p>
                    <p style="font-size: 0.9rem;">Les événements apparaîtront ici au fur et à mesure</p>
                </div>
            `;
            return;
        }

        events.forEach(event => {
            const card = this.createEventCard(event);
            container.appendChild(card);
        });
    }

    createEventCard(event) {
        const card = document.createElement('div');
        card.className = `event-card ${event.isRead ? '' : 'unread'}`;
        card.setAttribute('data-type', event.type);

        card.innerHTML = `
            <div class="event-header">
                <div class="event-type">
                    <span class="event-icon">${event.icon}</span>
                    <span>${this.getEventTypeName(event.type)}</span>
                </div>
                <div class="event-time">
                    ${event.relativeTime}
                </div>
            </div>
            
            <h4 class="event-title">${event.title}</h4>
            <div class="event-description">${event.description}</div>
            <div class="event-game-time">Jour ${event.gameDay} • ${event.formattedTime}</div>
            
            <div class="event-actions">
                ${this.createEventActions(event)}
            </div>
        `;

        return card;
    }

    getEventTypeName(type) {
        const typeNames = {
            construction: 'Construction',
            construction_complete: 'Construction terminée',
            upgrade_complete: 'Amélioration terminée',
            expedition_complete: 'Expédition terminée',
            expedition_success: 'Mission réussie',
            expedition_failure: 'Mission échouée',
            research_complete: 'Recherche terminée',
            adventurer_recruited: 'Aventurier recruté',
            adventurer_dismissed: 'Aventurier renvoyé',
            city_upgrade: 'Amélioration de ville',
            resource_gain: 'Gain de ressources',
            random_event: 'Événement aléatoire',
            danger: 'Danger',
            celebration: 'Célébration',
            trade: 'Commerce',
            discovery: 'Découverte'
        };
        return typeNames[type] || 'Événement';
    }

    createEventActions(event) {
        const actions = [];
        const isPaused = this.gameManager.city && this.gameManager.city.isPaused;
        
        if (!event.isRead) {
            actions.push(`
                <button class="event-action-btn acknowledge" 
                        onclick="app.markEventAsRead('${event.id}')"
                        ${isPaused ? 'disabled' : ''}>
                    ${isPaused ? 'Jeu en pause' : 'Marquer comme lu'}
                </button>
            `);
        }
        
        if (!event.isAcknowledged) {
            actions.push(`
                <button class="event-action-btn acknowledge" 
                        onclick="app.acknowledgeEvent('${event.id}')"
                        ${isPaused ? 'disabled' : ''}>
                    ${isPaused ? 'Jeu en pause' : 'Acquitter'}
                </button>
            `);
        }

        if (event.requiresChoice && event.choices && event.choices.length > 0) {
            event.choices.forEach(choice => {
                actions.push(`
                    <button class="event-action-btn choice" 
                            onclick="app.makeEventChoice('${event.id}', '${choice.id}')"
                            ${isPaused ? 'disabled' : ''}>
                        ${isPaused ? 'Jeu en pause' : choice.text}
                    </button>
                `);
            });
        }

        return actions.join('');
    }

    // Actions pour l'onglet Événements
    markEventAsRead(eventId) {
        const result = this.gameManager.markEventAsRead(eventId);
        this.showActionResult(result);
        
        if (result.success) {
            this.renderEvents();
        }
    }

    acknowledgeEvent(eventId) {
        const result = this.gameManager.acknowledgeEvent(eventId);
        this.showActionResult(result);
        
        if (result.success) {
            this.renderEvents();
        }
    }

    markAllEventsAsRead() {
        if (this.gameManager.city && this.gameManager.city.isPaused) {
            this.showActionResult({ success: false, message: 'Impossible de marquer les événements : jeu en pause' });
            return;
        }
        
        const result = this.gameManager.markAllEventsAsRead();
        this.showActionResult(result);
        
        if (result.success) {
            this.renderEvents();
        }
    }

    clearReadEvents() {
        if (this.gameManager.city && this.gameManager.city.isPaused) {
            this.showActionResult({ success: false, message: 'Impossible d\'effacer les événements : jeu en pause' });
            return;
        }
        
        const result = this.gameManager.clearReadEvents();
        this.showActionResult(result);
        
        if (result.success) {
            this.renderEvents();
        }
    }

    makeEventChoice(eventId, choiceId) {
        // Cette méthode sera implémentée dans les phases futures
        // pour les événements avec des choix
        console.log(`Choix ${choiceId} pour l'événement ${eventId}`);
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

// Sauvegarder avant fermeture
window.addEventListener('beforeunload', (event) => {
    if (app && app.gameManager) {
        app.gameManager.stopGameTimer();
        app.gameManager.autoSave();
    }
});

// Export pour utilisation dans d'autres modules
if (typeof module !== 'undefined' && module.exports) {
    module.exports = { GrimspireApp };
}