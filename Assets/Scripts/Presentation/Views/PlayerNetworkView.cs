#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Core;
using Presentation.Config;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;

namespace Presentation.Views
{
    [DisallowMultipleComponent]
    class PlayerNetworkView : NetworkBehaviour
    {
        [SerializeField]
        internal NetworkObject NetworkObj;

        static readonly PlayerConfig _playerConfig;

        // OnNetworkSpawn is called both on client and server
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsOwner)
                PresentationData.NetworkPlayers[(int)CoreData.PlayerId!.Value] = this;
        }

        internal void Move(Vector2 v)
        {
            Assert.IsTrue(v.sqrMagnitude < Vector3.one.sqrMagnitude, "Vector v must be normalized.");

            if (!IsOwner)
                return;

            Vector3 movement = new Vector3(v.x, 0, v.y) * _playerConfig.PlayerSpeed;
            transform.Translate(movement * Time.deltaTime);
        }
    }
}