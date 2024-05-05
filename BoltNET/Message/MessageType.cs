using System;
using System.Collections.Generic;
using System.Text;

namespace BoltNET.Messages
{
    public enum MessageType : byte
    {
        Unreliable,
        Channeled,
        Ack,
        Disconnect,
        Ping,
        Pong,
        ConnectRequest,
        ConnectAccept,
        Unknown
    }
    public enum DeliverType : byte
    {
        Unreliable = 0,
        UnreliableOrdered = 1,
        Reliable = 2,
        ReliableOrdered = 3

    }
}
