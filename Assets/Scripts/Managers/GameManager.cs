using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Wilberforce;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static bool cameFromLastLevel = false;
    public enum GameState
    {
        Gameplay,
        Tutorial,
        GameEnded
    }

    public event Action onGameEnd;

    [SerializeField] private float coinBaseScore = 500f;
    [SerializeField] private float countdownTime = 30f;
    [SerializeField] private float scoreRequiredPass = 100f;
    [SerializeField] private float scoreRequiredSilver = 200f;
    [SerializeField] private float scoreRequiredGold = 300f;
    [SerializeField] public float delayBeforeGoingToNextLevel = 1f;
    [SerializeField] private float playerDeathAngle = 10f;
    [SerializeField] private Vector2 playerDeathAddPosition;
    [SerializeField] private Color arrowStartColor;
    [SerializeField] private Color arrowEndColor;
    [SerializeField] private float flashThresholdPercentage = 0.3f;
    [SerializeField] private float minShake = 1f;
    [SerializeField] private float maxShake = 3f;
    [SerializeField] private bool returnToLevelSelect = false;
    [SerializeField] private TextMeshProUGUI tempText;

    [SerializeField] private List<string> sceneNames = new List<string>();

    [SerializeField] private float minAcceptableObstacleDistanceFromPlayer = 2f;
    [SerializeField] private GameObject obstaclesParent;
    [SerializeField] private List<string> possibleObstacleTags = new List<string>();
    [SerializeField] private GameObject loadingCanvas;
    [SerializeField] public GameObject pauseButtonPos;

    public event Action OnUpdateLanguage;
    public static Language language = Language.English;

    private List<GameObject> obstaclesList = new List<GameObject>();
    private float score;

    [Header("Prefabs")]
    [SerializeField] private GameObject playerDataManager;
    [SerializeField] private GameObject screenClickManager;

    [Header("Debug")] 
    [SerializeField] private bool doDebugs;
    [SerializeField] private string debugLevelSelectName;
    [SerializeField] private bool useObstacle;

    public GameState gameState = GameState.Gameplay;

    private bool isFlashing = false;
    public bool hasStarted = false;
    private int starsUnlocked;
    private Coroutine screenShake;

    public bool playerDied = false;
    public float Score => score;
    private float originalCountdown;

    public static int colorBlindID = 0;
    private bool hasColorblindAsset;
    
    private void Awake()
    {
        originalCountdown = countdownTime;
        
        Instance = this;
        
        InitializeObstacleList();

        QualitySettings.vSyncCount = 1;

        InstantiatePrefabs();
    }

    private void Start()
    {
        UpdateLanguage();
        
        SetColorblindMode();
    }

    private void Update()
    {
        if (gameState == GameState.GameEnded) return;
        if (!hasStarted) return;
        countdownTime -= Time.deltaTime;
        
        UpdateHUD();

        if (doDebugs)
        {
            DoDebugs();   
        }
    }

    public void SetColorblindMode()
    {
        Colorblind cb = Camera.main.GetComponent<Colorblind>();

        if (!cb && colorBlindID != 0)
        {
            Camera.main.AddComponent<Colorblind>();
            Camera.main.GetComponent<Colorblind>().Type = colorBlindID;
        }

        else if (cb && colorBlindID == 0)
        {
            cb.enabled = false;
        }
        
        else  if (cb && colorBlindID != 0)
        {
            cb.enabled = true;
        }

        if (cb != null && cb.enabled)
        {
            Camera.main.GetComponent<Colorblind>().Type = colorBlindID;
        }
    }

    public void NextColorblindMode()
    {
        colorBlindID++;
        if (colorBlindID == 4) colorBlindID = 0;
        SetColorblindMode();
    }
    
    public void UpdateLanguage()
    {
        LocalizationSystem.language = language;
        OnUpdateLanguage?.Invoke();
        PlayerPrefs.SetInt("Language", (int)language);
    }
    
    public void StartGame()
    {
        hasStarted = true;
        if(PlayerMovement.Instance) PlayerMovement.Instance.SetArrowCountdown(arrowStartColor, arrowEndColor, countdownTime);
    }


    public void DoDebugs()
    {
        /*if (Input.GetKeyDown(KeyCode.L))
        {
            LoadingCanvas.Instance.GoToScene(debugLevelSelectName);
        }*/
        
        if (Input.GetKey(KeyCode.Joystick1Button4))
        {
            if (Input.GetKeyDown(KeyCode.Joystick1Button5))
            {
                LoadingCanvas.Instance.GoToScene(debugLevelSelectName);
            }
        }
        
        if (Input.GetKey(KeyCode.Joystick1Button5))
        {
            if (Input.GetKeyDown(KeyCode.Joystick1Button4))
            {
                LoadingCanvas.Instance.GoToScene(debugLevelSelectName);
            }
        }
    }

    public void InstantiatePrefabs()
    {
        if (!PlayerDataManager.Instance)
        {
            if(playerDataManager) Instantiate(playerDataManager);
        }
        if (!ScreenClickManager.Instance)
        {
            if(screenClickManager) Instantiate(screenClickManager);
        }
        Instantiate(loadingCanvas, Vector3.zero, Quaternion.identity);
    }

    public void UpdateHUD()
    {
        if (!HUDManager.Instance) return;
        
        HUDManager.Instance.UpdateTimeText(countdownTime);

        if (countdownTime <= 0)
        {
            countdownTime = 0;
            HUDManager.Instance.UpdateTimeText(0);
            FinishLevel(false);
        }

        if ((countdownTime / originalCountdown < flashThresholdPercentage) && !isFlashing)
        {
            isFlashing = true;
            PlayerMovement.Instance.SetArrowFlash();
        }
    }

    public void FinishLevel(bool died)
    {
        playerDied = died;
        gameState = GameState.GameEnded;
        StartCoroutine(FinishLevelCoroutine(died));
        onGameEnd?.Invoke();
    }
    
    public IEnumerator FinishLevelCoroutine(bool died)
    {
        PlayerMovement.Instance.DisableArrow();
        if (died)
        {
            SpinPlayer();
        }
        yield return new WaitForSeconds(delayBeforeGoingToNextLevel);

        if (died)
        {
            HUDManager.Instance.EnableFinalText(false, 0);
            yield break;
        }
        
        if (score >= scoreRequiredGold)
        {
            starsUnlocked = 3;
            HUDManager.Instance.EnableFinalText(true, 3);
          //  print("Got Gold!");
        }
        else if (score >= scoreRequiredSilver)
        {
            //print("Got Silver!");
            starsUnlocked = 2;

            HUDManager.Instance.EnableFinalText(true, 2);
        }
        else if (score >= scoreRequiredPass)
        {
//            print("Got Bronze!");
            starsUnlocked = 1;

            HUDManager.Instance.EnableFinalText(true, 1);
        }
        else 
        {
           // print("Failed Level, Retrying!");

           starsUnlocked = 0;

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
            playerDeathAddPosition = new Vector2(Mathf.Abs(playerDeathAddPosition.x), Mathf.Abs(playerDeathAddPosition.y));
            if (PlayerMovement.Instance.transform.position.x < 0)
            {
                playerDeathAddPosition.x *= -1;
            }
            if (PlayerMovement.Instance.transform.position.y < 0)
            {
                playerDeathAddPosition.y *= -1;
            }
            
            PlayerMovement.Instance.transform.position += (Vector3)playerDeathAddPosition * Time.deltaTime;
            i += Time.deltaTime / delayBeforeGoingToNextLevel;
            yield return null;
        }
    }

    public void InitializeObstacleList()
    {
        if (!obstaclesParent) return;
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
        score += coinBaseScore * ComboBar.Instance.GetComboMultiplier();
//        Debug.Log(score);
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
        string currentSceneName = SceneManager.GetActiveScene().name;
        int nextSceneId = -1;
        nextSceneId = sceneNames.IndexOf(currentSceneName) + 1;
        if (nextSceneId == sceneNames.Count)
        {
            nextSceneId = 1; //index of level select
            cameFromLastLevel = true;
        }

        if (!next)
        {
            if(starsUnlocked > 0) PlayerDataManager.Instance.UnlockLevel(sceneNames[nextSceneId]);
            PlayerDataManager.Instance.ModifyLevel(SceneManager.GetActiveScene().name, score, starsUnlocked);
            LoadingCanvas.Instance.GoToScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            PlayerDataManager.Instance.UnlockLevel(sceneNames[nextSceneId]);
            PlayerDataManager.Instance.ModifyLevel(SceneManager.GetActiveScene().name, score, starsUnlocked);
            if (returnToLevelSelect)
            {
                nextSceneId = 1; //index of level select
            
            }
            LoadingCanvas.Instance.GoToScene(sceneNames[nextSceneId]);
        }
    }

    public void UnlockNext()
    {
        if (starsUnlocked == 0) return;

        string currentSceneName = SceneManager.GetActiveScene().name;
        int nextSceneId = -1;
        nextSceneId = sceneNames.IndexOf(currentSceneName) + 1;
        if (nextSceneId == sceneNames.Count)
        {
            nextSceneId = 1; //index of level select
            cameFromLastLevel = true;
        }

        PlayerDataManager.Instance.UnlockLevel(sceneNames[nextSceneId]);
        PlayerDataManager.Instance.ModifyLevel(SceneManager.GetActiveScene().name, score, starsUnlocked);
    }

    public void GoToMainMenu()
    {
        LoadingCanvas.Instance.GoToScene(sceneNames[0]); //index of menu screen
    }

    public void GoToScene(string sceneName)
    {
        LoadingCanvas.Instance.GoToScene(sceneName);
    }

    public float GetTime()
    {
        return countdownTime;
    }

    public void DoScreenShake()
    {
        if (screenShake != null)
        {
            StopCoroutine(screenShake);
            Camera.main.transform.eulerAngles = Vector3.zero;
        }
        screenShake = StartCoroutine(DoScreenShakeCoroutine());
    }

    public IEnumerator DoScreenShakeCoroutine()
    {
        float shakeFactor = minShake + (ComboBar.Instance.GetPercentage() * (maxShake - minShake));
        Camera.main.transform.eulerAngles = new Vector3 (0,0, shakeFactor);
        yield return new WaitForSecondsRealtime(0.02f);
        Camera.main.transform.eulerAngles = Vector3.zero;
        yield return new WaitForSecondsRealtime(0.02f);
        Camera.main.transform.eulerAngles = new Vector3 (0,0, -shakeFactor);
        yield return new WaitForSecondsRealtime(0.02f);
        Camera.main.transform.eulerAngles = Vector3.zero;
    }
}
