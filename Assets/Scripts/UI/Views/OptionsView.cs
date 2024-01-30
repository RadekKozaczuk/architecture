#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using UI.Config;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace UI.Views
{
    [DisallowMultipleComponent]
    class OptionsView : MonoBehaviour
    {
        static readonly UIConfig _config;

        [SerializeField]
        MainMenuView _mainMenuView;

        [SerializeField]
        Slider _musicSlider;

        [SerializeField]
        Slider _soundsSlider;

        [SerializeField]
        Button _backToMainMenu;

        void Awake()
        {
            _backToMainMenu.onClick.AddListener(MainMenu);
            _musicSlider.onValueChanged.AddListener(delegate { ChangeVolume("musicVolume", _musicSlider); });
            _soundsSlider.onValueChanged.AddListener(delegate { ChangeVolume("soundsVolume", _soundsSlider); });
            LoadSavedVolumes();
        }

        void MainMenu()
        {
            //Hide all children objects in Options.gameObject
            foreach (Transform child in this.transform)
                child.gameObject.SetActive(false);

            //Show all children objects in MainMenu.gameObject
            foreach (Transform child in _mainMenuView.transform)
                child.gameObject.SetActive(true);
        }

        void ChangeVolume(string currentName, Slider currentSlider) =>
            _config.AudioMixer.SetFloat(currentName, Mathf.Log10(currentSlider.value) * 50);

        void LoadSavedVolumes()
        {
            _musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
            _soundsSlider.value = PlayerPrefs.GetFloat("soundsVolume");
        }

        void OnDisable()
        {
            PlayerPrefs.SetFloat("musicVolume", _musicSlider.value);
            PlayerPrefs.SetFloat("soundsVolume", _soundsSlider.value);
        }
    }
}
