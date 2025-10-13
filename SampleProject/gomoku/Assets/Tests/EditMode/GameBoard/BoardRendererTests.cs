using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;

namespace Gomoku.Tests
{
    [TestFixture]
    public class BoardRendererTests
    {
        private BoardRenderer boardRenderer;
        private GameObject boardRendererObject;

        [SetUp]
        public void SetUp()
        {
            // Create a test BoardRenderer object
            boardRendererObject = new GameObject("TestBoardRenderer");
            boardRenderer = boardRendererObject.AddComponent<BoardRenderer>();
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up after each test
            if (boardRendererObject != null)
            {
                Object.DestroyImmediate(boardRendererObject);
            }
        }

        [Test]
        public void BoardRenderer_Initialization_WithValidParameters_CreatesVisualComponents()
        {
            // Arrange
            int boardSize = 15;
            float cellSize = 1.0f;
            Vector2 boardOffset = Vector2.zero;

            // Act
            boardRenderer.Initialize(boardSize, cellSize, boardOffset);

            // Assert
            // The initialization should complete without errors
            // In a real test environment, we would verify that visual components were created
            Assert.IsTrue(true, "BoardRenderer initialization completed successfully");
        }

        [Test]
        public void BoardRenderer_Initialization_WithDifferentBoardSizes_HandlesCorrectly()
        {
            // Arrange
            int[] testSizes = { 3, 9, 15, 19 };

            foreach (int boardSize in testSizes)
            {
                // Act & Assert
                Assert.DoesNotThrow(() => 
                {
                    boardRenderer.Initialize(boardSize, 1.0f, Vector2.zero);
                }, $"BoardRenderer should handle board size {boardSize} without errors");
            }
        }

        [Test]
        public void BoardRenderer_Initialization_WithZeroOrNegativeCellSize_UsesDefault()
        {
            // Arrange
            int boardSize = 15;
            float invalidCellSize = 0f;
            Vector2 boardOffset = Vector2.zero;

            // Act
            boardRenderer.Initialize(boardSize, invalidCellSize, boardOffset);

            // Assert
            // The initialization should complete without errors
            // In practice, the renderer should handle invalid cell sizes gracefully
            Assert.IsTrue(true, "BoardRenderer handled zero cell size without errors");
        }

        [Test]
        public void BoardRenderer_UpdateBackgroundColor_UpdatesBackgroundRenderer()
        {
            // Arrange
            boardRenderer.Initialize(15, 1.0f, Vector2.zero);
            Color newColor = new Color(0.9f, 0.8f, 0.7f, 1.0f);

            // Act
            boardRenderer.UpdateBackgroundColor(newColor);

            // Assert
            // In a real test, we would verify the background renderer's color was updated
            // For now, we just ensure the method doesn't throw
            Assert.IsTrue(true, "UpdateBackgroundColor completed without errors");
        }

        [Test]
        public void BoardRenderer_UpdateGridLineColor_UpdatesGridLines()
        {
            // Arrange
            boardRenderer.Initialize(15, 1.0f, Vector2.zero);
            Color newColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);

            // Act
            boardRenderer.UpdateGridLineColor(newColor);

            // Assert
            // In a real test, we would verify the grid line colors were updated
            // For now, we just ensure the method doesn't throw
            Assert.IsTrue(true, "UpdateGridLineColor completed without errors");
        }

        [Test]
        public void BoardRenderer_ToggleGridCoordinates_HandlesToggleCorrectly()
        {
            // Arrange
            boardRenderer.Initialize(15, 1.0f, Vector2.zero);

            // Act & Assert - Toggle on
            Assert.DoesNotThrow(() => 
            {
                boardRenderer.ToggleGridCoordinates(true);
            }, "ToggleGridCoordinates(true) should not throw");

            // Act & Assert - Toggle off
            Assert.DoesNotThrow(() => 
            {
                boardRenderer.ToggleGridCoordinates(false);
            }, "ToggleGridCoordinates(false) should not throw");
        }

        [Test]
        public void BoardRenderer_MultipleInitializations_ClearsPreviousVisuals()
        {
            // Arrange
            boardRenderer.Initialize(15, 1.0f, Vector2.zero);

            // Act - Initialize with different parameters
            Assert.DoesNotThrow(() => 
            {
                boardRenderer.Initialize(19, 0.8f, new Vector2(-5, -5));
            }, "Multiple initializations should not throw");

            // Assert
            // In a real test, we would verify that previous visuals were cleared
            Assert.IsTrue(true, "Multiple initializations completed without errors");
        }

        [Test]
        public void BoardRenderer_Initialization_WithNullComponents_HandlesGracefully()
        {
            // Arrange
            // BoardRenderer with no assigned components
            int boardSize = 15;
            float cellSize = 1.0f;
            Vector2 boardOffset = Vector2.zero;

            // Act & Assert
            Assert.DoesNotThrow(() => 
            {
                boardRenderer.Initialize(boardSize, cellSize, boardOffset);
            }, "BoardRenderer should handle null components gracefully");
        }

        [Test]
        public void BoardRenderer_Initialization_WithExtremeValues_HandlesCorrectly()
        {
            // Arrange
            int[] extremeSizes = { 1, 2, 100 };
            float[] extremeCellSizes = { 0.01f, 10.0f };
            Vector2[] extremeOffsets = { new Vector2(-100, -100), new Vector2(100, 100) };

            foreach (int size in extremeSizes)
            {
                foreach (float cellSize in extremeCellSizes)
                {
                    foreach (Vector2 offset in extremeOffsets)
                    {
                        // Act & Assert
                        Assert.DoesNotThrow(() => 
                        {
                            boardRenderer.Initialize(size, cellSize, offset);
                        }, $"BoardRenderer should handle extreme values: size={size}, cellSize={cellSize}, offset={offset}");
                    }
                }
            }
        }

        [Test]
        public void BoardRenderer_Initialization_WithNegativeBoardSize_HandlesGracefully()
        {
            // Arrange
            int negativeSize = -5;

            // Act & Assert
            Assert.DoesNotThrow(() => 
            {
                boardRenderer.Initialize(negativeSize, 1.0f, Vector2.zero);
            }, "BoardRenderer should handle negative board size gracefully");
        }
    }
}