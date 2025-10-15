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

        [Header("UI References")]
        private Button startButton;
        private Text startButtonText;

        [Header("Text Settings")]
        [SerializeField] private string startGameText = "开始游戏";

        private void Awake()
        {
            InitializeUI();
        }

        private void InitializeUI()
        {
            // Dynamically create StartButton
            if (startButtonPrefab != null)
            {
                GameObject buttonObject = Instantiate(startButtonPrefab, transform);
                startButton = buttonObject.GetComponent<Button>();
                // First try to get Text component directly on the same GameObject
                startButtonText = buttonObject.GetComponent<Text>();
                // If not found, then look for it in children
                if (startButtonText == null)
                {
                    startButtonText = buttonObject.GetComponentInChildren<Text>();
                }
            }

            // Ensure references are set
            if (startButton == null || startButtonText == null)
            {
                Debug.LogError("UIManager: Failed to create or find required UI elements!");
                return;
            }

            // Set start button text
            SetStartButtonText(startGameText);

            Debug.Log("UIManager: UI initialized successfully");
        }

        public void SetStartButtonText(string text)
        {
            if (startButtonText != null)
            {
                startButtonText.text = text;
            }
        }

        public string GetStartButtonText()
        {
            return startButtonText?.text ?? string.Empty;
        }

        /// <summary>
        /// Sets the active state of the UI Manager's root GameObject
        /// </summary>
        /// <param name="active">Whether to activate the UI</param>
        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}

