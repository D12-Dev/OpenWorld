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
    public class Networking
    {
        private TcpClient connection;
        private Stream s;
        private NetworkStream ns;
        private StreamWriter sw;
        private StreamReader sr;

        public string ip;
        public string port;
        public string username = "Offline User";
        public string password;

        public bool isTryingToConnect;
        public bool isConnectedToServer;

        public void TryConnectToServer()
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

                Main._Threading.GenerateThreads(1);
                ListenToServer();
            }

            catch
            {
                isTryingToConnect = false;
                Find.WindowStack.Add(new Dialog_MPTimeout());
            }
        }

        private void ListenToServer()
        {
            if (!Main._ParametersCache.isLoadingExistingGame) Main._Networking.SendData("Connect│" + username + "│" + password + "│" + Main._ParametersCache.versionCode + "│" + "NewGame" + "│" + Main._MPGame.GetCompactedModList());
            else Main._Networking.SendData("Connect│" + username + "│" + password + "│" + Main._ParametersCache.versionCode + "│" + "LoadGame" + "│" + Main._MPGame.GetCompactedModList());

            while (true)
            {
                if (!isConnectedToServer) return;

                Thread.Sleep(1);

                try
                {
                    if (ns.DataAvailable)
                    {
                        string data = sr.ReadLine();
                        data = Main._Encryption.DecryptString(data);

                        if (data != null)
                        {
                            if (data.StartsWith("MapDetails│"))
                            {
                                float globalCoverage = float.Parse(data.Split('│')[1]);
                                string seed = data.Split('│')[2];
                                int rainfall = int.Parse(data.Split('│')[3]);
                                int temperature = int.Parse(data.Split('│')[4]);
                                int population = int.Parse(data.Split('│')[5]);

                                List<string> settlements = new List<string>();
                                if (data.Split('│')[6].Count() > 0) settlements = data.Split('│')[6].Split('»').ToList();
                                Main._ParametersCache.onlineSettlements.Clear();
                                foreach (string str in settlements)
                                {
                                    Main._ParametersCache.onlineSettlements.Add(str.Split(':')[0], new List<string>() { str.Split(':')[1] });
                                }

                                int adminValue = int.Parse(data.Split('│')[7]);
                                if (adminValue == 1) Main._ParametersCache.isAdmin = true;
                                else Main._ParametersCache.isAdmin = false;

                                int wipeValue = int.Parse(data.Split('│')[8]);
                                if (wipeValue == 1) { }
                                else { }

                                Main._ParametersCache.roadMode = int.Parse(data.Split('│')[9]);

                                Main._ParametersCache.connectedPlayers = int.Parse(data.Split('│')[10]);

                                Main._ParametersCache.chatMode = int.Parse(data.Split('│')[11]);

                                Main._ParametersCache.modVerificationMode = int.Parse(data.Split('│')[12]);

                                Main._ParametersCache.modVerificationMode = int.Parse(data.Split('│')[13]);
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

                                Main._ParametersCache.connectedServerIdentifier = data.Split('│')[14];

                                Main._MPGame.CreateMultiplayerGame(globalCoverage, seed, rainfall, temperature, population);
                                continue;
                            }

                            else if (data.StartsWith("UpdateSettlements│"))
                            {
                                List<string> settlements = new List<string>();
                                if (data.Split('│')[1].Count() > 0) settlements = data.Split('│')[1].Split('»').ToList();

                                Main._ParametersCache.onlineSettlements.Clear();
                                foreach (string str in settlements)
                                {
                                    Main._ParametersCache.onlineSettlements.Add(str.Split(':')[0], new List<string>() { str.Split(':')[1] });
                                }

                                int adminValue = int.Parse(data.Split('│')[2]);
                                if (adminValue == 1) Main._ParametersCache.isAdmin = true;
                                else Main._ParametersCache.isAdmin = false;

                                int wipeValue = int.Parse(data.Split('│')[3]);
                                if (wipeValue == 1) { }
                                else { }

                                Main._ParametersCache.roadMode = int.Parse(data.Split('│')[4]);

                                Main._ParametersCache.connectedPlayers = int.Parse(data.Split('│')[5]);

                                Main._ParametersCache.chatMode = int.Parse(data.Split('│')[6]);

                                Main._ParametersCache.profanityMode = int.Parse(data.Split('│')[7]);

                                Main._ParametersCache.modVerificationMode = int.Parse(data.Split('│')[8]);
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

                                Main._ParametersCache.connectedServerIdentifier = data.Split('│')[9];

                                try
                                {
                                    if (data.Split('│')[10] != null)
                                    {
                                        string receivedGifts = data.Split('│')[10] + "│" + data.Split('│')[11];
                                        Main._ParametersCache.receiveGiftsData = receivedGifts;
                                    }
                                }
                                catch { }

                                Main._MPGame.LoadMultiplayerGame();
                                continue;
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

                                Main._Injections.thingsToDoInUpdate.Add(Main._MPGame.TryGenerateLetter);
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

                                    Main._MPCaravan.TakeFundsFromCaravan(); 
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
                                    Main._MPCaravan.ReturnTradesToCaravan();
                                    Main._ParametersCache.__MPTrade.Close();
                                    Find.WindowStack.Add(new Dialog_MPPlayerNotConnected());
                                    Main._ParametersCache.wantedSilver = "";
                                }

                                else if (data == "│SentTrade│Deal│")
                                {
                                    Main._MPCaravan.GiveFundsToCaravan();
                                    Main._ParametersCache.__MPWaiting.Close();
                                    Main._ParametersCache.tradedItemString = "";

                                    Main._ParametersCache.letterTitle = "Successful Trade";
                                    Main._ParametersCache.letterDescription = "You have traded with another settlement! \n\nTraded items have been deposited in your caravan.";
                                    Main._ParametersCache.letterType = LetterDefOf.PositiveEvent;
                                    Main._Injections.thingsToDoInUpdate.Add(Main._MPGame.TryGenerateLetter);

                                    Main._Injections.thingsToDoInUpdate.Add(Main._MPGame.ForceSave);
                                }

                                else if (data == "│SentTrade│Reject│")
                                {
                                    Main._MPCaravan.ReturnTradesToCaravan();
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
                                    Main._MPCaravan.ReturnTradesToCaravan();
                                    Main._ParametersCache.__MPBarter.Close();
                                    Find.WindowStack.Add(new Dialog_MPPlayerNotConnected());
                                }

                                else if (data == "│SentBarter│Deal│")
                                {
                                    Main._MPCaravan.ReceiveTradesFromPlayer(Main._ParametersCache.cachedItems);
                                    Main._ParametersCache.inTrade = false;
                                    Main._ParametersCache.cachedItems = null;
                                    Main._ParametersCache.tradedItemString = "";
                                    Main._ParametersCache.__MPWaiting.Close();

                                    Main._Injections.thingsToDoInUpdate.Add(Main._MPGame.ForceSave);
                                }

                                else if (data == "│SentBarter│Reject│")
                                {
                                    if (Main._ParametersCache.awaitingRebarter)
                                    {
                                        Main._MPCaravan.ReturnTradesToSettlement();
                                        Main._ParametersCache.__MPBarter.Close();
                                    }
                                    else Main._MPCaravan.ReturnTradesToCaravan();

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
                                    Main._Networking.SendData("TradeStatus│Reject│" + invoker);
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
                                    Main._Networking.SendData("BarterStatus│Reject│" + invoker);
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

                                Main._Injections.thingsToDoInUpdate.Add(Main._MPGame.TryGenerateLetter);
                            }

                            else if (data.StartsWith("│SentSpy│"))
                            {
                                if (data.StartsWith("│SentSpy│Confirm│"))
                                {
                                    Main._ParametersCache.silverAmount = 150;

                                    Main._MPCaravan.TakeFundsFromCaravan();

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

                                Main._Injections.thingsToDoInUpdate.Add(Main._MPGame.ExecuteEvent);

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

                            else if (data.StartsWith("RaidStatus│"))
                            {
                                if (data.StartsWith("RaidStatus│Invaded│"))
                                {
                                    Main._ParametersCache.rtsGenerationData = data;

                                    //Main._Injections.thingsToDoInUpdate.Add(Main._MPRTSE.TryPrepareHostForRaid);
                                }

                                else if (data == "RaidStatus│Start") Main._ParametersCache.__MPWaiting.Close();

                                else if (data == "RaidStatus│Finish") ;
                            }

                            else if (data.StartsWith("SentRaid│"))
                            {
                                if (data.StartsWith("SentRaid│Accept"))
                                {
                                    Main._ParametersCache.rtsGenerationData = data;

                                    //Main._Injections.thingsToDoInUpdate.Add(Main._MPRTSE.TryRaid);
                                    Main._ParametersCache.__MPWaiting.Close();
                                }
                                
                                else if (data == "SentRaid│Deny│")
                                {
                                    Main._ParametersCache.__MPWaiting.Close();
                                    Find.WindowStack.Add(new Dialog_MPPlayerNotConnected());
                                }
                            }

                            else if (data.StartsWith("RTSBuffer│"))
                            {
                                if (data.StartsWith("RTSBuffer│Move│"))
                                {
                                    data = data.Replace("RTSBuffer│Move│", "");

                                    Pawn pawn = null;
                                    IntVec3 cellToGo = new IntVec3(0, 0, 0);
                                    LocalTargetInfo target = null;
                                    Job job = null;

                                    if (data.Contains("»"))
                                    {
                                        string[] buffers = data.Split('»');

                                        foreach (string str in buffers)
                                        {
                                            string fixedData = str.Replace("Move│", "");

                                            pawn = Main._ParametersCache.enemyPawnData[int.Parse(fixedData.Split('│')[0])];
                                            cellToGo = new IntVec3(int.Parse(fixedData.Split('│')[1]), 0, int.Parse(fixedData.Split('│')[2]));
                                            target = new LocalTargetInfo(cellToGo);

                                            job = new Job(JobDefOf.GotoMindControlled, target);

                                            pawn.jobs.StopAll();
                                            pawn.jobs.StartJob(job);
                                        }
                                    }

                                    else
                                    {
                                        pawn = Main._ParametersCache.enemyPawnData[int.Parse(data.Split('│')[0])];
                                        cellToGo = new IntVec3(int.Parse(data.Split('│')[1]), 0, int.Parse(data.Split('│')[2]));
                                        target = new LocalTargetInfo(cellToGo);

                                        job = new Job(JobDefOf.GotoMindControlled, target);

                                        pawn.jobs.StopAll();
                                        pawn.jobs.StartJob(job);
                                    }
                                }

                                else if (data.StartsWith("RTSBuffer│Move&Attack│"))
                                {
                                    data = data.Replace("RTSBuffer│Move&Attack│", "");

                                    Pawn pawn = null;
                                    LocalTargetInfo enemy = null;
                                    Job job = null;

                                    if (data.Contains("»"))
                                    {
                                        string[] buffers = data.Split('»');

                                        foreach (string str in buffers)
                                        {
                                            string fixedData = str.Replace("Move&Attack│", "");

                                            pawn = Main._ParametersCache.enemyPawnData[int.Parse(fixedData.Split('│')[0])];
                                            enemy = new LocalTargetInfo(Main._ParametersCache.playerPawnData[int.Parse(fixedData.Split('│')[1])]);

                                            job = new Job(JobDefOf.AttackMelee, enemy);

                                            pawn.jobs.StopAll();
                                            pawn.jobs.StartJob(job);
                                        }
                                    }

                                    else
                                    {
                                        pawn = Main._ParametersCache.enemyPawnData[int.Parse(data.Split('│')[0])];
                                        enemy = new LocalTargetInfo(Main._ParametersCache.playerPawnData[int.Parse(data.Split('│')[1])]);

                                        job = new Job(JobDefOf.AttackMelee, enemy);

                                        pawn.jobs.StopAll();
                                        pawn.jobs.StartJob(job);
                                    }
                                }

                                else if (data.StartsWith("RTSBuffer│Take│"))
                                {
                                    data = data.Replace("RTSBuffer│Take│", "");

                                    Pawn pawn = null;
                                    LocalTargetInfo item = null;
                                    Job job = null;

                                    if (data.Contains("»"))
                                    {
                                        string[] buffers = data.Split('»');

                                        foreach (string str in buffers)
                                        {
                                            string fixedData = str.Replace("Take│", "");

                                            pawn = Main._ParametersCache.enemyPawnData[int.Parse(fixedData.Split('│')[0])];
                                            Thing thingToPickUp = new Thing();

                                            item = new LocalTargetInfo(thingToPickUp);

                                            job = new Job(JobDefOf.TakeCountToInventory, item);

                                            pawn.jobs.StopAll();
                                            pawn.jobs.StartJob(job);
                                        }
                                    }

                                    else
                                    {
                                        pawn = Main._ParametersCache.enemyPawnData[int.Parse(data.Split('│')[0])];
                                        Thing thingToPickUp = new Thing();

                                        item = new LocalTargetInfo(thingToPickUp);

                                        job = new Job(JobDefOf.AttackMelee, item);

                                        pawn.jobs.StopAll();
                                        pawn.jobs.StartJob(job);
                                    }
                                }
                            }
                        }
                    }
                }

                catch
                {
                    DisconnectFromServer();
                    return;
                }
            }
        }

        public void SendData(string data)
        {
            try
            {
                sw.WriteLine(Main._Encryption.EncryptString(data));
                sw.Flush();
            }

            catch { DisconnectFromServer(); }
        }

        public void CheckConnection()
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

        public void PvPChannel()
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

        public void DisconnectFromServer()
        {
            try { connection.Close(); }
            catch { }

            isConnectedToServer = false;
        }
    }
}