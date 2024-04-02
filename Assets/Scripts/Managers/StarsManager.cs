using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class StarsManager : MonoBehaviour
{
    public static StarsManager Instance;
    
    [SerializeField] private  GameObject star1, star2, star3;
    [SerializeField] private  GameObject starMask1, starMask2, starMask3;
    [SerializeField] private  bool popped1, popped2, popped3;
    public  GameObject flagsParent;

    [SerializeField] private float lastScore = 0;
    [SerializeField] private float maskSize = 2;
    [SerializeField] private float tweenDuration = 0.3f;
    [FormerlySerializedAs("minShake")] [SerializeField] private float shake = 1f;

    [SerializeField] private GameObject starsVFX;

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
            star1.GetComponent<SpriteRenderer>().color = Color.white;
            popped1 = true;
            GameObject temp = Instantiate(starsVFX, star1.transform.position, Quaternion.identity);
            temp.transform.eulerAngles = starsVFX.transform.eulerAngles;
            Destroy(temp, 2f);
        }
        if (!popped2)
        {
            star2.GetComponent<ScalePop>().PopOutAnimation();
            star2.GetComponent<SpriteRenderer>().color = Color.white;
            popped2 = true;
            GameObject temp = Instantiate(starsVFX, star2.transform.position, Quaternion.identity);
            temp.transform.eulerAngles = starsVFX.transform.eulerAngles;
            Destroy(temp, 2f);
        }
        if (!popped3)
        {
            star3.GetComponent<ScalePop>().PopOutAnimation();
            star3.GetComponent<SpriteRenderer>().color = Color.white;
            popped3 = true;
            GameObject temp = Instantiate(starsVFX, star3.transform.position, Quaternion.identity);
            temp.transform.eulerAngles = starsVFX.transform.eulerAngles;
            Destroy(temp, 2f);
            AudioManager.Instance.PlaySound(AudioManager.AudioType.Star);
        }
    }
    public void UpdateGold(float scorePercentage)
    {
        starMask1.SetActive(false);
        starMask2.SetActive(false);

        if (!popped1)
        {
            star1.GetComponent<ScalePop>().PopOutAnimation();
            star1.GetComponent<SpriteRenderer>().color = Color.white;
            popped1 = true;
            GameObject temp = Instantiate(starsVFX, star1.transform.position, Quaternion.identity);
            temp.transform.eulerAngles = starsVFX.transform.eulerAngles;
            Destroy(temp, 2f);
        }
        if (!popped2)
        {
            star2.GetComponent<ScalePop>().PopOutAnimation();
            star2.GetComponent<SpriteRenderer>().color = Color.white;
            popped2 = true;
            GameObject temp = Instantiate(starsVFX, star2.transform.position, Quaternion.identity);
            temp.transform.eulerAngles = starsVFX.transform.eulerAngles;
            Destroy(temp, 2f);
            AudioManager.Instance.PlaySound(AudioManager.AudioType.Star);
        }
        
        starMask1.transform.DOKill();
        starMask2.transform.DOKill();
        starMask3.transform.DOKill();
        starMask3.transform.DOMoveY(starMask3.transform.position.y - (maskSize * scorePercentage), tweenDuration);
    }
    public void UpdateSilver(float scorePercentage)
    {
        starMask1.SetActive(false);
        
        if (!popped1)
        {
            star1.GetComponent<ScalePop>().PopOutAnimation();
            star1.GetComponent<SpriteRenderer>().color = Color.white;
            popped1 = true;
            GameObject temp = Instantiate(starsVFX, star1.transform.position, Quaternion.identity);
            temp.transform.eulerAngles = starsVFX.transform.eulerAngles;
            Destroy(temp, 2f);
            AudioManager.Instance.PlaySound(AudioManager.AudioType.Star);
        }
        
        starMask1.transform.DOKill();
        starMask2.transform.DOKill();
        starMask2.transform.DOMoveY(starMask2.transform.position.y - (maskSize * scorePercentage), tweenDuration);
    }
    public void UpdatePass(float scorePercentage)
    {
        starMask1.transform.DOKill();
        starMask1.transform.DOMoveY(starMask1.transform.position.y - (maskSize * scorePercentage), tweenDuration);
    }

    public void ShakeFlag()
    {
        StartCoroutine(DoScreenCoroutine());
    }
    
    public IEnumerator DoScreenCoroutine()
    {
        float shakeFactor = shake;
        flagsParent.transform.localEulerAngles = new Vector3 (0,0, shakeFactor);
        yield return new WaitForSeconds(0.03f);
        flagsParent.transform.localEulerAngles = Vector3.zero;
        yield return new WaitForSeconds(0.03f);
        flagsParent.transform.localEulerAngles = new Vector3 (0,0, -shakeFactor);
        yield return new WaitForSeconds(0.03f);
        flagsParent.transform.localEulerAngles = Vector3.zero;
        yield return new WaitForSeconds(0.03f);
        flagsParent.transform.localEulerAngles = new Vector3 (0,0, shakeFactor/2);
        yield return new WaitForSeconds(0.03f);
        flagsParent.transform.localEulerAngles = Vector3.zero;
        yield return new WaitForSeconds(0.03f);
        flagsParent.transform.localEulerAngles = new Vector3 (0,0, -shakeFactor/2);
        yield return new WaitForSeconds(0.03f);
        flagsParent.transform.localEulerAngles = Vector3.zero;
        yield return new WaitForSeconds(0.03f);
        flagsParent.transform.localEulerAngles = new Vector3 (0,0, shakeFactor/4);
        yield return new WaitForSeconds(0.03f);
        flagsParent.transform.localEulerAngles = Vector3.zero;
        yield return new WaitForSeconds(0.03f);
        flagsParent.transform.localEulerAngles = new Vector3 (0,0, -shakeFactor/4);
        yield return new WaitForSeconds(0.03f);
        flagsParent.transform.localEulerAngles = Vector3.zero;
    }
}
