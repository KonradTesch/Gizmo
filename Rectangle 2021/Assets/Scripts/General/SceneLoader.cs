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
        /// reloads the scene with the next level
        /// </summary>
        public void LoadNextLevel()
        {
            if(SaveGameManager.instance.levelSaveData != null)
            {
                SaveGameManager.instance.SetNextLevel();
                if(SceneManager.GetActiveScene().name == "TutorialScene")
                {
                    SceneManager.LoadScene("LevelScene");
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