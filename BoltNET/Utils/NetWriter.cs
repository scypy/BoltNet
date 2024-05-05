using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BoltNET.Utils
{
    public class NetWriter
    {
        private byte[] _data;
        protected int _position;
        public byte[] RawData { get => _data; }

        public const int StringBufferMaxLength = 65535;
        private const int _defaultSize = 64;
        private readonly byte[] _stringBuffer = new byte[StringBufferMaxLength];

        public NetWriter()
        {
            _data = new byte[_defaultSize];
        }
        public NetWriter(int size)
        {
            _data = new byte[size];
        }

        public void WriteByte(byte value)
        {
            EnsureCapacity(1);
            _data[_position] = value;
            _position++;
        }

        public void WriteSByte(sbyte value)
        {
            EnsureCapacity(sizeof(sbyte));
            _data[_position] = (byte)value;
            _position++;
        }
        public void WriteBytes(byte[] data, int offset, int length)
        {
            EnsureCapacity(_position + length);
            Buffer.BlockCopy(data, offset, _data, _position, length);
            _position += length;
        }

        public void WriteBytes(byte[] data)
        {
            EnsureCapacity(_position + data.Length);
            Buffer.BlockCopy(data, 0, _data, _position, data.Length);
            _position += data.Length;
        }

        public void WriteShort(short value)
        {
            EnsureCapacity(2);
            Buffer.BlockCopy(BitConverter.GetBytes(value), 0, _data, _position, 2);
            _position += 2;
        }

        public void WriteUShort(ushort value)
        {
            EnsureCapacity(2);
            Buffer.BlockCopy(BitConverter.GetBytes(value), 0, _data, _position, 2);
            _position += 2;
        }

        public void WriteInt(int value)
        {
            EnsureCapacity(4);
            Buffer.BlockCopy(BitConverter.GetBytes(value), 0, _data, _position, 4);
            _position += 4;
        }

        public void WriteUInt(uint value)
        {
            EnsureCapacity(4);
            Buffer.BlockCopy(BitConverter.GetBytes(value), 0, _data, _position, 4);
            _position += 4;
        }

        public void WriteLong(long value)
        {
            EnsureCapacity(8);
            Buffer.BlockCopy(BitConverter.GetBytes(value), 0, _data, _position, 4);
            _position += 8;
        }

        public void WriteULong(ulong value)
        {
            EnsureCapacity(8);
            Buffer.BlockCopy(BitConverter.GetBytes(value), 0, _data, _position, 4);
            _position += 8;
        }

        public void WriteDouble(double value)
        {
            EnsureCapacity(8);
            Buffer.BlockCopy(BitConverter.GetBytes(value), 0, _data, _position, 8);
            _position += 8;

        }
        public void WriteString(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
            {
                WriteUShort(0);
                return;
            }
            int length = maxLength > 0 && value.Length > maxLength ? maxLength : value.Length;
            int size = Encoding.ASCII.GetBytes(value, 0, length, _stringBuffer, 0);

            if (size == 0 || size >= StringBufferMaxLength)
            {
                WriteUShort(0);
                return;
            }

            WriteUShort(checked((ushort)(size + 1)));
            WriteBytes(_stringBuffer, 0, size);
        }

        public void WriteArray(Array arr, int sz)
        {
            ushort length = arr == null ? (ushort)0 : (ushort)arr.Length;
            sz *= length;
            EnsureCapacity(_position + sz + 2);
            if (arr != null)
                Buffer.BlockCopy(arr, 0, _data, _position + 2, sz);
            _position += sz + 2;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureCapacity(int additionalBytes)
        {
            if (_position + additionalBytes > _data.Length)
            {
                int newCapacity = Math.Max(_data.Length * 2, _position + additionalBytes);
                Array.Resize(ref _data, newCapacity);
            }
        }
        //public static int WriteCompressed<T>(this BitWriter @this, T val, int bits, bool unsigned) where T : struct
        //{
        //    var size = Marshal.SizeOf<T>();
        //    var buf = new byte[size];
        //    var ptr = Marshal.AllocHGlobal(size);

        //    Marshal.StructureToPtr(val, ptr, false);
        //    Marshal.Copy(ptr, buf, 0, size);
        //    Marshal.FreeHGlobal(ptr);

        //    return @this.WriteCompressed(new ReadOnlySpan<byte>(buf), bits, unsigned);
        //}
    }
}
