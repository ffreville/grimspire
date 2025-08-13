/**
 * Classe Events - Représente un événement aléatoire du jeu
 */
class Events {
    constructor(id, name, description, type, icon, weight, sound, effects = {}, messages = [], requiresChoice = false, choices = []) {
        this.id = id;
        this.name = name;
        this.description = description;
        this.type = type;
        this.icon = icon;
        this.weight = weight;
        this.sound = sound;
        this.effects = effects;
        this.messages = messages;
        this.requiresChoice = requiresChoice;
        this.choices = choices;
    }

}

if (typeof module !== 'undefined' && module.exports) {
    module.exports = Events;
}
