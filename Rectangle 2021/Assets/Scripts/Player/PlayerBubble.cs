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

        [SerializeField] private float rotationSpeed;

        private bool falling;
        private float lastYPosition;
        private float normalGravity;

        private void Start()
        {
            normalGravity = rigidBody.gravityScale;
        }

        /// <summary>
        /// Moves the bubble player.
        /// </summary>
        public override void Move(Vector2 horizontalMove)
        {
            falling = CheckFalling();

            if (!falling || onRamp || horizontalMove.y < - 0.2f)
            {
                rigidBody.gravityScale = normalGravity;
                animator.SetBool("float", false);
            }
            else
            {
                rigidBody.gravityScale = lowGravityScale;
                animator.SetBool("float", true);
            }

            if(grounded && horizontalMove.x < 0.1f)
            {
            transform.rotation = Quaternion.Euler(0, 0, (transform.rotation.eulerAngles.z - rigidBody.velocity.x * rotationSpeed * Time.deltaTime) % 360);
            }
            else if(grounded && horizontalMove.x > 0.1f)
            {

                transform.rotation = Quaternion.Euler(0, 0, (transform.rotation.eulerAngles.z - rigidBody.velocity.x * rotationSpeed * Time.deltaTime) % 360);
            }
            else if(falling)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.identity, rotationSpeed * 20 * Time.deltaTime);
            }

            base.Move(horizontalMove);
        }

        /// <summary>
        /// Checks wheter the bubble player is falling.
        /// </summary>
        /// <returns></returns>
        private bool CheckFalling()
        {
            if (lastYPosition > transform.position.y && !grounded)
            {
                lastYPosition = transform.position.y;
                return true;
            }

            lastYPosition = transform.position.y;
            return false;
        }
    }
}