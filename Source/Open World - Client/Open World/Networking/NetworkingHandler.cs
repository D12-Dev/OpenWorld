using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace OpenWorld
{
    public static class NetworkingHandler
    {
        public static void PlanetHandle(string data)
        {
            data = data.Remove(0, 7);

            string[] planetDetails = data.Split('│');

            Main._ParametersCache.globalCoverage = float.Parse(planetDetails[0]);
            Main._ParametersCache.seed = planetDetails[1];
            Main._ParametersCache.rainfall = int.Parse(planetDetails[2]);
            Main._ParametersCache.temperature = int.Parse(planetDetails[3]);
            Main._ParametersCache.population = int.Parse(planetDetails[4]);
        }

        public static void SettlementsHandle(string data)
        {
            SettlementHandler.GetSettlementsFromServer(data);
        }

        public static void FactionStructuresHandle(string data)
        {
            FactionHandler.GetFactionStructuresFromServer(data);
        }

        public static void VariablesHandle(string data)
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

            Main._ParametersCache.chatMode = int.Parse(variablesData[3]);

            Main._ParametersCache.profanityMode = int.Parse(variablesData[4]);

            Main._ParametersCache.modVerificationMode = int.Parse(variablesData[5]);
            if (Main._ParametersCache.modVerificationMode == 1)
            {
                List<bool> modsReady = LoadedModManager.RunningMods.Select((ModContentPack mod) => !mod.ModMetaData.OnSteamWorkshop).ToList();
                if (modsReady.Count > 0)
                {
                    Networking.DisconnectFromServer();
                    Find.WindowStack.Add(new Dialog_MPModifiedMods());
                    return;
                }
            }

            Main._ParametersCache.connectedServerIdentifier = variablesData[6];
        }

        public static void NewGameHandle(string data)
        {
            Main._MPGame.CreateMultiplayerGame();
        }

        public static void LoadGameHandle(string data)
        {
            Main._MPGame.LoadMultiplayerGame();
        }

        public static void ChatMessageHandle(string data)
        {
            Main._MPChat.ReceiveMessage(data);
        }

        public static void NotificationHandle(string data)
        {
            Main._ParametersCache.letterTitle = "Server Notification";
            Main._ParametersCache.letterDescription = data.Split('│')[1];
            Main._ParametersCache.letterType = LetterDefOf.PositiveEvent;

            Injections.thingsToDoInUpdate.Add(Main._MPGame.TryGenerateLetter);
        }

        public static void AdminHandle(string data)
        {
            if (data == "Admin│Promote")
            {
                Main._ParametersCache.isAdmin = true;
                Find.WindowStack.Add(new Dialog_MPPromote());
            }

            else if (data == "Admin│Demote")
            {
                Prefs.DevMode = false;
                Main._ParametersCache.isAdmin = false;
                Find.WindowStack.Add(new Dialog_MPDemote());
            }
        }

        public static void SettlementBuilderHandle(string data)
        {
            if (data.StartsWith("SettlementBuilder│AddSettlement"))
            {
                SettlementHandler.AddSettlementInWorld(data);
            }

            else if (data.StartsWith("SettlementBuilder│RemoveSettlement"))
            {
                SettlementHandler.RemoveSettlementInWorld(data);
            }
        }

        public static void FactionStructureBuilderHandler(string data)
        {
            if (data.StartsWith("FactionStructureBuilder│AddStructure"))
            {
                FactionHandler.AddFactionStructureInWorld(data);
            }

            else if (data.StartsWith("FactionStructureBuilder│RemoveStructure"))
            {
                FactionHandler.RemoveFactionStructureInWorld(data);
            }
        }

        public static void SentEventHandle(string data)
        {
            if (data == "│SentEvent│Confirm│")
            {
                Main._ParametersCache.__MPBlackMarket.Close();

                MPCaravan.TakeFundsFromCaravan(2500);
            }

            else if (data == "│SentEvent│Deny│")
            {
                Main._ParametersCache.__MPBlackMarket.Close();
                Find.WindowStack.Add(new Dialog_MPPlayerNotConnected());
            }
        }

        public static void SentTradeHandle(string data)
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
        }

        public static void SentBarterHandle(string data)
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
        }

        public static void TradeRequestHandle(string data)
        {
            string invoker = data.Split('│')[1];
            string splitedString = data.Split('│')[2];
            string silverRequested = data.Split('│')[3];

            if (Main._ParametersCache.inTrade)
            {
                Networking.SendData("TradeStatus│Reject│" + invoker);
            }

            string[] tradeableItems = new string[0];
            if (splitedString.Contains('»')) tradeableItems = splitedString.Split('»').ToArray();
            else tradeableItems = new string[1] { splitedString };

            Find.WindowStack.Add(new Dialog_MPTradeRequest(invoker, tradeableItems, silverRequested));
        }

        public static void BarterRequestHandle(string data)
        {
            string invoker = data.Split('│')[1];
            string splitedString = data.Split('│')[2];

            if (Main._ParametersCache.inTrade)
            {
                Networking.SendData("BarterStatus│Reject│" + invoker);
            }

            string[] tradeableItems = new string[0];
            if (splitedString.Contains('»')) tradeableItems = splitedString.Split('»').ToArray();
            else tradeableItems = new string[1] { splitedString };

            Find.WindowStack.Add(new Dialog_MPBarterRequest(invoker, tradeableItems, false));
        }

        public static void SpiedHandle(string data)
        {
            if (Main._ParametersCache.spyWarnFlag) return;

            Main._ParametersCache.letterTitle = "You have been spied on!";
            Main._ParametersCache.letterDescription = data.Split('│')[1] + "'s settlement has been spying you! \n\nAlthough their actions are unclear you should be careful about this.";
            Main._ParametersCache.letterType = LetterDefOf.NegativeEvent;

            Injections.thingsToDoInUpdate.Add(Main._MPGame.TryGenerateLetter);
        }

        public static void SentSpyHandle(string data)
        {
            if (data.StartsWith("│SentSpy│Confirm│"))
            {
                MPCaravan.TakeFundsFromCaravan(150);

                Find.WindowStack.Add(new Dialog_MPSpyResult(data.Split('│')[3]));
            }

            else if (data == "│SentSpy│Deny│")
            {
                Find.WindowStack.Add(new Dialog_MPPlayerNotConnected());
            }
        }

        public static void ForcedEventHandle(string data)
        {
            Main._ParametersCache.forcedEvent = data.Split('│')[1];

            Injections.thingsToDoInUpdate.Add(Main._MPGame.ExecuteEvent);
        }

        public static void GiftedHandle(string data)
        {
            string splitedString = data.Split('│')[1];

            if (string.IsNullOrWhiteSpace(splitedString)) return;
            if (Main._ParametersCache.inTrade) return;

            string[] tradeableItems = new string[0];
            if (splitedString.Contains('»')) tradeableItems = splitedString.Split('»').ToArray();
            else tradeableItems = new string[1] { splitedString };

            Find.WindowStack.Add(new Dialog_MPGiftRequest(tradeableItems));
        }

        public static void PlayerListHandle(string data)
        {
            Main._ParametersCache.playerList.Clear();
            Main._ParametersCache.playerCount = 0;

            List<string> playerList = data.Split('│')[1].Split(':').ToList();
            foreach(string str in playerList)
            {
                if (string.IsNullOrWhiteSpace(str)) continue;
                else Main._ParametersCache.playerList.Add(str);
            }

            Main._ParametersCache.playerCount = int.Parse(data.Split('│')[2]);
        }

        public static void FactionManagementHandle(string data)
        {
            if (data.StartsWith("FactionManagement│Details│"))
            {
                FactionHandler.FactionDetailsHandle(data);
            }

            else if (data == "FactionManagement│Created")
            {
                Find.WindowStack.Add(new Dialog_MPFactionCreated());
            }

            else if (data == "FactionManagement│NameInUse")
            {
                Find.WindowStack.Add(new Dialog_MPFactionNameInUse());
            }

            else if (data == "FactionManagement│AlreadyInFaction")
            {
                Find.WindowStack.Add(new Dialog_MPFactionAlreadyInFaction());
            }

            else if (data == "FactionManagement│NotInFaction")
            {
                Find.WindowStack.Add(new Dialog_MPFactionNotInFaction());
            }

            else if (data == "FactionManagement│NoPowers")
            {
                Find.WindowStack.Add(new Dialog_MPFactionNoPower());
            }

            else if (data.StartsWith("FactionManagement│Invite"))
            {
                string factionName = data.Split('│')[2];

                Find.WindowStack.Add(new Dialog_MPFactionInvite(factionName));
            }
        }

        public static void PlayerNotConnectedHandle(string data)
        {
            Find.WindowStack.Add(new Dialog_MPPlayerNotConnected());
        }

        public static void DisconnectHandle(string data)
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

            Networking.DisconnectFromServer();
            return;
        }
    }
}