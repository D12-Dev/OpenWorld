using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.IO;
using System.Threading;

namespace Open_World_Server
{
    [System.Serializable]
    public static class MainProgram
    {
        //Instances
        public static Threading _Threading = new Threading();
        public static Networking _Networking = new Networking();
        public static Encryption _Encryption = new Encryption();
        public static ServerUtils _ServerUtils = new ServerUtils();
        public static PlayerUtils _PlayerUtils = new PlayerUtils();
        public static WorldUtils _WorldUtils = new WorldUtils();

        //Paths
        public static string mainFolderPath;
        public static string serverSettingsPath;
        public static string worldSettingsPath;
        public static string playersFolderPath;
        public static string modsFolderPath;
        public static string whitelistedModsFolderPath;
        public static string whitelistedUsersPath;
        public static string logFolderPath;

        //Player Parameters
        public static List<ServerClient> savedClients = new List<ServerClient>();
        public static Dictionary<string, List<string>> savedSettlements = new Dictionary<string, List<string>>();

        //Server Parameters
        public static string serverName = "";
        public static string serverDescription = "";
        public static string serverVersion = "v1.4.0";
        public static int maxPlayers = 300;
        public static int warningWealthThreshold = 10000;
        public static int banWealthThreshold = 100000;
        public static int idleTimer = 7;
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
        public static List<string> whitelistedUsernames = new List<string>();
        public static List<string> adminList = new List<string>();
        public static List<string> modList = new List<string>();
        public static List<string> whitelistedMods = new List<string>();
        public static List<string> chatCache = new List<string>();
        public static Dictionary<string, string> bannedIPs = new Dictionary<string, string>();

        //World Parameters
        public static float globeCoverage;
        public static string seed;
        public static int overallRainfall;
        public static int overallTemperature;
        public static int overallPopulation;

        static void Main()
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
            CultureInfo.CurrentUICulture = new CultureInfo("en-US", false);
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US", false);
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US", false);

            mainFolderPath = AppDomain.CurrentDomain.BaseDirectory;
            logFolderPath = mainFolderPath + Path.DirectorySeparatorChar + "Logs";
            
            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.LogToConsole("Server Startup:");
            ConsoleUtils.LogToConsole("Using Culture Info: [" + CultureInfo.CurrentCulture + "]");

            _ServerUtils.SetupPaths();
            _ServerUtils.CheckForFiles();

            _Threading.GenerateThreads(0);
            while (true) ListenForCommands();
        }

        public static void ListenForCommands()
        {
            Console.ForegroundColor = ConsoleColor.White;

            string fullCommand = Console.ReadLine().ToLower();
            string commandBase = fullCommand.Split(' ')[0];

            string commandArguments = "";
            if (fullCommand.Contains(' ')) commandArguments = fullCommand.Replace(fullCommand.Split(' ')[0], "").Remove(0, 1);

            Dictionary<string, Action> simpleCommands = new Dictionary<string, Action>()
            {
                {"help", SimpleCommands.HelpCommand},
                {"settings", SimpleCommands.SettingsCommand},
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
                {"investigate", AdvancedCommands.InvestigateCommand},
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
    }
}