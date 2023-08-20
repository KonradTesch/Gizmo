using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rectangle.UI;
using Rectangle.General;

namespace Rectangle.Level
{
    public class SuccessTrigger : MonoBehaviour
    {
        /// <summary>
        /// Canvas Object of the success canvas.
        /// </summary>
        [Tooltip("The canvas Object of the success canvas.")]
        public GameObject successPanel;

        /// <summary>
        /// The script of thu UI timer.
        /// </summary>
        [Tooltip("The script of thu UI timer.")]
        public TimerUI timerUI;

        private bool alreadyPlayd = false;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player") && !alreadyPlayd)
            {
                StartCoroutine(nameof(StopPlayer));
                GameBehavior.instance.uiAudioSource.PlayOneShot(GameBehavior.instance.winSound);

                alreadyPlayd = true;

                GameBehavior.onPlayerWin();

                successPanel.SetActive(true);
            }

            timerUI.StopTimer();

            SaveGameManager.Singleton.FinishLevel(GameBehavior.instance.levelBuilder.levelData, timerUI.time, GameBehavior.instance.usedTilesNumber);
        }

        /// <summary>
        /// Stopt the player after finishing the level.
        /// </summary>
        private IEnumerator StopPlayer()
        {
            // a small delay, that the player can see that he reached the finish.
            yield return new WaitForSeconds(0.5f);
            GameBehavior.instance.player.playerActive = false;
        }
    }
}