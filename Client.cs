using Connectivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Connectivity.SocketClient;

namespace ERMA.MMO
{
    public class Client
    {
        public ConnectionHandler Handler;
        protected ISandboxClientConnector SandboxConnector;

        public Client(ConnectionHandler connHandler, ISandboxClientConnector iSandboxConnector)
        {
            Handler = connHandler;
            SandboxConnector = iSandboxConnector;
            Handler.eventDataReceived += new Connectivity.ConnectionHandler.delDataReceived(DataReceived);
            //Handler.event += new Connection.ConnectionHandler.delConnectionStatusChanged(ConnectionStatusChanged);
        }
        /// <summary>
        /// Use only from inherited object and dont forget to set handler in constructor!
        /// </summary>
        /// <param name="iSandboxConnector"></param>
        public Client(ISandboxClientConnector iSandboxConnector)
        {
            SandboxConnector = iSandboxConnector;
        }
        protected void SetHandler(ConnectionHandler handler)
        {
            Handler = handler;
        }
        protected void DataReceived(RawPacket data)
        {
            while (Handler.HavePacketsToHandle())
            {
                Packet packetToHandle = Handler.GetPacket();
                Console.WriteLine(packetToHandle.Data.RawData);
                PacketProcessAsync((PacketID)packetToHandle.PacketID, packetToHandle.Data);
            }
            Console.WriteLine("Client: Data Received.");
        }
        protected void ConnectionStatusChanged(ConnectionStatus status)
        {
        }
        public void Send(Packet packet)
        {
            if (Handler != null)
            {
                Handler.SendData(packet.RawPacket);
            }
        }
        private async void PacketProcessAsync(PacketID packetID, PacketData data)
        {
            if (data != null)
            {
                await Task.Run(() =>
                {
                    this.PacketProcess(packetID, data);
                });
            }
        }
        private void PacketProcess(PacketID packetID, PacketData data)
        {
            if (data != null)
            {
                this.SandboxConnector.InvokeIncomingClientsFunction(this, packetID, data);
            }
        }
        public void Destroy()
        {
        }
    }
}
