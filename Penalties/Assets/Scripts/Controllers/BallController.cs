using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BallController : MonoBehaviour
{
    #region Variables

    [SerializeField] private Transform leftPost, rightPost, crossbar;
    private float goalOffset = 0.35f;

    private GameState gameState;

    private Camera mainCamera;

    private TargetController target;
    private LineRenderer lineRenderer;
    [HideInInspector] public int shootingDirection = -1;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private SpriteRenderer spriteRenderer;

    
    private KeeperController keeperController;
    private float shootingPower;

    #endregion Variables

    #region MonoBehaviour

    private void Awake()
    {
        GameManager.OnGameStateChanged += OnGameStateChanged;

        mainCamera = Camera.main;
        target = FindObjectOfType<TargetController>();
        keeperController = FindObjectOfType<KeeperController>();
        lineRenderer = GetComponent<LineRenderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        UpdateLineRenderer();
        startPosition = transform.position;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState newState)
    {
        gameState = newState;

        switch (gameState)
        {
            case GameState.Saved:
                spriteRenderer.color = Color.red;
                break;
            case GameState.Reset:
                spriteRenderer.color = Color.white;
                break;
        }
    }

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(gameState != GameState.Shooting) return;

        if(collision.gameObject.tag == "Goal") // if the ball hits the woodwork
        {
            ChangeSortingLayer(Vector3.zero, true);
            GameManager.Instance.UpdateGameState(GameState.Saved);
        }
        else if(collision.gameObject.tag == "Player") // if the ball hits the player
        {
            if(shootingPower > 2f) // If the shooting power is at least at 50%
            {
                if(Random.Range(0, 100) < 50 / 3 * shootingPower) // the keeper have a 50% chance to defend the ball if the shooting power is at 100%
                {
                    GameManager.Instance.UpdateGameState(GameState.Scored);
                    return;
                }
            }
            GameManager.Instance.UpdateGameState(GameState.Saved);
            ChangeSortingLayer(Vector3.zero, true);
        }
    }

    #endregion Unity Methods

    #region Mechanics Methods

    public async void Shoot(float power)
    {
        shootingPower = power;
        GameManager.Instance.UpdateGameState(GameState.Shooting);
        targetPosition = GetRealTargetPosition(target.transform.position);
        target.gameObject.SetActive(false);

        keeperController.GoingToShoot(power, targetPosition);

        ChangeSortingLayer(targetPosition);

        float time = 0;
        while(time < 1)
        {
            time += Time.deltaTime * power * 1.5f;
            if(gameState != GameState.Saved) transform.position = GlobalTools.GetPointInLine(startPosition, targetPosition, time, shootingDirection);
            await Task.Delay(1);
        }

        if(gameState != GameState.Saved && gameState != GameState.Scored)
        {
            if(spriteRenderer.sortingOrder == 2)
            {
                GameManager.Instance.UpdateGameState(GameState.Saved);
            }
            else
            {
                GameManager.Instance.UpdateGameState(GameState.Scored);
            }
        }

        await GlobalTools.WaitUntil(() => gameState == GameState.Reset);
        
        target.gameObject.SetActive(true);
        target.SetArrowsVisible(true);
        transform.position = startPosition;
        ShowLineRenderer(true);
    }

    #endregion Mechanics Methods

    #region Helper Methods

    public void ShowLineRenderer(bool show)
    {
        lineRenderer.enabled = show;
    }

    public void UpdateLineRenderer()
    {
        int linePoints = 40;
        Vector3[] line = new Vector3[linePoints];
        for (int i = 0; i < linePoints; i++)
        {
            line[i] = GlobalTools.GetPointInLine(transform.position, target.transform.position, (i + 1) / (float)linePoints, shootingDirection);
        }
        lineRenderer.positionCount = linePoints;
        lineRenderer.SetPositions(line);
    }

    private Vector3 GetRealTargetPosition(Vector3 targetPos)
    {
        if(shootingPower == 1) return targetPos;

        // Setting how much the ball will offset based on shooting power
        float x = ((Random.Range(0, 1) * 2) - 1) * Random.Range(0, shootingPower / 6);
        float y = Random.Range(0, shootingPower / 6);
        Vector3 offset = new Vector3(x, y, targetPos.z);

        return targetPos + offset;
    }

    private void ChangeSortingLayer(Vector3 targetPos, bool saved = false)
    {
        if(saved) spriteRenderer.sortingOrder = 2;
        else spriteRenderer.sortingOrder = targetPos.y < crossbar.position.y && targetPos.x > leftPost.position.x + goalOffset && targetPos.x < rightPost.position.x - goalOffset ? -1 : 2;
    }


    public void HideTargetArrows()
    {
        target.SetArrowsVisible(false);
    }
    
    #endregion Helper Methods


}
