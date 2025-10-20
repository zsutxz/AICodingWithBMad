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
        [SerializeField] private int boardSize = 15;
        [SerializeField] private float cellSize = 1f;
        [SerializeField] private Vector2 boardOffset = Vector2.zero;

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
        /// <param name="pieceType">Type of piece to place</param>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <returns>True if piece was successfully placed</returns>
        public bool PlacePiece(PlayerType pieceType, int x, int y)
        {
            if (!isInitialized || !IsValidCoordinate(x, y))
            {
                return false;
            }

            // Update the renderer
            boardRenderer.PlacePiece(pieceType, x, y);
            return true;
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
