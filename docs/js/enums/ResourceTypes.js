/**
 * Enum ResourceTypes - Définit les types de ressources disponibles dans le jeu
 */
class ResourceTypes {
    static GOLD = 'gold';
    static POPULATION = 'population';
    static MATERIALS = 'materials';
    static MAGIC = 'magic';
    static REPUTATION = 'reputation';

    // Méthode statique pour obtenir tous les types de ressources
    static getAllTypes() {
        return [
            ResourceTypes.GOLD,
            ResourceTypes.POPULATION,
            ResourceTypes.MATERIALS,
            ResourceTypes.MAGIC,
            ResourceTypes.REPUTATION
        ];
    }

    // Méthode statique pour obtenir l'icône d'une ressource
    static getIcon(resourceType) {
        const icons = {
            [ResourceTypes.GOLD]: '💰',
            [ResourceTypes.POPULATION]: '👥',
            [ResourceTypes.MATERIALS]: '🔨',
            [ResourceTypes.MAGIC]: '✨',
            [ResourceTypes.REPUTATION]: '🏛️'
        };
        return icons[resourceType] || '📦';
    }

    // Méthode statique pour obtenir le nom d'une ressource
    static getName(resourceType) {
        const names = {
            [ResourceTypes.GOLD]: 'Or',
            [ResourceTypes.POPULATION]: 'Population',
            [ResourceTypes.MATERIALS]: 'Matériaux',
            [ResourceTypes.MAGIC]: 'Magie',
            [ResourceTypes.REPUTATION]: 'Réputation'
        };
        return names[resourceType] || resourceType;
    }

    // Méthode statique pour vérifier si un type de ressource est valide
    static isValid(resourceType) {
        return ResourceTypes.getAllTypes().includes(resourceType);
    }
}

if (typeof module !== 'undefined' && module.exports) {
    module.exports = ResourceTypes;
}