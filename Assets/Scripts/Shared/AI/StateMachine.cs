using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Shared.AI.Interfaces;
using UnityEngine;

namespace Shared.AI
{
    public delegate void StateMachineActionEvent([NotNull] StateMachine sender, [NotNull] IStateMachineAction action);

    public class StateMachine
    {
        [CanBeNull]
        public StateMachineActionEvent OnActionStateChanged;

        [CanBeNull]
        public StateMachineActionEvent OnCurrentActionChanged;

        [CanBeNull]
        public StateMachineActionEvent OnActionFinished;

        [CanBeNull]
        public StateMachineActionEvent OnActionFailed;

        readonly List<IStateMachineAction> _actions = new();

        public IStateMachineAction CurrentAction => _actions.Count > 0
            ? _actions[0]
            : null;

        public IReadOnlyList<IStateMachineAction> Actions => _actions;

        public virtual void Update()
        {
            IStateMachineAction currentAction = CurrentAction;
            if (currentAction == null)
                return;

            switch (currentAction.CurrentState)
            {
                case StateMachineActionState.Awaiting:
                    if (!TryStartAction(currentAction))
                        return;

                    break;
                case StateMachineActionState.Starting:
                case StateMachineActionState.InProgress:
                case StateMachineActionState.Finishing:
                    break;
                case StateMachineActionState.Succeeded:
                case StateMachineActionState.Failed:
                    throw new InvalidOperationException($"Didn't expect action to be in {currentAction.CurrentState} state");
                default:
                    throw new ArgumentOutOfRangeException(currentAction.CurrentState.ToString());
            }

            if (!TryUpdateAction(currentAction))
                return;

            switch (currentAction.CurrentState)
            {
                case StateMachineActionState.Awaiting:
                    throw new InvalidOperationException($"Didn't expect action to be in {currentAction.CurrentState} state");
                case StateMachineActionState.Starting:
                case StateMachineActionState.InProgress:
                case StateMachineActionState.Finishing:
                    break;
                case StateMachineActionState.Succeeded:
                    _actions.RemoveAt(0);
                    OnActionFinished?.Invoke(this, currentAction);
                    if (CurrentAction != null)
                        OnCurrentActionChanged?.Invoke(this, CurrentAction);
                    break;
                case StateMachineActionState.Failed:
                    _actions.Clear();
                    OnActionFinished?.Invoke(this, currentAction);
                    OnActionFailed?.Invoke(this, currentAction);
                    if (CurrentAction != null)
                        OnCurrentActionChanged?.Invoke(this, CurrentAction);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(currentAction.CurrentState.ToString());
            }
        }

        public void TransitionToAction(IStateMachineAction action)
        {
            IStateMachineAction currentAction = CurrentAction;
            RemoveAllQueuedActions();

            if (CurrentAction != null)
                TryFinishAction(CurrentAction);

            if (action == null)
                return;

            _actions.Add(action);

            if (currentAction != CurrentAction)
                OnCurrentActionChanged?.Invoke(this, action);
        }

        public void QueueAction(IStateMachineAction action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            IStateMachineAction currentAction = CurrentAction;

            _actions.Add(action);

            if (currentAction != CurrentAction)
                OnCurrentActionChanged?.Invoke(this, action);
        }

        protected void RemoveAllQueuedActions()
        {
            for (int i = _actions.Count - 1 ; i > 0 ; --i)
                _actions.RemoveAt(i);

            if (CurrentAction == null)
                return;

            if (!CurrentAction.IsRunning())
                _actions.RemoveAt(0);
        }

        bool TryFinishAction([NotNull] IStateMachineAction action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            try
            {
                action.Finish();
                return true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                _actions.Remove(action);
                OnActionFailed?.Invoke(this, action);
                return false;
            }
        }

        bool TryStartAction([NotNull] IStateMachineAction action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            try
            {
                action.Start();
                OnActionStateChanged?.Invoke(this, action);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                _actions.Remove(action);
                OnActionFailed?.Invoke(this, action);
                return false;
            }
        }

        bool TryUpdateAction([NotNull] IStateMachineAction action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            try
            {
                StateMachineActionState currentState = action.CurrentState;

                action.Update();

                if (currentState != action.CurrentState)
                    OnActionStateChanged?.Invoke(this, action);

                return true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                _actions.Remove(action);
                OnActionFailed?.Invoke(this, action);
                return false;
            }
        }
    }
}