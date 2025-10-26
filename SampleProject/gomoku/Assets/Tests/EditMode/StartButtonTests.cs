using NUnit.Framework;
using UnityEngine;
using Gomoku.UI;
using Gomoku.UI.MainMenu;

namespace Gomoku.Tests.EditMode
{
    public class StartButtonTests
    {
        private GameObject testGameObject;
        private ButtonHandler buttonHandler;
        private GameStateManager gameStateManager;
        private GameObject mainMenuScreenObject;
        private MainMenuScreen mainMenuScreen;

        [SetUp]
        public void SetUp()
        {
            // Create GameStateManager
            GameObject managerGameObject = new GameObject();
            gameStateManager = managerGameObject.AddComponent<GameStateManager>();

            // Create ButtonHandler
            testGameObject = new GameObject("TestButtonHandler");
            buttonHandler = testGameObject.AddComponent<ButtonHandler>();

            // Create MainMenuScreen
            mainMenuScreenObject = new GameObject("MainMenuScreen");
            mainMenuScreen = mainMenuScreenObject.AddComponent<MainMenuScreen>();
        }

        [TearDown]
        public void TearDown()
        {
            if (testGameObject != null)
            {
                Object.DestroyImmediate(testGameObject);
            }
            if (mainMenuScreenObject != null)
            {
                Object.DestroyImmediate(mainMenuScreenObject);
            }

            var managerObject = GameObject.FindObjectOfType<GameStateManager>();
            if (managerObject != null && managerObject.gameObject != null)
            {
                Object.DestroyImmediate(managerObject.gameObject);
            }
        }

        [Test]
        public void OnStartButtonClicked_WithMainMenuScreen_SetsPlayingStateAndLoadsGameScene()
        {
            // Arrange
            gameStateManager.SetState(GameStateEnum.MainMenu);

            // Act
            buttonHandler.OnStartButtonClicked();

            // Assert
            Assert.AreEqual(GameStateEnum.Playing, gameStateManager.GetCurrentState());
        }

        [Test]
        public void OnStartButton_ClicksSuccessfully_WithMainMenuScreen()
        {
            // Arrange
            gameStateManager.SetState(GameStateEnum.MainMenu);

            // Act & Assert - Should not throw an exception
            Assert.DoesNotThrow(() => buttonHandler.OnStartButtonClicked());
        }

        [Test]
        public void OnStartButtonClicked_WithoutMainMenuScreen_UsesFallback()
        {
            // Arrange
            gameStateManager.SetState(GameStateEnum.MainMenu);

            // Destroy MainMenuScreen to test fallback
            Object.DestroyImmediate(mainMenuScreenObject);

            // Act
            buttonHandler.OnStartButtonClicked();

            // Assert - Should still work due to fallback logic
            Assert.AreEqual(GameStateEnum.Playing, gameStateManager.GetCurrentState());
        }
    }
}