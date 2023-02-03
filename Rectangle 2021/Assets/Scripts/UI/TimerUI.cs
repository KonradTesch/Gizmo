using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Rectangle.UI
{
    public class TimerUI : MonoBehaviour
    {
        /// <summary>
        /// The UI text for the timer.
        /// </summary>
        [Tooltip("The UI text for the timer.")]
        [SerializeField] private TextMeshProUGUI timeText;
        /// <summary>
        /// Wheter the timer start at the start of a scene.
        /// </summary>
        [Tooltip("Wheter the timer start at the start of a scene.")]
        [SerializeField] private bool startTimeAtStart;

        /// <summary>
        /// Wheter the timer is running.
        /// </summary>
        public static bool timer;
        [HideInInspector]public float time = 0;

        private bool badTime = false;

        void Start()
        {
            if (startTimeAtStart)
                timer = true;
            else
                timer = false;
        }

        void Update()
        {
            if (timer)
            {
                time += Time.deltaTime;

                int min = Mathf.FloorToInt(time / 60);
                int sec = Mathf.FloorToInt(time % 60);

                string currentTime = string.Format("{0:00}:{1:00}", min, sec);


                timeText.text = currentTime;

                if (General.SaveGameManager.instance.activeLevel != null && General.SaveGameManager.instance.activeLevel.bestTime > 0)
                {
                    if(time > General.SaveGameManager.instance.activeLevel.bestTime && !badTime)
                    {
                        General.GameBehavior.badTime();
                        badTime = true;
                    }
                }
            }
        }

        /// <summary>
        /// Stops the timer.
        /// </summary>
        public void StopTimer()
        {
            timer = false;
        }

        /// <summary>
        /// Resumes the timer.
        /// </summary>
        public void ResumeTimer()
        {
            timer = true;
        }
    }
}