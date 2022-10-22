using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace Open_World_Server
{
    public class ServerUtils
    {
        public void UpdateTitle()
        {
            Console.Title = MainProgram.serverName + " " + MainProgram.serverVersion + " / " + MainProgram._Networking.localAddress.ToString() + " / " + MainProgram._Networking.connectedClients.Count() + " Of " + MainProgram.maxPlayers + " Connected Players";
        }

        public void SetupPaths()
        {
            ConsoleUtils.LogToConsole("Base Directory At: [" + MainProgram.mainFolderPath + "]");
            Console.ForegroundColor = ConsoleColor.White;

            MainProgram.serverSettingsPath = MainProgram.mainFolderPath + Path.DirectorySeparatorChar + "Server Settings.txt";
            MainProgram.worldSettingsPath = MainProgram.mainFolderPath + Path.DirectorySeparatorChar + "World Settings.txt";
            MainProgram.playersFolderPath = MainProgram.mainFolderPath + Path.DirectorySeparatorChar + "Players";
            MainProgram.modsFolderPath = MainProgram.mainFolderPath + Path.DirectorySeparatorChar + "Mods";
            MainProgram.whitelistedModsFolderPath = MainProgram.mainFolderPath + Path.DirectorySeparatorChar + "Whitelisted Mods";
            MainProgram.whitelistedUsersPath = MainProgram.mainFolderPath + Path.DirectorySeparatorChar + "Whitelisted Players.txt";
        }

        public void CheckForFiles()
        {
            CheckServerVersion();
            CheckSettingsFile();
            CheckMods();
            CheckWhitelistedMods();
            MainProgram._PlayerUtils.CheckSavedPlayers();
            CheckForBannedPlayers();
            CheckForWhitelistedPlayers();
            CheckWorldFile();
        }

        private void CheckServerVersion()
        {
            string latestVersion = "";

            try
            {
                WebClient wc = new WebClient();
                latestVersion = wc.DownloadString("https://raw.githubusercontent.com/TastyLollipop/OpenWorld/main/Latest%20Versions%20Cache");
                latestVersion = latestVersion.Split('│')[1].Replace("- Latest Server Version: ", "");
                latestVersion = latestVersion.Remove(0, 1);
                latestVersion = latestVersion.Remove(latestVersion.Count() - 1, 1);
            }
            catch 
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                ConsoleUtils.LogToConsole("Version Check Failed. This is not dangerous");
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            if (MainProgram.serverVersion == latestVersion) ConsoleUtils.LogToConsole("Running Latest Version");
            else ConsoleUtils.LogToConsole("Running outdated version. Please Update From Github At Earliest Convenience To Prevent Errors");
            Console.ForegroundColor = ConsoleColor.White;
        }

        private void CheckSettingsFile()
        {
            if (File.Exists(MainProgram.serverSettingsPath))
            {
                string[] settings = File.ReadAllLines(MainProgram.serverSettingsPath);

                foreach(string setting in settings)
                {
                    if (setting.StartsWith("Server Name: "))
                    {
                        string splitString = setting.Replace("Server Name: ", "");
                        MainProgram.serverName = splitString;
                        continue;
                    }

                    else if (setting.StartsWith("Server Description: "))
                    {
                        string splitString = setting.Replace("Server Description: ", "");
                        MainProgram.serverDescription = splitString;
                        continue;
                    }

                    else if (setting.StartsWith("Server Local IP: "))
                    {
                        string splitString = setting.Replace("Server Local IP: ", "");
                        MainProgram._Networking.localAddress = IPAddress.Parse(splitString);
                        continue;
                    }

                    else if (setting.StartsWith("Server Port: "))
                    {
                        string splitString = setting.Replace("Server Port: ", "");
                        MainProgram._Networking.serverPort = int.Parse(splitString);
                        continue;
                    }

                    else if (setting.StartsWith("Max Players: "))
                    {
                        string splitString = setting.Replace("Max Players: ", "");
                        MainProgram.maxPlayers = int.Parse(splitString);
                        continue;
                    }

                    else if (setting.StartsWith("Allow Dev Mode: "))
                    {
                        string splitString = setting.Replace("Allow Dev Mode: ", "");

                        if (splitString == "True") MainProgram.allowDevMode = true;

                        continue;
                    }

                    else if (setting.StartsWith("Use Whitelist: "))
                    {
                        string splitString = setting.Replace("Use Whitelist: ", "");

                        if (splitString == "True") MainProgram.usingWhitelist = true;

                        continue;
                    }

                    else if (setting.StartsWith("Wealth Warning Threshold: "))
                    {
                        string splitString = setting.Replace("Wealth Warning Threshold: ", "");
                        MainProgram.warningWealthThreshold = int.Parse(splitString);
                        continue;
                    }

                    else if (setting.StartsWith("Wealth Ban Threshold: "))
                    {
                        string splitString = setting.Replace("Wealth Ban Threshold: ", "");
                        MainProgram.banWealthThreshold = int.Parse(splitString);
                        continue;
                    }

                    else if (setting.StartsWith("Use Wealth System: "))
                    {
                        string splitString = setting.Replace("Use Wealth System: ", "");
                        if (splitString == "True")
                        {
                            MainProgram.usingWealthSystem = true;
                        }
                        else if (splitString == "False")
                        {
                            MainProgram.usingWealthSystem = false;
                        }
                        continue;
                    }

                    else if (setting.StartsWith("Use Idle System: "))
                    {
                        string splitString = setting.Replace("Use Idle System: ", "");
                        if (splitString == "True")
                        {
                            MainProgram.usingIdleTimer = true;
                        }
                        else if (splitString == "False")
                        {
                            MainProgram.usingIdleTimer = false;
                        }
                        continue;
                    }

                    else if (setting.StartsWith("Idle Threshold (days): "))
                    {
                        string splitString = setting.Replace("Idle Threshold (days): ", "");
                        MainProgram.idleTimer = int.Parse(splitString);
                        continue;
                    }

                    else if (setting.StartsWith("Use Road System: "))
                    {
                        string splitString = setting.Replace("Use Road System: ", "");
                        if (splitString == "True")
                        {
                            MainProgram.usingRoadSystem = true;
                        }
                        else if (splitString == "False")
                        {
                            MainProgram.usingRoadSystem = false;
                        }
                        continue;
                    }

                    else if (setting.StartsWith("Aggressive Road Mode (WIP): "))
                    {
                        string splitString = setting.Replace("Aggressive Road Mode (WIP): ", "");
                        if (splitString == "True")
                        {
                            MainProgram.aggressiveRoadMode = true;
                        }
                        else if (splitString == "False")
                        {
                            MainProgram.aggressiveRoadMode = false;
                        }
                        continue;
                    }

                    else if (setting.StartsWith("Use Modlist Match: "))
                    {
                        string splitString = setting.Replace("Use Modlist Match: ", "");
                        if (splitString == "True")
                        {
                            MainProgram.forceModlist = true;
                        }
                        else if (splitString == "False")
                        {
                            MainProgram.forceModlist = false;
                        }
                        continue;
                    }

                    else if (setting.StartsWith("Use Modlist Config Match (WIP): "))
                    {
                        string splitString = setting.Replace("Use Modlist Config Match (WIP): ", "");
                        if (splitString == "True")
                        {
                            MainProgram.forceModlistConfigs = true;
                        }
                        else if (splitString == "False")
                        {
                            MainProgram.forceModlistConfigs = false;
                        }
                        continue;
                    }

                    else if (setting.StartsWith("Use Mod Verification: "))
                    {
                        string splitString = setting.Replace("Use Mod Verification: ", "");
                        if (splitString == "True")
                        {
                            MainProgram.usingModVerification = true;
                        }
                        else if (splitString == "False")
                        {
                            MainProgram.usingModVerification = false;
                        }
                        continue;
                    }

                    else if (setting.StartsWith("Use Chat: "))
                    {
                        string splitString = setting.Replace("Use Chat: ", "");
                        if (splitString == "True")
                        {
                            MainProgram.usingChat = true;
                        }
                        else if (splitString == "False")
                        {
                            MainProgram.usingChat = false;
                        }
                        continue;
                    }

                    else if (setting.StartsWith("Use Profanity filter: "))
                    {
                        string splitString = setting.Replace("Use Profanity filter: ", "");
                        if (splitString == "True")
                        {
                            MainProgram.usingProfanityFilter = true;
                        }
                        else if (splitString == "False")
                        {
                            MainProgram.usingProfanityFilter = false;
                        }
                        continue;
                    }
                }

                ConsoleUtils.LogToConsole("Loaded Settings File");
            }

            else
            {
                string[] settingsPreset = new string[]
                {
                    "- Server Details -",
                    "Server Name: My Server Name",
                    "Server Description: My Server Description",
                    "Server Local IP: 0.0.0.0",
                    "Server Port: 25555",
                    "Max Players: 300",
                    "Allow Dev Mode: False",
                    "Use Whitelist: False",
                    "",
                    "- Mod System Details -",
                    "Use Modlist Match: True",
                    "Use Modlist Config Match (WIP): False",
                    "Force Mod Verification: False",
                    "",
                    "- Chat System Details -",
                    "Use Chat: True",
                    "Use Profanity filter: True",
                    "",
                    "- Wealth System Details -",
                    "Use Wealth System: False",
                    "Wealth Warning Threshold: 10000",
                    "Wealth Ban Threshold: 100000",
                    "",
                    "- Idle System Details -",
                    "Use Idle System: True",
                    "Idle Threshold (days): 7",
                    "",
                    "- Road System Details -",
                    "Use Road System: True",
                    "Aggressive Road Mode (WIP): False",
                };

                File.WriteAllLines(MainProgram.serverSettingsPath, settingsPreset);

                ConsoleUtils.LogToConsole("Generating Settings File");

                CheckSettingsFile();
            }
        }

        public void CheckMods()
        {
            List<string> modlist = new List<string>();
            MainProgram.modList.Clear();

            if (!Directory.Exists(MainProgram.modsFolderPath))
            {
                Directory.CreateDirectory(MainProgram.modsFolderPath);
                ConsoleUtils.LogToConsole("No Mods Folder Found, Generating");
                return;
            }

            else
            {
                string[] modFolders = Directory.GetDirectories(MainProgram.modsFolderPath);

                if (modFolders.Length == 0)
                {
                    ConsoleUtils.LogToConsole("No Mods Found, Ignoring");
                    return;
                }

                else
                {
                    foreach (string modFolder in modFolders)
                    {
                        string aboutFilePath = modFolder + Path.DirectorySeparatorChar + "About" + Path.DirectorySeparatorChar + "About.xml";
                        string[] aboutLines = File.ReadAllLines(aboutFilePath);

                        foreach (string line in aboutLines)
                        {
                            if (line.Contains("<name>") && line.Contains("</name>"))
                            {
                                string modName = line;

                                string purgeString = modName.Split('<')[0];

                                modName = modName.Remove(0, purgeString.Count());
                                modName = modName.Replace("<name>", "");
                                modName = modName.Replace("</name>", "");

                                modlist.Add(modName);
                                break;
                            }
                        }
                    }

                    modlist.Sort();
                    MainProgram.modList = modlist.ToList();
                    ConsoleUtils.LogToConsole("Loaded [" + MainProgram.modList.Count() + "] Mods");
                }
            }
        }

        public void CheckWhitelistedMods()
        {
            List<string> whitelistedModsList = new List<string>();
            MainProgram.whitelistedMods.Clear();

            if (!Directory.Exists(MainProgram.whitelistedModsFolderPath))
            {
                Directory.CreateDirectory(MainProgram.whitelistedModsFolderPath);
                ConsoleUtils.LogToConsole("No Whitelisted Mods Folder Found, Generating");
                return;
            }

            else
            {
                string[] modFolders = Directory.GetDirectories(MainProgram.whitelistedModsFolderPath);

                if (modFolders.Length == 0) ConsoleUtils.LogToConsole("No Whitelisted Mods Found, Ignoring");

                else
                {
                    foreach (string modFolder in modFolders)
                    {
                        string aboutFilePath = modFolder + Path.DirectorySeparatorChar + "About" + Path.DirectorySeparatorChar + "About.xml";
                        string[] aboutLines = File.ReadAllLines(aboutFilePath);

                        foreach (string line in aboutLines)
                        {
                            if (line.Contains("<name>") && line.Contains("</name>"))
                            {
                                string modName = line;

                                string purgeString = modName.Split('<')[0];

                                modName = modName.Remove(0, purgeString.Count());
                                modName = modName.Replace("<name>", "");
                                modName = modName.Replace("</name>", "");

                                whitelistedModsList.Add(modName);
                                break;
                            }
                        }
                    }

                    whitelistedModsList.Sort();
                    MainProgram.whitelistedMods = whitelistedModsList;
                    ConsoleUtils.LogToConsole("Loaded [" + MainProgram.whitelistedMods.Count() + "] Whitelisted Mods");
                }
            }
        }

        private void CheckWorldFile()
        {
            if (File.Exists(MainProgram.worldSettingsPath))
            {
                string[] settings = File.ReadAllLines(MainProgram.worldSettingsPath);

                foreach (string setting in settings)
                {
                    if (setting.StartsWith("Globe Coverage (0.3, 0.5, 1.0): "))
                    {
                        string splitString = setting.Replace("Globe Coverage (0.3, 0.5, 1.0): ", "");
                        MainProgram.globeCoverage = float.Parse(splitString);
                        continue;
                    }

                    else if (setting.StartsWith("Seed: "))
                    {
                        string splitString = setting.Replace("Seed: ", "");
                        MainProgram.seed = splitString;
                        continue;
                    }

                    else if (setting.StartsWith("Overall Rainfall (0-6): "))
                    {
                        string splitString = setting.Replace("Overall Rainfall (0-6): ", "");
                        MainProgram.overallRainfall = int.Parse(splitString);
                        continue;
                    }

                    else if (setting.StartsWith("Overall Temperature (0-6): "))
                    {
                        string splitString = setting.Replace("Overall Temperature (0-6): ", "");
                        MainProgram.overallTemperature = int.Parse(splitString);
                        continue;
                    }

                    else if (setting.StartsWith("Overall Population (0-6): "))
                    {
                        string splitString = setting.Replace("Overall Population (0-6): ", "");
                        MainProgram.overallPopulation = int.Parse(splitString);
                        continue;
                    }
                }

                ConsoleUtils.LogToConsole("Loaded World File");
            }

            else
            {
                string[] settingsPreset = new string[]
{
                    "- World Settings -",
                    "Globe Coverage (0.3, 0.5, 1.0): 0.3",
                    "Seed: Seed",
                    "Overall Rainfall (0-6): 3",
                    "Overall Temperature (0-6): 3",
                    "Overall Population (0-6): 3"
                };

                File.WriteAllLines(MainProgram.worldSettingsPath, settingsPreset);

                ConsoleUtils.LogToConsole("Generating World File");

                CheckWorldFile();
            }
        }

        private void CheckForBannedPlayers()
        {
            if (!File.Exists(MainProgram.mainFolderPath + Path.DirectorySeparatorChar + "Banned IPs.data"))
            {
                ConsoleUtils.LogToConsole("No Bans File Found, Ignoring");
                return;
            }

            BanDataHolder list = SaveSystem.LoadBannedIPs();
            {
                MainProgram.bannedIPs = list.bannedIPs;
            }

            if (MainProgram.bannedIPs.Count() == 0) ConsoleUtils.LogToConsole("No Banned Players Found, Ignoring");
            else ConsoleUtils.LogToConsole("Loaded [" + MainProgram.bannedIPs.Count() + "] Banned Players");
        }

        public void CheckForWhitelistedPlayers()
        {
            MainProgram.whitelistedUsernames.Clear();

            if (!File.Exists(MainProgram.whitelistedUsersPath))
            {
                File.Create(MainProgram.whitelistedUsersPath);

                ConsoleUtils.LogToConsole("No Whitelisted Players File Found, Generating");
            }

            else
            {
                if (File.ReadAllLines(MainProgram.whitelistedUsersPath).Count() == 0) ConsoleUtils.LogToConsole("No Whitelisted Players Found, Ignoring");
                else
                {
                    foreach (string str in File.ReadAllLines(MainProgram.whitelistedUsersPath))
                    {
                        MainProgram.whitelistedUsernames.Add(str);
                    }

                    ConsoleUtils.LogToConsole("Loaded [" + MainProgram.whitelistedUsernames.Count() + "] Whitelisted Players");
                }
            }
        }

        public void LoginProcedures(ServerClient client, string data)
        {
            bool userPresent = false;

            client.username = data.Split('│')[1];
            client.password = data.Split('│')[2];
            
            string playerVersion = data.Split('│')[3];
            string joinMode = data.Split('│')[4];
            string playerMods = data.Split('│')[5];

            if (MainProgram.savedClients.Find(fetch => fetch.username == client.username) != null)
            {
                client.isAdmin = MainProgram.savedClients.Find(fetch => fetch.username == client.username).isAdmin;
            }
            else client.isAdmin = false;

            int devInt = 0;
            if (client.isAdmin || MainProgram.allowDevMode) devInt = 1;

            int wipeInt = 0;
            if (client.toWipe) wipeInt = 1;

            int roadInt = 0;
            if (MainProgram.usingRoadSystem) roadInt = 1;
            if (MainProgram.usingRoadSystem && MainProgram.aggressiveRoadMode) roadInt = 2;

            string name = MainProgram.serverName;
            int countInt = MainProgram._Networking.connectedClients.Count;

            int chatInt = 0;
            if (MainProgram.usingChat) chatInt = 1;

            int profanityInt = 0;
            if (MainProgram.usingProfanityFilter) profanityInt = 1;

            int modVerifyInt = 0;
            if (MainProgram.usingModVerification) modVerifyInt = 1;

            if (!TryJoin(client)) return;

            if (!CompareConnectingClientVersion(client, playerVersion)) return;

            if (!CompareConnectingClientWithWhitelist(client)) return;

            if (!ParseClientUsername(client)) return;

            void SendNewGameData()
            {
                MainProgram._PlayerUtils.SaveNewPlayerFile(client.username, client.password);

                float mmGC = MainProgram.globeCoverage;
                string? mmS = MainProgram.seed;
                int mmOR = MainProgram.overallRainfall;
                int mmOT = MainProgram.overallTemperature;
                int mmOP = MainProgram.overallPopulation;

                string settlementString = "";
                foreach (KeyValuePair<string, List<string>> pair in MainProgram.savedSettlements)
                {
                    settlementString += pair.Key + ":" + pair.Value[0] + "»";
                }
                if (settlementString.Count() > 0) settlementString = settlementString.Remove(settlementString.Count() - 1, 1);

                MainProgram._Networking.SendData(client, "MapDetails│" + mmGC + "│" + mmS + "│" + mmOR + "│" + mmOT + "│" + mmOP + "│" + settlementString + "│" + devInt + "│" + wipeInt + "│" + roadInt + "│" + countInt + "│" + chatInt + "│" + profanityInt + "│" + modVerifyInt + "│" + name);
            }

            void SendLoadGameData()
            {
                string settlementString = "";
                foreach (KeyValuePair<string, List<string>> pair in MainProgram.savedSettlements)
                {
                    if (pair.Value[0] == client.username) continue;
                    settlementString += pair.Key + ":" + pair.Value[0] + "»";
                }
                if (settlementString.Count() > 0) settlementString = settlementString.Remove(settlementString.Count() - 1, 1);

                string dataToSend = "UpdateSettlements│" + settlementString + "│" + devInt + "│" + wipeInt + "│" + roadInt + "│" + countInt + "│" + chatInt + "│" + profanityInt + "│" + modVerifyInt + "│" + name;

                if (client.giftString.Count() > 0)
                {
                    string giftsToSend = "";

                    foreach(string str in client.giftString)
                    {
                        giftsToSend += str + "»";
                    }
                    if (giftsToSend.Count() > 0) giftsToSend = giftsToSend.Remove(giftsToSend.Count() - 1, 1);

                    dataToSend += "│GiftedItems│" + giftsToSend;

                    client.giftString.Clear();
                }

                if (client.tradeString.Count() > 0)
                {
                    string tradesToSend = "";

                    foreach (string str in client.tradeString)
                    {
                        tradesToSend += str + "»";
                    }
                    if (tradesToSend.Count() > 0) tradesToSend = tradesToSend.Remove(tradesToSend.Count() - 1, 1);

                    dataToSend += "│TradedItems│" + tradesToSend;

                    client.tradeString.Clear();
                }

                foreach(ServerClient sc in MainProgram.savedClients)
                {
                    if (sc.username == client.username)
                    {
                        sc.giftString.Clear();
                        sc.tradeString.Clear();
                        break;
                    }
                }

                SaveSystem.SaveUserData(client);

                MainProgram._Networking.SendData(client, dataToSend);
            }

            foreach (ServerClient savedClient in MainProgram.savedClients)
            {
                if (savedClient.username.ToLower() == client.username.ToLower())
                {
                    userPresent = true;

                    client.username = savedClient.username;

                    if (savedClient.password == client.password)
                    {
                        if (!MainProgram._ServerUtils.CompareClientIPWithBans(client)) return;

                        if (!MainProgram._ServerUtils.CompareModsWithClient(client, playerMods)) return;

                        MainProgram._ServerUtils.CompareConnectingClientWithConnecteds(client);

                        MainProgram._ServerUtils.UpdateTitle();

                        ConsoleUtils.LogToConsole("Player [" + client.username + "] " + "[" + ((IPEndPoint)client.tcp.Client.RemoteEndPoint).Address.ToString() + "] " + "Has Connected");

                        RefreshClientCount(client);

                        if (joinMode == "NewGame")
                        {
                            SendNewGameData();
                            ConsoleUtils.LogToConsole("Player [" + client.username + "] Has Reset Game Progress");
                        }

                        else if (joinMode == "LoadGame")
                        {
                            MainProgram._PlayerUtils.GiveSavedDataToPlayer(client);
                            SendLoadGameData();
                        }
                    }

                    else
                    {
                        MainProgram._Networking.SendData(client, "Disconnect│WrongPassword");

                        client.disconnectFlag = true;
                        ConsoleUtils.LogToConsole("Player [" + client.username + "] Has Been Kicked For: [Wrong Password]");
                        return;
                    }

                    break;
                }
            }

            if (userPresent) return;

            else
            {
                if (!MainProgram._ServerUtils.CompareClientIPWithBans(client)) return;

                if (!MainProgram._ServerUtils.CompareModsWithClient(client, playerMods)) return;

                MainProgram._ServerUtils.CompareConnectingClientWithConnecteds(client);

                MainProgram._ServerUtils.UpdateTitle();

                ConsoleUtils.LogToConsole("New Player [" + client.username + "] " + "[" + ((IPEndPoint)client.tcp.Client.RemoteEndPoint).Address.ToString() + "] " + "Has Connected For The First Time");

                MainProgram._PlayerUtils.SaveNewPlayerFile(client.username, client.password);

                if (joinMode == "NewGame")
                {
                    SendNewGameData();
                }
      
                else if (joinMode == "LoadGame")
                {
                    SendLoadGameData();
                    ConsoleUtils.LogToConsole("Player [" + client.username + "] Has Registered With Existing Save");
                }
            }
        }

        public void SendChatMessage(ServerClient client, string data)
        {
            string message = data.Split('│')[2];

            string messageForConsole = "Chat - [" + client.username + "] " + message;

            ConsoleUtils.LogToConsole(messageForConsole);

            MainProgram.chatCache.Add("[" + DateTime.Now + "]" + " │ " + messageForConsole);

            try
            {
                foreach (ServerClient sc in MainProgram._Networking.connectedClients)
                {
                    if (sc == client) continue;
                    else MainProgram._Networking.SendData(sc, data);
                }
            }
            catch { }
        }

        public bool CompareModsWithClient(ServerClient client, string data)
        {
            if (client.isAdmin) return true;
            if (!MainProgram.forceModlist) return true;

            string[] clientMods = data.Split('»');

            string flaggedMods = "";

            bool flagged = false;

            foreach (string clientMod in clientMods)
            {
                if (MainProgram.whitelistedMods.Contains(clientMod)) continue;
                if (!MainProgram.modList.Contains(clientMod))
                {
                    flagged = true;
                    flaggedMods += clientMod + "»";
                }
            }

            foreach (string serverMod in MainProgram.modList)
            {
                if (!clientMods.Contains(serverMod))
                {
                    flagged = true;
                    flaggedMods += serverMod + "»";
                }
            }

            if (flagged)
            {
                ConsoleUtils.LogToConsole("Player [" + client.username + "] " + "Doesn't Have The Required Mod Or Mod Files Mismatch!");
                flaggedMods = flaggedMods.Remove(flaggedMods.Count() - 1, 1);
                MainProgram._Networking.SendData(client, "Disconnect│WrongMods│" + flaggedMods);

                client.disconnectFlag = true;
                return false;
            }
            else return true;
        }

        public void CompareConnectingClientWithConnecteds(ServerClient client)
        {
            foreach (ServerClient sc in MainProgram._Networking.connectedClients)
            {
                if (sc.username == client.username)
                {
                    if (sc == client) continue;

                    MainProgram._Networking.SendData(sc, "Disconnect│AnotherLogin");
                    sc.disconnectFlag = true;
                    break;
                }
            }
        }

        public bool CompareConnectingClientWithWhitelist(ServerClient client)
        {
            if (!MainProgram.usingWhitelist) return true;
            if (client.isAdmin) return true;

            foreach (string str in MainProgram.whitelistedUsernames)
            {
                if (str == client.username) return true;
            }

            MainProgram._Networking.SendData(client, "Disconnect│Whitelist");
            client.disconnectFlag = true;
            ConsoleUtils.LogToConsole("Player [" + client.username + "] Tried To Join But Is Not Whitelisted");
            return false;
        }

        public bool CompareConnectingClientVersion(ServerClient client, string clientVersion)
        {
            string latestVersion = "";

            try
            {
                WebClient wc = new WebClient();
                latestVersion = wc.DownloadString("https://raw.githubusercontent.com/TastyLollipop/OpenWorld/main/Latest%20Versions%20Cache");
                latestVersion = latestVersion.Split('│')[2].Replace("- Latest Client Version: ", "");
                latestVersion = latestVersion.Remove(0, 1);
                latestVersion = latestVersion.Remove(latestVersion.Count() - 1, 1);
            }
            catch { return true; }

            if (clientVersion == latestVersion) return true;
            else
            {
                MainProgram._Networking.SendData(client, "Disconnect│Version");
                client.disconnectFlag = true;
                ConsoleUtils.LogToConsole("Player [" + client.username + "] Tried To Join But Is Using Other Version");
                return false;
            }
        }

        public bool CompareClientIPWithBans(ServerClient client)
        {
            foreach(KeyValuePair<string, string> pair in MainProgram.bannedIPs)
            {
                if (pair.Key == ((IPEndPoint)client.tcp.Client.RemoteEndPoint).Address.ToString() || pair.Value == client.username)
                {
                    MainProgram._Networking.SendData(client, "Disconnect│Banned");
                    client.disconnectFlag = true;
                    ConsoleUtils.LogToConsole("Player [" + client.username + "] Tried To Join But Is Banned");
                    return false;
                }
            }

            return true;
        }

        public void RefreshClientCount(ServerClient client)
        {
            int count = MainProgram._Networking.connectedClients.Count;

            foreach (ServerClient sc in MainProgram._Networking.connectedClients)
            {
                if (sc == client) continue;

                try { MainProgram._Networking.SendData(sc, "│PlayerCountRefresh│" + count + "│"); }
                catch { continue; }
            }
        }

        public bool TryJoin(ServerClient client)
        {
            if (client.isAdmin) return true;

            if (MainProgram._Networking.connectedClients.Count() >= MainProgram.maxPlayers + 1)
            {
                MainProgram._Networking.SendData(client, "Disconnect│ServerFull");
                client.disconnectFlag = true;
                return false;
            }

            return true;
        }

        public bool ParseClientUsername(ServerClient client)
        {
            if (string.IsNullOrWhiteSpace(client.username))
            {
                MainProgram._Networking.SendData(client, "Disconnect│Corrupted");
                client.disconnectFlag = true;
                return false;
            }

            if (!client.username.All(character => Char.IsLetterOrDigit(character) || character == '_' || character == '-'))
            {
                MainProgram._Networking.SendData(client, "Disconnect│Corrupted");
                client.disconnectFlag = true;
                return false;
            }

            else return true;
        }
    }
}