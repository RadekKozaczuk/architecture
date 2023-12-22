#if UNITY_EDITOR || DEVELOPMENT_BUILD
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#nullable enable
using UnityEngine;

namespace Presentation.Config
{
    [CreateAssetMenu(fileName = "PresentationDebugConfig", menuName = "Config/Presentation/PresentationDebugConfig")]
    class PresentationDebugConfig : ScriptableObject { }
}
#endif