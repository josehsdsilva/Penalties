using System;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1)]
public class InputManager : MonoBehaviour
{
    #region Singleton
    public static InputManager Instance { get; private set; }
    #endregion Singleton

    public delegate void StartTouchEvent(Vector2 position);
    public event StartTouchEvent OnStartTouch;

    private TouchControls touchControls;

    private GameState gameState;

    private void Awake()
    {
        touchControls = new TouchControls();
        Instance = this;
        GameManager.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnEnable()
    {
        touchControls.Enable();
    }

    private void OnDisable()
    {
        touchControls.Disable();
    }

    private void Start()
    {
        touchControls.Touch.TouchPress.started += ctx => StartTouch(ctx);
    }

    private void OnGameStateChanged(GameState newState)
    {
        gameState = newState;
    }

    private void StartTouch(InputAction.CallbackContext context)
    {
        if(gameState != GameState.Idle) return;
        
        if(OnStartTouch != null)
        {
            OnStartTouch(touchControls.Touch.TouchPosition.ReadValue<Vector2>());
        }
    }
}
