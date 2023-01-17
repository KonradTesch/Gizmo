using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Rectangle.Player
{
    /// <summary>
    /// The base scipt for the player movement.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(AudioSource))]
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

        [SerializeField] protected float coyoteTime = 0.2f;

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

        [SerializeField]protected ParticleSystem dustParticle;

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

        [Header("Sounds")]
        [SerializeField] protected AudioClip moveSound;
        [SerializeField] protected AudioClip jumpSound;
        [SerializeField] protected AudioClip landingSound;
        /// <summary>
        /// Whetser the player is on the ground.
        /// </summary>
        protected bool grounded;

        /// <summary>
        /// Wheter the player is on a ramp.
        /// </summary>
        protected bool onRamp;

        /// <summary>
        /// Wheter the player is on a one-way platform.
        /// </summary>
        protected bool onPlatform;


        /// <summary>
        /// The current velocity of the player.
        /// </summary>
        protected Vector3 velocity;

        [HideInInspector]public Animator headAnimator;

        protected Rigidbody2D rigidBody;
        protected Collider2D col;
        protected AudioSource audioSource;


        protected Collider2D platformCollider;

        protected Animator animator;

        protected float currentCoyoteTime;
        protected bool timeAfterJump = false;



        private void Awake()
        {
            rigidBody = GetComponent<Rigidbody2D>();
            col = GetComponent<Collider2D>();
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            timeAfterJump = false;
        }

        protected virtual void FixedUpdate()
        {
            PositionCheck();

            if (grounded && !timeAfterJump)
            {
                currentCoyoteTime = coyoteTime;
            }
            else
            {
                currentCoyoteTime -= Time.deltaTime;
            }


            if((grounded || onRamp) && Mathf.Abs(rigidBody.velocity.x) >  0.1f && !audioSource.isPlaying)
            {
                audioSource.clip = moveSound;
                audioSource.loop = true;
                audioSource.Play();

            }
            else if (audioSource.clip == moveSound && Mathf.Abs(rigidBody.velocity.x) < 0.1f)
            {
                audioSource.Stop();
                audioSource.clip = null;
            }
        }

        /// <summary>
        /// Moves the player
        /// </summary>
        public virtual void Move(Vector2 horizontalMove)
        {
            if(animator != null)
                animator.SetFloat("horizontalMove", horizontalMove.x);

            if (grounded || airControl && !onRamp)
            {
                Vector2 targetVelocity = new Vector2(horizontalMove.x * moveSpeed * Time.fixedDeltaTime, rigidBody.velocity.y);

                if(targetVelocity.y < maxFallingSpped)
                {
                    targetVelocity.y = maxFallingSpped;
                }

                rigidBody.velocity = Vector3.SmoothDamp(rigidBody.velocity, targetVelocity, ref velocity, movementSmoothing);
            }


            if (onPlatform && horizontalMove.y < -0.2f && platformCollider != null)
            {
                StartCoroutine(FallThroughPlatform());
            }
        }

        /// <summary>
        /// Let the player jump.
        /// </summary>
        public virtual void Jump()
        {
            if (currentCoyoteTime > 0)
            {
                if(audioSource.isPlaying)
                {
                    audioSource.Stop();
                    audioSource.clip = null;
                }

                audioSource.PlayOneShot(jumpSound);

                CreateDust();

                rigidBody.AddForce(new Vector2(0f, jumpForce * 10));
                StartCoroutine(nameof(TimeAfterJump));

                grounded = false;
                currentCoyoteTime = 0;
            }
        }

        /// <summary>
        /// Checks the position of the player.
        /// </summary>
        protected virtual void PositionCheck()
        {
            onRamp = Physics2D.Raycast(new Vector2(col.bounds.center.x, col.bounds.min.y), Vector3.down, 0.3f, rampLayer);

            RaycastHit2D[] hits = new RaycastHit2D[3];
            ContactFilter2D filter = new ContactFilter2D
            {
                layerMask = groundLayer,
                useLayerMask = true
            };

            if (Physics2D.Raycast(new Vector2(col.bounds.center.x, col.bounds.min.y), Vector3.down, filter, hits , 0.2f) > 0)
            {
                if(!grounded && rigidBody.velocity.y < -0.1f) 
                {
                    audioSource.PlayOneShot(landingSound);
                }

                grounded = true;
                for(int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].collider != null && hits[i].collider.CompareTag("OneWayPlatform"))
                    {
                        onPlatform = true;
                        platformCollider = hits[i].collider;
                        break;
                    }
                    else
                    {
                        onPlatform = false;
                        platformCollider = null;
                    }
                }
            }
            else
            {
                grounded = false;
                onPlatform = false;
                platformCollider = null;
            }
        }

        protected void CreateDust()
        {
            dustParticle.Play();
        }

        protected IEnumerator FallThroughPlatform()
        {
            Collider2D platformCol = platformCollider;

            Physics2D.IgnoreCollision(col, platformCol);

            yield return new WaitForSeconds(0.25f);
            Physics2D.IgnoreCollision(col, platformCol, false);
            
        }

        protected IEnumerator TimeAfterJump()
        {
            timeAfterJump = true;
            yield return new WaitForSeconds(coyoteTime + 0.1f);

            timeAfterJump = false;
        }
    }
}