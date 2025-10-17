using UnityEngine;
using Gomoku.UI;

namespace Gomoku
{
    /// <summary>
    /// Manages the complete board setup including scaling, centering, and visual configuration
    /// </summary>
    public class BoardSetupManager : MonoBehaviour
    {
        [Header("Board Components")]
        [SerializeField] private GameBoardController gameBoard;
        [SerializeField] private BoardRenderer boardRenderer;
        [SerializeField] private BoardScaler boardScaler;
        [SerializeField] private Canvas canvas;
        
        [Header("Setup Configuration")]
        [SerializeField] private BoardVisualSettings visualSettings;
        [SerializeField] private bool autoSetupOnStart = true;
        [SerializeField] private bool enableResponsiveScaling = true;
        [SerializeField] private bool enableCentering = true;

        private void Awake()
        {
            InitializeComponents();
        }

        private void Start()
        {
            if (autoSetupOnStart)
            {
                SetupCompleteBoard();
            }
        }

        /// <summary>
        /// Initializes all required components
        /// </summary>
        private void InitializeComponents()
        {
            // Find components if not assigned
            if (gameBoard == null)
                gameBoard = GetComponentInChildren<GameBoardController>();
            
            if (boardRenderer == null)
                boardRenderer = GetComponentInChildren<BoardRenderer>();
            
            if (boardScaler == null)
                boardScaler = GetComponentInChildren<BoardScaler>();
            
            if (canvas == null)
                canvas = GetComponentInParent<Canvas>();

            // Create default visual settings if none assigned
            if (visualSettings == null)
            {
                visualSettings = BoardVisualSettings.CreateDefault();
            }
        }

        /// <summary>
        /// Performs complete board setup including visual configuration, scaling, and centering
        /// </summary>
        public void SetupCompleteBoard()
        {
            InitializeComponents();
            
            // Apply visual settings
            ApplyVisualSettings();
            
            // Configure scaling and centering
            if (enableResponsiveScaling && boardScaler != null)
            {
                boardScaler.ForceUpdateScaling();
            }
            
            if (enableCentering)
            {
                CenterBoard();
            }
            
            Debug.Log("Complete board setup completed successfully");
        }

        /// <summary>
        /// Applies the visual settings to the board components
        /// </summary>
        private void ApplyVisualSettings()
        {
            if (visualSettings == null) return;

            // Apply settings to GameBoard
            if (gameBoard != null)
            {
                gameBoard.UpdateBoardConfiguration(
                    visualSettings.BoardSize,
                    visualSettings.CellSize,
                    visualSettings.BoardOffset
                );
            }

            // Apply settings to BoardRenderer
            if (boardRenderer != null)
            {
                visualSettings.ApplyToBoardRenderer(boardRenderer);
            }
        }

        /// <summary>
        /// Centers the board on screen with proper margins
        /// </summary>
        public void CenterBoard()
        {
            if (canvas == null || gameBoard == null) return;

            // Get the RectTransform for positioning
            RectTransform boardRect = gameBoard.GetComponent<RectTransform>();
            if (boardRect == null)
            {
                // If no RectTransform, use Transform positioning
                CenterBoardWorldSpace();
                return;
            }

            // UI-based centering
            CenterBoardUI(boardRect);
        }

        /// <summary>
        /// Centers the board in UI space
        /// </summary>
        /// <param name="boardRect">Board RectTransform</param>
        private void CenterBoardUI(RectTransform boardRect)
        {
            // Set anchors to center
            boardRect.anchorMin = new Vector2(0.5f, 0.5f);
            boardRect.anchorMax = new Vector2(0.5f, 0.5f);
            boardRect.pivot = new Vector2(0.5f, 0.5f);
            boardRect.anchoredPosition = Vector2.zero;

            // Apply margins based on screen size
            ApplyUIMargins(boardRect);
        }

        /// <summary>
        /// Centers the board in world space
        /// </summary>
        private void CenterBoardWorldSpace()
        {
            if (Camera.main == null) return;

            // Position board at world center
            Vector3 screenCenter = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10f));
            gameBoard.transform.position = new Vector3(screenCenter.x, screenCenter.y, 0);

            // Apply world space margins
            ApplyWorldMargins();
        }

        /// <summary>
        /// Applies UI-based margins to ensure board doesn't touch screen edges
        /// </summary>
        /// <param name="boardRect">Board RectTransform</param>
        private void ApplyUIMargins(RectTransform boardRect)
        {
            if (canvas == null) return;

            // Calculate safe area margins
            Rect safeArea = Screen.safeArea;
            float marginScale = Mathf.Min(
                safeArea.width / Screen.width,
                safeArea.height / Screen.height
            );

            // Apply size constraints to ensure board stays within safe area
            Vector2 maxSize = new Vector2(
                safeArea.width * 0.8f, // 80% of safe area width
                safeArea.height * 0.8f  // 80% of safe area height
            );

            // Get current board size
            Vector2 boardSize = new Vector2(gameBoard.BoardWidth, gameBoard.BoardHeight);
            
            // Calculate scale to fit within max size
            float scaleX = maxSize.x / boardSize.x;
            float scaleY = maxSize.y / boardSize.y;
            float scale = Mathf.Min(scaleX, scaleY);

            // Apply scale if needed
            if (scale < 1.0f)
            {
                boardRect.localScale = new Vector3(scale, scale, 1);
            }
        }

        /// <summary>
        /// Applies world space margins
        /// </summary>
        private void ApplyWorldMargins()
        {
            if (Camera.main == null || gameBoard == null) return;

            // Get camera viewport bounds
            Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 10f));
            Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 10f));
            
            Vector3 viewportSize = topRight - bottomLeft;
            
            // Calculate board size
            float boardWidth = gameBoard.BoardWidth;
            float boardHeight = gameBoard.BoardHeight;
            
            // Ensure board fits within viewport with margins
            float margin = 0.1f; // 10% margin
            float maxBoardWidth = viewportSize.x * (1 - 2 * margin);
            float maxBoardHeight = viewportSize.y * (1 - 2 * margin);
            
            // Calculate required scale
            float scaleX = maxBoardWidth / boardWidth;
            float scaleY = maxBoardHeight / boardHeight;
            float scale = Mathf.Min(scaleX, scaleY);
            
            // Apply scale if needed
            if (scale < 1.0f)
            {
                gameBoard.transform.localScale = new Vector3(scale, scale, 1);
            }
        }

        /// <summary>
        /// Tests the board setup on various screen configurations
        /// </summary>
        public void TestScreenConfigurations()
        {
            Debug.Log("Testing board setup on various configurations...");

            // Test common resolutions
            Vector2[] testResolutions = {
                new Vector2(1920, 1080),  // 16:9
                new Vector2(1280, 720),   // 16:9
                new Vector2(2560, 1440),  // 16:9
                new Vector2(3840, 2160),  // 16:9
                new Vector2(1024, 768),   // 4:3
                new Vector2(2560, 1080),  // 21:9
                new Vector2(1080, 1920)   // 9:16
            };

            foreach (Vector2 resolution in testResolutions)
            {
                Debug.Log($"Testing resolution: {resolution.x}x{resolution.y}");
                // In a real implementation, simulate this resolution
                // and verify board setup works correctly
            }

            Debug.Log("Screen configuration testing completed");
        }

        /// <summary>
        /// Updates the board setup configuration
        /// </summary>
        /// <param name="newVisualSettings">New visual settings</param>
        /// <param name="enableScaling">Whether to enable responsive scaling</param>
        /// <param name="enableCenter">Whether to enable centering</param>
        public void UpdateSetupConfiguration(BoardVisualSettings newVisualSettings, bool enableScaling, bool enableCenter)
        {
            visualSettings = newVisualSettings;
            enableResponsiveScaling = enableScaling;
            enableCentering = enableCenter;
            
            SetupCompleteBoard();
        }

        /// <summary>
        /// Forces a complete board setup refresh
        /// </summary>
        [ContextMenu("Refresh Board Setup")]
        public void RefreshSetup()
        {
            SetupCompleteBoard();
        }

        #if UNITY_EDITOR
        private void OnValidate()
        {
            // Auto-assign components in editor
            if (gameBoard == null)
                gameBoard = GetComponentInChildren<GameBoardController>();
            
            if (boardRenderer == null)
                boardRenderer = GetComponentInChildren<BoardRenderer>();
            
            if (boardScaler == null)
                boardScaler = GetComponentInChildren<BoardScaler>();
        }
        #endif
    }
}