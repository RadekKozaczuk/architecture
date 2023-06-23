using Common.Enums;
using Presentation.Config;
using Shared;
using Unity.Netcode;
using UnityEngine;

namespace Presentation.Views
{
    [DisallowMultipleComponent]
    class PlayerNetworkView : NetworkBehaviour
    {
        [SerializeField]
        internal NetworkObject NetworkObj;

        // network variables can only be declared inside network objects
        readonly NetworkVariable<byte> _hp = new ();
        public NetworkVariable<PlayerId> PlayerId;

        static readonly PlayerConfig _playerConfig;

        internal void Move(Vector2 v)
        {
            Assert.True(v.sqrMagnitude < Vector3.one.sqrMagnitude, "Vector v must be normalized.");

            if (!IsOwner)
                return;

            Vector3 movement = new Vector3(v.x, 0, v.y) * _playerConfig.PlayerSpeed;
            transform.Translate(movement * Time.deltaTime);
        }

        // todo: pytanie czy to sie odapala i na cliencie i na hoscie
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            Debug.Log($"object spawned name :{name} and PlayerId: {(int)PlayerId.Value}");
            PresentationData.NetworkPlayers[(int)PlayerId.Value] = this;
        }

        [ServerRpc(RequireOwnership = false)]
        public void TestServerRpc()
        {
            Debug.Log($"executing server rpc");
        }

        [ClientRpc]
        public void TestClientRpc()
        {
            Debug.Log($"executing client rpc");
        }
    }
}