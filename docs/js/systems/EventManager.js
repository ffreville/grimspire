/**
 * Classe EventManager - Gestionnaire des événements de la ville
 */
class EventManager {
    constructor(city) {
        this.city = city;
        this.events = [];
        this.nextEventId = 1;
        this.randomEventTypes = [];
        this.scheduledEvents = []; // Événements programmés pour être générés dans les heures suivantes
        this.isInitialized = false;
        
        // Initialiser les types d'événements 
        this.randomEventTypes = this.initializeRandomEventTypes();
        this.isInitialized = true;
        
        // Générer les événements pour le jour actuel
        this.generateDailyRandomEvents();
    }

    // Initialiser les types d'événements aléatoires
    initializeRandomEventTypes() {
        try {
            // Utiliser les données d'événements depuis la variable globale
            if (!window.EVENTS_DATA) {
                throw new Error('EVENTS_DATA non disponible');
            }
            
            return window.EVENTS_DATA.map(eventData => {
                // Convertir les effets avec les constantes ResourceTypes
                const effects = {};
                if (eventData.effects) {
                    Object.entries(eventData.effects).forEach(([resource, amount]) => {
                        // Mapper les noms de ressources vers les constantes
                        const resourceKey = this.mapResourceName(resource);
                        effects[resourceKey] = amount;
                    });
                }
                
                // Traiter les choix si ils existent
                let choices = eventData.choices || [];
                if (choices.length > 0) {
                    choices = choices.map(choice => ({
                        ...choice,
                        effects: this.convertEffects(choice.effects || {})
                    }));
                }
                
                return new Events(
                    eventData.id,
                    eventData.name,
                    eventData.description,
                    eventData.type,
                    eventData.icon,
                    eventData.weight,
                    eventData.sound,
                    effects,
                    eventData.messages,
                    eventData.requiresChoice || false,
                    choices
                );
            });
        } catch (error) {
            console.error('Erreur lors du chargement des événements:', error);
            // Retourner un tableau vide en cas d'erreur pour éviter de casser le jeu
            return [];
        }
    }
    
    // Mapper les noms de ressources du JSON vers les constantes ResourceTypes
    mapResourceName(resourceName) {
        const mapping = {
            'gold': ResourceTypes.GOLD,
            'population': ResourceTypes.POPULATION,
            'materials': ResourceTypes.MATERIALS,
            'magic': ResourceTypes.MAGIC,
            'reputation': ResourceTypes.REPUTATION
        };
        return mapping[resourceName] || resourceName;
    }
    
    // Convertir les effets d'un objet JSON vers les constantes ResourceTypes
    convertEffects(effects) {
        const converted = {};
        Object.entries(effects).forEach(([resource, amount]) => {
            const resourceKey = this.mapResourceName(resource);
            converted[resourceKey] = amount;
        });
        return converted;
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
                event.isAcknowledged = true;
                count++;
            }
        });
        return count;
    }


    // Effacer les événements acquittés
    clearAcknowledgedEvents() {
        const initialCount = this.events.length;
        this.events = this.events.filter(event => !event.isAcknowledged);
        return initialCount - this.events.length;
    }

    // Obtenir les statistiques des événements
    getEventStats() {
        const unacknowledgedCount = this.events.filter(e => !e.isAcknowledged).length;
        const totalCount = this.events.length;
        const lastActivity = this.events.length > 0 ? this.events[0].formattedTime : '-';

        return {
            unacknowledgedCount,
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
        
        // Vider les événements programmés précédents
        this.scheduledEvents = [];
        
        // Programmer un événement toutes les heures à heure pile (de 1h à 23h)
        for (let hour = 1; hour < 24; hour++) {
            const randomEvent = this.selectRandomEvent();
            if (randomEvent) {
                this.scheduledEvents.push({
                    eventType: randomEvent,
                    scheduledTime: hour * 60, // Heure pile en minutes depuis minuit (1:00 = 60, 2:00 = 120, etc.)
                    day: this.city.day
                });
            }
        }
        
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
        // Jouer le son associé à l'événement si disponible
        if (event.originalEventType && event.originalEventType.sound && window.app && window.app.audioManager) {
            window.app.audioManager.playSound(event.originalEventType.sound);
        }
        
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
            
            // Auto-fermeture après 30 secondes pour les événements à choix
            setTimeout(() => {
                if (popup.classList.contains('active')) {
                    popup.classList.remove('active');
                }
            }, 30000);
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
                // Acquitter l'événement quand on clique sur Continuer
                this.acknowledgeEvent(event.id);
                popup.classList.remove('active');
            });
            
            // Auto-fermeture après 30 secondes
            setTimeout(() => {
                if (popup.classList.contains('active')) {
                    // Acquitter l'événement lors de l'auto-fermeture
                    this.acknowledgeEvent(event.id);
                    popup.classList.remove('active');
                }
            }, 30000);
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
            
            // Mettre à jour l'affichage des événements
            if (window.app && window.app.renderEvents) {
                window.app.renderEvents();
            }
            
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
        return ResourceTypes.getIcon(resource);
    }
    
    // Obtenir le nom d'une ressource
    getResourceName(resource) {
        return ResourceTypes.getName(resource);
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
        
        // Créer un événement de suivi pour indiquer le résultat (déjà acquitté)
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
        followUpEvent.isAcknowledged = true; // Événement de résultat déjà acquitté
        
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
            scheduledEvents: this.scheduledEvents
        };
    }

    static fromJSON(data, city) {
        const manager = new EventManager(city);
        if (data) {
            manager.events = data.events || [];
            manager.nextEventId = data.nextEventId || 1;
            manager.scheduledEvents = data.scheduledEvents || [];
        }
        return manager;
    }
}

if (typeof module !== 'undefined' && module.exports) {
    module.exports = EventManager;
}
