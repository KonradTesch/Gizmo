using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rectangle.General;

namespace Rectangle.Audio
{
    public class ScientistSounds : MonoBehaviour
    {
        [SerializeField] private AudioSource scientistAuidioSource;
        [SerializeField] private AudioClip[] deathSounds = new AudioClip[1];
        [SerializeField] private AudioClip[] winSounds = new AudioClip[1];
        [SerializeField] private AudioClip[] nutSounds = new AudioClip[1];
        [SerializeField] private AudioClip[] behindBestTimeSounds = new AudioClip[1];


        private float timer;
        private float nextRandomTime;

        private void Start()
        {
            GameBehavior.death = PlayDeathSound;
            GameBehavior.win = PlayWinSound;
            GameBehavior.star = PlayNutSound;
            GameBehavior.badTime = PlayBadTimeSound;
        }


        private void PlayDeathSound()
        {
            scientistAuidioSource.PlayOneShot(deathSounds[Random.Range(0, deathSounds.Length)]);

        }
        private void PlayWinSound()
        {
            scientistAuidioSource.PlayOneShot(winSounds[Random.Range(0, winSounds.Length)]);
        }
        private void PlayNutSound()
        {
            scientistAuidioSource.PlayOneShot(nutSounds[Random.Range(0, nutSounds.Length)]);
        }
        private void PlayBadTimeSound()
        {
            scientistAuidioSource.PlayOneShot(behindBestTimeSounds[Random.Range(0, behindBestTimeSounds.Length)]);
        }

    }
}
