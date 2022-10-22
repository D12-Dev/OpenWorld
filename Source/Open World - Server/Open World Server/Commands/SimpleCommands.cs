using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;

namespace Open_World_Server
{
    public static class SimpleCommands
    {
        //Miscellaneous

        public static void HelpCommand()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("List Of Available Commands:");
            Console.ForegroundColor = ConsoleColor.White;

            ConsoleUtils.WriteWithTime("Help - Displays Help Menu");
            ConsoleUtils.WriteWithTime("Settings - Displays Settings Menu");
            ConsoleUtils.WriteWithTime("Reload - Reloads All Available Settings Into The Server");
            ConsoleUtils.WriteWithTime("Status - Shows A General Overview Menu");
            ConsoleUtils.WriteWithTime("Settlements - Displays Settlements Menu");
            ConsoleUtils.WriteWithTime("List - Displays Player List Menu");
            ConsoleUtils.WriteWithTime("Whitelist - Shows All Whitelisted Players");
            ConsoleUtils.WriteWithTime("Clear - Clears The Console");
            ConsoleUtils.WriteWithTime("Exit - Exit - Closes The Server");
            ConsoleUtils.WriteWithTime("");

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Communication:");
            Console.ForegroundColor = ConsoleColor.White;

            ConsoleUtils.WriteWithTime("Say - Send A Chat Message");
            ConsoleUtils.WriteWithTime("Broadcast - Send A Letter To Every Player Connected");
            ConsoleUtils.WriteWithTime("Notify - Send A Letter To X Player");
            ConsoleUtils.WriteWithTime("Chat - Displays Chat Menu");
            ConsoleUtils.WriteWithTime("");

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Interaction:");
            Console.ForegroundColor = ConsoleColor.White;

            ConsoleUtils.WriteWithTime("Invoke - Invokes An Event To X Player");
            ConsoleUtils.WriteWithTime("Plague - Invokes An Event To All Connected Players");
            ConsoleUtils.WriteWithTime("Eventlist - Shows All Available Events");
            ConsoleUtils.WriteWithTime("GiveItem - Gives An Item To X Player");
            ConsoleUtils.WriteWithTime("GiveItemAll - Gives An Item To All Players");
            ConsoleUtils.WriteWithTime("Protect - Protects A Player From Any Event Temporarily");
            ConsoleUtils.WriteWithTime("Deprotect - Disables All Protections Given To X Player");
            ConsoleUtils.WriteWithTime("Immunize - Protects A Player From Any Event Permanently");
            ConsoleUtils.WriteWithTime("Deimmunize - Disables The Immunity Given To X Player");
            ConsoleUtils.WriteWithTime("");

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Admin Control:");
            Console.ForegroundColor = ConsoleColor.White;

            ConsoleUtils.WriteWithTime("Investigate - Displays All Data About X Player");
            ConsoleUtils.WriteWithTime("Promote - Promotes X Player To Admin");
            ConsoleUtils.WriteWithTime("Demote - Demotes X Player");
            ConsoleUtils.WriteWithTime("Adminlist - Shows All Server Admins");
            ConsoleUtils.WriteWithTime("Kick - Kicks X Player");
            ConsoleUtils.WriteWithTime("Ban - Bans X Player");
            ConsoleUtils.WriteWithTime("Pardon - Pardons X Player");
            ConsoleUtils.WriteWithTime("Banlist - Shows All Banned Players");
            ConsoleUtils.WriteWithTime("Wipe - Deletes Every Player Data In The Server");

            ConsoleUtils.WriteWithTime("");
        }

        public static void SettingsCommand()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Server Settings:");
            Console.ForegroundColor = ConsoleColor.White;

            ConsoleUtils.WriteWithTime("Server Name: " + MainProgram.serverName);
            ConsoleUtils.WriteWithTime("Server Description: " + MainProgram.serverDescription);
            ConsoleUtils.WriteWithTime("Server Local IP: " + MainProgram._Networking.localAddress);
            ConsoleUtils.WriteWithTime("Server Port: " + MainProgram._Networking.serverPort);
            ConsoleUtils.WriteWithTime("");

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("World Settings:");
            Console.ForegroundColor = ConsoleColor.White;

            ConsoleUtils.WriteWithTime("Globe Coverage: " + MainProgram.globeCoverage);
            ConsoleUtils.WriteWithTime("Seed: " + MainProgram.seed);
            ConsoleUtils.WriteWithTime("Overall Rainfall: " + MainProgram.overallRainfall);
            ConsoleUtils.WriteWithTime("Overall Temperature: " + MainProgram.overallTemperature);
            ConsoleUtils.WriteWithTime("Overall Population: " + MainProgram.overallPopulation);
            ConsoleUtils.WriteWithTime("");

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Server Mods: " + MainProgram.modList.Count);
            Console.ForegroundColor = ConsoleColor.White;

            if (MainProgram.modList.Count == 0) ConsoleUtils.WriteWithTime("No Mods Found");
            else foreach (string mod in MainProgram.modList) ConsoleUtils.WriteWithTime(mod);
            ConsoleUtils.WriteWithTime("");

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Server Whitelisted Mods: " + MainProgram.whitelistedMods.Count);
            Console.ForegroundColor = ConsoleColor.White;

            if (MainProgram.whitelistedMods.Count == 0) ConsoleUtils.WriteWithTime("No Whitelisted Mods Found");
            else foreach (string whitelistedMod in MainProgram.whitelistedMods) ConsoleUtils.WriteWithTime(whitelistedMod);

            ConsoleUtils.WriteWithTime("");
        }

        //Check this one
        public static void ExitCommand()
        {
            List<ServerClient> clientsToKick = new List<ServerClient>();

            foreach (ServerClient sc in MainProgram._Networking.connectedClients)
            {
                clientsToKick.Add(sc);
            }

            foreach (ServerClient sc in clientsToKick)
            {
                MainProgram._Networking.SendData(sc, "Disconnect│Closing");
                sc.disconnectFlag = true;
            }

            Environment.Exit(0);
        }

        public static void ClearCommand()
        {
            Console.Clear();
        }

        public static void ReloadCommand()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Reloading All Current Mods");

            MainProgram._ServerUtils.CheckMods();
            MainProgram._ServerUtils.CheckWhitelistedMods();

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Mods Have Been Reloaded");
            ConsoleUtils.WriteWithTime("");

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Reloading All Whitelisted Players");

            Console.ForegroundColor = ConsoleColor.Green;
            MainProgram._ServerUtils.CheckForWhitelistedPlayers();

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Whitelisted Players Have Been Reloaded");

            ConsoleUtils.WriteWithTime("");
        }

        public static void StatusCommand()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Server Status");
            Console.ForegroundColor = ConsoleColor.White;

            ConsoleUtils.WriteWithTime("Version: " + MainProgram.serverVersion);
            ConsoleUtils.WriteWithTime("Connection: Online");
            ConsoleUtils.WriteWithTime("Uptime: " + (DateTime.UtcNow - Process.GetCurrentProcess().StartTime.ToUniversalTime()));
            ConsoleUtils.WriteWithTime("");

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Mods:");
            Console.ForegroundColor = ConsoleColor.White;

            ConsoleUtils.WriteWithTime("Mods: " + MainProgram.modList.Count);
            ConsoleUtils.WriteWithTime("Whitelisted Mods: " + MainProgram.whitelistedMods.Count);
            ConsoleUtils.WriteWithTime("");

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Players:");
            Console.ForegroundColor = ConsoleColor.White;

            ConsoleUtils.WriteWithTime("Connected Players: " + MainProgram._Networking.connectedClients.Count);
            ConsoleUtils.WriteWithTime("Saved Players: " + MainProgram.savedClients.Count);
            ConsoleUtils.WriteWithTime("Saved Settlements: " + MainProgram.savedSettlements.Count);
            ConsoleUtils.WriteWithTime("Whitelisted Players: " + MainProgram.whitelistedUsernames.Count);
            ConsoleUtils.WriteWithTime("Max Players: " + MainProgram.maxPlayers);
            ConsoleUtils.WriteWithTime("");

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Modlist Settings:");
            Console.ForegroundColor = ConsoleColor.White;

            ConsoleUtils.WriteWithTime("Using Modlist Check: " + MainProgram.forceModlist);
            ConsoleUtils.WriteWithTime("Using Modlist Config Check: " + MainProgram.forceModlistConfigs);
            ConsoleUtils.WriteWithTime("Using Mod Verification: " + MainProgram.usingModVerification);
            ConsoleUtils.WriteWithTime("");

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Chat Settings:");
            Console.ForegroundColor = ConsoleColor.White;

            ConsoleUtils.WriteWithTime("Using Chat: " + MainProgram.usingChat);
            ConsoleUtils.WriteWithTime("Using Profanity Filter: " + MainProgram.usingProfanityFilter);
            ConsoleUtils.WriteWithTime("");

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Wealth Settings:");
            Console.ForegroundColor = ConsoleColor.White;

            ConsoleUtils.WriteWithTime("Using Wealth System: " + MainProgram.usingWealthSystem);
            ConsoleUtils.WriteWithTime("Warning Threshold: " + MainProgram.warningWealthThreshold);
            ConsoleUtils.WriteWithTime("Ban Threshold: " + MainProgram.banWealthThreshold);
            ConsoleUtils.WriteWithTime("");

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Idle Settings:");
            Console.ForegroundColor = ConsoleColor.White;

            ConsoleUtils.WriteWithTime("Using Idle System: " + MainProgram.usingIdleTimer);
            ConsoleUtils.WriteWithTime("Idle Threshold: " + MainProgram.idleTimer);
            ConsoleUtils.WriteWithTime("");

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Road Settings:");
            Console.ForegroundColor = ConsoleColor.White;

            ConsoleUtils.WriteWithTime("Using Road System: " + MainProgram.usingRoadSystem);
            ConsoleUtils.WriteWithTime("Aggressive Road Mode: " + MainProgram.aggressiveRoadMode);
            ConsoleUtils.WriteWithTime("");

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Miscellaneous Settings");
            Console.ForegroundColor = ConsoleColor.White;

            ConsoleUtils.WriteWithTime("Using Whitelist: " + MainProgram.usingWhitelist);
            ConsoleUtils.WriteWithTime("Allow Dev Mode: " + MainProgram.allowDevMode);

            ConsoleUtils.WriteWithTime("");
        }

        //Administration

        public static void WhiteListCommand()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Whitelisted Players: " + MainProgram.whitelistedUsernames.Count);
            Console.ForegroundColor = ConsoleColor.White;

            if (MainProgram.whitelistedUsernames.Count == 0) ConsoleUtils.WriteWithTime("No Whitelisted Players Found");
            else foreach (string str in MainProgram.whitelistedUsernames) ConsoleUtils.WriteWithTime("" + str);

            ConsoleUtils.WriteWithTime("");
        }

        //Check this one
        public static void AdminListCommand()
        {
            Console.Clear();

            MainProgram.adminList.Clear();

            foreach (ServerClient client in MainProgram.savedClients)
            {
                if (client.isAdmin) MainProgram.adminList.Add(client.username);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Server Administrators: " + MainProgram.adminList.Count);
            Console.ForegroundColor = ConsoleColor.White;

            if (MainProgram.adminList.Count == 0) ConsoleUtils.WriteWithTime("No Administrators Found");
            else foreach (string str in MainProgram.adminList) ConsoleUtils.WriteWithTime("" + str);

            ConsoleUtils.WriteWithTime("");
        }

        public static void BanListCommand()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Banned players: " + MainProgram.bannedIPs.Count);
            Console.ForegroundColor = ConsoleColor.White;

            if (MainProgram.bannedIPs.Count == 0) ConsoleUtils.WriteWithTime("No Banned Players");
            else foreach (KeyValuePair<string, string> pair in MainProgram.bannedIPs)
                {
                    ConsoleUtils.WriteWithTime("[" + pair.Value + "] - [" + pair.Key + "]");
                }

            ConsoleUtils.WriteWithTime("");
        }

        public static void WipeCommand()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Red;
            ConsoleUtils.WriteWithTime("WARNING! THIS ACTION WILL DELETE ALL PLAYER DATA. DO YOU WANT TO PROCEED? (Y/N)");
            Console.ForegroundColor = ConsoleColor.White;

            string response = Console.ReadLine();

            if (response == "Y")
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

                Console.ForegroundColor = ConsoleColor.Red;
                ConsoleUtils.WriteWithTime("All Player Files Have Been Set To Wipe");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else Console.Clear();
        }

        //Player Interaction

        public static void ListCommand()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Connected Players: " + MainProgram._Networking.connectedClients.Count);
            Console.ForegroundColor = ConsoleColor.White;

            if (MainProgram._Networking.connectedClients.Count == 0) ConsoleUtils.WriteWithTime("No Players Connected");
            else foreach (ServerClient client in MainProgram._Networking.connectedClients)
                {
                    try { ConsoleUtils.WriteWithTime("" + client.username); }
                    catch
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        ConsoleUtils.WriteWithTime("Error Processing Player With IP " + ((IPEndPoint)client.tcp.Client.RemoteEndPoint).Address.ToString());
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }

            ConsoleUtils.WriteWithTime("");

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Saved Players: " + MainProgram.savedClients.Count);
            Console.ForegroundColor = ConsoleColor.White;

            if (MainProgram.savedClients.Count == 0) ConsoleUtils.WriteWithTime("No Players Saved");
            else foreach (ServerClient savedClient in MainProgram.savedClients)
                {
                    try { ConsoleUtils.WriteWithTime("" + savedClient.username); }
                    catch
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        ConsoleUtils.WriteWithTime("Error Processing Player With IP " + ((IPEndPoint)savedClient.tcp.Client.RemoteEndPoint).Address.ToString());
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }

            ConsoleUtils.WriteWithTime("");
        }

        public static void SettlementsCommand()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Server Settlements: " + MainProgram.savedSettlements.Count);
            Console.ForegroundColor = ConsoleColor.White;

            if (MainProgram.savedSettlements.Count == 0) ConsoleUtils.WriteWithTime("No Active Settlements");
            else foreach (KeyValuePair<string, List<string>> pair in MainProgram.savedSettlements)
                {
                    ConsoleUtils.WriteWithTime("[" + pair.Key + "] - [" + pair.Value[0] + "]");
                }

            ConsoleUtils.WriteWithTime("");
        }

        public static void ChatCommand()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Server Chat:");
            Console.ForegroundColor = ConsoleColor.White;

            if (MainProgram.chatCache.Count == 0) ConsoleUtils.WriteWithTime("No Chat Messages");
            else foreach (string message in MainProgram.chatCache)
                {
                    ConsoleUtils.WriteWithTime(message);
                }

            ConsoleUtils.WriteWithTime("");
        }

        public static void EventListCommand()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("List Of Available Events:");

            Console.ForegroundColor = ConsoleColor.White;
            ConsoleUtils.WriteWithTime("Raid");
            ConsoleUtils.WriteWithTime("Infestation");
            ConsoleUtils.WriteWithTime("MechCluster");
            ConsoleUtils.WriteWithTime("ToxicFallout");
            ConsoleUtils.WriteWithTime("Manhunter");
            ConsoleUtils.WriteWithTime("Wanderer");
            ConsoleUtils.WriteWithTime("FarmAnimals");
            ConsoleUtils.WriteWithTime("ShipChunk");
            ConsoleUtils.WriteWithTime("GiveQuest");
            ConsoleUtils.WriteWithTime("TraderCaravan");

            ConsoleUtils.WriteWithTime("");
        }

        //Unknown

        public static void UnknownCommand(string command)
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Command [" + command + "] Not Found");
            Console.ForegroundColor = ConsoleColor.White;

            ConsoleUtils.WriteWithTime("");
        }
    }
}