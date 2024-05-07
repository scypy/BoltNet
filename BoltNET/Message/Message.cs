using BoltNET.Globals;
using System;
using System.Net.NetworkInformation;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
namespace BoltNET.Messages
{
    /*
     * Todo:
     * Implement Checksum
     */
    public class Message
    {
        private static readonly int PropertiesCount = Enum.GetValues(typeof(MessageType)).Length;


        private byte[] _data;
        private int _position;
        private int _offset;
        public int Position { get => _position; }
        public int Offset { get => _offset; }
        public byte[] Data { get => _data; }
        public int Size { get; set; }
        //Header
        public MessageType Property { get; set; }
        public ushort OpCode { get; private set; }
        public const int StringBufferMaxLength = 65535;
        public bool IsFragmented { get; set; }

        #region Constructors
        public Message(ushort opcode)
        {
            OpCode = opcode;
            Write(OpCode);
            this.Size = BoltGlobals.SendBufferSize;
            this._data = new byte[BoltGlobals.SendBufferSize];
        }
        public Message()
        {
            this.Size = BoltGlobals.SendBufferSize;
            this._data = new byte[BoltGlobals.SendBufferSize];
        }
        public Message(MessageType property, int size)
        {
            this.Property = property;
            this.Size = size;
            this._data = new byte[size];
        }
        public Message(byte[] data, int offset, int size)
        {
            this._data = data;
            this._offset = offset;
            this.Size = size;
        }
        #endregion

        public Message(byte[] rawData, int size)
        {
            _data = rawData;
            Size = size;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ResizeIfNeed(int newSize)
        {
            if (_data.Length < newSize)
            {
                Array.Resize(ref _data, Math.Max(newSize, _data.Length * 2));
            }
        }
        public byte WriteHeader(MessageType type)
        {
            return MessageHeader.WriteHeader(type);
        }

        public void ReadHeader(byte header, out MessageType type)
        {
            type = MessageType.Unknown;
            MessageHeader.ReadHeader(header, out MessageType msgType);
        }

        public override string ToString()
        {
            return $"Type: {Property}, Size: {Size}";
        }
        #region Writes
        public void Write(byte value)
        {
            EnsureCapacity(sizeof(byte));
            _data[_position] = value;
            _position++;
        }
        public void Write(sbyte value)
        {
            EnsureCapacity(sizeof(sbyte));
            _data[_position] = (byte)value;
            _position++;
        }
        public void Write(byte[] data, int offset, int length)
        {
            EnsureCapacity(_position + length);
            Buffer.BlockCopy(data, offset, _data, _position, length);
            _position += length;
        }
        public void Write(byte[] data)
        {
            EnsureCapacity(_position + data.Length);
            Buffer.BlockCopy(data, 0, _data, _position, data.Length);
            _position += data.Length;
        }
        public void Write(short value)
        {
            EnsureCapacity(sizeof(short));
            Buffer.BlockCopy(BitConverter.GetBytes(value), 0, _data, _position, 2);
            _position += 2;
        }
        public void Write(ushort value)
        {
            EnsureCapacity(sizeof(ushort));
            Buffer.BlockCopy(BitConverter.GetBytes(value), 0, _data, _position, 2);
            _position += 2;
        }
        public void Write(int value)
        {
            EnsureCapacity(sizeof(int));
            Buffer.BlockCopy(BitConverter.GetBytes(value), 0, _data, _position, 4);
            _position += 4;
        }

        public void Write(uint value)
        {
            EnsureCapacity(sizeof(uint));
            Buffer.BlockCopy(BitConverter.GetBytes(value), 0, _data, _position, 4);
            _position += 4;
        }
        public void Write(long value)
        {
            EnsureCapacity(sizeof(long));
            Buffer.BlockCopy(BitConverter.GetBytes(value), 0, _data, _position, 8);
            _position += 8;
        }

        public void Write(ulong value)
        {
            EnsureCapacity(sizeof(ulong));
            Buffer.BlockCopy(BitConverter.GetBytes(value), 0, _data, _position, 8);
            _position += 8;
        }
        public void Write(double value)
        {
            EnsureCapacity(sizeof(double));
            Buffer.BlockCopy(BitConverter.GetBytes(value), 0, _data, _position, 8);
            _position += 8;

        }
        public void Write(float value)
        {
            EnsureCapacity(sizeof(float));
            Buffer.BlockCopy(BitConverter.GetBytes(value), 0, _data, _position, 4);
            _position += 8;
        }

        public void Write(string value)
        {
            
            //nothing to write
           if (string.IsNullOrEmpty(value)) return;

           var data = Encoding.ASCII.GetBytes(value);
           ushort length = value.Length > ushort.MaxValue ? ushort.MaxValue : (ushort)data.Length;
           Write(checked(length));
           Write(data);
        }
        #endregion

        #region Reads
        public byte ReadByte()
        {
            byte result = _data[_position];
            _position++;
            return result;
        }
        public ushort ReadUShort()
        {
            ushort result = BitConverter.ToUInt16(_data, _position);
            _position += 2;
            return result;
        }
        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureCapacity(int additionalBytes)
        {
            if (_position + additionalBytes > _data.Length)
            {
                int newCapacity = Math.Max(_data.Length * 2, _position + additionalBytes);
                Array.Resize(ref _data, newCapacity);
            }
        }
    }
}
