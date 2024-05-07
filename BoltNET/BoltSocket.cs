using System.Net;
using System.Net.Sockets;
using BoltNET.Globals;
using BoltNET.Clients;
using BoltNET.Messages;
using BoltNET.Utils;
using System;
using System.Threading;
using System.Collections.Generic;
using BoltNET.BoltNET.Messages;
using System.Threading.Tasks;



namespace BoltNET
{

    public sealed class BoltSocket
    {
        private Socket _ipv4Socket;
        private Socket _ipv6Socket;
        public Socket IPV4Socket { get => _ipv4Socket; }
        public Socket IPV6Socket { get => _ipv6Socket; }
        public bool IsRunning { get; private set; }
        public readonly bool SupportsIPv6 = Socket.OSSupportsIPv6;
        public ConnectionInformation ConnectionInformation { get; set; }

        private Thread _logicThread;
        private Thread _clientThread;

        private uint _clientIdPool;

        private readonly Dictionary<IPEndPoint, Client> _peersDict = new Dictionary<IPEndPoint, Client>();
        private readonly Dictionary<IPEndPoint, ConnectionRequest> _requestsDict = new Dictionary<IPEndPoint, ConnectionRequest>();

        public BoltSocket(ConnectionInformation connectionInformation)
        {
            ConnectionInformation = connectionInformation;
        }
        public bool Start()
        {
            if (IsRunning) return false;
            AppDomain.CurrentDomain.UnhandledException += HandleException;
            _ipv4Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _ipv4Socket.DontFragment = true;
            _ipv4Socket.Blocking = false;
            bool bindSocket = Bind(ConnectionInformation.IPV4Address,
                ConnectionInformation.IPV4Address, ConnectionInformation.Port, ConnectionInformation.UseIPv6Dual);
            IsRunning = true;
            _logicThread = new Thread(ReceiveLogic);
            _logicThread.Start();

            _clientThread = new Thread(ReceiveLogic);
            _clientThread.Start();
            return bindSocket;
        }

        private void HandleException(object sender, UnhandledExceptionEventArgs e)
        {
            ((Exception)e.ExceptionObject).LogException();
        }

        private bool Bind(IPAddress addressIPv4, IPAddress addressIPv6, int port, bool ipv6Dual)
        {
            _ipv4Socket = new Socket(AddressFamily.InterNetwork,
                SocketType.Dgram, ProtocolType.Udp);

            if (!SetupAndBind(_ipv4Socket, new IPEndPoint(addressIPv4, port))) return false;
            int ipv4LocalPort = ((IPEndPoint)_ipv4Socket.LocalEndPoint).Port;

            if (!ipv6Dual || !SupportsIPv6)
            {
                return true;
            }
            _ipv6Socket = new Socket(AddressFamily.InterNetworkV6,
                SocketType.Dgram, ProtocolType.Udp);


            SetupAndBind(_ipv6Socket, new IPEndPoint(addressIPv6, ipv4LocalPort));
            return true;
        }

        public void CloseSocket()
        {
            IsRunning = false;
            if (_ipv4Socket != null)
                _ipv4Socket.Close();
            if (_ipv4Socket != null)
                _ipv6Socket.Close();
        }
        private bool SetupAndBind(Socket socket, IPEndPoint endpoint)
        {
            if (socket.AddressFamily == AddressFamily.InterNetwork)
            {
                try
                {
                    socket.DontFragment = true;
                }
                catch (SocketException e)
                {

                }

                try
                {
                    socket.EnableBroadcast = true;
                }
                catch (SocketException e)
                {
                    Console.WriteLine("Socket Broadcast not Supported!");
                }
            }

            try
            {
                socket.ReceiveBufferSize = BoltGlobals.ReceiveBufferSize;
                socket.SendBufferSize = BoltGlobals.SendBufferSize;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            try
            {
                const uint IOC_IN = 0x80000000;
                const uint IOC_VENDOR = 0x18000000;
                const uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;

                unchecked
                {
                    socket.IOControl((int)SIO_UDP_CONNRESET, new byte[] { 0 }, null);
                }
            }
            catch (Exception e)
            {
                // Ignore error when SIO_UDP_CONNRESET is not supported
                Console.WriteLine(e.Message);
            }

            try
            {
                socket.Ttl = (short)64;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            try
            {
                // Bind the socket to the OS
                socket.Bind(endpoint);
            }
            catch (SocketException bindException)
            {
                switch (bindException.SocketErrorCode)
                {
                    // IPv6 bind fix
                    case SocketError.AddressAlreadyInUse:
                        {
                            if (socket.AddressFamily == AddressFamily.InterNetworkV6)
                            {
                                try
                                {
                                    socket.SetSocketOption(SocketOptionLevel.IPv6, (SocketOptionName)27, true);
                                    socket.Bind(endpoint);
                                }
                                catch (SocketException e)
                                {
                                    Console.WriteLine("Socket bind failed after setting dual mode with exception: " + e);
                                    return false;
                                }

                                return true;
                            }
                        }
                        break;
                    // Fixes Unity exception for iOS (requires IPv6 but the runtime throws)
                    case SocketError.AddressFamilyNotSupported:
                        {
                            return true;
                        }
                }
            }

            return true;
        }

        public async Task<bool> Connect(IPEndPoint target)
        {
            await _ipv4Socket.ConnectAsync(target);
            return _ipv4Socket.Connected;
        }

        private uint GetNextClientId()
        {
            return _clientIdPool++;
        }

        private void ReceiveFrom(Socket s, ref EndPoint bufferEndPoint)
        {
            var packet = new Message(BoltGlobals.ReceiveBufferSize);
            packet.Size = s.ReceiveFrom(packet.Data, 0, BoltGlobals.ReceiveBufferSize, SocketFlags.None, ref bufferEndPoint);
            $"Receiving...".Log();
            OnMessageReceived(packet, (IPEndPoint)bufferEndPoint);
        }

        private void OnMessageReceived(Message packet, IPEndPoint bufferEndPoint)
        {
            packet.ReadHeader(packet.Data[0], out MessageType type);
            switch (type)
            {
                case MessageType.Unreliable:
                    $"Received Unreliable {type}".Log();
                    break;
                case MessageType.Channeled:
                    break;
                case MessageType.Ack:
                    break;
                case MessageType.Disconnect:
                    break;
                case MessageType.Ping:
                    break;
                case MessageType.Pong:
                    break;
                case MessageType.ConnectRequest:
                    break;
                case MessageType.ConnectAccept:
                    break;
                case MessageType.Unknown:
                    break;
                default:
                    break;
            }
        }

        private void ReceiveLogic()
        {
            EndPoint bufferEndPoint4 = new IPEndPoint(IPAddress.Any, 0);
            EndPoint bufferEndPoint6 = new IPEndPoint(IPAddress.IPv6Any, 0);
            var selectReadList = new List<Socket>(2);
            var socketv4 = _ipv4Socket;
            var socketV6 = _ipv6Socket;
            while (IsRunning)
            {
                try
                {
                    if (socketV6 == null)
                    {
                        if (socketv4.Available == 0 && !socketv4.Poll(500000, SelectMode.SelectRead))
                            continue;
                        ReceiveFrom(socketv4, ref bufferEndPoint4);
                    }
                    else
                    {
                        bool messageReceived = false;
                        if (socketv4.Available != 0)
                        {
                            ReceiveFrom(socketv4, ref bufferEndPoint4);
                            messageReceived = true;
                        }
                        if (socketV6.Available != 0)
                        {
                            ReceiveFrom(socketV6, ref bufferEndPoint6);
                            messageReceived = true;
                        }
                        if (messageReceived)
                            continue;

                        selectReadList.Clear();
                        selectReadList.Add(socketv4);
                        selectReadList.Add(socketV6);
                        Socket.Select(selectReadList, null, null, 500000);
                    }
                }
                catch (SocketException ex)
                {
                    $"Socket Error {ex.SocketErrorCode}".Log();
                }
                catch (ObjectDisposedException)
                {
                    return;
                }
                catch (ThreadAbortException)
                {
                    return;
                }
                catch (Exception e)
                {
                    return;
                }
            }
        }
        public bool SendUnreliable(Message message, IPEndPoint endpoint)
        {
            if (message.Size > BoltGlobals.SendBufferSize)
            {
            $"Packet Size Exceeds Send Limit!".Log(); return false;
            }
            message.WriteHeader(MessageType.Unreliable);
            return _ipv4Socket.SendTo(message.Data, message.Offset, message.Size, SocketFlags.None, endpoint) > 0;
        }
    }
}