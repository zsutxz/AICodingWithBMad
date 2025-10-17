using UnityEngine;
using Gomoku.UI;
namespace Gomoku
{
    /// <summary>
    /// Test script for verifying the BoardRenderer functionality
    /// </summary>
    public class BoardRendererTest : MonoBehaviour
    {
        [SerializeField] private GameBoardController gameBoard;
        [SerializeField] private PiecePlacement piecePlacement;

        private void Start()
        {
            // Initialize the game
            if (gameBoard != null && piecePlacement != null)
            {
                piecePlacement.InitializeGame();

                // Place a few test pieces
                piecePlacement.TryPlacePiece(7, 7, PlayerType.Black);
                piecePlacement.TryPlacePiece(7, 8, PlayerType.White);
                piecePlacement.TryPlacePiece(8, 7, PlayerType.Black);
                piecePlacement.TryPlacePiece(8, 8, PlayerType.White);
            }
        }
    }
}