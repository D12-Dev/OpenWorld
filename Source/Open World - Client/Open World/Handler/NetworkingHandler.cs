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
                    Find.WindowStack.Add(new OW_ErrorDialog("Local mods are not allowed"));
                    return;
                }
            }

            Main._ParametersCache.enforcedDifficultyMode = int.Parse(variablesData[6]);

            Main._ParametersCache.connectedServerIdentifier = variablesData[7];
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

            Injections.thingsToDoInUpdate.Add(RimworldHandler.GenerateLetter);
        }

        public static void AdminHandle(string data)
        {
            if (data == "Admin│Promote")
            {
                Main._ParametersCache.isAdmin = true;
                Find.WindowStack.Add(new OW_InfoDialog("You have been granted administrator privileges"));
            }

            else if (data == "Admin│Demote")
            {
                Prefs.DevMode = false;
                Main._ParametersCache.isAdmin = false;
                Find.WindowStack.Add(new OW_InfoDialog("You have lost administrator privileges"));
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
                Find.WindowStack.Add(new OW_ErrorDialog("Player unavailable for this action"));
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
                TradeHandler.ReturnTradesToCaravan();
                Main._ParametersCache.__MPTrade.Close();
                Find.WindowStack.Add(new OW_ErrorDialog("Player unavailable for this action"));
            }

            else if (data == "│SentTrade│Deal│")
            {
                TradeHandler.GiveTradeFundsToCaravan();
                Main._ParametersCache.__MPWaiting.Close();

                Main._ParametersCache.letterTitle = "Successful Trade";
                Main._ParametersCache.letterDescription = "You have traded with another settlement! \n\nTraded items have been deposited in your caravan.";
                Main._ParametersCache.letterType = LetterDefOf.PositiveEvent;
                Injections.thingsToDoInUpdate.Add(RimworldHandler.GenerateLetter);
            }

            else if (data == "│SentTrade│Reject│")
            {
                TradeHandler.ReturnTradesToCaravan();
                Main._ParametersCache.__MPWaiting.Close();
                Main._ParametersCache.__MPTrade.Close();
                Find.WindowStack.Add(new OW_ErrorDialog("The requested player rejected the trade"));
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
                BarterHandler.ReturnBarterToCaravan();
                Main._ParametersCache.__MPBarter.Close();
                Find.WindowStack.Add(new OW_ErrorDialog("Player unavailable for this action"));
            }

            else if (data == "│SentBarter│Deal│")
            {
                BarterHandler.ReceiveBarterToSettlement(Main._ParametersCache.cachedItems);
                Main._ParametersCache.__MPWaiting.Close();
            }

            else if (data == "│SentBarter│Reject│")
            {
                if (Main._ParametersCache.awaitingRebarter)
                {
                    BarterHandler.ReturnBarterToSettlement();
                    Main._ParametersCache.__MPBarter.Close();
                }
                else BarterHandler.ReturnBarterToCaravan();

                Main._ParametersCache.__MPWaiting.Close();

                Find.WindowStack.Add(new OW_ErrorDialog("The requested player rejected the trade"));
            }

            else if (data.StartsWith("│SentBarter│Rebarter│"))
            {
                string invoker = data.Split('│')[3];
                string[] items = data.Split('│')[4].Split('»').ToArray();

                List<string> tradeableItems = new List<string>();

                foreach (string str in items)
                {
                    if (string.IsNullOrWhiteSpace(str)) continue;
                    else tradeableItems.Add(str);
                }

                Find.WindowStack.Add(new Dialog_MPBarterRequest(invoker, tradeableItems, true));
            }
        }

        public static void TradeRequestHandle(string data)
        {
            string invoker = data.Split('│')[1];
            string splitedString = data.Split('│')[2];
            string silverRequested = data.Split('│')[3];

            if (!Main._ParametersCache.hasLoadedCorrectly)
            {
                Networking.SendData("TradeStatus│Reject│" + invoker);
                return;
            }

            if (Main._ParametersCache.inTrade)
            {
                Networking.SendData("TradeStatus│Reject│" + invoker);
                return;
            }

            string[] tradeableItems = new string[0];
            if (splitedString.Contains('»')) tradeableItems = splitedString.Split('»').ToArray();
            else tradeableItems = new string[1] { splitedString };

            Find.WindowStack.Add(new Dialog_MPTradeRequest(invoker, tradeableItems, silverRequested));
        }

        public static void BarterRequestHandle(string data)
        {
            string invoker = data.Split('│')[1];
            string[] splitedString = data.Split('│')[2].Split('»');

            if (!Main._ParametersCache.hasLoadedCorrectly)
            {
                Networking.SendData("BarterStatus│Reject│" + invoker);
                return;
            }

            if (Main._ParametersCache.inTrade)
            {
                Networking.SendData("BarterStatus│Reject│" + invoker);
                return;
            }

            List<string> tradeableItems = new List<string>();

            foreach (string str in splitedString)
            {
                if (string.IsNullOrWhiteSpace(str)) continue;
                else tradeableItems.Add(str);
            }

            Find.WindowStack.Add(new Dialog_MPBarterRequest(invoker, tradeableItems, false));
        }

        public static void SpiedHandle(string data)
        {
            if (Main._ParametersCache.spyWarnFlag) return;

            Main._ParametersCache.letterTitle = "You have been spied on!";
            Main._ParametersCache.letterDescription = data.Split('│')[1] + "'s settlement has been spying you! \n\nAlthough their actions are unclear you should be careful about this.";
            Main._ParametersCache.letterType = LetterDefOf.NegativeEvent;

            Injections.thingsToDoInUpdate.Add(RimworldHandler.GenerateLetter);
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
                Find.WindowStack.Add(new OW_ErrorDialog("Player unavailable for this action"));
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

            Main._ParametersCache.receiveGiftsData = splitedString;

            Main._MPGame.CheckForGifts();
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
                Find.WindowStack.Add(new OW_InfoDialog("Your faction has been created"));
            }

            else if (data == "FactionManagement│NameInUse")
            {
                Find.WindowStack.Add(new OW_ErrorDialog("Faction name already in use"));
            }

            else if (data == "FactionManagement│AlreadyInFaction")
            {
                Find.WindowStack.Add(new Dialog_MPFactionAlreadyInFaction());
            }

            else if (data == "FactionManagement│NotInFaction")
            {
                Find.WindowStack.Add(new OW_ErrorDialog("That player is not in your faction"));
            }

            else if (data == "FactionManagement│NoPowers")
            {
                Find.WindowStack.Add(new OW_ErrorDialog("You do not have enough faction power"));
            }

            else if (data.StartsWith("FactionManagement│Invite"))
            {
                string factionName = data.Split('│')[2];

                Find.WindowStack.Add(new Dialog_MPFactionInvite(factionName));
            }

            else if (data.StartsWith("FactionManagement│Silo"))
            {
                if (data.StartsWith("FactionManagement│Silo│Contents"))
                {
                    string itemsUnbuilt = data.Split('│')[3];

                    Main._ParametersCache.siloContents.Clear();

                    if (string.IsNullOrWhiteSpace(itemsUnbuilt)) return;

                    string[] itemsBuilt;
                    if (itemsUnbuilt.Contains('»')) itemsBuilt = itemsUnbuilt.Split('»');
                    else itemsBuilt = new string[] { itemsUnbuilt };

                    foreach(string str in itemsBuilt)
                    {
                        if (string.IsNullOrWhiteSpace(str)) continue;

                        string itemID = str.Split(':')[0];
                        string itemQuantity = str.Split(':')[1];
                        string itemQuality = str.Split(':')[2];
                        string itemMaterial = str.Split(':')[3];

                        List<string> itemDetails = new List<string>();
                        itemDetails.Add(itemID);
                        itemDetails.Add(itemQuantity);
                        itemDetails.Add(itemQuality);
                        itemDetails.Add(itemMaterial);

                        Main._ParametersCache.siloContents.Add(Main._ParametersCache.siloContents.Count, itemDetails);
                    }

                    Main._ParametersCache.canWithdrawSilo = true;
                }

                else if (data.StartsWith("FactionManagement│Silo│Withdraw"))
                {
                    string itemUnbuilt = data.Split('│')[3];

                    Main._ParametersCache.siloContents.Clear();

                    if (string.IsNullOrWhiteSpace(itemUnbuilt)) return;

                    string itemID = itemUnbuilt.Split(':')[0];
                    int itemQuantity = int.Parse(itemUnbuilt.Split(':')[1]);
                    int itemQuality = int.Parse(itemUnbuilt.Split(':')[2]);
                    string itemMaterial = itemUnbuilt.Split(':')[3];

                    SiloHandler.ReceiveMaterialFromSilo(itemID, itemQuantity, itemQuality, itemMaterial);
                }
            }

            else if (data.StartsWith("FactionManagement│ProductionSite"))
            {
                if (data.StartsWith("FactionManagement│ProductionSite│Tick"))
                {
                    ProductionSiteHandler.GetProductsToReceive(data);
                }
            }

            else if (data.StartsWith("FactionManagement│Bank"))
            {
                if (data.StartsWith("FactionManagement│Bank│Refresh"))
                {
                    int quantity = int.Parse(data.Split('│')[3]);

                    Main._ParametersCache.bankSilver = quantity;
                }

                else if (data.StartsWith("FactionManagement│Bank│Withdraw"))
                {
                    int quantity = int.Parse(data.Split('│')[3]);

                    if (quantity == 0) return;
                    else
                    {
                        Find.WindowStack.Add(new OW_InfoDialog("You have received the funds"));
                        MPCaravan.GiveFundsToCaravan(quantity);
                    }
                }
            }
        }

        public static void PlayerNotConnectedHandle(string data)
        {
            Find.WindowStack.Add(new OW_ErrorDialog("Player unavailable for this action"));
        }

        public static void DisconnectHandle(string data)
        {
            if (data == "Disconnect│UserTaken")
            {
                Find.WindowStack.Add(new OW_ErrorDialog("Username already in use or incorrect password"));
            }

            else if (data == "Disconnect│WrongPassword")
            {
                Find.WindowStack.Add(new OW_ErrorDialog("Username already in use or incorrect password"));
            }

            else if (data.StartsWith("Disconnect│WrongMods│"))
            {
                string[] flaggedMods = data.Split('│')[2].Split('»');

                Find.WindowStack.Add(new Dialog_MPWrongMods(flaggedMods));
            }

            else if (data == "Disconnect│Version")
            {
                Find.WindowStack.Add(new OW_ErrorDialog("Running an outdated or invalid version"));
            }

            else if (data == "Disconnect│Whitelist")
            {
                Find.WindowStack.Add(new OW_ErrorDialog("You are not whitelisted to the server"));
            }

            else if (data == "Disconnect│AnotherLogin")
            {
                Find.WindowStack.Add(new OW_ErrorDialog("You have connected elsewhere"));
            }

            else if (data == "Disconnect│ServerFull")
            {
                Find.WindowStack.Add(new OW_ErrorDialog("Server is full"));
            }

            else if (data == "Disconnect│Banned")
            {
                Find.WindowStack.Add(new OW_ErrorDialog("You are banned from the server"));
            }

            else if (data == "Disconnect│Closing")
            {
                Find.WindowStack.Add(new OW_ErrorDialog("Server is closing"));
            }

            else if (data == "Disconnect│Corrupted") Environment.Exit(0);

            Networking.DisconnectFromServer();
            return;
        }
    }
}