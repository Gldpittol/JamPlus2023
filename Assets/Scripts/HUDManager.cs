using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timeText;

    [SerializeField] private Image finalPanel;
    [SerializeField] private TextMeshProUGUI winText;
    [SerializeField] private TextMeshProUGUI loseText;

    private void Awake()
    {
        Instance = this;
        UpdateScoreText(0);
    }

    private void Update()
    {
        if (winText.gameObject.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GameManager.Instance.LoadNextScene(true);
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                GameManager.Instance.LoadNextScene(false);
            }
        }

        if (loseText.gameObject.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                GameManager.Instance.LoadNextScene(false);
            }
        }

    }

    public void UpdateScoreText(float score)
    {
        scoreText.text = "Score: " + score.ToString("F0");
    }
    
    public void UpdateTimeText(float time)
    {
        timeText.text = "Time Left: " + time.ToString("F0") + "s";
    }

    public void EnableFinalText(bool isWin)
    {
        if (finalPanel.enabled) return;
        if (isWin)
        {
            winText.gameObject.SetActive(true);
        }
        else
        {
            loseText.gameObject.SetActive(true);
        }

        finalPanel.enabled = true;
    }
}
