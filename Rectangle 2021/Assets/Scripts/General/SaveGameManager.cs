using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rectangle.General
{
    public class SaveGameManager : MonoBehaviour
    {
        public List<LevelSaveData> saveData;

        public void SaveData()
        {
            string saveString = "";

            foreach(LevelSaveData data in saveData)
            {
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

           saveString.Remove(saveString.Length - 1);

            PlayerPrefs.SetString("LevelSaveData", saveString);
        }

        public void LoadData()
        {
            string saveString = PlayerPrefs.GetString("LevelSaveData");

            string[] lavelSaves = saveString.Split("|");

            foreach(string lavelSave in lavelSaves)
            {
                string[] levelData = lavelSave.Split(":");

                bool avaivable;
                bool star;

                if (levelData[0] == "true")
                {
                    avaivable = true;
                }
                else
                {
                    avaivable = false;
                }

                if (levelData[2] == "true")
                {
                    star = true;
                }
                else
                {
                    star = false;
                }

                LevelSaveData data = new LevelSaveData()
                {
                    avaivable = avaivable,
                    bestTime = float.Parse(levelData[1]),
                    star = star
                };

                saveData.Add(data);
            }
        }
    }

    public class LevelSaveData
    {
        public bool avaivable;
        public float bestTime;
        public bool star;
    }
}
