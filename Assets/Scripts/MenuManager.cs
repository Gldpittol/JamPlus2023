using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private string firstLevel;
    [SerializeField] private GameObject firstPart;
    [SerializeField] private GameObject secondPart;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private Button button0;
    [SerializeField] private Button button1;
    [SerializeField] private Color highLightColor;

    private Button currentButton;
    private float currentDelay;

    private void Start()
    {
        currentButton = button0;
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
        
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentButton.onClick.Invoke();
           // AudioManager.Instance.PlaySound(AudioManager.AudioType.UIConfirm);
        }

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.S) ||
            Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentButton == button0)
            {
                currentButton = button1;
                currentButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = highLightColor;
                currentButton.GetComponent<ScalePop>().PopOutAnimation();
                button0.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
                //AudioManager.Instance.PlaySound(AudioManager.AudioType.UISelect);
            }
            else
            {
                currentButton = button0;
                currentButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = highLightColor;
                currentButton.GetComponent<ScalePop>().PopOutAnimation();
                button1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
                //AudioManager.Instance.PlaySound(AudioManager.AudioType.UISelect);
            }
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
}
