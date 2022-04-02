using UnityEngine;

public class TargetController : MonoBehaviour
{
    [SerializeField]
    Transform posteEsquerdo, trave, posteDireito, baliza;

    private InputManager inputManager;
    private Camera mainCamera;

    private BallController ballController;
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

    private void OnGameStateChanged(GameState newState)
    {
        gameState = newState;
    }

    private void OnEnable()
    {
        inputManager.OnStartTouch += Move;
    }

    private void OnDisable()
    {
        inputManager.OnStartTouch -= Move;
    }

    private void Move(Vector2 screenPosition)
    {
        if(gameState != GameState.Idle) return;
        
        Vector3 worldCoordinates = mainCamera.ScreenToWorldPoint(screenPosition);
        worldCoordinates.x = transform.position.x; 
        worldCoordinates.z = transform.position.z;

        if(worldCoordinates.y < baliza.position.y || worldCoordinates.y > trave.position.y) return;

        transform.position = worldCoordinates;
        ballController.UpdateLineRenderer();
    }
}
