using UnityEngine;

public class TargetController : MonoBehaviour
{
    [SerializeField]
    Transform posteEsquerdo, trave, posteDireito, baliza;

    private InputManager inputManager;
    private Camera mainCamera;

    private void Awake()
    {
        inputManager = InputManager.Instance;
        mainCamera = Camera.main;
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
        Vector3 worldCoordinates = mainCamera.ScreenToWorldPoint(screenPosition);
        worldCoordinates.z = transform.position.z;

        if(worldCoordinates.x < posteEsquerdo.position.x || worldCoordinates.x > posteDireito.position.x) return;

        if(worldCoordinates.y < baliza.position.y || worldCoordinates.y > trave.position.y) return;

        transform.position = worldCoordinates;
    }
}
