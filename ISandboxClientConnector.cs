using Connectivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERMA.MMO
{
    public interface ISandboxClientConnector
    {
        public bool InvokeIncomingClientsFunction(Client client, PacketID packetID, PacketData packetData);
    }
}
