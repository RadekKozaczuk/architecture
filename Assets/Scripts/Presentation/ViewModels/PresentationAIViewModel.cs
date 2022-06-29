using Presentation.Views;
using Shared.AI.Actions;
using UnityEngine;
using UnityEngine.Scripting;

namespace Presentation.ViewModels
{
    public partial class PresentationViewModel
    {
        [Preserve]
        PresentationViewModel()
        {
            
        }
        
        public static void FollowPlayerTarget(int agentId, float approachDistance = 2f)
        {
            EnemyView view = PresentationData.Enemy;
            Transform target = PresentationData.Player.transform;

            var navigationTarget = new OffsetNavigationTarget(
                new TransformNavigationTarget(target, false), approachDistance);
            view.SetTransitionToFollowAction(navigationTarget, .2f);
        }

        /// <summary>
        /// Continuously rotates NPC towards player until action is cancelled.
        /// </summary>
        public static void RotateNpcToPlayer(int agentId)
        {
            EnemyView view = PresentationData.Enemy;
            Transform target = PresentationData.Player.transform;

            var navigationTarget = new TransformNavigationTarget(target, false);
            var lookAtTarget = new LookAtTarget(navigationTarget) { AngleThreshold = 5f };

            view.SetTransitionToFollowAction(lookAtTarget, .2f);
        }

        /// <summary>
        /// Stops NPC's navigation action that is currently active.
        /// E.g. (Following player, rotating towards player, going to specific location)
        /// </summary>
        public static void StopAgentNavigation(int agentId)
        {
            EnemyView view = PresentationData.Enemy;
            view.FinishCurrentAction();
        }
    }
}