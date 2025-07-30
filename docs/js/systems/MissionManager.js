/**
 * Classe MissionManager - Gestion des missions et expéditions
 */
class MissionManager {
    constructor(city) {
        this.city = city;
        this.availableMissions = [];
        this.activeMissions = [];
        this.completedMissions = [];
        this.maxAvailableMissions = 5;
        this.refreshCost = { gold: 100 };
        this.refreshCooldown = 0;
        
        // Types de missions avec probabilités
        this.missionTypes = [
            { type: 'exploration', weight: 30 },
            { type: 'combat', weight: 25 },
            { type: 'diplomatie', weight: 20 },
            { type: 'treasure', weight: 25 }
        ];
        
        // Noms de missions par type
        this.missionNames = {
            'exploration': [
                'Exploration des Ruines Oubliées',
                'Cartographie des Terres Inconnues',
                'Investigation Mystérieuse',
                'Découverte Archéologique',
                'Reconnaissance de Territoire'
            ],
            'combat': [
                'Élimination de Menaces',
                'Nettoyage de Zone',
                'Combat contre les Bandits',
                'Chasse aux Monstres',
                'Défense de Caravane'
            ],
            'diplomatie': [
                'Mission Diplomatique',
                'Négociation Commerciale',
                'Médiation de Conflit',
                'Alliance Stratégique',
                'Traité de Paix'
            ],
            'treasure': [
                'Chasse au Trésor',
                'Récupération d\'Artefact',
                'Raid de Donjon',
                'Recherche de Reliques',
                'Vol Organisé'
            ]
        };
        
        // Générer les missions initiales
        this.generateInitialMissions();
    }

    generateInitialMissions() {
        for (let i = 0; i < this.maxAvailableMissions; i++) {
            this.availableMissions.push(this.generateRandomMission());
        }
    }

    generateRandomMission() {
        // Sélectionner un type de mission selon les poids
        const totalWeight = this.missionTypes.reduce((sum, type) => sum + type.weight, 0);
        let random = Math.random() * totalWeight;
        let selectedType = 'exploration';

        for (const missionType of this.missionTypes) {
            random -= missionType.weight;
            if (random <= 0) {
                selectedType = missionType.type;
                break;
            }
        }

        // Générer les paramètres de la mission
        const difficulty = Math.ceil(Math.random() * 5); // 1-5
        
        // Durées en jours de jeu (1-7 jours)
        const durations = [
            1,  // 1 jour
            2,  // 2 jours
            3,  // 3 jours
            4,  // 4 jours
            5,  // 5 jours
            6,  // 6 jours
            7   // 7 jours
        ];
        
        // Sélectionner une durée selon la difficulté (plus difficile = plus long)
        // Difficulté 1 -> 1-2 jours, difficulté 5 -> 5-7 jours
        const minDuration = difficulty;
        const maxDuration = Math.min(difficulty + 2, 7);
        const availableDurations = durations.slice(minDuration - 1, maxDuration);
        const duration = availableDurations[Math.floor(Math.random() * availableDurations.length)];
        
        // Sélectionner un nom
        const typeNames = this.missionNames[selectedType];
        const baseName = typeNames[Math.floor(Math.random() * typeNames.length)];
        const name = this.generateUniqueName(baseName);
        
        const id = `mission_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`;
        
        return new Mission(id, name, selectedType, difficulty, duration);
    }

    generateUniqueName(baseName) {
        // Ajouter des suffixes pour rendre les noms uniques
        const suffixes = [
            'I', 'II', 'III', 'IV', 'V',
            'Alpha', 'Beta', 'Gamma', 'Delta',
            'Nord', 'Sud', 'Est', 'Ouest',
            'Urgente', 'Secrète', 'Dangereuse', 'Lucrative'
        ];
        
        // Vérifier si le nom existe déjà
        const existingNames = [
            ...this.availableMissions.map(m => m.name),
            ...this.activeMissions.map(m => m.name),
            ...this.completedMissions.map(m => m.name)
        ];
        
        if (!existingNames.includes(baseName)) {
            return baseName;
        }
        
        // Ajouter un suffixe aléatoire
        const suffix = suffixes[Math.floor(Math.random() * suffixes.length)];
        return `${baseName} ${suffix}`;
    }

    canRefreshMissions() {
        if (this.refreshCooldown > 0) {
            return { canRefresh: false, reason: `Actualisation disponible dans ${this.refreshCooldown} tour(s)` };
        }
        
        if (!this.city.resources.canAfford(this.refreshCost)) {
            return { canRefresh: false, reason: 'Ressources insuffisantes pour l\'actualisation' };
        }
        
        if (!this.city.canPerformAction(1)) {
            return { canRefresh: false, reason: 'Points d\'action insuffisants' };
        }
        
        return { canRefresh: true };
    }

    refreshMissions() {
        const canRefresh = this.canRefreshMissions();
        if (!canRefresh.canRefresh) {
            return { success: false, message: canRefresh.reason };
        }
        
        // Payer le coût
        this.city.resources.spend(this.refreshCost);
        this.city.performAction(1);
        
        // Générer de nouvelles missions
        this.availableMissions = [];
        for (let i = 0; i < this.maxAvailableMissions; i++) {
            this.availableMissions.push(this.generateRandomMission());
        }
        
        // Définir le cooldown
        this.refreshCooldown = 2;
        
        return {
            success: true,
            message: `${this.maxAvailableMissions} nouvelles missions disponibles !`
        };
    }

    getMissionById(missionId) {
        return this.availableMissions.find(m => m.id === missionId) ||
               this.activeMissions.find(m => m.id === missionId) ||
               this.completedMissions.find(m => m.id === missionId);
    }

    canStartMission(missionId, selectedAdventurerIds) {
        const mission = this.availableMissions.find(m => m.id === missionId);
        if (!mission) {
            return { canStart: false, reason: 'Mission introuvable' };
        }

        if (mission.status !== 'available') {
            return { canStart: false, reason: 'Mission non disponible' };
        }

        const availableAdventurers = this.city.adventurers.filter(a => 
            !a.isOnMission && a.isAlive() && selectedAdventurerIds.includes(a.id)
        );

        if (availableAdventurers.length !== selectedAdventurerIds.length) {
            return { canStart: false, reason: 'Certains aventuriers ne sont pas disponibles' };
        }

        if (availableAdventurers.length < mission.requiredPartySize.min) {
            return { 
                canStart: false, 
                reason: `Minimum ${mission.requiredPartySize.min} aventurier(s) requis` 
            };
        }

        if (availableAdventurers.length > mission.requiredPartySize.max) {
            return { 
                canStart: false, 
                reason: `Maximum ${mission.requiredPartySize.max} aventurier(s) autorisés` 
            };
        }

        return { canStart: true, adventurers: availableAdventurers };
    }

    startMission(missionId, selectedAdventurerIds) {
        const canStart = this.canStartMission(missionId, selectedAdventurerIds);
        if (!canStart.canStart) {
            return { success: false, message: canStart.reason };
        }

        const mission = this.availableMissions.find(m => m.id === missionId);
        const result = mission.startMission(canStart.adventurers, Date.now());
        
        if (result.success) {
            // Déplacer la mission de disponible à active
            this.availableMissions = this.availableMissions.filter(m => m.id !== missionId);
            this.activeMissions.push(mission);
            
            // Marquer les aventuriers comme en mission
            canStart.adventurers.forEach(adventurer => {
                adventurer.isOnMission = true;
            });
        }
        
        return result;
    }

    updateActiveMissions() {
        const currentTime = Date.now();
        const completedMissions = [];
        
        this.activeMissions.forEach(mission => {
            mission.updateProgress(currentTime);
            
            if (mission.status === 'completed') {
                completedMissions.push(mission);
                this.processMissionCompletion(mission);
            }
        });
        
        // Déplacer les missions terminées
        completedMissions.forEach(mission => {
            this.activeMissions = this.activeMissions.filter(m => m.id !== mission.id);
            this.completedMissions.unshift(mission); // Ajouter au début
        });
        
        // Limiter les missions terminées gardées en mémoire
        if (this.completedMissions.length > 20) {
            this.completedMissions = this.completedMissions.slice(0, 20);
        }
        
        return completedMissions.length > 0;
    }

    processMissionCompletion(mission) {
        const results = mission.results;
        if (!results) return;
        
        // Libérer les aventuriers
        mission.adventurers.forEach(adventurerId => {
            const adventurer = this.city.getAdventurerById(adventurerId);
            if (adventurer) {
                adventurer.isOnMission = false;
                
                // Traiter les résultats pour cet aventurier
                const adventurerResult = results.adventurerResults.find(r => r.adventurerId === adventurerId);
                if (adventurerResult) {
                    this.applyAdventurerResults(adventurer, adventurerResult);
                }
            }
        });
        
        // Donner les récompenses à la ville
        if (results.success && results.rewards) {
            this.city.resources.gain(results.rewards);
        }
    }

    applyAdventurerResults(adventurer, result) {
        // Appliquer l'expérience
        if (result.experienceGained > 0) {
            adventurer.gainExperience(result.experienceGained);
        }
        
        // Appliquer les blessures
        if (result.injured) {
            const damage = Math.floor(adventurer.maxHealth * 0.3); // 30% de dégâts
            adventurer.takeDamage(damage);
        }
        
        // Traiter la mort (rare)
        if (!result.survived) {
            adventurer.health = 0;
        }
        
        // Récompenses spéciales (à implémenter plus tard si besoin)
        if (result.specialReward) {
            // Ajouter à un inventaire d'aventurier (futur)
        }
    }

    // Gestion du cooldown (appelé lors du changement de phase)
    processTurnChange() {
        if (this.refreshCooldown > 0) {
            this.refreshCooldown--;
        }
        
        // Mettre à jour les missions actives
        this.updateActiveMissions();
    }

    // Getters pour l'interface
    getAvailableMissions() {
        return this.availableMissions.map(m => m.getDisplayInfo());
    }

    getActiveMissions() {
        // Mettre à jour le progrès avant de retourner
        this.updateActiveMissions();
        return this.activeMissions.map(m => {
            const info = m.getDisplayInfo();
            info.formattedRemainingTime = m.getFormattedRemainingTime(Date.now());
            return info;
        });
    }

    getCompletedMissions() {
        return this.completedMissions.slice(0, 10).map(m => m.getDisplayInfo()); // Les 10 dernières
    }

    getRefreshInfo() {
        const canRefresh = this.canRefreshMissions();
        return {
            canRefresh: canRefresh.canRefresh,
            reason: canRefresh.reason || null,
            cost: this.refreshCost,
            cooldown: this.refreshCooldown
        };
    }

    getMissionStats() {
        const completed = this.completedMissions.length;
        const successful = this.completedMissions.filter(m => m.results && m.results.success).length;
        const active = this.activeMissions.length;
        const available = this.availableMissions.length;
        
        return {
            totalCompleted: completed,
            successfulMissions: successful,
            successRate: completed > 0 ? Math.round((successful / completed) * 100) : 0,
            activeMissions: active,
            availableMissions: available
        };
    }

    // Sérialisation pour la sauvegarde
    toJSON() {
        return {
            availableMissions: this.availableMissions.map(m => m.toJSON()),
            activeMissions: this.activeMissions.map(m => m.toJSON()),
            completedMissions: this.completedMissions.map(m => m.toJSON()),
            refreshCooldown: this.refreshCooldown
        };
    }

    static fromJSON(data, city) {
        const manager = new MissionManager(city);
        
        if (data.availableMissions) {
            manager.availableMissions = data.availableMissions.map(m => Mission.fromJSON(m));
        }
        
        if (data.activeMissions) {
            manager.activeMissions = data.activeMissions.map(m => Mission.fromJSON(m));
        }
        
        if (data.completedMissions) {
            manager.completedMissions = data.completedMissions.map(m => Mission.fromJSON(m));
        }
        
        manager.refreshCooldown = data.refreshCooldown || 0;
        
        return manager;
    }
}

if (typeof module !== 'undefined' && module.exports) {
    module.exports = MissionManager;
}