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
    [SerializeField] private Color defaultColor ;
    [SerializeField] private Color killColor ;
    [SerializeField] private Ease colorEase ;
    [SerializeField] private float spikesYMove ;
    [SerializeField] private float moveDuration = 0.1f ;
    [SerializeField] private GameObject pressurePlateDefault ;
    [SerializeField] private GameObject pressurePlateLowered ;
    [SerializeField] private float pressurePlateTweenDuration = 0.15f;

    private SpriteRenderer warningObjectSr;
    private float originalPressureY;
    private float originalKillY;
    private void Awake()
    {
        originalPressureY = pressurePlateDefault.transform.localPosition.y;
        warningObject.SetActive(false);
        killObject.SetActive(false);
        warningObjectSr = warningObject.GetComponent<SpriteRenderer>();
        warningObjectSr.color = invisibleColor;
        originalKillY = killObject.transform.localPosition.y;
    }

    private void Start()
    {
        GameManager.Instance.onGameEnd += StopSpikes;
    }
    
    private void OnDestroy()
    {
        GameManager.Instance.onGameEnd -= StopSpikes;
    }

    public void StopSpikes()
    {
        StopAllCoroutines();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !PlayerMovement.Instance.isInvulnerable())
        {
            StartCoroutine(ActivateSpikesCoroutine());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !killObject.activeInHierarchy)
        {
            StopAllCoroutines();
            warningObjectSr.DOKill();
            warningObject.SetActive(false);
            warningObjectSr.color = invisibleColor;
            pressurePlateDefault.transform.DOKill();
            pressurePlateDefault.transform.DOLocalMoveY(originalPressureY, pressurePlateTweenDuration);
        }
    }

  
    public IEnumerator ActivateSpikesCoroutine()
    {
        pressurePlateDefault.transform.DOKill();
        pressurePlateDefault.transform.DOLocalMoveY(pressurePlateLowered.transform.localPosition.y, pressurePlateTweenDuration);
        warningObject.SetActive(true);
        warningObjectSr.color = defaultColor;
        StartCoroutine(DoWarningCoroutine());
        yield return new WaitForSeconds(warningDuration);
        warningObject.SetActive(false);
        killObject.SetActive(true);
        killObject.transform.DOLocalMoveY(spikesYMove, moveDuration);
        yield return new WaitForSeconds(1f);
        killObject.transform.DOLocalMoveY(originalKillY, moveDuration);
        pressurePlateDefault.transform.DOLocalMoveY(originalPressureY, pressurePlateTweenDuration);
    }

    public IEnumerator DoWarningCoroutine()
    {
        warningObjectSr.color = killColor;
        yield return new WaitForSeconds(warningDuration / 4);
        warningObjectSr.color = defaultColor;
        yield return new WaitForSeconds(warningDuration / 4);
        warningObjectSr.color = killColor;
        yield return new WaitForSeconds(warningDuration / 8);
        warningObjectSr.color = defaultColor;
        yield return new WaitForSeconds(warningDuration / 8);
        warningObjectSr.color = killColor;
        yield return new WaitForSeconds(warningDuration / 16);
        warningObjectSr.color = defaultColor;
        yield return new WaitForSeconds(warningDuration / 16);
        warningObjectSr.color = killColor;
        yield return new WaitForSeconds(warningDuration / 32);
        warningObjectSr.color = defaultColor;
        yield return new WaitForSeconds(warningDuration / 32);
    }
}
