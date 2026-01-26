using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rectangle.Player;

namespace Rectangle.Level
{
    /// <summary>
    /// Stores the information for the player mode of a background.
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class BackgroundMode : MonoBehaviour
    {
        /// <summary>
        /// Mode of this background shape.
        /// </summary>
        [Tooltip("The mode of this background shape.")]
        public PlayerController.PlayerModes playerMode;

        /// <summary>
        /// Whether the tile on this background has an nut.
        /// </summary>
        public bool hasNut = false;
    }
}