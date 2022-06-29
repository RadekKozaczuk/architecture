using JetBrains.Annotations;
using Shared.AI.Interfaces;
using Shared.Interfaces;

namespace Shared.AI
{
    public delegate void StateMachineControllerActionEvent(
        [NotNull] StateMachineCharacterController sender, 
        [NotNull] IStateMachineAction action);
    
    public class StateMachineCharacterController : ICustomUpdate
    {
        public event StateMachineControllerActionEvent OnActionStateChanged;
        public event StateMachineControllerActionEvent OnCurrentActionChanged;
        public event StateMachineControllerActionEvent OnActionFinished;
        public event StateMachineControllerActionEvent OnActionFailed;
        
        public readonly NavMeshNavigationController NavigationController;
        public readonly StateMachine StateMachine;

        public StateMachineCharacterController(NavMeshNavigationController controller)
        {
            NavigationController = controller;
            StateMachine = new StateMachine();
            StateMachine.OnActionFailed += (_, action) => OnActionFailed?.Invoke(this, action);
            StateMachine.OnActionFinished += (_, action) => OnActionFinished?.Invoke(this, action);
            StateMachine.OnActionStateChanged += (_, action) => OnActionStateChanged?.Invoke(this, action);
            StateMachine.OnCurrentActionChanged += (_, action) => OnCurrentActionChanged?.Invoke(this, action);
        }

        public void CustomUpdate()
        {
            StateMachine.Update();
        }
    }
}