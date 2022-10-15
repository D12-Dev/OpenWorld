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
    [Serializable]
    public class OWServer
    {
        // -- Declarations --
        // Static Instances
        public static Threading _Threading = new Threading();
        public static Networking _Networking = new Networking();
        public static Encryption _Encryption = new Encryption();
        public static ServerUtils _ServerUtils = new ServerUtils();
        public static PlayerUtils _PlayerUtils = new PlayerUtils();
        public static WorldUtils _WorldUtils = new WorldUtils();
        public static CommandHandler _CommandHandler = new CommandHandler();

        // Paths
        public static string mainFolderPath, serverSettingsPath, worldSettingsPath, playersFolderPath, modsFolderPath, whitelistedModsFolderPath, whitelistedUsersPath, logFolderPath;

        // Player Parameters
        // TODO: HASH THE PASSWORDS!!!
        public static List<ServerClient> savedClients = new List<ServerClient>();
        public static Dictionary<string, List<string>> savedSettlements = new Dictionary<string, List<string>>();

        // Server Parameters
        public static string serverName = "",
            serverDescription = "",
            serverVersion = "v1.4.0";
        public static int maxPlayers = 300,
            warningWealthThreshold = 10000,
            banWealthThreshold = 100000,
            idleTimer = 7;
        public static bool usingIdleTimer = false,
            allowDevMode = false,
            usingWhitelist = false,
            usingWealthSystem = false,
            usingRoadSystem = false,
            aggressiveRoadMode = false,
            forceModlist = false,
            forceModlistConfigs = false,
            usingModVerification = false,
            usingChat = false,
            usingProfanityFilter = false;
        public static List<string> whitelistedUsernames = new List<string>(),
            adminList = new List<string>(),
            modList = new List<string>(),
            whitelistedMods = new List<string>(),
            chatCache = new List<string>();
        public static Dictionary<string, string> bannedIPs = new Dictionary<string, string>();

        // World Parameters
        public static float globeCoverage;
        public static string seed;
        public static int overallRainfall, overallTemperature, overallPopulation;

        // Console Colours
        public const ConsoleColor DEFAULT_COLOR = ConsoleColor.White,
          WARN_COLOR = ConsoleColor.Yellow,
          ERROR_COLOR = ConsoleColor.Red,
          MESSAGE_COLOR = ConsoleColor.Green;

        // Setup Tracking
        public static bool networkingDone = false;


        public int vCursorCache = 0, hCursorCache = 0;
        // -- End Declarations --

        

        static void Main()
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
            CultureInfo.CurrentUICulture = new CultureInfo("en-US", false);
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US", false);
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US", false);

            mainFolderPath = AppDomain.CurrentDomain.BaseDirectory;
            logFolderPath = mainFolderPath + Path.DirectorySeparatorChar + "Logs";

            Console.ForegroundColor = MESSAGE_COLOR;
            ServerUtils.WriteServerLog("--------------\nServer Startup\n--------------\nWelcome to Open World - Multiplayer for RimWorld", MESSAGE_COLOR);
            ServerUtils.WriteServerLog($"Using Culture Info: '{CultureInfo.CurrentCulture}'");

            _ServerUtils.SetupPaths();
            _ServerUtils.CheckForFiles();

            _Threading.GenerateThreads(0);
            while(true) ListenForCommands();
        }
        

        private static void ListenForCommands()
        {
            // Trim the leading and trailing white space off the commmand, if any, then pull the command word off to use in the switch.
            
            string command = Console.ReadLine().Trim(), commandWord = command.Split(" ")[0].ToLower();
            Dictionary<string, Action> simpleCommands = new Dictionary<string, Action>()
            {
                {"help", _CommandHandler.Help},
                {"settings", _CommandHandler.Settings},
                {"reload", _CommandHandler.Reload},
                {"status", _CommandHandler.Status},
                {"eventlist", _CommandHandler.EventList},
                {"chat", _CommandHandler.Chat},
                {"list", _CommandHandler.List},
                {"settlements", _CommandHandler.Settlements},
                {"banlist", _CommandHandler.BanList},
                {"adminlist", _CommandHandler.AdminList},
                {"whitelist", _CommandHandler.WhiteList},
                {"wipe", _CommandHandler.Wipe},
                {"clear", Console.Clear},
                {"exit", _CommandHandler.Exit}
            };
            Dictionary<string, Action<string>> complexCommands = new Dictionary<string, Action<string>>()
            {
                {"say", _CommandHandler.Say},
                {"broadcast", _CommandHandler.Broadcast},
                {"notify", _CommandHandler.Notify},
                {"invoke", _CommandHandler.Invoke},
                {"plague", _CommandHandler.Plague},
                {"investigate", _CommandHandler.Investigate},
                {"kick", _CommandHandler.Kick},
                {"ban", _CommandHandler.Ban},
                {"pardon", _CommandHandler.Pardon},
                {"promote", _CommandHandler.Promote},
                {"demote", _CommandHandler.Demote},
                {"giveitem", _CommandHandler.GiveItem},
                {"giveitemall", _CommandHandler.GiveItemAll},
                {"protect", _CommandHandler.Protect},
                {"deprotect", _CommandHandler.Deprotect},
                {"immunize", _CommandHandler.Immunize},
                {"deimmunize", _CommandHandler.Deimmunize}
            };
            if (simpleCommands.ContainsKey(commandWord)) simpleCommands[commandWord]();
            else if (complexCommands.ContainsKey(commandWord)) complexCommands[commandWord](command);
            else ServerUtils.WriteServerLog($"Command \"{command}\" Not Found\n", WARN_COLOR);
        }
    }
}