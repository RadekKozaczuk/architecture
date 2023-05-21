using Common.Enums;
using GameLogic.ViewModels;
using TMPro;
using UI.Config;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popups.Views
{
    [DisallowMultipleComponent]
    class CreateLobbyPopup : AbstractPopupView
    {
        [SerializeField]
        TMP_InputField _input;

        [SerializeField]
        Slider _slider;

        [SerializeField]
        Button _create;

        static readonly UIConfig _config;

        CreateLobbyPopup() : base(PopupType.CreateLobby) { }

        void Awake()
        {
            _input.onValueChanged.AddListener(JoinAction);
            _slider.onValueChanged.AddListener(SliderChanged);
            _create.onClick.AddListener(CreateAction);
        }

        internal override void Initialize()
        {
            
        }

        internal override void Close()
        {
            
        }

        void SliderChanged(float value)
        {
            
        }
        
        static void JoinAction(string text)
        {

        }

        void CreateAction()
        {
            GameLogicViewModel.CreateLobby(_input.text, (int)_slider.value);
        }
    }
}