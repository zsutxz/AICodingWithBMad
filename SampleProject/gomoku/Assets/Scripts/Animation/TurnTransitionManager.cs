using UnityEngine;
using Gomoku;
using Gomoku.Core;

namespace Gomoku.Animation
{
    /// <summary>
    /// Manages turn transition visual indicators and coordinates with TurnManager
    /// </summary>
    public class TurnTransitionManager : MonoBehaviour
    {
        [Header("Component References")]
        [SerializeField] private TurnManager turnManager;
        [SerializeField] private TurnTransition turnTransition;
        
        [Header("Transition Settings")]
        [SerializeField] private bool enableTurnTransitions = true;
        [SerializeField] private float transitionDelay = 0.2f;
        
        // Events
        public System.Action<PlayerType> OnTurnTransitionStart;
        public System.Action<PlayerType> OnTurnTransitionComplete;
        
        // Properties
        public bool IsTransitioning { get; private set; }
        public bool EnableTurnTransitions => enableTurnTransitions;
        
        private void Awake()
        {
            Initialize();
        }
        
        /// <summary>
        /// Initializes the turn transition manager
        /// </summary>
        private void Initialize()
        {
            // Auto-find components if not assigned
            if (turnManager == null)
            {
                turnManager = FindObjectOfType<TurnManager>();
            }
            
            if (turnTransition == null)
            {
                turnTransition = FindObjectOfType<TurnTransition>();
            }
            
            // Subscribe to turn manager events
            if (turnManager != null)
            {
                turnManager.OnPlayerTurnChanged.AddListener(HandlePlayerTurnChanged);
            }
            else
            {
                Debug.LogError("TurnTransitionManager: TurnManager reference not found");
            }
            
            if (turnTransition == null)
            {
                Debug.LogWarning("TurnTransitionManager: TurnTransition reference not found - transitions will be disabled");
                enableTurnTransitions = false;
            }
        }
        
        /// <summary>
        /// Handles player turn change events
        /// </summary>
        /// <param name="newPlayer">The new player whose turn it is</param>
        private void HandlePlayerTurnChanged(PlayerType newPlayer)
        {
            if (!enableTurnTransitions || turnTransition == null) return;
            
            StartTurnTransition(newPlayer);
        }
        
        /// <summary>
        /// Starts the turn transition
        /// </summary>
        /// <param name="newPlayer">The new player</param>
        public void StartTurnTransition(PlayerType newPlayer)
        {
            if (IsTransitioning)
            {
                turnTransition.StopTransitionAnimation();
            }
            
            IsTransitioning = true;
            OnTurnTransitionStart?.Invoke(newPlayer);
            
            // Update turn display
            turnTransition.UpdateTurnDisplay(newPlayer);
            
            // Start the transition animation
            turnTransition.StartTurnTransition(newPlayer);
            
            // Subscribe to completion event
            turnTransition.OnTransitionComplete += HandleTransitionComplete;
            
            Debug.Log($"Turn transition started for {newPlayer}");
        }
        
        /// <summary>
        /// Handles transition completion
        /// </summary>
        /// <param name="transition">The completed transition</param>
        private void HandleTransitionComplete(TurnTransition transition)
        {
            IsTransitioning = false;
            
            // Unsubscribe from event
            if (turnTransition != null)
            {
                turnTransition.OnTransitionComplete -= HandleTransitionComplete;
            }
            
            PlayerType currentPlayer = turnManager != null ? turnManager.CurrentPlayer : PlayerType.None;
            OnTurnTransitionComplete?.Invoke(currentPlayer);
            
            Debug.Log($"Turn transition completed for {currentPlayer}");
        }
        
        /// <summary>
        /// Stops the current turn transition
        /// </summary>
        public void StopTurnTransition()
        {
            if (turnTransition != null)
            {
                turnTransition.StopTransitionAnimation();
                
                // Unsubscribe from event
                turnTransition.OnTransitionComplete -= HandleTransitionComplete;
            }
            
            IsTransitioning = false;
        }
        
        /// <summary>
        /// Sets whether turn transitions are enabled
        /// </summary>
        /// <param name="enabled">Whether transitions are enabled</param>
        public void SetTurnTransitionsEnabled(bool enabled)
        {
            enableTurnTransitions = enabled;
            
            if (!enabled && IsTransitioning)
            {
                StopTurnTransition();
            }
        }
        
        /// <summary>
        /// Sets the turn transition delay
        /// </summary>
        /// <param name="delay">Transition delay</param>
        public void SetTransitionDelay(float delay)
        {
            transitionDelay = Mathf.Max(0f, delay);
        }
        
        /// <summary>
        /// Updates the turn display immediately without animation
        /// </summary>
        /// <param name="player">Current player</param>
        public void UpdateTurnDisplayImmediate(PlayerType player)
        {
            if (turnTransition != null)
            {
                turnTransition.UpdateTurnDisplay(player);
            }
        }
        
        /// <summary>
        /// Resets the turn transition manager
        /// </summary>
        public void ResetManager()
        {
            StopTurnTransition();
            
            if (turnTransition != null)
            {
                turnTransition.ResetTurnTransition();
            }
        }
        
        private void OnDestroy()
        {
            // Clean up event subscriptions
            if (turnManager != null)
            {
                turnManager.OnPlayerTurnChanged.RemoveListener(HandlePlayerTurnChanged);
            }
            
            if (turnTransition != null)
            {
                turnTransition.OnTransitionComplete -= HandleTransitionComplete;
            }
            
            StopTurnTransition();
        }
        
        #if UNITY_EDITOR
        /// <summary>
        /// Editor-only method for testing turn transitions
        /// </summary>
        [ContextMenu("Test Turn Transition - Player One")]
        private void TestTurnTransitionPlayerOne()
        {
            if (Application.isPlaying)
            {
                StartTurnTransition(PlayerType.PlayerOne);
            }
            else
            {
                Debug.Log("Turn transition test can only be run in Play mode");
            }
        }
        
        /// <summary>
        /// Editor-only method for testing turn transitions
        /// </summary>
        [ContextMenu("Test Turn Transition - Player Two")]
        private void TestTurnTransitionPlayerTwo()
        {
            if (Application.isPlaying)
            {
                StartTurnTransition(PlayerType.PlayerTwo);
            }
            else
            {
                Debug.Log("Turn transition test can only be run in Play mode");
            }
        }
        
        /// <summary>
        /// Editor-only method for stopping turn transitions
        /// </summary>
        [ContextMenu("Stop Turn Transition")]
        private void StopTurnTransitionEditor()
        {
            if (Application.isPlaying)
            {
                StopTurnTransition();
            }
            else
            {
                Debug.Log("Turn transition stop can only be run in Play mode");
            }
        }
        #endif
    }
}