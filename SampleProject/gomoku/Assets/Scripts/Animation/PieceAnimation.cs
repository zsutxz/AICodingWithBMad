using UnityEngine;
using Gomoku;
using Gomoku.Core;
using System.Collections;

namespace Gomoku.Animation
{
    /// <summary>
    /// Component for handling piece placement animations with fade-in and scale effects
    /// </summary>
    public class PieceAnimation : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField] private float fadeInDuration = 0.3f;
        [SerializeField] private float scaleDuration = 0.3f;
        [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        [Header("Visual Components")]
        [SerializeField] private SpriteRenderer pieceRenderer;
        
        // Animation state
        private bool isAnimating = false;
        private Coroutine currentAnimation;
        
        // Events
        public System.Action<PieceAnimation> OnAnimationComplete;
        public System.Action<PieceAnimation> OnAnimationStart;
        
        // Properties
        public bool IsAnimating => isAnimating;
        public float FadeInDuration => fadeInDuration;
        public float ScaleDuration => scaleDuration;
        
        private void Awake()
        {
            // Ensure we have a SpriteRenderer reference
            if (pieceRenderer == null)
            {
                pieceRenderer = GetComponent<SpriteRenderer>();
            }
            
            if (pieceRenderer == null)
            {
                Debug.LogError("PieceAnimation: No SpriteRenderer found on GameObject");
            }
        }
        
        /// <summary>
        /// Starts the piece placement animation
        /// </summary>
        /// <param name="animateScale">Whether to animate scale</param>
        /// <param name="animateFade">Whether to animate fade</param>
        public void StartPlacementAnimation(bool animateScale = true, bool animateFade = true)
        {
            if (isAnimating)
            {
                StopCurrentAnimation();
            }
            
            currentAnimation = StartCoroutine(AnimatePlacement(animateScale, animateFade));
        }
        
        /// <summary>
        /// Animates the piece placement with fade-in and scale effects
        /// </summary>
        /// <param name="animateScale">Whether to animate scale</param>
        /// <param name="animateFade">Whether to animate fade</param>
        /// <returns>Coroutine enumerator</returns>
        private IEnumerator AnimatePlacement(bool animateScale, bool animateFade)
        {
            isAnimating = true;
            OnAnimationStart?.Invoke(this);
            
            // Store initial values
            Vector3 initialScale = transform.localScale;
            Color initialColor = pieceRenderer.color;
            
            // Set starting values
            if (animateScale)
            {
                transform.localScale = Vector3.zero;
            }
            
            if (animateFade)
            {
                Color transparentColor = initialColor;
                transparentColor.a = 0f;
                pieceRenderer.color = transparentColor;
            }
            
            float elapsed = 0f;
            float maxDuration = Mathf.Max(fadeInDuration, scaleDuration);
            
            while (elapsed < maxDuration)
            {
                elapsed += Time.deltaTime;
                
                // Animate scale
                if (animateScale)
                {
                    float scaleT = Mathf.Clamp01(elapsed / scaleDuration);
                    float scaleValue = scaleCurve.Evaluate(scaleT);
                    transform.localScale = Vector3.Lerp(Vector3.zero, initialScale, scaleValue);
                }
                
                // Animate fade
                if (animateFade)
                {
                    float fadeT = Mathf.Clamp01(elapsed / fadeInDuration);
                    float fadeValue = fadeCurve.Evaluate(fadeT);
                    Color currentColor = initialColor;
                    currentColor.a = Mathf.Lerp(0f, initialColor.a, fadeValue);
                    pieceRenderer.color = currentColor;
                }
                
                yield return null;
            }
            
            // Ensure final values are exact
            if (animateScale)
            {
                transform.localScale = initialScale;
            }
            
            if (animateFade)
            {
                pieceRenderer.color = initialColor;
            }
            
            isAnimating = false;
            currentAnimation = null;
            OnAnimationComplete?.Invoke(this);
        }
        
        /// <summary>
        /// Stops the current animation
        /// </summary>
        public void StopCurrentAnimation()
        {
            if (currentAnimation != null)
            {
                StopCoroutine(currentAnimation);
                currentAnimation = null;
            }
            
            isAnimating = false;
        }
        
        /// <summary>
        /// Resets the animation state
        /// </summary>
        public void ResetAnimation()
        {
            StopCurrentAnimation();
            
            // Reset visual state
            if (pieceRenderer != null)
            {
                Color color = pieceRenderer.color;
                color.a = 1f;
                pieceRenderer.color = color;
            }
            
            transform.localScale = Vector3.one;
        }
        
        /// <summary>
        /// Sets the animation curves
        /// </summary>
        /// <param name="fadeCurve">Fade animation curve</param>
        /// <param name="scaleCurve">Scale animation curve</param>
        public void SetAnimationCurves(AnimationCurve fadeCurve, AnimationCurve scaleCurve)
        {
            this.fadeCurve = fadeCurve;
            this.scaleCurve = scaleCurve;
        }
        
        /// <summary>
        /// Sets the animation durations
        /// </summary>
        /// <param name="fadeDuration">Fade duration</param>
        /// <param name="scaleDuration">Scale duration</param>
        public void SetAnimationDurations(float fadeDuration, float scaleDuration)
        {
            this.fadeInDuration = Mathf.Max(0f, fadeDuration);
            this.scaleDuration = Mathf.Max(0f, scaleDuration);
        }
        
        #if UNITY_EDITOR
        /// <summary>
        /// Editor-only method for testing the animation
        /// </summary>
        [ContextMenu("Test Animation")]
        private void TestAnimation()
        {
            if (Application.isPlaying)
            {
                StartPlacementAnimation();
            }
            else
            {
                Debug.Log("Animation test can only be run in Play mode");
            }
        }
        
        /// <summary>
        /// Editor-only method for resetting the animation state
        /// </summary>
        [ContextMenu("Reset Animation State")]
        private void ResetAnimationState()
        {
            ResetAnimation();
        }
        #endif
    }
}