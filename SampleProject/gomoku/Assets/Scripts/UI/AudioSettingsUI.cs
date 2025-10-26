using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using TMPro;
using Gomoku.Audio;
using Gomoku.Core;

namespace Gomoku.UI
{
    /// <summary>
    /// Handles audio settings UI functionality with real-time updates
    /// </summary>
    public class AudioSettingsUI : MonoBehaviour
    {
        [Header("Volume Sliders")]
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Slider uiVolumeSlider;
        
        [Header("Volume Text Labels")]
        [SerializeField] private TextMeshProUGUI masterVolumeText;
        [SerializeField] private TextMeshProUGUI musicVolumeText;
        [SerializeField] private TextMeshProUGUI sfxVolumeText;
        [SerializeField] private TextMeshProUGUI uiVolumeText;
        
        [Header("Mute Toggles")]
        [SerializeField] private Toggle masterMuteToggle;
        [SerializeField] private Toggle musicMuteToggle;
        [SerializeField] private Toggle sfxMuteToggle;
        [SerializeField] private Toggle uiMuteToggle;
        
        [Header("Reset Button")]
        [SerializeField] private Button resetButton;
        [SerializeField] private Button saveButton;
        [SerializeField] private Button cancelButton;
        
        [Header("Settings Reference")]
        [SerializeField] private GameSettingsModel gameSettings;
        
        // Internal state
        private bool isInitialized = false;
        private (float master, float music, float sfx, float ui) initialVolumes;
        private (bool master, bool music, bool sfx, bool ui) initialMuteStates;
        
        private void Awake()
        {
            InitializeComponents();
        }
        
        private void Start()
        {
            SetupEventListeners();
            LoadInitialSettings();
            isInitialized = true;
        }
        
        private void OnDestroy()
        {
            CleanupEventListeners();
        }
        
        /// <summary>
        /// Initializes UI components and validates references
        /// </summary>
        private void InitializeComponents()
        {
            // Find or create GameSettings if not assigned
            if (gameSettings == null)
            {
                gameSettings = Resources.Load<GameSettingsModel>("GameSettingsModel");
                if (gameSettings == null)
                {
                    Debug.LogWarning("GameSettingsModel not found. Creating temporary settings.");
                    gameSettings = ScriptableObject.CreateInstance<GameSettingsModel>();
                }
            }
            
            // Validate slider references
            ValidateSlider(masterVolumeSlider, "Master Volume");
            ValidateSlider(musicVolumeSlider, "Music Volume");
            ValidateSlider(sfxVolumeSlider, "SFX Volume");
            ValidateSlider(uiVolumeSlider, "UI Volume");
            
            // Validate text references
            ValidateText(masterVolumeText, "Master Volume");
            ValidateText(musicVolumeText, "Music Volume");
            ValidateText(sfxVolumeText, "SFX Volume");
            ValidateText(uiVolumeText, "UI Volume");
            
            // Validate toggle references
            ValidateToggle(masterMuteToggle, "Master Mute");
            ValidateToggle(musicMuteToggle, "Music Mute");
            ValidateToggle(sfxMuteToggle, "SFX Mute");
            ValidateToggle(uiMuteToggle, "UI Mute");
            
            // Validate button references
            ValidateButton(resetButton, "Reset");
            ValidateButton(saveButton, "Save");
            ValidateButton(cancelButton, "Cancel");
        }
        
        /// <summary>
        /// Sets up event listeners for UI interactions
        /// </summary>
        private void SetupEventListeners()
        {
            // Slider listeners
            if (masterVolumeSlider != null)
                masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
            
            if (musicVolumeSlider != null)
                musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            
            if (sfxVolumeSlider != null)
                sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
            
            if (uiVolumeSlider != null)
                uiVolumeSlider.onValueChanged.AddListener(OnUIVolumeChanged);
            
            // Toggle listeners
            if (masterMuteToggle != null)
                masterMuteToggle.onValueChanged.AddListener(OnMasterMuteChanged);
            
            if (musicMuteToggle != null)
                musicMuteToggle.onValueChanged.AddListener(OnMusicMuteChanged);
            
            if (sfxMuteToggle != null)
                sfxMuteToggle.onValueChanged.AddListener(OnSFXMuteChanged);
            
            if (uiMuteToggle != null)
                uiMuteToggle.onValueChanged.AddListener(OnUIMuteChanged);
            
            // Button listeners
            if (resetButton != null)
                resetButton.onClick.AddListener(OnResetClicked);
            
            if (saveButton != null)
                saveButton.onClick.AddListener(OnSaveClicked);
            
            if (cancelButton != null)
                cancelButton.onClick.AddListener(OnCancelClicked);
            
            // Settings model listeners
            if (gameSettings != null)
            {
                gameSettings.OnMasterVolumeChanged += OnSettingsMasterVolumeChanged;
                gameSettings.OnMusicVolumeChanged += OnSettingsMusicVolumeChanged;
                gameSettings.OnSFXVolumeChanged += OnSettingsSFXVolumeChanged;
                gameSettings.OnUIVolumeChanged += OnSettingsUIVolumeChanged;
                gameSettings.OnMasterMutedChanged += OnSettingsMasterMuteChanged;
                gameSettings.OnMusicMutedChanged += OnSettingsMusicMuteChanged;
                gameSettings.OnSFXMutedChanged += OnSettingsSFXMuteChanged;
                gameSettings.OnUIMutedChanged += OnSettingsUIMuteChanged;
            }
        }
        
        /// <summary>
        /// Loads initial settings into UI
        /// </summary>
        private void LoadInitialSettings()
        {
            if (gameSettings == null) return;
            
            // Store initial values for cancel functionality
            initialVolumes = gameSettings.GetAudioVolumes();
            initialMuteStates = gameSettings.GetAudioMuteStates();
            
            // Load volumes
            if (masterVolumeSlider != null)
            {
                masterVolumeSlider.value = initialVolumes.master;
                UpdateVolumeText(masterVolumeText, initialVolumes.master);
            }
            
            if (musicVolumeSlider != null)
            {
                musicVolumeSlider.value = initialVolumes.music;
                UpdateVolumeText(musicVolumeText, initialVolumes.music);
            }
            
            if (sfxVolumeSlider != null)
            {
                sfxVolumeSlider.value = initialVolumes.sfx;
                UpdateVolumeText(sfxVolumeText, initialVolumes.sfx);
            }
            
            if (uiVolumeSlider != null)
            {
                uiVolumeSlider.value = initialVolumes.ui;
                UpdateVolumeText(uiVolumeText, initialVolumes.ui);
            }
            
            // Load mute states
            if (masterMuteToggle != null)
                masterMuteToggle.isOn = initialMuteStates.master;
            
            if (musicMuteToggle != null)
                musicMuteToggle.isOn = initialMuteStates.music;
            
            if (sfxMuteToggle != null)
                sfxMuteToggle.isOn = initialMuteStates.sfx;
            
            if (uiMuteToggle != null)
                uiMuteToggle.isOn = initialMuteStates.ui;
        }
        
        #region UI Event Handlers
        
        private void OnMasterVolumeChanged(float value)
        {
            if (!isInitialized) return;
            
            UpdateVolumeText(masterVolumeText, value);
            gameSettings.MasterVolume = value;
            AudioManager.Instance.SetMasterVolume(value);
        }
        
        private void OnMusicVolumeChanged(float value)
        {
            if (!isInitialized) return;
            
            UpdateVolumeText(musicVolumeText, value);
            gameSettings.MusicVolume = value;
            AudioManager.Instance.SetMusicVolume(value);
        }
        
        private void OnSFXVolumeChanged(float value)
        {
            if (!isInitialized) return;
            
            UpdateVolumeText(sfxVolumeText, value);
            gameSettings.SFXVolume = value;
            AudioManager.Instance.SetSFXVolume(value);
        }
        
        private void OnUIVolumeChanged(float value)
        {
            if (!isInitialized) return;
            
            UpdateVolumeText(uiVolumeText, value);
            gameSettings.UIVolume = value;
            AudioManager.Instance.SetUIVolume(value);
        }
        
        private void OnMasterMuteChanged(bool value)
        {
            if (!isInitialized) return;
            
            gameSettings.MasterMuted = value;
            if (value)
                AudioManager.Instance.ToggleMasterMute();
        }
        
        private void OnMusicMuteChanged(bool value)
        {
            if (!isInitialized) return;
            
            gameSettings.MusicMuted = value;
            if (value)
                AudioManager.Instance.ToggleMusicMute();
        }
        
        private void OnSFXMuteChanged(bool value)
        {
            if (!isInitialized) return;
            
            gameSettings.SFXMuted = value;
            if (value)
                AudioManager.Instance.ToggleSFXMute();
        }
        
        private void OnUIMuteChanged(bool value)
        {
            if (!isInitialized) return;
            
            gameSettings.UIMuted = value;
            if (value)
                AudioManager.Instance.ToggleUIMute();
        }
        
        private void OnResetClicked()
        {
            if (gameSettings != null)
            {
                gameSettings.ResetToDefaults();
                LoadInitialSettings();
            }
        }
        
        private void OnSaveClicked()
        {
            if (gameSettings != null)
            {
                gameSettings.SaveSettings();
                Debug.Log("Audio settings saved successfully");
            }
        }
        
        private void OnCancelClicked()
        {
            // Restore initial values
            if (gameSettings != null)
            {
                gameSettings.MasterVolume = initialVolumes.master;
                gameSettings.MusicVolume = initialVolumes.music;
                gameSettings.SFXVolume = initialVolumes.sfx;
                gameSettings.UIVolume = initialVolumes.ui;
                
                gameSettings.MasterMuted = initialMuteStates.master;
                gameSettings.MusicMuted = initialMuteStates.music;
                gameSettings.SFXMuted = initialMuteStates.sfx;
                gameSettings.UIMuted = initialMuteStates.ui;
                
                LoadInitialSettings();
            }
        }
        
        #endregion
        
        #region Settings Model Event Handlers
        
        private void OnSettingsMasterVolumeChanged(float value)
        {
            if (masterVolumeSlider != null)
            {
                masterVolumeSlider.value = value;
                UpdateVolumeText(masterVolumeText, value);
            }
        }

        private void OnSettingsMusicVolumeChanged(float value)
        {
            if (musicVolumeSlider != null)
            {
                musicVolumeSlider.value = value;
                UpdateVolumeText(musicVolumeText, value);
            }
        }

        private void OnSettingsSFXVolumeChanged(float value)
        {
            if (sfxVolumeSlider != null)
            {
                sfxVolumeSlider.value = value;
                UpdateVolumeText(sfxVolumeText, value);
            }
        }

        private void OnSettingsUIVolumeChanged(float value)
        {
            if (uiVolumeSlider != null)
            {
                uiVolumeSlider.value = value;
                UpdateVolumeText(uiVolumeText, value);
            }
        }

        private void OnSettingsMasterMuteChanged(bool value)
        {
            if (masterMuteToggle != null)
            {
                masterMuteToggle.isOn = value;
            }
        }

        private void OnSettingsMusicMuteChanged(bool value)
        {
            if (musicMuteToggle != null)
            {
                musicMuteToggle.isOn = value;
            }
        }

        private void OnSettingsSFXMuteChanged(bool value)
        {
            if (sfxMuteToggle != null)
            {
                sfxMuteToggle.isOn = value;
            }
        }

        private void OnSettingsUIMuteChanged(bool value)
        {
            if (uiMuteToggle != null)
            {
                uiMuteToggle.isOn = value;
            }
        }
        
        #endregion
        
        #region Helper Methods
        
        private void UpdateVolumeText(TextMeshProUGUI text, float value)
        {
            if (text != null)
            {
                text.text = Mathf.RoundToInt(value * 100).ToString();
            }
        }
        
        private void ValidateSlider(Slider slider, string name)
        {
            if (slider == null)
            {
                Debug.LogWarning($"{name} Slider not assigned in AudioSettingsUI");
            }
        }
        
        private void ValidateText(TextMeshProUGUI text, string name)
        {
            if (text == null)
            {
                Debug.LogWarning($"{name} Text not assigned in AudioSettingsUI");
            }
        }
        
        private void ValidateToggle(Toggle toggle, string name)
        {
            if (toggle == null)
            {
                Debug.LogWarning($"{name} Toggle not assigned in AudioSettingsUI");
            }
        }
        
        private void ValidateButton(Button button, string name)
        {
            if (button == null)
            {
                Debug.LogWarning($"{name} Button not assigned in AudioSettingsUI");
            }
        }
        
        private void CleanupEventListeners()
        {
            // Remove slider listeners
            if (masterVolumeSlider != null)
                masterVolumeSlider.onValueChanged.RemoveListener(OnMasterVolumeChanged);
            
            if (musicVolumeSlider != null)
                musicVolumeSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
            
            if (sfxVolumeSlider != null)
                sfxVolumeSlider.onValueChanged.RemoveListener(OnSFXVolumeChanged);
            
            if (uiVolumeSlider != null)
                uiVolumeSlider.onValueChanged.RemoveListener(OnUIVolumeChanged);
            
            // Remove toggle listeners
            if (masterMuteToggle != null)
                masterMuteToggle.onValueChanged.RemoveListener(OnMasterMuteChanged);
            
            if (musicMuteToggle != null)
                musicMuteToggle.onValueChanged.RemoveListener(OnMusicMuteChanged);
            
            if (sfxMuteToggle != null)
                sfxMuteToggle.onValueChanged.RemoveListener(OnSFXMuteChanged);
            
            if (uiMuteToggle != null)
                uiMuteToggle.onValueChanged.RemoveListener(OnUIMuteChanged);
            
            // Remove button listeners
            if (resetButton != null)
                resetButton.onClick.RemoveListener(OnResetClicked);
            
            if (saveButton != null)
                saveButton.onClick.RemoveListener(OnSaveClicked);
            
            if (cancelButton != null)
                cancelButton.onClick.RemoveListener(OnCancelClicked);
            
            // Remove settings model listeners
            if (gameSettings != null)
            {
                gameSettings.OnMasterVolumeChanged -= OnSettingsMasterVolumeChanged;
                gameSettings.OnMusicVolumeChanged -= OnSettingsMusicVolumeChanged;
                gameSettings.OnSFXVolumeChanged -= OnSettingsSFXVolumeChanged;
                gameSettings.OnUIVolumeChanged -= OnSettingsUIVolumeChanged;
                gameSettings.OnMasterMutedChanged -= OnSettingsMasterMuteChanged;
                gameSettings.OnMusicMutedChanged -= OnSettingsMusicMuteChanged;
                gameSettings.OnSFXMutedChanged -= OnSettingsSFXMuteChanged;
                gameSettings.OnUIMutedChanged -= OnSettingsUIMuteChanged;
            }
        }
        
        #endregion
    }
}