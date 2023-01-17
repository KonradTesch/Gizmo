using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rectangle.Audio
{
    public class PlayAudioOnEventOne : MonoBehaviour
    {
        public AudioSource source;
        public AudioClip clipOne;

        public void PlayAudioOne()
        {
            source.PlayOneShot(clipOne);
        }
    }
}
