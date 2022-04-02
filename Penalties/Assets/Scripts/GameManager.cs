using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState gameState;

    public static event Action<GameState> OnGameStateChanged;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateGameState(GameState.Idle);
    }

    public void UpdateGameState(GameState newState)
    {
        gameState = newState;

        // switch (newState)
        // {
        //     case GameState.Idle:
        //         break;
        //     case GameState.GoingToShoot:
        //     case GameState.GoingToShoot:
        //         break;
        //     case GameState.Shooting:
        //         break;
        //     case GameState.Scored:
        //         break;
        //     case GameState.Saved:
        //         break;
        //    default:
        //         throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        // }

        OnGameStateChanged?.Invoke(newState);
    }
}


public enum GameState
{
    Idle,
    GoingToShoot,
    KeeperReady,
    Shooting,
    Scored,
    Saved,
    Reset
}
