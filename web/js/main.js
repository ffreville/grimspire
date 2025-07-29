/**
 * Grimspire - Menu Principal
 * Phase 1.1 - Menu principal pour gérer le lancement d'une nouvelle partie
 */

class MainMenu {
    constructor() {
        this.initializeEventListeners();
    }

    initializeEventListeners() {
        // Bouton Nouvelle Partie
        const newGameBtn = document.getElementById('new-game-btn');
        if (newGameBtn) {
            newGameBtn.addEventListener('click', this.startNewGame.bind(this));
        }

        // Boutons désactivés pour les phases futures
        const loadGameBtn = document.getElementById('load-game-btn');
        const settingsBtn = document.getElementById('settings-btn');
        const quitBtn = document.getElementById('quit-btn');

        if (loadGameBtn) {
            loadGameBtn.addEventListener('click', this.showNotImplemented.bind(this, 'Charger Partie'));
        }
        
        if (settingsBtn) {
            settingsBtn.addEventListener('click', this.showNotImplemented.bind(this, 'Options'));
        }
        
        if (quitBtn) {
            quitBtn.addEventListener('click', this.showNotImplemented.bind(this, 'Quitter'));
        }
    }

    startNewGame() {
        console.log('Lancement d\'une nouvelle partie...');
        
        // Animation de transition
        this.fadeTransition(() => {
            this.switchToGameScreen();
        });
    }

    switchToGameScreen() {
        const mainMenu = document.getElementById('main-menu');
        const gameScreen = document.getElementById('game-screen');
        
        if (mainMenu && gameScreen) {
            mainMenu.classList.remove('active');
            gameScreen.classList.add('active');
            
            // Message temporaire pour la phase 1.1
            gameScreen.innerHTML = `
                <div style="text-align: center; padding: 40px;">
                    <h2 style="color: #d4af37; margin-bottom: 20px;">Bienvenue dans Grimspire!</h2>
                    <p style="font-size: 1.1rem; margin-bottom: 15px;">
                        Interface de jeu en cours de développement.
                    </p>
                    <p style="color: #999;">
                        Phase 1.1 terminée - Menu principal fonctionnel
                    </p>
                    <button onclick="mainMenuInstance.returnToMainMenu()" 
                            style="margin-top: 30px; padding: 10px 20px; 
                                   background: #8b5a3c; color: white; 
                                   border: none; border-radius: 5px; cursor: pointer;">
                        Retour au Menu Principal
                    </button>
                </div>
            `;
        }
    }

    returnToMainMenu() {
        const mainMenu = document.getElementById('main-menu');
        const gameScreen = document.getElementById('game-screen');
        
        if (mainMenu && gameScreen) {
            gameScreen.classList.remove('active');
            mainMenu.classList.add('active');
        }
    }

    fadeTransition(callback) {
        const app = document.getElementById('app');
        
        // Effet de fondu
        app.style.transition = 'opacity 0.3s ease-in-out';
        app.style.opacity = '0';
        
        setTimeout(() => {
            callback();
            app.style.opacity = '1';
        }, 300);
    }

    showNotImplemented(featureName) {
        console.log(`${featureName} - Fonctionnalité non implémentée`);
        
        // Message temporaire pour les fonctionnalités futures
        const message = document.createElement('div');
        message.style.cssText = `
            position: fixed;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
            background: rgba(0, 0, 0, 0.9);
            color: #e8e6e3;
            padding: 20px 30px;
            border-radius: 10px;
            border: 2px solid #8b5a3c;
            z-index: 1000;
            text-align: center;
        `;
        
        message.innerHTML = `
            <h3 style="color: #d4af37; margin-bottom: 10px;">${featureName}</h3>
            <p>Cette fonctionnalité sera implémentée dans les phases suivantes.</p>
            <button onclick="this.parentElement.remove()" 
                    style="margin-top: 15px; padding: 8px 16px; 
                           background: #8b5a3c; color: white; 
                           border: none; border-radius: 5px; cursor: pointer;">
                OK
            </button>
        `;
        
        document.body.appendChild(message);
        
        // Auto-suppression après 3 secondes
        setTimeout(() => {
            if (message.parentElement) {
                message.remove();
            }
        }, 3000);
    }
}

// Initialisation de l'application
let mainMenuInstance;

document.addEventListener('DOMContentLoaded', () => {
    console.log('Grimspire - Phase 1.1 initialisée');
    mainMenuInstance = new MainMenu();
});

// Gestion des erreurs globales
window.addEventListener('error', (event) => {
    console.error('Erreur JavaScript:', event.error);
});

// Export pour utilisation dans d'autres modules (phases futures)
if (typeof module !== 'undefined' && module.exports) {
    module.exports = { MainMenu };
}