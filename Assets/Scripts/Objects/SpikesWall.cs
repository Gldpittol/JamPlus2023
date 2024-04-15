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
    [SerializeField] private Sprite activatedSprite;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private SpriteRenderer killRenderer;
    [SerializeField] private SpriteRenderer defaultRenderer;
    [SerializeField] private Vector2 pressedOffset;

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
        killRenderer.color = killColor;

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
            /*warningObject.SetActive(false);
            warningObjectSr.color = invisibleColor;*/
            pressurePlateDefault.transform.DOKill();
            pressurePlateDefault.transform.DOLocalMoveY(originalPressureY, pressurePlateTweenDuration).OnComplete(SetAsNotPressed);
        }
    }

    public void SetAsPressed()
    {
        pressurePlateDefault.GetComponent<SpriteRenderer>().sprite = activatedSprite;
        pressurePlateDefault.GetComponent<SpriteRenderer>().sortingOrder = 45;

        if (transform.localEulerAngles.z == 0 || transform.localEulerAngles.z == 180 ||
            transform.localEulerAngles.z == -180)
        {
            pressurePlateDefault.transform.position += (Vector3)pressedOffset;
        }
        else if (transform.localEulerAngles.z == -90 || transform.localEulerAngles.z == 270 )
        {
            pressurePlateDefault.transform.position += new Vector3(pressedOffset.y, pressedOffset.x, 0);
        }
        else if (transform.localEulerAngles.z == 90 || transform.localEulerAngles.z == -270 )
        {
            pressurePlateDefault.transform.position += new Vector3(-pressedOffset.y, pressedOffset.x, 0);
        }
    }

    public void SetAsNotPressed()
    {
        pressurePlateDefault.GetComponent<SpriteRenderer>().sprite = defaultSprite;
        pressurePlateDefault.GetComponent<SpriteRenderer>().sortingOrder = 35;
        defaultRenderer.color = defaultColor;
    }

  
    public IEnumerator ActivateSpikesCoroutine()
    {
        pressurePlateDefault.transform.DOKill();
        pressurePlateDefault.transform.DOLocalMoveY(pressurePlateLowered.transform.localPosition.y, pressurePlateTweenDuration).OnComplete(SetAsPressed);
        /*warningObject.SetActive(true);
        warningObjectSr.color = defaultColor;*/
        StartCoroutine(DoWarningCoroutine());
        yield return new WaitForSeconds(warningDuration);
       // warningObject.SetActive(false);
        killObject.SetActive(true);
        pressurePlateDefault.SetActive(false);
        killObject.transform.DOLocalMoveY(spikesYMove, moveDuration);
        yield return new WaitForSeconds(1f);
        killObject.transform.DOLocalMoveY(originalKillY, moveDuration).OnComplete(DisableSpikes);
        pressurePlateDefault.transform.DOLocalMoveY(originalPressureY, pressurePlateTweenDuration).OnComplete(SetAsNotPressed);
    }

    public void DisableSpikes()
    {
        killObject.SetActive(false);
        pressurePlateDefault.SetActive(true);
        defaultRenderer.color = defaultColor;
    }

    public IEnumerator DoWarningCoroutine()
    {
        defaultRenderer.color = killColor;
        yield return new WaitForSeconds(warningDuration / 4);
        defaultRenderer.color = defaultColor;
        yield return new WaitForSeconds(warningDuration / 4);
        defaultRenderer.color = killColor;
        yield return new WaitForSeconds(warningDuration / 8);
        defaultRenderer.color = defaultColor;
        yield return new WaitForSeconds(warningDuration / 8);
        defaultRenderer.color = killColor;
        yield return new WaitForSeconds(warningDuration / 16);
        defaultRenderer.color = defaultColor;
        yield return new WaitForSeconds(warningDuration / 16);
        defaultRenderer.color = killColor;
        yield return new WaitForSeconds(warningDuration / 32);
        defaultRenderer.color = defaultColor;
        yield return new WaitForSeconds(warningDuration / 32);
    }
}
