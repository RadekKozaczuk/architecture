#nullable enable
using Common.Enums;

namespace Common.Systems
{
    public delegate void RequestStateChange(GameState requested, int[]? additionalScenesToLoad = null, int[]? additionalScenesToUnload = null);
    public delegate void ScheduleStateChange(GameState requested, int[]? additionalScenesToLoad = null, int[]? additionalScenesToUnload = null);
    public delegate GameState GetCurrentGameState();

    public static class GameStateSystem
    {
        public static event RequestStateChange OnStateChangeRequest = null!;
        public static event ScheduleStateChange OnScheduleStateChange = null!;
        public static event GetCurrentGameState OnGetCurrentGameState = null!;

        public static GameState CurrentState => OnGetCurrentGameState.Invoke();

        /// <summary>
        /// Actual state change may be delayed in time. Consecutive calls are not allowed.
        /// </summary>
        public static void RequestStateChange(GameState state, int[]? additionalScenesToLoad = null, int[]? additionalScenesToUnload = null)
            => OnStateChangeRequest.Invoke(state, additionalScenesToLoad, additionalScenesToUnload);

        public static void ScheduleStateChange(GameState state, int[]? additionalScenesToLoad = null, int[]? additionalScenesToUnload = null)
            => OnScheduleStateChange.Invoke(state, additionalScenesToLoad, additionalScenesToUnload);
    }
}