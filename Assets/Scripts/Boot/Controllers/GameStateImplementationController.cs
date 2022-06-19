using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Boot.Systems;
using Common;
using Common.Config;
using Common.Enums;
using Common.Systems;
using JetBrains.Annotations;
using Presentation.Controllers;
using Presentation.ViewModels;
using Shared;
using UI.Systems;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Boot.Controllers
{
    [UsedImplicitly]
    class GameStateImplementationController : IInitializable
    {
        readonly struct State
        {
            internal readonly Action<string[]> OnEntry;
            internal readonly Action<string[]> OnExit;

            internal State(Action<string[]> onEntry, Action<string[]> onExit)
            {
                OnEntry = onEntry;
                OnExit = onExit;
            }
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        [Inject]
        readonly DebugConfig _debugConfig;
#endif

        /// <summary>
        /// Must correspond to <see cref="GameState" />.
        /// </summary>
        readonly State[] _states;

        bool _bootScheduledToUnload;

        // all possible transitions
        // key: current state
        // value: list of possible transitions (target state, asynchronous transition function)
        readonly Dictionary<GameState, List<(GameState, Func<Task>)>> _transitions
            = new()
            {
                [GameState.Booting] = new List<(GameState, Func<Task>)> {(GameState.MainMenu, null)},
                [GameState.MainMenu] = new List<(GameState, Func<Task>)> {(GameState.Gameplay, null)},
                [GameState.Gameplay] = new List<(GameState, Func<Task>)> {(GameState.MainMenu, null)}
            };

        bool _awaitingLazyEvaluation = true;
        bool _transitioning;

        GameStateImplementationController()
        {
            _states = new[]
            {
                // Booting
                new State(null, null),
                // MainMenu
                new State(MainMenuOnEntry, MainMenuOnExit),
                // Gameplay
                new State(GameplayOnEntry, GameplayOnExit)
            };

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public void Initialize()
        {
            GameStateSystem.StateChangeRequestEvent += Execute;
            GameStateSystem.IsTransitioningEvent += () => _transitioning;
        }

        async void Execute(GameState current, GameState requested, string[] args = null)
        {
            // assertions
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (_transitions.TryGetValue(current, out List<(GameState state, Func<Task>)> listAssert))
            {
                bool found = false;
                for (int i = 0 ; i < listAssert.Count ; i++)
                    if (listAssert[i].state == requested)
                    {
                        found = true;
                        break;
                    }

                if (!found)
                    throw new Exception(ErrorMessage(current, requested));
            }
            else
                throw new Exception(ErrorMessage(current, requested));

            static string ErrorMessage(GameState p, GameState c) => $"Transition from {p} to {c} is not allowed or undefined.";

            if (_transitioning)
                throw new Exception("Game State machine is already transitioning to a different state. Consecutive calls are not allowed");
#endif

            _transitioning = true;
            _transitions.TryGetValue(current, out List<(GameState, Func<Task>)> list);

            // ReSharper disable once PossibleNullReferenceException
            for (int i = 0 ; i < list.Count ; i++)
            {
                (GameState state, Func<Task> function) = list[i];
                if (state != requested)
                    continue;

                // execute state's on-exit code
                _states[(int) current].OnExit?.Invoke(args);

                // execute transition's synchronous code
                await ExecuteTransitionSynchronousCode(current, requested);

                // execute transition's asynchronous code
                if (function != null)
                    await Task.Run(() => function());

                // actual state change
                GameStateSystem.ChangeState(requested);

                // this is a bit of a workaround
                // Zenject performs his initializations and ticks BEFORE 
                // Unity's Awake call
                // that means for example that UIReferenceHolder (and other holders) won't have their references on time.
                // To avoid that problem we wait for the callback
                if (_awaitingLazyEvaluation)
                    await Task.Run(
                        () =>
                        {
                            while (_awaitingLazyEvaluation) { }
                        });

                // execute state's on-entry code
                _states[(int) requested].OnEntry?.Invoke(args);
                
                MyDebug.Log($"DEBUG LOG: GameStateSystem: State changed from {current} to {requested}", _debugConfig.LogRequestedStateChange);

                break;
            }

            _transitioning = false;
        }

        /// <summary>
        /// Some code cannot be run from a different Task.
        /// </summary>
        async Task ExecuteTransitionSynchronousCode(GameState previous, GameState current)
        {
            if (previous == GameState.Booting && current == GameState.MainMenu)
            {
                await BootSystem.LoadScenes(Constants.MainMenuScene, Constants.CoreScene, Constants.UIScene);
                // this is necessary because Unity is stupid and despite me waiting for the scene to fully load up
                // it still prevents me from unloading boot scene because "unloading the last one is impossible"
                _bootScheduledToUnload = true;
            }
            else if (previous == GameState.MainMenu)
            {
                if (current == GameState.Gameplay)
                    await BootSystem.UnloadScenes(Constants.MainMenuScene);
            }
            else if (previous == GameState.Gameplay)
            {
                if (current == GameState.MainMenu)
                    await BootSystem.LoadScenes(Constants.MainMenuScene);
            }
        }

        static void MainMenuOnEntry(string[] args = null) { }

        static void MainMenuOnExit(string[] args = null) { }

        static void GameplayOnEntry(string[] args = null)
        {
            InputSystem.IsActive = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (args != null && args.Contains("loadgame"))
            {
                // load the game
            }
        }

        static void GameplayOnExit(string[] args = null)
        {
            InputSystem.IsActive = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.buildIndex == Constants.UIScene)
                _awaitingLazyEvaluation = false;

            if (_bootScheduledToUnload && scene.buildIndex != 0)
            {
                SceneManager.UnloadSceneAsync(Constants.BootScene);
                _bootScheduledToUnload = false;
            }


            if (scene.buildIndex == Constants.CoreScene)
            {
                // have to be here because Boot assembly does not see UI
                // and the view must be in UI not in common cause it touches GameLogic stuff
#if UNITY_EDITOR
                GameObject debug = GameObject.Find("DebugCommands");
                //debug.AddComponent<DebugCommandsView>();
#endif

                PresentationMainController.OnCoreSceneLoaded();
                PresentationViewModel.OnCoreSceneLoaded();
            }
        }
    }
}