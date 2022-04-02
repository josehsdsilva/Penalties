using System;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private InputManager inputManager;
    private Camera mainCamera;
    private BallController ballController;
    private Transform target;

    Vector3 startPosition;

    float power = 1;

    bool firstClick = true;

    GameState gameState;

    int inverted = -1;

    private void Awake()
    {
        inputManager = InputManager.Instance;
        mainCamera = Camera.main;
        ballController = FindObjectOfType<BallController>();
        target = FindObjectOfType<TargetController>().transform;
        GameManager.OnGameStateChanged += OnGameStateChanged;
        UpdateTargetAndLine();
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnEnable()
    {
        inputManager.OnStartTouch += Position;
    }

    private void OnDisable()
    {
        inputManager.OnStartTouch -= Position;
    }

    
    private void OnGameStateChanged(GameState newState)
    {
        gameState = newState;
    }

    private void Position(Vector2 screenPosition)
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

    private void UpdateTargetAndLine()
    {
        target.position = new Vector3(inverted * transform.position.x, target.position.y, target.position.z);
        ballController.UpdateLineRenderer();
    }
}
