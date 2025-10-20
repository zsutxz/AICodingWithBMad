using Gomoku.Managers;
using UnityEngine;

namespace Gomoku.Game
{
    /// <summary>
    /// Abstract base class for game states in the State pattern.
    /// Each concrete state (MainMenu, Playing, Paused, GameOver) will inherit from this class
    /// and implement the specific behavior for that state.
    /// </summary>
    public abstract class GameState
    {
        // Reference to the state manager that owns this state
        protected GameStateManager stateManager;

        // Reference to the main game manager
        protected GameManager gameManager;

        /// <summary>
        /// Constructor for the base GameState class.
        /// </summary>
        /// <param name="manager">The GameStateManager that owns this state</param>
        /// <param name="gameManager">The main GameManager instance</param>
        public GameState(GameStateManager manager, GameManager gameManager)
        {
            this.stateManager = manager;
            this.gameManager = gameManager;
        }

        /// <summary>
        /// Called when the state is entered.
        /// Override this method to implement state-specific initialization logic.
        /// </summary>
        public virtual void Enter()
        {
            Debug.Log($"Entering state: {this.GetType().Name}");
        }

        /// <summary>
        /// Called every frame while the state is active.
        /// Override this method to implement state-specific update logic.
        /// </summary>
        public virtual void Update()
        {
            // Default implementation does nothing
        }

        /// <summary>
        /// Called when the state is exited.
        /// Override this method to implement state-specific cleanup logic.
        /// </summary>
        public virtual void Exit()
        {
            Debug.Log($"Exiting state: {this.GetType().Name}");
        }
    }
}