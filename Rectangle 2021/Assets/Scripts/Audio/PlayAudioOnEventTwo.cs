using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rectangle.Audio
{
    public class PlayAudioOnEventTwo : MonoBehaviour
    {
        public AudioSource source;
        public AudioClip clipTwo;

        public void PlayAudioTwo()
        {
            source.PlayOneShot(clipTwo);
        }
    }
}
