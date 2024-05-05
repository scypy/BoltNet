namespace BoltNET.Messages
{
    public static class MessageHeader
    {
        public static byte WriteHeader(MessageType type)
        {
            switch (type)
            {
                case MessageType.Unreliable:
                case MessageType.Channeled:
                case MessageType.Ack:
                case MessageType.Disconnect:
                case MessageType.Ping:
                case MessageType.Pong:
                case MessageType.ConnectRequest:
                case MessageType.ConnectAccept:
                    return (byte)((byte)type & 0xF);
                default:
                    return ((byte)MessageType.Unknown) & 15;
            }
        }

        public static void ReadHeader(byte header, out MessageType type)
        {
            byte property = (byte)(header & 0xF);
            switch (property)
            {
                case (byte)MessageType.Unreliable:
                case (byte)MessageType.Channeled:
                case (byte)MessageType.Ack:
                case (byte)MessageType.Disconnect:
                case (byte)MessageType.Ping:
                case (byte)MessageType.Pong:
                case (byte)MessageType.ConnectRequest:
                case (byte)MessageType.ConnectAccept:
                    type = (MessageType)header;
                    return;
                default:
                    type = MessageType.Unknown;
                    return;
            }
        }
    }
}
