using UnityEngine;
using System.Collections;

namespace Gomoku.Animation
{
    /// <summary>
    /// Manages game state transition effects and coordinates with GameStateManager
    /// </summary>
    public class GameStateTransitionManager : MonoBehaviour
    {
        [Header("Component References")]
        [SerializeField] private GameStateManager gameStateManager;
        [SerializeField] private GameStateTransition gameStateTransition;
        
        [Header("Transition Settings")]
        [SerializeField] private bool enableStateTransitions = true;
        [SerializeField] private float transitionDuration = 0.5f;
        [SerializeField] private Color fadeColor = Color.black;
        
        [Header("State-Specific Settings")]
        [SerializeField] private bool fadeToMainMenu = true;
        [SerializeField] private bool fadeToGame = true;
        [SerializeField] private bool fadeToGameOver = true;
        
        // Transition state
        private bool isTransitioning = false;
        private Coroutine currentTransitionCoroutine;
        
        // Events
        public System.Action<GameStateEnum> OnStateTransitionStart;
        public System.Action<GameStateEnum> OnStateTransitionComplete;
        
        // Properties
        public bool IsTransitioning => isTransitioning;
        public bool EnableStateTransitions => enableStateTransitions;
        
        private void Awake()
        {
            Initialize();
        }
        
        /// <summary>
        /// Initializes the game state transition manager
        /// </summary>
        private void Initialize()
        {
            // Auto-find components if not assigned
            if (gameStateManager == null)
            {
                gameStateManager = FindObjectOfType<GameStateManager>();
            }
            
            if (gameStateTransition == null)
            {
                gameStateTransition = FindObjectOfType<GameStateTransition>();
            }
            
            if (gameStateManager == null)
            {
                Debug.LogError("GameStateTransitionManager: GameStateManager reference not found");
            }
            
            if (gameStateTransition == null)
            {
                Debug.LogWarning("GameStateTransitionManager: GameStateTransition reference not found - transitions will be disabled");
                enableStateTransitions = false;
            }
            
            // Configure the transition
            if (gameStateTransition != null)
            {
                gameStateTransition.SetFadeColor(fadeColor);
                gameStateTransition.SetFadeDuration(transitionDuration);
            }
        }
        
        /// <summary>
        /// Starts a state transition
        /// </summary>
        /// <param name="targetState">The target game state</param>
        public void StartStateTransition(GameStateEnum targetState)
        {
            if (!enableStateTransitions || gameStateTransition == null) return;
            
            if (isTransitioning)
            {
                StopCurrentTransition();
            }
            
            currentTransitionCoroutine = StartCoroutine(AnimateStateTransition(targetState));
        }
        
        /// <summary>
        /// Animates the state transition
        /// </summary>
        /// <param name="targetState">The target game state</param>
        /// <returns>Coroutine enumerator</returns>
        private IEnumerator AnimateStateTransition(GameStateEnum targetState)
        {
            isTransitioning = true;
            OnStateTransitionStart?.Invoke(targetState);
            
            // Check if we should fade for this state transition
            bool shouldFade = ShouldFadeForState(targetState);
            
            if (shouldFade)
            {
                // Fade in
                gameStateTransition.StartFadeTransition(true);
                
                // Wait for fade in to complete
                yield return new WaitForSeconds(transitionDuration);
                
                // Change game state (this would typically be done by the game logic)
                // For now, we'll just log the state change
                Debug.Log($"State transition to: {targetState}");
                
                // Fade out
                gameStateTransition.StartFadeTransition(false);
                
                // Wait for fade out to complete
                yield return new WaitForSeconds(transitionDuration);
            }
            else
            {
                // No fade, just change state
                Debug.Log($"State transition to: {targetState} (no fade)");
                yield return null;
            }
            
            isTransitioning = false;
            currentTransitionCoroutine = null;
            OnStateTransitionComplete?.Invoke(targetState);
        }
        
        /// <summary>
        /// Determines if a fade should be used for a state transition
        /// </summary>
        /// <param name="targetState">The target state</param>
        /// <returns>True if fade should be used</returns>
        private bool ShouldFadeForState(GameStateEnum targetState)
        {
            return targetState switch
            {
                GameStateEnum.MainMenu => fadeToMainMenu,
                GameStateEnum.Playing => fadeToGame,
                GameStateEnum.GameOver => fadeToGameOver,
                _ => true
            };
        }
        
        /// <summary>
        /// Stops the current transition
        /// </summary>
        public void StopCurrentTransition()
        {
            if (currentTransitionCoroutine != null)
            {
                StopCoroutine(currentTransitionCoroutine);
                currentTransitionCoroutine = null;
            }
            
            if (gameStateTransition != null)
            {
                gameStateTransition.StopCurrentTransition();
            }
            
            isTransitioning = false;
        }
        
        /// <summary>
        /// Sets whether state transitions are enabled
        /// </summary>
        /// <param name="enabled">Whether transitions are enabled</param>
        public void SetStateTransitionsEnabled(bool enabled)
        {
            enableStateTransitions = enabled;
            
            if (!enabled && isTransitioning)
            {
                StopCurrentTransition();
            }
        }
        
        /// <summary>
        /// Sets the transition duration
        /// </summary>
        /// <param name="duration">Transition duration</param>
        public void SetTransitionDuration(float duration)
        {
            transitionDuration = Mathf.Max(0.1f, duration);
            
            if (gameStateTransition != null)
            {
                gameStateTransition.SetFadeDuration(transitionDuration);
            }
        }
        
        /// <summary>
        /// Sets the fade color
        /// </summary>
        /// <param name="color">Fade color</param>
        public void SetFadeColor(Color color)
        {
            fadeColor = color;
            
            if (gameStateTransition != null)
            {
                gameStateTransition.SetFadeColor(fadeColor);
            }
        }
        
        /// <summary>
        /// Configures which states use fade transitions
        /// </summary>
        /// <param name="toMainMenu">Fade to main menu</param>
        /// <param name="toGame">Fade to game</param>
        /// <param name="toGameOver">Fade to game over</param>
        public void SetFadeStates(bool toMainMenu, bool toGame, bool toGameOver)
        {
            fadeToMainMenu = toMainMenu;
            fadeToGame = toGame;
            fadeToGameOver = toGameOver;
        }
        
        /// <summary>
        /// Immediately shows the fade overlay
        /// </summary>
        public void ShowFadeImmediate()
        {
            if (gameStateTransition != null)
            {
                gameStateTransition.ShowImmediate();
            }
        }
        
        /// <summary>
        /// Immediately hides the fade overlay
        /// </summary>
        public void HideFadeImmediate()
        {
            if (gameStateTransition != null)
            {
                gameStateTransition.HideImmediate();
            }
        }
        
        /// <summary>
        /// Resets the state transition manager
        /// </summary>
        public void ResetManager()
        {
            StopCurrentTransition();
            
            if (gameStateTransition != null)
            {
                gameStateTransition.HideImmediate();
            }
        }
        
        private void OnDestroy()
        {
            StopCurrentTransition();
        }
        
        #if UNITY_EDITOR
        /// <summary>
        /// Editor-only method for testing state transitions
        /// </summary>
        [ContextMenu("Test State Transition - Main Menu")]
        private void TestStateTransitionMainMenu()
        {
            if (Application.isPlaying)
            {
                StartStateTransition(GameStateEnum.MainMenu);
            }
            else
            {
                Debug.Log("State transition test can only be run in Play mode");
            }
        }
        
        /// <summary>
        /// Editor-only method for testing state transitions
        /// </summary>
        [ContextMenu("Test State Transition - Playing")]
        private void TestStateTransitionPlaying()
        {
            if (Application.isPlaying)
            {
                StartStateTransition(GameStateEnum.Playing);
            }
            else
            {
                Debug.Log("State transition test can only be run in Play mode");
            }
        }
        
        /// <summary>
        /// Editor-only method for testing state transitions
        /// </summary>
        [ContextMenu("Test State Transition - Game Over")]
        private void TestStateTransitionGameOver()
        {
            if (Application.isPlaying)
            {
                StartStateTransition(GameStateEnum.GameOver);
            }
            else
            {
                Debug.Log("State transition test can only be run in Play mode");
            }
        }
        
        /// <summary>
        /// Editor-only method for stopping state transitions
        /// </summary>
        [ContextMenu("Stop State Transition")]
        private void StopStateTransitionEditor()
        {
            if (Application.isPlaying)
            {
                StopCurrentTransition();
            }
            else
            {
                Debug.Log("State transition stop can only be run in Play mode");
            }
        }
        #endif
    }
}