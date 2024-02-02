#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Common.Enums;
using Common.Config;
using GameLogic.ViewModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popups.Views
{
    [DisallowMultipleComponent]
    class SettingsPopup : AbstractPopupView
    {
        static readonly AudioMixerConfig _config;

        [SerializeField]
        TextMeshProUGUI _musicVolumeText;

        [SerializeField]
        TextMeshProUGUI _soundVolumeText;

        [SerializeField]
        Slider _musicSlider;

        [SerializeField]
        Slider _soundSlider;

        [SerializeField]
        Button _back;

        SettingsPopup()
            : base(PopupType.Settings) { }

        void Awake()
        {
            _musicSlider.onValueChanged.AddListener(delegate { ChangeVolume("musicVolume", _musicSlider, _musicVolumeText); });
            _soundSlider.onValueChanged.AddListener(delegate { ChangeVolume("sfxVolume", _soundSlider, _soundVolumeText); });

            _back.onClick.AddListener(Back);
        }

        internal override void Initialize()
        {
            (int music, int sound) = GameLogicViewModel.LoadVolumeSettings();
            _musicSlider.value = music;
            _soundSlider.value = sound;
        }

        static void ChangeVolume(string currentName, Slider currentSlider, TextMeshProUGUI currentText)
        {
            // Convert our slider value to audio mixer dB. Section: (from -80dB, to +20dB)
            int valueToSet = -80 + (int)(currentSlider.value * 10);

            _config.AudioMixer.SetFloat(currentName, valueToSet);
            currentText.text = ((int)currentSlider.value).ToString();
        }

        void Back()
        {
            GameLogicViewModel.SaveVolumeSettings((int)_musicSlider.value, (int)_soundSlider.value);
            PopupSystem.CloseCurrentPopup();
        }
    }
}
