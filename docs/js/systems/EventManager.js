/**
 * Classe EventManager - Gestionnaire des événements de la ville
 */
class EventManager {
    constructor(city) {
        this.city = city;
        this.events = [];
        this.nextEventId = 1;
        this.randomEventTypes = this.initializeRandomEventTypes();
        this.dailyEventCount = { min: 4, max: 5 };
        this.scheduledEvents = []; // Événements programmés pour être générés dans les heures suivantes
    }

    // Initialiser les types d'événements aléatoires
    initializeRandomEventTypes() {
        return [
            {
                id: 'citizen_donation',
                name: 'Don d\'un citoyen',
                description: 'Un citoyen généreux fait un don à la ville',
                type: 'random_event',
                icon: '💰',
                weight: 60, // Probabilité relative réduite pour faire place aux événements à choix
                effects: {
                    gold: 5
                },
                messages: [
                    'Un marchand reconnaissant vous offre quelques pièces d\'or.',
                    'Un citoyen satisfait fait un petit don à la ville.',
                    'Vous trouvez une bourse perdue dans la rue.',
                    'Un aventurier de passage laisse une donation.',
                    'Les taxes locales rapportent un peu plus que prévu.'
                ]
            },
            {
                id: 'merchant_citizen_conflict',
                name: 'Conflit entre un commerçant et un habitant',
                description: 'Un conflit éclate entre un commerçant et un habitant au sujet d\'un produit défectueux.',
                type: 'choice_event',
                icon: '⚖️',
                weight: 20,
                requiresChoice: true,
                messages: [
                    'Un commerçant refuse de rembourser un habitant pour un produit défectueux. Le conflit s\'envenime et tous deux vous demandent de trancher.',
                    'Une dispute éclate au marché entre un marchand et un client. L\'affaire divise les habitants et nécessite votre intervention.',
                    'Un artisan refuse de reprendre un objet mal conçu. L\'acheteur exige justice. Votre décision influencera votre réputation.'
                ],
                choices: [
                    {
                        id: 'help_citizen',
                        text: 'Soutenir l\'habitant',
                        description: 'Vous ordonnez au commerçant de rembourser et payez une compensation à l\'habitant.',
                        effects: {
                            gold: -10,
                            reputation: 5
                        }
                    },
                    {
                        id: 'help_merchant',
                        text: 'Soutenir le commerçant', 
                        description: 'Vous soutenez le commerçant qui vous verse une somme en remerciement.',
                        effects: {
                            gold: 15,
                            reputation: -3
                        }
                    }
                ]
            },
            {
                id: 'festival_request',
                name: 'Demande pour organiser une fête',
                description: 'Les habitants demandent l\'organisation d\'une fête pour célébrer la prospérité de la ville.',
                type: 'choice_event',
                icon: '🎉',
                weight: 20,
                requiresChoice: true,
                messages: [
                    'Une délégation d\'habitants vous demande de financer une grande fête pour célébrer les récents succès de la ville.',
                    'Les citoyens souhaitent organiser un festival et demandent une contribution de la ville pour les festivités.',
                    'Un groupe d\'artisans propose d\'organiser une célébration publique si vous acceptez d\'en couvrir les frais.'
                ],
                choices: [
                    {
                        id: 'fund_festival',
                        text: 'Financer la fête',
                        description: 'Vous acceptez de financer la fête. Les habitants sont ravis et votre popularité augmente.',
                        effects: {
                            gold: -25,
                            reputation: 8
                        }
                    },
                    {
                        id: 'refuse_festival',
                        text: 'Refuser de financer',
                        description: 'Vous refusez de financer la fête. L\'argent reste dans les caisses mais les habitants sont déçus.',
                        effects: {
                            gold: 0,
                            reputation: -5
                        }
                    }
                ]
            }
        ];
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

    // Gérer le choix d'un événement
    makeEventChoice(eventId, choiceId) {
        const event = this.events.find(e => e.id === eventId);
        if (!event || !event.requiresChoice) {
            return { success: false, message: 'Événement non trouvé ou ne nécessite pas de choix' };
        }

        const choice = event.choices.find(c => c.id === choiceId);
        if (!choice) {
            return { success: false, message: 'Choix non valide' };
        }

        // Appliquer les effets du choix
        if (choice.effects) {
            this.city.resources.gain(choice.effects);
        }

        // Marquer l'événement comme traité et acquitté
        event.isRead = true;
        event.isAcknowledged = true;
        event.choiceMade = choiceId;
        event.choiceText = choice.text;

        return {
            success: true,
            message: `Choix "${choice.text}" sélectionné`,
            effects: choice.effects || {}
        };
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

    // Acquitter tous les événements
    acknowledgeAllEvents() {
        let count = 0;
        this.events.forEach(event => {
            if (!event.isAcknowledged) {
                event.isRead = true;
                event.isAcknowledged = true;
                count++;
            }
        });
        return count;
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

    // === ÉVÉNEMENTS ALÉATOIRES ===

    // Programmer des événements aléatoires pour un nouveau jour
    generateDailyRandomEvents() {
        // Déterminer le nombre d'événements à générer (entre 4 et 5)
        const eventCount = Math.floor(Math.random() * (this.dailyEventCount.max - this.dailyEventCount.min + 1)) + this.dailyEventCount.min;
        
        // Vider les événements programmés précédents
        this.scheduledEvents = [];
        
        // Programmer les événements à des heures différentes de la journée (éviter minuit-6h)
        const possibleHours = [];
        for (let hour = 6; hour < 24; hour++) { // De 6h à 23h
            for (let quarter = 0; quarter < 4; quarter++) { // Chaque quart d'heure
                possibleHours.push(hour * 60 + quarter * 15); // Convertir en minutes
            }
        }
        
        // Mélanger les heures disponibles
        for (let i = possibleHours.length - 1; i > 0; i--) {
            const j = Math.floor(Math.random() * (i + 1));
            [possibleHours[i], possibleHours[j]] = [possibleHours[j], possibleHours[i]];
        }
        
        // Programmer les événements
        for (let i = 0; i < eventCount && i < possibleHours.length; i++) {
            const randomEvent = this.selectRandomEvent();
            if (randomEvent) {
                this.scheduledEvents.push({
                    eventType: randomEvent,
                    scheduledTime: possibleHours[i], // Temps en minutes depuis minuit
                    day: this.city.day
                });
            }
        }
        
        console.log(`${this.scheduledEvents.length} événement(s) aléatoire(s) programmé(s) pour le jour ${this.city.day}`);
        return [];
    }
    
    // Vérifier et déclencher les événements programmés
    checkScheduledEvents() {
        if (!this.scheduledEvents || this.scheduledEvents.length === 0) return [];
        
        const triggeredEvents = [];
        const currentDay = this.city.day;
        const currentTime = this.city.currentTime; // Minutes depuis minuit
        
        // Trouver les événements à déclencher
        const eventsToTrigger = this.scheduledEvents.filter(scheduledEvent => 
            scheduledEvent.day === currentDay && 
            scheduledEvent.scheduledTime <= currentTime
        );
        
        // Déclencher les événements
        eventsToTrigger.forEach(scheduledEvent => {
            const event = this.createRandomEvent(scheduledEvent.eventType);
            triggeredEvents.push(event);
            
            // Afficher une popup pour l'événement aléatoire
            this.showRandomEventPopup(event);
        });
        
        // Retirer les événements déclenchés de la liste
        this.scheduledEvents = this.scheduledEvents.filter(scheduledEvent => 
            !(scheduledEvent.day === currentDay && scheduledEvent.scheduledTime <= currentTime)
        );
        
        return triggeredEvents;
    }

    // Sélectionner un événement aléatoire basé sur les poids
    selectRandomEvent() {
        if (this.randomEventTypes.length === 0) return null;
        
        // Calculer le poids total
        const totalWeight = this.randomEventTypes.reduce((sum, event) => sum + event.weight, 0);
        
        // Sélectionner un nombre aléatoire
        let random = Math.random() * totalWeight;
        
        // Trouver l'événement correspondant
        for (const eventType of this.randomEventTypes) {
            random -= eventType.weight;
            if (random <= 0) {
                return eventType;
            }
        }
        
        // Fallback - retourner le premier événement
        return this.randomEventTypes[0];
    }

    // Créer un événement aléatoire basé sur un type
    createRandomEvent(eventType) {
        // Sélectionner un message aléatoire
        const randomMessage = eventType.messages[Math.floor(Math.random() * eventType.messages.length)];
        
        // Pour les événements sans choix, appliquer les effets immédiatement
        if (!eventType.requiresChoice && eventType.effects && this.city.resources) {
            Object.entries(eventType.effects).forEach(([resource, amount]) => {
                if (this.city.resources[resource] !== undefined) {
                    this.city.resources[resource] += amount;
                }
            });
        }
        
        // Créer l'événement
        const event = this.createEvent(
            eventType.type,
            eventType.name,
            randomMessage,
            {
                eventTypeId: eventType.id,
                effects: eventType.effects,
                isRandomEvent: true,
                requiresChoice: eventType.requiresChoice || false,
                choices: eventType.choices || [],
                originalEventType: eventType // Garder une référence pour les choix
            }
        );
        
        // Forcer l'icône de l'événement
        event.icon = eventType.icon;
        
        return event;
    }
    
    // Afficher une popup pour un événement aléatoire
    showRandomEventPopup(event) {
        // Créer la popup s'il n'y en a pas déjà une
        let popup = document.getElementById('random-event-popup');
        if (!popup) {
            popup = this.createRandomEventPopup();
            document.body.appendChild(popup);
        }
        
        // Remplir le contenu de la popup
        const popupTitle = popup.querySelector('.popup-title');
        const popupIcon = popup.querySelector('.popup-icon');
        const popupDescription = popup.querySelector('.popup-description');
        const popupEffects = popup.querySelector('.popup-effects');
        const popupFooter = popup.querySelector('.popup-footer');
        
        popupTitle.textContent = event.title;
        popupIcon.textContent = event.icon;
        popupDescription.textContent = event.description;
        
        // Si c'est un événement avec choix, afficher les choix au lieu des effets
        if (event.requiresChoice && event.choices && event.choices.length > 0) {
            popupEffects.style.display = 'none';
            
            // Créer les boutons de choix
            popupFooter.innerHTML = '';
            event.choices.forEach(choice => {
                const choiceBtn = document.createElement('button');
                choiceBtn.className = 'popup-btn choice-btn';
                choiceBtn.innerHTML = `
                    <div class="choice-text">${choice.text}</div>
                    <div class="choice-effects">${this.formatChoiceEffects(choice.effects)}</div>
                `;
                
                choiceBtn.addEventListener('click', () => {
                    this.handleEventChoice(event.id, choice.id);
                    popup.classList.remove('active');
                });
                
                popupFooter.appendChild(choiceBtn);
            });
            
            // Pas d'auto-fermeture pour les événements à choix
        } else {
            // Événement normal - afficher les effets et bouton continuer
            if (event.effects && Object.keys(event.effects).length > 0) {
                popupEffects.innerHTML = '';
                Object.entries(event.effects).forEach(([resource, amount]) => {
                    const effectDiv = document.createElement('div');
                    effectDiv.className = 'popup-effect';
                    const sign = amount >= 0 ? '+' : '';
                    const amountClass = amount >= 0 ? 'effect-amount positive' : 'effect-amount negative';
                    effectDiv.innerHTML = `<span class="effect-resource">${this.getResourceIcon(resource)} ${this.getResourceName(resource)}</span><span class="${amountClass}">${sign}${amount}</span>`;
                    popupEffects.appendChild(effectDiv);
                });
                popupEffects.style.display = 'block';
            } else {
                popupEffects.style.display = 'none';
            }
            
            // Bouton continuer standard
            popupFooter.innerHTML = '<button class="popup-btn popup-close">Continuer</button>';
            const closeBtn = popupFooter.querySelector('.popup-close');
            closeBtn.addEventListener('click', () => {
                popup.classList.remove('active');
            });
            
            // Auto-fermeture après 5 secondes
            setTimeout(() => {
                if (popup.classList.contains('active')) {
                    popup.classList.remove('active');
                }
            }, 5000);
        }
        
        // Afficher la popup
        popup.classList.add('active');
    }
    
    // Formater les effets d'un choix pour l'affichage
    formatChoiceEffects(effects) {
        if (!effects || Object.keys(effects).length === 0) return '';
        
        const formattedEffects = Object.entries(effects).map(([resource, amount]) => {
            const icon = this.getResourceIcon(resource);
            const sign = amount >= 0 ? '+' : '';
            const className = amount >= 0 ? 'positive' : 'negative';
            return `<span class="${className}">${icon} ${sign}${amount}</span>`;
        });
        
        return formattedEffects.join(' ');
    }
    
    // Gérer le choix du joueur
    handleEventChoice(eventId, choiceId) {
        const result = this.processEventChoice(eventId, choiceId);
        if (result.success) {
            // Notifier le GameManager des changements de ressources
            if (window.gameManager) {
                window.gameManager.notifyResourcesChange();
                window.gameManager.notifyStateChange();
                window.gameManager.autoSave();
            }
            
            console.log(`Choix traité: ${result.choice.text} - ${result.message}`);
        } else {
            console.error('Erreur lors du traitement du choix:', result.message);
        }
    }
    
    // Créer la structure HTML de la popup
    createRandomEventPopup() {
        const popup = document.createElement('div');
        popup.id = 'random-event-popup';
        popup.className = 'random-event-popup';
        popup.innerHTML = `
            <div class="random-event-content">
                <div class="popup-header">
                    <div class="popup-icon"></div>
                    <h3 class="popup-title"></h3>
                </div>
                <div class="popup-body">
                    <p class="popup-description"></p>
                    <div class="popup-effects"></div>
                </div>
                <div class="popup-footer">
                    <button class="popup-btn popup-close">Continuer</button>
                </div>
            </div>
        `;
        
        // Ajouter l'événement de fermeture
        const closeBtn = popup.querySelector('.popup-close');
        closeBtn.addEventListener('click', () => {
            popup.classList.remove('active');
        });
        
        // Fermer en cliquant sur le fond
        popup.addEventListener('click', (e) => {
            if (e.target === popup) {
                popup.classList.remove('active');
            }
        });
        
        return popup;
    }
    
    // Obtenir l'icône d'une ressource
    getResourceIcon(resource) {
        const icons = {
            gold: '💰',
            population: '👥',
            materials: '🔨',
            magic: '✨',
            reputation: '🏛️'
        };
        return icons[resource] || '📦';
    }
    
    // Obtenir le nom d'une ressource
    getResourceName(resource) {
        const names = {
            gold: 'Or',
            population: 'Population',
            materials: 'Matériaux',
            magic: 'Magie',
            reputation: 'Réputation'
        };
        return names[resource] || resource;
    }
    
    // Traiter le choix d'un joueur pour un événement
    processEventChoice(eventId, choiceId) {
        const event = this.events.find(e => e.id === eventId);
        if (!event || !event.requiresChoice || !event.originalEventType) {
            return { success: false, message: 'Événement ou choix invalide' };
        }
        
        const choice = event.originalEventType.choices.find(c => c.id === choiceId);
        if (!choice) {
            return { success: false, message: 'Choix invalide' };
        }
        
        // Appliquer les effets du choix
        if (choice.effects && this.city.resources) {
            Object.entries(choice.effects).forEach(([resource, amount]) => {
                if (this.city.resources[resource] !== undefined) {
                    this.city.resources[resource] += amount;
                }
            });
        }
        
        // Marquer l'événement comme traité
        event.choiceMade = true;
        event.selectedChoice = choice;
        event.isAcknowledged = true;
        
        // Créer un événement de suivi pour indiquer le résultat
        const followUpEvent = this.createEvent(
            'choice_result',
            `Résultat : ${event.title}`,
            choice.description,
            {
                originalEventId: eventId,
                choice: choice,
                effects: choice.effects
            }
        );
        followUpEvent.icon = '✅';
        
        return { 
            success: true, 
            message: 'Choix traité avec succès',
            choice: choice,
            followUpEvent: followUpEvent
        };
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
            nextEventId: this.nextEventId,
            dailyEventCount: this.dailyEventCount,
            scheduledEvents: this.scheduledEvents
        };
    }

    static fromJSON(data, city) {
        const manager = new EventManager(city);
        if (data) {
            manager.events = data.events || [];
            manager.nextEventId = data.nextEventId || 1;
            manager.dailyEventCount = data.dailyEventCount || { min: 4, max: 5 };
            manager.scheduledEvents = data.scheduledEvents || [];
        }
        return manager;
    }
}

if (typeof module !== 'undefined' && module.exports) {
    module.exports = EventManager;
}