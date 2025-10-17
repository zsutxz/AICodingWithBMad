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
        [SerializeField] private GameBoardController gameBoardPrefab;

        [Header("UI References")]
        private Button startButton;
        private Text startButtonText;

        [Header("Text Settings")]
        [SerializeField] private string startGameText = "开始游戏";

        GameBoardController m_gameBoardCtr;
        private void Awake()
        {
            InitializeUI();
        }

        private void InitializeUI()
        {
            if(gameBoardPrefab != null)
            {
                m_gameBoardCtr = Instantiate<GameBoardController>(gameBoardPrefab, transform);
            }

            // Dynamically create StartButton
            if (startButtonPrefab != null)
            {
                GameObject buttonObject = Instantiate(startButtonPrefab, transform);
                startButton = buttonObject.GetComponent<Button>();
               // First try to get Text component directly on the same GameObject

            }

            Debug.Log("UIManager: UI initialized successfully");
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

