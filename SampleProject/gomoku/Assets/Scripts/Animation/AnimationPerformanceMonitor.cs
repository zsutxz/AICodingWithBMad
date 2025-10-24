using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;

namespace Gomoku.Animation
{
    /// <summary>
    /// Monitors animation performance and provides optimization recommendations
    /// </summary>
    public class AnimationPerformanceMonitor : MonoBehaviour
    {
        [Header("Performance Monitoring")]
        [SerializeField] private bool enablePerformanceMonitoring = true;
        [SerializeField] private float monitoringInterval = 5f;
        [SerializeField] private float frameTimeThreshold = 0.0167f; // 60 FPS
        
        [Header("Optimization Settings")]
        [SerializeField] private bool autoOptimize = false;
        [SerializeField] private int optimizationFrameRate = 30;
        
        // Performance tracking
        private float totalFrameTime;
        private float animationFrameTime;
        private int frameCount;
        private Coroutine monitoringCoroutine;
        
        // Animation tracking
        private Dictionary<string, AnimationPerformanceData> animationPerformanceData;
        
        // Events
        public System.Action<float> OnFrameRateUpdate;
        public System.Action<string> OnPerformanceWarning;
        public System.Action<string> OnOptimizationApplied;
        
        // Properties
        public bool EnablePerformanceMonitoring => enablePerformanceMonitoring;
        public float AverageFrameTime => frameCount > 0 ? totalFrameTime / frameCount : 0f;
        public float AverageAnimationTime => frameCount > 0 ? animationFrameTime / frameCount : 0f;
        public float CurrentFrameRate => AverageFrameTime > 0 ? 1f / AverageFrameTime : 0f;
        
        // Singleton instance
        private static AnimationPerformanceMonitor instance;
        public static AnimationPerformanceMonitor Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<AnimationPerformanceMonitor>();
                    if (instance == null)
                    {
                        GameObject monitorObject = new GameObject("AnimationPerformanceMonitor");
                        instance = monitorObject.AddComponent<AnimationPerformanceMonitor>();
                        DontDestroyOnLoad(monitorObject);
                    }
                }
                return instance;
            }
        }
        
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
        /// Initializes the performance monitor
        /// </summary>
        private void Initialize()
        {
            animationPerformanceData = new Dictionary<string, AnimationPerformanceData>();
            
            if (enablePerformanceMonitoring)
            {
                monitoringCoroutine = StartCoroutine(MonitorPerformance());
            }
            
            Debug.Log("AnimationPerformanceMonitor initialized");
        }
        
        /// <summary>
        /// Monitors performance at regular intervals
        /// </summary>
        /// <returns>Coroutine enumerator</returns>
        private IEnumerator MonitorPerformance()
        {
            while (enablePerformanceMonitoring)
            {
                yield return new WaitForSeconds(monitoringInterval);
                
                AnalyzePerformance();
                
                if (autoOptimize)
                {
                    CheckAndApplyOptimizations();
                }
            }
        }
        
        /// <summary>
        /// Analyzes current performance and provides recommendations
        /// </summary>
        private void AnalyzePerformance()
        {
            float frameRate = CurrentFrameRate;
            OnFrameRateUpdate?.Invoke(frameRate);
            
            // Check for performance issues
            if (frameRate < optimizationFrameRate)
            {
                string warning = $"Low frame rate detected: {frameRate:F1} FPS. Consider optimizing animations.";
                OnPerformanceWarning?.Invoke(warning);
                Debug.LogWarning(warning);
            }
            
            // Check for specific animation performance issues
            foreach (var kvp in animationPerformanceData)
            {
                if (kvp.Value.AverageDuration > 0.1f) // 100ms threshold
                {
                    string warning = $"Slow animation detected: {kvp.Key} (avg: {kvp.Value.AverageDuration:F3}s)";
                    OnPerformanceWarning?.Invoke(warning);
                    Debug.LogWarning(warning);
                }
            }
            
            // Log performance summary
            Debug.Log($"Performance Summary - Frame Rate: {frameRate:F1} FPS, " +
                     $"Animation Time: {AverageAnimationTime * 1000:F1}ms");
        }
        
        /// <summary>
        /// Checks and applies optimizations if needed
        /// </summary>
        private void CheckAndApplyOptimizations()
        {
            float frameRate = CurrentFrameRate;
            
            if (frameRate < optimizationFrameRate)
            {
                ApplyPerformanceOptimizations();
            }
        }
        
        /// <summary>
        /// Applies performance optimizations
        /// </summary>
        private void ApplyPerformanceOptimizations()
        {
            Debug.Log("Applying animation performance optimizations...");
            
            // Get animation settings
            AnimationSettings settings = GetAnimationSettings();
            if (settings != null)
            {
                // Reduce animation quality
                if (settings.AnimationQuality == AnimationQuality.High)
                {
                    settings.SetAnimationQuality(AnimationQuality.Medium);
                    OnOptimizationApplied?.Invoke("Reduced animation quality to Medium");
                }
                else if (settings.AnimationQuality == AnimationQuality.Medium)
                {
                    settings.SetAnimationQuality(AnimationQuality.Low);
                    OnOptimizationApplied?.Invoke("Reduced animation quality to Low");
                }
                
                // Reduce concurrent animations
                settings.SetPerformanceLimits(
                    Mathf.Max(5, settings.MaxConcurrentAnimations - 2),
                    Mathf.Max(10, settings.PieceAnimationPoolSize - 5),
                    Mathf.Max(2, settings.VictoryEffectPoolSize - 1)
                );
                
                OnOptimizationApplied?.Invoke("Reduced animation pool sizes");
            }
            
            // Stop expensive animations
            AnimationManager animationManager = AnimationManager.Instance;
            if (animationManager != null && animationManager.ActiveAnimationCount > 5)
            {
                animationManager.StopAllActiveAnimations();
                OnOptimizationApplied?.Invoke("Stopped excessive animations");
            }
        }
        
        /// <summary>
        /// Records animation performance data
        /// </summary>
        /// <param name="animationName">Name of the animation</param>
        /// <param name="duration">Animation duration</param>
        public void RecordAnimationPerformance(string animationName, float duration)
        {
            if (!enablePerformanceMonitoring) return;
            
            if (!animationPerformanceData.ContainsKey(animationName))
            {
                animationPerformanceData[animationName] = new AnimationPerformanceData();
            }
            
            animationPerformanceData[animationName].RecordDuration(duration);
        }
        
        /// <summary>
        /// Records frame time information
        /// </summary>
        /// <param name="totalTime">Total frame time</param>
        /// <param name="animationTime">Animation frame time</param>
        public void RecordFrameTime(float totalTime, float animationTime)
        {
            if (!enablePerformanceMonitoring) return;
            
            totalFrameTime += totalTime;
            animationFrameTime += animationTime;
            frameCount++;
            
            // Reset counters periodically to prevent overflow
            if (frameCount >= 1000)
            {
                totalFrameTime = 0f;
                animationFrameTime = 0f;
                frameCount = 0;
            }
        }
        
        /// <summary>
        /// Gets animation settings
        /// </summary>
        /// <returns>Animation settings, or null if not found</returns>
        private AnimationSettings GetAnimationSettings()
        {
            // In a real implementation, you would load this from Resources or another source
            return Resources.Load<AnimationSettings>("AnimationSettings");
        }
        
        /// <summary>
        /// Sets whether performance monitoring is enabled
        /// </summary>
        /// <param name="enabled">Whether monitoring is enabled</param>
        public void SetPerformanceMonitoringEnabled(bool enabled)
        {
            enablePerformanceMonitoring = enabled;
            
            if (enabled && monitoringCoroutine == null)
            {
                monitoringCoroutine = StartCoroutine(MonitorPerformance());
            }
            else if (!enabled && monitoringCoroutine != null)
            {
                StopCoroutine(monitoringCoroutine);
                monitoringCoroutine = null;
            }
        }
        
        /// <summary>
        /// Sets auto-optimization settings
        /// </summary>
        /// <param name="autoOptimize">Whether to auto-optimize</param>
        /// <param name="targetFrameRate">Target frame rate for optimization</param>
        public void SetAutoOptimization(bool autoOptimize, int targetFrameRate)
        {
            this.autoOptimize = autoOptimize;
            this.optimizationFrameRate = Mathf.Max(15, targetFrameRate);
        }
        
        /// <summary>
        /// Gets performance data for a specific animation
        /// </summary>
        /// <param name="animationName">Animation name</param>
        /// <returns>Performance data, or null if not found</returns>
        public AnimationPerformanceData GetAnimationPerformanceData(string animationName)
        {
            return animationPerformanceData.ContainsKey(animationName) ? 
                   animationPerformanceData[animationName] : null;
        }
        
        /// <summary>
        /// Gets all animation performance data
        /// </summary>
        /// <returns>Dictionary of performance data</returns>
        public Dictionary<string, AnimationPerformanceData> GetAllPerformanceData()
        {
            return new Dictionary<string, AnimationPerformanceData>(animationPerformanceData);
        }
        
        /// <summary>
        /// Resets performance monitoring data
        /// </summary>
        public void ResetPerformanceData()
        {
            totalFrameTime = 0f;
            animationFrameTime = 0f;
            frameCount = 0;
            animationPerformanceData.Clear();
            
            Debug.Log("Performance data reset");
        }
        
        private void OnDestroy()
        {
            if (instance == this)
            {
                if (monitoringCoroutine != null)
                {
                    StopCoroutine(monitoringCoroutine);
                }
            }
        }
        
        #if UNITY_EDITOR
        /// <summary>
        /// Editor-only method for testing performance monitoring
        /// </summary>
        [ContextMenu("Test Performance Monitor")]
        private void TestPerformanceMonitor()
        {
            if (Application.isPlaying)
            {
                Debug.Log($"Performance Monitor Status:");
                Debug.Log($"- Monitoring Enabled: {enablePerformanceMonitoring}");
                Debug.Log($"- Auto Optimize: {autoOptimize}");
                Debug.Log($"- Current Frame Rate: {CurrentFrameRate:F1} FPS");
                Debug.Log($"- Average Animation Time: {AverageAnimationTime * 1000:F1}ms");
                Debug.Log($"- Tracked Animations: {animationPerformanceData.Count}");
                
                foreach (var kvp in animationPerformanceData)
                {
                    Debug.Log($"  - {kvp.Key}: {kvp.Value.AverageDuration:F3}s (count: {kvp.Value.ExecutionCount})");
                }
            }
            else
            {
                Debug.Log("Performance monitor test can only be run in Play mode");
            }
        }
        
        /// <summary>
        /// Editor-only method for simulating performance issues
        /// </summary>
        [ContextMenu("Simulate Performance Issue")]
        private void SimulatePerformanceIssue()
        {
            if (Application.isPlaying)
            {
                // Record slow animation
                RecordAnimationPerformance("SimulatedSlowAnimation", 0.5f);
                
                // Record frame time that would cause low FPS
                RecordFrameTime(0.05f, 0.03f); // 20 FPS equivalent
                
                Debug.Log("Simulated performance issue - check console for warnings");
            }
            else
            {
                Debug.Log("Performance simulation can only be run in Play mode");
            }
        }
        
        /// <summary>
        /// Editor-only method for applying optimizations
        /// </summary>
        [ContextMenu("Apply Optimizations Now")]
        private void ApplyOptimizationsNow()
        {
            if (Application.isPlaying)
            {
                ApplyPerformanceOptimizations();
            }
            else
            {
                Debug.Log("Optimizations can only be applied in Play mode");
            }
        }
        #endif
    }
    
    /// <summary>
    /// Data structure for tracking animation performance
    /// </summary>
    [System.Serializable]
    public class AnimationPerformanceData
    {
        public int ExecutionCount { get; private set; }
        public float TotalDuration { get; private set; }
        public float AverageDuration => ExecutionCount > 0 ? TotalDuration / ExecutionCount : 0f;
        public float MinDuration { get; private set; } = float.MaxValue;
        public float MaxDuration { get; private set; } = float.MinValue;
        
        /// <summary>
        /// Records an animation duration
        /// </summary>
        /// <param name="duration">Animation duration</param>
        public void RecordDuration(float duration)
        {
            ExecutionCount++;
            TotalDuration += duration;
            MinDuration = Mathf.Min(MinDuration, duration);
            MaxDuration = Mathf.Max(MaxDuration, duration);
        }
        
        /// <summary>
        /// Resets the performance data
        /// </summary>
        public void Reset()
        {
            ExecutionCount = 0;
            TotalDuration = 0f;
            MinDuration = float.MaxValue;
            MaxDuration = float.MinValue;
        }
    }
}