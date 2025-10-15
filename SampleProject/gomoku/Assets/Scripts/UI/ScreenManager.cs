using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Gomoku.GameState;

namespace Gomoku.UI
{
    /// <summary>
    /// Manages scene transitions and screen navigation between different game states
    /// </summary>
    public class ScreenManager : MonoBehaviour
    {
        [Header("Scene Names")]
        [Tooltip("Name of the main menu scene")]
        [SerializeField] private string mainMenuSceneName = "MainMenu";
        
        [Tooltip("Name of the game scene")]
        [SerializeField] private string gameSceneName = "GameScene";
        
        [Tooltip("Name of the game over scene")]
        [SerializeField] private string gameOverSceneName = "GameOver";
        
        [Header("Transition Settings")]
        [Tooltip("Duration of scene transition fade effect in seconds")]
        [SerializeField] private float transitionDuration = 1.0f;
        
        [Tooltip("Reference to the GameStateManager for state synchronization")]
        [SerializeField] private GameStateManager gameStateManager;
        
        // Singleton instance
        private static ScreenManager instance;
        
        // Async operation for scene loading
        private AsyncOperation asyncOperation;
        
        private void Awake()
        {
            // Singleton pattern implementation
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                Debug.Log("ScreenManager initialized successfully");
            }
            else
            {
                Destroy(gameObject);
                Debug.LogWarning("Duplicate ScreenManager instance destroyed");
                return;
            }
            
            // Validate references
            if (gameStateManager == null)
            {
                gameStateManager = FindObjectOfType<GameStateManager>();
                if (gameStateManager == null)
                {
                    Debug.LogError("ScreenManager: GameStateManager not found in scene!");
                }
            }
        }
        
        /// <summary>
        /// Load a scene by name with optional transition effect
        /// </summary>
        /// <param name="sceneName">Name of the scene to load</param>
        /// <param name="useTransition">Whether to use a transition effect</param>
        public void LoadScene(string sceneName, bool useTransition = true)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("ScreenManager: Cannot load scene with null or empty name");
                return;
            }
            
            Debug.Log($"ScreenManager: Loading scene '{sceneName}'");
            
            if (useTransition)
            {
                StartCoroutine(LoadSceneWithTransition(sceneName));
            }
            else
            {
                SceneManager.LoadScene(sceneName);
            }
        }
        
        /// <summary>
        /// Load a scene asynchronously with transition effect
        /// </summary>
        /// <param name="sceneName">Name of the scene to load</param>
        /// <returns>Coroutine enumerator</returns>
        private IEnumerator LoadSceneWithTransition(string sceneName)
        {
            // Add transition effect here (e.g., fade to black)
            // This could be implemented with a UI panel or animation
            
            // Wait for transition duration
            yield return new WaitForSeconds(transitionDuration / 2);
            
            // Load the scene
            SceneManager.LoadScene(sceneName);
            
            // Wait for transition to complete
            yield return new WaitForSeconds(transitionDuration / 2);
            
            Debug.Log($"ScreenManager: Scene '{sceneName}' loaded with transition");
        }
        
        /// <summary>
        /// Load a scene asynchronously in the background
        /// </summary>
        /// <param name="sceneName">Name of the scene to load</param>
        /// <param name="activateOnLoad">Whether to activate the scene immediately when loaded</param>
        /// <returns>Coroutine enumerator</returns>
        public IEnumerator LoadSceneAsync(string sceneName, bool activateOnLoad = true)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("ScreenManager: Cannot load scene with null or empty name");
                yield break;
            }
            
            Debug.Log($"ScreenManager: Loading scene '{sceneName}' asynchronously");
            
            // Start loading the scene asynchronously
            asyncOperation = SceneManager.LoadSceneAsync(sceneName);
            asyncOperation.allowSceneActivation = activateOnLoad;
            
            // Wait until the scene is loaded
            while (!asyncOperation.isDone)
            {
                // Progress can be used for loading bar implementation
                Debug.Log($"Loading progress: {asyncOperation.progress * 100}%");
                yield return null;
            }
            
            Debug.Log($"ScreenManager: Scene '{sceneName}' loaded asynchronously");
        }
        
        /// <summary>
        /// Load the main menu scene
        /// </summary>
        public void LoadMainMenu()
        {
            LoadScene(mainMenuSceneName);
        }
        
        /// <summary>
        /// Load the game scene
        /// </summary>
        public void LoadGameScene()
        {
            LoadScene(gameSceneName);
        }
        
        /// <summary>
        /// Load the game over scene
        /// </summary>
        public void LoadGameOverScene()
        {
            LoadScene(gameOverSceneName);
        }
        
        /// <summary>
        /// Restart the current scene
        /// </summary>
        public void RestartCurrentScene()
        {
            string currentScene = SceneManager.GetActiveScene().name;
            LoadScene(currentScene);
        }
        
        /// <summary>
        /// Get the name of the currently active scene
        /// </summary>
        /// <returns>Name of the current scene</returns>
        public string GetCurrentSceneName()
        {
            return SceneManager.GetActiveScene().name;
        }
        
        /// <summary>
        /// Check if a specific scene is currently loaded
        /// </summary>
        /// <param name="sceneName">Name of the scene to check</param>
        /// <returns>True if the scene is currently loaded</returns>
        public bool IsSceneLoaded(string sceneName)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.name == sceneName && scene.isLoaded)
                {
                    return true;
                }
            }
            return false;
        }
        
        /// <summary>
        /// Get the async operation for the current scene loading (if any)
        /// </summary>
        /// <returns>Current AsyncOperation or null if no scene is being loaded</returns>
        public AsyncOperation GetAsyncOperation()
        {
            return asyncOperation;
        }
    }
}