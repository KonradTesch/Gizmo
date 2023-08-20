using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Rectangle.UI
{
    public class SettingsMenu : MonoBehaviour
    {
        /// <summary>
        /// The audiop mixer.
        /// </summary>
        [Tooltip("The audiop mixer.")]
        public AudioMixer audioMixer;

        /// <summary>
        /// The dropdown menu for the screen resolution setting.
        /// </summary>
        [Tooltip("The dropdown menu for the screen resolution setting.")]
        public Dropdown resolutionDropdown;

        /// <summary>
        /// The toggle to switch betwenn fullscreen and windowedd mode.
        /// </summary>
        [Tooltip("The toggle to switch betwenn fullscreen and windowedd mode.")]
        [SerializeField] private Toggle fullscreedToggle;

        /// <summary>
        /// The dropdown menu for the graphic quality.
        /// </summary>
        [Tooltip("The dropdown menu for the graphic quality.")]
        [SerializeField] private Dropdown qualityDropdown;

        /// <summary>
        /// The slider for the master volume.
        /// </summary>
        [Tooltip("The slider for the master volume.")]
        [SerializeField] private Slider masterVolumeSlider;

        /// <summary>
        /// The slider for the music volume.
        /// </summary>
        [Tooltip("The slider for the music volume.")]
        [SerializeField] private Slider musicVolumeSlider;

        /// <summary>
        /// The slider for the sfx volume.
        /// </summary>
        [Tooltip("The slider for the sfx volume.")]
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

        /// <summary>
        /// Saves the setting ito the PlayerPrefs.
        /// </summary>
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

        /// <summary>
        /// Loads the settings from the PlayerPrefs.
        /// </summary>
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

        /// <summary>
        /// Changes the master volume.
        /// </summary>
        public void SetMasterVolume(float volume)
        {
            float actualVolume = Mathf.Log10(volume) * 20;

            audioMixer.SetFloat("MasterVolume", actualVolume);
        }

        /// <summary>
        /// Changes the music volume.
        /// </summary>
        public void SetMusicVolume(float volume)
        {
            float actualVolume = Mathf.Log10(volume) * 20;

            audioMixer.SetFloat("MusicVolume", actualVolume);
        }

        /// <summary>
        /// Changes the sfx volume.
        /// </summary>
        public void SetSFXVolume(float volume)
        {
            float actualVolume = Mathf.Log10(volume) * 20;

            audioMixer.SetFloat("SFXVolume", actualVolume);
            audioMixer.SetFloat("UIVolume", actualVolume);
        }

        /// <summary>
        /// Changes the graphic quality.
        /// </summary>
        public void SetQuality(int qualityIndex)
        {
            QualitySettings.SetQualityLevel(qualityIndex);
        }

        /// <summary>
        /// Changes between fullscreen and windoiwed mode.
        /// </summary>
        public void SetFullscreen(bool fullscreen)
        {
            Screen.fullScreen = fullscreen;
        }

        /// <summary>
        /// Sets the screen resolution
        /// </summary>
        /// <param name="resolutionIndex"></param>
        public void SetResolution(int resolutionIndex)
        {
            Screen.SetResolution(resolutions[resolutionIndex].width, resolutions[resolutionIndex].height, Screen.fullScreen);
        }
    }
}
