using UnityEngine;
using System.Collections.Generic;

namespace Gomoku.GameBoard
{
    /// <summary>
    /// Represents a move on the game board
    /// </summary>
    public struct BoardMove
    {
        public int X;
        public int Y;
        public Gomoku.PlayerType Player;

        public BoardMove(int x, int y, Gomoku.PlayerType player)
        {
            X = x;
            Y = y;
            Player = player;
        }
    }

    /// <summary>
    /// Manages the game board state and move history
    /// </summary>
    public class GameBoardModel : MonoBehaviour
    {
        [Header("Board Settings")]
        [Tooltip("Size of the game board (e.g., 15 for a 15x15 board)")]
        [SerializeField] private int boardSize = 15;

        [Header("References")]
        [Tooltip("Reference to the Piece prefab for creating pieces")]
        [SerializeField] private GameObject piecePrefab;

        // 2D array to store the state of each board position
        // 0 = empty, 1 = black piece, 2 = white piece
        private int[,] boardState;

        // List to store the history of moves made in the game
        private List<BoardMove> moveHistory;

        // Reference to the parent transform for piece placement
        private Transform piecesParent;

        /// <summary>
        /// Initialize the board model
        /// </summary>
        private void Awake()
        {
            InitializeBoard();
            InitializeMoveHistory();

            // Find or create the parent transform for pieces
            piecesParent = transform.Find("Pieces");
            if (piecesParent == null)
            {
                GameObject piecesObj = new GameObject("Pieces");
                piecesObj.transform.SetParent(transform);
                piecesParent = piecesObj.transform;
            }

            Debug.Log($"GameBoardModel initialized with {boardSize}x{boardSize} board");
        }

        /// <summary>
        /// Initialize the board state array
        /// </summary>
        private void InitializeBoard()
        {
            boardState = new int[boardSize, boardSize];
            ClearBoard();
        }

        /// <summary>
        /// Initialize the move history list
        /// </summary>
        private void InitializeMoveHistory()
        {
            moveHistory = new List<BoardMove>();
        }

        /// <summary>
        /// Clear the board state (set all positions to empty)
        /// </summary>
        public void ClearBoard()
        {
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    boardState[i, j] = 0;
                }
            }
        }

        /// <summary>
        /// Reset the board to its initial state
        /// </summary>
        public void Reset()
        {
            ClearBoard();
            moveHistory.Clear();

            // Remove all piece game objects from the scene
            if (piecesParent != null)
            {
                foreach (Transform child in piecesParent)
                {
                    Destroy(child.gameObject);
                }
            }

            Debug.Log("Game board reset to initial state");
        }

        /// <summary>
        /// Check if a position on the board is empty
        /// </summary>
        /// <param name="x">X coordinate of the position</param>
        /// <param name="y">Y coordinate of the position</param>
        /// <returns>True if the position is empty, false otherwise</returns>
        public bool IsPositionEmpty(int x, int y)
        {
            if (x < 0 || x >= boardSize || y < 0 || y >= boardSize)
            {
                return false; // Position is out of bounds
            }

            return boardState[x, y] == 0;
        }

        /// <summary>
        /// Place a piece on the board at the specified position
        /// </summary>
        /// <param name="x">X coordinate of the position</param>
        /// <param name="y">Y coordinate of the position</param>
        /// <param name="player">The player placing the piece</param>
        /// <returns>True if the piece was placed successfully, false otherwise</returns>
        public bool PlacePiece(int x, int y, Gomoku.PlayerType player)
        {
            // Check if the position is within bounds and empty
            if (x < 0 || x >= boardSize || y < 0 || y >= boardSize)
            {
                Debug.LogWarning($"Attempted to place piece at out-of-bounds position ({x}, {y})");
                return false;
            }

            if (!IsPositionEmpty(x, y))
            {
                Debug.LogWarning($"Attempted to place piece at occupied position ({x}, {y})");
                return false;
            }

            // Update the board state
            boardState[x, y] = (int)player;

            // Add the move to the history
            moveHistory.Add(new BoardMove(x, y, player));

            // Create the visual piece in the scene
            if (piecePrefab != null && piecesParent != null)
            {
                Vector3 position = new Vector3(x, 0, y);
                GameObject piece = Instantiate(piecePrefab, position, Quaternion.identity, piecesParent);
                piece.name = $"Piece_{x}_{y}";

                // The piece should handle its own color based on the player type
                // This will be handled by the Piece component's SetupPiece method
            }

            Debug.Log($"Piece placed at ({x}, {y}) by {player}");
            return true;
        }

        /// <summary>
        /// Get the player who occupies a position on the board
        /// </summary>
        /// <param name="x">X coordinate of the position</param>
        /// <param name="y">Y coordinate of the position</param>
        /// <returns>The player type at the position, or PlayerType.None if empty</returns>
        public Gomoku.PlayerType GetPositionOccupant(int x, int y)
        {
            if (x < 0 || x >= boardSize || y < 0 || y >= boardSize)
            {
                return Gomoku.PlayerType.None;
            }

            int state = boardState[x, y];
            return state switch
            {
                1 => Gomoku.PlayerType.Black,
                2 => Gomoku.PlayerType.White,
                _ => Gomoku.PlayerType.None
            };
        }

        /// <summary>
        /// Get the size of the board
        /// </summary>
        /// <returns>The board size (e.g., 15 for a 15x15 board)</returns>
        public int GetBoardSize()
        {
            return boardSize;
        }

        /// <summary>
        /// Get the current move count
        /// </summary>
        /// <returns>The number of moves made so far</returns>
        public int GetMoveCount()
        {
            return moveHistory.Count;
        }

        /// <summary>
        /// Get the move at a specific index in the move history
        /// </summary>
        /// <param name="index">The index of the move in the history</param>
        /// <returns>The move at the specified index</returns>
        public BoardMove GetMove(int index)
        {
            if (index < 0 || index >= moveHistory.Count)
            {
                Debug.LogError($"Invalid move index: {index}");
                return new BoardMove(-1, -1, Gomoku.PlayerType.None);
            }

            return moveHistory[index];
        }

        /// <summary>
        /// Get the last move made
        /// </summary>
        /// <returns>The most recent move, or a default move if no moves have been made</returns>
        public BoardMove GetLastMove()
        {
            if (moveHistory.Count == 0)
            {
                return new BoardMove(-1, -1, Gomoku.PlayerType.None);
            }

            return moveHistory[moveHistory.Count - 1];
        }

        /// <summary>
        /// Check if the board is empty (no moves made yet)
        /// </summary>
        /// <returns>True if the board is empty, false otherwise</returns>
        public bool IsEmpty()
        {
            return moveHistory.Count == 0;
        }

        /// <summary>
        /// Undo the last move made on the board
        /// </summary>
        /// <returns>True if a move was undone, false if no moves to undo</returns>
        public bool UndoLastMove()
        {
            if (moveHistory.Count == 0)
            {
                Debug.Log("No moves to undo");
                return false;
            }

            // Get the last move
            BoardMove lastMove = moveHistory[moveHistory.Count - 1];

            // Remove the last move from history
            moveHistory.RemoveAt(moveHistory.Count - 1);

            // Clear the board position
            boardState[lastMove.X, lastMove.Y] = 0;

            // Remove the visual piece from the scene
            if (piecesParent != null)
            {
                string pieceName = $"Piece_{lastMove.X}_{lastMove.Y}";
                Transform pieceTransform = piecesParent.Find(pieceName);
                if (pieceTransform != null)
                {
                    Destroy(pieceTransform.gameObject);
                }
            }

            Debug.Log($"Last move undone: {lastMove.Player} at ({lastMove.X}, {lastMove.Y})");
            return true;
        }

        /// <summary>
        /// Gets the entire move history
        /// </summary>
        /// <returns>List of all moves made in the game</returns>
        public System.Collections.Generic.List<BoardMove> GetMoveHistory()
        {
            return moveHistory;
        }
    }
}