using UnityEngine;
using TMPro;
using Gomoku.GameBoard;

namespace Gomoku.UI
{
    /// <summary>
    /// Displays the current move count during gameplay
    /// </summary>
    public class MoveCounter : MonoBehaviour
    {
        [Header("Component References")]
        [SerializeField] private TextMeshProUGUI moveText;

        [Header("Text Settings")]
        [SerializeField] private string moveCounterFormat = "Moves: {0}";
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color milestoneColor = Color.yellow;

        [Header("Animation Settings")]
        [SerializeField] private float animationDuration = 0.5f;
        [SerializeField] private AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        // Milestone thresholds for special visual feedback
        private int[] milestones = { 10, 25, 50, 75, 100 };

        private void Awake()
        {
            InitializeMoveCounter();
        }

        private void OnDestroy()
        {
            CleanupEvents();
        }

        /// <summary>
        /// Initializes the move counter
        /// </summary>
        private void InitializeMoveCounter()
        {
            if (moveText == null)
                moveText = GetComponentInChildren<TextMeshProUGUI>();

            // Set initial display
            UpdateMoveDisplay(gameBoardModel.GetMoveCount(), false);

            Debug.Log("MoveCounter initialized");
        }

        /// <summary>
        /// Updates the move counter display
        /// </summary>
        /// <param name="moveCount">Current move count</param>
        /// <param name="animate">Whether to animate the change</param>
        private void UpdateMoveDisplay(int moveCount, bool animate)
        {
            string textToShow = string.Format(moveCounterFormat, moveCount);
            Color textColor = GetTextColor(moveCount);

            if (animate)
            {
                StartCoroutine(AnimateMoveChange(textToShow, textColor));
            }
            else
            {
                if (moveText != null)
                {
                    moveText.text = textToShow;
                    moveText.color = textColor;
                }
            }
        }

        /// <summary>
        /// Gets the appropriate text color based on move count
        /// </summary>
        /// <param name="moveCount">Current move count</param>
        /// <returns>Text color for display</returns>
        private Color GetTextColor(int moveCount)
        {
            // Check if current move count is a milestone
            foreach (int milestone in milestones)
            {
                if (moveCount == milestone)
                {
                    return milestoneColor;
                }
            }
            return normalColor;
        }

        /// <summary>
        /// Animates the move counter change
        /// </summary>
        private System.Collections.IEnumerator AnimateMoveChange(string newText, Color newColor)
        {
            if (moveText == null) yield break;

            Vector3 originalScale = moveText.transform.localScale;
            float elapsed = 0f;

            // Scale up
            while (elapsed < animationDuration / 2)
            {
                elapsed += Time.deltaTime;
                float t = animationCurve.Evaluate(elapsed / (animationDuration / 2));
                moveText.transform.localScale = Vector3.Lerp(originalScale, originalScale * 1.2f, t);
                yield return null;
            }

            // Change content at the peak
            moveText.text = newText;
            moveText.color = newColor;

            // Scale down
            elapsed = 0f;
            while (elapsed < animationDuration / 2)
            {
                elapsed += Time.deltaTime;
                float t = animationCurve.Evaluate(elapsed / (animationDuration / 2));
                moveText.transform.localScale = Vector3.Lerp(originalScale * 1.2f, originalScale, t);
                yield return null;
            }

            moveText.transform.localScale = originalScale; // Ensure it ends at the exact scale
        }

        /// <summary>
        /// Called when a move is made to update the counter
        /// </summary>
        public void OnMoveMade()
        {
            if (gameBoardModel != null)
            {
                int currentMoveCount = gameBoardModel.GetMoveCount();
                UpdateMoveDisplay(currentMoveCount, true);
            }
        }

        /// <summary>
        /// Called when the game is reset
        /// </summary>
        public void OnGameReset()
        {
            UpdateMoveDisplay(0, false);
        }

        /// <summary>
        /// Cleans up event subscriptions
        /// </summary>
        private void CleanupEvents()
        {
            // Clean up any event subscriptions if needed
        }

        /// <summary>
        /// Manually update the move counter display
        /// </summary>
        public void RefreshDisplay()
        {
            if (gameBoardModel != null)
            {
                int currentMoveCount = gameBoardModel.GetMoveCount();
                UpdateMoveDisplay(currentMoveCount, false);
            }
        }
    }
}