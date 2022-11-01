using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace OpenWorldServer
{
    public static class FactionProductionSiteHandler
    {
        public static void TickProduction()
        {
            while (true)
            {
                Thread.Sleep(600000);

                Faction[] allFactions = Server.savedFactions.ToArray();
                foreach (Faction faction in allFactions)
                {
                    foreach (FactionStructure structure in faction.factionStructures)
                    {
                        if (structure is FactionProductionSite)
                        {
                            SendProductionToMembers(faction);
                            break;
                        }
                    }
                }
            }
        }

        public static void SendProductionToMembers(Faction faction)
        {
            ServerClient[] dummyfactionMembers = faction.members.Keys.ToArray();
            foreach (ServerClient dummy in dummyfactionMembers)
            {
                ServerClient connected = Networking.connectedClients.Find(fetch => fetch.username == dummy.username);
                if (connected != null)
                {
                    Networking.SendData(connected, "FactionManagement│ProductionSite│Tick");
                }
            }
        }
    }
}
