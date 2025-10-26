using UnityEngine;

namespace Gomoku.UI.MainMenu
{
    public class MainMenuScreen : MonoBehaviour
    {
        void Start()
        {
            Gomoku.Systems.GameStateManager.Instance.SetState(Gomoku.Systems.GameStateEnum.MainMenu);
        }

        public void StartGame()
        {
            Gomoku.Systems.GameStateManager.Instance.SetState(Gomoku.Systems.GameStateEnum.Playing);
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
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
}