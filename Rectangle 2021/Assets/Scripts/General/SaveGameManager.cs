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

        [SerializeField] private string endSceneName;

        [Header("Level")]

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

                saveString += ":";

                if(data.shortestWay)
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

            string saveString = PlayerPrefs.GetString("LevelSaveData");

            string[] levelSaves = saveString.Split("|");

            string[] testData = levelSaves[0].Split(":");

            if (levelSaves.Length < 10 || testData.Length < 6)
            {

                PlayerPrefs.DeleteKey("LevelSaveData");
                return;
            }

            levelSaveData = new();

            foreach (string levelSave in levelSaves)
            {
                string[] levelData = levelSave.Split(":");

                LevelData level = Resources.Load<LevelData>(levelData[1]);

                bool avaivable;
                bool star;
                bool shortest;

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

                if (levelData[5] == "true")
                {
                    shortest = true;
                }
                else
                {
                    shortest = false;
                }


                LevelSaveData data = new LevelSaveData()
                {
                    levelName = levelData[0],
                    levelData = level,
                    avaivable = avaivable,
                    bestTime = float.Parse(levelData[3]),
                    star = star,
                    shortestWay = shortest
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
                    if(i + 2 < levelSaveData.Count)
                    {
                        activeLevel = levelSaveData[i + 1];
                    }
                    else
                    {
                        SceneManager.LoadScene(endSceneName);
                    }
                    return;
                }
            }
        }

        public void SetlastLevel()
        {
            for(int i = 0; i < levelSaveData.Count; i++)
            {
                if(i + 1 < levelSaveData.Count && !levelSaveData[i + 1].avaivable)
                {
                    activeLevel = levelSaveData[i];
                    SceneManager.LoadScene("LevelScene");
                    return;
                }
            }

            activeLevel = levelSaveData[levelSaveData.Count - 1];
            SceneManager.LoadScene("LevelScene");
        }

        public void FinishLevel(LevelData level, float time, int usedTileNumber)
        { 
            
            for(int i = 0; i < levelSaveData.Count; i++)
            {
                if (levelSaveData[i].levelData == level)
                {
                    if (levelSaveData[i].bestTime < time)
                    {
                        levelSaveData[i].bestTime = time;
                    }

                    if (usedTileNumber <= level.shortestWay)
                    {
                        levelSaveData[i].shortestWay = true;
                    }

                    if (i + 1 < levelSaveData.Count)
                    {
                        levelSaveData[i + 1].avaivable = true;
                    }
                    else
                    {
                        SceneManager.LoadScene(endSceneName);
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
        public bool shortestWay;
    }
}
