using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timeText;

    private void Awake()
    {
        Instance = this;
        UpdateScoreText(0);
    }

    public void UpdateScoreText(float score)
    {
        scoreText.text = "Score: " + score.ToString("F0");
    }
    
    public void UpdateTimeText(float time)
    {
        timeText.text = "Time Left: " + time.ToString("F0") + "s";
    }
}
