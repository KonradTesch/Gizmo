using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TMPro;

namespace Rectangle.UI
{
    public class LevelButtonUI : MonoBehaviour
    {
        [SerializeField] private Sprite[] buttonBackgrounds;
        [SerializeField] private TextMeshProUGUI levelNameUI;
        [SerializeField] private TextMeshProUGUI bestTimeUI;
        [SerializeField] private GameObject uiStar;

        private string levelName;
        private Button levelButton;

        private void Awake()
        {
            levelButton = GetComponent<Button>();
            levelButton.onClick.AddListener(LoadScene);
        }

        public void InitLevelButton(bool avaivable, string name, float time, bool starCollected)
        {
            levelButton.image.sprite = buttonBackgrounds[Random.Range(0, buttonBackgrounds.Length)];

            levelButton.interactable = avaivable;

            levelName = name;
            levelNameUI.text = name;

            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);

            string timeString = string.Format("{0:00}:{1:00}",minutes, seconds);

            bestTimeUI.text = timeString;

            if(starCollected)
            {
                uiStar.SetActive(true);
            }
        }

        private void LoadScene()
        {
            General.SaveGameManager.instance.SetActiveLevel(levelName);

            SceneManager.LoadScene("LevelScene");
        }
    }
}
