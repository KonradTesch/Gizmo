using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rectangle.Level
{
    public class BackgroundController : MonoBehaviour
    {

        [Header("Parallax Effect")]
        /// <summary>
        /// The factor for the speed differnece of the backround layers.
        /// </summary>
        [Tooltip("")]
        [SerializeField] private float parallaxMultiplier;

        /// <summary>
        /// The layers of the background.
        /// </summary>
        [Tooltip("The layers of the background.")]
        [SerializeField] private GameObject[] parallaxLayers;

        private Camera mainCam;

        private void Start()
        {
            mainCam = Camera.main;
        }

        private void FixedUpdate()
        {
            ParallaxScrolling();
        }

        /// <summary>
        /// Moves the backround layers with a parallax effect.
        /// </summary>
        private void ParallaxScrolling()
        {
            for (int i = 0; i < parallaxLayers.Length; i++)
            {
                parallaxLayers[i].transform.localPosition = mainCam.transform.position * parallaxMultiplier * i;
            }
        }
    }
}
