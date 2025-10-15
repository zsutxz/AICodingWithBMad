// WinDetectorIntegrationTests.cs
// Play Mode integration tests for the WinDetector class
// Tests integration between WinDetector, VictoryBanner, and game state management

using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.UI;
using Gomoku;
using Gomoku.UI;

/// <summary>
/// WinDetectorIntegrationTests contains play mode integration tests for the WinDetector class.
/// It tests the integration between WinDetector, VictoryBanner, and game state management.
/// </summary>
public class WinDetectorIntegrationTests
{
    private GameObject testGameObject;
    private WinDetector winDetector;
    private GameStateManager gameStateManager;
    private VictoryBanner victoryBanner;
    private Text winnerText;
    private GameObject bannerPanel;
    private WinCondition winCondition;
    private PiecePlacement piecePlacement;
    private GameBoardController gameBoard;
    
    [UnitySetUp]
    public IEnumerator SetUp()
    {
        // Create a test GameObject
        testGameObject = new GameObject("TestWinDetectorIntegration");

        // Add required components
        winDetector = testGameObject.AddComponent<WinDetector>();
        gameStateManager = testGameObject.AddComponent<GameStateManager>();
        gameBoard = testGameObject.AddComponent<GameBoardController>();
        piecePlacement = testGameObject.AddComponent<PiecePlacement>();

        // Set up references between components
        //piecePlacement.gameBoard = gameBoard;

        // Create and set up the WinCondition
        winCondition = ScriptableObject.CreateInstance<WinCondition>();
        //winDetector.winCondition = winCondition;
        //winDetector.piecePlacement = piecePlacement;
        //winDetector.gameStateManager = gameStateManager;

        // Create VictoryBanner and its dependencies
        GameObject bannerGO = new GameObject("VictoryBanner");
        victoryBanner = bannerGO.AddComponent<VictoryBanner>();

        // Create UI elements
        bannerPanel = new GameObject("BannerPanel");
        bannerPanel.SetActive(false);
        bannerPanel.transform.SetParent(bannerGO.transform);

        GameObject textGO = new GameObject("WinnerText");
        winnerText = textGO.AddComponent<Text>();
        textGO.transform.SetParent(bannerPanel.transform);

        // Set up references
        //victoryBanner.winnerText = winnerText;
        //victoryBanner.bannerPanel = bannerPanel;
        //victoryBanner.winDetector = winDetector;

        // Initialize the game systems
        piecePlacement.InitializeGame();

        // Ensure everything is set up before tests
        yield return null;
    }
    
    [UnityTearDown]
    public IEnumerator TearDown()
    {
        // Clean up
        Object.DestroyImmediate(testGameObject);
        Object.DestroyImmediate(victoryBanner.gameObject);
        ScriptableObject.DestroyImmediate(winCondition);
        
        yield return null;
    }
    
    [UnityTest]
    public IEnumerator CheckForWin_WinDetected_VictoryBannerShowsWinner()
    {
        // Ensure banner is initially hidden
        Assert.IsFalse(bannerPanel.activeSelf);

        // Act
        bool winDetected = winDetector.CheckForWin(new Vector2Int(4, 0));

        // Assert
        Assert.IsTrue(winDetected);
        Assert.AreEqual(PlayerType.Black, winDetector.GetWinner());
        Assert.AreEqual(GameState.GameOver, gameStateManager.GetCurrentState());
        Assert.IsTrue(bannerPanel.activeSelf);
        Assert.AreEqual("Black Wins!", winnerText.text);
        
        yield return null;
    }
    
    [UnityTest]
    public IEnumerator CheckForWin_NoWin_VictoryBannerStaysHidden()
    {
        // Arrange
        // Set up a board with no winning pattern
        piecePlacement.BoardState[0, 0] = PlayerType.Black;
        piecePlacement.BoardState[1, 0] = PlayerType.White;

        // Ensure banner is initially hidden
        Assert.IsFalse(bannerPanel.activeSelf);

        // Act
        bool winDetected = winDetector.CheckForWin(new Vector2Int(1, 0));

        // Assert
        Assert.IsFalse(winDetected);
        Assert.IsFalse(bannerPanel.activeSelf);
        Assert.AreEqual(GameState.Playing, gameStateManager.GetCurrentState());
        
        yield return null;
    }
    
    [UnityTest]
    public IEnumerator CheckForWin_VerticalWin_VictoryBannerShowsCorrectWinner()
    {
        // Arrange
        // Set up a vertical win for White player
        // Directly manipulate the board state
        for (int y = 0; y < 5; y++)
        {
            piecePlacement.BoardState[0, y] = PlayerType.White;
        }

        // Ensure banner is initially hidden
        Assert.IsFalse(bannerPanel.activeSelf);

        // Act
        bool winDetected = winDetector.CheckForWin(new Vector2Int(0, 4));

        // Assert
        Assert.IsTrue(winDetected);
        Assert.AreEqual(PlayerType.White, winDetector.GetWinner());
        Assert.AreEqual(GameState.GameOver, gameStateManager.GetCurrentState());
        Assert.IsTrue(bannerPanel.activeSelf);
        Assert.AreEqual("White Wins!", winnerText.text);
        
        yield return null;
    }
}