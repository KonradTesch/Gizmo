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
        }

        /// <summary>
        /// Checks the position of the spiley player.
        /// </summary>
        protected override void PositionCheck()
        {
            base.PositionCheck();

            onWall = false;

            if (Physics2D.Raycast(new Vector2(col.bounds.max.x, col.bounds.center.y), Vector2.right, 0.05f, groundLayer))
                onWall = true;
            else if (Physics2D.Raycast(new Vector2(col.bounds.min.x, col.bounds.center.y), Vector2.left, 0.05f, groundLayer))
                onWall = true;

            onWallEnd = false;
            if (Physics2D.Raycast(new Vector2(col.bounds.max.x, col.bounds.center.y - 0.1f), Vector2.right, 0.05f, groundLayer) && !onWall)
                onWallEnd = true;
            else if (Physics2D.Raycast(new Vector2(col.bounds.min.x, col.bounds.center.y - 0.1f), Vector2.left, 0.05f, groundLayer) && !onWall)
                onWallEnd = true;

        }

        /// <summary>
        /// Moves of the spikey player.
        /// </summary>
        public override void Move(Vector2 horizontalMove)
        {

            climb = horizontalMove.y > 0.1f;

            Vector2 targetVelocity = Vector2.zero;


            if (grounded || airControl)
            {
                targetVelocity = new Vector2(horizontalMove.x * moveSpeed * Time.fixedDeltaTime, rigidBody.velocity.y);

                if (targetVelocity.y < maxFallingSpped)
                {
                    targetVelocity.y = maxFallingSpped;
                }

            }

            if (onWall && climb)
            {
                targetVelocity = new Vector2(0, horizontalMove.y * climbSpeed * Time.fixedDeltaTime);
            }

            if (onWallEnd && Mathf.Abs(horizontalMove.x) > 0.2f)
            {
                targetVelocity = horizontalMove * moveSpeed * Time.fixedDeltaTime;
            }

            rigidBody.velocity = Vector3.SmoothDamp(rigidBody.velocity, targetVelocity, ref velocity, movementSmoothing);

            if (onPlatform && horizontalMove.y < -0.2f && platformCollider != null)
            {
                StartCoroutine(FallThroughPlatform());
            }


        }

        public override void Jump()
        {
            base.Jump();

            if (climb && Physics2D.Raycast(new Vector2(col.bounds.max.x, col.bounds.center.y), Vector2.right, 0.05f, groundLayer))
            {
                rigidBody.AddForce(new Vector2(-jumpForce * 10, jumpForce * 5), ForceMode2D.Force);
            }
            else if(climb && Physics2D.Raycast(new Vector2(col.bounds.min.x, col.bounds.center.y), Vector2.left, 0.05f, groundLayer))
            {
                rigidBody.AddForce(new Vector2(jumpForce * 10, jumpForce * 5), ForceMode2D.Force);
            }
        }
    }
}
