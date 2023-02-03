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
        [SerializeField] private Sprite emptyCheckbox;
        [SerializeField] private Sprite fullCheckpox;

        [Header("Level Info")]
        [SerializeField] private Image levelCompleteCkeckbox;
        [SerializeField] private Image shortestWayCheckbox;
        [SerializeField] private TextMeshProUGUI starText;
        [SerializeField] private TextMeshProUGUI bestTimeText;

        void Start()
        {
            if(SaveGameManager.instance.activeLevel != null)
            {
                LevelSaveData level = SaveGameManager.instance.activeLevel;

                for(int i = 0; i < SaveGameManager.instance.levelSaveData.Count; i++)
                {
                    if (SaveGameManager.instance.levelSaveData[i].levelName == level.levelName && SaveGameManager.instance.levelSaveData.Count >= i + 2)
                    {
                        if (SaveGameManager.instance.levelSaveData[i + 1].avaivable)
                        {
                            levelCompleteCkeckbox.sprite = fullCheckpox;
                        }
                        else
                        {
                            levelCompleteCkeckbox.sprite = fullCheckpox;
                        }
                    }
                    break;
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
