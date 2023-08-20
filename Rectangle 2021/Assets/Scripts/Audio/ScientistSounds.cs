using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rectangle.General;
using System.Xml.Serialization;

namespace Rectangle.Audio
{
    public class ScientistSounds : MonoBehaviour
    {
        [SerializeField] private AudioSource scientistAuidioSource;

        //The arrays for various sound comments of the scintist
        [SerializeField] private AudioClip[] deathSounds = new AudioClip[1];
        [SerializeField] private AudioClip[] winSounds = new AudioClip[1];
        [SerializeField] private AudioClip[] nutSounds = new AudioClip[1];
        [SerializeField] private AudioClip[] behindBestTimeSounds = new AudioClip[1];


        private void OnEnable()
        {
            GameBehavior.onPlayerDeath += PlayDeathSound;
            GameBehavior.onPlayerWin += PlayWinSound;
            GameBehavior.onCollectItem += PlayNutSound;
            GameBehavior.onBadTime += PlayBadTimeSound;
        }

        private void OnDisable()
        {
            GameBehavior.onPlayerDeath -= PlayDeathSound;
            GameBehavior.onPlayerWin -= PlayWinSound;
            GameBehavior.onCollectItem -= PlayNutSound;
            GameBehavior.onBadTime -= PlayBadTimeSound;
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
