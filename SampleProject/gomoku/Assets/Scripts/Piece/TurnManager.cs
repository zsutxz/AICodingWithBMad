using UnityEngine;

namespace Gomoku
{
    /// <summary>
    /// Manages player turns and current player state
    /// </summary>
    public class TurnManager : MonoBehaviour
    {
        [Header("Turn Settings")]
        [SerializeField] private PlayerType startingPlayer = PlayerType.Black;
        
        // Current player state
        private PlayerType currentPlayer;
        
        // Events
        public System.Action<PlayerType> OnPlayerTurnChanged;
        public System.Action<PlayerType> OnTurnStarted;
        
        // Properties
        public PlayerType CurrentPlayer => currentPlayer;
        public bool IsBlackTurn => currentPlayer == PlayerType.Black;
        public bool IsWhiteTurn => currentPlayer == PlayerType.White;
        
        /// <summary>
        /// Player types for the game
        /// </summary>
        public enum PlayerType
        {
            None = 0,
            Black = 1,
            White = 2
        }
        
        private void Awake()
        {
            InitializeTurnManager();
        }
        
        /// <summary>
        /// Initializes the turn manager with starting settings
        /// </summary>
        private void InitializeTurnManager()
        {
            currentPlayer = startingPlayer;
            
            if (Application.isPlaying)
            {
                Debug.Log($"TurnManager initialized. Starting player: {currentPlayer}");
            }
        }
        
        /// <summary>
        /// Starts the first turn
        /// </summary>
        public void StartGame()
        {
            currentPlayer = startingPlayer;
            OnTurnStarted?.Invoke(currentPlayer);
            OnPlayerTurnChanged?.Invoke(currentPlayer);
            
            Debug.Log($"Game started. Current player: {currentPlayer}");
        }
        
        /// <summary>
        /// Switches to the next player's turn
        /// </summary>
        public void NextTurn()
        {
            PlayerType previousPlayer = currentPlayer;
            
            // Switch to the other player
            currentPlayer = currentPlayer == PlayerType.Black ? PlayerType.White : PlayerType.Black;
            
            // Notify listeners
            OnPlayerTurnChanged?.Invoke(currentPlayer);
            
            Debug.Log($"Turn changed from {previousPlayer} to {currentPlayer}");
        }
        
        /// <summary>
        /// Resets the turn manager to initial state
        /// </summary>
        public void ResetTurns()
        {
            PlayerType previousPlayer = currentPlayer;
            currentPlayer = startingPlayer;
            
            if (previousPlayer != currentPlayer)
            {
                OnPlayerTurnChanged?.Invoke(currentPlayer);
            }
            
            Debug.Log($"Turns reset. Current player: {currentPlayer}");
        }
        
        /// <summary>
        /// Sets the current player directly (useful for loading game state)
        /// </summary>
        /// <param name="player">Player to set as current</param>
        public void SetCurrentPlayer(PlayerType player)
        {
            if (player == PlayerType.None)
            {
                Debug.LogWarning("Cannot set current player to None");
                return;
            }
            
            PlayerType previousPlayer = currentPlayer;
            currentPlayer = player;
            
            if (previousPlayer != currentPlayer)
            {
                OnPlayerTurnChanged?.Invoke(currentPlayer);
            }
            
            Debug.Log($"Current player set to: {currentPlayer}");
        }
        
        /// <summary>
        /// Gets the opponent of the specified player
        /// </summary>
        /// <param name="player">Player to get opponent for</param>
        /// <returns>The opponent player type</returns>
        public static PlayerType GetOpponent(PlayerType player)
        {
            return player switch
            {
                PlayerType.Black => PlayerType.White,
                PlayerType.White => PlayerType.Black,
                _ => PlayerType.None
            };
        }
        
        /// <summary>
        /// Gets the opponent of the current player
        /// </summary>
        /// <returns>The opponent of the current player</returns>
        public PlayerType GetCurrentOpponent()
        {
            return GetOpponent(currentPlayer);
        }
        
        /// <summary>
        /// Checks if the specified player is the current player
        /// </summary>
        /// <param name="player">Player to check</param>
        /// <returns>True if the player is the current player</returns>
        public bool IsCurrentPlayer(PlayerType player)
        {
            return currentPlayer == player;
        }
        
        /// <summary>
        /// Gets a string representation of the current turn state
        /// </summary>
        /// <returns>Turn state as string</returns>
        public string GetTurnStateString()
        {
            return $"Current Turn: {currentPlayer}";
        }
        
        /// <summary>
        /// Validates that a player type is valid for gameplay
        /// </summary>
        /// <param name="player">Player type to validate</param>
        /// <returns>True if valid for gameplay</returns>
        public static bool IsValidPlayer(PlayerType player)
        {
            return player == PlayerType.Black || player == PlayerType.White;
        }
        
        #if UNITY_EDITOR
        /// <summary>
        /// Editor-only method for debugging turn state
        /// </summary>
        [ContextMenu("Log Current Turn State")]
        private void LogCurrentTurnState()
        {
            Debug.Log($"Turn Manager State: {GetTurnStateString()}");
        }
        
        /// <summary>
        /// Editor-only method for testing turn switching
        /// </summary>
        [ContextMenu("Switch Turn (Debug)")]
        private void DebugSwitchTurn()
        {
            NextTurn();
        }
        #endif
    }
}