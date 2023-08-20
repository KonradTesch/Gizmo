using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Rectangle.General;

namespace Rectangle
{
    public class InfoPanel : MonoBehaviour
    {
        [Header("Checkbox")]

        /// <summary>
        /// The sprite of the empty checkbox.
        /// </summary>
        [Tooltip("The sprite of the empty checkbox.")]
        [SerializeField] private Sprite emptyCheckbox;

        /// <summary>
        /// The sprite of the checked checkbox.
        /// </summary>
        [Tooltip("The sprite of the checked checkbox.")]
        [SerializeField] private Sprite fullCheckpox;

        [Header("Level Info")]

        /// <summary>
        /// The UI element of the checkbox, that shows, that the has been completed before.
        /// </summary>
        [Tooltip("The UI element of the checkbox, that shows, that the has been completed before.")]
        [SerializeField] private Image levelCompleteCkeckbox;

        /// <summary>
        /// The UI element of the checkbox, that shows, that the level has  been completed with the shortest amount of tiles.
        /// </summary>
        [Tooltip("The UI element of the checkbox, that shows, that the level has  been completed with the shortest amount of tiles.")]
        [SerializeField] private Image shortestWayCheckbox;

        /// <summary>
        /// The text object that shows the amount of colllected items.
        /// </summary>
        [Tooltip("The text object that shows the amount of colllected items.")]
        [SerializeField] private TextMeshProUGUI starText;

        /// <summary>
        /// The text object that shows the best time.
        /// </summary>
        [Tooltip("The text object that shows the best time.")]
        [SerializeField] private TextMeshProUGUI bestTimeText;

        void Start()
        {
            if(SaveGameManager.Singleton.activeLevel != null)
            {
                LevelSaveData level = SaveGameManager.Singleton.activeLevel;

                for(int i = 0; i < SaveGameManager.Singleton.saveData.levelSaveData.Count; i++)
                {
                    if (SaveGameManager.Singleton.saveData.levelSaveData[i].levelName == level.levelName && SaveGameManager.Singleton.saveData.levelSaveData.Count >= i + 2)
                    {
                        if (SaveGameManager.Singleton.saveData.levelSaveData[i + 1].avaivable)
                        {
                            levelCompleteCkeckbox.sprite = fullCheckpox;
                        }
                        else
                        {
                            levelCompleteCkeckbox.sprite = emptyCheckbox;
                        }
                        break;
                    }
                }

                if(level.shortestWay)
                {
                    shortestWayCheckbox.sprite = fullCheckpox;
                }
                else
                {
                    shortestWayCheckbox.sprite = emptyCheckbox;
                }

                if(level.star)
                {
                    starText.text = "1/1";
                }
                else
                {
                    starText.text = "0/1";
                }

                if(level.bestTime > 0)
                {
                    string time;

                    int minutes = Mathf.FloorToInt(level.bestTime / 60);
                    int seconds = Mathf.FloorToInt(level.bestTime % 60);


                    time = string.Format("{0:00}:{1:00}", minutes, seconds);

                    bestTimeText.text = time;
                }
                else
                {
                    bestTimeText.text = "--:--";
                }
            }
        }

    }
}
