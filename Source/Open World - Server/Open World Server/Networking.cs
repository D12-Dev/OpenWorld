using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Open_World_Server
{
    public class Networking
    {
        private TcpListener server;
        public IPAddress localAddress;
        public int serverPort = 0;

        public List<ServerClient> connectedClients = new List<ServerClient>();
        public List<ServerClient> disconnectedClients = new List<ServerClient>();

        public void ReadyServer()
        {
            server = new TcpListener(localAddress, serverPort);
            server.Start();

            MainProgram._ServerUtils.UpdateTitle();

            MainProgram._ServerUtils.WriteServerLog("Server Started");
            MainProgram._ServerUtils.WriteServerLog("Type 'Help' To See Available Commands");

            MainProgram._Threading.GenerateThreads(1);

            ListenForIncomingUsers();
        }

        private void ListenForIncomingUsers()
        {
            server.BeginAcceptTcpClient(AcceptClients, server);
        }

        private void AcceptClients(IAsyncResult ar)
        {
            TcpListener listener = (TcpListener)ar.AsyncState;

            ServerClient newServerClient = new ServerClient(listener.EndAcceptTcpClient(ar));

            connectedClients.Add(newServerClient);

            MainProgram._Threading.GenerateClientThread(newServerClient);

            ListenForIncomingUsers();
        }

        public void ListenToClient(ServerClient client)
        {
            NetworkStream s = client.tcp.GetStream();
            StreamWriter sw = new StreamWriter(s);
            StreamReader sr = new StreamReader(s, true);

            while (true)
            {
                Thread.Sleep(1);

                try
                {
                    if (client.disconnectFlag)
                    {
                        disconnectedClients.Add(client);
                        return;
                    }

                    else if (!client.disconnectFlag && s.DataAvailable)
                    {
                        string encryptedData = sr.ReadLine();
                        string data = MainProgram._Encryption.DecryptString(encryptedData);
                        Debug.WriteLine(data);
                        
                        if (encryptedData != null)
                        {
                            if (encryptedData.StartsWith(MainProgram._Encryption.EncryptString("Connect│")))
                            {
                                MainProgram._ServerUtils.LoginProcedures(client, data);
                            }

                            else if (encryptedData.StartsWith(MainProgram._Encryption.EncryptString("ChatMessage│")))
                            {
                                MainProgram._ServerUtils.SendChatMessage(client, data);
                                continue;
                            }

                            else if (encryptedData.StartsWith(MainProgram._Encryption.EncryptString("NewSettlementID│")))
                            {
                                try
                                {
                                    client.wealth = float.Parse(data.Split('│')[2]);
                                    client.pawnCount = int.Parse(data.Split('│')[3]);

                                    MainProgram._PlayerUtils.CheckForPlayerWealth(client);
                                }
                                catch { }

                                MainProgram._WorldUtils.CheckForTileDisponibility(client, data.Split('│')[1]);
                                continue;
                            }

                            else if (encryptedData.StartsWith(MainProgram._Encryption.EncryptString("AbandonSettlementID│")))
                            {
                                if (client.homeTileID != data.Split('│')[1] || string.IsNullOrWhiteSpace(client.homeTileID)) continue;
                                else MainProgram._WorldUtils.RemoveSettlement(client, data.Split('│')[1]);
                                continue;
                            }

                            else if (encryptedData == MainProgram._Encryption.EncryptString("│NoSettlementInLoad│"))
                            {
                                if (string.IsNullOrWhiteSpace(client.homeTileID)) continue;
                                else MainProgram._WorldUtils.RemoveSettlement(client, client.homeTileID);
                                continue;
                            }

                            else if (encryptedData.StartsWith(MainProgram._Encryption.EncryptString("ForceEvent│")))
                            {
                                string dataToSend = "";

                                if (MainProgram._PlayerUtils.CheckForConnectedPlayers(data.Split('│')[2]))
                                {
                                    if (MainProgram._PlayerUtils.CheckForPlayerShield(data.Split('│')[2]))
                                    {
                                        dataToSend = "│SentEvent│Confirm│";

                                        MainProgram._PlayerUtils.SendEventToPlayer(client, data);
                                    }

                                    else dataToSend = "│SentEvent│Deny│";
                                }
                                else dataToSend = "│SentEvent│Deny│";

                                SendData(client, dataToSend);

                                continue;
                            }

                            else if (encryptedData.StartsWith(MainProgram._Encryption.EncryptString("SendGiftTo│")))
                            {
                                MainProgram._PlayerUtils.SendGiftToPlayer(client, data);
                                continue;
                            }

                            else if (encryptedData.StartsWith(MainProgram._Encryption.EncryptString("SendTradeTo│")))
                            {
                                string dataToSend = "";

                                if (MainProgram._PlayerUtils.CheckForConnectedPlayers(data.Split('│')[1]))
                                {
                                    dataToSend = "│SentTrade│Confirm│";

                                    MainProgram._PlayerUtils.SendTradeRequestToPlayer(client, data);
                                }
                                else dataToSend = "│SentTrade│Deny│";

                                SendData(client, dataToSend);

                                continue;
                            }

                            else if (encryptedData.StartsWith(MainProgram._Encryption.EncryptString("SendBarterTo│")))
                            {
                                string dataToSend = "";

                                if (MainProgram._PlayerUtils.CheckForConnectedPlayers(data.Split('│')[1]))
                                {
                                    dataToSend = "│SentBarter│Confirm│";

                                    MainProgram._PlayerUtils.SendBarterRequestToPlayer(client, data);
                                }
                                else dataToSend = "│SentBarter│Deny│";

                                SendData(client, dataToSend);

                                continue;
                            }

                            else if (encryptedData.StartsWith(MainProgram._Encryption.EncryptString("TradeStatus│")))
                            {
                                string username = data.Split('│')[2];
                                ServerClient target = null;

                                foreach(ServerClient sc in connectedClients)
                                {
                                    if (sc.username == username)
                                    {
                                        target = sc;
                                        break;
                                    }
                                }

                                if (target == null) return;
                                
                                if (encryptedData.StartsWith(MainProgram._Encryption.EncryptString("TradeStatus│Deal│")))
                                {
                                    SendData(target, "│SentTrade│Deal│");

                                    MainProgram._ServerUtils.WriteServerLog("Trade Done Between [" + target.username + "] And [" + client.username + "]");
                                }

                                else if (encryptedData.StartsWith(MainProgram._Encryption.EncryptString("TradeStatus│Reject│")))
                                {
                                    SendData(target, "│SentTrade│Reject│");
                                }

                                continue;
                            }

                            else if (encryptedData.StartsWith(MainProgram._Encryption.EncryptString("BarterStatus│")))
                            {
                                string username = data.Split('│')[2];
                                ServerClient target = null;

                                foreach (ServerClient sc in connectedClients)
                                {
                                    if (sc.username == username)
                                    {
                                        target = sc;
                                        break;
                                    }
                                    else if (sc.homeTileID == username)
                                    {
                                        target = sc;
                                        break;
                                    }
                                }

                                if (target == null) return;

                                if (encryptedData.StartsWith(MainProgram._Encryption.EncryptString("BarterStatus│Deal│")))
                                {
                                    SendData(target, "│SentBarter│Deal│");

                                    MainProgram._ServerUtils.WriteServerLog("Barter Done Between [" + target.username + "] And [" + client.username + "]");
                                }

                                else if (encryptedData.StartsWith(MainProgram._Encryption.EncryptString("BarterStatus│Reject│")))
                                {
                                    SendData(target, "│SentBarter│Reject│");
                                }

                                else if (encryptedData.StartsWith(MainProgram._Encryption.EncryptString("BarterStatus│Rebarter│")))
                                {
                                    SendData(target, "│SentBarter│Rebarter│" + client.username + "│" + data.Split('│')[3]);
                                }

                                continue;
                            }

                            else if (encryptedData.StartsWith(MainProgram._Encryption.EncryptString("GetSpyInfo│")))
                            {
                                string dataToSend = "";

                                if (MainProgram._PlayerUtils.CheckForConnectedPlayers(data.Split('│')[1]))
                                {
                                    dataToSend = "│SentSpy│Confirm│" + MainProgram._PlayerUtils.GetSpyData(data.Split('│')[1], client);
                                }
                                else dataToSend = "│SentSpy│Deny│";

                                SendData(client, dataToSend);

                                continue;
                            }

                            else if (encryptedData.StartsWith(MainProgram._Encryption.EncryptString("Raid│")))
                            {
                                if (data.StartsWith("Raid│TryRaid│"))
                                {
                                    string tileID = data.Split('│')[2];
                                    string enemyPawnsData = data.Split('│')[3];

                                    if (MainProgram._PlayerUtils.CheckForConnectedPlayers(tileID) && MainProgram._PlayerUtils.CheckForPlayerShield(tileID) && MainProgram._PlayerUtils.CheckForPvpAvailability(tileID))
                                    {
                                        SendData(client, "SentRaid│Accept│" + connectedClients.Find(fetch => fetch.homeTileID == tileID).pawnCount);
                                        client.inRTSE = true;

                                        foreach (ServerClient target in connectedClients)
                                        {
                                            if (target.homeTileID == tileID)
                                            {
                                                SendData(target, "RaidStatus│Invaded│" + enemyPawnsData);
                                                client.inRtsActionWith = target;
                                                target.inRtsActionWith = client;
                                                target.inRTSE = true;
                                                break;
                                            }
                                        }
                                    }

                                    else SendData(client, "SentRaid│Deny│");
                                }

                                else if (data == "Raid│Ready│")
                                {
                                    SendData(client.inRtsActionWith, "RaidStatus│Start");
                                }

                                else if (data == "Raid│Ended│") ;

                                continue;
                            }

                            else if (encryptedData.StartsWith(MainProgram._Encryption.EncryptString("RTSBuffer│")))
                            {
                                SendData(client.inRtsActionWith, data);
                            }
                        }
                    }
                }

                catch
                {
                    disconnectedClients.Add(client);
                    return;
                }
            }
        }

        public void SendData(ServerClient client, string data)
        {
            try
            {
                NetworkStream s = client.tcp.GetStream();
                StreamWriter sw = new StreamWriter(s);

                sw.WriteLine(MainProgram._Encryption.EncryptString(data));
                sw.Flush();
            }

            catch { }
        }

        public void KickClients(ServerClient client, string kickMode)
        {
            try { client.tcp.Close(); }
            catch { }

            try { connectedClients.Remove(client); }
            catch { }

            if (kickMode == "Normal") MainProgram._ServerUtils.WriteServerLog("Player [" + client.username + "] Has Disconnected");
            else if (kickMode == "Silent") { }
            else { }

            MainProgram._ServerUtils.RefreshClientCount(null);

            MainProgram._ServerUtils.UpdateTitle();
        }

        public void CheckClientsConnection()
        {
            MainProgram._ServerUtils.WriteServerLog("Network Line Started");
            MainProgram._ServerUtils.WriteServerLog(Environment.NewLine);

            while (true)
            {
                Thread.Sleep(100);

                try
                {
                    if (disconnectedClients.Count > 0)
                    {
                        KickClients(disconnectedClients[0], "Normal");

                        disconnectedClients.Remove(disconnectedClients[0]);
                    }
                }
                catch { continue; }

                try
                {
                    foreach (ServerClient client in connectedClients)
                    {
                        if (!IsClientConnected(client)) client.disconnectFlag = true;
                    }
                }

                catch { continue; }
            }

            bool IsClientConnected(ServerClient client)
            {
                try
                {
                    TcpClient c = client.tcp;

                    if (c != null && c.Client != null && c.Client.Connected)
                    {
                        if (c.Client.Poll(0, SelectMode.SelectRead))
                        {
                            return !(c.Client.Receive(new byte[1], SocketFlags.Peek) == 0);
                        }
                    }

                    return true;
                }

                catch { return false; }
            }
        }
    }
}
