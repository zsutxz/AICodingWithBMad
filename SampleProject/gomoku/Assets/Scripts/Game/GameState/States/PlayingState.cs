using UnityEngine;
using UnityEngine.SceneManagement;
using Gomoku.Managers;
using Gomoku.Core;

namespace Gomoku.Game
{
    /// <summary>
    /// Concrete state class for the Playing state.
    /// Handles the behavior and UI for the gameplay screen.
    /// </summary>
    public class PlayingState : GameState
    {
        /// <summary>
        /// Constructor for PlayingState.
        /// </summary>
        /// <param name="manager">The GameStateManager that owns this state</param>
        /// <param name="gameManager">The main GameManager instance</param>
        public PlayingState(GameStateManager manager, GameManager gameManager) : base(manager, gameManager) { }

        /// <summary>
        /// Called when entering the Playing state.
        /// Sets up the gameplay UI and audio.
        /// </summary>
        public override void Enter()
        {
            base.Enter();

            // Deactivate pause menu UI
            if (gameManager.uiManager != null)
            {
                gameManager.uiManager.SetActive(true);
                // Setup gameplay specific UI elements
            }

            // Play gameplay music
            if (gameManager.audioManager != null)
            {
                gameManager.audioManager.PlayGameplayMusic();
            }

            // Resume game time
            Time.timeScale = 1f;

            // Load game scene
            //if (gameManager.SceneLoader != null)
            //{
            //    gameManager.SceneLoader.LoadSceneAsync("GameScene");
            //}
            //else
            {
                SceneManager.LoadScene("GameScene");
            }

            // Notify other systems
            GameEvents.GameStateChanged(GameStateEnum.Playing);
        }

        /// <summary>
        /// Called every frame while in the Playing state.
        /// Handles input for pausing the game.
        /// </summary>
        public override void Update()
        {
            // Handle input for pausing
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                //stateManager.SetState(new PausedState(stateManager, gameManager));
            }
        }

        /// <summary>
        /// Called when exiting the Playing state.
        /// Cleans up gameplay specific resources.
        /// </summary>
        public override void Exit()
        {
            base.Exit();
            // Cleanup gameplay resources if needed
        }
    }
}