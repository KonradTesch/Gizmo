using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rectangle
{
    public class PlayRandomAudioOnEvent : MonoBehaviour
    {
        public AudioClip[] audioClipList;
        public AudioSource audioSource;
        // Start is called before the first frame update

        public void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void PlayRandomAudio()
        {
            AudioClip clip = RandomClip();
            audioSource.PlayOneShot(clip);
        }


        private AudioClip RandomClip()
        {
            //audioSource.pitch = 3;
            return audioClipList[Random.Range(0, audioClipList.Length)];
        }
    }
}