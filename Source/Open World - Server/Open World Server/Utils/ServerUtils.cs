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
            Console.Title = OWServer.serverName + " " + OWServer.serverVersion + " / " + OWServer._Networking.localAddress.ToString() + " / " + OWServer._Networking.connectedClients.Count() + " Of " + OWServer.maxPlayers + " Connected Players";
        }

        public void SetupPaths()
        {
            OWServer._ServerUtils.WriteServerLog("Base Directory At: [" + OWServer.mainFolderPath + "]");
            Console.ForegroundColor = ConsoleColor.White;

            OWServer.serverSettingsPath = OWServer.mainFolderPath + Path.DirectorySeparatorChar + "Server Settings.txt";
            OWServer.worldSettingsPath = OWServer.mainFolderPath + Path.DirectorySeparatorChar + "World Settings.txt";
            OWServer.playersFolderPath = OWServer.mainFolderPath + Path.DirectorySeparatorChar + "Players";
            OWServer.modsFolderPath = OWServer.mainFolderPath + Path.DirectorySeparatorChar + "Mods";
            OWServer.whitelistedModsFolderPath = OWServer.mainFolderPath + Path.DirectorySeparatorChar + "Whitelisted Mods";
            OWServer.whitelistedUsersPath = OWServer.mainFolderPath + Path.DirectorySeparatorChar + "Whitelisted Players.txt";
        }

        public void CheckForFiles()
        {
            CheckServerVersion();
            CheckSettingsFile();
            CheckMods();
            CheckWhitelistedMods();
            OWServer._PlayerUtils.CheckSavedPlayers();
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
                OWServer._ServerUtils.WriteServerLog("Version Check Failed. This is not dangerous");
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            if (OWServer.serverVersion == latestVersion) OWServer._ServerUtils.WriteServerLog("Running Latest Version");
            else OWServer._ServerUtils.WriteServerLog("Running outdated version. Please Update From Github At Earliest Convenience To Prevent Errors");
            Console.ForegroundColor = ConsoleColor.White;
        }

        private void CheckSettingsFile()
        {
            if (File.Exists(OWServer.serverSettingsPath))
            {
                string[] settings = File.ReadAllLines(OWServer.serverSettingsPath);

                foreach(string setting in settings)
                {
                    if (setting.StartsWith("Server Name: "))
                    {
                        string splitString = setting.Replace("Server Name: ", "");
                        OWServer.serverName = splitString;
                        continue;
                    }

                    else if (setting.StartsWith("Server Description: "))
                    {
                        string splitString = setting.Replace("Server Description: ", "");
                        OWServer.serverDescription = splitString;
                        continue;
                    }

                    else if (setting.StartsWith("Server Local IP: "))
                    {
                        string splitString = setting.Replace("Server Local IP: ", "");
                        OWServer._Networking.localAddress = IPAddress.Parse(splitString);
                        continue;
                    }

                    else if (setting.StartsWith("Server Port: "))
                    {
                        string splitString = setting.Replace("Server Port: ", "");
                        OWServer._Networking.serverPort = int.Parse(splitString);
                        continue;
                    }

                    else if (setting.StartsWith("Max Players: "))
                    {
                        string splitString = setting.Replace("Max Players: ", "");
                        OWServer.maxPlayers = int.Parse(splitString);
                        continue;
                    }

                    else if (setting.StartsWith("Allow Dev Mode: "))
                    {
                        string splitString = setting.Replace("Allow Dev Mode: ", "");

                        if (splitString == "True") OWServer.allowDevMode = true;

                        continue;
                    }

                    else if (setting.StartsWith("Use Whitelist: "))
                    {
                        string splitString = setting.Replace("Use Whitelist: ", "");

                        if (splitString == "True") OWServer.usingWhitelist = true;

                        continue;
                    }

                    else if (setting.StartsWith("Wealth Warning Threshold: "))
                    {
                        string splitString = setting.Replace("Wealth Warning Threshold: ", "");
                        OWServer.warningWealthThreshold = int.Parse(splitString);
                        continue;
                    }

                    else if (setting.StartsWith("Wealth Ban Threshold: "))
                    {
                        string splitString = setting.Replace("Wealth Ban Threshold: ", "");
                        OWServer.banWealthThreshold = int.Parse(splitString);
                        continue;
                    }

                    else if (setting.StartsWith("Use Wealth System: "))
                    {
                        string splitString = setting.Replace("Use Wealth System: ", "");
                        if (splitString == "True")
                        {
                            OWServer.usingWealthSystem = true;
                        }
                        else if (splitString == "False")
                        {
                            OWServer.usingWealthSystem = false;
                        }
                        continue;
                    }

                    else if (setting.StartsWith("Use Idle System: "))
                    {
                        string splitString = setting.Replace("Use Idle System: ", "");
                        if (splitString == "True")
                        {
                            OWServer.usingIdleTimer = true;
                        }
                        else if (splitString == "False")
                        {
                            OWServer.usingIdleTimer = false;
                        }
                        continue;
                    }

                    else if (setting.StartsWith("Idle Threshold (days): "))
                    {
                        string splitString = setting.Replace("Idle Threshold (days): ", "");
                        OWServer.idleTimer = int.Parse(splitString);
                        continue;
                    }

                    else if (setting.StartsWith("Use Road System: "))
                    {
                        string splitString = setting.Replace("Use Road System: ", "");
                        if (splitString == "True")
                        {
                            OWServer.usingRoadSystem = true;
                        }
                        else if (splitString == "False")
                        {
                            OWServer.usingRoadSystem = false;
                        }
                        continue;
                    }

                    else if (setting.StartsWith("Aggressive Road Mode (WIP): "))
                    {
                        string splitString = setting.Replace("Aggressive Road Mode (WIP): ", "");
                        if (splitString == "True")
                        {
                            OWServer.aggressiveRoadMode = true;
                        }
                        else if (splitString == "False")
                        {
                            OWServer.aggressiveRoadMode = false;
                        }
                        continue;
                    }

                    else if (setting.StartsWith("Use Modlist Match: "))
                    {
                        string splitString = setting.Replace("Use Modlist Match: ", "");
                        if (splitString == "True")
                        {
                            OWServer.forceModlist = true;
                        }
                        else if (splitString == "False")
                        {
                            OWServer.forceModlist = false;
                        }
                        continue;
                    }

                    else if (setting.StartsWith("Use Modlist Config Match (WIP): "))
                    {
                        string splitString = setting.Replace("Use Modlist Config Match (WIP): ", "");
                        if (splitString == "True")
                        {
                            OWServer.forceModlistConfigs = true;
                        }
                        else if (splitString == "False")
                        {
                            OWServer.forceModlistConfigs = false;
                        }
                        continue;
                    }

                    else if (setting.StartsWith("Use Mod Verification: "))
                    {
                        string splitString = setting.Replace("Use Mod Verification: ", "");
                        if (splitString == "True")
                        {
                            OWServer.usingModVerification = true;
                        }
                        else if (splitString == "False")
                        {
                            OWServer.usingModVerification = false;
                        }
                        continue;
                    }

                    else if (setting.StartsWith("Use Chat: "))
                    {
                        string splitString = setting.Replace("Use Chat: ", "");
                        if (splitString == "True")
                        {
                            OWServer.usingChat = true;
                        }
                        else if (splitString == "False")
                        {
                            OWServer.usingChat = false;
                        }
                        continue;
                    }

                    else if (setting.StartsWith("Use Profanity filter: "))
                    {
                        string splitString = setting.Replace("Use Profanity filter: ", "");
                        if (splitString == "True")
                        {
                            OWServer.usingProfanityFilter = true;
                        }
                        else if (splitString == "False")
                        {
                            OWServer.usingProfanityFilter = false;
                        }
                        continue;
                    }
                }

                OWServer._ServerUtils.WriteServerLog("Loaded Settings File");
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

                File.WriteAllLines(OWServer.serverSettingsPath, settingsPreset);

                OWServer._ServerUtils.WriteServerLog("Generating Settings File");

                CheckSettingsFile();
            }
        }

        public void CheckMods()
        {
            List<string> modlist = new List<string>();
            OWServer.modList.Clear();

            if (!Directory.Exists(OWServer.modsFolderPath))
            {
                Directory.CreateDirectory(OWServer.modsFolderPath);
                OWServer._ServerUtils.WriteServerLog("No Mods Folder Found, Generating");
                return;
            }

            else
            {
                string[] modFolders = Directory.GetDirectories(OWServer.modsFolderPath);

                if (modFolders.Length == 0)
                {
                    OWServer._ServerUtils.WriteServerLog("No Mods Found, Ignoring");
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
                    OWServer.modList = modlist.ToList();
                    OWServer._ServerUtils.WriteServerLog("Loaded [" + OWServer.modList.Count() + "] Mods");
                }
            }
        }

        public void CheckWhitelistedMods()
        {
            List<string> whitelistedModsList = new List<string>();
            OWServer.whitelistedMods.Clear();

            if (!Directory.Exists(OWServer.whitelistedModsFolderPath))
            {
                Directory.CreateDirectory(OWServer.whitelistedModsFolderPath);
                OWServer._ServerUtils.WriteServerLog("No Whitelisted Mods Folder Found, Generating");
                return;
            }

            else
            {
                string[] modFolders = Directory.GetDirectories(OWServer.whitelistedModsFolderPath);

                if (modFolders.Length == 0) OWServer._ServerUtils.WriteServerLog("No Whitelisted Mods Found, Ignoring");

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
                    OWServer.whitelistedMods = whitelistedModsList;
                    OWServer._ServerUtils.WriteServerLog("Loaded [" + OWServer.whitelistedMods.Count() + "] Whitelisted Mods");
                }
            }
        }

        private void CheckWorldFile()
        {
            if (File.Exists(OWServer.worldSettingsPath))
            {
                string[] settings = File.ReadAllLines(OWServer.worldSettingsPath);

                foreach (string setting in settings)
                {
                    if (setting.StartsWith("Globe Coverage (0.3, 0.5, 1.0): "))
                    {
                        string splitString = setting.Replace("Globe Coverage (0.3, 0.5, 1.0): ", "");
                        OWServer.globeCoverage = float.Parse(splitString);
                        continue;
                    }

                    else if (setting.StartsWith("Seed: "))
                    {
                        string splitString = setting.Replace("Seed: ", "");
                        OWServer.seed = splitString;
                        continue;
                    }

                    else if (setting.StartsWith("Overall Rainfall (0-6): "))
                    {
                        string splitString = setting.Replace("Overall Rainfall (0-6): ", "");
                        OWServer.overallRainfall = int.Parse(splitString);
                        continue;
                    }

                    else if (setting.StartsWith("Overall Temperature (0-6): "))
                    {
                        string splitString = setting.Replace("Overall Temperature (0-6): ", "");
                        OWServer.overallTemperature = int.Parse(splitString);
                        continue;
                    }

                    else if (setting.StartsWith("Overall Population (0-6): "))
                    {
                        string splitString = setting.Replace("Overall Population (0-6): ", "");
                        OWServer.overallPopulation = int.Parse(splitString);
                        continue;
                    }
                }

                OWServer._ServerUtils.WriteServerLog("Loaded World File");
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

                File.WriteAllLines(OWServer.worldSettingsPath, settingsPreset);

                OWServer._ServerUtils.WriteServerLog("Generating World File");

                CheckWorldFile();
            }
        }

        private void CheckForBannedPlayers()
        {
            if (!File.Exists(OWServer.mainFolderPath + Path.DirectorySeparatorChar + "Banned IPs.data"))
            {
                OWServer._ServerUtils.WriteServerLog("No Bans File Found, Ignoring");
                return;
            }

            BanDataHolder list = SaveSystem.LoadBannedIPs();
            {
                OWServer.bannedIPs = list.bannedIPs;
            }

            if (OWServer.bannedIPs.Count() == 0) OWServer._ServerUtils.WriteServerLog("No Banned Players Found, Ignoring");
            else OWServer._ServerUtils.WriteServerLog("Loaded [" + OWServer.bannedIPs.Count() + "] Banned Players");
        }

        public void CheckForWhitelistedPlayers()
        {
            OWServer.whitelistedUsernames.Clear();

            if (!File.Exists(OWServer.whitelistedUsersPath))
            {
                File.Create(OWServer.whitelistedUsersPath);

                OWServer._ServerUtils.WriteServerLog("No Whitelisted Players File Found, Generating");
            }

            else
            {
                if (File.ReadAllLines(OWServer.whitelistedUsersPath).Count() == 0) OWServer._ServerUtils.WriteServerLog("No Whitelisted Players Found, Ignoring");
                else
                {
                    foreach (string str in File.ReadAllLines(OWServer.whitelistedUsersPath))
                    {
                        OWServer.whitelistedUsernames.Add(str);
                    }

                    OWServer._ServerUtils.WriteServerLog("Loaded [" + OWServer.whitelistedUsernames.Count() + "] Whitelisted Players");
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

            if (OWServer.savedClients.Find(fetch => fetch.username == client.username) != null)
            {
                client.isAdmin = OWServer.savedClients.Find(fetch => fetch.username == client.username).isAdmin;
            }
            else client.isAdmin = false;

            int devInt = 0;
            if (client.isAdmin || OWServer.allowDevMode) devInt = 1;

            int wipeInt = 0;
            if (client.toWipe) wipeInt = 1;

            int roadInt = 0;
            if (OWServer.usingRoadSystem) roadInt = 1;
            if (OWServer.usingRoadSystem && OWServer.aggressiveRoadMode) roadInt = 2;

            string name = OWServer.serverName;
            int countInt = OWServer._Networking.connectedClients.Count;

            int chatInt = 0;
            if (OWServer.usingChat) chatInt = 1;

            int profanityInt = 0;
            if (OWServer.usingProfanityFilter) profanityInt = 1;

            int modVerifyInt = 0;
            if (OWServer.usingModVerification) modVerifyInt = 1;

            if (!TryJoin(client)) return;

            if (!CompareConnectingClientVersion(client, playerVersion)) return;

            if (!CompareConnectingClientWithWhitelist(client)) return;

            if (!ParseClientUsername(client)) return;

            void SendNewGameData()
            {
                OWServer._PlayerUtils.SaveNewPlayerFile(client.username, client.password);

                float mmGC = OWServer.globeCoverage;
                string? mmS = OWServer.seed;
                int mmOR = OWServer.overallRainfall;
                int mmOT = OWServer.overallTemperature;
                int mmOP = OWServer.overallPopulation;

                string settlementString = "";
                foreach (KeyValuePair<string, List<string>> pair in OWServer.savedSettlements)
                {
                    settlementString += pair.Key + ":" + pair.Value[0] + "»";
                }
                if (settlementString.Count() > 0) settlementString = settlementString.Remove(settlementString.Count() - 1, 1);

                OWServer._Networking.SendData(client, "MapDetails│" + mmGC + "│" + mmS + "│" + mmOR + "│" + mmOT + "│" + mmOP + "│" + settlementString + "│" + devInt + "│" + wipeInt + "│" + roadInt + "│" + countInt + "│" + chatInt + "│" + profanityInt + "│" + modVerifyInt + "│" + name);
            }

            void SendLoadGameData()
            {
                string settlementString = "";
                foreach (KeyValuePair<string, List<string>> pair in OWServer.savedSettlements)
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

                foreach(ServerClient sc in OWServer.savedClients)
                {
                    if (sc.username == client.username)
                    {
                        sc.giftString.Clear();
                        sc.tradeString.Clear();
                        break;
                    }
                }

                SaveSystem.SaveUserData(client);

                OWServer._Networking.SendData(client, dataToSend);
            }

            foreach (ServerClient savedClient in OWServer.savedClients)
            {
                if (savedClient.username.ToLower() == client.username.ToLower())
                {
                    userPresent = true;

                    client.username = savedClient.username;

                    if (savedClient.password == client.password)
                    {
                        if (!OWServer._ServerUtils.CompareClientIPWithBans(client)) return;

                        if (!OWServer._ServerUtils.CompareModsWithClient(client, playerMods)) return;

                        OWServer._ServerUtils.CompareConnectingClientWithConnecteds(client);

                        OWServer._ServerUtils.UpdateTitle();

                        OWServer._ServerUtils.WriteServerLog("Player [" + client.username + "] " + "[" + ((IPEndPoint)client.tcp.Client.RemoteEndPoint).Address.ToString() + "] " + "Has Connected");

                        RefreshClientCount(client);

                        if (joinMode == "NewGame")
                        {
                            SendNewGameData();
                            OWServer._ServerUtils.WriteServerLog("Player [" + client.username + "] Has Reset Game Progress");
                        }

                        else if (joinMode == "LoadGame")
                        {
                            OWServer._PlayerUtils.GiveSavedDataToPlayer(client);
                            SendLoadGameData();
                        }
                    }

                    else
                    {
                        OWServer._Networking.SendData(client, "Disconnect│WrongPassword");

                        client.disconnectFlag = true;
                        OWServer._ServerUtils.WriteServerLog("Player [" + client.username + "] Has Been Kicked For: [Wrong Password]");
                        return;
                    }

                    break;
                }
            }

            if (userPresent) return;

            else
            {
                if (!OWServer._ServerUtils.CompareClientIPWithBans(client)) return;

                if (!OWServer._ServerUtils.CompareModsWithClient(client, playerMods)) return;

                OWServer._ServerUtils.CompareConnectingClientWithConnecteds(client);

                OWServer._ServerUtils.UpdateTitle();

                OWServer._ServerUtils.WriteServerLog("New Player [" + client.username + "] " + "[" + ((IPEndPoint)client.tcp.Client.RemoteEndPoint).Address.ToString() + "] " + "Has Connected For The First Time");

                OWServer._PlayerUtils.SaveNewPlayerFile(client.username, client.password);

                if (joinMode == "NewGame")
                {
                    SendNewGameData();
                }
      
                else if (joinMode == "LoadGame")
                {
                    SendLoadGameData();
                    OWServer._ServerUtils.WriteServerLog("Player [" + client.username + "] Has Registered With Existing Save");
                }
            }
        }

        public void SendChatMessage(ServerClient client, string data)
        {
            string message = data.Split('│')[2];

            string messageForConsole = "Chat - [" + client.username + "] " + message;

            OWServer._ServerUtils.WriteServerLog(messageForConsole);

            OWServer.chatCache.Add("[" + DateTime.Now + "]" + " │ " + messageForConsole);

            try
            {
                foreach (ServerClient sc in OWServer._Networking.connectedClients)
                {
                    if (sc == client) continue;
                    else OWServer._Networking.SendData(sc, data);
                }
            }
            catch { }
        }

        public bool CompareModsWithClient(ServerClient client, string data)
        {
            if (client.isAdmin) return true;
            if (!OWServer.forceModlist) return true;

            string[] clientMods = data.Split('»');

            string flaggedMods = "";

            bool flagged = false;

            foreach (string clientMod in clientMods)
            {
                if (OWServer.whitelistedMods.Contains(clientMod)) continue;
                if (!OWServer.modList.Contains(clientMod))
                {
                    flagged = true;
                    flaggedMods += clientMod + "»";
                }
            }

            foreach (string serverMod in OWServer.modList)
            {
                if (!clientMods.Contains(serverMod))
                {
                    flagged = true;
                    flaggedMods += serverMod + "»";
                }
            }

            if (flagged)
            {
                OWServer._ServerUtils.WriteServerLog("Player [" + client.username + "] " + "Doesn't Have The Required Mod Or Mod Files Mismatch!");
                flaggedMods = flaggedMods.Remove(flaggedMods.Count() - 1, 1);
                OWServer._Networking.SendData(client, "Disconnect│WrongMods│" + flaggedMods);

                client.disconnectFlag = true;
                return false;
            }
            else return true;
        }

        public void CompareConnectingClientWithConnecteds(ServerClient client)
        {
            foreach (ServerClient sc in OWServer._Networking.connectedClients)
            {
                if (sc.username == client.username)
                {
                    if (sc == client) continue;

                    OWServer._Networking.SendData(sc, "Disconnect│AnotherLogin");
                    sc.disconnectFlag = true;
                    break;
                }
            }
        }

        public bool CompareConnectingClientWithWhitelist(ServerClient client)
        {
            bool allowConnection = false;
            // TODO: The usingWhitelist check should be done before this method is even called.
            if (!OWServer.usingWhitelist || client.isAdmin || OWServer.whitelistedUsernames.Any(x => x == client.username)) allowConnection = true;
            else
            {
                OWServer._Networking.SendData(client, "Disconnect│Whitelist");
                client.disconnectFlag = true;
                WriteServerLog("Player [" + client.username + "] Tried To Join But Is Not Whitelisted");
            }
            return allowConnection;
        }

        public bool CompareConnectingClientVersion(ServerClient client, string clientVersion)
        {
            bool allowToConnect = false;
            string latestVersion = "";

            try
            {
                WebClient wc = new WebClient();
                latestVersion = wc.DownloadString("https://raw.githubusercontent.com/TastyLollipop/OpenWorld/main/Latest%20Versions%20Cache");
                latestVersion = latestVersion.Split('│')[2].Replace("- Latest Client Version: ", "");
                latestVersion = latestVersion.Remove(0, 1);
                latestVersion = latestVersion.Remove(latestVersion.Count() - 1, 1);
            }
            catch {
                OWServer._ServerUtils.WriteServerLog($"Player {client.username} tried to connect, but the latest version number could not be retrieved. Allowing user to connect.", OWServer.WARN_COLOR);
                allowToConnect = true; 
            }
            if (clientVersion == latestVersion) allowToConnect = true;
            else
            {
                OWServer._Networking.SendData(client, "Disconnect│Version");
                client.disconnectFlag = true;
                WriteServerLog($"Player {client.username} tried to connect, but a version mismatch was detected. Disallowing connection.");
            }
            return allowToConnect;
        }

        public bool CompareClientIPWithBans(ServerClient client)
        {
            foreach(KeyValuePair<string, string> pair in OWServer.bannedIPs)
            {
                if (pair.Key == ((IPEndPoint)client.tcp.Client.RemoteEndPoint).Address.ToString() || pair.Value == client.username)
                {
                    OWServer._Networking.SendData(client, "Disconnect│Banned");
                    client.disconnectFlag = true;
                    WriteServerLog("Player [" + client.username + "] Tried To Join But Is Banned");
                    return false;
                }
            }

            return true;
        }

        public void RefreshClientCount(ServerClient client)
        {
            int count = OWServer._Networking.connectedClients.Count;

            foreach (ServerClient sc in OWServer._Networking.connectedClients)
            {
                if (sc == client) continue;

                try { OWServer._Networking.SendData(sc, "│PlayerCountRefresh│" + count + "│"); }
                catch { continue; }
            }
        }

        public bool TryJoin(ServerClient client)
        {
            if (!client.isAdmin && OWServer._Networking.connectedClients.Count() >= OWServer.maxPlayers + 1)
            {
                OWServer._Networking.SendData(client, "Disconnect│ServerFull");
                client.disconnectFlag = true;
                return false;
            }
            else return true;
        }

        public bool ParseClientUsername(ServerClient client)
        {
            if (string.IsNullOrWhiteSpace(client.username) || (!client.username.All(character => Char.IsLetterOrDigit(character) || character == '_' || character == '-')))
            {
                OWServer._Networking.SendData(client, "Disconnect│Corrupted");
                client.disconnectFlag = true;
                return false;
            }
            else return true;
        }
        public void WriteServerLog(string output, ConsoleColor color = OWServer.DEFAULT_COLOR)
        {
            Console.ForegroundColor = color;
            // TODO: Build string then write
            foreach (string line in output.Split("\n"))
            {
                string parsedLine = (string.IsNullOrWhiteSpace(line) ? "\n" : $"[{DateTime.Now.ToString("HH:mm:ss")}] | {line}");
                Console.WriteLine(parsedLine);

                if (line.StartsWith("Chat - [")) WriteToLog(line, "Chat");
                else if (line.StartsWith("Gift Done Between")) WriteToLog(line, "Gift");
                else if (line.StartsWith("Trade Done Between")) WriteToLog(line, "Trade");
                else if (line.StartsWith("Barter Done Between")) WriteToLog(line, "Barter");
                else if (line.StartsWith("Spy Done Between")) WriteToLog(line, "Spy");
                else if (line.StartsWith("PvP Done Between")) WriteToLog(line, "PvP");
                else if (line.StartsWith("Visit Done Between")) WriteToLog(line, "Visit");
                else WriteToLog(line, "Normal");
            }
            
        }
      

        public void WriteToLog(string data, string logMode)
        {
            string pathToday = OWServer.logFolderPath + Path.DirectorySeparatorChar + DateTime.Today.Month + "-" + DateTime.Today.Day + "-" + DateTime.Today.Year;
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