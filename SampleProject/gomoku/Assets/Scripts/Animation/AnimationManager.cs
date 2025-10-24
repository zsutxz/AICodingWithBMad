using UnityEngine;
using System.Collections.Generic;
using Gomoku;
using Gomoku.Core;

namespace Gomoku.Animation
{
    /// <summary>
    /// Central manager for coordinating all animation systems and optimizing performance
    /// </summary>
    public class AnimationManager : MonoBehaviour
    {
        [Header("Performance Settings")]
        [SerializeField] private bool enableAnimations = true;
        [SerializeField] private AnimationQuality animationQuality = AnimationQuality.High;
        [SerializeField] private int maxConcurrentAnimations = 10;
        
        [Header("Object Pooling")]
        [SerializeField] private int pieceAnimationPoolSize = 20;
        [SerializeField] private int victoryEffectPoolSize = 5;
        
        // Animation pools
        private Queue<GameObject> pieceAnimationPool;
        private Queue<GameObject> victoryEffectPool;
        
        // Active animations
        private List<MonoBehaviour> activeAnimations;
        
        // Events
        public System.Action<bool> OnAnimationToggle;
        public System.Action<AnimationQuality> OnQualityChanged;
        
        // Properties
        public bool EnableAnimations => enableAnimations;
        public AnimationQuality AnimationQuality => animationQuality;
        public int ActiveAnimationCount => activeAnimations?.Count ?? 0;
        
        // Singleton instance
        private static AnimationManager instance;
        public static AnimationManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<AnimationManager>();
                    if (instance == null)
                    {
                        GameObject managerObject = new GameObject("AnimationManager");
                        instance = managerObject.AddComponent<AnimationManager>();
                        DontDestroyOnLoad(managerObject);
                    }
                }
                return instance;
            }
        }

        public System.Func<object, bool> OnAnimationRegistered { get; set; }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                Initialize();
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        /// <summary>
        /// Initializes the animation manager
        /// </summary>
        private void Initialize()
        {
            // Initialize pools
            pieceAnimationPool = new Queue<GameObject>();
            victoryEffectPool = new Queue<GameObject>();
            
            // Initialize active animations list
            activeAnimations = new List<MonoBehaviour>();
            
            // Pre-populate pools if needed
            PrepopulatePools();
            
            Debug.Log("AnimationManager initialized");
        }
        
        /// <summary>
        /// Pre-populates object pools for better performance
        /// </summary>
        private void PrepopulatePools()
        {
            // In a real implementation, you would instantiate prefabs here
            // For now, we'll just initialize the queues
            Debug.Log($"Animation pools initialized - Piece: {pieceAnimationPoolSize}, Victory: {victoryEffectPoolSize}");
        }
        
        /// <summary>
        /// Toggles animations on/off
        /// </summary>
        /// <param name="enabled">Whether animations are enabled</param>
        public void SetAnimationsEnabled(bool enabled)
        {
            if (enableAnimations != enabled)
            {
                enableAnimations = enabled;
                OnAnimationToggle?.Invoke(enabled);
                
                if (!enabled)
                {
                    StopAllActiveAnimations();
                }
                
                Debug.Log($"Animations {(enabled ? "enabled" : "disabled")}");
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
                OnQualityChanged?.Invoke(quality);
                
                // Apply quality settings to active animations
                ApplyQualitySettings();
                
                Debug.Log($"Animation quality set to: {quality}");
            }
        }
        
        /// <summary>
        /// Applies quality settings to active animations
        /// </summary>
        private void ApplyQualitySettings()
        {
            // In a real implementation, you would adjust animation parameters
            // based on quality settings (e.g., reduce particle counts, simplify effects)
            
            switch (animationQuality)
            {
                case AnimationQuality.Low:
                    // Reduce animation complexity
                    break;
                case AnimationQuality.Medium:
                    // Balanced settings
                    break;
                case AnimationQuality.High:
                    // Full quality
                    break;
            }
        }
        
        /// <summary>
        /// Registers an active animation
        /// </summary>
        /// <param name="animation">The animation component</param>
        public void RegisterActiveAnimation(MonoBehaviour animation)
        {
            if (animation != null && !activeAnimations.Contains(animation))
            {
                activeAnimations.Add(animation);
                
                // Check if we need to limit concurrent animations
                if (activeAnimations.Count > maxConcurrentAnimations)
                {
                    LimitConcurrentAnimations();
                }
            }
        }
        
        /// <summary>
        /// Unregisters an active animation
        /// </summary>
        /// <param name="animation">The animation component</param>
        public void UnregisterActiveAnimation(MonoBehaviour animation)
        {
            if (animation != null)
            {
                activeAnimations.Remove(animation);
            }
        }
        
        /// <summary>
        /// Limits the number of concurrent animations
        /// </summary>
        private void LimitConcurrentAnimations()
        {
            // Remove oldest animations to stay within limit
            while (activeAnimations.Count > maxConcurrentAnimations)
            {
                var oldestAnimation = activeAnimations[0];
                
                // Try to stop the animation
                if (oldestAnimation is PieceAnimation pieceAnim)
                {
                    pieceAnim.StopCurrentAnimation();
                }
                else if (oldestAnimation is VictoryEffect victoryEffect)
                {
                    victoryEffect.StopVictoryAnimation();
                }
                
                activeAnimations.RemoveAt(0);
            }
        }
        
        /// <summary>
        /// Stops all active animations
        /// </summary>
        public void StopAllActiveAnimations()
        {
            foreach (var animation in activeAnimations)
            {
                if (animation is PieceAnimation pieceAnim)
                {
                    pieceAnim.StopCurrentAnimation();
                }
                else if (animation is VictoryEffect victoryEffect)
                {
                    victoryEffect.StopVictoryAnimation();
                }
                else if (animation is TurnTransition turnTransition)
                {
                    turnTransition.StopTransitionAnimation();
                }
                else if (animation is GameStateTransition stateTransition)
                {
                    stateTransition.StopCurrentTransition();
                }
            }
            
            activeAnimations.Clear();
            
            Debug.Log("All active animations stopped");
        }
        
        /// <summary>
        /// Gets a piece animation from the pool
        /// </summary>
        /// <returns>Piece animation GameObject</returns>
        public GameObject GetPieceAnimationFromPool()
        {
            if (pieceAnimationPool.Count > 0)
            {
                return pieceAnimationPool.Dequeue();
            }
            
            // Create new instance if pool is empty
            // In a real implementation, you would instantiate from a prefab
            Debug.Log("Creating new piece animation instance");
            return null;
        }
        
        /// <summary>
        /// Returns a piece animation to the pool
        /// </summary>
        /// <param name="animationObject">Animation GameObject to return</param>
        public void ReturnPieceAnimationToPool(GameObject animationObject)
        {
            if (animationObject != null && pieceAnimationPool.Count < pieceAnimationPoolSize)
            {
                pieceAnimationPool.Enqueue(animationObject);
            }
            else
            {
                // Destroy if pool is full
                Destroy(animationObject);
            }
        }
        
        /// <summary>
        /// Gets a victory effect from the pool
        /// </summary>
        /// <returns>Victory effect GameObject</returns>
        public GameObject GetVictoryEffectFromPool()
        {
            if (victoryEffectPool.Count > 0)
            {
                return victoryEffectPool.Dequeue();
            }
            
            // Create new instance if pool is empty
            Debug.Log("Creating new victory effect instance");
            return null;
        }
        
        /// <summary>
        /// Returns a victory effect to the pool
        /// </summary>
        /// <param name="effectObject">Effect GameObject to return</param>
        public void ReturnVictoryEffectToPool(GameObject effectObject)
        {
            if (effectObject != null && victoryEffectPool.Count < victoryEffectPoolSize)
            {
                victoryEffectPool.Enqueue(effectObject);
            }
            else
            {
                // Destroy if pool is full
                Destroy(effectObject);
            }
        }
        
        /// <summary>
        /// Cleans up the animation manager
        /// </summary>
        private void OnDestroy()
        {
            if (instance == this)
            {
                StopAllActiveAnimations();
                
                // Clear pools
                while (pieceAnimationPool.Count > 0)
                {
                    var obj = pieceAnimationPool.Dequeue();
                    if (obj != null)
                    {
                        Destroy(obj);
                    }
                }
                
                while (victoryEffectPool.Count > 0)
                {
                    var obj = victoryEffectPool.Dequeue();
                    if (obj != null)
                    {
                        Destroy(obj);
                    }
                }
                
                activeAnimations.Clear();
            }
        }
        
        #if UNITY_EDITOR
        /// <summary>
        /// Editor-only method for testing animation manager
        /// </summary>
        [ContextMenu("Test Animation Manager")]
        private void TestAnimationManager()
        {
            if (Application.isPlaying)
            {
                Debug.Log($"Animation Manager Status:");
                Debug.Log($"- Animations Enabled: {enableAnimations}");
                Debug.Log($"- Animation Quality: {animationQuality}");
                Debug.Log($"- Active Animations: {activeAnimations.Count}");
                Debug.Log($"- Piece Animation Pool: {pieceAnimationPool.Count}/{pieceAnimationPoolSize}");
                Debug.Log($"- Victory Effect Pool: {victoryEffectPool.Count}/{victoryEffectPoolSize}");
            }
            else
            {
                Debug.Log("Animation manager test can only be run in Play mode");
            }
        }
        
        /// <summary>
        /// Editor-only method for toggling animations
        /// </summary>
        [ContextMenu("Toggle Animations")]
        private void ToggleAnimations()
        {
            if (Application.isPlaying)
            {
                SetAnimationsEnabled(!enableAnimations);
            }
            else
            {
                Debug.Log("Animation toggle can only be run in Play mode");
            }
        }
        #endif
    }
    
    /// <summary>
    /// Animation quality levels
    /// </summary>
    public enum AnimationQuality
    {
        Low,
        Medium,
        High
    }
}