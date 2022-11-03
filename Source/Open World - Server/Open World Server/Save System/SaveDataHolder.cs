using System.Collections.Generic;

namespace OpenWorldServer
{
    [System.Serializable]
    public class BanDataHolder
    {
        public Dictionary<string, string> BannedIPs { get; set; } = new Dictionary<string, string>();

        public BanDataHolder(Dictionary<string, string> bannedIPs)
        {
            BannedIPs = bannedIPs;
        }
    }
}
