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
            ConsoleUtils.LogToConsole("List Of Available Commands:");
            Console.ForegroundColor = ConsoleColor.White;

            ConsoleUtils.LogToConsole("Help - Displays Help Menu");
            ConsoleUtils.LogToConsole("Settings - Displays Settings Menu");
            ConsoleUtils.LogToConsole("Modlist - Displays Mods Menu");
            ConsoleUtils.LogToConsole("List - Displays Player List Menu");
            ConsoleUtils.LogToConsole("Whitelist - Shows All Whitelisted Players");
            ConsoleUtils.LogToConsole("Settlements - Displays Settlements Menu");
            ConsoleUtils.LogToConsole("Faction - Displays All Data About X Faction");
            ConsoleUtils.LogToConsole("Reload - Reloads All Available Settings Into The Server");
            ConsoleUtils.LogToConsole("Status - Shows A General Overview Menu");
            ConsoleUtils.LogToConsole("Clear - Clears The Console");
            ConsoleUtils.LogToConsole("Exit - Closes The Server");
            ConsoleUtils.LogToConsole("");;

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.LogToConsole("Communication:");
            Console.ForegroundColor = ConsoleColor.White;

            ConsoleUtils.LogToConsole("Say - Send A Chat Message");
            ConsoleUtils.LogToConsole("Broadcast - Send A Letter To Every Player Connected");
            ConsoleUtils.LogToConsole("Notify - Send A Letter To X Player");
            ConsoleUtils.LogToConsole("Chat - Displays Chat Menu");
            ConsoleUtils.LogToConsole("");;

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.LogToConsole("Interaction:");
            Console.ForegroundColor = ConsoleColor.White;

            ConsoleUtils.LogToConsole("Invoke - Invokes An Event To X Player");
            ConsoleUtils.LogToConsole("Plague - Invokes An Event To All Connected Players");
            ConsoleUtils.LogToConsole("Eventlist - Shows All Available Events");
            ConsoleUtils.LogToConsole("GiveItem - Gives An Item To X Player");
            ConsoleUtils.LogToConsole("GiveItemAll - Gives An Item To All Players");
            ConsoleUtils.LogToConsole("Protect - Protects A Player From Any Event Temporarily");
            ConsoleUtils.LogToConsole("Deprotect - Disables All Protections Given To X Player");
            ConsoleUtils.LogToConsole("Immunize - Protects A Player From Any Event Permanently");
            ConsoleUtils.LogToConsole("Deimmunize - Disables The Immunity Given To X Player");
            ConsoleUtils.LogToConsole("");;

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.LogToConsole("Admin Control:");
            Console.ForegroundColor = ConsoleColor.White;

            ConsoleUtils.LogToConsole("Player - Displays All Data About X Player");
            ConsoleUtils.LogToConsole("Promote - Promotes X Player To Admin");
            ConsoleUtils.LogToConsole("Demote - Demotes X Player");
            ConsoleUtils.LogToConsole("Adminlist - Shows All Server Admins");
            ConsoleUtils.LogToConsole("Kick - Kicks X Player");
            ConsoleUtils.LogToConsole("Ban - Bans X Player");
            ConsoleUtils.LogToConsole("Pardon - Pardons X Player");
            ConsoleUtils.LogToConsole("Banlist - Shows All Banned Players");
            ConsoleUtils.LogToConsole("Wipe - Deletes Every Player Data In The Server");

            ConsoleUtils.LogToConsole("");;
        }

        public static void SettingsCommand()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.LogToConsole("Server Settings:");
            Console.ForegroundColor = ConsoleColor.White;

            ConsoleUtils.LogToConsole("Server Name: " + Server.serverName);
            ConsoleUtils.LogToConsole("Server Description: " + Server.serverDescription);
            ConsoleUtils.LogToConsole("Server Local IP: " + Networking.localAddress);
            ConsoleUtils.LogToConsole("Server Port: " + Networking.serverPort);
            ConsoleUtils.LogToConsole("");;

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.LogToConsole("World Settings:");
            Console.ForegroundColor = ConsoleColor.White;

            ConsoleUtils.LogToConsole("Globe Coverage: " + Server.globeCoverage);
            ConsoleUtils.LogToConsole("Seed: " + Server.seed);
            ConsoleUtils.LogToConsole("Overall Rainfall: " + Server.overallRainfall);
            ConsoleUtils.LogToConsole("Overall Temperature: " + Server.overallTemperature);
            ConsoleUtils.LogToConsole("Overall Population: " + Server.overallPopulation);
            ConsoleUtils.LogToConsole("");;
        }

        public static void ModListCommand()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.LogToConsole("Server Enforced Mods: " + Server.enforcedMods.Count);
            Console.ForegroundColor = ConsoleColor.White;

            if (Server.enforcedMods.Count == 0) ConsoleUtils.LogToConsole("No Enforced Mods Found");
            else foreach (string mod in Server.enforcedMods) ConsoleUtils.LogToConsole(mod);
            ConsoleUtils.LogToConsole("");;

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.LogToConsole("Server Whitelisted Mods: " + Server.whitelistedMods.Count);
            Console.ForegroundColor = ConsoleColor.White;

            if (Server.whitelistedMods.Count == 0) ConsoleUtils.LogToConsole("No Whitelisted Mods Found");
            else foreach (string whitelistedMod in Server.whitelistedMods) ConsoleUtils.LogToConsole(whitelistedMod);
            ConsoleUtils.LogToConsole("");;

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.LogToConsole("Server Blacklisted Mods: " + Server.blacklistedMods.Count);
            Console.ForegroundColor = ConsoleColor.White;

            if (Server.whitelistedMods.Count == 0) ConsoleUtils.LogToConsole("No Blacklisted Mods Found");
            else foreach (string blacklistedMod in Server.blacklistedMods) ConsoleUtils.LogToConsole(blacklistedMod);
            ConsoleUtils.LogToConsole("");;
        }

        public static void ExitCommand()
        {
            ServerClient[] clientsToKick = Networking.connectedClients.ToArray();
            foreach (ServerClient sc in clientsToKick)
            {
                Networking.SendData(sc, "Disconnect│Closing");
                sc.disconnectFlag = true;
            }

            Server.exit = true;
        }

        public static void ClearCommand()
        {
            Console.Clear();
        }

        public static void ReloadCommand()
        {
            Console.Clear();

            ModHandler.CheckMods(false);
            Console.ForegroundColor = ConsoleColor.Green;

            WorldHandler.CheckWorldFile();
            Console.ForegroundColor = ConsoleColor.Green;

            FactionHandler.CheckFactions(false);
            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.LogToConsole("");;

            PlayerUtils.CheckAllAvailablePlayers(false);
            Console.ForegroundColor = ConsoleColor.Green;
        }

        public static void StatusCommand()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.LogToConsole("Server Status");
            Console.ForegroundColor = ConsoleColor.White;

            ConsoleUtils.LogToConsole("Version: " + Server.serverVersion);
            ConsoleUtils.LogToConsole("Connection: Online");
            ConsoleUtils.LogToConsole("Uptime: " + (DateTime.UtcNow - Process.GetCurrentProcess().StartTime.ToUniversalTime()));
            ConsoleUtils.LogToConsole("");;

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.LogToConsole("Mods:");
            Console.ForegroundColor = ConsoleColor.White;

            ConsoleUtils.LogToConsole("Enforced Mods: " + Server.enforcedMods.Count);
            ConsoleUtils.LogToConsole("Whitelisted Mods: " + Server.whitelistedMods.Count);
            ConsoleUtils.LogToConsole("Blacklisted Mods: " + Server.blacklistedMods.Count);
            ConsoleUtils.LogToConsole("");;

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.LogToConsole("Players:");
            Console.ForegroundColor = ConsoleColor.White;

            ConsoleUtils.LogToConsole("Connected Players: " + Networking.connectedClients.Count);
            ConsoleUtils.LogToConsole("Saved Players: " + Server.savedClients.Count);
            ConsoleUtils.LogToConsole("Saved Settlements: " + Server.savedSettlements.Count);
            ConsoleUtils.LogToConsole("Whitelisted Players: " + Server.whitelistedUsernames.Count);
            ConsoleUtils.LogToConsole("Max Players: " + Server.maxPlayers);
            ConsoleUtils.LogToConsole("");;

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.LogToConsole("Modlist Settings:");
            Console.ForegroundColor = ConsoleColor.White;

            ConsoleUtils.LogToConsole("Using Modlist Check: " + Server.forceModlist);
            ConsoleUtils.LogToConsole("Using Modlist Config Check: " + Server.forceModlistConfigs);
            ConsoleUtils.LogToConsole("Using Mod Verification: " + Server.usingModVerification);
            ConsoleUtils.LogToConsole("");;

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.LogToConsole("Chat Settings:");
            Console.ForegroundColor = ConsoleColor.White;

            ConsoleUtils.LogToConsole("Using Chat: " + Server.usingChat);
            ConsoleUtils.LogToConsole("Using Profanity Filter: " + Server.usingProfanityFilter);
            ConsoleUtils.LogToConsole("");;

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.LogToConsole("Wealth Settings:");
            Console.ForegroundColor = ConsoleColor.White;

            ConsoleUtils.LogToConsole("Using Wealth System: " + Server.usingWealthSystem);
            ConsoleUtils.LogToConsole("Warning Threshold: " + Server.warningWealthThreshold);
            ConsoleUtils.LogToConsole("Ban Threshold: " + Server.banWealthThreshold);
            ConsoleUtils.LogToConsole("");;

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.LogToConsole("Idle Settings:");
            Console.ForegroundColor = ConsoleColor.White;

            ConsoleUtils.LogToConsole("Using Idle System: " + Server.usingIdleTimer);
            ConsoleUtils.LogToConsole("Idle Threshold: " + Server.idleTimer);
            ConsoleUtils.LogToConsole("");;

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.LogToConsole("Road Settings:");
            Console.ForegroundColor = ConsoleColor.White;

            ConsoleUtils.LogToConsole("Using Road System: " + Server.usingRoadSystem);
            ConsoleUtils.LogToConsole("Aggressive Road Mode: " + Server.aggressiveRoadMode);
            ConsoleUtils.LogToConsole("");;

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.LogToConsole("Miscellaneous Settings");
            Console.ForegroundColor = ConsoleColor.White;

            ConsoleUtils.LogToConsole("Using Whitelist: " + Server.usingWhitelist);
            ConsoleUtils.LogToConsole("Using Enforced Difficulty: " + Server.usingEnforcedDifficulty);
            ConsoleUtils.LogToConsole("Allow Dev Mode: " + Server.allowDevMode);

            ConsoleUtils.LogToConsole("");;
        }

        //Administration

        public static void WhiteListCommand()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.LogToConsole("Whitelisted Players: " + Server.whitelistedUsernames.Count);
            Console.ForegroundColor = ConsoleColor.White;

            if (Server.whitelistedUsernames.Count == 0) ConsoleUtils.LogToConsole("No Whitelisted Players Found");
            else foreach (string str in Server.whitelistedUsernames) ConsoleUtils.LogToConsole("" + str);

            ConsoleUtils.LogToConsole("");;
        }

        //Check this one
        public static void AdminListCommand()
        {
            Console.Clear();

            Server.adminList.Clear();

            ServerClient[] savedClients = Server.savedClients.ToArray();
            foreach (ServerClient client in savedClients)
            {
                if (client.isAdmin) Server.adminList.Add(client.username);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.LogToConsole("Server Administrators: " + Server.adminList.Count);
            Console.ForegroundColor = ConsoleColor.White;

            if (Server.adminList.Count == 0) ConsoleUtils.LogToConsole("No Administrators Found");
            else foreach (string str in Server.adminList) ConsoleUtils.LogToConsole("" + str);

            ConsoleUtils.LogToConsole("");;
        }

        public static void BanListCommand()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.LogToConsole("Banned players: " + Server.bannedIPs.Count);
            Console.ForegroundColor = ConsoleColor.White;

            if (Server.bannedIPs.Count == 0) ConsoleUtils.LogToConsole("No Banned Players");
            else
            {
                Dictionary<string, string> bannedIPs = Server.bannedIPs;
                foreach (KeyValuePair<string, string> pair in bannedIPs)
                {
                    ConsoleUtils.LogToConsole("[" + pair.Value + "] - [" + pair.Key + "]");
                }
            }

            ConsoleUtils.LogToConsole("");;
        }

        public static void WipeCommand()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Red;
            ConsoleUtils.LogToConsole("WARNING! THIS ACTION WILL DELETE ALL PLAYER DATA. DO YOU WANT TO PROCEED? (Y/N)");
            Console.ForegroundColor = ConsoleColor.White;

            string response = Console.ReadLine();

            if (response == "Y")
            {
                ServerClient[] clients = Networking.connectedClients.ToArray();
                foreach (ServerClient client in clients)
                {
                    client.disconnectFlag = true;
                }

                ServerClient[] savedClients = Server.savedClients.ToArray();
                foreach (ServerClient client in savedClients)
                {
                    client.wealth = 0;
                    client.pawnCount = 0;
                    PlayerUtils.SavePlayer(client);
                }

                Console.Clear();

                Console.ForegroundColor = ConsoleColor.Red;
                ConsoleUtils.LogToConsole("All Player Files Have Been Set To Wipe");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else Console.Clear();
        }

        //Player Interaction

        public static void ListCommand()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.LogToConsole("Connected Players: " + Networking.connectedClients.Count);
            Console.ForegroundColor = ConsoleColor.White;

            if (Networking.connectedClients.Count == 0) ConsoleUtils.LogToConsole("No Players Connected");
            else
            {
                ServerClient[] clients = Networking.connectedClients.ToArray();
                foreach (ServerClient client in clients)
                {
                    try { ConsoleUtils.LogToConsole("" + client.username); }
                    catch
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        ConsoleUtils.LogToConsole("Error Processing Player With IP " + ((IPEndPoint)client.tcp.Client.RemoteEndPoint).Address.ToString());
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
            }

            ConsoleUtils.LogToConsole("");;

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.LogToConsole("Saved Players: " + Server.savedClients.Count);
            Console.ForegroundColor = ConsoleColor.White;

            if (Server.savedClients.Count == 0) ConsoleUtils.LogToConsole("No Players Saved");
            else
            {
                ServerClient[] savedClients = Server.savedClients.ToArray();
                foreach (ServerClient savedClient in savedClients)
                {
                    try { ConsoleUtils.LogToConsole("" + savedClient.username); }
                    catch
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        ConsoleUtils.LogToConsole("Error Processing Player With IP " + ((IPEndPoint)savedClient.tcp.Client.RemoteEndPoint).Address.ToString());
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
            }

            ConsoleUtils.LogToConsole("");;

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.LogToConsole("Saved Factions: " + Server.savedFactions.Count);
            Console.ForegroundColor = ConsoleColor.White;

            if (Server.savedFactions.Count == 0) ConsoleUtils.LogToConsole("No Factions Saved");
            else
            {
                Faction[] factions = Server.savedFactions.ToArray();
                foreach (Faction savedFaction in factions)
                {
                    ConsoleUtils.LogToConsole(savedFaction.name);
                }
            }

            ConsoleUtils.LogToConsole("");;
        }

        public static void SettlementsCommand()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.LogToConsole("Server Settlements: " + Server.savedSettlements.Count);
            Console.ForegroundColor = ConsoleColor.White;

            if (Server.savedSettlements.Count == 0) ConsoleUtils.LogToConsole("No Active Settlements");
            else
            {
                Dictionary<string, List<string>> settlements = Server.savedSettlements;
                foreach (KeyValuePair<string, List<string>> pair in settlements)
                {
                    ConsoleUtils.LogToConsole("[" + pair.Key + "] - [" + pair.Value[0] + "]");
                }
            }

            ConsoleUtils.LogToConsole("");;
        }

        public static void ChatCommand()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.LogToConsole("Server Chat:");
            Console.ForegroundColor = ConsoleColor.White;

            if (Server.chatCache.Count == 0) ConsoleUtils.LogToConsole("No Chat Messages");
            else
            {
                string[] chat = Server.chatCache.ToArray();
                foreach (string message in chat)
                {
                    ConsoleUtils.LogToConsole(message);
                }
            }

            ConsoleUtils.LogToConsole("");;
        }

        public static void EventListCommand()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.LogToConsole("List Of Available Events:");

            Console.ForegroundColor = ConsoleColor.White;
            ConsoleUtils.LogToConsole("Raid");
            ConsoleUtils.LogToConsole("Infestation");
            ConsoleUtils.LogToConsole("MechCluster");
            ConsoleUtils.LogToConsole("ToxicFallout");
            ConsoleUtils.LogToConsole("Manhunter");
            ConsoleUtils.LogToConsole("Wanderer");
            ConsoleUtils.LogToConsole("FarmAnimals");
            ConsoleUtils.LogToConsole("ShipChunk");
            ConsoleUtils.LogToConsole("GiveQuest");
            ConsoleUtils.LogToConsole("TraderCaravan");

            ConsoleUtils.LogToConsole("");;
        }

        //Unknown

        public static void UnknownCommand(string command)
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.LogToConsole("Command [" + command + "] Not Found");
            Console.ForegroundColor = ConsoleColor.White;

            ConsoleUtils.LogToConsole("");;
        }
    }
}