#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Common.Config;
using UnityEngine;

namespace GameLogic.Systems
{
    static class PersistentStorageSystem
    {
        static readonly AudioMixerConfig _config;

        internal static (int music, int sound) LoadVolumeSettings() =>
            (PlayerPrefs.GetInt(PersistentStorageKey.Music.ToString()),
             PlayerPrefs.GetInt(PersistentStorageKey.Sound.ToString()));

        internal static void SaveVolumeSettings(int music, int sound)
        {
            PlayerPrefs.SetInt(PersistentStorageKey.Music.ToString(), music);
            PlayerPrefs.SetInt(PersistentStorageKey.Sound.ToString(), sound);
        }
    }
}