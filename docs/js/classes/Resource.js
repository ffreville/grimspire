/**
 * Classe Resource - Gestion des ressources de la ville
 */
class Resource {
    constructor(gold = 10000, population = 0, materials = 2000, magic = 0, reputation = 0) {
        this.gold = gold;
        this.population = population;
        this.materials = materials;
        this.magic = magic;
        this.reputation = reputation;
    }

    canAfford(cost) {
        return this.gold >= (cost.gold || 0) &&
               this.population >= (cost.population || 0) &&
               this.materials >= (cost.materials || 0) &&
               this.magic >= (cost.magic || 0);
    }

    spend(cost) {
        if (this.canAfford(cost)) {
            this.gold -= (cost.gold || 0);
            this.population -= (cost.population || 0);
            this.materials -= (cost.materials || 0);
            this.magic -= (cost.magic || 0);
            return true;
        }
        return false;
    }

    gain(amount) {
        this.gold += (amount.gold || 0);
        this.population += (amount.population || 0);
        this.materials += (amount.materials || 0);
        this.magic += (amount.magic || 0);
        this.reputation += (amount.reputation || 0);
    }

    toJSON() {
        return {
            gold: this.gold,
            population: this.population,
            materials: this.materials,
            magic: this.magic,
            reputation: this.reputation
        };
    }

    static fromJSON(data) {
        return new Resource(
            data.gold || 0,
            data.population || 0,
            data.materials || 0,
            data.magic || 0,
            data.reputation || 0
        );
    }
}

if (typeof module !== 'undefined' && module.exports) {
    module.exports = Resource;
}
