using BoltNET.Clients;
using BoltNET.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoltNET.Channels
{
    public class ReliableChannel : Channel
    {
        Dictionary<ushort, Message> _sentPackets = new Dictionary<ushort, Message>();
        ushort _sequence = 0;
        public ReliableChannel(Client client) : base(client)
        {
            this.Client = client;
        }

        public override void HandleIncomingMessage(Message msg)
        {
            lock (IncomingPackets)
            {
                IncomingPackets.Enqueue(msg);
            }
        }

        public override void HandleOutgoingMessage(Message msg)
        {
            lock (OutGoingPackets)
            {
                OutGoingPackets.Enqueue(msg);
            }
        }

        public override void Send(Message msg)
        {
            throw new NotImplementedException();
        }
        // TODO
        // Finish send/ack
        public void SendReliable(Message msg)
        {
            Client.SendChanneled(msg, (byte)DeliverType.ReliableOrdered);
            _sentPackets.Add(_sequence, msg);
        }
    }
}
