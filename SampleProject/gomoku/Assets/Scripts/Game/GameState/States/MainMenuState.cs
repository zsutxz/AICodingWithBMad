using UnityEngine;
using UnityEngine.SceneManagement;
using Gomoku.Managers;
using Gomoku.Core;

namespace Gomoku.Game
{
    /// <summary>
    /// Concrete state class for the Main Menu state.
    /// Handles the behavior and UI for the main menu screen.
    /// </summary>
    public class MainMenuState : GameState
    {
        /// <summary>
        /// Constructor for MainMenuState.
        /// </summary>
        /// <param name="manager">The GameStateManager that owns this state</param>
        /// <param name="gameManager">The main GameManager instance</param>
        public MainMenuState(GameStateManager manager, GameManager gameManager) : base(manager, gameManager) { }

        /// <summary>
        /// Called when entering the Main Menu state.
        /// Sets up the main menu UI and audio.
        /// </summary>
        public override void Enter()
        {
            base.Enter();

            // Activate main menu UI
            if (gameManager.uiManager != null)
            {
                gameManager.uiManager.SetActive(true);
                // Setup main menu specific UI elements
            }

            //// Play main menu music
            //if (gameManager.audioManager != null)
            //{
            //    gameManager.audioManager.PlayMainMenuMusic();
            //}

            // Pause game time
            Time.timeScale = 0f;


            // Load main menu scene
  

           SceneManager.LoadScene("MainMenu");
       

            // Notify other systems
            GameEvents.GameStateChanged(GameStateEnum.MainMenu);
        }

        /// <summary>
        /// Called every frame while in the Main Menu state.
        /// Handles input for starting the game, quitting, etc.
        /// </summary>
        public override void Update()
        {
            // Handle main menu input
            // This could be handled by UI buttons instead of direct input
        }

        /// <summary>
        /// Called when exiting the Main Menu state.
        /// Cleans up main menu specific resources.
        /// </summary>
        public override void Exit()
        {
            base.Exit();
            // Cleanup main menu resources if needed
        }
    }
}