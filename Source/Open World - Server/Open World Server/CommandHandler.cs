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
            MainProgram._ServerUtils.WriteServerLog("List Of Available Commands:", messageColor);
            MainProgram._ServerUtils.WriteServerLog("Help - Displays Help Menu\n" +
                "Settings - Displays Settings Menu\n" +
                "Reload - Reloads All Available Settings Into The Server\n" +
                "Status - Shows A General Overview Menu\n" +
                "Settlements - Displays Settlements Menu\n" +
                "List - Displays Player List Menu\n" +
                "Whitelist - Shows All Whitelisted Players\n" +
                "Clear - Clears The Console\n" +
                "Exit - Closes The Server\n");

            MainProgram._ServerUtils.WriteServerLog("Communication:", messageColor);
            MainProgram._ServerUtils.WriteServerLog("Say - Send A Chat Message\n" +
                "Broadcast - Send A Letter To Every Player Connected\n" +
                "Notify - Send A Letter To X Player\n" +
                "Chat - Displays Chat Menu\n");

            MainProgram._ServerUtils.WriteServerLog("Interaction:", messageColor);
            MainProgram._ServerUtils.WriteServerLog("Invoke - Invokes An Event To X Player\n" +
                "Plague - Invokes An Event To All Connected Players\n" +
                "Eventlist - Shows All Available Events\n" +
                "GiveItem - Gives An Item To X Player\n" +
                "GiveItemAll - Gives An Item To All Players\n" +
                "Protect - Protects A Player From Any Event Temporarily\n" +
                "Deprotect - Disables All Protections Given To X Player\n" +
                "Immunize - Protects A Player From Any Event Permanently\n" +
                "Deimmunize - Disables The Immunity Given To X Player\n");

            MainProgram._ServerUtils.WriteServerLog("Admin Control:", messageColor);
            MainProgram._ServerUtils.WriteServerLog("Investigate - Displays All Data About X Player\n" +
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
                MainProgram._ServerUtils.WriteServerLog("Missing Parameters\n", warnColor);
                // TODO: STOP CALLING THIS RECURSIVELY! IT'S CAUSING A MEMORY LEAK!
            }

            string messageForConsole = "Chat - [Console] " + message;

            MainProgram._ServerUtils.WriteServerLog(messageForConsole);

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
                    MainProgram._ServerUtils.WriteServerLog("Missing Parameters\n", warnColor);
                }
            }
            catch
            {
                MainProgram._ServerUtils.WriteServerLog("Missing Parameters\n", warnColor);
            }
            foreach (ServerClient sc in MainProgram._Networking.connectedClients)
            {
                MainProgram._Networking.SendData(sc, "Notification│" + text);
            }
            MainProgram._ServerUtils.WriteServerLog("Letter Sent To Every Connected Player\n", messageColor);
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
                    MainProgram._ServerUtils.WriteServerLog("Missing Parameters\n", warnColor);
                }
            }
            catch
            {
                MainProgram._ServerUtils.WriteServerLog("Missing Parameters\n", warnColor);
            }
            ServerClient targetClient = MainProgram._Networking.connectedClients.Find(fetch => fetch.username == target);
            if (targetClient == null)
            {
                MainProgram._ServerUtils.WriteServerLog($"Player {target} Not Found\n", warnColor);
            }
            else
            {
                MainProgram._Networking.SendData(targetClient, "Notification│" + text);
                MainProgram._ServerUtils.WriteServerLog($"Sent Letter To [{targetClient.username}]\n", messageColor);
            }
        }
        public void Settings()
        {
            MainProgram._ServerUtils.WriteServerLog("Server Settings:", messageColor);
            MainProgram._ServerUtils.WriteServerLog($"Server Name: {MainProgram.serverName}\nServer Description: {MainProgram.serverDescription}\nServer Local IP: {MainProgram._Networking.localAddress}\nServer Port: {MainProgram._Networking.serverPort}\n");

            MainProgram._ServerUtils.WriteServerLog("World Settings:", messageColor);
            MainProgram._ServerUtils.WriteServerLog($"Globe Coverage: {MainProgram.globeCoverage}\nSeed: {MainProgram.seed}\nOverall Rainfall: {MainProgram.overallRainfall}\nOverall Temperature: {MainProgram.overallTemperature}\nOverall Population: {MainProgram.overallPopulation}\n");

            MainProgram._ServerUtils.WriteServerLog($"Server Mods: [{MainProgram.modList.Count}]", messageColor);
            MainProgram._ServerUtils.WriteServerLog(MainProgram.modList.Count == 0 ? "No Mods Found\n" : string.Join("\n", MainProgram.modList.ToArray()) + "\n");

            MainProgram._ServerUtils.WriteServerLog($"Server WhiteListed Mods: [{MainProgram.modList.Count}]", messageColor);
            MainProgram._ServerUtils.WriteServerLog(MainProgram.whitelistedMods.Count == 0 ? "No Mods Found\n" : string.Join("\n", MainProgram.whitelistedMods.ToArray()) + "\n");
        }
        public void Reload()
        {
            MainProgram._ServerUtils.WriteServerLog("Reloading All Current Mods", messageColor);
            // TODO: Is this color change necessary?
            Console.ForegroundColor = defaultColor;
            MainProgram._ServerUtils.CheckMods();
            MainProgram._ServerUtils.CheckWhitelistedMods();
            MainProgram._ServerUtils.WriteServerLog("Mods Have Been Reloaded\n", messageColor);

            MainProgram._ServerUtils.WriteServerLog("Reloading All Whitelisted Players\n", messageColor);
            // TODO: Is this color change necessary?
            Console.ForegroundColor = defaultColor;
            MainProgram._ServerUtils.CheckForWhitelistedPlayers();
            MainProgram._ServerUtils.WriteServerLog("Whitelisted Players Have Been Reloaded", messageColor);
        }
        public void Status()
        {
            MainProgram._ServerUtils.WriteServerLog("Server Status", messageColor);
            MainProgram._ServerUtils.WriteServerLog($"Version: {MainProgram.serverVersion}\n" +
                "Connection: Online\n" +
                $"Uptime: [{DateTime.UtcNow - Process.GetCurrentProcess().StartTime.ToUniversalTime()}]\n");

            MainProgram._ServerUtils.WriteServerLog("Mods:", messageColor);
            MainProgram._ServerUtils.WriteServerLog($"Mods: {MainProgram.modList.Count()}\n" +
                $"WhiteListed Mods: {MainProgram.whitelistedMods.Count()}\n");

            MainProgram._ServerUtils.WriteServerLog("Players", messageColor);
            MainProgram._ServerUtils.WriteServerLog($"Connected Players: {MainProgram._Networking.connectedClients.Count()}\n" +
                $"Saved Players: {MainProgram.savedClients.Count()}\n" +
                $"Saved Settlements: {MainProgram.savedSettlements.Count()}\n" +
                $"Whitelisted Players: {MainProgram.whitelistedUsernames.Count()}\n" +
                $"Max Players: {MainProgram.maxPlayers}\n");

            MainProgram._ServerUtils.WriteServerLog("Modlist Settings", messageColor);
            MainProgram._ServerUtils.WriteServerLog($"Using Modlist Check: {MainProgram.forceModlist}\n" +
                $"Using Modlist Config Check: {MainProgram.forceModlistConfigs}\n" +
                $"Using Mod Verification: {MainProgram.usingModVerification}\n");

            MainProgram._ServerUtils.WriteServerLog("Chat Settings", messageColor);
            MainProgram._ServerUtils.WriteServerLog($"Using Chat: {MainProgram.usingChat}\n" +
                $"Using Profanity Filter: {MainProgram.usingProfanityFilter}\n");

            MainProgram._ServerUtils.WriteServerLog("Wealth Settings", messageColor);
            MainProgram._ServerUtils.WriteServerLog($"Using Wealth System: {MainProgram.usingWealthSystem}\n" +
                $"Warning Threshold: {MainProgram.warningWealthThreshold}\n" +
                $"Ban Threshold: {MainProgram.banWealthThreshold}\n");

            MainProgram._ServerUtils.WriteServerLog("Idle Settings", messageColor);
            MainProgram._ServerUtils.WriteServerLog($"Using Idle System: {MainProgram.usingIdleTimer}\n" +
                $"Idle Threshold: {MainProgram.idleTimer}\n");

            MainProgram._ServerUtils.WriteServerLog("Road Settings", messageColor);
            MainProgram._ServerUtils.WriteServerLog($"Using Road System: {MainProgram.usingRoadSystem}\n" +
                $"Aggressive Road Mode: {MainProgram.aggressiveRoadMode}\n");

            MainProgram._ServerUtils.WriteServerLog("Miscellaneous Settings", messageColor);
            MainProgram._ServerUtils.WriteServerLog($"Using Whitelist: {MainProgram.usingWhitelist}\n" +
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
                MainProgram._ServerUtils.WriteServerLog("Missing Parameters\n", warnColor);
            }

            target = MainProgram._Networking.connectedClients.Where(x => x.username == clientID).SingleOrDefault();
            if (target == null)
            {
                MainProgram._ServerUtils.WriteServerLog($"Player {clientID} Not Found\n", warnColor);
            }
            MainProgram._Networking.SendData(target, "ForcedEvent│" + eventID);
            MainProgram._ServerUtils.WriteServerLog($"Sent Event [{eventID}] to [{clientID}]\n", messageColor);
        }
        public void Plague(string command)
        {
            string eventID = "";
            try { eventID = command.Split(' ')[1]; }
            catch
            {
                MainProgram._ServerUtils.WriteServerLog("Missing Parameters\n", warnColor);
            }
            foreach (ServerClient client in MainProgram._Networking.connectedClients)
            {
                MainProgram._Networking.SendData(client, "ForcedEvent│" + eventID);
            }
            MainProgram._ServerUtils.WriteServerLog($"Sent Event [{eventID}] to Every Player\n", messageColor);
        }
        public void EventList()
        {
            MainProgram._ServerUtils.WriteServerLog("List Of Available Events:", messageColor);
            MainProgram._ServerUtils.WriteServerLog("Raid\nInfestation\nMechCluster\nToxicFallout\nManhunter\nFarmAnimals\nShipChunk\nGiveQuest\nTraderCaravan\n");
        }
        public void Chat()
        {
            MainProgram._ServerUtils.WriteServerLog("Server Chat:", messageColor);
            MainProgram._ServerUtils.WriteServerLog(MainProgram.chatCache.Count == 0 ? "No Chat Messages\n" : string.Join("\n", MainProgram.chatCache.ToArray()) + "\n");
        }
        public void List()
        {
            MainProgram._ServerUtils.WriteServerLog($"Connected Players: [{MainProgram._Networking.connectedClients.Count}]", messageColor);
            if (MainProgram._Networking.connectedClients.Count() == 0) Console.WriteLine("[{0}] | No Players Connected", DateTime.Now);
            else foreach (ServerClient client in MainProgram._Networking.connectedClients)
                {
                    try { Console.WriteLine("[{0}] | " + client.username, DateTime.Now); }
                    catch
                    {
                        MainProgram._ServerUtils.WriteServerLog($"Error Processing Player With IP [{((IPEndPoint)client.tcp.Client.RemoteEndPoint).Address}]", errorColor);
                    }
                }
            MainProgram._ServerUtils.WriteServerLog($"\nSaved Players: [{MainProgram.savedClients.Count}]", messageColor);
            if (MainProgram.savedClients.Count() == 0) Console.WriteLine("[{0}] | No Players Saved", DateTime.Now);
            else foreach (ServerClient savedClient in MainProgram.savedClients)
                {
                    try { Console.WriteLine("[{0}] | " + savedClient.username, DateTime.Now); }
                    catch
                    {
                        MainProgram._ServerUtils.WriteServerLog($"Error Processing Player With IP [{((IPEndPoint)savedClient.tcp.Client.RemoteEndPoint).Address}]", errorColor);
                    }
                }
        }
        public void Investigate(string command)
        {
            string clientID = "";
            try { clientID = command.Split(' ')[1]; }
            catch
            {
                MainProgram._ServerUtils.WriteServerLog("Missing Parameters\n", warnColor);
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
                    MainProgram._ServerUtils.WriteServerLog("Player Details:", messageColor);
                    MainProgram._ServerUtils.WriteServerLog($"Username: {client.username}\n" +
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
            if (!found) MainProgram._ServerUtils.WriteServerLog($"Player {clientID} Not Found\n", warnColor);
        }
        public void Settlements()
        {
            MainProgram._ServerUtils.WriteServerLog("Server Settlements:", messageColor);
            string logMessage = "";
            if (MainProgram.savedSettlements.Count == 0) logMessage = "No Active Settlements";
            else foreach (KeyValuePair<string, List<string>> pair in MainProgram.savedSettlements) logMessage += $"{pair.Key} - {pair.Value[0]}";
            MainProgram._ServerUtils.WriteServerLog($"{logMessage}\n");
        }
        public void BanList()
        {
            MainProgram._ServerUtils.WriteServerLog($"Banned Players: [{MainProgram.bannedIPs.Count}]", messageColor);
            string logMessage = "";
            if (MainProgram.savedSettlements.Count == 0) logMessage = "No Active Settlements\n";
            else foreach (KeyValuePair<string, string> pair in MainProgram.bannedIPs) logMessage += $"{pair.Key} - {pair.Value}\n";
            MainProgram._ServerUtils.WriteServerLog($"{logMessage}");
        }
        public void Kick(string command)
        {
            string clientID = "";
            try { clientID = command.Split(' ')[1]; }
            catch
            {
                MainProgram._ServerUtils.WriteServerLog("Missing Parameters\n", warnColor);
            }
            foreach (ServerClient client in MainProgram._Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    client.disconnectFlag = true;
                    MainProgram._ServerUtils.WriteServerLog($"Player {clientID} Has Been Kicked\n", warnColor);
                }
            }
            MainProgram._ServerUtils.WriteServerLog($"Player {clientID} Not Found\n", warnColor);
        }
        public void Ban(string command)
        {
            string clientID = "";
            try { clientID = command.Split(' ')[1]; }
            catch
            {
                MainProgram._ServerUtils.WriteServerLog("Missing Parameters\n", warnColor);
            }
            // TODO: Update this to Where() syntax, this is checking them all even after it finds the right one.
            foreach (ServerClient client in MainProgram._Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    MainProgram.bannedIPs.Add(((IPEndPoint)client.tcp.Client.RemoteEndPoint).Address.ToString(), client.username);
                    client.disconnectFlag = true;
                    SaveSystem.SaveBannedIPs(MainProgram.bannedIPs);
                    MainProgram._ServerUtils.WriteServerLog($"Player {client.username} Has Been Unbanned\n", warnColor);
                }
            }
            MainProgram._ServerUtils.WriteServerLog($"Player {clientID} Not Found\n", warnColor);
        }
        public void Pardon(string command)
        {
            string clientUsername = "";
            try { clientUsername = command.Split(' ')[1]; }
            catch
            {
                MainProgram._ServerUtils.WriteServerLog("Missing Parameters\n", warnColor);
            }
            // TODO: Update this to Where() syntax, this is checking them all even after it finds the right one.
            foreach (KeyValuePair<string, string> pair in MainProgram.bannedIPs)
            {
                if (pair.Value == clientUsername)
                {
                    MainProgram.bannedIPs.Remove(pair.Key);
                    SaveSystem.SaveBannedIPs(MainProgram.bannedIPs);
                    MainProgram._ServerUtils.WriteServerLog($"Player {clientUsername} Has Been Unbanned\n", messageColor);
                }
            }
            MainProgram._ServerUtils.WriteServerLog($"Player {clientUsername} Not Found\n", warnColor);
        }
        public void Promote(string command)
        {
            string clientID = "";
            try { clientID = command.Split(' ')[1]; }
            catch
            {
                MainProgram._ServerUtils.WriteServerLog("Missing Parameters\n", warnColor);
            }
            // TODO: Update this to Where() syntax, this is checking them all even after it finds the right one.
            foreach (ServerClient client in MainProgram._Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    if (client.isAdmin == true)
                    {
                        Console.ForegroundColor = messageColor;
                        MainProgram._ServerUtils.WriteServerLog("Player [" + client.username + "] Was Already An Administrator");
                        Console.ForegroundColor = defaultColor;
                        MainProgram._ServerUtils.WriteServerLog(Environment.NewLine);
                    }
                    else
                    {
                        client.isAdmin = true;
                        MainProgram.savedClients.Find(fetch => fetch.username == client.username).isAdmin = true;
                        SaveSystem.SaveUserData(client);

                        MainProgram._Networking.SendData(client, "│Promote│");

                        Console.ForegroundColor = messageColor;
                        MainProgram._ServerUtils.WriteServerLog("Player [" + client.username + "] Has Been Promoted");
                        Console.ForegroundColor = defaultColor;
                        MainProgram._ServerUtils.WriteServerLog(Environment.NewLine);
                    }
                }
            }
            MainProgram._ServerUtils.WriteServerLog($"Player {clientID} Not Found\n", warnColor);
        }
        public void Demote(string command)
        {
            string clientID = "";
            try { clientID = command.Split(' ')[1]; }
            catch
            {
                MainProgram._ServerUtils.WriteServerLog("Missing Parameters\n", warnColor);
            }
            // TODO: Update this to Where() syntax, this is checking them all even after it finds the right one.
            foreach (ServerClient client in MainProgram._Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    if (!client.isAdmin)
                    {
                        Console.ForegroundColor = messageColor;
                        MainProgram._ServerUtils.WriteServerLog("Player [" + client.username + "] Is Not An Administrator");
                        Console.ForegroundColor = defaultColor;
                        MainProgram._ServerUtils.WriteServerLog(Environment.NewLine);
                    }
                    else
                    {
                        client.isAdmin = false;
                        MainProgram.savedClients.Find(fetch => fetch.username == client.username).isAdmin = false;
                        SaveSystem.SaveUserData(client);

                        MainProgram._Networking.SendData(client, "│Demote│");

                        Console.ForegroundColor = messageColor;
                        MainProgram._ServerUtils.WriteServerLog("Player [" + client.username + "] Has Been Demoted");
                        Console.ForegroundColor = defaultColor;
                        MainProgram._ServerUtils.WriteServerLog(Environment.NewLine);
                    }
                }
            }
            MainProgram._ServerUtils.WriteServerLog($"Player {clientID} Not Found\n", warnColor);
        }
        public void GiveItem(string command)
        {
            // TODO: Prescreen the length and get rid of all the repeated try/catches.
            string clientID = "";
            try { clientID = command.Split(' ')[1]; }
            catch
            {
                MainProgram._ServerUtils.WriteServerLog($"Missing Parameter(s)\nUsage: GiveItem [username] [itemID] [itemQuantity] [itemQuality]\n", warnColor);
            }
            string itemID = "";
            try { itemID = command.Split(' ')[2]; }
            catch
            {
                MainProgram._ServerUtils.WriteServerLog($"Missing Parameter(s)\nUsage: GiveItem [username] [itemID] [itemQuantity] [itemQuality]\n", warnColor);
            }
            string itemQuantity = "";
            try { itemQuantity = command.Split(' ')[3]; }
            catch
            {
                MainProgram._ServerUtils.WriteServerLog($"Missing Parameter(s)\nUsage: GiveItem [username] [itemID] [itemQuantity] [itemQuality]\n", warnColor);
            }
            string itemQuality = "";
            try { itemQuality = command.Split(' ')[4]; }
            catch
            {
                MainProgram._ServerUtils.WriteServerLog($"Missing Parameter(s)\nUsage: GiveItem [username] [itemID] [itemQuantity] [itemQuality]\n", warnColor);
            }
            // TODO: Update this to Where() syntax, this is checking them all even after it finds the right one.
            foreach (ServerClient client in MainProgram._Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    MainProgram._Networking.SendData(client, "GiftedItems│" + itemID + "┼" + itemQuantity + "┼" + itemQuality + "┼");

                    MainProgram._ServerUtils.WriteServerLog($"Item Has Neen Gifted To Player [{client.username}]\n", messageColor);
                }
            }
            MainProgram._ServerUtils.WriteServerLog($"Player {clientID} Not Found\n", warnColor);
        }
        public void GiveItemAll(string command)
        {
            // TODO: Prescreen the length and get rid of all the repeated try/catches.
            string itemID = "";
            try { itemID = command.Split(' ')[1]; }
            catch
            {
                MainProgram._ServerUtils.WriteServerLog($"Missing Parameter(s)\nUsage: Giveitemall [itemID] [itemQuantity] [itemQuality]\n", warnColor);
            }
            string itemQuantity = "";
            try { itemQuantity = command.Split(' ')[2]; }
            catch
            {
                MainProgram._ServerUtils.WriteServerLog($"Missing Parameter(s)\nUsage: Giveitemall [itemID] [itemQuantity] [itemQuality]\n", warnColor);
            }
            string itemQuality = "";
            try { itemQuality = command.Split(' ')[3]; }
            catch
            {
                MainProgram._ServerUtils.WriteServerLog($"Missing Parameter(s)\nUsage: Giveitemall [itemID] [itemQuantity] [itemQuality]\n", warnColor);
            }
            foreach (ServerClient client in MainProgram._Networking.connectedClients)
            {
                MainProgram._Networking.SendData(client, "GiftedItems│" + itemID + "┼" + itemQuantity + "┼" + itemQuality + "┼");
                MainProgram._ServerUtils.WriteServerLog($"Item Has Neen Gifted To All Players\n", messageColor);
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
                MainProgram._ServerUtils.WriteServerLog("Missing Parameters\n", warnColor);
            }
            // TODO: Update this to Where() syntax, this is checking them all even after it finds the right one.
            foreach (ServerClient client in MainProgram._Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    client.eventShielded = true;
                    MainProgram.savedClients.Find(fetch => fetch.username == client.username).eventShielded = true;
                    MainProgram._ServerUtils.WriteServerLog($"Player [{client.username}] Has Been Protected\n", messageColor);
                }
            }
            MainProgram._ServerUtils.WriteServerLog($"Player {clientID} Not Found\n", warnColor);
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
                MainProgram._ServerUtils.WriteServerLog("Missing Parameters\n", warnColor);
            }
            // TODO: Update this to Where() syntax, this is checking them all even after it finds the right one.
            foreach (ServerClient client in MainProgram._Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    client.eventShielded = false;
                    MainProgram.savedClients.Find(fetch => fetch.username == client.username).eventShielded = false;
                    MainProgram._ServerUtils.WriteServerLog($"Player [{client.username}] Has Been Deprotected\n", messageColor);
                }
            }
            MainProgram._ServerUtils.WriteServerLog($"Player {clientID} Not Found\n", warnColor);
        }
        public void Immunize(string command)
        {
            string clientID = "";
            try { clientID = command.Split(' ')[1]; }
            catch
            {
                MainProgram._ServerUtils.WriteServerLog("Missing Parameters\n", warnColor);
            }
            // TODO: Update this to Where() syntax, this is checking them all even after it finds the right one.
            foreach (ServerClient client in MainProgram._Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    client.isImmunized = true;
                    MainProgram.savedClients.Find(fetch => fetch.username == client.username).isImmunized = true;
                    SaveSystem.SaveUserData(client);
                    MainProgram._ServerUtils.WriteServerLog($"Player [{client.username}] Has Been Immunized\n", messageColor);
                }
            }
            MainProgram._ServerUtils.WriteServerLog($"Player {clientID} Not Found\n", warnColor);
        }
        public void Deimmunize(string command)
        {
            string clientID = "";
            try { clientID = command.Split(' ')[1]; }
            catch
            {
                MainProgram._ServerUtils.WriteServerLog("Missing Parameters\n", warnColor);
            }
            // TODO: Update this to Where() syntax, this is checking them all even after it finds the right one.
            foreach (ServerClient client in MainProgram._Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    client.isImmunized = false;
                    MainProgram.savedClients.Find(fetch => fetch.username == client.username).isImmunized = false;
                    SaveSystem.SaveUserData(client);
                    MainProgram._ServerUtils.WriteServerLog($"Player [{client.username}] Has Been Deimmunized\n", messageColor);
                }
            }
            MainProgram._ServerUtils.WriteServerLog($"Player {clientID} Not Found\n", warnColor);
        }
        public void AdminList()
        {
            MainProgram.adminList.Clear();
            foreach (ServerClient client in MainProgram.savedClients) if (client.isAdmin) MainProgram.adminList.Add(client.username);
            MainProgram._ServerUtils.WriteServerLog($"Server Administrators: [{MainProgram.adminList.Count}]", messageColor);
            MainProgram._ServerUtils.WriteServerLog(MainProgram.adminList.Count == 0 ? "No Administrators Found\n" : string.Join("\n", MainProgram.adminList.ToArray()) + "\n");
        }
        public void WhiteList()
        {
            MainProgram._ServerUtils.WriteServerLog($"Whitelisted Players: [{MainProgram.whitelistedUsernames.Count}]", messageColor);
            MainProgram._ServerUtils.WriteServerLog(MainProgram.whitelistedUsernames.Count == 0 ? "No Whitelisted Players Found\n" : string.Join("\n", MainProgram.whitelistedUsernames.ToArray()) + "\n");
        }
        public void Wipe()
        {
            MainProgram._ServerUtils.WriteServerLog("WARNING! THIS ACTION WILL IRRECOVERABLY DELETE ALL PLAYER DATA. DO YOU WANT TO PROCEED? (Y/N)", errorColor);
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
                MainProgram._ServerUtils.WriteServerLog("All Player Files Have Been Set To Wipe", errorColor);
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
