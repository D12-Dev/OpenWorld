using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using HugsLib;
using HarmonyLib;
using Verse.Profile;

namespace OpenWorld
{
	public class Injections : ModBase
	{
		public override string ModIdentifier => Main._ParametersCache.modIdentifier;

		public static List<Action> thingsToDoInUpdate = new List<Action>();

		public static List<Action> queuedActions = new List<Action>();
	}

	//Inject Orders To ModBase
	[HarmonyPatch(typeof(ModBase), "OnGUI")]
	public static class InjectOrdersToRoot
	{
		[HarmonyPostfix]
		public static void InjectToRoot()
		{
			if (Injections.thingsToDoInUpdate.Count > 0)
			{
				foreach(Action a in Injections.thingsToDoInUpdate)
                {
					Injections.queuedActions.Add(a);
				}

				Injections.thingsToDoInUpdate.Clear();

				foreach (Action a in Injections.queuedActions)
				{
					a.Invoke();
				}

				Injections.queuedActions.Clear();
			}
		}
	}

	//Get Correct Name For Online Save
	[HarmonyPatch(typeof(GameDataSaveLoader), "SaveGame", typeof(string))]
	public static class SaveOnlineGame
	{
		[HarmonyPrefix]
		public static bool SaveGameName(ref string fileName)
		{
			if (!Main._ParametersCache.isPlayingOnline) return true;

			Find.GameInfo.permadeathModeUniqueName = Main._ParametersCache.onlineFileSaveName + " - " + Main._ParametersCache.connectedServerIdentifier + " - " + Main._ParametersCache.usernameText;
			fileName = Find.GameInfo.permadeathModeUniqueName;
			return true;
		}
	}

	//Get Tile ID When Starting New Game
	[HarmonyPatch(typeof(Game), "InitNewGame")]
	public static class GetTileIDWhenStartingGame
	{
		[HarmonyPostfix]
		public static void GetIDFromNewGame(Game __instance)
		{
			if (!Networking.isConnectedToServer) return;
			else
            {
				Main._MPGame.EnforceDificultyTweaks();

				Main._MPGame.DisableDevOptions();

				Main._MPGame.SendPlayerSettlementData(__instance);

				Main._ParametersCache.hasLoadedCorrectly = true;
			}
		}
	}

	//Get Tile ID When Loading Existing Game
	[HarmonyPatch(typeof(Game), "LoadGame")]
	public static class GetTileIDWhenLoadingGame
	{
		[HarmonyPostfix]
		public static void GetIDFromExistingGame(Game __instance)
		{
			FactionHandler.FindOnlineFactionsInWorld();

			if (Main._ParametersCache.isPlayingOnline)
			{
				SettlementHandler.ManageSettlementsInWorld();

				FactionHandler.ManageFactionStructuresInWorld();

				Main._MPWorld.HandleRoadGeneration();

				Main._MPGame.EnforceDificultyTweaks();

				Main._MPGame.DisableDevOptions();

				Main._MPGame.SendPlayerSettlementData(__instance);

				Main._ParametersCache.hasLoadedCorrectly = true;

				Main._MPGame.CheckForGifts();
			}
		}
	}

	//Spawn All Online Materials Before Starting Site
	[HarmonyPatch(typeof(Page_SelectStartingSite), "PreOpen")]
	public static class SpawnOnlineMaterials
    {
		[HarmonyPostfix]
		public static void SpawnMaterials()
        {
			if (!Main._ParametersCache.isPlayingOnline) return;

			Settlement alreadyGenerated = Find.WorldObjects.Settlements.Find(fetch => Main._ParametersCache.allFactions.Contains(fetch.Faction));
			if (alreadyGenerated != null) return;

			foreach (FactionDef item in DefDatabase<FactionDef>.AllDefs.OrderBy((FactionDef x) => x.hidden))
			{
				if (item.defName == "OnlineNeutral")
				{
					Main._ParametersCache.onlineNeutralFaction = FactionGenerator.NewGeneratedFaction(new FactionGeneratorParms(item));
					Find.FactionManager.Add(Main._ParametersCache.onlineNeutralFaction);

					foreach (KeyValuePair<int, List<string>> pair in Main._ParametersCache.onlineNeutralSettlements)
					{
						Settlement settlement = (Settlement)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.Settlement);
						settlement.Name = pair.Value[0];
						settlement.Tile = pair.Key;
						settlement.SetFaction(Main._ParametersCache.onlineNeutralFaction);
						Find.WorldObjects.Add(settlement);
					}
				}

				else if (item.defName == "OnlineAlly")
				{
					Main._ParametersCache.onlineAllyFaction = FactionGenerator.NewGeneratedFaction(new FactionGeneratorParms(item));
					Find.FactionManager.Add(Main._ParametersCache.onlineAllyFaction);

					foreach (KeyValuePair<int, List<string>> pair in Main._ParametersCache.onlineAllySettlements)
					{
						Settlement settlement = (Settlement)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.Settlement);
						settlement.Name = pair.Value[0];
						settlement.Tile = pair.Key;
						settlement.SetFaction(Main._ParametersCache.onlineAllyFaction);
						Find.WorldObjects.Add(settlement);
					}
				}

				else if (item.defName == "OnlineEnemy")
				{
					Main._ParametersCache.onlineEnemyFaction = FactionGenerator.NewGeneratedFaction(new FactionGeneratorParms(item));
					Find.FactionManager.Add(Main._ParametersCache.onlineEnemyFaction);

					foreach (KeyValuePair<int, List<string>> pair in Main._ParametersCache.onlineEnemySettlements)
					{
						Settlement settlement = (Settlement)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.Settlement);
						settlement.Name = pair.Value[0];
						settlement.Tile = pair.Key;
						settlement.SetFaction(Main._ParametersCache.onlineEnemyFaction);
						Find.WorldObjects.Add(settlement);
					}
				}
			}

			foreach (KeyValuePair<int, List<int>> pair in Main._ParametersCache.allFactionStructures)
			{
				string siteName = "";
				if (pair.Value[0] == 0) siteName = "Resource Silo";
				else if (pair.Value[0] == 1) siteName = "Marketplace";
				else if (pair.Value[0] == 2) siteName = "Production Site";
				else if (pair.Value[0] == 3) siteName = "Wonder Structure";
				else if (pair.Value[0] == 4) siteName = "Bank";
				else if (pair.Value[0] == 5) siteName = "Stable";
				else if (pair.Value[0] == 6) siteName = "Courier Station";

				Faction factionToGet = null;
				if (pair.Value[1] == 0) factionToGet = Main._ParametersCache.onlineNeutralFaction;
				else if (pair.Value[1] == 1) factionToGet = Main._ParametersCache.onlineAllyFaction;
				else if (pair.Value[1] == 2) factionToGet = Main._ParametersCache.onlineEnemyFaction;

				Site newSite = SiteMaker.MakeSite(sitePart: SitePartDefOf.Outpost,
												  tile: pair.Key,
												  threatPoints: 5000,
												  faction: factionToGet);

				newSite.customLabel = siteName;
				Find.WorldObjects.Add(newSite);
			}

			FactionHandler.FindOnlineFactionsInWorld();

			return;
		}
    }

	//Get Items Gifted To An Online Settlement Via Caravan
	[HarmonyPatch(typeof(Tradeable), "ResolveTrade")]
	public static class GetItemsGiftedToSettlementViaCaravan
	{
		[HarmonyPrefix]
		public static bool GetItemsGifted(List<Thing> ___thingsColony, int ___countToTransfer)
		{
			if (!Networking.isConnectedToServer) return true;

			if (Main._ParametersCache.allFactions.Contains(TradeSession.trader.Faction))
			{
				string itemDefName;

				QualityCategory qc = QualityCategory.Normal;
				try { ___thingsColony[0].TryGetQuality(out qc); }
				catch { }

				string stuffDefName = "";
				try { stuffDefName = ___thingsColony[0].Stuff.defName; }
				catch { }

				if (___thingsColony[0].def == ThingDefOf.MinifiedThing || ___thingsColony[0].def == ThingDefOf.MinifiedTree)
				{
					Thing innerThing = ___thingsColony[0].GetInnerIfMinified();
					itemDefName = "minified-" + innerThing.def.defName;
				}
				else itemDefName = ___thingsColony[0].def.defName;

				if (Main._ParametersCache.transferMode == 0) Main._ParametersCache.giftedItemsString += itemDefName + "┼" + ___countToTransfer + "┼" + ((int)qc) + "┼" + stuffDefName + "»";
				else if (Main._ParametersCache.transferMode == 1) Main._ParametersCache.tradedItemString += itemDefName + "┼" + ___countToTransfer + "┼" + ((int)qc) + "┼" + stuffDefName + "»";
				else if (Main._ParametersCache.transferMode == 2) Main._ParametersCache.barteredItemString += itemDefName + "┼" + ___countToTransfer + "┼" + ((int)qc) + "┼" + stuffDefName + "»";
				else if (Main._ParametersCache.transferMode == 3) Main._ParametersCache.depositItemsString += itemDefName + "┼" + ___countToTransfer + "┼" + ((int)qc) + "┼" + stuffDefName + "»";

				return true;
			}

			else return true;
		}
	}

	//Allow All Items To Be Traded In Online
	[HarmonyPatch(typeof(TradeDeal), "AddAllTradeables")]
	public static class AllowItemTrade
	{
		[HarmonyPrefix]
		public static bool AllowAllItems(ref List<Tradeable> ___tradeables)
		{
			if (Main._ParametersCache.allFactions.Contains(TradeSession.trader.Faction))
			{
				___tradeables = new List<Tradeable>();
				___tradeables.AddRange(Main._ParametersCache.listToShowInGiftMenu);
				___tradeables.AddRange(Main._ParametersCache.listToShowInTradeMenu);
				___tradeables.AddRange(Main._ParametersCache.listToShowInBarterMenu);
				___tradeables.AddRange(Main._ParametersCache.listToShowInSiloMenu);

				return false;
			}

			else return true;
		}
	}
}