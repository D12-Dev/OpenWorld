using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using Verse;
using Verse.AI;

namespace OpenWorld
{
    public static class Networking
    {
        private static TcpClient connection;
        private static Stream s;
        private static NetworkStream ns;
        private static StreamWriter sw;
        private static StreamReader sr;

        public static string ip;
        public static string port;
        public static string username = "Offline User";
        public static string password;

        public static bool isTryingToConnect;
        public static bool isConnectedToServer;

        public static void TryConnectToServer()
        {
            isTryingToConnect = true;

            try
            {
                connection = new TcpClient(ip, int.Parse(port));
                s = connection.GetStream();
                sw = new StreamWriter(s);
                sr = new StreamReader(s);
                ns = connection.GetStream();

                isConnectedToServer = true;
                isTryingToConnect = false;

                Dialog_MPParameters.__instance.Close();

                //Threading.GenerateThreads(1);
                ListenToServer();
            }

            catch
            {
                isTryingToConnect = false;
                Find.WindowStack.Add(new Dialog_MPTimeout());
            }
        }

        private static void ListenToServer()
        {
            if (!Main._ParametersCache.isLoadingExistingGame) SendData("Connect│" + username + "│" + password + "│" + Main._ParametersCache.versionCode + "│" + "NewGame" + "│" + Main._MPGame.GetCompactedModList());
            else SendData("Connect│" + username + "│" + password + "│" + Main._ParametersCache.versionCode + "│" + "LoadGame" + "│" + Main._MPGame.GetCompactedModList());

            while (true)
            {
                if (!isConnectedToServer) return;

                Thread.Sleep(1);

                if (ns.DataAvailable)
                {
                    string data = sr.ReadLine();
                    data = Encryption.DecryptString(data);
                    Log.Message(data);

                    if (data != null)
                    {
                        if (data.StartsWith("Planet│"))
                        {
                            data = data.Remove(0, 7);

                            string[] planetDetails = data.Split('│');

                            Main._ParametersCache.globalCoverage = float.Parse(planetDetails[0]);
                            Main._ParametersCache.seed = planetDetails[1];
                            Main._ParametersCache.rainfall = int.Parse(planetDetails[2]);
                            Main._ParametersCache.temperature = int.Parse(planetDetails[3]);
                            Main._ParametersCache.population = int.Parse(planetDetails[4]);
                        }

                        else if (data.StartsWith("Settlements│"))
                        {
                            data = data.Remove(0, 12);

                            Main._ParametersCache.onlineSettlements.Clear();

                            string[] settlementsToLoad = data.Split('│');

                            foreach (string str in settlementsToLoad)
                            {
                                if (string.IsNullOrWhiteSpace(str)) continue;

                                int settlementTile = int.Parse(str.Split(':')[0]);
                                string settlementName = str.Split(':')[1];

                                List<string> settlementDetails = new List<string>()
                                {
                                    settlementName
                                };

                                Main._ParametersCache.onlineSettlements.Add(settlementTile, settlementDetails);
                            }
                        }

                        else if (data.StartsWith("Variables│"))
                        {
                            data = data.Remove(0, 10);

                            string[] variablesData = data.Split('│');

                            int adminValue = int.Parse(variablesData[0]);
                            if (adminValue == 1) Main._ParametersCache.isAdmin = true;
                            else Main._ParametersCache.isAdmin = false;

                            int wipeValue = int.Parse(variablesData[1]);
                            if (wipeValue == 1) { }
                            else { }

                            Main._ParametersCache.roadMode = int.Parse(variablesData[2]);

                            Main._ParametersCache.connectedPlayers = int.Parse(variablesData[3]);

                            Main._ParametersCache.chatMode = int.Parse(variablesData[4]);

                            Main._ParametersCache.profanityMode = int.Parse(variablesData[5]);

                            Main._ParametersCache.modVerificationMode = int.Parse(variablesData[6]);
                            if (Main._ParametersCache.modVerificationMode == 1)
                            {
                                List<bool> modsReady = LoadedModManager.RunningMods.Select((ModContentPack mod) => !mod.ModMetaData.OnSteamWorkshop).ToList();
                                if (modsReady.Count > 0)
                                {
                                    DisconnectFromServer();
                                    Find.WindowStack.Add(new Dialog_MPModifiedMods());
                                    return;
                                }
                            }

                            Main._ParametersCache.connectedServerIdentifier = variablesData[7];
                        }

                        else if (data == "NewGame│")
                        {
                            Main._MPGame.CreateMultiplayerGame();
                        }

                        else if (data == "LoadGame│")
                        {
                            Main._MPGame.LoadMultiplayerGame();
                        }

                        else if (data.StartsWith("ChatMessage│"))
                        {
                            Main._MPChat.ReceiveMessage(data);
                            continue;
                        }

                        else if (data.StartsWith("Disconnect│"))
                        {
                            if (data == "Disconnect│UserTaken")
                            {
                                Find.WindowStack.Add(new Dialog_MPWrongUserPassword());
                            }

                            else if (data == "Disconnect│WrongPassword")
                            {
                                Find.WindowStack.Add(new Dialog_MPWrongUserPassword());
                            }

                            else if (data.StartsWith("Disconnect│WrongMods│"))
                            {
                                string[] flaggedMods = data.Split('│')[2].Split('»');

                                Find.WindowStack.Add(new Dialog_MPWrongMods(flaggedMods));
                            }

                            else if (data == "Disconnect│Version")
                            {
                                Find.WindowStack.Add(new Dialog_MPWrongVersion());
                            }

                            else if (data == "Disconnect│Whitelist")
                            {
                                Find.WindowStack.Add(new Dialog_MPWhitelisted());
                            }

                            else if (data == "Disconnect│Corrupted")
                            {
                                Environment.Exit(0);
                            }

                            else if (data == "Disconnect│AnotherLogin")
                            {
                                Find.WindowStack.Add(new Dialog_MPDisconnected());
                            }

                            else if (data == "Disconnect│ServerFull")
                            {
                                Find.WindowStack.Add(new Dialog_MPServerFull());
                            }

                            else if (data == "Disconnect│Banned")
                            {
                                Find.WindowStack.Add(new Dialog_MPBanned());
                            }

                            else if (data == "Disconnect│Closing")
                            {
                                Find.WindowStack.Add(new Dialog_MPDisconnected());
                            }

                            DisconnectFromServer();
                            return;
                        }

                        else if (data.StartsWith("Notification│"))
                        {
                            Main._ParametersCache.letterTitle = "Server Notification";
                            Main._ParametersCache.letterDescription = data.Split('│')[1];
                            Main._ParametersCache.letterType = LetterDefOf.PositiveEvent;

                            Injections.thingsToDoInUpdate.Add(Main._MPGame.TryGenerateLetter);
                        }

                        else if (data == "│Promote│")
                        {
                            Main._ParametersCache.isAdmin = true;
                            Find.WindowStack.Add(new Dialog_MPPromote());
                            continue;
                        }

                        else if (data == "│Demote│")
                        {
                            Prefs.DevMode = false;
                            Main._ParametersCache.isAdmin = false;
                            Find.WindowStack.Add(new Dialog_MPDemote());
                            continue;
                        }

                        else if (data.StartsWith("AddSettlement│"))
                        {
                            Main._ParametersCache.addSettlementData = data;

                            Main._MPWorld.AddSettlementRealtime();

                            continue;
                        }

                        else if (data.StartsWith("RemoveSettlement│"))
                        {
                            Main._ParametersCache.removeSettlementData = data;

                            Main._MPWorld.RemoveSettlementRealtime();

                            continue;
                        }

                        else if (data.StartsWith("│SentEvent│"))
                        {
                            if (data == "│SentEvent│Confirm│")
                            {
                                Main._ParametersCache.silverAmount = 2500;

                                Main._ParametersCache.__MPBlackMarket.Close();

                                MPCaravan.TakeFundsFromCaravan();
                            }

                            else if (data == "│SentEvent│Deny│")
                            {
                                Main._ParametersCache.__MPBlackMarket.Close();
                                Find.WindowStack.Add(new Dialog_MPPlayerNotConnected());
                            }

                            continue;
                        }

                        else if (data.StartsWith("│SentTrade│"))
                        {
                            if (data == "│SentTrade│Confirm│")
                            {
                                Find.WindowStack.Add(new Dialog_MPWaiting());
                            }

                            else if (data == "│SentTrade│Deny│")
                            {
                                MPCaravan.ReturnTradesToCaravan();
                                Main._ParametersCache.__MPTrade.Close();
                                Find.WindowStack.Add(new Dialog_MPPlayerNotConnected());
                                Main._ParametersCache.wantedSilver = "";
                            }

                            else if (data == "│SentTrade│Deal│")
                            {
                                MPCaravan.GiveFundsToCaravan();
                                Main._ParametersCache.__MPWaiting.Close();
                                Main._ParametersCache.tradedItemString = "";

                                Main._ParametersCache.letterTitle = "Successful Trade";
                                Main._ParametersCache.letterDescription = "You have traded with another settlement! \n\nTraded items have been deposited in your caravan.";
                                Main._ParametersCache.letterType = LetterDefOf.PositiveEvent;
                                Injections.thingsToDoInUpdate.Add(Main._MPGame.TryGenerateLetter);

                                Injections.thingsToDoInUpdate.Add(Main._MPGame.ForceSave);
                            }

                            else if (data == "│SentTrade│Reject│")
                            {
                                MPCaravan.ReturnTradesToCaravan();
                                Main._ParametersCache.__MPWaiting.Close();
                                Main._ParametersCache.__MPTrade.Close();
                                Find.WindowStack.Add(new Dialog_MPRejectedTrade());
                                Main._ParametersCache.wantedSilver = "";
                            }

                            continue;
                        }

                        else if (data.StartsWith("│SentBarter│"))
                        {
                            if (data == "│SentBarter│Confirm│")
                            {
                                Find.WindowStack.Add(new Dialog_MPWaiting());
                            }

                            else if (data == "│SentBarter│Deny│")
                            {
                                MPCaravan.ReturnTradesToCaravan();
                                Main._ParametersCache.__MPBarter.Close();
                                Find.WindowStack.Add(new Dialog_MPPlayerNotConnected());
                            }

                            else if (data == "│SentBarter│Deal│")
                            {
                                MPCaravan.ReceiveTradesFromPlayer(Main._ParametersCache.cachedItems);
                                Main._ParametersCache.inTrade = false;
                                Main._ParametersCache.cachedItems = null;
                                Main._ParametersCache.tradedItemString = "";
                                Main._ParametersCache.__MPWaiting.Close();

                                Injections.thingsToDoInUpdate.Add(Main._MPGame.ForceSave);
                            }

                            else if (data == "│SentBarter│Reject│")
                            {
                                if (Main._ParametersCache.awaitingRebarter)
                                {
                                    MPCaravan.ReturnTradesToSettlement();
                                    Main._ParametersCache.__MPBarter.Close();
                                }
                                else MPCaravan.ReturnTradesToCaravan();

                                Main._ParametersCache.inTrade = false;
                                Main._ParametersCache.cachedItems = null;
                                Main._ParametersCache.tradedItemString = "";
                                Main._ParametersCache.__MPWaiting.Close();

                                Find.WindowStack.Add(new Dialog_MPRejectedTrade());
                            }

                            else if (data.StartsWith("│SentBarter│Rebarter│"))
                            {
                                string invoker = data.Split('│')[3];
                                string[] items = data.Split('│')[4].Split('»').ToArray();

                                Find.WindowStack.Add(new Dialog_MPBarterRequest(invoker, items, true));
                            }

                            continue;
                        }

                        else if (data.StartsWith("TradeRequest│"))
                        {
                            string invoker = data.Split('│')[1];
                            string splitedString = data.Split('│')[2];
                            string silverRequested = data.Split('│')[3];

                            if (Main._ParametersCache.inTrade)
                            {
                                SendData("TradeStatus│Reject│" + invoker);
                                continue;
                            }

                            string[] tradeableItems = new string[0];
                            if (splitedString.Contains('»')) tradeableItems = splitedString.Split('»').ToArray();
                            else tradeableItems = new string[1] { splitedString };

                            Find.WindowStack.Add(new Dialog_MPTradeRequest(invoker, tradeableItems, silverRequested));
                            continue;
                        }

                        else if (data.StartsWith("BarterRequest│"))
                        {
                            string invoker = data.Split('│')[1];
                            string splitedString = data.Split('│')[2];

                            if (Main._ParametersCache.inTrade)
                            {
                                Networking.SendData("BarterStatus│Reject│" + invoker);
                                continue;
                            }

                            string[] tradeableItems = new string[0];
                            if (splitedString.Contains('»')) tradeableItems = splitedString.Split('»').ToArray();
                            else tradeableItems = new string[1] { splitedString };

                            Find.WindowStack.Add(new Dialog_MPBarterRequest(invoker, tradeableItems, false));
                            continue;
                        }

                        else if (data.StartsWith("Spy│"))
                        {
                            if (Main._ParametersCache.spyWarnFlag) continue;

                            Main._ParametersCache.letterTitle = "You have been spied on!";
                            Main._ParametersCache.letterDescription = data.Split('│')[1] + "'s settlement has been spying you! \n\nAlthough their actions are unclear you should be careful about this.";
                            Main._ParametersCache.letterType = LetterDefOf.NegativeEvent;

                            Injections.thingsToDoInUpdate.Add(Main._MPGame.TryGenerateLetter);
                        }

                        else if (data.StartsWith("│SentSpy│"))
                        {
                            if (data.StartsWith("│SentSpy│Confirm│"))
                            {
                                Main._ParametersCache.silverAmount = 150;

                                MPCaravan.TakeFundsFromCaravan();

                                Find.WindowStack.Add(new Dialog_MPSpyResult(data.Split('│')[3]));
                            }

                            else if (data == "│SentSpy│Deny│")
                            {
                                Find.WindowStack.Add(new Dialog_MPPlayerNotConnected());
                            }

                            continue;
                        }

                        else if (data.StartsWith("ForcedEvent│"))
                        {
                            Main._ParametersCache.forcedEvent = data.Split('│')[1];

                            Injections.thingsToDoInUpdate.Add(Main._MPGame.ExecuteEvent);

                            continue;
                        }

                        else if (data.StartsWith("GiftedItems│"))
                        {
                            string splitedString = data.Split('│')[1];

                            if (Main._ParametersCache.inTrade) continue;

                            string[] tradeableItems = new string[0];
                            if (splitedString.Contains('»')) tradeableItems = splitedString.Split('»').ToArray();
                            else tradeableItems = new string[1] { splitedString };

                            Find.WindowStack.Add(new Dialog_MPGiftRequest(tradeableItems));
                            continue;
                        }

                        else if (data.StartsWith("│RenderTransportPod│"))
                        {
                            Main._MPWorld.RenderPodInWorld(data);
                            continue;
                        }

                        else if (data.StartsWith("│PlayerCountRefresh│"))
                        {
                            Main._ParametersCache.connectedPlayers = int.Parse(data.Split('│')[2]);
                            continue;
                        }
                    }
                }
            }
        }

        public static void SendData(string data)
        {
            try
            {
                sw.WriteLine(Encryption.EncryptString(data));
                sw.Flush();
            }

            catch { DisconnectFromServer(); }
        }

        public static void CheckConnection()
        {
            while (true)
            {
                if (!isConnectedToServer) break;

                Thread.Sleep(1000);

                try
                {
                    if (!IsConnected(connection))
                    {
                        Find.WindowStack.Add(new Dialog_MPDisconnected());
                        DisconnectFromServer();
                        break;
                    }
                }

                catch
                {
                    Find.WindowStack.Add(new Dialog_MPDisconnected());
                    DisconnectFromServer();
                    break;
                }
            }

            bool IsConnected(TcpClient connection)
            {
                try
                {
                    TcpClient c = connection;

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

        public static void PvPChannel()
        {
            while (true)
            {
                Thread.Sleep(1);

                if (string.IsNullOrWhiteSpace(Main._ParametersCache.rtsBuffer)) continue;
                else
                {
                    Main._ParametersCache.rtsBuffer = Main._ParametersCache.rtsBuffer.Remove(Main._ParametersCache.rtsBuffer.Count() - 1, 1);
                    SendData(Main._ParametersCache.rtsBuffer);
                    Main._ParametersCache.rtsBuffer = "";
                    continue;
                }
            }
        }

        public static void DisconnectFromServer()
        {
            try { connection.Close(); }
            catch { }

            isConnectedToServer = false;
        }
    }
}