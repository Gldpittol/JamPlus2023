using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Shuriken : MonoBehaviour
{
    [SerializeField] private List<GameObject> childList;
    [SerializeField] private float duration = 4f;
    [SerializeField] private float angle = 700;

    private void Start()
    {
        DoRotate();
        transform.parent = null;
        foreach (GameObject child in childList)
        {
            child.transform.parent = null;
        }


        float furthestDistance = 0;
        int furthestId = 0;
        for (int i = 0; i < childList.Count; i++)
        {
            if (Vector2.Distance(PlayerMovement.Instance.transform.position, childList[i].transform.position) >
                furthestDistance)
            {
                furthestDistance = Vector2.Distance(PlayerMovement.Instance.transform.position,
                    childList[i].transform.position);
                furthestId = i;
            }
        }
        transform.position = childList[furthestId].transform.position;
        DoMove(furthestId);
    }

    private void Update()
    {
        if (GameManager.Instance.GetTime() <= 0)
        {
            transform.DOKill();
            StopAllCoroutines();
            enabled = false;
        }
    }

    public void DoRotate()
    {
        transform.DOLocalRotate(new Vector3(0,0,angle), duration, RotateMode.FastBeyond360).
            SetEase(Ease.Linear).OnComplete(()=>DoRotate());
    }

    public void DoMove(int i = 0)
    {
        int newId = (i + 1) % childList.Count;
        float maxDistance = 0;
        for (int j = 0; j < childList.Count; j++)
        {
            int nextJ = (j + 1) % childList.Count;
            if(Vector2.Distance(childList[j].transform.position, childList[nextJ].transform.position)> maxDistance)
            {
                maxDistance = Vector2.Distance(childList[j].transform.position, childList[nextJ].transform.position);
            }
        }

        float ratio = Vector2.Distance(childList[newId].transform.position, childList[i].transform.position) /
                      maxDistance;       
        transform.DOLocalMove(childList[newId].transform.position, duration * ratio).SetEase(Ease.Linear).OnComplete(() => DoMove(newId));
    }
}
