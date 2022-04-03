using UnityEngine;

public class TargetController : MonoBehaviour
{
    #region Variables

    [SerializeField] private Transform posteEsquerdo, trave, posteDireito, baliza;

    private GameState gameState;

    private InputManager inputManager;
    private Camera mainCamera;

    private BallController ballController;

    #endregion Variables

    #region MonoBehaviour

    private void Awake()
    {
        GameManager.OnGameStateChanged += OnGameStateChanged;

        inputManager = InputManager.Instance;
        mainCamera = Camera.main;
        ballController = FindObjectOfType<BallController>();
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnEnable()
    {
        inputManager.OnStartTouch += MoveWithInput;
    }

    private void OnDisable()
    {
        inputManager.OnStartTouch -= MoveWithInput;
    }

    #endregion MonoBehaviour

    #region GameState

    private void OnGameStateChanged(GameState newState)
    {
        gameState = newState;
    }

    #endregion GameState

    #region Input Methods

    private void MoveWithInput(Vector2 screenPosition)
    {
        Vector3 worldCoordinates = mainCamera.ScreenToWorldPoint(screenPosition);
        worldCoordinates.x = transform.position.x; 
        worldCoordinates.z = transform.position.z;

        if(worldCoordinates.y < baliza.position.y || worldCoordinates.y > trave.position.y) return;

        transform.position = worldCoordinates;
        ballController.UpdateLineRenderer();
    }

    #endregion Input Methods
}
