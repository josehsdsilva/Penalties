using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class KeeperController : MonoBehaviour
{
    #region Variables

    [SerializeField] private Transform leftPost, rightPost;

    private GameState gameState;

    private Vector3 startPosition;
    private Vector3 targetPosition;

    private int side = 0;

    private bool onIdleAnimation = false;

    #endregion Variables

    #region MonoBehaviour

    private void Awake()
    {
        GameManager.OnGameStateChanged += OnGameStateChanged;

        startPosition = transform.position;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= OnGameStateChanged;
    }

    #endregion MonoBehaviour

    #region GameState

    private void OnGameStateChanged(GameState newState)
    {
        gameState = newState;
        if(gameState == GameState.Idle && !onIdleAnimation) IdleMovement();
        if(gameState == GameState.Shooting) TryToDefend();
    }

    #endregion GameState

    #region Mechanics Methods

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

    public void GoingToShoot(float power, Vector3 position)
    {
        GetDefendingPosition(power, position);
    }

    private async void TryToDefend()
    {
        

        float time = 0;
        while(time < 1)
        {
            time += Time.deltaTime * 3;
            if(gameState != GameState.Saved) transform.position = Vector3.Lerp(startPosition, targetPosition, time);
            await Task.Delay(1);
        }

        await GlobalTools.WaitUntil(() => gameState == GameState.Reset);

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

    #endregion Mechanics Methods

    #region Helper Methods

    private void UpdateTargetPosition()
    {
        targetPosition = side == 0 ? new Vector3(leftPost.position.x + 0.5f, transform.position.y, transform.position.z) : new Vector3(rightPost.position.x - 0.5f, transform.position.y, transform.position.z);
    }

    private void GetDefendingPosition(float shootingPower, Vector3 position)
    {
        int randomRange = Random.Range(0, 300) + 1;
        if(randomRange < shootingPower * 100)
        {
            randomRange = Random.Range(0, 5) + 1;
            float randomX = leftPost.position.x + (randomRange * (rightPost.position.x / 3));
            targetPosition = new Vector3(randomX, transform.position.y, transform.position.z);
        }
        else
        {
            targetPosition = new Vector3(position.x, transform.position.y, transform.position.z);
        }
        
    }

    private void CheckState()
    {
        if(gameState == GameState.Idle) IdleMovement();
        else if(gameState == GameState.GoingToShoot) GameManager.Instance.UpdateGameState(GameState.KeeperReady);
    }

    #endregion Helper Methods
}
