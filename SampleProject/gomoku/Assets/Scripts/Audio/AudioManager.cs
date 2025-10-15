using UnityEngine;
using UnityEngine.Audio;

namespace Gomoku.Audio
{
    /// <summary>
    /// Manages audio playback and transitions between different game states
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
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

        private void Awake()
        {
            // Ensure this manager persists across scene changes
            DontDestroyOnLoad(gameObject);

            // Validate references
            if (musicSource == null || sfxSource == null)
            {
                Debug.LogError("AudioManager: Audio sources not assigned!");
                return;
            }

            // Set up initial state
            musicSource.loop = true;
            musicSource.playOnAwake = false;
            sfxSource.playOnAwake = false;

            Debug.Log("AudioManager initialized successfully");
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
        /// Play victory sound effect
        /// </summary>
        public void PlayVictorySound()
        {
            if (victorySound != null && sfxSource != null)
            {
                sfxSource.PlayOneShot(victorySound);
            }

            Debug.Log("Playing victory sound");
        }

        /// <summary>
        /// Play pause sound effect
        /// </summary>
        public void PlayPauseSound()
        {
            if (pauseSound != null && sfxSource != null)
            {
                sfxSource.PlayOneShot(pauseSound);
            }

            Debug.Log("Playing pause sound");
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
    }
}