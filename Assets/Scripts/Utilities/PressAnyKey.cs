using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PressAnyKey : MonoBehaviour
{
    //private Image img;
    [SerializeField] private Vector3 enlargedScale;
    [SerializeField] private Vector3 shrinkedScale;
    [SerializeField] private float duration;
    [SerializeField] private MenuManager menu;

    private float originalScale;
    private void Awake()
    {
        //img = GetComponent<Image>();
        originalScale = transform.localScale.x;
        Enlarge();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            menu.EnableSecondPart();
            enabled = false;
        }
    }

    public void Enlarge()
    {
        transform.DOScale(enlargedScale * originalScale, duration).SetEase(Ease.Linear).OnComplete(()=>Shrink());
    }
    
    public void Shrink()
    {
        transform.DOScale(shrinkedScale * originalScale, duration).SetEase(Ease.Linear).OnComplete(()=>Enlarge());
    }
}
