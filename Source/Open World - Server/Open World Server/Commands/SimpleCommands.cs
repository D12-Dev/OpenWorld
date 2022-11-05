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
        private static readonly Dictionary<string, Dictionary<string, string>> SETTINGS = new Dictionary<string, Dictionary<string, string>>()
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
            }
        };
        public static void SettingsCommand()
        {
            foreach(KeyValuePair<string, Dictionary<string, string>> setting in SETTINGS) 
            {
                ConsoleUtils.LogToConsole(setting.Key, ConsoleUtils.ConsoleLogMode.Heading);
                ConsoleUtils.LogToConsole(string.Join('\n', setting.Value.Select(x => $"{x.Key}: {x.Value}")));
            }
        }
        private static readonly Dictionary<string, List<string>> MOD_LIST = new Dictionary<string, List<string>>()
        {
            { "Enforced Mods", Server.enforcedMods },
            { "Whitelisted Mods", Server.whitelistedMods },
            { "Server Blacklisted Mods", Server.blacklistedMods }
        };
        public static void ModListCommand()
        {
            foreach (KeyValuePair<string, List<string>> subList in MOD_LIST)
            {
                ConsoleUtils.LogToConsole($"{subList.Key}: {subList.Value.Count}", ConsoleUtils.ConsoleLogMode.Heading);
                ConsoleUtils.LogToConsole(subList.Value.Count == 0 ? $"No {subList.Key}" : string.Join('\n', subList.Value));
            }
        }
        public static void ExitCommand()
        {
            foreach (ServerClient sc in Networking.connectedClients)
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
            ModHandler.CheckMods();
            WorldHandler.CheckWorldFile();
            FactionHandler.CheckFactions();
            PlayerUtils.CheckAllAvailablePlayers();
        }
        private static readonly Dictionary<string, Dictionary<string, string>> STATUSES_1 = new Dictionary<string, Dictionary<string, string>>()
        {
            { "Server Status", new Dictionary<string, string>()
                {
                    {"Version", Server.serverVersion },
                    {"Live", "True" },
                    {"Uptime", (DateTime.UtcNow - Process.GetCurrentProcess().StartTime.ToUniversalTime()).ToString() }
                }
            },
            {   "Mod List Status", new Dictionary<string, string>()
                {
                    {"Using Modlist Check", Server.forceModlist.ToString() },
                    {"Using Modlist Config Check", Server.forceModlistConfigs.ToString() },
                    {"Using Mod Verification", Server.usingModVerification.ToString() }
                }
            }
        };
        private static readonly Dictionary<string, Dictionary<string, string>> STATUSES_2 = new Dictionary<string, Dictionary<string, string>>()
        {
            { "Chat", new Dictionary<string, string>()
                {
                    {"Using Chat", Server.usingChat.ToString() },
                    {"Using Profanity Filter", Server.usingProfanityFilter.ToString() }
                }
            },
            {   "Wealth", new Dictionary<string, string>()
                {
                    {"Using Wealth System", Server.usingWealthSystem.ToString() },
                    {"Warning Threshold", Server.warningWealthThreshold.ToString() },
                    {"Ban Threshold", Server.banWealthThreshold.ToString() }
                }
            },
            {   "Idle", new Dictionary<string, string>()
                {
                    {"Using Idle System", Server.usingIdleTimer.ToString() },
                    {"Idle Threshold", Server.idleTimer.ToString() }
                }
            },
            {   "Road", new Dictionary<string, string>()
                {
                    {"Using Road System", Server.usingRoadSystem.ToString() },
                    {"Aggressive Road Mode", Server.aggressiveRoadMode.ToString() }
                }
            },
            {   "Miscellaneous", new Dictionary<string, string>()
                {
                    {"Using Enforced Difficulty", Server.usingEnforcedDifficulty.ToString() },
                    {"Allow Dev Mode", Server.allowDevMode.ToString() }
                }
            }
        };
        public static void StatusCommand()
        {
            foreach (KeyValuePair<string, Dictionary<string, string>> status in STATUSES_1)
            {
                ConsoleUtils.LogToConsole(status.Key, ConsoleUtils.ConsoleLogMode.Heading);
                ConsoleUtils.LogToConsole(string.Join('\n', status.Value.Select(x => $"{x.Key}: {x.Value}")));
            }
            ModListCommand();
            ConsoleUtils.LogToConsole("Players", ConsoleUtils.ConsoleLogMode.Heading);
            ListCommand();
            SettlementsCommand();
            ConsoleUtils.LogToConsole("Using Whitelist: " + Server.usingWhitelist);
            WhiteListCommand();
            BanListCommand();
            foreach (KeyValuePair<string, Dictionary<string, string>> status in STATUSES_2)
            {
                ConsoleUtils.LogToConsole(status.Key, ConsoleUtils.ConsoleLogMode.Heading);
                ConsoleUtils.LogToConsole(string.Join('\n', status.Value.Select(x => $"{x.Key}: {x.Value}")));
            }
        }
        public static void WhiteListCommand()
        {
            ConsoleUtils.LogToConsole("Whitelisted Players: " + Server.whitelistedUsernames.Count, ConsoleUtils.ConsoleLogMode.Heading);
            ConsoleUtils.LogToConsole(Server.whitelistedUsernames.Count == 0 ? "No Whitelisted Players Found" : string.Join('\n', Server.whitelistedUsernames));
        }
        public static void AdminListCommand()
        {      
            List<ServerClient> admins = Server.savedClients.Where(x => x.isAdmin).ToList();
            ConsoleUtils.LogToConsole("Server Administrators: " + admins.Count, ConsoleUtils.ConsoleLogMode.Heading);
            ConsoleUtils.LogToConsole(admins.Count == 0 ? "No Admins Found" : string.Join('\n', admins.Select(x => x.username)));
        }
        public static void BanListCommand()
        {
            ConsoleUtils.LogToConsole($"Banned players: {Server.bannedIPs.Count}", ConsoleUtils.ConsoleLogMode.Heading);
            ConsoleUtils.LogToConsole(Server.bannedIPs.Count == 0 ? "No Banned Players" : string.Join('\n', Server.bannedIPs.Select(x => $"[{x.Value}] - [{x.Key}]")));
        }
        public static void WipeCommand()
        {
            ConsoleUtils.LogToConsole("WARNING! THIS ACTION WILL DELETE ALL PLAYER DATA. DO YOU WANT TO PROCEED? (Y/N)", ConsoleUtils.ConsoleLogMode.Warning);
            if (Console.ReadLine().Trim().ToUpper() == "Y")
            {
                foreach (ServerClient client in Networking.connectedClients) client.disconnectFlag = true;
                foreach (ServerClient client in Server.savedClients)
                {
                    client.wealth = 0;
                    client.pawnCount = 0;
                    PlayerUtils.SavePlayer(client);
                }
                ConsoleUtils.LogToConsole("All Player Files Have Been Set To Wipe", ConsoleUtils.ConsoleLogMode.Info);
            }
            else ConsoleUtils.LogToConsole("Aborted Wipe Attempt", ConsoleUtils.ConsoleLogMode.Info);
        }
        public static void ListCommand()
        {
            ConsoleUtils.LogToConsole($"Connected Players: {Networking.connectedClients.Count}", ConsoleUtils.ConsoleLogMode.Heading);
            if (Networking.connectedClients.Count == 0) ConsoleUtils.LogToConsole("No Players Connected");
            else
            {
                foreach (ServerClient client in Networking.connectedClients)
                {
                    try 
                    { 
                        ConsoleUtils.LogToConsole(client.username); 
                    }
                    catch
                    {
                        ConsoleUtils.LogToConsole($"Error Processing Player With IP {((IPEndPoint)client.tcp.Client.RemoteEndPoint).Address}", ConsoleUtils.ConsoleLogMode.Error);
                    }
                }
            }
            ConsoleUtils.LogToConsole($"Saved Players: {Server.savedClients.Count}", ConsoleUtils.ConsoleLogMode.Heading);
            if (Server.savedClients.Count == 0) ConsoleUtils.LogToConsole("No Players Saved");
            else
            {
                foreach (ServerClient savedClient in Server.savedClients)
                {
                    try 
                    { 
                        ConsoleUtils.LogToConsole(savedClient.username); 
                    }
                    catch
                    {
                        ConsoleUtils.LogToConsole($"Error Processing Player With IP {((IPEndPoint)savedClient.tcp.Client.RemoteEndPoint).Address}", ConsoleUtils.ConsoleLogMode.Error);
                    }
                }
            }
            ConsoleUtils.LogToConsole("Saved Factions: " + Server.savedFactions.Count, ConsoleUtils.ConsoleLogMode.Heading);
            ConsoleUtils.LogToConsole(Server.chatCache.Count == 0 ? "No Factions Saved" : string.Join('\n', Server.savedFactions.Select(x => x.name)));
        }
        public static void SettlementsCommand()
        {
            ConsoleUtils.LogToConsole("Server Settlements: " + Server.savedSettlements.Count, ConsoleUtils.ConsoleLogMode.Heading);
            ConsoleUtils.LogToConsole(Server.chatCache.Count == 0 ? "No Settlements Saved" : string.Join('\n', Server.savedSettlements.Select(x => $"[{x.Key}] - [{x.Value[0]}]")));
        }
        public static void ChatCommand()
        {
            ConsoleUtils.LogToConsole("Server Chat", ConsoleUtils.ConsoleLogMode.Heading);
            ConsoleUtils.LogToConsole(Server.chatCache.Count == 0 ? "No Chat Messages in History" : string.Join('\n', Server.chatCache));
        }
        public static void EventListCommand()
        {
            ConsoleUtils.LogToConsole("List Of Available Events", ConsoleUtils.ConsoleLogMode.Heading);
            ConsoleUtils.LogToConsole("Raid\nInfestation\nMechCluster\nToxicFallout\nManhunter\nWanderer\nFarmAnimals\nShipChunk\nGiveQuest\nTraderCaravan");
        }
        public static void UnknownCommand(string command) => ConsoleUtils.LogToConsole("Command [" + command + "] Not Found", ConsoleUtils.ConsoleLogMode.Warning);
    }
}