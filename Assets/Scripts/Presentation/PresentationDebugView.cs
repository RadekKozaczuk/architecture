using Sirenix.OdinInspector;
using UnityEngine;

namespace Presentation.Views
{
    [DisallowMultipleComponent]
    public class PresentationDebugView : MonoBehaviour
    {
        [InfoBox("Broke engine.")]
        [Button]
        void BrokeOrRepairEngine() { }
    }
}