namespace Shared.AI.Interfaces
{
    /// <summary>
    /// Represents a single action executed in a state machine
    /// </summary>
    public interface IStateMachineAction
    {
        /// <summary>
        /// Current action state
        /// </summary>
        StateMachineActionState CurrentState { get; }

        /// <summary>
        /// Action failure reason
        /// </summary>
        public string FailureReason { get; }

        /// <summary>
        /// Initializes the action. Fails if action is already started or completed.
        /// </summary>
        void Start();

        /// <summary>
        /// Requests the action to finish gracefully, e.g. finalize, dispose. An action might not transition to finished stated
        /// immediately. Fails if an action is not started or already completed
        /// </summary>
        void Finish();

        /// <summary>
        /// Updates and progresses internal action state
        /// </summary>
        void Update();
    }
}