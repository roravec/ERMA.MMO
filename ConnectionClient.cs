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
        public string Host { get { return _host; } set { _host = value; } }
        private ushort _port = 19851;
        public ushort Port { get { return _port; } set { _port = value; } }
        protected SocketClient Client;

        public ConnectionClient(string ihost, ushort iport, ISandboxClientConnector sandboxConnector) :
            base(sandboxConnector)
        {
            _port = iport;
            _host = ihost;
            Client = new SocketClient(_host, _port);
            SetHandler(Client.Handler);
            Client.Handler.eventDataReceived += new Connectivity.ConnectionHandler.delDataReceived(DataReceived);
            Client.eventConnectionStatusChanged += new Connectivity.SocketClient.delConnectionStatusChanged(ConnectionStatusChanged);
        }

        public void Connect()
        {
            Client.Connect();
        }
        public void Disconnect()
        {
            Client.Disconnect();
        }
        public ConnectionStatus GetConnectionStatus()
        {
            return Client.ConnectionState;
        }
    }
}
