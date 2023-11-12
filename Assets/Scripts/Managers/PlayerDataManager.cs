using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    [Serializable]
    public class PlayerData
    {
        [Serializable]
        public class LevelData
        {
            public string levelName;
            public float highScore;
            public int starsAchieved;

            public LevelData()
            {
                levelName = "";
                highScore = 0;
                starsAchieved = 0;
            }
        }

        public List<LevelData> levelData = new List<LevelData>();
    }
    
    //ended class declarations
    
    public static PlayerDataManager Instance;

    [SerializeField] private PlayerData playerData = new PlayerData(); 

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UnlockLevel(string levelName)
    {
        if(CheckIfLevelIsAlreadyUnlocked(levelName)) return;
        
        playerData.levelData.Add(new PlayerData.LevelData());
        playerData.levelData[playerData.levelData.Count - 1].levelName = levelName;
        playerData.levelData[playerData.levelData.Count - 1].highScore = 0;
        playerData.levelData[playerData.levelData.Count - 1].starsAchieved = 0;
    }

    public bool CheckIfLevelIsAlreadyUnlocked(string levelName)
    {
        foreach (PlayerData.LevelData data in playerData.levelData)
        {
            if (data.levelName == levelName)
            {
                return true;
            }
        }

        return false;
    }

    public void ModifyLevel(string levelName, float highScore, int stars)
    {
        if (!CheckIfLevelIsAlreadyUnlocked(levelName))
        {
            playerData.levelData.Add(new PlayerData.LevelData());
            playerData.levelData[playerData.levelData.Count - 1].levelName = levelName;
            playerData.levelData[playerData.levelData.Count - 1].highScore = 0;
            playerData.levelData[playerData.levelData.Count - 1].starsAchieved = 0;
        }
        
        foreach (PlayerData.LevelData data in playerData.levelData)
        {
            if (data.levelName == levelName)
            {
                if (highScore > data.highScore)
                {
                    data.highScore = highScore;
                }
                if (stars > data.starsAchieved)
                {
                    data.starsAchieved = stars;
                }

                return;
            }
        }
    }

    public float GetHighScore(string levelName)
    {
        if (!CheckIfLevelIsAlreadyUnlocked(levelName))
        {
            return 0;
        }
        foreach (PlayerData.LevelData data in playerData.levelData)
        {
            if (data.levelName == levelName)
            {
                return data.highScore;
            }
        }
        return 0;
    }
    
    public int GetStarsAmount(string levelName)
    {
        if (!CheckIfLevelIsAlreadyUnlocked(levelName))
        {
            return 0;
        }
        foreach (PlayerData.LevelData data in playerData.levelData)
        {
            if (data.levelName == levelName)
            {
                return data.starsAchieved;
            }
        }
        return 0;
    }

    public bool CheckIfUnlocked(string levelName)
    {
        if (playerData.levelData.Count == 0) return false;

        foreach (PlayerData.LevelData data in playerData.levelData)
        {
            if (data.levelName == levelName)
            {
                return true;
            }
        }
        return false;
    }

    public void ResetData()
    {
        playerData = new PlayerData(); 
    }
}
