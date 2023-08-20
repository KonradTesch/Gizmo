using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Rectangle.Audio
{
    /// <summary>
    /// Plays a sound with a delay
    /// </summary>
    public class SoundDelay : MonoBehaviour
    {
        /// <summary>
        /// The audioclip to play.
        /// </summary>
        [Tooltip("The audioclip to play.")]
        [SerializeField] private AudioClip sound;

        /// <summary>
        /// The delay in seconds.
        /// </summary>
        [Tooltip("The delay in seconds.")]
        [SerializeField] private float delay;


        /// <summary>
        /// The audio mixer.
        /// </summary>
        [Tooltip("The audio mixer.")]
        [SerializeField] private AudioMixer audioMixer;

        /// <summary>
        /// The audio source.
        /// </summary>
        [Tooltip("The audio source.")]
        [SerializeField] private AudioSource audioSource;

        private float originalVolume;


        void Start()
        {
            if(audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }

            Invoke("PlaySound", delay);
        }

        /// <summary>
        /// Plays a simple sound.
        /// </summary>
        void PlaySound()
        {
            audioMixer.GetFloat("MusicVolume", out originalVolume);
            audioMixer.SetFloat("MusicVolume", -20f);
            audioSource.PlayOneShot(sound);
            Invoke("ResetVolume", sound.length);
        }

        /// <summary>
        /// Resets the vloume of the audio mixer.
        /// </summary>
        void ResetVolume()
        {
            audioMixer.SetFloat("MusicVolume", originalVolume);
            audioSource.outputAudioMixerGroup = null;
        }
    }
}
