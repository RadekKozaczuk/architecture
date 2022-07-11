using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Shared.Interfaces;

namespace Shared.AI
{
    public delegate void StateMachineActionEvent([NotNull] StateMachine sender, [NotNull] StateMachineActionBase action);

    public class StateMachine : ICustomUpdate
    {
        [CanBeNull]
        public StateMachineActionEvent OnActionStateChanged;

        [CanBeNull]
        public StateMachineActionEvent OnCurrentActionChanged;

        [CanBeNull]
        public StateMachineActionEvent OnActionFinished;

        [CanBeNull]
        public StateMachineActionEvent OnActionFailed;

        readonly List<StateMachineActionBase> _actions = new();

        public StateMachineActionBase CurrentAction => _actions.Count > 0 ? _actions[0] : null;

        public IReadOnlyList<StateMachineActionBase> Actions => _actions;

        public void CustomUpdate()
        {
            StateMachineActionBase currentAction = CurrentAction;
            if (currentAction == null)
                return;

            switch (currentAction.CurrentState)
            {
                case StateMachineActionState.Awaiting:
                    StartAction(currentAction);
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

            UpdateAction(currentAction);

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

        public void TransitionToAction(StateMachineActionBase action)
        {
            StateMachineActionBase currentAction = CurrentAction;
            RemoveAllQueuedActions();

            CurrentAction?.Finish();

            if (action == null)
                return;

            _actions.Add(action);

            if (currentAction != CurrentAction)
                OnCurrentActionChanged?.Invoke(this, action);
        }

        public void QueueAction(StateMachineActionBase action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            StateMachineActionBase currentAction = CurrentAction;

            _actions.Add(action);

            if (currentAction != CurrentAction)
                OnCurrentActionChanged?.Invoke(this, action);
        }

        void RemoveAllQueuedActions()
        {
            for (int i = _actions.Count - 1; i > 0; --i)
                _actions.RemoveAt(i);

            if (CurrentAction == null)
                return;

            if (!CurrentAction.IsRunning())
                _actions.RemoveAt(0);
        }

        void StartAction([NotNull] StateMachineActionBase action)
        {
            Assert.False(action == null, "StateMachine tried to start null action.");

            action.Start();
            OnActionStateChanged?.Invoke(this, action);
        }

        void UpdateAction([NotNull] StateMachineActionBase action)
        {
            Assert.False(action == null, "StateMachine tried to start null action.");

            StateMachineActionState currentState = action.CurrentState;

            action.Update();

            if (currentState != action.CurrentState)
                OnActionStateChanged?.Invoke(this, action);
        }
    }
}