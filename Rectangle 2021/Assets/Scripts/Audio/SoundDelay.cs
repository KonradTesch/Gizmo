using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Rectangle.Audio
{
    public class SoundDelay : MonoBehaviour
    {
        public AudioClip sound;
        public AudioMixer audioMixer;
        private float originalVolume;
        public AudioSource audioSource;

        void Start()
        {
            audioSource = GetComponent<AudioSource>();
            Invoke("PlaySound", 2f);
        }

        void PlaySound()
        {
            audioMixer.GetFloat("MusicVolume", out originalVolume);
            audioMixer.SetFloat("MusicVolume", -20f);
            audioSource.PlayOneShot(sound);
            Invoke("ResetVolume", sound.length);
        }

        void ResetVolume()
        {
            audioMixer.SetFloat("MusicVolume", originalVolume);
            audioSource.outputAudioMixerGroup = null;
        }
    }
}
