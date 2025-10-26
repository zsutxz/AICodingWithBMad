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
        private Gomoku.Systems.GameStateManager gameStateManager;
        private Gomoku.Systems.GameStateEnum lastState;
        
        void Start()
        {
            uiManager = FindObjectOfType<UIManager>();
            gameStateManager = Gomoku.Systems.GameStateManager.Instance;
            lastState = gameStateManager.GetCurrentState();
            
            // Initial UI setup based on current state
            UpdateUIForState(lastState);
        }
        
        void Update()
        {
            Gomoku.Systems.GameStateEnum currentState = gameStateManager.GetCurrentState();
            
            // Check if state has changed
            if (currentState != lastState)
            {
                UpdateUIForState(currentState);
                lastState = currentState;
            }
        }
        
        private void UpdateUIForState(Gomoku.Systems.GameStateEnum state)
        {
            switch (state)
            {
                case Gomoku.Systems.GameStateEnum.MainMenu:
                    uiManager?.ShowPauseButton(false);
                    uiManager?.HidePauseMenu();
                    break;
                    
                case Gomoku.Systems.GameStateEnum.Playing:
                    uiManager?.ShowPauseButton(true);
                    uiManager?.HidePauseMenu();
                    break;
                    
                case Gomoku.Systems.GameStateEnum.Paused:
                    uiManager?.ShowPauseButton(false);
                    uiManager?.ShowPauseMenu();
                    break;
                    
                case Gomoku.Systems.GameStateEnum.GameOver:
                    uiManager?.ShowPauseButton(false);
                    uiManager?.HidePauseMenu();
                    break;
            }
        }
    }
}