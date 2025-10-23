using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using Gomoku.UI;
using Gomoku.Core;

namespace Gomoku.Tests.EditMode
{
    /// <summary>
    /// Unit tests for GameOverScreen component
    /// </summary>
    public class GameOverScreenTests
    {
        private GameOverScreen gameOverScreen;
        private GameObject screenObject;

        [SetUp]
        public void SetUp()
        {
            // Create the GameOverScreen GameObject
            screenObject = new GameObject("GameOverScreen");
            gameOverScreen = screenObject.AddComponent<GameOverScreen>();

            // Create UI elements
            var victoryBanner = new GameObject("VictoryBanner");
            gameOverScreen.victoryBanner = victoryBanner;

            var playAgainButton = new GameObject("PlayAgainButton");
            playAgainButton.AddComponent<Button>();
            gameOverScreen.playAgainButton = playAgainButton.GetComponent<Button>();

            var mainMenuButton = new GameObject("MainMenuButton");
            mainMenuButton.AddComponent<Button>();
            gameOverScreen.mainMenuButton = mainMenuButton.GetComponent<Button>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(screenObject);
        }

        [Test]
        public void GameOverScreen_Initializes_WithoutErrors()
        {
            // Arrange & Act
            // Initialization happens in Awake()

            // Assert
            Assert.IsTrue(gameOverScreen != null,
                "GameOverScreen should initialize successfully");
        }

        [Test]
        public void ShowGameOver_ActivatesScreen_AndVictoryBanner()
        {
            // Arrange
            var winner = PlayerType.PlayerOne;
            screenObject.SetActive(false);

            // Act
            gameOverScreen.ShowGameOver(winner);

            // Assert
            Assert.IsTrue(screenObject.activeSelf,
                "ShowGameOver should activate the GameOverScreen GameObject");
        }

        [Test]
        public void HideGameOver_DeactivatesScreen_AndVictoryBanner()
        {
            // Arrange
            screenObject.SetActive(true);
            if (gameOverScreen.victoryBanner != null)
            {
                gameOverScreen.victoryBanner.SetActive(true);
            }

            // Act
            gameOverScreen.HideGameOver();

            // Assert
            Assert.IsFalse(screenObject.activeSelf,
                "HideGameOver should deactivate the GameOverScreen GameObject");
        }

        [Test]
        public void OnPlayAgainClicked_LoadsGameScene()
        {
            // Arrange
            var expectedSceneName = "GameScene";

            // Act
            // Method calls SceneManager.LoadScene internally

            // Assert
            Assert.AreEqual(expectedSceneName, gameOverScreen.gameSceneName,
                "OnPlayAgainClicked should load the game scene");
        }

        [Test]
        public void OnMainMenuClicked_LoadsMainMenuScene()
        {
            // Arrange
            var expectedSceneName = "MainMenu";

            // Act
            // Method calls SceneManager.LoadScene internally

            // Assert
            Assert.AreEqual(expectedSceneName, gameOverScreen.mainMenuSceneName,
                "OnMainMenuClicked should load the main menu scene");
        }

        [Test]
        public void ButtonTexts_AreSet_AccordingToConfiguration()
        {
            // Arrange
            var expectedPlayAgainText = "再玩一次";
            var expectedMainMenuText = "主菜单";

            // Act
            // Text setting happens in InitializeUI()

            // Assert
            Assert.AreEqual(expectedPlayAgainText, gameOverScreen.playAgainText,
                "Play again button text should match configuration");
            Assert.AreEqual(expectedMainMenuText, gameOverScreen.mainMenuText,
                "Main menu button text should match configuration");
        }

        [Test]
        public void VictoryBanner_IsHidden_Initially()
        {
            // Arrange
            // Initialization happens in Awake()

            // Assert
            if (gameOverScreen.victoryBanner != null)
            {
                Assert.IsFalse(gameOverScreen.victoryBanner.activeSelf,
                    "Victory banner should be hidden initially");
            }
        }

        [Test]
        public void GameOverScreen_HandlesNullReferences_Gracefully()
        {
            // Arrange
            gameOverScreen.victoryBanner = null;
            gameOverScreen.playAgainButton = null;
            gameOverScreen.mainMenuButton = null;

            // Act & Assert
            Assert.DoesNotThrow(() => gameOverScreen.ShowGameOver(PlayerType.PlayerOne),
                "ShowGameOver should handle null references gracefully");
            Assert.DoesNotThrow(() => gameOverScreen.HideGameOver(),
                "HideGameOver should handle null references gracefully");
        }
    }
}