using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Profile;
using Verse.Sound;

namespace OpenWorld
{
    public class MPGame
    {
		public void CreateMultiplayerGame()
		{
			Find.GameInitData.permadeathChosen = true;
			Find.GameInitData.permadeath = true;

			Main._ParametersCache.isPlayingOnline = true;
			Main._ParametersCache.isGeneratingNewOnlineGame = false;

			Main._MPGame.DisableDevOptions();

			OverallRainfall overalRainfall = (OverallRainfall)Main._ParametersCache.rainfall;
			OverallTemperature overallTemperature = (OverallTemperature)Main._ParametersCache.temperature;
			OverallPopulation overallPopulation = (OverallPopulation)Main._ParametersCache.population;

			LongEventHandler.QueueLongEvent(delegate
			{
				Find.GameInitData.ResetWorldRelatedMapInitData();

				Current.Game.World = WorldGenerator.GenerateWorld(Main._ParametersCache.globalCoverage, 
					Main._ParametersCache.seed, overalRainfall, overallTemperature, overallPopulation, null);

				LongEventHandler.ExecuteWhenFinished(delegate
				{
					if (Main._ParametersCache.__createWorldParams.next != null)
					{
						Find.WindowStack.Add(Main._ParametersCache.__createWorldParams.next);
					}

					MemoryUtility.UnloadUnusedUnityAssets();
					Find.World.renderer.RegenerateAllLayersNow();
					Main._ParametersCache.__createWorldParams.Close();
				});
			}, "GeneratingWorld", doAsynchronously: true, null);

			return;
		}

		public void LoadMultiplayerGame()
		{
			string saveName = Main._ParametersCache.onlineFileSaveName + " - " + Main._ParametersCache.connectedServerIdentifier + " - " + Main._ParametersCache.usernameText;

			Main._ParametersCache.isPlayingOnline = true;

			Main._MPGame.DisableDevOptions();

			LongEventHandler.QueueLongEvent(delegate
			{
				MemoryUtility.ClearAllMapsAndWorld();
				Current.Game = new Game();
				Current.Game.InitData = new GameInitData();
				Current.Game.InitData.gameToLoad = saveName;
				Current.Game.InitData.permadeathChosen = true;
				Current.Game.InitData.permadeath = true;
			}, "Play", "LoadingLongEvent", doAsynchronously: true, null);
		}

		public string GetCompactedModList()
		{
			List<string> loadedMods = LoadedModManager.RunningMods.Select((ModContentPack mod) => mod.Name).ToList();
			string compactedModList = "";

			foreach (string mod in loadedMods)
			{
				compactedModList = compactedModList + mod + "»";
			}

			compactedModList = compactedModList.Remove(compactedModList.Count() - 1, 1);

			return compactedModList;
		}

		public void DisableDevOptions()
		{
			Prefs.MaxNumberOfPlayerSettlements = 1;

			if (Main._ParametersCache.isAdmin) return;
			else Prefs.DevMode = false;
		}

		public void ExecuteEvent()
		{
			if (Find.AnyPlayerHomeMap == null) return;

			string eventType = Main._ParametersCache.forcedEvent;

			try
			{
				if (eventType == "Raid")
				{
					IncidentDef incidentDef = IncidentDefOf.RaidEnemy;
					IncidentParms defaultParms = StorytellerUtility.DefaultParmsNow(incidentDef.category, Find.AnyPlayerHomeMap);

					IncidentParms parms = new IncidentParms
					{
						customLetterLabel = "Black Market Event - Raid",
						target = Find.AnyPlayerHomeMap,
						points = defaultParms.points,
						faction = defaultParms.faction,
						raidStrategy = defaultParms.raidStrategy,
						raidArrivalMode = defaultParms.raidArrivalMode,
					};

					incidentDef.Worker.TryExecute(parms);
				}

				else if (eventType == "Infestation")
				{
					IncidentDef incidentDef = IncidentDefOf.Infestation;
					IncidentParms defaultParms = StorytellerUtility.DefaultParmsNow(incidentDef.category, Find.AnyPlayerHomeMap);

					IncidentParms parms = new IncidentParms
					{
						customLetterLabel = "Black Market Event - Infestation",
						target = Find.AnyPlayerHomeMap,
						points = defaultParms.points,
					};
					incidentDef.Worker.TryExecute(parms);
				}

				else if (eventType == "MechCluster")
				{
					IncidentDef incidentDef = IncidentDefOf.MechCluster;
					IncidentParms defaultParms = StorytellerUtility.DefaultParmsNow(incidentDef.category, Find.AnyPlayerHomeMap);

					IncidentParms parms = new IncidentParms
					{
						customLetterLabel = "Black Market Event - Cluster",
						target = Find.AnyPlayerHomeMap,
						points = defaultParms.points
					};
					incidentDef.Worker.TryExecute(parms);
				}

				else if (eventType == "ToxicFallout")
				{
					foreach(GameCondition condition in Find.World.GameConditionManager.ActiveConditions)
                    {
						if (condition.def == GameConditionDefOf.ToxicFallout) return;
                    }

					IncidentDef incidentDef = IncidentDefOf.ToxicFallout;
					IncidentParms defaultParms = StorytellerUtility.DefaultParmsNow(incidentDef.category, Find.AnyPlayerHomeMap);

					IncidentParms parms = new IncidentParms
					{
						customLetterLabel = "Black Market Event - Fallout",
						target = Find.AnyPlayerHomeMap,
						points = defaultParms.points
					};
					incidentDef.Worker.TryExecute(parms);
				}

				else if (eventType == "Manhunter")
				{
					IncidentDef incidentDef = IncidentDefOf.ManhunterPack;
					IncidentParms defaultParms = StorytellerUtility.DefaultParmsNow(incidentDef.category, Find.AnyPlayerHomeMap);

					IncidentParms parms = new IncidentParms
					{
						customLetterLabel = "Black Market Event - Manhunter",
						target = Find.AnyPlayerHomeMap,
						points = defaultParms.points
					};
					incidentDef.Worker.TryExecute(parms);
				}

				else if (eventType == "Wanderer")
				{
					IncidentDef incidentDef = IncidentDefOf.WandererJoin;
					IncidentParms defaultParms = StorytellerUtility.DefaultParmsNow(incidentDef.category, Find.AnyPlayerHomeMap);

					IncidentParms parms = new IncidentParms
					{
						customLetterLabel = "Black Market Event - Wanderer",
						target = Find.AnyPlayerHomeMap,
						points = defaultParms.points
					};
					incidentDef.Worker.TryExecute(parms);
				}

				else if (eventType == "FarmAnimals")
				{
					IncidentDef incidentDef = IncidentDefOf.FarmAnimalsWanderIn;
					IncidentParms defaultParms = StorytellerUtility.DefaultParmsNow(incidentDef.category, Find.AnyPlayerHomeMap);

					IncidentParms parms = new IncidentParms
					{
						customLetterLabel = "Black Market Event - Animals",
						target = Find.AnyPlayerHomeMap,
						points = defaultParms.points
					};
					incidentDef.Worker.TryExecute(parms);
				}

				else if (eventType == "ShipChunk")
				{
					IncidentDef incidentDef = IncidentDefOf.ShipChunkDrop;
					IncidentParms defaultParms = StorytellerUtility.DefaultParmsNow(incidentDef.category, Find.AnyPlayerHomeMap);

					IncidentParms parms = new IncidentParms
					{
						target = Find.AnyPlayerHomeMap,
						points = defaultParms.points
					};
					incidentDef.Worker.TryExecute(parms);

					Find.LetterStack.ReceiveLetter("Black Market Event - Chunks", "Space chunks seem to be falling from the sky! You might be able to get materials from them", LetterDefOf.PositiveEvent);
				}

				else if (eventType == "GiveQuest")
				{
					IncidentDef incidentDef = IncidentDefOf.GiveQuest_Random;
					IncidentParms defaultParms = StorytellerUtility.DefaultParmsNow(incidentDef.category, Find.AnyPlayerHomeMap);

					IncidentParms parms = new IncidentParms
					{
						customLetterLabel = "Black Market Event - Quest",
						target = Find.AnyPlayerHomeMap,
						points = defaultParms.points
					};
					incidentDef.Worker.TryExecute(parms);
				}

				else if (eventType == "TraderCaravan")
				{
					IncidentDef incidentDef = IncidentDefOf.TraderCaravanArrival;
					IncidentParms defaultParms = StorytellerUtility.DefaultParmsNow(incidentDef.category, Find.AnyPlayerHomeMap);

					IncidentParms parms = new IncidentParms
					{
						customLetterLabel = "Black Market Event - Trader",
						target = Find.AnyPlayerHomeMap,
						points = defaultParms.points,
						faction = defaultParms.faction,
						traderKind = defaultParms.traderKind
					};

					incidentDef.Worker.TryExecute(parms);
				}
			}

			catch { }

			Main._ParametersCache.forcedEvent = "";

			Injections.thingsToDoInUpdate.Add(Main._MPGame.ForceSave);
		}

        public void SendPlayerSettlementData(Game __instance)
        {
			string dataToSend = "UserSettlement│NewSettlementID│";
			dataToSend += __instance.CurrentMap.Tile + "│";
			dataToSend += (int)__instance.CurrentMap.wealthWatcher.WealthTotal + "│";
			dataToSend += __instance.CurrentMap.mapPawns.AllPawns.FindAll(pawn => pawn.IsColonistPlayerControlled).Count();

			Map map = Find.AnyPlayerHomeMap;
			string dataToSend2 = "UserSettlement│NewSettlementID│";
			dataToSend2 += map.Tile + "│";
			dataToSend2 += (int)map.wealthWatcher.WealthTotal + "│";
			dataToSend2 += map.mapPawns.AllPawns.FindAll(pawn => pawn.IsColonistPlayerControlled).Count();

			if (Find.CurrentMap != null && Find.CurrentMap == Find.AnyPlayerHomeMap) Networking.SendData(dataToSend);
			else if (Find.AnyPlayerHomeMap != null) Networking.SendData(dataToSend);
			else Networking.SendData("UserSettlement│NoSettlementInLoad");
		}

        public void CheckForGifts()
        {
            if (!string.IsNullOrWhiteSpace(Main._ParametersCache.receiveGiftsData))
			{
				if (Find.AnyPlayerHomeMap == null) Main._ParametersCache.receiveGiftsData = "";
				else
                {
					if (Main._ParametersCache.inTrade) return;

					string[] tradeableItems = new string[0];
					if (Main._ParametersCache.receiveGiftsData.Contains('»')) tradeableItems = Main._ParametersCache.receiveGiftsData.Split('»').ToArray();
					else tradeableItems = new string[1] { Main._ParametersCache.receiveGiftsData };

					Find.WindowStack.Add(new Dialog_MPGiftRequest(tradeableItems));
				}
			}
		}

		public void TryGenerateLetter()
        {
			try { Find.LetterStack.ReceiveLetter(Main._ParametersCache.letterTitle, Main._ParametersCache.letterDescription, Main._ParametersCache.letterType); }
			catch { }

			Main._ParametersCache.letterTitle = "";
			Main._ParametersCache.letterDescription = "";
			Main._ParametersCache.letterType = null;
		}

		public void ForceSave()
        {
			string fileName = Main._ParametersCache.onlineFileSaveName + " - " + Main._ParametersCache.connectedServerIdentifier + " - " + Main._ParametersCache.usernameText;

			try
			{
				SafeSaver.Save(GenFilePaths.FilePathForSavedGame(fileName), "savegame", delegate
				{
					ScribeMetaHeaderUtility.WriteMetaHeader();
					Game target = Current.Game;
					Scribe_Deep.Look(ref target, "game");
				}, Find.GameInfo.permadeathMode);
			}
			catch (Exception arg) { Log.Error("Exception while saving game: " + arg); }
		}
	}
}