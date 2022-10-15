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
            OWServer._ServerUtils.WriteServerLog("List Of Available Commands:", messageColor);
            OWServer._ServerUtils.WriteServerLog("Help - Displays Help Menu\n" +
                "Settings - Displays Settings Menu\n" +
                "Reload - Reloads All Available Settings Into The Server\n" +
                "Status - Shows A General Overview Menu\n" +
                "Settlements - Displays Settlements Menu\n" +
                "List - Displays Player List Menu\n" +
                "Whitelist - Shows All Whitelisted Players\n" +
                "Clear - Clears The Console\n" +
                "Exit - Closes The Server\n");

            OWServer._ServerUtils.WriteServerLog("Communication:", messageColor);
            OWServer._ServerUtils.WriteServerLog("Say - Send A Chat Message\n" +
                "Broadcast - Send A Letter To Every Player Connected\n" +
                "Notify - Send A Letter To X Player\n" +
                "Chat - Displays Chat Menu\n");

            OWServer._ServerUtils.WriteServerLog("Interaction:", messageColor);
            OWServer._ServerUtils.WriteServerLog("Invoke - Invokes An Event To X Player\n" +
                "Plague - Invokes An Event To All Connected Players\n" +
                "Eventlist - Shows All Available Events\n" +
                "GiveItem - Gives An Item To X Player\n" +
                "GiveItemAll - Gives An Item To All Players\n" +
                "Protect - Protects A Player From Any Event Temporarily\n" +
                "Deprotect - Disables All Protections Given To X Player\n" +
                "Immunize - Protects A Player From Any Event Permanently\n" +
                "Deimmunize - Disables The Immunity Given To X Player\n");

            OWServer._ServerUtils.WriteServerLog("Admin Control:", messageColor);
            OWServer._ServerUtils.WriteServerLog("Investigate - Displays All Data About X Player\n" +
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
        public void Say(string command)
        {
            string message = "";
            try { message = command.Remove(0, 4); }
            catch
            {
                OWServer._ServerUtils.WriteServerLog("Missing Parameters\n", warnColor);
            }

            string messageForConsole = "Chat - [Console] " + message;

            OWServer._ServerUtils.WriteServerLog(messageForConsole);

            OWServer.chatCache.Add("[" + DateTime.Now + "]" + " │ " + messageForConsole);

            try
            {
                foreach (ServerClient sc in OWServer._Networking.connectedClients)
                {
                    OWServer._Networking.SendData(sc, "ChatMessage│SERVER│" + message);
                }
            }
            catch { }
        }
        public void Broadcast(string command)
        {
            string text = "";
            try
            {
                command = command.Remove(0, 10);
                text = command;
                if (string.IsNullOrWhiteSpace(text))
                {
                    OWServer._ServerUtils.WriteServerLog("Missing Parameters\n", warnColor);
                }
            }
            catch
            {
                OWServer._ServerUtils.WriteServerLog("Missing Parameters\n", warnColor);
            }
            foreach (ServerClient sc in OWServer._Networking.connectedClients)
            {
                OWServer._Networking.SendData(sc, "Notification│" + text);
            }
            OWServer._ServerUtils.WriteServerLog("Letter Sent To Every Connected Player\n", messageColor);
        }
        public void Notify(string command)
        {
            string target = "", text = "";
            try
            {
                command = command.Remove(0, 7);
                target = command.Split(' ')[0];
                text = command.Replace(target + " ", "");

                if (string.IsNullOrWhiteSpace(text))
                {
                    OWServer._ServerUtils.WriteServerLog("Missing Parameters\n", warnColor);
                }
            }
            catch
            {
                OWServer._ServerUtils.WriteServerLog("Missing Parameters\n", warnColor);
            }
            ServerClient targetClient = OWServer._Networking.connectedClients.Find(fetch => fetch.username == target);
            if (targetClient == null)
            {
                OWServer._ServerUtils.WriteServerLog($"Player {target} Not Found\n", warnColor);
            }
            else
            {
                OWServer._Networking.SendData(targetClient, "Notification│" + text);
                OWServer._ServerUtils.WriteServerLog($"Sent Letter To [{targetClient.username}]\n", messageColor);
            }
        }
        public void Settings()
        {
            OWServer._ServerUtils.WriteServerLog("Server Settings:", messageColor);
            OWServer._ServerUtils.WriteServerLog($"Server Name: {OWServer.serverName}\nServer Description: {OWServer.serverDescription}\nServer Local IP: {OWServer._Networking.localAddress}\nServer Port: {OWServer._Networking.serverPort}\n");

            OWServer._ServerUtils.WriteServerLog("World Settings:", messageColor);
            OWServer._ServerUtils.WriteServerLog($"Globe Coverage: {OWServer.globeCoverage}\nSeed: {OWServer.seed}\nOverall Rainfall: {OWServer.overallRainfall}\nOverall Temperature: {OWServer.overallTemperature}\nOverall Population: {OWServer.overallPopulation}\n");

            OWServer._ServerUtils.WriteServerLog($"Server Mods: [{OWServer.modList.Count}]", messageColor);
            OWServer._ServerUtils.WriteServerLog(OWServer.modList.Count == 0 ? "No Mods Found\n" : string.Join("\n", OWServer.modList.ToArray()) + "\n");

            OWServer._ServerUtils.WriteServerLog($"Server WhiteListed Mods: [{OWServer.modList.Count}]", messageColor);
            OWServer._ServerUtils.WriteServerLog(OWServer.whitelistedMods.Count == 0 ? "No Mods Found\n" : string.Join("\n", OWServer.whitelistedMods.ToArray()) + "\n");
        }
        public void Reload()
        {
            OWServer._ServerUtils.WriteServerLog("Reloading All Current Mods", messageColor);
            // TODO: Is this color change necessary?
            Console.ForegroundColor = defaultColor;
            OWServer._ServerUtils.CheckMods();
            OWServer._ServerUtils.CheckWhitelistedMods();
            OWServer._ServerUtils.WriteServerLog("Mods Have Been Reloaded\n", messageColor);

            OWServer._ServerUtils.WriteServerLog("Reloading All Whitelisted Players\n", messageColor);
            // TODO: Is this color change necessary?
            Console.ForegroundColor = defaultColor;
            OWServer._ServerUtils.CheckForWhitelistedPlayers();
            OWServer._ServerUtils.WriteServerLog("Whitelisted Players Have Been Reloaded", messageColor);
        }
        public void Status()
        {
            OWServer._ServerUtils.WriteServerLog("Server Status", messageColor);
            OWServer._ServerUtils.WriteServerLog($"Version: {OWServer.serverVersion}\n" +
                "Connection: Online\n" +
                $"Uptime: [{DateTime.UtcNow - Process.GetCurrentProcess().StartTime.ToUniversalTime()}]\n");

            OWServer._ServerUtils.WriteServerLog("Mods:", messageColor);
            OWServer._ServerUtils.WriteServerLog($"Mods: {OWServer.modList.Count()}\n" +
                $"WhiteListed Mods: {OWServer.whitelistedMods.Count()}\n");

            OWServer._ServerUtils.WriteServerLog("Players", messageColor);
            OWServer._ServerUtils.WriteServerLog($"Connected Players: {OWServer._Networking.connectedClients.Count()}\n" +
                $"Saved Players: {OWServer.savedClients.Count()}\n" +
                $"Saved Settlements: {OWServer.savedSettlements.Count()}\n" +
                $"Whitelisted Players: {OWServer.whitelistedUsernames.Count()}\n" +
                $"Max Players: {OWServer.maxPlayers}\n");

            OWServer._ServerUtils.WriteServerLog("Modlist Settings", messageColor);
            OWServer._ServerUtils.WriteServerLog($"Using Modlist Check: {OWServer.forceModlist}\n" +
                $"Using Modlist Config Check: {OWServer.forceModlistConfigs}\n" +
                $"Using Mod Verification: {OWServer.usingModVerification}\n");

            OWServer._ServerUtils.WriteServerLog("Chat Settings", messageColor);
            OWServer._ServerUtils.WriteServerLog($"Using Chat: {OWServer.usingChat}\n" +
                $"Using Profanity Filter: {OWServer.usingProfanityFilter}\n");

            OWServer._ServerUtils.WriteServerLog("Wealth Settings", messageColor);
            OWServer._ServerUtils.WriteServerLog($"Using Wealth System: {OWServer.usingWealthSystem}\n" +
                $"Warning Threshold: {OWServer.warningWealthThreshold}\n" +
                $"Ban Threshold: {OWServer.banWealthThreshold}\n");

            OWServer._ServerUtils.WriteServerLog("Idle Settings", messageColor);
            OWServer._ServerUtils.WriteServerLog($"Using Idle System: {OWServer.usingIdleTimer}\n" +
                $"Idle Threshold: {OWServer.idleTimer}\n");

            OWServer._ServerUtils.WriteServerLog("Road Settings", messageColor);
            OWServer._ServerUtils.WriteServerLog($"Using Road System: {OWServer.usingRoadSystem}\n" +
                $"Aggressive Road Mode: {OWServer.aggressiveRoadMode}\n");

            OWServer._ServerUtils.WriteServerLog("Miscellaneous Settings", messageColor);
            OWServer._ServerUtils.WriteServerLog($"Using Whitelist: {OWServer.usingWhitelist}\n" +
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
                OWServer._ServerUtils.WriteServerLog("Missing Parameters\n", warnColor);
            }

            target = OWServer._Networking.connectedClients.Where(x => x.username == clientID).SingleOrDefault();
            if (target == null)
            {
                OWServer._ServerUtils.WriteServerLog($"Player {clientID} Not Found\n", warnColor);
            }
            OWServer._Networking.SendData(target, "ForcedEvent│" + eventID);
            OWServer._ServerUtils.WriteServerLog($"Sent Event [{eventID}] to [{clientID}]\n", messageColor);
        }
        public void Plague(string command)
        {
            string eventID = "";
            try { eventID = command.Split(' ')[1]; }
            catch
            {
                OWServer._ServerUtils.WriteServerLog("Missing Parameters\n", warnColor);
            }
            foreach (ServerClient client in OWServer._Networking.connectedClients)
            {
                OWServer._Networking.SendData(client, "ForcedEvent│" + eventID);
            }
            OWServer._ServerUtils.WriteServerLog($"Sent Event [{eventID}] to Every Player\n", messageColor);
        }
        public void EventList()
        {
            OWServer._ServerUtils.WriteServerLog("List Of Available Events:", messageColor);
            OWServer._ServerUtils.WriteServerLog("Raid\nInfestation\nMechCluster\nToxicFallout\nManhunter\nFarmAnimals\nShipChunk\nGiveQuest\nTraderCaravan\n");
        }
        public void Chat()
        {
            OWServer._ServerUtils.WriteServerLog("Server Chat:", messageColor);
            OWServer._ServerUtils.WriteServerLog(OWServer.chatCache.Count == 0 ? "No Chat Messages\n" : string.Join("\n", OWServer.chatCache.ToArray()) + "\n");
        }
        public void List()
        {
            OWServer._ServerUtils.WriteServerLog($"Connected Players: [{OWServer._Networking.connectedClients.Count}]", messageColor);
            if (OWServer._Networking.connectedClients.Count() == 0) Console.WriteLine("[{0}] | No Players Connected", DateTime.Now);
            else foreach (ServerClient client in OWServer._Networking.connectedClients)
                {
                    try { Console.WriteLine("[{0}] | " + client.username, DateTime.Now); }
                    catch
                    {
                        OWServer._ServerUtils.WriteServerLog($"Error Processing Player With IP [{((IPEndPoint)client.tcp.Client.RemoteEndPoint).Address}]", errorColor);
                    }
                }
            OWServer._ServerUtils.WriteServerLog($"\nSaved Players: [{OWServer.savedClients.Count}]", messageColor);
            if (OWServer.savedClients.Count() == 0) Console.WriteLine("[{0}] | No Players Saved", DateTime.Now);
            else foreach (ServerClient savedClient in OWServer.savedClients)
                {
                    try { Console.WriteLine("[{0}] | " + savedClient.username, DateTime.Now); }
                    catch
                    {
                        OWServer._ServerUtils.WriteServerLog($"Error Processing Player With IP [{((IPEndPoint)savedClient.tcp.Client.RemoteEndPoint).Address}]", errorColor);
                    }
                }
        }
        public void Investigate(string command)
        {
            string clientID = "";
            try { clientID = command.Split(' ')[1]; }
            catch
            {
                OWServer._ServerUtils.WriteServerLog("Missing Parameters\n", warnColor);
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
                    OWServer._ServerUtils.WriteServerLog("Player Details:", messageColor);
                    OWServer._ServerUtils.WriteServerLog($"Username: {client.username}\n" +
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
            if (!found) OWServer._ServerUtils.WriteServerLog($"Player {clientID} Not Found\n", warnColor);
        }
        public void Settlements()
        {
            OWServer._ServerUtils.WriteServerLog("Server Settlements:", messageColor);
            string logMessage = "";
            if (OWServer.savedSettlements.Count == 0) logMessage = "No Active Settlements";
            else foreach (KeyValuePair<string, List<string>> pair in OWServer.savedSettlements) logMessage += $"{pair.Key} - {pair.Value[0]}";
            OWServer._ServerUtils.WriteServerLog($"{logMessage}\n");
        }
        public void BanList()
        {
            OWServer._ServerUtils.WriteServerLog($"Banned Players: [{OWServer.bannedIPs.Count}]", messageColor);
            string logMessage = "";
            if (OWServer.savedSettlements.Count == 0) logMessage = "No Active Settlements\n";
            else foreach (KeyValuePair<string, string> pair in OWServer.bannedIPs) logMessage += $"{pair.Key} - {pair.Value}\n";
            OWServer._ServerUtils.WriteServerLog($"{logMessage}");
        }
        public void Kick(string command)
        {
            string clientID = "";
            try { clientID = command.Split(' ')[1]; }
            catch
            {
                OWServer._ServerUtils.WriteServerLog("Missing Parameters\n", warnColor);
            }
            foreach (ServerClient client in OWServer._Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    client.disconnectFlag = true;
                    OWServer._ServerUtils.WriteServerLog($"Player {clientID} Has Been Kicked\n", warnColor);
                }
            }
            OWServer._ServerUtils.WriteServerLog($"Player {clientID} Not Found\n", warnColor);
        }
        public void Ban(string command)
        {
            string clientID = "";
            try { clientID = command.Split(' ')[1]; }
            catch
            {
                OWServer._ServerUtils.WriteServerLog("Missing Parameters\n", warnColor);
            }
            // TODO: Update this to Where() syntax, this is checking them all even after it finds the right one.
            foreach (ServerClient client in OWServer._Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    OWServer.bannedIPs.Add(((IPEndPoint)client.tcp.Client.RemoteEndPoint).Address.ToString(), client.username);
                    client.disconnectFlag = true;
                    SaveSystem.SaveBannedIPs(OWServer.bannedIPs);
                    OWServer._ServerUtils.WriteServerLog($"Player {client.username} Has Been Unbanned\n", warnColor);
                }
            }
            OWServer._ServerUtils.WriteServerLog($"Player {clientID} Not Found\n", warnColor);
        }
        public void Pardon(string command)
        {
            string clientUsername = "";
            try { clientUsername = command.Split(' ')[1]; }
            catch
            {
                OWServer._ServerUtils.WriteServerLog("Missing Parameters\n", warnColor);
            }
            // TODO: Update this to Where() syntax, this is checking them all even after it finds the right one.
            foreach (KeyValuePair<string, string> pair in OWServer.bannedIPs)
            {
                if (pair.Value == clientUsername)
                {
                    OWServer.bannedIPs.Remove(pair.Key);
                    SaveSystem.SaveBannedIPs(OWServer.bannedIPs);
                    OWServer._ServerUtils.WriteServerLog($"Player {clientUsername} Has Been Unbanned\n", messageColor);
                }
            }
            OWServer._ServerUtils.WriteServerLog($"Player {clientUsername} Not Found\n", warnColor);
        }
        public void Promote(string command)
        {
            string clientID = "";
            try { clientID = command.Split(' ')[1]; }
            catch
            {
                OWServer._ServerUtils.WriteServerLog("Missing Parameters\n", warnColor);
            }
            // TODO: Update this to Where() syntax, this is checking them all even after it finds the right one.
            foreach (ServerClient client in OWServer._Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    if (client.isAdmin == true)
                    {
                        Console.ForegroundColor = messageColor;
                        OWServer._ServerUtils.WriteServerLog("Player [" + client.username + "] Was Already An Administrator");
                        Console.ForegroundColor = defaultColor;
                        OWServer._ServerUtils.WriteServerLog(Environment.NewLine);
                    }
                    else
                    {
                        client.isAdmin = true;
                        OWServer.savedClients.Find(fetch => fetch.username == client.username).isAdmin = true;
                        SaveSystem.SaveUserData(client);

                        OWServer._Networking.SendData(client, "│Promote│");

                        Console.ForegroundColor = messageColor;
                        OWServer._ServerUtils.WriteServerLog("Player [" + client.username + "] Has Been Promoted");
                        Console.ForegroundColor = defaultColor;
                        OWServer._ServerUtils.WriteServerLog(Environment.NewLine);
                    }
                }
            }
            OWServer._ServerUtils.WriteServerLog($"Player {clientID} Not Found\n", warnColor);
        }
        public void Demote(string command)
        {
            string clientID = "";
            try { clientID = command.Split(' ')[1]; }
            catch
            {
                OWServer._ServerUtils.WriteServerLog("Missing Parameters\n", warnColor);
            }
            // TODO: Update this to Where() syntax, this is checking them all even after it finds the right one.
            foreach (ServerClient client in OWServer._Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    if (!client.isAdmin)
                    {
                        Console.ForegroundColor = messageColor;
                        OWServer._ServerUtils.WriteServerLog("Player [" + client.username + "] Is Not An Administrator");
                        Console.ForegroundColor = defaultColor;
                        OWServer._ServerUtils.WriteServerLog(Environment.NewLine);
                    }
                    else
                    {
                        client.isAdmin = false;
                        OWServer.savedClients.Find(fetch => fetch.username == client.username).isAdmin = false;
                        SaveSystem.SaveUserData(client);

                        OWServer._Networking.SendData(client, "│Demote│");

                        Console.ForegroundColor = messageColor;
                        OWServer._ServerUtils.WriteServerLog("Player [" + client.username + "] Has Been Demoted");
                        Console.ForegroundColor = defaultColor;
                        OWServer._ServerUtils.WriteServerLog(Environment.NewLine);
                    }
                }
            }
            OWServer._ServerUtils.WriteServerLog($"Player {clientID} Not Found\n", warnColor);
        }
        public void GiveItem(string command)
        {
            // TODO: Prescreen the length and get rid of all the repeated try/catches.
            string clientID = "";
            try { clientID = command.Split(' ')[1]; }
            catch
            {
                OWServer._ServerUtils.WriteServerLog($"Missing Parameter(s)\nUsage: GiveItem [username] [itemID] [itemQuantity] [itemQuality]\n", warnColor);
            }
            string itemID = "";
            try { itemID = command.Split(' ')[2]; }
            catch
            {
                OWServer._ServerUtils.WriteServerLog($"Missing Parameter(s)\nUsage: GiveItem [username] [itemID] [itemQuantity] [itemQuality]\n", warnColor);
            }
            string itemQuantity = "";
            try { itemQuantity = command.Split(' ')[3]; }
            catch
            {
                OWServer._ServerUtils.WriteServerLog($"Missing Parameter(s)\nUsage: GiveItem [username] [itemID] [itemQuantity] [itemQuality]\n", warnColor);
            }
            string itemQuality = "";
            try { itemQuality = command.Split(' ')[4]; }
            catch
            {
                OWServer._ServerUtils.WriteServerLog($"Missing Parameter(s)\nUsage: GiveItem [username] [itemID] [itemQuantity] [itemQuality]\n", warnColor);
            }
            // TODO: Update this to Where() syntax, this is checking them all even after it finds the right one.
            foreach (ServerClient client in OWServer._Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    OWServer._Networking.SendData(client, "GiftedItems│" + itemID + "┼" + itemQuantity + "┼" + itemQuality + "┼");

                    OWServer._ServerUtils.WriteServerLog($"Item Has Neen Gifted To Player [{client.username}]\n", messageColor);
                }
            }
            OWServer._ServerUtils.WriteServerLog($"Player {clientID} Not Found\n", warnColor);
        }
        public void GiveItemAll(string command)
        {
            // TODO: Prescreen the length and get rid of all the repeated try/catches.
            string itemID = "";
            try { itemID = command.Split(' ')[1]; }
            catch
            {
                OWServer._ServerUtils.WriteServerLog($"Missing Parameter(s)\nUsage: Giveitemall [itemID] [itemQuantity] [itemQuality]\n", warnColor);
            }
            string itemQuantity = "";
            try { itemQuantity = command.Split(' ')[2]; }
            catch
            {
                OWServer._ServerUtils.WriteServerLog($"Missing Parameter(s)\nUsage: Giveitemall [itemID] [itemQuantity] [itemQuality]\n", warnColor);
            }
            string itemQuality = "";
            try { itemQuality = command.Split(' ')[3]; }
            catch
            {
                OWServer._ServerUtils.WriteServerLog($"Missing Parameter(s)\nUsage: Giveitemall [itemID] [itemQuantity] [itemQuality]\n", warnColor);
            }
            foreach (ServerClient client in OWServer._Networking.connectedClients)
            {
                OWServer._Networking.SendData(client, "GiftedItems│" + itemID + "┼" + itemQuantity + "┼" + itemQuality + "┼");
                OWServer._ServerUtils.WriteServerLog($"Item Has Neen Gifted To All Players\n", messageColor);
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
                OWServer._ServerUtils.WriteServerLog("Missing Parameters\n", warnColor);
            }
            // TODO: Update this to Where() syntax, this is checking them all even after it finds the right one.
            foreach (ServerClient client in OWServer._Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    client.eventShielded = true;
                    OWServer.savedClients.Find(fetch => fetch.username == client.username).eventShielded = true;
                    OWServer._ServerUtils.WriteServerLog($"Player [{client.username}] Has Been Protected\n", messageColor);
                }
            }
            OWServer._ServerUtils.WriteServerLog($"Player {clientID} Not Found\n", warnColor);
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
                OWServer._ServerUtils.WriteServerLog("Missing Parameters\n", warnColor);
            }
            // TODO: Update this to Where() syntax, this is checking them all even after it finds the right one.
            foreach (ServerClient client in OWServer._Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    client.eventShielded = false;
                    OWServer.savedClients.Find(fetch => fetch.username == client.username).eventShielded = false;
                    OWServer._ServerUtils.WriteServerLog($"Player [{client.username}] Has Been Deprotected\n", messageColor);
                }
            }
            OWServer._ServerUtils.WriteServerLog($"Player {clientID} Not Found\n", warnColor);
        }
        public void Immunize(string command)
        {
            string clientID = "";
            try { clientID = command.Split(' ')[1]; }
            catch
            {
                OWServer._ServerUtils.WriteServerLog("Missing Parameters\n", warnColor);
            }
            // TODO: Update this to Where() syntax, this is checking them all even after it finds the right one.
            foreach (ServerClient client in OWServer._Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    client.isImmunized = true;
                    OWServer.savedClients.Find(fetch => fetch.username == client.username).isImmunized = true;
                    SaveSystem.SaveUserData(client);
                    OWServer._ServerUtils.WriteServerLog($"Player [{client.username}] Has Been Immunized\n", messageColor);
                }
            }
            OWServer._ServerUtils.WriteServerLog($"Player {clientID} Not Found\n", warnColor);
        }
        public void Deimmunize(string command)
        {
            string clientID = "";
            try { clientID = command.Split(' ')[1]; }
            catch
            {
                OWServer._ServerUtils.WriteServerLog("Missing Parameters\n", warnColor);
            }
            // TODO: Update this to Where() syntax, this is checking them all even after it finds the right one.
            foreach (ServerClient client in OWServer._Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    client.isImmunized = false;
                    OWServer.savedClients.Find(fetch => fetch.username == client.username).isImmunized = false;
                    SaveSystem.SaveUserData(client);
                    OWServer._ServerUtils.WriteServerLog($"Player [{client.username}] Has Been Deimmunized\n", messageColor);
                }
            }
            OWServer._ServerUtils.WriteServerLog($"Player {clientID} Not Found\n", warnColor);
        }
        public void AdminList()
        {
            OWServer.adminList.Clear();
            foreach (ServerClient client in OWServer.savedClients) if (client.isAdmin) OWServer.adminList.Add(client.username);
            OWServer._ServerUtils.WriteServerLog($"Server Administrators: [{OWServer.adminList.Count}]", messageColor);
            OWServer._ServerUtils.WriteServerLog(OWServer.adminList.Count == 0 ? "No Administrators Found\n" : string.Join("\n", OWServer.adminList.ToArray()) + "\n");
        }
        public void WhiteList()
        {
            OWServer._ServerUtils.WriteServerLog($"Whitelisted Players: [{OWServer.whitelistedUsernames.Count}]", messageColor);
            OWServer._ServerUtils.WriteServerLog(OWServer.whitelistedUsernames.Count == 0 ? "No Whitelisted Players Found\n" : string.Join("\n", OWServer.whitelistedUsernames.ToArray()) + "\n");
        }
        public void Wipe()
        {
            OWServer._ServerUtils.WriteServerLog("WARNING! THIS ACTION WILL IRRECOVERABLY DELETE ALL PLAYER DATA. DO YOU WANT TO PROCEED? (Y/N)", errorColor);
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
                Console.Clear();
                OWServer._ServerUtils.WriteServerLog("All Player Files Have Been Set To Wipe", errorColor);
            }
            else
            {
                Console.Clear();
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
