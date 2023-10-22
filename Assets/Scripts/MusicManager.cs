using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioClip menuClip;
    [SerializeField] private AudioClip gameClip;
    [SerializeField] private float fadeDuration = 0.6f;

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
            StartCoroutine(SwapSoundCoroutine(gameClip));
            id = 1;
        }
        else if (id != 0)
        {
            StartCoroutine(SwapSoundCoroutine(menuClip));
            id = 0;
        }

        audSource.loop = true;
        audSource.Play();
    }

    public IEnumerator SwapSoundCoroutine(AudioClip clip)
    {
        audSource.DOFade(0, fadeDuration);
        yield return new WaitForSeconds(fadeDuration);
        audSource.clip = clip;
        audSource.Play();
        audSource.DOFade(1, fadeDuration);
    }
}
