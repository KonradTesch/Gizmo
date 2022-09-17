using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }
}