using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Singleton
    public static UIManager Instance { get; private set; }

    #endregion Singleton

    #region Variables

    [SerializeField] private RectTransform settingsButton, resetButton, quitButton;
    [SerializeField] private Text scoredText, percentageText, attemptsText;

    private bool settingsOpened = false;
    private bool animating = false;

    private Vector2 resetButtonPosition, quitButtonPosition, settingsButtonPosition;

    private int lastScreenWidth = 0;
    private int lastScreenHeight = 0;

    public delegate void ScreenSizeChangedEvent();
    public event ScreenSizeChangedEvent OnScreenSizeChangedEvent;

    #endregion Variables

    #region MonoBehaviour

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateUI();
        OnScreenSizeChangedEvent += OnScreenSizeChanged;
    }
 
    private void FixedUpdate()
    {
        if (lastScreenWidth != Screen.width || lastScreenHeight != Screen.height)
        {
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
            OnScreenSizeChangedEvent.Invoke();
        }
    }

    #endregion MonoBehaviour

    #region Update UI

    public void OnScreenSizeChanged()
    {
        settingsButtonPosition = settingsButton.anchoredPosition;
        resetButtonPosition = settingsButtonPosition + new Vector2(0, 30);
        quitButtonPosition = settingsButtonPosition + new Vector2(0, 60);

        if(settingsOpened)
        {
            resetButton.anchoredPosition = resetButtonPosition;
            quitButton.anchoredPosition = quitButtonPosition;
        }
        else
        {
            resetButton.anchoredPosition = settingsButtonPosition;
            quitButton.anchoredPosition = settingsButtonPosition;
        }
    }

    public void UpdateUI(int scored = 0, int attempts = 0)
    {
        scoredText.text = $"Scored - {scored}";
        percentageText.text = attempts == 0 ? "" : Math.Round(((double)scored / (double)attempts) * 100, 2).ToString() + "%";
        attemptsText.text = $"{attempts} - Attempts";
    }

    public async void ToggleSettings()
    {
        if(animating) return;

        animating = true;

        if(settingsOpened)
        {
            float time = 0;
            while(time < 1)
            {
                time += Time.deltaTime * 2;
                quitButton.anchoredPosition = Vector3.Lerp(quitButtonPosition, resetButtonPosition, time);
                await Task.Delay(1);
            }
            quitButton.gameObject.SetActive(false);
            time = 0;
            while(time < 1)
            {
                time += Time.deltaTime * 2;
                resetButton.anchoredPosition = Vector3.Lerp(resetButtonPosition, settingsButtonPosition, time);
                await Task.Delay(1);
            }
            resetButton.gameObject.SetActive(false);
            GameManager.Instance.UpdateGameState(GameState.Idle);
        }
        else
        {
            GameManager.Instance.UpdateGameState(GameState.OnSettings);
            resetButton.gameObject.SetActive(true);
            float time = 0;
            while(time < 1)
            {
                time += Time.deltaTime * 2;
                resetButton.anchoredPosition = Vector3.Lerp(settingsButtonPosition, resetButtonPosition, time);
                await Task.Delay(1);
            }
            quitButton.gameObject.SetActive(true);
            time = 0;
            while(time < 1)
            {
                time += Time.deltaTime * 2;
                quitButton.anchoredPosition = Vector3.Lerp(resetButtonPosition, quitButtonPosition, time);
                await Task.Delay(1);
            }
        }

        animating = false;
        settingsOpened = !settingsOpened;
    }

    #endregion Update UI
}
