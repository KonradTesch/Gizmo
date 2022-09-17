using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;
using UnityEngine.UI;

using Rectangle.Player;
using Rectangle.UI;
using Rectangle.Level;

namespace Rectangle.General
{
    /// <summary>
    /// Controls the general game mechanics.
    /// </summary>
    public class GameBehavior : MonoBehaviour
    {
        [Header("Player")]

        /// <summary>
        /// The player mode controller.
        /// </summary>
        [Tooltip("The player mode controller.")]
        [SerializeField] private ModeController player;

        [Header("UI")]

        /// <summary>
        /// The UI canvas for gebugging.
        /// </summary>
        [Tooltip("The UI canvas for gebugging.")]
        [SerializeField] private GameObject debugUI;

        /// <summary>
        /// The UI canvas with the button at the begin of a level.
        /// </summary>
        [Tooltip("The UI canvas with the button at the begin of a level.")]
        [SerializeField] private GameObject buttonUI;

        /// <summary>
        /// The button to start the level.
        /// </summary>
        [Tooltip("The button to start the level.")]
        [SerializeField] private Button startLevelButton;

        [Header("Camera")]

        /// <summary>
        /// The virtual Ccamera that follows the player.
        /// </summary>
        [Tooltip("The virtual Ccamera that follows the player.")]
        [SerializeField] private CinemachineVirtualCamera levelCam;

        [Header("Level")]

        /// <summary>
        /// The list of shapes for the player moe.
        /// </summary>
        [Tooltip("The list of shapes for the player moe.")]
        [SerializeField] private BackgroundMode[] modeShapes;

        /// <summary>
        /// The list of background colliders on the level grid.
        /// </summary>
        [Tooltip("The list of background colliders on the level grid.")]
        [SerializeField] private GameObject[] gridColliders;

        private TextMeshProUGUI startPlayText;
        private bool canStart;

        void Awake()
        {
            startPlayText = startLevelButton.gameObject.GetComponentInChildren<TextMeshProUGUI>();

            startLevelButton.enabled = false;
            debugUI.GetComponent<DebugUI>().enabled = false;
            foreach (BackgroundMode bg in modeShapes)
            {
                bg.enabled = false;
            }
        }

        void Update()
        {
            if (CheckGridCollider())
            {
                startPlayText.color = Color.black;
                startLevelButton.enabled = true;
                canStart = true;

            }
            else
            {
                startPlayText.color = new Color(0, 0, 0, 0.3f);
                canStart = false;
                startLevelButton.enabled = false;

            }

        }

        /// <summary>
        /// Begins the actual level after the placement of the mode shapes.
        /// </summary>
        public void StartPlayMode()
        {
            Debug.Log("GameBehavior: -> StartPlayMode()");
            if (canStart)
            {
                debugUI.GetComponent<DebugUI>().enabled = true;
                TimerUI.timer = true;
                player.gameObject.SetActive(true);
                buttonUI.SetActive(false);
                levelCam.Priority = 2;
                foreach (BackgroundMode bg in modeShapes)
                {
                    bg.gameObject.GetComponent<ModeShape>().enabled = false;
                    bg.enabled = true;
                }
            }
            Debug.Log("GameBehavior: <- StartPlayMode()");
        }

        /// <summary>
        /// Checks if every gackground grid collider is covered with a mode shape.
        /// </summary>
        /// <returns></returns>
        bool CheckGridCollider()
        {
            foreach (GameObject obj in gridColliders)
            {
                if (!obj.GetComponent<IsGridUsed>().isUsed)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Sets all mode shapes to there correct positions.
        /// </summary>
        public void ShapesToCorrectPosition()
        {
            foreach (BackgroundMode bg in modeShapes)
            {
                Vector2 pos = bg.gameObject.GetComponentInParent<ModeGroup>().correctPosition;
                bg.gameObject.transform.parent.position = pos;
                bg.gameObject.transform.parent.localScale = Vector3.one;
                bg.gameObject.GetComponent<ModeShape>().enabled = false;


            }

            foreach (GameObject obj in gridColliders)
            {
                obj.GetComponent<IsGridUsed>().isUsed = true;
            }

        }
    }
}