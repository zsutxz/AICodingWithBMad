// VictoryBanner.cs
// UI component for displaying winner information when a player wins the game

using UnityEngine;
using UnityEngine.UI;
using Gomoku;
using Gomoku.Core;
using static Gomoku.TurnManager;
using System;

namespace Gomoku.UI
{
    /// <summary>
    /// VictoryBanner is responsible for displaying the winner information when a player wins the game.
    /// It shows a banner with the winner's name and a victory message.
    /// </summary>
    public class VictoryBanner : MonoBehaviour
    {
        // Reference to the text component that displays the winner's name
        [SerializeField] private Text winnerText;

        // Reference to the game object that contains the victory banner UI
        [SerializeField] private GameObject bannerPanel;

        // Reference to the win detector to listen for win events
        [SerializeField] private WinDetector winDetector;

        // Reference to the player who won the game
        private PlayerType winner;

        private void Awake()
        {
            // Validate references
            if (winnerText == null)
            {
                Debug.LogError("WinnerText reference not set in VictoryBanner");
            }

            if (bannerPanel == null)
            {
                Debug.LogError("BannerPanel reference not set in VictoryBanner");
            }

            if (winDetector == null)
            {
                Debug.LogError("WinDetector reference not set in VictoryBanner");
            }

            // Initially hide the banner
            HideBanner();
        }

        private void OnEnable()
        {
            //// Subscribe to win events
            //if (winDetector != null)
            //{
            //    winDetector.onWinDetected.AddListener(OnWinDetected);
            //}
        }



        private void OnDisable()
        {
            //// Unsubscribe from win events
            //if (winDetector != null)
            //{
            //    winDetector.onWinDetected.RemoveListener(OnWinDetected);
            //}
        }

        /// <summary>
        /// Called when a win is detected by the WinDetector.
        /// Displays the victory banner with the winner's information.
        /// </summary>
        /// <param name="winner">The player who won the game</param>
        private void OnWinDetected(PlayerType winner)
        {
            this.winner = winner;

            // Display the winner's name
            if (winnerText != null)
            {
                winnerText.text = $"{(winner == PlayerType.PlayerOne ? "Black" : "White")} Wins!";
            }

            // Show the victory banner
            ShowBanner();
        }

        /// <summary>
        /// Shows the victory banner UI.
        /// </summary>
        private void ShowBanner()
        {
            if (bannerPanel != null)
            {
                bannerPanel.SetActive(true);
            }
        }

        /// <summary>
        /// Hides the victory banner UI.
        /// </summary>
        private void HideBanner()
        {
            if (bannerPanel != null)
            {
                bannerPanel.SetActive(false);
            }
        }

        /// <summary>
        /// Resets the victory banner for a new game.
        /// </summary>
        public void Reset()
        {
            HideBanner();
            winner = PlayerType.None;
        }

        /// <summary>
        /// Sets the winner of the game and displays the victory banner.
        /// This method is provided for external control of the victory banner.
        /// </summary>
        /// <param name="winner">The player who won the game</param>
        public void SetWinner(PlayerType winner)
        {
            this.winner = winner;

            // Display the winner's name
            if (winnerText != null)
            {
                winnerText.text = $"{(winner == PlayerType.PlayerOne ? "Black" : "White")} Wins!";
            }

            // Show the victory banner
            ShowBanner();
        }
    }
}