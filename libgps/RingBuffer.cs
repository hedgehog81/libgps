using System;
using System.IO;

namespace Location.Gps
{
    internal class RingBuffer
    {
        private int m_size;
        private int[] m_index;
        private byte[] m_buffer;

        private enum IndxType
        {
            Read,
            Write
        }


        public RingBuffer(int capacity)
        {
            m_buffer = new byte[capacity];
            m_index = new int[2];
        }


        public int BytesToRead
        {
            get
            {
                return m_size;
            }
        }

        public int Capacity
        {
            get
            {
                return m_buffer.Length;
            }
        }

        public bool IsFull
        {
            get
            {
                return BytesToRead == Capacity;
            }
        }

        public byte Get()
        {
            if (m_size == 0)
                throw new InvalidOperationException("The buffer is empty.");

            byte ret = m_buffer[GetIndex(IndxType.Read)];
            AdvanceIndex(IndxType.Read, 1);
            return ret;
        }

        public void Put(byte b)
        {
            if (BytesToRead == Capacity)
                throw new InvalidOperationException("The buffer is full.");

            m_buffer[GetIndex(IndxType.Write)] = b;
            AdvanceIndex(IndxType.Write, 1);
        }

        public void Put(Stream strm)
        {
            if (BytesToRead == Capacity)
                throw new InvalidOperationException("The buffer is full.");

            if (GetIndex(IndxType.Write) < GetIndex(IndxType.Read))
            {
                int size = GetIndex(IndxType.Read) - GetIndex(IndxType.Write);
                int read = strm.Read(m_buffer, GetIndex(IndxType.Write), size);
                AdvanceIndex(IndxType.Write, read);
            }
            else
            {
                int size = Capacity - GetIndex(IndxType.Write);
                int read = strm.Read(m_buffer, GetIndex(IndxType.Write), size);
                AdvanceIndex(IndxType.Write, read);

                if (read == size)
                {
                    size = GetIndex(IndxType.Read);
                    read = strm.Read(m_buffer, 0, size);
                    AdvanceIndex(IndxType.Write, read);
                }
            }

        }

        public void Clear()
        {
            m_index[0] = m_index[1] = m_size = 0;
        }

        private int GetIndex(IndxType indx)
        {
            return m_index[(int)indx];
        }

        private int AdvanceIndex(IndxType indx, int distance)
        {
            int prevIndx = m_index[(int)indx];
            m_index[(int)indx] = (prevIndx + distance) % Capacity;

            m_size += distance * ((indx == IndxType.Read) ? -1 : 1);

            return m_index[(int)indx];
        }

    }

}
