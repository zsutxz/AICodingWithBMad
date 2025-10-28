using NUnit.Framework;
using UnityEngine;
using Gomoku.UI;
using Gomoku.Core;
using System.Collections;

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
            intersectionDetector.Initialize(15, 1.0f, Vector2.zero,new Vector2(1280,720));
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
            bool placed = piecePlacement.TryPlacePiece(0, 0, PlayerType.PlayerOne);
            Assert.IsTrue(placed);
            Assert.AreEqual(PlayerType.PlayerOne, piecePlacement.BoardState[0, 0]);
        }

        [Test]
        public void TryPlacePiece_Does_Not_Place_Piece_On_Occupied_Intersection()
        {
            piecePlacement.TryPlacePiece(0, 0, PlayerType.PlayerOne);
            bool placed = piecePlacement.TryPlacePiece(0, 0, PlayerType.PlayerTwo);
            Assert.IsFalse(placed);
            Assert.AreEqual(PlayerType.PlayerOne, piecePlacement.BoardState[0, 0]);
        }

        [Test]
        public void TryPlacePiece_Does_Not_Place_Piece_On_Invalid_Coordinates()
        {
            bool placed = piecePlacement.TryPlacePiece(-1, 15, PlayerType.PlayerOne);
            Assert.IsFalse(placed);
        }

        [Test]
        public void PlacePiece_Switches_Turn()
        {
            // TurnManager starts with Black by default
            piecePlacement.TryPlacePiece(0, 0, turnManager.CurrentPlayer);
            Assert.AreEqual(PlayerType.PlayerTwo, turnManager.CurrentPlayer);
        }

        // QA Recommended: Tests for rapid click scenarios during turn transitions
        [Test]
        public void Rapid_Consecutive_Clicks_Should_Be_Handled_Correctly()
        {
            // Store initial player
            PlayerType initialPlayer = turnManager.CurrentPlayer;
            int initialTurnCount = turnManager.TurnCount;

            // Simulate rapid clicks on same position
            bool firstClick = piecePlacement.TryPlacePiece(0, 0, initialPlayer);
            bool secondClick = piecePlacement.TryPlacePiece(0, 0, turnManager.CurrentPlayer);
            bool thirdClick = piecePlacement.TryPlacePiece(0, 0, turnManager.CurrentPlayer);

            // Only first click should succeed (position occupied)
            Assert.IsTrue(firstClick, "First click should succeed on empty intersection");
            Assert.IsFalse(secondClick, "Second click should fail on occupied intersection");
            Assert.IsFalse(thirdClick, "Third click should fail on occupied intersection");

            // Turn should only switch once
            Assert.AreEqual(initialTurnCount + 1, turnManager.TurnCount, "Turn count should only increase by 1");
            Assert.AreNotEqual(initialPlayer, turnManager.CurrentPlayer, "Current player should have changed");
        }

        [Test]
        public void Rapid_Clicks_During_Turn_Transition_Should_Maintain_Game_State_Integrity()
        {
            // Place first piece to trigger turn transition
            piecePlacement.TryPlacePiece(0, 0, turnManager.CurrentPlayer);
            PlayerType afterFirstPlacement = turnManager.CurrentPlayer;

            // Immediately try to place another piece during the turn transition
            bool rapidPlacement = piecePlacement.TryPlacePiece(1, 1, afterFirstPlacement);

            // Should succeed as it's now the other player's turn
            Assert.IsTrue(rapidPlacement, "Rapid placement after turn change should succeed");
            Assert.AreEqual(PlayerType.PlayerOne, piecePlacement.BoardState[0, 0]);
            Assert.AreEqual(PlayerType.PlayerTwo, piecePlacement.BoardState[1, 1]);
        }

        [Test]
        public void Simultaneous_Clicks_On_Different_Intersections_Should_Work_Correctly()
        {
            PlayerType currentPlayer = turnManager.CurrentPlayer;
            int initialMoveCount = piecePlacement.MoveCount;

            // Simulate simultaneous clicks on different positions
            bool firstPlacement = piecePlacement.TryPlacePiece(0, 0, currentPlayer);
            bool secondPlacement = piecePlacement.TryPlacePiece(1, 1, currentPlayer);

            // Only first should succeed (same player attempting both)
            Assert.IsTrue(firstPlacement, "First placement should succeed");
            Assert.IsFalse(secondPlacement, "Second placement with same player should fail");

            // Game state should be consistent
            Assert.AreEqual(currentPlayer, piecePlacement.BoardState[0, 0]);
            Assert.AreEqual(PlayerType.None, piecePlacement.BoardState[1, 1]);
            Assert.AreEqual(initialMoveCount + 1, piecePlacement.MoveCount);
        }

        // QA Recommended: Integration tests for complete piece placement flow
        [Test]
        public void Complete_Piece_Placement_Flow_Should_Work_End_To_End()
        {
            // Test complete flow: click detection → validation → placement → turn change
            PlayerType initialPlayer = turnManager.CurrentPlayer;
            int initialTurnCount = turnManager.TurnCount;
            int initialMoveCount = piecePlacement.MoveCount;

            // Simulate intersection click (as would come from IntersectionDetector)
            int targetX = 5;
            int targetY = 5;

            // Verify initial state
            Assert.AreEqual(PlayerType.None, piecePlacement.BoardState[targetX, targetY]);
            Assert.AreEqual(initialTurnCount, turnManager.TurnCount);
            Assert.AreEqual(initialMoveCount, piecePlacement.MoveCount);

            // Execute piece placement
            bool placementSuccess = piecePlacement.TryPlacePiece(targetX, targetY, initialPlayer);

            // Verify all state changes occurred correctly
            Assert.IsTrue(placementSuccess, "Piece placement should succeed");
            Assert.AreEqual(initialPlayer, piecePlacement.BoardState[targetX, targetY], "Board state should show placed piece");
            Assert.AreEqual(initialTurnCount + 1, turnManager.TurnCount, "Turn count should increment");
            Assert.AreEqual(initialMoveCount + 1, piecePlacement.MoveCount, "Move count should increment");
            Assert.AreNotEqual(initialPlayer, turnManager.CurrentPlayer, "Current player should switch");
        }

        [Test]
        public void Piece_Placement_Flow_Should_Handle_Invalid_Coordinates_Gracefully()
        {
            PlayerType initialPlayer = turnManager.CurrentPlayer;
            int initialTurnCount = turnManager.TurnCount;

            // Test various invalid coordinates
            bool negativeCoords = piecePlacement.TryPlacePiece(-1, -1, initialPlayer);
            bool outOfBoundsCoords = piecePlacement.TryPlacePiece(15, 15, initialPlayer);
            bool mixedInvalidCoords = piecePlacement.TryPlacePiece(-1, 15, initialPlayer);

            // All should fail without changing game state
            Assert.IsFalse(negativeCoords, "Negative coordinates should fail");
            Assert.IsFalse(outOfBoundsCoords, "Out of bounds coordinates should fail");
            Assert.IsFalse(mixedInvalidCoords, "Mixed invalid coordinates should fail");

            // Game state should remain unchanged
            Assert.AreEqual(initialPlayer, turnManager.CurrentPlayer, "Current player should not change");
            Assert.AreEqual(initialTurnCount, turnManager.TurnCount, "Turn count should not change");
            Assert.AreEqual(0, piecePlacement.MoveCount, "Move count should not change");
        }

        [Test]
        public void Multiple_Piece_Placements_Should_Maintain_Correct_Game_State()
        {
            // Test sequence of valid placements
            var placements = new[]
            {
                (0, 0, PlayerType.PlayerOne),
                (1, 1, PlayerType.PlayerTwo),
                (2, 2, PlayerType.PlayerOne),
                (3, 3, PlayerType.PlayerTwo)
            };

            for (int i = 0; i < placements.Length; i++)
            {
                var (x, y, expectedPlayer) = placements[i];
                PlayerType currentPlayer = turnManager.CurrentPlayer;

                // Verify it's the expected player's turn
                Assert.AreEqual(expectedPlayer, currentPlayer, $"Turn {i + 1}: Should be {expectedPlayer}'s turn");

                // Place piece
                bool placementSuccess = piecePlacement.TryPlacePiece(x, y, currentPlayer);
                Assert.IsTrue(placementSuccess, $"Turn {i + 1}: Placement at ({x}, {y}) should succeed");

                // Verify board state
                Assert.AreEqual(expectedPlayer, piecePlacement.BoardState[x, y], $"Turn {i + 1}: Board should show {expectedPlayer} at ({x}, {y})");

                // Verify turn switched
                Assert.AreNotEqual(expectedPlayer, turnManager.CurrentPlayer, $"Turn {i + 1}: Player should have switched");
            }

            // Verify final game state
            Assert.AreEqual(4, piecePlacement.MoveCount, "Should have 4 moves in history");
            Assert.AreEqual(4, turnManager.TurnCount, "Should have completed 4 turns");
        }

        [Test]
        public void Piece_Placement_Should_Integrate_With_Turn_Manager_Events()
        {
            bool turnChangedTriggered = false;
            bool turnCompletedTriggered = false;
            PlayerType completedPlayer = PlayerType.None;

            // Subscribe to turn manager events
            turnManager.OnPlayerTurnChanged.AddListener(player => turnChangedTriggered = true);
            turnManager.OnTurnCompleted.AddListener(player =>
            {
                turnCompletedTriggered = true;
                completedPlayer = player;
            });

            // Place a piece
            PlayerType initialPlayer = turnManager.CurrentPlayer;
            piecePlacement.TryPlacePiece(0, 0, initialPlayer);

            // Verify events were triggered
            Assert.IsTrue(turnCompletedTriggered, "Turn completed event should be triggered");
            Assert.IsTrue(turnChangedTriggered, "Turn changed event should be triggered");
            Assert.AreEqual(initialPlayer, completedPlayer, "Completed event should show the player who just moved");
        }
    }
}
