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

    #region Variables

    public delegate void StartTouchEvent(Vector2 position);
    public event StartTouchEvent OnStartTouch;
    public delegate void EndTouchEvent(bool visible);
    public event EndTouchEvent OnEndTouch;

    private TouchControls touchControls;

    private GameState gameState;

    private Coroutine touching;

    #endregion Variables

    #region MonoBehaviour

    private void Awake()
    {
        Instance = this;

        GameManager.OnGameStateChanged += OnGameStateChanged;

        touchControls = new TouchControls();
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

    #endregion MonoBehaviour

    #region GameState

    private void OnGameStateChanged(GameState newState)
    {
        gameState = newState;
    }

    #endregion GameState

    #region Input Methods

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
        OnEndTouch?.Invoke(true);
        if(OnStartTouch != null)
        {
            StopCoroutine(touching);
        }
    }

    private IEnumerator OnStartTouchCourotine()
    {
        while(true)
        {
            if(gameState != GameState.Idle)
            {
                StopCoroutine(touching);
                break;
            }
            OnStartTouch(touchControls.Touch.TouchPosition.ReadValue<Vector2>());
            yield return null;
        }
    }
    
    #endregion Input Methods
}
