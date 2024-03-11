#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.IO;
using Common;
using Presentation.ViewModels;
using Shared;
using UnityEngine;

namespace GameLogic.Systems
{
    static class SaveLoadSystem
    {
        internal static bool SaveFileExist => File.Exists(_savePath);

        const string SaveFileName = "savegame.sav";
        static readonly string _savePath = Path.Combine(Application.persistentDataPath, SaveFileName);

        internal static void SaveGame()
        {
            // save version
            const byte Version = 0;
            BinaryWriter writer = new(File.Open(_savePath, FileMode.Create));

            // player
            writer.Write(Version);
            writer.Write((byte)CommonData.CurrentLevel);

            PresentationViewModel.SaveGame(writer);

            writer.Close();
        }

        internal static void LoadGame() { }

        [React]
        static void OnInventoryChangedSignal()
        {
            Debug.Log("SIGNAL RECEIVED SaveLoadSystem OnInventoryChangedSignal");
        }
    }
}