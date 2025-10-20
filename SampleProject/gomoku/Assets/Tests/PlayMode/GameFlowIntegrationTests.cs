using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using System.Collections;
using Gomoku;
using Gomoku.UI;
using Gomoku.Core;

namespace Gomoku.Tests.PlayMode
{
    /// <summary>
    /// Integration tests for complete game flow scenarios
    /// Tests the end-to-end game experience from start to finish
    /// </summary>
    public class GameFlowIntegrationTests
    {
        private GameStateManager gameStateManager;
        private ScreenManager screenManager;
        private UIManager uiManager;
        private GameOverScreen gameOverScreen;
        private WinDetector winDetector;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            // Load the main menu scene
            yield return SceneManager.LoadSceneAsync("MainMenu");

            // Find or create required components
            gameStateManager = Object.FindObjectOfType<GameStateManager>();
            if (gameStateManager == null)
            {
                var gameStateManagerObj = new GameObject("GameStateManager");
                gameStateManager = gameStateManagerObj.AddComponent<GameStateManager>();
            }

            screenManager = Object.FindObjectOfType<ScreenManager>();
            if (screenManager == null)
            {
                var screenManagerObj = new GameObject("ScreenManager");
                screenManager = screenManagerObj.AddComponent<ScreenManager>();
            }

            uiManager = Object.FindObjectOfType<UIManager>();
            if (uiManager == null)
            {
                var uiManagerObj = new GameObject("UIManager");
                uiManager = uiManagerObj.AddComponent<UIManager>();
            }

            // Wait for scene to fully load
            yield return new WaitForSeconds(0.5f);
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            // Clean up any created objects
            var gameStateManagers = Object.FindObjectsOfType<GameStateManager>();
            foreach (var manager in gameStateManagers)
            {
                if (manager != null)
                {
                    Object.DestroyImmediate(manager.gameObject);
                }
            }

            var screenManagers = Object.FindObjectsOfType<ScreenManager>();
            foreach (var manager in screenManagers)
            {
                if (manager != null)
                {
                    Object.DestroyImmediate(manager.gameObject);
                }
            }

            var uiManagers = Object.FindObjectsOfType<UIManager>();
            foreach (var manager in uiManagers)
            {
                if (manager != null)
                {
                    Object.DestroyImmediate(manager.gameObject);
                }
            }

            yield return null;
        }

        [UnityTest]
        public IEnumerator GameFlow_CompleteSession_FromMainMenuToGameOver()
        {
            // Arrange - Start from main menu
            Assert.AreEqual(GameState.MainMenu, gameStateManager.GetCurrentState(),
                "Game should start in MainMenu state");

            // Act - Transition to playing state
            gameStateManager.SetState(GameState.Playing);
            yield return new WaitForSeconds(0.5f);

            // Assert - Should be in playing state
            Assert.AreEqual(GameState.Playing, gameStateManager.GetCurrentState(),
                "Game should transition to Playing state");

            // Act - Simulate game over (victory condition)
            gameStateManager.SetState(GameState.GameOver);
            yield return new WaitForSeconds(0.5f);

            // Assert - Should be in game over state
            Assert.AreEqual(GameState.GameOver, gameStateManager.GetCurrentState(),
                "Game should transition to GameOver state");

            // Act - Return to main menu
            gameStateManager.SetState(GameState.MainMenu);
            yield return new WaitForSeconds(0.5f);

            // Assert - Should return to main menu
            Assert.AreEqual(GameState.MainMenu, gameStateManager.GetCurrentState(),
                "Game should return to MainMenu state");
        }

        [UnityTest]
        public IEnumerator GameFlow_PauseAndResume_WorksCorrectly()
        {
            // Arrange - Start from playing state
            gameStateManager.SetState(GameState.Playing);
            yield return new WaitForSeconds(0.5f);

            // Act - Pause the game
            gameStateManager.PauseGame();
            yield return new WaitForSeconds(0.5f);

            // Assert - Should be in paused state
            Assert.AreEqual(GameState.Paused, gameStateManager.GetCurrentState(),
                "Game should be in Paused state after pausing");

            // Act - Resume the game
            gameStateManager.ResumeGame();
            yield return new WaitForSeconds(0.5f);

            // Assert - Should return to playing state
            Assert.AreEqual(GameState.Playing, gameStateManager.GetCurrentState(),
                "Game should return to Playing state after resuming");
        }

        [UnityTest]
        public IEnumerator GameFlow_RestartGame_ResetsStateCorrectly()
        {
            // Arrange - Start from game over state
            gameStateManager.SetState(GameState.GameOver);
            yield return new WaitForSeconds(0.5f);

            // Act - Restart the game
            gameStateManager.RestartGame();
            yield return new WaitForSeconds(0.5f);

            // Assert - Should return to playing state
            Assert.AreEqual(GameState.Playing, gameStateManager.GetCurrentState(),
                "Game should return to Playing state after restart");
        }

        [UnityTest]
        public IEnumerator GameFlow_MultipleSessions_NoStateCorruption()
        {
            // Test multiple complete game sessions to ensure no state corruption
            for (int i = 0; i < 3; i++)
            {
                // Complete game session
                gameStateManager.SetState(GameState.Playing);
                yield return new WaitForSeconds(0.2f);
                
                gameStateManager.SetState(GameState.GameOver);
                yield return new WaitForSeconds(0.2f);
                
                gameStateManager.SetState(GameState.MainMenu);
                yield return new WaitForSeconds(0.2f);

                // Verify state integrity
                Assert.AreEqual(GameState.MainMenu, gameStateManager.GetCurrentState(),
                    $"Game should be in MainMenu state after session {i + 1}");
            }
        }

        [UnityTest]
        public IEnumerator ScreenManager_Navigation_WorksCorrectly()
        {
            // Test screen navigation through ScreenManager
            
            // Navigate to game scene
            screenManager.LoadGameScene();
            yield return new WaitForSeconds(1.0f);

            // Verify we're in the game scene
            Assert.AreEqual("GameScene", screenManager.GetCurrentSceneName(),
                "ScreenManager should load GameScene correctly");

            // Navigate to game over scene
            screenManager.LoadGameOverScene();
            yield return new WaitForSeconds(1.0f);

            // Verify we're in the game over scene
            Assert.AreEqual("GameOver", screenManager.GetCurrentSceneName(),
                "ScreenManager should load GameOver scene correctly");

            // Navigate back to main menu
            screenManager.LoadMainMenu();
            yield return new WaitForSeconds(1.0f);

            // Verify we're back in main menu
            Assert.AreEqual("MainMenu", screenManager.GetCurrentSceneName(),
                "ScreenManager should load MainMenu scene correctly");
        }

        [UnityTest]
        public IEnumerator UIManager_StateChanges_UpdateUIProperly()
        {
            // Test that UI Manager responds to state changes
            
            // Start in main menu
            gameStateManager.SetState(GameState.MainMenu);
            yield return new WaitForSeconds(0.5f);

            // UI should be active
            Assert.IsTrue(uiManager.gameObject.activeSelf,
                "UIManager should be active in MainMenu state");

            // Transition to playing
            gameStateManager.SetState(GameState.Playing);
            yield return new WaitForSeconds(0.5f);

            // UI should still be active
            Assert.IsTrue(uiManager.gameObject.activeSelf,
                "UIManager should be active in Playing state");

            // Transition to paused
            gameStateManager.SetState(GameState.Paused);
            yield return new WaitForSeconds(0.5f);

            // UI should still be active
            Assert.IsTrue(uiManager.gameObject.activeSelf,
                "UIManager should be active in Paused state");
        }

        [UnityTest]
        public IEnumerator GameOverScreen_DisplaysCorrectly()
        {
            // Test GameOverScreen functionality
            
            // Create GameOverScreen if it doesn't exist
            gameOverScreen = Object.FindObjectOfType<GameOverScreen>();
            if (gameOverScreen == null)
            {
                var gameOverScreenObj = new GameObject("GameOverScreen");
                gameOverScreen = gameOverScreenObj.AddComponent<GameOverScreen>();
            }

            // Show game over screen
            gameOverScreen.ShowGameOver(PlayerType.PlayerOne);
            yield return new WaitForSeconds(0.5f);

            // Verify screen is active
            Assert.IsTrue(gameOverScreen.gameObject.activeSelf,
                "GameOverScreen should be active after ShowGameOver");

            // Hide game over screen
            gameOverScreen.HideGameOver();
            yield return new WaitForSeconds(0.5f);

            // Verify screen is hidden
            Assert.IsFalse(gameOverScreen.gameObject.activeSelf,
                "GameOverScreen should be hidden after HideGameOver");
        }

        [UnityTest]
        public IEnumerator MemoryUsage_StableAcrossMultipleSessions()
        {
            // Test for memory leaks across multiple game sessions
            var initialMemory = System.GC.GetTotalMemory(true);
            
            // Run multiple game sessions
            for (int i = 0; i < 5; i++)
            {
                gameStateManager.SetState(GameState.Playing);
                yield return new WaitForSeconds(0.1f);
                
                gameStateManager.SetState(GameState.GameOver);
                yield return new WaitForSeconds(0.1f);
                
                gameStateManager.SetState(GameState.MainMenu);
                yield return new WaitForSeconds(0.1f);
            }

            // Force garbage collection
            System.GC.Collect();
            yield return new WaitForSeconds(0.5f);

            var finalMemory = System.GC.GetTotalMemory(true);
            
            // Memory usage should be stable (within reasonable bounds)
            var memoryIncrease = finalMemory - initialMemory;
            Assert.Less(memoryIncrease, 10 * 1024 * 1024, // Less than 10MB increase
                "Memory usage should remain stable across multiple game sessions");
        }
    }
}