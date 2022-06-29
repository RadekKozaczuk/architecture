using System;
using Shared.AI.Interfaces;

namespace Shared.AI
{
    /// <summary>
    /// Base class for state machine actions
    /// </summary>
    public abstract class StateMachineActionBase : IStateMachineAction
    {
        public StateMachineActionState CurrentState { get; protected set; }
        public string FailureReason { get; protected set; }

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

        public abstract void Update();
    }
}