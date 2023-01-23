using Common.Enums;
using Common.Systems;
using UnityEngine;

namespace Presentation.Views
{
    [DisallowMultipleComponent]
    class PortalView : MonoBehaviour
    {
        void OnTriggerEnter(Collider col)
        {
            if (!col.TryGetComponent(out PlayerView _))
                return;

            GameStateSystem.RequestStateChange(GameState.Gameplay);
        }
    }
}