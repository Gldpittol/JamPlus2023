using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Serialization;

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
        public List<int> seenTutorialsList = new List<int>();

        public PlayerData()
        {
            levelData = new List<LevelData>();
            seenTutorialsList = new List<int>();
        }

        public PlayerData(PlayerData data)
        {
            levelData = data.levelData;
            seenTutorialsList = data.seenTutorialsList;
        }
    }
    
    public static PlayerDataManager Instance;
    
    [SerializeField] private PlayerData playerData = new PlayerData(); 
    [SerializeField] private string path;
    [SerializeField] private string saveName;
    [SerializeField] private string extensionType;
    
    [Header("Debug")]
    [SerializeField] private bool debugMode = true;

    private BinaryFormatter formatter;
    private string currentPath;
    
    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Update()
    {
        if (!debugMode) return;
        
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadGame();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            SaveGame();
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            ResetData();
        }
    }
    
    public void Initialize()
    {
        playerData = new PlayerData();
        formatter = new BinaryFormatter();
        LoadGame();
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

    public void AddSeenTutorial(int hash)
    {
        playerData.seenTutorialsList.Add(hash);
    }

    public bool CheckIfSeenTutorial(int hash)
    {
        return playerData.seenTutorialsList.Contains(hash);
    }

    public float GetCompletionPercentage()
    {
        float totalStars = 0;
        float receivedStars = 0;
        
        foreach (PlayerData.LevelData data in playerData.levelData)
        {
            if (data.levelName != "LevelSelect" && data.levelName != "MainMenu")
            {
                totalStars += 3;
                receivedStars += data.starsAchieved;
            }
        }

        return receivedStars / totalStars;
    }
    
    public void LoadGame()
    {
        if (!Directory.Exists(Application.persistentDataPath + path))
        {
            if(debugMode)print("No Save Found");
            SaveGame();
            return;
        }
        
        currentPath = Application.persistentDataPath + path + saveName + extensionType;
        FileStream file = File.Open(currentPath, FileMode.Open);
        PlayerData tempData = formatter.Deserialize(file) as PlayerData;
        playerData = new PlayerData(tempData);
        file.Close();
        if(debugMode)print("Loaded");
    }

    public void SaveGame()
    {
        if(debugMode) print(Application.persistentDataPath + path);

        if (!Directory.Exists(Application.persistentDataPath + path))
        {
            Directory.CreateDirectory(Application.persistentDataPath + path);
            playerData = new PlayerData();
            if(debugMode)print("Creating First Save");
        }
        
        currentPath = Application.persistentDataPath + path + saveName + extensionType;
        FileStream file = File.Create(currentPath);
        formatter.Serialize(file, playerData);
        file.Close();
        if(debugMode) print("Saved");
    }
    public void ResetData()
    {
        playerData = new PlayerData();
        SaveGame();
        LoadGame();
        if(debugMode) print("Resetting");
    }
}
