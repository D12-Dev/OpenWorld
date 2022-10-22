using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace OpenWorldServer
{
    public class PlayerUtils
    {
        public static void SaveNewPlayerFile(string username, string password)
        {
            foreach (ServerClient savedClient in Server.savedClients)
            {
                if (savedClient.username == username)
                {
                    if (!string.IsNullOrWhiteSpace(savedClient.homeTileID)) Server._WorldUtils.RemoveSettlement(savedClient, savedClient.homeTileID);
                    savedClient.wealth = 0;
                    savedClient.pawnCount = 0;
                    SaveSystem.SaveUserData(savedClient);
                    return;
                }
            }

            ServerClient dummy = new ServerClient(null);
            dummy.username = username;
            dummy.password = password;
            dummy.homeTileID = null;

            Server.savedClients.Add(dummy);
            SaveSystem.SaveUserData(dummy);
        }

        public static void GiveSavedDataToPlayer(ServerClient client)
        {
            foreach (ServerClient savedClient in Server.savedClients)
            {
                if (savedClient.username == client.username)
                {
                    client.homeTileID = savedClient.homeTileID;
                    client.giftString = savedClient.giftString;
                    client.tradeString = savedClient.tradeString;
                    client.pawnCount = savedClient.pawnCount;
                    client.wealth = savedClient.wealth;
                    client.isImmunized = savedClient.isImmunized;
                    return;
                }
            }
        }

        public static void CheckAllAvailablePlayers(bool newLine)
        {
            if (newLine) Console.WriteLine("");

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUtils.LogToConsole("Player Check:");
            Console.ForegroundColor = ConsoleColor.White;

            CheckSavedPlayers();
            CheckForBannedPlayers();
            CheckForWhitelistedPlayers();
        }

        private static void CheckSavedPlayers()
        {
            if (!Directory.Exists(Server.playersFolderPath))
            {
                Directory.CreateDirectory(Server.playersFolderPath);
                ConsoleUtils.LogToConsole("No Players Folder Found, Generating");
                return;
            }

            else
            {
                string[] playerFiles = Directory.GetFiles(Server.playersFolderPath);

                foreach (string file in playerFiles)
                {
                    if (Server.usingIdleTimer)
                    {
                        FileInfo fi = new FileInfo(file);
                        if (fi.LastAccessTime < DateTime.Now.AddDays(-Server.idleTimer))
                        {
                            fi.Delete();
                            continue;
                        }
                    }

                    MainDataHolder data = SaveSystem.LoadUserData(Path.GetFileNameWithoutExtension(file));
                    {
                        ServerClient dummy = data.serverclient;
                        Server.savedClients.Add(dummy);
                        if (!string.IsNullOrWhiteSpace(dummy.homeTileID))
                        {
                            try { Server.savedSettlements.Add(dummy.homeTileID, new List<string>() { dummy.username }); }
                            catch 
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                ConsoleUtils.LogToConsole("Error! Player " + dummy.username + " Is Using A Cloned Entry! Skipping Entry");
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                        }
                    }
                }

                if (Server.savedClients.Count == 0) ConsoleUtils.LogToConsole("No Saved Players Found, Ignoring");
                else ConsoleUtils.LogToConsole("Loaded [" + Server.savedClients.Count + "] Player Files");
            }
        }

        private static void CheckForBannedPlayers()
        {
            if (!File.Exists(Server.mainFolderPath + Path.DirectorySeparatorChar + "Banned IPs.data"))
            {
                ConsoleUtils.LogToConsole("No Bans File Found, Ignoring");
                return;
            }

            BanDataHolder list = SaveSystem.LoadBannedIPs();
            {
                Server.bannedIPs = list.bannedIPs;
            }

            if (Server.bannedIPs.Count == 0) ConsoleUtils.LogToConsole("No Banned Players Found, Ignoring");
            else ConsoleUtils.LogToConsole("Loaded [" + Server.bannedIPs.Count + "] Banned Players");
        }

        private static void CheckForWhitelistedPlayers()
        {
            Server.whitelistedUsernames.Clear();

            if (!File.Exists(Server.whitelistedUsersPath))
            {
                File.Create(Server.whitelistedUsersPath);

                ConsoleUtils.LogToConsole("No Whitelisted Players File Found, Generating");
            }

            else
            {
                if (File.ReadAllLines(Server.whitelistedUsersPath).Count() == 0) ConsoleUtils.LogToConsole("No Whitelisted Players Found, Ignoring");
                else
                {
                    foreach (string str in File.ReadAllLines(Server.whitelistedUsersPath))
                    {
                        Server.whitelistedUsernames.Add(str);
                    }

                    ConsoleUtils.LogToConsole("Loaded [" + Server.whitelistedUsernames.Count + "] Whitelisted Players");
                }
            }
        }

        public static void CheckForPlayerWealth(ServerClient client)
        {
            if (Server.usingWealthSystem == false) return;
            if (Server.banWealthThreshold == 0 && Server.warningWealthThreshold == 0) return;
            if (client.isAdmin) return;

            int wealthToCompare = (int) Server.savedClients.Find(fetch => fetch.username == client.username).wealth;

            if (client.wealth - wealthToCompare > Server.banWealthThreshold && Server.banWealthThreshold > 0)
            {
                SaveSystem.SaveUserData(client);
                Server.savedClients.Find(fetch => fetch.username == client.username).wealth = client.wealth;
                Server.savedClients.Find(fetch => fetch.username == client.username).pawnCount = client.pawnCount;

                Server.bannedIPs.Add(((IPEndPoint)client.tcp.Client.RemoteEndPoint).Address.ToString(), client.username);
                client.disconnectFlag = true;
                SaveSystem.SaveBannedIPs(Server.bannedIPs);

                Console.ForegroundColor = ConsoleColor.Red;
                ConsoleUtils.LogToConsole("Player [" + client.username + "]'s Wealth Triggered Alarm [" + wealthToCompare + " > " + (int)client.wealth + "], Banning");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else if (client.wealth - wealthToCompare > Server.warningWealthThreshold && Server.warningWealthThreshold > 0)
            {
                SaveSystem.SaveUserData(client);
                Server.savedClients.Find(fetch => fetch.username == client.username).wealth = client.wealth;
                Server.savedClients.Find(fetch => fetch.username == client.username).pawnCount = client.pawnCount;

                Console.ForegroundColor = ConsoleColor.Yellow;
                ConsoleUtils.LogToConsole("Player [" + client.username + "]'s Wealth Triggered Warning [" + wealthToCompare + " > " + (int) client.wealth + "]");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                SaveSystem.SaveUserData(client);
                Server.savedClients.Find(fetch => fetch.username == client.username).wealth = client.wealth;
                Server.savedClients.Find(fetch => fetch.username == client.username).pawnCount = client.pawnCount;
            }
        }

        public static bool CheckForConnectedPlayers(string tileID)
        {
            foreach (ServerClient client in Server._Networking.connectedClients)
            {
                if (client.homeTileID == tileID) return true;
            }

            return false;
        }

        public static bool CheckForPlayerShield(string tileID)
        {
            foreach (ServerClient client in Server._Networking.connectedClients)
            {
                if (client.homeTileID == tileID && !client.eventShielded && !client.isImmunized)
                {
                    client.eventShielded = true;
                    return true;
                }
            }

            return false;
        }

        public static bool CheckForPvpAvailability(string tileID)
        {
            foreach (ServerClient client in Server._Networking.connectedClients)
            {
                if (client.homeTileID == tileID && !client.inRTSE && !client.isImmunized)
                {
                    client.inRTSE = true;
                    return true;
                }
            }

            return false;
        }

        public static string GetSpyData(string tileID, ServerClient origin)
        {
            foreach (ServerClient client in Server._Networking.connectedClients)
            {
                if (client.homeTileID == tileID)
                {
                    string dataToReturn = client.pawnCount.ToString() + "»" + client.wealth.ToString() + "»" + client.eventShielded + "»" + client.inRTSE;

                    if (client.giftString.Count > 0) dataToReturn += "»" + "True";
                    else dataToReturn += "»" + "False";

                    if (client.tradeString.Count > 0) dataToReturn += "»" + "True";
                    else dataToReturn += "»" + "False";

                    Random rnd = new Random();
                    int chance = rnd.Next(0, 2);
                    if (chance == 1) Server._Networking.SendData(client, "Spy│" + origin.username);

                    ConsoleUtils.LogToConsole("Spy Done Between [" + origin.username + "] And [" + client.username + "]");

                    return dataToReturn;
                }
            }

            return "";
        }

        public static void SendEventToPlayer(ServerClient invoker, string data)
        {
            string dataToSend = "ForcedEvent│" + data.Split('│')[1];

            foreach (ServerClient sc in Server._Networking.connectedClients)
            {
                if (sc.homeTileID == data.Split('│')[2])
                {
                    ConsoleUtils.LogToConsole("Player [" + invoker.username + "] Has Sent Forced Event [" + data.Split('│')[1] + "] To [" + sc.username + "]");
                    Server._Networking.SendData(sc, dataToSend);
                    break;
                }
            }
        }

        public static void SendGiftToPlayer(ServerClient invoker, string data)
        {
            string tileToSend = data.Split('│')[1];
            string dataToSend = "GiftedItems│" + data.Split('│')[2];

            try
            {
                string sendMode = data.Split('│')[3];

                if (!string.IsNullOrWhiteSpace(sendMode) && sendMode == "Pod")
                {
                    foreach (ServerClient sc in Server._Networking.connectedClients)
                    {
                        if (sc == invoker) continue;
                        if (sc.homeTileID == tileToSend) continue;

                        Server._Networking.SendData(sc, "│RenderTransportPod│" + invoker.homeTileID + "│" + tileToSend + "│");
                    }
                }
            }
            catch { }

            foreach (ServerClient sc in Server._Networking.connectedClients)
            {
                if (sc.homeTileID == tileToSend)
                {
                    Server._Networking.SendData(sc, dataToSend);
                    ConsoleUtils.LogToConsole("Gift Done Between [" + invoker.username + "] And [" + sc.username + "]");
                    return;
                }
            }

            dataToSend = dataToSend.Replace("GiftedItems│", "");

            foreach(ServerClient sc in Server.savedClients)
            {
                if (sc.homeTileID == tileToSend)
                {
                    sc.giftString.Add(dataToSend);
                    SaveSystem.SaveUserData(sc);
                    ConsoleUtils.LogToConsole("Gift Done Between [" + invoker.username + "] And [" + sc.username + "] But Was Offline. Saving");
                    return;
                }
            }
        }

        public static void SendTradeRequestToPlayer(ServerClient invoker, string data)
        {
            string dataToSend = "TradeRequest│" + invoker.username + "│" + data.Split('│')[2] + "│" + data.Split('│')[3];

            foreach (ServerClient sc in Server._Networking.connectedClients)
            {
                if (sc.homeTileID == data.Split('│')[1])
                {
                    Server._Networking.SendData(sc, dataToSend);
                    return;
                }
            }
        }

        public static void SendBarterRequestToPlayer(ServerClient invoker, string data)
        {
            string dataToSend = "BarterRequest│" + invoker.homeTileID + "│" + data.Split('│')[2];

            foreach (ServerClient sc in Server._Networking.connectedClients)
            {
                if (sc.homeTileID == data.Split('│')[1])
                {
                    Server._Networking.SendData(sc, dataToSend);
                    return;
                }
            }
        }
    }
}
