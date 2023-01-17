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
        [Header("Special Ability")]

        /// <summary>
        /// The speed of the climbing player.
        /// </summary>
        [Tooltip("The speed of the climbing player.")]
        [SerializeField] private float climbSpeed = 20f;

        [SerializeField] private float rotationSpeed;

        [Space()]

        [SerializeField] private AudioClip climbSound;



        private bool onWallRight;
        private bool onWallLeft;
        private bool onWallEnd;

        public bool climb;


        protected override void FixedUpdate()
        {
            PositionCheck();

            if ((grounded || onRamp) && !timeAfterJump)
            {
                currentCoyoteTime = coyoteTime;
            }
            else
            {
                currentCoyoteTime -= Time.deltaTime;
            }

        }

        /// <summary>
        /// Checks the position of the spiley player.
        /// </summary>
        protected override void PositionCheck()
        {
            base.PositionCheck();

            onWallRight = false;
            onWallLeft = false;

            if (Physics2D.Raycast(new Vector2(col.bounds.max.x, col.bounds.center.y), Vector2.right, 0.03f, groundLayer))
                onWallRight = true;
            else if (Physics2D.Raycast(new Vector2(col.bounds.min.x, col.bounds.center.y), Vector2.left, 0.03f, groundLayer))
                onWallLeft = true;

            onWallEnd = false;
            if (Physics2D.Raycast(new Vector2(col.bounds.max.x, col.bounds.center.y - 0.1f), Vector2.right, 0.05f, groundLayer) && !onWallRight)
                onWallEnd = true;
            else if (Physics2D.Raycast(new Vector2(col.bounds.min.x, col.bounds.center.y - 0.1f), Vector2.left, 0.05f, groundLayer) && !onWallRight)
                onWallEnd = true;

        }

        /// <summary>
        /// Moves of the spikey player.
        /// </summary>
        public override void Move(Vector2 horizontalMove)
        {
            climb = horizontalMove.y > 0.1f && (onWallRight || onWallLeft);

            Vector2 targetVelocity = Vector2.zero;


            if (grounded || airControl )
            {
                  targetVelocity = new Vector2(horizontalMove.x * moveSpeed * Time.fixedDeltaTime, rigidBody.velocity.y);

                if (targetVelocity.y < maxFallingSpped)
                {
                    targetVelocity.y = maxFallingSpped;
                }

                if ((grounded || onRamp) && rigidBody.velocity.x < -0.1f)
                {
                    transform.rotation = Quaternion.Euler(0, 0, (transform.rotation.eulerAngles.z - rigidBody.velocity.x * rotationSpeed * Time.deltaTime) % 360);
                }
                else if ((grounded || onRamp) && rigidBody.velocity.x > 0.1f)
                {

                    transform.rotation = Quaternion.Euler(0, 0, (transform.rotation.eulerAngles.z - rigidBody.velocity.x * rotationSpeed * Time.deltaTime) % 360);
                }

            }

            if (climb)
            {
                if(!audioSource.isPlaying)
                {
                    Debug.Log("Start playing climbing");
                    audioSource.clip = climbSound;
                    audioSource.loop = true;
                    audioSource.Play();
                }

                targetVelocity = new Vector2(0, horizontalMove.y * climbSpeed * Time.fixedDeltaTime);
                if(onWallRight)
                {
                    transform.rotation = Quaternion.Euler(0, 0, (transform.rotation.eulerAngles.z - (rigidBody.velocity.y * rotationSpeed * Time.deltaTime)) % 360);
                }
                else if (onWallLeft)
                {
                    transform.rotation = Quaternion.Euler(0, 0, (transform.rotation.eulerAngles.z + (rigidBody.velocity.y * rotationSpeed * Time.deltaTime)) % 360);
                }
            }
            else if(audioSource.clip == climbSound)
            {
                Debug.Log("Stop playing climbing");
                audioSource.clip = null;
                audioSource.Stop();
            }

            if (onWallEnd && Mathf.Abs(horizontalMove.x) > 0.2f)
            {
                targetVelocity = horizontalMove * moveSpeed * Time.fixedDeltaTime;
                targetVelocity = new Vector2(horizontalMove.x * moveSpeed, horizontalMove.y * climbSpeed) * Time.deltaTime;

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

            if (climb && onWallRight)
            {
                rigidBody.AddForce(new Vector2(-jumpForce * 10, jumpForce * 5), ForceMode2D.Force);
            }
            else if(climb && onWallLeft)
            {
                rigidBody.AddForce(new Vector2(jumpForce * 10, jumpForce * 5), ForceMode2D.Force);
            }
        }
    }
}
