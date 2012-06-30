using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SlaysherNetworking.Packets.Utils;

namespace SlaysherNetworking.Packets
{
    public abstract class Packet
    {
        public const string ServerCrLf = "\r\n";

        public abstract void Read(byte[] reader);

        public abstract void Write();

        protected PacketWriter Writer;

        public MemoryStream Stream
        {
            get { return Writer.UnderlyingStream; }
        }

        protected Packet()
        {
            Writer = new PacketWriter();
        }
    }
}