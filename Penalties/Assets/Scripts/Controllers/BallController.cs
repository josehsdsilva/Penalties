using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BallController : MonoBehaviour
{
    #region Variables

    [SerializeField] private Transform posteEsquerdo, posteDireito, trave;
    private float goalOffet = 0.35f;

    private GameState gameState;

    private Camera mainCamera;

    private TargetController target;
    private LineRenderer lineRenderer;
    [HideInInspector] public int inverted = 1;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private SpriteRenderer spriteRenderer;
    private float shootingPower;

    #endregion Variables

    #region MonoBehaviour

    private void Awake()
    {
        GameManager.OnGameStateChanged += OnGameStateChanged;

        mainCamera = Camera.main;
        target = FindObjectOfType<TargetController>();
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

        if(collision.gameObject.tag == "Baliza")
        {
            ChangeSortingLayer(Vector3.zero, true);
            GameManager.Instance.UpdateGameState(GameState.Saved);
        }
        else if(collision.gameObject.tag == "Player")
        {
            if(shootingPower > 2f) // Pelo menos meia barra de força
            {
                if(Random.Range(0, 100) < 50 / 3 * shootingPower) // 50% de chance de ser golo com a força maxima
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
        targetPosition = GetRealTargetPosition(target.transform.position, power);
        target.gameObject.SetActive(false);

        ChangeSortingLayer(targetPosition);

        float time = 0;
        while(time < 1)
        {
            time += Time.deltaTime * power * 1.5f;
            if(gameState != GameState.Saved) transform.position = GetPointInLine(startPosition, targetPosition, time);
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
        int linePoints = 20;
        Vector3[] line = new Vector3[linePoints];
        for (int i = 0; i < linePoints; i++)
        {
            line[i] = GetPointInLine( transform.position, target.transform.position, (i + 1) / (float)linePoints);
        }
        lineRenderer.positionCount = linePoints;
        lineRenderer.SetPositions(line);
    }

    private Vector3 GetPointInLine(Vector3 startPos, Vector3 endPos, float t)
    {
        Vector3 midPos = new Vector3(startPos.x - inverted * (endPos.x - startPos.x), (startPos.y + endPos.y) / 2, (startPos.z + endPos.z) / 2);

        float x = ((1 - t) * (1 - t) * startPos.x) + (2 * t * (1 - t) * midPos.x) + (t * t * endPos.x);
        float y = ((1 - t) * (1 - t) * startPos.y) + (2 * t * (1 - t) * midPos.y) + (t * t * endPos.y);
        float z = startPos.z + (t * (endPos.z - startPos.z));

        return new Vector3(x, y, z);
    }

    private Vector3 GetRealTargetPosition(Vector3 targetPos, float power)
    {
        if(power == 1) return targetPos;

        float x = ((Random.Range(0, 1) * 2) - 1) * Random.Range(0, power / 6);
        float y = Random.Range(0, power / 6);
        Vector3 offset = new Vector3(x, y, targetPos.z);

        return targetPos + offset;
    }

    private void ChangeSortingLayer(Vector3 targetPos, bool saved = false)
    {
        if(saved) spriteRenderer.sortingOrder = 2;
        else spriteRenderer.sortingOrder = targetPos.y < trave.position.y && targetPos.x > posteEsquerdo.position.x + goalOffet && targetPos.x < posteDireito.position.x - goalOffet ? -1 : 2;
    }
    
    #endregion Helper Methods

}
