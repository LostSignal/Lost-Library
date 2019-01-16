//-----------------------------------------------------------------------
// <copyright file="BinaryReaderExtensions.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using System.IO;

    public static class BinaryReaderExtensions
    {
        public static List<bool> ReadBoolList(this BinaryReader reader)
        {
            int count = reader.ReadInt32();
            var list = new List<bool>(count);

            for (int i = 0; i < count; i++)
            {
                list.Add(reader.ReadBoolean());
            }

            return list;
        }

        public static List<char> ReadCharList(this BinaryReader reader)
        {
            int count = reader.ReadInt32();
            return new List<char>(reader.ReadChars(count));
        }

        public static List<string> ReadStringList(this BinaryReader reader)
        {
            int count = reader.ReadInt32();
            var list = new List<string>(count);

            for (int i = 0; i < count; i++)
            {
                list.Add(reader.ReadString());
            }

            return list;
        }

        public static List<float> ReadFloatList(this BinaryReader reader)
        {
            int count = reader.ReadInt32();
            var list = new List<float>(count);

            for (int i = 0; i < count; i++)
            {
                list.Add(reader.ReadSingle());
            }

            return list;
        }

        public static List<double> ReadDoubleList(this BinaryReader reader)
        {
            int count = reader.ReadInt32();
            var list = new List<double>(count);

            for (int i = 0; i < count; i++)
            {
                list.Add(reader.ReadDouble());
            }

            return list;
        }

        public static List<decimal> ReadDecimalList(this BinaryReader reader)
        {
            int count = reader.ReadInt32();
            var list = new List<decimal>(count);

            for (int i = 0; i < count; i++)
            {
                list.Add(reader.ReadDecimal());
            }

            return list;
        }

        public static List<byte> ReadByteList(this BinaryReader reader)
        {
            int count = reader.ReadInt32();
            return new List<byte>(reader.ReadBytes(count));
        }

        public static List<sbyte> ReadSByteList(this BinaryReader reader)
        {
            int count = reader.ReadInt32();
            var list = new List<sbyte>(count);

            for (int i = 0; i < count; i++)
            {
                list.Add(reader.ReadSByte());
            }

            return list;
        }

        public static List<short> ReadShortList(this BinaryReader reader)
        {
            int count = reader.ReadInt32();
            var list = new List<short>(count);

            for (int i = 0; i < count; i++)
            {
                list.Add(reader.ReadInt16());
            }

            return list;
        }

        public static List<ushort> ReadUShortList(this BinaryReader reader)
        {
            int count = reader.ReadInt32();
            var list = new List<ushort>(count);

            for (int i = 0; i < count; i++)
            {
                list.Add(reader.ReadUInt16());
            }

            return list;
        }

        public static List<int> ReadIntList(this BinaryReader reader)
        {
            int count = reader.ReadInt32();
            var list = new List<int>(count);

            for (int i = 0; i < count; i++)
            {
                list.Add(reader.ReadInt32());
            }

            return list;
        }

        public static List<uint> ReadUIntList(this BinaryReader reader)
        {
            int count = reader.ReadInt32();
            var list = new List<uint>(count);

            for (int i = 0; i < count; i++)
            {
                list.Add(reader.ReadUInt32());
            }

            return list;
        }

        public static List<long> ReadLongList(this BinaryReader reader)
        {
            int count = reader.ReadInt32();
            var list = new List<long>(count);

            for (int i = 0; i < count; i++)
            {
                list.Add(reader.ReadInt64());
            }

            return list;
        }

        public static List<ulong> ReadULongList(this BinaryReader reader)
        {
            int count = reader.ReadInt32();
            var list = new List<ulong>(count);

            for (int i = 0; i < count; i++)
            {
                list.Add(reader.ReadUInt64());
            }

            return list;
        }

        public static T Read<T>(this BinaryReader reader) where T : IBinaryBlob, new()
        {
            T t = new T();
            t.Deserialize(reader);
            return t;
        }
    }
}
