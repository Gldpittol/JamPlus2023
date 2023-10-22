using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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

    [Header("Scroll")] 
    [SerializeField] private GameObject scrollParent;
    [SerializeField] private GameObject sheet;
    [SerializeField] private float upTweenDuration;
    [SerializeField] private float sheetTweenDuration1;
    [SerializeField] private float sheetTweenDuration2;

    private bool isAnimating = false;
    private void Awake()
    {
        Instance = this;
        UpdateScoreText(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            EnableFinalText(true);
        }
        
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
        if (isAnimating) return;
        isAnimating = true;
        StartCoroutine(SummonScrollCoroutine(isWin));
        
        /*if (finalPanel.enabled) return;
        if (isWin)
        {
            winText.gameObject.SetActive(true);
        }
        else
        {
            loseText.gameObject.SetActive(true);
        }

        finalPanel.enabled = true;*/
    }

    public IEnumerator SummonScrollCoroutine(bool isWin)
    {
        scrollParent.GetComponent<RectTransform>().DOLocalMoveY(68,upTweenDuration);
        yield return new WaitForSeconds(upTweenDuration);
        sheet.GetComponent<RectTransform>().DOLocalMoveX(-50,sheetTweenDuration1);
        yield return new WaitForSeconds(sheetTweenDuration1);
        sheet.GetComponent<RectTransform>().DOLocalMoveX(-60,sheetTweenDuration2);
        yield return new WaitForSeconds(sheetTweenDuration1);
        GiveStars();
    }

    public void GiveStars()
    {
        
    }
}
