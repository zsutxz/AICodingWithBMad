using UnityEngine;
using UnityEngine.Events;

namespace Gomoku.Core
{
    /// <summary>
    /// Centralized event system for decoupled communication between game systems.
    /// This replaces direct component references with a publish-subscribe pattern
    /// to improve modularity and maintainability.
    /// </summary>
    public static class GameEvents
    {
        // Game State Events
        public static event System.Action<GameStateEnum> OnGameStateChanged;
        public static event System.Action OnGameStarted;
        public static event System.Action OnGameEnded;
        public static event System.Action OnGameReset;

        // Player Events
        public static event System.Action<PlayerType> OnPlayerTurnChanged;
        public static event System.Action<PlayerType> OnTurnCompleted;
        public static event System.Action<PlayerType> OnPlayerWon;

        // Board Events
        public static event System.Action<int, int, PlayerType> OnPiecePlaced;
        public static event System.Action<int, int> OnPieceRemoved;
        public static event System.Action OnBoardCleared;

        // Input Events
        public static event System.Action<int, int> OnIntersectionClicked;
        public static event System.Action<int, int> OnValidIntersectionDetected;

        // Win Detection Events
        public static event System.Action<PlayerType> OnWinDetected;
        public static event System.Action OnDrawDetected;

        // UI Events
        public static event System.Action<bool> OnPauseToggled;
        public static event System.Action<string> OnNotificationShown;

        #region Game State Event Dispatchers

        /// <summary>
        /// Triggered when the game state changes (MainMenu, Playing, Paused, GameOver).
        /// </summary>
        /// <param name="state">The new game state</param>
        public static void GameStateChanged(GameStateEnum state)
        {
            OnGameStateChanged?.Invoke(state);
        }

        /// <summary>
        /// Triggered when a new game is started.
        /// </summary>
        public static void GameStarted()
        {
            OnGameStarted?.Invoke();
        }

        /// <summary>
        /// Triggered when the current game ends.
        /// </summary>
        public static void GameEnded()
        {
            OnGameEnded?.Invoke();
        }

        /// <summary>
        /// Triggered when the game is reset to its initial state.
        /// </summary>
        public static void GameReset()
        {
            OnGameReset?.Invoke();
        }

        #endregion

        #region Player Event Dispatchers

        /// <summary>
        /// Triggered when the player turn changes.
        /// </summary>
        /// <param name="player">The player whose turn it is</param>
        public static void PlayerTurnChanged(PlayerType player)
        {
            OnPlayerTurnChanged?.Invoke(player);
        }

        /// <summary>
        /// Triggered when a player completes their turn.
        /// </summary>
        /// <param name="player">The player who completed their turn</param>
        public static void TurnCompleted(PlayerType player)
        {
            OnTurnCompleted?.Invoke(player);
        }

        /// <summary>
        /// Triggered when a player wins the game.
        /// </summary>
        /// <param name="player">The winning player</param>
        public static void PlayerWon(PlayerType player)
        {
            OnPlayerWon?.Invoke(player);
        }

        #endregion

        #region Board Event Dispatchers

        /// <summary>
        /// Triggered when a piece is placed on the board.
        /// </summary>
        /// <param name="x">X coordinate of the placement</param>
        /// <param name="y">Y coordinate of the placement</param>
        /// <param name="player">The player who placed the piece</param>
        public static void PiecePlaced(int x, int y, PlayerType player)
        {
            OnPiecePlaced?.Invoke(x, y, player);
        }

        /// <summary>
        /// Triggered when a piece is removed from the board.
        /// </summary>
        /// <param name="x">X coordinate of the removal</param>
        /// <param name="y">Y coordinate of the removal</param>
        public static void PieceRemoved(int x, int y)
        {
            OnPieceRemoved?.Invoke(x, y);
        }

        /// <summary>
        /// Triggered when all pieces are cleared from the board.
        /// </summary>
        public static void BoardCleared()
        {
            OnBoardCleared?.Invoke();
        }

        #endregion

        #region Input Event Dispatchers

        /// <summary>
        /// Triggered when an intersection on the board is clicked.
        /// </summary>
        /// <param name="x">X coordinate of the intersection</param>
        /// <param name="y">Y coordinate of the intersection</param>
        public static void IntersectionClicked(int x, int y)
        {
            OnIntersectionClicked?.Invoke(x, y);
        }

        /// <summary>
        /// Triggered when a valid intersection is detected from input.
        /// </summary>
        /// <param name="x">X coordinate of the intersection</param>
        /// <param name="y">Y coordinate of the intersection</param>
        public static void ValidIntersectionDetected(int x, int y)
        {
            OnValidIntersectionDetected?.Invoke(x, y);
        }

        #endregion

        #region UI Event Dispatchers

        /// <summary>
        /// Triggered when the pause state is toggled.
        /// </summary>
        /// <param name="paused">True if the game is now paused, false if resumed</param>
        public static void PauseToggled(bool paused)
        {
            OnPauseToggled?.Invoke(paused);
        }

        /// <summary>
        /// Triggered when a notification should be shown to the player.
        /// </summary>
        /// <param name="message">The notification message</param>
        public static void NotificationShown(string message)
        {
            OnNotificationShown?.Invoke(message);
        }

        #endregion
    }
}