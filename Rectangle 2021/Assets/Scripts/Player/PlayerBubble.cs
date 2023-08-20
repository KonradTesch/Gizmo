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
        [Header("Special Ability")]

        /// <summary>
        /// The gravity scale when the player floats.
        /// </summary>
        [Tooltip("The gravity scale when the player floats.")]
        [SerializeField] private float lowGravityScale = 0.2f;

        /// <summary>
        /// The speed of the rotation.
        /// </summary>
        [Tooltip("The speed of the rotation")]
        [SerializeField] private float rotationSpeed;

        [Space()]
        /// <summary>
        /// The audio clip when the player open the wings.
        /// </summary>
        [Tooltip("The audio clip for the floating sound.")]
        [SerializeField] private AudioClip openWingsSound;

        /// <summary>
        /// The audio clip for the floating sound.
        /// </summary>
        [Tooltip("The audio clip for the floating sound.")]
        [SerializeField] private AudioClip floatSound;
 
        private bool falling;
        private bool floating = false;
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

            if(floating)
            {
                if(falling)
                {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.identity, rotationSpeed * 20 * Time.deltaTime);
                    rigidBody.gravityScale = lowGravityScale;

                    if(!audioSource.isPlaying)
                    {
                        audioSource.clip = floatSound;
                        audioSource.loop = true;
                        audioSource.Play();
                    }
                }
                else if (audioSource.clip == floatSound)
                {
                }

                animator.SetBool("float", true);


                if (onRamp || grounded)
                {
                    rigidBody.gravityScale = normalGravity;
                    animator.SetBool("float", false);

                    if(audioSource.clip == floatSound)
                    {
                        audioSource.clip = null;
                        audioSource.Stop();
                    }

                    floating = false;
                }
            }

            if((grounded || onRamp) && rigidBody.velocity.x < -0.1f)
            {
            transform.rotation = Quaternion.Euler(0, 0, (transform.rotation.eulerAngles.z - rigidBody.velocity.x * rotationSpeed * Time.deltaTime) % 360);
            }
            else if((grounded || onRamp) && rigidBody.velocity.x > 0.1f)
            {

                transform.rotation = Quaternion.Euler(0, 0, (transform.rotation.eulerAngles.z - rigidBody.velocity.x * rotationSpeed * Time.deltaTime) % 360);
            }
            else if(falling)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.identity, rotationSpeed * 5 * Time.deltaTime);
            }

            base.Move(horizontalMove);
        }

        /// <summary>
        /// Checks wheter the bubble player is falling.
        /// </summary>
        private bool CheckFalling()
        {
            if (lastYPosition > transform.position.y && !grounded && currentCoyoteTime < 0)
            {
                lastYPosition = transform.position.y;
                return true;
            }

            lastYPosition = transform.position.y;
            return false;
        }

        /// <summary>
        /// Checks the jump input to start or cancel floating.
        /// </summary>
        public override void Jump()
        {
            base.Jump();

            if (!grounded && !onRamp && !floating && !timeAfterJump)
            {
                floating = true;
                audioSource.PlayOneShot(openWingsSound);
            }
            else if (floating)
            {
                rigidBody.gravityScale = normalGravity;
                animator.SetBool("float", false);

                if (audioSource.clip == floatSound)
                {
                    audioSource.clip = null;
                    audioSource.Stop();
                }

                floating = false;
            }
        }
    }
}