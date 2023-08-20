using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rectangle.Level
{
    /// <summary>
    /// Stores wheter this tile is alredy used by a level block.
    /// </summary>
    public class GridField : MonoBehaviour
    {
        /// <summary>
        /// Whether this tile is alredy used by a levelblock.
        /// </summary>
        public bool isUsed = false;

        /// <summary>
        /// The sprite renderer of the grid field.
        /// </summary>
        [HideInInspector] public SpriteRenderer backgroundRend;

        /// <summary>
        /// Whether there is an collectable item on this grid field.
        /// </summary>
        public bool collectableItem = false;
    }
}