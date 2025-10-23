// WinDetector.cs
// Implements win detection logic for Gomoku game
// Detects five-in-a-row connections in horizontal, vertical, and diagonal directions

using UnityEngine;
using UnityEngine.Events;
using Gomoku;
using Gomoku.Core;
using System;

/// <summary>
/// WinDetector is responsible for detecting when a player has won by connecting five pieces in a row.
/// It checks for winning patterns in all directions (horizontal, vertical, diagonal) after each move.
/// </summary>
public class WinDetector : MonoBehaviour
{
    // Reference to the win condition ScriptableObject that defines the rules for winning
    [SerializeField] private WinCondition winCondition;
    // Reference to the piece placement system that contains the current state of the board
    [SerializeField] private PiecePlacement piecePlacement;
    
    // Reference to the game state manager to trigger state transitions when a win is detected
    [SerializeField] private GameStateManager gameStateManager;
    
    // Unity Event that is triggered when a win is detected, passing the winning player
    [SerializeField] public UnityEvent<PlayerType> onWinDetected;
    
    // The player who won the game (None if no winner yet)
    private PlayerType winner;
    
    // Flag to indicate if the game has been won
    private bool gameWon = false;
    
    private void Awake()
    {
        // Validate references
        if (piecePlacement == null)
        {
            Debug.LogError("PiecePlacement reference not set in WinDetector");
        }
        
        if (gameStateManager == null)
        {
            Debug.LogError("GameStateManager reference not set in WinDetector");
        }
        
        if (onWinDetected == null)
        {
            Debug.LogWarning("onWinDetected UnityEvent not set in WinDetector");
        }
        
        // Load WinCondition from Resources if not assigned
        if (winCondition == null)
        {
            winCondition = Resources.Load<WinCondition>("WinCondition");
            if (winCondition == null)
            {
                Debug.LogError("WinCondition not found in Resources folder. Please create a WinCondition asset.");
            }
        }
    }
    
    /// <summary>
    /// Checks for a winning condition on the board after a move has been made.
    /// This method should be called after each move to detect if a player has won.
    /// </summary>
    /// <param name="lastMovePosition">The position where the last piece was placed</param>
    /// <returns>True if a win condition was detected, false otherwise</returns>
    public bool CheckForWin(Vector2Int lastMovePosition)
    {
        // Don't check for win if the game has already been won
        if (gameWon)
        {
            return false;
        }
        
        // Get the player who made the last move
        PlayerType lastPlayer = piecePlacement.GetPieceAt(lastMovePosition.x, lastMovePosition.y);
        
        // Don't check for win if WinCondition is not set
        if (winCondition == null)
        {
            Debug.LogError("WinCondition is not set in WinDetector");
            return false;
        }
        
        // Validate references
        if (piecePlacement == null)
        {
            Debug.LogError("PiecePlacement reference not set in WinDetector");
            return false;
        }
        
        // Check for win in all directions based on WinCondition settings
        bool winDetected = false;
        
        if (winCondition.ShouldCheckHorizontal())
        {
            winDetected = winDetected || CheckHorizontalWin(lastMovePosition, lastPlayer);
        }
        
        if (!winDetected && winCondition.ShouldCheckVertical())
        {
            winDetected = winDetected || CheckVerticalWin(lastMovePosition, lastPlayer);
        }
        
        if (!winDetected && winCondition.ShouldCheckDiagonal())
        {
            winDetected = winDetected || CheckDiagonalWin(lastMovePosition, lastPlayer);
        }
        
        if (!winDetected && winCondition.ShouldCheckAntiDiagonal())
        {
            winDetected = winDetected || CheckAntiDiagonalWin(lastMovePosition, lastPlayer);
        }
        
        if (winDetected)
        {
            winner = lastPlayer;
            gameWon = true;
            
            // Trigger the win event
            onWinDetected?.Invoke(winner);
            
            // Transition to game over state
            gameStateManager.SetState(GameStateEnum.GameOver);
            
            return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// Checks for a horizontal win (5 pieces in a row) containing the last move position.
    /// Optimized to only check around the last move position.
    /// </summary>
    /// <param name="position">The position of the last move</param>
    /// <param name="player">The player who made the last move</param>
    /// <returns>True if a horizontal win is detected</returns>
    private bool CheckHorizontalWin(Vector2Int position, PlayerType player)
    {
        // Get the row of the last move
        int row = position.y;
        
        // Get the number of pieces needed to win from WinCondition
        int piecesNeeded = winCondition.GetPiecesToWin();
        
        // Start from the leftmost position that could form a winning line with the current position
        int startCol = Mathf.Max(0, position.x - piecesNeeded + 1);
        
        // End at the rightmost position that could form a winning line with the current position
        int endCol = Mathf.Min(14 - piecesNeeded + 1, position.x);
        
        // Check each possible starting position for a horizontal winning line
        for (int col = startCol; col <= endCol; col++)
        {
            // Check if there are enough consecutive pieces of the same player starting at (col, row)
            bool winFound = true;
            for (int i = 0; i < piecesNeeded; i++)
            {
                if (piecePlacement.GetPieceAt(col + i, row) != player)
                {
                    winFound = false;
                    break;
                }
            }
            
            if (winFound)
            {
                return true;
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// Checks for a vertical win (5 pieces in a row) containing the last move position.
    /// Optimized to only check around the last move position.
    /// </summary>
    /// <param name="position">The position of the last move</param>
    /// <param name="player">The player who made the last move</param>
    /// <returns>True if a vertical win is detected</returns>
    private bool CheckVerticalWin(Vector2Int position, PlayerType player)
    {
        // Get the column of the last move
        int col = position.x;
        
        // Get the number of pieces needed to win from WinCondition
        int piecesNeeded = winCondition.GetPiecesToWin();
        
        // Start from the topmost position that could form a winning line with the current position
        int startRow = Mathf.Max(0, position.y - piecesNeeded + 1);
        
        // End at the bottommost position that could form a winning line with the current position
        int endRow = Mathf.Min(14 - piecesNeeded + 1, position.y);
        
        // Check each possible starting position for a vertical winning line
        for (int row = startRow; row <= endRow; row++)
        {
            // Check if there are enough consecutive pieces of the same player starting at (col, row)
            bool winFound = true;
            for (int i = 0; i < piecesNeeded; i++)
            {
                if (piecePlacement.GetPieceAt(col, row + i) != player)
                {
                    winFound = false;
                    break;
                }
            }
            
            if (winFound)
            {
                return true;
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// Checks for a diagonal win (top-left to bottom-right) containing the last move position.
    /// Optimized to only check around the last move position.
    /// </summary>
    /// <param name="position">The position of the last move</param>
    /// <param name="player">The player who made the last move</param>
    /// <returns>True if a diagonal win is detected</returns>
    private bool CheckDiagonalWin(Vector2Int position, PlayerType player)
    {
        // Get the number of pieces needed to win from WinCondition
        int piecesNeeded = winCondition.GetPiecesToWin();
        
        // Calculate the offset needed to go from the current position to the start of a potential winning line
        int offset = Mathf.Min(position.x, position.y);
        
        // Start from the leftmost-topmost position that could form a winning line with the current position
        int startOffset = Mathf.Max(0, offset - piecesNeeded + 1);
        
        // End at the rightmost-bottommost position that could form a winning line with the current position
        int endOffset = Mathf.Min(offset, 14 - Mathf.Max(position.x, position.y) - piecesNeeded + 1);
        
        // Adjust start and end positions based on the constraints
        Vector2Int startPos = new Vector2Int(position.x - offset, position.y - offset);
        
        // Check each possible starting position for a diagonal winning line
        for (int i = startOffset; i <= endOffset; i++)
        {
            // Check if there are enough consecutive pieces of the same player starting at (startPos.x + i, startPos.y + i)
            bool winFound = true;
            for (int j = 0; j < piecesNeeded; j++)
            {
                Vector2Int checkPos = new Vector2Int(startPos.x + i + j, startPos.y + i + j);
                if (piecePlacement.GetPieceAt(checkPos.x, checkPos.y) != player)
                {
                    winFound = false;
                    break;
                }
            }
            
            if (winFound)
            {
                return true;
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// Checks for an anti-diagonal win (top-right to bottom-left) containing the last move position.
    /// Optimized to only check around the last move position.
    /// </summary>
    /// <param name="position">The position of the last move</param>
    /// <param name="player">The player who made the last move</param>
    /// <returns>True if an anti-diagonal win is detected</returns>
    private bool CheckAntiDiagonalWin(Vector2Int position, PlayerType player)
    {
        // Get the number of pieces needed to win from WinCondition
        int piecesNeeded = winCondition.GetPiecesToWin();
        
        // Calculate the offset needed to go from the current position to the start of a potential winning line
        int offset = Mathf.Min(14 - position.x, position.y);
        
        // Start from the rightmost-topmost position that could form a winning line with the current position
        int startOffset = Mathf.Max(0, offset - piecesNeeded + 1);
        
        // End at the leftmost-bottommost position that could form a winning line with the current position
        int endOffset = Mathf.Min(offset, 14 - (14 - position.x + position.y) - piecesNeeded + 1);
        
        // Adjust start and end positions based on the constraints
        Vector2Int startPos = new Vector2Int(position.x + offset, position.y - offset);
        
        // Check each possible starting position for an anti-diagonal winning line
        for (int i = startOffset; i <= endOffset; i++)
        {
            // Check if there are enough consecutive pieces of the same player starting at (startPos.x - i, startPos.y + i)
            bool winFound = true;
            for (int j = 0; j < piecesNeeded; j++)
            {
                Vector2Int checkPos = new Vector2Int(startPos.x - i - j, startPos.y + i + j);
                if (piecePlacement.GetPieceAt(checkPos.x, checkPos.y) != player)
                {
                    winFound = false;
                    break;
                }
            }
            
            if (winFound)
            {
                return true;
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// Gets the winner of the game.
    /// </summary>
    /// <returns>The winning player, or None if no winner yet</returns>
    public PlayerType GetWinner()
    {
        return winner;
    }
    
    /// <summary>
    /// Checks if the game has been won.
    /// </summary>
    /// <returns>True if the game has been won, false otherwise</returns>
    public bool IsGameWon()
    {
        return gameWon;
    }
    
    /// <summary>
    /// Resets the win detector for a new game.
    /// </summary>
    public void Reset()
    {
        winner = PlayerType.None;
        gameWon = false;
    }

    /// <summary>
    /// Checks if there is a winner in the current game state.
    /// </summary>
    /// <returns>True if there is a winner, false otherwise</returns>
    internal bool HasWinner()
    {
        return gameWon && winner != PlayerType.None;
    }
}

