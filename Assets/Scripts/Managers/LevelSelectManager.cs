using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using DG.Tweening;
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

    public void EnableInteract()
    {
        canInteract = true;
    }

    private void CheckInputs()
    {
        if (Input.GetKeyDown(KeyCode.Joystick1Button1) || Input.GetKeyDown(KeyCode.R))
        {
            GoToMenu();
            canInteract = true;
        }


        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0))
        {
            canInteract = false;
            currentSelection.Clicked();
        }

        Vector2 axis = new Vector2( Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

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
}
