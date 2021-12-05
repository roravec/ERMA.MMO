using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERMA.MMO
{
    /// <summary>
    /// Class for storing database object od PacketAction
    /// Provides just basic informations about Packets like PacketID for identification of a packet, his Name, direction of a communication 
    /// and mainly the name of a Method that is called when packet will come to processing
    /// </summary>
    public class PacketAction
    {
        /// <summary>
        /// ID of packet id database
        /// </summary>
        public uint ID { get; set; }
        /// <summary>
        /// Actual Packet ID
        /// </summary>
        public ushort PacketID { get; set; }
        /// <summary>
        /// Direction of packet - probably will not be used
        /// </summary>
        public short PacketDirection { get; set; } // TODO - use this?
        /// <summary>
        /// Name of a method (packet action)
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// Description of packet funcionality
        /// </summary>
        public String Description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ushort NumberOfParameters { get; set; }
    }
}
