using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using Gomoku.Core;

namespace Gomoku.Tests.EditMode
{
    public class InputHandlerTests
    {
        private GameObject testGameObject;
        private InputHandler inputHandler;
        private GameStateManager gameStateManager;
        
        [SetUp]
        public void SetUp()
        {
            // Create GameStateManager first
            GameObject managerGameObject = new GameObject();
            gameStateManager = managerGameObject.AddComponent<GameStateManager>();
            
            // Create InputHandler
            testGameObject = new GameObject();
            inputHandler = testGameObject.AddComponent<InputHandler>();
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
        public void InputHandler_PausesGameWhenESCPressedInPlayingState()
        {
            // Arrange
            gameStateManager.SetState(GameStateEnum.Playing);
            
            // Act
            inputHandler.HandlePauseInput();
            
            // Assert
            Assert.AreEqual(GameStateEnum.Paused, gameStateManager.GetCurrentState());
            Assert.AreEqual(0f, Time.timeScale);
        }
        
        [Test]
        public void InputHandler_ResumesGameWhenESCPressedInPausedState()
        {
            // Arrange
            gameStateManager.SetState(GameStateEnum.Paused);
            Time.timeScale = 0f;
            
            // Act
            inputHandler.HandlePauseInput();
            
            // Assert
            Assert.AreEqual(GameStateEnum.Playing, gameStateManager.GetCurrentState());
            Assert.AreEqual(1f, Time.timeScale);
        }
        
        [Test]
        public void InputHandler_DoesNothingWhenESCPressedInMainMenuState()
        {
            // Arrange
            gameStateManager.SetState(GameStateEnum.MainMenu);
            Time.timeScale = 1f;
            
            // Act
            inputHandler.HandlePauseInput();
            
            // Assert
            Assert.AreEqual(GameStateEnum.MainMenu, gameStateManager.GetCurrentState());
            Assert.AreEqual(1f, Time.timeScale);
        }
        
        [Test]
        public void InputHandler_DoesNothingWhenESCPressedInGameOverState()
        {
            // Arrange
            gameStateManager.SetState(GameStateEnum.GameOver);
            Time.timeScale = 1f;
            
            // Act
            inputHandler.HandlePauseInput();
            
            // Assert
            Assert.AreEqual(GameStateEnum.GameOver, gameStateManager.GetCurrentState());
            Assert.AreEqual(1f, Time.timeScale);
        }
    }
}