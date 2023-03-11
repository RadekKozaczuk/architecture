using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Shared.Systems
{
    public static class SaveLoadUtils
    {
        public static Quaternion ReadQuaternion(BinaryReader reader) =>
            new (reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

        public static Vector3[] ReadArrayVector3(BinaryReader reader, int size)
        {
            var array = new Vector3[size];
            for (int i = 0; i < size; i++)
                array[i] = ReadVector3(reader);
            return array;
        }

        public static Vector3 ReadVector3(BinaryReader reader) => new (reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

        public static Vector3Int ReadVector3Int(BinaryReader reader) => new (reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());

        public static ReadonlyVector3Int ReadReadonlyVector3Int(BinaryReader reader) => new (reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());

        public static List<Vector2> ReadListVector2(BinaryReader reader, int size)
        {
            var list = new List<Vector2>(size);
            for (int i = 0; i < size; i++)
                list.Add(ReadVector2(reader));
            return list;
        }

        public static Vector2[] ReadArrayVector2(BinaryReader reader, int size)
        {
            var array = new Vector2[size];
            for (int i = 0; i < size; i++)
                array[i] = ReadVector2(reader);
            return array;
        }

        public static Vector2 ReadVector2(BinaryReader reader) => new (reader.ReadSingle(), reader.ReadSingle());

        public static int[] ReadArrayInt32(BinaryReader reader, int size)
        {
            int[] array = new int[size];
            for (int i = 0; i < size; i++)
                array[i] = reader.ReadInt32();
            return array;
        }

        public static void Write(BinaryWriter writer, Quaternion value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.z);
            writer.Write(value.w);
        }

        public static void Write(BinaryWriter writer, IReadOnlyList<Vector3> array)
        {
            for (int i = 0; i < array.Count; i++)
                Write(writer, array[i]);
        }

        public static void Write(BinaryWriter writer, Vector3 value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.z);
        }

        public static void Write(BinaryWriter writer, Vector3Int value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.z);
        }

        public static void Write(BinaryWriter writer, ReadonlyVector3Int value)
        {
            writer.Write(value.X);
            writer.Write(value.Y);
            writer.Write(value.Z);
        }

        public static void Write(BinaryWriter writer, List<Vector2> list)
        {
            foreach (Vector2 v in list)
                Write(writer, v);
        }

        public static void Write(BinaryWriter writer, IReadOnlyList<Vector2> array)
        {
            for (int i = 0; i < array.Count; i++)
                Write(writer, array[i]);
        }

        public static void Write(BinaryWriter writer, Vector2 value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
        }

        public static void Write(BinaryWriter writer, IReadOnlyList<int> array)
        {
            for (int i = 0; i < array.Count; i++)
                writer.Write(array[i]);
        }
    }
}