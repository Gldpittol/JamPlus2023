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
        isFading = true;
        fadePanel.DOFade(1, fadeInDuration).OnComplete(()=>LoadScene(sceneName));
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
