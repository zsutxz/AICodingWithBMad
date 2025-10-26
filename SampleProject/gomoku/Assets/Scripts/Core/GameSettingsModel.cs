using UnityEngine;
using System;

namespace Gomoku.Core
{
    /// <summary>
    /// Game settings model that handles persistence of game preferences
    /// </summary>
    [CreateAssetMenu(fileName = "GameSettingsModel", menuName = "Gomoku/Game Settings Model")]
    public class GameSettingsModel : ScriptableObject
    {
        [Header("Audio Settings")]
        [Range(0f, 1f)]
        [SerializeField] private float masterVolume = 1.0f;
        
        [Range(0f, 1f)]
        [SerializeField] private float musicVolume = 0.8f;
        
        [Range(0f, 1f)]
        [SerializeField] private float sfxVolume = 1.0f;
        
        [Range(0f, 1f)]
        [SerializeField] private float uiVolume = 0.9f;
        
        [SerializeField] private bool masterMuted = false;
        [SerializeField] private bool musicMuted = false;
        [SerializeField] private bool sfxMuted = false;
        [SerializeField] private bool uiMuted = false;
        
        [Header("Game Settings")]
        [SerializeField] private bool showHints = true;
        [SerializeField] private bool showAnimations = true;
        [SerializeField] private bool showLastMove = true;
        [SerializeField] private float gameSpeed = 1.0f;
        
        // PlayerPrefs keys
        private const string MASTER_VOLUME_KEY = "GameSettings_MasterVolume";
        private const string MUSIC_VOLUME_KEY = "GameSettings_MusicVolume";
        private const string SFX_VOLUME_KEY = "GameSettings_SFXVolume";
        private const string UI_VOLUME_KEY = "GameSettings_UIVolume";
        private const string MASTER_MUTED_KEY = "GameSettings_MasterMuted";
        private const string MUSIC_MUTED_KEY = "GameSettings_MusicMuted";
        private const string SFX_MUTED_KEY = "GameSettings_SFXMuted";
        private const string UI_MUTED_KEY = "GameSettings_UIMuted";
        private const string SHOW_HINTS_KEY = "GameSettings_ShowHints";
        private const string SHOW_ANIMATIONS_KEY = "GameSettings_ShowAnimations";
        private const string SHOW_LAST_MOVE_KEY = "GameSettings_ShowLastMove";
        private const string GAME_SPEED_KEY = "GameSettings_GameSpeed";
        
        // Events for settings changes
        public event Action<float> OnMasterVolumeChanged;
        public event Action<float> OnMusicVolumeChanged;
        public event Action<float> OnSFXVolumeChanged;
        public event Action<float> OnUIVolumeChanged;
        public event Action<bool> OnMasterMutedChanged;
        public event Action<bool> OnMusicMutedChanged;
        public event Action<bool> OnSFXMutedChanged;
        public event Action<bool> OnUIMutedChanged;
        public event Action<bool> OnShowHintsChanged;
        public event Action<bool> OnShowAnimationsChanged;
        public event Action<bool> OnShowLastMoveChanged;
        public event Action<float> OnGameSpeedChanged;
        
        // Properties
        public float MasterVolume
        {
            get => masterVolume;
            set
            {
                masterVolume = Mathf.Clamp01(value);
                PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, masterVolume);
                OnMasterVolumeChanged?.Invoke(masterVolume);
            }
        }
        
        public float MusicVolume
        {
            get => musicVolume;
            set
            {
                musicVolume = Mathf.Clamp01(value);
                PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, musicVolume);
                OnMusicVolumeChanged?.Invoke(musicVolume);
            }
        }
        
        public float SFXVolume
        {
            get => sfxVolume;
            set
            {
                sfxVolume = Mathf.Clamp01(value);
                PlayerPrefs.SetFloat(SFX_VOLUME_KEY, sfxVolume);
                OnSFXVolumeChanged?.Invoke(sfxVolume);
            }
        }
        
        public float UIVolume
        {
            get => uiVolume;
            set
            {
                uiVolume = Mathf.Clamp01(value);
                PlayerPrefs.SetFloat(UI_VOLUME_KEY, uiVolume);
                OnUIVolumeChanged?.Invoke(uiVolume);
            }
        }
        
        public bool MasterMuted
        {
            get => masterMuted;
            set
            {
                masterMuted = value;
                PlayerPrefs.SetInt(MASTER_MUTED_KEY, masterMuted ? 1 : 0);
                OnMasterMutedChanged?.Invoke(masterMuted);
            }
        }
        
        public bool MusicMuted
        {
            get => musicMuted;
            set
            {
                musicMuted = value;
                PlayerPrefs.SetInt(MUSIC_MUTED_KEY, musicMuted ? 1 : 0);
                OnMusicMutedChanged?.Invoke(musicMuted);
            }
        }
        
        public bool SFXMuted
        {
            get => sfxMuted;
            set
            {
                sfxMuted = value;
                PlayerPrefs.SetInt(SFX_MUTED_KEY, sfxMuted ? 1 : 0);
                OnSFXMutedChanged?.Invoke(sfxMuted);
            }
        }
        
        public bool UIMuted
        {
            get => uiMuted;
            set
            {
                uiMuted = value;
                PlayerPrefs.SetInt(UI_MUTED_KEY, uiMuted ? 1 : 0);
                OnUIMutedChanged?.Invoke(uiMuted);
            }
        }
        
        public bool ShowHints
        {
            get => showHints;
            set
            {
                showHints = value;
                PlayerPrefs.SetInt(SHOW_HINTS_KEY, showHints ? 1 : 0);
                OnShowHintsChanged?.Invoke(showHints);
            }
        }
        
        public bool ShowAnimations
        {
            get => showAnimations;
            set
            {
                showAnimations = value;
                PlayerPrefs.SetInt(SHOW_ANIMATIONS_KEY, showAnimations ? 1 : 0);
                OnShowAnimationsChanged?.Invoke(showAnimations);
            }
        }
        
        public bool ShowLastMove
        {
            get => showLastMove;
            set
            {
                showLastMove = value;
                PlayerPrefs.SetInt(SHOW_LAST_MOVE_KEY, showLastMove ? 1 : 0);
                OnShowLastMoveChanged?.Invoke(showLastMove);
            }
        }
        
        public float GameSpeed
        {
            get => gameSpeed;
            set
            {
                gameSpeed = Mathf.Clamp(value, 0.5f, 2.0f);
                PlayerPrefs.SetFloat(GAME_SPEED_KEY, gameSpeed);
                OnGameSpeedChanged?.Invoke(gameSpeed);
            }
        }
        
        private void OnEnable()
        {
            LoadSettings();
        }
        
        /// <summary>
        /// Loads settings from PlayerPrefs
        /// </summary>
        public void LoadSettings()
        {
            // Load audio settings
            masterVolume = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, masterVolume);
            musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, musicVolume);
            sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, sfxVolume);
            uiVolume = PlayerPrefs.GetFloat(UI_VOLUME_KEY, uiVolume);
            
            masterMuted = PlayerPrefs.GetInt(MASTER_MUTED_KEY, masterMuted ? 1 : 0) == 1;
            musicMuted = PlayerPrefs.GetInt(MUSIC_MUTED_KEY, musicMuted ? 1 : 0) == 1;
            sfxMuted = PlayerPrefs.GetInt(SFX_MUTED_KEY, sfxMuted ? 1 : 0) == 1;
            uiMuted = PlayerPrefs.GetInt(UI_MUTED_KEY, uiMuted ? 1 : 0) == 1;
            
            // Load game settings
            showHints = PlayerPrefs.GetInt(SHOW_HINTS_KEY, showHints ? 1 : 0) == 1;
            showAnimations = PlayerPrefs.GetInt(SHOW_ANIMATIONS_KEY, showAnimations ? 1 : 0) == 1;
            showLastMove = PlayerPrefs.GetInt(SHOW_LAST_MOVE_KEY, showLastMove ? 1 : 0) == 1;
            gameSpeed = PlayerPrefs.GetFloat(GAME_SPEED_KEY, gameSpeed);
            
            Debug.Log("Game settings loaded from PlayerPrefs");
        }
        
        /// <summary>
        /// Saves all settings to PlayerPrefs
        /// </summary>
        public void SaveSettings()
        {
            PlayerPrefs.Save();
            Debug.Log("Game settings saved to PlayerPrefs");
        }
        
        /// <summary>
        /// Resets all settings to default values
        /// </summary>
        public void ResetToDefaults()
        {
            // Reset audio settings
            MasterVolume = 1.0f;
            MusicVolume = 0.8f;
            SFXVolume = 1.0f;
            UIVolume = 0.9f;
            
            MasterMuted = false;
            MusicMuted = false;
            SFXMuted = false;
            UIMuted = false;
            
            // Reset game settings
            ShowHints = true;
            ShowAnimations = true;
            ShowLastMove = true;
            GameSpeed = 1.0f;
            
            Debug.Log("Game settings reset to defaults");
        }
        
        /// <summary>
        /// Gets all audio volume levels as a tuple
        /// </summary>
        public (float master, float music, float sfx, float ui) GetAudioVolumes()
        {
            return (masterVolume, musicVolume, sfxVolume, uiVolume);
        }
        
        /// <summary>
        /// Gets all audio mute states as a tuple
        /// </summary>
        public (bool master, bool music, bool sfx, bool ui) GetAudioMuteStates()
        {
            return (masterMuted, musicMuted, sfxMuted, uiMuted);
        }
        
        /// <summary>
        /// Applies audio settings to AudioManager
        /// </summary>
        public void ApplyAudioSettings()
        {
            if (Gomoku.Audio.AudioManager.Instance != null)
            {
                var audioManager = Gomoku.Audio.AudioManager.Instance;
                
                audioManager.SetMasterVolume(masterVolume);
                audioManager.SetMusicVolume(musicVolume);
                audioManager.SetSFXVolume(sfxVolume);
                audioManager.SetUIVolume(uiVolume);
                
                if (masterMuted) audioManager.ToggleMasterMute();
                if (musicMuted) audioManager.ToggleMusicMute();
                if (sfxMuted) audioManager.ToggleSFXMute();
                if (uiMuted) audioManager.ToggleUIMute();
                
                Debug.Log("Audio settings applied to AudioManager");
            }
            else
            {
                Debug.LogWarning("AudioManager instance not found. Cannot apply audio settings.");
            }
        }
    }
}