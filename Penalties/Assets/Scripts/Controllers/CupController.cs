using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CupController : MonoBehaviour
{
    #region Variables

    GameState gameState;

    [SerializeField] private ParticleSystem confetti;
    private SpriteRenderer spriteRenderer;
    float scale;

    #endregion Variables

    #region MonoBehaviour

    private void Awake()
    {
        GameManager.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= OnGameStateChanged;
    }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        scale = transform.localScale.x;
    }

    #endregion MonoBehaviour

    #region GameState

    private void OnGameStateChanged(GameState newState)
    {
        if(newState == GameState.Animation)
        {
            PlayAnimation();
        }
    }

    #endregion GameState

    #region Animation Methods

    private async void PlayAnimation()
    {
        spriteRenderer.enabled = true;

        confetti.gameObject.SetActive(true);
        confetti.Play();

        float time = 0;
        while(time < 1)
        {
            time += Time.deltaTime * 4;
            transform.localScale = new Vector3(scale * time, scale * time, 1);
            await Task.Delay(1);
        }
        

        await GlobalTools.WaitForSeconds(3);


        time = 1;
        while(time > 0)
        {
            time -= Time.deltaTime * 5;
            transform.localScale = new Vector3(scale * time, scale * time, 1);
            await Task.Delay(1);
        }
        
        confetti.Stop();
        confetti.gameObject.SetActive(false);
        spriteRenderer.enabled = false;

        GameManager.Instance.UpdateGameState(GameState.Reset);
    }

    #endregion Animation Methods
}
