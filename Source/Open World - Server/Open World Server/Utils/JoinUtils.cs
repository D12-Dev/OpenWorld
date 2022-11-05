using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;

namespace OpenWorldServer
{
    public static class JoinUtils
    {
        public static void LoginProcedures(ServerClient client, string data)
        {
            string[] clientData = data.Split('|');
            client.username = clientData[1].ToLower();
            client.password = clientData[2];
            string playerVersion = clientData[3];
            string joinMode = clientData[4];
            string playerMods = clientData[5];

            if (ValidateClient(client, playerVersion, playerMods))
            { 
                if (!Server.savedClients.Any(x => x.username == client.username)) PlayerUtils.SaveNewPlayerFile(client.username, client.password);
                ConsoleUtils.UpdateTitle();
                ServerUtils.SendPlayerListToAll(client);
                if (joinMode == "NewGame")
                {
                    SendNewGameData(client);
                    ConsoleUtils.LogToConsole($"Player {client.username} has connected as a new player from the IP {((IPEndPoint)client.tcp.Client.RemoteEndPoint).Address}.");
                }
                else if (joinMode == "LoadGame")
                {
                    PlayerUtils.GiveSavedDataToPlayer(client);
                    SendLoadGameData(client);
                    ConsoleUtils.LogToConsole($"Player {client.username} has connected as a returning player from the IP {((IPEndPoint)client.tcp.Client.RemoteEndPoint).Address}.");
                }
                else ConsoleUtils.LogToConsole($"Player {client.username} Provided An Invalid Join Mode: {joinMode}.");
            }
        }
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
        private static bool ValidatePassword(ServerClient client)
        {
            ServerClient clientToFetch = Server.savedClients.Find(fetch => fetch.username == client.username);

            client.username = clientToFetch.username;

            if (clientToFetch.password != client.password)
            {

                return false;
            }

            return true;
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

        public static bool ValidateClient(ServerClient client, string clientVersion, string clientModData)
        {
            bool valid = true;
            string data = "";
            ServerClient otherConnection;
            // Password Check - If there's not a username/password combo that matches, but there is a username that matches.
            if (!Server.savedClients.Any(x => x.username == client.username && x.password == client.password) && Server.savedClients.Any(x => x.username == client.username))
            {
                data = "Disconnect│WrongPassword";
                valid = false;
                ConsoleUtils.LogToConsole($"Player {client.username} Tried To Join Using an Invalid Password.");

            }
            // Kick Them If They're Already Connected
            if ((otherConnection = Networking.connectedClients.Where(x => x.username == client.username && x != client).SingleOrDefault()) != null)
            {
                Networking.SendData(otherConnection, "Disconnect│AnotherLogin");
                otherConnection.disconnectFlag = true;
                ConsoleUtils.LogToConsole($"Player {client.username} Joined, Was Already Connected, And Their Existing Connection Was Terminated.");
            }
            // Whitelist Check - If the server is using a whitelist, the client isn't an admin and the user isn't on the whitelist.
            if (Server.usingWhitelist && !client.isAdmin && !Server.whitelistedUsernames.Contains(client.username))
            {
                data = "Disconnect│Whitelist";
                valid = false;
                ConsoleUtils.LogToConsole($"Player {client.username} Tried To Join But Is Not Whitelisted.");
            }
            // Banlist Check - If the user's IP or username are on the ban list.
            if (Server.bannedIPs.Any(x => x.Key == ((IPEndPoint)client.tcp.Client.RemoteEndPoint).Address.ToString() || x.Value == client.username))
            {
                data = "Disconnect│Banned";
                valid = false;
                ConsoleUtils.LogToConsole($"Player {client.username} Tried To Join But Is Banned.");
            }
            // Version Check - If there is a version specified on the server and the client doesn't match it.
            if (!string.IsNullOrWhiteSpace(Server.latestClientVersion) && clientVersion != Server.latestClientVersion)
            {
                data = "Disconnect│Version";
                valid = false;
                ConsoleUtils.LogToConsole($"Player {client.username} Tried To Join But Is Running A Different Version.");
            }
            // Client Limit Check - If the client isn't an admin and the server is full.
            if (!client.isAdmin && Networking.connectedClients.Count() >= Server.maxPlayers + 1)
            {
                data = "Disconnect│ServerFull";
                valid = false;
                ConsoleUtils.LogToConsole($"Player {client.username} Tried To Join But The Client Limit Is Reached.");
            }
            // Mods Check
            if (!client.isAdmin && Server.forceModlist)
            {
                string[] clientMods = clientModData.Split('»');
                List<string> clientMissing = Server.enforcedMods.Where(x => !clientMods.Contains(x)).ToList();
                List<string> clientBlacklisted = Server.blacklistedMods.Where(x => clientMods.Contains(x)).ToList();
                List<string> clientUnidentified = clientMods.Where(x => !Server.whitelistedMods.Concat(Server.blacklistedMods).Concat(Server.enforcedMods).Contains(x)).ToList();
                List<string> flagged = clientMissing.Concat(clientBlacklisted).Concat(clientUnidentified).ToList();
                if (flagged.Count > 0)
                {
                    data = $"Disconnect│WrongMods│{string.Join('»', flagged)}";
                    valid = false;
                    ConsoleUtils.LogToConsole($"Player {client.username} Tried To Join But Has A Mod Mismatch.");
                }
            }
            // Username Check - If the username is empty, or has invalid characters.
            if (string.IsNullOrWhiteSpace(client.username) || (!client.username.All(character => char.IsLetterOrDigit(character) || character == '_' || character == '-')))
            {
                data = "Disconnect│Corrupted";
                valid = false;
                ConsoleUtils.LogToConsole($"Player {client.username} Tried To Join But Their Username Data Is Invalid or Empty.");
            }
            if (!valid)
            {
                Networking.SendData(client, data);
                client.disconnectFlag = true;
            }
            return valid;
        }
    }
}
