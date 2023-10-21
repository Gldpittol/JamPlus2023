using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private float countdownTime = 30f;
    [SerializeField] private float scoreRequiredPass = 100f;
    [SerializeField] private float scoreRequiredSilver = 200f;
    [SerializeField] private float scoreRequiredGold = 300f;

    [SerializeField] private string nextSceneName;

    [SerializeField] private float minAcceptableObstacleDistanceFromPlayer = 2f;
    [SerializeField] private GameObject obstaclesParent;
    [SerializeField] private List<string> possibleObstacleTags = new List<string>();

    private List<GameObject> obstaclesList = new List<GameObject>();
    private float score;

    [Header("Debug")] 
    [SerializeField] private bool useObstacle;

    private bool levelEnded = false;

    public bool LevelEnded => levelEnded;

    private void Awake()
    {
        Instance = this;
        
        InitializeObstacleList();
    }

    private void Update()
    {
        if (levelEnded) return;
        countdownTime -= Time.deltaTime;
        HUDManager.Instance.UpdateTimeText(countdownTime);

        if (countdownTime <= 0)
        {
            countdownTime = 0;
            HUDManager.Instance.UpdateTimeText(0);
            FinishLevel(false);
        }
    }

    public void FinishLevel(bool died)
    {
        levelEnded = true;
        if (died)
        {
            print("Died,Retrying!");

            SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
            return;
        }
        
        if (score >= scoreRequiredGold)
        {
            SceneManager.LoadScene(nextSceneName, LoadSceneMode.Single);
            print("Got Gold!");
        }
        else if (score >= scoreRequiredSilver)
        {
            print("Got Silver!");

            SceneManager.LoadScene(nextSceneName, LoadSceneMode.Single);
        }
        else if (score >= scoreRequiredPass)
        {
            print("Got Bronze!");

            SceneManager.LoadScene(nextSceneName, LoadSceneMode.Single);
        }
        else 
        {
            print("Failed Level, Retrying!");

            SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        }
    }

    public void InitializeObstacleList()
    {
        if (useObstacle)
        {
            obstaclesParent.transform.position = Vector2.zero;
        }
        foreach (Transform child in obstaclesParent.GetComponentsInChildren<Transform>(true))
        {
            if (possibleObstacleTags.Contains(child.tag))
            {
                obstaclesList.Add(child.gameObject);
            }
        }
    }

    public void UpdateScore()
    {
        score += ComboBar.Instance.GetComboMultiplier();
        HUDManager.Instance.UpdateScoreText(score);
    }

    public void SpawnObstacle(int iter = 0)
    {
        if (obstaclesList.Count == 0) return;
        if (iter > 50) return;
        int rngIndex = Random.Range(0, obstaclesList.Count);
        if (Vector2.Distance(obstaclesList[rngIndex].transform.position,
                PlayerMovement.Instance.transform.position) > minAcceptableObstacleDistanceFromPlayer)
        {
            obstaclesList[rngIndex].SetActive(true);
            obstaclesList.RemoveAt(rngIndex);
        }
        else
        {
            print(iter);
            SpawnObstacle(iter+1);
        }
    }
}
