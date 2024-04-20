using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class LevelSelectManager : MonoBehaviour
{
    public static LevelSelectManager Instance;
    [SerializeField] private GameObject backButton;
    [SerializeField] private List<LevelButton> levelButtonList = new List<LevelButton>();
    [SerializeField] private float delay = 0.2f;
    [SerializeField] private int verticalIncrement = 5;
    [SerializeField] private Vector2 selectionSizeSmall;
    [SerializeField] private Vector2 selectionSizeBig;
    [SerializeField] private float sizeChangeDuration = 0.2f;
    [SerializeField] private float deadZone = 0.8f;
    [SerializeField] private GameObject winScreen;

    [Header("Zones")] 
    [SerializeField] private List<GameObject> zoneList = new List<GameObject>();
    [SerializeField] private GameObject defaultZonePanel;

    private float currentDelayVertical = 0;
    private float currentDelayHorizontal = 0;

    private bool canInteract = true;
    private LevelButton currentSelection = null;
    private int currentId = -1;
    private bool updatedSelection = false;
    private int oldId = -1;

    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (GameManager.cameFromLastLevel)
        {
            GameManager.cameFromLastLevel = false;
            if(winScreen) winScreen.SetActive(true);
        }
    }

    private void Update()
    {
        if (!canInteract) return;
        currentDelayVertical -= Time.deltaTime;
        currentDelayHorizontal -= Time.deltaTime;
        CheckInputs();
    }

    private void LateUpdate()
    {
        if (currentId == oldId) return;
        if (updatedSelection)
        {
            if (currentId != -1 && oldId != -1)
            {
                levelButtonList[oldId].transform.DOScale(selectionSizeSmall, sizeChangeDuration);
            }

            levelButtonList[currentId].transform.DOScale(selectionSizeBig, sizeChangeDuration);
            
            updatedSelection = false;
            oldId = currentId;
        }
    }

    public void DisableWinScreen()
    {
        winScreen.SetActive(false);
    }

    public void EnableInteract()
    {
        canInteract = true;
    }

    private void CheckInputs()
    {
        if (winScreen && winScreen.activeInHierarchy)
        {
            if(Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Joystick1Button1))
            {
                winScreen.SetActive(false);
            }
            return;
        }
        
        if (Input.GetKeyDown(KeyCode.Joystick1Button1) || Input.GetKeyDown(KeyCode.R))
        {
            GoToMenu();
            canInteract = true;
        }
        
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0))
        {
            if (!currentSelection) return;
            canInteract = false;
            currentSelection.Clicked();
        }

        Vector2 axis = new Vector2( Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D) ||
            Input.GetKeyDown(KeyCode.A))
        {
            axis = new Vector2( Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }
        
        bool xNegative = axis.x < 0;
        bool yNegative = axis.y < 0;
        axis = new Vector2(Mathf.Abs(axis.x) > deadZone ? 1 : 0, Mathf.Abs(axis.y) > deadZone ? 1 : 0);
        axis = new Vector2(xNegative ? axis.x * -1 : axis.x * 1, yNegative ? axis.y * -1 : axis.y * 1);
        
        if (currentId == -1 && axis != Vector2.zero)
        {
            currentId = 0;
            currentSelection = levelButtonList[0];
            currentDelayHorizontal = delay;
            currentDelayVertical = delay;
            updatedSelection = true;
            return;
        }
        
        if (axis.x > 0 && currentDelayHorizontal <=0)
        {
            currentId++;
            currentDelayHorizontal = delay;
            if (currentId >= levelButtonList.Count)
            {
                currentId = levelButtonList.Count - 1;
            }
            currentSelection = levelButtonList[currentId];
            updatedSelection = true;
        }
        
        if (axis.x < 0 && currentDelayHorizontal <=0)
        {
           
            currentId--;
            currentDelayHorizontal = delay;
            if (currentId < 0)
            {
                currentId = 0;
            }
            currentSelection = levelButtonList[currentId];
            updatedSelection = true;
        }
        
        if (axis.y < 0 && currentDelayVertical <=0)
        {
            currentId+=verticalIncrement;
            currentDelayVertical = delay;
            if (currentId >= levelButtonList.Count)
            {
                currentId = levelButtonList.Count - 1;
            }
            currentSelection = levelButtonList[currentId];
            updatedSelection = true;
        }
        
        if (axis.y > 0 && currentDelayVertical <=0)
        {
           
            currentId -= verticalIncrement;
            currentDelayVertical = delay;
            if (currentId < 0)
            {
                currentId = 0;
            }
            currentSelection = levelButtonList[currentId];
            updatedSelection = true;
        }
    }

    public void GoToLevel(string sceneName)
    {
        GameManager.Instance.GoToScene(sceneName);
    }

    public void GoToMenu()
    {
        backButton.GetComponent<ScalePop>().PopOutAnimation();
        GameManager.Instance.GoToMainMenu();
    }

    public void OpenZoneScreen(int zoneId)
    {
        LoadingCanvas.Instance.SwapUiPanel(defaultZonePanel, zoneList[zoneId]);
    }

    public void CloseZoneScreen(int zoneId)
    {
        LoadingCanvas.Instance.SwapUiPanel(zoneList[zoneId], defaultZonePanel);
    }
}
