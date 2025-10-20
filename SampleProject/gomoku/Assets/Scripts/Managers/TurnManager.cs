using UnityEngine;
using UnityEngine.Events;
using Gomoku.Core;

namespace Gomoku
{
    /// <summary>
    /// Manages player turns and turn-based game logic
    /// </summary>
    public class TurnManager : MonoBehaviour
    {
        [Header("Turn Settings")]
        [Tooltip("Starting player for the game")]
        [SerializeField] private PlayerType startingPlayer = PlayerType.PlayerOne;

        [Header("Events")]
        [Tooltip("Event triggered when the player turn changes")]
        [SerializeField] private UnityEvent<PlayerType> onPlayerTurnChanged;

        [Tooltip("Event triggered when a turn is completed")]
        [SerializeField] private UnityEvent<PlayerType> onTurnCompleted;

        // Current player whose turn it is
        private PlayerType currentPlayer;

        // Total number of turns taken
        private int turnCount = 0;

        #region MonoBehaviour Methods

        /// <summary>
        /// Initialize the turn manager
        /// </summary>
        private void Awake()
        {
            InitializeTurnManager();
        }

        /// <summary>
        /// Clean up event subscriptions
        /// </summary>
        private void OnDestroy()
        {
            // Clean up any event subscriptions if needed
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get the current player
        /// </summary>
        /// <returns>The current player whose turn it is</returns>
        public PlayerType CurrentPlayer
        {
            get { return currentPlayer; }
        }

        /// <summary>
        /// Get the total number of turns taken
        /// </summary>
        /// <returns>The total turn count</returns>
        public int TurnCount
        {
            get { return turnCount; }
        }

        /// <summary>
        /// End the current turn and switch to the next player
        /// </summary>
        public void EndTurn()
        {
            // Store the previous player
            PlayerType previousPlayer = currentPlayer;

            // Switch to the next player
            currentPlayer = GetNextPlayer(currentPlayer);

            // Increment turn counter
            turnCount++;

            // Trigger events
            onTurnCompleted?.Invoke(previousPlayer);
            onPlayerTurnChanged?.Invoke(currentPlayer);

            Debug.Log($"Turn ended. Now {currentPlayer}'s turn (Turn #{turnCount})");
        }

        /// <summary>
        /// Reset the turn manager to initial state
        /// </summary>
        public void ResetTurns()
        {
            currentPlayer = startingPlayer;
            turnCount = 0;

            // Trigger turn change event
            onPlayerTurnChanged?.Invoke(currentPlayer);

            Debug.Log($"Turn manager reset. Starting with {currentPlayer}");
        }

        /// <summary>
        /// Get the next player in sequence
        /// </summary>
        /// <param name="current">The current player</param>
        /// <returns>The next player</returns>
        public PlayerType GetNextPlayer(PlayerType current)
        {
            return current switch
            {
                PlayerType.PlayerOne => PlayerType.PlayerTwo,
                PlayerType.PlayerTwo => PlayerType.PlayerOne,
                _ => startingPlayer
            };
        }

        /// <summary>
        /// Check if it's a specific player's turn
        /// </summary>
        /// <param name="player">The player to check</param>
        /// <returns>True if it's the specified player's turn</returns>
        public bool IsPlayerTurn(PlayerType player)
        {
            return currentPlayer == player;
        }

        /// <summary>
        /// Set the starting player for the game
        /// </summary>
        /// <param name="player">The player to start with</param>
        public void SetStartingPlayer(PlayerType player)
        {
            startingPlayer = player;
            ResetTurns();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initialize the turn manager
        /// </summary>
        private void InitializeTurnManager()
        {
            currentPlayer = startingPlayer;
            turnCount = 0;

            Debug.Log($"TurnManager initialized. Starting with {currentPlayer}");
        }
        #endregion
        /// <summary>
        /// Utility: validate a player type
        /// </summary>
        public static bool IsValidPlayer(PlayerType player)
        {
            return player == PlayerType.PlayerOne || player == PlayerType.PlayerTwo;
        }
        #region Events

        /// <summary>
        /// Event triggered when player turn changes
        /// </summary>
        public UnityEvent<PlayerType> OnPlayerTurnChanged
        {
            get { return onPlayerTurnChanged; }
            set {   onPlayerTurnChanged =value; }
        }

        /// <summary>
        /// Event triggered when a turn is completed
        /// </summary>
        public UnityEvent<PlayerType> OnTurnCompleted
        {
            get { return onTurnCompleted; }
        }

        #endregion
    }
}
