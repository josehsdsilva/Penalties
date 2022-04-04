using System;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables

    [SerializeField] private GameObject leftArrow, rightArrow;

    private GameState gameState;

    private InputManager inputManager;
    private Camera mainCamera;
    private BallController ballController;
    private Transform target;
    private Vector3 startPosition, targetPosition;
    private float shootingPower = 1;
    private int shootingDirection = -1;

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
        inputManager.OnEndTouch += SetArrowsVisible;
    }

    private void OnDisable()
    {
        inputManager.OnStartTouch -= PositionWithInput;
        inputManager.OnEndTouch -= SetArrowsVisible;
    }

    #endregion MonoBehaviour

    #region GameState

    private void OnGameStateChanged(GameState newState)
    {
        gameState = newState;

        if(gameState == GameState.Idle)
        {
            SetArrowsVisible(true);
        }
    }

    #endregion GameState

    #region Input Methods

    // Position the player
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

        SetArrowsVisible(false);

        worldCoordinates.y = transform.position.y;

        transform.position = worldCoordinates;
        UpdateTargetAndLine();
    }

    #endregion Input Methods

    #region Mechanics Methods
    public async void RunAndShoot()
    {
        if(gameState != GameState.Idle) return;

        SetArrowsVisible(false);
        ballController.HideTargetArrows();

        GameManager.Instance.UpdateGameState(GameState.GoingToShoot);

        ballController.ShowLineRenderer(false);

        // Wait until the keeper is ready
        await GlobalTools.WaitUntil(() => gameState == GameState.KeeperReady);

        // Run to the ball
        startPosition = transform.position;
        targetPosition = ballController.transform.position;
        float time = 0;
        while(time < 1)
        {
            time += Time.deltaTime * shootingPower;
            transform.position = GlobalTools.GetPointInLine(startPosition, targetPosition, time);
            await Task.Delay(1);
        }

        // Kick the ball
        ballController.Shoot(shootingPower);

        // Go back to starting position
        time = 0;
        while(time < 1)
        {
            time += Time.deltaTime / 2;
            transform.position = GlobalTools.GetPointInLine(targetPosition, startPosition, time);
            await Task.Delay(1);
        }
    }

    public void SetShootingPower(float power)
    {
        shootingPower = 1 + power * 2;
    }

    public void InvertShootingDirection()
    {
        shootingDirection *= -1;
        ballController.shootingDirection *= -1;
        UpdateTargetAndLine();
    }

    #endregion Mechanics Methods

    #region Helper Methods

    private void UpdateTargetAndLine()
    {
        target.position = new Vector3(shootingDirection * transform.position.x, target.position.y, target.position.z);
        ballController.UpdateLineRenderer();
    }

    private void SetArrowsVisible(bool visible)
    {
        leftArrow.SetActive(visible);
        rightArrow.SetActive(visible);
    }

    #endregion Helper Methods
}
