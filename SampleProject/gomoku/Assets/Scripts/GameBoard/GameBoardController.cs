using UnityEngine;
using Gomoku;
using Gomoku.Core;
using System;

namespace Gomoku.UI
{
    /// <summary>
    /// Manages the game board UI components and coordinates between rendering and input detection
    /// </summary>
    public class GameBoardController : MonoBehaviour
    {
        [Header("Board Configuration")]
        [SerializeField] private int boardSize = 0;
        [SerializeField] private float cellSize = 50;
        [SerializeField] private Vector2 boardOffset = new Vector2(0, 0);

        [Header("Components")]
        [SerializeField] private BoardRenderer boardRenderer;
        [SerializeField] private IntersectionDetector intersectionDetector;

        // Internal state
        private bool isInitialized = false;

        /// <summary>
        /// Gets the size of the board
        /// </summary>
        public int BoardSize => boardSize;

        /// <summary>
        /// Gets the size of each cell in world units
        /// </summary>
        public float CellSize => cellSize;

        /// <summary>
        /// Gets the offset of the board in world units
        /// </summary>
        public Vector2 BoardOffset => boardOffset;

        /// <summary>
        /// Gets whether the board is initialized
        /// </summary>
        public bool IsInitialized => isInitialized;

        public float BoardWidth { get; internal set; }
        public float BoardHeight { get; internal set; }

        private void Awake()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes the game board controller and its components
        /// </summary>
        public void Initialize()
        {
            if (isInitialized)
            {
                Debug.LogWarning("GameBoardController is already initialized");
                return;
            }

            // Validate references
            if (boardRenderer == null)
            {
                Debug.LogError("GameBoardController: BoardRenderer reference is not set");
                return;
            }

            if (intersectionDetector == null)
            {
                Debug.LogError("GameBoardController: IntersectionDetector reference is not set");
                return;
            }

            // Initialize components
            boardRenderer.Initialize(boardSize, cellSize, boardOffset);
            intersectionDetector.Initialize(boardSize, cellSize, boardOffset);

            isInitialized = true;
            Debug.Log($"GameBoardController initialized with {boardSize}x{boardSize} board");
        }

        /// <summary>
        /// Checks if the given coordinates are valid on the board
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <returns>True if coordinates are valid</returns>
        public bool IsValidCoordinate(int x, int y)
        {
            return x >= 0 && x < boardSize && y >= 0 && y < boardSize;
        }

        /// <summary>
        /// Converts board coordinates to world position
        /// </summary>
        /// <param name="x">X coordinate on board</param>
        /// <param name="y">Y coordinate on board</param>
        /// <returns>World position</returns>
        public Vector3 BoardToWorldPosition(int x, int y)
        {
            return new Vector3(
                boardOffset.x + x * cellSize,
                boardOffset.y + y * cellSize,
                0f
            );
        }

        /// <summary>
        /// Converts world position to board coordinates
        /// </summary>
        /// <param name="worldPosition">World position</param>
        /// <param name="x">Output X coordinate</param>
        /// <param name="y">Output Y coordinate</param>
        /// <returns>True if successfully converted to valid coordinates</returns>
        public bool WorldToBoardPosition(Vector3 worldPosition, out int x, out int y)
        {
            x = y = -1;

            // Calculate relative position to board offset
            float relX = worldPosition.x - boardOffset.x;
            float relY = worldPosition.y - boardOffset.y;

            // Convert to board coordinates by dividing by cell size and rounding
            x = Mathf.RoundToInt(relX / cellSize);
            y = Mathf.RoundToInt(relY / cellSize);

            // Validate coordinates
            return IsValidCoordinate(x, y);
        }

        /// <summary>
        /// Places a piece on the board at the specified coordinates
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="playerType">Type of player piece to place</param>
        /// <returns>True if piece was successfully placed</returns>
        public bool PlacePiece(int x, int y, PlayerType playerType)
        {
            if (!isInitialized || !IsValidCoordinate(x, y))
            {
                Debug.LogWarning($"Cannot place piece at ({x}, {y}): Board not initialized or invalid coordinates");
                return false;
            }

            // Update the renderer
            boardRenderer.PlacePiece(playerType, x, y);
            Debug.Log($"Placed {playerType} piece at board coordinates ({x}, {y})");
            return true;
        }

        /// <summary>
        /// Generates multiple pieces on the board for testing or setup purposes
        /// </summary>
        /// <param name="pieceData">Array of piece data containing coordinates and player types</param>
        /// <returns>Number of successfully placed pieces</returns>
        public int GeneratePieces(PieceData[] pieceData)
        {
            if (!isInitialized)
            {
                Debug.LogError("Cannot generate pieces: Board not initialized");
                return 0;
            }

            if (pieceData == null || pieceData.Length == 0)
            {
                Debug.LogWarning("No piece data provided for generation");
                return 0;
            }

            int successCount = 0;
            foreach (var piece in pieceData)
            {
                if (PlacePiece(piece.X, piece.Y, piece.PlayerType))
                {
                    successCount++;
                }
            }

            Debug.Log($"Generated {successCount}/{pieceData.Length} pieces on the board");
            return successCount;
        }

        /// <summary>
        /// Generates a random piece on the board
        /// </summary>
        /// <param name="playerType">Type of player piece to place</param>
        /// <param name="avoidExistingPieces">Whether to avoid positions with existing pieces</param>
        /// <returns>True if piece was successfully placed</returns>
        public bool GenerateRandomPiece(PlayerType playerType, bool avoidExistingPieces = true)
        {
            if (!isInitialized)
            {
                Debug.LogError("Cannot generate random piece: Board not initialized");
                return false;
            }

            // Try multiple times to find a valid position
            for (int attempts = 0; attempts < 100; attempts++)
            {
                int x = UnityEngine.Random.Range(0, boardSize);
                int y = UnityEngine.Random.Range(0, boardSize);

                if (PlacePiece(x, y, playerType))
                {
                    Debug.Log($"Generated random {playerType} piece at ({x}, {y})");
                    return true;
                }

                if (!avoidExistingPieces)
                {
                    break;
                }
            }

            Debug.LogWarning($"Failed to generate random {playerType} piece after 100 attempts");
            return false;
        }

        /// <summary>
        /// Generates a line of pieces for testing purposes
        /// </summary>
        /// <param name="startX">Starting X coordinate</param>
        /// <param name="startY">Starting Y coordinate</param>
        /// <param name="direction">Direction of the line (horizontal, vertical, or diagonal)</param>
        /// <param name="length">Length of the line</param>
        /// <param name="playerType">Type of player pieces to place</param>
        /// <returns>Number of successfully placed pieces</returns>
        public int GeneratePieceLine(int startX, int startY, LineDirection direction, int length, PlayerType playerType)
        {
            if (!isInitialized)
            {
                Debug.LogError("Cannot generate piece line: Board not initialized");
                return 0;
            }

            if (length <= 0)
            {
                Debug.LogWarning("Invalid line length for piece generation");
                return 0;
            }

            int successCount = 0;
            int dx = 0, dy = 0;

            // Set direction offsets
            switch (direction)
            {
                case LineDirection.Horizontal:
                    dx = 1;
                    break;
                case LineDirection.Vertical:
                    dy = 1;
                    break;
                case LineDirection.Diagonal:
                    dx = 1;
                    dy = 1;
                    break;
                case LineDirection.AntiDiagonal:
                    dx = 1;
                    dy = -1;
                    break;
            }

            for (int i = 0; i < length; i++)
            {
                int x = startX + i * dx;
                int y = startY + i * dy;

                if (PlacePiece(x, y, playerType))
                {
                    successCount++;
                }
            }

            Debug.Log($"Generated {successCount}/{length} {playerType} pieces in {direction} line starting at ({startX}, {startY})");
            return successCount;
        }

        /// <summary>
        /// Removes a piece from the board at the specified coordinates
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <returns>True if piece was successfully removed</returns>
        public bool RemovePiece(int x, int y)
        {
            if (!isInitialized || !IsValidCoordinate(x, y))
            {
                return false;
            }

            // Update the renderer
            boardRenderer.RemovePiece(x, y);
            return true;
        }

        /// <summary>
        /// Clears all pieces from the board
        /// </summary>
        public void ClearBoard()
        {
            if (!isInitialized)
            {
                Debug.LogWarning("GameBoardController not initialized. Cannot clear board.");
                return;
            }

            // Clear pieces from the renderer
            if (boardRenderer != null)
            {
                boardRenderer.ClearBoard();
            }
            else
            {
                Debug.LogError("BoardRenderer reference not set. Cannot clear board.");
            }

            Debug.Log("Game board cleared successfully");
        }

        /// <summary>
        /// Updates the board size and reinitializes components
        /// </summary>
        /// <param name="newSize">New board size</param>
        public void UpdateBoardSize(int newSize)
        {
            if (newSize < 2 || newSize > 25)
            {
                Debug.LogError($"Invalid board size: {newSize}. Must be between 2 and 25.");
                return;
            }

            boardSize = newSize;

            // Reinitialize components with new size
            if (isInitialized)
            {
                Initialize();
            }
        }

        /// <summary>
        /// Updates the cell size and reinitializes components
        /// </summary>
        /// <param name="newSize">New cell size</param>
        public void UpdateCellSize(float newSize)
        {
            if (newSize <= 0)
            {
                Debug.LogError($"Invalid cell size: {newSize}. Must be greater than 0.");
                return;
            }

            cellSize = newSize;

            // Reinitialize components with new size
            if (isInitialized)
            {
                Initialize();
            }
        }

        /// <summary>
        /// Updates the board offset and reinitializes components
        /// </summary>
        /// <param name="newOffset">New board offset</param>
        public void UpdateBoardOffset(Vector2 newOffset)
        {
            boardOffset = newOffset;

            // Reinitialize components with new offset
            if (isInitialized)
            {
                Initialize();
            }
        }

        /// <summary>
        /// Updates the board configuration and reinitializes components
        /// </summary>
        /// <param name="boardSize">New board size</param>
        /// <param name="cellSize">New cell size</param>
        /// <param name="boardOffset">New board offset</param>
        public void UpdateBoardConfiguration(int boardSize, float cellSize, Vector2 boardOffset)
        {
            // Validate parameters
            if (boardSize < 2 || boardSize > 25)
            {
                Debug.LogError($"Invalid board size: {boardSize}. Must be between 2 and 25.");
                return;
            }

            if (cellSize <= 0)
            {
                Debug.LogError($"Invalid cell size: {cellSize}. Must be greater than 0.");
                return;
            }

            // Update configuration
            this.boardSize = boardSize;
            this.cellSize = cellSize;
            this.boardOffset = boardOffset;

            // Reinitialize components with new configuration
            if (isInitialized)
            {
                Initialize();
            }
        }

        /// <summary>
        /// Gets the bounds of the game board in world space
        /// </summary>
        /// <returns>The bounds of the game board</returns>
        public Bounds GetBoardBounds()
        {
            if (!isInitialized)
            {
                Debug.LogWarning("GameBoardController not initialized. Returning default bounds.");
                return new Bounds(Vector3.zero, Vector3.one);
            }

            // Calculate board bounds
            float boardWidth = (boardSize - 1) * cellSize;
            float boardHeight = (boardSize - 1) * cellSize;
            Vector3 center = new Vector3(boardOffset.x + boardWidth / 2, boardOffset.y + boardHeight / 2, 0);
            Vector3 size = new Vector3(boardWidth, boardHeight, 0.1f);

            return new Bounds(center, size);
        }

        /// <summary>
        /// Creates a sample board configuration for testing
        /// </summary>
        /// <param name="patternType">Type of pattern to create</param>
        public void CreateTestBoard(TestPattern patternType = TestPattern.Basic)
        {
            if (!isInitialized)
            {
                Debug.LogError("Cannot create test board: Board not initialized");
                return;
            }

            // Clear any existing pieces
            ClearBoard();

            switch (patternType)
            {
                case TestPattern.Basic:
                    // Create a simple pattern with a few pieces
                    PlacePiece(7, 7, PlayerType.PlayerOne);   // Center black piece
                    PlacePiece(7, 8, PlayerType.PlayerTwo);   // White piece above
                    PlacePiece(8, 7, PlayerType.PlayerTwo);   // White piece to the right
                    PlacePiece(6, 7, PlayerType.PlayerTwo);   // White piece to the left
                    break;

                case TestPattern.LineTest:
                    // Test winning lines
                    GeneratePieceLine(5, 7, LineDirection.Horizontal, 5, PlayerType.PlayerOne);
                    GeneratePieceLine(7, 5, LineDirection.Vertical, 4, PlayerType.PlayerTwo);
                    break;

                case TestPattern.DiagonalTest:
                    // Test diagonal patterns
                    GeneratePieceLine(5, 5, LineDirection.Diagonal, 4, PlayerType.PlayerOne);
                    GeneratePieceLine(9, 5, LineDirection.AntiDiagonal, 3, PlayerType.PlayerTwo);
                    break;

                case TestPattern.Random:
                    // Generate random pieces
                    for (int i = 0; i < 10; i++)
                    {
                        PlayerType playerType = (i % 2 == 0) ? PlayerType.PlayerOne : PlayerType.PlayerTwo;
                        GenerateRandomPiece(playerType, true);
                    }
                    break;

                case TestPattern.FullLine:
                    // Test near-winning scenarios
                    GeneratePieceLine(3, 7, LineDirection.Horizontal, 4, PlayerType.PlayerOne);
                    PlacePiece(8, 7, PlayerType.PlayerTwo);
                    GeneratePieceLine(7, 3, LineDirection.Vertical, 4, PlayerType.PlayerOne);
                    PlacePiece(7, 8, PlayerType.PlayerTwo);
                    break;
            }

            Debug.Log($"Created test board with pattern: {patternType}");
        }

        /// <summary>
        /// Initializes the game board controller for testing purposes
        /// </summary>
        /// <param name="testBoardSize">Board size for testing</param>
        public void InitializeForTest(int testBoardSize)
        {
            // Set test configuration
            boardSize = testBoardSize;
            cellSize = 1.0f;
            boardOffset = Vector2.zero;

            // Initialize components
            if (boardRenderer != null)
            {
                boardRenderer.Initialize(boardSize, cellSize, boardOffset);
            }

            if (intersectionDetector != null)
            {
                intersectionDetector.Initialize(boardSize, cellSize, boardOffset);
            }

            isInitialized = true;
            Debug.Log($"GameBoardController initialized for testing with {boardSize}x{boardSize} board");
        }
    }
}
