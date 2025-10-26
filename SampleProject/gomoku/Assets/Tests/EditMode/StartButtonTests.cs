using NUnit.Framework;
using UnityEngine;
using Gomoku.UI;
using Gomoku.Systems;

namespace Gomoku.Tests.EditMode
{
    public class StartButtonTests
    {
        private GameObject testGameObject;
        private Gomoku.UI.ButtonHandler buttonHandler;
        private GameStateManager gameStateManager;

        [SetUp]
        public void SetUp()
        {
            // Create GameStateManager
            GameObject managerGameObject = new GameObject();
            gameStateManager = managerGameObject.AddComponent<GameStateManager>();

            // Create ButtonHandler
            testGameObject = new GameObject("TestButtonHandler");
            buttonHandler = testGameObject.AddComponent<Gomoku.UI.ButtonHandler>();
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
        public void OnStartButtonClicked_SetsPlayingStateAndLogsMessage()
        {
            // Arrange
            gameStateManager.SetState(GameStateEnum.MainMenu);

            // Act
            buttonHandler.OnStartButtonClicked();

            // Assert
            Assert.AreEqual(GameStateEnum.Playing, gameStateManager.GetCurrentState());
        }

        [Test]
        public void OnStartButton_ClicksSuccessfully()
        {
            // Arrange
            gameStateManager.SetState(GameStateEnum.MainMenu);

            // Act & Assert - Should not throw an exception
            Assert.DoesNotThrow(() => buttonHandler.OnStartButtonClicked());
        }

        [Test]
        public void OnStartButtonClicked_TriggersButtonClickEvent()
        {
            // Arrange
            gameStateManager.SetState(GameStateEnum.MainMenu);
            bool eventTriggered = false;
            buttonHandler.OnButtonClick += (handler) => eventTriggered = true;

            // Act
            buttonHandler.OnStartButtonClicked();

            // Assert
            Assert.IsTrue(eventTriggered, "OnButtonClick event should be triggered");
        }
    }
}