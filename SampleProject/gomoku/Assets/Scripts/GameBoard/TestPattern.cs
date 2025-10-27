namespace Gomoku
{
    /// <summary>
    /// Represents different test patterns for board setup
    /// </summary>
    public enum TestPattern
    {
        /// <summary>
        /// Basic pattern with a few pieces around the center
        /// </summary>
        Basic,

        /// <summary>
        /// Pattern with horizontal and vertical lines for testing winning conditions
        /// </summary>
        LineTest,

        /// <summary>
        /// Pattern with diagonal lines for testing diagonal winning conditions
        /// </summary>
        DiagonalTest,

        /// <summary>
        /// Random placement of pieces for general testing
        /// </summary>
        Random,

        /// <summary>
        /// Pattern with near-winning lines (4 in a row) for testing game end conditions
        /// </summary>
        FullLine
    }
}