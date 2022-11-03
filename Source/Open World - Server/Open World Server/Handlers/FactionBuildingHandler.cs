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

            Faction[] factions = Server.savedFactions.ToArray();
            foreach (Faction faction in factions)
            {
                if (client.faction == null) factionValue = 0;
                if (client.faction != null)
                {
                    if (client.faction == faction) factionValue = 1;
                    else factionValue = 2;
                }

                FactionStructure[] structures = faction.factionStructures.ToArray();
                foreach (FactionStructure structure in structures)
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

            if (!CheckForGlobalStructureCap(newStructureIntValue)) return;

            if (!CheckForStructureCap(faction, newStructureIntValue)) return;

            if (!CheckIfTileIsAvailableForStructure(faction, newStructureTile)) return;

            FactionStructure structureToBuild = null;
            if (newStructureIntValue == 0) structureToBuild = new FactionSilo(faction, newStructureTile);
            else if (newStructureIntValue == 1) structureToBuild = new FactionMarketplace(faction, newStructureTile);
            else if (newStructureIntValue == 2) structureToBuild = new FactionProductionSite(faction, newStructureTile);
            else if (newStructureIntValue == 3) structureToBuild = new FactionWonder(faction, newStructureTile);

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

        public static bool CheckForGlobalStructureCap(int structureType)
        {
            Faction[] serverFactions = Server.savedFactions.ToArray();

            if (structureType != 3) return true;
            else foreach (Faction serverFaction in serverFactions)
            {
                foreach (FactionStructure structure in serverFaction.factionStructures)
                {
                    if (structure.structureType == 3) return false;
                }
            }

            return true;
        }

        public static bool CheckForStructureCap(Faction faction, int structureType)
        {
            FactionStructure structureOfSameType = faction.factionStructures.Find(fetch => fetch.structureType == structureType);
            if (structureOfSameType != null)
            {
                if (structureOfSameType is FactionSilo) return false;
                else if (structureOfSameType is FactionProductionSite) return false;
                else if (structureOfSameType is FactionMarketplace) return true;
                else return false;
            }
            else return true;
        }

        public static bool CheckIfTileIsAvailableForStructure(Faction faction, int structureTile)
        {
            FactionStructure presentStructure = faction.factionStructures.Find(fetch => fetch.structureTile == structureTile);
            if (presentStructure != null) return false;
            else return true;
        }
    }
}
