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

    private float originalVolume;
    private float defaultMusicVolume;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audSource = GetComponent<AudioSource>();
            defaultMusicVolume = audSource.volume;
            UpdateMusic();
            UpdateMusicVolume();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateMusic()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu" || SceneManager.GetActiveScene().name == "LevelSelect")
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
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            originalVolume = defaultMusicVolume * PlayerPrefs.GetFloat("MusicVolume");
        }
        else
        {
            originalVolume = defaultMusicVolume;
        }
        if (audSource.clip)
        {
            //print(clip.name);
            //print(audSource.clip.name);
            if (clip.name == audSource.clip.name) yield break;
        }
       
        audSource.DOFade(0, fadeDuration);
        yield return new WaitForSecondsRealtime(fadeDuration);
        audSource.clip = clip;
        audSource.loop = true;
        audSource.Play();
        audSource.DOFade(originalVolume, fadeDuration);
    }

    public void UpdateMusicVolume()
    {
        if (!PlayerPrefs.HasKey("MusicVolume")) audSource.volume = defaultMusicVolume;
        audSource.volume = defaultMusicVolume * PlayerPrefs.GetFloat("MusicVolume");
    }
}
