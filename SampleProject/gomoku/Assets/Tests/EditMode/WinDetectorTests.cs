// WinDetectorTests.cs
// Unit tests for the WinDetector class
// Tests horizontal, vertical, and diagonal win detection

using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Gomoku;

/// <summary>
/// WinDetectorTests contains unit tests for the WinDetector class.
/// It tests win detection in horizontal, vertical, and diagonal directions.
/// </summary>
public class WinDetectorTests
{
    public WinDetector winDetector;
    public GameStateManager gameStateManager;
    public WinCondition winCondition;
    public PiecePlacement piecePlacement;
    public GameBoardController gameBoard;


    [SetUp]
    public void SetUp()
    {
        // Create a GameObject to attach the components to
        GameObject testGameObject = new GameObject("TestWinDetector");

        // Add the required components
        winDetector = testGameObject.AddComponent<WinDetector>();
        gameStateManager = testGameObject.AddComponent<GameStateManager>();
        gameBoard = testGameObject.AddComponent<GameBoardController>();
        piecePlacement = testGameObject.AddComponent<PiecePlacement>();

        //// Set up references between components
        //piecePlacement.gameBoard = gameBoard;

        //// Set up references in WinDetector
        //winDetector.gameStateManager = gameStateManager;
        //winDetector.piecePlacement = piecePlacement;

        // Create and set up the WinCondition
        winCondition = ScriptableObject.CreateInstance<WinCondition>();
        //winDetector.winCondition = winCondition;

        // Initialize the game systems
        piecePlacement.InitializeGame();
    }
    
    [TearDown]
    public void TearDown()
    {
        // Clean up
        Object.DestroyImmediate(winDetector.gameObject);
        ScriptableObject.DestroyImmediate(winCondition);
    }
    
    [Test]
    public void CheckHorizontalWin_DetectsFiveInARow_Horizontal()
    {
        // Arrange
        // Set up a horizontal win for Black player
        // Directly manipulate the board state
        for (int x = 0; x < 5; x++)
        {
            piecePlacement.BoardState[x, 0] = PlayerType.Black;
        }

        // Act
        bool winDetected = winDetector.CheckForWin(new Vector2Int(4, 0));

        // Assert
        Assert.IsTrue(winDetected);
        Assert.AreEqual(PlayerType.Black, winDetector.GetWinner());
        Assert.AreEqual(GameState.GameOver, gameStateManager.GetCurrentState());
    }
    
    [Test]
    public void CheckVerticalWin_DetectsFiveInARow_Vertical()
    {
        // Arrange
        // Set up a vertical win for White player
        // Directly manipulate the board state
        for (int y = 0; y < 5; y++)
        {
            piecePlacement.BoardState[0, y] = PlayerType.White;
        }

        // Act
        bool winDetected = winDetector.CheckForWin(new Vector2Int(0, 4));

        // Assert
        Assert.IsTrue(winDetected);
        Assert.AreEqual(PlayerType.White, winDetector.GetWinner());
        Assert.AreEqual(GameState.GameOver, gameStateManager.GetCurrentState());
    }
    
    [Test]
    public void CheckDiagonalWin_DetectsFiveInARow_Diagonal()
    {
        // Arrange
        // Set up a diagonal win (top-left to bottom-right) for Black player
        // Directly manipulate the board state
        for (int i = 0; i < 5; i++)
        {
            piecePlacement.BoardState[i, i] = PlayerType.Black;
        }

        // Act
        bool winDetected = winDetector.CheckForWin(new Vector2Int(4, 4));

        // Assert
        Assert.IsTrue(winDetected);
        Assert.AreEqual(PlayerType.Black, winDetector.GetWinner());
        Assert.AreEqual(GameState.GameOver, gameStateManager.GetCurrentState());
    }
    
    [Test]
    public void CheckAntiDiagonalWin_DetectsFiveInARow_AntiDiagonal()
    {
        // Arrange
        // Set up a diagonal win (top-right to bottom-left) for White player
        // Directly manipulate the board state
        for (int i = 0; i < 5; i++)
        {
            piecePlacement.BoardState[4 - i, i] = PlayerType.White;
        }

        // Act
        bool winDetected = winDetector.CheckForWin(new Vector2Int(0, 4));

        // Assert
        Assert.IsTrue(winDetected);
        Assert.AreEqual(PlayerType.White, winDetector.GetWinner());
        Assert.AreEqual(GameState.GameOver, gameStateManager.GetCurrentState());
    }
    
    [Test]
    public void CheckForWin_NoWin_ReturnsFalse()
    {
        // Arrange
        // Set up a board with no winning pattern
        piecePlacement.BoardState[0, 0] = PlayerType.Black;
        piecePlacement.BoardState[1, 0] = PlayerType.White;
        piecePlacement.BoardState[0, 1] = PlayerType.White;
        piecePlacement.BoardState[1, 1] = PlayerType.Black;

        // Act
        bool winDetected = winDetector.CheckForWin(new Vector2Int(1, 1));

        // Assert
        Assert.IsFalse(winDetected);
        Assert.AreEqual(GameState.Playing, gameStateManager.GetCurrentState());
    }
    
    [Test]
    public void CheckForWin_WinAfterMultipleMoves_DetectsWin()
    {
        //// Arrange
        //// Set up a winning pattern with moves in non-sequential order
        //gameBoardModel.SetCellState(new Vector2Int(0, 0), Player.Player1);
        //gameBoardModel.SetCellState(new Vector2Int(1, 0), Player.Player1);
        //gameBoardModel.SetCellState(new Vector2Int(2, 0), Player.Player1);
        //gameBoardModel.SetCellState(new Vector2Int(3, 0), Player.Player1);
        //// Leave position (4,0) empty for now
        
        //// Make some other moves
        //gameBoardModel.SetCellState(new Vector2Int(0, 1), Player.Player2);
        //gameBoardModel.SetCellState(new Vector2Int(1, 1), Player.Player2);
        
        //// Complete the winning move
        //gameBoardModel.SetCellState(new Vector2Int(4, 0), Player.Player1);
        
        // Act
        bool winDetected = winDetector.CheckForWin(new Vector2Int(4, 0));
        
        // Assert
        Assert.IsTrue(winDetected);
        //Assert.AreEqual(Player.Player1, winDetector.GetWinner());
        Assert.AreEqual(GameState.GameOver, gameStateManager.GetCurrentState());
    }
    
    [Test]
    public void CheckForWin_DifferentWinConditions_WorksWithCustomPiecesToWin()
    {
        // Arrange
        // Change the win condition to require only 3 pieces to win
        //winCondition.piecesToWin = 3;

        // Set up a horizontal win with 3 pieces for Black player
        // Directly manipulate the board state
        for (int x = 0; x < 3; x++)
        {
            piecePlacement.BoardState[x, 0] = PlayerType.Black;
        }

        // Act
        bool winDetected = winDetector.CheckForWin(new Vector2Int(2, 0));

        // Assert
        Assert.IsTrue(winDetected);
        Assert.AreEqual(PlayerType.Black, winDetector.GetWinner());
        Assert.AreEqual(GameState.GameOver, gameStateManager.GetCurrentState());
    }
    
    [Test]
    public void CheckForWin_DiagonalWin_DetectsWinInMiddleOfBoard()
    {
        //// Arrange
        //// Set up a diagonal win in the middle of the board
        //for (int i = 0; i < 5; i++)
        //{
        //    gameBoardModel.SetCellState(new Vector2Int(5 + i, 5 + i), Player.Player1);
        //}
        
        // Act
        bool winDetected = winDetector.CheckForWin(new Vector2Int(9, 9));
        
        // Assert
        Assert.IsTrue(winDetected);
        //Assert.AreEqual(Player.Player1, winDetector.GetWinner());
        Assert.AreEqual(GameState.GameOver, gameStateManager.GetCurrentState());
    }
}