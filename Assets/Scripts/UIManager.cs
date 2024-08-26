using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameController gameController;
    [SerializeField] private TMP_Text scoreText, gameOverScoreText;
    [SerializeField] private GameObject gameUIPanel, gameOverPanel;
    
    private void Awake()
    {
        gameOverPanel.SetActive(false);
        gameController.OnGameOver += UIOnGameOver;
    }

    private void UIOnGameOver(object sender, EventArgs e)
    {
        gameUIPanel.SetActive(false);
        gameOverPanel.SetActive(true);
        gameOverScoreText.text = gameController.Score.ToString();
    }

    void LateUpdate()
    {
        scoreText.text = gameController.Score.ToString();
    }
}
