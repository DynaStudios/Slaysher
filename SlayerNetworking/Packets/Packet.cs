using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;

using SlaysherNetworking.Packets.Utils;

namespace SlaysherNetworking.Packets
{
    public abstract class Packet
    {
        public abstract void Read(PacketReader reader);

        public abstract void Write();

        protected PacketWriter Writer;

        private int _length;

        public bool Shared { get; private set; }

        public int Written;

        private byte[] _buffer;

        private int _sharesNum;

        protected virtual int Length
        {
            get { return _length; }
            set { _length = value; }
        }

        public PacketType GetPacketType()
        {
            return PacketMap.GetPacketType(GetType());
        }

        protected Packet()
        {
        }

        public void SetCapacity()
        {
            Writer = PacketWriter.CreateInstance(Length);
            Writer.Write((byte)GetPacketType());
        }

        public void SetCapacity(int fixedLength)
        {
            _length = fixedLength;
            SetCapacity();
        }

        public void SetCapacity(int fixedLength, params string[] args)
        {
            byte[] bytes;

            _length = fixedLength;
            Queue<byte[]> strings = new Queue<byte[]>();
            for (int i = 0; i < args.Length; ++i)
            {
                bytes = ASCIIEncoding.BigEndianUnicode.GetBytes(args[i]);
                _length += bytes.Length;
                strings.Enqueue(bytes);
            }

            Writer = PacketWriter.CreateInstance(Length, strings);
            Writer.Write((byte)GetPacketType());
        }

        public void SetShared(int num)
        {
            if (num == 0)
            {
                _sharesNum = 1;
            }
            Shared = true;
            _sharesNum = num;
            Write();

            _buffer = new byte[Length];
            byte[] underlyingBuffer = Writer.UnderlyingStream.GetBuffer();
            try
            {
                Buffer.BlockCopy(underlyingBuffer, 0, _buffer, 0, Length);
            }
            catch (Exception e)
            {
                throw new Exception(String.Format("Writer {0}, Request {1} \r\n{2}", underlyingBuffer.Length, Length, e));
            }
        }

        public void Release()
        {
            if (!Shared)
            {
                PacketWriter.ReleaseInstance(Writer);
                _buffer = null;
            }
            else
            {
                int shares = Interlocked.Decrement(ref _sharesNum);

                if (shares == 0)
                {
                    PacketWriter.ReleaseInstance(Writer);
                    _buffer = null;
                }
            }
        }

        public byte[] GetBuffer()
        {
            if (!Shared)
            {
                _buffer = new byte[Length];
                byte[] underlyingBuffer = Writer.UnderlyingStream.GetBuffer();
                try
                {
                    Buffer.BlockCopy(underlyingBuffer, 0, _buffer, 0, Length);
                }
                catch (Exception e)
                {
                    throw new Exception(String.Format("Writer {0}, Request {1} \r\n{2}", underlyingBuffer.Length, Length, e));
                }
            }

            return _buffer;
        }

#if DEBUG
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            Type t = this.GetType();
            sb.AppendFormat("Packet: {0}\n", t.Name);
            PropertyInfo[] pis = t.GetProperties();
            for (int i = 0; i < pis.Length; i++)
            {
                try
                {
                    PropertyInfo pi = (PropertyInfo)pis.GetValue(i);
                    sb.AppendFormat("{0}: {1}" + Environment.NewLine, pi.Name, pi.GetValue(this, new object[] { }));
                }
                catch (Exception)
                {}
            }

            return sb.ToString();
        }
#else
        public override string ToString()
        {
            return base.ToString();
        }
#endif
    }
}