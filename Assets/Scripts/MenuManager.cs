using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private string firstLevel;
    [SerializeField] private GameObject firstPart;
    [SerializeField] private GameObject secondPart;
    [SerializeField] private GameObject creditsPanel;

    public void StartGame()
    {
        LoadingCanvas.Instance.GoToScene(firstLevel);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void EnableSecondPart()
    {
        firstPart.SetActive(false);
        secondPart.SetActive(true);
    }

    public void OpenCredits(bool creditsState)
    {
        creditsPanel.SetActive(creditsState);
    }
}
