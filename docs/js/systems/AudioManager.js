/**
 * Gestionnaire audio pour la musique et les effets sonores
 */
class AudioManager {
    constructor() {
        this.currentMusic = null;
        this.musicVolume = 0.15;
        this.soundVolume = 0.8;
        this.isMusicMuted = false;
        this.isSoundMuted = false;
        this.activeIntervals = new Map(); // Pour gérer les fade en cours
        
        // Initialiser les fichiers audio
        this.music = {
            title: new Audio('sounds/title.mp3'),
            game: new Audio('sounds/game2.mp3')
        };
        
        // Initialiser les effets sonores
        this.sounds = {
            hammer: new Audio('sounds/hammer.mp3'),
            clic: new Audio('sounds/clic.mp3'),
            pop: new Audio('sounds/pop.mp3'),
            interface_button: new Audio('sounds/interface_button.mp3'),
            coin: new Audio('sounds/coin.mp3'),
            angry: new Audio('sounds/angry.mp3'),
            crowd: new Audio('sounds/crowd.mp3')
        };
        
        // Configuration des musiques en boucle
        Object.values(this.music).forEach(audio => {
            audio.loop = true;
            audio.volume = this.musicVolume;
        });
        
        // Configuration des effets sonores
        Object.values(this.sounds).forEach(audio => {
            audio.loop = false;
            audio.volume = this.soundVolume;
        });
    }
    
    /**
     * Jouer une musique
     */
    playMusic(musicName, fadeIn = true) {
        console.log(`Tentative de lecture de la musique: ${musicName}`);
        
        if (this.isMusicMuted) {
            console.log('Musique muette, pas de lecture');
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
                    this.fadeIn(this.currentMusic, this.isMusicMuted ? 0 : this.musicVolume, 500); // Fade in plus rapide (500ms)
                } else {
                    this.currentMusic.volume = this.isMusicMuted ? 0 : this.musicVolume;
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
            if (this.isMusicMuted) {
                console.log('Musique en pause car désactivée');
                return;
            }
            
            const playPromise = this.currentMusic.play();
            if (playPromise !== undefined) {
                playPromise.then(() => {
                    console.log('Musique reprise');
                    // S'assurer que le volume est correct
                    this.currentMusic.volume = this.isMusicMuted ? 0 : this.musicVolume;
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
        if (this.currentMusic && !this.isMusicMuted) {
            this.currentMusic.volume = this.musicVolume;
        }
    }
    
    /**
     * Définir le volume des effets sonores
     */
    setSoundVolume(volume) {
        this.soundVolume = Math.max(0, Math.min(1, volume));
        Object.values(this.sounds).forEach(audio => {
            if (!this.isSoundMuted) {
                audio.volume = this.soundVolume;
            }
        });
    }
    
    /**
     * Activer/désactiver la musique (compatibilité)
     */
    toggleMute() {
        return this.toggleMusicMute();
    }
    
    /**
     * Activer/désactiver la musique
     */
    toggleMusicMute() {
        this.isMusicMuted = !this.isMusicMuted;
        
        if (this.isMusicMuted) {
            if (this.currentMusic) {
                this.currentMusic.volume = 0;
            }
        } else {
            if (this.currentMusic) {
                this.currentMusic.volume = this.musicVolume;
            }
        }
        
        return this.isMusicMuted;
    }
    
    /**
     * Activer/désactiver les effets sonores
     */
    toggleSoundMute() {
        this.isSoundMuted = !this.isSoundMuted;
        
        Object.values(this.sounds).forEach(audio => {
            if (this.isSoundMuted) {
                audio.volume = 0;
            } else {
                audio.volume = this.soundVolume;
            }
        });
        
        return this.isSoundMuted;
    }
    
    /**
     * Jouer un effet sonore
     */
    playSound(soundName) {
        if (this.isSoundMuted) {
            console.log(`Son ${soundName} muet, pas de lecture`);
            return;
        }
        
        const sound = this.sounds[soundName];
        if (!sound) {
            console.warn(`Son "${soundName}" introuvable`);
            return;
        }
        
        // Remettre le son au début et le jouer
        sound.currentTime = 0;
        sound.volume = this.soundVolume;
        
        const playPromise = sound.play();
        if (playPromise !== undefined) {
            playPromise.then(() => {
                //console.log(`Son ${soundName} joué avec succès`);
            }).catch(error => {
                console.error(`Erreur lors de la lecture du son ${soundName}:`, error);
            });
        }
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
            musicVolume: this.musicVolume,
            soundVolume: this.soundVolume,
            isMusicMuted: this.isMusicMuted,
            isSoundMuted: this.isSoundMuted,
            // Compatibilité avec l'ancien code
            volume: this.musicVolume,
            isMuted: this.isMusicMuted,
            isPlaying: this.currentMusic && !this.currentMusic.paused,
            isPaused: this.currentMusic && this.currentMusic.paused
        };
    }
}

// Export pour utilisation dans d'autres fichiers
if (typeof module !== 'undefined' && module.exports) {
    module.exports = AudioManager;
}
