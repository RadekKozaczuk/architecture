#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Sirenix.OdinInspector;
using UI.Views;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

namespace UI.Config
{
    [CreateAssetMenu(fileName = "UIConfig", menuName = "Config/UI/UIConfig")]
    class UIConfig : ScriptableObject
    {
        [SerializeField]
        internal InputActionAsset InputActionAsset;

        [SerializeField]
        [Range(0, 0.3f)]
        [InfoBox("How high of a percent of the screen height player has to move finger for a swipe.", InfoMessageType.None)]
        [SuffixLabel("s")]
        internal float SwipeMinPercentage = 0.2f;

        [SerializeField]
        internal LobbyListElementView LobbyListElement;

        [SerializeField]
        internal LobbyPlayerElementView LobbyPlayerElementView;

        [SerializeField]
        internal AudioMixer AudioMixer;
    }
}