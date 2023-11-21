using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    [Header("Stars")] 
    [SerializeField] private GameObject starGold1;
    [SerializeField] private GameObject starGold2;
    [SerializeField] private GameObject starGold3;
    [SerializeField] private GameObject stampWin;
    [SerializeField] private GameObject stampLose;
    [SerializeField] private GameObject textWin;
    [SerializeField] private GameObject textLost;
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private GameObject pressSpaceTextLose;
    [SerializeField] private GameObject pressSpaceTextWin;
    [SerializeField] private float delayBetweenStamps = 0.5f;
    [SerializeField] private GameObject blur;
    [SerializeField] private GameObject touchPanel;

    [Header("Options")] 
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject pauseButton;
  //  [SerializeField] private GameObject continueButton;
  //  [SerializeField] private GameObject levelSelectButton;
  //  [SerializeField] private GameObject retryButton;

    private bool isRetrying;
    private bool isPaused = false;
    private bool isAnimating = false;
    private bool canGoToNextLevel;
    private bool won = false;
    private void Awake()
    {
        Instance = this;
        UpdateScoreText(0);
    }

    private void Update()
    {
        if (!isAnimating) return;
        
        if (!canGoToNextLevel)
        {
            if(Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Space))
            {
                Time.timeScale = 100;
            }
            
            return;
        }

        if (won && Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.Instance.LoadNextScene(true);
        }
        else if(Input.GetKeyDown(KeyCode.R))
        {
            GameManager.Instance.LoadNextScene(false);
        }
    }

    public void ClickedScreen(bool isRight)
    {
        if (isPaused) return;
        
        if (!isAnimating)
        {
            PlayerMovement.Instance.InputPerformed();
            return;
        }
        
        if (!canGoToNextLevel)
        {
            Time.timeScale = 100;
        }
        else
        {
            if (won)
            {
                if (isRight)
                {
                    GameManager.Instance.LoadNextScene(true);
                }
                else
                {
                    GameManager.Instance.LoadNextScene(false);
                }
            }
            else
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

    public void EnableFinalText(bool isWin, int stars)
    {
        if (isAnimating) return;

        won = isWin;
        isAnimating = true;
        pauseButton.SetActive(false);
        blur.SetActive(true);
        StartCoroutine(SummonScrollCoroutine(isWin, stars));
        
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

    public IEnumerator SummonScrollCoroutine(bool isWin, int stars)
    {
        score.text = "Final Score: " + GameManager.Instance.Score;
        if (isWin)
        {
            textWin.SetActive(true);
        }
        else
        {
            textLost.SetActive(true);
        }
        scrollParent.GetComponent<RectTransform>().DOLocalMoveY(68,upTweenDuration);
        yield return new WaitForSeconds(upTweenDuration);
        Time.timeScale = 1;
        sheet.GetComponent<RectTransform>().DOLocalMoveX(-50,sheetTweenDuration1);
        yield return new WaitForSeconds(sheetTweenDuration1);
        sheet.GetComponent<RectTransform>().DOLocalMoveX(-60,sheetTweenDuration2);
        yield return new WaitForSeconds(sheetTweenDuration1);
        GiveStars(isWin, stars);
        Time.timeScale = 1;
    }

    public void GiveStars(bool isWin, int stars)
    {
        StartCoroutine(GiveStarsCoroutine(isWin, stars));
    }

    public IEnumerator GiveStarsCoroutine(bool isWin, int stars)
    {
        yield return new WaitForSeconds(delayBetweenStamps);

        if (!isWin)
        {
            stampLose.SetActive(true);
            if (Time.timeScale == 1)
            {
                stampLose.GetComponent<ScalePop>().PopOutAnimation();
                AudioManager.Instance.PlaySound(AudioManager.AudioType.CatStamp);
            }
            AudioManager.Instance.PlaySound(AudioManager.AudioType.Stamp);

            yield return new WaitForSeconds(delayBetweenStamps);
            canGoToNextLevel = true;
            pressSpaceTextLose.SetActive(true);
            Time.timeScale = 1;
            yield break;
        }

        else
        {
            if (stars >= 1)
            {
                starGold1.SetActive(true);
                if (Time.timeScale == 1)
                {
                    starGold1.GetComponent<ScalePop>().PopOutAnimation();
                    AudioManager.Instance.PlaySound(AudioManager.AudioType.Star);
                }

                yield return new WaitForSeconds(delayBetweenStamps);
            }
            if (stars >= 2)
            {
                starGold2.SetActive(true);
                if (Time.timeScale == 1)
                {
                    starGold2.GetComponent<ScalePop>().PopOutAnimation();
                    AudioManager.Instance.PlaySound(AudioManager.AudioType.Star);
                }
                yield return new WaitForSeconds(delayBetweenStamps);
            }
            if (stars == 3)
            {
                starGold3.SetActive(true);
                if (Time.timeScale == 1)
                {
                    starGold3.GetComponent<ScalePop>().PopOutAnimation();
                    AudioManager.Instance.PlaySound(AudioManager.AudioType.Star);
                }
                yield return new WaitForSeconds(delayBetweenStamps);
            }
            stampWin.SetActive(true);
            if (Time.timeScale == 1)
            {
                stampWin.GetComponent<ScalePop>().PopOutAnimation();
                AudioManager.Instance.PlaySound(AudioManager.AudioType.CatStamp);
            }
            AudioManager.Instance.PlaySound(AudioManager.AudioType.Stamp);

            yield return new WaitForSeconds(delayBetweenStamps);
            canGoToNextLevel = true;
            pressSpaceTextWin.SetActive(true);
            Time.timeScale = 1;
        }
    }

    public void GoToLevelSelect()
    {
        isRetrying = true;
        Continue();
        LoadingCanvas.Instance.GoToScene("LevelSelect");
     //   levelSelectButton.GetComponent<ScalePop>().PopOutAnimation();
    }

    public void Retry()
    {
        isRetrying = true;
        Continue();
        LoadingCanvas.Instance.GoToScene(SceneManager.GetActiveScene().name);
      //  retryButton.GetComponent<ScalePop>().PopOutAnimation();
    }
    public void Continue()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
      //  if(continueButton) continueButton.GetComponent<ScalePop>().PopOutAnimation();
    }
    
    public void Pause()
    {
        if (GameManager.Instance.LevelEnded) return;
        if (isRetrying) return;

      //  pauseButton.GetComponent<ScalePop>().PopOutAnimation();
        isPaused = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }
}
