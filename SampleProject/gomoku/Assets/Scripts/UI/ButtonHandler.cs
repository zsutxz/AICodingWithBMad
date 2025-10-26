using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using Gomoku.Audio;
using Gomoku.Animation;

namespace Gomoku.UI
{
    /// <summary>
    /// Unified button handler for all UI buttons in the game
    /// </summary>
    public class ButtonHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        [Header("Animation Settings")]
        [SerializeField] private float hoverScale = 1.1f;
        [SerializeField] private float clickScale = 0.95f;
        [SerializeField] private float animationDuration = 0.2f;

        [Header("Game State References")]
        private Gomoku.Systems.GameStateManager gameStateManager;
        private SimpleAnimationSystem animationSystem;

        // Events for existing tests
        public System.Action<ButtonHandler> OnButtonHover;
        public System.Action<ButtonHandler> OnButtonClick;

        // Animation properties
        private Transform buttonTransform;
        private Vector3 originalScale;
        private bool isAnimating = false;
        private bool isInteractable = true;
        private bool isHovered = false;
        private bool isPressed = false;

        // Public properties for existing tests
        public bool IsAnimating => isAnimating;
        public bool IsHovered => isHovered;
        public bool IsPressed => isPressed;
  
        void Awake()
        {
            buttonTransform = transform;
            originalScale = buttonTransform.localScale;
        }

        void Start()
        {
            gameStateManager = Gomoku.Systems.GameStateManager.Instance;
            animationSystem = SimpleAnimationSystem.Instance;
        }
        
        /// <summary>
        /// Called when pause button is clicked
        /// </summary>
        public void OnPauseButtonClicked()
        {
            if (gameStateManager.IsInState(Gomoku.Systems.GameStateEnum.Playing))
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

            // Try to find MainMenuScreen first
            var mainMenuScreen = FindObjectOfType<Gomoku.UI.MainMenu.MainMenuScreen>();
            if (mainMenuScreen != null)
            {
                Debug.Log("MainMenuScreen found, using it to start game");
                mainMenuScreen.StartGame();
            }
            else
            {
                Debug.Log("MainMenuScreen not found, using direct game start");
                // Direct: Set game state and load scene directly
                gameStateManager.SetState(Gomoku.Systems.GameStateEnum.Playing);
                UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
            }

            OnButtonClick?.Invoke(this);
        }
        
        /// <summary>
        /// Called when resume button is clicked
        /// </summary>
        public void OnResumeButtonClicked()
        {
            if (gameStateManager.IsInState(Gomoku.Systems.GameStateEnum.Paused))
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
            isHovered = true;
            isAnimating = true;
            if (animationSystem != null)
            {
                StopAllCoroutines();
                StartCoroutine(animationSystem.AnimateScale(buttonTransform, buttonTransform.localScale, originalScale * hoverScale, animationDuration));
            }
            OnButtonHover?.Invoke(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isHovered = false;
            isAnimating = false;
            if (animationSystem != null)
            {
                StopAllCoroutines();
                StartCoroutine(animationSystem.AnimateScale(buttonTransform, buttonTransform.localScale, originalScale, animationDuration));
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isPressed = true;
            isAnimating = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isPressed = false;
            isAnimating = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // Play UI click sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayUIClick();
            }

            if (animationSystem != null)
            {
                StopAllCoroutines();
                StartCoroutine(ClickAnimation());
            }
        }

        // Animation method
        private IEnumerator ClickAnimation()
        {
            // Scale down
            yield return animationSystem.AnimateScale(buttonTransform, buttonTransform.localScale, originalScale * clickScale, animationDuration);
            // Scale back to appropriate scale (hover or original)
            float targetScale = isHovered ? hoverScale : 1.0f;
            yield return animationSystem.AnimateScale(buttonTransform, buttonTransform.localScale, originalScale * targetScale, animationDuration);
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