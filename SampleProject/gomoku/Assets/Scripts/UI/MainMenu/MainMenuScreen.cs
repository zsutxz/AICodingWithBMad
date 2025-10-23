using UnityEngine;
using UnityEngine.UI;
using Gomoku.Game;
using Gomoku.Audio;

namespace Gomoku.UI.MainMenu
{
    /// <summary>
    /// Manages the main menu screen UI and interactions
    /// </summary>
    public class MainMenuScreen : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button startButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button exitButton;

        [Header("Dependencies")]
        [SerializeField] public GameStateManager gameStateManager;
        [SerializeField] public AudioManager audioManager;
        [SerializeField] public SceneLoader sceneLoader;

        #region MonoBehaviour Methods

        /// <summary>
        /// Initialize the main menu screen
        /// </summary>
        private void Awake()
        {
            InitializeComponents();
            SetupButtonListeners();
        }

        #endregion

        #region Initialization Methods

        /// <summary>
        /// Initialize required components and validate references
        /// </summary>
        private void InitializeComponents()
        {
            // Validate UI references
            if (startButton == null)
                Debug.LogError("MainMenuScreen: Start button reference not assigned!");

            if (settingsButton == null)
                Debug.LogError("MainMenuScreen: Settings button reference not assigned!");

            if (exitButton == null)
                Debug.LogError("MainMenuScreen: Exit button reference not assigned!");

            // Validate dependencies
            if (gameStateManager == null)
                Debug.LogError("MainMenuScreen: GameStateManager reference not assigned!");

            if (audioManager == null)
                Debug.LogError("MainMenuScreen: AudioManager reference not assigned!");

            if (sceneLoader == null)
                Debug.LogError("MainMenuScreen: SceneLoader reference not assigned!");

            Debug.Log("MainMenuScreen initialized successfully");
        }

        /// <summary>
        /// Set up button click listeners
        /// </summary>
        private void SetupButtonListeners()
        {
            // Remove any existing listeners to prevent duplicates
            startButton.onClick.RemoveAllListeners();
            settingsButton.onClick.RemoveAllListeners();
            exitButton.onClick.RemoveAllListeners();

            // Add click listeners
            startButton.onClick.AddListener(OnStartButtonClicked);
            settingsButton.onClick.AddListener(OnSettingsButtonClicked);
            exitButton.onClick.AddListener(OnExitButtonClicked);
        }

        #endregion

        #region Button Event Handlers

        /// <summary>
        /// Handle start button click
        /// </summary>
        private void OnStartButtonClicked()
        {
            // Play button click sound
            if (audioManager != null)
            {
                audioManager.PlayPauseSound();
            }

            // Transition to playing state
            if (gameStateManager != null)
            {
                gameStateManager.SetState(GameStateEnum.Playing);
            }
            else
            {
                // Fallback: directly load game scene
                if (sceneLoader != null)
                {
                    //sceneLoader.LoadScene("GameScene");
                }
                else
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
                }
            }
        }

        /// <summary>
        /// Handle settings button click
        /// </summary>
        private void OnSettingsButtonClicked()
        {
            // Play button click sound
            if (audioManager != null)
            {
                audioManager.PlayPauseSound();
            }

            // TODO: Implement settings panel opening logic
            Debug.Log("Settings button clicked - settings panel not implemented yet");
        }

        /// <summary>
        /// Handle exit button click
        /// </summary>
        private void OnExitButtonClicked()
        {
            // Play button click sound
            if (audioManager != null)
            {
                audioManager.PlayPauseSound();
            }

            // Confirm exit on non-editor platforms
            #if UNITY_EDITOR
                Debug.Log("Exit button clicked - exiting application (editor)");
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Debug.Log("Exit button clicked - exiting application");
                Application.Quit();
            #endif
        }

        #endregion
    }
}