#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Core.Enums;
using Shared.Systems;

// ReSharper disable InvalidXmlDocComment

namespace Core.Systems
{
    public delegate void ChangeState(GameState requested, int[]? additionalScenesToLoad = null,
        int[]? additionalScenesToUnload = null, (StateTransitionParameter key, object value)[]? parameters = null,
        int[]? scenesToSynchronize = null);

    public delegate void ChangeStatePreLoad(GameState requested, int[]? additionalScenesToLoad = null,
        int[]? additionalScenesToUnload = null, (StateTransitionParameter key, object value)[]? parameters = null);

    public delegate void FinalizePreLoad();

    public delegate GameState GetCurrentGameState();
    public delegate void EndFrameSignal();
    public delegate object? GetTransitionParameter(StateTransitionParameter key);

    public static class GameStateSystem
    {
        public static event ChangeState OnChangeState = null!;
        public static event ChangeStatePreLoad OnChangeStatePreLoad = null!;
        public static event FinalizePreLoad OnFinalizePreLoad = null!;
        public static event GetCurrentGameState OnGetCurrentGameState = null!;
        public static event EndFrameSignal OnEndFrameSignal = null!;
        public static event GetTransitionParameter OnGetTransitionParameter = null!;

        public static GameState CurrentState => OnGetCurrentGameState.Invoke();

        /// <summary>
        /// Scenes to load and unload are defined in <see cref="GameStateMachine{TState,TTransitionParameter}" />'s constructor.
        /// Additional scenes defined here are special cases that does not occur all the time and therefore could not be defined in the constructor.
        /// These scenes should not overlap with the ones defined in the GameStateMachine's constructor.
        /// Actual state change may be delayed in time. Consecutive calls are not allowed.
        /// </summary>
        public static void ChangeState(GameState state, int[]? additionalScenesToLoad = null,
            int[]? additionalScenesToUnload = null, (StateTransitionParameter key, object value)[]? parameters = null,
            int[]? scenesToSynchronize = null) =>
            OnChangeState.Invoke(state, additionalScenesToLoad, additionalScenesToUnload, parameters, scenesToSynchronize);

        /// <summary>
        /// Simplified version of <see cref="Systems.ChangeState"/>.
        /// </summary>
        public static void ChangeState(GameState state, (StateTransitionParameter key, bool value) parameter) =>
            OnChangeState.Invoke(state, null, null, new []{(parameter.key, (object)parameter.value)});

        /// <summary>
        /// Performs only the scene loading part of the <see cref="Systems.ChangeState"/> method.
        /// Should be used when we want to start scene loading earlier, and then transition to the target state at any moment
        /// by calling <see cref="Systems.ChangeState"/>. If when call happen loading is still on going
        /// the execution of the call will simply wait until all scenes are loaded.
        /// Consecutive calls are not allowed.
        /// </summary>
        public static void ChangeStatePreLoad(GameState state, int[]? additionalScenesToLoad = null,
            int[]? additionalScenesToUnload = null, (StateTransitionParameter key, object value)[]? parameters = null) =>
            OnChangeStatePreLoad.Invoke(state, additionalScenesToLoad, additionalScenesToUnload, parameters);

        /// <summary>
        /// Simplified version of <see cref="Systems.ChangeStatePreLoad"/>.
        /// </summary>
        public static void ChangeStatePreLoad(GameState state, (StateTransitionParameter key, int value) parameter) =>
            OnChangeStatePreLoad.Invoke(state, null, null, new []{(parameter.key, (object)parameter.value)});

        public static void FinalizePreLoad() => OnFinalizePreLoad.Invoke();

        public static void SendEndFrameSignal() => OnEndFrameSignal.Invoke();

        /// <summary>
        /// Returns the value of the given parameters if present, otherwise default.
        /// Meaning this method will return null for reference types, and default for value types.
        /// The parameter must be present otherwise method will throw an exception.
        /// </summary>
        public static object? GetTransitionParameter(StateTransitionParameter key) => OnGetTransitionParameter.Invoke(key);
    }
}