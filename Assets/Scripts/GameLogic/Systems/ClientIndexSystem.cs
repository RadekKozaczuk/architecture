using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using System.Linq;
using Core;

namespace GameLogic.Systems
{
    public static class ClientIndexSystem
    {
        static readonly List<ulong?> _ids = new();

        [RuntimeInitializeOnLoadMethod]
        public static async void Initialize()
        {
            while (!NetworkManager.Singleton)
                await Task.Yield();

            NetworkManager.Singleton.OnClientConnectedCallback += RegistryClient;
            NetworkManager.Singleton.OnClientDisconnectCallback += UnRegistryClient;
        }

        static void RegistryClient(ulong clientId)
        {
            if (!NetworkManager.Singleton.IsHost && !NetworkManager.Singleton.IsServer)
                return;

            int freeIndex = _ids.
                Select((id, index) => new {id, index}).
                Where(i => i.id == null).
                Select(i => i.index).
                Where(i => i > 0).
                DefaultIfEmpty(_ids.Count).
                First();

            if (freeIndex == _ids.Count)
                _ids.Add(null);

            _ids[freeIndex] = clientId;
            Signals.ClientConnected(clientId, freeIndex);
        }

        static void UnRegistryClient(ulong disconnectClientId)
        {
            if (!NetworkManager.Singleton.IsHost && !NetworkManager.Singleton.IsServer)
                return;

            int freeIndex = _ids.
                Select((id, index) => new {id, index}).
                Where(client => client.id == disconnectClientId).
                Select(client => client.index).
                First();

            _ids[freeIndex] = null;
            Signals.ClientDisconnected(disconnectClientId, freeIndex);

            if (disconnectClientId == 0)
                _ids.Clear();
        }
    }
}