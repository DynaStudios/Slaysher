using System.IO;
using System.Text;

namespace SlaysherNetworking.Packets.Utils
{
    public class PacketWriter
    {
        private MemoryStream _stream;

        public MemoryStream UnderlyingStream
        {
            get { return _stream; }
            set { _stream = value; }
        }

        public PacketWriter()
        {
            _stream = new MemoryStream();
        }

        public void Write(string text)
        {
            byte[] data = Encoding.UTF8.GetBytes(text);
            _stream.Write(data, 0, data.Length);
        }

        //Add Write Routines for int, long, byte, etc.
    }
}