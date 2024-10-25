#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Presentation.ViewModels;
using GameLogic.ViewModels;
using UnityEngine.UI;
using Core.Systems;
using UnityEngine;
using Core.Enums;
using UI.Popups;
using Core;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UI.Views
{
    [DisallowMultipleComponent]
    class MainMenuView : MonoBehaviour
    {
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
            _newGame.onClick.AddListener(NewGame);
            _coop.onClick.AddListener(Coop);
            _loadGame.onClick.AddListener(LoadGame);
            _loadGame.interactable = GameLogicViewModel.SaveFileExist;
            _settings.onClick.AddListener(Settings);
            _quit.onClick.AddListener(Quit);
        }

        static void NewGame()
        {
            CoreData.MachineRole = MachineRole.LocalSimulation;

            PresentationViewModel.PlaySound(Sound.ClickSelect);
            CoreData.CurrentLevel = Level.HubLocation;
            GameStateSystem.ChangeState(GameState.Gameplay, new[] {(int)CoreData.CurrentLevel});
        }

        static void LoadGame()
        {
            PresentationViewModel.PlaySound(Sound.ClickSelect);
            GameStateSystem.ChangeState(GameState.Gameplay, parameters: new[] {(StateTransitionParameter.LoadGameRequested, (object)true)});
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