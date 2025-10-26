using UnityEngine;
using Gomoku.UI;

namespace Gomoku.Game
{
    /// <summary>
    /// Listens to game state changes and updates UI accordingly
    /// </summary>
    public class GameStateListener : MonoBehaviour
    {
        private UIManager uiManager;
        private GameStateManager gameStateManager;
        private GameStateEnum lastState;
        
        void Start()
        {
            uiManager = FindObjectOfType<UIManager>();
            gameStateManager = GameStateManager.Instance;
            lastState = gameStateManager.GetCurrentState();
            
            // Initial UI setup based on current state
            UpdateUIForState(lastState);
        }
        
        void Update()
        {
            GameStateEnum currentState = gameStateManager.GetCurrentState();
            
            // Check if state has changed
            if (currentState != lastState)
            {
                UpdateUIForState(currentState);
                lastState = currentState;
            }
        }
        
        private void UpdateUIForState(GameStateEnum state)
        {
            switch (state)
            {
                case GameStateEnum.MainMenu:
                    uiManager?.ShowPauseButton(false);
                    uiManager?.HidePauseMenu();
                    break;
                    
                case GameStateEnum.Playing:
                    uiManager?.ShowPauseButton(true);
                    uiManager?.HidePauseMenu();
                    break;
                    
                case GameStateEnum.Paused:
                    uiManager?.ShowPauseButton(false);
                    uiManager?.ShowPauseMenu();
                    break;
                    
                case GameStateEnum.GameOver:
                    uiManager?.ShowPauseButton(false);
                    uiManager?.HidePauseMenu();
                    break;
            }
        }
    }
}