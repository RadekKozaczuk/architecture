#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Shared.Systems
{
    public class GameStateMachine<T> : ICustomUpdate where T : Enum
    {
        readonly struct StateDto
        {
            internal readonly Action<string[]> OnEntry;
            internal readonly Action<string[]> OnExit;

            internal StateDto(Action<string[]> onEntry, Action<string[]> onExit)
            {
                OnEntry = onEntry;
                OnExit = onExit;
            }
        }

        readonly struct TransitionDto
        {
            internal readonly T From;
            internal readonly T To;
            internal readonly int[] ScenesToLoad;
            internal readonly int[] ScenesToUnload;

            internal TransitionDto(T from, T to, int[] scenesToLoad, int[] scenesToUnload)
            {
                From = from;
                To = to;
                ScenesToLoad = scenesToLoad;
                ScenesToUnload = scenesToUnload;
            }
        }

        readonly List<TransitionDto> _transitions;
        readonly Dictionary<T, StateDto> _states = new();
        readonly bool _logRequestedStateChange;
        T _currentState;
        (T requestedState, string[] args)? _scheduledState;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        bool _transitioning;
#endif

        public GameStateMachine(IReadOnlyList<(T, T to, int[] scenesToLoad, int[] scenesToUnload)> transitions,
            IReadOnlyList<(T state, Action<string[]> onEntry, Action<string[]> onExit)> states, T initialState, bool logRequestedStateChange = false)
        {
            _transitions = new List<TransitionDto>(transitions.Count);
            for (int i = 0; i < transitions.Count; i++)
            {
                (T from, T to, int[] scenesToLoad, int[] scenesToUnload) = transitions[i];
                _transitions.Add(new TransitionDto(from, to, scenesToLoad, scenesToUnload));
            }

            _currentState = initialState;
            _logRequestedStateChange = logRequestedStateChange;

            for (int i = 0; i < states.Count; i++)
                _states.Add(states[i].state, new StateDto(states[i].onEntry, states[i].onExit));
        }

        public void CustomUpdate()
        {
            if (_scheduledState == null)
                return;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (_transitioning)
                return;
#endif

            RequestStateChange(_scheduledState.Value.requestedState, _scheduledState.Value.args);
            _scheduledState = null;
        }

        /// <summary>
        /// Actual state change may be delayed in time. Consecutive calls are not allowed.
        /// </summary>
        public async void RequestStateChange(T state, params string[] args)
        {
            List<TransitionDto> transitions = _transitions.FindAll(t => Equal(t.From, _currentState) && Equal(t.To, state));

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (_logRequestedStateChange)
                Debug.Log($"DEBUG LOG: Requested game state change from {_currentState} to {state}");
            if (transitions.Count > 1)
                throw new Exception($"Transition from {_currentState} to {state} is defined more than once.");
            if (transitions.Count == 0)
                throw new Exception($"Transition from {_currentState} to {state} is not defined.");
            if (_transitioning)
                throw new Exception("Game State machine is already transitioning to a different state. Consecutive calls are not allowed. "
                                    + "If you wanted to schedule a transition please call ScheduleStateChange method instead. ");

            _transitioning = true;
#endif

            TransitionDto transition = transitions[0];

            // execute state's on-exit code
            _states.TryGetValue(transition.From, out StateDto fromState);
            fromState.OnExit?.Invoke(args);

            // execute transition's synchronous code
            if (transition.ScenesToLoad.Length > 0)
                await LoadScenes(transition.ScenesToLoad);
            if (transition.ScenesToUnload.Length > 0)
                await UnloadScenes(transition.ScenesToUnload);

            _currentState = state;

            // execute state's on-entry code
            _states.TryGetValue(transition.To, out StateDto toState);
            toState.OnEntry?.Invoke(args);

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (_logRequestedStateChange)
                Debug.Log($"DEBUG LOG: GameStateSystem: State changed from {_currentState} to {state}");

            _transitioning = false;
#endif
        }

        public void ScheduleStateChange(T state, params string[] args) => _scheduledState = (state, args);

        public T GetCurrentState() => _currentState;

        static async Task LoadScenes(params int[] scenes)
        {
            var asyncOperations = new AsyncOperation[scenes.Length];
            asyncOperations[0] = SceneManager.LoadSceneAsync(scenes[0], LoadSceneMode.Additive);

            for (int i = 1; i < scenes.Length; i++)
                asyncOperations[i] = SceneManager.LoadSceneAsync(scenes[i], LoadSceneMode.Additive);

            await AwaitAsyncOperations(asyncOperations);

            // wait a frame so every Awake and Start method is called
            await Utils.WaitForNextFrame();
        }

        static async Task UnloadScenes(params int[] scenes)
        {
            int operationCount = scenes.Length;
            var asyncOperations = new AsyncOperation[operationCount];

            for (int i = 0; i < scenes.Length; i++)
                asyncOperations[i] = SceneManager.UnloadSceneAsync(scenes[i]);

            await AwaitAsyncOperations(asyncOperations);
        }

        /// <summary>
        /// Waits until all operations are either done or have progress greater equal 0.9.
        /// </summary>
        static async Task AwaitAsyncOperations(params AsyncOperation[] operations)
        {
            while (operations.All(t => t.isDone))
                await Task.Delay(1);
        }

        static bool Equal(Enum a, Enum b) => Enum.GetName(a.GetType(), a) == Enum.GetName(b.GetType(), b);
    }
}