using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;

namespace Gomoku.Tests
{
    [TestFixture]
    public class GameBoardTests
    {
        private GameBoard gameBoard;
        private GameObject gameBoardObject;

        [SetUp]
        public void SetUp()
        {
            // Create a test GameBoard object
            gameBoardObject = new GameObject("TestGameBoard");
            gameBoard = gameBoardObject.AddComponent<GameBoard>();
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up after each test
            if (gameBoardObject != null)
            {
                Object.DestroyImmediate(gameBoardObject);
            }
        }

        [Test]
        public void GameBoard_Initialization_WithValidSize_SetsPropertiesCorrectly()
        {
            // Arrange
            int expectedSize = 15;
            float expectedCellSize = 1.0f;
            Vector2 expectedOffset = Vector2.zero;

            // Act
            gameBoard.UpdateBoardConfiguration(expectedSize, expectedCellSize, expectedOffset);

            // Assert
            Assert.AreEqual(expectedSize, gameBoard.BoardSize, "Board size should be set correctly");
            Assert.AreEqual(expectedCellSize, gameBoard.CellSize, "Cell size should be set correctly");
            Assert.AreEqual(expectedOffset, gameBoard.BoardOffset, "Board offset should be set correctly");
        }

        [Test]
        public void GameBoard_Initialization_WithInvalidSize_ClampsToMinimum()
        {
            // Arrange
            int invalidSize = 2; // Below minimum of 3

            // Act
            gameBoard.UpdateBoardConfiguration(invalidSize, 1.0f, Vector2.zero);

            // Assert
            Assert.AreEqual(3, gameBoard.BoardSize, "Board size should be clamped to minimum of 3");
        }

        [Test]
        public void GameBoard_BoardToWorldPosition_WithValidCoordinates_ReturnsCorrectPosition()
        {
            // Arrange
            int boardSize = 15;
            float cellSize = 1.0f;
            Vector2 boardOffset = new Vector2(-7, -7);
            gameBoard.UpdateBoardConfiguration(boardSize, cellSize, boardOffset);

            // Test various positions
            int[,] testCoordinates = {
                {0, 0}, {7, 7}, {14, 14}, {0, 14}, {14, 0}
            };

            Vector3[] expectedPositions = {
                new Vector3(-7, -7, 0),
                new Vector3(0, 0, 0),
                new Vector3(7, 7, 0),
                new Vector3(-7, 7, 0),
                new Vector3(7, -7, 0)
            };

            for (int i = 0; i < testCoordinates.GetLength(0); i++)
            {
                int x = testCoordinates[i, 0];
                int y = testCoordinates[i, 1];
                Vector3 expectedPosition = expectedPositions[i];

                // Act
                Vector3 actualPosition = gameBoard.BoardToWorldPosition(x, y);

                // Assert
                Assert.AreEqual(expectedPosition, actualPosition, 
                    $"Position for ({x}, {y}) should be {expectedPosition}");
            }
        }

        [Test]
        public void GameBoard_BoardToWorldPosition_WithInvalidCoordinates_ReturnsZeroAndLogsWarning()
        {
            // Arrange
            gameBoard.UpdateBoardConfiguration(15, 1.0f, Vector2.zero);

            // Test invalid coordinates
            int[,] invalidCoordinates = {
                {-1, 0}, {15, 0}, {0, -1}, {0, 15}, {-1, -1}, {15, 15}
            };

            for (int i = 0; i < invalidCoordinates.GetLength(0); i++)
            {
                int x = invalidCoordinates[i, 0];
                int y = invalidCoordinates[i, 1];

                // Act
                Vector3 actualPosition = gameBoard.BoardToWorldPosition(x, y);

                // Assert
                Assert.AreEqual(Vector3.zero, actualPosition, 
                    $"Position for invalid coordinates ({x}, {y}) should be Vector3.zero");
            }
        }

        [Test]
        public void GameBoard_WorldToBoardPosition_WithValidWorldPosition_ReturnsCorrectCoordinates()
        {
            // Arrange
            int boardSize = 15;
            float cellSize = 1.0f;
            Vector2 boardOffset = new Vector2(-7, -7);
            gameBoard.UpdateBoardConfiguration(boardSize, cellSize, boardOffset);

            // Test various world positions
            Vector3[] testWorldPositions = {
                new Vector3(-7, -7, 0),  // Should be (0, 0)
                new Vector3(0, 0, 0),    // Should be (7, 7)
                new Vector3(7, 7, 0),    // Should be (14, 14)
                new Vector3(-6.5f, -6.5f, 0),  // Should round to (1, 1)
                new Vector3(6.5f, 6.5f, 0)     // Should round to (13, 13)
            };

            int[,] expectedCoordinates = {
                {0, 0}, {7, 7}, {14, 14}, {1, 1}, {13, 13}
            };

            for (int i = 0; i < testWorldPositions.Length; i++)
            {
                Vector3 worldPosition = testWorldPositions[i];
                int expectedX = expectedCoordinates[i, 0];
                int expectedY = expectedCoordinates[i, 1];

                // Act
                bool success = gameBoard.WorldToBoardPosition(worldPosition, out int actualX, out int actualY);

                // Assert
                Assert.IsTrue(success, $"World position {worldPosition} should be valid");
                Assert.AreEqual(expectedX, actualX, $"X coordinate should be {expectedX}");
                Assert.AreEqual(expectedY, actualY, $"Y coordinate should be {expectedY}");
            }
        }

        [Test]
        public void GameBoard_WorldToBoardPosition_WithInvalidWorldPosition_ReturnsFalse()
        {
            // Arrange
            gameBoard.UpdateBoardConfiguration(15, 1.0f, Vector2.zero);

            // Test world positions outside the board
            Vector3[] invalidWorldPositions = {
                new Vector3(-10, 0, 0),   // Too far left
                new Vector3(10, 0, 0),    // Too far right
                new Vector3(0, -10, 0),   // Too far down
                new Vector3(0, 10, 0),    // Too far up
                new Vector3(-10, -10, 0), // Far outside
                new Vector3(10, 10, 0)    // Far outside
            };

            foreach (Vector3 worldPosition in invalidWorldPositions)
            {
                // Act
                bool success = gameBoard.WorldToBoardPosition(worldPosition, out int x, out int y);

                // Assert
                Assert.IsFalse(success, $"World position {worldPosition} should be invalid");
                Assert.AreEqual(-1, x, "X coordinate should be -1 for invalid position");
                Assert.AreEqual(-1, y, "Y coordinate should be -1 for invalid position");
            }
        }

        [Test]
        public void GameBoard_IsValidCoordinate_WithValidCoordinates_ReturnsTrue()
        {
            // Arrange
            gameBoard.UpdateBoardConfiguration(15, 1.0f, Vector2.zero);

            // Test all valid coordinates
            for (int x = 0; x < 15; x++)
            {
                for (int y = 0; y < 15; y++)
                {
                    // Act
                    bool isValid = gameBoard.IsValidCoordinate(x, y);

                    // Assert
                    Assert.IsTrue(isValid, $"Coordinate ({x}, {y}) should be valid");
                }
            }
        }

        [Test]
        public void GameBoard_IsValidCoordinate_WithInvalidCoordinates_ReturnsFalse()
        {
            // Arrange
            gameBoard.UpdateBoardConfiguration(15, 1.0f, Vector2.zero);

            // Test invalid coordinates
            int[,] invalidCoordinates = {
                {-1, 0}, {15, 0}, {0, -1}, {0, 15}, {-1, -1}, {15, 15},
                {-5, 5}, {5, -5}, {20, 5}, {5, 20}, {-10, -10}, {20, 20}
            };

            for (int i = 0; i < invalidCoordinates.GetLength(0); i++)
            {
                int x = invalidCoordinates[i, 0];
                int y = invalidCoordinates[i, 1];

                // Act
                bool isValid = gameBoard.IsValidCoordinate(x, y);

                // Assert
                Assert.IsFalse(isValid, $"Coordinate ({x}, {y}) should be invalid");
            }
        }

        [Test]
        public void GameBoard_GetBoardBounds_ReturnsCorrectBounds()
        {
            // Arrange
            int boardSize = 15;
            float cellSize = 1.0f;
            Vector2 boardOffset = new Vector2(-7, -7);
            gameBoard.UpdateBoardConfiguration(boardSize, cellSize, boardOffset);

            Vector3 expectedCenter = new Vector3(0, 0, 0);
            Vector3 expectedSize = new Vector3(14, 14, 0.1f);

            // Act
            Bounds bounds = gameBoard.GetBoardBounds();

            // Assert
            Assert.AreEqual(expectedCenter, bounds.center, "Bounds center should be correct");
            Assert.AreEqual(expectedSize, bounds.size, "Bounds size should be correct");
        }

        [Test]
        public void GameBoard_Properties_ReturnCorrectValues()
        {
            // Arrange
            int boardSize = 15;
            float cellSize = 1.5f;
            Vector2 boardOffset = new Vector2(-10, -10);
            gameBoard.UpdateBoardConfiguration(boardSize, cellSize, boardOffset);

            float expectedWidth = (boardSize - 1) * cellSize; // 14 * 1.5 = 21
            float expectedHeight = (boardSize - 1) * cellSize; // 14 * 1.5 = 21

            // Act & Assert
            Assert.AreEqual(expectedWidth, gameBoard.BoardWidth, "Board width should be calculated correctly");
            Assert.AreEqual(expectedHeight, gameBoard.BoardHeight, "Board height should be calculated correctly");
        }
    }
}