using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace OpenWorldServer
{
    public static class FactionSiloHandler
    {
        public static string GetSiloContents(Faction faction, string siloTileID)
        {
            int structureTile = int.Parse(siloTileID);

            FactionSilo siloToFech = (FactionSilo)faction.factionStructures.Find(fetch => fetch is FactionSilo
            && fetch.structureTile == structureTile);

            string dataToSend = "FactionManagement│Silo│Contents│";

            if (siloToFech == null) return dataToSend;

            if (siloToFech.holdingItems == null) siloToFech.holdingItems = new Dictionary<int, List<string>>();
            foreach (KeyValuePair<int, List<string>> pair in siloToFech.holdingItems)
            {
                dataToSend += pair.Value[0] + ":";
                dataToSend += pair.Value[1] + ":";
                dataToSend += pair.Value[2] + ":";
                dataToSend += pair.Value[3] + "»";
            }

            return dataToSend;
        }

        public static void DepositIntoSilo(Faction faction, string siloTileID, string items)
        {
            int structureTile = int.Parse(siloTileID);

            FactionSilo siloToFech = (FactionSilo)faction.factionStructures.Find(fetch => fetch is FactionSilo
            && fetch.structureTile == structureTile);

            if (siloToFech == null) return;

            int newDictionaryCount = 0;
            Dictionary<int, List<string>> a = new Dictionary<int, List<string>>();
            foreach (KeyValuePair<int, List<string>> pair in siloToFech.holdingItems)
            {
                a.Add(newDictionaryCount, pair.Value);
                newDictionaryCount++;
            }

            siloToFech.holdingItems = a;

            string[] builtItems = items.Split('»');
            int siloItemsCount = siloToFech.holdingItems.Count;
            foreach (string str in builtItems)
            {
                if (string.IsNullOrWhiteSpace(str)) continue;

                string itemID = "Silver";
                if (!string.IsNullOrWhiteSpace(str.Split('┼')[0])) itemID = str.Split('┼')[0];

                int itemCount = 0;
                if (!string.IsNullOrWhiteSpace(str.Split('┼')[1])) itemCount = int.Parse(str.Split('┼')[1]);

                int itemQuality = 0;
                if (!string.IsNullOrWhiteSpace(str.Split('┼')[2])) itemQuality = int.Parse(str.Split('┼')[2]);

                string itemMaterial = "0";
                if (!string.IsNullOrWhiteSpace(str.Split('┼')[3])) itemMaterial = str.Split('┼')[3];

                List<string> itemValues = new List<string>();
                itemValues.Add(itemID);
                itemValues.Add(itemCount.ToString());
                itemValues.Add(itemQuality.ToString());
                itemValues.Add(itemMaterial);

                if (siloToFech.holdingItems == null) siloToFech.holdingItems = new Dictionary<int, List<string>>();
                siloToFech.holdingItems.Add(siloItemsCount, itemValues);
                siloItemsCount++;
            }

            var orderedDictionary = siloToFech.holdingItems.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            siloToFech.holdingItems = orderedDictionary;

            FactionHandler.SaveFaction(faction);

            ServerClient[] dummyfactionMembers = faction.members.Keys.ToArray();
            foreach (ServerClient dummy in dummyfactionMembers)
            {
                ServerClient connected = Networking.connectedClients.Find(fetch => fetch.username == dummy.username);
                if (connected != null)
                {
                    Networking.SendData(connected, GetSiloContents(faction, siloTileID));
                }
            }
        }

        public static void WithdrawFromSilo(Faction faction, string siloTileID, string siloItemID, ServerClient client)
        {
            int structureTile = int.Parse(siloTileID);

            int itemID = int.Parse(siloItemID);

            FactionSilo siloToFech = (FactionSilo)faction.factionStructures.Find(fetch => fetch is FactionSilo
                        && fetch.structureTile == structureTile);

            if (siloToFech == null) return;

            foreach(KeyValuePair<int, List<string>> pair in siloToFech.holdingItems)
            {
                if (pair.Key == itemID)
                {
                    string dataToSend = "FactionManagement│Silo│Withdraw│";
                    dataToSend += pair.Value[0] + ":";
                    dataToSend += pair.Value[1] + ":";
                    dataToSend += pair.Value[2] + ":";
                    dataToSend += pair.Value[3];

                    Networking.SendData(client, dataToSend);
                    break;
                }
            }

            siloToFech.holdingItems.Remove(itemID);

            int newDictionaryCount = 0;
            Dictionary<int, List<string>> orderedDictionary = new Dictionary<int, List<string>>();
            foreach (KeyValuePair<int, List<string>> pair in siloToFech.holdingItems)
            {
                orderedDictionary.Add(newDictionaryCount, pair.Value);
                newDictionaryCount++;
            }

            siloToFech.holdingItems = orderedDictionary;

            FactionHandler.SaveFaction(faction);

            ServerClient[] dummyfactionMembers = faction.members.Keys.ToArray();
            foreach (ServerClient dummy in dummyfactionMembers)
            {
                ServerClient connected = Networking.connectedClients.Find(fetch => fetch.username == dummy.username);
                if (connected != null)
                {
                    Networking.SendData(connected, GetSiloContents(faction, siloTileID));
                }
            }
        }
    }
}
