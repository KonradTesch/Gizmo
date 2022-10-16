using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rectangle.Player
{
    /// <summary>
    /// The player movement of the spikey mode.
    /// </summary>
    public class PlayerSpikey : PlayerBase
    {
        /// <summary>
        /// The speed of the climbing player.
        /// </summary>
        [Tooltip("The speed of the climbing player.")]
        [SerializeField] private float climbSpeed = 20f;

        /// <summary>
        /// Wheter the player is next to a wall.
        /// </summary>
        [HideInInspector] public bool onWall;
        private bool onWallEnd;

        private bool climb;

        private void Update()
        {
            PositionCheck();

            if (Input.GetKey(KeyCode.W))
            {
                climb = true;
            }
            else
            {
                climb = false;
            }
        }

        /// <summary>
        /// Checks the position of the spiley player.
        /// </summary>
        protected override void PositionCheck()
        {
            base.PositionCheck();

            onWall = false;

            if (Physics2D.Raycast(new Vector2(col.bounds.max.x, col.bounds.center.y), Vector2.right, 0.2f, groundLayer))
                onWall = true;
            else if (Physics2D.Raycast(new Vector2(col.bounds.min.x, col.bounds.center.y), Vector2.left, 0.2f, groundLayer))
                onWall = true;

            onWallEnd = false;
            if (Physics2D.Raycast(new Vector2(col.bounds.max.x, col.bounds.center.y - 0.1f), Vector2.right, 0.2f, groundLayer) && !onWall)
                onWallEnd = true;
            else if (Physics2D.Raycast(new Vector2(col.bounds.min.x, col.bounds.center.y - 0.1f), Vector2.left, 0.2f, groundLayer) && !onWall)
                onWallEnd = true;

        }

        /// <summary>
        /// Moves of the spikey player.
        /// </summary>
        public override void Move(Vector2 horizontalMove)
        {

            if (grounded || airControl  || onWallEnd)
            {
                Vector2 targetVelocity = new Vector2(horizontalMove.x * 10 * Time.fixedDeltaTime, rigidBody.velocity.y);

                if(targetVelocity.y < maxFallingSpped)
                {
                    targetVelocity.y = maxFallingSpped;
                }

                rigidBody.velocity = Vector3.SmoothDamp(rigidBody.velocity, targetVelocity, ref velocity, movementSmoothing);
            }

            if (onWall && climb )
            {
                rigidBody.gravityScale = 0f;
                rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
                rigidBody.constraints = RigidbodyConstraints2D.FreezePositionX;
                rigidBody.MovePosition(transform.position + Vector3.up * climbSpeed * Time.fixedDeltaTime);
            }
            else
            {
                rigidBody.constraints = RigidbodyConstraints2D.None;
                rigidBody.gravityScale = 2f;
            }

            if (onWallEnd)
            {
                rigidBody.gravityScale = 0f;
                rigidBody.MovePosition(transform.position + Vector3.up * climbSpeed * Time.fixedDeltaTime);
                rigidBody.constraints = RigidbodyConstraints2D.None;

            }


        }
    }
}
