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
            foreach(LevelSaveData level in SaveGameManager.instance.levelSaveData)
            {
                LevelButtonUI levelButton = Instantiate(levelButtonPrefab, transform).GetComponent<LevelButtonUI>();

                levelButton.InitLevelButton(level.avaivable, level.levelName, level.bestTime, level.star);
            }
        }
    }
}
