using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private string levelName;
    [SerializeField] private bool alwaysUnlocked = false;

    private Button myBytton;
    private void Awake()
    {
        myBytton = GetComponent<Button>();
        myBytton.onClick.AddListener(Clicked);
    }

    private void Start()
    {
        CheckIfUnlocked();
    }

    public void CheckIfUnlocked()
    {
        if (alwaysUnlocked) return;

        myBytton.interactable = PlayerDataManager.Instance.CheckIfUnlocked(levelName);
    }

    public void Clicked()
    {
        LevelSelectManager.Instance.GoToLevel(levelName);
    }
}
