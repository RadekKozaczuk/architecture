using System;
using Common;
using Common.Config;
using Common.Enums;
using Common.Systems;
using GameLogic;
using GameLogic.ViewModels;
using Presentation;
using Presentation.ViewModels;
using Shared.DependencyInjector;
using Shared.Systems;
using UI;
using UI.Systems;
using UI.ViewModels;
using UnityEngine;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using Common.Views;
using GameLogic.Views;
#endif

namespace Boot.Controllers
{
    /// <summary>
    /// Contains all the high-level logic that cannot be executed from within <see cref="GameLogic" /> namespace.
    /// </summary>
    [DisallowMultipleComponent]
    class MainBootController : MonoBehaviour
    {
        [SerializeField]
        EventSystem _eventSystem;

        static bool _isCoreSceneLoaded;
        static GameStateMachine<GameState> _gameStateSystem;

        static DebugConfig _config;

        void Start()
        {
            ConfigInjector.Run(new[] {"Boot", "Common", "GameLogic", "Presentation", "UI"});

            SceneContext.Installers.Add(new BootInstaller());
            SceneContext.Installers.Add(new CommonInstaller());
            SceneContext.Installers.Add(new GameLogicInstaller());
            SceneContext.Installers.Add(new PresentationInstaller());
            SceneContext.Installers.Add(new UIInstaller());

            SceneContext.Run();

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
                    (GameState.Booting, null, null),
                    (GameState.MainMenu, MainMenuOnEntry, MainMenuOnExit),
                    (GameState.Gameplay, GameplayOnEntry, GameplayOnExit)
                }, GameState.Booting, _config.LogRequestedStateChange);

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

#if UNITY_EDITOR
            GameObject debugCommands = Instantiate(new GameObject(), Vector3.zero, Quaternion.identity);
            debugCommands.AddComponent<CommonDebugView>();
            debugCommands.AddComponent<GameLogicDebugView>();
            // had to add it because if set in the line above, it was named "DebugCommands(Clone)" for some reason
            debugCommands.name
                = "DebugCommands"; // had to add it because if set in the line above, it was named "DebugCommands(Clone)" for some reason
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

        static void MainMenuOnEntry(string[] args = null) { }

        static void MainMenuOnExit(string[] args = null) { }

        static void GameplayOnEntry(string[] args = null)
        {
            UIViewModel.GameplayOnEntry();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            GameLogicViewModel.GameplayOnEntry();

            if (args != null && args.Contains("loadgame"))
            {
                // load the game
            }
        }

        static void GameplayOnExit(string[] args = null)
        {
            UIViewModel.GameplayOnExit();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            GameLogicViewModel.GameplayOnExit();
        }
    }
}