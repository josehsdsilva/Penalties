using UnityEngine;

public class TargetController : MonoBehaviour
{
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private Camera mainCamera;

    private void OnEnable()
    {
        inputManager.OnStartTouch += Move;
    }

    private void OnDisable()
    {
        inputManager.OnStartTouch -= Move;
    }

    public void Move(Vector2 screenPosition, float time)
    {
        Vector3 worldCoordinates = mainCamera.ScreenToWorldPoint(screenPosition);
        worldCoordinates.z = transform.position.z;
        if(worldCoordinates.x > -2.5 && worldCoordinates.x < 2.5)
        {
            if(worldCoordinates.y > 2 && worldCoordinates.y < 4.5)
            {
                transform.position = worldCoordinates;
            }
        }
    }
}
