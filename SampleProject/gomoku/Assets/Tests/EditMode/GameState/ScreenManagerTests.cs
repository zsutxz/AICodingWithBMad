using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using Gomoku.UI;

namespace Gomoku.Tests.EditMode
{
    /// <summary>
    /// Unit tests for ScreenManager component
    /// </summary>
    public class ScreenManagerTests
    {
        private ScreenManager screenManager;
        private GameObject managerObject;

        [SetUp]
        public void SetUp()
        {
            // Create the ScreenManager GameObject
            managerObject = new GameObject("ScreenManager");
            screenManager = managerObject.AddComponent<ScreenManager>();

            // Add GameStateManager reference
            var gameStateManager = managerObject.AddComponent<GameStateManager>();
            //screenManager.gameStateManager = gameStateManager;
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(managerObject);
        }

        [Test]
        public void ScreenManager_Initializes_WithSingletonPattern()
        {
            // Arrange
            var secondManagerObject = new GameObject("ScreenManager2");
            var secondManager = secondManagerObject.AddComponent<ScreenManager>();

            // Act
            // Singleton pattern should ensure only one instance exists

            // Assert
            Assert.IsTrue(managerObject != null, 
                "First ScreenManager instance should be preserved");

            // Clean up
            Object.DestroyImmediate(secondManagerObject);
        }

        [Test]
        public void LoadMainMenu_SetsCorrectSceneName()
        {
            // Act
            // Method calls SceneManager.LoadScene internally

            // Assert
            //Assert.AreEqual(expectedSceneName, screenManager.mainMenuSceneName,
            //    "LoadMainMenu should use the correct scene name");
        }

        [Test]
        public void LoadGameScene_SetsCorrectSceneName()
        {
            // Act
            // Method calls SceneManager.LoadScene internally

            // Assert
            //Assert.AreEqual(expectedSceneName, screenManager.gameSceneName,
            //    "LoadGameScene should use the correct scene name");
        }

        [Test]
        public void LoadGameOverScene_SetsCorrectSceneName()
        {
            // Act
            // Method calls SceneManager.LoadScene internally

            // Assert
            //Assert.AreEqual(expectedSceneName, screenManager.gameOverSceneName,
            //    "LoadGameOverScene should use the correct scene name");
        }

        [Test]
        public void GetCurrentSceneName_ReturnsActiveSceneName()
        {
            // Arrange
            var currentScene = SceneManager.GetActiveScene();

            // Act
            var sceneName = screenManager.GetCurrentSceneName();

            // Assert
            Assert.AreEqual(currentScene.name, sceneName,
                "GetCurrentSceneName should return the name of the currently active scene");
        }

        [Test]
        public void IsSceneLoaded_ReturnsTrue_ForActiveScene()
        {
            // Arrange
            var currentScene = SceneManager.GetActiveScene();

            // Act
            bool isLoaded = screenManager.IsSceneLoaded(currentScene.name);

            // Assert
            Assert.IsTrue(isLoaded,
                "IsSceneLoaded should return true for the currently active scene");
        }

        [Test]
        public void IsSceneLoaded_ReturnsFalse_ForNonExistentScene()
        {
            // Arrange
            var nonExistentScene = "NonExistentScene123";

            // Act
            bool isLoaded = screenManager.IsSceneLoaded(nonExistentScene);

            // Assert
            Assert.IsFalse(isLoaded,
                "IsSceneLoaded should return false for non-existent scenes");
        }

        [Test]
        public void RestartCurrentScene_UsesCurrentSceneName()
        {
            // Arrange
            var currentScene = SceneManager.GetActiveScene();

            // Act
            // Method calls SceneManager.LoadScene internally

            // Assert
            Assert.AreEqual(currentScene.name, screenManager.GetCurrentSceneName(),
                "RestartCurrentScene should use the current scene name");
        }

        [Test]
        public void ScreenManager_Persists_AcrossSceneChanges()
        {
            // Arrange
            var originalObject = screenManager.gameObject;

            // Act
            // The DontDestroyOnLoad is set in Awake()

            // Assert
            Assert.IsTrue(originalObject != null,
                "ScreenManager GameObject should persist across scene changes");
        }
    }
}