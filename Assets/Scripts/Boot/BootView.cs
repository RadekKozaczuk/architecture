using System;
using Common;
using Common.Enums;
using Common.Systems;
using GameLogic.ViewModels;
using Presentation.ViewModels;
using Shared.Systems;
using UI.ViewModels;
using UnityEngine;
using UnityEngine.EventSystems;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
using Common.Config;
#endif
#if UNITY_EDITOR
using Presentation.Views;
using Common.Views;
using GameLogic.Views;
#endif

namespace Boot
{
    /// <summary>
    /// Contains all the high-level logic that cannot be executed from within <see cref="GameLogic" /> namespace.
    /// </summary>
    [DisallowMultipleComponent]
    class BootView : MonoBehaviour
    {
        [SerializeField]
        EventSystem _eventSystem;

        static bool _isCoreSceneLoaded;
        static GameStateMachine<GameState> _gameStateSystem;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        static DebugConfig _config;
#endif

        void Start()
        {
            Injector.Run();

            _gameStateSystem = new GameStateMachine<GameState>(
                new[]
                {
                    (GameState.Booting, GameState.MainMenu, new[] {Constants.MainMenuScene, Constants.CoreScene, Constants.UIScene},
                     Array.Empty<int>()),
                    (GameState.MainMenu, GameState.Gameplay, Array.Empty<int>(), new[] {Constants.MainMenuScene}),
                    (GameState.Gameplay, GameState.MainMenu, new[] {Constants.MainMenuScene}, Array.Empty<int>())
                },
                new (GameState, Action<string[]>, Action<string[]>)[]
                {
                    (GameState.Booting, null, BootingOnExit),
                    (GameState.MainMenu, MainMenuOnEntry, MainMenuOnExit),
                    (GameState.Gameplay, GameplayOnEntry, GameplayOnExit)
                }, GameState.Booting
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                , _config.LogRequestedStateChange);
#else
            );
#endif

            GameStateSystem.OnStateChangeRequest += _gameStateSystem.RequestStateChange;
            GameStateSystem.OnScheduleStateChange += _gameStateSystem.ScheduleStateChange;
            GameStateSystem.OnGetCurrentGameState += _gameStateSystem.GetCurrentState;
            GameStateSystem.RequestStateChange(GameState.MainMenu);

            DontDestroyOnLoad(_eventSystem);
            DontDestroyOnLoad(this);

            Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.ScriptOnly);
            Application.SetStackTraceLogType(LogType.Assert, StackTraceLogType.ScriptOnly);
            Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.ScriptOnly);
            Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.ScriptOnly);
            Application.SetStackTraceLogType(LogType.Exception, StackTraceLogType.ScriptOnly);

#if UNITY_ANDROID || UNITY_IOS
            Application.targetFrameRate = 30;
#else
            Application.targetFrameRate = 120;
#endif

#if UNITY_EDITOR
            GameObject debugCommands = Instantiate(new GameObject(), Vector3.zero, Quaternion.identity);
            debugCommands.AddComponent<CommonDebugView>();
            debugCommands.AddComponent<PresentationDebugView>();
            debugCommands.AddComponent<GameLogicDebugView>();
            // had to add it because if set in the line above, it was named "DebugCommands(Clone)" for some reason
            debugCommands.name = "DebugCommands";
            DontDestroyOnLoad(debugCommands);
#endif
        }

        void FixedUpdate()
        {
            if (_isCoreSceneLoaded)
            {
                GameLogicViewModel.CustomFixedUpdate();
                PresentationViewModel.CustomFixedUpdate();
                UIViewModel.CustomFixedUpdate();
            }
        }

        void Update()
        {
            if (_isCoreSceneLoaded)
            {
                GameLogicViewModel.CustomUpdate();
                PresentationViewModel.CustomUpdate();
                UIViewModel.CustomUpdate();
                _gameStateSystem.CustomUpdate();
            }
        }

        void LateUpdate()
        {
            if (_isCoreSceneLoaded)
            {
                GameLogicViewModel.CustomLateUpdate();
                PresentationViewModel.CustomLateUpdate();
                UIViewModel.CustomLateUpdate();
            }
        }

        internal static void OnCoreSceneLoaded() => _isCoreSceneLoaded = true;

        static void BootingOnExit(string[] args = null)
        {
            GameLogicViewModel.BootingOnExit();
            PresentationViewModel.BootingOnExit();
            UIViewModel.BootingOnExit();
        }

        static void MainMenuOnEntry(string[] args = null)
        {
            GameLogicViewModel.MainMenuOnEntry();
            PresentationViewModel.MainMenuOnEntry();
            UIViewModel.MainMenuOnEntry();
        }

        static void MainMenuOnExit(string[] args = null)
        {
            GameLogicViewModel.MainMenuOnExit();
            PresentationViewModel.MainMenuOnExit();
            UIViewModel.MainMenuOnExit();
        }

        static void GameplayOnEntry(string[] args = null)
        {
            GameLogicViewModel.GameplayOnEntry();

            if (args != null && args.Contains("loadgame"))
            {
                // load the game
            }

            PresentationViewModel.GameplayOnEntry();
            UIViewModel.GameplayOnEntry();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        static void GameplayOnExit(string[] args = null)
        {
            GameLogicViewModel.GameplayOnExit();
            PresentationViewModel.GameplayOnExit();
            UIViewModel.GameplayOnExit();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}