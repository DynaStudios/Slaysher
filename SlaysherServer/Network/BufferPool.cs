using System.Collections.Generic;

namespace SlaysherServer.Network
{
    public class BufferPool
    {
        private static List<BufferPool> _mPools = new List<BufferPool>();

        public static List<BufferPool> Pools
        {
            get { return _mPools; }
            set { _mPools = value; }
        }

        private readonly string _mName;

        private readonly int _mInitialCapacity;
        private readonly int _mBufferSize;

        private int _mMisses;

        private readonly Queue<byte[]> _mFreeBuffers;

        public void GetInfo(out string name, out int freeCount, out int initialCapacity, out int currentCapacity,
                            out int bufferSize, out int misses)
        {
            lock (this)
            {
                name = _mName;
                freeCount = _mFreeBuffers.Count;
                initialCapacity = _mInitialCapacity;
                currentCapacity = _mInitialCapacity*(1 + _mMisses);
                bufferSize = _mBufferSize;
                misses = _mMisses;
            }
        }

        public BufferPool(string name, int initialCapacity, int bufferSize)
        {
            _mName = name;

            _mInitialCapacity = initialCapacity;
            _mBufferSize = bufferSize;

            _mFreeBuffers = new Queue<byte[]>(initialCapacity);

            for (int i = 0; i < initialCapacity; ++i)
                _mFreeBuffers.Enqueue(new byte[bufferSize]);

            lock (_mPools)
                _mPools.Add(this);
        }

        public byte[] AcquireBuffer()
        {
            lock (this)
            {
                if (_mFreeBuffers.Count > 0)
                    return _mFreeBuffers.Dequeue();

                ++_mMisses;

                for (int i = 0; i < _mInitialCapacity; ++i)
                    _mFreeBuffers.Enqueue(new byte[_mBufferSize]);

                return _mFreeBuffers.Dequeue();
            }
        }

        public void ReleaseBuffer(byte[] buffer)
        {
            if (buffer == null)
                return;

            lock (this)
                _mFreeBuffers.Enqueue(buffer);
        }

        public void Free()
        {
            lock (_mPools)
                _mPools.Remove(this);
        }
    }
}