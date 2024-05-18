using BoltNET.Channels;
using System.Net;
using BoltNET.Messages;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using BoltNET.Globals;
using System.Linq;

namespace BoltNET.Clients
{
    public enum ClientState : byte
    {
        RequestionConnection,
        Connected,
        Disconnected,
        IPChanged
    }
    public class Client
    {
        public uint ID { get; }
        public IPEndPoint EndPoint { get; private set; }

        internal volatile Client NextClient;
        internal volatile Client PrevClient;
        private BoltSocket _socket;
        public BoltSocket Socket { get => _socket; }
        public int MTU { get; private set; }
        public int RTT { get; private set; }

        public int Ping => RTT / 2;

        public Channel UnreliableOrderedChannel;
        public Channel ReliableOrderedChannel;
        public Channel ReliableUnorderedChannel;

        public ClientState State { get; set; }
        public DateTime LastMessageReceived { get; private set; }
        public DateTime LastMessageSent { get; private set; }

        private Queue<Message> _unreliableChannel;
        private ReliableChannel _reliableChannel;
        //ReliableOrderedChannel,
        public Client(uint id, IPEndPoint endPoint, BoltSocket socket)
        {
            this.ID = id;
            this.EndPoint = endPoint;
            this._socket = socket;
            MTU = SetMTU();
        }

        public bool SendChanneled(Message msg, byte channelId)
        {
            return false;
        }

        private int SetMTU()
        {
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            int mtu = 0;
            foreach (NetworkInterface adapter in interfaces)
            {
                if (adapter.OperationalStatus != OperationalStatus.Up) continue;
                IPInterfaceProperties properties = adapter.GetIPProperties();
                IPv4InterfaceProperties ipv4Properties = properties.GetIPv4Properties();
                IPv6InterfaceProperties ipv6Properties = properties.GetIPv6Properties();
                mtu = ipv4Properties.Mtu;
            }
            return mtu <= BoltGlobals.MINIMUM_MTU ? BoltGlobals.MINIMUM_MTU : mtu;
        }

        public bool Disconnect()
        {
            if (this.State != ClientState.Disconnected)
                return _socket.DisconnectClient(this, EndPoint);
            else return false;
        }
    }
}
