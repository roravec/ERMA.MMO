using Connectivity;
using DatabaseConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERMA.MMO
{
    /// <summary>
    /// Dictionary of packet actions - provides methods for reading packets actions from database
    /// Normally you should create instance od PacketActionDictionary with DictionaryID that is same as application you are building.
    /// Dictionary will then read all posiible packets for this communication from DB and it will be able to provide "name of packet action" to sandbox - wchich will run the action.
    /// </summary>
    public class PacketActionDictionary
    {
        /// <summary>
        /// Create instance with already established access to database.
        /// It will automatically update dictionary from database.
        /// </summary>
        /// <param name="iPacketFunctionDictionaryID"></param>
        /// <param name="dbConnector"></param>
        public PacketActionDictionary(short iPacketFunctionDictionaryID, IGenericDatabaseConnector dbConnector)
        {
            DictionaryID = iPacketFunctionDictionaryID;
            DBConnector = dbConnector;
            UpdateDictionaryFromDatabase();
        }
        /// <summary>
        /// Create instance without access to database.
        /// You have to provide dictionary manually or you can add packet actions manually by method AddNewAction
        /// </summary>
        /// <param name="iPacketFunctionDictionaryID"></param>
        public PacketActionDictionary(short iPacketFunctionDictionaryID)
        {
            DictionaryID = iPacketFunctionDictionaryID;
        }

        /// <summary>
        /// CRITICAL - Specifies what functions will be used in this application
        /// </summary>
        protected readonly short DictionaryID;
        /// <summary>
        /// Database connector
        /// </summary>
        protected readonly IGenericDatabaseConnector DBConnector = null;
        /// <summary>
        /// List of all actions of current dictionary
        /// </summary>
        protected List<PacketAction> PacketActions = new List<PacketAction>();

        /// <summary>
        /// Add a new action to current dictionary manually
        /// </summary>
        /// <param name="packetFunction"></param>
        public void AddNewAction(PacketAction packetFunction)
        {
            PacketActions.Add(packetFunction);
        }
        /// <summary>
        /// Updates actual function dictionary from database
        /// </summary>
        public void UpdateDictionaryFromDatabase()
        {
            // todo
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="packetID"></param>
        /// <returns>null if packetID is not in the list</returns>
        public PacketAction GetFunction(int packetID)
        {
            //Console.WriteLine("Looking for function. PacketID:"+packetID);
            PacketAction packetAction = null;
            try
            {
                packetAction = PacketActions.FirstOrDefault(cl => cl.PacketID == packetID);
            }
            catch (Exception e)
            {
                Console.WriteLine("Packet Action Dictionary: "+e.Message);
            }
            return packetAction;
        }
    }
}