using Gomoku.UI;
using UnityEngine;

namespace Gomoku
{
    /// <summary>
    /// Handles responsive scaling of the game board for different screen sizes and aspect ratios
    /// </summary>
    public class BoardScaler : MonoBehaviour
    {
        [Header("Scaling Configuration")]
        [SerializeField] private Canvas canvas;
        [SerializeField] private RectTransform boardContainer;
        [SerializeField] private GameBoardController gameBoard;
        
        [Header("Screen Size Settings")]
        [SerializeField] private Vector2 referenceResolution = new Vector2(1920, 1080);
        [SerializeField] private float minBoardSize = 400f;
        [SerializeField] private float maxBoardSize = 800f;
        [SerializeField] private float marginPercentage = 0.1f; // 10% margin on all sides
        
        [Header("Aspect Ratio Handling")]
        [SerializeField] private bool maintainAspectRatio = true;
        [SerializeField] private float aspectRatioTolerance = 0.1f;
        
        private Camera mainCamera;
        private Vector2 lastScreenSize;
        private float lastAspectRatio;

        private void Awake()
        {
            InitializeComponents();
            UpdateBoardScaling();
        }

        private void Start()
        {
            // Ensure scaling is applied after all components are initialized
            UpdateBoardScaling();
        }

        private void Update()
        {
            // Check if screen size or aspect ratio has changed
            Vector2 currentScreenSize = new Vector2(Screen.width, Screen.height);
            float currentAspectRatio = (float)Screen.width / Screen.height;
            
            if (currentScreenSize != lastScreenSize || 
                Mathf.Abs(currentAspectRatio - lastAspectRatio) > aspectRatioTolerance)
            {
                UpdateBoardScaling();
                lastScreenSize = currentScreenSize;
                lastAspectRatio = currentAspectRatio;
            }
        }

        /// <summary>
        /// Initializes required components
        /// </summary>
        private void InitializeComponents()
        {
            if (canvas == null)
            {
                canvas = GetComponentInParent<Canvas>();
                if (canvas == null)
                {
                    canvas = FindObjectOfType<Canvas>();
                }
            }

            if (boardContainer == null)
            {
                boardContainer = GetComponent<RectTransform>();
            }

            if (gameBoard == null)
            {
                gameBoard = GetComponentInChildren<GameBoardController>();
            }

            mainCamera = Camera.main;
            
            lastScreenSize = new Vector2(Screen.width, Screen.height);
            lastAspectRatio = (float)Screen.width / Screen.height;
        }

        /// <summary>
        /// Updates the board scaling based on current screen configuration
        /// </summary>
        public void UpdateBoardScaling()
        {
            if (canvas == null || boardContainer == null || gameBoard == null)
            {
                Debug.LogWarning("BoardScaler: Required components not assigned");
                return;
            }

            Vector2 screenSize = GetEffectiveScreenSize();
            float scaleFactor = CalculateOptimalScale(screenSize);
            
            ApplyScaling(scaleFactor);
            CenterBoardWithMargins(screenSize, scaleFactor);
            
            Debug.Log($"Board scaled to factor: {scaleFactor:F2} for screen: {screenSize}");
        }

        /// <summary>
        /// Gets the effective screen size considering canvas scaling
        /// </summary>
        /// <returns>Effective screen size in pixels</returns>
        private Vector2 GetEffectiveScreenSize()
        {
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                return new Vector2(Screen.width, Screen.height);
            }
            else
            {
                // For world space or camera space rendering
                return referenceResolution;
            }
        }

        /// <summary>
        /// Calculates the optimal scale factor for the board
        /// </summary>
        /// <param name="screenSize">Current screen size</param>
        /// <returns>Optimal scale factor</returns>
        private float CalculateOptimalScale(Vector2 screenSize)
        {
            // Calculate available space considering margins
            float availableWidth = screenSize.x * (1 - 2 * marginPercentage);
            float availableHeight = screenSize.y * (1 - 2 * marginPercentage);
            
            // Get board dimensions
            float boardWidth = gameBoard.BoardWidth;
            float boardHeight = gameBoard.BoardHeight;
            
            // Calculate scale factors for width and height
            float widthScale = availableWidth / boardWidth;
            float heightScale = availableHeight / boardHeight;
            
            // Use the smaller scale to ensure board fits entirely
            float scale = Mathf.Min(widthScale, heightScale);
            
            // Clamp to min/max size constraints
            float scaledBoardSize = Mathf.Max(boardWidth, boardHeight) * scale;
            if (scaledBoardSize < minBoardSize)
            {
                scale = minBoardSize / Mathf.Max(boardWidth, boardHeight);
            }
            else if (scaledBoardSize > maxBoardSize)
            {
                scale = maxBoardSize / Mathf.Max(boardWidth, boardHeight);
            }
            
            return scale;
        }

        /// <summary>
        /// Applies the calculated scale to the board
        /// </summary>
        /// <param name="scale">Scale factor to apply</param>
        private void ApplyScaling(float scale)
        {
            if (boardContainer != null)
            {
                boardContainer.localScale = new Vector3(scale, scale, 1);
            }
            
            // If using world space, also update the GameBoard cell size
            if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                // Update the GameBoard configuration with scaled cell size
                float originalCellSize = 1.0f; // Assuming original cell size is 1.0
                float scaledCellSize = originalCellSize * scale;
                
                // Note: This would require modifying the GameBoard to support runtime scaling
                // For now, we rely on the RectTransform scaling
            }
        }

        /// <summary>
        /// Centers the board with appropriate margins
        /// </summary>
        /// <param name="screenSize">Current screen size</param>
        /// <param name="scale">Current scale factor</param>
        private void CenterBoardWithMargins(Vector2 screenSize, float scale)
        {
            if (boardContainer == null) return;

            // Calculate scaled board dimensions
            float scaledWidth = gameBoard.BoardWidth * scale;
            float scaledHeight = gameBoard.BoardHeight * scale;
            
            // Center the board
            boardContainer.anchoredPosition = Vector2.zero;
            
            // For UI-based boards, ensure proper anchoring
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                boardContainer.anchorMin = new Vector2(0.5f, 0.5f);
                boardContainer.anchorMax = new Vector2(0.5f, 0.5f);
                boardContainer.pivot = new Vector2(0.5f, 0.5f);
            }
        }

        /// <summary>
        /// Tests the board scaling on various common resolutions
        /// </summary>
        public void TestCommonResolutions()
        {
            Vector2[] testResolutions = {
                new Vector2(1920, 1080),  // 1080p
                new Vector2(1280, 720),   // 720p
                new Vector2(2560, 1440),  // 1440p
                new Vector2(3840, 2160),  // 4K
                new Vector2(1024, 768),   // 4:3
                new Vector2(2560, 1080),  // Ultra-wide
                new Vector2(1080, 1920)   // Portrait
            };

            foreach (Vector2 resolution in testResolutions)
            {
                Debug.Log($"Testing resolution: {resolution.x}x{resolution.y}");
                // In a real implementation, you would simulate this resolution
                // and verify the board scales correctly
            }
        }

        /// <summary>
        /// Forces an immediate update of board scaling
        /// </summary>
        [ContextMenu("Update Board Scaling")]
        public void ForceUpdateScaling()
        {
            UpdateBoardScaling();
        }

        /// <summary>
        /// Updates scaling configuration
        /// </summary>
        /// <param name="newMinSize">New minimum board size</param>
        /// <param name="newMaxSize">New maximum board size</param>
        /// <param name="newMarginPercentage">New margin percentage</param>
        public void UpdateScalingConfiguration(float newMinSize, float newMaxSize, float newMarginPercentage)
        {
            minBoardSize = Mathf.Max(100f, newMinSize);
            maxBoardSize = Mathf.Max(minBoardSize, newMaxSize);
            marginPercentage = Mathf.Clamp01(newMarginPercentage);
            
            UpdateBoardScaling();
        }

        #if UNITY_EDITOR
        private void OnValidate()
        {
            // Ensure valid values in editor
            minBoardSize = Mathf.Max(100f, minBoardSize);
            maxBoardSize = Mathf.Max(minBoardSize, maxBoardSize);
            marginPercentage = Mathf.Clamp01(marginPercentage);
            aspectRatioTolerance = Mathf.Max(0.01f, aspectRatioTolerance);
        }

        private void OnDrawGizmosSelected()
        {
            // Draw visualization of scaling bounds in editor
            if (boardContainer != null && gameBoard != null)
            {
                // Draw board bounds
                Gizmos.color = Color.green;
                Vector3 boardCenter = boardContainer.position;
                Vector3 boardSize = new Vector3(gameBoard.BoardWidth, gameBoard.BoardHeight, 0.1f) * boardContainer.localScale.x;
                Gizmos.DrawWireCube(boardCenter, boardSize);
                
                // Draw margin bounds
                Gizmos.color = Color.yellow;
                Vector3 marginSize = new Vector3(
                    Screen.width * (1 - 2 * marginPercentage),
                    Screen.height * (1 - 2 * marginPercentage),
                    0.1f
                );
                Gizmos.DrawWireCube(Vector3.zero, marginSize);
            }
        }
        #endif
    }
}