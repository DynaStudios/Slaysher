﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SlaysherNetworking.Packets.Utils
{
    public class PacketReader
    {
        private byte[] _Data;
        private int _Size;
        private int _Index;
        private bool _Failed;

        public int Index
        {
            get
            {
                return _Index;
            }
        }

        public int Size
        {
            get
            {
                return _Size;
            }
        }

        public bool Failed
        {
            get { return _Failed; }
            set { _Failed = value; }
        }

        public PacketReader(byte[] data, int size)
        {
            _Data = data;
            _Size = size;
            _Index = 1;
            _Failed = false;
        }

        public bool CheckBoundaries(int size)
        {
            if ((_Index + size) > _Size)
                _Failed = true;

            return !_Failed;
        }

        public byte ReadByte()
        {
            if (!CheckBoundaries(1))
                return 0;

            int b = _Data[_Index++];

            return (byte)b;
        }

        public byte[] ReadBytes(int Count)
        {
            if (!CheckBoundaries(Count))
                return null;

            byte[] input = new byte[Count];

            for (int i = Count - 1; i >= 0; i--)
            {
                input[i] = ReadByte();
            }

            return input;
        }

        public sbyte ReadSByte()
        {
            return unchecked((sbyte)ReadByte());
        }

        public short ReadShort()
        {
            if (!CheckBoundaries(2))
                return 0;
            return unchecked((short)((ReadByte() << 8) | ReadByte()));
        }

        public int ReadInt()
        {
            if (!CheckBoundaries(4))
                return 0;
            return unchecked((ReadByte() << 24) | (ReadByte() << 16) | (ReadByte() << 8) | ReadByte());
        }

        public long ReadLong()
        {
            if (!CheckBoundaries(8))
                return 0;
            return unchecked((ReadByte() << 56) | (ReadByte() << 48) | (ReadByte() << 40) | (ReadByte() << 32)
                | (ReadByte() << 24) | (ReadByte() << 16) | (ReadByte() << 8) | ReadByte());
        }

        public unsafe float ReadFloat()
        {
            if (!CheckBoundaries(4))
                return 0;
            int i = ReadInt();
            return *(float*)&i;
        }

        public unsafe double ReadDouble()
        {
            if (!CheckBoundaries(8))
                return 0;
            byte[] r = new byte[8];
            for (int i = 7; i >= 0; i--)
            {
                r[i] = ReadByte();
            }
            return BitConverter.ToDouble(r, 0);
        }

        public string ReadString16(short maxLen)
        {
            int len = ReadShort();
            if (len > maxLen)
                throw new IOException("String field too long");

            if (!CheckBoundaries(len * 2))
                return "";

            byte[] b = new byte[len * 2];
            for (int i = 0; i < len * 2; i++)
                b[i] = ReadByte();
            return ASCIIEncoding.BigEndianUnicode.GetString(b);
        }

        public string ReadString8(short maxLen)
        {
            int len = ReadShort();
            if (len > maxLen)
                throw new IOException("String field too long");

            if (!CheckBoundaries(len))
                return "";

            byte[] b = new byte[len];
            for (int i = 0; i < len; i++)
                b[i] = (byte)ReadByte();
            return ASCIIEncoding.UTF8.GetString(b);
        }

        public bool ReadBool()
        {
            return ReadByte() == 1;
        }

        public double ReadDoublePacked()
        {
            return (double)ReadInt() / 32.0;
        }
    }
}