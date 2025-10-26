using UnityEngine;
using UnityEngine.UI;

namespace Gomoku.UI
{
    /// <summary>
    /// Manages all user interface elements and interactions
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("UI Prefabs")]
        [SerializeField] private GameObject startButtonPrefab;
        [SerializeField] private GameObject pauseButtonPrefab;
        [SerializeField] private GameBoardController gameBoardPrefab;

        [Header("UI References")]
        private Button startButton;
        private Button pauseButton;
        private Text startButtonText;
        private GameObject pauseMenu;

        [Header("Text Settings")]
        [SerializeField] private string startGameText = "开始游戏";

        private GameBoardController m_gameBoardCtr;
        private ButtonHandler buttonHandler;
        private GameStateManager gameStateManager;

        private void Awake()
        {
            InitializeUI();
        }

        private void InitializeUI()
        {
            gameStateManager = GameStateManager.Instance;
            
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
    }
}

