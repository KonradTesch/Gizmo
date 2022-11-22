using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rectangle.UI;

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
        /// TimerUI script of the UI timer.
        /// </summary>
        [Tooltip("The TimerUI script of the UI timer.")]
        public TimerUI timerUI;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                successPanel.SetActive(true);
            }

            timerUI.StopTimer();
        }
    }
}