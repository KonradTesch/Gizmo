using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Rectangle.UI
{
    public class SettingsMenu : MonoBehaviour
    {
        public AudioMixer audioMixer;

        public Dropdown resolutionDropdown;

        Resolution[] resolutions;
        private void Start()
        {
            resolutions = Screen.resolutions;

            resolutionDropdown.ClearOptions();

            List<string> resoluttionsStrings = new();

            int currentResolutionIndex = 0;

            for(int i = 0; i < resolutions.Length; i++)
            {
                resoluttionsStrings.Add(resolutions[i].width + " x " + resolutions[i].height);

                if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                }
            }

            resolutionDropdown.AddOptions(resoluttionsStrings);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }

        public void SetMasterVolume(float volume)
        {
            audioMixer.SetFloat("MasterVolume", volume);
        }
        public void SetMusicVolume(float volume)
        {
            audioMixer.SetFloat("MusicVolume", volume);
        }

        public void SetSFXVolume(float volume)
        {
            audioMixer.SetFloat("SFXVolume", volume);
            audioMixer.SetFloat("UIVolume", volume);
        }

        public void SetQuality(int qualityIndex)
        {
            QualitySettings.SetQualityLevel(qualityIndex);
        }

        public void SetFullscreen(bool fullscreen)
        {
            Screen.fullScreen = fullscreen;
        }

        public void SetResolution(int resolutionIndex)
        {
            Screen.SetResolution(resolutions[resolutionIndex].width, resolutions[resolutionIndex].height, Screen.fullScreen);
        }
    }
}
