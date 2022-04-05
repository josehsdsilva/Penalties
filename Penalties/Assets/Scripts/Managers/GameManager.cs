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

    private int attempts, scored, streak;

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
            case GameState.OnSettings:
                break;
            case GameState.GoingToShoot:
                break;
            case GameState.KeeperReady:
                break;
            case GameState.Shooting:
                break;
            case GameState.Scored:
                OnScored();
                break;
            case GameState.Saved:
                OnSaved();
                break;
            case GameState.Reset:
                break;
            case GameState.Animation:
                break;
           default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnGameStateChanged?.Invoke(newState);
    }

    private async void OnScored()
    {
        attempts++;
        scored++;
        streak++;
        UIManager.Instance.UpdateUI(scored, attempts);
        if(streak >= 3)
        {
            streak = 0;
            UpdateGameState(GameState.Animation);
        }
        else
        {
            await GlobalTools.WaitForSeconds(2);
            UpdateGameState(GameState.Reset);
        }
    }

    private async void OnSaved()
    {
        attempts++;
        streak = 0;
        UIManager.Instance.UpdateUI(scored, attempts);
        await GlobalTools.WaitForSeconds(2);
        UpdateGameState(GameState.Reset);
    }

    #endregion GameState

    #region Helper Methods

    public void ResetStatistics()
    {
        attempts = 0;
        scored = 0;
        streak = 0;
        UIManager.Instance.UpdateUI(scored, attempts);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    #endregion Helper Methods
}

#region GameState Enum
public enum GameState
{
    Idle,
    OnSettings,
    GoingToShoot,
    KeeperReady,
    Shooting,
    Scored,
    Saved,
    Animation,
    Reset,
}

#endregion GameState Enum
