using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BoltNET.Clients
{
    //Class for config that Client can edit.
    public class ConnectionInformation
    {
        public IPAddress IPV4Address { get; set; }
        public IPAddress IPV6Address { get; set; }
        public ushort Port { get; set; }
        public bool UseIPv6Dual { get; set; }
        public int CustomMTU { get; set; }
        public int CustomBuffer { get; set; }
    }
}
