using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectManager : MonoBehaviour
{
    public static LevelSelectManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void GoToLevel(string sceneName)
    {
        LoadingCanvas.Instance.GoToScene(sceneName);
    }
}