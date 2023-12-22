#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Presentation.Views;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Presentation.Config
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Config/Presentation/PlayerConfig")]
    class PlayerConfig : ScriptableObject
    {
        [SerializeField]
        [InfoBox("Player prefab when we play in single player.", InfoMessageType.None)]
        internal PlayerView PlayerPrefab;

        [SerializeField]
        [InfoBox("Player prefab when we play in multiplayer and the player is controller by the server.", InfoMessageType.None)]
        internal PlayerNetworkView PlayerServerPrefab;

        [SerializeField]
        [InfoBox("Player prefab when we play in multiplayer and the player is controller by the client.", InfoMessageType.None)]
        internal PlayerNetworkView PlayerClientPrefab;

        [Min(0)]
        [MaxValue(100)]
        [SerializeField]
        [InfoBox("In Unity units per second.", InfoMessageType.None)]
        internal float PlayerSpeed = 7;
    }
}