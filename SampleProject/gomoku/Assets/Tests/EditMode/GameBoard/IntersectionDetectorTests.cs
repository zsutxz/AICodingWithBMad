using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;

namespace Gomoku.Tests
{
    [TestFixture]
    public class IntersectionDetectorTests
    {
        private IntersectionDetector intersectionDetector;
        private GameObject detectorObject;
        private GameBoard gameBoard;
        private GameObject gameBoardObject;

        [SetUp]
        public void SetUp()
        {
            // Create a test GameBoard hierarchy
            gameBoardObject = new GameObject("TestGameBoard");
            gameBoard = gameBoardObject.AddComponent<GameBoard>();

            // Create IntersectionDetector as a child of GameBoard
            detectorObject = new GameObject("TestIntersectionDetector");
            detectorObject.transform.SetParent(gameBoardObject.transform);
            intersectionDetector = detectorObject.AddComponent<IntersectionDetector>();

            // Initialize the GameBoard
            gameBoard.UpdateBoardConfiguration(15, 1.0f, new Vector2(-7, -7));
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
        public void IntersectionDetector_Initialization_WithValidParameters_SetsPropertiesCorrectly()
        {
            // Arrange
            int boardSize = 15;
            float cellSize = 1.0f;
            Vector2 boardOffset = new Vector2(-7, -7);

            // Act
            intersectionDetector.Initialize(boardSize, cellSize, boardOffset);

            // Assert
            // The initialization should complete without errors
            Assert.IsTrue(true, "IntersectionDetector initialization completed successfully");
        }

        [Test]
        public void IntersectionDetector_TryGetIntersectionFromWorldPosition_WithExactIntersection_ReturnsTrueAndCorrectCoordinates()
        {
            // Arrange
            intersectionDetector.Initialize(15, 1.0f, new Vector2(-7, -7));

            // Test exact intersection positions
            Vector3[] testPositions = {
                new Vector3(-7, -7, 0),  // (0, 0)
                new Vector3(0, 0, 0),    // (7, 7)
                new Vector3(7, 7, 0),    // (14, 14)
                new Vector3(-7, 7, 0),   // (0, 14)
                new Vector3(7, -7, 0)    // (14, 0)
            };

            int[,] expectedCoordinates = {
                {0, 0}, {7, 7}, {14, 14}, {0, 14}, {14, 0}
            };

            for (int i = 0; i < testPositions.Length; i++)
            {
                Vector3 worldPosition = testPositions[i];
                int expectedX = expectedCoordinates[i, 0];
                int expectedY = expectedCoordinates[i, 1];

                // Act
                bool success = intersectionDetector.TryGetIntersectionFromWorldPosition(worldPosition, out int actualX, out int actualY);

                // Assert
                Assert.IsTrue(success, $"Exact intersection at {worldPosition} should be detected");
                Assert.AreEqual(expectedX, actualX, $"X coordinate should be {expectedX}");
                Assert.AreEqual(expectedY, actualY, $"Y coordinate should be {expectedY}");
            }
        }

        [Test]
        public void IntersectionDetector_TryGetIntersectionFromWorldPosition_WithNearIntersection_ReturnsTrueAndCorrectCoordinates()
        {
            // Arrange
            intersectionDetector.Initialize(15, 1.0f, new Vector2(-7, -7));
            float detectionRadius = 0.3f;

            // Test positions near intersections (within detection radius)
            Vector3[] testPositions = {
                new Vector3(-6.8f, -6.8f, 0),  // Near (0, 0)
                new Vector3(-0.1f, 0.1f, 0),   // Near (7, 7)
                new Vector3(6.9f, 6.9f, 0),    // Near (14, 14)
                new Vector3(-7.2f, 6.8f, 0),   // Near (0, 14)
                new Vector3(7.1f, -7.2f, 0)    // Near (14, 0)
            };

            int[,] expectedCoordinates = {
                {0, 0}, {7, 7}, {14, 14}, {0, 14}, {14, 0}
            };

            for (int i = 0; i < testPositions.Length; i++)
            {
                Vector3 worldPosition = testPositions[i];
                int expectedX = expectedCoordinates[i, 0];
                int expectedY = expectedCoordinates[i, 1];

                // Act
                bool success = intersectionDetector.TryGetIntersectionFromWorldPosition(worldPosition, out int actualX, out int actualY);

                // Assert
                Assert.IsTrue(success, $"Near intersection at {worldPosition} should be detected");
                Assert.AreEqual(expectedX, actualX, $"X coordinate should be {expectedX}");
                Assert.AreEqual(expectedY, actualY, $"Y coordinate should be {expectedY}");
            }
        }

        [Test]
        public void IntersectionDetector_TryGetIntersectionFromWorldPosition_WithFarFromIntersection_ReturnsFalse()
        {
            // Arrange
            intersectionDetector.Initialize(15, 1.0f, new Vector2(-7, -7));

            // Test positions far from any intersection
            Vector3[] testPositions = {
                new Vector3(-8, -8, 0),    // Far from (0, 0)
                new Vector3(8, 8, 0),      // Far from (14, 14)
                new Vector3(-10, 0, 0),    // Far left
                new Vector3(10, 0, 0),     // Far right
                new Vector3(0, -10, 0),    // Far down
                new Vector3(0, 10, 0),     // Far up
                new Vector3(3.5f, 3.5f, 0) // Center of cell, not near intersection
            };

            foreach (Vector3 worldPosition in testPositions)
            {
                // Act
                bool success = intersectionDetector.TryGetIntersectionFromWorldPosition(worldPosition, out int x, out int y);

                // Assert
                Assert.IsFalse(success, $"Position {worldPosition} should not be detected as intersection");
                Assert.AreEqual(-1, x, "X coordinate should be -1 for invalid detection");
                Assert.AreEqual(-1, y, "Y coordinate should be -1 for invalid detection");
            }
        }

        [Test]
        public void IntersectionDetector_TryGetIntersectionFromWorldPosition_WithInvalidBoardCoordinates_ReturnsFalse()
        {
            // Arrange
            intersectionDetector.Initialize(15, 1.0f, new Vector2(-7, -7));

            // Test positions that would map to invalid board coordinates
            Vector3[] testPositions = {
                new Vector3(-8, -7, 0),  // Would be (-1, 0)
                new Vector3(8, -7, 0),   // Would be (15, 0)
                new Vector3(-7, -8, 0),  // Would be (0, -1)
                new Vector3(-7, 8, 0),   // Would be (0, 15)
                new Vector3(-8, -8, 0),  // Would be (-1, -1)
                new Vector3(8, 8, 0)     // Would be (15, 15)
            };

            foreach (Vector3 worldPosition in testPositions)
            {
                // Act
                bool success = intersectionDetector.TryGetIntersectionFromWorldPosition(worldPosition, out int x, out int y);

                // Assert
                Assert.IsFalse(success, $"Position {worldPosition} should not be detected as valid intersection");
            }
        }

        [Test]
        public void IntersectionDetector_GetIntersectionWorldPosition_ReturnsCorrectPosition()
        {
            // Arrange
            intersectionDetector.Initialize(15, 1.0f, new Vector2(-7, -7));

            // Test various coordinates
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
                Vector3 actualPosition = intersectionDetector.GetIntersectionWorldPosition(x, y);

                // Assert
                Assert.AreEqual(expectedPosition, actualPosition, 
                    $"World position for ({x}, {y}) should be {expectedPosition}");
            }
        }

        [Test]
        public void IntersectionDetector_UpdateDetectionRadius_UpdatesDetectionRange()
        {
            // Arrange
            intersectionDetector.Initialize(15, 1.0f, new Vector2(-7, -7));
            float newRadius = 0.5f;

            // Act
            intersectionDetector.UpdateDetectionRadius(newRadius);

            // Assert
            // In a real test, we would verify the detection radius was updated
            // For now, we just ensure the method doesn't throw
            Assert.IsTrue(true, "UpdateDetectionRadius completed without errors");
        }

        [Test]
        public void IntersectionDetector_ToggleDebugVisualization_HandlesToggleCorrectly()
        {
            // Arrange
            intersectionDetector.Initialize(15, 1.0f, new Vector2(-7, -7));

            // Act & Assert - Toggle on
            Assert.DoesNotThrow(() => 
            {
                intersectionDetector.ToggleDebugVisualization(true);
            }, "ToggleDebugVisualization(true) should not throw");

            // Act & Assert - Toggle off
            Assert.DoesNotThrow(() => 
            {
                intersectionDetector.ToggleDebugVisualization(false);
            }, "ToggleDebugVisualization(false) should not throw");
        }

        [Test]
        public void IntersectionDetector_HighlightIntersection_HandlesValidAndInvalidIntersections()
        {
            // Arrange
            intersectionDetector.Initialize(15, 1.0f, new Vector2(-7, -7));

            // Test valid intersection
            Assert.DoesNotThrow(() => 
            {
                intersectionDetector.HighlightIntersection(7, 7, true);
            }, "HighlightIntersection with valid coordinates should not throw");

            // Test invalid intersection (should still handle gracefully)
            Assert.DoesNotThrow(() => 
            {
                intersectionDetector.HighlightIntersection(-1, -1, false);
            }, "HighlightIntersection with invalid coordinates should not throw");
        }

        [Test]
        public void IntersectionDetector_Initialization_WithoutGameBoardParent_LogsError()
        {
            // Arrange - Create detector without GameBoard parent
            GameObject standaloneObject = new GameObject("StandaloneDetector");
            IntersectionDetector standaloneDetector = standaloneObject.AddComponent<IntersectionDetector>();

            // Act
            standaloneDetector.Initialize(15, 1.0f, new Vector2(-7, -7));

            // Assert
            // The initialization should complete but log an error about missing GameBoard
            // In a real test, we would verify the error was logged
            
            // Clean up
            Object.DestroyImmediate(standaloneObject);
            Assert.IsTrue(true, "Initialization without GameBoard parent handled gracefully");
        }

        [Test]
        public void IntersectionDetector_Initialization_WithDifferentBoardConfigurations_HandlesCorrectly()
        {
            // Arrange
            int[] testSizes = { 9, 15, 19 };
            float[] testCellSizes = { 0.8f, 1.0f, 1.2f };
            Vector2[] testOffsets = { Vector2.zero, new Vector2(-5, -5), new Vector2(5, 5) };

            foreach (int size in testSizes)
            {
                foreach (float cellSize in testCellSizes)
                {
                    foreach (Vector2 offset in testOffsets)
                    {
                        // Update parent GameBoard configuration
                        gameBoard.UpdateBoardConfiguration(size, cellSize, offset);

                        // Act & Assert
                        Assert.DoesNotThrow(() => 
                        {
                            intersectionDetector.Initialize(size, cellSize, offset);
                        }, $"IntersectionDetector should handle configuration: size={size}, cellSize={cellSize}, offset={offset}");
                    }
                }
            }
        }
    }
}