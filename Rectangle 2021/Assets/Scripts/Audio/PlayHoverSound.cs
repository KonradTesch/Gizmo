using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rectangle.Audio
{
    public class PlayHoverSound : MonoBehaviour
    {
        [SerializeField]
        private AudioSource hoverSource;
        [SerializeField]
        private AudioClip hoverClip;
        


        public void HoverPlay()
        {
            hoverSource.PlayOneShot(hoverClip);
        }

    }
}
