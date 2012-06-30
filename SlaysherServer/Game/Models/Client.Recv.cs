using System;
using System.Net.Sockets;
using System.Threading;
using SlaysherServer.Network;

namespace SlaysherServer.Game.Models
{
    public partial class Client
    {
        public int TimesEnqueuedForRecv;
        private readonly object _queueSwapLock = new object();

        private void RecvStart()
        {
            if (!Running)
            {
                // DisposeRecvSystem();
                return;
            }

            if (!_socket.Connected)
            {
                //  Stop();
                return;
            }

            try
            {
                bool pending = _socket.ReceiveAsync(_recvSocketEvent);

                if (!pending)
                    RecvCompleted(null, _recvSocketEvent);
            }
            catch
            {
                //Yo Patrick you should really catch this shit!
            }
        }

        private void RecvCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (Running)
            {
                if (DateTime.Now + TimeSpan.FromSeconds(5) > _nextActivityCheck)
                    _nextActivityCheck = DateTime.Now + TimeSpan.FromSeconds(5);
                RecvProcess(e);
            }
        }

        private void RecvProcess(SocketAsyncEventArgs e)
        {
            lock (_queueSwapLock)
                _currentBuffer.Enqueue(e.Buffer, 0, e.BytesTransferred);

            int newValue = Interlocked.Increment(ref TimesEnqueuedForRecv);

            if ((newValue - 1) == 0)
                Server.RecvClientQueue.Enqueue(this);

            _server.NetworkSignal.Set();

            RecvStart();
        }

        public ByteQueue GetBufferToProcess()
        {
            lock (_queueSwapLock)
            {
                ByteQueue temp = _currentBuffer;
                _currentBuffer = _processedBuffer;
                _processedBuffer = temp;
            }

            return _processedBuffer;
        }
    }
}