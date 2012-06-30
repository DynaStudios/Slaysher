using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        private int _Length;

        public bool Shared { get; private set; }

        public int Written;

        private byte[] _buffer;

        private int _sharesNum;

        protected virtual int Length { get { return _Length; } set { _Length = value; } }

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
            _Length = fixedLength;
            SetCapacity();
        }

        public void SetCapacity(int fixedLength, params string[] args)
        {
            byte[] bytes;

            _Length = fixedLength;
            Queue<byte[]> strings = new Queue<byte[]>();
            for (int i = 0; i < args.Length; ++i)
            {
                bytes = ASCIIEncoding.BigEndianUnicode.GetBytes(args[i]);
                _Length += bytes.Length;
                strings.Enqueue(bytes);
            }

            Writer = PacketWriter.CreateInstance(Length, strings);
            Writer.Write((byte)GetPacketType());
        }

        //Add Release Methods

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
    }
}