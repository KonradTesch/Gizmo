using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rectangle.Player
{
    /// <summary>
    /// The base scipt for the player movement.
    /// </summary>
    public class PlayerBase : MonoBehaviour
    {
        [Header("Movement")]
        /// <summary>
        /// The player movement speed.
        /// </summary>
        [Tooltip("The player movement speed.")]
        [SerializeField] protected float moveSpeed = 20;

        /// <summary>
        /// The factor with which the player movement is smoothed.
        /// </summary>
        [Tooltip("The factor with which the player movement is smoothed.")]
        [SerializeField] protected float movementSmoothing = 0.05f;

        [Header("Jump")]
        /// <summary>
        /// The jump force.
        /// </summary>
        [Tooltip("The jump force.")]
        [SerializeField] protected float jumpForce = 45;

        /// <summary>
        /// The maximum speed when falling.
        /// </summary>
        [Tooltip("The maximum speed when falling.")]
        [SerializeField] protected float maxFallingSpped = -11;

        /// <summary>
        /// Whether it is possible to control the player in the air.
        /// </summary>
        [Tooltip("Whether it is possible to control the player in the air.")]
        [SerializeField] protected bool airControl = true;

        [Header("Layers")]
        /// <summary>
        /// The layer of the ground tiles.
        /// </summary>
        [Tooltip("The layer of the ground tiles.")]
        [SerializeField] protected LayerMask groundLayer;

        /// <summary>
        /// The layer of the ramp tiles.
        /// </summary>
        [Tooltip("The layer of the ramp tiles.")]
        [SerializeField] private LayerMask rampLayer;

        /// <summary>
        /// Whetser the player is on the ground.
        /// </summary>
        [HideInInspector] public bool grounded;

        /// <summary>
        /// Wheter the player is on a ramp.
        /// </summary>
        [HideInInspector] public bool onRamp;

        /// <summary>
        /// The current velocity of the player.
        /// </summary>
        protected Vector3 velocity;

        protected Rigidbody2D rigidBody;
        protected Collider2D col;

        //protected float horizontalMove;

        private void Awake()
        {
            rigidBody = GetComponent<Rigidbody2D>();
            col = GetComponent<Collider2D>();

        }

        protected virtual void FixedUpdate()
        {
            PositionCheck();

            //horizontalMove = Input.GetAxis("Horizontal") * moveSpeed;
            //Move();
        }

        /// <summary>
        /// Moves the player
        /// </summary>
        public virtual void Move(Vector2 horizontalMove)
        {
            if (grounded || airControl && !onRamp)
            {
                Vector2 targetVelocity = new Vector2(horizontalMove.x * moveSpeed * Time.fixedDeltaTime, rigidBody.velocity.y);

                if(targetVelocity.y < maxFallingSpped)
                {
                    targetVelocity.y = maxFallingSpped;
                }

                rigidBody.velocity = Vector3.SmoothDamp(rigidBody.velocity, targetVelocity, ref velocity, movementSmoothing);
            }
        }

        /// <summary>
        /// Let the player jump.
        /// </summary>
        public virtual void Jump()
        {
            if (grounded)
            {
                grounded = false;
                rigidBody.AddForce(new Vector2(0f, jumpForce * 10));
            }
        }

        /// <summary>
        /// Checks the position of the player.
        /// </summary>
        protected virtual void PositionCheck()
        {
            grounded = Physics2D.Raycast(new Vector2(col.bounds.center.x, col.bounds.min.y), Vector3.down, 0.2f, groundLayer);
            onRamp = Physics2D.Raycast(new Vector2(col.bounds.center.x, col.bounds.min.y), Vector3.down, 0.3f, rampLayer);
        }
    }
}