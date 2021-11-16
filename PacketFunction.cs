using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERMA.MMO
{
    public class PacketFunction
    {
        public uint ID { get; set; }
        public ushort PacketID { get; set; }
        public short PacketDirection { get; set; }
        public String Name { get; set; }
        public ushort NumberOfParameters { get; set; }
    }
}
