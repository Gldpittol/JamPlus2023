using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioClip menuClip;
    [SerializeField] private AudioClip gameClip;

    private AudioSource audSource;
    public static MusicManager Instance;

    public int id;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audSource = GetComponent<AudioSource>();
            UpdateMusic();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateMusic()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu" && id != 1)
        {
            audSource.clip = gameClip;
            id = 1;
        }
        else if (id != 0)
        {
            audSource.clip = menuClip;
            id = 0;
        }

        audSource.loop = true;
        audSource.Play();
    }
}
