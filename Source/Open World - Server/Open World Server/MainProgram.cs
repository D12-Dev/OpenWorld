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
    public class MainProgram
    {
        // Instances
        public static MainProgram _MainProgram = new MainProgram();
        public static Threading _Threading = new Threading();
        public static Networking _Networking = new Networking();
        public static Encryption _Encryption = new Encryption();
        public static ServerUtils _ServerUtils = new ServerUtils();
        public static PlayerUtils _PlayerUtils = new PlayerUtils();
        public static WorldUtils _WorldUtils = new WorldUtils();

        // Paths
        public string mainFolderPath, serverSettingsPath, worldSettingsPath, playersFolderPath, modsFolderPath, whitelistedModsFolderPath, whitelistedUsersPath, logFolderPath;

        // Player Parameters
        public List<ServerClient> savedClients = new List<ServerClient>();
        public Dictionary<string, List<string>> savedSettlements = new Dictionary<string, List<string>>();

        // Server Parameters
        public string serverName = "",
            serverDescription = "",
            serverVersion = "v1.4.0";
        public int maxPlayers = 300,
            warningWealthThreshold = 10000,
            banWealthThreshold = 100000,
            idleTimer = 7;
        public bool usingIdleTimer = false,
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
        public List<string> whitelistedUsernames = new List<string>(),
            adminList = new List<string>(),
            modList = new List<string>(),
            whitelistedMods = new List<string>(),
            chatCache = new List<string>();
        public Dictionary<string, string> bannedIPs = new Dictionary<string, string>();

        // World Parameters
        public float globeCoverage;
        public string seed;
        public int overallRainfall, overallTemperature, overallPopulation;

        // Console Colours
        private const ConsoleColor defaultColor = ConsoleColor.White,
            warnColor = ConsoleColor.Yellow,
            errorColor = ConsoleColor.Red,
            messageColor = ConsoleColor.Green;

        private void WriteColoredLog(string output, ConsoleColor color = defaultColor)
        {
            Console.ForegroundColor = color;
            foreach (string line in output.Split("\n")) Console.WriteLine(string.IsNullOrWhiteSpace(line) ? "\n" : $"[{DateTime.Now}] | {line}");
        }
        static void Main()
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
            CultureInfo.CurrentUICulture = new CultureInfo("en-US", false);
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US", false);
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US", false);

            _MainProgram.mainFolderPath = AppDomain.CurrentDomain.BaseDirectory;
            _MainProgram.logFolderPath = _MainProgram.mainFolderPath + Path.DirectorySeparatorChar + "Logs";

            Console.ForegroundColor = messageColor;
            _ServerUtils.LogToConsole("Server Startup:");
            _ServerUtils.LogToConsole("Using Culture Info: [" + CultureInfo.CurrentCulture + "]");

            _ServerUtils.SetupPaths();
            _ServerUtils.CheckForFiles();

            _Threading.GenerateThreads(0);
            _MainProgram.ListenForCommands();
        }
        private void Help()
        {
            Console.Clear();
            WriteColoredLog("List Of Available Commands:", ConsoleColor.Green);
            WriteColoredLog("Help - Displays Help Menu\n" +
                "Settings - Displays Settings Menu\n" +
                "Reload - Reloads All Available Settings Into The Server\n" +
                "Status - Shows A General Overview Menu\n" +
                "Settlements - Displays Settlements Menu\n" +
                "List - Displays Player List Menu\n" +
                "Whitelist - Shows All Whitelisted Players\n" +
                "Clear - Clears The Console\n" +
                "Exit - Closes The Server\n");

            WriteColoredLog("Communication:", ConsoleColor.Green);
            WriteColoredLog("Say - Send A Chat Message\n" +
                "Broadcast - Send A Letter To Every Player Connected\n" +
                "Notify - Send A Letter To X Player\n" +
                "Chat - Displays Chat Menu\n");

            WriteColoredLog("Interaction:", ConsoleColor.Green);
            WriteColoredLog("Invoke - Invokes An Event To X Player\n" +
                "Plague - Invokes An Event To All Connected Players\n" +
                "Eventlist - Shows All Available Events\n" +
                "GiveItem - Gives An Item To X Player\n" +
                "GiveItemAll - Gives An Item To All Players\n" +
                "Protect - Protects A Player From Any Event Temporarily\n" +
                "Deprotect - Disables All Protections Given To X Player\n" +
                "Immunize - Protects A Player From Any Event Permanently\n" +
                "Deimmunize - Disables The Immunity Given To X Player\n");

            WriteColoredLog("Admin Control:", ConsoleColor.Green);
            WriteColoredLog("Investigate - Displays All Data About X Player\n" +
                "Promote - Promotes X Player To Admin\n" +
                "Demote - Demotes X Player\n" +
                "Adminlist - Shows All Server Admins\n" +
                "Kick - Kicks X Player\n" +
                "Ban - Bans X Player\n" +
                "Pardon - Pardons X Player\n" +
                "Banlist - Shows All Banned Players\n" +
                "Wipe - Deletes Every Player Data In The Server\n");
        }
        private void Say(string command)
        {
            string message = "";
            try { message = command.Remove(0, 4); }
            catch
            {
                WriteColoredLog("Missing Parameters\n", ConsoleColor.Yellow);
                ListenForCommands();
            }

            string messageForConsole = "Chat - [Console] " + message;

            _ServerUtils.LogToConsole(messageForConsole);

            _MainProgram.chatCache.Add("[" + DateTime.Now + "]" + " │ " + messageForConsole);

            try
            {
                foreach (ServerClient sc in _Networking.connectedClients)
                {
                    _Networking.SendData(sc, "ChatMessage│SERVER│" + message);
                }
            }
            catch { }
        }
        private void Broadcast(string command)
        {
            Console.Clear();

            string text = "";

            try
            {
                command = command.Remove(0, 10);
                text = command;

                if (string.IsNullOrWhiteSpace(text))
                {
                    WriteColoredLog("Missing Parameters\n", ConsoleColor.Yellow);
                    ListenForCommands();
                }
            }
            catch
            {
                WriteColoredLog("Missing Parameters\n", ConsoleColor.Yellow);
                ListenForCommands();
            }

            foreach (ServerClient sc in _Networking.connectedClients)
            {
                _Networking.SendData(sc, "Notification│" + text);
            }
            WriteColoredLog("Letter Sent To Every Connected Player\n", ConsoleColor.Green);
            ListenForCommands();
        }
        private void Notify(string command)
        {
            Console.Clear();

            string target = "";
            string text = "";

            try
            {
                command = command.Remove(0, 7);
                target = command.Split(' ')[0];
                text = command.Replace(target + " ", "");

                if (string.IsNullOrWhiteSpace(text))
                {
                    WriteColoredLog("Missing Parameters", ConsoleColor.Yellow);
                    Console.WriteLine(Environment.NewLine);
                    ListenForCommands();
                }
            }
            catch
            {
                WriteColoredLog("Missing Parameters", ConsoleColor.Yellow);
                Console.WriteLine(Environment.NewLine);
                ListenForCommands();
            }

            ServerClient targetClient = _Networking.connectedClients.Find(fetch => fetch.username == target);

            if (targetClient == null)
            {
                WriteColoredLog($"Player {target} Not Found", ConsoleColor.Yellow);
                Console.WriteLine(Environment.NewLine);
                ListenForCommands();
            }
            else
            {
                _Networking.SendData(targetClient, "Notification│" + text);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | Sent Letter To [{1}]", DateTime.Now, targetClient.username);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(Environment.NewLine);
                ListenForCommands();
            }
        }
        private void Settings()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[{0}] | Server Settings:", DateTime.Now);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("[{0}] | Server Name: {1}", DateTime.Now, serverName);
            Console.WriteLine("[{0}] | Server Description: {1}", DateTime.Now, serverDescription);
            Console.WriteLine("[{0}] | Server Local IP: {1}", DateTime.Now, _Networking.localAddress);
            Console.WriteLine("[{0}] | Server Port: {1}", DateTime.Now, _Networking.serverPort);

            Console.WriteLine(Environment.NewLine);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[{0}] | World Settings:", DateTime.Now);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("[{0}] | Globe Coverage: {1}", DateTime.Now, globeCoverage);
            Console.WriteLine("[{0}] | Seed: {1}", DateTime.Now, seed);
            Console.WriteLine("[{0}] | Overall Rainfall: {1}", DateTime.Now, overallRainfall);
            Console.WriteLine("[{0}] | Overall Temperature: {1}", DateTime.Now, overallTemperature);
            Console.WriteLine("[{0}] | Overall Population: {1}", DateTime.Now, overallPopulation);

            Console.WriteLine(Environment.NewLine);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[{0}] | Server Mods: [{1}]", DateTime.Now, modList.Count);
            Console.ForegroundColor = ConsoleColor.White;

            if (modList.Count() == 0) Console.WriteLine("[{0}] | No Mods Found", DateTime.Now);
            else foreach (string mod in modList) Console.WriteLine("[{0}] | {1}", DateTime.Now, mod);

            Console.WriteLine(Environment.NewLine);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[{0}] | Server Whitelisted Mods: [{1}]", DateTime.Now, whitelistedMods.Count);
            Console.ForegroundColor = ConsoleColor.White;

            if (whitelistedMods.Count == 0) Console.WriteLine("[{0}] | No Whitelisted Mods Found", DateTime.Now);
            else foreach (string whitelistedMod in whitelistedMods) Console.WriteLine("[{0}] | {1}", DateTime.Now, whitelistedMod);

            Console.WriteLine(Environment.NewLine);
        }
        private void Reload()
        {
            Console.Clear();
            WriteColoredLog("Reloading All Current Mods", ConsoleColor.Green);
            Console.ForegroundColor = ConsoleColor.White;
            _ServerUtils.CheckMods();
            _ServerUtils.CheckWhitelistedMods();
            WriteColoredLog("Mods Have Been Reloaded", ConsoleColor.Green);
            Console.WriteLine(Environment.NewLine);

            WriteColoredLog("Reloading All Whitelisted Players", ConsoleColor.Green);
            Console.ForegroundColor = ConsoleColor.White;
            _ServerUtils.CheckForWhitelistedPlayers();
            WriteColoredLog("Whitelisted Players Have Been Reloaded", ConsoleColor.Green);
            Console.WriteLine(Environment.NewLine);
        }
        private void Status()
        {
            Console.Clear();
            WriteColoredLog("Server Status", ConsoleColor.Green);
            WriteColoredLog($"Version: {MainProgram._MainProgram.serverVersion}\n" +
                "Connection: Online\n" +
                $"Uptime: [{DateTime.UtcNow - Process.GetCurrentProcess().StartTime.ToUniversalTime()}]");
            Console.WriteLine(Environment.NewLine);

            WriteColoredLog("Mods:", ConsoleColor.Green);
            WriteColoredLog($"Mods: {_MainProgram.modList.Count()}\n" +
                $"WhiteListed Mods: {_MainProgram.whitelistedMods.Count()}");
            Console.WriteLine(Environment.NewLine);

            WriteColoredLog("Players", ConsoleColor.Green);
            WriteColoredLog($"Connected Players: {_Networking.connectedClients.Count()}\n" +
                $"Saved Players: {_MainProgram.savedClients.Count()}\n" +
                $"Saved Settlements: {_MainProgram.savedSettlements.Count()}\n" +
                $"Whitelisted Players: {_MainProgram.whitelistedUsernames.Count()}\n" +
                $"Max Players: {_MainProgram.maxPlayers}");
            Console.WriteLine(Environment.NewLine);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[{0}] | Modlist Settings:", DateTime.Now);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("[{0}] | Using Modlist Check: [{1}]", DateTime.Now, _MainProgram.forceModlist);
            Console.WriteLine("[{0}] | Using Modlist Config Check: [{1}]", DateTime.Now, _MainProgram.forceModlistConfigs);
            Console.WriteLine("[{0}] | Using Mod Verification: [{1}]", DateTime.Now, _MainProgram.usingModVerification);
            Console.WriteLine(Environment.NewLine);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[{0}] | Chat Settings:", DateTime.Now);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("[{0}] | Using Chat: [{1}]", DateTime.Now, _MainProgram.usingChat);
            Console.WriteLine("[{0}] | Using Profanity Filter: [{1}]", DateTime.Now, _MainProgram.usingProfanityFilter);
            Console.WriteLine(Environment.NewLine);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[{0}] | Wealth Settings:", DateTime.Now);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("[{0}] | Using Wealth System: [{1}]", DateTime.Now, _MainProgram.usingWealthSystem);
            Console.WriteLine("[{0}] | Warning Threshold: [{1}]", DateTime.Now, _MainProgram.warningWealthThreshold);
            Console.WriteLine("[{0}] | Ban Threshold: [{1}]", DateTime.Now, _MainProgram.banWealthThreshold);
            Console.WriteLine(Environment.NewLine);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[{0}] | Idle Settings:", DateTime.Now);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("[{0}] | Using Idle System: [{1}]", DateTime.Now, _MainProgram.usingIdleTimer);
            Console.WriteLine("[{0}] | Idle Threshold: [{1}]", DateTime.Now, _MainProgram.idleTimer);
            Console.WriteLine(Environment.NewLine);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[{0}] | Road Settings:", DateTime.Now);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("[{0}] | Using Road System: [{1}]", DateTime.Now, _MainProgram.usingRoadSystem);
            Console.WriteLine("[{0}] | Aggressive Road Mode: [{1}]", DateTime.Now, _MainProgram.aggressiveRoadMode);
            Console.WriteLine(Environment.NewLine);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[{0}] | Miscellaneous Settings", DateTime.Now);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("[{0}] | Using Whitelist: [{1}]", DateTime.Now, _MainProgram.usingWhitelist);
            Console.WriteLine("[{0}] | Allow Dev Mode: [{1}]", DateTime.Now, _MainProgram.allowDevMode);
            Console.WriteLine(Environment.NewLine);
        }
        private void Invoke(string command)
        {
            Console.Clear();

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
                WriteColoredLog("Missing Parameters", ConsoleColor.Yellow);
                Console.WriteLine(Environment.NewLine);
                ListenForCommands();
            }

            foreach (ServerClient client in _Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    target = client;
                    break;
                }
            }

            if (target == null)
            {
                WriteColoredLog($"Player {clientID} Not Found", ConsoleColor.Yellow);
                Console.WriteLine(Environment.NewLine);
                ListenForCommands();
            }

            _Networking.SendData(target, "ForcedEvent│" + eventID);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[{0}] | Sent Event [{1}] to [{2}]", DateTime.Now, eventID, clientID);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(Environment.NewLine);
        }
        private void Plague(string command)
        {
            Console.Clear();

            string eventID = "";

            try { eventID = command.Split(' ')[1]; }
            catch
            {
                WriteColoredLog("Missing Parameters", ConsoleColor.Yellow);
                Console.WriteLine(Environment.NewLine);
                ListenForCommands();
            }

            foreach (ServerClient client in _Networking.connectedClients)
            {
                _Networking.SendData(client, "ForcedEvent│" + eventID);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[{0}] | Sent Event [{1}] To Every Player", DateTime.Now, eventID);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(Environment.NewLine);
        }
        private void EventList()
        {
            Console.Clear();
            WriteColoredLog("List Of Available Events:", ConsoleColor.Green);
            WriteColoredLog("Raid\nInfestation\nMechCluster\nToxicFallout\nManhunter\nFarmAnimals\nShipChunk\nGiveQuest\nTraderCaravan");
            Console.WriteLine(Environment.NewLine);
        }
        private void Chat()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[{0}] | Server Chat:", DateTime.Now);
            Console.ForegroundColor = ConsoleColor.White;

            if (chatCache.Count == 0) Console.WriteLine("[{0}] | No Chat Messages", DateTime.Now);
            else foreach (string message in chatCache)
                {
                    Console.WriteLine(message);
                }

            Console.WriteLine(Environment.NewLine);
        }
        private void List()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[{0}] | Connected Players: [{1}]", DateTime.Now, _Networking.connectedClients.Count);
            Console.ForegroundColor = ConsoleColor.White;

            if (_Networking.connectedClients.Count() == 0) Console.WriteLine("[{0}] | No Players Connected", DateTime.Now);
            else foreach (ServerClient client in _Networking.connectedClients)
                {
                    try { Console.WriteLine("[{0}] | " + client.username, DateTime.Now); }
                    catch
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("[{0}] | Error Processing Player With IP [{1}]", DateTime.Now, ((IPEndPoint)client.tcp.Client.RemoteEndPoint).Address.ToString());
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }

            Console.WriteLine(Environment.NewLine);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[{0}] | Saved Players: [{1}]", DateTime.Now, _MainProgram.savedClients.Count);
            Console.ForegroundColor = ConsoleColor.White;

            if (_MainProgram.savedClients.Count() == 0) Console.WriteLine("[{0}] | No Players Saved", DateTime.Now);
            else foreach (ServerClient savedClient in _MainProgram.savedClients)
                {
                    try { Console.WriteLine("[{0}] | " + savedClient.username, DateTime.Now); }
                    catch
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("[{0}] | Error Processing Player With IP [{1}]", DateTime.Now, ((IPEndPoint)savedClient.tcp.Client.RemoteEndPoint).Address.ToString());
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }

            Console.WriteLine(Environment.NewLine);
        }
        private void Investigate(string command)
        {
            Console.Clear();

            string clientID = "";
            try { clientID = command.Split(' ')[1]; }
            catch
            {
                WriteColoredLog("Missing Parameters", ConsoleColor.Yellow);
                Console.WriteLine(Environment.NewLine);
                ListenForCommands();
            }

            foreach (ServerClient client in savedClients)
            {
                if (client.username == clientID)
                {
                    ServerClient clientToInvestigate = null;

                    bool isConnected = false;
                    string ip = "None";

                    if (_Networking.connectedClients.Find(fetch => fetch.username == client.username) != null)
                    {
                        clientToInvestigate = _Networking.connectedClients.Find(fetch => fetch.username == client.username);
                        isConnected = true;
                        ip = ((IPEndPoint)clientToInvestigate.tcp.Client.RemoteEndPoint).Address.ToString();
                    }

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[{0}] | Player Details: ", DateTime.Now);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("[{0}] | Username: [{1}]", DateTime.Now, client.username);
                    Console.WriteLine("[{0}] | Password: [{1}]", DateTime.Now, client.password);
                    Console.WriteLine("[{0}] | Admin: [{1}]", DateTime.Now, client.isAdmin);
                    Console.WriteLine("[{0}] | Online: [{1}]", DateTime.Now, isConnected);
                    Console.WriteLine("[{0}] | Connection IP: [{1}]", DateTime.Now, ip);
                    Console.WriteLine("[{0}] | Home Tile ID: [{1}]", DateTime.Now, client.homeTileID);
                    Console.WriteLine("[{0}] | Stored Gifts: [{1}]", DateTime.Now, client.giftString.Count());
                    Console.WriteLine("[{0}] | Stored Trades: [{1}]", DateTime.Now, client.tradeString.Count());
                    Console.WriteLine("[{0}] | Wealth Value: [{1}]", DateTime.Now, client.wealth);
                    Console.WriteLine("[{0}] | Pawn Count: [{1}]", DateTime.Now, client.pawnCount);
                    Console.WriteLine("[{0}] | Immunized: [{1}]", DateTime.Now, client.isImmunized);
                    Console.WriteLine("[{0}] | Event Shielded: [{1}]", DateTime.Now, client.eventShielded);
                    Console.WriteLine("[{0}] | In RTSE: [{1}]", DateTime.Now, client.inRTSE);
                    Console.WriteLine(Environment.NewLine);
                    ListenForCommands();
                }
            }

            WriteColoredLog($"Player {clientID} Not Found", ConsoleColor.Yellow);
            Console.WriteLine(Environment.NewLine);
        }
        private void Settlements()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[{0}] | Server Settlements: [{1}]", DateTime.Now, savedSettlements.Count);
            Console.ForegroundColor = ConsoleColor.White;

            if (savedSettlements.Count == 0) Console.WriteLine("[{0}] | No Active Settlements", DateTime.Now);
            else foreach (KeyValuePair<string, List<string>> pair in savedSettlements)
                {
                    Console.WriteLine("[{0}] | {1} - {2} ", DateTime.Now, pair.Key, pair.Value[0]);
                }

            Console.WriteLine(Environment.NewLine);
        }
        private void BanList()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[{0}] | Banned players: [{1}]", DateTime.Now, bannedIPs.Count());
            Console.ForegroundColor = ConsoleColor.White;

            if (bannedIPs.Count == 0) Console.WriteLine("[{0}] | No Banned Players", DateTime.Now);
            else foreach (KeyValuePair<string, string> pair in bannedIPs)
                {
                    Console.WriteLine("[{0}] | [{1}] - [{2}]", DateTime.Now, pair.Value, pair.Key);
                }

            Console.WriteLine(Environment.NewLine);
        }
        private void Kick(string command)
        {
            Console.Clear();

            string clientID = "";
            try { clientID = command.Split(' ')[1]; }
            catch
            {
                WriteColoredLog("Missing Parameters", ConsoleColor.Yellow);
                Console.WriteLine(Environment.NewLine);
                ListenForCommands();
            }

            foreach (ServerClient client in _Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    client.disconnectFlag = true;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[{0}] | Player [{1}] Has Been Kicked", DateTime.Now, clientID);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(Environment.NewLine);
                    ListenForCommands();
                }
            }

            WriteColoredLog($"Player {clientID} Not Found", ConsoleColor.Yellow);
            Console.WriteLine(Environment.NewLine);
        }
        private void Ban(string command)
        {

            Console.Clear();

            string clientID = "";
            try { clientID = command.Split(' ')[1]; }
            catch
            {
                WriteColoredLog("Missing Parameters", ConsoleColor.Yellow);
                Console.WriteLine(Environment.NewLine);
                ListenForCommands();
            }

            foreach (ServerClient client in _Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    bannedIPs.Add(((IPEndPoint)client.tcp.Client.RemoteEndPoint).Address.ToString(), client.username);
                    client.disconnectFlag = true;
                    SaveSystem.SaveBannedIPs(bannedIPs);
                    Console.ForegroundColor = ConsoleColor.Green;
                    _ServerUtils.LogToConsole("Player [" + client.username + "] Has Been Banned");
                    Console.ForegroundColor = ConsoleColor.White;
                    _ServerUtils.LogToConsole(Environment.NewLine);
                    ListenForCommands();
                }
            }

            WriteColoredLog($"Player {clientID} Not Found", ConsoleColor.Yellow);
            Console.WriteLine(Environment.NewLine);
        }
        private void Pardon(string command)
        {
            Console.Clear();

            string clientUsername = "";
            try { clientUsername = command.Split(' ')[1]; }
            catch
            {
                WriteColoredLog("Missing Parameters", ConsoleColor.Yellow);
                Console.WriteLine(Environment.NewLine);
                ListenForCommands();
            }

            foreach (KeyValuePair<string, string> pair in bannedIPs)
            {
                if (pair.Value == clientUsername)
                {
                    bannedIPs.Remove(pair.Key);
                    SaveSystem.SaveBannedIPs(bannedIPs);
                    Console.ForegroundColor = ConsoleColor.Green;
                    _ServerUtils.LogToConsole("Player [" + clientUsername + "] Has Been Unbanned");
                    Console.ForegroundColor = ConsoleColor.White;
                    _ServerUtils.LogToConsole(Environment.NewLine);
                    ListenForCommands();
                }
            }

            WriteColoredLog($"Player {clientUsername} Not Found", ConsoleColor.Yellow);
            Console.WriteLine(Environment.NewLine);
        }
        private void Promote(string command)
        {
            Console.Clear();

            string clientID = "";
            try { clientID = command.Split(' ')[1]; }
            catch
            {
                WriteColoredLog("Missing Parameters", ConsoleColor.Yellow);
                Console.WriteLine(Environment.NewLine);
                ListenForCommands();
            }

            foreach (ServerClient client in _Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    if (client.isAdmin == true)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        _ServerUtils.LogToConsole("Player [" + client.username + "] Was Already An Administrator");
                        Console.ForegroundColor = ConsoleColor.White;
                        _ServerUtils.LogToConsole(Environment.NewLine);
                    }

                    else
                    {
                        client.isAdmin = true;
                        _MainProgram.savedClients.Find(fetch => fetch.username == client.username).isAdmin = true;
                        SaveSystem.SaveUserData(client);

                        _Networking.SendData(client, "│Promote│");

                        Console.ForegroundColor = ConsoleColor.Green;
                        _ServerUtils.LogToConsole("Player [" + client.username + "] Has Been Promoted");
                        Console.ForegroundColor = ConsoleColor.White;
                        _ServerUtils.LogToConsole(Environment.NewLine);
                    }

                    ListenForCommands();
                }
            }

            WriteColoredLog($"Player {clientID} Not Found", ConsoleColor.Yellow);
            Console.WriteLine(Environment.NewLine);
        }
        private void Demote(string command)
        {
            Console.Clear();

            string clientID = "";
            try { clientID = command.Split(' ')[1]; }
            catch
            {
                WriteColoredLog("Missing Parameters", ConsoleColor.Yellow);
                Console.WriteLine(Environment.NewLine);
                ListenForCommands();
            }

            foreach (ServerClient client in _Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    if (!client.isAdmin)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        _ServerUtils.LogToConsole("Player [" + client.username + "] Is Not An Administrator");
                        Console.ForegroundColor = ConsoleColor.White;
                        _ServerUtils.LogToConsole(Environment.NewLine);
                    }

                    else
                    {
                        client.isAdmin = false;
                        _MainProgram.savedClients.Find(fetch => fetch.username == client.username).isAdmin = false;
                        SaveSystem.SaveUserData(client);

                        _Networking.SendData(client, "│Demote│");

                        Console.ForegroundColor = ConsoleColor.Green;
                        _ServerUtils.LogToConsole("Player [" + client.username + "] Has Been Demoted");
                        Console.ForegroundColor = ConsoleColor.White;
                        _ServerUtils.LogToConsole(Environment.NewLine);
                    }

                    ListenForCommands();
                }
            }

            WriteColoredLog($"Player {clientID} Not Found", ConsoleColor.Yellow);
            Console.WriteLine(Environment.NewLine);
        }
        private void GiveItem(string command)
        {
            Console.Clear();

            string clientID = "";
            try { clientID = command.Split(' ')[1]; }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | Missing Parameters", DateTime.Now);
                Console.WriteLine("[{0}] | Usage: Giveitem [username] [itemID] [itemQuantity] [itemQuality]", DateTime.Now);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(Environment.NewLine);
                ListenForCommands();
            }

            string itemID = "";
            try { itemID = command.Split(' ')[2]; }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | Missing Parameters", DateTime.Now);
                Console.WriteLine("[{0}] | Usage: GiveItem [username] [itemID] [itemQuantity] [itemQuality]", DateTime.Now);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(Environment.NewLine);
                ListenForCommands();
            }

            string itemQuantity = "";
            try { itemQuantity = command.Split(' ')[3]; }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | Missing Parameters", DateTime.Now);
                Console.WriteLine("[{0}] | Usage: GiveItem [username] [itemID] [itemQuantity] [itemQuality]", DateTime.Now);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(Environment.NewLine);
                ListenForCommands();
            }

            string itemQuality = "";
            try { itemQuality = command.Split(' ')[4]; }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | Missing Parameters", DateTime.Now);
                Console.WriteLine("[{0}] | Usage: GiveItem [username] [itemID] [itemQuantity] [itemQuality]", DateTime.Now);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(Environment.NewLine);
                ListenForCommands();
            }

            foreach (ServerClient client in _Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    _Networking.SendData(client, "GiftedItems│" + itemID + "┼" + itemQuantity + "┼" + itemQuality + "┼");

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[{0}] | Item Has Neen Gifted To Player [{1}]", DateTime.Now, client.username);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(Environment.NewLine);
                    ListenForCommands();
                }
            }

            WriteColoredLog($"Player {clientID} Not Found", ConsoleColor.Yellow);
            Console.WriteLine(Environment.NewLine);
        }
        private void GiveItemAll(string command)
        {
            Console.Clear();

            string itemID = "";
            try { itemID = command.Split(' ')[1]; }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | Missing Parameters", DateTime.Now);
                Console.WriteLine("[{0}] | Usage: Giveitemall [itemID] [itemQuantity] [itemQuality]", DateTime.Now);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(Environment.NewLine);
                ListenForCommands();
            }

            string itemQuantity = "";
            try { itemQuantity = command.Split(' ')[2]; }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | Missing Parameters", DateTime.Now);
                Console.WriteLine("[{0}] | Usage: Giveitemall [itemID] [itemQuantity] [itemQuality]", DateTime.Now);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(Environment.NewLine);
                ListenForCommands();
            }

            string itemQuality = "";
            try { itemQuality = command.Split(' ')[3]; }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | Missing Parameters", DateTime.Now);
                Console.WriteLine("[{0}] | Usage: Giveitemall [itemID] [itemQuantity] [itemQuality]", DateTime.Now);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(Environment.NewLine);
                ListenForCommands();
            }

            foreach (ServerClient client in _Networking.connectedClients)
            {
                _Networking.SendData(client, "GiftedItems│" + itemID + "┼" + itemQuantity + "┼" + itemQuality + "┼");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | Item Has Neen Gifted To All Players", DateTime.Now);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(Environment.NewLine);
                ListenForCommands();
            }
        }
        private void Protect(string command)
        {
            Console.Clear();

            string clientID = "";
            try { clientID = command.Split(' ')[1]; }
            catch
            {
                WriteColoredLog("Missing Parameters", ConsoleColor.Yellow);
                Console.WriteLine(Environment.NewLine);
                ListenForCommands();
            }

            foreach (ServerClient client in _Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    client.eventShielded = true;
                    _MainProgram.savedClients.Find(fetch => fetch.username == client.username).eventShielded = true;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[{0}] | Player [{1}] Has Been Protected", DateTime.Now, client.username);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(Environment.NewLine);
                    ListenForCommands();
                }
            }

            WriteColoredLog($"Player {clientID} Not Found", ConsoleColor.Yellow);
            Console.WriteLine(Environment.NewLine);
        }
        private void Deprotect(string command)
        {
            Console.Clear();

            string clientID = "";
            try { clientID = command.Split(' ')[1]; }
            catch
            {
                WriteColoredLog("Missing Parameters", ConsoleColor.Yellow);
                Console.WriteLine(Environment.NewLine);
                ListenForCommands();
            }

            foreach (ServerClient client in _Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    client.eventShielded = false;
                    _MainProgram.savedClients.Find(fetch => fetch.username == client.username).eventShielded = false;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[{0}] | Player [{1}] Has Been Deprotected", DateTime.Now, client.username);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(Environment.NewLine);
                    ListenForCommands();
                }
            }

            WriteColoredLog($"Player {clientID} Not Found", ConsoleColor.Yellow);
            Console.WriteLine(Environment.NewLine);
        }
        private void Immunize(string command)
        {
            Console.Clear();

            string clientID = "";
            try { clientID = command.Split(' ')[1]; }
            catch
            {
                WriteColoredLog("Missing Parameters", ConsoleColor.Yellow);
                Console.WriteLine(Environment.NewLine);
                ListenForCommands();
            }

            foreach (ServerClient client in _Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    client.isImmunized = true;
                    _MainProgram.savedClients.Find(fetch => fetch.username == client.username).isImmunized = true;
                    SaveSystem.SaveUserData(client);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[{0}] | Player [{1}] Has Been Inmmunized", DateTime.Now, client.username);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(Environment.NewLine);
                    ListenForCommands();
                }
            }

            WriteColoredLog($"Player {clientID} Not Found", ConsoleColor.Yellow);
            Console.WriteLine(Environment.NewLine);
        }
        private void Deimmunize(string command)
        {
            Console.Clear();

            string clientID = "";
            try { clientID = command.Split(' ')[1]; }
            catch
            {
                WriteColoredLog("Missing Parameters", ConsoleColor.Yellow);
                Console.WriteLine(Environment.NewLine);
                ListenForCommands();
            }

            foreach (ServerClient client in _Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    client.isImmunized = false;
                    _MainProgram.savedClients.Find(fetch => fetch.username == client.username).isImmunized = false;
                    SaveSystem.SaveUserData(client);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[{0}] | Player [{1}] Has Been Deinmmunized", DateTime.Now, client.username);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(Environment.NewLine);
                    ListenForCommands();
                }
            }

            WriteColoredLog($"Player {clientID} Not Found", ConsoleColor.Yellow);
            Console.WriteLine(Environment.NewLine);
        }
        private void AdminList()
        {
            adminList.Clear();

            foreach (ServerClient client in savedClients)
            {
                if (client.isAdmin) adminList.Add(client.username);
            }

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[{0}] | Server Administrators: [{1}]", DateTime.Now, adminList.Count);
            Console.ForegroundColor = ConsoleColor.White;

            if (adminList.Count() == 0) Console.WriteLine("[{0}] | No Administrators Found", DateTime.Now);
            else foreach (string str in adminList) Console.WriteLine("[{0}] | {1}", DateTime.Now, str);

            Console.WriteLine(Environment.NewLine);
        }
        private void WhiteList()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[{0}] | Whitelisted Players: [{1}]", DateTime.Now, whitelistedUsernames.Count);
            Console.ForegroundColor = ConsoleColor.White;

            if (whitelistedUsernames.Count() == 0) Console.WriteLine("[{0}] | No Whitelisted Players Found", DateTime.Now);
            else foreach (string str in whitelistedUsernames) Console.WriteLine("[{0}] | {1}", DateTime.Now, str);

            Console.WriteLine(Environment.NewLine);
        }
        private void Wipe()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[{0}] | WARNING! THIS ACTION WILL DELETE ALL PLAYER DATA. DO YOU WANT TO PROCEED? (Y/N)", DateTime.Now, whitelistedUsernames.Count);
            Console.ForegroundColor = ConsoleColor.White;

            string response = Console.ReadLine();

            if (response == "Y")
            {
                foreach (ServerClient client in _Networking.connectedClients)
                {
                    client.disconnectFlag = true;
                }

                Thread.Sleep(1000);

                foreach (ServerClient client in _MainProgram.savedClients)
                {
                    client.wealth = 0;
                    client.pawnCount = 0;
                    SaveSystem.SaveUserData(client);
                }

                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[{0}] | All Player Files Have Been Set To Wipe", DateTime.Now, whitelistedUsernames.Count);
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.Clear();
                ListenForCommands();
            }
        }
        private void Exit()
        {
            List<ServerClient> clientsToKick = new List<ServerClient>();

            foreach (ServerClient sc in _Networking.connectedClients)
            {
                clientsToKick.Add(sc);
            }

            foreach (ServerClient sc in clientsToKick)
            {
                _Networking.SendData(sc, "Disconnect│Closing");
                sc.disconnectFlag = true;
            }

            Environment.Exit(0);
        }
        private void ListenForCommands()
        {
            Console.ForegroundColor = ConsoleColor.White;

            // Trim the leading and trailing white space off the commmand, if any, then pull the command word off to use in the switch.
            string command = Console.ReadLine().Trim(), commandWord = command.Split(" ")[0].ToLower();

            Dictionary<string, Action> simpleCommands = new Dictionary<string, Action>()
            {
                {"help", Help},
                {"settings", Settings},
                {"reload", Reload},
                {"status", Status},
                {"eventlist", EventList},
                {"chat", Chat},
                {"list", List},
                {"settlements", Settlements},
                {"banlist", BanList},
                {"adminlist", AdminList},
                {"whitelist", WhiteList},
                {"wipe", Wipe},
                {"clear", Console.Clear},
                {"exit", Exit}
            };
            Dictionary<string, Action<string>> complexCommands = new Dictionary<string, Action<string>>()
            {
                {"say", Say},
                {"broadcast", Broadcast},
                {"notify", Notify},
                {"invoke", Invoke},
                {"plague", Plague},
                {"investigate", Investigate},
                {"kick", Kick},
                {"ban", Ban},
                {"pardon", Pardon},
                {"promote", Promote},
                {"demote", Demote},
                {"giveitem", GiveItem},
                {"giveitemall", GiveItemAll},
                {"protect", Protect},
                {"deprotect", Deprotect},
                {"immunize", Immunize},
                {"deimmunize", Deimmunize}
            };

            if (simpleCommands.ContainsKey(commandWord)) simpleCommands[commandWord]();
            else if (complexCommands.ContainsKey(commandWord)) complexCommands[commandWord](command);
            else
            {
                Console.Clear();
                WriteColoredLog($"Command \"{command}\" Not Found", ConsoleColor.Yellow);
                Console.WriteLine(Environment.NewLine);
            }
            ListenForCommands();
        }
    }
}