using System;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables

    private GameState gameState;

    private InputManager inputManager;
    private Camera mainCamera;
    private BallController ballController;
    private Transform target;
    private Vector3 startPosition;
    private float power = 1;
    private int inverted = -1;

    private bool firstClick = true;

    #endregion Variables

    #region MonoBehaviour
    private void Awake()
    {
        GameManager.OnGameStateChanged += OnGameStateChanged;

        inputManager = InputManager.Instance;
        mainCamera = Camera.main;
        ballController = FindObjectOfType<BallController>();
        target = FindObjectOfType<TargetController>().transform;

        UpdateTargetAndLine();
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnEnable()
    {
        inputManager.OnStartTouch += PositionWithInput;
    }

    private void OnDisable()
    {
        inputManager.OnStartTouch -= PositionWithInput;
    }

    #endregion MonoBehaviour

    #region GameState

    private void OnGameStateChanged(GameState newState)
    {
        gameState = newState;
    }

    #endregion GameState

    #region Input Methods

    private void PositionWithInput(Vector2 screenPosition)
    {
        if(firstClick)
        {
            firstClick = false;
            return;
        }

        Vector3 worldCoordinates = mainCamera.ScreenToWorldPoint(screenPosition);
        worldCoordinates.z = transform.position.z;

        if(worldCoordinates.y < transform.position.y || worldCoordinates.y > transform.position.y + 2) return;

        worldCoordinates.y = transform.position.y;

        transform.position = worldCoordinates;
        UpdateTargetAndLine();
    }

    #endregion Input Methods

    #region Mechanics Methods
    public async void RunAndShoot(Vector3 position)
    {
        await GlobalTools.WaitUntil(() => gameState == GameState.KeeperReady);

        startPosition = transform.position;
        float time = 0;
        while(time < 1)
        {
            time += Time.deltaTime * power;
            transform.position = Vector3.Lerp(startPosition, position, time);
            await Task.Delay(1);
        }
        ballController.Shoot(power);
        time = 0;
        while(time < 1)
        {
            time += Time.deltaTime / 2;
            transform.position = Vector3.Lerp(position, startPosition, time);
            await Task.Delay(1);
        }
    }

    public void SetPower(float _power)
    {
        power = 1 + _power * 2;
    }

    public void ToggleInverted()
    {
        inverted *= -1;
        ballController.inverted *= -1;
        UpdateTargetAndLine();
    }

    #endregion Mechanics Methods

    #region Helper Methods

    private void UpdateTargetAndLine()
    {
        target.position = new Vector3(inverted * transform.position.x, target.position.y, target.position.z);
        ballController.UpdateLineRenderer();
    }

    #endregion Helper Methods
}
