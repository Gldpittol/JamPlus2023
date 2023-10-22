using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class StarsManager : MonoBehaviour
{
    public static StarsManager Instance;
    
    [SerializeField] private  GameObject star1, star2, star3;
    [SerializeField] private  GameObject starMask1, starMask2, starMask3;
    [SerializeField] private  bool popped1, popped2, popped3;

    [SerializeField] private float lastScore = 0;
    [SerializeField] private float maskSize = 2;
    [SerializeField] private float tweenDuration = 0.3f;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateStars(float score, float scorePass, float scoreSilver, float scoreGold)
    {
        float scoreDiff = score - lastScore;
        if (score >= scoreGold)
        {
            UpdateFinished();
        }
        else if (score >= scoreSilver)
        {
            UpdateGold(scoreDiff / (scoreGold - scoreSilver));
        }
        else if (score >= scorePass)
        {
            UpdateSilver(scoreDiff / (scoreSilver - scorePass));
        }
        else 
        {
            UpdatePass(scoreDiff / scorePass);
        }

        lastScore = score;
    }

    public void UpdateFinished()
    {
        starMask1.SetActive(false);
        starMask2.SetActive(false);
        starMask3.SetActive(false);

        if (!popped1)
        {
            star1.GetComponent<ScalePop>().PopOutAnimation();
            popped1 = true;
        }
        if (!popped2)
        {
            star2.GetComponent<ScalePop>().PopOutAnimation();
            popped2 = true;
        }
        if (!popped3)
        {
            star3.GetComponent<ScalePop>().PopOutAnimation();
            popped3 = true;
        }
    }
    public void UpdateGold(float scorePercentage)
    {
        starMask1.SetActive(false);
        starMask2.SetActive(false);

        if (!popped1)
        {
            star1.GetComponent<ScalePop>().PopOutAnimation();
            popped1 = true;
        }
        if (!popped2)
        {
            star2.GetComponent<ScalePop>().PopOutAnimation();
            popped2 = true;
        }
        
        starMask3.transform.DOKill();
        starMask3.transform.DOMoveY(-maskSize * scorePercentage, tweenDuration);
    }
    public void UpdateSilver(float scorePercentage)
    {
        starMask1.SetActive(false);
        
        if (!popped1)
        {
            star1.GetComponent<ScalePop>().PopOutAnimation();
            popped1 = true;
        }
        
        starMask2.transform.DOKill();
        starMask2.transform.DOMoveY(-maskSize * scorePercentage, tweenDuration);
    }
    public void UpdatePass(float scorePercentage)
    {
        starMask1.transform.DOKill();
        starMask1.transform.DOMoveY(-maskSize * scorePercentage, tweenDuration);
    }
    
}
