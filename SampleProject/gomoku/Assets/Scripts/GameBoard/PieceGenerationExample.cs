using UnityEngine;
using Gomoku.Core;
using Gomoku.UI;

namespace Gomoku
{
    /// <summary>
    /// Example script demonstrating how to use the piece generation functions
    /// </summary>
    public class PieceGenerationExample : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameBoardController gameBoardController;

        [Header("Generation Settings")]
        [SerializeField] private bool generateOnStart = true;
        [SerializeField] private TestPattern startPattern = TestPattern.Basic;

        private void Start()
        {
            if (generateOnStart && gameBoardController != null)
            {
                // Wait a frame for the board to be fully initialized
                Invoke(nameof(CreateTestBoard), 0.1f);
            }
        }

        /// <summary>
        /// Creates a test board with the specified pattern
        /// </summary>
        public void CreateTestBoard()
        {
            if (gameBoardController != null && gameBoardController.IsInitialized)
            {
                gameBoardController.CreateTestBoard(startPattern);
            }
        }

        /// <summary>
        /// Example: Place individual pieces
        /// </summary>
        [ContextMenu("Place Example Pieces")]
        public void PlaceExamplePieces()
        {
            if (gameBoardController == null) return;

            // Clear board first
            gameBoardController.ClearBoard();

            // Place some example pieces
            gameBoardController.PlacePiece(7, 7, PlayerType.PlayerOne);   // Center black piece
            gameBoardController.PlacePiece(7, 8, PlayerType.PlayerTwo);   // White piece above
            gameBoardController.PlacePiece(8, 7, PlayerType.PlayerTwo);   // White piece to the right
            gameBoardController.PlacePiece(6, 7, PlayerType.PlayerTwo);   // White piece to the left
            gameBoardController.PlacePiece(7, 6, PlayerType.PlayerTwo);   // White piece below
        }

        /// <summary>
        /// Example: Generate a winning line
        /// </summary>
        [ContextMenu("Generate Winning Line")]
        public void GenerateWinningLine()
        {
            if (gameBoardController == null) return;

            // Clear board first
            gameBoardController.ClearBoard();

            // Generate a 5-in-a-row horizontally
            gameBoardController.GeneratePieceLine(5, 7, LineDirection.Horizontal, 5, PlayerType.PlayerOne);

            Debug.Log("Generated winning line: PlayerOne has 5 pieces horizontally");
        }

        /// <summary>
        /// Example: Generate random pieces
        /// </summary>
        [ContextMenu("Generate Random Pieces")]
        public void GenerateRandomPieces()
        {
            if (gameBoardController == null) return;

            // Clear board first
            gameBoardController.ClearBoard();

            // Generate 20 random pieces
            for (int i = 0; i < 20; i++)
            {
                PlayerType playerType = (i % 2 == 0) ? PlayerType.PlayerOne : PlayerType.PlayerTwo;
                gameBoardController.GenerateRandomPiece(playerType, true);
            }

            Debug.Log("Generated 20 random pieces");
        }

        /// <summary>
        /// Example: Use PieceData array
        /// </summary>
        [ContextMenu("Generate from PieceData")]
        public void GenerateFromPieceData()
        {
            if (gameBoardController == null) return;

            // Clear board first
            gameBoardController.ClearBoard();

            // Create piece data array
            PieceData[] pieces = new PieceData[]
            {
                new PieceData(7, 7, PlayerType.PlayerOne),
                new PieceData(7, 8, PlayerType.PlayerTwo),
                new PieceData(8, 8, PlayerType.PlayerOne),
                new PieceData(8, 7, PlayerType.PlayerTwo),
                new PieceData(9, 9, PlayerType.PlayerOne),
                new PieceData(6, 6, PlayerType.PlayerTwo)
            };

            // Generate pieces from data
            int placedCount = gameBoardController.GeneratePieces(pieces);
            Debug.Log($"Generated {placedCount} pieces from PieceData array");
        }

        /// <summary>
        /// Clear all pieces from the board
        /// </summary>
        [ContextMenu("Clear Board")]
        public void ClearBoard()
        {
            if (gameBoardController != null)
            {
                gameBoardController.ClearBoard();
                Debug.Log("Board cleared");
            }
        }

        /// <summary>
        /// Test diagonal lines
        /// </summary>
        [ContextMenu("Test Diagonal Lines")]
        public void TestDiagonalLines()
        {
            if (gameBoardController == null) return;

            // Clear board first
            gameBoardController.ClearBoard();

            // Generate diagonal line
            gameBoardController.GeneratePieceLine(5, 5, LineDirection.Diagonal, 4, PlayerType.PlayerOne);

            // Generate anti-diagonal line
            gameBoardController.GeneratePieceLine(9, 5, LineDirection.AntiDiagonal, 3, PlayerType.PlayerTwo);

            Debug.Log("Generated diagonal test patterns");
        }

        private void OnValidate()
        {
            // Try to find GameBoardController if not assigned
            if (gameBoardController == null)
            {
                gameBoardController = FindObjectOfType<GameBoardController>();
            }
        }
    }
}