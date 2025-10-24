using UnityEngine;

namespace Gomoku.Animation
{
    /// <summary>
    /// Central configuration for animation performance and quality settings
    /// </summary>
    [CreateAssetMenu(fileName = "AnimationSettings", menuName = "Gomoku/Animation Settings")]
    public class AnimationSettings : ScriptableObject
    {
        [Header("Global Animation Settings")]
        [SerializeField] private bool enableAnimations = true;
        [SerializeField] private AnimationQuality animationQuality = AnimationQuality.High;
        
        [Header("Performance Limits")]
        [SerializeField] private int maxConcurrentAnimations = 10;
        [SerializeField] private int pieceAnimationPoolSize = 20;
        [SerializeField] private int victoryEffectPoolSize = 5;
        
        [Header("Quality Settings")]
        [SerializeField] private bool enableParticleEffects = true;
        [SerializeField] private int maxParticlesPerEffect = 50;
        [SerializeField] private bool enableComplexShaders = true;
        [SerializeField] private bool enablePostProcessing = true;
        
        [Header("Animation Durations")]
        [SerializeField] private float piecePlacementDuration = 0.3f;
        [SerializeField] private float victoryHighlightDuration = 1.5f;
        [SerializeField] private float turnTransitionDuration = 0.5f;
        [SerializeField] private float stateTransitionDuration = 0.5f;
        [SerializeField] private float buttonAnimationDuration = 0.2f;
        
        [Header("Frame Rate Optimization")]
        [SerializeField] private bool limitAnimationFrameRate = false;
        [SerializeField] private int targetAnimationFrameRate = 30;
        [SerializeField] private bool useFixedTimeStepForAnimations = false;
        
        // Events
        public System.Action<bool> OnAnimationsToggled;
        public System.Action<AnimationQuality> OnQualityChanged;
        
        // Properties
        public bool EnableAnimations => enableAnimations;
        public AnimationQuality AnimationQuality => animationQuality;
        public int MaxConcurrentAnimations => maxConcurrentAnimations;
        public int PieceAnimationPoolSize => pieceAnimationPoolSize;
        public int VictoryEffectPoolSize => victoryEffectPoolSize;
        public bool EnableParticleEffects => enableParticleEffects;
        public int MaxParticlesPerEffect => maxParticlesPerEffect;
        public bool EnableComplexShaders => enableComplexShaders;
        public bool EnablePostProcessing => enablePostProcessing;
        public float PiecePlacementDuration => piecePlacementDuration;
        public float VictoryHighlightDuration => victoryHighlightDuration;
        public float TurnTransitionDuration => turnTransitionDuration;
        public float StateTransitionDuration => stateTransitionDuration;
        public float ButtonAnimationDuration => buttonAnimationDuration;
        public bool LimitAnimationFrameRate => limitAnimationFrameRate;
        public int TargetAnimationFrameRate => targetAnimationFrameRate;
        public bool UseFixedTimeStepForAnimations => useFixedTimeStepForAnimations;
        
        /// <summary>
        /// Sets whether animations are enabled
        /// </summary>
        /// <param name="enabled">Whether animations are enabled</param>
        public void SetAnimationsEnabled(bool enabled)
        {
            if (enableAnimations != enabled)
            {
                enableAnimations = enabled;
                OnAnimationsToggled?.Invoke(enabled);
            }
        }
        
        /// <summary>
        /// Sets the animation quality level
        /// </summary>
        /// <param name="quality">Animation quality level</param>
        public void SetAnimationQuality(AnimationQuality quality)
        {
            if (animationQuality != quality)
            {
                animationQuality = quality;
                ApplyQualitySettings(quality);
                OnQualityChanged?.Invoke(quality);
            }
        }
        
        /// <summary>
        /// Applies quality-specific settings
        /// </summary>
        /// <param name="quality">Quality level to apply</param>
        private void ApplyQualitySettings(AnimationQuality quality)
        {
            switch (quality)
            {
                case AnimationQuality.Low:
                    enableParticleEffects = false;
                    enableComplexShaders = false;
                    enablePostProcessing = false;
                    maxConcurrentAnimations = 5;
                    pieceAnimationPoolSize = 10;
                    victoryEffectPoolSize = 3;
                    limitAnimationFrameRate = true;
                    targetAnimationFrameRate = 20;
                    break;
                    
                case AnimationQuality.Medium:
                    enableParticleEffects = true;
                    enableComplexShaders = false;
                    enablePostProcessing = false;
                    maxConcurrentAnimations = 8;
                    pieceAnimationPoolSize = 15;
                    victoryEffectPoolSize = 4;
                    limitAnimationFrameRate = true;
                    targetAnimationFrameRate = 30;
                    break;
                    
                case AnimationQuality.High:
                    enableParticleEffects = true;
                    enableComplexShaders = true;
                    enablePostProcessing = true;
                    maxConcurrentAnimations = 15;
                    pieceAnimationPoolSize = 25;
                    victoryEffectPoolSize = 6;
                    limitAnimationFrameRate = false;
                    targetAnimationFrameRate = 60;
                    break;
            }
        }
        
        /// <summary>
        /// Sets performance limits
        /// </summary>
        /// <param name="maxAnimations">Maximum concurrent animations</param>
        /// <param name="piecePoolSize">Piece animation pool size</param>
        /// <param name="victoryPoolSize">Victory effect pool size</param>
        public void SetPerformanceLimits(int maxAnimations, int piecePoolSize, int victoryPoolSize)
        {
            maxConcurrentAnimations = Mathf.Max(1, maxAnimations);
            pieceAnimationPoolSize = Mathf.Max(5, piecePoolSize);
            victoryEffectPoolSize = Mathf.Max(2, victoryPoolSize);
        }
        
        /// <summary>
        /// Sets quality features
        /// </summary>
        /// <param name="particles">Enable particle effects</param>
        /// <param name="shaders">Enable complex shaders</param>
        /// <param name="postProcessing">Enable post processing</param>
        public void SetQualityFeatures(bool particles, bool shaders, bool postProcessing)
        {
            enableParticleEffects = particles;
            enableComplexShaders = shaders;
            enablePostProcessing = postProcessing;
        }
        
        /// <summary>
        /// Sets animation durations
        /// </summary>
        /// <param name="pieceDuration">Piece placement duration</param>
        /// <param name="victoryDuration">Victory highlight duration</param>
        /// <param name="turnDuration">Turn transition duration</param>
        /// <param name="stateDuration">State transition duration</param>
        /// <param name="buttonDuration">Button animation duration</param>
        public void SetAnimationDurations(float pieceDuration, float victoryDuration, float turnDuration, 
                                         float stateDuration, float buttonDuration)
        {
            piecePlacementDuration = Mathf.Max(0.1f, pieceDuration);
            victoryHighlightDuration = Mathf.Max(0.5f, victoryDuration);
            turnTransitionDuration = Mathf.Max(0.1f, turnDuration);
            stateTransitionDuration = Mathf.Max(0.1f, stateDuration);
            buttonAnimationDuration = Mathf.Max(0.05f, buttonDuration);
        }
        
        /// <summary>
        /// Sets frame rate optimization settings
        /// </summary>
        /// <param name="limitFrameRate">Whether to limit frame rate</param>
        /// <param name="targetFrameRate">Target frame rate</param>
        /// <param name="useFixedTimeStep">Whether to use fixed time step</param>
        public void SetFrameRateOptimization(bool limitFrameRate, int targetFrameRate, bool useFixedTimeStep)
        {
            limitAnimationFrameRate = limitFrameRate;
            targetAnimationFrameRate = Mathf.Max(15, targetFrameRate);
            useFixedTimeStepForAnimations = useFixedTimeStep;
        }
        
        /// <summary>
        /// Gets the recommended animation duration for a specific type
        /// </summary>
        /// <param name="animationType">Type of animation</param>
        /// <returns>Recommended duration</returns>
        public float GetRecommendedDuration(AnimationType animationType)
        {
            return animationType switch
            {
                AnimationType.PiecePlacement => piecePlacementDuration,
                AnimationType.VictoryHighlight => victoryHighlightDuration,
                AnimationType.TurnTransition => turnTransitionDuration,
                AnimationType.StateTransition => stateTransitionDuration,
                AnimationType.ButtonInteraction => buttonAnimationDuration,
                _ => 0.3f
            };
        }
        
        /// <summary>
        /// Checks if a specific feature is enabled for the current quality level
        /// </summary>
        /// <param name="feature">Feature to check</param>
        /// <returns>True if feature is enabled</returns>
        public bool IsFeatureEnabled(AnimationFeature feature)
        {
            return feature switch
            {
                AnimationFeature.ParticleEffects => enableParticleEffects,
                AnimationFeature.ComplexShaders => enableComplexShaders,
                AnimationFeature.PostProcessing => enablePostProcessing,
                _ => true
            };
        }
        
        /// <summary>
        /// Resets to default settings
        /// </summary>
        public void ResetToDefaults()
        {
            enableAnimations = true;
            animationQuality = AnimationQuality.High;
            maxConcurrentAnimations = 10;
            pieceAnimationPoolSize = 20;
            victoryEffectPoolSize = 5;
            enableParticleEffects = true;
            maxParticlesPerEffect = 50;
            enableComplexShaders = true;
            enablePostProcessing = true;
            piecePlacementDuration = 0.3f;
            victoryHighlightDuration = 1.5f;
            turnTransitionDuration = 0.5f;
            stateTransitionDuration = 0.5f;
            buttonAnimationDuration = 0.2f;
            limitAnimationFrameRate = false;
            targetAnimationFrameRate = 30;
            useFixedTimeStepForAnimations = false;
        }
        
        #if UNITY_EDITOR
        /// <summary>
        /// Editor-only method for testing animation settings
        /// </summary>
        [ContextMenu("Log Animation Settings")]
        private void LogAnimationSettings()
        {
            Debug.Log($"Animation Settings:");
            Debug.Log($"- Enabled: {enableAnimations}");
            Debug.Log($"- Quality: {animationQuality}");
            Debug.Log($"- Max Concurrent: {maxConcurrentAnimations}");
            Debug.Log($"- Piece Pool: {pieceAnimationPoolSize}");
            Debug.Log($"- Victory Pool: {victoryEffectPoolSize}");
            Debug.Log($"- Particles: {enableParticleEffects}");
            Debug.Log($"- Complex Shaders: {enableComplexShaders}");
            Debug.Log($"- Post Processing: {enablePostProcessing}");
            Debug.Log($"- Frame Rate Limit: {limitAnimationFrameRate} ({targetAnimationFrameRate} FPS)");
            Debug.Log($"- Fixed Time Step: {useFixedTimeStepForAnimations}");
        }
        
        /// <summary>
        /// Editor-only method for applying quality settings
        /// </summary>
        [ContextMenu("Apply Low Quality Settings")]
        private void ApplyLowQualitySettings()
        {
            SetAnimationQuality(AnimationQuality.Low);
        }
        
        /// <summary>
        /// Editor-only method for applying quality settings
        /// </summary>
        [ContextMenu("Apply Medium Quality Settings")]
        private void ApplyMediumQualitySettings()
        {
            SetAnimationQuality(AnimationQuality.Medium);
        }
        
        /// <summary>
        /// Editor-only method for applying quality settings
        /// </summary>
        [ContextMenu("Apply High Quality Settings")]
        private void ApplyHighQualitySettings()
        {
            SetAnimationQuality(AnimationQuality.High);
        }
        
        /// <summary>
        /// Editor-only method for resetting to defaults
        /// </summary>
        [ContextMenu("Reset to Defaults")]
        private void ResetToDefaultsEditor()
        {
            ResetToDefaults();
        }
        #endif
    }
    
    ///// <summary>
    ///// Animation quality levels
    ///// </summary>
    //public enum AnimationQuality
    //{
    //    Low,
    //    Medium,
    //    High
    //}
    
    /// <summary>
    /// Types of animations
    /// </summary>
    public enum AnimationType
    {
        PiecePlacement,
        VictoryHighlight,
        TurnTransition,
        StateTransition,
        ButtonInteraction
    }
    
    /// <summary>
    /// Animation features that can be toggled
    /// </summary>
    public enum AnimationFeature
    {
        ParticleEffects,
        ComplexShaders,
        PostProcessing
    }
}