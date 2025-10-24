using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Gomoku.Animation
{
    /// <summary>
    /// Component for handling visual transitions between game states
    /// </summary>
    public class GameStateTransition : MonoBehaviour
    {
        [Header("Transition Settings")]
        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        [Header("Visual Components")]
        [SerializeField] private CanvasGroup fadeCanvasGroup;
        [SerializeField] private Image fadeOverlay;
        
        // Animation state
        private bool isTransitioning = false;
        private Coroutine currentTransition;
        
        // Events
        public System.Action<GameStateTransition> OnTransitionStart;
        public System.Action<GameStateTransition> OnTransitionComplete;
        
        // Properties
        public bool IsTransitioning => isTransitioning;
        public float FadeDuration => fadeDuration;
        
        private void Awake()
        {
            // Ensure we have required components
            if (fadeCanvasGroup == null)
            {
                fadeCanvasGroup = GetComponent<CanvasGroup>();
            }
            
            // Create fade overlay if not provided
            if (fadeOverlay == null && fadeCanvasGroup != null)
            {
                CreateFadeOverlay();
            }
            
            // Initialize fade state
            if (fadeCanvasGroup != null)
            {
                fadeCanvasGroup.alpha = 0f;
                fadeCanvasGroup.blocksRaycasts = false;
            }
        }
        
        /// <summary>
        /// Starts a fade transition
        /// </summary>
        /// <param name="fadeIn">True to fade in, false to fade out</param>
        public void StartFadeTransition(bool fadeIn)
        {
            if (isTransitioning)
            {
                StopCurrentTransition();
            }
            
            currentTransition = StartCoroutine(AnimateFadeTransition(fadeIn));
        }
        
        /// <summary>
        /// Animates a fade transition
        /// </summary>
        /// <param name="fadeIn">True to fade in, false to fade out</param>
        /// <returns>Coroutine enumerator</returns>
        private IEnumerator AnimateFadeTransition(bool fadeIn)
        {
            isTransitioning = true;
            OnTransitionStart?.Invoke(this);
            
            if (fadeCanvasGroup == null)
            {
                Debug.LogError("GameStateTransition: No CanvasGroup found for fade transition");
                isTransitioning = false;
                yield break;
            }
            
            // Enable raycast blocking during transition
            fadeCanvasGroup.blocksRaycasts = true;
            
            float startAlpha = fadeIn ? 0f : 1f;
            float targetAlpha = fadeIn ? 1f : 0f;
            
            float elapsed = 0f;
            
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeDuration;
                float curveValue = fadeCurve.Evaluate(t);
                
                fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, curveValue);
                
                yield return null;
            }
            
            // Ensure final alpha is exact
            fadeCanvasGroup.alpha = targetAlpha;
            
            // Disable raycast blocking after fade out
            if (!fadeIn)
            {
                fadeCanvasGroup.blocksRaycasts = false;
            }
            
            isTransitioning = false;
            currentTransition = null;
            OnTransitionComplete?.Invoke(this);
        }
        
        /// <summary>
        /// Stops the current transition
        /// </summary>
        public void StopCurrentTransition()
        {
            if (currentTransition != null)
            {
                StopCoroutine(currentTransition);
                currentTransition = null;
            }
            
            ResetTransitionState();
            isTransitioning = false;
        }
        
        /// <summary>
        /// Resets the transition state
        /// </summary>
        private void ResetTransitionState()
        {
            if (fadeCanvasGroup != null)
            {
                fadeCanvasGroup.alpha = 0f;
                fadeCanvasGroup.blocksRaycasts = false;
            }
        }
        
        /// <summary>
        /// Creates a fade overlay if none is provided
        /// </summary>
        private void CreateFadeOverlay()
        {
            if (fadeCanvasGroup == null) return;
            
            // Create a child GameObject for the fade overlay
            GameObject overlayObject = new GameObject("FadeOverlay");
            overlayObject.transform.SetParent(fadeCanvasGroup.transform, false);
            
            // Add Image component
            fadeOverlay = overlayObject.AddComponent<Image>();
            fadeOverlay.color = Color.black;
            
            // Set up RectTransform to cover entire screen
            RectTransform rectTransform = fadeOverlay.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;
            
            Debug.Log("GameStateTransition: Created default fade overlay");
        }
        
        /// <summary>
        /// Sets the fade overlay color
        /// </summary>
        /// <param name="color">Fade color</param>
        public void SetFadeColor(Color color)
        {
            if (fadeOverlay != null)
            {
                fadeOverlay.color = color;
            }
        }
        
        /// <summary>
        /// Sets the fade duration
        /// </summary>
        /// <param name="duration">Fade duration</param>
        public void SetFadeDuration(float duration)
        {
            fadeDuration = Mathf.Max(0.1f, duration);
        }
        
        /// <summary>
        /// Immediately shows the fade overlay
        /// </summary>
        public void ShowImmediate()
        {
            StopCurrentTransition();
            
            if (fadeCanvasGroup != null)
            {
                fadeCanvasGroup.alpha = 1f;
                fadeCanvasGroup.blocksRaycasts = true;
            }
        }
        
        /// <summary>
        /// Immediately hides the fade overlay
        /// </summary>
        public void HideImmediate()
        {
            StopCurrentTransition();
            
            if (fadeCanvasGroup != null)
            {
                fadeCanvasGroup.alpha = 0f;
                fadeCanvasGroup.blocksRaycasts = false;
            }
        }
        
        #if UNITY_EDITOR
        /// <summary>
        /// Editor-only method for testing fade in
        /// </summary>
        [ContextMenu("Test Fade In")]
        private void TestFadeIn()
        {
            if (Application.isPlaying)
            {
                StartFadeTransition(true);
            }
            else
            {
                Debug.Log("Fade transition test can only be run in Play mode");
            }
        }
        
        /// <summary>
        /// Editor-only method for testing fade out
        /// </summary>
        [ContextMenu("Test Fade Out")]
        private void TestFadeOut()
        {
            if (Application.isPlaying)
            {
                StartFadeTransition(false);
            }
            else
            {
                Debug.Log("Fade transition test can only be run in Play mode");
            }
        }
        
        /// <summary>
        /// Editor-only method for resetting the transition
        /// </summary>
        [ContextMenu("Reset Transition")]
        private void ResetTransition()
        {
            StopCurrentTransition();
        }
        #endif
    }
}