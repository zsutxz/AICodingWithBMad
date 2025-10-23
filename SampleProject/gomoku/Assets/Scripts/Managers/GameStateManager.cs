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
        throw new NotImplementedException();
    }

    public void ReturnToMainMenu()
    {
        throw new NotImplementedException();
    }

    public GameStateEnum GetCurrentState()
    {
       return CurrentState;

    }

    public void PauseGame()
    {
        throw new NotImplementedException();
    }

    public bool IsInState(GameStateEnum testState)
    {
        throw new NotImplementedException();
    }
}