using UnityEngine;
using UnityEngine.SceneManagement;
using Gomoku.Core;
using Gomoku.Managers;

namespace Gomoku.Game
{
    /// <summary>
    /// Concrete state class for the Game Over state.
    /// Handles the behavior and UI for the game over screen.
    /// </summary>
    public class GameOverState : GameState
    {
        private PlayerType winner;

        /// <summary>
        /// Constructor for GameOverState.
        /// </summary>
        /// <param name="manager">The GameStateManager that owns this state</param>
        /// <param name="gameManager">The main GameManager instance</param>
        /// <param name="winner">The player who won the game</param>
        public GameOverState(GameStateManager manager, GameManager gameManager, PlayerType winner = PlayerType.None) : base(manager, gameManager)
        {
            this.winner = winner;
        }

        /// <summary>
        /// Called when entering the Game Over state.
        /// Sets up the game over UI and audio.
        /// </summary>
        public override void Enter()
        {
            base.Enter();

            // Activate game over UI
            if (gameManager.uiManager != null)
            {
                gameManager.uiManager.SetActive(true);
            }

            //// Play victory sound and set audio state
            //if (gameManager.audioManager != null)
            //{
            //    gameManager.audioManager.PlayVictorySound();
            //    gameManager.audioManager.SetGameOverState();
            //}

            // Show the victory banner with winner information
            //if (gameManager.gameOverScreen != null)
            //{
            //    gameManager.gameOverScreen.ShowGameOver(winner);
            //}

            // Pause game time
            Time.timeScale = 0f;

            // Load game over scene
            if (gameManager.SceneLoader != null)
            {
                //gameManager.SceneLoader.LoadSceneAsync("GameOver");
            }
            else
            {
                SceneManager.LoadScene("GameOver");
            }

            // Notify other systems
            GameEvents.GameStateChanged(GameStateEnum.GameOver);
            if (winner != PlayerType.None)
            {
                GameEvents.PlayerWon(winner);
            }
        }

        /// <summary>
        /// Called every frame while in the Game Over state.
        /// Handles input for restarting the game or returning to main menu.
        /// </summary>
        public override void Update()
        {
            // Handle input for restarting game or returning to main menu
            // This would typically be handled by UI buttons
        }

        /// <summary>
        /// Called when exiting the Game Over state.
        /// Cleans up game over specific resources.
        /// </summary>
        public override void Exit()
        {
            base.Exit();
            // Cleanup game over resources if needed
        }
    }
}