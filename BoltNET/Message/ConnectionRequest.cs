using BoltNET.Clients;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoltNET.Messages
{
    public struct ConnectionRequest : ISerializableMessage<ConnectionRequest>
    {
        public byte Key;
        public ConnectionRequest DeSerialize(Message message)
        {
            message.ReadByte();
            return this;
        }
        // TODO: Connect with key
        public Message Serialize()
        {
            Message msg = new Message(0);
            msg.Write(Key);
            return msg;
        }
    }
}
