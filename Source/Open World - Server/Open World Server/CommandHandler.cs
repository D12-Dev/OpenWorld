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
            MainProgram.WriteColoredLog("List Of Available Commands:", messageColor);
            MainProgram.WriteColoredLog("Help - Displays Help Menu\n" +
                "Settings - Displays Settings Menu\n" +
                "Reload - Reloads All Available Settings Into The Server\n" +
                "Status - Shows A General Overview Menu\n" +
                "Settlements - Displays Settlements Menu\n" +
                "List - Displays Player List Menu\n" +
                "Whitelist - Shows All Whitelisted Players\n" +
                "Clear - Clears The Console\n" +
                "Exit - Closes The Server\n");

            MainProgram.WriteColoredLog("Communication:", messageColor);
            MainProgram.WriteColoredLog("Say - Send A Chat Message\n" +
                "Broadcast - Send A Letter To Every Player Connected\n" +
                "Notify - Send A Letter To X Player\n" +
                "Chat - Displays Chat Menu\n");

            MainProgram.WriteColoredLog("Interaction:", messageColor);
            MainProgram.WriteColoredLog("Invoke - Invokes An Event To X Player\n" +
                "Plague - Invokes An Event To All Connected Players\n" +
                "Eventlist - Shows All Available Events\n" +
                "GiveItem - Gives An Item To X Player\n" +
                "GiveItemAll - Gives An Item To All Players\n" +
                "Protect - Protects A Player From Any Event Temporarily\n" +
                "Deprotect - Disables All Protections Given To X Player\n" +
                "Immunize - Protects A Player From Any Event Permanently\n" +
                "Deimmunize - Disables The Immunity Given To X Player\n");

            MainProgram.WriteColoredLog("Admin Control:", messageColor);
            MainProgram.WriteColoredLog("Investigate - Displays All Data About X Player\n" +
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
                MainProgram.WriteColoredLog("Missing Parameters\n", warnColor);
                // TODO: STOP CALLING THIS RECURSIVELY! IT'S CAUSING A MEMORY LEAK!
            }

            string messageForConsole = "Chat - [Console] " + message;

            MainProgram._ServerUtils.LogToConsole(messageForConsole);

            MainProgram.chatCache.Add("[" + DateTime.Now + "]" + " │ " + messageForConsole);

            try
            {
                foreach (ServerClient sc in MainProgram._Networking.connectedClients)
                {
                    MainProgram._Networking.SendData(sc, "ChatMessage│SERVER│" + message);
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
                    MainProgram.WriteColoredLog("Missing Parameters\n", warnColor);
                }
            }
            catch
            {
                MainProgram.WriteColoredLog("Missing Parameters\n", warnColor);
            }
            foreach (ServerClient sc in MainProgram._Networking.connectedClients)
            {
                MainProgram._Networking.SendData(sc, "Notification│" + text);
            }
            MainProgram.WriteColoredLog("Letter Sent To Every Connected Player\n", messageColor);
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
                    MainProgram.WriteColoredLog("Missing Parameters\n", warnColor);
                }
            }
            catch
            {
                MainProgram.WriteColoredLog("Missing Parameters\n", warnColor);
            }
            ServerClient targetClient = MainProgram._Networking.connectedClients.Find(fetch => fetch.username == target);
            if (targetClient == null)
            {
                MainProgram.WriteColoredLog($"Player {target} Not Found\n", warnColor);
            }
            else
            {
                MainProgram._Networking.SendData(targetClient, "Notification│" + text);
                MainProgram.WriteColoredLog($"Sent Letter To [{targetClient.username}]\n", messageColor);
            }
        }
        public void Settings()
        {
            MainProgram.WriteColoredLog("Server Settings:", messageColor);
            MainProgram.WriteColoredLog($"Server Name: {MainProgram.serverName}\nServer Description: {MainProgram.serverDescription}\nServer Local IP: {MainProgram._Networking.localAddress}\nServer Port: {MainProgram._Networking.serverPort}\n");

            MainProgram.WriteColoredLog("World Settings:", messageColor);
            MainProgram.WriteColoredLog($"Globe Coverage: {MainProgram.globeCoverage}\nSeed: {MainProgram.seed}\nOverall Rainfall: {MainProgram.overallRainfall}\nOverall Temperature: {MainProgram.overallTemperature}\nOverall Population: {MainProgram.overallPopulation}\n");

            MainProgram.WriteColoredLog($"Server Mods: [{MainProgram.modList.Count}]", messageColor);
            MainProgram.WriteColoredLog(MainProgram.modList.Count == 0 ? "No Mods Found\n" : string.Join("\n", MainProgram.modList.ToArray()) + "\n");

            MainProgram.WriteColoredLog($"Server WhiteListed Mods: [{MainProgram.modList.Count}]", messageColor);
            MainProgram.WriteColoredLog(MainProgram.whitelistedMods.Count == 0 ? "No Mods Found\n" : string.Join("\n", MainProgram.whitelistedMods.ToArray()) + "\n");
        }
        public void Reload()
        {
            MainProgram.WriteColoredLog("Reloading All Current Mods", messageColor);
            // TODO: Is this color change necessary?
            Console.ForegroundColor = defaultColor;
            MainProgram._ServerUtils.CheckMods();
            MainProgram._ServerUtils.CheckWhitelistedMods();
            MainProgram.WriteColoredLog("Mods Have Been Reloaded\n", messageColor);

            MainProgram.WriteColoredLog("Reloading All Whitelisted Players\n", messageColor);
            // TODO: Is this color change necessary?
            Console.ForegroundColor = defaultColor;
            MainProgram._ServerUtils.CheckForWhitelistedPlayers();
            MainProgram.WriteColoredLog("Whitelisted Players Have Been Reloaded", messageColor);
        }
        public void Status()
        {
            MainProgram.WriteColoredLog("Server Status", messageColor);
            MainProgram.WriteColoredLog($"Version: {MainProgram.serverVersion}\n" +
                "Connection: Online\n" +
                $"Uptime: [{DateTime.UtcNow - Process.GetCurrentProcess().StartTime.ToUniversalTime()}]\n");

            MainProgram.WriteColoredLog("Mods:", messageColor);
            MainProgram.WriteColoredLog($"Mods: {MainProgram.modList.Count()}\n" +
                $"WhiteListed Mods: {MainProgram.whitelistedMods.Count()}\n");

            MainProgram.WriteColoredLog("Players", messageColor);
            MainProgram.WriteColoredLog($"Connected Players: {MainProgram._Networking.connectedClients.Count()}\n" +
                $"Saved Players: {MainProgram.savedClients.Count()}\n" +
                $"Saved Settlements: {MainProgram.savedSettlements.Count()}\n" +
                $"Whitelisted Players: {MainProgram.whitelistedUsernames.Count()}\n" +
                $"Max Players: {MainProgram.maxPlayers}\n");

            MainProgram.WriteColoredLog("Modlist Settings", messageColor);
            MainProgram.WriteColoredLog($"Using Modlist Check: {MainProgram.forceModlist}\n" +
                $"Using Modlist Config Check: {MainProgram.forceModlistConfigs}\n" +
                $"Using Mod Verification: {MainProgram.usingModVerification}\n");

            MainProgram.WriteColoredLog("Chat Settings", messageColor);
            MainProgram.WriteColoredLog($"Using Chat: {MainProgram.usingChat}\n" +
                $"Using Profanity Filter: {MainProgram.usingProfanityFilter}\n");

            MainProgram.WriteColoredLog("Wealth Settings", messageColor);
            MainProgram.WriteColoredLog($"Using Wealth System: {MainProgram.usingWealthSystem}\n" +
                $"Warning Threshold: {MainProgram.warningWealthThreshold}\n" +
                $"Ban Threshold: {MainProgram.banWealthThreshold}\n");

            MainProgram.WriteColoredLog("Idle Settings", messageColor);
            MainProgram.WriteColoredLog($"Using Idle System: {MainProgram.usingIdleTimer}\n" +
                $"Idle Threshold: {MainProgram.idleTimer}\n");

            MainProgram.WriteColoredLog("Road Settings", messageColor);
            MainProgram.WriteColoredLog($"Using Road System: {MainProgram.usingRoadSystem}\n" +
                $"Aggressive Road Mode: {MainProgram.aggressiveRoadMode}\n");

            MainProgram.WriteColoredLog("Miscellaneous Settings", messageColor);
            MainProgram.WriteColoredLog($"Using Whitelist: {MainProgram.usingWhitelist}\n" +
                $"Allow Dev Mode: {MainProgram.allowDevMode}\n");
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
                MainProgram.WriteColoredLog("Missing Parameters\n", warnColor);
            }

            target = MainProgram._Networking.connectedClients.Where(x => x.username == clientID).SingleOrDefault();
            if (target == null)
            {
                MainProgram.WriteColoredLog($"Player {clientID} Not Found\n", warnColor);
            }
            MainProgram._Networking.SendData(target, "ForcedEvent│" + eventID);
            MainProgram.WriteColoredLog($"Sent Event [{eventID}] to [{clientID}]\n", messageColor);
        }
        public void Plague(string command)
        {
            string eventID = "";
            try { eventID = command.Split(' ')[1]; }
            catch
            {
                MainProgram.WriteColoredLog("Missing Parameters\n", warnColor);
            }
            foreach (ServerClient client in MainProgram._Networking.connectedClients)
            {
                MainProgram._Networking.SendData(client, "ForcedEvent│" + eventID);
            }
            MainProgram.WriteColoredLog($"Sent Event [{eventID}] to Every Player\n", messageColor);
        }
        public void EventList()
        {
            MainProgram.WriteColoredLog("List Of Available Events:", messageColor);
            MainProgram.WriteColoredLog("Raid\nInfestation\nMechCluster\nToxicFallout\nManhunter\nFarmAnimals\nShipChunk\nGiveQuest\nTraderCaravan\n");
        }
        public void Chat()
        {
            MainProgram.WriteColoredLog("Server Chat:", messageColor);
            MainProgram.WriteColoredLog(MainProgram.chatCache.Count == 0 ? "No Chat Messages\n" : string.Join("\n", MainProgram.chatCache.ToArray()) + "\n");
        }
        public void List()
        {
            MainProgram.WriteColoredLog($"Connected Players: [{MainProgram._Networking.connectedClients.Count}]", messageColor);
            if (MainProgram._Networking.connectedClients.Count() == 0) Console.WriteLine("[{0}] | No Players Connected", DateTime.Now);
            else foreach (ServerClient client in MainProgram._Networking.connectedClients)
                {
                    try { Console.WriteLine("[{0}] | " + client.username, DateTime.Now); }
                    catch
                    {
                        MainProgram.WriteColoredLog($"Error Processing Player With IP [{((IPEndPoint)client.tcp.Client.RemoteEndPoint).Address}]", errorColor);
                    }
                }
            MainProgram.WriteColoredLog($"\nSaved Players: [{MainProgram.savedClients.Count}]", messageColor);
            if (MainProgram.savedClients.Count() == 0) Console.WriteLine("[{0}] | No Players Saved", DateTime.Now);
            else foreach (ServerClient savedClient in MainProgram.savedClients)
                {
                    try { Console.WriteLine("[{0}] | " + savedClient.username, DateTime.Now); }
                    catch
                    {
                        MainProgram.WriteColoredLog($"Error Processing Player With IP [{((IPEndPoint)savedClient.tcp.Client.RemoteEndPoint).Address}]", errorColor);
                    }
                }
        }
        public void Investigate(string command)
        {
            string clientID = "";
            try { clientID = command.Split(' ')[1]; }
            catch
            {
                MainProgram.WriteColoredLog("Missing Parameters\n", warnColor);
            }
            bool found = false;
            foreach (ServerClient client in MainProgram.savedClients)
            {
                if (client.username == clientID)
                {
                    found = true;
                    ServerClient clientToInvestigate = null;
                    bool isConnected = false;
                    string ip = "None";
                    if (MainProgram._Networking.connectedClients.Find(fetch => fetch.username == client.username) != null)
                    {
                        clientToInvestigate = MainProgram._Networking.connectedClients.Find(fetch => fetch.username == client.username);
                        isConnected = true;
                        ip = ((IPEndPoint)clientToInvestigate.tcp.Client.RemoteEndPoint).Address.ToString();
                    }
                    MainProgram.WriteColoredLog("Player Details:", messageColor);
                    MainProgram.WriteColoredLog($"Username: {client.username}\n" +
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
            if (!found) MainProgram.WriteColoredLog($"Player {clientID} Not Found\n", warnColor);
        }
        public void Settlements()
        {
            MainProgram.WriteColoredLog("Server Settlements:", messageColor);
            string logMessage = "";
            if (MainProgram.savedSettlements.Count == 0) logMessage = "No Active Settlements";
            else foreach (KeyValuePair<string, List<string>> pair in MainProgram.savedSettlements) logMessage += $"{pair.Key} - {pair.Value[0]}";
            MainProgram.WriteColoredLog($"{logMessage}\n");
        }
        public void BanList()
        {
            MainProgram.WriteColoredLog($"Banned Players: [{MainProgram.bannedIPs.Count}]", messageColor);
            string logMessage = "";
            if (MainProgram.savedSettlements.Count == 0) logMessage = "No Active Settlements\n";
            else foreach (KeyValuePair<string, string> pair in MainProgram.bannedIPs) logMessage += $"{pair.Key} - {pair.Value}\n";
            MainProgram.WriteColoredLog($"{logMessage}");
        }
        public void Kick(string command)
        {
            string clientID = "";
            try { clientID = command.Split(' ')[1]; }
            catch
            {
                MainProgram.WriteColoredLog("Missing Parameters\n", warnColor);
            }
            foreach (ServerClient client in MainProgram._Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    client.disconnectFlag = true;
                    MainProgram.WriteColoredLog($"Player {clientID} Has Been Kicked\n", warnColor);
                }
            }
            MainProgram.WriteColoredLog($"Player {clientID} Not Found\n", warnColor);
        }
        public void Ban(string command)
        {
            string clientID = "";
            try { clientID = command.Split(' ')[1]; }
            catch
            {
                MainProgram.WriteColoredLog("Missing Parameters\n", warnColor);
            }
            // TODO: Update this to Where() syntax, this is checking them all even after it finds the right one.
            foreach (ServerClient client in MainProgram._Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    MainProgram.bannedIPs.Add(((IPEndPoint)client.tcp.Client.RemoteEndPoint).Address.ToString(), client.username);
                    client.disconnectFlag = true;
                    SaveSystem.SaveBannedIPs(MainProgram.bannedIPs);
                    MainProgram.WriteColoredLog($"Player {client.username} Has Been Unbanned\n", warnColor);
                }
            }
            MainProgram.WriteColoredLog($"Player {clientID} Not Found\n", warnColor);
        }
        public void Pardon(string command)
        {
            string clientUsername = "";
            try { clientUsername = command.Split(' ')[1]; }
            catch
            {
                MainProgram.WriteColoredLog("Missing Parameters\n", warnColor);
            }
            // TODO: Update this to Where() syntax, this is checking them all even after it finds the right one.
            foreach (KeyValuePair<string, string> pair in MainProgram.bannedIPs)
            {
                if (pair.Value == clientUsername)
                {
                    MainProgram.bannedIPs.Remove(pair.Key);
                    SaveSystem.SaveBannedIPs(MainProgram.bannedIPs);
                    MainProgram.WriteColoredLog($"Player {clientUsername} Has Been Unbanned\n", messageColor);
                }
            }
            MainProgram.WriteColoredLog($"Player {clientUsername} Not Found\n", warnColor);
        }
        public void Promote(string command)
        {
            string clientID = "";
            try { clientID = command.Split(' ')[1]; }
            catch
            {
                MainProgram.WriteColoredLog("Missing Parameters\n", warnColor);
            }
            // TODO: Update this to Where() syntax, this is checking them all even after it finds the right one.
            foreach (ServerClient client in MainProgram._Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    if (client.isAdmin == true)
                    {
                        Console.ForegroundColor = messageColor;
                        MainProgram._ServerUtils.LogToConsole("Player [" + client.username + "] Was Already An Administrator");
                        Console.ForegroundColor = defaultColor;
                        MainProgram._ServerUtils.LogToConsole(Environment.NewLine);
                    }
                    else
                    {
                        client.isAdmin = true;
                        MainProgram.savedClients.Find(fetch => fetch.username == client.username).isAdmin = true;
                        SaveSystem.SaveUserData(client);

                        MainProgram._Networking.SendData(client, "│Promote│");

                        Console.ForegroundColor = messageColor;
                        MainProgram._ServerUtils.LogToConsole("Player [" + client.username + "] Has Been Promoted");
                        Console.ForegroundColor = defaultColor;
                        MainProgram._ServerUtils.LogToConsole(Environment.NewLine);
                    }
                }
            }
            MainProgram.WriteColoredLog($"Player {clientID} Not Found\n", warnColor);
        }
        public void Demote(string command)
        {
            string clientID = "";
            try { clientID = command.Split(' ')[1]; }
            catch
            {
                MainProgram.WriteColoredLog("Missing Parameters\n", warnColor);
            }
            // TODO: Update this to Where() syntax, this is checking them all even after it finds the right one.
            foreach (ServerClient client in MainProgram._Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    if (!client.isAdmin)
                    {
                        Console.ForegroundColor = messageColor;
                        MainProgram._ServerUtils.LogToConsole("Player [" + client.username + "] Is Not An Administrator");
                        Console.ForegroundColor = defaultColor;
                        MainProgram._ServerUtils.LogToConsole(Environment.NewLine);
                    }
                    else
                    {
                        client.isAdmin = false;
                        MainProgram.savedClients.Find(fetch => fetch.username == client.username).isAdmin = false;
                        SaveSystem.SaveUserData(client);

                        MainProgram._Networking.SendData(client, "│Demote│");

                        Console.ForegroundColor = messageColor;
                        MainProgram._ServerUtils.LogToConsole("Player [" + client.username + "] Has Been Demoted");
                        Console.ForegroundColor = defaultColor;
                        MainProgram._ServerUtils.LogToConsole(Environment.NewLine);
                    }
                }
            }
            MainProgram.WriteColoredLog($"Player {clientID} Not Found\n", warnColor);
        }
        public void GiveItem(string command)
        {
            // TODO: Prescreen the length and get rid of all the repeated try/catches.
            string clientID = "";
            try { clientID = command.Split(' ')[1]; }
            catch
            {
                MainProgram.WriteColoredLog($"Missing Parameter(s)\nUsage: GiveItem [username] [itemID] [itemQuantity] [itemQuality]\n", warnColor);
            }
            string itemID = "";
            try { itemID = command.Split(' ')[2]; }
            catch
            {
                MainProgram.WriteColoredLog($"Missing Parameter(s)\nUsage: GiveItem [username] [itemID] [itemQuantity] [itemQuality]\n", warnColor);
            }
            string itemQuantity = "";
            try { itemQuantity = command.Split(' ')[3]; }
            catch
            {
                MainProgram.WriteColoredLog($"Missing Parameter(s)\nUsage: GiveItem [username] [itemID] [itemQuantity] [itemQuality]\n", warnColor);
            }
            string itemQuality = "";
            try { itemQuality = command.Split(' ')[4]; }
            catch
            {
                MainProgram.WriteColoredLog($"Missing Parameter(s)\nUsage: GiveItem [username] [itemID] [itemQuantity] [itemQuality]\n", warnColor);
            }
            // TODO: Update this to Where() syntax, this is checking them all even after it finds the right one.
            foreach (ServerClient client in MainProgram._Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    MainProgram._Networking.SendData(client, "GiftedItems│" + itemID + "┼" + itemQuantity + "┼" + itemQuality + "┼");

                    MainProgram.WriteColoredLog($"Item Has Neen Gifted To Player [{client.username}]\n", messageColor);
                }
            }
            MainProgram.WriteColoredLog($"Player {clientID} Not Found\n", warnColor);
        }
        public void GiveItemAll(string command)
        {
            // TODO: Prescreen the length and get rid of all the repeated try/catches.
            string itemID = "";
            try { itemID = command.Split(' ')[1]; }
            catch
            {
                MainProgram.WriteColoredLog($"Missing Parameter(s)\nUsage: Giveitemall [itemID] [itemQuantity] [itemQuality]\n", warnColor);
            }
            string itemQuantity = "";
            try { itemQuantity = command.Split(' ')[2]; }
            catch
            {
                MainProgram.WriteColoredLog($"Missing Parameter(s)\nUsage: Giveitemall [itemID] [itemQuantity] [itemQuality]\n", warnColor);
            }
            string itemQuality = "";
            try { itemQuality = command.Split(' ')[3]; }
            catch
            {
                MainProgram.WriteColoredLog($"Missing Parameter(s)\nUsage: Giveitemall [itemID] [itemQuantity] [itemQuality]\n", warnColor);
            }
            foreach (ServerClient client in MainProgram._Networking.connectedClients)
            {
                MainProgram._Networking.SendData(client, "GiftedItems│" + itemID + "┼" + itemQuantity + "┼" + itemQuality + "┼");
                MainProgram.WriteColoredLog($"Item Has Neen Gifted To All Players\n", messageColor);
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
                MainProgram.WriteColoredLog("Missing Parameters\n", warnColor);
            }
            // TODO: Update this to Where() syntax, this is checking them all even after it finds the right one.
            foreach (ServerClient client in MainProgram._Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    client.eventShielded = true;
                    MainProgram.savedClients.Find(fetch => fetch.username == client.username).eventShielded = true;
                    MainProgram.WriteColoredLog($"Player [{client.username}] Has Been Protected\n", messageColor);
                }
            }
            MainProgram.WriteColoredLog($"Player {clientID} Not Found\n", warnColor);
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
                MainProgram.WriteColoredLog("Missing Parameters\n", warnColor);
            }
            // TODO: Update this to Where() syntax, this is checking them all even after it finds the right one.
            foreach (ServerClient client in MainProgram._Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    client.eventShielded = false;
                    MainProgram.savedClients.Find(fetch => fetch.username == client.username).eventShielded = false;
                    MainProgram.WriteColoredLog($"Player [{client.username}] Has Been Deprotected\n", messageColor);
                }
            }
            MainProgram.WriteColoredLog($"Player {clientID} Not Found\n", warnColor);
        }
        public void Immunize(string command)
        {
            string clientID = "";
            try { clientID = command.Split(' ')[1]; }
            catch
            {
                MainProgram.WriteColoredLog("Missing Parameters\n", warnColor);
            }
            // TODO: Update this to Where() syntax, this is checking them all even after it finds the right one.
            foreach (ServerClient client in MainProgram._Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    client.isImmunized = true;
                    MainProgram.savedClients.Find(fetch => fetch.username == client.username).isImmunized = true;
                    SaveSystem.SaveUserData(client);
                    MainProgram.WriteColoredLog($"Player [{client.username}] Has Been Immunized\n", messageColor);
                }
            }
            MainProgram.WriteColoredLog($"Player {clientID} Not Found\n", warnColor);
        }
        public void Deimmunize(string command)
        {
            string clientID = "";
            try { clientID = command.Split(' ')[1]; }
            catch
            {
                MainProgram.WriteColoredLog("Missing Parameters\n", warnColor);
            }
            // TODO: Update this to Where() syntax, this is checking them all even after it finds the right one.
            foreach (ServerClient client in MainProgram._Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    client.isImmunized = false;
                    MainProgram.savedClients.Find(fetch => fetch.username == client.username).isImmunized = false;
                    SaveSystem.SaveUserData(client);
                    MainProgram.WriteColoredLog($"Player [{client.username}] Has Been Deimmunized\n", messageColor);
                }
            }
            MainProgram.WriteColoredLog($"Player {clientID} Not Found\n", warnColor);
        }
        public void AdminList()
        {
            MainProgram.adminList.Clear();
            foreach (ServerClient client in MainProgram.savedClients) if (client.isAdmin) MainProgram.adminList.Add(client.username);
            MainProgram.WriteColoredLog($"Server Administrators: [{MainProgram.adminList.Count}]", messageColor);
            MainProgram.WriteColoredLog(MainProgram.adminList.Count == 0 ? "No Administrators Found\n" : string.Join("\n", MainProgram.adminList.ToArray()) + "\n");
        }
        public void WhiteList()
        {
            MainProgram.WriteColoredLog($"Whitelisted Players: [{MainProgram.whitelistedUsernames.Count}]", messageColor);
            MainProgram.WriteColoredLog(MainProgram.whitelistedUsernames.Count == 0 ? "No Whitelisted Players Found\n" : string.Join("\n", MainProgram.whitelistedUsernames.ToArray()) + "\n");
        }
        public void Wipe()
        {
            MainProgram.WriteColoredLog("WARNING! THIS ACTION WILL IRRECOVERABLY DELETE ALL PLAYER DATA. DO YOU WANT TO PROCEED? (Y/N)", errorColor);
            if (Console.ReadLine().Trim().ToUpper() == "Y")
            {
                foreach (ServerClient client in MainProgram._Networking.connectedClients)
                {
                    client.disconnectFlag = true;
                }
                Thread.Sleep(1000);
                foreach (ServerClient client in MainProgram.savedClients)
                {
                    client.wealth = 0;
                    client.pawnCount = 0;
                    SaveSystem.SaveUserData(client);
                }
                Console.Clear();
                MainProgram.WriteColoredLog("All Player Files Have Been Set To Wipe", errorColor);
            }
            else
            {
                Console.Clear();
            }
        }
        public void Exit()
        {
            foreach (ServerClient sc in MainProgram._Networking.connectedClients)
            {
                MainProgram._Networking.SendData(sc, "Disconnect│Closing");
                sc.disconnectFlag = true;
            }
            Environment.Exit(0);
        }

    }
}
