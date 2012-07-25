using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SlaysherNetworking.Packets.Utils
{
    public class PacketWriter
    {
        private static readonly ConcurrentStack<PacketWriter> Pool = new ConcurrentStack<PacketWriter>();

        private int _capacity;

        public int Capacity
        {
            get { return _capacity; }
            set { _capacity = value; }
        }

        private MemoryStream _stream;

        public MemoryStream UnderlyingStream
        {
            get { return _stream; }
            set { _stream = value; }
        }

        private Queue<byte[]> _strings;

        public Queue<byte[]> Strings
        {
            get { return _strings; }
            set { _strings = value; }
        }

        public PacketWriter(int capacity)
        {
            _stream = new MemoryStream(capacity);
            _capacity = capacity;
        }

        public static PacketWriter CreateInstance(int capacity, Queue<byte[]> strings)
        {
            PacketWriter pw = CreateInstance(capacity);
            pw.Strings = strings;
            return pw;
        }

        public static PacketWriter CreateInstance()
        {
            return CreateInstance(32);
        }

        public static PacketWriter CreateInstance(int capacity)
        {
            PacketWriter pw = null;

            if (Pool.Count > 0)
            {
                Pool.TryPop(out pw);

                if (pw != null)
                {
                    pw._capacity = capacity;
                    pw._stream.SetLength(0);
                    pw._stream.Position = 0;
                }
            }

            return pw ?? (new PacketWriter(capacity));
        }

        public static void ReleaseInstance(PacketWriter pw)
        {
            if (pw == null) throw new ArgumentNullException("pw");
            if (Pool.TryPop(out pw)) return;
            try
            {
                using (StreamWriter op = new StreamWriter("neterr.log", true))
                {
                    op.WriteLine("{0}\tInstance pool contains writer", DateTime.Now);
                    op.WriteLine();
                }
            }
            catch
            {
                Console.WriteLine("net error");
            }
        }

        public void Write(byte data)
        {
            _stream.WriteByte(data);
        }

        public void WriteByte(byte data)
        {
            _stream.WriteByte(data);
        }

        public void Write(sbyte data)
        {
            Write(unchecked((byte) data));
        }

        public void Write(short data)
        {
            Write(unchecked((byte) (data >> 8)));
            Write(unchecked((byte) data));
        }

        public void Write(ushort data)
        {
            Write(unchecked((byte) (data >> 8)));
            Write(unchecked((byte) data));
        }

        public void Write(int data)
        {
            Write(unchecked((byte) (data >> 24)));
            Write(unchecked((byte) (data >> 16)));
            Write(unchecked((byte) (data >> 8)));
            Write(unchecked((byte) data));
        }

        public void Write(long data)
        {
            Write(unchecked((byte) (data >> 56)));
            Write(unchecked((byte) (data >> 48)));
            Write(unchecked((byte) (data >> 40)));
            Write(unchecked((byte) (data >> 32)));
            Write(unchecked((byte) (data >> 24)));
            Write(unchecked((byte) (data >> 16)));
            Write(unchecked((byte) (data >> 8)));
            Write(unchecked((byte) data));
        }

        public unsafe void Write(float data)
        {
            Write(*(int*) &data);
        }

        public unsafe void Write(double data)
        {
            Write(*(long*) &data);
        }

        public void Write(string data)
        {
            byte[] b;
            int length = data.Length;
            if (_strings != null && _strings.Count > 0)
            {
                b = _strings.Dequeue();
                length = b.Length/2;
            }
            else
                b = Encoding.BigEndianUnicode.GetBytes(data);

            Write((short) length);
            Write(b, 0, b.Length);
        }

        public void Write8(string data)
        {
            byte[] b = Encoding.UTF8.GetBytes(data);
            Write((short) b.Length);
            Write(b, 0, b.Length);
        }

        public void Write(bool data)
        {
            Write((byte) (data ? 1 : 0));
        }

        public void WritePacket(Packet packet)
        {
            Write((byte) packet.GetPacketType());
            //packet.WriteFlush(this);
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            _stream.Write(buffer, offset, count);
        }

        public void WriteDoublePacked(double d)
        {
            Write((int) (d*32.0));
        }
    }
}