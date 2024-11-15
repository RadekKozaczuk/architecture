#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Presentation.ViewModels;
using GameLogic.ViewModels;
using Core.Systems;
using Shared.Systems;
using UI.ViewModels;
using UnityEngine;
using Core.Enums;
using System.IO;
using Shared;
using System;
using Core;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
using Core.Config;
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

        void Awake()
        {
            // at this point we know the machine role but only if it is a server
            if (Application.platform == RuntimePlatform.LinuxServer)
            {
                Application.targetFrameRate = 60;
                CoreData.IsMultiplayer = true;
                CoreData.MachineRole = MachineRole.DedicatedServer;
            }

            // increase priority so that main menu can appear faster
            Application.backgroundLoadingPriority = ThreadPriority.High;
            // injection must be done in awake because fields cannot be injected into in the same method they are used in
            // start will be at least 1 frame later than Awake.
            Architecture.Initialize(SignalProcessorPrecalculatedArrays.SignalCount,
                                    SignalProcessorPrecalculatedArrays.SignalNames,
                                    SignalProcessorPrecalculatedArrays.SignalQueues);
        }

        void Start()
        {
            SceneManager.sceneLoaded += (scene, _) =>
            {
                if (scene.buildIndex == Constants.CoreScene)
                {
                    SceneManager.UnloadSceneAsync(Constants.BootScene);
                    _isCoreSceneLoaded = true;
                    PresentationViewModel.OnCoreSceneLoaded();
                }

                if (scene.buildIndex == Constants.UIScene)
                    UIViewModel.OnUISceneLoaded();
            };

            Architecture.InvokeInitialization();

#if UNITY_SERVER
            _gameStateMachine = CreateStateMachine_Server();
#else
            _gameStateMachine = CreateStateMachine();
#endif

            // set transition parameters
            _gameStateMachine.AddTransitionParameter(StateTransitionParameter.HubSceneRequested, typeof(bool));
            _gameStateMachine.AddTransitionParameter(StateTransitionParameter.LoadGameRequested, typeof(bool));

            GameStateSystem.OnChangeState += _gameStateMachine.ChangeState;
            GameStateSystem.OnChangeStatePreLoad += _gameStateMachine.ChangeStatePreLoad;
            GameStateSystem.OnFinalizePreLoad += _gameStateMachine.FinalizePreLoad;
            GameStateSystem.OnGetCurrentGameState += _gameStateMachine.GetCurrentState;
            GameStateSystem.OnEndFrameSignal += _gameStateMachine.EndFrameSignal;
            GameStateSystem.OnGetTransitionParameter += _gameStateMachine.GetTransitionParameter;

#if UNITY_SERVER
            GameStateSystem.RequestStateChange(GameState.Gameplay);
#else
            GameStateSystem.ChangeState(GameState.MainMenu);
#endif

            DontDestroyOnLoad(_eventSystem);
            DontDestroyOnLoad(this);

            Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.ScriptOnly);
            Application.SetStackTraceLogType(LogType.Assert, StackTraceLogType.ScriptOnly);
            Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.ScriptOnly);
            Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.ScriptOnly);
            Application.SetStackTraceLogType(LogType.Exception, StackTraceLogType.ScriptOnly);

            // by default, we keep the frame rate low (>=30) to keep the temperature low
            // feel free to change these values when the game gets closer to the release
#if UNITY_EDITOR
            Application.targetFrameRate = 30;
#elif UNITY_ANDROID || UNITY_IOS
            // Mobile platforms ignore QualitySettings.vSyncCount
            // Use Application.targetFrameRate to control the frame rate on mobile platforms.
            Application.targetFrameRate = 30;
#elif DEVELOPMENT_BUILD
            // On all other platforms, Unity ignores the value of targetFrameRate if you set vSyncCount
            // and calculates the target frame rate by dividing the platform's default target frame rate by the value of vSyncCount.
            QualitySettings.vSyncCount = (int)(Screen.currentResolution.refreshRateRatio.value / 30);
            Application.targetFrameRate = 60;
#else
            QualitySettings.vSyncCount = (int)(Screen.currentResolution.refreshRateRatio.value / 45);
            Application.targetFrameRate = 60;
#endif

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            DebugCommandSystem.Add("win_mission", "Instantly wins the mission.", GameLogicViewModel.WinMission);
            DebugCommandSystem.AddParameterized("give_gold", "Give gold", 100, 0, 1000, value =>
                MyDebug.Log($"Parametrized debug command called with the parameter equal to {value}"));
            DebugCommandSystem.Add("fail_mission", "Instantly fails current mission.", GameLogicViewModel.FailMission);
#endif
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

                Architecture.ExecuteSentSignals();
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

        static GameStateMachine<GameState, StateTransitionParameter> CreateStateMachine() =>
            new(new List<(
                    GameState from,
                    GameState to,
                    Func<(int[]?, int[]?)>? scenesToLoadUnload)>
                {
                    (GameState.Booting,
                     GameState.MainMenu,
                     () => (new[] {Constants.MainMenuScene, Constants.CoreScene, Constants.UIScene}, null)),
                    (GameState.MainMenu,
                     GameState.Gameplay,
                     () => (ScenesToLoadFromMainMenuToGameplay(), new[] {Constants.MainMenuScene})),
                    (GameState.Gameplay,
                     GameState.MainMenu,
                     () => (new[] {Constants.MainMenuScene}, ScenesToUnloadFromGameplayToMainMenu())),
                    (GameState.Gameplay,
                     GameState.Gameplay,
                     ScenesToLoadUnloadFromGameplayToGameplay)
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

        /// <summary>
        /// Creates simplified version of the state machine. Used only on the server.
        /// </summary>
        // ReSharper disable once UnusedMember.Local
        static GameStateMachine<GameState, StateTransitionParameter> CreateStateMachine_Server() =>
            new(new List<(
                    GameState from,
                    GameState to,
                    Func<(int[]?, int[]?)>? scenesToLoadUnload)>
                {
                    (GameState.Booting,
                     GameState.Gameplay,
                     () => (new[] {Constants.CoreScene, (int)Level.HubLocation}, null)),
                    (GameState.Gameplay,
                     GameState.Gameplay,
                     ScenesToLoadUnloadFromGameplayToGameplay)
                },
                new (GameState, Action?, Action?)[]
                {
                    (GameState.Booting, null, BootingOnExit),
                    (GameState.Gameplay, GameplayOnEntry, GameplayOnExit)
                }
            );

        static void BootingOnExit()
        {
            Application.backgroundLoadingPriority = ThreadPriority.Normal;

            (int music, int sound) = GameLogicViewModel.LoadVolumeSettings();
            PresentationViewModel.SetMusicVolume(music);
            PresentationViewModel.SetSoundVolume(sound);

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
                CoreData.SaveGameReader!.Close();
                CoreData.SaveGameReader = null;
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
                CoreData.SaveGameReader = new BinaryReader(new MemoryStream(data));

                int _ = CoreData.SaveGameReader.ReadByte(); // save game version
                CoreData.CurrentLevel = (Level)CoreData.SaveGameReader.ReadByte();

                return new[] {(int)CoreData.CurrentLevel};
            }

            return null;
        }

        /// <summary>
        /// Returns ids of all currently open scenes except for <see cref="Constants.CoreScene" />, <see cref="Constants.MainMenuScene" />
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
                CoreData.SaveGameReader = new BinaryReader(new MemoryStream(data));

                int _ = CoreData.SaveGameReader.ReadByte(); // save game version
                currentLevel = (int)CoreData.CurrentLevel;
                CoreData.CurrentLevel = (Level)CoreData.SaveGameReader.ReadByte();

                return (new[] {(int)CoreData.CurrentLevel}, new[] {currentLevel});
            }

            bool hubRequested = (bool)GameStateSystem.GetTransitionParameter(StateTransitionParameter.HubSceneRequested)!;

            // requested going back to hub
            if (hubRequested)
            {
                currentLevel = (int)CoreData.CurrentLevel;
                CoreData.CurrentLevel = Level.HubLocation;
                return (new[] {(int)Level.HubLocation}, new[] {currentLevel});
            }

            if (CoreData.CurrentLevel == Level.HubLocation)
            {
                CoreData.CurrentLevel += 1;
                return (new[] {(int)CoreData.CurrentLevel}, new[] {(int)CoreData.CurrentLevel - 1});
            }

            CoreData.CurrentLevel = Level.Level0;
            return (new[] {(int)Level.Level0}, new[] {(int)Level.HubLocation});
        }
    }
}