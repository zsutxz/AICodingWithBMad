using NUnit.Framework;
using UnityEngine;
using Gomoku.UI;
using Gomoku.Systems;

namespace Gomoku.Tests.EditMode
{
    public class ButtonHandlerTests
    {
        private GameObject testGameObject;
        private ButtonHandler buttonHandler;
        private GameStateManager gameStateManager;
        
        [SetUp]
        public void SetUp()
        {
            // Create GameStateManager first
            GameObject managerGameObject = new GameObject();
            gameStateManager = managerGameObject.AddComponent<GameStateManager>();
            
            // Create ButtonHandler
            testGameObject = new GameObject();
            buttonHandler = testGameObject.AddComponent<ButtonHandler>();
        }
        
        [TearDown]
        public void TearDown()
        {
            if (testGameObject != null)
            {
                Object.DestroyImmediate(testGameObject);
            }
            
            var managerObject = GameObject.FindObjectOfType<GameStateManager>();
            if (managerObject != null && managerObject.gameObject != null)
            {
                Object.DestroyImmediate(managerObject.gameObject);
            }
        }
        
        [Test]
        public void OnPauseButtonClicked_PausesGameInPlayingState()
        {
            // Arrange
            gameStateManager.SetState(GameStateEnum.Playing);
            
            // Act
            buttonHandler.OnPauseButtonClicked();
            
            // Assert
            Assert.AreEqual(GameStateEnum.Paused, gameStateManager.GetCurrentState());
            Assert.AreEqual(0f, Time.timeScale);
        }
        
        [Test]
        public void OnPauseButtonClicked_DoesNothingWhenNotPlaying()
        {
            // Arrange
            gameStateManager.SetState(GameStateEnum.MainMenu);
            
            // Act
            buttonHandler.OnPauseButtonClicked();
            
            // Assert
            Assert.AreEqual(GameStateEnum.MainMenu, gameStateManager.GetCurrentState());
        }
        
        [Test]
        public void OnResumeButtonClicked_ResumesGameInPausedState()
        {
            // Arrange
            gameStateManager.SetState(GameStateEnum.Paused);
            Time.timeScale = 0f;
            
            // Act
            buttonHandler.OnResumeButtonClicked();
            
            // Assert
            Assert.AreEqual(GameStateEnum.Playing, gameStateManager.GetCurrentState());
            Assert.AreEqual(1f, Time.timeScale);
        }
        
        [Test]
        public void OnResumeButtonClicked_DoesNothingWhenNotPaused()
        {
            // Arrange
            gameStateManager.SetState(GameStateEnum.Playing);
            
            // Act
            buttonHandler.OnResumeButtonClicked();
            
            // Assert
            Assert.AreEqual(GameStateEnum.Playing, gameStateManager.GetCurrentState());
        }
        
        [Test]
        public void OnQuitButtonClicked_ReturnsToMainMenu()
        {
            // Arrange
            gameStateManager.SetState(GameStateEnum.Paused);
            Time.timeScale = 0f;
            
            // Act
            buttonHandler.OnQuitButtonClicked();
            
            // Assert
            Assert.AreEqual(GameStateEnum.MainMenu, gameStateManager.GetCurrentState());
            Assert.AreEqual(1f, Time.timeScale);
        }
        
        [Test]
        public void OnStartButtonClicked_LogsMessage()
        {
            // Arrange - This test just verifies the method doesn't crash
            gameStateManager.SetState(GameStateEnum.MainMenu);
            
            // Act & Assert - Should not throw an exception
            Assert.DoesNotThrow(() => buttonHandler.OnStartButtonClicked());
        }
    }
}