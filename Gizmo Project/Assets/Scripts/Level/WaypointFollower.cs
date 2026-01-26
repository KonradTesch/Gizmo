using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rectangle.Level
{
    //A script to move an object throuch the level.
    public class WaypointFollower : MonoBehaviour
    {
        /// <summary>
        /// The speed of the moving object.
        /// </summary>
        [Tooltip("The speed of the moving object.")]
        public float moveSpeed;

        /// <summary>
        /// The type of object.
        /// </summary>
        [Tooltip("The type of object.")]
        public MovingType movingType;

        /// <summary>
        /// An Array with trhe waypoint that object is moving along.
        /// </summary>
        [Tooltip("An Array with trhe waypoint that object is moving along.")]
        public Vector2[] waypoints = new Vector2[] { Vector2.zero };

        [Header("Vanishing")]

        /// <summary>
        /// Wheter this object vanished after the player touches it.
        /// </summary>
        [Tooltip("Wheter this object vanished after the player touches it.")]
        public bool vanishing;

        /// <summary>
        /// The time this object vanished, after the player touches it.
        /// </summary>
        [Tooltip("The time this object vanished, after the player touches it.")]
        public float vanishingTime;

        private float timer;
        private int waypointIndex = 0;

        public enum MovingType { Platform, Spikes, Ramp, Background};

        private void OnEnable()
        {
            switch (movingType)
            {
                case MovingType.Platform:
                    gameObject.layer = LayerMask.NameToLayer("Ground");
                    break;
                case MovingType.Spikes:
                    gameObject.AddComponent<Spikes>();
                    break;
                case MovingType.Ramp:
                    gameObject.layer = LayerMask.NameToLayer("Ramp");
                    break;
                case MovingType.Background:
                    Destroy(GetComponent<Collider2D>());
                    break;
            }

        }

        void Update()
        {
            timer += Time.deltaTime * moveSpeed * transform.lossyScale.x;

            Vector2 lastStop = waypoints[waypointIndex] * transform.lossyScale;
            Vector2 nextStop;
            if(waypoints.Length <= waypointIndex + 1)
            {
                nextStop = waypoints[0] * transform.lossyScale;
            }
            else
            {
                nextStop = waypoints[waypointIndex + 1] * transform.lossyScale;
            }

            transform.position = Vector2.Lerp(lastStop, nextStop, timer / Vector2.Distance(lastStop, nextStop));

            if(transform.position == (Vector3)nextStop)
            {
                waypointIndex++;

                if ( waypoints.Length <= waypointIndex)
                {
                    waypointIndex = 0;
                }
                timer = 0;
            }
        
        }
    }
}
