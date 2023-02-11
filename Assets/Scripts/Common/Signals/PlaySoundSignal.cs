using Common.Enums;
using Shared;
using UnityEngine;

namespace Common.Signals
{
    public sealed class PlaySoundSignal : AbstractSignal
    {
        public Vector3 Position;
        public Sound SoundType;

        public PlaySoundSignal(Vector3 position, Sound type)
        {
            Position = position;
            SoundType = type;
        }

        public PlaySoundSignal() : this(default, default) { }
    }
}