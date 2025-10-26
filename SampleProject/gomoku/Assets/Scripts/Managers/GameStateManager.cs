using System;
using UnityEngine;

public enum GameStateEnum
{
    MainMenu,
    Playing,
    Paused,
    GameOver
}

public class GameStateManager : MonoBehaviour
{
    private static GameStateManager instance;
    public GameStateEnum CurrentState { get; private set; }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            CurrentState = GameStateEnum.MainMenu;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static GameStateManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject managerObject = new GameObject("GameStateManager");
                instance = managerObject.AddComponent<GameStateManager>();
                DontDestroyOnLoad(managerObject);
            }
            return instance;
        }
    }

    public void SetState(GameStateEnum newState)
    {
        CurrentState = newState;
    }

    public void ResumeGame()
    {
        if (CurrentState == GameStateEnum.Paused)
        {
            SetState(GameStateEnum.Playing);
            Time.timeScale = 1f;
            Debug.Log("Game resumed");
        }
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f; // Ensure normal time scale
        SetState(GameStateEnum.MainMenu);
        Debug.Log("Returned to main menu");
    }

    public GameStateEnum GetCurrentState()
    {
       return CurrentState;

    }

    public void PauseGame()
    {
        if (CurrentState == GameStateEnum.Playing)
        {
            SetState(GameStateEnum.Paused);
            Time.timeScale = 0f;
            Debug.Log("Game paused");
        }
    }

    public bool IsInState(GameStateEnum testState)
    {
        return CurrentState == testState;
    }
}