using Common.Enums;

namespace Common.Systems
{
    public delegate void RequestStateChange(GameState requested, string[] args);
    public delegate void ScheduleStateChange(GameState requested, string[] args);
    public delegate GameState GetCurrentGameState();

    public static class GameStateSystem
    {
        public static event RequestStateChange OnStateChangeRequest;
        public static event ScheduleStateChange OnScheduleStateChange;
        public static event GetCurrentGameState OnGetCurrentGameState;

        // ReSharper disable once PossibleNullReferenceException
        public static GameState CurrentState => OnGetCurrentGameState.Invoke();

        /// <summary>
        /// Actual state change may be delayed in time. Consecutive calls are not allowed.
        /// </summary>
        public static void RequestStateChange(GameState state, params string[] args) =>
            // ReSharper disable once PossibleNullReferenceException
            OnStateChangeRequest.Invoke(state, args);

        // ReSharper disable once PossibleNullReferenceException
        public static void ScheduleStateChange(GameState state, params string[] args) => OnScheduleStateChange.Invoke(state, args);
    }
}