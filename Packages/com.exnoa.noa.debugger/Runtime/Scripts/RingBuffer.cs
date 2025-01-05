using System;
using System.Collections;
using System.Collections.Generic;

namespace NoaDebugger
{
    sealed class RingBuffer<T> : IEnumerable<T>
    {
        readonly T[] _buffer;

        public int Count { get; private set; }

        public int Capacity => _buffer.Length;

        public bool IsEmpty => Count == 0;

        public bool IsFull => Count == Capacity;

        public int Top { get; private set; } = 0;

        public int Tail { get; private set; } = 0;

        public RingBuffer(int capacity)
        {
            if (capacity < 1)
            {
                throw new ArgumentException("Capacity must be greater than 1.");
            }
            _buffer = new T[capacity];
        }

        public void Append(T element)
        {
            if (Count > 0)
            {
                Tail = ++Tail % Capacity;

                if (Tail == Top)
                {
                    Top = ++Top % Capacity;
                }
            }

            _buffer[Tail] = element;

            if (Count < Capacity)
            {
                ++Count;
            }
        }

        public void Clear()
        {
            Top = 0;
            Tail = 0;
            Count = 0;
        }

        public void Remove(int index)
        {
            if (index < 0 || Count <= index)
            {
                throw new IndexOutOfRangeException();
            }

            if (Count == 1)
            {
                Clear();
            }
            else
            {
                for (int i = (Top + index) % Capacity; i != Tail; i = ++i % Capacity)
                {
                    _buffer[i] = _buffer[(i + 1) % Capacity];
                }
                Tail = --Tail % Capacity;
                --Count;
            }
        }

        public T At(int index)
        {
            if (index < 0 || Count <= index)
            {
                throw new IndexOutOfRangeException();
            }
            return _buffer[(Top + index) % Capacity];
        }

        public int NextPosition(int current)
        {
            if (current == Tail)
            {
                return Tail;
            }
            if ((current < 0)
                || (Capacity <= current)
                || (Top <= Tail && Tail < current)
                || (Tail < current && current < Top))
            {
                throw new IndexOutOfRangeException();
            }
            return ++current % Capacity;
        }

        IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => new Enumerator(this);

        struct Enumerator : IEnumerator<T>
        {
            readonly RingBuffer<T> _target;

            int _current;

            int _count;

            public Enumerator(RingBuffer<T> target)
            {
                _target = target;
                _current = (_target.Top - 1) % _target.Capacity;
                _count = 0;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (_count == _target.Count)
                {
                    return false;
                }
                _current = ++_current % _target.Capacity;
                ++_count;
                return true;
            }

            public void Reset()
            {
                _current = (_target.Top - 1) % _target.Capacity;
                _count = 0;
            }

            public T Current => _target._buffer[_current];

            object IEnumerator.Current => _target._buffer[_current];
        }
    }
}
