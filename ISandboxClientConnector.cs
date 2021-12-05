using Connectivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERMA.MMO
{
    /// <summary>
    /// Provides interface for clients to call packet actions/methods thru Sandbox
    /// </summary>
    public interface ISandboxClientConnector
    {
        /// <summary>
        /// Client can call Sandbox method to Invoke Packets action
        /// </summary>
        /// <param name="client"></param>
        /// <param name="packetID"></param>
        /// <param name="packetData"></param>
        /// <returns></returns>
        public bool InvokeIncomingClientsFunction(Client client, int packetID, PacketData packetData);
    }
}
