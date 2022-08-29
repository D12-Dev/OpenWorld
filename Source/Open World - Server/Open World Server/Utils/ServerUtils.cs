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
            Console.Title = MainProgram._MainProgram.serverName + " " + MainProgram._MainProgram.serverVersion + " / " + MainProgram._Networking.localAddress.ToString() + " / " + MainProgram._Networking.connectedClients.Count() + " Of " + MainProgram._MainProgram.maxPlayers + " Connected Players";
        }

        public void SetupPaths()
        {
            MainProgram._ServerUtils.LogToConsole("Base Directory At: [" + MainProgram._MainProgram.mainFolderPath + "]");
            Console.ForegroundColor = ConsoleColor.White;

            MainProgram._MainProgram.serverSettingsPath = MainProgram._MainProgram.mainFolderPath + Path.DirectorySeparatorChar + "Server Settings.txt";
            MainProgram._MainProgram.worldSettingsPath = MainProgram._MainProgram.mainFolderPath + Path.DirectorySeparatorChar + "World Settings.txt";
            MainProgram._MainProgram.playersFolderPath = MainProgram._MainProgram.mainFolderPath + Path.DirectorySeparatorChar + "Players";
            MainProgram._MainProgram.modsFolderPath = MainProgram._MainProgram.mainFolderPath + Path.DirectorySeparatorChar + "Mods";
            MainProgram._MainProgram.whitelistedModsFolderPath = MainProgram._MainProgram.mainFolderPath + Path.DirectorySeparatorChar + "Whitelisted Mods";
            MainProgram._MainProgram.whitelistedUsersPath = MainProgram._MainProgram.mainFolderPath + Path.DirectorySeparatorChar + "Whitelisted Players.txt";
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
                MainProgram._ServerUtils.LogToConsole("Version Check Failed. This is not dangerous");
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            if (MainProgram._MainProgram.serverVersion == latestVersion) MainProgram._ServerUtils.LogToConsole("Running Latest Version");
            else MainProgram._ServerUtils.LogToConsole("Running outdated version. Please Update From Github At Earliest Convenience To Prevent Errors");
            Console.ForegroundColor = ConsoleColor.White;
        }

        private void CheckSettingsFile()
        {
            if (File.Exists(MainProgram._MainProgram.serverSettingsPath))
            {
                string[] settings = File.ReadAllLines(MainProgram._MainProgram.serverSettingsPath);

                foreach(string setting in settings)
                {
                    if (setting.StartsWith("Server Name: "))
                    {
                        string splitString = setting.Replace("Server Name: ", "");
                        MainProgram._MainProgram.serverName = splitString;
                        continue;
                    }

                    else if (setting.StartsWith("Server Description: "))
                    {
                        string splitString = setting.Replace("Server Description: ", "");
                        MainProgram._MainProgram.serverDescription = splitString;
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
                        MainProgram._MainProgram.maxPlayers = int.Parse(splitString);
                        continue;
                    }

                    else if (setting.StartsWith("Allow Dev Mode: "))
                    {
                        string splitString = setting.Replace("Allow Dev Mode: ", "");

                        if (splitString == "True") MainProgram._MainProgram.allowDevMode = true;

                        continue;
                    }

                    else if (setting.StartsWith("Use Whitelist: "))
                    {
                        string splitString = setting.Replace("Use Whitelist: ", "");

                        if (splitString == "True") MainProgram._MainProgram.usingWhitelist = true;

                        continue;
                    }

                    else if (setting.StartsWith("Wealth Warning Threshold: "))
                    {
                        string splitString = setting.Replace("Wealth Warning Threshold: ", "");
                        MainProgram._MainProgram.warningWealthThreshold = int.Parse(splitString);
                        continue;
                    }

                    else if (setting.StartsWith("Wealth Ban Threshold: "))
                    {
                        string splitString = setting.Replace("Wealth Ban Threshold: ", "");
                        MainProgram._MainProgram.banWealthThreshold = int.Parse(splitString);
                        continue;
                    }

                    else if (setting.StartsWith("Use Wealth System: "))
                    {
                        string splitString = setting.Replace("Use Wealth System: ", "");
                        if (splitString == "True")
                        {
                            MainProgram._MainProgram.usingWealthSystem = true;
                        }
                        else if (splitString == "False")
                        {
                            MainProgram._MainProgram.usingWealthSystem = false;
                        }
                        continue;
                    }

                    else if (setting.StartsWith("Use Idle System: "))
                    {
                        string splitString = setting.Replace("Use Idle System: ", "");
                        if (splitString == "True")
                        {
                            MainProgram._MainProgram.usingIdleTimer = true;
                        }
                        else if (splitString == "False")
                        {
                            MainProgram._MainProgram.usingIdleTimer = false;
                        }
                        continue;
                    }

                    else if (setting.StartsWith("Idle Threshold (days): "))
                    {
                        string splitString = setting.Replace("Idle Threshold (days): ", "");
                        MainProgram._MainProgram.idleTimer = int.Parse(splitString);
                        continue;
                    }

                    else if (setting.StartsWith("Use Road System: "))
                    {
                        string splitString = setting.Replace("Use Road System: ", "");
                        if (splitString == "True")
                        {
                            MainProgram._MainProgram.usingRoadSystem = true;
                        }
                        else if (splitString == "False")
                        {
                            MainProgram._MainProgram.usingRoadSystem = false;
                        }
                        continue;
                    }

                    else if (setting.StartsWith("Aggressive Road Mode (WIP): "))
                    {
                        string splitString = setting.Replace("Aggressive Road Mode (WIP): ", "");
                        if (splitString == "True")
                        {
                            MainProgram._MainProgram.aggressiveRoadMode = true;
                        }
                        else if (splitString == "False")
                        {
                            MainProgram._MainProgram.aggressiveRoadMode = false;
                        }
                        continue;
                    }

                    else if (setting.StartsWith("Use Modlist Match: "))
                    {
                        string splitString = setting.Replace("Use Modlist Match: ", "");
                        if (splitString == "True")
                        {
                            MainProgram._MainProgram.forceModlist = true;
                        }
                        else if (splitString == "False")
                        {
                            MainProgram._MainProgram.forceModlist = false;
                        }
                        continue;
                    }

                    else if (setting.StartsWith("Use Modlist Config Match (WIP): "))
                    {
                        string splitString = setting.Replace("Use Modlist Config Match (WIP): ", "");
                        if (splitString == "True")
                        {
                            MainProgram._MainProgram.forceModlistConfigs = true;
                        }
                        else if (splitString == "False")
                        {
                            MainProgram._MainProgram.forceModlistConfigs = false;
                        }
                        continue;
                    }

                    else if (setting.StartsWith("Use Mod Verification: "))
                    {
                        string splitString = setting.Replace("Use Mod Verification: ", "");
                        if (splitString == "True")
                        {
                            MainProgram._MainProgram.usingModVerification = true;
                        }
                        else if (splitString == "False")
                        {
                            MainProgram._MainProgram.usingModVerification = false;
                        }
                        continue;
                    }

                    else if (setting.StartsWith("Use Chat: "))
                    {
                        string splitString = setting.Replace("Use Chat: ", "");
                        if (splitString == "True")
                        {
                            MainProgram._MainProgram.usingChat = true;
                        }
                        else if (splitString == "False")
                        {
                            MainProgram._MainProgram.usingChat = false;
                        }
                        continue;
                    }

                    else if (setting.StartsWith("Use Profanity filter: "))
                    {
                        string splitString = setting.Replace("Use Profanity filter: ", "");
                        if (splitString == "True")
                        {
                            MainProgram._MainProgram.usingProfanityFilter = true;
                        }
                        else if (splitString == "False")
                        {
                            MainProgram._MainProgram.usingProfanityFilter = false;
                        }
                        continue;
                    }
                }

                MainProgram._ServerUtils.LogToConsole("Loaded Settings File");
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

                File.WriteAllLines(MainProgram._MainProgram.serverSettingsPath, settingsPreset);

                MainProgram._ServerUtils.LogToConsole("Generating Settings File");

                CheckSettingsFile();
            }
        }

        public void CheckMods()
        {
            List<string> modlist = new List<string>();
            MainProgram._MainProgram.modList.Clear();

            if (!Directory.Exists(MainProgram._MainProgram.modsFolderPath))
            {
                Directory.CreateDirectory(MainProgram._MainProgram.modsFolderPath);
                MainProgram._ServerUtils.LogToConsole("No Mods Folder Found, Generating");
                return;
            }

            else
            {
                string[] modFolders = Directory.GetDirectories(MainProgram._MainProgram.modsFolderPath);

                if (modFolders.Length == 0)
                {
                    MainProgram._ServerUtils.LogToConsole("No Mods Found, Ignoring");
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
                    MainProgram._MainProgram.modList = modlist.ToList();
                    MainProgram._ServerUtils.LogToConsole("Loaded [" + MainProgram._MainProgram.modList.Count() + "] Mods");
                }
            }
        }

        public void CheckWhitelistedMods()
        {
            List<string> whitelistedModsList = new List<string>();
            MainProgram._MainProgram.whitelistedMods.Clear();

            if (!Directory.Exists(MainProgram._MainProgram.whitelistedModsFolderPath))
            {
                Directory.CreateDirectory(MainProgram._MainProgram.whitelistedModsFolderPath);
                MainProgram._ServerUtils.LogToConsole("No Whitelisted Mods Folder Found, Generating");
                return;
            }

            else
            {
                string[] modFolders = Directory.GetDirectories(MainProgram._MainProgram.whitelistedModsFolderPath);

                if (modFolders.Length == 0) MainProgram._ServerUtils.LogToConsole("No Whitelisted Mods Found, Ignoring");

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
                    MainProgram._MainProgram.whitelistedMods = whitelistedModsList;
                    MainProgram._ServerUtils.LogToConsole("Loaded [" + MainProgram._MainProgram.whitelistedMods.Count() + "] Whitelisted Mods");
                }
            }
        }

        private void CheckWorldFile()
        {
            if (File.Exists(MainProgram._MainProgram.worldSettingsPath))
            {
                string[] settings = File.ReadAllLines(MainProgram._MainProgram.worldSettingsPath);

                foreach (string setting in settings)
                {
                    if (setting.StartsWith("Globe Coverage (0.3, 0.5, 1.0): "))
                    {
                        string splitString = setting.Replace("Globe Coverage (0.3, 0.5, 1.0): ", "");
                        MainProgram._MainProgram.globeCoverage = float.Parse(splitString);
                        continue;
                    }

                    else if (setting.StartsWith("Seed: "))
                    {
                        string splitString = setting.Replace("Seed: ", "");
                        MainProgram._MainProgram.seed = splitString;
                        continue;
                    }

                    else if (setting.StartsWith("Overall Rainfall (0-6): "))
                    {
                        string splitString = setting.Replace("Overall Rainfall (0-6): ", "");
                        MainProgram._MainProgram.overallRainfall = int.Parse(splitString);
                        continue;
                    }

                    else if (setting.StartsWith("Overall Temperature (0-6): "))
                    {
                        string splitString = setting.Replace("Overall Temperature (0-6): ", "");
                        MainProgram._MainProgram.overallTemperature = int.Parse(splitString);
                        continue;
                    }

                    else if (setting.StartsWith("Overall Population (0-6): "))
                    {
                        string splitString = setting.Replace("Overall Population (0-6): ", "");
                        MainProgram._MainProgram.overallPopulation = int.Parse(splitString);
                        continue;
                    }
                }

                MainProgram._ServerUtils.LogToConsole("Loaded World File");
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

                File.WriteAllLines(MainProgram._MainProgram.worldSettingsPath, settingsPreset);

                MainProgram._ServerUtils.LogToConsole("Generating World File");

                CheckWorldFile();
            }
        }

        private void CheckForBannedPlayers()
        {
            if (!File.Exists(MainProgram._MainProgram.mainFolderPath + Path.DirectorySeparatorChar + "Banned IPs.data"))
            {
                MainProgram._ServerUtils.LogToConsole("No Bans File Found, Ignoring");
                return;
            }

            BanDataHolder list = SaveSystem.LoadBannedIPs();
            {
                MainProgram._MainProgram.bannedIPs = list.bannedIPs;
            }

            if (MainProgram._MainProgram.bannedIPs.Count() == 0) MainProgram._ServerUtils.LogToConsole("No Banned Players Found, Ignoring");
            else MainProgram._ServerUtils.LogToConsole("Loaded [" + MainProgram._MainProgram.bannedIPs.Count() + "] Banned Players");
        }

        public void CheckForWhitelistedPlayers()
        {
            MainProgram._MainProgram.whitelistedUsernames.Clear();

            if (!File.Exists(MainProgram._MainProgram.whitelistedUsersPath))
            {
                File.Create(MainProgram._MainProgram.whitelistedUsersPath);

                MainProgram._ServerUtils.LogToConsole("No Whitelisted Players File Found, Generating");
            }

            else
            {
                if (File.ReadAllLines(MainProgram._MainProgram.whitelistedUsersPath).Count() == 0) MainProgram._ServerUtils.LogToConsole("No Whitelisted Players Found, Ignoring");
                else
                {
                    foreach (string str in File.ReadAllLines(MainProgram._MainProgram.whitelistedUsersPath))
                    {
                        MainProgram._MainProgram.whitelistedUsernames.Add(str);
                    }

                    MainProgram._ServerUtils.LogToConsole("Loaded [" + MainProgram._MainProgram.whitelistedUsernames.Count() + "] Whitelisted Players");
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

            if (MainProgram._MainProgram.savedClients.Find(fetch => fetch.username == client.username) != null)
            {
                client.isAdmin = MainProgram._MainProgram.savedClients.Find(fetch => fetch.username == client.username).isAdmin;
            }
            else client.isAdmin = false;

            int devInt = 0;
            if (client.isAdmin || MainProgram._MainProgram.allowDevMode) devInt = 1;

            int wipeInt = 0;
            if (client.toWipe) wipeInt = 1;

            int roadInt = 0;
            if (MainProgram._MainProgram.usingRoadSystem) roadInt = 1;
            if (MainProgram._MainProgram.usingRoadSystem && MainProgram._MainProgram.aggressiveRoadMode) roadInt = 2;

            string name = MainProgram._MainProgram.serverName;
            int countInt = MainProgram._Networking.connectedClients.Count;

            int chatInt = 0;
            if (MainProgram._MainProgram.usingChat) chatInt = 1;

            int profanityInt = 0;
            if (MainProgram._MainProgram.usingProfanityFilter) profanityInt = 1;

            int modVerifyInt = 0;
            if (MainProgram._MainProgram.usingModVerification) modVerifyInt = 1;

            if (!TryJoin(client)) return;

            if (!CompareConnectingClientVersion(client, playerVersion)) return;

            if (!CompareConnectingClientWithWhitelist(client)) return;

            if (!ParseClientUsername(client)) return;

            void SendNewGameData()
            {
                MainProgram._PlayerUtils.SaveNewPlayerFile(client.username, client.password);

                float mmGC = MainProgram._MainProgram.globeCoverage;
                string? mmS = MainProgram._MainProgram.seed;
                int mmOR = MainProgram._MainProgram.overallRainfall;
                int mmOT = MainProgram._MainProgram.overallTemperature;
                int mmOP = MainProgram._MainProgram.overallPopulation;

                string settlementString = "";
                foreach (KeyValuePair<string, List<string>> pair in MainProgram._MainProgram.savedSettlements)
                {
                    settlementString += pair.Key + ":" + pair.Value[0] + "»";
                }
                if (settlementString.Count() > 0) settlementString = settlementString.Remove(settlementString.Count() - 1, 1);

                MainProgram._Networking.SendData(client, "MapDetails│" + mmGC + "│" + mmS + "│" + mmOR + "│" + mmOT + "│" + mmOP + "│" + settlementString + "│" + devInt + "│" + wipeInt + "│" + roadInt + "│" + countInt + "│" + chatInt + "│" + profanityInt + "│" + modVerifyInt + "│" + name);
            }

            void SendLoadGameData()
            {
                string settlementString = "";
                foreach (KeyValuePair<string, List<string>> pair in MainProgram._MainProgram.savedSettlements)
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

                foreach(ServerClient sc in MainProgram._MainProgram.savedClients)
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

            foreach (ServerClient savedClient in MainProgram._MainProgram.savedClients)
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

                        MainProgram._ServerUtils.LogToConsole("Player [" + client.username + "] " + "[" + ((IPEndPoint)client.tcp.Client.RemoteEndPoint).Address.ToString() + "] " + "Has Connected");

                        RefreshClientCount(client);

                        if (joinMode == "NewGame")
                        {
                            SendNewGameData();
                            MainProgram._ServerUtils.LogToConsole("Player [" + client.username + "] Has Reset Game Progress");
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
                        MainProgram._ServerUtils.LogToConsole("Player [" + client.username + "] Has Been Kicked For: [Wrong Password]");
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

                MainProgram._ServerUtils.LogToConsole("New Player [" + client.username + "] " + "[" + ((IPEndPoint)client.tcp.Client.RemoteEndPoint).Address.ToString() + "] " + "Has Connected For The First Time");

                MainProgram._PlayerUtils.SaveNewPlayerFile(client.username, client.password);

                if (joinMode == "NewGame")
                {
                    SendNewGameData();
                }
      
                else if (joinMode == "LoadGame")
                {
                    SendLoadGameData();
                    MainProgram._ServerUtils.LogToConsole("Player [" + client.username + "] Has Registered With Existing Save");
                }
            }
        }

        public void SendChatMessage(ServerClient client, string data)
        {
            string message = data.Split('│')[2];

            string messageForConsole = "Chat - [" + client.username + "] " + message;

            MainProgram._ServerUtils.LogToConsole(messageForConsole);

            MainProgram._MainProgram.chatCache.Add("[" + DateTime.Now + "]" + " │ " + messageForConsole);

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
            if (!MainProgram._MainProgram.forceModlist) return true;

            string[] clientMods = data.Split('»');

            string flaggedMods = "";

            bool flagged = false;

            foreach (string clientMod in clientMods)
            {
                if (MainProgram._MainProgram.whitelistedMods.Contains(clientMod)) continue;
                if (!MainProgram._MainProgram.modList.Contains(clientMod))
                {
                    flagged = true;
                    flaggedMods += clientMod + "»";
                }
            }

            foreach (string serverMod in MainProgram._MainProgram.modList)
            {
                if (!clientMods.Contains(serverMod))
                {
                    flagged = true;
                    flaggedMods += serverMod + "»";
                }
            }

            if (flagged)
            {
                MainProgram._ServerUtils.LogToConsole("Player [" + client.username + "] " + "Doesn't Have The Required Mod Or Mod Files Mismatch!");
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
            if (!MainProgram._MainProgram.usingWhitelist) return true;
            if (client.isAdmin) return true;

            foreach (string str in MainProgram._MainProgram.whitelistedUsernames)
            {
                if (str == client.username) return true;
            }

            MainProgram._Networking.SendData(client, "Disconnect│Whitelist");
            client.disconnectFlag = true;
            LogToConsole("Player [" + client.username + "] Tried To Join But Is Not Whitelisted");
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
                LogToConsole("Player [" + client.username + "] Tried To Join But Is Using Other Version");
                return false;
            }
        }

        public bool CompareClientIPWithBans(ServerClient client)
        {
            foreach(KeyValuePair<string, string> pair in MainProgram._MainProgram.bannedIPs)
            {
                if (pair.Key == ((IPEndPoint)client.tcp.Client.RemoteEndPoint).Address.ToString() || pair.Value == client.username)
                {
                    MainProgram._Networking.SendData(client, "Disconnect│Banned");
                    client.disconnectFlag = true;
                    LogToConsole("Player [" + client.username + "] Tried To Join But Is Banned");
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

            if (MainProgram._Networking.connectedClients.Count() >= MainProgram._MainProgram.maxPlayers + 1)
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

        public void LogToConsole(string data)
        {
            string dataToLog = "";
            if (data != Environment.NewLine) dataToLog = "[" + DateTime.Now + "]" + " │ " + data;
            else dataToLog = "";

            Console.WriteLine(dataToLog);

            if (data.StartsWith("Chat - [")) WriteToLog(dataToLog, "Chat");
            else if (data.StartsWith("Gift Done Between")) WriteToLog(dataToLog, "Gift");
            else if (data.StartsWith("Trade Done Between")) WriteToLog(dataToLog, "Trade");
            else if (data.StartsWith("Barter Done Between")) WriteToLog(dataToLog, "Barter");
            else if (data.StartsWith("Spy Done Between")) WriteToLog(dataToLog, "Spy");
            else if (data.StartsWith("PvP Done Between")) WriteToLog(dataToLog, "PvP");
            else if (data.StartsWith("Visit Done Between")) WriteToLog(dataToLog, "Visit");
            else WriteToLog(dataToLog, "Normal");
        }

        public void WriteToLog(string data, string logMode)
        {
            string pathToday = MainProgram._MainProgram.logFolderPath + Path.DirectorySeparatorChar + DateTime.Today.Month + "-" + DateTime.Today.Day + "-" + DateTime.Today.Year;
            if (!Directory.Exists(pathToday)) Directory.CreateDirectory(pathToday);

            string logName;
            if (logMode == "Chat") logName = "Chat.txt";
            else if (logMode == "Gift") logName = "Gift.txt";
            else if (logMode == "Trade") logName = "Trade.txt";
            else if (logMode == "Barter") logName = "Barter.txt";
            else if (logMode == "Spy") logName = "Spy.txt";
            else if (logMode == "PvP") logName = "PvP.txt";
            else if (logMode == "Visit") logName = "Visit.txt";
            else logName = "Log.txt";

            try { File.AppendAllText(pathToday + Path.DirectorySeparatorChar + logName, data + Environment.NewLine); }
            catch { }
        }
    }
}