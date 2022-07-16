using Presentation.Views;
using Shared;
using Shared.AI.NavigationTargets;
using UnityEngine;
using UnityEngine.Scripting;

namespace Presentation.ViewModels
{
    public partial class PresentationViewModel
    {
        [Preserve]
        PresentationViewModel() { }

        /// <summary>
        /// Continuously rotates NPC towards player until action is cancelled.
        /// </summary>
        public static void RotateNpcToPlayer(int agentId)
        {
            // TODO: use the id
            EnemyView view = PresentationSceneReferenceHolder.Wolf1;
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
        /// <param name="enemyView"></param>
        /// <param name="targetId"></param>
        /// <param name="approachDistance"></param>
        /// <param name="approachAngle"></param>
        /// <param name="targetRelativeYaw"></param>
        internal static void GoAndFaceTargetWithOffset(
            EnemyView enemyView, int targetId, float approachDistance, float approachAngle = 0f, float targetRelativeYaw = 0f
        )
        {
            Assert.True(approachDistance >= 0, "ApproachDistance cannot be lower than 0.");
            Assert.True(approachAngle is >= 0 and <= 360, "ApproachAngle must be a value from 0 to 360.");
            Assert.True(targetRelativeYaw is >= 0 and <= 360, "TargetRelativeYaw must be a value from 0 to 360.");

            Transform target = PresentationSceneReferenceHolder.Target; // should be based on targetId

            // TODO: to trzeba zrobic jako jeden po prostu niech tranform ma te dodatkowe parametry nie ma sensu opakowywac go
            // TODO: to strasznie nie czytelne
            var navigationTarget = new OffsetNavigationTarget(
                new TransformNavigationTarget(target, true), approachDistance, approachAngle, targetRelativeYaw);

            enemyView.SetTransitionToAction(navigationTarget, 1f);
        }

        /// <summary>
        /// This constantly follows player with no end goal. Just follows till the end of time.
        /// Has to be cancelled explicitly.
        /// </summary>
        /// <param name="enemyView"></param>
        /// <param name="approachDistance"></param>
        static void FollowPlayerTarget(EnemyView enemyView, float approachDistance = 2f)
        {
            // TODO: use the id

            Transform target = PresentationSceneReferenceHolder.Target;

            var navigationTarget = new OffsetNavigationTarget(new TransformNavigationTarget(target, false), approachDistance);
            enemyView.SetTransitionToFollowAction(navigationTarget, .2f);
        }
    }
}