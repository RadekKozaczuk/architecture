using UnityEngine;

namespace Presentation.AnimationBehaviours
{
    // AnimationBehaviours are extra pieces of code that we can add to animations in state machine.
    class ExampleAnimationBehaviour : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }
    }
}