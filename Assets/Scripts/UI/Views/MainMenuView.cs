#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Common;
using Common.Enums;
using Common.Systems;
using GameLogic.ViewModels;
using Presentation.ViewModels;
using UI.Popups;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UI.Views
{
    [DisallowMultipleComponent]
    class MainMenuView : MonoBehaviour
    {
        [SerializeField]
        RectTransform _rectTransform;

        [SerializeField]
        Canvas _canvas;

        [SerializeField]
        Button _newGame;

        [SerializeField]
        Button _coop;

        [SerializeField]
        Button _loadGame;

        [SerializeField]
        Button _settings;

        [SerializeField]
        Button _quit;

        void Awake()
        {
            AnchorCheck();

            _newGame.onClick.AddListener(NewGame);
            _coop.onClick.AddListener(Coop);
            _loadGame.onClick.AddListener(LoadGame);
            _loadGame.interactable = GameLogicViewModel.SaveFileExist;
            _settings.onClick.AddListener(Settings);
            _quit.onClick.AddListener(Quit);
        }

        void AnchorCheck()
        {
            Rect rect = RectTransformUtility.PixelAdjustRect(_canvas.GetComponent<RectTransform>(), _canvas);

            // If screen is in portrait mode
            if (rect.width < rect.height)
            {
                _rectTransform.anchorMin = new Vector2(0.05f, _rectTransform.anchorMin.y);
                _rectTransform.anchorMax = new Vector2(0.95f, _rectTransform.anchorMax.y);
            }
        }

        static void NewGame()
        {
            PresentationViewModel.PlaySound(Sound.ClickSelect);
            CommonData.CurrentLevel = Level.HubLocation;
            GameStateSystem.RequestStateChange(GameState.Gameplay, new[] {(int)CommonData.CurrentLevel});
        }

        static void LoadGame()
        {
            PresentationViewModel.PlaySound(Sound.ClickSelect);
            GameStateSystem.RequestStateChange(GameState.Gameplay, parameters: new[] {(StateTransitionParameter.LoadGameRequested, (object)true)});
        }

        static void Coop()
        {
            PresentationViewModel.PlaySound(Sound.ClickSelect);
            PopupSystem.ShowPopup(PopupType.SigningIn);
        }

        static void Settings()
        {
            PresentationViewModel.PlaySound(Sound.ClickSelect);
            PopupSystem.ShowPopup(PopupType.Settings);
        }

        static void Quit()
        {
            PresentationViewModel.PlaySound(Sound.ClickSelect);

#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}