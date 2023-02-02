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
        [SerializeField] private TextMeshProUGUI levelName;
        [SerializeField] private TextMeshProUGUI bestTime;
        [SerializeField] private GameObject star;

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

            levelName.text = name;

            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);

            string timeString = string.Format("{0:00}:{1:00}",minutes, seconds);

            bestTime.text = timeString;

            if(starCollected)
            {
                star.SetActive(true);
            }
        }

        private void LoadScene()
        {
            SceneManager.LoadScene(levelName.text);
        }
    }
}
