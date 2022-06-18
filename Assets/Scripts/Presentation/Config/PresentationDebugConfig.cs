#if UNITY_EDITOR || DEVELOPMENT_BUILD
using UnityEngine;

namespace Presentation.Config
{
    [CreateAssetMenu(fileName = "PresentationDebugConfig", menuName = "Config/Presentation/PresentationDebugConfig")]
    class PresentationDebugConfig : ScriptableObject { }
}
#endif