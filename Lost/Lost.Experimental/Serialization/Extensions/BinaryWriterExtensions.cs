//-----------------------------------------------------------------------
// <copyright file="BinaryWriterExtensions.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.Serialization
{
    using System.Collections.Generic;
    using System.IO;

    //// TODO have tons of methods that could go away if unity properly supported default parameters.  Once
    ////      those are working correctly, should update this file accordinly.

    public static class BinaryWriterExtensions
    {
        #region writing primitive lists

        public static void WriteList(this BinaryWriter writer, List<bool> list)
        {
            writer.Write(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                writer.Write(list[i]);
            }
        }

        public static void WriteList(this BinaryWriter writer, List<char> list)
        {
            writer.Write(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                writer.Write(list[i]);
            }
        }

        public static void WriteList(this BinaryWriter writer, List<string> list)
        {
            writer.Write(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                writer.Write(list[i]);
            }
        }

        public static void WriteList(this BinaryWriter writer, List<float> list)
        {
            writer.Write(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                writer.Write(list[i]);
            }
        }

        public static void WriteList(this BinaryWriter writer, List<double> list)
        {
            writer.Write(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                writer.Write(list[i]);
            }
        }

        public static void WriteList(this BinaryWriter writer, List<decimal> list)
        {
            writer.Write(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                writer.Write(list[i]);
            }
        }

        public static void WriteList(this BinaryWriter writer, List<byte> list)
        {
            writer.Write(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                writer.Write(list[i]);
            }
        }

        public static void WriteList(this BinaryWriter writer, List<sbyte> list)
        {
            writer.Write(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                writer.Write(list[i]);
            }
        }

        public static void WriteList(this BinaryWriter writer, List<short> list)
        {
            writer.Write(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                writer.Write(list[i]);
            }
        }

        public static void WriteList(this BinaryWriter writer, List<ushort> list)
        {
            writer.Write(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                writer.Write(list[i]);
            }
        }

        public static void WriteList(this BinaryWriter writer, List<int> list)
        {
            writer.Write(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                writer.Write(list[i]);
            }
        }

        public static void WriteList(this BinaryWriter writer, List<uint> list)
        {
            writer.Write(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                writer.Write(list[i]);
            }
        }

        public static void WriteList(this BinaryWriter writer, List<long> list)
        {
            writer.Write(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                writer.Write(list[i]);
            }
        }

        public static void WriteList(this BinaryWriter writer, List<ulong> list)
        {
            writer.Write(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                writer.Write(list[i]);
            }
        }

        #endregion

        #region writing primitive list members

        public static void WriteMember(this BinaryWriter writer, byte memberId, List<bool> list)
        {
            if (list != null)
            {
                writer.Write(memberId);
                writer.WriteList(list);
            }
        }

        public static void WriteMember(this BinaryWriter writer, byte memberId, List<char> list)
        {
            if (list != null)
            {
                writer.Write(memberId);
                writer.WriteList(list);
            }
        }

        public static void WriteMember(this BinaryWriter writer, byte memberId, List<string> list)
        {
            if (list != null)
            {
                writer.Write(memberId);
                writer.WriteList(list);
            }
        }

        public static void WriteMember(this BinaryWriter writer, byte memberId, List<float> list)
        {
            if (list != null)
            {
                writer.Write(memberId);
                writer.WriteList(list);
            }
        }

        public static void WriteMember(this BinaryWriter writer, byte memberId, List<double> list)
        {
            if (list != null)
            {
                writer.Write(memberId);
                writer.WriteList(list);
            }
        }

        public static void WriteMember(this BinaryWriter writer, byte memberId, List<decimal> list)
        {
            if (list != null)
            {
                writer.Write(memberId);
                writer.WriteList(list);
            }
        }

        public static void WriteMember(this BinaryWriter writer, byte memberId, List<byte> list)
        {
            if (list != null)
            {
                writer.Write(memberId);
                writer.WriteList(list);
            }
        }

        public static void WriteMember(this BinaryWriter writer, byte memberId, List<sbyte> list)
        {
            if (list != null)
            {
                writer.Write(memberId);
                writer.WriteList(list);
            }
        }

        public static void WriteMember(this BinaryWriter writer, byte memberId, List<short> list)
        {
            if (list != null)
            {
                writer.Write(memberId);
                writer.WriteList(list);
            }
        }

        public static void WriteMember(this BinaryWriter writer, byte memberId, List<ushort> list)
        {
            if (list != null)
            {
                writer.Write(memberId);
                writer.WriteList(list);
            }
        }

        public static void WriteMember(this BinaryWriter writer, byte memberId, List<int> list)
        {
            if (list != null)
            {
                writer.Write(memberId);
                writer.WriteList(list);
            }
        }

        public static void WriteMember(this BinaryWriter writer, byte memberId, List<uint> list)
        {
            if (list != null)
            {
                writer.Write(memberId);
                writer.WriteList(list);
            }
        }

        public static void WriteMember(this BinaryWriter writer, byte memberId, List<long> list)
        {
            if (list != null)
            {
                writer.Write(memberId);
                writer.WriteList(list);
            }
        }

        public static void WriteMember(this BinaryWriter writer, byte memberId, List<ulong> list)
        {
            if (list != null)
            {
                writer.Write(memberId);
                writer.WriteList(list);
            }
        }

        #endregion

        #region writing primitive members

        public static void WriteMember(this BinaryWriter writer, byte memberId, bool value, bool defaultValue)
        {
            if (value != defaultValue)
            {
                writer.Write(memberId);
                writer.Write(value);
            }
        }

        public static void WriteMember(this BinaryWriter writer, byte memberId, bool value)
        {
            WriteMember(writer, memberId, value, default(bool));
        }

        public static void WriteMember(this BinaryWriter writer, byte memberId, char value, char defaultValue)
        {
            if (value != defaultValue)
            {
                writer.Write(memberId);
                writer.Write(value);
            }
        }

        public static void WriteMember(this BinaryWriter writer, byte memberId, char value)
        {
            WriteMember(writer, memberId, value, default(char));
        }

        public static void WriteMember(this BinaryWriter writer, byte memberId, string value, string defaultValue)
        {
            if (value != defaultValue)
            {
                writer.Write(memberId);
                writer.Write(value);
            }
        }

        public static void WriteMember(this BinaryWriter writer, byte memberId, string value)
        {
            WriteMember(writer, memberId, value, default(string));
        }

        public static void WriteMember(this BinaryWriter writer, byte memberId, float value, float defaultValue)
        {
            if (value != defaultValue)
            {
                writer.Write(memberId);
                writer.Write(value);
            }
        }

        public static void WriteMember(this BinaryWriter writer, byte memberId, float value)
        {
            WriteMember(writer, memberId, value, default(float));
        }

        public static void WriteMember(this BinaryWriter writer, byte memberId, double value, double defaultValue)
        {
            if (value != defaultValue)
            {
                writer.Write(memberId);
                writer.Write(value);
            }
        }

        public static void WriteMember(this BinaryWriter writer, byte memberId, double value)
        {
            WriteMember(writer, memberId, value, default(double));
        }

        public static void WriteMember(this BinaryWriter writer, byte memberId, decimal value, decimal defaultValue)
        {
            if (value != defaultValue)
            {
                writer.Write(memberId);
                writer.Write(value);
            }
        }

        public static void WriteMember(this BinaryWriter writer, byte memberId, decimal value)
        {
            WriteMember(writer, memberId, value, default(decimal));
        }

        public static void WriteMember(this BinaryWriter writer, byte memberId, byte value, byte defaultValue)
        {
            if (value != defaultValue)
            {
                writer.Write(memberId);
                writer.Write(value);
            }
        }

        public static void WriteMember(this BinaryWriter writer, byte memberId, byte value)
        {
            WriteMember(writer, memberId, value, default(byte));
        }

        public static void WriteMember(this BinaryWriter writer, byte memberId, sbyte value, sbyte defaultValue)
        {
            if (value != defaultValue)
            {
                writer.Write(memberId);
                writer.Write(value);
            }
        }

        public static void WriteMember(this BinaryWriter writer, byte memberId, sbyte value)
        {
            WriteMember(writer, memberId, value, default(sbyte));
        }

        public static void WriteMember(this BinaryWriter writer, byte memberId, short value, short defaultValue)
        {
            if (value != defaultValue)
            {
                writer.Write(memberId);
                writer.Write(value);
            }
        }

        public static void WriteMember(this BinaryWriter writer, byte memberId, short value)
        {
            WriteMember(writer, memberId, value, default(short));
        }

        public static void WriteMember(this BinaryWriter writer, byte memberId, ushort value, ushort defaultValue)
        {
            if (value != defaultValue)
            {
                writer.Write(memberId);
                writer.Write(value);
            }
        }

        public static void WriteMember(this BinaryWriter writer, byte memberId, ushort value)
        {
            WriteMember(writer, memberId, value, default(ushort));
        }

        public static void WriteMember(this BinaryWriter writer, byte memberId, int value, int defaultValue)
        {
            if (value != defaultValue)
            {
                writer.Write(memberId);
                writer.Write(value);
            }
        }

        public static void WriteMember(this BinaryWriter writer, byte memberId, int value)
        {
            WriteMember(writer, memberId, value, default(int));
        }

        public static void WriteMember(this BinaryWriter writer, byte memberId, uint value, uint defaultValue)
        {
            if (value != defaultValue)
            {
                writer.Write(memberId);
                writer.Write(value);
            }
        }

        public static void WriteMember(this BinaryWriter writer, byte memberId, uint value)
        {
            WriteMember(writer, memberId, value, default(uint));
        }

        public static void WriteMember(this BinaryWriter writer, byte memberId, long value, long defaultValue)
        {
            if (value != defaultValue)
            {
                writer.Write(memberId);
                writer.Write(value);
            }
        }

        public static void WriteMember(this BinaryWriter writer, byte memberId, long value)
        {
            WriteMember(writer, memberId, value, default(long));
        }

        public static void WriteMember(this BinaryWriter writer, byte memberId, ulong value, ulong defaultValue)
        {
            if (value != defaultValue)
            {
                writer.Write(memberId);
                writer.Write(value);
            }
        }

        public static void WriteMember(this BinaryWriter writer, byte memberId, ulong value)
        {
            WriteMember(writer, memberId, value, default(ulong));
        }

        #endregion

        #region writing IBinaryBlog

        public static void WriteMember(this BinaryWriter writer, byte memberId, IBinaryBlob blob)
        {
            if (blob != null)
            {
                writer.Write(memberId);
                writer.Write(blob);
            }
        }

        public static void Write(this BinaryWriter writer, IBinaryBlob blob)
        {
            blob.Serialize(writer);
        }

        #endregion
    }
}
