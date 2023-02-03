using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Rectangle.General
{
    public class SaveGameManager : MonoBehaviour
    {
        public static SaveGameManager instance;

        public List<LevelSaveData> saveData;

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
            }

            if(PlayerPrefs.HasKey("LevelSaveData"))
            {
                LoadData();
            }
        }

        private void OnApplicationQuit()
        {
            if(saveData.Count > 0)
            {
                SaveData();
            }
        }

        public void SaveData()
        {
            string saveString = "";

            foreach(LevelSaveData data in saveData)
            {

                saveString += data.sceneName + ":";

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
            saveData = new();

            string saveString = PlayerPrefs.GetString("LevelSaveData");

            string[] lavelSaves = saveString.Split("|");

            foreach(string levelSave in lavelSaves)
            {
                string[] levelData = levelSave.Split(":");

                bool avaivable;
                bool star;

                if (levelData[1] == "true")
                {
                    avaivable = true;
                }
                else
                {
                    avaivable = false;
                }

                if (levelData[3] == "true")
                {
                    star = true;
                }
                else
                {
                    star = false;
                }

                LevelSaveData data = new LevelSaveData()
                {
                    sceneName= levelData[0],
                    avaivable = avaivable,
                    bestTime = float.Parse(levelData[2]),
                    star = star
                };

                saveData.Add(data);
            }
        }

        public void FinishLevel(string name, float time)
        { 
            
            for(int i = 0; i < saveData.Count; i++)
            {
                if (saveData[i].sceneName == name)
                {
                    if (saveData[i].bestTime < time)
                    {
                        saveData[i].bestTime = time;
                    }

                    if (i + 1 < saveData.Count)
                    {
                        saveData[i + 1].avaivable = true;
                    }
                    return;
                }
            }
        }

        public void CollectStar(string name)
        {
            foreach (LevelSaveData levelData in saveData)
            {
                if(levelData.sceneName == name)
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
        public string sceneName;
        public bool avaivable;
        public float bestTime;
        public bool star;
    }
}
