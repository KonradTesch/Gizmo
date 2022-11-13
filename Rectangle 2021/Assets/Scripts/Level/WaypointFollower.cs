using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rectangle.Level
{
    public class WaypointFollower : MonoBehaviour
    {
        [SerializeField] private float moveSpeed;
        [SerializeField] private Vector2[] waypoints = new Vector2[] { Vector2.zero };

        private float timer;
        private int waypointIndex = 0;

        void Update()
        {
            timer += Time.fixedDeltaTime * moveSpeed * transform.lossyScale.x;

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
