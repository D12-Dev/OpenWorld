using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.IO;
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

			if (!File.Exists(Main._ParametersCache.gameSavePath + Path.DirectorySeparatorChar + saveName + ".rws"))
            {
				Find.WindowStack.Add(new Dialog_MPMissingSave());
				Networking.DisconnectFromServer();
				return;
            }

			Main._ParametersCache.isPlayingOnline = true;

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

		public void EnforceDificultyTweaks()
        {
			if (Main._ParametersCache.enforcedDifficultyMode == 0) return;

			Current.Game.storyteller.difficulty.threatScale = 2.0f;
			Current.Game.storyteller.difficulty.allowBigThreats = true;
			Current.Game.storyteller.difficulty.allowViolentQuests = true;
			Current.Game.storyteller.difficulty.allowIntroThreats = true;
			Current.Game.storyteller.difficulty.predatorsHuntHumanlikes = true;
			Current.Game.storyteller.difficulty.allowExtremeWeatherIncidents = true;

			Current.Game.storyteller.difficulty.cropYieldFactor = 0.50f;
			Current.Game.storyteller.difficulty.mineYieldFactor = 0.50f;
			Current.Game.storyteller.difficulty.butcherYieldFactor = 0.50f;
			Current.Game.storyteller.difficulty.researchSpeedFactor = 0.90f;
			Current.Game.storyteller.difficulty.questRewardValueFactor = 0.5f;
			Current.Game.storyteller.difficulty.raidLootPointsFactor = 0.50f;
			Current.Game.storyteller.difficulty.tradePriceFactorLoss = 0.65f;
			Current.Game.storyteller.difficulty.maintenanceCostFactor = 1.0f;
			Current.Game.storyteller.difficulty.scariaRotChance = 1.0f;
			Current.Game.storyteller.difficulty.enemyDeathOnDownedChanceFactor = 1.0f;

			Current.Game.storyteller.difficulty.colonistMoodOffset = -10f;
			Current.Game.storyteller.difficulty.foodPoisonChanceFactor = 1.50f;
			Current.Game.storyteller.difficulty.manhunterChanceOnDamageFactor = 2.00f;
			Current.Game.storyteller.difficulty.playerPawnInfectionChanceFactor = 1.25f;
			Current.Game.storyteller.difficulty.diseaseIntervalFactor = 1.25f;
			Current.Game.storyteller.difficulty.deepDrillInfestationChanceFactor = 2.00f;
			Current.Game.storyteller.difficulty.friendlyFireChanceFactor = 0.5f;
			Current.Game.storyteller.difficulty.allowInstantKillChance = 1.0f;

			Current.Game.storyteller.difficulty.allowTraps = true;
			Current.Game.storyteller.difficulty.allowTurrets = true;
			Current.Game.storyteller.difficulty.allowMortars = true;

			Current.Game.storyteller.difficulty.adaptationEffectFactor = 1.0f;
			Current.Game.storyteller.difficulty.adaptationGrowthRateFactorOverZero = 1.0f;
			Current.Game.storyteller.difficulty.fixedWealthMode = false;
		}

		public void ExecuteEvent()
		{
			if (!Main._ParametersCache.hasLoadedCorrectly)
            {
				Main._ParametersCache.forcedEvent = "";
				return;
			}

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

			if (Find.CurrentMap != null && Find.CurrentMap == Find.AnyPlayerHomeMap) Networking.SendData(dataToSend);
			else if (Find.AnyPlayerHomeMap != null) Networking.SendData(dataToSend);
			else Networking.SendData("UserSettlement│NoSettlementInLoad");
		}

        public void CheckForGifts()
        {
			if (string.IsNullOrWhiteSpace(Main._ParametersCache.receiveGiftsData)) return;

			if (!Main._ParametersCache.hasLoadedCorrectly) return;

			if (Main._ParametersCache.inTrade)
            {
				GiftHandler.ResetGiftVariables();
				return;
			}

			if (Find.AnyPlayerHomeMap == null)
            {
				GiftHandler.ResetGiftVariables();
				return;
			}

			string[] tradeableItems;
			if (Main._ParametersCache.receiveGiftsData.Contains('»')) tradeableItems = Main._ParametersCache.receiveGiftsData.Split('»').ToArray();
			else tradeableItems = new string[1] { Main._ParametersCache.receiveGiftsData };

			GiftHandler.ResetGiftVariables();

			Find.WindowStack.Add(new Dialog_MPGiftRequest(tradeableItems));
		}

		//Breaks game saves sometimes, need to search better way of force saving
		public void ForceSave()
        {
			//string fileName = Main._ParametersCache.onlineFileSaveName + " - " + Main._ParametersCache.connectedServerIdentifier + " - " + Main._ParametersCache.usernameText;

			//try
			//{
			//	SafeSaver.Save(GenFilePaths.FilePathForSavedGame(fileName), "savegame", delegate
			//	{
			//		ScribeMetaHeaderUtility.WriteMetaHeader();
			//		Game target = Current.Game;
			//		Scribe_Deep.Look(ref target, "game");
			//	}, Find.GameInfo.permadeathMode);
			//}
			//catch (Exception arg) { Log.Error("Exception while saving game: " + arg); }
		}
	}
}