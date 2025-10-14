using UnityEngine;
using UnityEngine.UI;

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
        // 动态创建StartButton
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
        
        // 确保引用已设置
        if (startButton == null || startButtonText == null)
        {
            Debug.LogError("UIManager: Failed to create or find required UI elements!");
            return;
        }

        // 设置开始按钮的文本
        SetStartButtonText(startGameText);

        // 可以在这里添加其他UI初始化逻辑
        Debug.Log("UIManager: UI initialized successfully");
    }

    /// <summary>
    /// 设置开始按钮的文本
    /// </summary>
    /// <param name="text">要显示的文本</param>
    public void SetStartButtonText(string text)
    {
        if (startButtonText != null)
        {
            startButtonText.text = text;
        }
    }

    /// <summary>
    /// 获取当前开始按钮的文本
    /// </summary>
    /// <returns>当前文本</returns>
    public string GetStartButtonText()
    {
        return startButtonText?.text ?? string.Empty;
    }
}