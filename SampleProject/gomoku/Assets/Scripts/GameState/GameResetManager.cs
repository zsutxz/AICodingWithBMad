using UnityEngine;

namespace Gomoku.GameState
{
    /// <summary>
    /// Manages game state persistence and reset functionality
    /// Ensures all game systems properly reset to initial state when starting a new game
    /// </summary>
    public class GameResetManager : MonoBehaviour
    {
        [Tooltip("Reference to the TurnManager to reset player turns")]
        [SerializeField] private TurnManager turnManager;

        [Tooltip("Reference to the GameStateManager to coordinate state transitions")]
        [SerializeField] private GameStateManager gameStateManager;

        [Tooltip("Reference to the VictoryBanner to reset winner display")]
        [SerializeField] private UI.VictoryBanner victoryBanner;

        [Tooltip("Reference to the GameBoardModel to reset board state")]
        [SerializeField] private GameBoard.GameBoardModel gameBoardModel;

        /// <summary>
        /// Reset all game systems to their initial state for a new game
        /// </summary>
        public void ResetGame()
        {
            Debug.Log("Resetting game state...");

            // Reset the game board
            if (gameBoardModel != null)
            {
                gameBoardModel.Reset();
                Debug.Log("Game board reset completed");
            }

            // Reset player turns
            if (turnManager != null)
            {
                turnManager.ResetTurns();
                Debug.Log("Turn manager reset completed");
            }

            // Reset victory banner
            if (victoryBanner != null)
            {
                victoryBanner.Reset();
                Debug.Log("Victory banner reset completed");
            }

            // Notify the game state manager that we're starting a new game
            if (gameStateManager != null)
            {
                gameStateManager.SetState(Gomoku.GameState.GameState.Playing);
                Debug.Log("Game state transitioned to Playing");
            }

            Debug.Log("Game reset completed successfully");
        }
    }
}