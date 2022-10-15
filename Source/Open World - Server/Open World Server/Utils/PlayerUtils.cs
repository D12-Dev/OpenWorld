using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Open_World_Server
{
    public class PlayerUtils
    {
        public void SaveNewPlayerFile(string username, string password)
        {
            foreach (ServerClient savedClient in OWServer.savedClients)
            {
                if (savedClient.username == username)
                {
                    if (!string.IsNullOrWhiteSpace(savedClient.homeTileID)) OWServer._WorldUtils.RemoveSettlement(savedClient, savedClient.homeTileID);
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

            OWServer.savedClients.Add(dummy);
            SaveSystem.SaveUserData(dummy);
        }

        public void GiveSavedDataToPlayer(ServerClient client)
        {
            foreach (ServerClient savedClient in OWServer.savedClients)
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

        public void CheckSavedPlayers()
        {
            if (!Directory.Exists(OWServer.playersFolderPath))
            {
                Directory.CreateDirectory(OWServer.playersFolderPath);
                ServerUtils.WriteServerLog("No Players Folder Found, Generating");
                return;
            }

            else
            {
                string[] playerFiles = Directory.GetFiles(OWServer.playersFolderPath);

                foreach (string file in playerFiles)
                {
                    if (OWServer.usingIdleTimer)
                    {
                        FileInfo fi = new FileInfo(file);
                        if (fi.LastAccessTime < DateTime.Now.AddDays(-OWServer.idleTimer))
                        {
                            fi.Delete();
                            continue;
                        }
                    }

                    MainDataHolder data = SaveSystem.LoadUserData(Path.GetFileNameWithoutExtension(file));
                    {
                        ServerClient dummy = data.serverclient;
                        OWServer.savedClients.Add(dummy);
                        if (!string.IsNullOrWhiteSpace(dummy.homeTileID))
                        {
                            try { OWServer.savedSettlements.Add(dummy.homeTileID, new List<string>() { dummy.username }); }
                            catch 
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                ServerUtils.WriteServerLog("Error! Player " + dummy.username + " Is Using A Cloned Entry! Skipping Entry");
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                        }
                    }
                }

                if (OWServer.savedClients.Count == 0) ServerUtils.WriteServerLog("No Saved Players Found, Ignoring");
                else ServerUtils.WriteServerLog("Loaded [" + OWServer.savedClients.Count + "] Player Files");
            }
        }

        public void CheckForPlayerWealth(ServerClient client)
        {
            if (OWServer.usingWealthSystem == false) return;
            if (OWServer.banWealthThreshold == 0 && OWServer.warningWealthThreshold == 0) return;
            if (client.isAdmin) return;

            int wealthToCompare = (int) OWServer.savedClients.Find(fetch => fetch.username == client.username).wealth;

            if (client.wealth - wealthToCompare > OWServer.banWealthThreshold && OWServer.banWealthThreshold > 0)
            {
                SaveSystem.SaveUserData(client);
                OWServer.savedClients.Find(fetch => fetch.username == client.username).wealth = client.wealth;
                OWServer.savedClients.Find(fetch => fetch.username == client.username).pawnCount = client.pawnCount;

                OWServer.bannedIPs.Add(((IPEndPoint)client.tcp.Client.RemoteEndPoint).Address.ToString(), client.username);
                client.disconnectFlag = true;
                SaveSystem.SaveBannedIPs(OWServer.bannedIPs);

                Console.ForegroundColor = ConsoleColor.Red;
                ServerUtils.WriteServerLog("Player [" + client.username + "]'s Wealth Triggered Alarm [" + wealthToCompare + " > " + (int)client.wealth + "], Banning");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else if (client.wealth - wealthToCompare > OWServer.warningWealthThreshold && OWServer.warningWealthThreshold > 0)
            {
                SaveSystem.SaveUserData(client);
                OWServer.savedClients.Find(fetch => fetch.username == client.username).wealth = client.wealth;
                OWServer.savedClients.Find(fetch => fetch.username == client.username).pawnCount = client.pawnCount;

                Console.ForegroundColor = ConsoleColor.Yellow;
                ServerUtils.WriteServerLog("Player [" + client.username + "]'s Wealth Triggered Warning [" + wealthToCompare + " > " + (int) client.wealth + "]");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                SaveSystem.SaveUserData(client);
                OWServer.savedClients.Find(fetch => fetch.username == client.username).wealth = client.wealth;
                OWServer.savedClients.Find(fetch => fetch.username == client.username).pawnCount = client.pawnCount;
            }
        }

        public bool CheckForConnectedPlayers(string tileID)
        {
            foreach (ServerClient client in OWServer._Networking.connectedClients)
            {
                if (client.homeTileID == tileID) return true;
            }

            return false;
        }

        public bool CheckForPlayerShield(string tileID)
        {
            foreach (ServerClient client in OWServer._Networking.connectedClients)
            {
                if (client.homeTileID == tileID && !client.eventShielded && !client.isImmunized)
                {
                    client.eventShielded = true;
                    return true;
                }
            }

            return false;
        }

        public bool CheckForPvpAvailability(string tileID)
        {
            foreach (ServerClient client in OWServer._Networking.connectedClients)
            {
                if (client.homeTileID == tileID && !client.inRTSE && !client.isImmunized)
                {
                    client.inRTSE = true;
                    return true;
                }
            }

            return false;
        }

        public string GetSpyData(string tileID, ServerClient origin)
        {
            foreach (ServerClient client in OWServer._Networking.connectedClients)
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
                    if (chance == 1) OWServer._Networking.SendData(client, "Spy│" + origin.username);

                    ServerUtils.WriteServerLog("Spy Done Between [" + origin.username + "] And [" + client.username + "]");

                    return dataToReturn;
                }
            }

            return "";
        }

        public void SendEventToPlayer(ServerClient invoker, string data)
        {
            string dataToSend = "ForcedEvent│" + data.Split('│')[1];

            foreach (ServerClient sc in OWServer._Networking.connectedClients)
            {
                if (sc.homeTileID == data.Split('│')[2])
                {
                    ServerUtils.WriteServerLog("Player [" + invoker.username + "] Has Sent Forced Event [" + data.Split('│')[1] + "] To [" + sc.username + "]");
                    OWServer._Networking.SendData(sc, dataToSend);
                    break;
                }
            }
        }

        public void SendGiftToPlayer(ServerClient invoker, string data)
        {
            string tileToSend = data.Split('│')[1];
            string dataToSend = "GiftedItems│" + data.Split('│')[2];

            try
            {
                string sendMode = data.Split('│')[3];

                if (!string.IsNullOrWhiteSpace(sendMode) && sendMode == "Pod")
                {
                    foreach (ServerClient sc in OWServer._Networking.connectedClients)
                    {
                        if (sc == invoker) continue;
                        if (sc.homeTileID == tileToSend) continue;

                        OWServer._Networking.SendData(sc, "│RenderTransportPod│" + invoker.homeTileID + "│" + tileToSend + "│");
                    }
                }
            }
            catch { }

            foreach (ServerClient sc in OWServer._Networking.connectedClients)
            {
                if (sc.homeTileID == tileToSend)
                {
                    OWServer._Networking.SendData(sc, dataToSend);
                    ServerUtils.WriteServerLog("Gift Done Between [" + invoker.username + "] And [" + sc.username + "]");
                    return;
                }
            }

            dataToSend = dataToSend.Replace("GiftedItems│", "");

            foreach(ServerClient sc in OWServer.savedClients)
            {
                if (sc.homeTileID == tileToSend)
                {
                    sc.giftString.Add(dataToSend);
                    SaveSystem.SaveUserData(sc);
                    ServerUtils.WriteServerLog("Gift Done Between [" + invoker.username + "] And [" + sc.username + "] But Was Offline. Saving");
                    return;
                }
            }
        }

        public void SendTradeRequestToPlayer(ServerClient invoker, string data)
        {
            string dataToSend = "TradeRequest│" + invoker.username + "│" + data.Split('│')[2] + "│" + data.Split('│')[3];

            foreach (ServerClient sc in OWServer._Networking.connectedClients)
            {
                if (sc.homeTileID == data.Split('│')[1])
                {
                    OWServer._Networking.SendData(sc, dataToSend);
                    return;
                }
            }
        }

        public void SendBarterRequestToPlayer(ServerClient invoker, string data)
        {
            string dataToSend = "BarterRequest│" + invoker.homeTileID + "│" + data.Split('│')[2];

            foreach (ServerClient sc in OWServer._Networking.connectedClients)
            {
                if (sc.homeTileID == data.Split('│')[1])
                {
                    OWServer._Networking.SendData(sc, dataToSend);
                    return;
                }
            }
        }
    }
}
