using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rectangle.Level
{
    public class WaypointFollower : MonoBehaviour
    {
        public float moveSpeed;
        public MovingType movingType;
        public Vector2[] waypoints = new Vector2[] { Vector2.zero };

        [Header("Vanishing")]
        public bool vanishing;
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
