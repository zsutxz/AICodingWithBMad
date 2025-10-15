using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Gomoku.GameState;

namespace Gomoku.UI
{
    /// <summary>
    /// Handles pause button functionality with visual feedback
    /// </summary>
    public class PauseButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [Header("Component References")]
        [SerializeField] private Button pauseButton;
        [SerializeField] private Image buttonIcon;
        [SerializeField] private GameStateManager gameStateManager;

        [Header("Visual Settings")]
        [SerializeField] private Sprite pauseIcon;
        [SerializeField] private Sprite resumeIcon;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color hoverColor = new Color(0.8f, 0.8f, 0.8f);
        [SerializeField] private Color pressedColor = new Color(0.6f, 0.6f, 0.6f);

        [Header("Animation Settings")]
        [SerializeField] private float hoverScale = 1.1f;
        [SerializeField] private float clickScale = 0.9f;
        [SerializeField] private float animationDuration = 0.2f;

        private Vector3 originalScale;
        private bool isPaused = false;
        private Coroutine currentAnimation;

        private void Awake()
        {
            InitializePauseButton();
        }

        private void OnDestroy()
        {
            CleanupEvents();
        }

        /// <summary>
        /// Initializes the pause button
        /// </summary>
        private void InitializePauseButton()
        {
            // Auto-find references
            if (pauseButton == null)
                pauseButton = GetComponent<Button>();

            if (buttonIcon == null)
                buttonIcon = GetComponentInChildren<Image>();

            if (gameStateManager == null)
                gameStateManager = FindObjectOfType<GameStateManager>();

            // Store original scale
            originalScale = transform.localScale;

            // Set up button click handler
            if (pauseButton != null)
            {
                pauseButton.onClick.AddListener(OnPauseButtonClicked);
            }

            // Set initial icon
            UpdateButtonIcon(false);

            Debug.Log("PauseButton initialized");
        }

        /// <summary>
        /// Handles pause button click
        /// </summary>
        private void OnPauseButtonClicked()
        {
            TogglePause();
        }

        /// <summary>
        /// Toggles pause state
        /// </summary>
        private void TogglePause()
        {
            if (gameStateManager == null)
            {
                Debug.LogError("GameStateManager reference not found for PauseButton");
                return;
            }

            if (isPaused)
            {
                // Resume game
                gameStateManager.ResumeGame();
                isPaused = false;
            }
            else
            {
                // Pause game
                gameStateManager.PauseGame();
                isPaused = true;
            }

            // Update button icon
            UpdateButtonIcon(isPaused);

            // Play click animation
            StartCoroutine(PlayClickAnimation());

            Debug.Log($"Game {(isPaused ? "paused" : "resumed")}");
        }

        /// <summary>
        /// Updates the button icon based on pause state
        /// </summary>
        /// <param name="paused">Whether the game is paused</param>
        private void UpdateButtonIcon(bool paused)
        {
            if (buttonIcon != null)
            {
                buttonIcon.sprite = paused ? resumeIcon : pauseIcon;
            }
        }

        /// <summary>
        /// Called when pointer enters the button
        /// </summary>
        /// <param name="eventData">Pointer event data</param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (pauseButton != null && pauseButton.interactable)
            {
                StartCoroutine(PlayHoverAnimation(hoverScale));
                if (buttonIcon != null)
                {
                    buttonIcon.color = hoverColor;
                }
            }
        }

        /// <summary>
        /// Called when pointer exits the button
        /// </summary>
        /// <param name="eventData">Pointer event data</param>
        public void OnPointerExit(PointerEventData eventData)
        {
            StartCoroutine(PlayHoverAnimation(1f));
            if (buttonIcon != null)
            {
                buttonIcon.color = normalColor;
            }
        }

        /// <summary>
        /// Called when pointer clicks the button
        /// </summary>
        /// <param name="eventData">Pointer event data</param>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (buttonIcon != null)
            {
                buttonIcon.color = pressedColor;
            }
        }

        /// <summary>
        /// Plays hover animation
        /// </summary>
        /// <param name="targetScale">Target scale for animation</param>
        private System.Collections.IEnumerator PlayHoverAnimation(float targetScale)
        {
            if (currentAnimation != null)
            {
                StopCoroutine(currentAnimation);
            }

            Vector3 startScale = transform.localScale;
            Vector3 endScale = originalScale * targetScale;
            float elapsed = 0f;

            while (elapsed < animationDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / animationDuration;
                transform.localScale = Vector3.Lerp(startScale, endScale, t);
                yield return null;
            }

            transform.localScale = endScale;
            currentAnimation = null;
        }

        /// <summary>
        /// Plays click animation
        /// </summary>
        private System.Collections.IEnumerator PlayClickAnimation()
        {
            Vector3 startScale = transform.localScale;
            Vector3 clickScale = originalScale * this.clickScale;
            float elapsed = 0f;

            // Scale down
            while (elapsed < animationDuration / 2)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (animationDuration / 2);
                transform.localScale = Vector3.Lerp(startScale, clickScale, t);
                yield return null;
            }

            // Scale back up
            elapsed = 0f;
            while (elapsed < animationDuration / 2)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (animationDuration / 2);
                transform.localScale = Vector3.Lerp(clickScale, originalScale, t);
                yield return null;
            }

            transform.localScale = originalScale;
        }

        /// <summary>
        /// Updates the pause state from external source
        /// </summary>
        /// <param name="paused">Whether the game is paused</param>
        public void SetPauseState(bool paused)
        {
            isPaused = paused;
            UpdateButtonIcon(isPaused);
        }

        /// <summary>
        /// Sets the button interactable state
        /// </summary>
        /// <param name="interactable">Whether the button should be interactable</param>
        public void SetInteractable(bool interactable)
        {
            if (pauseButton != null)
            {
                pauseButton.interactable = interactable;
            }
        }

        /// <summary>
        /// Cleans up event subscriptions
        /// </summary>
        private void CleanupEvents()
        {
            if (pauseButton != null)
            {
                pauseButton.onClick.RemoveListener(OnPauseButtonClicked);
            }
        }
    }
}