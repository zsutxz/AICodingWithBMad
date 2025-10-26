using UnityEngine;
using UnityEngine.SceneManagement;
using Gomoku.Systems;
using Gomoku.Audio;
using Gomoku.UI;

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

            // Ensure UIManager exists and is properly set up
            EnsureUIManagerExists();
        }

        private void EnsureUIManagerExists()
        {
            // Log current state before cleanup
            Debug.Log($"Bootstrap: Starting UIManager setup. Current scene: '{UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}'");

            // First, VERY aggressively clean up any UIManager components in scenes
            CleanupSceneUIManagers();

            // Add a second cleanup pass after a brief delay to catch any UIManager components
            // that might be instantiated during the scene loading process
            StartCoroutine(SecondaryCleanup());

            // Check if UIManager already exists (should be after cleanup)
            var existingUIManager = UIManager.Instance;
            if (existingUIManager != null)
            {
                Debug.Log($"Bootstrap: UIManager already exists and properly initialized (GameObject: '{existingUIManager.gameObject.name}', Scene: '{GetSceneName(existingUIManager.gameObject)}')");
                return;
            }

            // Create UIManager if it doesn't exist
            Debug.Log("Bootstrap: Creating UIManager singleton");
            var uiManagerObject = new GameObject("UIManager_Singleton");
            var uiManager = uiManagerObject.AddComponent<UIManager>();

            // Set it to not destroy on load so it persists across scenes
            DontDestroyOnLoad(uiManagerObject);

            Debug.Log($"Bootstrap: UIManager singleton created successfully (GameObject: '{uiManagerObject.name}')");
        }

        private System.Collections.IEnumerator SecondaryCleanup()
        {
            // Wait for end of frame to catch any UIManager components that might be instantiated
            yield return new WaitForEndOfFrame();

            // Check again for any UIManager components that might have been created
            var sceneUIManagers = Object.FindObjectsOfType<UIManager>();
            foreach (var uiManager in sceneUIManagers)
            {
                if (uiManager != UIManager.Instance)
                {
                    Debug.LogWarning($"Bootstrap: Secondary cleanup found UIManager on GameObject '{uiManager.gameObject.name}' in scene '{GetSceneName(uiManager.gameObject)}'. Removing it.");
                    uiManager.gameObject.SetActive(false);
                    Object.DestroyImmediate(uiManager.gameObject);
                }
            }
        }

        /// <summary>
        /// Helper method to get the scene name for a GameObject
        /// </summary>
        /// <param name="obj">GameObject to get scene name for</param>
        /// <returns>Scene name or "Unknown" if not available</returns>
        private static string GetSceneName(GameObject obj)
        {
            if (obj == null) return "Null";

            var scene = obj.scene;
            if (scene.IsValid()) return scene.name;

            // If GameObject is not part of any scene (like DontDestroyOnLoad objects)
            if (obj.transform.parent != null)
            {
                return "DontDestroyOnLoad";
            }

            return "Unknown";
        }

        private void CleanupSceneUIManagers()
        {
            // Find all UIManager components in the current scene
            var sceneUIManagers = Object.FindObjectsOfType<UIManager>();
            int cleanedCount = 0;

            foreach (var uiManager in sceneUIManagers)
            {
                // Only clean up if it's not our singleton instance
                if (uiManager != UIManager.Instance)
                {
                    Debug.Log($"Bootstrap: Removing UIManager from GameObject '{uiManager.name}' in scene '{GetSceneName(uiManager.gameObject)}'");

                    // Disable the GameObject immediately to prevent any Awake calls
                    uiManager.gameObject.SetActive(false);

                    // Destroy the GameObject
                    Object.DestroyImmediate(uiManager.gameObject);
                    cleanedCount++;
                }
            }

            if (cleanedCount > 0)
            {
                Debug.Log($"Bootstrap: Cleaned up {cleanedCount} UIManager instances from scene '{UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}'");
            }
            else
            {
                Debug.Log($"Bootstrap: No UIManager cleanup needed in scene '{UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}'");
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