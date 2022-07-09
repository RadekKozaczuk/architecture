using Presentation.Views;
using Shared.AI.Actions;
using Shared.AI.NavigationTargets;
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
        
        /// <summary>
        /// This constantly follows player with no end goal. Just follows till the end of time.
        /// Has to be cancelled explicitly.
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="approachDistance"></param>
        static void FollowPlayerTarget(int agentId, float approachDistance = 2f)
        {
            // TODO: use the id
            
            EnemyView view = PresentationSceneReferenceHolder.Enemy;
            Transform target = PresentationSceneReferenceHolder.Target;

            var navigationTarget = new OffsetNavigationTarget(new TransformNavigationTarget(target, false), approachDistance);
            view.SetTransitionToFollowAction(navigationTarget, .2f);
        }

        /// <summary>
        /// Continuously rotates NPC towards player until action is cancelled.
        /// </summary>
        public static void RotateNpcToPlayer(int agentId)
        {
            Debug.Log("rotate called");
            
            // TODO: use the id
            EnemyView view = PresentationSceneReferenceHolder.Enemy;
            Transform target = PresentationSceneReferenceHolder.Target;

            var lookAtTarget = new LookAtTarget(target, 5f);

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
        
        /// <summary>
        /// This action ends one the target is reached
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="targetId"></param>
        /// <param name="approachDistance"></param>
        public static void GoAndFaceTargetWithOffset(int agentId, int targetId, float approachDistance)
        {
            EnemyView view = PresentationSceneReferenceHolder.Enemy; // should be based on agent Id
            Transform target = PresentationSceneReferenceHolder.Target; // should be based on targetId
            
            var navigationTarget = new OffsetNavigationTarget(
                new TransformNavigationTarget(target, true), approachDistance, 120, 180);

            view.SetTransitionToAction(navigationTarget, 1f);
        }
    }
}