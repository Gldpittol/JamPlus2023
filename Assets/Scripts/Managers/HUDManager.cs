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
    
    [Header("Scroll")] 
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private GameObject scrollParent;
    [SerializeField] private GameObject sheet;
    [SerializeField] private float upTweenDuration;
    [SerializeField] private float sheetTweenDuration1;
    [SerializeField] private float sheetTweenDuration2;
    [SerializeField] private GameObject touchPanel;

    [Header("Stars")] 
    [SerializeField] private GameObject starGold1;
    [SerializeField] private GameObject starGold2;
    [SerializeField] private GameObject starGold3;
    [SerializeField] private GameObject stampWin;
    [SerializeField] private GameObject stampLose;
    [SerializeField] private Sprite stampWinSprite1,stampWinSprite2,stampWinSprite3;

    [SerializeField] private TextMeshProUGUI textWin;
    [SerializeField] private TextMeshProUGUI textLost;
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private GameObject pressSpaceTextLose;
    [SerializeField] private GameObject pressSpaceTextWin;
    [SerializeField] private GameObject pressSpaceTextMid;
    [SerializeField] private GameObject levelSelectButtonWin;
    [SerializeField] private GameObject levelSelectButtonLose;

    [SerializeField] private float delayBetweenStamps = 0.5f;
    [SerializeField] private GameObject blur;

    [Header("Options")] 
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private TextMeshProUGUI pauseTextLevelName;

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
            if(Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Joystick1Button1))
            {
                Time.timeScale = 100;
            }
            
            return;
        }

        if (won && (Input.GetKeyDown(KeyCode.Space)|| Input.GetKeyDown(KeyCode.Joystick1Button0)))
        {
            GameManager.Instance.LoadNextScene(true);
        }
        else if((Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Joystick1Button1)))
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
    }

    public void GoToNextLevel()
    {
        GameManager.Instance.LoadNextScene(true);
    }

    public void RetryLevel()
    {
        GameManager.Instance.LoadNextScene(false);
    }

    public void UpdateScoreText(float score)
    {
        var tempList = new List<object>();
        tempList.Add(GetLevelName());
        
        scoreText.GetComponent<TextLocalizerUI>().UpdateParameters(tempList);
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
        
        var tempList = new List<object>();
        tempList.Add(GetLevelName());
        
        if (isWin)
        {
            textWin.GetComponent<TextLocalizerUI>().UpdateParameters(tempList); 
            textWin.gameObject.SetActive(true);
        }
        else
        {
            textLost.GetComponent<TextLocalizerUI>().UpdateParameters(tempList);
            textLost.gameObject.SetActive(true);
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
            stampLose.transform.parent =  scrollParent.transform.parent;

            if (Time.timeScale == 1)
            {
                stampLose.GetComponent<ScalePop>().PopOutAnimation();
                AudioManager.Instance.PlaySound(AudioManager.AudioType.CatStamp);
            }
            AudioManager.Instance.PlaySound(AudioManager.AudioType.Stamp);

            yield return new WaitForSeconds(delayBetweenStamps);
            canGoToNextLevel = true;
            pressSpaceTextMid.SetActive(true);
            levelSelectButtonLose.SetActive(true);

            pressSpaceTextMid.transform.parent = touchPanel.transform;
            levelSelectButtonLose.transform.parent = touchPanel.transform;

            Time.timeScale = 1;
            yield break;
        }

        else
        {
            if (stars >= 1)
            {
                starGold1.SetActive(true);
                starGold1.transform.parent.transform.parent = scrollParent.transform.parent;
                stampWin.GetComponent<Image>().sprite = stampWinSprite1;
                stampWin.transform.parent =  scrollParent.transform.parent;

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
                starGold2.transform.parent.transform.parent = scrollParent.transform.parent;
                stampWin.GetComponent<Image>().sprite = stampWinSprite2;
                stampWin.transform.parent =  scrollParent.transform.parent;

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
                starGold3.transform.parent.transform.parent = scrollParent.transform.parent;
                stampWin.GetComponent<Image>().sprite = stampWinSprite3;
                stampWin.transform.parent =  scrollParent.transform.parent;
                if (Time.timeScale == 1)
                {
                    starGold3.GetComponent<ScalePop>().PopOutAnimation();
                    AudioManager.Instance.PlaySound(AudioManager.AudioType.Star);
                }
                yield return new WaitForSeconds(delayBetweenStamps);
            }
            stampWin.SetActive(true);
            GameManager.Instance.DoScreenShake();
            if (Time.timeScale == 1)
            {
                stampWin.GetComponent<ScalePop>().PopOutAnimation();
                AudioManager.Instance.PlaySound(AudioManager.AudioType.CatStamp);
            }
            AudioManager.Instance.PlaySound(AudioManager.AudioType.Stamp);

            yield return new WaitForSeconds(delayBetweenStamps);
            canGoToNextLevel = true;
            pressSpaceTextWin.SetActive(true);
            pressSpaceTextLose.SetActive(true);
            levelSelectButtonWin.SetActive(true);

            pressSpaceTextLose.transform.parent = touchPanel.transform;
            pressSpaceTextWin.transform.parent = touchPanel.transform;
            levelSelectButtonWin.transform.parent = touchPanel.transform;

            Time.timeScale = 1;
        }
    }

    public void GoToLevelSelect()
    {
        isRetrying = true;
        Continue();
        if (GameManager.Instance.gameState == GameManager.GameState.GameEnded) GameManager.Instance.UnlockNext();
        else GameManager.Instance.gameState = GameManager.GameState.GameEnded;
        LoadingCanvas.Instance.GoToScene("LevelSelect");
     //   levelSelectButton.GetComponent<ScalePop>().PopOutAnimation();
    }

    public void Retry()
    {
        GameManager.Instance.gameState = GameManager.GameState.GameEnded;
        isRetrying = true;
        Continue();
        LoadingCanvas.Instance.GoToScene(SceneManager.GetActiveScene().name);
      //  retryButton.GetComponent<ScalePop>().PopOutAnimation();
    }
    
    public bool IsPaused
    {
        get => isPaused;
        set => isPaused = value;
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
        if (GameManager.Instance.gameState == GameManager.GameState.GameEnded) return;
        if (isRetrying) return;


        var tempList = new List<object>();
        tempList.Add(GetLevelName());
        pauseTextLevelName.GetComponent<TextLocalizerUI>().UpdateParameters(tempList);
        isPaused = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        
    }

    public string GetLevelName()
    {
        string name = SceneManager.GetActiveScene().name;
        
        for (int i = 0; i < name.Length; i++)
        {
            if (Char.IsDigit(name[i]))
            {
                name = name.Substring(i);
                break;
            }
        }

        return name;
    }
}
