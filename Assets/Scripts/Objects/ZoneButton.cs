using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ZoneButton : MonoBehaviour
{
    [SerializeField] private int zoneId;
    [SerializeField] private string zoneName;
    [SerializeField] private string requiredLevelToUnlock;
    [SerializeField] private bool alwaysUnlocked;
    [SerializeField] private TextMeshProUGUI zoneText;
        
    private bool isUnlocked;
    private Button myButton;

    private void Awake()
    {
        myButton = GetComponent<Button>();
        zoneText.text = zoneName;
        myButton.onClick.AddListener(Clicked);
    }

    private void Start()
    {
        CheckIfUnlocked();
    }

    public void CheckIfUnlocked()
    {
        if (alwaysUnlocked) return;
        isUnlocked = PlayerDataManager.Instance.CheckIfUnlocked(requiredLevelToUnlock);
        myButton.interactable = isUnlocked;
        if (!isUnlocked)
        {
            zoneText.DOFade(0.5f, 0);
        }
    }
    
    public void Clicked()
    {
        if (!myButton.interactable) return;
        
        GetComponent<ScalePop>().PopOutAnimation();
        LevelSelectManager.Instance.OpenZoneScreen(zoneId);
    }
}
