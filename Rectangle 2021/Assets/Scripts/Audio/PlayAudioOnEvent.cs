using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rectangle.Audio
{
    public class PlayAudioOnEvent : MonoBehaviour
    {
        public AudioSource source;
        public AudioClip clip;

        public void PlayAudio()
        {
            source.PlayOneShot(clip);
        }
    }
}
