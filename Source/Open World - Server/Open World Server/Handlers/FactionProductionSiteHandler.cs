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
                    FactionStructure productionSiteToFind = faction.factionStructures.Find(fetch => fetch is FactionProductionSite);

                    if (productionSiteToFind == null) continue;
                    else SendProductionToMembers(faction);
                }

                ConsoleUtils.LogToConsole("[Factions Production Site Tick]");
            }
        }

        public static void SendProductionToMembers(Faction faction)
        {
            ServerClient[] dummyfactionMembers = faction.members.Keys.ToArray();
            int productionSitesInFaction = faction.factionStructures.FindAll(fetch => fetch is FactionProductionSite).Count();

            foreach (ServerClient dummy in dummyfactionMembers)
            {
                ServerClient connected = Networking.connectedClients.Find(fetch => fetch.username == dummy.username);
                if (connected != null)
                {
                    Networking.SendData(connected, "FactionManagement│ProductionSite│Tick" + "│" + productionSitesInFaction);
                }
            }
        }
    }
}
