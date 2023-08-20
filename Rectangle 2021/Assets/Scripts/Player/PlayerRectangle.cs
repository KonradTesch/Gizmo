using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rectangle.Player
{
    /// <summary>
    /// The player movement for the rectangle mode.
    /// </summary>
    public class PlayerRectangle : PlayerBase
    {
        /// <summary>
        /// The audio clip for the double jump.
        /// </summary>
        [Tooltip("The audio clip for the double jump.")]
        [SerializeField] private AudioClip doubleJumpSound;

        private bool canDoubleJump;

        /// <summary>
        /// Let the rectangle player jump.
        /// </summary>
        public override void Jump()
        {
            if (currentCoyoteTime > 0)
            {
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                    audioSource.clip = null;
                }

                audioSource.PlayOneShot(jumpSounds[Random.Range(0, jumpSounds.Length)]);

                CreateDust();

                grounded = false;

                //Resets the velocity on the y axis
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);

                //Add the jump force
                rigidBody.AddForce(new Vector2(0f, jumpForce * 10));
                currentCoyoteTime = 0;
            }
            else if (canDoubleJump && !onRamp)
            {
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                    audioSource.clip = null;
                }

                audioSource.PlayOneShot(doubleJumpSound);

                //Resets the velocity on the y axis
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);

                //Add the jump force
                rigidBody.AddForce(new Vector2(0f, jumpForce * 10));
                canDoubleJump = false;
            }
        }

        /// <summary>
        /// The position check for the double jump and to keep the right rotation.
        /// </summary>
        protected override void PositionCheck()
        {
            base.PositionCheck();
            if(grounded)
            {
                canDoubleJump = true;
            }

            if ((Mathf.Abs(transform.rotation.eulerAngles.z) % 90 < 1 || Mathf.Abs(transform.rotation.eulerAngles.z) % 90 > 89) && Mathf.Abs(transform.rotation.eulerAngles.z) > 45)
            {
                transform.rotation = Quaternion.identity;
            }

        }
    }
}