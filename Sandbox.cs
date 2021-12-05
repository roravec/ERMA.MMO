using Connectivity;
using DatabaseConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
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
        /// <summary>
        /// 
        /// </summary>
        protected PacketActionDictionary PacketActionsDictionary;
        /// <summary>
        /// Database connector
        /// </summary>
        protected DatabaseConnector.DatabaseConnector DBConnector;
        /// <summary>
        /// All servers created under this sandbox
        /// </summary>
        protected List<ConnectionServer> Servers = new List<ConnectionServer>();
        /// <summary>
        /// All clients created under this sandbox
        /// </summary>
        protected List<ConnectionClient> Clients = new List<ConnectionClient>();
        /// <summary>
        /// InvokeIncomingClientsFunction will look for method in this class
        /// </summary>
        protected Type ChildClassType;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="packetFunctionDictionary"></param>
        /// <param name="childType"></param>
        public Sandbox(PacketActionDictionary packetFunctionDictionary, Type childType=null)
        {
            if (childType == null)
                ChildClassType = this.GetType();
            else
                ChildClassType = childType;
            SetPacketFunctionsDictionary(packetFunctionDictionary);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="packetFunctionDictionary"></param>
        public void SetPacketFunctionsDictionary(PacketActionDictionary packetFunctionDictionary)
        {
            PacketActionsDictionary = packetFunctionDictionary;
        }

        /// <summary>
        /// Starts all servers and clients
        /// </summary>
        public void Start()
        {
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
            foreach (ConnectionServer server in Servers)
                server.Stop();
            foreach (ConnectionClient client in Clients)
                client.Disconnect();
        }

        /// <summary>
        /// Creates instance of TCP server on port
        /// </summary>
        /// <param name="port"></param>
        /// <param name="protocol">Default is TCP</param>
        /// <returns></returns>
        public ConnectionServer CreateServer(ushort port, ConnectionProtocol protocol = ConnectionProtocol.TCP)
        {
            var newServer = new ConnectionServer(port, this);
            Servers.Add(newServer);
            return newServer;
        }
        /// <summary>
        /// Creates instance of TCP client that will connect to specific host:port
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="protocol">Default is TCP</param>
        /// <returns></returns>
        public ConnectionClient CreateClient(string host, ushort port, ConnectionProtocol protocol=ConnectionProtocol.TCP)
        {
            var newClient = new ConnectionClient(host, port, this);
            Clients.Add(newClient);
            return newClient;
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName"></param>
        /// <returns></returns>
        protected MethodInfo GetMethodByName(String methodName)
        {
            MethodInfo mInfo = ChildClassType.GetMethod(methodName, new Type[] { typeof(PacketData) });
            return mInfo;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public PacketActionDictionary GetPacketActionsDictionary()
        {
            return PacketActionsDictionary;
        }
        /// <summary>
        /// Invokes method in child class for packet handling
        /// </summary>
        /// <param name="client"></param>
        /// <param name="packetID"></param>
        /// <param name="packetData"></param>
        /// <returns></returns>
        public virtual bool InvokeIncomingClientsFunction(Client client, int packetID, PacketData packetData)
        {
            var dictionary = GetPacketActionsDictionary();
            if (dictionary == null)
            {
                Console.WriteLine("Sandbox: InvokeIncomingClientsFunction - Dictionary was not set.");
                return false;
            }
            var functionToCall = dictionary.GetFunction(packetID);
            if (functionToCall != null)
            {
                //Console.WriteLine("InvokeIncomingClientsFunction - PacketFunction:"+functionToCall.Name);
                MethodInfo method = GetMethodByName(functionToCall.Name);
                if (method != null)
                {
                    object result = null;
                    object[] parametersArray = new object[] { packetData };
                    result = method.Invoke(this, parametersArray);
                    return true;
                }
                else
                {
                    Console.WriteLine("Sandbox: InvokeIncomingClientsFunction - Unhandled packet: Assigned method was not found in the application.");
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Sandbox: InvokeIncomingClientsFunction - Unhandled packet: No method was assigned to handle the packet.");
                return false;
            }
        }

        /// <summary>
        /// Makes an object from json string. Mandatory to know a type of object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static T JsonDeserialize<T>(string jsonString)
        {
            try
            {
                var NameObject = JsonSerializer.Deserialize<T>(jsonString);
                return NameObject;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return default(T);
        }
        /// <summary>
        /// Makes a json string from object
        /// </summary>
        /// <param name="objectToSerialize"></param>
        /// <returns></returns>
        public static string JsonSerialize(object objectToSerialize)
        {
            string jsonString = "";
            try
            {
                jsonString = JsonSerializer.Serialize(objectToSerialize, objectToSerialize.GetType());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return jsonString;
        }
    }
}
