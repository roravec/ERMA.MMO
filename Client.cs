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
    /// Universal Client parent - this class provides basic funcionality for connection clients
    /// It has ConnectionHandler, Sandbox connector, event for DataReceived, Send() nethod for simple sending and it also implements packet processing method. 
    /// </summary>
    /// <example>
    /// You should use this as parent for you own class because it does not provide funcionality for communication or you can provide your own ConnectionHandler to this class.
    /// Use either <ref cref="ConnectionClient">ConnectionClient</ref> for Client to server connection or use 
    /// </example>
    public class Client : IDisposable
    {
        /// <summary>
        /// Connaction handler that provides basic events and funcionality for socket communication
        /// </summary>
        public ConnectionHandler Handler;
        /// <summary>
        /// Connector to Sandbox which will provide packet action sorting
        /// </summary>
        protected ISandboxClientConnector SandboxConnector;
        /// <summary>
        /// Connection socket protocol used 
        /// </summary>
        public readonly ConnectionProtocol Protocol = ConnectionProtocol.TCP;
        /// <summary>
        /// If connection was already established provide ConnectionHandler of this connection
        /// </summary>
        /// <param name="connHandler"></param>
        /// <param name="iSandboxConnector"></param>
        /// <param name="protocol">Default is TCP, you can choose between TCP and UDP</param>
        public Client(ConnectionHandler connHandler, ISandboxClientConnector iSandboxConnector, ConnectionProtocol protocol = ConnectionProtocol.TCP)
        {
            Protocol = protocol;
            Handler = connHandler;
            SandboxConnector = iSandboxConnector;
            Handler.eventDataReceived += new Connectivity.ConnectionHandler.delDataReceived(DataReceived);
            //Handler.event += new Connection.ConnectionHandler.delConnectionStatusChanged(ConnectionStatusChanged);
        }
        /// <summary>
        /// Use only from inherited object and dont forget to set handler in constructor!
        /// </summary>
        /// <param name="iSandboxConnector"></param>
        /// <param name="protocol">Default is TCP, you can choose between TCP and UDP</param>
        public Client(ISandboxClientConnector iSandboxConnector, ConnectionProtocol protocol=ConnectionProtocol.TCP)
        {
            Protocol = protocol;
            SandboxConnector = iSandboxConnector;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        protected void SetHandler(ConnectionHandler handler)
        {
            Handler = handler;
        }
        /// <summary>
        /// Client has received data - eventDataReceived will call this method
        /// </summary>
        /// <param name="data"></param>
        protected void DataReceived(RawPacket data)
        {
            while (Handler.HavePacketsToHandle())
            {
                Packet packetToHandle = Handler.GetPacket();
                Console.WriteLine(packetToHandle.Data.RawData);
                PacketProcessAsync((int)packetToHandle.PacketID, packetToHandle.Data);
            }
            Console.WriteLine("Client: Data Received.");
        }
        /// <summary>
        /// Do something when connection status was changed
        /// </summary>
        /// <param name="status"></param>
        protected void ConnectionStatusChanged(ConnectionStatus status)
        {
        }
        /// <summary>
        /// Send Packet to this remote client
        /// </summary>
        /// <param name="packet"></param>
        public void Send(Packet packet)
        {
            if (Handler != null)
            {
                Handler.SendData(packet.RawPacket);
            }
        }
        private async void PacketProcessAsync(int packetID, PacketData data)
        {
            if (data != null)
            {
                await Task.Run(() =>
                {
                    this.PacketProcess(packetID, data);
                });
            }
        }
        private void PacketProcess(int packetID, PacketData data)
        {
            if (data != null)
            {
                this.SandboxConnector.InvokeIncomingClientsFunction(this, packetID, data);
            }
        }
        /// <summary>
        /// Call after client was disconnected to do cleaning after him
        /// It is called from Dispose() !
        /// </summary>
        private void Destroy()
        {
        }
        /// <summary>
        /// Implemented from IDisposable - will do clean up after object's destruction
        /// </summary>
        public void Dispose()
        {
            Destroy();
        }
    }
}
