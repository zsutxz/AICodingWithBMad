using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Gomoku;
using Gomoku.Core;

namespace Gomoku.UI
{
    /// <summary>
    /// UI component for displaying game results and navigation options
    /// </summary>
    public class GameOverScreen : MonoBehaviour
    {
        [Header("UI References")]
        [Tooltip("Reference to the victory banner to display winner information")]
        [SerializeField] public GameObject victoryBanner;

        [Tooltip("Reference to the play again button")]
        [SerializeField] public Button playAgainButton;

        [Tooltip("Reference to the main menu button")]
        [SerializeField] public Button mainMenuButton;

        [Header("Text Settings")]
        [Tooltip("Text to display on the play again button")]
        [SerializeField] public string playAgainText = "再玩一次";

        [Tooltip("Text to display on the main menu button")]
        [SerializeField] public string mainMenuText = "主菜单";

        [Header("Scene Management")]
        [Tooltip("Name of the game scene to load when playing again")]
        [SerializeField] public string gameSceneName = "GameScene";

        [Tooltip("Name of the main menu scene to load when returning to main menu")]
        [SerializeField] public string mainMenuSceneName = "MainMenu";

        private void Awake()
        {
            InitializeUI();
        }

        private void InitializeUI()
        {
            // Set up button text
            if (playAgainButton != null && !string.IsNullOrEmpty(playAgainText))
            {
                Text playAgainButtonText = playAgainButton.GetComponentInChildren<Text>();
                if (playAgainButtonText != null)
                {
                    playAgainButtonText.text = playAgainText;
                }
            }

            if (mainMenuButton != null && !string.IsNullOrEmpty(mainMenuText))
            {
                Text mainMenuButtonText = mainMenuButton.GetComponentInChildren<Text>();
                if (mainMenuButtonText != null)
                {
                    mainMenuButtonText.text = mainMenuText;
                }
            }

            // Set up button click listeners
            if (playAgainButton != null)
            {
                playAgainButton.onClick.AddListener(OnPlayAgainClicked);
            }

            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.AddListener(OnMainMenuClicked);
            }

            // Initially hide the victory banner
            if (victoryBanner != null)
            {
                victoryBanner.SetActive(false);
            }

            Debug.Log("GameOverScreen initialized successfully");
        }

        /// <summary>
        /// Called when the play again button is clicked
        /// </summary>
        private void OnPlayAgainClicked()
        {
            Debug.Log("Play again button clicked");

            // Load the game scene
            SceneManager.LoadScene(gameSceneName);
        }

        /// <summary>
        /// Called when the main menu button is clicked
        /// </summary>
        private void OnMainMenuClicked()
        {
            Debug.Log("Main menu button clicked");

            // Load the main menu scene
            SceneManager.LoadScene(mainMenuSceneName);
        }

        /// <summary>
        /// Show the game over screen with the specified winner
        /// </summary>
        /// <param name="winner">The player who won the game</param>
        public void ShowGameOver(PlayerType winner)
        {
            // Make sure the game over screen is active
            gameObject.SetActive(true);

            // Show the victory banner with the winner information
            if (victoryBanner != null)
            {
                victoryBanner.SetActive(true);

                // Get the VictoryBanner component and set the winner
                VictoryBanner banner = victoryBanner.GetComponent<VictoryBanner>();
                if (banner != null)
                {
                    // VictoryBanner already listens to WinDetector, so we don't need to set the winner here
                    // The banner will automatically update when the win is detected
                }
            }

            Debug.Log($"Game over screen displayed for winner: {winner}");
        }

        /// <summary>
        /// Hide the game over screen
        /// </summary>
        public void HideGameOver()
        {
            // Hide the victory banner
            if (victoryBanner != null)
            {
                victoryBanner.SetActive(false);
            }

            // Hide this game over screen
            gameObject.SetActive(false);

            Debug.Log("Game over screen hidden");
        }
    }
}