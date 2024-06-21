#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Core.Enums;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Config
{
    /// <summary>
    /// List of scenes that should be loaded with lower priority to extend loading over a longer period of time and prevent performance drops.
    /// </summary>
    [CreateAssetMenu(fileName = "SceneConfig", menuName = "Config/Core/SceneConfig")]
    public class SceneConfig : ScriptableObject
    {
        // todo: add validation that checks if a scene is listed no more than once
        // todo: add quality of life improvement that set the scene value of a newly added element to something that is not yet present on the list

        public enum ActivationMode
        {
            StateChange, OverTime
        }

        [System.Serializable]
        public class ExtActivation
        {
            [TableColumnWidth(100)]
            public Level Level = Level.HubLocation;
            [TableColumnWidth(100)]
            public ActivationMode When;
            [Range(0, 2)]
            [SuffixLabel("s", true)]
            [TableColumnWidth(100)]
            [DisableIf("@this.When == ActivationMode.StateChange")]
            public float ActivationTime = 0.5f;
        }

        [Space(15)]

        [TableList]
        [SerializeField]
        [InfoBox("Scenes NOT listed here will be loaded normally (as they are in the editor). \n"
                 + "Scenes listed here should have all their root objects deactivated in the editor. \n\n"
                 + "StateChange means the scene will activate all its root objects immediately during the next GameStateMachine.RequestStateChange call. \n"
                 + "OverTime means the scene will start activate it roots immediately after the scene is loaded but will spread activation "
                 + "over the given amount of time. Equal amount of root objects per frame. \n\n"
                 + "StateChange is useful when we want to load a bigger scene earlier but activate it when the actual state transition happens. \n"
                 + "OverTime is useful when we want to spread activation process over because it is heavy and we know the scene won't be seen by the player during the process anyway.", InfoMessageType.None)]
        public ExtActivation[] CustomActivation;
    }
}