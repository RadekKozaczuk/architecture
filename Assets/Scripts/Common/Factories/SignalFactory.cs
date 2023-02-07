using Common.Enums;
using Common.Signals;
using Shared.Systems;
using UnityEngine;

namespace Common.Factories
{
    /// <summary>
    /// Interface for sending signals
    /// </summary>
    public static class SignalFactory
    {
        /// <summary>
        /// Method for sending <see cref="InventoryChangedSignal">InventoryChanged signals.</see>
        /// </summary>
        public static void SendInventoryChangedSignal() => SignalProcessor.SendSignal(new InventoryChangedSignal());

        /// <summary>
        /// Method for sending <see cref="MissionCompleteSignal">MissionComplete signals.</see>
        /// </summary>
        public static void SendMissionCompleteSignal() => SignalProcessor.SendSignal(new MissionCompleteSignal());

        /// <summary>
        /// Method for sending <see cref="MissionFailedSignal">MissionFailed signals.</see>
        /// </summary>
        public static void SendMissionFailedSignal() => SignalProcessor.SendSignal(new MissionFailedSignal());

        /// <summary>
        /// Method for sending <see cref="PlaySoundSignal">PlaySound signals.</see>
        /// </summary>
        public static void SendPlaySoundSignal(Vector3 position, Sound type) => SignalProcessor.SendSignal(new PlaySoundSignal(position, type));

        /// <summary>
        /// Method for sending <see cref="PopupRequestedSignal">PopupRequested signals.</see>
        /// </summary>
        public static void SendPopupRequestedSignal(PopupType popupType) => SignalProcessor.SendSignal(new PopupRequestedSignal(popupType));
    }
}
