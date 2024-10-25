#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Core;
using Core.Enums;
using Core.Systems;
using UnityEngine;

namespace Presentation.Views
{
    [DisallowMultipleComponent]
    class PortalView : MonoBehaviour
    {
        void OnTriggerEnter(Collider col)
        {
            if (col.TryGetComponent(out PlayerView _) || col.TryGetComponent(out PlayerNetworkView _))
                GameStateSystem.ChangeState(GameState.Gameplay, scenesToSynchronize: new []{(int)CoreData.CurrentLevel + 1});
        }
    }
}