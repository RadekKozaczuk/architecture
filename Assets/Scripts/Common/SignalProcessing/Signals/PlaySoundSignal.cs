using Common.Enums;
using UnityEngine;

namespace Common.SignalProcessing.Signals
{
    public sealed class PlaySoundSignal : AbstractSignal
    {
        public readonly Vector3 Position;
        public readonly Sound SoundType;

        public PlaySoundSignal(Vector3 position, Sound type)
        {
            Position = position;
            SoundType = type;
        }
    }
}