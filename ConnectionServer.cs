using Connectivity;
using ERMA.MMO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ERMA.MMO
{
    /// <summary>
    /// Wrapper around Connectivity.SocketServer
    /// Provides easy way to create and manage TCP/UDP socket server
    /// ERMA.MMO.ConnectionServer is suppose to run under ERMA.MMO.Sandbox (can be easily created from there) or other ISandboxClientConnector 
    /// because clients of server and server itself will call ISandboxClientConnector for packet sorting.
    /// </summary>
    /// <example>
    /// First create Sandbox! Or any other ISandboxClientConnector
    /// Call ConnectionServer(ushort port, ISandboxClientConnector)
    /// Subscribe to events EventNewClientConnected and EventClientDisconnected
    /// </example>
    public class ConnectionServer
    {
        /// <summary>
        /// Delegate for EventNewClientConnected
        /// </summary>
        /// <param name="client"></param>
        public delegate void delNewClientConnected(Client client);
        /// <summary>
        /// Fired when new client connect to server and added to clients list. Pass null pointer of Client - event will set the new client there.
        /// </summary>
        public event delNewClientConnected EventNewClientConnected;
        /// <summary>
        /// Delegate for event EventClientDisconnected
        /// </summary>
        /// <param name="client"></param>
        public delegate void delClientDisconnected(Client client);
        /// <summary>
        /// Fired when new client has been disconnected from server and removed from clients list. Pass null pointer of Client - event will set the disconnected client there.
        /// </summary>
        public event delClientDisconnected EventClientDisconnected;
        /// <summary>
        /// Connection socket protocol used 
        /// </summary>
        public readonly ConnectionProtocol Protocol = ConnectionProtocol.TCP;
        private readonly SocketServer SocketServer;
        private ushort _port;
        /// <summary>
        /// Server is listening on this port
        /// </summary>
        public ushort Port
        {
            get { return _port; }
            private set { _port = value; }
        }

        private Object _lockClients = new Object();
        private List<Client> Clients = new List<Client>();
        /// <summary>
        /// Connector to Sandbox which will provide packet action sorting
        /// </summary>
        protected ISandboxClientConnector SandboxConnector = null;
        
        /// <summary>
        /// Server will listen on this thread
        /// </summary>
        protected Thread ServerThread { get; set; }
        /// <summary>
        /// Action to call server listening function
        /// </summary>
        protected Action ListeningLoopAction;
        /// <summary>
        /// Is server fully running?
        /// </summary>
        public bool Running = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="isbc"></param>
        /// <param name="protocol">Default is TCP, you can choose between TCP and UDP</param>
        /// <param name="customListeningLoopAction"></param>
        public ConnectionServer(ushort port, ISandboxClientConnector isbc, ConnectionProtocol protocol = ConnectionProtocol.TCP, Action customListeningLoopAction = null)
        {
            Protocol = protocol;
            // socket port
            Port = port;
            SandboxConnector = isbc;
            // setup loop method for listening
            if (customListeningLoopAction != null)
                ListeningLoopAction = customListeningLoopAction;
            else
                ListeningLoopAction = StartListeningLoop;
            // create socket server instance and assign methods to handle client connections and disconnections
            this.SocketServer = new SocketServer(Port, Protocol);
            this.SocketServer.eventClientConnected += new SocketServer.delClientConnected(this.NewClientConnectedToSocketServer);
            this.SocketServer.eventClientDisconnected += new SocketServer.delClientDisconnected(this.ClientDisconnectedFromSocketServer);
        }
        /// <summary>
        /// It will create unique thread that will handle communication listening
        /// </summary>
        /// <returns>1 - successful; 0 - already running</returns>
        public int Start()
        {
            if (!Running)
            {
                ServerThread = new Thread(new ThreadStart(ListeningLoopAction));
                ServerThread.Start();
                Running = true;
                return 1;
            }
            else
                return 0;
        }
        /// <summary>
        /// Do not run this in main thread, in will create listening loop
        /// </summary>
        protected void StartListeningLoop()
        {
            SocketServer.Start();
        }
        /// <summary>
        /// Interrupts socket server listening thread
        /// </summary>
        /// <returns></returns>
        public int Stop()
        {
            ServerThread.Interrupt();
            return 1;
        }

        /// <summary>
        /// Set custom thread where server will be listening
        /// </summary>
        /// <param name="thread"></param>
        public void SetServerThread(Thread thread)
        {
            ServerThread = thread;
        }
        /// <summary>
        /// Server is listening on this thread
        /// </summary>
        /// <returns></returns>
        internal Thread GetServerThread()
        {
            return ServerThread;
        }

        private void ThreadTCPServer()
        {
            if (true)
            {
                Start();
            }
        }

        private void NewClientConnectedToSocketServer(ConnectionHandler connHandler)
        {
            Console.WriteLine("Server: New client connected");
            Client client = new Client(connHandler, SandboxConnector);
            ClientsAdd(client);
            // fire event to subscribers
            if (EventNewClientConnected != null)
                EventNewClientConnected(client);
        }
        private void ClientDisconnectedFromSocketServer(ConnectionHandler connHandler)
        {
            Console.WriteLine("Server: Client disconnected");
            var disconnectedClient = GetClientInListByConnectionHandler(connHandler);
            ClientsRemove(disconnectedClient);
            // fire event to subscribers
            if (EventClientDisconnected != null)
                EventClientDisconnected(disconnectedClient);
        }
        /// <summary>
        /// Add new client to list. Use only when new client is connected.
        /// </summary>
        /// <param name="client"></param>
        internal void ClientsAdd(Client client)
        {
            if (client != null)
            {
                lock (_lockClients)
                {
                    this.Clients.Add(client);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        internal Client GetClientInListByConnectionHandler(ConnectionHandler client)
        {
            if (client != null)
            {
                Client tempClient = this.Clients.Single(cl => cl.Handler == client);
                return tempClient;
            }
            return null;
        }
        /// <summary>
        /// Remove client from list of clients - use only after client was disconnected.
        /// </summary>
        /// <param name="client"></param>
        internal void ClientsRemove(ConnectionHandler client)
        {
            if (client != null)
            {
                var tempClient = GetClientInListByConnectionHandler(client);
                if (tempClient != null)
                {
                    lock (_lockClients)
                    {
                        this.Clients.Remove((Client)tempClient);
                    }
                }
            }
        }
        /// <summary>
        /// Remove client from list of clients - use only after client was disconnected.
        /// </summary>
        /// <param name="client"></param>
        internal void ClientsRemove(Client client)
        {
            if (client != null)
            {
                lock (_lockClients)
                {
                    this.Clients.Remove(client);
                }
            }
        }
    }
}
