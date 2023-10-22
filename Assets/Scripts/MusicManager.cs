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
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            StartCoroutine(SwapSoundCoroutine(menuClip));
        }
        else
        {
            StartCoroutine(SwapSoundCoroutine(gameClip));
        }
    }

    public IEnumerator SwapSoundCoroutine(AudioClip clip)
    {
        if (audSource.clip)
        {
            //print(clip.name);
            //print(audSource.clip.name);
            if (clip.name == audSource.clip.name) yield break;
        }
       
        audSource.DOFade(0, fadeDuration);
        yield return new WaitForSeconds(fadeDuration);
        audSource.clip = clip;
        audSource.loop = true;
        audSource.Play();
        audSource.DOFade(1, fadeDuration);
    }
}
