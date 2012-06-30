using System;
using System.Net.Sockets;

namespace SlaysherServer.Network.Events
{
    public class TcpEventArgs : EventArgs
    {
        public Socket TcpSocket { get; private set; }

        public bool Cancelled { get; set; }

        internal TcpEventArgs(Socket socket)
        {
            TcpSocket = socket;
        }
    }
}