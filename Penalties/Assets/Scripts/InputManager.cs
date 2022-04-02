using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1)]
public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public delegate void StartTouchEvent(Vector2 position, float time);
    public event StartTouchEvent OnStartTouch;

    private TouchControls touchControls;

    private void Awake()
    {
        touchControls = new TouchControls();
        Instance = this;
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

    private void StartTouch(InputAction.CallbackContext context)
    {
        if(OnStartTouch != null)
        {
            OnStartTouch(touchControls.Touch.TouchPosition.ReadValue<Vector2>(), (float)context.startTime);
        }
    }
}
