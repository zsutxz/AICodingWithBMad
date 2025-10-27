using UnityEngine;
using Gomoku.Core;

namespace Gomoku
{
    /// <summary>
    /// Represents data for placing a piece on the board
    /// </summary>
    [System.Serializable]
    public struct PieceData
    {
        [Tooltip("X coordinate on the board")]
        public int X;

        [Tooltip("Y coordinate on the board")]
        public int Y;

        [Tooltip("Type of player piece to place")]
        public PlayerType PlayerType;

        /// <summary>
        /// Creates a new piece data
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="playerType">Player type</param>
        public PieceData(int x, int y, PlayerType playerType)
        {
            X = x;
            Y = y;
            PlayerType = playerType;
        }

        /// <summary>
        /// Returns a string representation of the piece data
        /// </summary>
        /// <returns>String representation</returns>
        public override string ToString()
        {
            return $"Piece({X}, {Y}, {PlayerType})";
        }
    }
}