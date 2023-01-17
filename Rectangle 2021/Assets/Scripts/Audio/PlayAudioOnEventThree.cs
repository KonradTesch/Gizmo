using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rectangle.Audio
{
    public class PlayAudioOnEventThree : MonoBehaviour
    {
        public AudioSource source;
        public AudioClip clipThree;

        public void PlayAudioThree()
        {
            source.PlayOneShot(clipThree);
        }
    }
}
