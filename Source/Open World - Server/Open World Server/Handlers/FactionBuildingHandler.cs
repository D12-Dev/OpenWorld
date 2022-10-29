using System;
using System.Collections.Generic;
using System.Text;

namespace OpenWorldServer
{
    public static class FactionBuildingHandler
    {
        public static string GetAllFactionStructures(ServerClient client)
        {
            string dataToSend = "FactionStructures│";

            int factionValue = 0;

            foreach (Faction faction in Server.savedFactions)
            {
                if (client.faction == null) factionValue = 0;
                if (client.faction != null)
                {
                    if (client.faction == faction) factionValue = 1;
                    else factionValue = 2;
                }

                foreach (FactionStructure structure in faction.factionStructures)
                {
                    dataToSend += structure.structureTile + ":" + structure.structureType + ":" + factionValue + "»";
                }
            }

            return dataToSend;
        }

        public static void BuildStructure(Faction faction, string tileID, string structureID)
        {
            int newStructureTile = int.Parse(tileID);
            int newStructureIntValue = int.Parse(structureID);

            FactionStructure structureOfSameType = faction.factionStructures.Find(fetch => fetch.structureType == newStructureIntValue);
            if (structureOfSameType != null) return;

            FactionStructure presentStructure = faction.factionStructures.Find(fetch => fetch.structureTile == newStructureTile);
            if (presentStructure != null) return;

            FactionStructure structureToBuild = null;
            if (newStructureIntValue == 0) structureToBuild = new FactionSilo(faction, newStructureTile);
            else if (newStructureIntValue == 1) structureToBuild = new FactionMarketplace(faction, newStructureTile);
            else if (newStructureIntValue == 2) structureToBuild = new FactionProductionSite(faction, newStructureTile);

            if (structureToBuild == null) return;

            faction.factionStructures.Add(structureToBuild);

            FactionHandler.SaveFaction(faction);

            int factionValue = 0;
            foreach (ServerClient client in Networking.connectedClients)
            {
                if (client.faction == null) factionValue = 0;
                if (client.faction != null)
                {
                    if (client.faction == faction) factionValue = 1;
                    else factionValue = 2;
                }

                Networking.SendData(client, "FactionStructureBuilder│AddStructure" + "│" + newStructureTile + "│" + newStructureIntValue + "│" + factionValue);
            }
        }

        public static void DestroyStructure(Faction faction, string tileID)
        {
            int structureTile = int.Parse(tileID);

            FactionStructure structureToDestroy = faction.factionStructures.Find(fetch => fetch.structureTile == structureTile);
            if (structureToDestroy == null) return;

            faction.factionStructures.Remove(structureToDestroy);

            FactionHandler.SaveFaction(faction);

            foreach (ServerClient client in Networking.connectedClients)
            {
                Networking.SendData(client, "FactionStructureBuilder│RemoveStructure" + "│" + structureTile);
            }
        }
    }
}
