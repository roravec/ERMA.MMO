using Connectivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Connectivity.SocketClient;

namespace ERMA.MMO
{
    /// <summary>
    /// Client that can connect standalone to server
    /// </summary>
    public class ConnectionClient : Client
    {
        private string _host = "";
        /// <summary>
        /// Hostname
        /// </summary>
        public string Host { get { return _host; } set { _host = value; } }
        private ushort _port = 19851;
        /// <summary>
        /// 
        /// </summary>
        public ushort Port { get { return _port; } set { _port = value; } }
        /// <summary>
        /// Instance of Connectivity.SocketClient
        /// Whole communication is driven by this.
        /// </summary>
        protected SocketClient Client;

        /// <summary>
        /// Create instance of ConnectionClient which is inherited from Client so you can easily send and receive messages over TCP/IP
        /// Check <see cref="Client">Client</see> for more information about available methods.
        /// </summary>
        /// <param name="ihost"></param>
        /// <param name="iport"></param>
        /// <param name="sandboxConnector"></param>
        /// <param name="protocol">Default is TCP, you can choose between TCP and UDP</param>
        public ConnectionClient(string ihost, ushort iport, ISandboxClientConnector sandboxConnector, ConnectionProtocol protocol = ConnectionProtocol.TCP) :
            base(sandboxConnector, protocol)
        {
            // TODO: UDP
            _port = iport;
            _host = ihost;
            Client = new SocketClient(_host, _port, Protocol);
            SetHandler(Client.Handler);
            Client.Handler.eventDataReceived += new Connectivity.ConnectionHandler.delDataReceived(DataReceived);
            Client.eventConnectionStatusChanged += new Connectivity.SocketClient.delConnectionStatusChanged(ConnectionStatusChanged);
        }

        /// <summary>
        /// Start a connection to server
        /// </summary>
        public void Connect()
        {
            Client.Connect();
        }
        /// <summary>
        /// Stops the connection to server
        /// </summary>
        public void Disconnect()
        {
            Client.Disconnect();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Current connection status of the client</returns>
        public ConnectionStatus GetConnectionStatus()
        {
            return Client.ConnectionState;
        }
    }
}