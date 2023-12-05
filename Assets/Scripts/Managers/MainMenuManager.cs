using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject firstPart;
    [SerializeField] private GameObject firstPart2;

    [SerializeField] private GameObject secondPart;
    [SerializeField] private GameObject secondPart2;

    [SerializeField] private GameObject creditsPanel;

    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject optionsPanel2;

    [FormerlySerializedAs("buttonsList")] [SerializeField] private List<Button> mainMenuButtonsList = new List<Button>();
    [SerializeField] private Color highLightColor;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider audioSlider;
    [SerializeField] private List<GameObject> optionsInteractableList = new List<GameObject>();
    [SerializeField] private float sliderIncrement = 0.1f;
    [SerializeField] private float delayBetweenAutoMove = 0.1f;

    [Header("First Part Refs")] 
    [SerializeField] private Image playerFirstPart;
    [SerializeField] private GameObject blackBorderTopFirstPart;
    [SerializeField] private GameObject blackBorderBottomFirstPart;
    [SerializeField] private RectTransform titleFirstPart;
    [SerializeField] private TextMeshProUGUI pressAnyText;
    [SerializeField] private float firstPartFadeDuration;
    [SerializeField] private Image bgFirstPart;
    [SerializeField] private Sprite secondPartSprite;
    [SerializeField] private RectTransform titleFinalPos;
    [SerializeField] private AnimationCurve titleEase;

    [Header("Second Part Refs")] 
    [SerializeField] private float secondPartFadeInDuration;
    [SerializeField] private float secondPartFadeOutDuration;

    [Header("Second Part Refs")] 
    [SerializeField] private float thirdPartFadeInDuration;
    [SerializeField] private float thirdPartFadeOutDuration;
    [SerializeField] private List<TextMeshProUGUI> textFadeListThirdPart = new List<TextMeshProUGUI>();
    [SerializeField] private List<Image> imgFadeListThirdPart = new List<Image>();

    private float currentDelayAutoMove;
    private Button currentButton;
    private GameObject currentOptionsSelection;
    private float currentDelay;
    private bool pressedSpace;
    private bool isOnFirstPart;
    private bool isOnSecondPart;
    private bool isOnOptions;
    private bool isAnimating;

    private void Start()
    {
        currentButton = mainMenuButtonsList[0];
        currentButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = highLightColor;
    }

    private void Update()
    {
        currentDelay -= Time.deltaTime;
        currentDelayAutoMove -= Time.deltaTime;    
        
        if (isAnimating) return;

        if (creditsPanel.activeInHierarchy)
        {
            if (Input.anyKeyDown)
            {
                creditsPanel.SetActive(false);
              //  AudioManager.Instance.PlaySound(AudioManager.AudioType.UIBack);
            }
        }

        CheckInputs();
    }

    public void CheckInputs()
    {
        if (isAnimating) return;

        if (creditsPanel.activeInHierarchy || firstPart.activeInHierarchy)
        {
            currentDelay = 0.2f;
            return;
        }
        if (currentDelay > 0) return;
        
        if (secondPart.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0))
            {
                pressedSpace = true;
                currentButton.onClick.Invoke();
                // AudioManager.Instance.PlaySound(AudioManager.AudioType.UIConfirm);
            }
           CheckInputMainCanvas();
        }
        else if (optionsPanel.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0))
            {
                pressedSpace = true;
                Button tempButton = currentOptionsSelection.GetComponent<Button>();
                if(tempButton) tempButton.onClick.Invoke();
                // AudioManager.Instance.PlaySound(AudioManager.AudioType.UIConfirm);
            }
            CheckInputOptionsCanvas();
        }
    }

    public void CheckInputMainCanvas()
    {
        if (isAnimating) return;
        if (currentDelay > 0) return;
        if (currentDelayAutoMove > 0) return;

        if (Input.GetAxisRaw("Vertical") > 0)
        {
            int buttonId = mainMenuButtonsList.IndexOf(currentButton);
            int newButtonId = (buttonId - 1) % mainMenuButtonsList.Count;
            if (newButtonId == -1) newButtonId = mainMenuButtonsList.Count - 1;

            currentButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
            currentButton = mainMenuButtonsList[newButtonId];
            currentButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = highLightColor;
            currentButton.GetComponent<ScalePop>().PopOutAnimation();
            currentDelayAutoMove = delayBetweenAutoMove;
            //AudioManager.Instance.PlaySound(AudioManager.AudioType.UISelect);
        }
        else if (Input.GetAxisRaw("Vertical") < 0)
        {
            int buttonId = mainMenuButtonsList.IndexOf(currentButton);
            int newButtonId = (buttonId + 1) % mainMenuButtonsList.Count;

            currentButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
            currentButton = mainMenuButtonsList[newButtonId];
            currentButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = highLightColor;
            currentButton.GetComponent<ScalePop>().PopOutAnimation();
            currentDelayAutoMove = delayBetweenAutoMove;
            //AudioManager.Instance.PlaySound(AudioManager.AudioType.UISelect);
        }
    }

    public void CheckInputOptionsCanvas()
    {
        if (isAnimating) return;
        if (currentDelay > 0) return;
        if (currentDelayAutoMove > 0) return;

        if (Input.GetAxisRaw("Vertical") > 0)
        {
            int optionsId = optionsInteractableList.IndexOf(currentOptionsSelection);
            int newOptionsId = (optionsId - 1) % optionsInteractableList.Count;
            if (newOptionsId == -1) newOptionsId = optionsInteractableList.Count - 1;

            currentOptionsSelection.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
            currentOptionsSelection = optionsInteractableList[newOptionsId];
            currentOptionsSelection.GetComponentInChildren<TextMeshProUGUI>().color = highLightColor;
            currentOptionsSelection.GetComponent<ScalePop>().PopOutAnimation();
            currentDelayAutoMove = delayBetweenAutoMove;
            //AudioManager.Instance.PlaySound(AudioManager.AudioType.UISelect);
        }
        else if (Input.GetAxisRaw("Vertical") < 0)
        {
            int optionsId = optionsInteractableList.IndexOf(currentOptionsSelection);
            int newOptionsId = (optionsId + 1) % optionsInteractableList.Count;

            currentOptionsSelection.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
            currentOptionsSelection = optionsInteractableList[newOptionsId];
            currentOptionsSelection.GetComponentInChildren<TextMeshProUGUI>().color = highLightColor;
            currentOptionsSelection.GetComponent<ScalePop>().PopOutAnimation();
            currentDelayAutoMove = delayBetweenAutoMove;
            //AudioManager.Instance.PlaySound(AudioManager.AudioType.UISelect);
        }
        else if (Input.GetAxisRaw("Horizontal") > 0)
        {
            if (currentOptionsSelection.GetComponent<Slider>())
            {
                Slider tempSlider = currentOptionsSelection.GetComponent<Slider>();
                tempSlider.value += sliderIncrement;
                if (tempSlider.value > 1) tempSlider.value = 1;
                tempSlider.onValueChanged.Invoke(tempSlider.value);
                currentDelayAutoMove = delayBetweenAutoMove;
            }
        }
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            if (currentOptionsSelection.GetComponent<Slider>())
            {
                Slider tempSlider = currentOptionsSelection.GetComponent<Slider>();
                tempSlider.value -= sliderIncrement;
                if (tempSlider.value < 0) tempSlider.value = 0;
                tempSlider.onValueChanged.Invoke(tempSlider.value);
                currentDelayAutoMove = delayBetweenAutoMove;
            }
        }
    }

    public void ClickedScreen()
    {
        if (firstPart.activeInHierarchy)
        {
            StartCoroutine(EnableSecondPartCoroutine());

            currentButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
        }
        
        if (creditsPanel.activeInHierarchy)
        {
            creditsPanel.SetActive(false);
        }
    }

    public void StartGame()
    {
        GameManager.Instance.LoadNextScene(true);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void EnableSecondPart()
    {
        StartCoroutine(EnableSecondPartCoroutine());
    }

    public void OpenCredits(bool creditsState)
    {
        creditsPanel.SetActive(creditsState);
    }

    public void ResetData()
    {
        print("Data Cleared Successfully");
        PlayerDataManager.Instance.ResetData();
    }

    public void UpdateMusicVolume()
    {
        PlayerPrefs.SetFloat("MusicVolume",musicSlider.value);
        MusicManager.Instance.UpdateMusicVolume();
    }
    
    public void UpdateAudioVolume()
    {
        PlayerPrefs.SetFloat("AudioVolume",audioSlider.value);
    }

    public void OnEnableSliders()
    {
        StartCoroutine(EnableOptions());
        currentOptionsSelection = musicSlider.gameObject;
        if (pressedSpace)
        {
            musicSlider.GetComponentInChildren<TextMeshProUGUI>().color = highLightColor;
            pressedSpace = false;
        }
        currentOptionsSelection = optionsInteractableList[0];
        
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        }
        else
        {
            musicSlider.value = 1;
        }
        if (PlayerPrefs.HasKey("AudioVolume"))
        {
            audioSlider.value = PlayerPrefs.GetFloat("AudioVolume");
        }
        else
        {
            audioSlider.value = 1;
        }
    }

    public void DisableOptionsPanel()
    {
        StartCoroutine(DisableOptions());
        currentDelay = 0.2f;
    }

    public IEnumerator EnableSecondPartCoroutine()
    {
        if (isOnFirstPart) yield break;
        isAnimating = true;
        isOnFirstPart = true;

        bgFirstPart.sprite = secondPartSprite;
        blackBorderTopFirstPart.GetComponent<RectTransform>().DOAnchorPosY(1300, firstPartFadeDuration);
        blackBorderBottomFirstPart.GetComponent<RectTransform>().DOAnchorPosY(-1300, firstPartFadeDuration);
        pressAnyText.DOFade(0, firstPartFadeDuration);
        playerFirstPart.DOFade(0, firstPartFadeDuration);
        titleFirstPart.DOAnchorPos(titleFinalPos.position, firstPartFadeDuration).SetEase(titleEase);
        
        yield return new WaitForSeconds(firstPartFadeDuration);
        titleFirstPart.transform.parent = titleFinalPos.transform;
        firstPart.SetActive(false);
        firstPart2.SetActive(false);
        secondPart2.SetActive(true);
        secondPart.SetActive(true);

        foreach (Button b in mainMenuButtonsList)
        {
            Image i = b.GetComponent<Image>();
            TextMeshProUGUI t = b.transform.GetChild(0).transform.GetComponent<TextMeshProUGUI>();
            i.DOFade(1, secondPartFadeInDuration);
            t.DOFade(1, secondPartFadeInDuration);
        }

        isAnimating = false;
    }
    
    

    public IEnumerator EnableOptions()
    {
        if (isAnimating) yield break;
        isAnimating = true;

        titleFirstPart.GetComponent<Image>().DOFade(0, secondPartFadeOutDuration);
        foreach (Button b in mainMenuButtonsList)
        {
            Image i = b.GetComponent<Image>();
            TextMeshProUGUI t = b.transform.GetChild(0).transform.GetComponent<TextMeshProUGUI>();
            i.DOFade(0, secondPartFadeInDuration);
            t.DOFade(0, secondPartFadeInDuration);
        }
        
        foreach (TextMeshProUGUI t in textFadeListThirdPart)
        {
            t.DOFade(0, 0);
        }
        
        foreach (Image i in imgFadeListThirdPart)
        {
            i.DOFade(0, 0);
        }

        yield return new WaitForSeconds(secondPartFadeOutDuration);
        optionsPanel.SetActive(true);
        optionsPanel2.SetActive(true);
        secondPart.SetActive(false);
        secondPart2.SetActive(false);
        
        foreach (TextMeshProUGUI t in textFadeListThirdPart)
        {
            t.DOFade(1, thirdPartFadeInDuration);
        }
        foreach (Image i in imgFadeListThirdPart)
        {
            i.DOFade(1, thirdPartFadeInDuration);
        }
        
        yield return new WaitForSeconds(thirdPartFadeInDuration);

        isAnimating = false;
    }
    
    public IEnumerator DisableOptions()
    {
        if (isAnimating) yield break;
        isAnimating = true;

        foreach (TextMeshProUGUI t in textFadeListThirdPart)
        {
            t.DOFade(0, thirdPartFadeOutDuration);
        }
        foreach (Image i in imgFadeListThirdPart)
        {
            i.DOFade(0, thirdPartFadeOutDuration);
        }
        
        yield return new WaitForSeconds(thirdPartFadeOutDuration);
        
        currentOptionsSelection.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;

        optionsPanel.SetActive(false);
        optionsPanel2.SetActive(false);
        secondPart.SetActive(true);
        secondPart2.SetActive(true);
        
        titleFirstPart.GetComponent<Image>().DOFade(1, secondPartFadeInDuration);
        foreach (Button b in mainMenuButtonsList)
        {
            Image i = b.GetComponent<Image>();
            TextMeshProUGUI t = b.transform.GetChild(0).transform.GetComponent<TextMeshProUGUI>();
            i.DOFade(1, secondPartFadeInDuration);
            t.DOFade(1, secondPartFadeInDuration);
        }
        
        yield return new WaitForSeconds(secondPartFadeInDuration);

        isAnimating = false;
    }
}
