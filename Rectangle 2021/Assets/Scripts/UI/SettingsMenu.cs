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

        [SerializeField] private Toggle fullscreedToggle;
        [SerializeField] private Dropdown qualityDropdown;
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;

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

            if(PlayerPrefs.HasKey("settings"))
            {
                LoadSettings();
            }
        }

        public void SaveSettings()
        {
            string saveString = "";

            saveString += resolutionDropdown.value + "|";
            
            if(fullscreedToggle.isOn)
            {
                saveString += "true" + "|";
            }
            else
            {
                saveString += "false" + "|";
            }

            saveString += qualityDropdown.value + "|";
            saveString += masterVolumeSlider.value + "|";
            saveString += musicVolumeSlider.value + "|";
            saveString += sfxVolumeSlider.value;

            PlayerPrefs.SetString("settings", saveString);
        }

        private void LoadSettings()
        {
            string saveString = PlayerPrefs.GetString("settings");

            string[] settings = saveString.Split("|");

            resolutionDropdown.value = int.Parse(settings[0]);
            resolutionDropdown.RefreshShownValue();
            SetResolution(int.Parse(settings[0]));

            if (settings[1] == "true")
            {
                fullscreedToggle.isOn = true;
                Screen.fullScreen = true;
            }
            else
            {
                fullscreedToggle.isOn = false;
                Screen.fullScreen = false;

            }

            qualityDropdown.value = int.Parse(settings[2]);
            qualityDropdown.RefreshShownValue();
            SetQuality(int.Parse(settings[2]));

            masterVolumeSlider.value = float.Parse(settings[3]);
            SetMasterVolume(float.Parse(settings[3]));

            musicVolumeSlider.value = float.Parse(settings[4]);
            SetMusicVolume(float.Parse(settings[4]));

            sfxVolumeSlider.value = float.Parse(settings[5]);
            SetSFXVolume(float.Parse(settings[5]));


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
