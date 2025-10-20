using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Gomoku.Core;

namespace Gomoku.UI
{
    /// <summary>
    /// Displays the current player's turn with animated feedback
    /// </summary>
    public class TurnIndicator : MonoBehaviour
    {
        [Header("Component References")]
        [SerializeField] private TurnManager turnManager;
        [SerializeField] private TextMeshProUGUI turnText;
        [SerializeField] private Image playerIcon;

        [Header("Visual Settings")]
        [SerializeField] private Sprite blackPlayerSprite;
        [SerializeField] private Sprite whitePlayerSprite;
        [SerializeField] private Color blackPlayerColor = Color.black;
        [SerializeField] private Color whitePlayerColor = new Color(0.9f, 0.9f, 0.9f);
        [SerializeField] private string blackTurnText = "Black's Turn";
        [SerializeField] private string whiteTurnText = "White's Turn";

        [Header("Animation Settings")]
        [SerializeField] private float animationDuration = 0.3f;
        [SerializeField] private AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private Coroutine turnChangeAnimation;

        private void Awake()
        {
            InitializeTurnIndicator();
        }

        private void OnDestroy()
        {
            CleanupEvents();
        }

        /// <summary>
        /// Initializes the turn indicator
        /// </summary>
        private void InitializeTurnIndicator()
        {
            // Auto-find references
            if (turnManager == null) 
                turnManager = FindObjectOfType<TurnManager>();

            if (turnText == null) 
                turnText = GetComponentInChildren<TextMeshProUGUI>();

            if (playerIcon == null) 
                playerIcon = GetComponentInChildren<Image>();

            if (turnManager == null)
            {
                Debug.LogError("TurnManager reference not found for TurnIndicator");
                enabled = false;
                return;
            }

            // Subscribe to turn changes
            //turnManager.OnPlayerTurnChanged += Handle_OnPlayerTurnChanged;

            // Set initial state
            UpdateTurnDisplay(turnManager.CurrentPlayer, false);

            Debug.Log("TurnIndicator initialized");
        }

        /// <summary>
        /// Handles the turn changed event
        /// </summary>
        private void Handle_OnPlayerTurnChanged(PlayerType newPlayer)
        {
            UpdateTurnDisplay(newPlayer, true);
        }

        /// <summary>
        /// Updates the turn indicator display
        /// </summary>
        /// <param name="player">Current player</param>
        /// <param name="animate">Whether to animate the change</param>
        public void UpdateTurnDisplay(PlayerType player, bool animate)
        {
            string textToShow = "";
            Sprite iconToShow = null;
            Color textColor = Color.white;

            switch (player)
            {
                case PlayerType.PlayerOne:
                    textToShow = blackTurnText;
                    iconToShow = blackPlayerSprite;
                    textColor = blackPlayerColor;
                    break;

                case PlayerType.PlayerTwo:
                    textToShow = whiteTurnText;
                    iconToShow = whitePlayerSprite;
                    textColor = whitePlayerColor;
                    break;
            }

            if (animate)
            {
                if (turnChangeAnimation != null)
                {
                    StopCoroutine(turnChangeAnimation);
                }
                turnChangeAnimation = StartCoroutine(AnimateTurnChange(textToShow, iconToShow, textColor));
            }
            else
            {
                if (turnText != null) turnText.text = textToShow;
                if (playerIcon != null) playerIcon.sprite = iconToShow;
                if (turnText != null) turnText.color = textColor;
            }
        }

        /// <summary>
        /// Animates the turn change feedback
        /// </summary>
        private System.Collections.IEnumerator AnimateTurnChange(string newText, Sprite newIcon, Color newColor)
        {
            // Animation: scale down, change content, scale up
            Vector3 originalScale = transform.localScale;
            float elapsed = 0f;

            // Scale down
            while (elapsed < animationDuration / 2)
            {
                elapsed += Time.deltaTime;
                float t = animationCurve.Evaluate(elapsed / (animationDuration / 2));
                transform.localScale = Vector3.Lerp(originalScale, Vector3.one * 0.1f, t);
                yield return null;
            }

            // Change content at the midpoint
            if (turnText != null) turnText.text = newText;
            if (playerIcon != null) playerIcon.sprite = newIcon;
            if (turnText != null) turnText.color = newColor;

            // Scale up
            elapsed = 0f;
            while (elapsed < animationDuration / 2)
            {
                elapsed += Time.deltaTime;
                float t = animationCurve.Evaluate(elapsed / (animationDuration / 2));
                transform.localScale = Vector3.Lerp(Vector3.one * 0.1f, originalScale, t);
                yield return null;
            }

            transform.localScale = originalScale; // Ensure it ends at the exact scale
            turnChangeAnimation = null;
        }

        /// <summary>
        /// Cleans up event subscriptions
        /// </summary>
        private void CleanupEvents()
        {
            if (turnManager != null)
            {
                turnManager.OnPlayerTurnChanged.RemoveListener(new UnityEngine.Events.UnityAction<Gomoku.Core.PlayerType>(Handle_OnPlayerTurnChanged));
            }
        }
    }
}
