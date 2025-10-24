using UnityEngine;
using System.Collections;
using Gomoku;
using Gomoku.Core;

namespace Gomoku.Animation
{
    /// <summary>
    /// Component for providing visual indicators during turn transitions
    /// </summary>
    public class TurnTransition : MonoBehaviour
    {
        [Header("Turn Transition Settings")]
        [SerializeField] private float transitionDuration = 0.5f;
        [SerializeField] private AnimationCurve colorFadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private AnimationCurve pulseCurve = AnimationCurve.EaseInOut(0, 1, 1, 1.2f);
        
        [Header("Visual Settings")]
        [SerializeField] private Color playerOneColor = Color.black;
        [SerializeField] private Color playerTwoColor = Color.white;
        [SerializeField] private Color transitionColor = Color.yellow;
        
        [Header("Visual Components")]
        [SerializeField] private SpriteRenderer indicatorRenderer;
        [SerializeField] private TMPro.TextMeshProUGUI turnText;
        
        // Animation state
        private bool isAnimating = false;
        private Coroutine transitionAnimation;
        private Color originalColor;
        
        // Events
        public System.Action<TurnTransition> OnTransitionStart;
        public System.Action<TurnTransition> OnTransitionComplete;
        
        // Properties
        public bool IsAnimating => isAnimating;
        public float TransitionDuration => transitionDuration;
        
        private void Awake()
        {
            // Ensure we have visual components
            if (indicatorRenderer == null)
            {
                indicatorRenderer = GetComponent<SpriteRenderer>();
            }
            
            if (indicatorRenderer != null)
            {
                originalColor = indicatorRenderer.color;
            }
        }
        
        /// <summary>
        /// Starts the turn transition animation
        /// </summary>
        /// <param name="newPlayer">The player whose turn is starting</param>
        public void StartTurnTransition(PlayerType newPlayer)
        {
            if (isAnimating)
            {
                StopTransitionAnimation();
            }
            
            transitionAnimation = StartCoroutine(AnimateTurnTransition(newPlayer));
        }
        
        /// <summary>
        /// Animates the turn transition with color fade and pulse effects
        /// </summary>
        /// <param name="newPlayer">The player whose turn is starting</param>
        /// <returns>Coroutine enumerator</returns>
        private IEnumerator AnimateTurnTransition(PlayerType newPlayer)
        {
            isAnimating = true;
            OnTransitionStart?.Invoke(this);
            
            Color targetColor = GetPlayerColor(newPlayer);
            
            float elapsed = 0f;
            
            while (elapsed < transitionDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / transitionDuration;
                float colorT = colorFadeCurve.Evaluate(t);
                float pulseT = pulseCurve.Evaluate(t);
                
                // Apply color fade effect
                if (indicatorRenderer != null)
                {
                    Color currentColor = Color.Lerp(originalColor, transitionColor, colorT);
                    indicatorRenderer.color = currentColor;
                }
                
                // Apply pulse effect to scale
                transform.localScale = Vector3.one * pulseT;
                
                // Update text if available
                if (turnText != null)
                {
                    turnText.color = Color.Lerp(originalColor, targetColor, colorT);
                    
                    // Add slight scale effect to text
                    turnText.transform.localScale = Vector3.one * Mathf.Lerp(0.8f, 1f, colorT);
                }
                
                yield return null;
            }
            
            // Set final state
            if (indicatorRenderer != null)
            {
                indicatorRenderer.color = targetColor;
            }
            
            if (turnText != null)
            {
                turnText.color = targetColor;
                turnText.transform.localScale = Vector3.one;
            }
            
            transform.localScale = Vector3.one;
            
            isAnimating = false;
            transitionAnimation = null;
            OnTransitionComplete?.Invoke(this);
        }
        
        /// <summary>
        /// Stops the transition animation
        /// </summary>
        public void StopTransitionAnimation()
        {
            if (transitionAnimation != null)
            {
                StopCoroutine(transitionAnimation);
                transitionAnimation = null;
            }
            
            ResetVisualState();
            isAnimating = false;
        }
        
        /// <summary>
        /// Resets the visual state
        /// </summary>
        private void ResetVisualState()
        {
            if (indicatorRenderer != null)
            {
                indicatorRenderer.color = originalColor;
            }
            
            if (turnText != null)
            {
                turnText.color = originalColor;
                turnText.transform.localScale = Vector3.one;
            }
            
            transform.localScale = Vector3.one;
        }
        
        /// <summary>
        /// Gets the color associated with a player
        /// </summary>
        /// <param name="player">The player</param>
        /// <returns>The player's color</returns>
        private Color GetPlayerColor(PlayerType player)
        {
            return player switch
            {
                PlayerType.PlayerOne => playerOneColor,
                PlayerType.PlayerTwo => playerTwoColor,
                _ => originalColor
            };
        }
        
        /// <summary>
        /// Sets the player colors
        /// </summary>
        /// <param name="playerOneColor">Color for player one</param>
        /// <param name="playerTwoColor">Color for player two</param>
        public void SetPlayerColors(Color playerOneColor, Color playerTwoColor)
        {
            this.playerOneColor = playerOneColor;
            this.playerTwoColor = playerTwoColor;
        }
        
        /// <summary>
        /// Sets the transition color
        /// </summary>
        /// <param name="color">Transition color</param>
        public void SetTransitionColor(Color color)
        {
            transitionColor = color;
        }
        
        /// <summary>
        /// Sets the transition duration
        /// </summary>
        /// <param name="duration">Transition duration</param>
        public void SetTransitionDuration(float duration)
        {
            transitionDuration = Mathf.Max(0.1f, duration);
        }
        
        /// <summary>
        /// Updates the turn text display
        /// </summary>
        /// <param name="player">Current player</param>
        public void UpdateTurnDisplay(PlayerType player)
        {
            if (turnText != null)
            {
                string playerName = player switch
                {
                    PlayerType.PlayerOne => "Black",
                    PlayerType.PlayerTwo => "White",
                    _ => "Unknown"
                };
                
                turnText.text = $"{playerName}'s Turn";
                turnText.color = GetPlayerColor(player);
            }
        }
        
        #if UNITY_EDITOR
        /// <summary>
        /// Editor-only method for testing the turn transition
        /// </summary>
        [ContextMenu("Test Turn Transition - Player One")]
        private void TestTransitionPlayerOne()
        {
            if (Application.isPlaying)
            {
                StartTurnTransition(PlayerType.PlayerOne);
            }
            else
            {
                Debug.Log("Turn transition test can only be run in Play mode");
            }
        }
        
        /// <summary>
        /// Editor-only method for testing the turn transition
        /// </summary>
        [ContextMenu("Test Turn Transition - Player Two")]
        private void TestTransitionPlayerTwo()
        {
            if (Application.isPlaying)
            {
                StartTurnTransition(PlayerType.PlayerTwo);
            }
            else
            {
                Debug.Log("Turn transition test can only be run in Play mode");
            }
        }
        
        /// <summary>
        /// Editor-only method for resetting the turn transition
        /// </summary>
        [ContextMenu("Reset Turn Transition")]
        public void ResetTurnTransition()
        {
            StopTransitionAnimation();
        }
        #endif
    }
}