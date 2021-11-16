using System;
using IniParser;
using IniParser.Model;

namespace ERMA.MMO
{
    public static class Configuration
    {
        public static void ReadConfig()
        {
            try
            {
                var parser = new FileIniDataParser();
                IniData data = parser.ReadFile("config.ini");
                ConfigurationSetup.DBServer = data["Database"]["Server"];
                ConfigurationSetup.DBLogin = data["Database"]["Login"];
                ConfigurationSetup.DBPassword = data["Database"]["Password"];
                ConfigurationSetup.DBDatabase = data["Database"]["Database"];
                ushort.TryParse(data["Other"]["Port"], out ConfigurationSetup.Port);
            }
            catch (Exception e) { }
        }
        public static ConfigSet ConfigurationSetup = new ConfigSet();
    }

    public class ConfigSet
    {
        public ConfigSet() { }

        public ConfigSet(String host, String login, String password, String database, ushort listeningPort)
        {
            DBServer = host;
            DBLogin = login;
            DBPassword = password;
            DBDatabase = database;
            Port = listeningPort;
        }

        public String DBServer = "localhost";
        public String DBLogin = "root";
        public String DBPassword = "xax";
        public String DBDatabase = "cup2020";
        public ushort Port = 11985;
        public int TickTime = 1000;
    }
}
