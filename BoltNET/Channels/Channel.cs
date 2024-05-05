using BoltNET.Clients;
using BoltNET.Messages;
using System.Collections.Generic;

namespace BoltNET.Channels
{
    public abstract class Channel
    {
        public Queue<Message> OutGoingPackets = new Queue<Message>(100);
        public Queue<Message> IncomingPackets = new Queue<Message>(100);
        public Client Client { get; protected set; }
        public int OutGoingPacketCount { get => OutGoingPackets.Count; }

        protected Channel(Client client)
        {
            this.Client = client;
        }
        public abstract void HandleOutgoingMessage(Message msg);
        public abstract void HandleIncomingMessage(Message msg);
        public abstract void Send(Message msg);
    }
}
