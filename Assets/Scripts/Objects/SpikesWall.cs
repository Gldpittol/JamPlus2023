using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class SpikesWall : MonoBehaviour
{
    [FormerlySerializedAs("delayInBetween")] [SerializeField] private float initialDelay = 5f;
    [SerializeField] private float warningDuration = 2f;
    [SerializeField] private GameObject warningObject ;
    [SerializeField] private GameObject killObject;
    [SerializeField] private Color invisibleColor ;
    [SerializeField] private Color warningColor ;
    [SerializeField] private Color killColor ;
    [SerializeField] private Ease colorEase ;
    [SerializeField] private float spikesYMove ;
    [SerializeField] private float moveDuration = 0.1f ;

    private SpriteRenderer warningObjectSr;
    private void Awake()
    {
        warningObject.SetActive(false);
        killObject.SetActive(false);
        warningObjectSr = warningObject.GetComponent<SpriteRenderer>();
        warningObjectSr.color = invisibleColor;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(ActivateSpikesCoroutine());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StopAllCoroutines();
            warningObjectSr.DOKill();
            warningObject.SetActive(false);
            warningObjectSr.color = invisibleColor;
        }
    }

  
    public IEnumerator ActivateSpikesCoroutine()
    {
        warningObject.SetActive(true);
        warningObjectSr.DOColor(warningColor, initialDelay - warningDuration).SetEase(colorEase);
        yield return new WaitForSeconds(initialDelay - warningDuration);
        warningObject.SetActive(true);
        warningObjectSr.DOColor(killColor, warningDuration).SetEase(colorEase);
        yield return new WaitForSeconds(warningDuration);
        warningObject.SetActive(false);
        killObject.SetActive(true);
        killObject.transform.DOLocalMoveY(spikesYMove, moveDuration);
    }
}
