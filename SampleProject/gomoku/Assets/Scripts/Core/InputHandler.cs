using UnityEngine;

namespace Gomoku.Core
{
    /// <summary>
    /// Handles keyboard input for game controls
    /// </summary>
    public class InputHandler : MonoBehaviour
    {
        private GameStateManager gameStateManager;
        
        void Start()
        {
            gameStateManager = GameStateManager.Instance;
        }
        
        void Update()
        {
            // Check for ESC key to toggle pause
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                HandlePauseInput();
            }
        }
        
        public void HandlePauseInput()
        {
            if (gameStateManager.IsInState(GameStateEnum.Playing))
            {
                gameStateManager.PauseGame();
            }
            else if (gameStateManager.IsInState(GameStateEnum.Paused))
            {
                gameStateManager.ResumeGame();
            }
        }
    }
}