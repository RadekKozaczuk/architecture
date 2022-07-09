using System;

namespace Shared.AI
{
    /// <summary>
    /// Base class for state machine actions
    /// </summary>
    public abstract class StateMachineActionBase
    {
        /// <summary>
        /// Current action state
        /// </summary>
        internal StateMachineActionState CurrentState { get; set; }
        /// <summary>
        /// Action failure reason
        /// </summary>
        public string FailureReason { get; protected set; }

        /// <summary>
        /// Initializes the action. Fails if action is already started or completed.
        /// </summary>
        public virtual void Start()
        {
            switch (CurrentState)
            {
                case StateMachineActionState.Awaiting:
                    CurrentState = StateMachineActionState.Starting;
                    break;
                case StateMachineActionState.Starting:
                    break;
                default:
                    throw new InvalidOperationException($"Cannot start action when in {CurrentState} state");
            }
        }

        /// <summary>
        /// Requests the action to finish gracefully, e.g. finalize, dispose. An action might not transition to finished stated
        /// immediately. Fails if an action is not started or already completed
        /// </summary>
        public virtual void Finish()
        {
            switch (CurrentState)
            {
                case StateMachineActionState.Starting:
                case StateMachineActionState.InProgress:
                    CurrentState = StateMachineActionState.Finishing;
                    break;
                case StateMachineActionState.Finishing:
                    break;

                default:
                    throw new InvalidOperationException($"Cannot finish action when in {CurrentState} state");
            }
        }

        /// <summary>
        /// Updates and progresses internal action state
        /// </summary>
        public abstract void Update();
    }
}