using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace Open_World_Server
{
    public class CommandHandler
    {
        private const ConsoleColor defaultColor = ConsoleColor.White,
          warnColor = ConsoleColor.Yellow,
          errorColor = ConsoleColor.Red,
          messageColor = ConsoleColor.Green;
        public void Help()
        {
            ServerUtils.WriteServerLog("Command Library", messageColor);
            ServerUtils.WriteServerLog("Help - Displays Help Menu\n" +
                "Settings - Displays Settings Menu\n" +
                "Reload - Reloads All Available Settings Into The Server\n" +
                "Status - Shows A General Overview Menu\n" +
                "Settlements - Displays Settlements Menu\n" +
                "List - Displays Player List Menu\n" +
                "Whitelist - Shows All Whitelisted Players\n" +
                "Clear - Clears The Console\n" +
                "Exit - Closes The Server\n");

            ServerUtils.WriteServerLog("Communication:", messageColor);
            ServerUtils.WriteServerLog("Say - Send A Chat Message\n" +
                "NotifyAll - Send A Letter To Every Player Connected\n" +
                "Notify - Send A Letter To X Player\n" +
                "Chat - Displays Chat Menu\n");

            ServerUtils.WriteServerLog("Interaction:", messageColor);
            ServerUtils.WriteServerLog("Invoke - Invokes An Event To X Player\n" +
                "Plague - Invokes An Event To All Connected Players\n" +
                "Eventlist - Shows All Available Events\n" +
                "GiveItem - Gives An Item To X Player\n" +
                "GiveItemAll - Gives An Item To All Players\n" +
                "Protect - Protects A Player From Any Event Temporarily\n" +
                "Deprotect - Disables All Protections Given To X Player\n" +
                "Immunize - Protects A Player From Any Event Permanently\n" +
                "Deimmunize - Disables The Immunity Given To X Player\n");

            ServerUtils.WriteServerLog("Admin Control:", messageColor);
            ServerUtils.WriteServerLog("Investigate - Displays All Data About X Player\n" +
                "Promote - Promotes X Player To Admin\n" +
                "Demote - Demotes X Player\n" +
                "Adminlist - Shows All Server Admins\n" +
                "Kick - Kicks X Player\n" +
                "Ban - Bans X Player\n" +
                "Pardon - Pardons X Player\n" +
                "Banlist - Shows All Banned Players\n" +
                "Wipe - Deletes Every Player Data In The Server\n");
        }
        // TODO: Parameterize the methods so the whole command doesn't have to be passed in, it should be caught when the predicates are called.
        public void Say(string[] args)
        {
            // Args is expected to be split on spaces, but since the say command takes multiple words as a string, we'll glue them back together.
            string message = String.Join(' ', args);
            // If the result is whitespace, they didn't give us a message.
            if (string.IsNullOrWhiteSpace(message)) ServerUtils.WriteServerLog("Missing Parameter 'message'\nCorrect Usage: \"say [message]\" where [message] is one or more characters, including spaces.\n", warnColor);
            // If it's not, handle the command.
            else
            {
                string messageForConsole = "Chat - [Console] " + message;

                ServerUtils.WriteServerLog(messageForConsole);

                OWServer.chatCache.Add("[" + DateTime.Now + "]" + " │ " + messageForConsole);


                // TODO: This should really be a static broadcast method in Networking rather than a foreach everywhere we send to all clients.
                foreach (ServerClient sc in OWServer._Networking.connectedClients)
                {
                    try
                    {
                        OWServer._Networking.SendData(sc, "ChatMessage│SERVER│" + message);
                    }
                    catch
                    {
                        ServerUtils.WriteServerLog($"Failed to send chat data to client {sc.username}.", errorColor);
                    }
                }
            }
        }
        public void NotifyAll(string[] args)
        {
            // Args is expected to be split on spaces, but since the say command takes multiple words as a string, we'll glue them back together.
            string message = String.Join(' ', args);
            // If the result is whitespace, they didn't give us a message.
            if (string.IsNullOrWhiteSpace(message)) ServerUtils.WriteServerLog("Missing Parameter 'message'\nCorrect Usage: \"notifyall [message]\" where [message] is one or more characters, including spaces.\n", warnColor);
            // If it's not, handle the command.
            else
            {
                foreach (ServerClient sc in OWServer._Networking.connectedClients)
                {
                    try
                    {
                        // TODO: These pefixes should really be an enum parameter.
                        OWServer._Networking.SendData(sc, "Notification│" + message);
                    }
                    catch
                    {
                        ServerUtils.WriteServerLog($"Failed to send notification data to client {sc.username}.", errorColor);
                    }
                }
                ServerUtils.WriteServerLog("Notification sent to all connected players.\n", messageColor);
            }
        }
        public void Notify(string[] args)
        {
            string target = "", message = "";

            if (args.Length < 2 || string.IsNullOrWhiteSpace(target = args[0]) || string.IsNullOrWhiteSpace(message = String.Join(' ', args.TakeLast(args.Length - 1).ToArray()))) ServerUtils.WriteServerLog("Missing Parameter(s)\nCorrect Usage: \"notify [target] [message]\" where [target] is a player's username and [message] is one or more characters, including spaces.\n", warnColor);
            else
            {
                ServerClient targetClient;
                if ((targetClient = OWServer._Networking.connectedClients.Where(x => x.username == target).SingleOrDefault()) == null) ServerUtils.WriteServerLog($"Player {target} was not found.\n", warnColor);
                else
                {
                    OWServer._Networking.SendData(targetClient, "Notification│" + message);
                    ServerUtils.WriteServerLog($"Notification sent to player {targetClient.username}.\n", messageColor);
                }
            }
            
        }
        public void Settings()
        {
            ServerUtils.WriteServerLog("Server Settings:", messageColor);
            ServerUtils.WriteServerLog($"Server Name: {OWServer.serverName}\nServer Description: {OWServer.serverDescription}\nServer Local IP: {OWServer._Networking.localAddress}\nServer Port: {OWServer._Networking.serverPort}\n");

            ServerUtils.WriteServerLog("World Settings:", messageColor);
            ServerUtils.WriteServerLog($"Globe Coverage: {OWServer.globeCoverage}\nSeed: {OWServer.seed}\nOverall Rainfall: {OWServer.overallRainfall}\nOverall Temperature: {OWServer.overallTemperature}\nOverall Population: {OWServer.overallPopulation}\n");

            ServerUtils.WriteServerLog($"Server Mods: [{OWServer.modList.Count}]", messageColor);
            ServerUtils.WriteServerLog(OWServer.modList.Count == 0 ? "No Mods Found\n" : string.Join("\n", OWServer.modList.ToArray()) + "\n");

            ServerUtils.WriteServerLog($"Server WhiteListed Mods: [{OWServer.modList.Count}]", messageColor);
            ServerUtils.WriteServerLog(OWServer.whitelistedMods.Count == 0 ? "No Mods Found\n" : string.Join("\n", OWServer.whitelistedMods.ToArray()) + "\n");
        }
        public void Reload()
        {
            ServerUtils.WriteServerLog("Reloading All Current Mods", messageColor);
            OWServer._ServerUtils.CheckMods();
            OWServer._ServerUtils.CheckWhitelistedMods();
            ServerUtils.WriteServerLog("Mods Have Been Reloaded", messageColor);
            ServerUtils.WriteServerLog("Reloading All Whitelisted Players", messageColor);
            OWServer._ServerUtils.CheckForWhitelistedPlayers();
            ServerUtils.WriteServerLog("Whitelisted Players Have Been Reloaded", messageColor);
        }
        public void Status()
        {
            ServerUtils.WriteServerLog("Server Status", messageColor);
            ServerUtils.WriteServerLog($"Version: {OWServer.serverVersion}\n" +
                "Connection: Online\n" +
                $"Uptime: [{DateTime.UtcNow - Process.GetCurrentProcess().StartTime.ToUniversalTime()}]\n");

            ServerUtils.WriteServerLog("Mods:", messageColor);
            ServerUtils.WriteServerLog($"Mods: {OWServer.modList.Count()}\n" +
                $"WhiteListed Mods: {OWServer.whitelistedMods.Count()}\n");

            ServerUtils.WriteServerLog("Players", messageColor);
            ServerUtils.WriteServerLog($"Connected Players: {OWServer._Networking.connectedClients.Count()}\n" +
                $"Saved Players: {OWServer.savedClients.Count()}\n" +
                $"Saved Settlements: {OWServer.savedSettlements.Count()}\n" +
                $"Whitelisted Players: {OWServer.whitelistedUsernames.Count()}\n" +
                $"Max Players: {OWServer.maxPlayers}\n");

            ServerUtils.WriteServerLog("Modlist Settings", messageColor);
            ServerUtils.WriteServerLog($"Using Modlist Check: {OWServer.forceModlist}\n" +
                $"Using Modlist Config Check: {OWServer.forceModlistConfigs}\n" +
                $"Using Mod Verification: {OWServer.usingModVerification}\n");

            ServerUtils.WriteServerLog("Chat Settings", messageColor);
            ServerUtils.WriteServerLog($"Using Chat: {OWServer.usingChat}\n" +
                $"Using Profanity Filter: {OWServer.usingProfanityFilter}\n");

            ServerUtils.WriteServerLog("Wealth Settings", messageColor);
            ServerUtils.WriteServerLog($"Using Wealth System: {OWServer.usingWealthSystem}\n" +
                $"Warning Threshold: {OWServer.warningWealthThreshold}\n" +
                $"Ban Threshold: {OWServer.banWealthThreshold}\n");

            ServerUtils.WriteServerLog("Idle Settings", messageColor);
            ServerUtils.WriteServerLog($"Using Idle System: {OWServer.usingIdleTimer}\n" +
                $"Idle Threshold: {OWServer.idleTimer}\n");

            ServerUtils.WriteServerLog("Road Settings", messageColor);
            ServerUtils.WriteServerLog($"Using Road System: {OWServer.usingRoadSystem}\n" +
                $"Aggressive Road Mode: {OWServer.aggressiveRoadMode}\n");

            ServerUtils.WriteServerLog("Miscellaneous Settings", messageColor);
            ServerUtils.WriteServerLog($"Using Whitelist: {OWServer.usingWhitelist}\n" +
                $"Allow Dev Mode: {OWServer.allowDevMode}\n");
        }
        public void Invoke(string command)
        {
            string clientID = "";
            string eventID = "";
            ServerClient target = null;
            try
            {
                clientID = command.Split(' ')[1];
                eventID = command.Split(' ')[2];
            }
            catch
            {
                ServerUtils.WriteServerLog("Missing Parameters\n", warnColor);
            }

            target = OWServer._Networking.connectedClients.Where(x => x.username == clientID).SingleOrDefault();
            if (target == null)
            {
                ServerUtils.WriteServerLog($"Player {clientID} Not Found\n", warnColor);
            }
            OWServer._Networking.SendData(target, "ForcedEvent│" + eventID);
            ServerUtils.WriteServerLog($"Sent Event [{eventID}] to [{clientID}]\n", messageColor);
        }
        public void Plague(string command)
        {
            string eventID = "";
            try { eventID = command.Split(' ')[1]; }
            catch
            {
                ServerUtils.WriteServerLog("Missing Parameters\n", warnColor);
            }
            foreach (ServerClient client in OWServer._Networking.connectedClients)
            {
                OWServer._Networking.SendData(client, "ForcedEvent│" + eventID);
            }
            ServerUtils.WriteServerLog($"Sent Event [{eventID}] to Every Player\n", messageColor);
        }
        public void EventList()
        {
            ServerUtils.WriteServerLog("List Of Available Events:", messageColor);
            ServerUtils.WriteServerLog("Raid\nInfestation\nMechCluster\nToxicFallout\nManhunter\nFarmAnimals\nShipChunk\nGiveQuest\nTraderCaravan\n");
        }
        public void Chat()
        {
            ServerUtils.WriteServerLog("Server Chat:", messageColor);
            ServerUtils.WriteServerLog(OWServer.chatCache.Count == 0 ? "No Chat Messages\n" : string.Join("\n", OWServer.chatCache.ToArray()) + "\n");
        }
        public void List()
        {
            ServerUtils.WriteServerLog($"Connected Players: [{OWServer._Networking.connectedClients.Count}]", messageColor);
            if (OWServer._Networking.connectedClients.Count() == 0) Console.WriteLine("[{0}] | No Players Connected", DateTime.Now);
            else foreach (ServerClient client in OWServer._Networking.connectedClients)
                {
                    try { Console.WriteLine("[{0}] | " + client.username, DateTime.Now); }
                    catch
                    {
                        ServerUtils.WriteServerLog($"Error Processing Player With IP [{((IPEndPoint)client.tcp.Client.RemoteEndPoint).Address}]", errorColor);
                    }
                }
            ServerUtils.WriteServerLog($"\nSaved Players: [{OWServer.savedClients.Count}]", messageColor);
            if (OWServer.savedClients.Count() == 0) Console.WriteLine("[{0}] | No Players Saved", DateTime.Now);
            else foreach (ServerClient savedClient in OWServer.savedClients)
                {
                    try { Console.WriteLine("[{0}] | " + savedClient.username, DateTime.Now); }
                    catch
                    {
                        ServerUtils.WriteServerLog($"Error Processing Player With IP [{((IPEndPoint)savedClient.tcp.Client.RemoteEndPoint).Address}]", errorColor);
                    }
                }
        }
        public void Investigate(string command)
        {
            string clientID = "";
            try { clientID = command.Split(' ')[1]; }
            catch
            {
                ServerUtils.WriteServerLog("Missing Parameters\n", warnColor);
            }
            bool found = false;
            foreach (ServerClient client in OWServer.savedClients)
            {
                if (client.username == clientID)
                {
                    found = true;
                    ServerClient clientToInvestigate = null;
                    bool isConnected = false;
                    string ip = "None";
                    if (OWServer._Networking.connectedClients.Find(fetch => fetch.username == client.username) != null)
                    {
                        clientToInvestigate = OWServer._Networking.connectedClients.Find(fetch => fetch.username == client.username);
                        isConnected = true;
                        ip = ((IPEndPoint)clientToInvestigate.tcp.Client.RemoteEndPoint).Address.ToString();
                    }
                    ServerUtils.WriteServerLog("Player Details:", messageColor);
                    ServerUtils.WriteServerLog($"Username: {client.username}\n" +
                        $"Password: {client.password}\n" +
                        $"Admin: {client.isAdmin}\n" +
                        $"Online: {isConnected}\n" +
                        $"Connection IP: {ip}\n" +
                        $"Home Tile ID: {client.homeTileID}\n" +
                        $"Stored Gifts: {client.giftString.Count()}\n" +
                        $"Stored Trades: {client.tradeString.Count()}\n" +
                        $"Wealth Value: {client.wealth}\n" +
                        $"Pawn Count: {client.pawnCount}\n" +
                        $"Immunized: {client.isImmunized}\n" +
                        $"Event Shielded: {client.eventShielded}\n" +
                        $"In RTSE: {client.inRTSE}\n");
                }
            }
            if (!found) ServerUtils.WriteServerLog($"Player {clientID} Not Found\n", warnColor);
        }
        public void Settlements()
        {
            ServerUtils.WriteServerLog("Server Settlements:", messageColor);
            string logMessage = "";
            if (OWServer.savedSettlements.Count == 0) logMessage = "No Active Settlements";
            else foreach (KeyValuePair<string, List<string>> pair in OWServer.savedSettlements) logMessage += $"{pair.Key} - {pair.Value[0]}";
            ServerUtils.WriteServerLog($"{logMessage}\n");
        }
        public void BanList()
        {
            ServerUtils.WriteServerLog($"Banned Players: [{OWServer.bannedIPs.Count}]", messageColor);
            string logMessage = "";
            if (OWServer.savedSettlements.Count == 0) logMessage = "No Active Settlements\n";
            else foreach (KeyValuePair<string, string> pair in OWServer.bannedIPs) logMessage += $"{pair.Key} - {pair.Value}\n";
            ServerUtils.WriteServerLog($"{logMessage}");
        }
        public void Kick(string command)
        {
            string clientID = "";
            try { clientID = command.Split(' ')[1]; }
            catch
            {
                ServerUtils.WriteServerLog("Missing Parameters\n", warnColor);
            }
            foreach (ServerClient client in OWServer._Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    client.disconnectFlag = true;
                    ServerUtils.WriteServerLog($"Player {clientID} Has Been Kicked\n", warnColor);
                }
            }
            ServerUtils.WriteServerLog($"Player {clientID} Not Found\n", warnColor);
        }
        public void Ban(string command)
        {
            string clientID = "";
            try { clientID = command.Split(' ')[1]; }
            catch
            {
                ServerUtils.WriteServerLog("Missing Parameters\n", warnColor);
            }
            // TODO: Update this to Where() syntax, this is checking them all even after it finds the right one.
            foreach (ServerClient client in OWServer._Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    OWServer.bannedIPs.Add(((IPEndPoint)client.tcp.Client.RemoteEndPoint).Address.ToString(), client.username);
                    client.disconnectFlag = true;
                    SaveSystem.SaveBannedIPs(OWServer.bannedIPs);
                    ServerUtils.WriteServerLog($"Player {client.username} Has Been Unbanned\n", warnColor);
                }
            }
            ServerUtils.WriteServerLog($"Player {clientID} Not Found\n", warnColor);
        }
        public void Pardon(string command)
        {
            string clientUsername = "";
            try { clientUsername = command.Split(' ')[1]; }
            catch
            {
                ServerUtils.WriteServerLog("Missing Parameters\n", warnColor);
            }
            // TODO: Update this to Where() syntax, this is checking them all even after it finds the right one.
            foreach (KeyValuePair<string, string> pair in OWServer.bannedIPs)
            {
                if (pair.Value == clientUsername)
                {
                    OWServer.bannedIPs.Remove(pair.Key);
                    SaveSystem.SaveBannedIPs(OWServer.bannedIPs);
                    ServerUtils.WriteServerLog($"Player {clientUsername} Has Been Unbanned\n", messageColor);
                }
            }
            ServerUtils.WriteServerLog($"Player {clientUsername} Not Found\n", warnColor);
        }
        public void Promote(string command)
        {
            string clientID = "";
            try { clientID = command.Split(' ')[1]; }
            catch
            {
                ServerUtils.WriteServerLog("Missing Parameters\n", warnColor);
            }
            // TODO: Update this to Where() syntax, this is checking them all even after it finds the right one.
            foreach (ServerClient client in OWServer._Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    if (client.isAdmin == true)
                    {
                        Console.ForegroundColor = messageColor;
                        ServerUtils.WriteServerLog("Player [" + client.username + "] Was Already An Administrator");
                        Console.ForegroundColor = defaultColor;
                        ServerUtils.WriteServerLog(Environment.NewLine);
                    }
                    else
                    {
                        client.isAdmin = true;
                        OWServer.savedClients.Find(fetch => fetch.username == client.username).isAdmin = true;
                        SaveSystem.SaveUserData(client);

                        OWServer._Networking.SendData(client, "│Promote│");

                        Console.ForegroundColor = messageColor;
                        ServerUtils.WriteServerLog("Player [" + client.username + "] Has Been Promoted");
                        Console.ForegroundColor = defaultColor;
                        ServerUtils.WriteServerLog(Environment.NewLine);
                    }
                }
            }
            ServerUtils.WriteServerLog($"Player {clientID} Not Found\n", warnColor);
        }
        public void Demote(string command)
        {
            string clientID = "";
            try { clientID = command.Split(' ')[1]; }
            catch
            {
                ServerUtils.WriteServerLog("Missing Parameters\n", warnColor);
            }
            // TODO: Update this to Where() syntax, this is checking them all even after it finds the right one.
            foreach (ServerClient client in OWServer._Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    if (!client.isAdmin)
                    {
                        Console.ForegroundColor = messageColor;
                        ServerUtils.WriteServerLog("Player [" + client.username + "] Is Not An Administrator");
                        Console.ForegroundColor = defaultColor;
                        ServerUtils.WriteServerLog(Environment.NewLine);
                    }
                    else
                    {
                        client.isAdmin = false;
                        OWServer.savedClients.Find(fetch => fetch.username == client.username).isAdmin = false;
                        SaveSystem.SaveUserData(client);

                        OWServer._Networking.SendData(client, "│Demote│");

                        Console.ForegroundColor = messageColor;
                        ServerUtils.WriteServerLog("Player [" + client.username + "] Has Been Demoted");
                        Console.ForegroundColor = defaultColor;
                        ServerUtils.WriteServerLog(Environment.NewLine);
                    }
                }
            }
            ServerUtils.WriteServerLog($"Player {clientID} Not Found\n", warnColor);
        }
        public void GiveItem(string command)
        {
            // TODO: Prescreen the length and get rid of all the repeated try/catches.
            string clientID = "";
            try { clientID = command.Split(' ')[1]; }
            catch
            {
                ServerUtils.WriteServerLog($"Missing Parameter(s)\nUsage: GiveItem [username] [itemID] [itemQuantity] [itemQuality]\n", warnColor);
            }
            string itemID = "";
            try { itemID = command.Split(' ')[2]; }
            catch
            {
                ServerUtils.WriteServerLog($"Missing Parameter(s)\nUsage: GiveItem [username] [itemID] [itemQuantity] [itemQuality]\n", warnColor);
            }
            string itemQuantity = "";
            try { itemQuantity = command.Split(' ')[3]; }
            catch
            {
                ServerUtils.WriteServerLog($"Missing Parameter(s)\nUsage: GiveItem [username] [itemID] [itemQuantity] [itemQuality]\n", warnColor);
            }
            string itemQuality = "";
            try { itemQuality = command.Split(' ')[4]; }
            catch
            {
                ServerUtils.WriteServerLog($"Missing Parameter(s)\nUsage: GiveItem [username] [itemID] [itemQuantity] [itemQuality]\n", warnColor);
            }
            // TODO: Update this to Where() syntax, this is checking them all even after it finds the right one.
            foreach (ServerClient client in OWServer._Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    OWServer._Networking.SendData(client, "GiftedItems│" + itemID + "┼" + itemQuantity + "┼" + itemQuality + "┼");

                    ServerUtils.WriteServerLog($"Item Has Neen Gifted To Player [{client.username}]\n", messageColor);
                }
            }
            ServerUtils.WriteServerLog($"Player {clientID} Not Found\n", warnColor);
        }
        public void GiveItemAll(string command)
        {
            // TODO: Prescreen the length and get rid of all the repeated try/catches.
            string itemID = "";
            try { itemID = command.Split(' ')[1]; }
            catch
            {
                ServerUtils.WriteServerLog($"Missing Parameter(s)\nUsage: Giveitemall [itemID] [itemQuantity] [itemQuality]\n", warnColor);
            }
            string itemQuantity = "";
            try { itemQuantity = command.Split(' ')[2]; }
            catch
            {
                ServerUtils.WriteServerLog($"Missing Parameter(s)\nUsage: Giveitemall [itemID] [itemQuantity] [itemQuality]\n", warnColor);
            }
            string itemQuality = "";
            try { itemQuality = command.Split(' ')[3]; }
            catch
            {
                ServerUtils.WriteServerLog($"Missing Parameter(s)\nUsage: Giveitemall [itemID] [itemQuantity] [itemQuality]\n", warnColor);
            }
            foreach (ServerClient client in OWServer._Networking.connectedClients)
            {
                OWServer._Networking.SendData(client, "GiftedItems│" + itemID + "┼" + itemQuantity + "┼" + itemQuality + "┼");
                ServerUtils.WriteServerLog($"Item Has Neen Gifted To All Players\n", messageColor);
            }
        }
        public void Protect(string command)
        {
            string clientID = "";
            try
            {
                clientID = command.Split(' ')[1];
            }
            catch
            {
                ServerUtils.WriteServerLog("Missing Parameters\n", warnColor);
            }
            // TODO: Update this to Where() syntax, this is checking them all even after it finds the right one.
            foreach (ServerClient client in OWServer._Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    client.eventShielded = true;
                    OWServer.savedClients.Find(fetch => fetch.username == client.username).eventShielded = true;
                    ServerUtils.WriteServerLog($"Player [{client.username}] Has Been Protected\n", messageColor);
                }
            }
            ServerUtils.WriteServerLog($"Player {clientID} Not Found\n", warnColor);
        }
        public void Deprotect(string command)
        {
            string clientID = "";
            try
            {
                clientID = command.Split(' ')[1];
            }
            catch
            {
                ServerUtils.WriteServerLog("Missing Parameters\n", warnColor);
            }
            // TODO: Update this to Where() syntax, this is checking them all even after it finds the right one.
            foreach (ServerClient client in OWServer._Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    client.eventShielded = false;
                    OWServer.savedClients.Find(fetch => fetch.username == client.username).eventShielded = false;
                    ServerUtils.WriteServerLog($"Player [{client.username}] Has Been Deprotected\n", messageColor);
                }
            }
            ServerUtils.WriteServerLog($"Player {clientID} Not Found\n", warnColor);
        }
        public void Immunize(string command)
        {
            string clientID = "";
            try { clientID = command.Split(' ')[1]; }
            catch
            {
                ServerUtils.WriteServerLog("Missing Parameters\n", warnColor);
            }
            // TODO: Update this to Where() syntax, this is checking them all even after it finds the right one.
            foreach (ServerClient client in OWServer._Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    client.isImmunized = true;
                    OWServer.savedClients.Find(fetch => fetch.username == client.username).isImmunized = true;
                    SaveSystem.SaveUserData(client);
                    ServerUtils.WriteServerLog($"Player [{client.username}] Has Been Immunized\n", messageColor);
                }
            }
            ServerUtils.WriteServerLog($"Player {clientID} Not Found\n", warnColor);
        }
        public void Deimmunize(string command)
        {
            string clientID = "";
            try { clientID = command.Split(' ')[1]; }
            catch
            {
                ServerUtils.WriteServerLog("Missing Parameters\n", warnColor);
            }
            // TODO: Update this to Where() syntax, this is checking them all even after it finds the right one.
            foreach (ServerClient client in OWServer._Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    client.isImmunized = false;
                    OWServer.savedClients.Find(fetch => fetch.username == client.username).isImmunized = false;
                    SaveSystem.SaveUserData(client);
                    ServerUtils.WriteServerLog($"Player [{client.username}] Has Been Deimmunized\n", messageColor);
                }
            }
            ServerUtils.WriteServerLog($"Player {clientID} Not Found\n", warnColor);
        }
        public void AdminList()
        {
            OWServer.adminList.Clear();
            foreach (ServerClient client in OWServer.savedClients) if (client.isAdmin) OWServer.adminList.Add(client.username);
            ServerUtils.WriteServerLog($"Server Administrators: [{OWServer.adminList.Count}]", messageColor);
            ServerUtils.WriteServerLog(OWServer.adminList.Count == 0 ? "No Administrators Found\n" : string.Join("\n", OWServer.adminList.ToArray()) + "\n");
        }
        public void WhiteList()
        {
            ServerUtils.WriteServerLog($"Whitelisted Players: [{OWServer.whitelistedUsernames.Count}]", messageColor);
            ServerUtils.WriteServerLog(OWServer.whitelistedUsernames.Count == 0 ? "No Whitelisted Players Found\n" : string.Join("\n", OWServer.whitelistedUsernames.ToArray()) + "\n");
        }
        public void Wipe()
        {
            ServerUtils.WriteServerLog("WARNING! THIS ACTION WILL IRRECOVERABLY DELETE ALL PLAYER DATA. DO YOU WANT TO PROCEED? (Y/N)", errorColor);
            if (Console.ReadLine().Trim().ToUpper() == "Y")
            {
                foreach (ServerClient client in OWServer._Networking.connectedClients)
                {
                    client.disconnectFlag = true;
                }
                Thread.Sleep(1000);
                foreach (ServerClient client in OWServer.savedClients)
                {
                    client.wealth = 0;
                    client.pawnCount = 0;
                    SaveSystem.SaveUserData(client);
                }
                ServerUtils.WriteServerLog("All Player Files Have Been Set To Wipe", errorColor);
            }
            else
            {
                ServerUtils.WriteServerLog("Aborted", warnColor);
            }
        }
        public void Exit()
        {
            foreach (ServerClient sc in OWServer._Networking.connectedClients)
            {
                OWServer._Networking.SendData(sc, "Disconnect│Closing");
                sc.disconnectFlag = true;
            }
            Environment.Exit(0);
        }

    }
}
