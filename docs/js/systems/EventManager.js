/**
 * Classe EventManager - Gestionnaire des événements de la ville
 */
class EventManager {
    constructor(city) {
        this.city = city;
        this.events = [];
        this.nextEventId = 1;
    }

    // Créer un nouvel événement
    createEvent(type, title, description, additionalData = {}) {
        const event = {
            id: `event_${this.nextEventId++}`,
            type: type,
            title: title,
            description: description,
            timestamp: Date.now(),
            gameDay: this.city.day,
            gameTime: this.city.currentTime,
            formattedTime: this.city.getFormattedTime(),
            isRead: false,
            isAcknowledged: false,
            requiresChoice: false,
            choices: [],
            icon: this.getEventIcon(type),
            ...additionalData
        };

        this.events.unshift(event); // Ajouter en tête pour avoir les plus récents en premier
        return event;
    }

    // Obtenir l'icône selon le type d'événement
    getEventIcon(type) {
        const icons = {
            construction: '🏗️',
            construction_complete: '✅',
            upgrade_complete: '⬆️',
            expedition_complete: '🏴‍☠️',
            expedition_success: '🎉',
            expedition_failure: '💀',
            research_complete: '🔬',
            adventurer_recruited: '⚔️',
            adventurer_dismissed: '👋',
            city_upgrade: '🏛️',
            resource_gain: '💰',
            random_event: '🎲',
            danger: '⚠️',
            celebration: '🎊',
            trade: '🛒',
            discovery: '🔍'
        };
        return icons[type] || '📰';
    }

    // Marquer un événement comme lu
    markAsRead(eventId) {
        const event = this.events.find(e => e.id === eventId);
        if (event) {
            event.isRead = true;
            return true;
        }
        return false;
    }

    // Acquitter un événement
    acknowledgeEvent(eventId) {
        const event = this.events.find(e => e.id === eventId);
        if (event) {
            event.isRead = true;
            event.isAcknowledged = true;
            return true;
        }
        return false;
    }

    // Marquer tous les événements comme lus
    markAllAsRead() {
        let count = 0;
        this.events.forEach(event => {
            if (!event.isRead) {
                event.isRead = true;
                count++;
            }
        });
        return count;
    }

    // Effacer les événements lus
    clearReadEvents() {
        const initialCount = this.events.length;
        this.events = this.events.filter(event => !event.isRead || !event.isAcknowledged);
        return initialCount - this.events.length;
    }

    // Obtenir les statistiques des événements
    getEventStats() {
        const unreadCount = this.events.filter(e => !e.isRead).length;
        const totalCount = this.events.length;
        const lastActivity = this.events.length > 0 ? this.events[0].formattedTime : '-';

        return {
            unreadCount,
            totalCount,
            lastActivity
        };
    }

    // Obtenir tous les événements pour l'affichage
    getAllEvents() {
        return this.events.map(event => ({
            ...event,
            relativeTime: this.getRelativeTime(event.timestamp)
        }));
    }

    // Obtenir le temps relatif d'un événement
    getRelativeTime(timestamp) {
        const now = Date.now();
        const diff = now - timestamp;
        const minutes = Math.floor(diff / (1000 * 60));
        
        if (minutes < 1) return 'À l\'instant';
        if (minutes < 60) return `Il y a ${minutes}min`;
        
        const hours = Math.floor(minutes / 60);
        if (hours < 24) return `Il y a ${hours}h`;
        
        const days = Math.floor(hours / 24);
        return `Il y a ${days}j`;
    }

    // === ÉVÉNEMENTS SPÉCIFIQUES ===

    // Événement de construction terminée
    onBuildingConstructionComplete(building) {
        const title = `Construction terminée : ${building.customName}`;
        const description = `Le bâtiment "${building.customName}" (${building.buildingType.name}) a été construit avec succès !`;
        
        let additionalInfo = '';
        if (building.buildingType.unlocksTab) {
            additionalInfo = ` Ce bâtiment débloque l'onglet "${building.buildingType.unlocksTab}".`;
        }

        return this.createEvent(
            'construction_complete',
            title,
            description + additionalInfo,
            {
                buildingId: building.id,
                buildingType: building.buildingType.name,
                district: building.buildingType.district
            }
        );
    }

    // Événement d'amélioration terminée
    onBuildingUpgradeComplete(building) {
        const title = `Amélioration terminée : ${building.customName}`;
        const description = `Le bâtiment "${building.customName}" a été amélioré au niveau ${building.level} !`;

        return this.createEvent(
            'upgrade_complete',
            title,
            description,
            {
                buildingId: building.id,
                buildingType: building.buildingType.name,
                newLevel: building.level
            }
        );
    }

    // Événement de recherche terminée
    onResearchComplete(upgrade) {
        const title = `Recherche terminée : ${upgrade.name}`;
        const description = `La recherche "${upgrade.name}" a été terminée avec succès ! ${upgrade.description}`;

        return this.createEvent(
            'research_complete',
            title,
            description,
            {
                upgradeId: upgrade.id,
                upgradeName: upgrade.name
            }
        );
    }

    // Événement de mission terminée
    onMissionComplete(mission, results) {
        const success = results && results.success;
        const type = success ? 'expedition_success' : 'expedition_failure';
        const title = success ? `Mission réussie : ${mission.name}` : `Mission échouée : ${mission.name}`;
        const description = results.message || 'Mission terminée.';

        return this.createEvent(
            type,
            title,
            description,
            {
                missionId: mission.id,
                missionName: mission.name,
                success: success,
                rewards: success ? mission.rewards : null,
                adventurers: mission.adventurers
            }
        );
    }

    // Événement de recrutement d'aventurier
    onAdventurerRecruited(adventurer) {
        const title = `Nouvel aventurier recruté : ${adventurer.name}`;
        const description = `${adventurer.name}, un ${adventurer.class} de niveau ${adventurer.level}, a rejoint votre guilde !`;

        return this.createEvent(
            'adventurer_recruited',
            title,
            description,
            {
                adventurerId: adventurer.id,
                adventurerName: adventurer.name,
                adventurerClass: adventurer.class,
                adventurerLevel: adventurer.level
            }
        );
    }

    // Méthodes de sérialisation
    toJSON() {
        return {
            events: this.events,
            nextEventId: this.nextEventId
        };
    }

    static fromJSON(data, city) {
        const manager = new EventManager(city);
        if (data) {
            manager.events = data.events || [];
            manager.nextEventId = data.nextEventId || 1;
        }
        return manager;
    }
}

if (typeof module !== 'undefined' && module.exports) {
    module.exports = EventManager;
}