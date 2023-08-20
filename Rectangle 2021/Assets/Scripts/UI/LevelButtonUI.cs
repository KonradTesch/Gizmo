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
        /// <summary>
        /// The grayed backgrounds of the tile buttons.
        /// </summary>
        [Tooltip("The grayed backgrounds of the tile buttons.")]
        [SerializeField] private Sprite[] buttonBackgrounds;

        /// <summary>
        /// The text object for the level name.
        /// </summary>
        [Tooltip("The text object for the level name.")]
        [SerializeField] private TextMeshProUGUI levelNameUI;

        /// <summary>
        /// The text object for the best time.
        /// </summary>
        [Tooltip("The text object for the best time.")]
        [SerializeField] private TextMeshProUGUI bestTimeUI;

        /// <summary>
        /// The UI object for the collectbale item.
        /// </summary>
        [Tooltip("The UI object for the collectbale item.")]
        [SerializeField] private GameObject uiStar;

        private string levelName;
        private Button levelButton;

        private void Awake()
        {
            levelButton = GetComponent<Button>();
            levelButton.onClick.AddListener(LoadScene);
        }

        /// <summary>
        /// Initiates the level tile buttons.
        /// </summary>
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

        /// <summary>
        /// Reloads the level scene with the new active level(Or loads the tutorial scene).
        /// </summary>
        private void LoadScene()
        {
            General.SaveGameManager.Singleton.SetActiveLevel(levelName);
            if(levelName == "Tutorial")
            {
                SceneManager.LoadScene("TutorialScene");
            }
            else
            {
                SceneManager.LoadScene("LevelScene");
            }
        }
    }
}
