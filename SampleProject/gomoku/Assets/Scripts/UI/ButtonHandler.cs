using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace Gomoku.UI
{
    /// <summary>
    /// Provides standardized button interaction feedback and behavior
    /// </summary>
    public class ButtonHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [Header("Component References")]
        [SerializeField] private Button button;
        [SerializeField] private Image buttonImage;
        [SerializeField] private TextMeshProUGUI buttonText;

        [Header("Visual Settings")]
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color hoverColor = new Color(0.8f, 0.8f, 0.8f);
        [SerializeField] private Color pressedColor = new Color(0.6f, 0.6f, 0.6f);
        [SerializeField] private Color disabledColor = new Color(0.5f, 0.5f, 0.5f);

        [Header("Animation Settings")]
        [SerializeField] private float hoverScale = 1.05f;
        [SerializeField] private float clickScale = 0.95f;
        [SerializeField] private float animationDuration = 0.15f;

        private Vector3 originalScale;
        private Coroutine currentAnimation;

        private void Awake()
        {
            InitializeButtonHandler();
        }

        /// <summary>
        /// Initializes the button handler
        /// </summary>
        private void InitializeButtonHandler()
        {
            // Auto-find references
            if (button == null)
                button = GetComponent<Button>();

            if (buttonImage == null)
                buttonImage = GetComponent<Image>();

            if (buttonText == null)
                buttonText = GetComponentInChildren<TextMeshProUGUI>();

            // Store original scale
            originalScale = transform.localScale;

            // Set initial colors
            UpdateVisualState();

            Debug.Log("ButtonHandler initialized");
        }

        /// <summary>
        /// Called when pointer enters the button
        /// </summary>
        /// <param name="eventData">Pointer event data</param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (button != null && button.interactable)
            {
                StartCoroutine(PlayHoverAnimation(hoverScale));
                SetColors(hoverColor);
            }
        }

        /// <summary>
        /// Called when pointer exits the button
        /// </summary>
        /// <param name="eventData">Pointer event data</param>
        public void OnPointerExit(PointerEventData eventData)
        {
            StartCoroutine(PlayHoverAnimation(1f));
            UpdateVisualState();
        }

        /// <summary>
        /// Called when pointer clicks the button
        /// </summary>
        /// <param name="eventData">Pointer event data</param>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (button != null && button.interactable)
            {
                StartCoroutine(PlayClickAnimation());
                SetColors(pressedColor);
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
            Vector3 clickScale = originalScale * clickScale;
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
        /// Sets colors for button elements
        /// </summary>
        /// <param name="color">Color to apply</param>
        private void SetColors(Color color)
        {
            if (buttonImage != null)
            {
                buttonImage.color = color;
            }

            if (buttonText != null)
            {
                buttonText.color = color;
            }
        }

        /// <summary>
        /// Updates visual state based on button interactability
        /// </summary>
        private void UpdateVisualState()
        {
            if (button != null)
            {
                Color targetColor = button.interactable ? normalColor : disabledColor;
                SetColors(targetColor);
            }
            else
            {
                SetColors(normalColor);
            }
        }

        /// <summary>
        /// Sets the button interactable state
        /// </summary>
        /// <param name="interactable">Whether the button should be interactable</param>
        public void SetInteractable(bool interactable)
        {
            if (button != null)
            {
                button.interactable = interactable;
            }
            UpdateVisualState();
        }

        /// <summary>
        /// Gets the button interactable state
        /// </summary>
        /// <returns>Whether the button is interactable</returns>
        public bool IsInteractable()
        {
            return button != null && button.interactable;
        }

        /// <summary>
        /// Adds a click listener to the button
        /// </summary>
        /// <param name="action">Action to execute on click</param>
        public void AddClickListener(UnityEngine.Events.UnityAction action)
        {
            if (button != null)
            {
                button.onClick.AddListener(action);
            }
        }

        /// <summary>
        /// Removes all click listeners from the button
        /// </summary>
        public void RemoveAllClickListeners()
        {
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
            }
        }

        /// <summary>
        /// Sets the button text
        /// </summary>
        /// <param name="text">Text to display</param>
        public void SetText(string text)
        {
            if (buttonText != null)
            {
                buttonText.text = text;
            }
        }

        /// <summary>
        /// Gets the current button text
        /// </summary>
        /// <returns>Current button text</returns>
        public string GetText()
        {
            return buttonText != null ? buttonText.text : string.Empty;
        }
    }
}