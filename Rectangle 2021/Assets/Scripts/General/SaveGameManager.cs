using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rectangle.LevelCreation;
using System.IO;
using System.Xml.Linq;

namespace Rectangle.General
{
    public class SaveGameManager : MonoBehaviour
    {
        public static SaveGameManager Singleton;

        [Header("Save File")]

        /// <summary>
        /// The name of the svae file.
        /// </summary>
        [Tooltip("The name of the svae file.")]        
        [SerializeField] private string saveFileName;

        /// <summary>
        /// The code word to encrypt the save file.
        /// </summary>
        [Tooltip("TThe code word to encrypt the save file.")]
        [SerializeField] private readonly string encryptionCodeWord = "84216109";

        [Header("Scene Names")]

        /// <summary>
        /// The name of the tutorial scene.
        /// </summary>
        [Tooltip("The name of the tutorial scene.")]
        [SerializeField] private string tutorialSceneName;

        /// <summary>
        /// The name of the level scene.
        /// </summary>
        [Tooltip("The name of the level scene.")]
        [SerializeField] private string levelSceneName;

        /// <summary>
        /// The name of the end scene.
        /// </summary>
        [Tooltip("The name of the end scene.")]
        [SerializeField] private string endSceneName;

        [Header("Level")]

        [SerializeField] private LevelData[] levelOrder;

        /// <summary>
        /// The game save data.
        /// </summary>
        [Tooltip("The game save data.")]
        public SaveData saveData;

        /// <summary>
        /// The currently played level.
        /// </summary>
        [HideInInspector] public LevelSaveData activeLevel;

        private void Awake()
        {
            if(Singleton != null)
            {
                Destroy(this);
            }
            else
            {
                Singleton = this;
                DontDestroyOnLoad(this);

                LoadData();
            }
        }

        private void OnApplicationQuit()
        {
            if(saveData != null)
            {
                SaveData();
            }
        }

        /// <summary>
        /// Saves the level progression.
        /// </summary>
        public void SaveData()
        {

            string fullPath = Path.Combine(Application.persistentDataPath, saveFileName);

            string saveDataString = JsonUtility.ToJson(saveData);

            saveDataString = EncryptDecrypt(saveDataString);

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(saveDataString);
                }
            }
        }

        /// <summary>
        /// Loads the level data from the player prefs
        /// </summary>
        private void LoadData()
        {
            string fullPath = Path.Combine(Application.persistentDataPath, saveFileName);


            if (File.Exists(fullPath))
            {
                string loadData;

                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        loadData = reader.ReadToEnd();
                    }
                }

                loadData = EncryptDecrypt(loadData);

                saveData = JsonUtility.FromJson<SaveData>(loadData);

            }

            if(saveData == null || saveData.levelSaveData.Count != levelOrder.Length)
            {
                saveData = new SaveData();
                saveData.levelSaveData = new List<LevelSaveData>();

                for (int i = 0; i < levelOrder.Length; i++)
                {
                    LevelSaveData newLevelSaveData = new LevelSaveData();
                    newLevelSaveData.levelData = levelOrder[i];

                    if (i <= 1)
                    {
                        newLevelSaveData.avaivable = true;
                    }

                    saveData.levelSaveData.Add(newLevelSaveData);
                }

                SaveData();
            }
        }

        /// <summary>
        /// Sets the active level.
        /// </summary>
        public void SetActiveLevel(string levelName)
        {
            foreach(LevelSaveData level in saveData.levelSaveData)
            {
                if(level.levelName == levelName)
                {
                    activeLevel = level;
                    return;
                }
            }
        }

        /// <summary>
        /// Sets the active level to the next level.
        /// </summary>
        public void SetNextLevel()
        {
            SaveData();
            for (int i = 0; i < saveData.levelSaveData.Count; i++)
            {
                if (saveData.levelSaveData[i].levelName == activeLevel.levelName)
                {
                    if(i + 2 < saveData.levelSaveData.Count)
                    {
                        activeLevel = saveData.levelSaveData[i + 1];
                    }
                    else
                    {
                        SceneManager.LoadScene(endSceneName);
                    }
                    return;
                }
            }
        }

        /// <summary>
        /// Sets the active level to the last level, that isn't completed.
        /// </summary>
        public void SetlastLevel()
        {
            if (saveData.levelSaveData[0].bestTime == 0)
            {
                activeLevel = saveData.levelSaveData[0];
                SceneManager.LoadScene(tutorialSceneName);
                return;
            }

            for (int i = 0; i < saveData.levelSaveData.Count; i++)
            {
                if(i + 1 < saveData.levelSaveData.Count && !saveData.levelSaveData[i + 1].avaivable)
                {
                    activeLevel = saveData.levelSaveData[i];
                    SceneManager.LoadScene(levelSceneName);
                    return;
                }
            }

            activeLevel = saveData.levelSaveData[saveData.levelSaveData.Count - 1];
            SceneManager.LoadScene(levelSceneName);
        }

        /// <summary>
        /// Updates the data of a finished level.
        /// </summary>
        public void FinishLevel(LevelData level, float time, int usedTileNumber)
        { 
            
            for(int i = 0; i < saveData.levelSaveData.Count; i++)
            {
                if (saveData.levelSaveData[i].levelData == level)
                {
                    if (saveData.levelSaveData[i].bestTime < time)
                    {
                        saveData.levelSaveData[i].bestTime = time;
                    }

                    if (usedTileNumber <= level.shortestWay)
                    {
                        saveData.levelSaveData[i].shortestWay = true;
                    }

                    if (i + 1 < saveData.levelSaveData.Count)
                    {
                        saveData.levelSaveData[i + 1].avaivable = true;
                    }
                    else
                    {
                        SceneManager.LoadScene(endSceneName);
                        Destroy(GameBehavior.instance);
                        Destroy(this);
                    }

                    return;
                }
            }
        }

        /// <summary>
        /// Updates the data of a level, after collecting a star (or nut).
        /// </summary>
        public void CollectItem(LevelData level)
        {
            foreach (LevelSaveData levelData in saveData.levelSaveData)
            {
                if(levelData.levelData == level)
                {
                    levelData.star = true;
                    return;
                }
            }
        }

        /// <summary>
        /// Encrypt or Decrypt the save data.
        /// </summary>
        private string EncryptDecrypt(string data)
        {
            string modifiedData = "";

            for (int i = 0; i < data.Length; i++)
            {
                modifiedData += (char)(data[i] ^ encryptionCodeWord[i % encryptionCodeWord.Length]);
            }

            return modifiedData;
        }

    }


    /// <summary>
    /// The save data for all levels
    /// </summary>
    [System.Serializable]
    public class SaveData
    {
        public List<LevelSaveData> levelSaveData;
    }


    /// <summary>
    /// The save data for a single level.
    /// </summary>
    [System.Serializable]
    public class LevelSaveData
    {
        public string levelName;
        public LevelData levelData;
        public bool avaivable;
        public float bestTime;
        public bool star;
        public bool shortestWay;
    }
}
