using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Gomoku;
using Gomoku.Core;

namespace Gomoku.Animation
{
    /// <summary>
    /// Component for highlighting winning pieces with glow and pulse effects
    /// </summary>
    public class VictoryEffect : MonoBehaviour
    {
        [Header("Victory Effect Settings")]
        [SerializeField] private float glowDuration = 1.5f;
        [SerializeField] private float pulseIntensity = 2f;
        [SerializeField] private AnimationCurve pulseCurve = AnimationCurve.EaseInOut(0, 1, 1, 2);
        [SerializeField] private Color glowColor = Color.yellow;
        
        [Header("Visual Components")]
        [SerializeField] private SpriteRenderer pieceRenderer;
        [SerializeField] private Material glowMaterial;
        
        // Animation state
        private bool isAnimating = false;
        private Coroutine victoryAnimation;
        private Material originalMaterial;
        private Color originalColor;
        
        // Events
        public System.Action<VictoryEffect> OnVictoryEffectStart;
        public System.Action<VictoryEffect> OnVictoryEffectComplete;
        
        // Properties
        public bool IsAnimating => isAnimating;
        public float GlowDuration => glowDuration;
        
        private void Awake()
        {
            // Ensure we have a SpriteRenderer reference
            if (pieceRenderer == null)
            {
                pieceRenderer = GetComponent<SpriteRenderer>();
            }
            
            if (pieceRenderer == null)
            {
                Debug.LogError("VictoryEffect: No SpriteRenderer found on GameObject");
                return;
            }
            
            // Store original material and color
            originalMaterial = pieceRenderer.material;
            originalColor = pieceRenderer.color;
            
            // If no glow material specified, create a basic one
            if (glowMaterial == null)
            {
                CreateDefaultGlowMaterial();
            }
        }
        
        /// <summary>
        /// Starts the victory highlight animation
        /// </summary>
        public void StartVictoryHighlight()
        {
            if (isAnimating)
            {
                StopVictoryAnimation();
            }
            
            victoryAnimation = StartCoroutine(AnimateVictoryHighlight());
        }
        
        /// <summary>
        /// Animates the victory highlight with glow and pulse effects
        /// </summary>
        /// <returns>Coroutine enumerator</returns>
        private IEnumerator AnimateVictoryHighlight()
        {
            isAnimating = true;
            OnVictoryEffectStart?.Invoke(this);
            
            // Apply glow material
            if (glowMaterial != null)
            {
                pieceRenderer.material = glowMaterial;
            }
            
            float elapsed = 0f;
            
            while (elapsed < glowDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / glowDuration;
                float pulseValue = pulseCurve.Evaluate(t);
                
                // Apply pulse effect to scale
                transform.localScale = Vector3.one * pulseValue;
                
                // Apply glow color effect
                Color currentColor = Color.Lerp(originalColor, glowColor, Mathf.PingPong(t * 2f, 1f));
                pieceRenderer.color = currentColor;
                
                yield return null;
            }
            
            // Reset to original state
            ResetVisualState();
            
            isAnimating = false;
            victoryAnimation = null;
            OnVictoryEffectComplete?.Invoke(this);
        }
        
        /// <summary>
        /// Stops the victory animation
        /// </summary>
        public void StopVictoryAnimation()
        {
            if (victoryAnimation != null)
            {
                StopCoroutine(victoryAnimation);
                victoryAnimation = null;
            }
            
            ResetVisualState();
            isAnimating = false;
        }
        
        /// <summary>
        /// Resets the visual state to original
        /// </summary>
        private void ResetVisualState()
        {
            if (pieceRenderer != null)
            {
                pieceRenderer.material = originalMaterial;
                pieceRenderer.color = originalColor;
                transform.localScale = Vector3.one;
            }
        }
        
        /// <summary>
        /// Creates a default glow material for the victory effect
        /// </summary>
        private void CreateDefaultGlowMaterial()
        {
            // In a real implementation, you would create a custom shader material here
            // For now, we'll use a standard material with increased emission
            glowMaterial = new Material(Shader.Find("Standard"));
            glowMaterial.EnableKeyword("_EMISSION");
            glowMaterial.SetColor("_EmissionColor", glowColor);
            
            Debug.Log("VictoryEffect: Created default glow material");
        }
        
        /// <summary>
        /// Sets the glow color for the victory effect
        /// </summary>
        /// <param name="color">Glow color</param>
        public void SetGlowColor(Color color)
        {
            glowColor = color;
            
            // Update material if it exists
            if (glowMaterial != null)
            {
                glowMaterial.SetColor("_EmissionColor", glowColor);
            }
        }
        
        /// <summary>
        /// Sets the victory effect duration
        /// </summary>
        /// <param name="duration">Animation duration</param>
        public void SetGlowDuration(float duration)
        {
            glowDuration = Mathf.Max(0.1f, duration);
        }
        
        /// <summary>
        /// Sets the pulse intensity
        /// </summary>
        /// <param name="intensity">Pulse intensity multiplier</param>
        public void SetPulseIntensity(float intensity)
        {
            pulseIntensity = Mathf.Max(1f, intensity);
        }
        
        #if UNITY_EDITOR
        /// <summary>
        /// Editor-only method for testing the victory effect
        /// </summary>
        [ContextMenu("Test Victory Effect")]
        private void TestVictoryEffect()
        {
            if (Application.isPlaying)
            {
                StartVictoryHighlight();
            }
            else
            {
                Debug.Log("Victory effect test can only be run in Play mode");
            }
        }
        
        /// <summary>
        /// Editor-only method for resetting the victory effect
        /// </summary>
        [ContextMenu("Reset Victory Effect")]
        private void ResetVictoryEffect()
        {
            StopVictoryAnimation();
        }
        #endif
    }
}