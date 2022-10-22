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

	//Render Multiplayer Button In Main Menu
	[HarmonyPatch(typeof(MainMenuDrawer), "DoMainMenuControls")]
	public static class InjectMultiplayerButtonOnMainScreen
	{
		[HarmonyPrefix]
		public static bool PreInjectToMainScreen(Rect rect)
		{
			if (!(Current.ProgramState == ProgramState.Entry)) return true;

			Vector2 buttonLocation = new Vector2(rect.x, rect.y);
			Vector2 buttonSize = new Vector2(170f, 45f);
			if (Widgets.ButtonText(new Rect(buttonLocation.x, buttonLocation.y, buttonSize.x, buttonSize.y), "Multiplayer"))
			{
				Find.WindowStack.Add(new Dialog_MPMultiplayerType());
			}
			return true;
		}

		[HarmonyPostfix]
		public static void PostInjectToMainScreen(Rect rect)
		{
			if (!(Current.ProgramState == ProgramState.Entry)) return;

			Vector2 buttonLocation = new Vector2(rect.x, rect.y);
			Vector2 buttonSize = new Vector2(170f, 45f);
			if (Widgets.ButtonText(new Rect(buttonLocation.x, buttonLocation.y, buttonSize.x, buttonSize.y), "Multiplayer"))
			{
				//Do nothing since it's a dummy
			}
			return;
		}
	}

	//Inject Multiplayer Joining At Storyteller Selection
	[HarmonyPatch(typeof(Page_CreateWorldParams), "DoWindowContents")]
	public static class InjectMultiplayerJoinAtWorldParams
	{
		[HarmonyPrefix]
		public static bool PreInjectToWorldParams(Rect rect, Page_CreateWorldParams __instance)
		{
			if (!Main._ParametersCache.isGeneratingNewOnlineGame) return true;
			if (!(Current.ProgramState == ProgramState.Entry)) return true;

			Vector2 buttonSize = new Vector2(150f, 38f);
			Vector2 buttonLocation = new Vector2(rect.xMax - buttonSize.x, rect.yMax - buttonSize.y);
			if (Widgets.ButtonText(new Rect(buttonLocation.x, buttonLocation.y, buttonSize.x, buttonSize.y), ""))
			{
				Main._ParametersCache.__createWorldParams = __instance;
				Find.WindowStack.Add(new Dialog_MPParameters());
			}

			Vector2 buttonLocation2 = new Vector2(rect.xMin, rect.yMax - buttonSize.y);
			if (Widgets.ButtonText(new Rect(buttonLocation2.x, buttonLocation2.y, buttonSize.x, buttonSize.y), ""))
			{
				Main._ParametersCache.isGeneratingNewOnlineGame = false;
				__instance.Close();
			}

			return true;
		}

		[HarmonyPostfix]
		public static void PostInjectToWorldParams(Rect rect)
		{
			if (!Main._ParametersCache.isGeneratingNewOnlineGame) return;
			if (!(Current.ProgramState == ProgramState.Entry)) return;

			Vector2 buttonSize = new Vector2(150f, 38f);
			Vector2 buttonLocation = new Vector2(rect.xMax - buttonSize.x, rect.yMax - buttonSize.y);
			if (Widgets.ButtonText(new Rect(buttonLocation.x, buttonLocation.y, buttonSize.x, buttonSize.y), "Join"))
			{
				//Do nothing since it's a dummy
			}

			Vector2 buttonLocation2 = new Vector2(rect.xMin, rect.yMax - buttonSize.y);
			if (Widgets.ButtonText(new Rect(buttonLocation2.x, buttonLocation2.y, buttonSize.x, buttonSize.y), "Close"))
			{
				//Do nothing since it's a dummy
			}

			return;
		}
	}

	//Render In Game Stuff
	[HarmonyPatch(typeof(MainMenuDrawer), "DoMainMenuControls")]
	public static class RenderStuffInEscMenu
	{
        [HarmonyPrefix]
		public static bool ModifySaveButton()
        {
			if (!Main._ParametersCache.isPlayingOnline) return true;
			if (!(Current.ProgramState == ProgramState.Playing)) return true;

			Vector2 buttonSize = new Vector2(170f, 45f);
			if (Widgets.ButtonText(new Rect(0, (buttonSize.y + 7) * 2, buttonSize.x, buttonSize.y), ""))
			{
				LongEventHandler.QueueLongEvent(delegate
				{
					Main._ParametersCache.isPlayingOnline = false;
					if (Networking.isConnectedToServer) Networking.DisconnectFromServer();

					Find.GameInfo.permadeathModeUniqueName = Main._ParametersCache.onlineFileSaveName + " - " + Main._ParametersCache.connectedServerIdentifier + " - " + Main._ParametersCache.usernameText;
					GameDataSaveLoader.SaveGame(Find.GameInfo.permadeathModeUniqueName);
					MemoryUtility.ClearAllMapsAndWorld();
				}, "Entry", "SavingLongEvent", doAsynchronously: false, null, showExtraUIInfo: false);
			}

			return true;
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

	//Disconnect From Server When Going Back From World Map
	[HarmonyPatch(typeof(Page_SelectStartingSite), "DoCustomBottomButtons")]
	public static class DisconnectWhenBack
	{
		[HarmonyPrefix]
		public static bool DisconnectOnBack(Page_SelectStartingSite __instance)
		{
			if (!Main._ParametersCache.isPlayingOnline) return true;

			int num = TutorSystem.TutorialMode ? 4 : 5;
			int num2 = (num < 4 || !((float)UI.screenWidth < 540f + (float)num * (150f + 10f))) ? 1 : 2;
			int num3 = Mathf.CeilToInt((float)num / (float)num2);
			float num4 = 150f * (float)num3 + 10f * (float)(num3 + 1);
			float num5 = (float)num2 * 38f + 10f * (float)(num2 + 1);
			Rect rect = new Rect(((float)UI.screenWidth - num4) / 2f, (float)UI.screenHeight - num5 - 4f, num4, num5);
			WorldInspectPane worldInspectPane = Find.WindowStack.WindowOfType<WorldInspectPane>();
			if (worldInspectPane != null && rect.x < InspectPaneUtility.PaneWidthFor(worldInspectPane) + 4f)
			{
				rect.x = InspectPaneUtility.PaneWidthFor(worldInspectPane) + 4f;
			}

			Widgets.DrawWindowBackground(rect);
			float num6 = rect.xMin + 10f;
			float num7 = rect.yMin + 10f;
			Text.Font = GameFont.Small;
			if ((Widgets.ButtonText(new Rect(num6, num7, 150f, 38f), "Back".Translate()) || KeyBindingDefOf.Cancel.KeyDownEvent))
			{
				Main._ParametersCache.isPlayingOnline = false;
				if (Networking.isConnectedToServer) Networking.DisconnectFromServer();

				if (__instance.prev != null)
				{
					Find.WindowStack.Add(__instance.prev);
					__instance.Close();
				}
			}

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
			string dataToSend = "NewSettlementID│";
			dataToSend += __instance.CurrentMap.Tile + "│";
			dataToSend += (int) __instance.CurrentMap.wealthWatcher.WealthTotal + "│";
			dataToSend += __instance.CurrentMap.mapPawns.AllPawns.FindAll(pawn => pawn.IsColonistPlayerControlled).Count();

			if (Networking.isConnectedToServer) Networking.SendData(dataToSend);
			else return;
		}
	}

	//Get Tile ID When Loading Existing Game
	[HarmonyPatch(typeof(Game), "LoadGame")]
	public static class GetTileIDWhenLoadingGame
	{
		[HarmonyPostfix]
		public static void GetIDFromExistingGame(Game __instance)
		{
			Main._MPWorld.FindOnlineFactionInWorld();

			if (Main._ParametersCache.isPlayingOnline)
			{
				Main._MPWorld.HandleSettlementsLocation();

				Main._MPWorld.HandleRoadGeneration();

				Main._MPGame.SendPlayerSettlementData(__instance);

				Main._MPGame.CheckForGifts();
			}

			return;
		}
	}

	//Plant Settlements Of Other Players In The World
	[HarmonyPatch(typeof(FactionGenerator), "GenerateFactionsIntoWorld")]
	public static class SpawnSettlements
	{
		[HarmonyPrefix]
		public static bool SpawnOnlineSettlements()
		{
			if (!Main._ParametersCache.isPlayingOnline) return true;

			foreach (FactionDef item in DefDatabase<FactionDef>.AllDefs.OrderBy((FactionDef x) => x.hidden))
			{
				if (item.defName == "Online")
				{
					Main._ParametersCache.faction = FactionGenerator.NewGeneratedFaction(new FactionGeneratorParms(item));
					Find.FactionManager.Add(Main._ParametersCache.faction);

					foreach(KeyValuePair<int, List<string>> pair in Main._ParametersCache.onlineSettlements)
					{
						Settlement settlement = (Settlement)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.Settlement);
						settlement.SetFaction(Main._ParametersCache.faction);
						settlement.Tile = pair.Key;
						settlement.Name = pair.Value[0] + "'s Settlement";
						Find.WorldObjects.Add(settlement);
					}

					return true;
				}
			}

			return true;
		}
	}

	//Get Tile ID Of New Settlement
	[HarmonyPatch(typeof(SettleInEmptyTileUtility), "Settle")]
	public static class GetTileIDOfNewSettlement
	{
		[HarmonyPostfix]
		public static void GetIDFromNewSettlement(ref Caravan caravan)
		{
			if (Networking.isConnectedToServer)
			{
				if (Find.AnyPlayerHomeMap == null) Networking.SendData("NewSettlementID│" + caravan.Tile);
				else return;
			}
			else return;
		}
	}

	//Get Tile ID Of Abandoned Settlement
	[HarmonyPatch(typeof(SettlementAbandonUtility), "Abandon")]
	public static class GetTileIDOfAbandonedSettlement
	{
		[HarmonyPostfix]
		public static void GetIDFromAbandonedSettlement(ref Settlement settlement)
		{
			if (Networking.isConnectedToServer)
			{
				if (Find.AnyPlayerHomeMap == null) Networking.SendData("AbandonSettlementID│" + settlement.Tile);
				else return;
			}
			else return;
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

			if (TradeSession.trader.Faction == Main._ParametersCache.faction)
			{
				string itemDefName = "";

				if (___thingsColony[0] is Pawn)
                {
					Pawn sentPawn = (Pawn) ___thingsColony[0];
					string pawnData = "pawn┼";
					pawnData += sentPawn.Name + "┼";
					pawnData += sentPawn.ageTracker.AgeBiologicalTicks + "┼";
					pawnData += sentPawn.ageTracker.AgeChronologicalTicks + "┼";
					pawnData += (int)sentPawn.gender + "┼";

					pawnData += "‼";

					try { pawnData += sentPawn.story.Childhood.identifier + "┼"; }
					catch { }

					try { pawnData += sentPawn.story.Adulthood.identifier + "┼"; }
					catch { }

					pawnData += "‼";

					foreach (SkillRecord s in sentPawn.skills.skills)
                    {
						pawnData += s.levelInt + "-";
						pawnData += (int)s.passion + "┼";
					}

					pawnData += "‼";

					foreach (Trait t in sentPawn.story.traits.allTraits)
                    {
						pawnData += t.Label + "┼";
					}

					pawnData = pawnData.Remove(pawnData.Count() - 1, 1);

					//Add more

					if (Main._ParametersCache.transferMode == "Gift") Main._ParametersCache.giftedItemsString += pawnData + "»";
					else if (Main._ParametersCache.transferMode == "Trade") Main._ParametersCache.tradedItemString += pawnData + "»";
				}

				else
				{
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

					if (Main._ParametersCache.transferMode == "Gift") Main._ParametersCache.giftedItemsString += itemDefName + "┼" + ___countToTransfer + "┼" + ((int)qc) + "┼" + stuffDefName + "»";
					else if (Main._ParametersCache.transferMode == "Trade") Main._ParametersCache.tradedItemString += itemDefName + "┼" + ___countToTransfer + "┼" + ((int)qc) + "┼" + stuffDefName + "»";
				}
				return true;
			}

			else return true;
		}
	}

    //Get Items Gifted To An Online Settlement Via Drop Pod
    [HarmonyPatch(typeof(TransportPodsArrivalAction_GiveGift), "GetFloatMenuOptions")]
    public static class ChangeMenuOptionsOnPod
    {
        [HarmonyPostfix]
        public static void ChangeOptions(ref IEnumerable<FloatMenuOption> __result, Settlement settlement, CompLaunchable representative)
        {
            if (settlement.Faction == Main._ParametersCache.faction)
            {
                var floatMenuList = __result.ToList();
                floatMenuList.Clear();

                if (!Networking.isConnectedToServer)
                {
                    __result = floatMenuList;
                    return;
                }

                Action action = delegate
                {
                    MPCaravan.SendGiftedPodsToSettlement(representative.TransportersInGroup, settlement);
                    representative.TryLaunch(settlement.Tile, new TransportPodsArrivalAction_GiveGift(settlement));
                };

                FloatMenuOption option = new FloatMenuOption(label: "Send a gift to " + settlement.Name, action: action);

                floatMenuList.Add(option);
                __result = floatMenuList;
                return;
            }

            else return;
        }
    }

	//Forbid attacking AI Via Drop Pod
	[HarmonyPatch(typeof(TransportPodsArrivalAction_AttackSettlement), "GetFloatMenuOptions")]
	public static class ForbidAttackAI
	{
		[HarmonyPostfix]
		public static void ForbidAttack(ref IEnumerable<FloatMenuOption> __result, Settlement settlement, CompLaunchable representative)
		{
			if (settlement.Faction == Main._ParametersCache.faction)
			{
				var floatMenuList = __result.ToList();
				floatMenuList.Clear();

				__result = floatMenuList;
				return;
			}

			else return;
		}
	}

	//Allow All Items To Be Traded In Online
	[HarmonyPatch(typeof(TradeDeal), "AddAllTradeables")]
	public static class AllowItemTrade
	{
		[HarmonyPrefix]
		public static bool AllowAllItems(ref List<Tradeable> ___tradeables)
		{
			if (TradeSession.trader.Faction == Main._ParametersCache.faction)
			{
				___tradeables = Main._ParametersCache.listToShowInGiftMenu;
				return false;
			}

			else return true;
		}
	}

	//Prevent Options Change
	[HarmonyPatch(typeof(Dialog_Options), "DoWindowContents")]
	public static class PreventOptions
	{
		[HarmonyPostfix]
		public static void PreventOptionsChange()
		{
			if (!Main._ParametersCache.isPlayingOnline) return;

			Main._MPGame.DisableDevOptions();
		}
	}

	//Prevent Goodwill Change Next To Other Player
	[HarmonyPatch(typeof(SettlementProximityGoodwillUtility), "AppendProximityGoodwillOffsets")]
	public static class PrevenGoodwillChangeOnSettle
	{
		[HarmonyPrefix]
		public static bool PreventGoodwillChange(ref int tile, ref List<Pair<Settlement, int>> outOffsets)
		{
			int maxDist = SettlementProximityGoodwillUtility.MaxDist;
			List<Settlement> settlements = Find.WorldObjects.Settlements;
			for (int i = 0; i < settlements.Count; i++)
			{
				Settlement settlement = settlements[i];
				if (settlement.Faction == null || settlement.Faction == Main._ParametersCache.faction || settlement.Faction == Faction.OfPlayer || settlement.Faction.def.permanentEnemy || settlement.Faction.PlayerGoodwill == -100)
				{
					continue;
				}

				int num = Find.WorldGrid.TraversalDistanceBetween(tile, settlement.Tile, passImpassable: false, maxDist);
				if (num != int.MaxValue)
				{
					int num2 = Mathf.RoundToInt(DiplomacyTuning.Goodwill_PerQuadrumFromSettlementProximity.Evaluate(num));
					if (num2 != 0)
					{
						outOffsets.Add(new Pair<Settlement, int>(settlement, num2));
					}
				}
			}

			return false;
		}
	}

	//Get Online Settlement Gizmos
	[HarmonyPatch(typeof(Settlement), "GetGizmos")]
	public static class SetSettlementGizmosForOnline
	{
		[HarmonyPostfix]
		public static void SetSettlementGizmos(ref IEnumerable<Gizmo> __result, Settlement __instance)
		{
			if (__instance.Faction == Main._ParametersCache.faction)
			{
				Main._ParametersCache.focusedSettlement = __instance;

				var gizmoList = __result.ToList();
				gizmoList.Clear();

				if (!Networking.isConnectedToServer)
				{
					__result = gizmoList;
					return;
				}

				__result = gizmoList;
			}

			else if (__instance.Faction == Find.FactionManager.OfPlayer)
            {
				var gizmoList = __result.ToList();

				if (!Networking.isConnectedToServer) return;

				Command_Action command_TradingPost = new Command_Action
				{
					defaultLabel = "Trading Post",
					defaultDesc = "See what the world has to offer",
					icon = ContentFinder<Texture2D>.Get("UI/Commands/MergeCaravans"),
					action = delegate
					{
						Find.WindowStack.Add(new Dialog_MPNotImplemented());
						//Find.WindowStack.Add(new Dialog_MPMarket());
					}
				};

				gizmoList.Add(command_TradingPost);
				__result = gizmoList;
			}

			else return;
		}
	}

	//Get All Caravan Gizmos For Online Settlements
	[HarmonyPatch(typeof(Settlement), "GetCaravanGizmos")]
	public static class SetCaravanGizmosForOnlineSettlement
	{
		[HarmonyPostfix]
		public static void SetCaravanGizmos(ref IEnumerable<Gizmo> __result, Settlement __instance, Caravan caravan)
		{
			if (__instance.Faction == Main._ParametersCache.faction)
			{
				var gizmoList = __result.ToList();
				List<Gizmo> removeList = new List<Gizmo>();

				foreach (Command_Action action in gizmoList)
                {
					if (action.defaultLabel == "CommandTrade".Translate()) removeList.Add(action);
					else if (action.defaultLabel == "CommandAttackSettlement".Translate()) removeList.Add(action);
					else if (action.defaultLabel == "CommandOfferGifts".Translate()) removeList.Add(action);
				}
				foreach(Gizmo g in removeList) gizmoList.Remove(g);

				if (!Networking.isConnectedToServer)
				{
					__result = gizmoList;
					return;
				}

				Command_Action command_Attack = new Command_Action
				{
					defaultLabel = "Attack",
					defaultDesc = "Attack this settlement",
					icon = ContentFinder<Texture2D>.Get("UI/Commands/AttackSettlement"),
					action = delegate
					{
						if (!Networking.isConnectedToServer) return;

						Main._ParametersCache.focusedSettlement = __instance;
						Main._ParametersCache.focusedCaravan = caravan;

						//Main._MPRTSE.SetupRaidParameters();

						Find.WindowStack.Add(new Dialog_MPNotImplemented());
					}
				};

				Command_Action command_Visit = new Command_Action
				{
					defaultLabel = "Visit",
					defaultDesc = "Visit this settlement",
					icon = ContentFinder<Texture2D>.Get("UI/Commands/Settle"),
					action = delegate
					{
						if (!Networking.isConnectedToServer) return;

						Main._ParametersCache.focusedSettlement = __instance;
						Main._ParametersCache.focusedCaravan = caravan;

						Find.WindowStack.Add(new Dialog_MPNotImplemented());
					}
				};

				Command_Action command_Spy = new Command_Action
				{
					defaultLabel = "Spy",
					defaultDesc = "Pay in silver to spy on this settlement",
					icon = ContentFinder<Texture2D>.Get("UI/Commands/ShowMap"),
					action = delegate
					{
						if (!Networking.isConnectedToServer) return;

						Main._ParametersCache.focusedSettlement = __instance;
						Main._ParametersCache.focusedCaravan = caravan;

						Find.WindowStack.Add(new Dialog_MPSpy());
					}
				};

				Command_Action command_Trade = new Command_Action
				{
					defaultLabel = "Trade",
					defaultDesc = "Trade with this settlement",
					icon = ContentFinder<Texture2D>.Get("UI/Commands/Trade"),
					action = delegate
					{
						if (!Networking.isConnectedToServer) return;

						Main._ParametersCache.focusedSettlement = __instance;
						Main._ParametersCache.focusedCaravan = caravan;

						Find.WindowStack.Add(new Dialog_MPTrade());
					}
				};

				Command_Action command_Barter = new Command_Action
				{
					defaultLabel = "Barter",
					defaultDesc = "Barter with this settlement",
					icon = ContentFinder<Texture2D>.Get("UI/Commands/FulfillTradeRequest"),
					action = delegate
					{
						if (!Networking.isConnectedToServer) return;

						Main._ParametersCache.focusedSettlement = __instance;
						Main._ParametersCache.focusedCaravan = caravan;

						//Injections.OpenDialogs(3);
						Find.WindowStack.Add(new Dialog_MPBarter(false, null));
					}
				};

				Command_Action command_Gift = new Command_Action
				{
					defaultLabel = "Gift",
					defaultDesc = "Gift items to this settlement",
					icon = ContentFinder<Texture2D>.Get("UI/Commands/OfferGifts"),
					action = delegate
					{
						if (!Networking.isConnectedToServer) return;

						Main._ParametersCache.focusedSettlement = __instance;
						Main._ParametersCache.focusedCaravan = caravan;

						Find.WindowStack.Add(new Dialog_MPGift());
					}
				};

				Command_Action command_InvokeEvent = new Command_Action
				{
					defaultLabel = "Black Market",
					defaultDesc = "Pay silver to cause events in this settlement",
					icon = ContentFinder<Texture2D>.Get("UI/Commands/CallAid"),
					action = delegate
					{
						if (!Networking.isConnectedToServer) return;

						Main._ParametersCache.focusedSettlement = __instance;
						Main._ParametersCache.focusedCaravan = caravan;

						Find.WindowStack.Add(new Dialog_MPBlackMarket());
					}
				};

				gizmoList.Add(command_Attack);
				gizmoList.Add(command_Visit);
				gizmoList.Add(command_Spy);
				gizmoList.Add(command_Trade);
				gizmoList.Add(command_Barter);
				gizmoList.Add(command_Gift);
				gizmoList.Add(command_InvokeEvent);

				__result = gizmoList;
			}

			else return;
		}
	}

	//Get All World Map Gizmos For Online Settlements
	[HarmonyPatch(typeof(Settlement), "GetFloatMenuOptions")]
	public static class SetWorldMapGizmos
	{
		[HarmonyPostfix]
		public static void SetGizmos(ref IEnumerable<FloatMenuOption> __result, Caravan caravan, Settlement __instance)
		{
			if (__instance.Faction == Main._ParametersCache.faction)
			{
				var gizmoList = __result.ToList();
				gizmoList.Clear();

				if (CaravanVisitUtility.SettlementVisitedNow(caravan) != __instance)
				{
					foreach (FloatMenuOption floatMenuOption2 in CaravanArrivalAction_VisitSettlement.GetFloatMenuOptions(caravan, __instance))
					{
						gizmoList.Add(floatMenuOption2);
					}
				}

				__result = gizmoList;
			}

			else return;
		}
	}

	//Add Find Button To Planet View
	[HarmonyPatch(typeof(WorldInspectPane), "SetInitialSizeAndPosition")]
	public static class AddFindButton
    {
		[HarmonyPrefix]
		public static bool AddButton(ref WITab[] ___TileTabs)
        {
			if (___TileTabs.Count() == 3) return true;

			if (Networking.isConnectedToServer)
			{
				___TileTabs = new WITab[3]
				{
					new MP_WITabFind(),
					new WITab_Terrain(),
					new WITab_Planet()
				};
			}

			return true;
        }
	}
}