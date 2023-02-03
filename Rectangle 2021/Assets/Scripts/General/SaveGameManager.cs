using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rectangle.LevelCreation;

namespace Rectangle.General
{
    public class SaveGameManager : MonoBehaviour
    {
        public static SaveGameManager instance;

        public List<LevelSaveData> levelSaveData;

        [HideInInspector] public LevelSaveData activeLevel;

        private void Awake()
        {
            if(instance != null)
            {
                Destroy(this);
            }
            else
            {
                instance = this;
                DontDestroyOnLoad(this);
                if(PlayerPrefs.HasKey("LevelSaveData"))
                {
                    LoadData();
                }
            }
        }

        private void OnApplicationQuit()
        {
            if(levelSaveData.Count > 0)
            {
                SaveData();
            }
        }

        public void SaveData()
        {
            string saveString = "";

            foreach(LevelSaveData data in levelSaveData)
            {

                saveString += data.levelName + ":";

                saveString += data.levelData.name + ":";

                if(data.avaivable)
                {
                    saveString += "true";
                }
                else
                {
                    saveString += "false";
                }

                saveString += ":";

                saveString += data.bestTime.ToString();

                saveString += ":";

                if(data.star)
                {
                    saveString += "true";
                }
                else
                {
                    saveString += "false";
                }

                saveString += "|";
            }

            saveString = saveString.Substring(0, saveString.Length - 1);

            PlayerPrefs.SetString("LevelSaveData", saveString);
        }

        private void LoadData()
        {
            levelSaveData = new();

            string saveString = PlayerPrefs.GetString("LevelSaveData");

            string[] lavelSaves = saveString.Split("|");

            foreach(string levelSave in lavelSaves)
            {
                string[] levelData = levelSave.Split(":");

                LevelData level = Resources.Load<LevelData>(levelData[1]);

                bool avaivable;
                bool star;

                if (levelData[2] == "true")
                {
                    avaivable = true;
                }
                else
                {
                    avaivable = false;
                }

                if (levelData[4] == "true")
                {
                    star = true;
                }
                else
                {
                    star = false;
                }

                LevelSaveData data = new LevelSaveData()
                {
                    levelName = levelData[0],
                    levelData = level,
                    avaivable = avaivable,
                    bestTime = float.Parse(levelData[3]),
                    star = star
                };

                this.levelSaveData.Add(data);
            }
        }

        public void SetActiveLevel(string levelName)
        {
            foreach(LevelSaveData level in levelSaveData)
            {
                if(level.levelName == levelName)
                {
                    activeLevel = level;
                    return;
                }
            }
        }

        public void SetNextLevel()
        {
            SaveData();
            for (int i = 0; i < levelSaveData.Count; i++)
            {
                if (levelSaveData[i].levelName == activeLevel.levelName)
                {
                    activeLevel = levelSaveData[i +1];
                    return;
                }
            }
        }

        public void FinishLevel(LevelData level, float time)
        { 
            
            for(int i = 0; i < levelSaveData.Count; i++)
            {
                if (levelSaveData[i].levelData == level)
                {
                    if (levelSaveData[i].bestTime < time)
                    {
                        levelSaveData[i].bestTime = time;
                    }

                    if (i + 1 < levelSaveData.Count)
                    {
                        levelSaveData[i + 1].avaivable = true;
                    }
                    return;
                }
            }
        }

        public void CollectStar(LevelData level)
        {
            foreach (LevelSaveData levelData in levelSaveData)
            {
                if(levelData.levelData == level)
                {
                    levelData.star = true;
                    return;
                }
            }
        }
    }

    [System.Serializable]
    public class LevelSaveData
    {
        public string levelName;
        public LevelCreation.LevelData levelData;
        public bool avaivable;
        public float bestTime;
        public bool star;
    }
}
