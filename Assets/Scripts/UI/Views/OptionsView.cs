#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace UI.Views
{
    [DisallowMultipleComponent]
    class OptionsView : MonoBehaviour
    {
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
        }

        void MainMenu()
        {
            _mainMenuView.gameObject.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }
}
