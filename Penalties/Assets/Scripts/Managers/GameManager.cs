using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton

    public static GameManager Instance { get; private set; }

    #endregion Singleton

    #region Variables

    public GameState gameState;

    public static event Action<GameState> OnGameStateChanged;

    #endregion Variables

    #region MonoBehaviour

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateGameState(GameState.Idle);
    }

    #endregion MonoBehaviour

    #region GameState

    public void UpdateGameState(GameState newState)
    {
        gameState = newState;

        switch (newState)
        {
            case GameState.Idle:
                break;
            case GameState.GoingToShoot:
                break;
            case GameState.KeeperReady:
                break;
            case GameState.Shooting:
                break;
            case GameState.Scored:
                UIManager.Instance.Scored();
                break;
            case GameState.Saved:
                UIManager.Instance.Saved();
                break;
            case GameState.Reset:
                break;
            case GameState.GameReset:
                UIManager.Instance.Reset();
                break;
           default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnGameStateChanged?.Invoke(newState);
    }

    public void NextPenaltie()
    {
        UpdateGameState(GameState.Reset);
    }

    public void ResetGame()
    {
        UpdateGameState(GameState.GameReset);
        UpdateGameState(GameState.Reset);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    #endregion GameState
}

#region GameState Enum
public enum GameState
{
    Idle,
    GoingToShoot,
    KeeperReady,
    Shooting,
    Scored,
    Saved,
    Reset,
    GameReset
}

#endregion GameState Enum
