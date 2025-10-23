using UnityEngine;
using UnityEngine.SceneManagement;
using Gomoku.UI;

public class MainMenuScreen : MonoBehaviour
{
    void Start()
    {
        GameStateManager.Instance.SetState(GameStateEnum.MainMenu);
    }

    public void StartGame()
    {
        GameStateManager.Instance.SetState(GameStateEnum.Playing);
        SceneLoader.LoadScene("GameScene");
    }

    public void OpenSettings()
    {
        //UIManager.Instance.ShowSettingsPanel();
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}