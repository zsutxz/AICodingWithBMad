using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Gomoku.SceneManagement;
using Gomoku.Audio;
using Gomoku.UI;
using System;

namespace Gomoku.Core
{
    /// <summary>
    /// Enum representing the different game states
    /// </summary>
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        GameOver
    }
}

namespace Gomoku
{
    using Gomoku.Core;

    /// <summary>
    /// Manages the overall game state and transitions between states
    /// </summary>
    public class GameStateManager : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Starting state when the game launches")]
        [SerializeField] private GameState startingState = GameState.MainMenu;

        [Header("Events")]
        [Tooltip("Event triggered when the game state changes")]
        [SerializeField] private UnityEvent<GameState, GameState> onStateChange;

        [Header("References")]
        [Tooltip("Reference to the UI Manager for screen transitions")]
        [SerializeField] public UIManager uiManager;

        [Tooltip("Reference to the Audio Manager for feedback")]
        [SerializeField] public AudioManager audioManager;

        [Tooltip("Reference to the Game Over Screen for displaying results")]
        [SerializeField] public GameOverScreen gameOverScreen;

        [Tooltip("Reference to the Win Detector for victory detection")]
        [SerializeField] public WinDetector winDetector;

        [Tooltip("Reference to the SceneLoader for scene transitions")]
        [SerializeField] public SceneLoader sceneLoader;

        // Current state of the game
        public GameState currentState;

        // Previous state for resuming gameplay
        private GameState previousState;
        internal UnityEvent<GameState> OnStateChange;

        #region MonoBehaviour Methods

        /// <summary>
        /// Initialize the game state manager
        /// </summary>
        private void Awake()
        {
            // Set the initial state
            currentState = startingState;

            // Initialize previous state
            previousState = currentState;

            // Ensure this manager persists across scene changes
            DontDestroyOnLoad(gameObject);

            // Log the initial state
            Debug.Log($"GameStateManager initialized in {currentState} state");
        }

        /// <summary>
        /// Handle state-specific logic each frame
        /// </summary>
        private void Update()
        {
            // Handle input for pausing/unpausing
            if (currentState == GameState.Playing && Input.GetKeyDown(KeyCode.Escape))
            {
                SetState(GameState.Paused);
            }
            else if (currentState == GameState.Paused && Input.GetKeyDown(KeyCode.Escape))
            {
                SetState(previousState);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Set the current game state
        /// </summary>
        /// <param name="newState">The new state to transition to</param>
        public void SetState(GameState newState)
        {
            // Don't do anything if we're already in this state
            if (currentState == newState)
            {
                return;
            }

            // Store the previous state if we're leaving Playing state
            if (currentState == GameState.Playing)
            {
                previousState = currentState;
            }

            // Store the old state for the event
            GameState oldState = currentState;

            // Update the current state
            currentState = newState;

            // Handle specific state transitions
            switch (newState)
            {
                case GameState.MainMenu:
                    HandleMainMenuState();
                    break;
                case GameState.Playing:
                    HandlePlayingState();
                    break;
                case GameState.Paused:
                    HandlePausedState();
                    break;
                case GameState.GameOver:
                    HandleGameOverState();
                    break;
            }

            // Trigger the state change event
            onStateChange?.Invoke(oldState, newState);

            // Log the state change
            Debug.Log($"Game state changed from {oldState} to {newState}");
        }

        /// <summary>
        /// Get the current game state
        /// </summary>
        /// <returns>The current game state</returns>
        public GameState GetCurrentState()
        {
            return currentState;
        }

        /// <summary>
        /// Check if the game is in a specific state
        /// </summary>
        /// <param name="state">The state to check</param>
        /// <returns>True if the game is in the specified state</returns>
        public bool IsInState(GameState state)
        {
            return currentState == state;
        }

        /// <summary>
        /// Pause the game
        /// </summary>
        public void PauseGame()
        {
            if (currentState == GameState.Playing)
            {
                SetState(GameState.Paused);
            }
        }

        /// <summary>
        /// Resume the game from pause
        /// </summary>
        public void ResumeGame()
        {
            if (currentState == GameState.Paused)
            {
                SetState(previousState);
            }
        }

        /// <summary>
        /// Restart the game
        /// </summary>
        public void RestartGame()
        {
            SetState(GameState.Playing);
        }

        /// <summary>
        /// Return to main menu
        /// </summary>
        public void ReturnToMainMenu()
        {
            SetState(GameState.MainMenu);
        }

        #endregion

        #region State Handler Methods

        /// <summary>
        /// Handle logic for when entering the Main Menu state
        /// </summary>
        private void HandleMainMenuState()
        {
            // Activate main menu UI
            if (uiManager != null)
            {
                uiManager.gameObject.SetActive(true);
                // Additional UI setup for main menu
            }

            // Play main menu music
            if (audioManager != null)
            {
                audioManager.PlayMainMenuMusic();
            }

            // Pause any ongoing gameplay
            Time.timeScale = 0f;

            // Load main menu scene
            if (sceneLoader != null)
            {
                sceneLoader.LoadScene("MainMenu");
            }
            else
            {
                SceneManager.LoadScene("MainMenu");
            }
        }

        /// <summary>
        /// Handle logic for when entering the Playing state
        /// </summary>
        private void HandlePlayingState()
        {
            // Deactivate pause menu UI
            if (uiManager != null)
            {
                uiManager.SetActive(true);
                // Additional UI setup for gameplay
            }

            // Play gameplay music
            if (audioManager != null)
            {
                audioManager.PlayGameplayMusic();
            }

            // Resume game time
            Time.timeScale = 1f;

            // Load game scene
            if (sceneLoader != null)
            {
                sceneLoader.LoadSceneAsync("GameScene");
            }
            else
            {
                SceneManager.LoadScene("GameScene");
            }
        }

        /// <summary>
        /// Handle logic for when entering the Paused state
        /// </summary>
        private void HandlePausedState()
        {
            // Activate pause menu UI
            if (uiManager != null)
            {
                uiManager.SetActive(true);
                // Additional UI setup for pause menu
            }

            // Play pause sound effect
            if (audioManager != null)
            {
                audioManager.PlayPauseSound();
                audioManager.SetPausedState();
            }

            // Pause game time
            Time.timeScale = 0f;
        }

        /// <summary>
        /// Handle logic for when entering the Game Over state
        /// </summary>
        private void HandleGameOverState()
        {
            // Activate game over UI
            if (uiManager != null)
            {
                uiManager.SetActive(true);
            }

            // Play victory sound and set audio state
            if (audioManager != null)
            {
                audioManager.PlayVictorySound();
                audioManager.SetGameOverState();
            }

            // Show the victory banner with winner information
            if (gameOverScreen != null)
            {
                // Get the winner from the WinDetector
                if (winDetector != null && winDetector.HasWinner())
                {
                    var winner = winDetector.GetWinner();
                    gameOverScreen.ShowGameOver(winner);
                }
                else
                {
                    // Fallback: if no winner is detected, default to black
                    gameOverScreen.ShowGameOver(PlayerType.PlayerOne);
                }
            }

            // Pause game time
            Time.timeScale = 0f;

            // Load game over scene
            if (sceneLoader != null)
            {
                sceneLoader.LoadSceneAsync("GameOver");
            }
            else
            {
                SceneManager.LoadScene("GameOver");
            }
        }

        #endregion
    }
}

