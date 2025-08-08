/**
 * Gestionnaire audio pour la musique et les effets sonores
 */
class AudioManager {
    constructor() {
        this.currentMusic = null;
        this.musicVolume = 0.5;
        this.isMuted = false;
        this.activeIntervals = new Map(); // Pour gérer les fade en cours
        
        // Initialiser les fichiers audio
        this.music = {
            title: new Audio('sounds/title.mp3'),
            game: new Audio('sounds/game.mp3')
        };
        
        // Configuration des musiques en boucle
        Object.values(this.music).forEach(audio => {
            audio.loop = true;
            audio.volume = this.musicVolume;
        });
    }
    
    /**
     * Jouer une musique
     */
    playMusic(musicName, fadeIn = true) {
        console.log(`Tentative de lecture de la musique: ${musicName}`);
        
        if (this.isMuted) {
            console.log('Audio muet, pas de lecture');
            return;
        }
        
        const newMusic = this.music[musicName];
        if (!newMusic) {
            console.warn(`Musique "${musicName}" introuvable`);
            return;
        }
        
        // Si c'est déjà la musique en cours, ne rien faire
        if (this.currentMusic === newMusic && !this.currentMusic.paused) {
            console.log('Musique déjà en cours de lecture');
            return;
        }
        
        // Sauvegarder la référence à l'ancienne musique pour le fade out
        const oldMusic = this.currentMusic;
        
        // Démarrer la nouvelle musique immédiatement
        this.currentMusic = newMusic;
        
        const playPromise = this.currentMusic.play();
        
        if (playPromise !== undefined) {
            playPromise.then(() => {
                console.log(`Musique ${musicName} démarrée avec succès`);
                if (fadeIn) {
                    this.currentMusic.volume = 0;
                    this.fadeIn(this.currentMusic, this.musicVolume, 500); // Fade in plus rapide (500ms)
                } else {
                    this.currentMusic.volume = this.musicVolume;
                }
                
                // Faire le fade out de l'ancienne musique APRÈS avoir démarré la nouvelle
                if (oldMusic && oldMusic !== this.currentMusic && !oldMusic.paused) {
                    this.fadeOut(oldMusic, () => {
                        oldMusic.pause();
                        oldMusic.currentTime = 0;
                    }, 500); // Fade out plus rapide (500ms)
                }
            }).catch(error => {
                console.error(`Erreur lors de la lecture de ${musicName}:`, error);
                
                // En cas d'erreur, remettre l'ancienne musique comme musique actuelle
                this.currentMusic = oldMusic;
                
                // Essayer de diagnostiquer le problème
                if (error.name === 'NotAllowedError') {
                    console.warn('Autoplay bloqué par le navigateur. Interaction utilisateur requise.');
                } else if (error.name === 'NotSupportedError') {
                    console.warn('Format audio non supporté');
                } else {
                    console.warn('Erreur audio inconnue:', error);
                }
            });
        }
    }
    
    /**
     * Arrêter la musique
     */
    stopMusic(fadeOut = true) {
        if (!this.currentMusic) return;
        
        if (fadeOut) {
            this.fadeOut(this.currentMusic, () => {
                this.currentMusic.pause();
                this.currentMusic.currentTime = 0;
                this.currentMusic = null;
            });
        } else {
            this.currentMusic.pause();
            this.currentMusic.currentTime = 0;
            this.currentMusic = null;
        }
    }

    /**
     * Mettre en pause la musique
     */
    pauseMusic() {
        if (this.currentMusic && !this.currentMusic.paused) {
            this.currentMusic.pause();
            console.log('Musique mise en pause');
        }
    }

    /**
     * Reprendre la musique
     */
    resumeMusic() {
        if (this.currentMusic && this.currentMusic.paused) {
            if (this.isMuted) {
                console.log('Musique en pause car désactivée');
                return;
            }
            
            const playPromise = this.currentMusic.play();
            if (playPromise !== undefined) {
                playPromise.then(() => {
                    console.log('Musique reprise');
                    // S'assurer que le volume est correct
                    this.currentMusic.volume = this.musicVolume;
                }).catch(error => {
                    console.warn('Impossible de reprendre la musique:', error);
                });
            }
        }
    }
    
    /**
     * Définir le volume de la musique
     */
    setMusicVolume(volume) {
        this.musicVolume = Math.max(0, Math.min(1, volume));
        if (this.currentMusic && !this.isMuted) {
            this.currentMusic.volume = this.musicVolume;
        }
    }
    
    /**
     * Activer/désactiver le son
     */
    toggleMute() {
        this.isMuted = !this.isMuted;
        
        if (this.isMuted) {
            if (this.currentMusic) {
                this.currentMusic.volume = 0;
            }
        } else {
            if (this.currentMusic) {
                this.currentMusic.volume = this.musicVolume;
            }
        }
        
        return this.isMuted;
    }
    
    /**
     * Fade in progressif
     */
    fadeIn(audio, targetVolume, duration = 1000) {
        // Arrêter tout fade en cours sur cet audio
        this.clearFade(audio);
        
        const steps = 20;
        const stepVolume = targetVolume / steps;
        const stepTime = duration / steps;
        
        let currentStep = 0;
        const fadeInterval = setInterval(() => {
            currentStep++;
            if (audio.paused) {
                clearInterval(fadeInterval);
                this.activeIntervals.delete(audio);
                return;
            }
            
            audio.volume = Math.min(stepVolume * currentStep, targetVolume);
            
            if (currentStep >= steps) {
                clearInterval(fadeInterval);
                this.activeIntervals.delete(audio);
                audio.volume = targetVolume;
            }
        }, stepTime);
        
        this.activeIntervals.set(audio, fadeInterval);
    }
    
    /**
     * Fade out progressif
     */
    fadeOut(audio, callback = null, duration = 1000) {
        // Arrêter tout fade en cours sur cet audio
        this.clearFade(audio);
        
        const steps = 20;
        const initialVolume = audio.volume;
        const stepVolume = initialVolume / steps;
        const stepTime = duration / steps;
        
        let currentStep = 0;
        const fadeInterval = setInterval(() => {
            currentStep++;
            if (audio.paused) {
                clearInterval(fadeInterval);
                this.activeIntervals.delete(audio);
                if (callback) callback();
                return;
            }
            
            audio.volume = Math.max(initialVolume - (stepVolume * currentStep), 0);
            
            if (currentStep >= steps || audio.volume <= 0) {
                clearInterval(fadeInterval);
                this.activeIntervals.delete(audio);
                audio.volume = 0;
                if (callback) callback();
            }
        }, stepTime);
        
        this.activeIntervals.set(audio, fadeInterval);
    }
    
    /**
     * Arrêter les fades en cours pour un audio
     */
    clearFade(audio) {
        if (this.activeIntervals.has(audio)) {
            clearInterval(this.activeIntervals.get(audio));
            this.activeIntervals.delete(audio);
        }
    }
    
    /**
     * Obtenir l'état actuel de l'audio
     */
    getStatus() {
        return {
            currentMusic: this.currentMusic ? 
                (this.currentMusic === this.music.title ? 'title' : 'game') : 'none',
            volume: this.musicVolume,
            isMuted: this.isMuted,
            isPlaying: this.currentMusic && !this.currentMusic.paused,
            isPaused: this.currentMusic && this.currentMusic.paused
        };
    }
}

// Export pour utilisation dans d'autres fichiers
if (typeof module !== 'undefined' && module.exports) {
    module.exports = AudioManager;
}