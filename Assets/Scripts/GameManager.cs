using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
    [SerializeField] private float delayBeforeGoingToNextLevel = 1f;
    [SerializeField] private float playerDeathAngle = 10f;
    [SerializeField] private Vector2 playerDeathAddPosition;
    [SerializeField] private Color arrowStartColor;
    [SerializeField] private Color arrowEndColor;
    [SerializeField] private float flashThreshold = 8f;
    [SerializeField] private float minShake = 1f;
    [SerializeField] private float maxShake = 3f;

    [SerializeField] private string nextSceneName;

    [SerializeField] private float minAcceptableObstacleDistanceFromPlayer = 2f;
    [SerializeField] private GameObject obstaclesParent;
    [SerializeField] private List<string> possibleObstacleTags = new List<string>();
    [SerializeField] private GameObject loadingCanvas;

    private List<GameObject> obstaclesList = new List<GameObject>();
    private float score;

    [Header("Debug")] 
    [SerializeField] private bool useObstacle;

    private bool levelEnded = false;
    private bool isFlashing = false;

    public bool LevelEnded => levelEnded;
    public float Score => score;
    
    private void Awake()
    {
        Instance = this;
        
        InitializeObstacleList();

        QualitySettings.vSyncCount = 1;

        Instantiate(loadingCanvas, Vector3.zero, Quaternion.identity);
    }

    private void Start()
    {
        PlayerMovement.Instance.SetArrowCountdown(arrowStartColor, arrowEndColor, countdownTime);
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

        if (countdownTime < flashThreshold && !isFlashing)
        {
            isFlashing = true;
            PlayerMovement.Instance.SetArrowFlash();
        }
    }

    public void FinishLevel(bool died)
    {
        levelEnded = true;
        StartCoroutine(FinishLevelCoroutine(died));
    }

    public IEnumerator FinishLevelCoroutine(bool died)
    {
        PlayerMovement.Instance.DisableArrow();
        if (died)
        {
            SpinPlayer();
        }
        yield return new WaitForSeconds(delayBeforeGoingToNextLevel);

        /*if (died)
        {
            print("Died,Retrying!");

            HUDManager.Instance.EnableFinalText(false, 0);
            yield break;
        }*/
        
        if (score >= scoreRequiredGold)
        {
            HUDManager.Instance.EnableFinalText(true, 3);
            print("Got Gold!");
        }
        else if (score >= scoreRequiredSilver)
        {
            print("Got Silver!");

            HUDManager.Instance.EnableFinalText(true, 2);
        }
        else if (score >= scoreRequiredPass)
        {
            print("Got Bronze!");

            HUDManager.Instance.EnableFinalText(true, 1);
        }
        else 
        {
            print("Failed Level, Retrying!");

            HUDManager.Instance.EnableFinalText(false, 0);
        }
    }

    public void SpinPlayer()
    {
        StartCoroutine(SpinPlayerCoroutine());
    }

    public IEnumerator SpinPlayerCoroutine()
    {
        PlayerMovement.Instance.Die(playerDeathAngle, delayBeforeGoingToNextLevel);
        float i = 0;

        while (i < 1)
        {
            PlayerMovement.Instance.transform.position += (Vector3)playerDeathAddPosition * Time.deltaTime;
            i += Time.deltaTime / delayBeforeGoingToNextLevel;
            yield return null;
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
        StarsManager.Instance.UpdateStars(score, scoreRequiredPass,scoreRequiredSilver,scoreRequiredGold);
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
//            print(iter);
            SpawnObstacle(iter+1);
        }
    }

    public void LoadNextScene(bool next)
    {
        if (!next)
        {
            LoadingCanvas.Instance.GoToScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            LoadingCanvas.Instance.GoToScene(nextSceneName);
        }
    }

    public float GetTime()
    {
        return countdownTime;
    }

    public void DoScreenShake()
    {
        StartCoroutine(DoScreenShakeCoroutine());
    }

    public IEnumerator DoScreenShakeCoroutine()
    {
        float shakeFactor = minShake + (ComboBar.Instance.GetPercentage() * (maxShake - minShake));
        Camera.main.transform.eulerAngles = new Vector3 (0,0, shakeFactor);
        yield return null;
        Camera.main.transform.eulerAngles = Vector3.zero;
        yield return null;
        Camera.main.transform.eulerAngles = new Vector3 (0,0, -shakeFactor);
        yield return null;
        Camera.main.transform.eulerAngles = Vector3.zero;
    }
}
