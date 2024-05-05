using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BoltNET.Utils
{
    public class NetReader
    {
        private byte[] _data;
        private int _position;
        private int _offset;

        public byte[] RawData { get => _data; }
        public NetReader(byte[] buffer, int index, int length)
        {

        }
        public byte ReadByte()
        {
            byte result = _data[_position];
            _position++;
            return result;
        }
        public sbyte ReadSByte()
        {
            return (sbyte)ReadByte();
        }
        public int ReadInt()
        {
            int result = BitConverter.ToInt32(_data, _position);
            _position += 4;
            return result;
        }
        public uint ReadUInt()
        {
            uint result = BitConverter.ToUInt32(_data, _position);
            _position += 4;
            return result;
        }

        public float ReadFloat()
        {
            float result = BitConverter.ToSingle(_data, _position);
            _position += 4;
            return result;
        }
        public double ReadDouble()
        {
            double result = BitConverter.ToDouble(_data, _position);
            _position += 8;
            return result;
        }
        public ushort ReadUShort()
        {
            ushort result = BitConverter.ToUInt16(_data, _position);
            _position += 2;
            return result;
        }
        public short ReadShort()
        {
            short result = BitConverter.ToInt16(_data, _position);
            _position += 2;
            return result;
        }
        public string ReadString()
        {
            // Check if there is enough data to read the string length
            if (_position + sizeof(int) > _data.Length)
            {
                // Handle insufficient data, throw an exception, or return a default value
                throw new InvalidOperationException("Insufficient data to read string length.");
            }

            // Read the length of the string from the data buffer
            int length = BitConverter.ToInt32(_data, _position);
            _position += sizeof(int);

            // Check if there is enough data to read the actual string content
            if (_position + length > _data.Length)
            {
                // Handle insufficient data, throw an exception, or return a default value
                throw new InvalidOperationException("Insufficient data to read string content.");
            }

            // Read the string content from the data buffer
            string result = Encoding.UTF8.GetString(_data, _position, length);
            _position += length;

            return result;
        }

        public long ReadLong()
        {
            long result = BitConverter.ToInt64(_data, _position);
            _position += 8;
            return result;
        }
        public ulong ReadULong()
        {
            ulong result = BitConverter.ToUInt64(_data, _position);
            _position += 8;
            return result;
        }

        public T[] ReadArray<T>(ushort size)
        {
            ushort length = BitConverter.ToUInt16(_data, _position);
            _position += 2;
            T[] result = new T[length];
            length *= size;
            Buffer.BlockCopy(_data, _position, result, 0, length);
            _position += length;
            return result;
        }
        public bool[] GetBoolArray()
        {
            return ReadArray<bool>(1);
        }

        public ushort[] GetUShortArray()
        {
            return ReadArray<ushort>(2);
        }

        public short[] GetShortArray()
        {
            return ReadArray<short>(2);
        }

        public int[] GetIntArray()
        {
            return ReadArray<int>(4);
        }

        public uint[] GetUIntArray()
        {
            return ReadArray<uint>(4);
        }

        public float[] GetFloatArray()
        {
            return ReadArray<float>(4);
        }

        public double[] GetDoubleArray()
        {
            return ReadArray<double>(8);
        }

        public long[] GetLongArray()
        {
            return ReadArray<long>(8);
        }

        public ulong[] GetULongArray()
        {
            return ReadArray<ulong>(8);
        }

        public string[] GetStringArray()
        {
            ushort length = ReadUShort();
            string[] arr = new string[length];
            for (int i = 0; i < length; i++)
            {
                arr[i] = ReadString();
            }
            return arr;
        }

        public bool ReadBool()
        {
            return ReadByte() == 1;
        }

        public char GetChar()
        {
            return (char)ReadUShort();
        }


        public void Clear()
        {
            _position = 0;
            _data = null;
            _offset = 0;
        }
    }
}
