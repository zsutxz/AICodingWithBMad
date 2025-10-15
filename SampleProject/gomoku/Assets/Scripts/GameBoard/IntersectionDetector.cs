using UnityEngine;
using UnityEngine.EventSystems;

namespace Gomoku
{
    /// <summary>
    /// Handles detection of clicks on board intersections and converts them to board coordinates
    /// </summary>
    public class IntersectionDetector : MonoBehaviour, IPointerClickHandler
    {
        [Header("Detection Settings")]
        [SerializeField] private float detectionRadius = 0.3f;
        [SerializeField] private LayerMask detectionLayerMask = -1;

        [Header("Visual Feedback")]
        [SerializeField] private bool showDetectionDebug = false;
        [SerializeField] private Color validIntersectionColor = new Color(0f, 1f, 0f, 0.3f);
        [SerializeField] private Color invalidIntersectionColor = new Color(1f, 0f, 0f, 0.3f);

        private int boardSize;
        private float cellSize;
        private Vector2 boardOffset;
        private GameBoardController gameBoard;

        // Events
        public System.Action<int, int> OnIntersectionClicked;
        public System.Action<int, int> OnValidIntersectionDetected;

        /// <summary>
        /// Initializes the intersection detector with board configuration
        /// </summary>
        /// <param name="boardSize">Size of the board (15x15)</param>
        /// <param name="cellSize">Size of each cell in world units</param>
        /// <param name="boardOffset">Offset of the board in world units</param>
        public void Initialize(int boardSize, float cellSize, Vector2 boardOffset)
        {
            this.boardSize = boardSize;
            this.cellSize = cellSize;
            this.boardOffset = boardOffset;

            // Get reference to parent GameBoard
            gameBoard = GetComponentInParent<GameBoardController>();
            if (gameBoard == null)
            {
                Debug.LogError("IntersectionDetector must be a child of a GameBoardController component.");
            }

            // Ensure we have a collider for detection
            BoxCollider2D collider = GetComponent<BoxCollider2D>();
            if (collider == null)
            {
                collider = gameObject.AddComponent<BoxCollider2D>();
            }

            // Set collider size to cover the entire board
            float boardWidth = (boardSize - 1) * cellSize;
            float boardHeight = (boardSize - 1) * cellSize;
            collider.size = new Vector2(boardWidth, boardHeight);
            collider.offset = new Vector2(boardOffset.x + boardWidth / 2, boardOffset.y + boardHeight / 2);
        }

        /// <summary>
        /// Handles pointer click events on the board
        /// </summary>
        /// <param name="eventData">Pointer event data</param>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;

            Vector3 worldPosition = GetWorldPositionFromPointer(eventData);
            if (TryGetIntersectionFromWorldPosition(worldPosition, out int x, out int y))
            {
                OnIntersectionClicked?.Invoke(x, y);
                
                if (showDetectionDebug)
                {
                    Debug.Log($"Intersection clicked at ({x}, {y})");
                }
            }
        }

        /// <summary>
        /// Converts pointer position to world position
        /// </summary>
        /// <param name="eventData">Pointer event data</param>
        /// <returns>World position of the pointer</returns>
        private Vector3 GetWorldPositionFromPointer(PointerEventData eventData)
        {
            // For UI-based detection
            RectTransform rectTransform = GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                RectTransformUtility.ScreenPointToWorldPointInRectangle(
                    rectTransform, 
                    eventData.position, 
                    eventData.pressEventCamera, 
                    out Vector3 worldPosition
                );
                return worldPosition;
            }
            else
            {
                // For world-space detection
                return Camera.main.ScreenToWorldPoint(eventData.position);
            }
        }

        /// <summary>
        /// Attempts to find the nearest valid intersection from a world position
        /// </summary>
        /// <param name="worldPosition">World position to check</param>
        /// <param name="x">Output X coordinate</param>
        /// <param name="y">Output Y coordinate</param>
        /// <returns>True if a valid intersection was found</returns>
        public bool TryGetIntersectionFromWorldPosition(Vector3 worldPosition, out int x, out int y)
        {
            x = -1;
            y = -1;

            // Convert world position to approximate board coordinates
            float approxX = (worldPosition.x - boardOffset.x) / cellSize;
            float approxY = (worldPosition.y - boardOffset.y) / cellSize;

            // Round to nearest integer coordinates
            int candidateX = Mathf.RoundToInt(approxX);
            int candidateY = Mathf.RoundToInt(approxY);

            // Check if coordinates are valid
            if (gameBoard != null && gameBoard.IsValidCoordinate(candidateX, candidateY))
            {
                // Get the exact intersection position
                Vector3 intersectionPosition = gameBoard.BoardToWorldPosition(candidateX, candidateY);
                
                // Check if the click is within the detection radius of the intersection
                float distance = Vector3.Distance(worldPosition, intersectionPosition);
                if (distance <= detectionRadius)
                {
                    x = candidateX;
                    y = candidateY;
                    OnValidIntersectionDetected?.Invoke(x, y);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the world position of a specific intersection
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <returns>World position of the intersection</returns>
        public Vector3 GetIntersectionWorldPosition(int x, int y)
        {
            if (gameBoard != null)
            {
                return gameBoard.BoardToWorldPosition(x, y);
            }
            else
            {
                return new Vector3(
                    boardOffset.x + x * cellSize,
                    boardOffset.y + y * cellSize,
                    0
                );
            }
        }

        /// <summary>
        /// Highlights a specific intersection for visual feedback
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="isValid">Whether the intersection is valid</param>
        public void HighlightIntersection(int x, int y, bool isValid)
        {
            if (!showDetectionDebug) return;

            Vector3 position = GetIntersectionWorldPosition(x, y);
            Color highlightColor = isValid ? validIntersectionColor : invalidIntersectionColor;

            // Create a temporary visual indicator
            GameObject highlight = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            highlight.transform.position = position;
            highlight.transform.localScale = Vector3.one * detectionRadius * 2;
            
            Renderer renderer = highlight.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = highlightColor;
            }

            // Auto-destroy after a short time
            Destroy(highlight, 1f);
        }

        /// <summary>
        /// Updates the detection radius
        /// </summary>
        /// <param name="newRadius">New detection radius</param>
        public void UpdateDetectionRadius(float newRadius)
        {
            detectionRadius = Mathf.Max(0.1f, newRadius);
        }

        /// <summary>
        /// Toggles debug visualization
        /// </summary>
        /// <param name="show">Whether to show debug visuals</param>
        public void ToggleDebugVisualization(bool show)
        {
            showDetectionDebug = show;
        }

        #if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!showDetectionDebug || boardSize == 0) return;

            // Draw detection radius around each intersection
            Gizmos.color = validIntersectionColor;
            for (int x = 0; x < boardSize; x++)
            {
                for (int y = 0; y < boardSize; y++)
                {
                    Vector3 position = GetIntersectionWorldPosition(x, y);
                    Gizmos.DrawWireSphere(position, detectionRadius);
                }
            }

            // Draw board bounds
            Gizmos.color = Color.blue;
            float boardWidth = (boardSize - 1) * cellSize;
            float boardHeight = (boardSize - 1) * cellSize;
            Vector3 center = new Vector3(
                boardOffset.x + boardWidth / 2,
                boardOffset.y + boardHeight / 2,
                0
            );
            Vector3 size = new Vector3(boardWidth, boardHeight, 0.1f);
            Gizmos.DrawWireCube(center, size);
        }
        #endif
    }
}