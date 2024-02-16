#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Common.Enums;
using GameLogic.ViewModels;
using Presentation.ViewModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popups.Views
{
    [DisallowMultipleComponent]
    class SettingsPopup : AbstractPopupView
    {
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

        internal override void Initialize()
        {
            (int music, int sound) = GameLogicViewModel.LoadVolumeSettings();
            _musicSlider.value = music;
            _soundSlider.value = sound;
            _musicVolumeText.text = music.ToString();
            _soundVolumeText.text = sound.ToString();

            _musicSlider.onValueChanged.AddListener(delegate
            {
                PresentationViewModel.SetMusicVolume((int)_musicSlider.value);
                _musicVolumeText.text = ((int)_musicSlider.value).ToString();
            });

            _soundSlider.onValueChanged.AddListener(delegate
            {
                PresentationViewModel.SetSoundVolume((int)_soundSlider.value);
                _soundVolumeText.text = ((int)_soundSlider.value).ToString();
            });

            _back.onClick.AddListener(Back);
        }

        void Back()
        {
            PresentationViewModel.PlaySound(Sound.ClickSelect);
            GameLogicViewModel.SaveVolumeSettings((int)_musicSlider.value, (int)_soundSlider.value);
            PopupSystem.CloseCurrentPopup();
        }
    }
}
