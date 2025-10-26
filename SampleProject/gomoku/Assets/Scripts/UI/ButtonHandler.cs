using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Gomoku.UI
{
    /// <summary>
    /// Handles UI button interactions for the game
    /// </summary>
    public class ButtonHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        private GameStateManager gameStateManager;
        
        // Events for ButtonAnimationManager
        public System.Action<ButtonHandler> OnButtonHover;
        public System.Action<ButtonHandler> OnButtonClick;
        
        // Animation properties
        private bool isAnimating = false;
        private bool isInteractable = true;
        private float animationDuration = 0.2f;
        private float hoverScale = 1.1f;
        private float clickScale = 0.9f;
        
        // Public properties for existing tests
        public bool IsAnimating => isAnimating;
        public bool IsHovered { get; private set; }
        public bool IsPressed { get; private set; }
        
        void Start()
        {
            gameStateManager = GameStateManager.Instance;
        }
        
        /// <summary>
        /// Called when pause button is clicked
        /// </summary>
        public void OnPauseButtonClicked()
        {
            if (gameStateManager.IsInState(GameStateEnum.Playing))
            {
                gameStateManager.PauseGame();
            }
            OnButtonClick?.Invoke(this);
        }
        
        /// <summary>
        /// Called when start button is clicked
        /// </summary>
        public void OnStartButtonClicked()
        {
            Debug.Log("Start button clicked");
            OnButtonClick?.Invoke(this);
        }
        
        /// <summary>
        /// Called when resume button is clicked
        /// </summary>
        public void OnResumeButtonClicked()
        {
            if (gameStateManager.IsInState(GameStateEnum.Paused))
            {
                gameStateManager.ResumeGame();
            }
            OnButtonClick?.Invoke(this);
        }
        
        /// <summary>
        /// Called when settings button is clicked
        /// </summary>
        public void OnSettingsButtonClicked()
        {
            Debug.Log("Settings button clicked - TODO: Implement settings menu");
            OnButtonClick?.Invoke(this);
        }
        
        /// <summary>
        /// Called when quit button is clicked
        /// </summary>
        public void OnQuitButtonClicked()
        {
            Debug.Log("Quit button clicked - Returning to main menu");
            gameStateManager.ReturnToMainMenu();
            OnButtonClick?.Invoke(this);
        }
        
        // Pointer event handlers for existing tests
        public void OnPointerEnter(PointerEventData eventData)
        {
            IsHovered = true;
            isAnimating = true;
            OnButtonHover?.Invoke(this);
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            IsHovered = false;
            isAnimating = false;
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            IsPressed = true;
            isAnimating = true;
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            IsPressed = false;
            isAnimating = false;
        }
        
        // Methods for ButtonAnimationManager compatibility
        public void SetButtonColors(Color normal, Color highlighted, Color pressed)
        {
            // Implementation for setting button colors
            // This would typically modify the button's colors
        }
        
        public void SetButtonColors(Color normal, Color highlighted, Color pressed, Color disabled)
        {
            // Implementation for setting button colors including disabled state
        }
        
        public void SetAnimationScales(float hoverScale, float clickScale)
        {
            this.hoverScale = hoverScale;
            this.clickScale = clickScale;
        }
        
        public void SetAnimationScales(float normalScale, float hoverScale, float clickScale)
        {
            this.hoverScale = hoverScale;
            this.clickScale = clickScale;
        }
        
        public void SetAnimationDuration(float duration)
        {
            this.animationDuration = duration;
        }
        
        public void SetInteractable(bool interactable)
        {
            this.isInteractable = interactable;
        }
        
        public bool IsAnimatingMethod()
        {
            return isAnimating;
        }
        
        public void TriggerHoverEvent(bool isHovering)
        {
            if (isHovering)
            {
                OnButtonHover?.Invoke(this);
            }
        }
    }
}