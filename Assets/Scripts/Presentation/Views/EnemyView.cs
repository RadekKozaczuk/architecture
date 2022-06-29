using System;
using JetBrains.Annotations;
using Shared.AI;
using Shared.AI.Actions;
using Shared.AI.Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace Presentation.Views
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyView : MonoBehaviour
    {
        [SerializeField]
        Animator _animator;
        [SerializeField]
        NavMeshAgent _agent;
        
        NavMeshNavigationController _navMeshNavigationController;
        StateMachineCharacterController _stateMachineCharacterController;

        internal void Initialize()
        {
            _navMeshNavigationController = new NavMeshNavigationController(_agent);
            _stateMachineCharacterController = new StateMachineCharacterController(_navMeshNavigationController);

            _stateMachineCharacterController.OnActionStateChanged += HandleActionStateChanged;
            _stateMachineCharacterController.OnCurrentActionChanged += HandleCurrentActionChanged;
            _stateMachineCharacterController.OnActionFinished += HandleActionFinished;
            _stateMachineCharacterController.OnActionFailed += HandleActionFailed;
        }
        
        internal void SetTransitionToAction(StaticNavigationTarget navigationTarget)
        {
            _stateMachineCharacterController.StateMachine.TransitionToAction(
                new NavigateToTargetAction(_navMeshNavigationController, navigationTarget));
        }

        internal void SetTransitionToAction(OffsetNavigationTarget navigationTarget, float targetPathRefreshInternal)
        {
            _stateMachineCharacterController.StateMachine.TransitionToAction(
                new NavigateToTargetAction(_navMeshNavigationController, navigationTarget)
                {
                    TargetPathRefreshInterval = TimeSpan.FromSeconds(targetPathRefreshInternal),
                });
        }

        internal void SetTransitionToFollowAction(INavigationTarget navigationTarget, float targetPathRefreshInternal)
        {
            _stateMachineCharacterController.StateMachine.TransitionToAction(
                new FollowTargetAction(_navMeshNavigationController, navigationTarget)
                {
                    TargetPathRefreshInterval = TimeSpan.FromSeconds(targetPathRefreshInternal),
                });
        }

        internal void FinishCurrentAction()
        {
            _stateMachineCharacterController.StateMachine.CurrentAction?.Finish();
        }
        
        void HandleActionStateChanged([NotNull] StateMachineCharacterController sender, [NotNull] IStateMachineAction action)
        {
        }

        void HandleCurrentActionChanged([NotNull] StateMachineCharacterController sender, [NotNull] IStateMachineAction action)
        {
            //ARSceneReferenceHolder.PlayerNavMeshObstacle.enabled = true;
        }

        void HandleActionFinished([NotNull] StateMachineCharacterController sender, [NotNull] IStateMachineAction action)
        {
            // TODO: for now only this one interests us
            //SignalProcessor.SendSignal(new AIEventSignal(Id, SpecialNpcType));
        }

        void HandleActionFailed([NotNull] StateMachineCharacterController sender, [NotNull] IStateMachineAction action)
        {
        }
    }
}