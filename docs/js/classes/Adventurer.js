/**
 * Classe Adventurer - Représente un aventurier
 */
class Adventurer {
    constructor(id, name, adventurerClass) {
        this.id = id;
        this.name = name;
        this.class = adventurerClass;
        this.level = 1;
        this.experience = 0;
        this.experienceToNext = this.calculateExperienceToNext();
        
        // Statistiques de base selon la classe
        this.stats = this.generateBaseStats();
        
        // État
        this.health = 100;
        this.maxHealth = 100;
        this.isOnMission = false;
        this.equipment = {
            weapon: null,
            armor: null,
            accessory: null
        };
        
        // Spécialisations débloquées
        this.specializations = [];
    }

    generateBaseStats() {
        const classStats = {
            'guerrier': { force: 15, intelligence: 8, agilite: 10, charisme: 9, chance: 8 },
            'mage': { force: 6, intelligence: 16, agilite: 8, charisme: 12, chance: 8 },
            'voleur': { force: 9, intelligence: 11, agilite: 16, charisme: 8, chance: 11 },
            'clerc': { force: 10, intelligence: 13, agilite: 8, charisme: 15, chance: 9 },
            'ranger': { force: 12, intelligence: 10, agilite: 14, charisme: 10, chance: 9 }
        };
        
        const baseStats = classStats[this.class] || classStats['guerrier'];
        
        // Ajouter une variation aléatoire de ±2
        const stats = {};
        for (const [stat, value] of Object.entries(baseStats)) {
            const variation = Math.floor(Math.random() * 5) - 2; // -2 à +2
            stats[stat] = Math.max(1, value + variation);
        }
        
        return stats;
    }

    calculateExperienceToNext() {
        return this.level * 100;
    }

    gainExperience(amount) {
        this.experience += amount;
        
        while (this.experience >= this.experienceToNext) {
            this.levelUp();
        }
    }

    levelUp() {
        this.experience -= this.experienceToNext;
        this.level++;
        this.experienceToNext = this.calculateExperienceToNext();
        
        // Amélioration des stats
        this.improveStats();
        
        // Augmentation de la santé maximale
        this.maxHealth += 10;
        this.health = this.maxHealth;
        
        // Déblocage de spécialisations
        this.unlockSpecializations();
    }

    improveStats() {
        // Chaque classe a des tendances d'amélioration différentes
        const improvements = {
            'guerrier': ['force', 'force', 'agilite'],
            'mage': ['intelligence', 'intelligence', 'charisme'],
            'voleur': ['agilite', 'agilite', 'chance'],
            'clerc': ['charisme', 'intelligence', 'force'],
            'ranger': ['agilite', 'force', 'chance']
        };
        
        const classImprovements = improvements[this.class] || improvements['guerrier'];
        const statToImprove = classImprovements[Math.floor(Math.random() * classImprovements.length)];
        
        this.stats[statToImprove] += 1;
    }

    unlockSpecializations() {
        // Spécialisations débloquées à certains niveaux
        const specializations = {
            5: this.getLevel5Specialization(),
            10: this.getLevel10Specialization(),
            15: this.getLevel15Specialization()
        };
        
        if (specializations[this.level]) {
            this.specializations.push(specializations[this.level]);
        }
    }

    getLevel5Specialization() {
        const spec = {
            'guerrier': 'Berserker',
            'mage': 'Elementaliste',
            'voleur': 'Assassin',
            'clerc': 'Paladin',
            'ranger': 'Traqueur'
        };
        return spec[this.class] || 'Vétéran';
    }

    getLevel10Specialization() {
        const spec = {
            'guerrier': 'Champion',
            'mage': 'Archimage',
            'voleur': 'Maître Voleur',
            'clerc': 'Grand Prêtre',
            'ranger': 'Maître Ranger'
        };
        return spec[this.class] || 'Expert';
    }

    getLevel15Specialization() {
        return 'Légende';
    }

    equipItem(item, slot) {
        if (this.equipment.hasOwnProperty(slot)) {
            this.equipment[slot] = item;
            return true;
        }
        return false;
    }

    heal(amount = null) {
        if (amount === null) {
            this.health = this.maxHealth;
        } else {
            this.health = Math.min(this.maxHealth, this.health + amount);
        }
    }

    takeDamage(amount) {
        this.health = Math.max(0, this.health - amount);
        return this.health <= 0;
    }

    isAlive() {
        return this.health > 0;
    }

    getCombatPower() {
        let power = this.stats.force + this.stats.agilite + (this.stats.intelligence * 0.5);
        power *= (1 + (this.level - 1) * 0.1);
        
        // Bonus d'équipement (sera implémenté plus tard)
        // power += this.getEquipmentBonus();
        
        return Math.floor(power);
    }

    getDisplayInfo() {
        return {
            id: this.id,
            name: this.name,
            class: this.class,
            level: this.level,
            experience: this.experience,
            experienceToNext: this.experienceToNext,
            stats: { ...this.stats },
            health: this.health,
            maxHealth: this.maxHealth,
            isOnMission: this.isOnMission,
            specializations: [...this.specializations],
            combatPower: this.getCombatPower()
        };
    }

    toJSON() {
        return {
            id: this.id,
            name: this.name,
            class: this.class,
            level: this.level,
            experience: this.experience,
            stats: this.stats,
            health: this.health,
            maxHealth: this.maxHealth,
            isOnMission: this.isOnMission,
            equipment: this.equipment,
            specializations: this.specializations
        };
    }

    static fromJSON(data) {
        const adventurer = new Adventurer(data.id, data.name, data.class);
        adventurer.level = data.level || 1;
        adventurer.experience = data.experience || 0;
        adventurer.stats = data.stats || adventurer.stats;
        adventurer.health = data.health || 100;
        adventurer.maxHealth = data.maxHealth || 100;
        adventurer.isOnMission = data.isOnMission || false;
        adventurer.equipment = data.equipment || adventurer.equipment;
        adventurer.specializations = data.specializations || [];
        adventurer.experienceToNext = adventurer.calculateExperienceToNext();
        return adventurer;
    }
}

if (typeof module !== 'undefined' && module.exports) {
    module.exports = Adventurer;
}