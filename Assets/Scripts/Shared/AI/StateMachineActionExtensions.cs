using System;
using JetBrains.Annotations;

namespace Shared.AI
{
    public static class StateMachineActionExtensions
    {
        /// <summary>
        /// Returns true if action is currently running, false otherwise
        /// </summary>
        public static bool IsRunning([NotNull] this StateMachineActionBase self)
        {
            if (self == null)
                throw new ArgumentNullException(nameof(self));

            switch (self.CurrentState)
            {
                case StateMachineActionState.Awaiting:
                case StateMachineActionState.Succeeded:
                case StateMachineActionState.Failed:
                    return false;
                case StateMachineActionState.Starting:
                case StateMachineActionState.InProgress:
                case StateMachineActionState.Finishing:
                    return true;
                default:
                    throw new ArgumentOutOfRangeException(self.CurrentState.ToString());
            }
        }
    }
}