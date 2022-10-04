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
        public static GameBehavior instance;

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
        /// The script, tht builds the tilemap level.
        /// </summary>
        [Tooltip("The script, tht builds the tilemap level.")]
        public LevelBuilder levelBuilder;

        /// <summary>
        /// The list of level tiles for the player mode.
        /// </summary>
        [Tooltip("The list of level tiles for the player mode.")]
        [SerializeField] private LevelTile[] levelTiles;

        /// <summary>
        /// The list of background colliders on the level grid.
        /// </summary>
        [Tooltip("The list of background colliders on the level grid.")]
        [SerializeField] private GameObject[] gridColliders;

        private TextMeshProUGUI startPlayText;
        private bool canStart;

        void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this);
                return;
            }

            startPlayText = startLevelButton.gameObject.GetComponentInChildren<TextMeshProUGUI>();

            startLevelButton.enabled = false;
            debugUI.GetComponent<DebugUI>().enabled = false;

            startPlayText.color = new Color(0, 0, 0, 0.3f);
            canStart = false;
        }

        /// <summary>
        /// Begins the actual level after the placement of the mode shapes.
        /// </summary>
        public void StartPlayMode()
        {
            Debug.Log("GameBehavior: -> StartPlayMode()");
            if (canStart)
            {
                levelBuilder.BuildLevel();

                debugUI.GetComponent<DebugUI>().enabled = true;
                TimerUI.timer = true;
                player.gameObject.SetActive(true);
                buttonUI.SetActive(false);
                levelCam.Priority = 2;
            }
            Debug.Log("GameBehavior: <- StartPlayMode()");
        }

        /// <summary>
        /// Checks if every gackground grid collider is covered with a mode shape.
        /// </summary>
        /// <returns></returns>
        public void CheckGridCollider()
        {
            if(levelBuilder.CheckLevel())
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

    }
}