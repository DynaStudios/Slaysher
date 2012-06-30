using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using SlaysherServer.Game.Models;

namespace SlaysherServer
{
    public class Server
    {
        private readonly Socket _listener;
        private readonly SocketAsyncEventArgs _acceptEventArgs;

        //Server Properties
        public const int MaxClientConnections = 10;

        public ConcurrentDictionary<int, Client> Clients { get; private set; }

        public Server()
        {
            //Network Setup
            _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            _acceptEventArgs = new SocketAsyncEventArgs();
            _acceptEventArgs.Completed += AcceptCompleted;

            //Vars Init
            Clients = new ConcurrentDictionary<int, Client>();
        }

        private void AcceptCompleted(object sender, SocketAsyncEventArgs e)
        {
        }
    }
}