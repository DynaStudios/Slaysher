using System;
using System.IO;
using System.Text;

namespace SlaysherNetworking.Packets.Utils
{
    public class PacketReader
    {
        private readonly byte[] _data;
        private readonly int _size;
        private int _index;
        private bool _failed;

        public int Index
        {
            get
            {
                return _index;
            }
        }

        public int Size
        {
            get
            {
                return _size;
            }
        }

        public bool Failed
        {
            get { return _failed; }
            set { _failed = value; }
        }

        public PacketReader(byte[] data, int size)
        {
            _data = data;
            _size = size;
            _index = 1;
            _failed = false;
        }

        public bool CheckBoundaries(int size)
        {
            if ((_index + size) > _size)
                _failed = true;

            return !_failed;
        }

        public byte ReadByte()
        {
            if (!CheckBoundaries(1))
                return 0;

            int b = _data[_index++];

            return (byte)b;
        }

        public byte[] ReadBytes(int count)
        {
            if (!CheckBoundaries(count))
                return null;

            byte[] input = new byte[count];

            for (int i = count - 1; i >= 0; i--)
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

        public double ReadDouble()
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
            return Encoding.BigEndianUnicode.GetString(b);
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
                b[i] = ReadByte();
            return Encoding.UTF8.GetString(b);
        }

        public bool ReadBool()
        {
            return ReadByte() == 1;
        }

        public double ReadDoublePacked()
        {
            return ReadInt() / 32.0;
        }
    }
}