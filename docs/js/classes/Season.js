/**
 * Classe Season - Gestionnaire des saisons dans Grimspire
 */
class Season {
    constructor() {
        this.seasons = [
            { id: 'spring', name: 'Printemps', icon: '🌸', color: '#2ECC71' },
            { id: 'summer', name: 'Été', icon: '☀️', color: '#F39C12' },
            { id: 'autumn', name: 'Automne', icon: '🍂', color: '#D35400' },
            { id: 'winter', name: 'Hiver', icon: '❄️', color: '#3498DB' }
        ];
        
        this.daysPerSeason = 10;
    }

    /**
     * Calcule la saison actuelle basée sur le jour
     * @param {number} day - Jour actuel (commence à 1)
     * @returns {Object} Informations sur la saison actuelle
     */
    getCurrentSeason(day) {
        // Calculer l'index de la saison (0-3)
        const seasonIndex = Math.floor((day - 1) / this.daysPerSeason) % this.seasons.length;
        const currentSeason = this.seasons[seasonIndex];
        
        // Calculer le jour dans la saison actuelle (1-10)
        const dayInSeason = ((day - 1) % this.daysPerSeason) + 1;
        
        // Calculer l'année actuelle (commence à 1)
        const year = Math.floor((day - 1) / (this.seasons.length * this.daysPerSeason)) + 1;
        
        return {
            season: currentSeason,
            dayInSeason: dayInSeason,
            daysLeftInSeason: this.daysPerSeason - dayInSeason,
            year: year,
            totalDays: day
        };
    }

    /**
     * Obtient les informations d'affichage de la saison
     * @param {number} day - Jour actuel
     * @returns {Object} Informations formatées pour l'affichage
     */
    getSeasonDisplay(day) {
        const seasonInfo = this.getCurrentSeason(day);
        
        return {
            icon: seasonInfo.season.icon,
            name: seasonInfo.season.name,
            color: seasonInfo.season.color,
            dayInSeason: seasonInfo.dayInSeason,
            daysPerSeason: this.daysPerSeason,
            year: seasonInfo.year,
            formattedText: `${seasonInfo.season.name} - Jour ${seasonInfo.dayInSeason}/${this.daysPerSeason}`,
            progressPercent: (seasonInfo.dayInSeason / this.daysPerSeason) * 100
        };
    }

    /**
     * Vérifie si on vient de changer de saison
     * @param {number} previousDay - Jour précédent
     * @param {number} currentDay - Jour actuel
     * @returns {boolean} True si on vient de changer de saison
     */
    hasSeasonChanged(previousDay, currentDay) {
        if (previousDay >= currentDay) return false;
        
        const previousSeason = this.getCurrentSeason(previousDay);
        const currentSeason = this.getCurrentSeason(currentDay);
        
        return previousSeason.season.id !== currentSeason.season.id;
    }

    /**
     * Obtient la prochaine saison
     * @param {number} day - Jour actuel
     * @returns {Object} Informations sur la prochaine saison
     */
    getNextSeason(day) {
        const currentSeasonInfo = this.getCurrentSeason(day);
        const nextSeasonIndex = (this.seasons.findIndex(s => s.id === currentSeasonInfo.season.id) + 1) % this.seasons.length;
        
        return {
            season: this.seasons[nextSeasonIndex],
            daysUntilNext: currentSeasonInfo.daysLeftInSeason
        };
    }

    /**
     * Obtient toutes les saisons disponibles
     * @returns {Array} Liste de toutes les saisons
     */
    getAllSeasons() {
        return [...this.seasons];
    }

    /**
     * Sérialise les données de saison pour la sauvegarde
     * @returns {Object} Données sérialisées
     */
    toJSON() {
        return {
            daysPerSeason: this.daysPerSeason
        };
    }

    /**
     * Restaure les données de saison depuis une sauvegarde
     * @param {Object} data - Données à restaurer
     * @returns {Season} Instance restaurée
     */
    static fromJSON(data) {
        const season = new Season();
        if (data.daysPerSeason) {
            season.daysPerSeason = data.daysPerSeason;
        }
        return season;
    }
}

if (typeof module !== 'undefined' && module.exports) {
    module.exports = Season;
}