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
    

    public void SendAnalyticDeath(GameObject deathObject)
    {
        string deathObjectString;
        if(deathObject != null)
        {
            deathObjectString = deathObject.name + ": " + "X: " + deathObject.transform.position.x + " Y: " +
                                    deathObject.transform.position.y;
        }
        else
        {
            deathObjectString = "Time Ended";
        }

        CustomEvent myEvent = new CustomEvent("playerDeath")
        {
            { "scene", SceneManager.GetActiveScene().name},
            { "score", GameManager.Instance.Score},
            { "deathObject", deathObjectString},
        };
        AnalyticsService.Instance.RecordEvent(myEvent);
    }

    public void SendAnalyticLevelClear(int starAmount)
    {
        string sceneName = SceneManager.GetActiveScene().name;
        float highScore = GameManager.Instance.Score;
        if (PlayerDataManager.Instance.GetHighScore(sceneName) > highScore)
        {
            highScore = PlayerDataManager.Instance.GetHighScore(sceneName);
        }
        print(PlayerDataManager.Instance.GetTries(sceneName));
        CustomEvent myEvent = new CustomEvent("levelClear")
        {
            { "scene", sceneName},
            { "tries", PlayerDataManager.Instance.GetTries(sceneName)},
            { "hasCleared", starAmount > 0},
            { "score", GameManager.Instance.Score},
            { "highScore", highScore},
            { "starAmount", starAmount},
            { "jumps", PlayerMovement.Instance.jumps},
            { "firstTry", starAmount > 0 && PlayerDataManager.Instance.GetTries(sceneName) < 2},
        };
        AnalyticsService.Instance.RecordEvent(myEvent);
    }

    public void SendAnalyticsFinishedGame(float completionPercentage, int totalStars)
    {
        CustomEvent myEvent = new CustomEvent("finishedGame")
        {
            { "completionRate", completionPercentage },
            { "totalStars", totalStars},
            { "TotalPlaytime", PlayerDataManager.Instance.Playtime },
            { "languageGame", LocalizationSystem.language.ToString()}
        };
        
        AnalyticsService.Instance.RecordEvent(myEvent);
    }
}
