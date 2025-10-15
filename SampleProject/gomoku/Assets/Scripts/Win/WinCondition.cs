using UnityEngine;

/// <summary>
/// WinCondition is a ScriptableObject that defines the rules for winning in Gomoku.
/// It specifies how many pieces are needed to win and which directions to check.
/// </summary>
[CreateAssetMenu(fileName = "WinCondition", menuName = "Gomoku/Win Condition", order = 1)]
public class WinCondition : ScriptableObject
{
    [Header("Win Conditions")]
    [SerializeField] private int piecesToWin = 5;
    [SerializeField] private bool checkHorizontal = true;
    [SerializeField] private bool checkVertical = true;
    [SerializeField] private bool checkDiagonal = true;
    [SerializeField] private bool checkAntiDiagonal = true;
    
    [Header("Display")]
    [SerializeField] private string conditionName = "Standard Gomoku";
    
    /// <summary>
    /// Gets the number of pieces needed to win.
    /// </summary>
    /// <returns>The number of consecutive pieces needed to win</returns>
    public int GetPiecesToWin()
    {
        return piecesToWin;
    }
    
    /// <summary>
    /// Checks if horizontal wins should be detected.
    /// </summary>
    /// <returns>True if horizontal wins should be checked</returns>
    public bool ShouldCheckHorizontal()
    {
        return checkHorizontal;
    }
    
    /// <summary>
    /// Checks if vertical wins should be detected.
    /// </summary>
    /// <returns>True if vertical wins should be checked</returns>
    public bool ShouldCheckVertical()
    {
        return checkVertical;
    }
    
    /// <summary>
    /// Checks if diagonal wins should be detected.
    /// </summary>
    /// <returns>True if diagonal wins should be checked</returns>
    public bool ShouldCheckDiagonal()
    {
        return checkDiagonal;
    }
    
    /// <summary>
    /// Checks if anti-diagonal wins should be detected.
    /// </summary>
    /// <returns>True if anti-diagonal wins should be checked</returns>
    public bool ShouldCheckAntiDiagonal()
    {
        return checkAntiDiagonal;
    }
    
    /// <summary>
    /// Gets the name of this win condition.
    /// </summary>
    /// <returns>The display name of the win condition</returns>
    public string GetConditionName()
    {
        return conditionName;
    }
}