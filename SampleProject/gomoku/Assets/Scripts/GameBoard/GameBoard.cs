using UnityEngine;

namespace Gomoku
{
    /// <summary>
    /// Main component responsible for managing the 15x15 game board logic
    /// </summary>
    public class GameBoardController : MonoBehaviour
    {
        [Header("Board Configuration")]
        [SerializeField] private int boardSize = 15;
        [SerializeField] private float cellSize = 1.0f;
        [SerializeField] private Vector2 boardOffset = Vector2.zero;

        [Header("Visual Components")]
        [SerializeField] private BoardRenderer boardRenderer;
        [SerializeField] private IntersectionDetector intersectionDetector;

        /// <summary>
        /// Gets the size of the board (15x15)
        /// </summary>
        public int BoardSize => boardSize;

        /// <summary>
        /// Gets the size of each cell in world units
        /// </summary>
        public float CellSize => cellSize;

        /// <summary>
        /// Gets the board offset in world units
        /// </summary>
        public Vector2 BoardOffset => boardOffset;

        /// <summary>
        /// Gets the total board width in world units
        /// </summary>
        public float BoardWidth => (boardSize - 1) * cellSize;

        /// <summary>
        /// Gets the total board height in world units
        /// </summary>
        public float BoardHeight => (boardSize - 1) * cellSize;

        private void Awake()
        {
            ValidateComponents();
            InitializeBoard();
        }

        /// <summary>
        /// Validates that required components are properly assigned
        /// </summary>
        private void ValidateComponents()
        {
            if (boardRenderer == null)
            {
                boardRenderer = GetComponentInChildren<BoardRenderer>();
                if (boardRenderer == null)
                {
                    Debug.LogError("BoardRenderer component not found on GameBoard or its children.");
                }
            }

            if (intersectionDetector == null)
            {
                intersectionDetector = GetComponentInChildren<IntersectionDetector>();
                if (intersectionDetector == null)
                {
                    Debug.LogError("IntersectionDetector component not found on GameBoard or its children.");
                }
            }
        }

        /// <summary>
        /// Initializes the game board with default configuration
        /// </summary>
        private void InitializeBoard()
        {
            // Ensure board size is valid
            if (boardSize < 3)
            {
                Debug.LogWarning("Board size too small. Setting to minimum size of 3.");
                boardSize = 3;
            }

            // Initialize visual components
            if (boardRenderer != null)
            {
                boardRenderer.Initialize(boardSize, cellSize, boardOffset);
            }

            if (intersectionDetector != null)
            {
                intersectionDetector.Initialize(boardSize, cellSize, boardOffset);
            }

            Debug.Log($"GameBoard initialized with size {boardSize}x{boardSize}");
        }

        /// <summary>
        /// Converts board coordinates to world position
        /// </summary>
        /// <param name="x">X coordinate (0-14)</param>
        /// <param name="y">Y coordinate (0-14)</param>
        /// <returns>World position of the intersection</returns>
        public Vector3 BoardToWorldPosition(int x, int y)
        {
            if (!IsValidCoordinate(x, y))
            {
                Debug.LogWarning($"Invalid board coordinates: ({x}, {y})");
                return Vector3.zero;
            }

            float worldX = boardOffset.x + x * cellSize;
            float worldY = boardOffset.y + y * cellSize;

            return new Vector3(worldX, worldY, 0);
        }

        /// <summary>
        /// Converts world position to board coordinates
        /// </summary>
        /// <param name="worldPosition">World position to convert</param>
        /// <param name="x">Output X coordinate</param>
        /// <param name="y">Output Y coordinate</param>
        /// <returns>True if the position corresponds to a valid board coordinate</returns>
        public bool WorldToBoardPosition(Vector3 worldPosition, out int x, out int y)
        {
            x = Mathf.RoundToInt((worldPosition.x - boardOffset.x) / cellSize);
            y = Mathf.RoundToInt((worldPosition.y - boardOffset.y) / cellSize);

            return IsValidCoordinate(x, y);
        }

        /// <summary>
        /// Checks if the given coordinates are within the valid board range
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <returns>True if coordinates are valid</returns>
        public bool IsValidCoordinate(int x, int y)
        {
            return x >= 0 && x < boardSize && y >= 0 && y < boardSize;
        }

        /// <summary>
        /// Gets the bounds of the entire board in world space
        /// </summary>
        /// <returns>Bounds of the board</returns>
        public Bounds GetBoardBounds()
        {
            Vector3 center = new Vector3(boardOffset.x + BoardWidth / 2, boardOffset.y + BoardHeight / 2, 0);
            Vector3 size = new Vector3(BoardWidth, BoardHeight, 0.1f);
            return new Bounds(center, size);
        }

        /// <summary>
        /// Updates the board configuration (can be called at runtime)
        /// </summary>
        /// <param name="newBoardSize">New board size</param>
        /// <param name="newCellSize">New cell size</param>
        /// <param name="newBoardOffset">New board offset</param>
        public void UpdateBoardConfiguration(int newBoardSize, float newCellSize, Vector2 newBoardOffset)
        {
            boardSize = newBoardSize;
            cellSize = newCellSize;
            boardOffset = newBoardOffset;

            // Update visual components
            if (boardRenderer != null)
            {
                boardRenderer.Initialize(boardSize, cellSize, boardOffset);
            }

            if (intersectionDetector != null)
            {
                intersectionDetector.Initialize(boardSize, cellSize, boardOffset);
            }
        }

        /// <summary>
        /// Initializes the game board for testing purposes with default configuration
        /// </summary>
        /// <param name="size">Size of the board</param>
        public void InitializeForTest(int size)
        {
            boardSize = size;

            // Initialize visual components
            if (boardRenderer != null)
            {
                boardRenderer.Initialize(boardSize, cellSize, boardOffset);
            }

            if (intersectionDetector != null)
            {
                intersectionDetector.Initialize(boardSize, cellSize, boardOffset);
            }

            Debug.Log($"GameBoard initialized for test with size {boardSize}x{boardSize}");
        }

        /// <summary>
        /// Notifies the board renderer that a piece has been placed
        /// </summary>
        /// <param name="pieceType">The type of piece that was placed</param>
        /// <param name="x">X coordinate on the board</param>
        /// <param name="y">Y coordinate on the board</param>
        public void NotifyPiecePlaced(PlayerType pieceType, int x, int y)
        {
            if (boardRenderer != null)
            {
                boardRenderer.PlacePiece(pieceType, x, y);
            }
        }

        /// <summary>
        /// Notifies the board renderer that a piece has been removed
        /// </summary>
        /// <param name="x">X coordinate on the board</param>
        /// <param name="y">Y coordinate on the board</param>
        public void NotifyPieceRemoved(int x, int y)
        {
            if (boardRenderer != null)
            {
                boardRenderer.RemovePiece(x, y);
            }
        }

        #if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            // Draw board bounds in editor
            Gizmos.color = Color.green;
            Bounds bounds = GetBoardBounds();
            Gizmos.DrawWireCube(bounds.center, bounds.size);

            // Draw grid intersections
            Gizmos.color = Color.white;
            for (int x = 0; x < boardSize; x++)
            {
                for (int y = 0; y < boardSize; y++)
                {
                    Vector3 position = BoardToWorldPosition(x, y);
                    Gizmos.DrawWireSphere(position, 0.05f);
                }
            }
        }
        #endif
    }
}