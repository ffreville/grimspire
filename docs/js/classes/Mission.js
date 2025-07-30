/**
 * Classe Mission - Représente une expédition/mission
 */
class Mission {
    constructor(id, name, type, difficulty, duration) {
        this.id = id;
        this.name = name;
        this.type = type; // 'exploration', 'combat', 'diplomatie', 'treasure'
        this.difficulty = difficulty; // 1-5
        this.duration = duration; // en jours de jeu (1-7)
        this.description = this.generateDescription();
        
        // État de la mission
        this.status = 'available'; // 'available', 'active', 'completed', 'failed'
        this.adventurers = [];
        this.startTime = null;
        this.endTime = null;
        this.progress = 0;
        
        // Récompenses et risques
        this.rewards = this.calculateRewards();
        this.risks = this.calculateRisks();
        this.requiredPartySize = this.calculateRequiredPartySize();
        
        // Résultats (après mission)
        this.results = null;
    }

    generateDescription() {
        const descriptions = {
            'exploration': [
                'Explorer les ruines antiques au nord de la ville',
                'Cartographier les cavernes mystérieuses de la forêt sombre',
                'Investiguer les étranges lumières dans les marais',
                'Découvrir ce qui se cache dans la tour abandonnée',
                'Explorer le temple oublié dans les montagnes'
            ],
            'combat': [
                'Éliminer le groupe de bandits qui terrorise les routes',
                'Affronter la meute de loups géants près du village',
                'Nettoyer le nid de gobelins dans les collines',
                'Chasser le dragon qui menace les fermes',
                'Repousser les orcs qui attaquent les caravanes'
            ],
            'diplomatie': [
                'Négocier un traité commercial avec la cité voisine',
                'Résoudre le conflit entre les guildes marchandes',
                'Convaincre les elfes de la forêt de nous aider',
                'Établir des relations avec la tribu nomade',
                'Médier le différend territorial entre deux villages'
            ],
            'treasure': [
                'Récupérer le trésor perdu dans les catacombes',
                'Voler les plans secrets de la forteresse ennemie',
                'Trouver l\'artefact magique dans le donjon maudit',
                'Rechercher les gemmes rares dans les mines profondes',
                'Récupérer les reliques sacrées du sanctuaire'
            ]
        };
        
        const typeDescriptions = descriptions[this.type] || descriptions['exploration'];
        return typeDescriptions[Math.floor(Math.random() * typeDescriptions.length)];
    }

    calculateRewards() {
        const baseRewards = {
            gold: 100 + (this.difficulty * 50),
            experience: 50 + (this.difficulty * 25),
            materials: Math.floor(this.difficulty * 10),
            magic: Math.floor(this.difficulty * 5)
        };

        // Bonus selon le type de mission
        const typeMultipliers = {
            'exploration': { experience: 1.5, materials: 1.2 },
            'combat': { gold: 1.3, experience: 1.2 },
            'diplomatie': { gold: 1.2, reputation: 10 },
            'treasure': { gold: 1.5, magic: 1.5 }
        };

        const multiplier = typeMultipliers[this.type] || {};
        const rewards = { ...baseRewards };

        Object.keys(multiplier).forEach(key => {
            if (rewards[key]) {
                rewards[key] = Math.floor(rewards[key] * multiplier[key]);
            } else {
                rewards[key] = multiplier[key];
            }
        });

        // Variation aléatoire ±20%
        Object.keys(rewards).forEach(key => {
            const variation = 0.8 + (Math.random() * 0.4); // 0.8 à 1.2
            rewards[key] = Math.floor(rewards[key] * variation);
        });

        return rewards;
    }

    calculateRisks() {
        return {
            injuryChance: Math.min(this.difficulty * 15, 60), // 15% à 60%
            deathChance: Math.min(this.difficulty * 5, 20),   // 5% à 20%
            failureChance: Math.max(60 - (this.difficulty * 10), 10) // 50% à 10%
        };
    }

    calculateRequiredPartySize() {
        return {
            min: Math.max(1, this.difficulty - 1),
            max: this.difficulty + 2,
            recommended: this.difficulty + 1
        };
    }

    canStartMission(availableAdventurers) {
        if (this.status !== 'available') {
            return { canStart: false, reason: 'Mission non disponible' };
        }

        if (availableAdventurers.length < this.requiredPartySize.min) {
            return { 
                canStart: false, 
                reason: `Minimum ${this.requiredPartySize.min} aventurier(s) requis` 
            };
        }

        return { canStart: true };
    }

    startMission(selectedAdventurers, currentTime) {
        if (this.status !== 'available') {
            return { success: false, message: 'Mission déjà en cours ou terminée' };
        }

        if (selectedAdventurers.length < this.requiredPartySize.min) {
            return { success: false, message: 'Pas assez d\'aventuriers sélectionnés' };
        }

        if (selectedAdventurers.length > this.requiredPartySize.max) {
            return { success: false, message: 'Trop d\'aventuriers sélectionnés' };
        }

        // Marquer les aventuriers comme en mission
        selectedAdventurers.forEach(adventurer => {
            adventurer.isOnMission = true;
        });

        this.adventurers = selectedAdventurers.map(a => a.id);
        this.status = 'active';
        this.startTime = currentTime;
        // Conversion : duration en jours de jeu -> millisecondes réelles
        // 1 jour de jeu = 1 minute réelle = 60000ms
        this.endTime = currentTime + (this.duration * 60000); 
        this.progress = 0;

        return { 
            success: true, 
            message: `Mission "${this.name}" démarrée avec ${selectedAdventurers.length} aventurier(s)` 
        };
    }

    updateProgress(currentTime) {
        if (this.status !== 'active' || !this.startTime || !this.endTime) {
            return;
        }

        const elapsed = currentTime - this.startTime;
        const total = this.endTime - this.startTime;
        this.progress = Math.min(100, Math.floor((elapsed / total) * 100));

        // Vérifier si la mission est terminée
        if (currentTime >= this.endTime) {
            this.completeMission();
        }
    }

    completeMission() {
        if (this.status !== 'active') return;

        this.status = 'completed';
        this.progress = 100;
        this.results = this.calculateMissionResults();
    }

    calculateMissionResults() {
        // Calculer le succès de la mission en fonction des aventuriers
        const partyPower = this.calculatePartyPower();
        const requiredPower = this.difficulty * 20;
        const successChance = Math.min(95, Math.max(5, (partyPower / requiredPower) * 70 + 15));
        
        const isSuccess = Math.random() * 100 < successChance;
        
        const results = {
            success: isSuccess,
            adventurerResults: [],
            rewards: isSuccess ? { ...this.rewards } : this.calculateFailureRewards(),
            message: ''
        };

        // Calculer les résultats pour chaque aventurier
        this.adventurers.forEach(adventurerId => {
            const adventurerResult = this.calculateAdventurerResult(isSuccess);
            results.adventurerResults.push({
                adventurerId: adventurerId,
                ...adventurerResult
            });
        });

        // Générer le message de résultat
        results.message = this.generateResultMessage(results);

        return results;
    }

    calculatePartyPower() {
        // Cette méthode sera appelée avec les vraies données des aventuriers
        // Pour l'instant, on estime basé sur le nombre d'aventuriers
        return this.adventurers.length * 15; // Estimation de base
    }

    calculateAdventurerResult(missionSuccess) {
        const result = {
            survived: true,
            injured: false,
            experienceGained: 0,
            specialReward: null
        };

        if (missionSuccess) {
            // Mission réussie - moins de risques
            const injuryRoll = Math.random() * 100;
            result.injured = injuryRoll < (this.risks.injuryChance * 0.5);
            result.experienceGained = Math.floor(this.rewards.experience / this.adventurers.length);
            
            // Chance de récompense spéciale
            if (Math.random() < 0.1) {
                result.specialReward = this.generateSpecialReward();
            }
        } else {
            // Mission échouée - plus de risques
            const injuryRoll = Math.random() * 100;
            const deathRoll = Math.random() * 100;
            
            result.injured = injuryRoll < this.risks.injuryChance;
            result.survived = deathRoll >= this.risks.deathChance;
            result.experienceGained = Math.floor(this.rewards.experience * 0.3 / this.adventurers.length);
        }

        return result;
    }

    calculateFailureRewards() {
        const failureRewards = {};
        Object.keys(this.rewards).forEach(key => {
            failureRewards[key] = Math.floor(this.rewards[key] * 0.2); // 20% des récompenses
        });
        return failureRewards;
    }

    generateSpecialReward() {
        const rewards = [
            'Potion de Guérison',
            'Amulette de Protection',
            'Carte au Trésor',
            'Grimoire Ancien',
            'Gemme Magique'
        ];
        return rewards[Math.floor(Math.random() * rewards.length)];
    }

    generateResultMessage(results) {
        if (results.success) {
            const injured = results.adventurerResults.filter(r => r.injured).length;
            const dead = results.adventurerResults.filter(r => !r.survived).length;
            
            let message = `Mission "${this.name}" accomplie avec succès ! `;
            if (dead > 0) {
                message += `${dead} aventurier(s) ont péri. `;
            } else if (injured > 0) {
                message += `${injured} aventurier(s) ont été blessés. `;
            } else {
                message += 'Tous les aventuriers sont revenus indemnes ! ';
            }
            return message;
        } else {
            return `Mission "${this.name}" échouée. Les aventuriers rentrent bredouilles...`;
        }
    }

    getRemainingTime(currentTime) {
        if (this.status !== 'active' || !this.endTime) return 0;
        return Math.max(0, this.endTime - currentTime);
    }

    getFormattedDuration() {
        if (this.duration === 1) {
            return '1 jour';
        } else {
            return `${this.duration} jours`;
        }
    }

    getFormattedRemainingTime(currentTime) {
        const remaining = this.getRemainingTime(currentTime);
        // 1 jour de jeu = 1 minute réelle = 60000ms
        const remainingDays = remaining / 60000;
        
        if (remainingDays >= 1) {
            const days = Math.floor(remainingDays);
            const hours = Math.floor((remainingDays % 1) * 24);
            
            if (hours === 0) {
                return days === 1 ? '1 jour' : `${days} jours`;
            } else {
                const dayText = days === 1 ? '1 jour' : `${days} jours`;
                const hourText = hours === 1 ? '1h' : `${hours}h`;
                return `${dayText} ${hourText}`;
            }
        } else {
            const hours = Math.floor(remainingDays * 24);
            const minutes = Math.floor((remainingDays * 24 * 60) % 60);
            
            if (hours === 0) {
                return `${Math.max(1, minutes)}min`;
            } else {
                return `${hours}h${minutes > 0 ? `${minutes}min` : ''}`;
            }
        }
    }

    getDifficultyStars() {
        return '⭐'.repeat(this.difficulty);
    }

    getTypeIcon() {
        const icons = {
            'exploration': '🗺️',
            'combat': '⚔️',
            'diplomatie': '🤝',
            'treasure': '💎'
        };
        return icons[this.type] || '📋';
    }

    getDisplayInfo() {
        return {
            id: this.id,
            name: this.name,
            type: this.type,
            difficulty: this.difficulty,
            duration: this.duration,
            description: this.description,
            status: this.status,
            progress: this.progress,
            adventurers: this.adventurers,
            rewards: this.rewards,
            risks: this.risks,
            requiredPartySize: this.requiredPartySize,
            results: this.results,
            formattedDuration: this.getFormattedDuration(),
            difficultyStars: this.getDifficultyStars(),
            typeIcon: this.getTypeIcon()
        };
    }

    toJSON() {
        return {
            id: this.id,
            name: this.name,
            type: this.type,
            difficulty: this.difficulty,
            duration: this.duration,
            description: this.description,
            status: this.status,
            adventurers: this.adventurers,
            startTime: this.startTime,
            endTime: this.endTime,
            progress: this.progress,
            rewards: this.rewards,
            risks: this.risks,
            requiredPartySize: this.requiredPartySize,
            results: this.results
        };
    }

    static fromJSON(data) {
        const mission = new Mission(data.id, data.name, data.type, data.difficulty, data.duration);
        mission.description = data.description || mission.description;
        mission.status = data.status || 'available';
        mission.adventurers = data.adventurers || [];
        mission.startTime = data.startTime || null;
        mission.endTime = data.endTime || null;
        mission.progress = data.progress || 0;
        mission.rewards = data.rewards || mission.rewards;
        mission.risks = data.risks || mission.risks;
        mission.requiredPartySize = data.requiredPartySize || mission.requiredPartySize;
        mission.results = data.results || null;
        return mission;
    }
}

if (typeof module !== 'undefined' && module.exports) {
    module.exports = Mission;
}