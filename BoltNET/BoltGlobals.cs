using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BoltNET.Globals
{
    public static class BoltGlobals
    {
        public const int HeaderSize = 1;
        public const int ChanneledHeader = 4;
        public const int SendBufferSize = 1024;
        public const int ReceiveBufferSize = 1024;
        public const int SOCKET_PACKET_TTL = 64;
        //Minimum on most systems
        public const int MINIMUM_MTU = 576;

    }
}
