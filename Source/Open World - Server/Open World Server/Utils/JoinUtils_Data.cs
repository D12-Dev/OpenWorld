using System;
using System.Collections.Generic;
using System.Text;

namespace OpenWorldServer
{
    public static partial class JoinUtils
    {
        private static void SendNewGameData(ServerClient client)
        {
            //We give saved data back to return data that is not removed at new creation
            PlayerUtils.GiveSavedDataToPlayer(client);
            PlayerUtils.SaveNewPlayerFile(client.username, client.password);

            Networking.SendData(client, GetPlanetToSend());

            Networking.SendData(client, GetSettlementsToSend(client));

            Networking.SendData(client, GetVariablesToSend(client));

            ServerUtils.SendPlayerList(client);

            Networking.SendData(client, FactionHandler.GetFactionDetails(client));

            Networking.SendData(client, FactionBuildingHandler.GetAllFactionStructures(client));

            Networking.SendData(client, "NewGame│");
        }
        private static void SendLoadGameData(ServerClient client)
        {
            Networking.SendData(client, GetSettlementsToSend(client));

            Networking.SendData(client, GetVariablesToSend(client));

            ServerUtils.SendPlayerList(client);

            Networking.SendData(client, FactionHandler.GetFactionDetails(client));

            Networking.SendData(client, FactionBuildingHandler.GetAllFactionStructures(client));

            Networking.SendData(client, GetGiftsToSend(client));

            Networking.SendData(client, GetTradesToSend(client));

            Networking.SendData(client, "LoadGame│");
        }
        public static string GetPlanetToSend()
        {
            string dataToSend = "Planet│";

            float mmGC = Server.globeCoverage;
            string mmS = Server.seed;
            int mmOR = Server.overallRainfall;
            int mmOT = Server.overallTemperature;
            int mmOP = Server.overallPopulation;

            return dataToSend + mmGC + "│" + mmS + "│" + mmOR + "│" + mmOT + "│" + mmOP;
        }

        public static string GetSettlementsToSend(ServerClient client)
        {
            string dataToSend = "Settlements│";

            if (Server.savedSettlements.Count == 0) return dataToSend;

            else
            {
                Dictionary<string, List<string>> settlements = Server.savedSettlements;
                foreach (KeyValuePair<string, List<string>> pair in settlements)
                {
                    if (pair.Value[0] == client.username) continue;

                    int factionValue = 0;

                    ServerClient clientToCompare = Server.savedClients.Find(fetch => fetch.username == pair.Value[0]);
                    if (client.faction == null) factionValue = 0;
                    if (clientToCompare.faction == null) factionValue = 0;
                    else if (client.faction != null && clientToCompare.faction != null)
                    {
                        if (client.faction.name == clientToCompare.faction.name) factionValue = 1;
                        else factionValue = 2;
                    }

                    dataToSend += pair.Key + ":" + pair.Value[0] + ":" + factionValue + "│";
                }

                return dataToSend;
            }
        }

        public static string GetVariablesToSend(ServerClient client)
        {
            string dataToSend = "Variables│";

            if (Server.savedClients.Find(fetch => fetch.username == client.username) != null)
            {
                client.isAdmin = Server.savedClients.Find(fetch => fetch.username == client.username).isAdmin;
            }
            else client.isAdmin = false;

            int devInt = client.isAdmin || Server.allowDevMode ? 1 : 0;

            int wipeInt = client.toWipe ? 1 : 0;

            int roadInt = 0;
            if (Server.usingRoadSystem) roadInt = 1;
            if (Server.usingRoadSystem && Server.aggressiveRoadMode) roadInt = 2;

            int chatInt = Server.usingChat ? 1 : 0;

            int profanityInt = Server.usingProfanityFilter ? 1 : 0;

            int modVerifyInt = Server.usingModVerification ? 1 : 0;

            int enforcedDifficultyInt = Server.usingEnforcedDifficulty ? 1 : 0;

            string name = Server.serverName;

            return dataToSend + devInt + "│" + wipeInt + "│" + roadInt + "│" + chatInt + "│" + profanityInt + "│" + modVerifyInt + "│" + enforcedDifficultyInt + "│" + name;
        }

        public static string GetGiftsToSend(ServerClient client)
        {
            string dataToSend = "GiftedItems│";

            if (client.giftString.Count == 0) return dataToSend;

            else
            {
                string giftsToSend = "";

                foreach (string str in client.giftString) giftsToSend += str + "»";

                dataToSend += giftsToSend;

                client.giftString.Clear();

                return dataToSend;
            }
        }

        public static string GetTradesToSend(ServerClient client)
        {
            string dataToSend = "TradedItems│";

            if (client.tradeString.Count == 0) return dataToSend;

            else
            {
                string tradesToSend = "";

                foreach (string str in client.tradeString) tradesToSend += str + "»";

                dataToSend += tradesToSend;

                client.tradeString.Clear();

                return dataToSend;
            }
        }

    }
}
