using Connectivity;
using DatabaseConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERMA.MMO
{
    public enum PacketFunctionDictionaryType {MLHADATASERVER = 0, MLHAWORLDSERVER, MLHACLIENT };

    public class PacketFunctionsDictionary
    {
        public PacketFunctionsDictionary(PacketFunctionDictionaryType inDictionaryID, IGenericDatabaseConnector dbConnector)
        {
            DictionaryID = inDictionaryID;
            DBConnector = dbConnector;
        }

        /// <summary>
        /// CRITICAL - Specifies what functions will be used in this application
        /// </summary>
        protected readonly PacketFunctionDictionaryType DictionaryID = PacketFunctionDictionaryType.MLHADATASERVER;
        protected readonly IGenericDatabaseConnector DBConnector = null;
        protected List<PacketFunction> PacketFunctions = new List<PacketFunction>();

        public void AddNewFunction(PacketFunction packetFunction)
        {
            PacketFunctions.Add(packetFunction);
        }
        /// <summary>
        /// Updates actual function dictionary from database
        /// </summary>
        public void UpdateDictionaryFromDatabase()
        {
            // todo
        }
        public PacketFunction GetFunction(PacketID packetID)
        {
            return GetFunction((ushort)packetID);
        }
        public PacketFunction GetFunction(ushort packetID)
        {
            Console.WriteLine("Looking for function. PacketID:"+packetID);
            PacketFunction pf = PacketFunctions.FirstOrDefault(cl => cl.PacketID == packetID);
            return pf;
        }
    }
}

/*
 * Dictionary<int, Func<string, bool>>
This allows you to store functions that take a string parameter and return boolean.

dico[5] = foo => foo == "Bar";
Or if the function is not anonymous:

dico[5] = Foo;
where Foo is defined like this:

public bool Foo(string bar)
{
    ...
}
*/