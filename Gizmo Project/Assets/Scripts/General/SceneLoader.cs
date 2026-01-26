using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Rectangle.General
{
    /// <summary>
    /// Controls the changing between and reloading of scenes.
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        /// <summary>
        /// The name of the tutorial scene.
        /// </summary>
        [Tooltip("The name of the tutorial scene.")]
        [SerializeField] private string tutorialSceneName;

        /// <summary>
        /// The name of the level scene.
        /// </summary>
        [Tooltip("The name of the level scene.")]
        [SerializeField] private string levelSceneName;


        /// <summary>
        /// Reloads the scene with the next level.
        /// </summary>
        public void LoadNextLevel()
        {
            if(SaveGameManager.Singleton.saveData != null)
            {
                SaveGameManager.Singleton.SetNextLevel();
                if(SceneManager.GetActiveScene().name == tutorialSceneName)
                {
                    SceneManager.LoadScene(levelSceneName);
                }
                else
                {
                    ReloadScene();
                }
            }

        }

        /// <summary>
        /// Reloads the current scene.
        /// </summary>
        public void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Time.timeScale = 1;
        }

        /// <summary>
        /// Loads teh scene at the given index.
        /// </summary>
        /// <param name="index"></param>
        public void LoadSceneAtIndex(int index)
        {
            SceneManager.LoadScene(index);
            Time.timeScale = 1;
        }

        /// <summary>
        /// Quits the game.
        /// </summary>
        public void ExitGame()
        {
            Application.Quit();
        }
    }
}