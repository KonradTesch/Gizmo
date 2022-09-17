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
        public ModeController.PlayerModes playerMode;

        /// <summary>
        /// Collider of this background shape.
        /// </summary>
        [HideInInspector] public BoxCollider2D bgCollider;

        private void Awake()
        {
            bgCollider = GetComponent<BoxCollider2D>();
        }
    }
}