using UnityEngine;
using Gomoku.Managers;
using Gomoku.Core;

namespace Gomoku.Game
{
    /// <summary>
    /// Concrete state class for the Paused state.
    /// Handles the behavior and UI for the pause menu.
    /// </summary>
    public class PausedState : GameState
    {
        /// <summary>
        /// Constructor for PausedState.
        /// </summary>
        /// <param name="manager">The GameStateManager that owns this state</param>
        /// <param name="gameManager">The main GameManager instance</param>
        public PausedState(GameStateManager manager, GameManager gameManager) : base(manager, gameManager) { }

        /// <summary>
        /// Called when entering the Paused state.
        /// Sets up the pause menu UI and audio.
        /// </summary>
        public override void Enter()
        {
            base.Enter();

            // Activate pause menu UI
            if (gameManager.uiManager != null)
            {
                gameManager.uiManager.SetActive(true);
                // Setup pause menu specific UI elements
            }

            // Play pause sound effect
            if (gameManager.audioManager != null)
            {
                gameManager.audioManager.PlayPauseSound();
                gameManager.audioManager.SetPausedState();
            }

            // Pause game time
            Time.timeScale = 0f;

            // Notify other systems
            GameEvents.GameStateChanged(GameStateEnum.Paused);
            GameEvents.PauseToggled(true);
        }

        /// <summary>
        /// Called every frame while in the Paused state.
        /// Handles input for resuming the game or returning to main menu.
        /// </summary>
        public override void Update()
        {
            // Handle input for unpausing
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                //stateManager.SetState(new PlayingState(stateManager, gameManager));
            }
        }

        /// <summary>
        /// Called when exiting the Paused state.
        /// Cleans up pause menu specific resources.
        /// </summary>
        public override void Exit()
        {
            base.Exit();

            // Resume audio
            //if (stateManager.audioManager != null)
            //{
            //    stateManager.audioManager.SetPlayingState();
            //}

            // Notify other systems
            GameEvents.PauseToggled(false);
        }
    }
}