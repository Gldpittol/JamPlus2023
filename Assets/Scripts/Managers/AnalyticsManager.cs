using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Analytics;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance;

    private void Awake()
    {
        if (!Instance)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private async void Start()
    {
        await UnityServices.InitializeAsync();
        AnalyticsService.Instance.StartDataCollection();
    }

    public void SendAnalyticDeath()
    {
        
        CustomEvent myEvent = new CustomEvent("Died")
        {
            { "playerDeath", SceneManager.GetActiveScene().name},
        };
        AnalyticsService.Instance.RecordEvent(myEvent);
        
    }
}
