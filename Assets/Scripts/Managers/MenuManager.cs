using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private string firstLevel;
    [SerializeField] private GameObject firstPart;
    [SerializeField] private GameObject secondPart;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject optionsPanel;
    [FormerlySerializedAs("buttonsList")] [SerializeField] private List<Button> mainMenuButtonsList = new List<Button>();
    [SerializeField] private Color highLightColor;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider audioSlider;
    [SerializeField] private List<GameObject> optionsInteractableList = new List<GameObject>();
    [SerializeField] private float sliderIncrement = 0.1f;

    private Button currentButton;
    private GameObject currentOptionsSelection;
    private float currentDelay;

    private void Start()
    {
        currentButton = mainMenuButtonsList[0];
        currentButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = highLightColor;
    }

    private void Update()
    {
        currentDelay -= Time.deltaTime;
            
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
        if (creditsPanel.activeInHierarchy || firstPart.activeInHierarchy)
        {
            currentDelay = 0.2f;
            return;
        }
        if (currentDelay > 0) return;
        
        if (secondPart.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                currentButton.onClick.Invoke();
                // AudioManager.Instance.PlaySound(AudioManager.AudioType.UIConfirm);
            }
           CheckInputMainCanvas();
        }
        else if (optionsPanel.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Button tempButton = currentOptionsSelection.GetComponent<Button>();
                if(tempButton) tempButton.onClick.Invoke();
                // AudioManager.Instance.PlaySound(AudioManager.AudioType.UIConfirm);
            }
            CheckInputOptionsCanvas();
        }
    }

    public void CheckInputMainCanvas()
    {
        if (currentDelay > 0) return;
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            int buttonId = mainMenuButtonsList.IndexOf(currentButton);
            int newButtonId = (buttonId - 1) % mainMenuButtonsList.Count;
            if (newButtonId == -1) newButtonId = mainMenuButtonsList.Count - 1;

            currentButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
            currentButton = mainMenuButtonsList[newButtonId];
            currentButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = highLightColor;
            currentButton.GetComponent<ScalePop>().PopOutAnimation();
            //AudioManager.Instance.PlaySound(AudioManager.AudioType.UISelect);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            int buttonId = mainMenuButtonsList.IndexOf(currentButton);
            int newButtonId = (buttonId + 1) % mainMenuButtonsList.Count;

            currentButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
            currentButton = mainMenuButtonsList[newButtonId];
            currentButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = highLightColor;
            currentButton.GetComponent<ScalePop>().PopOutAnimation();
            //AudioManager.Instance.PlaySound(AudioManager.AudioType.UISelect);
        }
    }

    public void CheckInputOptionsCanvas()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            int optionsId = optionsInteractableList.IndexOf(currentOptionsSelection);
            int newOptionsId = (optionsId - 1) % optionsInteractableList.Count;
            if (newOptionsId == -1) newOptionsId = optionsInteractableList.Count - 1;

            currentOptionsSelection.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
            currentOptionsSelection = optionsInteractableList[newOptionsId];
            currentOptionsSelection.GetComponentInChildren<TextMeshProUGUI>().color = highLightColor;
            currentOptionsSelection.GetComponent<ScalePop>().PopOutAnimation();
            //AudioManager.Instance.PlaySound(AudioManager.AudioType.UISelect);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            int optionsId = optionsInteractableList.IndexOf(currentOptionsSelection);
            int newOptionsId = (optionsId + 1) % optionsInteractableList.Count;

            currentOptionsSelection.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
            currentOptionsSelection = optionsInteractableList[newOptionsId];
            currentOptionsSelection.GetComponentInChildren<TextMeshProUGUI>().color = highLightColor;
            currentOptionsSelection.GetComponent<ScalePop>().PopOutAnimation();
            //AudioManager.Instance.PlaySound(AudioManager.AudioType.UISelect);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentOptionsSelection.GetComponent<Slider>())
            {
                Slider tempSlider = currentOptionsSelection.GetComponent<Slider>();
                tempSlider.value += sliderIncrement;
                if (tempSlider.value > 1) tempSlider.value = 1;
                tempSlider.onValueChanged.Invoke(tempSlider.value);
            }
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentOptionsSelection.GetComponent<Slider>())
            {
                Slider tempSlider = currentOptionsSelection.GetComponent<Slider>();
                tempSlider.value -= sliderIncrement;
                if (tempSlider.value < 0) tempSlider.value = 0;
                tempSlider.onValueChanged.Invoke(tempSlider.value);
            }
        }
    }

    public void ClickedScreen()
    {
        if (firstPart.activeInHierarchy)
        {
            firstPart.SetActive(false);
            secondPart.SetActive(true);
           // currentButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
        }
        
        if (creditsPanel.activeInHierarchy)
        {
            creditsPanel.SetActive(false);
        }
    }

    public void StartGame()
    {
        LoadingCanvas.Instance.GoToScene(firstLevel);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void EnableSecondPart()
    {
        firstPart.SetActive(false);
        secondPart.SetActive(true);
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
        currentOptionsSelection = musicSlider.gameObject;
        musicSlider.GetComponentInChildren<TextMeshProUGUI>().color = highLightColor;
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
        currentOptionsSelection.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
        currentDelay = 0.2f;
        optionsPanel.SetActive(false);
        secondPart.SetActive(true);
    }
}
