using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Singleton
    public static UIManager Instance { get; private set; }

    #endregion Singleton

    #region Variables

    [SerializeField] private GameObject statisticsPanel;
    [SerializeField] private Text statisticsText;

    private int goalAttempts, scored;

    #endregion Variables

    #region MonoBehaviour

    private void Awake()
    {
        Instance = this;
    }

    #endregion MonoBehaviour

    #region Update UI

    public void Scored()
    {
        goalAttempts++;
        scored++;
        UpdateUI();
    }

    public void Saved()
    {
        goalAttempts++;
        UpdateUI();
    }

    public void Reset()
    {
        goalAttempts = 0;
        scored = 0;
        UpdateUI();
    }

    private async void UpdateUI()
    {
        statisticsPanel.SetActive(true);
        double percentage = Math.Round(((double)scored / (double)goalAttempts) * 100, 2);
        statisticsText.text = $"Golos Marcados: {scored} / {goalAttempts} \nPercentagem de acerto: {percentage}%";
    }

    #endregion Update UI
}
