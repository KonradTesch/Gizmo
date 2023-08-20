using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rectangle.Audio
{
    /// <summary>
    /// Plays a random audio clip.
    /// </summary>
    public class PlayRandomAudioOnEvent : MonoBehaviour
    {
        /// <summary>
        /// The list with possible audio clips.
        /// </summary>
        [Tooltip("The list with possible audio clips.")]
        [SerializeField] private AudioClip[] audioClipList;

        /// <summary>
        /// The audio source.
        /// </summary>
        [Tooltip("The audio source.")]
        [SerializeField] private AudioSource audioSource;

        public void Awake()
        {
            if(audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }
        }

        /// <summary>
        /// Plays a random audio Clip from the list (Triggered by an Unity Event)
        /// </summary>
        public void PlayRandomAudio()
        {
            AudioClip clip = audioClipList[Random.Range(0, audioClipList.Length)];
            audioSource.PlayOneShot(clip);
        }
    }
}