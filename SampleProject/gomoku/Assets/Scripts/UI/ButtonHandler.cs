using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using Gomoku.Audio;

namespace Gomoku.UI
{
    /// <summary>
    /// Enhanced button handler with hover and click animations
    /// </summary>
    public class ButtonHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Animation Settings")]
        [SerializeField] private float hoverScale = 1.1f;
        [SerializeField] private float clickScale = 0.9f;
        [SerializeField] private float animationDuration = 0.2f;
        [SerializeField] private AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        [Header("Color Settings")]
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color hoverColor = new Color(0.9f, 0.9f, 0.9f, 1f);
        [SerializeField] private Color clickColor = new Color(0.8f, 0.8f, 0.8f, 1f);
        
        [Header("Visual Components")]
        [SerializeField] private Button button;
        [SerializeField] private Image buttonImage;
        [SerializeField] private TMPro.TextMeshProUGUI buttonText;
        
        [Header("Audio Settings")]
        [SerializeField] private bool playAudioFeedback = true;
        [SerializeField] private bool playHoverSound = true;
        [SerializeField] private bool playClickSound = true;
        
        // Animation state
        private bool isAnimating = false;
        private Coroutine currentAnimation;
        private Vector3 originalScale;
        private Color originalTextColor;
        
        // Button state
        private bool isHovered = false;
        private bool isPressed = false;
        
        // Events
        public System.Action<ButtonHandler> OnButtonHover;
        public System.Action<ButtonHandler> OnButtonClick;
        
        // Properties
        public bool IsAnimating => isAnimating;
        public bool IsHovered => isHovered;
        public bool IsPressed => isPressed;
        
        private void Awake()
        {
            // Ensure we have required components
            if (button == null)
            {
                button = GetComponent<Button>();
            }
            
            if (buttonImage == null)
            {
                buttonImage = GetComponent<Image>();
            }
            
            if (buttonText == null)
            {
                buttonText = GetComponentInChildren<TMPro.TextMeshProUGUI>();
            }
            
            // Store original values
            originalScale = transform.localScale;
            
            if (buttonText != null)
            {
                originalTextColor = buttonText.color;
            }
            
            // Set up button click listener
            if (button != null)
            {
                button.onClick.AddListener(HandleButtonClick);
            }
        }
        
        private void OnDestroy()
        {
            // Clean up event listeners
            if (button != null)
            {
                button.onClick.RemoveListener(HandleButtonClick);
            }
        }
        
        /// <summary>
        /// Handles pointer enter (hover) event
        /// </summary>
        /// <param name="eventData">Pointer event data</param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!button.interactable) return;
            
            isHovered = true;
            StartAnimation(ButtonState.Hover);
            
            // Play hover sound
            if (playAudioFeedback && playHoverSound)
            {
                AudioManager.Instance.PlayUIHoverSound();
            }
            
            OnButtonHover?.Invoke(this);
        }
        
        /// <summary>
        /// Handles pointer exit event
        /// </summary>
        /// <param name="eventData">Pointer event data</param>
        public void OnPointerExit(PointerEventData eventData)
        {
            isHovered = false;
            isPressed = false;
            StartAnimation(ButtonState.Normal);
        }
        
        /// <summary>
        /// Handles pointer down (press) event
        /// </summary>
        /// <param name="eventData">Pointer event data</param>
        public void OnPointerDown(PointerEventData eventData)
        {
            if (!button.interactable) return;
            
            isPressed = true;
            StartAnimation(ButtonState.Pressed);
        }
        
        /// <summary>
        /// Handles pointer up (release) event
        /// </summary>
        /// <param name="eventData">Pointer event data</param>
        public void OnPointerUp(PointerEventData eventData)
        {
            if (!button.interactable) return;
            
            isPressed = false;
            StartAnimation(isHovered ? ButtonState.Hover : ButtonState.Normal);
        }
        
        /// <summary>
        /// Handles button click event
        /// </summary>
        private void HandleButtonClick()
        {
            if (!button.interactable) return;
            
            // Play click sound
            if (playAudioFeedback && playClickSound)
            {
                AudioManager.Instance.PlayUIClick();
            }
            
            OnButtonClick?.Invoke(this);
            
            // Add click animation
            StartAnimation(ButtonState.Click);
        }
        
        /// <summary>
        /// Starts the button animation
        /// </summary>
        /// <param name="targetState">Target button state</param>
        private void StartAnimation(ButtonState targetState)
        {
            if (isAnimating)
            {
                StopCurrentAnimation();
            }
            
            currentAnimation = StartCoroutine(AnimateButton(targetState));
        }
        
        /// <summary>
        /// Animates the button to the target state
        /// </summary>
        /// <param name="targetState">Target button state</param>
        /// <returns>Coroutine enumerator</returns>
        private IEnumerator AnimateButton(ButtonState targetState)
        {
            isAnimating = true;
            
            Vector3 targetScale = GetTargetScale(targetState);
            Color targetColor = GetTargetColor(targetState);
            Color targetTextColor = GetTargetTextColor(targetState);
            
            Vector3 startScale = transform.localScale;
            Color startColor = buttonImage != null ? buttonImage.color : Color.white;
            Color startTextColor = buttonText != null ? buttonText.color : Color.white;
            
            float elapsed = 0f;
            
            while (elapsed < animationDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / animationDuration;
                float curveValue = animationCurve.Evaluate(t);
                
                // Animate scale
                transform.localScale = Vector3.Lerp(startScale, targetScale, curveValue);
                
                // Animate colors
                if (buttonImage != null)
                {
                    buttonImage.color = Color.Lerp(startColor, targetColor, curveValue);
                }
                
                if (buttonText != null)
                {
                    buttonText.color = Color.Lerp(startTextColor, targetTextColor, curveValue);
                }
                
                yield return null;
            }
            
            // Ensure final values are exact
            transform.localScale = targetScale;
            
            if (buttonImage != null)
            {
                buttonImage.color = targetColor;
            }
            
            if (buttonText != null)
            {
                buttonText.color = targetTextColor;
            }
            
            isAnimating = false;
            currentAnimation = null;
        }
        
        /// <summary>
        /// Stops the current animation
        /// </summary>
        private void StopCurrentAnimation()
        {
            if (currentAnimation != null)
            {
                StopCoroutine(currentAnimation);
                currentAnimation = null;
            }
            
            isAnimating = false;
        }
        
        /// <summary>
        /// Gets the target scale for a button state
        /// </summary>
        /// <param name="state">Button state</param>
        /// <returns>Target scale</returns>
        private Vector3 GetTargetScale(ButtonState state)
        {
            return state switch
            {
                ButtonState.Normal => originalScale,
                ButtonState.Hover => originalScale * hoverScale,
                ButtonState.Pressed => originalScale * clickScale,
                ButtonState.Click => originalScale,
                _ => originalScale
            };
        }
        
        /// <summary>
        /// Gets the target color for a button state
        /// </summary>
        /// <param name="state">Button state</param>
        /// <returns>Target color</returns>
        private Color GetTargetColor(ButtonState state)
        {
            return state switch
            {
                ButtonState.Normal => normalColor,
                ButtonState.Hover => hoverColor,
                ButtonState.Pressed => clickColor,
                ButtonState.Click => normalColor,
                _ => normalColor
            };
        }
        
        /// <summary>
        /// Gets the target text color for a button state
        /// </summary>
        /// <param name="state">Button state</param>
        /// <returns>Target text color</returns>
        private Color GetTargetTextColor(ButtonState state)
        {
            return state switch
            {
                ButtonState.Normal => originalTextColor,
                ButtonState.Hover => originalTextColor,
                ButtonState.Pressed => originalTextColor,
                ButtonState.Click => originalTextColor,
                _ => originalTextColor
            };
        }
        
        /// <summary>
        /// Sets the button interactable state
        /// </summary>
        /// <param name="interactable">Whether the button is interactable</param>
        public void SetInteractable(bool interactable)
        {
            if (button != null)
            {
                button.interactable = interactable;
            }
            
            if (!interactable)
            {
                StopCurrentAnimation();
                ResetToNormalState();
            }
        }
        
        /// <summary>
        /// Resets the button to normal state
        /// </summary>
        private void ResetToNormalState()
        {
            isHovered = false;
            isPressed = false;
            
            transform.localScale = originalScale;
            
            if (buttonImage != null)
            {
                buttonImage.color = normalColor;
            }
            
            if (buttonText != null)
            {
                buttonText.color = originalTextColor;
            }
        }
        
        /// <summary>
        /// Sets the button colors
        /// </summary>
        /// <param name="normal">Normal color</param>
        /// <param name="hover">Hover color</param>
        /// <param name="click">Click color</param>
        public void SetButtonColors(Color normal, Color hover, Color click)
        {
            normalColor = normal;
            hoverColor = hover;
            clickColor = click;
        }
        
        /// <summary>
        /// Sets the animation scales
        /// </summary>
        /// <param name="hoverScale">Hover scale multiplier</param>
        /// <param name="clickScale">Click scale multiplier</param>
        public void SetAnimationScales(float hoverScale, float clickScale)
        {
            this.hoverScale = Mathf.Max(1f, hoverScale);
            this.clickScale = Mathf.Max(0.5f, clickScale);
        }
        
        /// <summary>
        /// Sets the animation duration
        /// </summary>
        /// <param name="duration">Animation duration</param>
        public void SetAnimationDuration(float duration)
        {
            animationDuration = Mathf.Max(0.1f, duration);
        }
        
        #if UNITY_EDITOR
        /// <summary>
        /// Editor-only method for testing button animations
        /// </summary>
        [ContextMenu("Test Button Animation")]
        private void TestButtonAnimation()
        {
            if (Application.isPlaying)
            {
                StartAnimation(ButtonState.Hover);
            }
            else
            {
                Debug.Log("Button animation test can only be run in Play mode");
            }
        }
        
        /// <summary>
        /// Editor-only method for resetting button state
        /// </summary>
        [ContextMenu("Reset Button State")]
        private void ResetButtonState()
        {
            StopCurrentAnimation();
            ResetToNormalState();
        }
        #endif
    }
    
    /// <summary>
    /// Button states for animation
    /// </summary>
    public enum ButtonState
    {
        Normal,
        Hover,
        Pressed,
        Click
    }
}