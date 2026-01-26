using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rectangle.General;

namespace Rectangle.UI
{
    /// <summary>
    /// Controls the pause menu.
    /// </summary>
    public class MenuUI : MonoBehaviour
    {
        /// <summary>
        /// The keyboard key to open the menu.
        /// </summary>
        [Tooltip("The keyboard key to open the menu.")]
        [SerializeField] private KeyCode menuKey;

        /// <summary>
        /// The panel object of the pause menu screen.
        /// </summary>
        [Tooltip("The panel object of the pause menu screen.")]
        [SerializeField] private GameObject menuPanel;

        [SerializeField] private GameObject gameOverPanel;

        [SerializeField] private TimerUI timerUI;

        void Update()
        {
            if (Input.GetKeyDown(menuKey))
            {
                if (menuPanel.activeSelf == true)
                    Resume();
                else
                    Pause();
            }
        }

        /// <summary>
        /// Pauses the game and activates the menu screen.
        /// </summary>
        public void Pause()
        {
            menuPanel.SetActive(true);
            Time.timeScale = 0;
        }

        /// <summary>
        /// Deactivates the menu screen and resumes the game.
        /// </summary>
        public void Resume()
        {
            menuPanel.SetActive(false);
            Time.timeScale = 1;
        }

        public void Respawn()
        {
            gameOverPanel.SetActive(false);
            GameBehavior.instance.player.PlayerRespawn();
            timerUI.ResumeTimer();
        }
    }
}