using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BGTween : MonoBehaviour
{
    public Vector2 offset;
    public float tweenDuration = 30f;
    public Ease tweenEase = Ease.Linear;

    private void Awake()
    {
        transform.DOLocalMove((Vector2)transform.localPosition + offset, tweenDuration).SetEase(tweenEase);
    }
}
