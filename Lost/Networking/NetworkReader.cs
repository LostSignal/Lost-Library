//-----------------------------------------------------------------------
// <copyright file="NetworkReader.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.Networking
{
    using System;
    using System.Text;
    using UnityEngine;

    public class NetworkReader
    {
        NetBuffer m_buf;

        const int k_MaxStringLength = 1024 * 32;
        const int k_InitialStringBufferSize = 1024;
        static byte[] s_StringReaderBuffer;
        static Encoding s_Encoding;

        public NetworkReader()
        {
            m_buf = new NetBuffer();
            Initialize();
        }

        public NetworkReader(NetworkWriter writer)
        {
            m_buf = new NetBuffer(writer.AsArray());
            Initialize();
        }

        public NetworkReader(byte[] buffer)
        {
            m_buf = new NetBuffer(buffer);
            Initialize();
        }

        public NetworkReader(NetBuffer netBuffer)
        {
            m_buf = netBuffer;
            Initialize();
        }

        static void Initialize()
        {
            if (s_Encoding == null)
            {
                s_StringReaderBuffer = new byte[k_InitialStringBufferSize];
                s_Encoding = new UTF8Encoding();
            }
        }

        public byte[] RawBuffer { get { return m_buf.RawBuffer; } }
        public uint Position { get { return m_buf.Position; } }
        public int Length { get { return m_buf.Length; } }

        public void SeekZero()
        {
            m_buf.SeekZero();
        }

        internal void Replace(byte[] buffer)
        {
            m_buf.Replace(buffer);
        }

        // http://sqlite.org/src4/doc/trunk/www/varint.wiki
        // NOTE: big endian.

        public UInt32 ReadPackedUInt32()
        {
            byte a0 = ReadByte();
            if (a0 < 241)
            {
                return a0;
            }
            byte a1 = ReadByte();
            if (a0 >= 241 && a0 <= 248)
            {
                return (UInt32)(240 + 256 * (a0 - 241) + a1);
            }
            byte a2 = ReadByte();
            if (a0 == 249)
            {
                return (UInt32)(2288 + 256 * a1 + a2);
            }
            byte a3 = ReadByte();
            if (a0 == 250)
            {
                return a1 + (((UInt32)a2) << 8) + (((UInt32)a3) << 16);
            }
            byte a4 = ReadByte();
            if (a0 >= 251)
            {
                return a1 + (((UInt32)a2) << 8) + (((UInt32)a3) << 16) + (((UInt32)a4) << 24);
            }
            throw new IndexOutOfRangeException("ReadPackedUInt32() failure: " + a0);
        }

        public UInt64 ReadPackedUInt64()
        {
            byte a0 = ReadByte();
            if (a0 < 241)
            {
                return a0;
            }
            byte a1 = ReadByte();
            if (a0 >= 241 && a0 <= 248)
            {
                return 240 + 256 * (a0 - ((UInt64)241)) + a1;
            }
            byte a2 = ReadByte();
            if (a0 == 249)
            {
                return 2288 + (((UInt64)256) * a1) + a2;
            }
            byte a3 = ReadByte();
            if (a0 == 250)
            {
                return a1 + (((UInt64)a2) << 8) + (((UInt64)a3) << 16);
            }
            byte a4 = ReadByte();
            if (a0 == 251)
            {
                return a1 + (((UInt64)a2) << 8) + (((UInt64)a3) << 16) + (((UInt64)a4) << 24);
            }


            byte a5 = ReadByte();
            if (a0 == 252)
            {
                return a1 + (((UInt64)a2) << 8) + (((UInt64)a3) << 16) + (((UInt64)a4) << 24) + (((UInt64)a5) << 32);
            }


            byte a6 = ReadByte();
            if (a0 == 253)
            {
                return a1 + (((UInt64)a2) << 8) + (((UInt64)a3) << 16) + (((UInt64)a4) << 24) + (((UInt64)a5) << 32) + (((UInt64)a6) << 40);
            }


            byte a7 = ReadByte();
            if (a0 == 254)
            {
                return a1 + (((UInt64)a2) << 8) + (((UInt64)a3) << 16) + (((UInt64)a4) << 24) + (((UInt64)a5) << 32) + (((UInt64)a6) << 40) + (((UInt64)a7) << 48);
            }


            byte a8 = ReadByte();
            if (a0 == 255)
            {
                return a1 + (((UInt64)a2) << 8) + (((UInt64)a3) << 16) + (((UInt64)a4) << 24) + (((UInt64)a5) << 32) + (((UInt64)a6) << 40) + (((UInt64)a7) << 48) + (((UInt64)a8) << 56);
            }
            throw new IndexOutOfRangeException("ReadPackedUInt64() failure: " + a0);
        }

        public byte ReadByte()
        {
            return m_buf.ReadByte();
        }

        public sbyte ReadSByte()
        {
            return (sbyte)m_buf.ReadByte();
        }

        public short ReadInt16()
        {
            ushort value = 0;
            value |= m_buf.ReadByte();
            value |= (ushort)(m_buf.ReadByte() << 8);
            return (short)value;
        }

        public ushort ReadUInt16()
        {
            ushort value = 0;
            value |= m_buf.ReadByte();
            value |= (ushort)(m_buf.ReadByte() << 8);
            return value;
        }

        public int ReadInt32()
        {
            uint value = 0;
            value |= m_buf.ReadByte();
            value |= (uint)(m_buf.ReadByte() << 8);
            value |= (uint)(m_buf.ReadByte() << 16);
            value |= (uint)(m_buf.ReadByte() << 24);
            return (int)value;
        }

        public uint ReadUInt32()
        {
            uint value = 0;
            value |= m_buf.ReadByte();
            value |= (uint)(m_buf.ReadByte() << 8);
            value |= (uint)(m_buf.ReadByte() << 16);
            value |= (uint)(m_buf.ReadByte() << 24);
            return value;
        }

        public long ReadInt64()
        {
            ulong value = 0;

            ulong other = m_buf.ReadByte();
            value |= other;

            other = ((ulong)m_buf.ReadByte()) << 8;
            value |= other;

            other = ((ulong)m_buf.ReadByte()) << 16;
            value |= other;

            other = ((ulong)m_buf.ReadByte()) << 24;
            value |= other;

            other = ((ulong)m_buf.ReadByte()) << 32;
            value |= other;

            other = ((ulong)m_buf.ReadByte()) << 40;
            value |= other;

            other = ((ulong)m_buf.ReadByte()) << 48;
            value |= other;

            other = ((ulong)m_buf.ReadByte()) << 56;
            value |= other;

            return (long)value;
        }

        public ulong ReadUInt64()
        {
            ulong value = 0;
            ulong other = m_buf.ReadByte();
            value |= other;

            other = ((ulong)m_buf.ReadByte()) << 8;
            value |= other;

            other = ((ulong)m_buf.ReadByte()) << 16;
            value |= other;

            other = ((ulong)m_buf.ReadByte()) << 24;
            value |= other;

            other = ((ulong)m_buf.ReadByte()) << 32;
            value |= other;

            other = ((ulong)m_buf.ReadByte()) << 40;
            value |= other;

            other = ((ulong)m_buf.ReadByte()) << 48;
            value |= other;

            other = ((ulong)m_buf.ReadByte()) << 56;
            value |= other;
            return value;
        }

        public decimal ReadDecimal()
        {
            Int32[] bits = new Int32[4];

            bits[0] = ReadInt32();
            bits[1] = ReadInt32();
            bits[2] = ReadInt32();
            bits[3] = ReadInt32();

            return new decimal(bits);
        }

        public float ReadSingle()
        {
            uint value = ReadUInt32();
            return FloatConversion.ToSingle(value);
        }

        public double ReadDouble()
        {
            ulong value = ReadUInt64();
            return FloatConversion.ToDouble(value);
        }

        public string ReadString()
        {
            UInt16 numBytes = ReadUInt16();
            if (numBytes == 0)
                return "";

            if (numBytes >= k_MaxStringLength)
            {
                throw new IndexOutOfRangeException("ReadString() too long: " + numBytes);
            }

            while (numBytes > s_StringReaderBuffer.Length)
            {
                s_StringReaderBuffer = new byte[s_StringReaderBuffer.Length * 2];
            }

            m_buf.ReadBytes(s_StringReaderBuffer, numBytes);

            char[] chars = s_Encoding.GetChars(s_StringReaderBuffer, 0, numBytes);
            return new string(chars);
        }

        public char ReadChar()
        {
            return (char)m_buf.ReadByte();
        }

        public bool ReadBoolean()
        {
            int value = m_buf.ReadByte();
            return value == 1;
        }

        public byte[] ReadBytes(int count)
        {
            if (count < 0)
            {
                throw new IndexOutOfRangeException("NetworkReader ReadBytes " + count);
            }
            byte[] value = new byte[count];
            m_buf.ReadBytes(value, (uint)count);
            return value;
        }

        public byte[] ReadBytesAndSize()
        {
            ushort sz = ReadUInt16();
            if (sz == 0)
                return new byte[0];

            return ReadBytes(sz);
        }

        public Vector2 ReadVector2()
        {
            return new Vector2(ReadSingle(), ReadSingle());
        }

        public Vector3 ReadVector3()
        {
            return new Vector3(ReadSingle(), ReadSingle(), ReadSingle());
        }

        public Vector4 ReadVector4()
        {
            return new Vector4(ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle());
        }

        public Color ReadColor()
        {
            return new Color(ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle());
        }

        public Color32 ReadColor32()
        {
            return new Color32(ReadByte(), ReadByte(), ReadByte(), ReadByte());
        }

        public Quaternion ReadQuaternion()
        {
            return new Quaternion(ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle());
        }

        public Rect ReadRect()
        {
            return new Rect(ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle());
        }

        public Plane ReadPlane()
        {
            return new Plane(ReadVector3(), ReadSingle());
        }

        public Ray ReadRay()
        {
            return new Ray(ReadVector3(), ReadVector3());
        }

        public override string ToString()
        {
            return m_buf.ToString();
        }

        public TMsg ReadMessage<TMsg>() where TMsg : Message, new()
        {
            var msg = new TMsg();
            msg.Deserialize(this);
            return msg;
        }
    }
}
