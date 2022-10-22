using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;

namespace OpenWorldServer
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
            ConsoleUtils.WriteWithTime("Modlist - Displays Mods Menu");
            ConsoleUtils.WriteWithTime("Reload - Reloads All Available Settings Into The Server");
            ConsoleUtils.WriteWithTime("Status - Shows A General Overview Menu");
            ConsoleUtils.WriteWithTime("Settlements - Displays Settlements Menu");
            ConsoleUtils.WriteWithTime("List - Displays Player List Menu");
            ConsoleUtils.WriteWithTime("Whitelist - Shows All Whitelisted Players");
            ConsoleUtils.WriteWithTime("Clear - Clears The Console");
            ConsoleUtils.WriteWithTime("Exit - Closes The Server");
            Console.WriteLine("");

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Communication:");
            Console.ForegroundColor = ConsoleColor.White;

            ConsoleUtils.WriteWithTime("Say - Send A Chat Message");
            ConsoleUtils.WriteWithTime("Broadcast - Send A Letter To Every Player Connected");
            ConsoleUtils.WriteWithTime("Notify - Send A Letter To X Player");
            ConsoleUtils.WriteWithTime("Chat - Displays Chat Menu");
            Console.WriteLine("");

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
            Console.WriteLine("");

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

            Console.WriteLine("");
        }

        public static void SettingsCommand()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Server Settings:");
            Console.ForegroundColor = ConsoleColor.White;

            ConsoleUtils.WriteWithTime("Server Name: " + Server.serverName);
            ConsoleUtils.WriteWithTime("Server Description: " + Server.serverDescription);
            ConsoleUtils.WriteWithTime("Server Local IP: " + Server._Networking.localAddress);
            ConsoleUtils.WriteWithTime("Server Port: " + Server._Networking.serverPort);
            Console.WriteLine("");

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("World Settings:");
            Console.ForegroundColor = ConsoleColor.White;

            ConsoleUtils.WriteWithTime("Globe Coverage: " + Server.globeCoverage);
            ConsoleUtils.WriteWithTime("Seed: " + Server.seed);
            ConsoleUtils.WriteWithTime("Overall Rainfall: " + Server.overallRainfall);
            ConsoleUtils.WriteWithTime("Overall Temperature: " + Server.overallTemperature);
            ConsoleUtils.WriteWithTime("Overall Population: " + Server.overallPopulation);
            Console.WriteLine("");
        }

        public static void ModListCommand()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Server Enforced Mods: " + Server.enforcedMods.Count);
            Console.ForegroundColor = ConsoleColor.White;

            if (Server.enforcedMods.Count == 0) ConsoleUtils.WriteWithTime("No Enforced Mods Found");
            else foreach (string mod in Server.enforcedMods) ConsoleUtils.WriteWithTime(mod);
            Console.WriteLine("");

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Server Whitelisted Mods: " + Server.whitelistedMods.Count);
            Console.ForegroundColor = ConsoleColor.White;

            if (Server.whitelistedMods.Count == 0) ConsoleUtils.WriteWithTime("No Whitelisted Mods Found");
            else foreach (string whitelistedMod in Server.whitelistedMods) ConsoleUtils.WriteWithTime(whitelistedMod);
            Console.WriteLine("");

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Server Blacklisted Mods: " + Server.blacklistedMods.Count);
            Console.ForegroundColor = ConsoleColor.White;

            if (Server.whitelistedMods.Count == 0) ConsoleUtils.WriteWithTime("No Blacklisted Mods Found");
            else foreach (string blacklistedMod in Server.blacklistedMods) ConsoleUtils.WriteWithTime(blacklistedMod);
            Console.WriteLine("");
        }

        //Check this one
        public static void ExitCommand()
        {
            List<ServerClient> clientsToKick = new List<ServerClient>();

            foreach (ServerClient sc in Server._Networking.connectedClients)
            {
                clientsToKick.Add(sc);
            }

            foreach (ServerClient sc in clientsToKick)
            {
                Server._Networking.SendData(sc, "Disconnect│Closing");
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

            ServerUtils.CheckAllAvailableMods(false);
            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Mods Have Been Reloaded");
            Console.WriteLine("");

            PlayerUtils.CheckAllAvailablePlayers(false);
            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Whitelisted Players Have Been Reloaded");
            Console.WriteLine("");
        }

        public static void StatusCommand()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Server Status");
            Console.ForegroundColor = ConsoleColor.White;

            ConsoleUtils.WriteWithTime("Version: " + Server.serverVersion);
            ConsoleUtils.WriteWithTime("Connection: Online");
            ConsoleUtils.WriteWithTime("Uptime: " + (DateTime.UtcNow - Process.GetCurrentProcess().StartTime.ToUniversalTime()));
            Console.WriteLine("");

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Mods:");
            Console.ForegroundColor = ConsoleColor.White;

            ConsoleUtils.WriteWithTime("Enforced Mods: " + Server.enforcedMods.Count);
            ConsoleUtils.WriteWithTime("Whitelisted Mods: " + Server.whitelistedMods.Count);
            ConsoleUtils.WriteWithTime("Blacklisted Mods: " + Server.blacklistedMods.Count);
            Console.WriteLine("");

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Players:");
            Console.ForegroundColor = ConsoleColor.White;

            ConsoleUtils.WriteWithTime("Connected Players: " + Server._Networking.connectedClients.Count);
            ConsoleUtils.WriteWithTime("Saved Players: " + Server.savedClients.Count);
            ConsoleUtils.WriteWithTime("Saved Settlements: " + Server.savedSettlements.Count);
            ConsoleUtils.WriteWithTime("Whitelisted Players: " + Server.whitelistedUsernames.Count);
            ConsoleUtils.WriteWithTime("Max Players: " + Server.maxPlayers);
            Console.WriteLine("");

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Modlist Settings:");
            Console.ForegroundColor = ConsoleColor.White;

            ConsoleUtils.WriteWithTime("Using Modlist Check: " + Server.forceModlist);
            ConsoleUtils.WriteWithTime("Using Modlist Config Check: " + Server.forceModlistConfigs);
            ConsoleUtils.WriteWithTime("Using Mod Verification: " + Server.usingModVerification);
            Console.WriteLine("");

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Chat Settings:");
            Console.ForegroundColor = ConsoleColor.White;

            ConsoleUtils.WriteWithTime("Using Chat: " + Server.usingChat);
            ConsoleUtils.WriteWithTime("Using Profanity Filter: " + Server.usingProfanityFilter);
            Console.WriteLine("");

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Wealth Settings:");
            Console.ForegroundColor = ConsoleColor.White;

            ConsoleUtils.WriteWithTime("Using Wealth System: " + Server.usingWealthSystem);
            ConsoleUtils.WriteWithTime("Warning Threshold: " + Server.warningWealthThreshold);
            ConsoleUtils.WriteWithTime("Ban Threshold: " + Server.banWealthThreshold);
            Console.WriteLine("");

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Idle Settings:");
            Console.ForegroundColor = ConsoleColor.White;

            ConsoleUtils.WriteWithTime("Using Idle System: " + Server.usingIdleTimer);
            ConsoleUtils.WriteWithTime("Idle Threshold: " + Server.idleTimer);
            Console.WriteLine("");

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Road Settings:");
            Console.ForegroundColor = ConsoleColor.White;

            ConsoleUtils.WriteWithTime("Using Road System: " + Server.usingRoadSystem);
            ConsoleUtils.WriteWithTime("Aggressive Road Mode: " + Server.aggressiveRoadMode);
            Console.WriteLine("");

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Miscellaneous Settings");
            Console.ForegroundColor = ConsoleColor.White;

            ConsoleUtils.WriteWithTime("Using Whitelist: " + Server.usingWhitelist);
            ConsoleUtils.WriteWithTime("Allow Dev Mode: " + Server.allowDevMode);

            Console.WriteLine("");
        }

        //Administration

        public static void WhiteListCommand()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Whitelisted Players: " + Server.whitelistedUsernames.Count);
            Console.ForegroundColor = ConsoleColor.White;

            if (Server.whitelistedUsernames.Count == 0) ConsoleUtils.WriteWithTime("No Whitelisted Players Found");
            else foreach (string str in Server.whitelistedUsernames) ConsoleUtils.WriteWithTime("" + str);

            Console.WriteLine("");
        }

        //Check this one
        public static void AdminListCommand()
        {
            Console.Clear();

            Server.adminList.Clear();

            foreach (ServerClient client in Server.savedClients)
            {
                if (client.isAdmin) Server.adminList.Add(client.username);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Server Administrators: " + Server.adminList.Count);
            Console.ForegroundColor = ConsoleColor.White;

            if (Server.adminList.Count == 0) ConsoleUtils.WriteWithTime("No Administrators Found");
            else foreach (string str in Server.adminList) ConsoleUtils.WriteWithTime("" + str);

            Console.WriteLine("");
        }

        public static void BanListCommand()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Banned players: " + Server.bannedIPs.Count);
            Console.ForegroundColor = ConsoleColor.White;

            if (Server.bannedIPs.Count == 0) ConsoleUtils.WriteWithTime("No Banned Players");
            else foreach (KeyValuePair<string, string> pair in Server.bannedIPs)
                {
                    ConsoleUtils.WriteWithTime("[" + pair.Value + "] - [" + pair.Key + "]");
                }

            Console.WriteLine("");
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
                foreach (ServerClient client in Server._Networking.connectedClients)
                {
                    client.disconnectFlag = true;
                }

                Thread.Sleep(1000);

                foreach (ServerClient client in Server.savedClients)
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
            ConsoleUtils.WriteWithTime("Connected Players: " + Server._Networking.connectedClients.Count);
            Console.ForegroundColor = ConsoleColor.White;

            if (Server._Networking.connectedClients.Count == 0) ConsoleUtils.WriteWithTime("No Players Connected");
            else foreach (ServerClient client in Server._Networking.connectedClients)
            {
                try { ConsoleUtils.WriteWithTime("" + client.username); }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    ConsoleUtils.WriteWithTime("Error Processing Player With IP " + ((IPEndPoint)client.tcp.Client.RemoteEndPoint).Address.ToString());
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }

            Console.WriteLine("");

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Saved Players: " + Server.savedClients.Count);
            Console.ForegroundColor = ConsoleColor.White;

            if (Server.savedClients.Count == 0) ConsoleUtils.WriteWithTime("No Players Saved");
            else foreach (ServerClient savedClient in Server.savedClients)
            {
                try { ConsoleUtils.WriteWithTime("" + savedClient.username); }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    ConsoleUtils.WriteWithTime("Error Processing Player With IP " + ((IPEndPoint)savedClient.tcp.Client.RemoteEndPoint).Address.ToString());
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }

            Console.WriteLine("");
        }

        public static void SettlementsCommand()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Server Settlements: " + Server.savedSettlements.Count);
            Console.ForegroundColor = ConsoleColor.White;

            if (Server.savedSettlements.Count == 0) ConsoleUtils.WriteWithTime("No Active Settlements");
            else foreach (KeyValuePair<string, List<string>> pair in Server.savedSettlements)
                {
                    ConsoleUtils.WriteWithTime("[" + pair.Key + "] - [" + pair.Value[0] + "]");
                }

            Console.WriteLine("");
        }

        public static void ChatCommand()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Server Chat:");
            Console.ForegroundColor = ConsoleColor.White;

            if (Server.chatCache.Count == 0) ConsoleUtils.WriteWithTime("No Chat Messages");
            else foreach (string message in Server.chatCache)
                {
                    ConsoleUtils.WriteWithTime(message);
                }

            Console.WriteLine("");
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

            Console.WriteLine("");
        }

        //Unknown

        public static void UnknownCommand(string command)
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.WriteWithTime("Command [" + command + "] Not Found");
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("");
        }
    }
}