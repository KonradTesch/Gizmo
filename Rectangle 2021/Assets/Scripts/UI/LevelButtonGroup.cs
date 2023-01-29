using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rectangle.General;

namespace Rectangle.UI
{
    public class LevelButtonGroup : MonoBehaviour
    {
        [SerializeField] private GameObject levelButtonPrefab; 

        void Start()
        {
            foreach(LevelSaveData data in SaveGameManager.instance.saveData)
            {
                LevelButtonUI levelButton = Instantiate(levelButtonPrefab, transform).GetComponent<LevelButtonUI>();

                levelButton.InitLevelButton(data.avaivable, data.sceneName, data.bestTime, data.star);
            }
        }
    }
}
