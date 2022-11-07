using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;

namespace OpenWorldServer
{
    public static partial class JoinUtils
    {
        public static void ProcessLoginAttempt(ServerClient client, string data)
        {
            string[] clientData = data.Split('│');
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
        
        private static bool ValidateClient(ServerClient client, string clientVersion, string clientModData)
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
