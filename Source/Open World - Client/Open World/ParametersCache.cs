using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.IO;
using Verse;

namespace OpenWorld
{
    public class ParametersCache
    {
        //Planet Parameters
        public float globalCoverage;
        public string seed;
        public int rainfall;
        public int temperature;
        public int population;

        //Server Parameters
        public int roadMode = 0;
        public int chatMode = 0;
        public int modVerificationMode = 0;
        public int profanityMode = 0;
        public int enforcedDifficultyMode = 0;

        //Player Flags
        public bool pvpFlag;
        public bool offlinePvpFlag;
        public bool visitFlag;
        public bool secretFlag;
        public bool spyWarnFlag;
        public bool hasLoadedCorrectly;
        public bool isLoadingExistingGame;
        public bool isGeneratingNewOnlineGame;
        public bool isPlayingOnline;

        //Planet Factions
        public List<Faction> allFactions = new List<Faction>();
        public Faction onlineNeutralFaction;
        public Faction onlineAllyFaction;
        public Faction onlineEnemyFaction;

        public Dictionary<int, List<int>> allFactionStructures = new Dictionary<int, List<int>>();

        //Faction Silo
        public Dictionary<int, List<string>> siloContents = new Dictionary<int, List<string>>();
        public bool canWithdrawSilo;

        //Online Player Faction
        public bool hasFaction;
        public string factionName;
        public Dictionary<string, int> factionMembers = new Dictionary<string, int>();

        //Online Settlements Lists
        public Dictionary<int, List<string>> allSettlements = new Dictionary<int, List<string>>();
        public Dictionary<int, List<string>> onlineNeutralSettlements = new Dictionary<int, List<string>>();
        public Dictionary<int, List<string>> onlineAllySettlements = new Dictionary<int, List<string>>();
        public Dictionary<int, List<string>> onlineEnemySettlements = new Dictionary<int, List<string>>();

        //Online Players
        public List<string> playerList = new List<string>();
        public int playerCount = 0;

        //Item Transfer Caches
        public Dialog_MPGift __MPGift;
        public Dialog_MPTrade __MPTrade;
        public Dialog_MPBarter __MPBarter;
        public Dialog_MPFactionSiloDeposit __MPSiloDeposit;

        public string giftedItemsString = "";
        public string tradedItemString = "";
        public string barteredItemString = "";
        public string depositItemsString = "";

        public List<Tradeable> listToShowInGiftMenu = new List<Tradeable>();
        public List<Tradeable> listToShowInTradeMenu = new List<Tradeable>();
        public List<Tradeable> listToShowInBarterMenu = new List<Tradeable>();
        public List<Tradeable> listToShowInSiloMenu = new List<Tradeable>();

        //Trade References
        public Dialog_MPWaiting __MPWaiting;
        public string wantedSilver;
        public bool inTrade;

        //Barter References
        public Dialog_MPBarterRequest __MPBarterRequest;
        public List<string> cachedItems;
        public bool awaitingRebarter;

        //Black Market References
        public Dialog_MPBlackMarket __MPBlackMarket;
        public string blackEventType;

        //Production Site References
        public int productionSiteProduct = 0;

        //General Purpose
        public string gameSavePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).Replace("Roaming", "LocalLow") + Path.DirectorySeparatorChar + "Ludeon Studios" + Path.DirectorySeparatorChar + "RimWorld by Ludeon Studios" + Path.DirectorySeparatorChar + "Saves";

        //Dialog_MPParameters
        public string ipText;
        public string portText;
        public string usernameText;

        //Stuff
        public Page_CreateWorldParams __createWorldParams;
        public string onlineFileSaveName = "Open World Save";
        public string modIdentifier = "OpenWorld";
        public string connectedServerIdentifier = "";
        public bool isAdmin = false;

        //Letters
        public string letterTitle;
        public string letterDescription;
        public LetterDef letterType;

        //Injections
        public Settlement focusedSettlement;
        public Caravan focusedCaravan;
        public int focusedTile;
        public SoundDef soundToUse;
        public string forcedEvent;
        public string receiveGiftsData;
        public string serverStatusString;
        public int transferMode;
        public string versionCode = "Faction Warfare";
    }
}
