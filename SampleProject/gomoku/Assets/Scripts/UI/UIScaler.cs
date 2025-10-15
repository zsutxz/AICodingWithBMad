using UnityEngine;
using UnityEngine.UI;

namespace Gomoku.UI
{
    /// <summary>
    /// Handles responsive UI scaling for different screen resolutions and aspect ratios
    /// </summary>
    public class UIScaler : MonoBehaviour
    {
        [Header("Canvas Settings")]
        [SerializeField] private CanvasScaler canvasScaler;
        [SerializeField] private Vector2 referenceResolution = new Vector2(1920, 1080);
        [SerializeField] private float matchWidthOrHeight = 0.5f;

        [Header("Scaling Settings")]
        [SerializeField] private float minScale = 0.7f;
        [SerializeField] private float maxScale = 1.3f;
        [SerializeField] private bool enableDynamicScaling = true;

        [Header("Resolution Testing")]
        [SerializeField] private Vector2[] testResolutions = {
            new Vector2(1920, 1080), // 1080p
            new Vector2(1280, 720),  // 720p
            new Vector2(2560, 1440), // 1440p
            new Vector2(3840, 2160), // 4K
            new Vector2(1024, 768),  // 4:3
            new Vector2(2560, 1080), // 21:9
            new Vector2(2732, 2048)  // iPad Pro
        };

        private Vector2 currentResolution;
        private float currentScale;

        private void Awake()
        {
            InitializeUIScaler();
        }

        private void Start()
        {
            UpdateUIScaling();
        }

        /// <summary>
        /// Initializes the UI scaler
        /// </summary>
        private void InitializeUIScaler()
        {
            // Auto-find canvas scaler
            if (canvasScaler == null)
                canvasScaler = GetComponent<CanvasScaler>();

            if (canvasScaler != null)
            {
                // Configure canvas scaler
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.referenceResolution = referenceResolution;
                canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                canvasScaler.matchWidthOrHeight = matchWidthOrHeight;
            }

            currentResolution = new Vector2(Screen.width, Screen.height);

            Debug.Log("UIScaler initialized");
        }

        /// <summary>
        /// Updates UI scaling based on current screen configuration
        /// </summary>
        public void UpdateUIScaling()
        {
            currentResolution = new Vector2(Screen.width, Screen.height);
            currentScale = CalculateScaleFactor();

            if (enableDynamicScaling)
            {
                ApplyDynamicScaling();
            }

            Debug.Log($"UI scaling updated: Resolution {currentResolution}, Scale {currentScale:F2}");
        }

        /// <summary>
        /// Calculates the scale factor based on screen resolution
        /// </summary>
        /// <returns>Scale factor</returns>
        private float CalculateScaleFactor()
        {
            float widthRatio = currentResolution.x / referenceResolution.x;
            float heightRatio = currentResolution.y / referenceResolution.y;

            // Use match width or height setting
            float scale = Mathf.Lerp(widthRatio, heightRatio, matchWidthOrHeight);

            // Apply min/max constraints
            scale = Mathf.Clamp(scale, minScale, maxScale);

            return scale;
        }

        /// <summary>
        /// Applies dynamic scaling to UI elements
        /// </summary>
        private void ApplyDynamicScaling()
        {
            // This method can be extended to apply custom scaling logic
            // to specific UI elements if needed

            // For now, the CanvasScaler handles most scaling automatically
            // Additional custom scaling logic can be added here
        }

        /// <summary>
        /// Tests UI scaling on different resolutions
        /// </summary>
        public void TestScalingOnResolutions()
        {
            Debug.Log("Testing UI scaling on different resolutions:");

            foreach (Vector2 testResolution in testResolutions)
            {
                float testScale = CalculateScaleForResolution(testResolution);
                Debug.Log($"Resolution {testResolution}: Scale {testScale:F2}");
            }
        }

        /// <summary>
        /// Calculates scale factor for a specific resolution
        /// </summary>
        /// <param name="resolution">Resolution to test</param>
        /// <returns>Scale factor for the resolution</returns>
        private float CalculateScaleForResolution(Vector2 resolution)
        {
            float widthRatio = resolution.x / referenceResolution.x;
            float heightRatio = resolution.y / referenceResolution.y;

            float scale = Mathf.Lerp(widthRatio, heightRatio, matchWidthOrHeight);
            scale = Mathf.Clamp(scale, minScale, maxScale);

            return scale;
        }

        /// <summary>
        /// Adjusts scaling for different aspect ratios
        /// </summary>
        /// <param name="aspectRatio">Current aspect ratio</param>
        public void AdjustForAspectRatio(float aspectRatio)
        {
            // Adjust scaling based on aspect ratio
            if (aspectRatio > 2.0f) // Ultra-wide
            {
                // Slightly reduce scale for ultra-wide displays
                currentScale *= 0.9f;
            }
            else if (aspectRatio < 1.3f) // Tall/narrow
            {
                // Slightly increase scale for tall displays
                currentScale *= 1.1f;
            }

            // Ensure scale stays within bounds
            currentScale = Mathf.Clamp(currentScale, minScale, maxScale);

            Debug.Log($"Adjusted scale for aspect ratio {aspectRatio:F2}: {currentScale:F2}");
        }

        /// <summary>
        /// Gets the current scale factor
        /// </summary>
        /// <returns>Current scale factor</returns>
        public float GetCurrentScale()
        {
            return currentScale;
        }

        /// <summary>
        /// Gets the current resolution
        /// </summary>
        /// <returns>Current resolution</returns>
        public Vector2 GetCurrentResolution()
        {
            return currentResolution;
        }

        /// <summary>
        /// Gets the reference resolution
        /// </summary>
        /// <returns>Reference resolution</returns>
        public Vector2 GetReferenceResolution()
        {
            return referenceResolution;
        }

        /// <summary>
        /// Sets the reference resolution
        /// </summary>
        /// <param name="newResolution">New reference resolution</param>
        public void SetReferenceResolution(Vector2 newResolution)
        {
            referenceResolution = newResolution;
            if (canvasScaler != null)
            {
                canvasScaler.referenceResolution = referenceResolution;
            }
            UpdateUIScaling();
        }

        /// <summary>
        /// Sets the match width or height value
        /// </summary>
        /// <param name="value">New match value (0 = width, 1 = height)</param>
        public void SetMatchWidthOrHeight(float value)
        {
            matchWidthOrHeight = Mathf.Clamp01(value);
            if (canvasScaler != null)
            {
                canvasScaler.matchWidthOrHeight = matchWidthOrHeight;
            }
            UpdateUIScaling();
        }

        /// <summary>
        /// Validates that the UI scaler is properly configured
        /// </summary>
        /// <returns>True if configuration is valid</returns>
        public bool ValidateConfiguration()
        {
            if (canvasScaler == null)
            {
                Debug.LogError("UIScaler: CanvasScaler reference is missing");
                return false;
            }

            if (referenceResolution.x <= 0 || referenceResolution.y <= 0)
            {
                Debug.LogError("UIScaler: Invalid reference resolution");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Called when screen resolution changes
        /// </summary>
        private void OnRectTransformDimensionsChange()
        {
            if (gameObject.activeInHierarchy)
            {
                UpdateUIScaling();
            }
        }
    }
}