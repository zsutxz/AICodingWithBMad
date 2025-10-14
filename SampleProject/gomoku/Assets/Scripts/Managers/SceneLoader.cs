using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Gomoku.SceneManagement
{
    /// <summary>
    /// Manages scene loading operations including synchronous and asynchronous loading
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Reference to the loading screen canvas")]
        [SerializeField] private GameObject loadingScreen;

        [Tooltip("Progress bar UI element to show loading progress")]
        [SerializeField] private Slider progressBar;

        [Header("Settings")]
        [Tooltip("Whether to show loading screen for fast loads")]
        [SerializeField] private bool alwaysShowLoadingScreen = true;

        [Tooltip("Minimum time to show loading screen even if scene loads quickly")]
        [SerializeField] private float minLoadingScreenTime = 0.5f;

        // Flag to track if a scene is currently being loaded
        private bool isLoading = false;

        #region MonoBehaviour Methods

        /// <summary>
        /// Initialize the scene loader
        /// </summary>
        private void Awake()
        {
            // Ensure this manager persists across scene changes
            DontDestroyOnLoad(gameObject);

            // Hide loading screen initially
            if (loadingScreen != null)
            {
                loadingScreen.SetActive(false);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Load a scene synchronously
        /// </summary>
        /// <param name="sceneName">Name of the scene to load</param>
        public void LoadScene(string sceneName)
        {
            if (isLoading)
            {
                Debug.LogWarning("Scene load already in progress. Aborting request to load: " + sceneName);
                return;
            }

            Debug.Log($"Loading scene synchronously: {sceneName}");

            try
            {
                SceneManager.LoadScene(sceneName);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load scene {sceneName}: " + e.Message);
            }
        }

        /// <summary>
        /// Load a scene asynchronously with loading screen
        /// </summary>
        /// <param name="sceneName">Name of the scene to load</param>
        public void LoadSceneAsync(string sceneName)
        {
            if (isLoading)
            {
                Debug.LogWarning("Scene load already in progress. Aborting request to load: " + sceneName);
                return;
            }

            StartCoroutine(LoadSceneAsyncCoroutine(sceneName));
        }

        /// <summary>
        /// Get the current loading progress
        /// </summary>
        /// <returns>Progress value between 0 and 1</returns>
        public float GetLoadingProgress()
        {
            if (!isLoading)
            {
                return 1f;
            }

            // In a real implementation, this would track the actual async operation progress
            // For now, we'll return a placeholder value
            return 0.5f;
        }

        /// <summary>
        /// Check if a scene is currently being loaded
        /// </summary>
        /// <returns>True if loading is in progress</returns>
        public bool IsLoading()
        {
            return isLoading;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Coroutine for asynchronous scene loading
        /// </summary>
        /// <param name="sceneName">Name of the scene to load</param>
        /// <returns>IEnumerator for the coroutine</returns>
        private IEnumerator LoadSceneAsyncCoroutine(string sceneName)
        {
            isLoading = true;
            float loadStartTime = Time.time;

            // Show loading screen
            if (loadingScreen != null)
            {
                loadingScreen.SetActive(true);
            }

            // Initialize progress bar
            if (progressBar != null)
            {
                progressBar.value = 0f;
            }

            Debug.Log($"Starting async scene load: {sceneName}");

            // Start the asynchronous operation
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);

            // Don't allow the Scene to activate until we allow it
            asyncOperation.allowSceneActivation = false;


            // Wait for the asynchronous operation to complete
            while (!asyncOperation.isDone)
            {
                // Update progress bar (0 to 0.9)
                float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);

                if (progressBar != null)
                {
                    progressBar.value = progress;
                }

                // Check if we can activate the scene
                if (asyncOperation.progress >= 0.9f)
                {
                    // Ensure minimum loading screen time
                    float elapsedTime = Time.time - loadStartTime;
                    if (elapsedTime >= minLoadingScreenTime)
                    {
                        // Activate the scene
                        asyncOperation.allowSceneActivation = true;
                    }
                }

                yield return null;
            }

            // Loading complete
            isLoading = false;

            // Hide loading screen
            if (loadingScreen != null)
            {
                loadingScreen.SetActive(false);
            }

            Debug.Log($"Scene loaded successfully: {sceneName}");
        }

        #endregion
    }
}