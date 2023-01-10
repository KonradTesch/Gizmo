using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rectangle.Level
{
    public class BackgroundController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer lastLayer;

        [Header("Parallax Effect")]
        [SerializeField] private float parallaxMultiplier;
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

        public void SetBGColor(Color color)
        {
            lastLayer.color = color;
        }

        private void ParallaxScrolling()
        {
            for (int i = 0; i < parallaxLayers.Length; i++)
            {
                parallaxLayers[i].transform.localPosition = mainCam.transform.position * parallaxMultiplier * i;
            }
        }
    }
}
