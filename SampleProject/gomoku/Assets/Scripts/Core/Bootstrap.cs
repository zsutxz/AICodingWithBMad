using UnityEngine;
using UnityEngine.SceneManagement;
using Gomoku.Systems;
using Gomoku.Audio;

namespace Gomoku.Core
{
    /// <summary>
    /// Bootstrap script that handles game initialization
    /// </summary>
    public class Bootstrap : MonoBehaviour
    {
        [Header("Bootstrap Settings")]
        [SerializeField] private bool autoStartGame = true;
        [SerializeField] private float initializationDelay = 0.5f;

        void Start()
        {
            if (autoStartGame)
            {
                StartCoroutine(InitializeGame());
            }
        }

        private System.Collections.IEnumerator InitializeGame()
        {
            Debug.Log("Bootstrap: Starting game initialization...");

            // Wait a short time to ensure everything is loaded
            yield return new WaitForSeconds(initializationDelay);

            // Initialize core systems
            InitializeCoreSystems();

            // Check current scene and set appropriate game state
            string currentScene = SceneManager.GetActiveScene().name;
            Debug.Log($"Bootstrap: Current scene is '{currentScene}'");

            SetGameStateForScene(currentScene);

            Debug.Log("Bootstrap: Game initialization complete");
        }

        private void InitializeCoreSystems()
        {
            // Ensure GameStateManager exists
            var gameStateManager = GameStateManager.Instance;
            if (gameStateManager != null)
            {
                Debug.Log("Bootstrap: GameStateManager initialized");
            }

            // Ensure AudioManager exists
            var audioManager = AudioManager.Instance;
            if (audioManager != null)
            {
                Debug.Log("Bootstrap: AudioManager initialized");
            }
        }

        private void SetGameStateForScene(string sceneName)
        {
            var gameStateManager = GameStateManager.Instance;
            if (gameStateManager == null)
            {
                Debug.LogError("Bootstrap: GameStateManager not available");
                return;
            }

            switch (sceneName)
            {
                case "MainMenu":
                    gameStateManager.SetState(GameStateEnum.MainMenu);
                    break;
                case "GameScene":
                    gameStateManager.SetState(GameStateEnum.Playing);
                    break;
                case "GameOver":
                    gameStateManager.SetState(GameStateEnum.GameOver);
                    break;
                default:
                    Debug.LogWarning($"Bootstrap: Unknown scene '{sceneName}', setting to MainMenu");
                    gameStateManager.SetState(GameStateEnum.MainMenu);
                    break;
            }
        }

        private void OnApplicationQuit()
        {
            Debug.Log("Bootstrap: Application quitting");
        }
    }
}