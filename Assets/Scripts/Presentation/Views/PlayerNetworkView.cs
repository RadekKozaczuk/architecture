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

        static readonly PlayerConfig _playerConfig;

        internal void Move(Vector2 v)
        {
            Assert.True(v.sqrMagnitude < Vector3.one.sqrMagnitude, "Vector v must be normalized.");

            if (!IsOwner)
                return;

            Vector3 movement = new Vector3(v.x, 0, v.y) * _playerConfig.PlayerSpeed;
            transform.Translate(movement * Time.deltaTime);
        }

        public override void OnNetworkSpawn()
        {
            _hp.OnValueChanged += (byte prevVal, byte newVal) =>
            {
                Debug.Log("fdg");
            };
            base.OnNetworkSpawn();
        }

        // server rpc are functions that can be called both by client and server
        // but WILL BE RUN ONLY ON THE SERVER
        // server rpc functions can only be declared inside network objects
        [ServerRpc]
        void TestServerRpc()
        {

        }

        // client rpc are functions that can only be called by server
        // but WILL BE RUN on all or selected clients 
        // client rpc functions can only be declared inside network objects
        [ClientRpc]
        void TestClientRpc()
        {

        }
    }
}