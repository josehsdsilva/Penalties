using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class KeeperController : MonoBehaviour
{
    [SerializeField]
    Transform posteEsquerdo, posteDireito;

    Vector3 startPosition;
    Vector3 targetPosition;

    int side = 0;

    private GameState gameState;
    bool onIdleAnimation = false;

    private void Awake()
    {
        startPosition = transform.position;
        GameManager.OnGameStateChanged += OnGameStateChanged;
        IdleMovement();
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState newState)
    {
        gameState = newState;
        if(gameState == GameState.Idle && !onIdleAnimation) IdleMovement();
        if(gameState == GameState.Shooting) TryToDefend();
    }

    private async void IdleMovement()
    {
        onIdleAnimation = true;
        UpdateTargetPosition();

        float scale = 1;

        float time = 0;
        while(time < 1)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, targetPosition, time);
            await Task.Delay(1);
        }
        time = 0;
        while(time < 1)
        {
            scale = gameState == GameState.Idle ? 1 : 2;
            time += Time.deltaTime * scale;
            transform.position = Vector3.Lerp(targetPosition, startPosition, time);
            await Task.Delay(1);
        }

        side = side == 0 ? 1 : 0;

        onIdleAnimation = false;
        CheckState();
    }
    private async void TryToDefend()
    {
        GetRandomPosition();

        float time = 0;
        while(time < 1)
        {
            time += Time.deltaTime * 2;
            transform.position = Vector3.Lerp(startPosition, targetPosition, time);
            await Task.Delay(1);
        }

        GameManager.Instance.UpdateGameState(GameState.Scored);

        GoBackToStartPosition();
    }

    private async void GoBackToStartPosition()
    {
        targetPosition = transform.position;
        float time = 0;
        while(time < 1)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(targetPosition, startPosition, time);
            await Task.Delay(1);
        }

        GameManager.Instance.UpdateGameState(GameState.Idle);
    }

    private void UpdateTargetPosition()
    {
        targetPosition = side == 0 ? new Vector3(posteEsquerdo.position.x + 0.5f, transform.position.y, transform.position.z) : new Vector3(posteDireito.position.x - 0.5f, transform.position.y, transform.position.z);
    }

    private void GetRandomPosition()
    {
        float randomX = Random.Range(0, (posteDireito.position.x - 0.5f) * 2);
        targetPosition = new Vector3(randomX - (posteDireito.transform.position.x - 0.5f), transform.position.y, transform.position.z);
    }

    private void CheckState()
    {
        if(gameState == GameState.Idle) IdleMovement();
        else if(gameState == GameState.GoingToShoot) GameManager.Instance.UpdateGameState(GameState.KeeperReady);
    }
}
