using System;
using System.Collections.Generic;
using System.Text;
using Connectivity;


namespace ERMA.MMO
{
    public enum PacketID : ushort
    {
        CHAT = 1, WORLDINIT, CHARACTERINITST0, UNITAPPEARS, CPLAYERINFO, SUNITLOCATIONUPDATE, SUNITDISAPPEARS, CLOGOUT, CLOCATIONUPDATE
    }
    public static class Packets
    {
        public static int ExpectMessagesInPacket(PacketID packetID)
        {
            switch (packetID)
            {
                case PacketID.CHAT: return 1;
                case PacketID.WORLDINIT: return 0;
                case PacketID.CHARACTERINITST0: return 6;
                case PacketID.UNITAPPEARS: return 7;
                case PacketID.CPLAYERINFO: return 2;
                case PacketID.SUNITLOCATIONUPDATE: return 5;
                case PacketID.SUNITDISAPPEARS: return 2;
                case PacketID.CLOGOUT: return 1;
                case PacketID.CLOCATIONUPDATE: return 3;
                default: return 0;
            }
        }
        private static char SEPARATOR = Packet.DataSeparator;
        // PACKETS ---------------------------------------------------------------------------------------------
        public static Packet PacketTestMessage(string message)
        {
            Packet packet = new Packet((ushort)PacketID.CHAT, message);
            return packet;
        }
        public static Packet PacketUnitAppears(uint ID, int type, string name, int skinID, float x, float y, float z)
        {
            char separator = Packet.DataSeparator;
            string data = string.Format("{0}" + separator + "{1}" + separator + "{2}" + separator + "{3}" + separator + "{4}" + separator + "{5}" + separator + "{6}", ID, type, name, skinID, x, y, z);
            Packet packet = new Packet((ushort)PacketID.UNITAPPEARS, data);
            return packet;
        }
        public static Packet PacketCharacterInitStage0(uint ID, string name, int skinID, float x, float y, float z)
        {
            char separator = Packet.DataSeparator;
            string data = string.Format("{0}" + separator + "{1}" + separator + "{2}" + separator + "{3}" + separator + "{4}" + separator + "{5}", (int)ID, name, skinID, x, y, z);
            Packet packet = new Packet((ushort)PacketID.CHARACTERINITST0, data);
            return packet;
        }
        public static Packet PacketSUnitLocationUpdate(uint ID, int type, float x, float y, float z)
        {
            string data = string.Format("{0}" + SEPARATOR + "{1}" + SEPARATOR + "{2}" + SEPARATOR + "{3}" + SEPARATOR + "{4}", ID, type, x, y, z);
            Packet packet = new Packet((ushort)PacketID.SUNITLOCATIONUPDATE, data);
            return packet;
        }
        public static Packet PacketSUnitDisappers(uint ID, int type)
        {
            string data = string.Format("{0}" + SEPARATOR + "{1}", ID, type);
            Packet packet = new Packet((ushort)PacketID.SUNITDISAPPEARS, data);
            return packet;
        }


        // --------------------------------------------------------------------------------------------------
        public static PacketID PacketIDReceived(Packet packet)
        {
            if (packet != null)
            {
                ushort ID = packet.PacketID;
                return (PacketID)ID;
            }
            return 0;
        }
        public static bool MessagesInPacketOK(PacketID packetID, PacketData data)
        {
            if (data != null)
            {
                if (data.Count == Packets.ExpectMessagesInPacket(packetID))
                    return true;
                else if (Packets.ExpectMessagesInPacket(packetID) == 0)
                    return true;
            }
            return false;
        }
        public static int GetIntFromPacketData(PacketData data, int index)
        {
            if (data != null && data.Data.Count > index && index >= 0)
            {
                string sData = data.Data[index];
                int iData = 0;
                int.TryParse(sData, out iData);
                return iData;
            }
            return 0;
        }
        public static string GetStringFromPacketData(PacketData data, int index)
        {
            if (data != null && data.Data.Count > index && index >= 0)
            {
                string sData = data.Data[index];
                return sData;
            }
            return "";
        }
        public static float GetFloatFromPacketData(PacketData data, int index)
        {
            if (data != null && data.Data.Count > index && index >= 0)
            {
                string sData = data.Data[index];
                float iData = 0f;
                float.TryParse(sData, out iData);
                return iData;
            }
            return 0f;
        }
    }
}
