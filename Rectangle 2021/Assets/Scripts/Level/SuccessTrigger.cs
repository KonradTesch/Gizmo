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

        public TimerUI timerUI;


        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                StartCoroutine(nameof(StopPlayer));
                successPanel.SetActive(true);
            }

            timerUI.StopTimer();

            SaveGameManager.instance.FinishLevel(GameBehavior.instance.levelBuilder.levelData, timerUI.time, GameBehavior.instance.usedTilesNumber);
        }

        private IEnumerator StopPlayer()
        {
            yield return new WaitForSeconds(0.25f);
            General.GameBehavior.instance.player.playerActive = false;
        }
    }
}