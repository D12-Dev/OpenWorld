using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using Verse;

namespace OpenWorld
{
    public class ParametersCache
    {
        //GENERAL PURPOSE GOES HERE
        public string gameSavePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).Replace("Roaming", "LocalLow") + "Ludeon Studios/RimWorld by Ludeon Studios/Saves" + "\\";
        public bool pvpFlag;
        public bool offlinePvpFlag;
        public bool visitFlag;
        public bool secretFlag;
        public bool spyWarnFlag;
        public Dialog_MPDisconnected __MPDisconnected;

        //Dialog_MPParameters
        public string ipText;
        public string portText;
        public string usernameText;

        //Stuff
        public Page_CreateWorldParams __createWorldParams;
        public string onlineFileSaveName = "Open World Save";
        public string modIdentifier = "OpenWorld";
        public string connectedServerIdentifier = "";
        public bool isLoadingExistingGame = false;
        public bool isGeneratingNewOnlineGame = false;
        public bool isPlayingOnline = false;
        public bool isAdmin = false;
        public int roadMode = 0;
        public int chatMode = 0;
        public int modVerificationMode = 0;
        public int profanityMode = 0;
        public int connectedPlayers = 0;
        public Faction faction;
        public Dictionary<int, List<string>> onlineSettlements = new Dictionary<int, List<string>>();

        //Dialog_MPBlackMarket
        public Dialog_MPBlackMarket __MPBlackMarket;
        public string blackEventType;
        public int silverAmount;

        //Dialog_MPGift
        public List<Tradeable> listToShowInGiftMenu = new List<Tradeable>();
        public Dialog_MPGift __MPGift;
        public string giftedItemsString;

        //Dialog_MPMarket
        public Dialog_MPMarket __MPMarket;
        public Dictionary<int, List<string>> buyListings = new Dictionary<int, List<string>>();
        public Dictionary<int, List<string>> sellListings = new Dictionary<int, List<string>>();
        public Dictionary<int, List<string>> yourListings = new Dictionary<int, List<string>>();

        //Dialog_MPTrade
        public Dialog_MPTrade __MPTrade;
        public Dialog_MPWaiting __MPWaiting;
        public string tradedItemString;
        public string wantedSilver;
        public bool inTrade;

        //Dialog_MPBarter
        public Dialog_MPBarter __MPBarter;
        public Dialog_MPBarterRequest __MPBarterRequest;
        public string[] cachedItems;
        public bool awaitingRebarter;

        //Dialog_MPServerList
        public Dialog_MPServerList __MPServerList;

        //MPGame
        public string letterTitle;
        public string letterDescription;
        public LetterDef letterType;

        //Injections
        public Settlement focusedSettlement;
        public Caravan focusedCaravan;
        public SoundDef soundToUse;
        public string forcedEvent;
        public string addSettlementData;
        public string removeSettlementData;
        public string receiveGiftsData;
        public string serverStatusString;
        public string transferMode;
        public string versionCode = "Going 1.4";

        //RTS
        public bool isInRTS;
        public string rtsBuffer = "";
        public string rtsGenerationData = "";
        public List<Pawn> playerPawnData = new List<Pawn>();
        public List<Pawn> enemyPawnData = new List<Pawn>();
    }
}
