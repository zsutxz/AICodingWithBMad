using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

namespace Gomoku.Audio
{
    /// <summary>
    /// Manages audio playback and transitions between different game states
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        private static AudioManager instance;

        [Header("Audio Mixers")]
        [Tooltip("Reference to the main audio mixer")]
        [SerializeField] private AudioMixer mainMixer;

        [Header("Audio Clips")]
        [Tooltip("Background music for the main menu")]
        [SerializeField] private AudioClip mainMenuMusic;

        [Tooltip("Background music for gameplay")]
        [SerializeField] private AudioClip gameplayMusic;

        [Tooltip("Sound effect when the game is won")]
        [SerializeField] private AudioClip victorySound;

        [Tooltip("Sound effect when the game is paused")]
        [SerializeField] private AudioClip pauseSound;

        [Tooltip("Sound effect for UI button clicks")]
        [SerializeField] private AudioClip uiClickSound;

        [Tooltip("Sound effect for piece placement")]
        [SerializeField] private AudioClip piecePlacementSound;

        [Tooltip("Sound effect for invalid piece placement")]
        [SerializeField] private AudioClip invalidPlacementSound;

        [Tooltip("Sound effect for UI button hover")]
        [SerializeField] private AudioClip uiHoverSound;

        [Header("Audio Sources")]
        [Tooltip("Primary audio source for music")]
        [SerializeField] private AudioSource musicSource;

        [Tooltip("Secondary audio source for sound effects")]
        [SerializeField] private AudioSource sfxSource;

        [Header("Mixer Snapshots")]
        [Tooltip("Snapshot for main menu audio settings")]
        [SerializeField] private AudioMixerSnapshot mainMenuSnapshot;

        [Tooltip("Snapshot for gameplay audio settings")]
        [SerializeField] private AudioMixerSnapshot gameplaySnapshot;

        [Tooltip("Snapshot for paused game audio settings")]
        [SerializeField] private AudioMixerSnapshot pausedSnapshot;

        [Tooltip("Snapshot for game over audio settings")]
        [SerializeField] private AudioMixerSnapshot gameOverSnapshot;

        [Header("Transition Settings")]
        [Tooltip("Duration of audio transitions between states in seconds")]
        [SerializeField] private float transitionDuration = 1.0f;
        
        [Header("Volume Settings")]
        [Tooltip("Master volume level (0-1)")]
        [SerializeField] private float masterVolume = 1.0f;
        
        [Tooltip("Music volume level (0-1)")]
        [SerializeField] private float musicVolume = 0.8f;
        
        [Tooltip("SFX volume level (0-1)")]
        [SerializeField] private float sfxVolume = 1.0f;
        
        [Tooltip("UI volume level (0-1)")]
        [SerializeField] private float uiVolume = 0.9f;
        
        [Header("Audio Mixer Groups")]
        [Tooltip("Master mixer group")]
        [SerializeField] private AudioMixerGroup masterGroup;
        
        [Tooltip("Music mixer group")]
        [SerializeField] private AudioMixerGroup musicGroup;
        
        [Tooltip("SFX mixer group")]
        [SerializeField] private AudioMixerGroup sfxGroup;
        
        [Tooltip("UI mixer group")]
        [SerializeField] private AudioMixerGroup uiGroup;
        
        // Volume settings keys for PlayerPrefs
        private const string MASTER_VOLUME_KEY = "MasterVolume";
        private const string MUSIC_VOLUME_KEY = "MusicVolume";
        private const string SFX_VOLUME_KEY = "SFXVolume";
        private const string UI_VOLUME_KEY = "UIVolume";
        
        // Mute states
        private bool masterMuted = false;
        private bool musicMuted = false;
        private bool sfxMuted = false;
        private bool uiMuted = false;
        
        // Performance optimization: object pooling for audio sources
        private Queue<AudioSource> sfxPool;
        private const int SFX_POOL_SIZE = 10;
        private const float MAX_SOUND_DURATION = 1.0f; // Max duration for sound effects as per requirements
        
        // Performance optimization: audio source cleanup coroutine
        private Coroutine cleanupCoroutine;
        

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);

                // Ensure audio sources exist, create them if needed
                EnsureAudioSourcesExist();

                // Set up initial state
                musicSource.loop = true;
                musicSource.playOnAwake = false;
                sfxSource.playOnAwake = false;
            
                // Initialize SFX pool for performance
            InitializeSFXPool();
            
            // Load saved volume settings
            LoadVolumeSettings();
            
            // Apply initial volume settings
            ApplyVolumeSettings();
            
            // Start cleanup coroutine
            cleanupCoroutine = StartCoroutine(AudioSourceCleanup());

            Debug.Log("AudioManager initialized successfully");
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void EnsureAudioSourcesExist()
        {
            // Create music source if not assigned
            if (musicSource == null)
            {
                Debug.Log("AudioManager: Creating music source");
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.name = "MusicSource";
            }

            // Create SFX source if not assigned
            if (sfxSource == null)
            {
                Debug.Log("AudioManager: Creating SFX source");
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.name = "SFXSource";
            }
        }

        public static AudioManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject managerObject = new GameObject("AudioManager");
                    instance = managerObject.AddComponent<AudioManager>();
                    DontDestroyOnLoad(managerObject);
                }
                return instance;
            }
        }

        /// <summary>
        /// Play main menu music with appropriate mixer settings
        /// </summary>
        public void PlayMainMenuMusic()
        {
            if (mainMenuSnapshot != null)
            {
                mainMenuSnapshot.TransitionTo(transitionDuration);
            }

            if (mainMenuMusic != null && musicSource != null)
            {
                musicSource.clip = mainMenuMusic;
                musicSource.Play();
            }

            Debug.Log("Playing main menu music");
        }

        /// <summary>
        /// Play gameplay music with appropriate mixer settings
        /// </summary>
        public void PlayGameplayMusic()
        {
            if (gameplaySnapshot != null)
            {
                gameplaySnapshot.TransitionTo(transitionDuration);
            }

            if (gameplayMusic != null && musicSource != null)
            {
                musicSource.clip = gameplayMusic;
                musicSource.Play();
            }

            Debug.Log("Playing gameplay music");
        }

        /// <summary>
        /// Play victory sound effect (optimized)
        /// </summary>
        public void PlayVictorySound()
        {
            if (victorySound != null)
            {
                PlayOptimizedSFX(victorySound);
            }

            Debug.Log("Playing victory sound");
        }

        /// <summary>
        /// Play pause sound effect (optimized)
        /// </summary>
        public void PlayPauseSound()
        {
            if (pauseSound != null)
            {
                PlayOptimizedSFX(pauseSound);
            }

            Debug.Log("Playing pause sound");
        }

        /// <summary>
        /// Play UI click sound effect (optimized)
        /// </summary>
        public void PlayUIClick()
        {
            if (uiClickSound != null)
            {
                PlayOptimizedSFX(uiClickSound);
            }

            Debug.Log("Playing UI click sound");
        }

        /// <summary>
        /// Play piece placement sound effect (optimized)
        /// </summary>
        public void PlayPiecePlacementSound()
        {
            if (piecePlacementSound != null)
            {
                PlayOptimizedSFX(piecePlacementSound);
            }

            Debug.Log("Playing piece placement sound");
        }

        /// <summary>
        /// Play invalid placement sound effect (optimized)
        /// </summary>
        public void PlayInvalidPlacementSound()
        {
            if (invalidPlacementSound != null)
            {
                PlayOptimizedSFX(invalidPlacementSound);
            }

            Debug.Log("Playing invalid placement sound");
        }

        /// <summary>
        /// Play UI hover sound effect (optimized)
        /// </summary>
        public void PlayUIHoverSound()
        {
            if (uiHoverSound != null)
            {
                PlayOptimizedSFX(uiHoverSound);
            }

            Debug.Log("Playing UI hover sound");
        }

        /// <summary>
        /// Transition to paused state audio settings
        /// </summary>
        public void SetPausedState()
        {
            if (pausedSnapshot != null)
            {
                pausedSnapshot.TransitionTo(transitionDuration);
            }

            Debug.Log("Audio paused state activated");
        }

        /// <summary>
        /// Transition to game over state audio settings
        /// </summary>
        public void SetGameOverState()
        {
            if (gameOverSnapshot != null)
            {
                gameOverSnapshot.TransitionTo(transitionDuration);
            }

            Debug.Log("Audio game over state activated");
        }

        /// <summary>
        /// Stop all audio playback
        /// </summary>
        public void StopAllAudio()
        {
            if (musicSource != null)
            {
                musicSource.Stop();
            }

            Debug.Log("All audio stopped");
        }
        
        /// <summary>
        /// Sets the master volume level
        /// </summary>
        /// <param name="volume">Volume level (0-1)</param>
        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            ApplyVolumeSettings();
            PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, masterVolume);
            PlayerPrefs.Save();
            Debug.Log($"Master volume set to {masterVolume}");
        }
        
        /// <summary>
        /// Sets the music volume level
        /// </summary>
        /// <param name="volume">Volume level (0-1)</param>
        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            ApplyVolumeSettings();
            PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, musicVolume);
            PlayerPrefs.Save();
            Debug.Log($"Music volume set to {musicVolume}");
        }
        
        /// <summary>
        /// Sets the SFX volume level
        /// </summary>
        /// <param name="volume">Volume level (0-1)</param>
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            ApplyVolumeSettings();
            PlayerPrefs.SetFloat(SFX_VOLUME_KEY, sfxVolume);
            PlayerPrefs.Save();
            Debug.Log($"SFX volume set to {sfxVolume}");
        }
        
        /// <summary>
        /// Sets the UI volume level
        /// </summary>
        /// <param name="volume">Volume level (0-1)</param>
        public void SetUIVolume(float volume)
        {
            uiVolume = Mathf.Clamp01(volume);
            ApplyVolumeSettings();
            PlayerPrefs.SetFloat(UI_VOLUME_KEY, uiVolume);
            PlayerPrefs.Save();
            Debug.Log($"UI volume set to {uiVolume}");
        }
        
        /// <summary>
        /// Toggles master mute
        /// </summary>
        public void ToggleMasterMute()
        {
            masterMuted = !masterMuted;
            ApplyVolumeSettings();
            Debug.Log($"Master mute toggled to {masterMuted}");
        }
        
        /// <summary>
        /// Toggles music mute
        /// </summary>
        public void ToggleMusicMute()
        {
            musicMuted = !musicMuted;
            ApplyVolumeSettings();
            Debug.Log($"Music mute toggled to {musicMuted}");
        }
        
        /// <summary>
        /// Toggles SFX mute
        /// </summary>
        public void ToggleSFXMute()
        {
            sfxMuted = !sfxMuted;
            ApplyVolumeSettings();
            Debug.Log($"SFX mute toggled to {sfxMuted}");
        }
        
        /// <summary>
        /// Toggles UI mute
        /// </summary>
        public void ToggleUIMute()
        {
            uiMuted = !uiMuted;
            ApplyVolumeSettings();
            Debug.Log($"UI mute toggled to {uiMuted}");
        }
        
        /// <summary>
        /// Gets current volume levels
        /// </summary>
        /// <returns>Volume levels in order: master, music, sfx, ui</returns>
        public (float master, float music, float sfx, float ui) GetVolumeLevels()
        {
            return (masterVolume, musicVolume, sfxVolume, uiVolume);
        }
        
        /// <summary>
        /// Gets current mute states
        /// </summary>
        /// <returns>Mute states in order: master, music, sfx, ui</returns>
        public (bool master, bool music, bool sfx, bool ui) GetMuteStates()
        {
            return (masterMuted, musicMuted, sfxMuted, uiMuted);
        }
        
        /// <summary>
        /// Loads volume settings from PlayerPrefs
        /// </summary>
        private void LoadVolumeSettings()
        {
            masterVolume = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, masterVolume);
            musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, musicVolume);
            sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, sfxVolume);
            uiVolume = PlayerPrefs.GetFloat(UI_VOLUME_KEY, uiVolume);
            
            Debug.Log("Volume settings loaded from PlayerPrefs");
        }
        
        /// <summary>
        /// Applies current volume settings to audio sources and mixer
        /// </summary>
        private void ApplyVolumeSettings()
        {
            // Apply to audio sources directly if no mixer groups are assigned
            if (mainMixer == null)
            {
                if (musicSource != null)
                {
                    float finalMusicVolume = musicMuted ? 0f : musicVolume * masterVolume;
                    musicSource.volume = finalMusicVolume;
                }
                
                if (sfxSource != null)
                {
                    float finalSFXVolume = sfxMuted ? 0f : sfxVolume * masterVolume;
                    sfxSource.volume = finalSFXVolume;
                }
            }
            else
            {
                // Apply to audio mixer groups
                SetMixerVolume("MasterVolume", masterMuted ? -80f : VolumeToDecibels(masterVolume));
                SetMixerVolume("MusicVolume", musicMuted ? -80f : VolumeToDecibels(musicVolume * masterVolume));
                SetMixerVolume("SFXVolume", sfxMuted ? -80f : VolumeToDecibels(sfxVolume * masterVolume));
                SetMixerVolume("UIVolume", uiMuted ? -80f : VolumeToDecibels(uiVolume * masterVolume));
            }
        }
        
        /// <summary>
        /// Sets mixer group volume
        /// </summary>
        /// <param name="parameterName">Mixer parameter name</param>
        /// <param name="volume">Volume in decibels</param>
        private void SetMixerVolume(string parameterName, float volume)
        {
            if (mainMixer != null)
            {
                mainMixer.SetFloat(parameterName, volume);
            }
        }
        
        /// <summary>
        /// Converts linear volume (0-1) to decibels
        /// </summary>
        /// <param name="linearVolume">Linear volume</param>
        /// <returns>Volume in decibels</returns>
        private float VolumeToDecibels(float linearVolume)
        {
            if (linearVolume <= 0f)
                return -80f;
            
            return 20f * Mathf.Log10(linearVolume);
        }
        
        /// <summary>
        /// Resets all volume settings to defaults
        /// </summary>
        public void ResetVolumeSettings()
        {
            masterVolume = 1.0f;
            musicVolume = 0.8f;
            sfxVolume = 1.0f;
            uiVolume = 0.9f;
            
            masterMuted = false;
            musicMuted = false;
            sfxMuted = false;
            uiMuted = false;
            
            ApplyVolumeSettings();
            
            // Clear PlayerPrefs
            PlayerPrefs.DeleteKey(MASTER_VOLUME_KEY);
            PlayerPrefs.DeleteKey(MUSIC_VOLUME_KEY);
            PlayerPrefs.DeleteKey(SFX_VOLUME_KEY);
            PlayerPrefs.DeleteKey(UI_VOLUME_KEY);
            PlayerPrefs.Save();
            
            Debug.Log("Volume settings reset to defaults");
        }
        
        /// <summary>
        /// Initializes the SFX object pool for performance optimization
        /// </summary>
        private void InitializeSFXPool()
        {
            sfxPool = new Queue<AudioSource>();
            
            for (int i = 0; i < SFX_POOL_SIZE; i++)
            {
                AudioSource sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
                sfxPool.Enqueue(sfxSource);
            }
            
            Debug.Log($"SFX pool initialized with {SFX_POOL_SIZE} audio sources");
        }
        
        /// <summary>
        /// Plays SFX using object pooling for performance optimization
        /// </summary>
        /// <param name="clip">Audio clip to play</param>
        private void PlayOptimizedSFX(AudioClip clip)
        {
            if (clip == null) return;
            
            // Validate clip duration to prevent intrusive sounds
            if (clip.length > MAX_SOUND_DURATION)
            {
                Debug.LogWarning($"Audio clip '{clip.name}' duration ({clip.length}s) exceeds maximum recommended duration ({MAX_SOUND_DURATION}s)");
            }
            
            AudioSource sfxSource = GetPooledSFXSource();
            if (sfxSource != null)
            {
                sfxSource.pitch = 1.0f; // Reset pitch to default
                sfxSource.panStereo = 0.0f; // Reset pan to center
                sfxSource.spatialBlend = 0.0f; // 2D sound
                sfxSource.reverbZoneMix = 0.0f; // No reverb for UI sounds
                sfxSource.PlayOneShot(clip);
            }
        }
        
        /// <summary>
        /// Gets an available SFX audio source from the pool
        /// </summary>
        /// <returns>Available audio source or null if pool is exhausted</returns>
        private AudioSource GetPooledSFXSource()
        {
            if (sfxPool == null || sfxPool.Count == 0)
            {
                Debug.LogWarning("SFX pool exhausted. Consider increasing pool size.");
                return null;
            }
            
            return sfxPool.Dequeue();
        }
        
        /// <summary>
        /// Returns an SFX audio source to the pool
        /// </summary>
        /// <param name="source">Audio source to return</param>
        private void ReturnSFXSourceToPool(AudioSource source)
        {
            if (source != null && sfxPool != null && sfxPool.Count < SFX_POOL_SIZE)
            {
                source.Stop();
                source.clip = null;
                sfxPool.Enqueue(source);
            }
        }
        
        /// <summary>
        /// Cleanup coroutine for managing audio sources and preventing memory leaks
        /// </summary>
        /// <returns>Coroutine enumerator</returns>
        private IEnumerator AudioSourceCleanup()
        {
            while (true)
            {
                yield return new WaitForSeconds(5.0f); // Check every 5 seconds
                
                // Return any stray audio sources to pool
                if (sfxPool != null)
                {
                    AudioSource[] allSources = GetComponentsInChildren<AudioSource>();
                    foreach (AudioSource source in allSources)
                    {
                        if (source != musicSource && source != sfxSource && !source.isPlaying)
                        {
                            ReturnSFXSourceToPool(source);
                        }
                    }
                }
                
                // Optional: Unload unused audio resources
                if (!Application.isPlaying)
                {
                    Resources.UnloadUnusedAssets();
                }
            }
        }
        
        /// <summary>
        /// Gets current audio performance metrics
        /// </summary>
        /// <returns>Performance information tuple</returns>
        public (int activeSources, int pooledSources, float totalMemoryUsage) GetPerformanceMetrics()
        {
            AudioSource[] allSources = GetComponentsInChildren<AudioSource>();
            int activeSources = 0;
            
            foreach (AudioSource source in allSources)
            {
                if (source.isPlaying)
                {
                    activeSources++;
                }
            }
            
            int pooledSources = sfxPool?.Count ?? 0;
            
            // Estimate memory usage (rough calculation)
            float totalMemoryUsage = (allSources.Length * 0.1f) + (pooledSources * 0.1f); // MB
            
            return (activeSources, pooledSources, totalMemoryUsage);
        }
        
        /// <summary>
        /// Preloads frequently used audio clips for performance
        /// </summary>
        public void PreloadAudioClips()
        {
            AudioClip[] clipsToPreload = {
                piecePlacementSound,
                invalidPlacementSound,
                victorySound,
                uiClickSound,
                uiHoverSound,
                pauseSound
            };
            
            int loadedCount = 0;
            foreach (AudioClip clip in clipsToPreload)
            {
                if (clip != null)
                {
                    // Force loading of audio clip data
                    clip.LoadAudioData();
                    loadedCount++;
                }
            }
            
            Debug.Log($"Preloaded {loadedCount} audio clips for performance");
        }
        
        private void OnDestroy()
        {
            // Cleanup coroutine
            if (cleanupCoroutine != null)
            {
                StopCoroutine(cleanupCoroutine);
            }
            
            // Cleanup SFX pool
            if (sfxPool != null)
            {
                while (sfxPool.Count > 0)
                {
                    AudioSource source = sfxPool.Dequeue();
                    if (source != null)
                    {
                        DestroyImmediate(source);
                    }
                }
                sfxPool.Clear();
            }
            
            Debug.Log("AudioManager cleanup completed");
        }
    }
}