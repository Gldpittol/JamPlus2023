using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    public enum AudioType
    {
        Jump,
        Collect,
        Death,
        Star,
        Stamp,
        CatDeath,
        CatStamp,
        UISelect,
        UIConfirm,
        UIBack,
        Smoke,
        Bounce
    }
    
    [System.Serializable]
    public struct AudioStruct
    {
        public AudioType type;
        public AudioClip[] clip;
        public float volume;
        public bool adaptativePitch;
        public float defaultPitch;
        public float maxPitch;
        public float startPoint;
    }
    
    public static AudioManager Instance;

    public List<AudioStruct> audioList = new List<AudioStruct>();

    [SerializeField] private GameObject musicManager;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (!MusicManager.Instance)
        {
            Instantiate(musicManager);
        }
        else
        {
            MusicManager.Instance.UpdateMusic();
        }
    }

    public void PlaySound(AudioType type)
    {
        AudioSource tempSource = gameObject.AddComponent<AudioSource>();
        AudioStruct audioStr = GetAudioStruct(type);
        int rnd = Random.Range(0, audioStr.clip.Length);
        AudioClip clip = audioStr.clip[rnd];
        
        if (clip == null)
        {
            print("No Clip Found For The Associated Type");
            return;
        }
        
        tempSource.clip = clip;
        if(audioStr.adaptativePitch) tempSource.pitch = audioStr.defaultPitch + ((audioStr.maxPitch - audioStr.defaultPitch) * ComboBar.Instance.GetPercentage());
        if(PlayerPrefs.HasKey("AudioVolume"))
        {
            tempSource.volume = audioStr.volume * PlayerPrefs.GetFloat("AudioVolume");
        }
        else
        {
            tempSource.volume = audioStr.volume;
        }
        tempSource.time = audioStr.startPoint;
        tempSource.Play();
        Destroy(tempSource, clip.length);
    }

    public AudioStruct GetAudioStruct(AudioType type)
    {
        foreach (AudioStruct audio in audioList)
        {
            if (audio.type == type)
            {
                return audio;
            }
        }

        return default;
    }
}
