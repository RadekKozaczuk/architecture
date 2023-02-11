using Common.Enums;
using Common.Signals;
using Shared.Systems;
using UnityEngine;

namespace Common.Factories
{
    public static class SignalFactory
    {
        public static void SendInventoryChangedSignal()
            => SignalProcessor.SendSignal(SignalProcessor.GetSignal<InventoryChangedSignal>());

        public static void SendMissionCompleteSignal()
            => SignalProcessor.SendSignal(SignalProcessor.GetSignal<MissionCompleteSignal>());

        public static void SendMissionFailedSignal()
            => SignalProcessor.SendSignal(SignalProcessor.GetSignal<MissionFailedSignal>());

        public static void SendPlaySoundSignal(Vector3 position, Sound soundType)
        {
            var signal = SignalProcessor.GetSignal<PlaySoundSignal>();
            signal.Position = position;
            signal.SoundType = soundType;
            SignalProcessor.SendSignal(signal);
        }

        public static void SendPopupRequestedSignal(PopupType popupType)
        {
            var signal = SignalProcessor.GetSignal<PopupRequestedSignal>();
            signal.PopupType = popupType;
            SignalProcessor.SendSignal(signal);
        }
    }
}
