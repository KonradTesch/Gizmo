using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rectangle.Level
{
    public class ModeGroup : MonoBehaviour
    {
        /// <summary>
        /// The correct position on the level grid for this shape.
        /// </summary>
        [Tooltip("The correct position on the level grid for this shape.")]
        public Vector2 correctPosition;

        /// <summary>
        /// Grid colliders over which this shape is placed.
        /// </summary>
        [HideInInspector] public List<IsGridUsed> gridColliders;

        /// <summary>
        /// Last grid colliders over which this shape was placed.
        /// </summary>
        [HideInInspector] public List<IsGridUsed> latestGrid;

        /// <summary>
        /// Original position at the start.
        /// </summary>
        [HideInInspector] public Vector3 originPosition;

        private void Start()
        {
            originPosition = transform.position;
        }
    }
}