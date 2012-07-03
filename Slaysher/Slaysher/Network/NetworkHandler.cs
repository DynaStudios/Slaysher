using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Slaysher.Network
{
    public class NetworkHandler
    {
        private TcpClient _socket;

        private byte[] _recvBuffer;
        private SocketAsyncEventArgs _sendSocketEvent;
        private SocketAsyncEventArgs _recvSocketEvent;

        private bool _sendSystemDisposed;
        private bool _recvSystemDisposed;

        public NetworkHandler()
        {
            _socket = new TcpClient();
        }

        internal void ConnectToServer(string ipAddress, int port)
        {
            _socket.Connect(ipAddress, port);
        }
    }
}