using System;
using System.Collections.Generic;

namespace OpenWorldServer
{
    [System.Serializable]
    public static class Server
    {
        //Meta
        public static bool exit = false;

        //Paths
        public static string mainFolderPath;
        public static string serverSettingsPath;
        public static string worldSettingsPath;
        public static string playersFolderPath;
        public static string factionsFolderPath;
        public static string enforcedModsFolderPath;
        public static string whitelistedModsFolderPath;
        public static string blacklistedModsFolderPath;
        public static string whitelistedUsersPath;
        public static string logFolderPath;

        //Player Parameters
        public static List<ServerClient> savedClients = new List<ServerClient>();
        public static Dictionary<string, List<string>> savedSettlements = new Dictionary<string, List<string>>();

        //Server Details
        public static string serverName = "";
        public static string serverDescription = "";
        public static string serverVersion = "v1.4.1";

        //Server Variables
        public static int maxPlayers = 300;
        public static int warningWealthThreshold = 10000;
        public static int banWealthThreshold = 100000;
        public static int idleTimer = 7;

        //Server Booleans
        public static bool usingIdleTimer = false;
        public static bool allowDevMode = false;
        public static bool usingWhitelist = false;
        public static bool usingWealthSystem = false;
        public static bool usingRoadSystem = false;
        public static bool aggressiveRoadMode = false;
        public static bool forceModlist = false;
        public static bool forceModlistConfigs = false;
        public static bool usingModVerification = false;
        public static bool usingChat = false;
        public static bool usingProfanityFilter = false;
        public static bool usingEnforcedDifficulty = false;

        //Server Mods
        public static List<string> enforcedMods = new List<string>();
        public static List<string> whitelistedMods = new List<string>();
        public static List<string> blacklistedMods = new List<string>();

        //Server Lists
        public static List<string> whitelistedUsernames = new List<string>();
        public static List<string> adminList = new List<string>();
        public static List<string> chatCache = new List<string>();
        public static Dictionary<string, string> bannedIPs = new Dictionary<string, string>();
        public static List<Faction> savedFactions = new List<Faction>();

        //World Parameters
        public static float globeCoverage;
        public static string seed;
        public static int overallRainfall;
        public static int overallTemperature;
        public static int overallPopulation;
        public static string latestClientVersion;

        static void Main()
        {
            ServerUtils.SetPaths();
            ServerUtils.SetCulture();

            ServerUtils.CheckServerVersion();
            ServerUtils.CheckClientVersionRequirement();
            ServerUtils.CheckSettingsFile();

            ModHandler.CheckMods(true);
            FactionHandler.CheckFactions(true);
            WorldHandler.CheckWorldFile();
            PlayerUtils.CheckAllAvailablePlayers(false);

            Threading.GenerateThreads(0);

            while (!exit) ListenForCommands();
        }

        public static void ListenForCommands()
        {
            Console.ForegroundColor = ConsoleColor.White;

            string fullCommand = Console.ReadLine();
            string commandBase = fullCommand.Split(' ')[0].ToLower();

            string commandArguments = "";
            if (fullCommand.Contains(' ')) commandArguments = fullCommand.Replace(fullCommand.Split(' ')[0], "").Remove(0, 1);

            Dictionary<string, Action> simpleCommands = new Dictionary<string, Action>()
            {
                {"help", SimpleCommands.HelpCommand},
                {"settings", SimpleCommands.SettingsCommand},
                {"modlist", SimpleCommands.ModListCommand},
                {"reload", SimpleCommands.ReloadCommand},
                {"status", SimpleCommands.StatusCommand},
                {"eventlist", SimpleCommands.EventListCommand},
                {"chat", SimpleCommands.ChatCommand},
                {"list", SimpleCommands.ListCommand},
                {"settlements", SimpleCommands.SettlementsCommand},
                {"banlist", SimpleCommands.BanListCommand},
                {"adminlist", SimpleCommands.AdminListCommand},
                {"whitelist", SimpleCommands.WhiteListCommand},
                {"wipe", SimpleCommands.WipeCommand},
                {"clear", SimpleCommands.ClearCommand},
                {"exit", SimpleCommands.ExitCommand}
            };

            Dictionary<string, Action> advancedCommands = new Dictionary<string, Action>()
            {
                {"say", AdvancedCommands.SayCommand},
                {"broadcast", AdvancedCommands.BroadcastCommand},
                {"notify", AdvancedCommands.NotifyCommand},
                {"invoke", AdvancedCommands.InvokeCommand},
                {"plague", AdvancedCommands.PlagueCommand},
                {"player", AdvancedCommands.PlayerDetailsCommand},
                {"faction", AdvancedCommands.FactionDetailsCommand},
                {"kick", AdvancedCommands.KickCommand},
                {"ban", AdvancedCommands.BanCommand},
                {"pardon", AdvancedCommands.PardonCommand},
                {"promote", AdvancedCommands.PromoteCommand},
                {"demote", AdvancedCommands.DemoteCommand},
                {"giveitem", AdvancedCommands.GiveItemCommand},
                {"giveitemall", AdvancedCommands.GiveItemAllCommand},
                {"protect", AdvancedCommands.ProtectCommand},
                {"deprotect", AdvancedCommands.DeprotectCommand},
                {"immunize", AdvancedCommands.ImmunizeCommand},
                {"deimmunize", AdvancedCommands.DeimmunizeCommand}
            };

            try
            {
                if (simpleCommands.ContainsKey(commandBase))
                {
                    simpleCommands[commandBase]();
                }

                else if (advancedCommands.ContainsKey(commandBase))
                {
                    AdvancedCommands.commandData = commandArguments;
                    advancedCommands[commandBase]();
                }

                else SimpleCommands.UnknownCommand(commandBase);
            }

            catch 
            {
                Console.ForegroundColor = ConsoleColor.Red;
                ConsoleUtils.WriteWithTime("Command Caught Exception");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}