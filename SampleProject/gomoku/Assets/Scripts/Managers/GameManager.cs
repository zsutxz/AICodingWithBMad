using UnityEngine;
using UnityEngine.Events;

namespace Gomoku.Managers
{
    /// <summary>
    /// Manages overall game state and provides global game services
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        // Singleton instance
        private static GameManager instance;

        // Events for game state changes
        [Header("Events")]
        [Tooltip("Event triggered when the game starts")]
        [SerializeField] private UnityEvent onGameStart;

        [Tooltip("Event triggered when the game ends")]
        [SerializeField] private UnityEvent onGameEnd;

        [Tooltip("Event triggered when a player wins")]
        [SerializeField] private UnityEvent<int> onPlayerWin;

        [Tooltip("Event triggered when the game is reset")]
        [SerializeField] private UnityEvent onGameReset;

        #region Singleton Pattern

        /// <summary>
        /// Gets the singleton instance of the GameManager
        /// </summary>
        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    // Try to find existing instance in the scene
                    instance = FindObjectOfType<GameManager>();

                    // If no instance exists, create a new one
                    if (instance == null)
                    {
                        GameObject managerObject = new GameObject("GameManager");
                        instance = managerObject.AddComponent<GameManager>();
                    }
                }
                return instance;
            }
        }

        #endregion

        #region MonoBehaviour Methods

        /// <summary>
        /// Initialize the game manager
        /// </summary>
        private void Awake()
        {
            // Ensure only one instance exists
            if (instance == null)
            {
                instance = this;
                // Don't destroy this object when loading new scenes
                DontDestroyOnLoad(gameObject);
                Debug.Log("GameManager initialized");
            }
            else if (instance != this)
            {
                // If another instance exists, destroy this one
                Destroy(gameObject);
                return;
            }

            // Initialize any game systems
          InitializeGameSystems();
        }

        /// <summary>
        /// Called when the script instance is being destroyed
        /// </summary>
        private void OnDestroy()
        {
            // Clean up when the instance is destroyed
            if (instance == this)
            {
                instance = null;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Start a new game
        /// </summary>
        public void StartGame()
        {
            Debug.Log("Starting new game");
            // Trigger game start event
            onGameStart?.Invoke();
        }

        /// <summary>
        /// End the current game
        /// </summary>
        public void EndGame()
        {
            Debug.Log("Ending game");
            // Trigger game end event
            onGameEnd?.Invoke();
        }

        /// <summary>
        /// Declare a player as the winner
        /// </summary>
        /// <param name="playerId">The ID of the winning player (1 or 2)</param>
        public void DeclareWinner(int playerId)
        {
            Debug.Log($"Player {playerId} wins!");
            // Trigger player win event
            onPlayerWin?.Invoke(playerId);
        }

        /// <summary>
        /// Reset the game to its initial state
        /// </summary>
        public void ResetGame()
        {
            Debug.Log("Resetting game");
            // Trigger game reset event
            onGameReset?.Invoke();
        }

        /// <summary>
        /// Quit the application
        /// </summary>
        public void QuitGame()
        {
            Debug.Log("Quitting game");
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initialize any required game systems
        /// </summary>
        private void InitializeGameSystems()
        {
            // Initialize any game systems here
            // For example, initialize audio, save system, analytics, etc.
            Debug.Log("Initializing game systems");
        }

        #endregion
    }
}