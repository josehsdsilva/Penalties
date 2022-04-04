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

    [SerializeField] private Transform settingsButton, resetButton, quitButton;
    [SerializeField] private Text scoredText, percentageText, attemptsText;

    private int attempts, scored;

    private bool settingsOpened = false, animating = false;

    private Vector2 resetButtonPosition, quitButtonPosition, settingsButtonPosition;

    #endregion Variables

    #region MonoBehaviour

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateUI();
        settingsButtonPosition = settingsButton.position;
        resetButtonPosition = resetButton.transform.position;
        quitButtonPosition = quitButton.transform.position;
    }

    #endregion MonoBehaviour

    #region Update UI

    public void Scored()
    {
        attempts++;
        scored++;
        UpdateUI();
    }

    public void Saved()
    {
        attempts++;
        UpdateUI();
    }

    public void Reset()
    {
        attempts = 0;
        scored = 0;
        UpdateUI();
    }

    private void UpdateUI()
    {
        scoredText.text = $"Scored - {scored}";
        percentageText.text = attempts == 0 ? "" : ((float)scored / (float)attempts * 100).ToString("0.00") + "%";
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
                quitButton.position = Vector3.Lerp(quitButtonPosition, resetButtonPosition, time);
                await Task.Delay(1);
            }
            quitButton.gameObject.SetActive(false);
            time = 0;
            while(time < 1)
            {
                time += Time.deltaTime * 2;
                resetButton.position = Vector3.Lerp(resetButtonPosition, settingsButtonPosition, time);
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
                resetButton.position = Vector3.Lerp(settingsButtonPosition, resetButtonPosition, time);
                await Task.Delay(1);
            }
            quitButton.gameObject.SetActive(true);
            time = 0;
            while(time < 1)
            {
                time += Time.deltaTime * 2;
                quitButton.position = Vector3.Lerp(resetButtonPosition, quitButtonPosition, time);
                await Task.Delay(1);
            }
        }

        animating = false;
        settingsOpened = !settingsOpened;
    }

    #endregion Update UI
}
