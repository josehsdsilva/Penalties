using System;
using System.Collections;
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
        touchControls.Touch.TouchPress.canceled += ctx => EndTouch(ctx);
    }

    private void OnGameStateChanged(GameState newState)
    {
        gameState = newState;
    }

    Coroutine touching;

    private void StartTouch(InputAction.CallbackContext context)
    {
        if(gameState != GameState.Idle) return;
        
        if(OnStartTouch != null)
        {
            touching = StartCoroutine("OnStartTouchCourotine");
        }
    }

    private void EndTouch(InputAction.CallbackContext context)
    {        
        if(OnStartTouch != null)
        {
            StopCoroutine(touching);
        }
    }

    IEnumerator OnStartTouchCourotine()
    {
        while(true)
        {
            OnStartTouch(touchControls.Touch.TouchPosition.ReadValue<Vector2>());
            yield return null;
        }
    }
}
