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
		public void CreateMultiplayerGame(float globeCoverage, string seed, int rainfall, int temperature, int population)
		{
			Find.GameInitData.permadeathChosen = true;
			Find.GameInitData.permadeath = true;

			Main._ParametersCache.isPlayingOnline = true;

			Main._MPGame.DisableDevOptions();

			OverallRainfall overalRainfall = OverallRainfall.AlmostNone;
			OverallTemperature overallTemperature = OverallTemperature.VeryCold;
			OverallPopulation overallPopulation = OverallPopulation.AlmostNone;

			if (rainfall == 0) overalRainfall = OverallRainfall.AlmostNone;
			else if (rainfall == 1) overalRainfall = OverallRainfall.Little;
			else if (rainfall == 2) overalRainfall = OverallRainfall.LittleBitLess;
			else if (rainfall == 3) overalRainfall = OverallRainfall.Normal;
			else if (rainfall == 4) overalRainfall = OverallRainfall.LittleBitMore;
			else if (rainfall == 5) overalRainfall = OverallRainfall.High;
			else if (rainfall == 6) overalRainfall = OverallRainfall.VeryHigh;

			if (temperature == 0) overallTemperature = OverallTemperature.VeryCold;
			else if (temperature == 1) overallTemperature = OverallTemperature.Cold;
			else if (temperature == 2) overallTemperature = OverallTemperature.LittleBitColder;
			else if (temperature == 3) overallTemperature = OverallTemperature.Normal;
			else if (temperature == 4) overallTemperature = OverallTemperature.LittleBitWarmer;
			else if (temperature == 5) overallTemperature = OverallTemperature.Hot;
			else if (temperature == 6) overallTemperature = OverallTemperature.VeryHot;

			if (population == 0) overallPopulation = OverallPopulation.AlmostNone;
			else if (population == 1) overallPopulation = OverallPopulation.Little;
			else if (population == 2) overallPopulation = OverallPopulation.LittleBitLess;
			else if (population == 3) overallPopulation = OverallPopulation.Normal;
			else if (population == 4) overallPopulation = OverallPopulation.LittleBitMore;
			else if (population == 5) overallPopulation = OverallPopulation.High;
			else if (population == 6) overallPopulation = OverallPopulation.VeryHigh;

			LongEventHandler.QueueLongEvent(delegate
			{
				Find.GameInitData.ResetWorldRelatedMapInitData();
				Current.Game.World = WorldGenerator.GenerateWorld(globeCoverage, seed, overalRainfall, overallTemperature, overallPopulation, null);
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
			if (Main._ParametersCache.isLoadingExistingGame)
			{
				try
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

				catch { Main._Networking.DisconnectFromServer(); }
			}
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

			Main._Injections.thingsToDoInUpdate.Add(Main._MPGame.ForceSave);
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