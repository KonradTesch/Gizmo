using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Rectangle.UI
{
    /// <summary>
    /// Controls the main menu and its transition animation.
    /// </summary>
    public class MainMenu : MonoBehaviour
    {
        /// <summary>
        /// The main camera.
        /// </summary>
        [Tooltip("The main camera.")]
        [SerializeField] private Camera cam;

        /// <summary>
        /// The panel of the main menu.
        /// </summary>
        [Tooltip("The panel of the main menu.")]
        [SerializeField] private RectTransform mainMenuPanel;

        /// <summary>
        /// The time of the transition.
        /// </summary>
        [Tooltip("The time (in seconds) of the transition.")]
        [SerializeField] private float transitionTime;

        /// <summary>
        /// The curve for the transtion speed over timer.
        /// </summary>
        [Tooltip("The curve for the transtion speed over timer.")]
        [SerializeField] private AnimationCurve transitionCurve;

        private float panelTimer;
        private bool panelTransition;
        private float panelStartPos;
        private float panelEndPos;

        private float camTimer;
        private bool camTransition;
        private float camStartPos;
        private float camEndPos;

        private void Awake()
        {
            Time.timeScale = 1;
        }
        void Update()
        {
            if (panelTransition)
            {
                panelTimer += Time.deltaTime;
                mainMenuPanel.anchoredPosition = new Vector2(Transition(panelStartPos, panelEndPos, transitionTime), 0);

                if (panelTimer > transitionTime)
                {
                    panelTransition = false;
                }

            }

            if (camTransition)
            {
                camTimer += Time.deltaTime;
                cam.transform.position = new Vector3(Transition(camStartPos, camEndPos, transitionTime), cam.transform.position.y, cam.transform.position.z);

                if (camTimer > transitionTime)
                {
                    camTransition = false;
                }
            }
        }

        /// <summary>
        /// Calculates the position (x-axis) between start and end, based on the timer and animation curve.
        /// </summary>
        /// <param name="start"> Start position (x-axis).</param>
        /// <param name="end">End position (x-axis).</param>
        /// <param name="duration">Complete transition time.</param>
        /// <returns>position at the current time.</returns>
        public float Transition(float start, float end, float duration)
        {
            if (panelTimer > duration)
                return end;

            float range = end - start;
            float time = panelTimer / duration;
            float percent = transitionCurve.Evaluate(time);

            return start + (range * percent);
        }

        /// <summary>
        /// Loads the scene with the given name.
        /// </summary>
        /// <param name="sceneName"> The scene name.</param>
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        /// <summary>
        /// Start the transition of the ui panel.
        /// </summary>
        /// <param name="end"></param>
        public void StartPanelTransition(float end)
        {
            panelTimer = 0;
            panelStartPos = mainMenuPanel.anchoredPosition.x;
            panelEndPos = end;
            panelTransition = true;
        }

        /// <summary>
        /// Starts the transition of the camera.
        /// </summary>
        /// <param name="end"></param>
        public void StartCamTransition(float end)
        {
            camTimer = 0;
            camStartPos = cam.transform.position.x;
            camEndPos = end;
            camTransition = true;

        }
    }
}