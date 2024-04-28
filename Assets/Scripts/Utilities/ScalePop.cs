using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ScalePop : MonoBehaviour
{
    [SerializeField] private bool getTransform = true;
    [SerializeField] Transform _transform;
    [SerializeField] private float popInDuration = 0.05f;
    [SerializeField] private float popOutDuration = 0.1f;
    [SerializeField] private Vector3 popOutScale = new Vector3(1.1f, 1.1f, 1.1f);

    [Header("Elastic Animation")]
    [SerializeField] float inDuration = 0.14f;
    [SerializeField] float outDuration = 0.6f;
    [SerializeField] private Vector3 elasticScale = new Vector3(1.1f, 1.1f, 1.1f);

    private Vector3 _originalScale;
    private Vector3 targetOriginalScale;

    private void Awake()
    {
        if (getTransform) _transform = GetComponent<Transform>();
        _originalScale = transform.localScale;
        targetOriginalScale = _transform.localScale;
    }

    public void PopOutAnimation(bool ignoreTimescale = false)
    {
        transform.localScale = _originalScale;

        transform.DOKill();
        _transform.DOScale(popOutScale, popInDuration).SetUpdate(true).OnComplete(() => PopOutFinish(ignoreTimescale));
    }

    public void PopOutFinish(bool ignoreTimescale = false)
    {
        _transform.DOScale(targetOriginalScale, popOutDuration).SetUpdate(ignoreTimescale);
    }

    public void ElasticPop()
    {
        _transform.DOScale(elasticScale, inDuration).OnComplete(() => _transform.DOScale(targetOriginalScale, outDuration).SetEase(Ease.OutElastic));
    }
}
