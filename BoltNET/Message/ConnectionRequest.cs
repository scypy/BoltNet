using BoltNET.Clients;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoltNET.Messages
{
    public class ConnectionRequest : ISerializableMessage<ConnectionRequest>
    {
        public ConnectionRequest DeSerialize(Message message)
        {
            return this;
        }
        // TODO: Connect with key
        public Message Serialize()
        {
            return new Message(-1);
        }
    }
}
