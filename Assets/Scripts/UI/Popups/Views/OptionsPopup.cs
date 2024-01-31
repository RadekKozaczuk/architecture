#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Common.Enums;
using Common.Config;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popups.Views
{
    [DisallowMultipleComponent]
    class OptionsPopup : AbstractPopupView
    {
        static readonly AudioMixerConfig _config;

        [SerializeField]
        TextMeshProUGUI _musicVolumeText;

        [SerializeField]
        TextMeshProUGUI _sfxVolumeText;

        [SerializeField]
        Slider _musicSlider;

        [SerializeField]
        Slider _sfxSlider;

        [SerializeField]
        Button _back;

        OptionsPopup()
            : base(PopupType.Options) { }

        void Awake()
        {
            _musicSlider.onValueChanged.AddListener(delegate { ChangeVolume("musicVolume", _musicSlider, _musicVolumeText); });
            _sfxSlider.onValueChanged.AddListener(delegate { ChangeVolume("sfxVolume", _sfxSlider, _sfxVolumeText); });
            LoadVolumes();

            _back.onClick.AddListener(Back);
        }

        void ChangeVolume(string currentName, Slider currentSlider, TextMeshProUGUI currentText)
        {
            //Convert our slider value to audio mixer dB. Section: (from -80dB, to +20dB)
            int valueToSet = -80 + (int)(currentSlider.value * 10);

            _config.AudioMixer.SetFloat(currentName, valueToSet);
            currentText.text = ((int)currentSlider.value).ToString();
        }

        void LoadVolumes()
        {
            _musicSlider.value = PlayerPrefs.GetInt("musicVolume");
            _sfxSlider.value = PlayerPrefs.GetInt("sfxVolume");
        }

        void Back()
        {
            PlayerPrefs.SetInt("musicVolume", (int)_musicSlider.value);
            PlayerPrefs.SetInt("sfxVolume", (int)_sfxSlider.value);
            PopupSystem.CloseCurrentPopup();
        }
    }
}
