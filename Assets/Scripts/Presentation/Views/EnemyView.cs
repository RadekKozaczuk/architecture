using System;
using JetBrains.Annotations;
using Shared.AI;
using Shared.AI.Actions;
using Shared.AI.NavigationTargets;
using Shared.Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace Presentation.Views
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NavMeshAgent))]
    class EnemyView : MonoBehaviour, ICustomUpdate
    {
        [SerializeField]
        Animator _animator;
        [SerializeField]
        NavMeshAgent _agent;
        
        NavMeshNavigationController _navMeshNavigationController;
        StateMachineCharacterController _stateMachineCharacterController;

        bool _initialize;
        
        public void CustomUpdate()
        {
            // TODO: dummy solution normally Initialize method is called upon object instantiation
            if(!_initialize)
                return;
            
            // TODO: change so that only one update is needed - this is needlessly convoluted
            _navMeshNavigationController.CustomUpdate();
            _stateMachineCharacterController.CustomUpdate();
        }
        
        internal void Initialize()
        {
            // TODO: dummy solution normally Initialize method is called upon object instantiation
            _initialize = true;
            
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
        
        void HandleActionStateChanged([NotNull] StateMachineCharacterController sender, [NotNull] StateMachineActionBase action)
        {
            Debug.Log("ActionStateChanged");
        }

        void HandleCurrentActionChanged([NotNull] StateMachineCharacterController sender, [NotNull] StateMachineActionBase action)
        {
            Debug.Log("CurrentActionChanged");
            //ARSceneReferenceHolder.PlayerNavMeshObstacle.enabled = true;
        }

        void HandleActionFinished([NotNull] StateMachineCharacterController sender, [NotNull] StateMachineActionBase action)
        {
            Debug.Log("ActionFinished");
            // TODO: for now only this one interests us
            //SignalProcessor.SendSignal(new AIEventSignal(Id, SpecialNpcType));
        }

        void HandleActionFailed([NotNull] StateMachineCharacterController sender, [NotNull] StateMachineActionBase action)
        {
            Debug.Log("ActionFailed");
        }
    }
}