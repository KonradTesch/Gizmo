using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Rectangle.Level;

namespace Rectangle.Player
{
    /// <summary>
    /// Controls the changing between modes of the player.
    /// </summary>
    public class ModeController : MonoBehaviour
    {
        /// <summary>
        /// The list of the differnt modes (must have all 4 modes).
        /// </summary>
        [Tooltip("The list of the differnt modes (must have all 4 modes).")]
        [SerializeField] private PlayerMode[] playerModes = new PlayerMode[]
        {
            new PlayerMode(PlayerModes.Rectanle, null),
            new PlayerMode(PlayerModes.Bubble, null),
            new PlayerMode(PlayerModes.Spikey, null),
            new PlayerMode(PlayerModes.Little, null)
        };

        /// <summary>
        /// The virtual camera that follows the player.
        /// </summary>
        [Tooltip("The virtual camera that follows the player.")]
        [SerializeField] private CinemachineVirtualCamera virtualCam;

        /// <summary>
        /// The layer for the background colliders.
        /// </summary>
        [Tooltip("The layer for the background colliders.")]

        [SerializeField] private LayerMask backgroundLayer;

        /// <summary>
        /// The player that is active at the moment.
        /// </summary>
        [HideInInspector] public PlayerBase activePlayer;

        private BackgroundMode activeBackground;
        private Dictionary<PlayerModes, PlayerBase> modes;

        /// <summary>
        /// The enum with all player modes.
        /// </summary>
        public enum PlayerModes { None, Rectanle, Bubble, Spikey, Little}

        void Awake()
        {
            Debug.Log("ModeController: -> Awake()");
            InitModes();
            activePlayer = modes[PlayerModes.Rectanle];
            Debug.Log("ModeController: <- Awake()   activePlayer = " + activePlayer.name);

        }

        void Update()
        {
            PlayerModes lastMode = PlayerModes.None;
            if (activeBackground != null)
            {
                lastMode = activeBackground.playerMode;
            }

            activeBackground = Physics2D.OverlapPoint(activePlayer.transform.position, backgroundLayer).GetComponent<BackgroundMode>();

            if(lastMode != activeBackground.playerMode)
            {
                ChangeMode(activeBackground.playerMode);
            }
        }

        /// <summary>
        /// Changes the player to a different mode.
        /// </summary>
        /// <param name="mode">
        /// The mode to change to.
        /// </param>
        public void ChangeMode(PlayerModes mode)
        {
            Debug.Log($"ModeController: -> ChangeMode(mode = {mode})");

            Vector3 currentPosition = activePlayer.transform.position;
            Vector2 currentVelocity = activePlayer.GetComponent<Rigidbody2D>().velocity;
            activePlayer.gameObject.SetActive(false);

            activePlayer = modes[mode];

            activePlayer.gameObject.transform.SetPositionAndRotation(currentPosition, Quaternion.identity);
            activePlayer.gameObject.SetActive(true);
            activePlayer.GetComponent<Rigidbody2D>().velocity = currentVelocity;


            virtualCam.Follow = activePlayer.transform;
            virtualCam.LookAt = activePlayer.transform;

            if(mode == PlayerModes.Little)
            {
                virtualCam.m_Lens.OrthographicSize = 2.5f;
            }
            else
            {
                virtualCam.m_Lens.OrthographicSize = 5f;
            }
            Debug.Log("ModeController: <- ChangeMode()");

        }

        /// <summary>
        /// Initiates the modes dictiornary.
        /// </summary>
        private void InitModes()
        {
            Debug.Log("ModeController: -> InitModes()");
            modes = new();

            foreach(PlayerMode mode in playerModes)
            {
                if(!modes.ContainsKey(mode.playerMode))
                {
                    modes.Add(mode.playerMode, mode.player);
                }
            }
            Debug.Log("ModeController: <- InitModes()");

        }
    }

    [System.Serializable]
    public class PlayerMode
    {
        /// <summary>
        /// The mode of the player.
        /// </summary>
        [Tooltip("The mode of the player.")]
        public ModeController.PlayerModes playerMode;
        /// <summary>
        /// The movement script of the player.
        /// </summary>
        [Tooltip("The fitting movement script of the player.")]
        public PlayerBase player;

        public PlayerMode(ModeController.PlayerModes mode, PlayerBase playerMove)
        {
            playerMode = mode;
            player = playerMove;
        }
    }
}