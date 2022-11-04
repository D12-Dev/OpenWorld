using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace OpenWorld
{
    public static class FactionHandler
    {
        public enum MemberRank { Member, Moderator, Leader }

        public static void AddFactionStructureInWorld(string data)
        {
            int structureTile = int.Parse(data.Split('│')[2]);
            int structureType = int.Parse(data.Split('│')[3]);
            int factionValue = int.Parse(data.Split('│')[4]);

            string siteName = "";
            if (structureType == 0) siteName = "Resource Silo";
            else if (structureType == 1) siteName = "Marketplace";
            else if (structureType == 2) siteName = "Production Site";
            else if (structureType == 3) siteName = "Wonder Structure";
            else if (structureType == 4) siteName = "Bank";
            else if (structureType == 5) siteName = "Stable";
            else if (structureType == 6) siteName = "Courier Station";

            Faction factionToGet = null;
            if (factionValue == 0) factionToGet = Main._ParametersCache.onlineNeutralFaction;
            else if (factionValue == 1) factionToGet = Main._ParametersCache.onlineAllyFaction;
            else if (factionValue == 2) factionToGet = Main._ParametersCache.onlineEnemyFaction;

            Site newSite = SiteMaker.MakeSite(sitePart: SitePartDefOf.Outpost,
                                              tile: structureTile,
                                              threatPoints: 5000,
                                              faction: factionToGet);

            newSite.customLabel = siteName;

            Find.WorldObjects.Add(newSite);

            Main._ParametersCache.allFactionStructures.Add(structureTile, new List<int>() { structureType,
                factionValue });
        }

        public static void RemoveFactionStructureInWorld(string data)
        {
            int structureTile = int.Parse(data.Split('│')[2]);

            Site siteToRemove = Find.WorldObjects.Sites.Find(fetch => fetch.Tile == structureTile);

            Find.WorldObjects.Remove(siteToRemove);

            Main._ParametersCache.allFactionStructures.Remove(structureTile);
        }

        public static void FactionDetailsHandle(string data)
        {
            string factionName = data.Split('│')[2];
            if (string.IsNullOrWhiteSpace(factionName))
            {
                Main._ParametersCache.hasFaction = false;
                Main._ParametersCache.factionName = "";
                Main._ParametersCache.factionMembers.Clear();
            }

            else
            {
                Main._ParametersCache.hasFaction = true;
                Main._ParametersCache.factionName = factionName;

                Main._ParametersCache.factionMembers.Clear();
                string[] factionMembers = data.Split('│')[3].Split('»');
                foreach (string str in factionMembers)
                {
                    if (string.IsNullOrWhiteSpace(str)) continue;

                    string memberName = str.Split(':')[0];
                    int memberRank = int.Parse(str.Split(':')[1]);
                    Main._ParametersCache.factionMembers.Add(memberName, memberRank);
                }
            }
        }

        public static void GetFactionStructuresFromServer(string data)
        {
            data = data.Split('│')[1];

            string[] structures = data.Split('»');

            Main._ParametersCache.allFactionStructures.Clear();

            foreach (string structure in structures)
            {
                if (string.IsNullOrWhiteSpace(structure)) continue;

                int structureTile = int.Parse(structure.Split(':')[0]);
                int structureType = int.Parse(structure.Split(':')[1]);
                int structureFaction = int.Parse(structure.Split(':')[2]);

                Main._ParametersCache.allFactionStructures.Add(structureTile, new List<int>() { structureType,
                structureFaction });
            }
        }

        public static void ManageFactionStructuresInWorld()
        {
            List<Site> existingStructures = new List<Site>();
            List<Site> serverStructures = new List<Site>();

            //Get existing structures
            foreach (Site site in Find.WorldObjects.Sites)
            {
                if (Main._ParametersCache.allFactions.Contains(site.Faction))
                {
                    existingStructures.Add(site);
                }
            }

            //Get server structures
            foreach (KeyValuePair<int, List<int>> pair in Main._ParametersCache.allFactionStructures)
            {
                string siteName = "";
                if (pair.Value[0] == 0) siteName = "Resource Silo";
                else if (pair.Value[0] == 1) siteName = "Marketplace";
                else if (pair.Value[0] == 2) siteName = "Production Site";
                else if (pair.Value[0] == 3) siteName = "Wonder Structure";

                Faction factionToGet = null;
                if (pair.Value[1] == 0) factionToGet = Main._ParametersCache.onlineNeutralFaction;
                else if (pair.Value[1] == 1) factionToGet = Main._ParametersCache.onlineAllyFaction;
                else if (pair.Value[1] == 2) factionToGet = Main._ParametersCache.onlineEnemyFaction;

                Site newSite = SiteMaker.MakeSite(sitePart: SitePartDefOf.Outpost,
                                                  tile: pair.Key,
                                                  threatPoints: 5000,
                                                  faction: factionToGet);

                newSite.customLabel = siteName;
                serverStructures.Add(newSite);
            }

            //Remove existing structures
            foreach(Site site in existingStructures)
            {
                Site siteToRemove = Find.WorldObjects.Sites.Find(fetch => fetch.Tile == site.Tile);
                Find.WorldObjects.Remove(siteToRemove);
            }

            //Add server structures
            foreach(Site site in serverStructures)
            {
                Find.WorldObjects.Add(site);
            }
        }

        public static void FindOnlineFactionsInWorld()
        {
            List<Faction> factions = Find.FactionManager.AllFactions.ToList();
            Main._ParametersCache.onlineNeutralFaction = factions.Find(fetch => fetch.Name == "Open World Settlements Neutral");
            Main._ParametersCache.onlineAllyFaction = factions.Find(fetch => fetch.Name == "Open World Settlements Ally");
            Main._ParametersCache.onlineEnemyFaction = factions.Find(fetch => fetch.Name == "Open World Settlements Enemy");

            Main._ParametersCache.allFactions.Clear();
            Main._ParametersCache.allFactions.Add(Main._ParametersCache.onlineNeutralFaction);
            Main._ParametersCache.allFactions.Add(Main._ParametersCache.onlineAllyFaction);
            Main._ParametersCache.allFactions.Add(Main._ParametersCache.onlineEnemyFaction);
        }
    }
}
