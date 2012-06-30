using System;
using System.Text;

namespace SlaysherServer.Network
{
    public class ByteQueue
    {
        private int _mHead;
        private int _mTail;
        private int _mSize;

        private byte[] _mBuffer;

        public int Length { get { return _mSize; } }

        public byte[] UnderlyingBuffer
        {
            get { return _mBuffer; }
            set { _mBuffer = value; }
        }

        public int Head
        {
            get { return _mHead; }
        }

        public int Tail
        {
            get { return _mTail; }
        }

        public int Size
        {
            get { return _mSize; }
        }

        public ByteQueue()
        {
            _mBuffer = new byte[2048];
        }

        public void Clear()
        {
            _mHead = 0;
            _mTail = 0;
            _mSize = 0;
        }

        public void SetCapacity(int capacity, bool alwaysNewBuffer)
        {
            if (!alwaysNewBuffer)
            {
                if (_mBuffer == null || _mBuffer.Length < capacity)
                    _mBuffer = new byte[capacity];
            }
            else
            {
                var newBuffer = new byte[capacity];

                if (_mSize > 0)
                {
                    if (_mHead < _mTail)
                    {
                        Buffer.BlockCopy(_mBuffer, _mHead, newBuffer, 0, _mSize);
                    }
                    else
                    {
                        Buffer.BlockCopy(_mBuffer, _mHead, newBuffer, 0, _mBuffer.Length - _mHead);
                        Buffer.BlockCopy(_mBuffer, 0, newBuffer, _mBuffer.Length - _mHead, _mTail);
                    }
                }

                _mBuffer = newBuffer;
            }

            _mHead = 0;
            _mTail = _mSize;
        }

        public string[] GetCommands()
        {
            string[] line = Encoding.UTF8.GetString(_mBuffer).Split('\n');

            return line;
        }

        public int Dequeue(byte[] buffer, int offset, int size)
        {
            if (size > _mSize)
                size = _mSize;

            if (size == 0)
                return 0;

            if (_mHead < _mTail)
            {
                Buffer.BlockCopy(_mBuffer, _mHead, buffer, offset, size);
            }
            else
            {
                int rightLength = (_mBuffer.Length - _mHead);

                if (rightLength >= size)
                {
                    Buffer.BlockCopy(_mBuffer, _mHead, buffer, offset, size);
                }
                else
                {
                    Buffer.BlockCopy(_mBuffer, _mHead, buffer, offset, rightLength);
                    Buffer.BlockCopy(_mBuffer, 0, buffer, offset + rightLength, size - rightLength);
                }
            }

            _mHead = (_mHead + size) % _mBuffer.Length;
            _mSize -= size;

            if (_mSize == 0)
            {
                _mHead = 0;
                _mTail = 0;
            }

            return size;
        }

        public void Enqueue(byte[] buffer, int offset, int size)
        {
            if ((_mSize + size) > _mBuffer.Length)
                SetCapacity((_mSize + size + 2047) & ~2047, true);

            if (_mHead < _mTail)
            {
                int rightLength = (_mBuffer.Length - _mTail);

                if (rightLength >= size)
                {
                    Buffer.BlockCopy(buffer, offset, _mBuffer, _mTail, size);
                }
                else
                {
                    Buffer.BlockCopy(buffer, offset, _mBuffer, _mTail, rightLength);
                    Buffer.BlockCopy(buffer, offset + rightLength, _mBuffer, 0, size - rightLength);
                }
            }
            else
            {
                Buffer.BlockCopy(buffer, offset, _mBuffer, _mTail, size);
            }

            _mTail = (_mTail + size) % _mBuffer.Length;
            _mSize += size;
        }
    }
}