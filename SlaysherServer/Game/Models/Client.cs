using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using SlaysherServer.Network;

namespace SlaysherServer.Game.Models
{
    public partial class Client
    {
        public volatile bool Running = true;

        //Network Buffers
        private ByteQueue _currentBuffer;

        private ByteQueue _processedBuffer;

        public ByteQueue FragPackets { get; set; }

        public static SocketAsyncEventArgsPool SendSocketEventPool = new SocketAsyncEventArgsPool(10);
        public static SocketAsyncEventArgsPool RecvSocketEventPool = new SocketAsyncEventArgsPool(10);
        public static BufferPool RecvBufferPool = new BufferPool("Receive", 2048, 2048);

        private byte[] _recvBuffer;
        private SocketAsyncEventArgs _sendSocketEvent;
        private SocketAsyncEventArgs _recvSocketEvent;

        private bool _sendSystemDisposed;
        private bool _recvSystemDisposed;

        private readonly object _disposeLock = new object();

        //Client specific Stuff
        public int ClientId { get; private set; }

        private DateTime _nextActivityCheck;

        private Server _server;
        private readonly Socket _socket;

        public Client(int nextClientId, Server server, Socket socket)
        {
            ClientId = nextClientId;
            _server = server;
            _socket = socket;

            _nextActivityCheck = DateTime.Now + TimeSpan.FromSeconds(30);
        }

        public void Start()
        {
            _sendSocketEvent = SendSocketEventPool.Pop();
            _recvSocketEvent = RecvSocketEventPool.Pop();
            _recvBuffer = RecvBufferPool.AcquireBuffer();

            _recvSocketEvent.SetBuffer(_recvBuffer, 0, _recvBuffer.Length);
            _recvSocketEvent.Completed += RecvCompleted;
            _sendSocketEvent.Completed += Send_Completed;

            Task.Factory.StartNew(RecvStart);
        }
    }
}