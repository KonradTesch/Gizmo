using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rectangle
{
    public class BackgroundController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer lastLayer;

        [Header("Parallax Effect")]
        [SerializeField] private Camera mainCam;
        [SerializeField] private float parallaxMultiplierMultiplier;
        [SerializeField] private GameObject[] parallaxLayers;

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
                parallaxLayers[i].transform.localPosition = mainCam.transform.position * parallaxMultiplierMultiplier * i;
            }
        }
    }
}
