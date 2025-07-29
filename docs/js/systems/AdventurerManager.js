/**
 * Classe AdventurerManager - Gestion des aventuriers (recrutement, génération)
 */
class AdventurerManager {
    constructor(city) {
        this.city = city;
        this.availableForRecruitment = [];
        this.recruitmentCost = { gold: 200, reputation: 5 };
        this.searchCost = { gold: 50 };
        this.lastSearchTime = 0;
        this.searchCooldown = 0; // En tours
        
        // Noms et classes possibles pour la génération
        this.namePool = {
            male: [
                'Aldric', 'Bjorn', 'Cedric', 'Darius', 'Eldric', 'Finn', 'Gareth', 'Hadwin',
                'Ivan', 'Jarek', 'Kael', 'Leif', 'Magnus', 'Nolan', 'Osric', 'Perin',
                'Quinn', 'Ragnar', 'Soren', 'Theron', 'Ulric', 'Viktor', 'Willem', 'Xander'
            ],
            female: [
                'Aria', 'Brenna', 'Cora', 'Diana', 'Elena', 'Freya', 'Gwen', 'Hilda',
                'Iris', 'Jora', 'Kira', 'Luna', 'Mira', 'Nina', 'Olga', 'Petra',
                'Qira', 'Raven', 'Sera', 'Tara', 'Uma', 'Vera', 'Willa', 'Xylia'
            ]
        };
        
        this.classPool = ['guerrier', 'mage', 'voleur', 'clerc', 'ranger'];
        
        // Générer les premiers aventuriers disponibles
        this.generateInitialAdventurers();
    }

    generateInitialAdventurers() {
        // Générer 3-5 aventuriers de départ
        const count = 3 + Math.floor(Math.random() * 3);
        for (let i = 0; i < count; i++) {
            this.availableForRecruitment.push(this.generateRandomAdventurer());
        }
    }

    generateRandomAdventurer() {
        const gender = Math.random() < 0.5 ? 'male' : 'female';
        const namePool = this.namePool[gender];
        const name = namePool[Math.floor(Math.random() * namePool.length)];
        const adventurerClass = this.classPool[Math.floor(Math.random() * this.classPool.length)];
        const id = `recruit_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`;
        
        const adventurer = new Adventurer(id, name, adventurerClass);
        
        // Ajouter une petite variation de niveau pour les recrues
        const levelVariation = Math.floor(Math.random() * 3); // 0-2 niveaux bonus
        for (let i = 0; i < levelVariation; i++) {
            adventurer.gainExperience(adventurer.experienceToNext);
        }
        
        // Marquer comme recrue disponible
        adventurer.isRecruit = true;
        adventurer.recruitmentCost = this.calculateRecruitmentCost(adventurer);
        
        return adventurer;
    }

    calculateRecruitmentCost(adventurer) {
        const baseCost = { ...this.recruitmentCost };
        const levelMultiplier = 1 + (adventurer.level - 1) * 0.5;
        
        return {
            gold: Math.floor(baseCost.gold * levelMultiplier),
            reputation: Math.floor(baseCost.reputation * levelMultiplier)
        };
    }

    canSearchForAdventurers() {
        if (this.searchCooldown > 0) {
            return { canSearch: false, reason: `Recherche disponible dans ${this.searchCooldown} tour(s)` };
        }
        
        if (!this.city.resources.canAfford(this.searchCost)) {
            return { canSearch: false, reason: 'Ressources insuffisantes pour la recherche' };
        }
        
        if (!this.city.canPerformAction(1)) {
            return { canSearch: false, reason: 'Points d\'action insuffisants' };
        }
        
        return { canSearch: true };
    }

    searchForAdventurers() {
        const canSearch = this.canSearchForAdventurers();
        if (!canSearch.canSearch) {
            return { success: false, message: canSearch.reason, newAdventurers: [] };
        }
        
        // Payer le coût de la recherche
        this.city.resources.spend(this.searchCost);
        this.city.performAction(1);
        
        // Générer nouveaux aventuriers
        const newCount = 2 + Math.floor(Math.random() * 3); // 2-4 nouveaux aventuriers
        const newAdventurers = [];
        
        for (let i = 0; i < newCount; i++) {
            const adventurer = this.generateRandomAdventurer();
            newAdventurers.push(adventurer);
        }
        
        // Remplacer les anciens aventuriers disponibles
        this.availableForRecruitment = newAdventurers;
        
        // Définir le cooldown (1-2 tours)
        this.searchCooldown = 1 + Math.floor(Math.random() * 2);
        
        return {
            success: true,
            message: `${newCount} nouveaux aventuriers trouvés !`,
            newAdventurers: newAdventurers.map(a => a.getDisplayInfo())
        };
    }

    canRecruitAdventurer(adventurerId) {
        const adventurer = this.availableForRecruitment.find(a => a.id === adventurerId);
        if (!adventurer) {
            return { canRecruit: false, reason: 'Aventurier introuvable' };
        }
        
        if (!this.city.resources.canAfford(adventurer.recruitmentCost)) {
            return { canRecruit: false, reason: 'Ressources insuffisantes pour le recrutement' };
        }
        
        if (!this.city.canPerformAction(1)) {
            return { canRecruit: false, reason: 'Points d\'action insuffisants' };
        }
        
        // Vérifier la limite d'aventuriers (par exemple 20 max)
        if (this.city.adventurers.length >= 20) {
            return { canRecruit: false, reason: 'Limite d\'aventuriers atteinte (20 max)' };
        }
        
        return { canRecruit: true, adventurer };
    }

    recruitAdventurer(adventurerId) {
        const canRecruit = this.canRecruitAdventurer(adventurerId);
        if (!canRecruit.canRecruit) {
            return { success: false, message: canRecruit.reason };
        }
        
        const adventurer = canRecruit.adventurer;
        
        // Payer le coût de recrutement
        this.city.resources.spend(adventurer.recruitmentCost);
        this.city.performAction(1);
        
        // Retirer des aventuriers disponibles et ajouter à la ville
        this.availableForRecruitment = this.availableForRecruitment.filter(a => a.id !== adventurerId);
        
        // Nettoyer les propriétés de recrutement
        delete adventurer.isRecruit;
        delete adventurer.recruitmentCost;
        
        // Ajouter à la ville
        this.city.addAdventurer(adventurer);
        
        return {
            success: true,
            message: `${adventurer.name} a rejoint votre guilde !`,
            adventurer: adventurer.getDisplayInfo()
        };
    }

    dismissAdventurer(adventurerId) {
        const adventurer = this.city.getAdventurerById(adventurerId);
        if (!adventurer) {
            return { success: false, message: 'Aventurier introuvable' };
        }
        
        if (adventurer.isOnMission) {
            return { success: false, message: 'Impossible de renvoyer un aventurier en mission' };
        }
        
        // Retirer de la ville
        this.city.removeAdventurer(adventurerId);
        
        // Récupérer une partie des ressources investies
        const refund = {
            gold: Math.floor(this.recruitmentCost.gold * 0.3), // 30% de remboursement
            reputation: 0
        };
        
        this.city.resources.gain(refund);
        
        return {
            success: true,
            message: `${adventurer.name} a quitté votre guilde. Remboursement: ${refund.gold} or`
        };
    }

    // Gestion du cooldown (appelé lors du changement de phase)
    processTurnChange() {
        if (this.searchCooldown > 0) {
            this.searchCooldown--;
        }
    }

    // Getters pour l'interface
    getRecruitableAdventurers() {
        return this.availableForRecruitment.map(a => {
            const info = a.getDisplayInfo();
            info.recruitmentCost = a.recruitmentCost;
            info.isRecruit = true;
            return info;
        });
    }

    getRecruitedAdventurers() {
        return this.city.adventurers.map(a => a.getDisplayInfo());
    }

    getSearchInfo() {
        const canSearch = this.canSearchForAdventurers();
        return {
            canSearch: canSearch.canSearch,
            reason: canSearch.reason || null,
            cost: this.searchCost,
            cooldown: this.searchCooldown,
            availableCount: this.availableForRecruitment.length
        };
    }

    // Statistiques
    getGuildStats() {
        const recruited = this.city.adventurers;
        const available = this.getAvailableAdventurers();
        
        const classCounts = {};
        this.classPool.forEach(cls => classCounts[cls] = 0);
        
        recruited.forEach(adv => {
            classCounts[adv.class] = (classCounts[adv.class] || 0) + 1;
        });
        
        const totalLevels = recruited.reduce((sum, adv) => sum + adv.level, 0);
        const avgLevel = recruited.length > 0 ? Math.round(totalLevels / recruited.length * 10) / 10 : 0;
        
        return {
            totalRecruited: recruited.length,
            totalAvailable: available.length,
            availableForMission: available.length,
            onMission: recruited.filter(a => a.isOnMission).length,
            averageLevel: avgLevel,
            classCounts: classCounts
        };
    }

    getAvailableAdventurers() {
        return this.city.adventurers.filter(a => !a.isOnMission && a.isAlive());
    }

    // Sérialisation pour la sauvegarde
    toJSON() {
        return {
            availableForRecruitment: this.availableForRecruitment.map(a => a.toJSON()),
            searchCooldown: this.searchCooldown,
            lastSearchTime: this.lastSearchTime
        };
    }

    static fromJSON(data, city) {
        const manager = new AdventurerManager(city);
        
        if (data.availableForRecruitment) {
            manager.availableForRecruitment = data.availableForRecruitment.map(advData => {
                const adventurer = Adventurer.fromJSON(advData);
                adventurer.isRecruit = true;
                adventurer.recruitmentCost = manager.calculateRecruitmentCost(adventurer);
                return adventurer;
            });
        }
        
        manager.searchCooldown = data.searchCooldown || 0;
        manager.lastSearchTime = data.lastSearchTime || 0;
        
        return manager;
    }
}

if (typeof module !== 'undefined' && module.exports) {
    module.exports = AdventurerManager;
}