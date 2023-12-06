using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> textList = new List<GameObject>();
    private int currentTextID = 0;
    private bool canGoNext = false;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0))
        {
            GoToNextText();
        }
    }

    private void Start()
    {
        StartCoroutine(StopTimeCoroutine());
    }

    public IEnumerator StopTimeCoroutine()
    {
        yield return new WaitForSeconds(LoadingCanvas.Instance.FadeOutDuration);
        Time.timeScale = 0;
        canGoNext = true;
        EnableText(currentTextID);
    }

    public void GoToNextText()
    {
        if (!canGoNext) return;
        
        currentTextID++;
        EnableText(currentTextID);
    }
    
    public void EnableText(int i)
    {
        if (i >= textList.Count)
        {
            FinishTutorial();
            return;
        }
        textList[i].gameObject.SetActive(true);
    }

    private void FinishTutorial()
    {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }
}
