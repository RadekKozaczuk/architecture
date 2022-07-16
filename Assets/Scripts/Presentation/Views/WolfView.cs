using System;
using JetBrains.Annotations;
using Shared.AI;
using Shared.AI.NavigationTargets;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Presentation.Views
{
    [DisallowMultipleComponent]
    class WolfView : EnemyView
    {
        enum WolfAIState
        {
            IsGoingTowardsPlayer,
            IsChargingJump,
            IsPerformingJump,
            IsThinkingWhatNext
        }

        WolfAIState _state;
        float _timer;

        Vector3 _dirVec;

        internal new void Initialize()
        {
            base.Initialize();
            _stateMachineCharacterController.OnActionFinished += HandleActionFinished;
        }

        internal new void CustomUpdate()
        {
            base.CustomUpdate();

            // TODO: dummy solution normally Initialize method is called upon object instantiation
            if (!_initialize)
                return;

            switch (_state)
            {
                case WolfAIState.IsGoingTowardsPlayer:

                case WolfAIState.IsChargingJump:
                    // TODO: add here some animation of barking dog 

                    _timer -= Time.deltaTime;
                    if (_timer > 0)
                        return;

                    // get direction vector to the player
                    _dirVec = (PresentationSceneReferenceHolder.Target.position - transform.position).normalized;
                    _state = WolfAIState.IsPerformingJump;
                    _timer = 1f;
                    return;
                case WolfAIState.IsPerformingJump:
                    // travel for 1s on a straight line towards player and either hit a wall/prop, the player, or nothing
                    // add animation when hit the wall, and where hit a player

                    // go in this direction for a duration of a second
                    transform.position += _dirVec;

                    _timer -= Time.deltaTime;
                    if (_timer > 0)
                        return;

                    _state = WolfAIState.IsThinkingWhatNext;

                    return;
                case WolfAIState.IsThinkingWhatNext:
                    // probably just go again towards player
                    // or maybe if damage start skomlec ane go away somewhere
                    return;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        internal void DoWolfieThings()
        {
            float approachDistance = Random.Range(10f, 20f);
            float approachAngle = Random.Range(0f, 360f);

            Transform target = PresentationSceneReferenceHolder.Target; // should be based on targetId

            // TODO: to trzeba zrobic jako jeden po prostu niech tranform ma te dodatkowe parametry nie ma sensu opakowywac go
            // TODO: to strasznie nie czytelne
            var navigationTarget = new OffsetNavigationTarget(new TransformNavigationTarget(target, true), approachDistance, approachAngle);

            SetTransitionToAction(navigationTarget, 1f);
        }

        void HandleActionFinished([NotNull] StateMachineCharacterController sender, [NotNull] StateMachineActionBase action)
        {
            // wait a little bittinio (like for example 1 sec)
            // and if player does not go away perform jump attack
            Debug.Log("Doggo is charging to perform a jump attack.");

            _state = WolfAIState.IsChargingJump;
            _timer = 1f; // TODO: should be taken from the config
        }
    }
}