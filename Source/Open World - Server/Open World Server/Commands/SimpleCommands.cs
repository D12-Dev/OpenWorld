using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace OpenWorldServer
{
    public static class SimpleCommands
    {
        //Miscellaneous
        private static Dictionary<string, Dictionary<string, string>> Settings = new Dictionary<string, Dictionary<string, string>>()
        {
            { "Server Settings", new Dictionary<string, string>()
                {
                    {"Server Name", Server.serverName },
                    {"Server Description", Server.serverDescription },
                    {"Server Local IP", Networking.localAddress.ToString() },
                    {"Server Port", Networking.serverPort.ToString() },
                }
            },
            { "World Settings", new Dictionary<string, string>()
                {
                    {"Globe Coverage", Server.globeCoverage.ToString() },
                    {"Seed", Server.seed },
                    {"Overall Rainfall", Server.overallRainfall.ToString() },
                    {"Overall Temperature", Server.overallTemperature.ToString() },
                    { "Overall Population", Server.overallPopulation.ToString()}
                }
            },
        };
        public static void HelpCommand()
        {
            Console.Clear();
            ConsoleUtils.LogToConsole("List of Available Commands", ConsoleUtils.ConsoleLogMode.Heading);
            ConsoleUtils.LogToConsole("Help - Displays Help Menu\nSettings - Displays Settings Menu\nModlist - Displays Mods Menu\nList - Displays Player List Menu\nWhitelist - Shows All Whitelisted Players\nSettlements - Displays Settlements Menu\nFaction - Displays All Data About X Faction\nReload - Reloads All Available Settings Into The Server\nStatus - Shows A General Overview Menu\nClear - Clears The Console\nExit - Closes The Server");
            ConsoleUtils.LogToConsole("Communication", ConsoleUtils.ConsoleLogMode.Heading);
            ConsoleUtils.LogToConsole("Say - Send A Chat Message\nBroadcast - Send A Letter To Every Player Connected\nNotify - Send A Letter To X Player\nChat - Displays Chat Menu");
            ConsoleUtils.LogToConsole("Interaction", ConsoleUtils.ConsoleLogMode.Heading);
            ConsoleUtils.LogToConsole("Invoke - Invokes An Event To X Player\nPlague - Invokes An Event To All Connected Players\nEventlist - Shows All Available Events\nGiveItem - Gives An Item To X Player\nGiveItemAll - Gives An Item To All Players\nProtect - Protects A Player From Any Event Temporarily\nDeprotect - Disables All Protections Given To X Player\nImmunize - Protects A Player From Any Event Permanently\nDeimmunize - Disables The Immunity Given To X Player");
            ConsoleUtils.LogToConsole("Admin Control", ConsoleUtils.ConsoleLogMode.Heading);
            ConsoleUtils.LogToConsole("Player - Displays All Data About X Player\nPromote - Promotes X Player To Admin\nDemote - Demotes X Player\nAdminlist - Shows All Server Admins\nKick - Kicks X Player\nBan - Bans X Player\nPardon - Pardons X Player\nBanlist - Shows All Banned Players\nWipe - Deletes Every Player Data In The Server");
        }

        public static void SettingsCommand()
        {
            Console.Clear();

            ConsoleUtils.LogToConsole("Server Settings", ConsoleUtils.ConsoleLogMode.Heading);
            ConsoleUtils.LogToConsole(string.Join('\n', Settings["Server Settings"].Select(x => $"{x.Key}: {x.Value}")));
            ConsoleUtils.LogToConsole("World Settings", ConsoleUtils.ConsoleLogMode.Heading);
            ConsoleUtils.LogToConsole(string.Join('\n', Settings["World Settings"].Select(x => $"{x.Key}: {x.Value}")));
        }

        public static void ModListCommand()
        {
            Console.Clear();

            ConsoleUtils.LogToConsole("Server Enforced Mods: " + Server.enforcedMods.Count, ConsoleUtils.ConsoleLogMode.Heading);

            if (Server.enforcedMods.Count == 0) ConsoleUtils.LogToConsole("No Enforced Mods Found");
            else foreach (string mod in Server.enforcedMods) ConsoleUtils.LogToConsole(mod);

            ConsoleUtils.LogToConsole("Server Whitelisted Mods: " + Server.whitelistedMods.Count, ConsoleUtils.ConsoleLogMode.Heading);

            if (Server.whitelistedMods.Count == 0) ConsoleUtils.LogToConsole("No Whitelisted Mods Found");
            else foreach (string whitelistedMod in Server.whitelistedMods) ConsoleUtils.LogToConsole(whitelistedMod);

            ConsoleUtils.LogToConsole("Server Blacklisted Mods: " + Server.blacklistedMods.Count, ConsoleUtils.ConsoleLogMode.Heading);

            if (Server.whitelistedMods.Count == 0) ConsoleUtils.LogToConsole("No Blacklisted Mods Found");
            else foreach (string blacklistedMod in Server.blacklistedMods) ConsoleUtils.LogToConsole(blacklistedMod);
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
            ModHandler.CheckMods();
            WorldHandler.CheckWorldFile();
            FactionHandler.CheckFactions();

            PlayerUtils.CheckAllAvailablePlayers();
        }

        public static void StatusCommand()
        {
            Console.Clear();

            ConsoleUtils.LogToConsole("Server Status", ConsoleUtils.ConsoleLogMode.Heading);

            ConsoleUtils.LogToConsole("Version: " + Server.serverVersion);
            ConsoleUtils.LogToConsole("Connection: Online");
            ConsoleUtils.LogToConsole("Uptime: " + (DateTime.UtcNow - Process.GetCurrentProcess().StartTime.ToUniversalTime()));

            ConsoleUtils.LogToConsole("Mods", ConsoleUtils.ConsoleLogMode.Heading);

            ConsoleUtils.LogToConsole("Enforced Mods: " + Server.enforcedMods.Count);
            ConsoleUtils.LogToConsole("Whitelisted Mods: " + Server.whitelistedMods.Count);
            ConsoleUtils.LogToConsole("Blacklisted Mods: " + Server.blacklistedMods.Count);

            ConsoleUtils.LogToConsole("Players", ConsoleUtils.ConsoleLogMode.Heading);

            ConsoleUtils.LogToConsole("Connected Players: " + Networking.connectedClients.Count);
            ConsoleUtils.LogToConsole("Saved Players: " + Server.savedClients.Count);
            ConsoleUtils.LogToConsole("Saved Settlements: " + Server.savedSettlements.Count);
            ConsoleUtils.LogToConsole("Whitelisted Players: " + Server.whitelistedUsernames.Count);
            ConsoleUtils.LogToConsole("Max Players: " + Server.maxPlayers);

            ConsoleUtils.LogToConsole("Modlist Settings", ConsoleUtils.ConsoleLogMode.Heading);

            ConsoleUtils.LogToConsole("Using Modlist Check: " + Server.forceModlist);
            ConsoleUtils.LogToConsole("Using Modlist Config Check: " + Server.forceModlistConfigs);
            ConsoleUtils.LogToConsole("Using Mod Verification: " + Server.usingModVerification);

            ConsoleUtils.LogToConsole("Chat Settings", ConsoleUtils.ConsoleLogMode.Heading);

            ConsoleUtils.LogToConsole("Using Chat: " + Server.usingChat);
            ConsoleUtils.LogToConsole("Using Profanity Filter: " + Server.usingProfanityFilter);

            ConsoleUtils.LogToConsole("Wealth Settings", ConsoleUtils.ConsoleLogMode.Heading);

            ConsoleUtils.LogToConsole("Using Wealth System: " + Server.usingWealthSystem);
            ConsoleUtils.LogToConsole("Warning Threshold: " + Server.warningWealthThreshold);
            ConsoleUtils.LogToConsole("Ban Threshold: " + Server.banWealthThreshold);

            ConsoleUtils.LogToConsole("Idle Settings", ConsoleUtils.ConsoleLogMode.Heading);

            ConsoleUtils.LogToConsole("Using Idle System: " + Server.usingIdleTimer);
            ConsoleUtils.LogToConsole("Idle Threshold: " + Server.idleTimer);

            ConsoleUtils.LogToConsole("Road Settings", ConsoleUtils.ConsoleLogMode.Heading);

            ConsoleUtils.LogToConsole("Using Road System: " + Server.usingRoadSystem);
            ConsoleUtils.LogToConsole("Aggressive Road Mode: " + Server.aggressiveRoadMode);

            ConsoleUtils.LogToConsole("Miscellaneous Settings", ConsoleUtils.ConsoleLogMode.Heading);

            ConsoleUtils.LogToConsole("Using Whitelist: " + Server.usingWhitelist);
            ConsoleUtils.LogToConsole("Using Enforced Difficulty: " + Server.usingEnforcedDifficulty);
            ConsoleUtils.LogToConsole("Allow Dev Mode: " + Server.allowDevMode);

        }

        //Administration

        public static void WhiteListCommand()
        {
            Console.Clear();

            ConsoleUtils.LogToConsole("Whitelisted Players: " + Server.whitelistedUsernames.Count, ConsoleUtils.ConsoleLogMode.Heading);

            if (Server.whitelistedUsernames.Count == 0) ConsoleUtils.LogToConsole("No Whitelisted Players Found");
            else foreach (string str in Server.whitelistedUsernames) ConsoleUtils.LogToConsole("" + str);

        }

        //Check this one
        public static void AdminListCommand()
        {
            Console.Clear();

            Server.adminList.Clear();

            ServerClient[] savedClients = Server.savedClients.ToArray();
            foreach (ServerClient client in savedClients) if (client.isAdmin) Server.adminList.Add(client.username);
            

            ConsoleUtils.LogToConsole("Server Administrators: " + Server.adminList.Count, ConsoleUtils.ConsoleLogMode.Heading);

            if (Server.adminList.Count == 0) ConsoleUtils.LogToConsole("No Administrators Found");
            else foreach (string str in Server.adminList) ConsoleUtils.LogToConsole("" + str);

        }

        public static void BanListCommand()
        {
            Console.Clear();

            ConsoleUtils.LogToConsole("Banned players: " + Server.bannedIPs.Count, ConsoleUtils.ConsoleLogMode.Heading);

            if (Server.bannedIPs.Count == 0) ConsoleUtils.LogToConsole("No Banned Players");
            else
            {
                Dictionary<string, string> bannedIPs = Server.bannedIPs;
                foreach (KeyValuePair<string, string> pair in bannedIPs) ConsoleUtils.LogToConsole("[" + pair.Value + "] - [" + pair.Key + "]");
                
            }

        }

        public static void WipeCommand()
        {
            Console.Clear();

            ConsoleUtils.LogToConsole("WARNING! THIS ACTION WILL DELETE ALL PLAYER DATA. DO YOU WANT TO PROCEED? (Y/N)", ConsoleUtils.ConsoleLogMode.Warning);
            string response = Console.ReadLine();

            if (response == "Y")
            {
                ServerClient[] clients = Networking.connectedClients.ToArray();
                foreach (ServerClient client in clients) client.disconnectFlag = true;
                

                ServerClient[] savedClients = Server.savedClients.ToArray();
                foreach (ServerClient client in savedClients)
                {
                    client.wealth = 0;
                    client.pawnCount = 0;
                    PlayerUtils.SavePlayer(client);
                }

                Console.Clear();

                ConsoleUtils.LogToConsole("All Player Files Have Been Set To Wipe", ConsoleUtils.ConsoleLogMode.Info);

            }
            else Console.Clear();
        }

        //Player Interaction

        public static void ListCommand()
        {
            Console.Clear();

            ConsoleUtils.LogToConsole("Connected Players: " + Networking.connectedClients.Count, ConsoleUtils.ConsoleLogMode.Heading);

            if (Networking.connectedClients.Count == 0) ConsoleUtils.LogToConsole("No Players Connected");
            else
            {
                ServerClient[] clients = Networking.connectedClients.ToArray();
                foreach (ServerClient client in clients)
                {
                    try { ConsoleUtils.LogToConsole("" + client.username); }
                    catch
                    {
                        ConsoleUtils.LogToConsole("Error Processing Player With IP " + ((IPEndPoint)client.tcp.Client.RemoteEndPoint).Address.ToString(), ConsoleUtils.ConsoleLogMode.Error);
                    }
                }
            }



            ConsoleUtils.LogToConsole("Saved Players: " + Server.savedClients.Count, ConsoleUtils.ConsoleLogMode.Heading);

            if (Server.savedClients.Count == 0) ConsoleUtils.LogToConsole("No Players Saved");
            else
            {
                ServerClient[] savedClients = Server.savedClients.ToArray();
                foreach (ServerClient savedClient in savedClients)
                {
                    try { ConsoleUtils.LogToConsole("" + savedClient.username); }
                    catch
                    {
                        ConsoleUtils.LogToConsole("Error Processing Player With IP " + ((IPEndPoint)savedClient.tcp.Client.RemoteEndPoint).Address.ToString(), ConsoleUtils.ConsoleLogMode.Error);
                    }
                }
            }
            ConsoleUtils.LogToConsole("Saved Factions: " + Server.savedFactions.Count, ConsoleUtils.ConsoleLogMode.Heading);

            if (Server.savedFactions.Count == 0) ConsoleUtils.LogToConsole("No Factions Saved");
            else
            {
                Faction[] factions = Server.savedFactions.ToArray();
                foreach (Faction savedFaction in factions) ConsoleUtils.LogToConsole(savedFaction.name);
                
            }

        }

        public static void SettlementsCommand()
        {
            Console.Clear();

            ConsoleUtils.LogToConsole("Server Settlements: " + Server.savedSettlements.Count, ConsoleUtils.ConsoleLogMode.Heading);

            if (Server.savedSettlements.Count == 0) ConsoleUtils.LogToConsole("No Active Settlements");
            else
            {
                Dictionary<string, List<string>> settlements = Server.savedSettlements;
                foreach (KeyValuePair<string, List<string>> pair in settlements) ConsoleUtils.LogToConsole("[" + pair.Key + "] - [" + pair.Value[0] + "]");
                
            }

        }

        public static void ChatCommand()
        {
            Console.Clear();

            ConsoleUtils.LogToConsole("Server Chat", ConsoleUtils.ConsoleLogMode.Heading);

            if (Server.chatCache.Count == 0) ConsoleUtils.LogToConsole("No Chat Messages");
            else
            {
                string[] chat = Server.chatCache.ToArray();
                foreach (string message in chat) ConsoleUtils.LogToConsole(message);
            }

        }

        public static void EventListCommand()
        {
            Console.Clear();
            ConsoleUtils.LogToConsole("List Of Available Events", ConsoleUtils.ConsoleLogMode.Heading);

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

        }

        //Unknown

        public static void UnknownCommand(string command)
        {
            Console.Clear();

            ConsoleUtils.LogToConsole("Command [" + command + "] Not Found", ConsoleUtils.ConsoleLogMode.Warning);

        }
    }
}