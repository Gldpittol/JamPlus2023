using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private string firstLevel;

    public void StartGame()
    {
        LoadingCanvas.Instance.GoToScene(firstLevel);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
