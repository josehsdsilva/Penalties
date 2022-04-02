using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BallController : MonoBehaviour
{
    private InputManager inputManager;
    private Camera mainCamera;
    private PlayerController player;
    private TargetController target;

    Vector3 startPosition;
    Vector3 targetPosition;

    private void Awake()
    {
        inputManager = InputManager.Instance;
        mainCamera = Camera.main;
        player = FindObjectOfType<PlayerController>();
        target = FindObjectOfType<TargetController>();
    }

    private void OnEnable()
    {
        inputManager.OnStartTouch += Clicked;
    }

    private void OnDisable()
    {
        inputManager.OnStartTouch -= Clicked;
    }

    private void Clicked(Vector2 screenPosition)
    {
        Vector3 worldCoordinates = mainCamera.ScreenToWorldPoint(screenPosition);
        worldCoordinates.z = transform.position.z;

        if(IsInsideCircle(worldCoordinates.x, worldCoordinates.y))
        {
            player.RunAndShoot(transform.position);
        }
    }

    bool IsInsideCircle( float x, float y )
    {  
        float dx = Mathf.Abs(x - transform.position.x);
        float dy = Mathf.Abs(y - transform.position.y);
        return ( dx * dx + dy * dy <= transform.localScale.x * transform.localScale.y );
    }

    public async void Shoot(float power)
    {
        GameManager.Instance.UpdateGameState(GameState.Shooting);
        startPosition = transform.position;
        targetPosition = target.transform.position;
        target.gameObject.SetActive(false);
        float time = 0;
        while(time < 1)
        {
            time += Time.deltaTime * power * 1.5f;
            transform.position = Vector3.Lerp(startPosition, targetPosition, time);
            await Task.Delay(1);
        }

        await Task.Delay(1000);
        target.gameObject.SetActive(true);
        transform.position = startPosition;
    }
}
