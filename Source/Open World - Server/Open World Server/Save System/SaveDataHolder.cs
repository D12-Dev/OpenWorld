using System.Collections.Generic;

namespace OpenWorldServer
{
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
