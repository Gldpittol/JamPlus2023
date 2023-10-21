using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public enum AudioType
    {
        Jump,
        Collect
    }
    
    [System.Serializable]
    public struct AudioStruct
    {
        public AudioType type;
        public AudioClip clip;
        public float volume;
        public bool adaptativePitch;
        public float defaultPitch;
        public float maxPitch;
    }
    
    public static AudioManager Instance;

    public List<AudioStruct> audioList = new List<AudioStruct>();

    public void PlaySound(AudioType type)
    {
        AudioSource tempSource = gameObject.AddComponent<AudioSource>();
        AudioStruct audioStr = GetAudioStruct(type);
        AudioClip clip = audioStr.clip;
        
        if (clip == null)
        {
            print("No Clip Found For The Associated Type");
            return;
        }
        
        tempSource.clip = clip;
        if(audioStr.adaptativePitch) tempSource.pitch = audioStr.defaultPitch + ((audioStr.maxPitch - audioStr.defaultPitch) * ComboBar.Instance.GetPercentage());
        tempSource.volume = audioStr.volume;
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
