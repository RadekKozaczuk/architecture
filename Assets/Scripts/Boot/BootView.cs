#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Common;
using Common.Config;
using Common.Enums;
using Common.Systems;
using GameLogic.ViewModels;
using Presentation.ViewModels;
using Shared;
using Shared.Systems;
using UI.ViewModels;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
using Shared.DebugCommands;
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
        static GameStateMachine<GameState, StateTransitionParameter> _gameStateMachine;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        // readonly fields are initialized only at the start and the null-forgiving operator is only a hint for the compiler.
        // Ultimately it will be null when readonly unless set differently.
        static readonly DebugConfig _config;
#endif

        static readonly SceneConfig _sceneConfig;

        void Awake()
        {
            // increase priority so that main menu can appear faster
            Application.backgroundLoadingPriority = ThreadPriority.High;

            // we have to manually call the constructor to ensure it happens before the BootView.Start method. 
            RuntimeHelpers.RunClassConstructor(typeof(SignalProcessor).TypeHandle);

            // injection must be done in awake because fields cannot be injected into in the same method they are used in
            Architecture.Initialize();
        }

        void Start()
        {
            List<int> overTimeSceneIds = new ();
            List<int> stateChangeSceneIds = new ();
            for (int i = 0; i < _sceneConfig.CustomActivation.Length; i++)
            {
                SceneConfig.ExtActivation activation = _sceneConfig.CustomActivation[i];
                if (activation.When == SceneConfig.ActivationMode.OverTime)
                    overTimeSceneIds.Add((int)activation.Level);
                else if (activation.When == SceneConfig.ActivationMode.StateChange)
                    stateChangeSceneIds.Add((int)activation.Level);
            }

            Architecture.ControllerInjectionAndInitialization(overTimeSceneIds, stateChangeSceneIds);

            Signals.InventoryChanged();
            Signals.HpChanged(123, 1.5f);

            _gameStateMachine = new GameStateMachine<GameState, StateTransitionParameter>(
                new List<(
                    GameState from,
                    GameState to,
                    Func<(int[]?, int[]?)>? scenesToLoadUnload,
                    Action? betweenLoadAndUnload)>
                {
                    (GameState.Booting,
                     GameState.MainMenu,
                     () => (new[] {Constants.MainMenuScene, Constants.CoreScene, Constants.UIScene}, null),
                     null),
                    (GameState.MainMenu,
                     GameState.Gameplay,
                     () => (ScenesToLoadFromMainMenuToGameplay(), new[] {Constants.MainMenuScene}),
                     null),
                    (GameState.Gameplay,
                     GameState.MainMenu,
                     () => (new[] {Constants.MainMenuScene}, ScenesToUnloadFromGameplayToMainMenu()),
                     null),
                    (GameState.Gameplay,
                     GameState.Gameplay,
                     ScenesToLoadUnloadFromGameplayToGameplay,
                     null)
                },
                new (GameState, Action?, Action?)[]
                {
                    (GameState.Booting, null, BootingOnExit),
                    (GameState.MainMenu, MainMenuOnEntry, MainMenuOnExit),
                    (GameState.Gameplay, GameplayOnEntry, GameplayOnExit)
                }
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                // ReSharper disable once MergeConditionalExpression
                // ReSharper disable once SimplifyConditionalTernaryExpression
                , _config is null ? false : _config.LogRequestedStateChange);
#else
            );
#endif

            // set transition parameters
            _gameStateMachine.AddTransitionParameter(StateTransitionParameter.HubSceneRequested, typeof(bool));
            _gameStateMachine.AddTransitionParameter(StateTransitionParameter.LoadGameRequested, typeof(bool));

            GameStateSystem.OnStateChangeRequest += _gameStateMachine.RequestStateChange;
            GameStateSystem.OnRequestPreLoad += _gameStateMachine.RequestPreLoad;
            GameStateSystem.OnActivateRoots_StateChange += _gameStateMachine.ActivateRoots_StateChange;
            GameStateSystem.OnActivateRoots_OverTime += _gameStateMachine.ActivateRoots_OverTime;
            GameStateSystem.OnGetCurrentGameState += _gameStateMachine.GetCurrentState;
            GameStateSystem.OnEndFrameSignal += _gameStateMachine.EndFrameSignal;
            GameStateSystem.OnGetTransitionParameter += _gameStateMachine.GetTransitionParameter;
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
            Application.targetFrameRate = 60;
#endif

/*#if UNITY_EDITOR || DEVELOPMENT_BUILD
            DebugCommandSystem.Add("win_mission", "Instantly wins the mission.", GameLogicViewModel.WinMission);
            DebugCommandSystem.AddParameterized("give_gold", "Give gold", 100, 0, 1000, value =>
            {
                MyDebug.Log($"Parametrized debug command called with the parameter equal to {value}");
            });
            DebugCommandSystem.Add("fail_mission", "Instantly fails current mission.", GameLogicViewModel.FailMission);
#endif*/
		}

		void FixedUpdate()
        {
            if (GameStateSystem.CurrentState == GameState.Booting)
                return;

            if (_isCoreSceneLoaded)
            {
                GameLogicViewModel.CustomFixedUpdate();
                PresentationViewModel.CustomFixedUpdate();
                UIViewModel.CustomFixedUpdate();
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                DebugCommandSystem.CustomUpdate();
#endif
            }
        }

        void Update()
        {
            if (GameStateSystem.CurrentState == GameState.Booting)
                return;

            if (_isCoreSceneLoaded)
            {
                GameLogicViewModel.CustomUpdate();
                PresentationViewModel.CustomUpdate();
                UIViewModel.CustomUpdate();

                SignalProcessor.ExecuteSentSignals();
            }
        }

        void LateUpdate()
        {
            if (GameStateSystem.CurrentState == GameState.Booting)
                return;

            if (_isCoreSceneLoaded)
            {
                GameLogicViewModel.CustomLateUpdate();
                PresentationViewModel.CustomLateUpdate();
                UIViewModel.CustomLateUpdate();
            }

            GameStateSystem.SendEndFrameSignal();
        }

        internal static void OnCoreSceneLoaded() => _isCoreSceneLoaded = true;

        static void BootingOnExit()
        {
            Application.backgroundLoadingPriority = ThreadPriority.Normal;
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

            bool loadGameRequested = (bool)GameStateSystem.GetTransitionParameter(StateTransitionParameter.LoadGameRequested)!;
            if (loadGameRequested)
            {
                CommonData.SaveGameReader!.Close();
                CommonData.SaveGameReader = null;
            }
        }

        static void GameplayOnExit()
        {
            GameLogicViewModel.GameplayOnExit();
            PresentationViewModel.GameplayOnExit();
            UIViewModel.GameplayOnExit();
        }

        static int[]? ScenesToLoadFromMainMenuToGameplay()
        {
            bool loadGameRequested = (bool)GameStateSystem.GetTransitionParameter(StateTransitionParameter.LoadGameRequested)!;
            if (loadGameRequested)
            {
                byte[] data = File.ReadAllBytes(Path.Combine(Application.persistentDataPath, "savegame.sav"));
                CommonData.SaveGameReader = new BinaryReader(new MemoryStream(data));

                int _ = CommonData.SaveGameReader.ReadByte(); // save game version
                CommonData.CurrentLevel = (Level)CommonData.SaveGameReader.ReadByte();

                return new[] {(int)CommonData.CurrentLevel};
            }

            return null;
        }

        /// <summary>
        /// Returns ids of all currently open scenes except for <see cref="Constants.CoreScene" />, <see cref="Constants.MainMenuScene" />,
        /// and <see cref="Constants.UIScene" />
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

        static (int[]? scenesToLoad, int[]? scenesToUnload) ScenesToLoadUnloadFromGameplayToGameplay()
        {
            int currentLevel;
            bool loadGameRequested = (bool)GameStateSystem.GetTransitionParameter(StateTransitionParameter.LoadGameRequested)!;

            // player is in gameplay and wants to load a save game
            if (loadGameRequested)
            {
                byte[] data = File.ReadAllBytes(Path.Combine(Application.persistentDataPath, "savegame.sav"));
                CommonData.SaveGameReader = new BinaryReader(new MemoryStream(data));

                int _ = CommonData.SaveGameReader.ReadByte(); // save game version
                currentLevel = (int)CommonData.CurrentLevel;
                CommonData.CurrentLevel = (Level)CommonData.SaveGameReader.ReadByte();

                return (new[] {(int)CommonData.CurrentLevel}, new[] {currentLevel});
            }

            bool hubRequested = (bool)GameStateSystem.GetTransitionParameter(StateTransitionParameter.HubSceneRequested)!;

            // requested going back to hub
            if (hubRequested)
            {
                currentLevel = (int)CommonData.CurrentLevel;
                CommonData.CurrentLevel = Level.HubLocation;
                return (new[] {(int)Level.HubLocation}, new[] {currentLevel});
            }

            if (CommonData.CurrentLevel == Level.HubLocation)
            {
                CommonData.CurrentLevel += 1;
                return (new[] {(int)CommonData.CurrentLevel}, new[] {(int)CommonData.CurrentLevel - 1});
            }

            CommonData.CurrentLevel = Level.Level0;
            return (new[] {(int)Level.Level0}, new[] {(int)Level.HubLocation});
        }
    }
}