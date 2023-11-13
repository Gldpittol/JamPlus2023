using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private string levelTextString;
    [SerializeField] private string levelName;
    [SerializeField] private bool alwaysUnlocked = false;
    [SerializeField] private Image star1Fill;
    [SerializeField] private Image star2Fill;
    [SerializeField] private Image star3Fill;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI levelText;

    private Button myBytton;
    private void Awake()
    {
        levelText.text = levelTextString;
        myBytton = GetComponent<Button>();
        myBytton.onClick.AddListener(Clicked);
    }

    private void Start()
    {
        CheckIfUnlocked();
    }

    public void CheckIfUnlocked()
    {
        int starsUnlocked = PlayerDataManager.Instance.GetStarsAmount(levelName);
        float highscore = PlayerDataManager.Instance.GetHighScore(levelName);
        highScoreText.text = "Highscore: " + highscore.ToString("F0");
        if(starsUnlocked >= 3) star3Fill.gameObject.SetActive(true);
        if(starsUnlocked >= 2) star2Fill.gameObject.SetActive(true);
        if(starsUnlocked >= 1) star1Fill.gameObject.SetActive(true);

        if (alwaysUnlocked) return;
        
        bool isUnlocked = PlayerDataManager.Instance.CheckIfUnlocked(levelName);
        myBytton.interactable = isUnlocked;
        if (!isUnlocked)
        {
            levelText.DOFade(0.5f, 0);
            highScoreText.DOFade(0.5f, 0);
        }
    }

    public void Clicked()
    {
        LevelSelectManager.Instance.GoToLevel(levelName);
    }
}
