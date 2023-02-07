using System.IO;
using Common;
using Presentation.ViewModels;
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
            const byte Level = 0;
            BinaryWriter writer = new(File.Open(_savePath, FileMode.Create));

            // player
            writer.Write(Version);
            writer.Write(Level);

            PresentationViewModel.SaveGame(writer);

            writer.Close();
        }

        internal static void LoadGame()
        {
            byte[] data = File.ReadAllBytes(_savePath);
            BinaryReader reader = new(new MemoryStream(data));

            int _ = reader.ReadByte(); // save game version
            CommonData.CurrentLevel = reader.ReadByte();

            PresentationViewModel.LoadGame(reader);
        }
    }
}