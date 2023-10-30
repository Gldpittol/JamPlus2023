using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Glow : MonoBehaviour
{
    [SerializeField] private float popDuration = 0.3f;
    [SerializeField] private float trueSize = 0.7f;
    [SerializeField] private float largeSize = 0.5f;
    [SerializeField] private float smallSize = 0.9f;

    private void Start()
    {
        ContinuousPop();
        transform.localScale = new Vector3(trueSize, trueSize, trueSize);
    }

    public void ContinuousPop()
    {
        StartCoroutine(ContinuousPopCoroutine());
    }

    public IEnumerator ContinuousPopCoroutine()
    {
        transform.DOScale(new Vector3(largeSize, largeSize, largeSize), popDuration);
        yield return new WaitForSeconds(popDuration);
        transform.DOScale(new Vector3(smallSize, smallSize, smallSize), popDuration);
        yield return new WaitForSeconds(popDuration);
        ContinuousPop();
    }
}
