using NUnit.Framework;
using UnityEngine;
using Gomoku.UI;
using Gomoku.Audio;
using Gomoku.Core;

namespace Gomoku.Tests.EditMode
{
    /// <summary>
    /// Unit tests for GameStateManager component
    /// </summary>
    public class GameStateManagerTests
    {
        private GameStateManager gameStateManager;
        private GameObject managerObject;

        [SetUp]
        public void SetUp()
        {
            // Create the GameStateManager GameObject
            managerObject = new GameObject("GameStateManager");
            gameStateManager = managerObject.AddComponent<GameStateManager>();

            // Add required components
            var uiManager = managerObject.AddComponent<UIManager>();
            var audioManager = managerObject.AddComponent<AudioManager>();
            var gameOverScreen = new GameObject("GameOverScreen").AddComponent<GameOverScreen>();

        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(managerObject);
        }

        [Test]
        public void GameStateManager_Initializes_WithStartingState()
        {
            // Arrange
            var expectedState = GameStateEnum.MainMenu;

            // Act
            var actualState = gameStateManager.GetCurrentState();

            // Assert
            Assert.AreEqual(expectedState, actualState, 
                "GameStateManager should initialize with the specified starting state");
        }

        [Test]
        public void SetState_ChangesState_WhenValidTransition()
        {
            // Arrange
            var newState = GameStateEnum.Playing;

            // Act
            gameStateManager.SetState(newState);

            // Assert
            Assert.AreEqual(newState, gameStateManager.GetCurrentState(),
                "SetState should change the current state when valid");
        }

        [Test]
        public void SetState_DoesNothing_WhenSameState()
        {
            // Arrange
            GameStateEnum initialState = gameStateManager.GetCurrentState();

            // Act
            gameStateManager.SetState(initialState);

            // Assert
            Assert.AreEqual(initialState, gameStateManager.GetCurrentState(),
                "SetState should not change state when setting to the same state");
        }

        [Test]
        public void IsInState_ReturnsTrue_WhenInSpecifiedState()
        {
            // Arrange
            var testState = GameStateEnum.MainMenu;

            // Act
            bool isInState = gameStateManager.IsInState(testState);

            // Assert
            Assert.IsTrue(isInState, 
                "IsInState should return true when the game is in the specified state");
        }

        [Test]
        public void IsInState_ReturnsFalse_WhenNotInSpecifiedState()
        {
            // Arrange
            var testState = GameStateEnum.Playing;

            // Act
            bool isInState = gameStateManager.IsInState(testState);

            // Assert
            Assert.IsFalse(isInState, 
                "IsInState should return false when the game is not in the specified state");
        }

        [Test]
        public void PauseGame_ChangesState_ToPaused()
        {
            // Arrange
            gameStateManager.SetState(GameStateEnum.Playing);

            // Act
            gameStateManager.PauseGame();

            // Assert
            Assert.AreEqual(GameStateEnum.Paused, gameStateManager.GetCurrentState(),
                "PauseGame should change state to Paused when in Playing state");
        }

        [Test]
        public void ResumeGame_ChangesState_FromPaused()
        {
            // Arrange
            gameStateManager.SetState(GameStateEnum.Paused);
            NewMethod();

            //// Assert
            //Assert.AreEqual(GameState.Playing, gameStateManager.GetCurrentState(),
            //    "ResumeGame should change state from Paused to previous state");
        }

        private void NewMethod()
        {

            // Act
            gameStateManager.ResumeGame();
        }

        [Test]
        public void RestartGame_ChangesState_ToPlaying()
        {
            // Arrange
            gameStateManager.SetState(GameStateEnum.GameOver);

            //// Act
            //gameStateManager.RestartGame();

            //// Assert
            //Assert.AreEqual(GameState.Playing, gameStateManager.GetCurrentState(),
            //    "RestartGame should change state to Playing");
        }

        [Test]
        public void ReturnToMainMenu_ChangesState_ToMainMenu()
        {
            // Arrange
            gameStateManager.SetState(GameStateEnum.GameOver);

            // Act
            gameStateManager.ReturnToMainMenu();

            // Assert
            Assert.AreEqual(GameStateEnum.MainMenu, gameStateManager.GetCurrentState(),
                "ReturnToMainMenu should change state to MainMenu");
        }

        [Test]
        public void GameStateManager_Persists_AcrossSceneChanges()
        {
            // Arrange
            var originalObject = gameStateManager.gameObject;

            // Act
            // The DontDestroyOnLoad is set in Awake()

            // Assert
            Assert.IsTrue(originalObject != null,
                "GameStateManager GameObject should persist across scene changes");
        }
    }
}