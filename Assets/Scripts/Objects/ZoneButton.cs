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
    [SerializeField] private string zoneBase;
    [SerializeField] private string zoneNumber;
    [SerializeField] private string requiredLevelToUnlock;
    [SerializeField] private bool alwaysUnlocked;
    [SerializeField] private TextMeshProUGUI zoneText;
    [SerializeField] private TextMeshProUGUI zoneText1;
    [SerializeField] private Sprite lockedSprite;
    
    private bool isUnlocked;
    private Button myButton;

    private void Awake()
    {
        myButton = GetComponent<Button>();
        zoneText.text = zoneNumber;
        zoneText1.text = zoneBase;
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
            zoneText.DOFade(0, 0);
            zoneText1.DOFade(0, 0);
            GetComponent<Image>().sprite = lockedSprite;
            GetComponent<Image>().SetNativeSize();
        }
    }
    
    public void Clicked()
    {
        if (!myButton.interactable) return;
        
        GetComponent<ScalePop>().PopOutAnimation();
        LevelSelectManager.Instance.OpenZoneScreen(zoneId);
    }
}
