#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ControlFlow.Interfaces;
using ControlFlow.Systems;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Shared.Systems
{
    public class GameStateMachine<T> : GameStateMachineInternal<T>, ICustomUpdate where T : struct, Enum
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        bool _transitioning;
        readonly bool _logRequestedStateChange;
#endif

        public GameStateMachine(IReadOnlyList<(T from, T to, Func<(int[]?, int[]?)>? scenesToLoadUnload)> transitions,
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            IReadOnlyList<(T state, Action? onEntry, Action? onExit)> states, T initialState, bool logRequestedStateChange = false)
#else
            IReadOnlyList<(T state, Action? onEntry, Action? onExit)> states, T initialState)
#endif
            : base(transitions, states, initialState)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _logRequestedStateChange = logRequestedStateChange;
#endif
        }

        public void CustomUpdate()
        {
            if (_scheduledState == null)
                return;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (_transitioning)
                return;
#endif

            RequestStateChange(_scheduledState.Value, _additionalScenesToLoad, _additionalScenesToUnload);
            _scheduledState = null;
        }

        /// <summary>
        /// Actual state change may be delayed in time. Consecutive calls are not allowed.
        /// Additional scenes, whether to-load or to-unload, must not collide with the scenes defined in the constructor.
        /// </summary>
        public async void RequestStateChange(T state, int[]? additionalScenesToLoad = null, int[]? additionalScenesToUnload = null)
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
            (int[]? scenesToLoad, int[]? scenesToUnload)? scenesToLoadUnload = transition.ScenesToLoadUnload?.Invoke();

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (scenesToLoadUnload != null)
            {
                Assert.False(Utils.HasDuplicates(CombineArrays(scenesToLoadUnload.Value.scenesToLoad, additionalScenesToLoad)),
                             "GameStateMachine was asked to load the same scene more than once.");
                Assert.False(Utils.HasDuplicates(CombineArrays(scenesToLoadUnload.Value.scenesToUnload, additionalScenesToUnload)),
                             "GameStateMachine was asked to unload the same scene more than once.");
            }
#endif 

            // execute state's on-exit code
            _states.TryGetValue(transition.From, out StateDto fromState);
            fromState.OnExit?.Invoke();

            if (scenesToLoadUnload != null)
            {
                // execute transition's synchronous code
                if (scenesToLoadUnload.Value.scenesToLoad is { Length: > 0 } || additionalScenesToLoad is { Length: > 0})
                    await LoadScenes(CombineArrays(scenesToLoadUnload.Value.scenesToLoad, additionalScenesToLoad));

                if (scenesToLoadUnload.Value.scenesToUnload is { Length: > 0 } || additionalScenesToUnload is { Length: > 0})
                    await UnloadScenes(CombineArrays(scenesToLoadUnload.Value.scenesToUnload, additionalScenesToUnload));
            }

            // change state
            _currentState = state;

            // execute state's on-entry code
            _states.TryGetValue(transition.To, out StateDto toState);
            toState.OnEntry?.Invoke();

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (_logRequestedStateChange)
                Debug.Log($"DEBUG LOG: GameStateSystem: State changed from {_currentState} to {state}");

            _transitioning = false;
#endif
        }

        /// <summary>
        /// Returns a copy.
        /// If a and b are both null or empty, return empty array.
        /// </summary>
        static int[] CombineArrays(int[]? a, int[]? b)
        {
            if (a is { Length: > 0 } && b is { Length: > 0})
            {
                int[] arr = new int[a.Length + b.Length];

                int i = 0;
                for (; i < a.Length; i++)
                    arr[i] = a[i];

                for (int j = 0; j < b.Length; j++)
                    arr[i++] = b[j];

                return arr;
            }

            if (a is {Length: > 0})
            {
                int[] arr = new int[a.Length];

                int i = 0;
                for (; i < a.Length; i++)
                    arr[i] = a[i];

                return arr;
            }

            if (b is {Length: > 0})
            {
                int[] arr = new int[b.Length];

                int i = 0;
                for (; i < b.Length; i++)
                    arr[i] = b[i];

                return arr;
            }

            return Array.Empty<int>();
        }

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
            while (!operations.All(t => t.isDone))
                await Task.Delay(1);
        }
    }
}