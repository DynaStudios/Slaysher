using System;
using System.Collections.Concurrent;
using System.Net.Sockets;

namespace SlaysherServer.Network
{
    public class SocketAsyncEventArgsPool
    {
        private readonly ConcurrentStack<SocketAsyncEventArgs> _mEventsPool;

        public SocketAsyncEventArgsPool(int numConnection)
        {
            _mEventsPool = new ConcurrentStack<SocketAsyncEventArgs>();
        }

        public SocketAsyncEventArgs Pop()
        {
            if (_mEventsPool.IsEmpty)
                return new SocketAsyncEventArgs();

            SocketAsyncEventArgs popped;
            _mEventsPool.TryPop(out popped);

            return popped;
        }

        public void Push(SocketAsyncEventArgs item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("Items added to a SocketAsyncEventArgsPool cannot be null");
            }

            _mEventsPool.Push(item);
        }

        public int Count
        {
            get { return _mEventsPool.Count; }
        }

        public void Dispose()
        {
            foreach (SocketAsyncEventArgs e in _mEventsPool)
            {
                e.Dispose();
            }

            _mEventsPool.Clear();
        }
    }
}