using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikesWall : MonoBehaviour
{
    [SerializeField] private float delayInBetween = 5f;
    [SerializeField] private float onScreenDuration = 1f;
    [SerializeField] private float warningDuration = 2f;
    [SerializeField] private GameObject warningObject ;
    [SerializeField] private GameObject killObject;

    private void Start()
    {
        ActivateSpikes();
    }

    public void ActivateSpikes()
    {
        StartCoroutine(ActivateSpikesCoroutine());
    }

    public IEnumerator ActivateSpikesCoroutine()
    {
        yield return new WaitForSeconds(delayInBetween - warningDuration);
        warningObject.SetActive(true);
        yield return new WaitForSeconds(warningDuration);
        warningObject.SetActive(false);
        killObject.SetActive(true);
        yield return new WaitForSeconds(onScreenDuration);
        warningObject.SetActive(false);
        killObject.SetActive(false);
        ActivateSpikes();
    }
}
