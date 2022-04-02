using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [SerializeField]
    private Transform posteEsquerdo, posteDireito, trave;
    private InputManager inputManager;
    private Camera mainCamera;
    private PlayerController player;
    private TargetController target;
    private LineRenderer lineRenderer;

    Vector3 startPosition;
    Vector3 targetPosition;
    [HideInInspector]
    public int inverted = 1;

    GameState gameState;

    private void Awake()
    {
        inputManager = InputManager.Instance;
        mainCamera = Camera.main;
        player = FindObjectOfType<PlayerController>();
        target = FindObjectOfType<TargetController>();
        lineRenderer = GetComponent<LineRenderer>();
        UpdateLineRenderer();
        startPosition = transform.position;
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
        inputManager.OnStartTouch += Clicked;
    }

    private void OnDisable()
    {
        inputManager.OnStartTouch -= Clicked;
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

    Vector3 GetPointInLine(Vector3 startPos, Vector3 endPos, float t)
    {
        Vector3 midPos = new Vector3(startPos.x - inverted * (endPos.x - startPos.x), (startPos.y + endPos.y) / 2, (startPos.z + endPos.z) / 2);

        float x = ((1 - t) * (1 - t) * startPos.x) + (2 * t * (1 - t) * midPos.x) + (t * t * endPos.x);
        float y = ((1 - t) * (1 - t) * startPos.y) + (2 * t * (1 - t) * midPos.y) + (t * t * endPos.y);
        float z = startPos.z + (t * (endPos.z - startPos.z));

        return new Vector3(x, y, z);
    }

    private void Clicked(Vector2 screenPosition)
    {
        Vector3 worldCoordinates = mainCamera.ScreenToWorldPoint(screenPosition);
        worldCoordinates.z = transform.position.z;

        if(IsInsideCircle(worldCoordinates.x, worldCoordinates.y))
        {
            if(gameState == GameState.GoingToShoot) return;
            GameManager.Instance.UpdateGameState(GameState.GoingToShoot);

            player.RunAndShoot(transform.position);
            lineRenderer.enabled = false;
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
        targetPosition = GetRealTargetPosition(target.transform.position, power);
        target.gameObject.SetActive(false);
        float time = 0;
        while(time < 1)
        {
            time += Time.deltaTime * power * 1.5f;
            transform.position = GetPointInLine(startPosition, targetPosition, time);
            await Task.Delay(1);
        }

        await GlobalTools.WaitUntil(() => gameState == GameState.Reset);
        
        target.gameObject.SetActive(true);
        transform.position = startPosition;
        lineRenderer.enabled = true;
    }

    private Vector3 GetRealTargetPosition(Vector3 targetPos, float power)
    {
        if(power == 1) return targetPos;

        float x = ((Random.Range(0, 1) * 2) - 1) * Random.Range(0, power / 6);
        float y = Random.Range(0, power / 6);
        Vector3 offset = new Vector3(x, y, targetPos.y + y < trave.position.y ? targetPos.x + x > posteEsquerdo.position.x ? targetPos.x + x < posteEsquerdo.position.x ? 0 : 2 : 2 : 2);

        return targetPos + offset;
    }
}
