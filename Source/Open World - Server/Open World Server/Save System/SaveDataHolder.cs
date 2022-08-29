using System.Collections.Generic;

namespace Open_World_Server
{
    [System.Serializable]
    public class MainDataHolder
    {
        public ServerClient serverclient;

        public MainDataHolder(ServerClient clientToSave)
        {
            serverclient = clientToSave;
        }
    }

    [System.Serializable]
    public class BanDataHolder
    {
        public Dictionary<string, string> bannedIPs = new Dictionary<string, string>();

        public BanDataHolder(Dictionary<string, string> bannedIPs)
        {
            this.bannedIPs = bannedIPs;
        }
    }
}
