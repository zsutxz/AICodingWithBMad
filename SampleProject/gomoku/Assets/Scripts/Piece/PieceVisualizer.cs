using UnityEngine;

namespace Gomoku
{
    /// <summary>
    /// Provides visual feedback for piece placement and board state
    /// </summary>
    public class PieceVisualizer : MonoBehaviour
    {
        [Header("Component References")]
        [SerializeField] private GameBoard gameBoard;
        [SerializeField] private IntersectionDetector intersectionDetector;
        [SerializeField] private PiecePlacement piecePlacement;

        [Header("Visual Settings")]
        [SerializeField] private bool enableHoverFeedback = true;
        [SerializeField] private GameObject hoverIndicatorPrefab;
        [SerializeField] private Color validPlacementColor = new Color(0.5f, 1f, 0.5f, 0.5f);
        [SerializeField] private Color invalidPlacementColor = new Color(1f, 0.5f, 0.5f, 0.5f);

        // Hover indicator
        private GameObject hoverIndicator;
        private SpriteRenderer hoverIndicatorRenderer;
        private bool isHoverIndicatorActive;

        private void Awake()
        {
            InitializeVisualizer();
        }

        private void Update()
        {
            if (enableHoverFeedback)
            {
                UpdateHoverIndicator();
            }
        }

        private void OnDestroy()
        {
            CleanupEvents();
        }

        /// <summary>
        /// Initializes the piece visualizer
        /// </summary>
        private void InitializeVisualizer()
        {
            // Auto-find references
            if (gameBoard == null) gameBoard = FindObjectOfType<GameBoard>();
            if (intersectionDetector == null) intersectionDetector = FindObjectOfType<IntersectionDetector>();
            if (piecePlacement == null) piecePlacement = FindObjectOfType<PiecePlacement>();

            // Validate references
            if (gameBoard == null || intersectionDetector == null || piecePlacement == null)
            {
                Debug.LogError("Missing required references for PieceVisualizer!");
                enabled = false;
                return;
            }

            // Create hover indicator
            if (hoverIndicatorPrefab != null)
            {
                hoverIndicator = Instantiate(hoverIndicatorPrefab, transform);
                hoverIndicatorRenderer = hoverIndicator.GetComponent<SpriteRenderer>();
                hoverIndicator.SetActive(false);
                isHoverIndicatorActive = false;
            }

            // Subscribe to events
            piecePlacement.OnPiecePlaced += Handle_OnPiecePlaced;
            // We can also subscribe to invalid placement to give feedback

            Debug.Log("PieceVisualizer initialized");
        }

        /// <summary>
        /// Updates the hover indicator position and appearance
        /// </summary>
        private void UpdateHoverIndicator()
        {
            if (hoverIndicator == null || !piecePlacement.GameActive) 
            {
                if (isHoverIndicatorActive) 
                {
                    hoverIndicator.SetActive(false);
                    isHoverIndicatorActive = false;
                }
                return;
            }

            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0;

            if (intersectionDetector.TryGetIntersectionFromWorldPosition(mouseWorldPos, out int x, out int y))
            {
                bool isValidPlacement = !piecePlacement.IsPositionOccupied(x, y);

                // Update indicator position
                hoverIndicator.transform.position = gameBoard.BoardToWorldPosition(x, y);

                // Update indicator color based on validity
                if (hoverIndicatorRenderer != null)
                {
                    hoverIndicatorRenderer.color = isValidPlacement ? validPlacementColor : invalidPlacementColor;
                }

                // Show indicator
                if (!isHoverIndicatorActive) 
                {
                    hoverIndicator.SetActive(true);
                    isHoverIndicatorActive = true;
                }
            }
            else
            {
                // Hide indicator if not over an intersection
                if (isHoverIndicatorActive)
                {
                    hoverIndicator.SetActive(false);
                    isHoverIndicatorActive = false;
                }
            }
        }

        /// <summary>
        /// Handles the piece placed event
        /// </summary>
        private void Handle_OnPiecePlaced(Vector2Int position, PlayerType player)
        {
            // Optionally, create a highlight or animation on the placed piece
            // e.g., a brief flash or a growing circle effect
            Debug.Log($"Visualizer received OnPiecePlaced event at ({position.x}, {position.y})");
        }

        /// <summary>
        /// Enables or disables the hover feedback
        /// </summary>
        public void SetHoverFeedback(bool enabled)
        {
            enableHoverFeedback = enabled;
            if (!enabled && isHoverIndicatorActive)
            {
                hoverIndicator.SetActive(false);
                isHoverIndicatorActive = false;
            }
        }

        /// <summary>
        /// Cleans up event subscriptions
        /// </summary>
        private void CleanupEvents()
        {
            if (piecePlacement != null)
            {
                piecePlacement.OnPiecePlaced -= Handle_OnPiecePlaced;
            }
        }
    }
}
