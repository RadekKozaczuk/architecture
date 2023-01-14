#nullable enable
using System;
using System.Collections.Generic;
using Common;
using Common.Enums;
using Common.Systems;
using GameLogic.ViewModels;
using Presentation.ViewModels;
using Shared.Systems;
using UI.ViewModels;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
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
                new List<(GameState from, GameState to, Func<int[]>? scenesToLoad, Func<int[]>? scenesToUnload)>
                {
                    (GameState.Booting, GameState.MainMenu, () => new[] {Constants.MainMenuScene, Constants.CoreScene, Constants.UIScene}, null),
                    (GameState.MainMenu, GameState.Gameplay, null, () => new[] {Constants.MainMenuScene}),
                    (GameState.Gameplay, GameState.MainMenu, () => new[] {Constants.MainMenuScene}, ScenesToUnloadFromGameplayToMainMenu)
                },
                new (GameState, Action?, Action?)[]
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

        static void BootingOnExit()
        {
            GameLogicViewModel.BootingOnExit();
            PresentationViewModel.BootingOnExit();
            UIViewModel.BootingOnExit();
        }

        static void MainMenuOnEntry()
        {
            GameLogicViewModel.MainMenuOnEntry();
            PresentationViewModel.MainMenuOnEntry();
            UIViewModel.MainMenuOnEntry();
        }

        static void MainMenuOnExit()
        {
            GameLogicViewModel.MainMenuOnExit();
            PresentationViewModel.MainMenuOnExit();
            UIViewModel.MainMenuOnExit();
        }

        static void GameplayOnEntry()
        {
            GameLogicViewModel.GameplayOnEntry();
            PresentationViewModel.GameplayOnEntry();
            UIViewModel.GameplayOnEntry();

            // save/load logic
            if (CommonData.LoadRequested)
            {
                CommonData.LoadRequested = false;
            }

            int sceneToLoad = CommonData.IsInHubLocation ? Constants.HubScene : Constants.Level0Scene;
            SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);

            //Cursor.lockState = CursorLockMode.Locked;
            //Cursor.visible = false;
        }

        static void GameplayOnExit()
        {
            GameLogicViewModel.GameplayOnExit();
            PresentationViewModel.GameplayOnExit();
            UIViewModel.GameplayOnExit();
            //Cursor.lockState = CursorLockMode.None;
            //Cursor.visible = true;
        }

        /// <summary>
        /// Returns ids of all currently open scenes except for <see cref="Constants.CoreScene"/>, <see cref="Constants.MainMenuScene"/>, and <see cref="Constants.UIScene"/>
        /// </summary>
        static int[] ScenesToUnloadFromGameplayToMainMenu()
        {
            int countLoaded = SceneManager.sceneCount;
            var scenesToUnload = new List<int>(countLoaded);

            for (int i = 0; i < countLoaded; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.buildIndex is Constants.CoreScene or Constants.MainMenuScene or Constants.UIScene)
                    continue;

                scenesToUnload.Add(scene.buildIndex);
            }

            return scenesToUnload.ToArray();
        }
    }
}