using System;
using JetBrains.Annotations;
using Shared.AI.NavigationTargets;

namespace Shared.AI.Actions
{
    /// <summary>
    /// A state machine action that follows the target. The action will be executing infinitely if not failed or finished
    /// explicitly.
    /// </summary>
    public class FollowTargetAction : NavigateToTargetAction
    {
        public FollowTargetAction([NotNull] NavMeshNavigationController controller, [NotNull] INavigationTarget target)
            : base(controller, target) { }

        protected override void FinalizeAction(bool succeeded, string failureReason = null)
        {
            if (!succeeded || CurrentState == StateMachineActionState.Finishing)
            {
                base.FinalizeAction(succeeded, failureReason);
                return;
            }

            ActionStartTime = DateTime.Now;
        }
    }
}