#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#nullable enable

using UnityEngine;

namespace GameLogic.Config
{
    [CreateAssetMenu(fileName = "GameplayConfig", menuName = "Config/GameLogic/GameplayConfig")]
    class GameplayConfig : ScriptableObject { }
}