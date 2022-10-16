using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Rectangle.Level;
using Rectangle.General;

namespace Rectangle.Player
{
    /// <summary>
    /// Controls the movement and changing between modes of the player.
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        /// <summary>
        /// The list of the differnt modes (must have all 4 modes).
        /// </summary>
        [Tooltip("The list of the differnt modes (must have all 4 modes).")]
        [SerializeField] private PlayerMode[] playerModes = new PlayerMode[]
        {
            new PlayerMode(PlayerModes.Rectangle, null),
            new PlayerMode(PlayerModes.Bubble, null),
            new PlayerMode(PlayerModes.Spikey, null),
            new PlayerMode(PlayerModes.Little, null)
        };

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
        private CameraController camController;

        private PlayerInput playerInput;
        private PlayerInputActions inputActions;

        /// <summary>
        /// The enum with all player modes.
        /// </summary>
        public enum PlayerModes { None, Rectangle, Bubble, Spikey, Little}

        void Awake()
        {
            Debug.Log("ModeController: -> Awake()");

            inputActions = new PlayerInputActions();
            inputActions.Player.Enable();

            camController = Camera.main.GetComponent<CameraController>();
            InitModes();
            activePlayer = modes[PlayerModes.Rectangle];
            CheckBackground();
            Debug.Log("ModeController: <- Awake()   activePlayer = " + activePlayer.name);

        }

        void Update()
        {
            CheckBackground();
        }

        private void FixedUpdate()
        {
            Vector2 movement = inputActions.Player.Move.ReadValue<Vector2>();
            activePlayer.Move(movement);
        }

        private void CheckBackground()
        {
            BackgroundMode lastBackground = null;
            if (activeBackground != null)
            {
                lastBackground = activeBackground;
            }

            activeBackground = Physics2D.OverlapPoint(activePlayer.transform.position, backgroundLayer).GetComponent<BackgroundMode>();

            if (lastBackground != activeBackground)
            {
                ChangeMode(activeBackground.playerMode);
                camController.CameraTransition(activeBackground.transform.position);
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

            Debug.Log("ModeController: <- ChangeMode()");

        }

        public void Jump(InputAction.CallbackContext context)
        {
            if(context.performed)
            {
                activePlayer.Jump();
            }
        }

        public void Move(InputAction.CallbackContext context)
        {
            activePlayer.Move(context.ReadValue<Vector2>());
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
        public PlayerController.PlayerModes playerMode;
        /// <summary>
        /// The movement script of the player.
        /// </summary>
        [Tooltip("The fitting movement script of the player.")]
        public PlayerBase player;

        public PlayerMode(PlayerController.PlayerModes mode, PlayerBase playerMove)
        {
            playerMode = mode;
            player = playerMove;
        }
    }
}