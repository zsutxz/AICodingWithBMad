using NUnit.Framework;
using UnityEngine;
using Gomoku.UI;

namespace Gomoku.Tests
{
    [TestFixture]
    public class PiecePlacementTests
    {
        private PiecePlacement piecePlacement;
        private GameBoardController gameBoard;
        private TurnManager turnManager;
        private IntersectionDetector intersectionDetector;

        [SetUp]
        public void SetUp()
        {
            GameObject go = new GameObject("TestSetup");
            gameBoard = go.AddComponent<GameBoardController>();
            turnManager = go.AddComponent<TurnManager>();
            intersectionDetector = go.AddComponent<IntersectionDetector>();
            piecePlacement = go.AddComponent<PiecePlacement>();

            // Initialize components as they would be in the game
            gameBoard.InitializeForTest(15);
            intersectionDetector.Initialize(15, 1.0f, Vector2.zero);
            piecePlacement.InitializeGame();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(piecePlacement.gameObject);
        }

        [Test]
        public void TryPlacePiece_Places_Piece_On_Empty_Intersection()
        {
            bool placed = piecePlacement.TryPlacePiece(0, 0, PlayerType.Black);
            Assert.IsTrue(placed);
            Assert.AreEqual(PlayerType.Black, piecePlacement.BoardState[0, 0]);
        }

        [Test]
        public void TryPlacePiece_Does_Not_Place_Piece_On_Occupied_Intersection()
        {
            piecePlacement.TryPlacePiece(0, 0, PlayerType.Black);
            bool placed = piecePlacement.TryPlacePiece(0, 0, PlayerType.White);
            Assert.IsFalse(placed);
            Assert.AreEqual(PlayerType.Black, piecePlacement.BoardState[0, 0]);
        }

        [Test]
        public void TryPlacePiece_Does_Not_Place_Piece_On_Invalid_Coordinates()
        {
            bool placed = piecePlacement.TryPlacePiece(-1, 15, PlayerType.Black);
            Assert.IsFalse(placed);
        }

        [Test]
        public void PlacePiece_Switches_Turn()
        {
            // TurnManager starts with Black by default
            piecePlacement.TryPlacePiece(0, 0, turnManager.CurrentPlayer);
            Assert.AreEqual(PlayerType.White, turnManager.CurrentPlayer);
        }
    }
}
