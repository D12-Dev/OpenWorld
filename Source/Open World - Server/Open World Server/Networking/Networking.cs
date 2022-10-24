using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace OpenWorldServer
{
    public static class Networking
    {
        private static TcpListener server;
        public static IPAddress localAddress;
        public static int serverPort = 0;

        public static List<ServerClient> connectedClients = new List<ServerClient>();
        public static List<ServerClient> disconnectedClients = new List<ServerClient>();

        public static void ReadyServer()
        {
            server = new TcpListener(localAddress, serverPort);
            server.Start();

            ConsoleUtils.UpdateTitle();

            Threading.GenerateThreads(1);

            ListenForIncomingUsers();
        }

        private static void ListenForIncomingUsers()
        {
            server.BeginAcceptTcpClient(AcceptClients, server);
        }

        private static void AcceptClients(IAsyncResult ar)
        {
            TcpListener listener = (TcpListener)ar.AsyncState;

            ServerClient newServerClient = new ServerClient(listener.EndAcceptTcpClient(ar));

            connectedClients.Add(newServerClient);

            Threading.GenerateClientThread(newServerClient);

            ListenForIncomingUsers();
        }

        public static void ListenToClient(ServerClient client)
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
                        string data = Encryption.DecryptString(encryptedData);
                        Debug.WriteLine(data);
                        
                        if (encryptedData != null)
                        {
                            if (encryptedData.StartsWith(Encryption.EncryptString("Connect│")))
                            {
                                NetworkingHandler.ConnectHandle(client, data);
                            }

                            else if (encryptedData.StartsWith(Encryption.EncryptString("ChatMessage│")))
                            {
                                NetworkingHandler.ChatMessageHandle(client, data);
                            }

                            else if (data.StartsWith("UserSettlement│"))
                            {
                                NetworkingHandler.UserSettlementHandle(client, data);
                            }

                            else if (encryptedData.StartsWith(Encryption.EncryptString("ForceEvent│")))
                            {
                                NetworkingHandler.ForceEventHandle(client, data);
                            }

                            else if (encryptedData.StartsWith(Encryption.EncryptString("SendGiftTo│")))
                            {
                                NetworkingHandler.SendGiftHandle(client, data);
                            }

                            else if (encryptedData.StartsWith(Encryption.EncryptString("SendTradeTo│")))
                            {
                                NetworkingHandler.SendTradeHandle(client, data);
                            }

                            else if (encryptedData.StartsWith(Encryption.EncryptString("SendBarterTo│")))
                            {
                                NetworkingHandler.SendBarterHandle(client, data);
                            }

                            else if (encryptedData.StartsWith(Encryption.EncryptString("TradeStatus│")))
                            {
                                NetworkingHandler.TradeStatusHandle(client, data);
                            }

                            else if (encryptedData.StartsWith(Encryption.EncryptString("BarterStatus│")))
                            {
                                NetworkingHandler.BarterStatusHandle(client, data);
                            }

                            else if (encryptedData.StartsWith(Encryption.EncryptString("GetSpyInfo│")))
                            {
                                NetworkingHandler.SpyInfoHandle(client, data);
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

        public static void SendData(ServerClient client, string data)
        {
            try
            {
                NetworkStream s = client.tcp.GetStream();
                StreamWriter sw = new StreamWriter(s);

                sw.WriteLine(Encryption.EncryptString(data));
                sw.Flush();
            }

            catch { }
        }

        public static void KickClients(ServerClient client, string kickMode)
        {
            try { client.tcp.Close(); }
            catch { }

            try { connectedClients.Remove(client); }
            catch { }

            if (kickMode == "Normal") ConsoleUtils.LogToConsole("Player [" + client.username + "] Has Disconnected");
            else if (kickMode == "Silent") { }
            else { }

            ServerUtils.RefreshClientCount(null);

            ConsoleUtils.UpdateTitle();
        }

        public static void CheckClientsConnection()
        {
            ConsoleUtils.DisplayNetworkStatus();

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
