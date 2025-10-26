using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Gomoku.UI
{
    /// <summary>
    /// Unified UI management system that handles all UI elements, screens, and scene transitions
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("UI Prefabs")]
        [SerializeField] private GameObject startButtonPrefab;
        [SerializeField] private GameObject pauseButtonPrefab;
        [SerializeField] private GameBoardController gameBoardPrefab;

        [Header("Scene Names")]
        [Tooltip("Name of the main menu scene")]
        [SerializeField] private string mainMenuSceneName = "MainMenu";

        [Tooltip("Name of the game scene")]
        [SerializeField] private string gameSceneName = "GameScene";

        [Header("Transition Settings")]
        [Tooltip("Duration of scene transition fade effect in seconds")]
        [SerializeField] private float transitionDuration = 1.0f;

        [Header("UI References")]
        private Button startButton;
        private Button pauseButton;
        private Text startButtonText;
        private GameObject pauseMenu;

        private GameBoardController m_gameBoardCtr;
        private ButtonHandler buttonHandler;
        private Gomoku.Systems.GameStateManager gameStateManager;

        // Singleton instance
        private static UIManager instance;
        public static UIManager Instance => instance;

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

        /// <summary>
        /// Clean up any UIManager instances in the current scene (useful for testing or debugging)
        /// </summary>
        public static void CleanupSceneUIManagers()
        {
            var sceneUIManagers = Object.FindObjectsOfType<UIManager>();
            foreach (var uiManager in sceneUIManagers)
            {
                if (uiManager != instance)
                {
                    Debug.Log($"Cleaning up UIManager from GameObject '{uiManager.name}' in scene '{GetSceneName(uiManager.gameObject)}'");
                    Object.DestroyImmediate(uiManager.gameObject);
                }
            }
        }

        // Async operation for scene loading
        private AsyncOperation asyncOperation;

        private void Awake()
        {
            // Enhanced singleton pattern implementation with detailed debugging
            Debug.Log($"UIManager: Awake() called for GameObject '{gameObject.name}' in scene '{GetSceneName(gameObject)}'. Current instance: {(instance != null ? instance.gameObject.name : "null")}");

            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeUI();
                Debug.Log($"UIManager: Singleton established for GameObject '{gameObject.name}' in scene '{GetSceneName(gameObject)}'");
            }
            else
            {
                // Check if the existing instance is actually different from this one
                if (instance != this)
                {
                    // Log detailed information for debugging
                    Debug.LogWarning($"UIManager: Singleton conflict detected. " +
                              $"Existing: '{instance.gameObject.name}' (Scene: {GetSceneName(instance.gameObject)}) " +
                              $"Duplicate: '{gameObject.name}' (Scene: {GetSceneName(gameObject)})");

                    // If this duplicate is from a scene, deactivate it immediately to prevent any Awake conflicts
                    gameObject.SetActive(false);

                    // Destroy this duplicate instance to maintain singleton pattern
                    Destroy(gameObject);
                }
                else
                {
                    // This should not happen, but handle it gracefully
                    Debug.LogError("UIManager: Self-reference detected in singleton pattern. This indicates a serious issue!");
                }
            }
        }

        private void InitializeUI()
        {
            gameStateManager = Gomoku.Systems.GameStateManager.Instance;
            
            // Create ButtonHandler for UI interactions
            GameObject handlerObject = new GameObject("ButtonHandler");
            handlerObject.transform.SetParent(transform);
            buttonHandler = handlerObject.AddComponent<ButtonHandler>();
            
            if(gameBoardPrefab != null)
            {
                m_gameBoardCtr = Instantiate<GameBoardController>(gameBoardPrefab, transform);
            }

            // Dynamically create StartButton
            if (startButtonPrefab != null)
            {
                GameObject buttonObject = Instantiate(startButtonPrefab, transform);
                startButton = buttonObject.GetComponent<Button>();
                startButton.onClick.AddListener(buttonHandler.OnStartButtonClicked);
            }
            
            // Create pause button
            CreatePauseButton();

            Debug.Log("UIManager: UI initialized successfully");
        }
        
        private void CreatePauseButton()
        {
            if (pauseButtonPrefab != null)
            {
                GameObject pauseButtonObject = Instantiate(pauseButtonPrefab, transform);
                pauseButton = pauseButtonObject.GetComponent<Button>();
                pauseButton.onClick.AddListener(buttonHandler.OnPauseButtonClicked);
                
                // Initially hide pause button until game starts
                pauseButtonObject.SetActive(false);
            }
        }


        /// <summary>
        /// Sets the active state of the UI Manager's root GameObject
        /// </summary>
        /// <param name="active">Whether to activate the UI</param>
        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
        
        /// <summary>
        /// Shows or hides the pause button based on game state
        /// </summary>
        /// <param name="show">Whether to show the pause button</param>
        public void ShowPauseButton(bool show)
        {
            if (pauseButton != null && pauseButton.gameObject != null)
            {
                pauseButton.gameObject.SetActive(show);
            }
        }
        
        /// <summary>
        /// Shows the pause menu overlay
        /// </summary>
        public void ShowPauseMenu()
        {
            if (pauseMenu == null)
            {
                CreatePauseMenu();
            }
            pauseMenu.SetActive(true);
        }
        
        /// <summary>
        /// Hides the pause menu overlay
        /// </summary>
        public void HidePauseMenu()
        {
            if (pauseMenu != null)
            {
                pauseMenu.SetActive(false);
            }
        }
        
        private void CreatePauseMenu()
        {
            // Create pause menu container
            pauseMenu = new GameObject("PauseMenu");
            pauseMenu.transform.SetParent(transform);
            
            // Add background panel
            GameObject background = new GameObject("Background");
            background.transform.SetParent(pauseMenu.transform);
            RectTransform bgRect = background.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            bgRect.anchoredPosition = Vector2.zero;
            
            Image bgImage = background.AddComponent<Image>();
            bgImage.color = new Color(0, 0, 0, 0.8f);
            
            // Create menu panel
            GameObject menuPanel = new GameObject("MenuPanel");
            menuPanel.transform.SetParent(pauseMenu.transform);
            RectTransform panelRect = menuPanel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.3f, 0.3f);
            panelRect.anchorMax = new Vector2(0.7f, 0.7f);
            panelRect.sizeDelta = Vector2.zero;
            panelRect.anchoredPosition = Vector2.zero;
            
            Image panelImage = menuPanel.AddComponent<Image>();
            panelImage.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);
            
            // Add pause menu buttons (will be implemented in next task)
            CreatePauseMenuButtons(menuPanel);
        }
        
        private void CreatePauseMenuButtons(GameObject panel)
        {
            // Create title
            GameObject titleObject = new GameObject("Title");
            titleObject.transform.SetParent(panel.transform);
            RectTransform titleRect = titleObject.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 0.8f);
            titleRect.anchorMax = new Vector2(0.5f, 0.9f);
            titleRect.sizeDelta = new Vector2(200, 40);
            titleRect.anchoredPosition = Vector2.zero;
            
            Text titleText = titleObject.AddComponent<Text>();
            titleText.text = "PAUSED";
            titleText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            titleText.fontSize = 32;
            titleText.color = Color.white;
            titleText.alignment = TextAnchor.MiddleCenter;
            
            // Create Resume button
            CreateMenuButton(panel, "Resume", new Vector2(0.5f, 0.6f), buttonHandler.OnResumeButtonClicked);
            
            // Create Settings button
            CreateMenuButton(panel, "Settings", new Vector2(0.5f, 0.4f), buttonHandler.OnSettingsButtonClicked);
            
            // Create Quit button
            CreateMenuButton(panel, "Quit", new Vector2(0.5f, 0.2f), buttonHandler.OnQuitButtonClicked);
        }
        
        private void CreateMenuButton(GameObject parent, string buttonText, Vector2 anchorPreset, UnityEngine.Events.UnityAction onClickAction)
        {
            GameObject buttonObject = new GameObject($"{buttonText}Button");
            buttonObject.transform.SetParent(parent.transform);
            
            RectTransform buttonRect = buttonObject.AddComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(anchorPreset.x - 0.2f, anchorPreset.y - 0.05f);
            buttonRect.anchorMax = new Vector2(anchorPreset.x + 0.2f, anchorPreset.y + 0.05f);
            buttonRect.sizeDelta = Vector2.zero;
            buttonRect.anchoredPosition = Vector2.zero;
            
            // Add button image
            Image buttonImage = buttonObject.AddComponent<Image>();
            buttonImage.color = new Color(0.3f, 0.3f, 0.3f, 0.8f);
            
            // Add button component
            Button button = buttonObject.AddComponent<Button>();
            button.targetGraphic = buttonImage;
            button.onClick.AddListener(onClickAction);
            
            // Add text to button
            GameObject textObject = new GameObject("Text");
            textObject.transform.SetParent(buttonObject.transform);
            
            RectTransform textRect = textObject.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.anchoredPosition = Vector2.zero;
            
            Text text = textObject.AddComponent<Text>();
            text.text = buttonText;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = 18;
            text.color = Color.white;
            text.alignment = TextAnchor.MiddleCenter;
        }

        #region Scene Management Methods

        /// <summary>
        /// Load a scene by name with optional transition effect
        /// </summary>
        /// <param name="sceneName">Name of the scene to load</param>
        /// <param name="useTransition">Whether to use a transition effect</param>
        public void LoadScene(string sceneName, bool useTransition = true)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("UIManager: Cannot load scene with null or empty name");
                return;
            }

            Debug.Log($"UIManager: Loading scene '{sceneName}'");

            // Check if scene exists in build settings
            if (!IsSceneInBuildSettings(sceneName))
            {
                Debug.LogError($"UIManager: Scene '{sceneName}' is not in build settings. Please add it to Build Settings.");
                return;
            }

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
        /// Check if a scene is included in build settings
        /// </summary>
        private bool IsSceneInBuildSettings(string sceneName)
        {
            // This is a simple check - in a real Unity project, you might want to use
            // UnityEditor.EditorBuildSettings.scenes to check build settings
            // For now, we'll check against known scene names
            string[] knownScenes = { "MainMenu", "GameScene", "GameOver" };
            return System.Array.Exists(knownScenes, scene => scene == sceneName);
        }

        /// <summary>
        /// Load a scene asynchronously with transition effect
        /// </summary>
        /// <param name="sceneName">Name of the scene to load</param>
        /// <returns>Coroutine enumerator</returns>
        private IEnumerator LoadSceneWithTransition(string sceneName)
        {
            // Wait for transition duration
            yield return new WaitForSeconds(transitionDuration / 2);

            // Load the scene
            SceneManager.LoadScene(sceneName);

            // Wait for transition to complete
            yield return new WaitForSeconds(transitionDuration / 2);

            Debug.Log($"UIManager: Scene '{sceneName}' loaded with transition");
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
                Debug.LogError("UIManager: Cannot load scene with null or empty name");
                yield break;
            }

            Debug.Log($"UIManager: Loading scene '{sceneName}' asynchronously");

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

            Debug.Log($"UIManager: Scene '{sceneName}' loaded asynchronously");
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

        #endregion
    }
}

