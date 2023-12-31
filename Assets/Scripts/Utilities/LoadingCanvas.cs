using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingCanvas : MonoBehaviour
{
    public static LoadingCanvas Instance;
    
    [SerializeField] private float fadeInDuration;
    [SerializeField] private float fadeOutDuration;
    [SerializeField] private Image fadePanel;

    private bool isFading;
    
    public float FadeOutDuration => fadeOutDuration;
    private void Awake()
    {
        Instance = this;
        fadePanel.gameObject.SetActive(true);
    }

    private void Start()
    {
        fadePanel.DOFade(0, fadeOutDuration);
    }

    public void GoToScene(string sceneName)
    {
        if (isFading) return;
        GetComponentInChildren<Image>().raycastTarget = true;
        isFading = true;
        fadePanel.DOFade(1, fadeInDuration).OnComplete(()=>LoadScene(sceneName));
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    public void SwapUiPanel(GameObject oldPanel, GameObject newPanel)
    {
        if (isFading) return;
        GetComponentInChildren<Image>().raycastTarget = true;
        isFading = true;
        fadePanel.DOFade(1, fadeInDuration).OnComplete(()=>SwapUiPanelFinish(oldPanel,newPanel));
    }

    public void SwapUiPanelFinish(GameObject oldPanel, GameObject newPanel)
    {
        oldPanel.SetActive(false);
        newPanel.SetActive(true);
        fadePanel.DOFade(0, fadeOutDuration).OnComplete(()=>FinishUiPanel());
    }

    public void FinishUiPanel()
    {
        GetComponentInChildren<Image>().raycastTarget = false;
        isFading = false;
    }
}
