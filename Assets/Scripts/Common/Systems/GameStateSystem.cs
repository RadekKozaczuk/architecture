#nullable enable
using System;
using System.Collections;
using Common.Enums;

namespace Common.Systems
{
    public delegate void RequestStateChange(GameState requested, int[]? additionalScenesToLoad = null, int[]? additionalScenesToUnload = null);
    public delegate void RequestPreLoad(GameState requested, int[]? additionalScenesToLoad = null);
    public delegate void ActivateRoots_OverTime(IEnumerator coroutine);
    public delegate void ActivateRoots_StateChange(Action action);
    public delegate GameState GetCurrentGameState();

    public static class GameStateSystem
    {
        public static event RequestStateChange OnStateChangeRequest = null!;
        public static event RequestPreLoad OnRequestPreLoad = null!;
        public static event ActivateRoots_OverTime OnActivateRoots_OverTime = null!;
        public static event ActivateRoots_StateChange OnActivateRoots_StateChange = null!;
        public static event GetCurrentGameState OnGetCurrentGameState = null!;

        public static GameState CurrentState => OnGetCurrentGameState.Invoke();

        /// <summary>
        /// Keep in mind that scenes to load and unload are defined in <see cref="Shared.Systems.GameStateMachine" />'s constructor.
        /// Additional scenes defined here are special cases that does not occur all the time and therefore could not be defined in the constructor.
        /// Scenes should not overlap with the ones defined in the game state machine constructor.
        /// Actual state change may be delayed in time. Consecutive calls are not allowed.
        /// </summary>
        public static void RequestStateChange(GameState state, int[]? additionalScenesToLoad = null, int[]? additionalScenesToUnload = null) =>
            OnStateChangeRequest.Invoke(state, additionalScenesToLoad, additionalScenesToUnload);

        /// <summary>
        /// Performs only the scene loading part of the <see cref="RequestStateChange"/> method.
        /// Should be used when we want to start scene loading earlier, and then transition to the target state at any moment
        /// by calling <see cref="RequestStateChange"/>. If when call happen loading is still on going
        /// the execution of the call will simply wait until all scenes are loaded.
        /// Consecutive calls are not allowed.
        /// </summary>
        public static void RequestPreLoad(GameState state, int[]? additionalScenesToLoad = null) =>
            OnRequestPreLoad.Invoke(state, additionalScenesToLoad);

        public static void ActivateRoots_OverTime(IEnumerator coroutine) => OnActivateRoots_OverTime.Invoke(coroutine);

        public static void ActivateRoots_StateChange(Action action) => OnActivateRoots_StateChange.Invoke(action);
    }
}