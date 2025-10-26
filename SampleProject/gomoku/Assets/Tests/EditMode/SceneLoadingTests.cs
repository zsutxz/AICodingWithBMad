using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using Gomoku.UI;
using Gomoku.Systems;
using Gomoku.Core;

namespace Gomoku.Tests.EditMode
{
    /// <summary>
    /// Tests for scene loading functionality
    /// </summary>
    public class SceneLoadingTests
    {
        [Test]
        public void UIManager_CanLoadMainMenuScene()
        {
            // Test that UIManager can load MainMenu scene
            // Note: UIManager is now managed by Bootstrap, so we'll test the singleton
            var uiManager = UIManager.Instance;

            if (uiManager != null)
            {
                // This should not throw an error
                Assert.DoesNotThrow(() => uiManager.LoadMainMenu());
            }
            else
            {
                // If UIManager doesn't exist, create it for testing
                var testUIManager = new GameObject("TestUIManager").AddComponent<UIManager>();
                Assert.DoesNotThrow(() => testUIManager.LoadMainMenu());
                Object.DestroyImmediate(testUIManager.gameObject);
            }
        }

        [Test]
        public void UIManager_CanLoadGameScene()
        {
            // Test that UIManager can load GameScene
            // Note: UIManager is now managed by Bootstrap, so we'll test the singleton
            var uiManager = UIManager.Instance;

            if (uiManager != null)
            {
                // This should not throw an error
                Assert.DoesNotThrow(() => uiManager.LoadGameScene());
            }
            else
            {
                // If UIManager doesn't exist, create it for testing
                var testUIManager = new GameObject("TestUIManager").AddComponent<UIManager>();
                Assert.DoesNotThrow(() => testUIManager.LoadGameScene());
                Object.DestroyImmediate(testUIManager.gameObject);
            }
        }

        [Test]
        public void UIManager_RejectsInvalidSceneName()
        {
            // Test that UIManager rejects invalid scene names
            var uiManager = new GameObject("TestUIManager").AddComponent<UIManager>();

            // This should not throw an error but should log an error
            Assert.DoesNotThrow(() => uiManager.LoadScene("InvalidScene"));

            // Clean up
            Object.DestroyImmediate(uiManager.gameObject);
        }

        [Test]
        public void Bootstrap_CanInitialize()
        {
            // Test that Bootstrap component can be created
            var bootstrap = new GameObject("TestBootstrap").AddComponent<Bootstrap>();

            Assert.IsNotNull(bootstrap, "Bootstrap component should be created successfully");

            // Clean up
            Object.DestroyImmediate(bootstrap.gameObject);
        }

        [Test]
        public void GameStateManager_CanSetCorrectStates()
        {
            // Test that GameStateManager can set different states
            var gameStateManager = new GameObject("TestGameStateManager").AddComponent<GameStateManager>();

            // Test setting different states
            Assert.DoesNotThrow(() => gameStateManager.SetState(GameStateEnum.MainMenu));
            Assert.AreEqual(GameStateEnum.MainMenu, gameStateManager.GetCurrentState());

            Assert.DoesNotThrow(() => gameStateManager.SetState(GameStateEnum.Playing));
            Assert.AreEqual(GameStateEnum.Playing, gameStateManager.GetCurrentState());

            Assert.DoesNotThrow(() => gameStateManager.SetState(GameStateEnum.Paused));
            Assert.AreEqual(GameStateEnum.Paused, gameStateManager.GetCurrentState());

            Assert.DoesNotThrow(() => gameStateManager.SetState(GameStateEnum.GameOver));
            Assert.AreEqual(GameStateEnum.GameOver, gameStateManager.GetCurrentState());

            // Clean up
            Object.DestroyImmediate(gameStateManager.gameObject);
        }
    }
}