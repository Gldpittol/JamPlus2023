using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> textList = new List<GameObject>();
    [SerializeField] private int tutorialID;
    [SerializeField] private float masterAppearanceDuration;
    [SerializeField] private RectTransform masterObject;
    [SerializeField] private RectTransform masterFinalPosObject;
    [SerializeField] private GameObject firstBubble;
    [SerializeField] private GameObject firstDialogue;
    [SerializeField] private GameObject smokeVFX;
    [SerializeField] private float smokeVFXDuration = 2f;
    [SerializeField] private float bubbleFadeOutDuration = 2f;

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
        if (PlayerDataManager.Instance.CheckIfSeenTutorial(tutorialID))
        {
            gameObject.SetActive(false);
            return;
        }
        
        StartCoroutine(StopTimeCoroutine());
    }

    public IEnumerator StopTimeCoroutine()
    {
        GameManager.Instance.gameState = GameManager.GameState.Tutorial;
        yield return new WaitForSeconds(LoadingCanvas.Instance.FadeOutDuration);
        Time.timeScale = 0;
        DoMasterAnimation();
    }

    public void DoMasterAnimation()
    {
        StartCoroutine(MasterAnimationCoroutine());
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
        if(i >=1)textList[i-1].gameObject.SetActive(false);
    }

    private void FinishTutorial()
    {
        StartCoroutine(MasterSmokeCoroutine());
    }

    public IEnumerator MasterAnimationCoroutine()
    {
        firstBubble.SetActive(false);
        firstDialogue.SetActive(false);
        
        masterObject.DOAnchorPos(masterFinalPosObject.anchoredPosition, masterAppearanceDuration).SetUpdate(true);
        
        yield return new WaitForSecondsRealtime(masterAppearanceDuration);

        canGoNext = true;
        firstBubble.SetActive(true);
        firstDialogue.SetActive(true);
        EnableText(currentTextID);
    }

    public IEnumerator MasterSmokeCoroutine()
    {
        canGoNext = false;
        var mainModule = smokeVFX.GetComponent<ParticleSystem>().main;
        mainModule.startLifetime = smokeVFXDuration;
        smokeVFX.SetActive(true);
        smokeVFX.GetComponent<ParticleSystem>().Play();
        PlayerDataManager.Instance.AddSeenTutorial(tutorialID);

        firstBubble.GetComponent<Image>().DOFade(0, bubbleFadeOutDuration).SetUpdate(true);
        textList[currentTextID - 1].transform.GetChild(0).GetComponent<TextMeshProUGUI>().DOFade(0, bubbleFadeOutDuration)
            .SetUpdate(true);

        yield return new WaitForSecondsRealtime(bubbleFadeOutDuration);

        masterObject.GetComponent<Image>().DOFade(0, smokeVFXDuration/2).SetUpdate(true);

        yield return new WaitForSecondsRealtime(smokeVFXDuration);

        Time.timeScale = 1;
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        GameManager.Instance.gameState = GameManager.GameState.Gameplay;
        
        gameObject.SetActive(false);
    }
}
