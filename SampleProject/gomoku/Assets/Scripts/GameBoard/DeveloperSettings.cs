using UnityEngine;

namespace Gomoku
{
    /// <summary>
    /// Developer settings for debugging and development features
    /// </summary>
    [CreateAssetMenu(fileName = "DeveloperSettings", menuName = "Gomoku/Developer Settings")]
    public class DeveloperSettings : ScriptableObject
    {
        [Header("Board Debug Features")]
        [SerializeField] private bool showGridCoordinates = false;
        [SerializeField] private bool showIntersectionDebug = false;
        [SerializeField] private bool showBoardBounds = false;
        [SerializeField] private bool enableClickDebugging = false;

        [Header("Performance Debugging")]
        [SerializeField] private bool showFPS = false;
        [SerializeField] private bool logPerformanceMetrics = false;

        [Header("Visual Debugging")]
        [SerializeField] private Color debugColor = Color.yellow;
        [SerializeField] private float debugLineDuration = 2.0f;

        // Events for settings changes
        public System.Action<bool> OnGridCoordinatesChanged;
        public System.Action<bool> OnIntersectionDebugChanged;
        public System.Action<bool> OnBoardBoundsChanged;

        // Public properties
        public bool ShowGridCoordinates 
        { 
            get => showGridCoordinates; 
            set 
            { 
                if (showGridCoordinates != value)
                {
                    showGridCoordinates = value;
                    OnGridCoordinatesChanged?.Invoke(value);
                }
            } 
        }

        public bool ShowIntersectionDebug 
        { 
            get => showIntersectionDebug; 
            set 
            { 
                if (showIntersectionDebug != value)
                {
                    showIntersectionDebug = value;
                    OnIntersectionDebugChanged?.Invoke(value);
                }
            } 
        }

        public bool ShowBoardBounds 
        { 
            get => showBoardBounds; 
            set 
            { 
                if (showBoardBounds != value)
                {
                    showBoardBounds = value;
                    OnBoardBoundsChanged?.Invoke(value);
                }
            } 
        }

        public bool EnableClickDebugging { get => enableClickDebugging; set => enableClickDebugging = value; }
        public bool ShowFPS { get => showFPS; set => showFPS = value; }
        public bool LogPerformanceMetrics { get => logPerformanceMetrics; set => logPerformanceMetrics = value; }
        public Color DebugColor { get => debugColor; set => debugColor = value; }
        public float DebugLineDuration { get => debugLineDuration; set => debugLineDuration = Mathf.Max(0.1f, value); }

        /// <summary>
        /// Creates default developer settings
        /// </summary>
        /// <returns>Default DeveloperSettings instance</returns>
        public static DeveloperSettings CreateDefault()
        {
            DeveloperSettings settings = CreateInstance<DeveloperSettings>();
            
            // Default settings (all debug features disabled)
            settings.showGridCoordinates = false;
            settings.showIntersectionDebug = false;
            settings.showBoardBounds = false;
            settings.enableClickDebugging = false;
            settings.showFPS = false;
            settings.logPerformanceMetrics = false;
            settings.debugColor = Color.yellow;
            settings.debugLineDuration = 2.0f;
            
            return settings;
        }

        /// <summary>
        /// Toggles all debug features on/off
        /// </summary>
        /// <param name="enable">Whether to enable all debug features</param>
        public void ToggleAllDebugFeatures(bool enable)
        {
            ShowGridCoordinates = enable;
            ShowIntersectionDebug = enable;
            ShowBoardBounds = enable;
            EnableClickDebugging = enable;
            ShowFPS = enable;
            LogPerformanceMetrics = enable;
        }

        /// <summary>
        /// Applies these settings to board components
        /// </summary>
        /// <param name="boardRenderer">BoardRenderer to apply settings to</param>
        /// <param name="intersectionDetector">IntersectionDetector to apply settings to</param>
        public void ApplyToBoardComponents(BoardRenderer boardRenderer, IntersectionDetector intersectionDetector)
        {
            if (boardRenderer != null)
            {
                boardRenderer.ToggleGridCoordinates(showGridCoordinates);
            }

            if (intersectionDetector != null)
            {
                intersectionDetector.ToggleDebugVisualization(showIntersectionDebug);
            }
        }

        #if UNITY_EDITOR
        private void OnValidate()
        {
            // Ensure valid values
            debugLineDuration = Mathf.Max(0.1f, debugLineDuration);
        }
        #endif
    }
}