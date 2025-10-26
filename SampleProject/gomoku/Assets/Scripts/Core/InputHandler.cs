using UnityEngine;

namespace Gomoku.Core
{
    /// <summary>
    /// Handles keyboard input for game controls
    /// </summary>
    public class InputHandler : MonoBehaviour
    {
        private Gomoku.Systems.GameStateManager gameStateManager;
        
        void Start()
        {
            gameStateManager = Gomoku.Systems.GameStateManager.Instance;
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
            if (gameStateManager.IsInState(Gomoku.Systems.GameStateEnum.Playing))
            {
                gameStateManager.PauseGame();
            }
            else if (gameStateManager.IsInState(Gomoku.Systems.GameStateEnum.Paused))
            {
                gameStateManager.ResumeGame();
            }
        }
    }
}