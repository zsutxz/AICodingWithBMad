namespace Gomoku
{
    /// <summary>
    /// Represents the direction of a line on the game board
    /// </summary>
    public enum LineDirection
    {
        /// <summary>
        /// Horizontal line (left to right)
        /// </summary>
        Horizontal,

        /// <summary>
        /// Vertical line (bottom to top)
        /// </summary>
        Vertical,

        /// <summary>
        /// Main diagonal line (bottom-left to top-right)
        /// </summary>
        Diagonal,

        /// <summary>
        /// Anti-diagonal line (top-left to bottom-right)
        /// </summary>
        AntiDiagonal
    }
}