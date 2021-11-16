using Connectivity;
using DatabaseConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ERMA.MMO
{
    /// <summary>
    /// Class provides functionality for creating TCP/UDP servers and clients, DB connection, packet handling.
    /// Inherit from this class to make your own Sandbox MMO application
    /// In constructor provide Type childType - this is a type of class where you define all packet methods
    /// </summary>
    public class Sandbox : ISandboxClientConnector
    {
        protected PacketFunctionsDictionary PacketFunctionsDictionary;
        protected DatabaseConnector.DatabaseConnector DBConnector;
        protected List<ConnectionServer> Servers = new List<ConnectionServer>();
        protected List<ConnectionClient> Clients = new List<ConnectionClient>();
        protected ConfigSet ConfigurationSetup;
        protected Type ChildClassType;

        public Sandbox(ConfigSet configuration, PacketFunctionDictionaryType packetFunctionDictionaryType, Type childType=null)
        {
            if (childType == null)
                ChildClassType = this.GetType();
            else
                ChildClassType = childType;
            FirstTimeInit(configuration, packetFunctionDictionaryType);
        }
        /// <summary>
        /// This method is called only from constructor.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="packetFunctionDictionaryType"></param>
        private void FirstTimeInit(ConfigSet configuration, PacketFunctionDictionaryType packetFunctionDictionaryType)
        {
            ConfigurationSetup = configuration;
            PacketFunctionsDictionary = new PacketFunctionsDictionary(packetFunctionDictionaryType, (IGenericDatabaseConnector)DBConnector);
            PacketFunctionsDictionary.UpdateDictionaryFromDatabase();
        }

        /// <summary>
        /// Starts all servers and clients
        /// </summary>
        public void Start()
        {
            // todo: start all servers and clients
            foreach (ConnectionServer server in Servers)
                server.Start();
            foreach (ConnectionClient client in Clients)
                client.Connect();
        }
        /// <summary>
        /// Stops all servers and clients
        /// </summary>
        public void Stop()
        {
            // todo: start all servers and clients
            foreach (ConnectionServer server in Servers)
                server.Stop();
            foreach (ConnectionClient client in Clients)
                client.Disconnect();
        }

        /// <summary>
        /// Creates instance of TCP server on port
        /// </summary>
        public ConnectionServer CreateTCPServer(ushort port)
        {
            var newServer = new ConnectionServer(port, this);
            Servers.Add(newServer);
            return newServer;
        }
        /// <summary>
        /// Creates instance of TCP client that will connect to specific host:port
        /// </summary>
        public ConnectionClient CreateTCPClient(string host, ushort port)
        {
            var newClient = new ConnectionClient(host, port, this);
            Clients.Add(newClient);
            return newClient;
        }
        /// <summary>
        /// Creates instance of UDP server on port
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public ConnectionServer CreateUDPServer(ushort port)
        {
            // TODO
            return null;
        }
        /// <summary>
        /// Creates instance of UDP client that will connect to specific host:port
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public ConnectionClient CreateUDPClient(string host, ushort port)
        {
            // TODO
            return null;
        }
        /// <summary>
        /// Creates MySQL DB connector
        /// </summary>
        /// <param name="dbHost"></param>
        /// <param name="dbLogin"></param>
        /// <param name="dbPassword"></param>
        /// <param name="dbDatabase"></param>
        public void CreateDBConnection(string dbHost, string dbLogin, string dbPassword, string dbDatabase)
        {
            DBConnector = new DatabaseConnector.DatabaseConnector(dbHost, dbLogin, dbPassword, dbDatabase);
        }

        protected MethodInfo GetMethodByName(String methodName)
        {
            MethodInfo mInfo = ChildClassType.GetMethod(methodName, new Type[] { typeof(PacketData) });
            return mInfo;
        }
        public PacketFunctionsDictionary GetPacketFunctionsDictionary()
        {
            return PacketFunctionsDictionary;
        }
        /// <summary>
        /// Invokes method in child class for packet handling
        /// </summary>
        /// <param name="client"></param>
        /// <param name="packetID"></param>
        /// <param name="packetData"></param>
        /// <returns></returns>
        public virtual bool InvokeIncomingClientsFunction(Client client, PacketID packetID, PacketData packetData)
        {
            var functionToCall = GetPacketFunctionsDictionary().GetFunction(packetID);
            if (functionToCall != null)
            {
                Console.WriteLine("InvokeIncomingClientsFunction - PacketFunction:"+functionToCall.Name);
                MethodInfo method = GetMethodByName(functionToCall.Name);
                if (method != null)
                {
                    object result = null;
                    object[] parametersArray = new object[] { packetData };
                    result = method.Invoke(this, parametersArray);
                }
                else
                    Console.WriteLine("InvokeIncomingClientsFunction - Unhandled packet: Assigned method was not found in the application.");
            }
            else
                Console.WriteLine("InvokeIncomingClientsFunction - Unhandled packet: No method was assigned to handle the packet.");
            return true;
        }
    }
}
