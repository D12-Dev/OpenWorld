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
        // TODO: WHY IS THIS HERE?
        public static MainProgram _MainProgram = new MainProgram();

        // -- Declarations --
        // Static Instances
        public static Threading _Threading = new Threading();
        public static Networking _Networking = new Networking();
        public static Encryption _Encryption = new Encryption();
        public static ServerUtils _ServerUtils = new ServerUtils();
        public static PlayerUtils _PlayerUtils = new PlayerUtils();
        public static WorldUtils _WorldUtils = new WorldUtils();

        // Paths
        public string mainFolderPath, serverSettingsPath, worldSettingsPath, playersFolderPath, modsFolderPath, whitelistedModsFolderPath, whitelistedUsersPath, logFolderPath;

        // Player Parameters
        // TODO: HASH THE PASSWORDS!!!
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
        // -- End Declarations --

        // TODO: Resolve duplication with ServerUtils.LogToConsole()
        private static void WriteColoredLog(string output, ConsoleColor color = defaultColor)
        {
            Console.ForegroundColor = color;
            // TODO: Build string then write
            foreach (string line in output.Split("\n")) Console.WriteLine(string.IsNullOrWhiteSpace(line) ? "\n" : $"[{DateTime.Now.ToString("HH:mm:ss")}] | {line}");
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
            WriteColoredLog("--------------\nServer Startup\n--------------\nWelcome to Open World - Multiplayer for RimWorld", messageColor);
            WriteColoredLog($"Using Culture Info: '{CultureInfo.CurrentCulture}'");

            _ServerUtils.SetupPaths();
            _ServerUtils.CheckForFiles();

            _Threading.GenerateThreads(0);
            _MainProgram.ListenForCommands();
        }
        private void Help()
        {
            WriteColoredLog("List Of Available Commands:", messageColor);
            WriteColoredLog("Help - Displays Help Menu\n" +
                "Settings - Displays Settings Menu\n" +
                "Reload - Reloads All Available Settings Into The Server\n" +
                "Status - Shows A General Overview Menu\n" +
                "Settlements - Displays Settlements Menu\n" +
                "List - Displays Player List Menu\n" +
                "Whitelist - Shows All Whitelisted Players\n" +
                "Clear - Clears The Console\n" +
                "Exit - Closes The Server\n");

            WriteColoredLog("Communication:", messageColor);
            WriteColoredLog("Say - Send A Chat Message\n" +
                "Broadcast - Send A Letter To Every Player Connected\n" +
                "Notify - Send A Letter To X Player\n" +
                "Chat - Displays Chat Menu\n");

            WriteColoredLog("Interaction:", messageColor);
            WriteColoredLog("Invoke - Invokes An Event To X Player\n" +
                "Plague - Invokes An Event To All Connected Players\n" +
                "Eventlist - Shows All Available Events\n" +
                "GiveItem - Gives An Item To X Player\n" +
                "GiveItemAll - Gives An Item To All Players\n" +
                "Protect - Protects A Player From Any Event Temporarily\n" +
                "Deprotect - Disables All Protections Given To X Player\n" +
                "Immunize - Protects A Player From Any Event Permanently\n" +
                "Deimmunize - Disables The Immunity Given To X Player\n");

            WriteColoredLog("Admin Control:", messageColor);
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
        // TODO: Parameterize the methods so the whole command doesn't have to be passed in, it should be caught when the predicates are called.
        private void Say(string command)
        {
            string message = "";
            try { message = command.Remove(0, 4); }
            catch
            {
                WriteColoredLog("Missing Parameters\n", warnColor);
                // TODO: STOP CALLING THIS RECURSIVELY! IT'S CAUSING A MEMORY LEAK!
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
            string text = "";
            try
            {
                command = command.Remove(0, 10);
                text = command;
                if (string.IsNullOrWhiteSpace(text))
                {
                    WriteColoredLog("Missing Parameters\n", warnColor);
                    ListenForCommands();
                }
            }
            catch
            {
                WriteColoredLog("Missing Parameters\n", warnColor);
                ListenForCommands();
            }
            foreach (ServerClient sc in _Networking.connectedClients)
            {
                _Networking.SendData(sc, "Notification│" + text);
            }
            WriteColoredLog("Letter Sent To Every Connected Player\n", messageColor);
            ListenForCommands();
        }
        private void Notify(string command)
        {
            string target = "", text = "";
            try
            {
                command = command.Remove(0, 7);
                target = command.Split(' ')[0];
                text = command.Replace(target + " ", "");

                if (string.IsNullOrWhiteSpace(text))
                {
                    WriteColoredLog("Missing Parameters\n", warnColor);
                    ListenForCommands();
                }
            }
            catch
            {
                WriteColoredLog("Missing Parameters\n", warnColor);
                ListenForCommands();
            }
            ServerClient targetClient = _Networking.connectedClients.Find(fetch => fetch.username == target);
            if (targetClient == null)
            {
                WriteColoredLog($"Player {target} Not Found\n", warnColor);
                ListenForCommands();
            }
            else
            {
                _Networking.SendData(targetClient, "Notification│" + text);
                WriteColoredLog($"Sent Letter To [{targetClient.username}]\n", messageColor);
                ListenForCommands();
            }
        }
        private void Settings()
        {
            WriteColoredLog("Server Settings:", messageColor);
            WriteColoredLog($"Server Name: {serverName}\nServer Description: {serverDescription}\nServer Local IP: {_Networking.localAddress}\nServer Port: {_Networking.serverPort}\n");

            WriteColoredLog("World Settings:", messageColor);
            WriteColoredLog($"Globe Coverage: {globeCoverage}\nSeed: {seed}\nOverall Rainfall: {overallRainfall}\nOverall Temperature: {overallTemperature}\nOverall Population: {overallPopulation}\n");

            WriteColoredLog($"Server Mods: [{modList.Count}]", messageColor);
            WriteColoredLog(modList.Count == 0 ? "No Mods Found\n" : string.Join("\n", modList.ToArray()) + "\n");

            WriteColoredLog($"Server WhiteListed Mods: [{modList.Count}]", messageColor);
            WriteColoredLog(whitelistedMods.Count == 0 ? "No Mods Found\n" : string.Join("\n", whitelistedMods.ToArray()) + "\n");
        }
        private void Reload()
        {
            WriteColoredLog("Reloading All Current Mods", messageColor);
            // TODO: Is this color change necessary?
            Console.ForegroundColor = defaultColor;
            _ServerUtils.CheckMods();
            _ServerUtils.CheckWhitelistedMods();
            WriteColoredLog("Mods Have Been Reloaded\n", messageColor);

            WriteColoredLog("Reloading All Whitelisted Players\n", messageColor);
            // TODO: Is this color change necessary?
            Console.ForegroundColor = defaultColor;
            _ServerUtils.CheckForWhitelistedPlayers();
            WriteColoredLog("Whitelisted Players Have Been Reloaded", messageColor);
        }
        private void Status()
        {
            WriteColoredLog("Server Status", messageColor);
            WriteColoredLog($"Version: {MainProgram._MainProgram.serverVersion}\n" +
                "Connection: Online\n" +
                $"Uptime: [{DateTime.UtcNow - Process.GetCurrentProcess().StartTime.ToUniversalTime()}]\n");

            WriteColoredLog("Mods:", messageColor);
            WriteColoredLog($"Mods: {_MainProgram.modList.Count()}\n" +
                $"WhiteListed Mods: {_MainProgram.whitelistedMods.Count()}\n");

            WriteColoredLog("Players", messageColor);
            WriteColoredLog($"Connected Players: {_Networking.connectedClients.Count()}\n" +
                $"Saved Players: {_MainProgram.savedClients.Count()}\n" +
                $"Saved Settlements: {_MainProgram.savedSettlements.Count()}\n" +
                $"Whitelisted Players: {_MainProgram.whitelistedUsernames.Count()}\n" +
                $"Max Players: {_MainProgram.maxPlayers}\n");

            WriteColoredLog("Modlist Settings", messageColor);
            WriteColoredLog($"Using Modlist Check: {_MainProgram.forceModlist}\n" +
                $"Using Modlist Config Check: {_MainProgram.forceModlistConfigs}\n" +
                $"Using Mod Verification: {_MainProgram.usingModVerification}\n");

            WriteColoredLog("Chat Settings", messageColor);
            WriteColoredLog($"Using Chat: {_MainProgram.usingChat}\n" +
                $"Using Profanity Filter: {_MainProgram.usingProfanityFilter}\n");

            WriteColoredLog("Wealth Settings", messageColor);
            WriteColoredLog($"Using Wealth System: {_MainProgram.usingWealthSystem}\n" +
                $"Warning Threshold: {_MainProgram.warningWealthThreshold}\n" +
                $"Ban Threshold: {_MainProgram.banWealthThreshold}\n");

            WriteColoredLog("Idle Settings", messageColor);
            WriteColoredLog($"Using Idle System: {_MainProgram.usingIdleTimer}\n" +
                $"Idle Threshold: {_MainProgram.idleTimer}\n");

            WriteColoredLog("Road Settings", messageColor);
            WriteColoredLog($"Using Road System: {_MainProgram.usingRoadSystem}\n" +
                $"Aggressive Road Mode: {_MainProgram.aggressiveRoadMode}\n");

            WriteColoredLog("Miscellaneous Settings", messageColor);
            WriteColoredLog($"Using Whitelist: {_MainProgram.usingWhitelist}\n" +
                $"Allow Dev Mode: {_MainProgram.allowDevMode}\n");
        }
        private void Invoke(string command)
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
                WriteColoredLog("Missing Parameters\n", warnColor);
                ListenForCommands();
            }

            target = _Networking.connectedClients.Where(x => x.username == clientID).SingleOrDefault();
            if (target == null)
            {
                WriteColoredLog($"Player {clientID} Not Found\n", warnColor);
                ListenForCommands();
            }
            _Networking.SendData(target, "ForcedEvent│" + eventID);
            WriteColoredLog($"Sent Event [{eventID}] to [{clientID}]\n", messageColor);
        }
        private void Plague(string command)
        {
            string eventID = "";
            try { eventID = command.Split(' ')[1]; }
            catch
            {
                WriteColoredLog("Missing Parameters\n", warnColor);
                ListenForCommands();
            }
            foreach (ServerClient client in _Networking.connectedClients)
            {
                _Networking.SendData(client, "ForcedEvent│" + eventID);
            }
            WriteColoredLog($"Sent Event [{eventID}] to Every Player\n", messageColor);
        }
        private void EventList()
        {
            WriteColoredLog("List Of Available Events:", messageColor);
            WriteColoredLog("Raid\nInfestation\nMechCluster\nToxicFallout\nManhunter\nFarmAnimals\nShipChunk\nGiveQuest\nTraderCaravan\n");
        }
        private void Chat()
        {
            WriteColoredLog("Server Chat:", messageColor);
            WriteColoredLog(chatCache.Count == 0 ? "No Chat Messages\n" : string.Join("\n", chatCache.ToArray()) + "\n");
        }
        private void List()
        {
            WriteColoredLog($"Connected Players: [{_Networking.connectedClients.Count}]", messageColor);
            if (_Networking.connectedClients.Count() == 0) Console.WriteLine("[{0}] | No Players Connected", DateTime.Now);
            else foreach (ServerClient client in _Networking.connectedClients)
            {
                try { Console.WriteLine("[{0}] | " + client.username, DateTime.Now); }
                catch
                {
                    WriteColoredLog($"Error Processing Player With IP [{((IPEndPoint)client.tcp.Client.RemoteEndPoint).Address}]", errorColor);
                }
            }
            WriteColoredLog($"\nSaved Players: [{_MainProgram.savedClients.Count}]", messageColor);
            if (_MainProgram.savedClients.Count() == 0) Console.WriteLine("[{0}] | No Players Saved", DateTime.Now);
            else foreach (ServerClient savedClient in _MainProgram.savedClients)
            {
                try { Console.WriteLine("[{0}] | " + savedClient.username, DateTime.Now); }
                catch
                {
                    WriteColoredLog($"Error Processing Player With IP [{((IPEndPoint)savedClient.tcp.Client.RemoteEndPoint).Address}]", errorColor);
                }
            }
        }
        private void Investigate(string command)
        {
            string clientID = "";
            try { clientID = command.Split(' ')[1]; }
            catch
            {
                WriteColoredLog("Missing Parameters\n", warnColor);
                ListenForCommands();
            }
            bool found = false;
            foreach (ServerClient client in savedClients)
            {
                if (client.username == clientID)
                {
                    found = true;
                    ServerClient clientToInvestigate = null;
                    bool isConnected = false;
                    string ip = "None";
                    if (_Networking.connectedClients.Find(fetch => fetch.username == client.username) != null)
                    {
                        clientToInvestigate = _Networking.connectedClients.Find(fetch => fetch.username == client.username);
                        isConnected = true;
                        ip = ((IPEndPoint)clientToInvestigate.tcp.Client.RemoteEndPoint).Address.ToString();
                    }
                    WriteColoredLog("Player Details:", messageColor);
                    WriteColoredLog($"Username: {client.username}\n" +
                        $"Password: {client.password}\n" +
                        $"Admin: {client.isAdmin}" +
                        $"Online: {isConnected}" +
                        $"Connection IP: {ip}" +
                        $"Home Tile ID: {client.homeTileID}" +
                        $"Stored Gifts: {client.giftString.Count()}" +
                        $"Stored Trades: {client.tradeString.Count()}" +
                        $"Wealth Value: {client.wealth}" +
                        $"Pawn Count: {client.pawnCount}" +
                        $"Immunized: {client.isImmunized}" +
                        $"Event Shielded: {client.eventShielded}" +
                        $"In RTSE: {client.inRTSE}\n");
                    ListenForCommands();
                }
            }
            if (!found) WriteColoredLog($"Player {clientID} Not Found\n", warnColor);
        }
        private void Settlements()
        {
            WriteColoredLog("Server Settlements:", messageColor);
            string logMessage = "";
            if (savedSettlements.Count == 0) logMessage = "No Active Settlements";
            else foreach (KeyValuePair<string, List<string>> pair in savedSettlements) logMessage += $"{pair.Key} - {pair.Value[0]}";
            WriteColoredLog($"{logMessage}\n");
        }
        private void BanList()
        {
            WriteColoredLog($"Banned Players: [{bannedIPs.Count}]", messageColor);
            string logMessage = "";
            if (savedSettlements.Count == 0) logMessage = "No Active Settlements\n";
            else foreach (KeyValuePair<string, string> pair in bannedIPs) logMessage += $"{pair.Key} - {pair.Value}\n";
            WriteColoredLog($"{logMessage}");
        }
        private void Kick(string command)
        {
            string clientID = "";
            try { clientID = command.Split(' ')[1]; }
            catch
            {
                WriteColoredLog("Missing Parameters\n", warnColor);
                ListenForCommands();
            }
            foreach (ServerClient client in _Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    client.disconnectFlag = true;
                    WriteColoredLog($"Player {clientID} Has Been Kicked\n", warnColor);
                    ListenForCommands();
                }
            }
            WriteColoredLog($"Player {clientID} Not Found\n", warnColor);
        }
        private void Ban(string command)
        {
            string clientID = "";
            try { clientID = command.Split(' ')[1]; }
            catch
            {
                WriteColoredLog("Missing Parameters\n", warnColor);
                ListenForCommands();
            }
            // TODO: Update this to Where() syntax, this is checking them all even after it finds the right one.
            foreach (ServerClient client in _Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    bannedIPs.Add(((IPEndPoint)client.tcp.Client.RemoteEndPoint).Address.ToString(), client.username);
                    client.disconnectFlag = true;
                    SaveSystem.SaveBannedIPs(bannedIPs);
                    WriteColoredLog($"Player {client.username} Has Been Unbanned\n", warnColor);
                    ListenForCommands();
                }
            }
            WriteColoredLog($"Player {clientID} Not Found\n", warnColor);
        }
        private void Pardon(string command)
        {
            string clientUsername = "";
            try { clientUsername = command.Split(' ')[1]; }
            catch
            {
                WriteColoredLog("Missing Parameters\n", warnColor);
                ListenForCommands();
            }
            // TODO: Update this to Where() syntax, this is checking them all even after it finds the right one.
            foreach (KeyValuePair<string, string> pair in bannedIPs)
            {
                if (pair.Value == clientUsername)
                {
                    bannedIPs.Remove(pair.Key);
                    SaveSystem.SaveBannedIPs(bannedIPs);
                    WriteColoredLog($"Player {clientUsername} Has Been Unbanned\n", messageColor);
                    ListenForCommands();
                }
            }
            WriteColoredLog($"Player {clientUsername} Not Found\n", warnColor);
        }
        private void Promote(string command)
        {
            string clientID = "";
            try { clientID = command.Split(' ')[1]; }
            catch
            {
                WriteColoredLog("Missing Parameters\n", warnColor);
                ListenForCommands();
            }
            // TODO: Update this to Where() syntax, this is checking them all even after it finds the right one.
            foreach (ServerClient client in _Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    if (client.isAdmin == true)
                    {
                        Console.ForegroundColor = messageColor;
                        _ServerUtils.LogToConsole("Player [" + client.username + "] Was Already An Administrator");
                        Console.ForegroundColor = defaultColor;
                        _ServerUtils.LogToConsole(Environment.NewLine);
                    }
                    else
                    {
                        client.isAdmin = true;
                        _MainProgram.savedClients.Find(fetch => fetch.username == client.username).isAdmin = true;
                        SaveSystem.SaveUserData(client);

                        _Networking.SendData(client, "│Promote│");

                        Console.ForegroundColor = messageColor;
                        _ServerUtils.LogToConsole("Player [" + client.username + "] Has Been Promoted");
                        Console.ForegroundColor = defaultColor;
                        _ServerUtils.LogToConsole(Environment.NewLine);
                    }
                    ListenForCommands();
                }
            }
            WriteColoredLog($"Player {clientID} Not Found\n", warnColor);
        }
        private void Demote(string command)
        {
            string clientID = "";
            try { clientID = command.Split(' ')[1]; }
            catch
            {
                WriteColoredLog("Missing Parameters\n", warnColor);
                ListenForCommands();
            }
            // TODO: Update this to Where() syntax, this is checking them all even after it finds the right one.
            foreach (ServerClient client in _Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    if (!client.isAdmin)
                    {
                        Console.ForegroundColor = messageColor;
                        _ServerUtils.LogToConsole("Player [" + client.username + "] Is Not An Administrator");
                        Console.ForegroundColor = defaultColor;
                        _ServerUtils.LogToConsole(Environment.NewLine);
                    }
                    else
                    {
                        client.isAdmin = false;
                        _MainProgram.savedClients.Find(fetch => fetch.username == client.username).isAdmin = false;
                        SaveSystem.SaveUserData(client);

                        _Networking.SendData(client, "│Demote│");

                        Console.ForegroundColor = messageColor;
                        _ServerUtils.LogToConsole("Player [" + client.username + "] Has Been Demoted");
                        Console.ForegroundColor = defaultColor;
                        _ServerUtils.LogToConsole(Environment.NewLine);
                    }
                    ListenForCommands();
                }
            }
            WriteColoredLog($"Player {clientID} Not Found\n", warnColor);
        }
        private void GiveItem(string command)
        {
            // TODO: Prescreen the length and get rid of all the repeated try/catches.
            string clientID = "";
            try { clientID = command.Split(' ')[1]; }
            catch
            {
                WriteColoredLog($"Missing Parameter(s)\nUsage: GiveItem [username] [itemID] [itemQuantity] [itemQuality]\n", warnColor);
                ListenForCommands();
            }
            string itemID = "";
            try { itemID = command.Split(' ')[2]; }
            catch
            {
                WriteColoredLog($"Missing Parameter(s)\nUsage: GiveItem [username] [itemID] [itemQuantity] [itemQuality]\n", warnColor);
                ListenForCommands();
            }
            string itemQuantity = "";
            try { itemQuantity = command.Split(' ')[3]; }
            catch
            {
                WriteColoredLog($"Missing Parameter(s)\nUsage: GiveItem [username] [itemID] [itemQuantity] [itemQuality]\n", warnColor);
                ListenForCommands();
            }
            string itemQuality = "";
            try { itemQuality = command.Split(' ')[4]; }
            catch
            {
                WriteColoredLog($"Missing Parameter(s)\nUsage: GiveItem [username] [itemID] [itemQuantity] [itemQuality]\n", warnColor);
                ListenForCommands();
            }
            // TODO: Update this to Where() syntax, this is checking them all even after it finds the right one.
            foreach (ServerClient client in _Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    _Networking.SendData(client, "GiftedItems│" + itemID + "┼" + itemQuantity + "┼" + itemQuality + "┼");

                    WriteColoredLog($"Item Has Neen Gifted To Player [{client.username}]\n", messageColor);
                    ListenForCommands();
                }
            }
            WriteColoredLog($"Player {clientID} Not Found\n", warnColor);
        }
        private void GiveItemAll(string command)
        {
            // TODO: Prescreen the length and get rid of all the repeated try/catches.
            string itemID = "";
            try { itemID = command.Split(' ')[1]; }
            catch
            {
                WriteColoredLog($"Missing Parameter(s)\nUsage: Giveitemall [itemID] [itemQuantity] [itemQuality]\n", warnColor);
                ListenForCommands();
            }
            string itemQuantity = "";
            try { itemQuantity = command.Split(' ')[2]; }
            catch
            {
                WriteColoredLog($"Missing Parameter(s)\nUsage: Giveitemall [itemID] [itemQuantity] [itemQuality]\n", warnColor);
                ListenForCommands();
            }
            string itemQuality = "";
            try { itemQuality = command.Split(' ')[3]; }
            catch
            {
                WriteColoredLog($"Missing Parameter(s)\nUsage: Giveitemall [itemID] [itemQuantity] [itemQuality]\n", warnColor);
                ListenForCommands();
            }
            foreach (ServerClient client in _Networking.connectedClients)
            {
                _Networking.SendData(client, "GiftedItems│" + itemID + "┼" + itemQuantity + "┼" + itemQuality + "┼");
                WriteColoredLog($"Item Has Neen Gifted To All Players\n", messageColor);
                ListenForCommands();
            }
        }
        private void Protect(string command)
        {
            string clientID = "";
            try
            {
                clientID = command.Split(' ')[1];
            }
            catch
            {
                WriteColoredLog("Missing Parameters\n", warnColor);
                ListenForCommands();
            }
            // TODO: Update this to Where() syntax, this is checking them all even after it finds the right one.
            foreach (ServerClient client in _Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    client.eventShielded = true;
                    _MainProgram.savedClients.Find(fetch => fetch.username == client.username).eventShielded = true;
                    WriteColoredLog($"Player [{client.username}] Has Been Protected\n", messageColor);
                    ListenForCommands();
                }
            }
            WriteColoredLog($"Player {clientID} Not Found\n", warnColor);
        }
        private void Deprotect(string command)
        {
            string clientID = "";
            try
            {
                clientID = command.Split(' ')[1];
            }
            catch
            {
                WriteColoredLog("Missing Parameters\n", warnColor);
                ListenForCommands();
            }
            // TODO: Update this to Where() syntax, this is checking them all even after it finds the right one.
            foreach (ServerClient client in _Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    client.eventShielded = false;
                    _MainProgram.savedClients.Find(fetch => fetch.username == client.username).eventShielded = false;
                    WriteColoredLog($"Player [{client.username}] Has Been Deprotected\n", messageColor);
                    ListenForCommands();
                }
            }
            WriteColoredLog($"Player {clientID} Not Found\n", warnColor);
        }
        private void Immunize(string command)
        {
            string clientID = "";
            try { clientID = command.Split(' ')[1]; }
            catch
            {
                WriteColoredLog("Missing Parameters\n", warnColor);
                ListenForCommands();
            }
            // TODO: Update this to Where() syntax, this is checking them all even after it finds the right one.
            foreach (ServerClient client in _Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    client.isImmunized = true;
                    _MainProgram.savedClients.Find(fetch => fetch.username == client.username).isImmunized = true;
                    SaveSystem.SaveUserData(client);
                    WriteColoredLog($"Player [{client.username}] Has Been Immunized\n", messageColor);
                    ListenForCommands();
                }
            }
            WriteColoredLog($"Player {clientID} Not Found\n", warnColor);
        }
        private void Deimmunize(string command)
        {
            string clientID = "";
            try { clientID = command.Split(' ')[1]; }
            catch
            {
                WriteColoredLog("Missing Parameters\n", warnColor);
                ListenForCommands();
            }
            // TODO: Update this to Where() syntax, this is checking them all even after it finds the right one.
            foreach (ServerClient client in _Networking.connectedClients)
            {
                if (client.username == clientID)
                {
                    client.isImmunized = false;
                    _MainProgram.savedClients.Find(fetch => fetch.username == client.username).isImmunized = false;
                    SaveSystem.SaveUserData(client);
                    WriteColoredLog($"Player [{client.username}] Has Been Deimmunized\n", messageColor);
                    ListenForCommands();
                }
            }
            WriteColoredLog($"Player {clientID} Not Found\n", warnColor);
        }
        private void AdminList()
        {
            adminList.Clear();
            foreach (ServerClient client in savedClients) if (client.isAdmin) adminList.Add(client.username);
            WriteColoredLog($"Server Administrators: [{adminList.Count}]", messageColor);
            WriteColoredLog(adminList.Count == 0 ? "No Administrators Found\n" : string.Join("\n", adminList.ToArray()) + "\n");
        }
        private void WhiteList()
        {
            WriteColoredLog($"Whitelisted Players: [{whitelistedUsernames.Count}]", messageColor);
            WriteColoredLog(whitelistedUsernames.Count == 0 ? "No Whitelisted Players Found\n" : string.Join("\n", whitelistedUsernames.ToArray()) + "\n");
        }
        private void Wipe()
        {
            WriteColoredLog("WARNING! THIS ACTION WILL IRRECOVERABLY DELETE ALL PLAYER DATA. DO YOU WANT TO PROCEED? (Y/N)", errorColor);
            if (Console.ReadLine().Trim().ToUpper() == "Y")
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
                WriteColoredLog("All Player Files Have Been Set To Wipe", errorColor);
            }
            else
            {
                Console.Clear();
                ListenForCommands();
            }
        }
        private void Exit()
        {
            foreach (ServerClient sc in _Networking.connectedClients)
            {
                _Networking.SendData(sc, "Disconnect│Closing");
                sc.disconnectFlag = true;
            }
            Environment.Exit(0);
        }
        private void ListenForCommands()
        {
            // Trim the leading and trailing white space off the commmand, if any, then pull the command word off to use in the switch.
            Console.Write("Enter Command> ");
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
            Console.Clear();
            if (simpleCommands.ContainsKey(commandWord)) simpleCommands[commandWord]();
            else if (complexCommands.ContainsKey(commandWord)) complexCommands[commandWord](command);
            else WriteColoredLog($"Command \"{command}\" Not Found\n", warnColor);
            ListenForCommands();
        }
    }
}