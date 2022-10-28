using System;
using System.Collections.Generic;
using System.Text;

namespace OpenWorldServer
{
    [System.Serializable]
    public class Faction
    {
        public string name = "";
        public int wealth = 0;
        public Dictionary<ServerClient, FactionHandler.MemberRank> members = new Dictionary<ServerClient, FactionHandler.MemberRank>();
        public List<FactionStructure> factionStructures = new List<FactionStructure>();
    }
}