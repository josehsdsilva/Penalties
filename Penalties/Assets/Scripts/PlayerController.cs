using System;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private InputManager inputManager;
    private Camera mainCamera;
    private BallController ballController;

    Vector3 startPosition;

    float power = 1;

    bool firstClick = true;

    GameState gameState;

    private void Awake()
    {
        inputManager = InputManager.Instance;
        mainCamera = Camera.main;
        ballController = FindObjectOfType<BallController>();
        GameManager.OnGameStateChanged += OnGameStateChanged;
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
    }

    public async void RunAndShoot(Vector3 position)
    {
        GameManager.Instance.UpdateGameState(GameState.GoingToShoot);

        await WaitUntil(() => gameState == GameState.KeeperReady);

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

    public static async Task WaitUntil(Func<bool> condition, int frequency = 25, int timeout = -1)
    {
        var waitTask = Task.Run(async () =>
        {
            while (!condition()) await Task.Delay(frequency);
        });

        if (waitTask != await Task.WhenAny(waitTask, 
                Task.Delay(timeout))) 
            throw new TimeoutException();
    }

    public void SetPower(float _power)
    {
        power = 1 + _power * 2;
    }
}
