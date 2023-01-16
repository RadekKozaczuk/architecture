using System.Collections.Generic;
using System.IO;
using Shared;
using UnityEngine;

namespace GameLogic.Systems
{
    static class SaveLoadSystem
    {
        static readonly string _saveFileName = "savegame.sav";
        static readonly string _savePath = Path.Combine(Application.persistentDataPath, _saveFileName);

        static BinaryWriter _writer;
        static BinaryReader _reader;

        internal static void SaveGame()
        {

        }

        internal static void LoadGame()
        {

        }

        static Quaternion ReadQuaternion() => new (_reader.ReadSingle(), _reader.ReadSingle(), _reader.ReadSingle(), _reader.ReadSingle());

        static Vector3[] ReadArrayVector3(int size)
        {
            var array = new Vector3[size];
            for (int i = 0; i < size; i++)
                array[i] = ReadVector3();
            return array;
        }

        static Vector3 ReadVector3() => new (_reader.ReadSingle(), _reader.ReadSingle(), _reader.ReadSingle());

        static Vector3Int ReadVector3Int() => new (_reader.ReadInt32(), _reader.ReadInt32(), _reader.ReadInt32());

        static ReadonlyVector3Int ReadReadonlyVector3Int() => new (_reader.ReadInt32(), _reader.ReadInt32(), _reader.ReadInt32());

        static List<Vector2> ReadListVector2(int size)
        {
            var list = new List<Vector2>(size);
            for (int i = 0; i < size; i++)
                list.Add(ReadVector2());
            return list;
        }

        static Vector2[] ReadArrayVector2(int size)
        {
            var array = new Vector2[size];
            for (int i = 0; i < size; i++)
                array[i] = ReadVector2();
            return array;
        }

        static Vector2 ReadVector2() => new (_reader.ReadSingle(), _reader.ReadSingle());

        static int[] ReadArrayInt32(int size)
        {
            var array = new int[size];
            for (int i = 0; i < size; i++)
                array[i] = _reader.ReadInt32();
            return array;
        }

        static void Write(Quaternion value)
        {
            _writer.Write(value.x);
            _writer.Write(value.y);
            _writer.Write(value.z);
            _writer.Write(value.w);
        }

        static void Write(IReadOnlyList<Vector3> array)
        {
            for (int i = 0; i < array.Count; i++)
                Write(array[i]);
        }

        static void Write(Vector3 value)
        {
            _writer.Write(value.x);
            _writer.Write(value.y);
            _writer.Write(value.z);
        }

        static void Write(Vector3Int value)
        {
            _writer.Write(value.x);
            _writer.Write(value.y);
            _writer.Write(value.z);
        }

        static void Write(ReadonlyVector3Int value)
        {
            _writer.Write(value.X);
            _writer.Write(value.Y);
            _writer.Write(value.Z);
        }

        static void Write(List<Vector2> list)
        {
            foreach (Vector2 v in list)
                Write(v);
        }

        static void Write(IReadOnlyList<Vector2> array)
        {
            for (int i = 0; i < array.Count; i++)
                Write(array[i]);
        }

        static void Write(Vector2 value)
        {
            _writer.Write(value.x);
            _writer.Write(value.y);
        }

        static void Write(IReadOnlyList<int> array)
        {
            for (int i = 0; i < array.Count; i++)
                _writer.Write(array[i]);
        }
    }
}