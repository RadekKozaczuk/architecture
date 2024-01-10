#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.Collections.Generic;
using Common.Enums;
using Shared.Systems;
using UnityEngine;

// ReSharper disable UnusedParameter.Global

namespace Common
{
    public static class Signals
    {
        class SignalsImplementation : ISignals
        {
            void ISignals.InventoryChanged() { }
            /*void ISignals.LobbyChanged(string lobbyName, string lobbyCode, List<(string playerName, string playerId, bool isHost)> players) { }
            void ISignals.MissionComplete() { }
            void ISignals.MissionFailed() { }
            void ISignals.PlaySound(Vector3 position, Sound sound) { }
            void ISignals.PopupRequested(PopupType popupType) { }*/
            void ISignals.HpChanged(int a, float b) { } // for test only, delete when ready
        }

        /// <summary>
        /// Universal signal indicating that something has changed in the inventory.
        /// An item was added, removed, moved to a different slot, or its quantity has changed.
        /// </summary>
        public static void InventoryChanged() => _signals.InventoryChanged();

        /// <summary>
        /// Indicates that something has changed in the lobby.
        /// If LobbyName is different than null then the name changed.
        /// If Players list is different than null then something changed in one or more players.
        /// The list always represents the current state, and not only the change.
        /// </summary>
        /*public static void LobbyChanged(string lobbyName, string lobbyCode, List<(string playerName, string playerId, bool isHost)> players) =>
            _signals.LobbyChanged(lobbyName, lobbyCode, players);
        public static void MissionComplete() => _signals.MissionComplete();
        public static void MissionFailed() => _signals.MissionFailed();
        public static void PlaySound(Vector3 position, Sound sound) => _signals.PlaySound(position, sound);
        public static void PopupRequested(PopupType popupType) => _signals.PopupRequested(popupType);*/
        public static void HpChanged(int a, float b) => _signals.HpChanged(a, b);

        static readonly ISignals _signals = Architecture.Interception<ISignals>(new SignalsImplementation());
    }

    public interface ISignals
    {
        void InventoryChanged();
        /*void LobbyChanged(string lobbyName, string lobbyCode, List<(string playerName, string playerId, bool isHost)> players);
        void MissionComplete();
        void MissionFailed();
        void PlaySound(Vector3 position, Sound sound);
        void PopupRequested(PopupType popupType);*/
        void HpChanged(int a, float b);
    }
}
