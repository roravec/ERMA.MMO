using Connectivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERMA.MMO
{
    /// <summary>
    /// Client that is only part of some server instance. Cannot connect directly to remote server.
    /// </summary>
    public class ServerClient : Client
    {
        public SocketConnectionHandler SocketHandler;

        public ServerClient(SocketConnectionHandler connHandler, ISandboxClientConnector iSandboxConnector) : 
            base(connHandler, iSandboxConnector)
        {
            SocketHandler = connHandler;
        }
    }
}
