using UnityEngine;

using System;

namespace Gomoku.UI
{
    /// <summary>
    /// Integrates UI components with game state management system
    /// </summary>
    public class UIGameStateIntegrator : MonoBehaviour
    {
        [Header("UI Component References")]
        [SerializeField] private TurnIndicator turnIndicator;
        [SerializeField] private MoveCounter moveCounter;
        [SerializeField] private PauseButton pauseButton;
        [SerializeField] private GameOverScreen gameOverScreen;

        [Header("Game System References")]
        [SerializeField] private GameStateManager gameStateManager;
        [SerializeField] private TurnManager turnManager;

        [Header("Integration Settings")]
        [SerializeField] private bool autoInitialize = true;
        [SerializeField] private bool enableStateEvents = true;

        private object gameBoardModel;
        private void Awake()
        {
            if (autoInitialize)
            {
                InitializeIntegration();
            }
        }

        private void OnDestroy()
        {
            CleanupEvents();
        }

        /// <summary>
        /// Initializes the UI-game state integration
        /// </summary>
        public void InitializeIntegration()
        {
            FindMissingReferences();
            SetupEventHandlers();
            InitializeUIState();

            Debug.Log("UI-GameState integration initialized");
        }

        /// <summary>
        /// Finds missing references automatically
        /// </summary>
        private void FindMissingReferences()
        {
            // Find game state manager
            if (gameStateManager == null)
                gameStateManager = FindObjectOfType<GameStateManager>();

            // Find turn manager
            if (turnManager == null)
                turnManager = FindObjectOfType<TurnManager>();


            // Find UI components
            if (turnIndicator == null)
                turnIndicator = FindObjectOfType<TurnIndicator>();

            if (moveCounter == null)
                moveCounter = FindObjectOfType<MoveCounter>();

            if (pauseButton == null)
                pauseButton = FindObjectOfType<PauseButton>();

            if (gameOverScreen == null)
                gameOverScreen = FindObjectOfType<GameOverScreen>();
        }

        /// <summary>
        /// Sets up event handlers for game state changes
        /// </summary>
        private void SetupEventHandlers()
        {
            if (!enableStateEvents) return;

            // Game state manager events
            if (gameStateManager != null)
            {
                gameStateManager.OnStateChange.AddListener(HandleGameStateChange);
            }

            //// Turn manager events
            //if (turnManager != null)
            //{
            //    turnManager.OnPlayerTurnChanged += HandlePlayerTurnChanged;
            //    turnManager.OnTurnCompleted += HandleTurnCompleted;
            //}

            // Game board model events (if available)
            // Note: GameBoardModel may need to be extended to include move events
        }

        private void HandleGameStateChange(GameState arg0)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initializes UI state based on current game state
        /// </summary>
        private void InitializeUIState()
        {
            if (gameStateManager != null)
            {
                HandleGameStateChange(GameState.MainMenu, gameStateManager.GetCurrentState());
            }

            if (turnManager != null && turnIndicator != null)
            {
                // Initialize turn indicator
                turnIndicator.UpdateTurnDisplay(turnManager.CurrentPlayer, false);
            }

            if (gameBoardModel != null && moveCounter != null)
            {
                // Initialize move counter
                moveCounter.RefreshDisplay();
            }
        }

        /// <summary>
        /// Handles game state changes
        /// </summary>
        /// <param name="oldState">Previous game state</param>
        /// <param name="newState">New game state</param>
        private void HandleGameStateChange(GameState oldState, GameState newState)
        {
            Debug.Log($"Game state changed from {oldState} to {newState}");

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
        }

        /// <summary>
        /// Handles Main Menu state
        /// </summary>
        private void HandleMainMenuState()
        {
            // Hide in-game UI elements
            SetInGameUIVisibility(false);

            // Reset UI state
            ResetUIState();
        }

        /// <summary>
        /// Handles Playing state
        /// </summary>
        private void HandlePlayingState()
        {
            // Show in-game UI elements
            SetInGameUIVisibility(true);

            // Enable UI interactions
            SetUIInteractable(true);

            // Update UI state
            UpdateUIForGameplay();
        }

        /// <summary>
        /// Handles Paused state
        /// </summary>
        private void HandlePausedState()
        {
            // Update pause button state
            if (pauseButton != null)
            {
                pauseButton.SetPauseState(true);
            }

            // Disable some UI interactions if needed
            SetUIInteractable(false);
        }

        /// <summary>
        /// Handles Game Over state
        /// </summary>
        private void HandleGameOverState()
        {
            // Show game over screen
            if (gameOverScreen != null)
            {
                // Get winner information and show game over screen
                // This would need to be implemented based on win detection
                gameOverScreen.ShowGameOver(PlayerType.Black); // Placeholder
            }

            // Disable in-game UI interactions
            SetUIInteractable(false);
        }

        /// <summary>
        /// Handles player turn changes
        /// </summary>
        /// <param name="newPlayer">New player whose turn it is</param>
        private void HandlePlayerTurnChanged(PlayerType newPlayer)
        {
            // Turn indicator should handle this automatically via event subscription
            // This method can be used for additional turn change logic
            Debug.Log($"Player turn changed to {newPlayer}");
        }

        /// <summary>
        /// Handles turn completion
        /// </summary>
        /// <param name="completedPlayer">Player who completed the turn</param>
        private void HandleTurnCompleted(PlayerType completedPlayer)
        {
            // Update move counter
            if (moveCounter != null)
            {
                moveCounter.OnMoveMade();
            }

            Debug.Log($"Turn completed by {completedPlayer}");
        }

        /// <summary>
        /// Sets visibility of in-game UI elements
        /// </summary>
        /// <param name="visible">Whether to show in-game UI</param>
        private void SetInGameUIVisibility(bool visible)
        {
            if (turnIndicator != null)
                turnIndicator.gameObject.SetActive(visible);

            if (moveCounter != null)
                moveCounter.gameObject.SetActive(visible);

            if (pauseButton != null)
                pauseButton.gameObject.SetActive(visible);
        }

        /// <summary>
        /// Sets interactable state of UI elements
        /// </summary>
        /// <param name="interactable">Whether UI should be interactable</param>
        private void SetUIInteractable(bool interactable)
        {
            if (pauseButton != null)
                pauseButton.SetInteractable(interactable);
        }

        /// <summary>
        /// Resets UI state to initial values
        /// </summary>
        private void ResetUIState()
        {
            if (moveCounter != null)
                moveCounter.OnGameReset();

            if (pauseButton != null)
                pauseButton.SetPauseState(false);
        }

        /// <summary>
        /// Updates UI for gameplay state
        /// </summary>
        private void UpdateUIForGameplay()
        {
            if (pauseButton != null)
                pauseButton.SetPauseState(false);

            if (moveCounter != null)
                moveCounter.RefreshDisplay();
        }

        /// <summary>
        /// Cleans up event subscriptions
        /// </summary>
        private void CleanupEvents()
        {
            if (gameStateManager != null)
            {
                // 修正：OnStateChange 只接受 UnityAction<GameState>，而不是带两个参数的委托
                gameStateManager.OnStateChange.RemoveListener(HandleGameStateChange);
            }

            if (turnManager != null)
            {
                // 假设 turnManager 的事件是 UnityEvent<PlayerType> 类型
                turnManager.OnPlayerTurnChanged.RemoveListener(HandlePlayerTurnChanged);
                turnManager.OnTurnCompleted.RemoveListener(HandleTurnCompleted);
            }
        }

        /// <summary>
        /// Manually triggers a UI update based on current game state
        /// </summary>
        public void RefreshUI()
        {
            if (gameStateManager != null)
            {
                HandleGameStateChange(GameState.MainMenu, gameStateManager.GetCurrentState());
            }

            if (turnManager != null && turnIndicator != null)
            {
                turnIndicator.UpdateTurnDisplay(turnManager.CurrentPlayer, false);
            }

            if (gameBoardModel != null && moveCounter != null)
            {
                moveCounter.RefreshDisplay();
            }
        }

        /// <summary>
        /// Validates that all required references are set
        /// </summary>
        /// <returns>True if integration is properly configured</returns>
        public bool ValidateIntegration()
        {
            bool isValid = true;

            if (gameStateManager == null)
            {
                Debug.LogError("UIGameStateIntegrator: GameStateManager reference is missing");
                isValid = false;
            }

            if (turnManager == null)
            {
                Debug.LogError("UIGameStateIntegrator: TurnManager reference is missing");
                isValid = false;
            }

            return isValid;
        }
    }
}
