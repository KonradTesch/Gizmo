using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rectangle.Player
{
    /// <summary>
    /// The player movement for the bubble mode.
    /// </summary>
    public class PlayerBubble : PlayerBase
    {
        /// <summary>
        /// The gravity scale when the player floats.
        /// </summary>
        [Tooltip("The gravity scale when the player floats.")]
        [SerializeField] private float lowGravityScale = 0.2f;

        private bool falling;
        private float lastYPosition;



        /// <summary>
        /// Moves the bubble player.
        /// </summary>
        public override void Move(Vector2 horizontalMove)
        {
            falling = CheckFalling();

            if (!falling || onRamp || horizontalMove.y < - 0.2f)
            {
                rigidBody.gravityScale = 5f;
                animator.SetBool("float", false);
            }
            else
            {
                rigidBody.gravityScale = lowGravityScale;
                animator.SetBool("float", true);
            }

            base.Move(horizontalMove);
        }

        /// <summary>
        /// Checks wheter the bubble player is falling.
        /// </summary>
        /// <returns></returns>
        private bool CheckFalling()
        {
            if (lastYPosition > transform.position.y)
            {
                lastYPosition = transform.position.y;
                return true;
            }

            lastYPosition = transform.position.y;
            return false;
        }
    }
}