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

    private InputManager inputManager;
    private Camera mainCamera;

    private PlayerController player;
    private TargetController target;
    private LineRenderer lineRenderer;
    [HideInInspector] public int inverted = 1;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private SpriteRenderer spriteRenderer;

    #endregion Variables

    #region MonoBehaviour

    private void Awake()
    {
        GameManager.OnGameStateChanged += OnGameStateChanged;

        inputManager = InputManager.Instance;
        mainCamera = Camera.main;
        player = FindObjectOfType<PlayerController>();
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

    private void OnEnable()
    {
        inputManager.OnStartTouch += ClickedWithInput;
    }

    private void OnDisable()
    {
        inputManager.OnStartTouch -= ClickedWithInput;
    }

    private void OnGameStateChanged(GameState newState)
    {
        gameState = newState;
    }

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(gameState != GameState.Shooting) return;
        if(collision.gameObject.tag == "Player")
        {
            ChangeSortingLayer(Vector3.zero, true);
            GameManager.Instance.UpdateGameState(GameState.Saved);
        }
    }

    #endregion Unity Methods

    #region Input Methods

    private void ClickedWithInput(Vector2 screenPosition)
    {
        Vector3 worldCoordinates = mainCamera.ScreenToWorldPoint(screenPosition);
        worldCoordinates.z = transform.position.z;

        if(IsInsideCircle(worldCoordinates.x, worldCoordinates.y))
        {
            GameManager.Instance.UpdateGameState(GameState.GoingToShoot);

            player.RunAndShoot(transform.position);
            lineRenderer.enabled = false;
        }
    }

    #endregion Input Methods
    
    #region Mechanics Methods

    public async void Shoot(float power)
    {
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

        if(gameState != GameState.Saved)
        {
            if(spriteRenderer.sortingOrder == 1)
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
        lineRenderer.enabled = true;
    }

    #endregion Mechanics Methods

    #region Helper Methods

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
    private bool IsInsideCircle( float x, float y )
    {  
        float dx = Mathf.Abs(x - transform.position.x);
        float dy = Mathf.Abs(y - transform.position.y);
        return ( dx * dx + dy * dy <= transform.localScale.x * transform.localScale.y );
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
        if(saved) spriteRenderer.sortingOrder = 1;
        else spriteRenderer.sortingOrder = targetPos.y < trave.position.y && targetPos.x > posteEsquerdo.position.x + goalOffet && targetPos.x < posteDireito.position.x - goalOffet ? -1 : 1;
    }
    
    #endregion Helper Methods

}
