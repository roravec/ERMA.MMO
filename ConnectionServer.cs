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
    public class ConnectionServer
    {
        public delegate void delNewClientConnected(ConnectionHandler connHandler);
        public event delNewClientConnected EventNewClientConnected;
        public delegate void delClientDisconnected(ConnectionHandler connHandler);
        public event delClientDisconnected EventClientDisconnected;

        private readonly SocketServer SocketServer;
        private ushort _port;
        public ushort Port
        {
            get { return _port; }
            private set { _port = value; }
        }

        private Object _lockClients = new Object();
        private List<ServerClient> Clients = new List<ServerClient>();
        protected ISandboxClientConnector SandboxConnector = null;

        protected Thread ServerThread { get; set; }
        protected Action ListeningLoopAction;
        protected bool Running = false;

        public ConnectionServer(ushort port, ISandboxClientConnector isbc, Action customListeningLoopAction = null)
        {
            // socket port
            Port = port;
            SandboxConnector = isbc;
            // setup loop method for listening
            if (customListeningLoopAction != null)
                ListeningLoopAction = customListeningLoopAction;
            else
                ListeningLoopAction = StartListeningLoop;
            // create socket server instance and assign methods to handle client connections and disconnections
            this.SocketServer = new SocketServer(Port);
            this.SocketServer.eventClientConnected += new SocketServer.delClientConnected(this.NewClientConnected);
            this.SocketServer.eventClientDisconnected += new SocketServer.delClientDisconnected(this.ClientDisconnected);
        }
        /// <summary>
        /// It will create unique thread that will handle communication listening
        /// </summary>
        /// <param name="thread"></param>
        /// <returns>1 - successful; 0 - already running</returns>
        public int Start()
        {
            if (!Running)
            {
                ServerThread = new Thread(new ThreadStart(ListeningLoopAction));
                ServerThread.Start();
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

        public void SetServerThread(Thread thread)
        {
            ServerThread = thread;
        }
        public Thread GetServerThread()
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

        public void NewClientConnected(SocketConnectionHandler connHandler)
        {
            // fire event to subscribers
            if (EventNewClientConnected != null)
                EventNewClientConnected(connHandler);
            Console.WriteLine("Server: New client connected");
            ServerClient client = new ServerClient(connHandler, SandboxConnector);
            ClientsAdd(client);
        }
        public void ClientDisconnected(ConnectionHandler connHandler)
        {
            // fire event to subscribers
            if (EventClientDisconnected != null)
                EventClientDisconnected(connHandler);
            Console.WriteLine("Server: Client disconnected");
            ClientsRemove(connHandler);
        }

        public void ClientsAdd(ServerClient client)
        {
            if (client != null)
            {
                lock (_lockClients)
                {
                    this.Clients.Add(client);
                }
            }
        }
        public void ClientsRemove(ConnectionHandler client)
        {
            if (client != null)
            {
                lock (_lockClients)
                {
                    ServerClient tempClient = this.Clients.Single(cl => cl.Handler == client);
                    this.Clients.Remove(tempClient);
                }
            }
        }
        public void ClientsRemove(ServerClient client)
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
