using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace OpenWorld
{
    class GizmoInjections
    {
		//Get Online Settlement Gizmos
		[HarmonyPatch(typeof(Settlement), "GetGizmos")]
		public static class SetSettlementGizmosForOnline
		{
			[HarmonyPostfix]
			public static void SetSettlementGizmos(ref IEnumerable<Gizmo> __result, Settlement __instance)
			{
				if (Main._ParametersCache.allFactions.Contains(__instance.Faction))
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
							Find.WindowStack.Add(new OW_ErrorDialog("This action is not implemented yet"));
						}
					};

					Command_Action command_FactionMenu = new Command_Action
					{
						defaultLabel = "Faction Menu",
						defaultDesc = "Open Your Faction Menu",
						icon = ContentFinder<Texture2D>.Get("UI/Icons/VisitorsHelp"),
						action = delegate
						{
							Find.WindowStack.Add(new Dialog_MPFactionMenu());
						}
					};

					gizmoList.Add(command_TradingPost);
					gizmoList.Add(command_FactionMenu);
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
				if (Main._ParametersCache.allFactions.Contains(__instance.Faction))
				{
					var gizmoList = __result.ToList();
					List<Gizmo> removeList = new List<Gizmo>();

					foreach (Command_Action action in gizmoList)
					{
						if (action.defaultLabel == "CommandTrade".Translate()) removeList.Add(action);
						else if (action.defaultLabel == "CommandAttackSettlement".Translate()) removeList.Add(action);
						else if (action.defaultLabel == "CommandOfferGifts".Translate()) removeList.Add(action);
					}
					foreach (Gizmo g in removeList) gizmoList.Remove(g);

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

							Find.WindowStack.Add(new OW_ErrorDialog("This action is not implemented yet"));
						}
					};

					Command_Action command_RealtimeAttack = new Command_Action
					{
						defaultLabel = "Realtime Attack",
						defaultDesc = "Attack this settlement realtime",
						icon = ContentFinder<Texture2D>.Get("UI/Commands/AttackSettlement"),
						action = delegate
						{
							if (!Networking.isConnectedToServer) return;

							Main._ParametersCache.focusedSettlement = __instance;
							Main._ParametersCache.focusedCaravan = caravan;

							Find.WindowStack.Add(new OW_ErrorDialog("This action is not implemented yet"));
						}
					};

					Command_Action command_RealtimeVisit = new Command_Action
					{
						defaultLabel = "Realtime Visit",
						defaultDesc = "Visit this settlement realtime",
						icon = ContentFinder<Texture2D>.Get("UI/Commands/Settle"),
						action = delegate
						{
							if (!Networking.isConnectedToServer) return;

							Main._ParametersCache.focusedSettlement = __instance;
							Main._ParametersCache.focusedCaravan = caravan;

							Find.WindowStack.Add(new OW_ErrorDialog("This action is not implemented yet"));
						}
					};

					Command_Action command_Faction = new Command_Action
					{
						defaultLabel = "Faction Menu",
						defaultDesc = "Open the faction menu",
						icon = ContentFinder<Texture2D>.Get("UI/Icons/VisitorsHelp"),
						action = delegate
						{
							if (!Networking.isConnectedToServer) return;

							Main._ParametersCache.focusedSettlement = __instance;
							Main._ParametersCache.focusedCaravan = caravan;

							Find.WindowStack.Add(new Dialog_MPFactionOnPlayer());
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

							if (RimworldHandler.CheckIfAnySocialPawn(0)) Find.WindowStack.Add(new Dialog_MPTrade());
							else Find.WindowStack.Add(new OW_ErrorDialog("Pawn does not have enough social to trade"));
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

							if (RimworldHandler.CheckIfAnySocialPawn(0)) Find.WindowStack.Add(new Dialog_MPBarter(false, null));
							else Find.WindowStack.Add(new OW_ErrorDialog("Pawn does not have enough social to trade"));
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

							if (RimworldHandler.CheckIfAnySocialPawn(0)) Find.WindowStack.Add(new Dialog_MPGift());
							else Find.WindowStack.Add(new OW_ErrorDialog("Pawn does not have enough social to trade"));
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
					gizmoList.Add(command_RealtimeAttack);
					gizmoList.Add(command_RealtimeVisit);
					gizmoList.Add(command_Spy);
					gizmoList.Add(command_Trade);
					gizmoList.Add(command_Barter);
					gizmoList.Add(command_Gift);
					gizmoList.Add(command_InvokeEvent);

					if (Main._ParametersCache.hasFaction == true)
					{
						gizmoList.Add(command_Faction);
					}

					__result = gizmoList;
				}

				else return;
			}
		}

		//Get All World Map Gizmos For Globe
		[HarmonyPatch(typeof(Caravan), "GetGizmos")]
		public static class SetGlobeGizmos
		{
			[HarmonyPostfix]
			public static void SetGizmos(ref IEnumerable<Gizmo> __result, Caravan __instance)
			{
				if (Networking.isConnectedToServer && Main._ParametersCache.hasFaction)
				{
					List<WorldObject> worldObjects = Find.WorldObjects.AllWorldObjects;

					WorldObject objectToFind = worldObjects.Find(fetch => fetch.Tile == __instance.Tile && fetch != __instance);

					List<Gizmo> gizmoList = __result.ToList();

					if (objectToFind != null)
					{
						if (objectToFind.def != WorldObjectDefOf.Site) return;
						else
						{
							Command_Action Command_AttackSite = new Command_Action
							{
								defaultLabel = "Attack site",
								defaultDesc = "Attack this site",
								icon = ContentFinder<Texture2D>.Get("UI/Commands/AttackSettlement"),
								action = delegate
								{
									Main._ParametersCache.focusedTile = objectToFind.Tile;
									Find.WindowStack.Add(new OW_ErrorDialog("This action is not implemented yet"));
								}
							};

							Command_Action Command_AccessSite = new Command_Action
							{
								defaultLabel = "Access Site",
								defaultDesc = "Access this site",
								icon = ContentFinder<Texture2D>.Get("UI/Commands/Install"),
								action = delegate
								{
									int siteType = 0;
									foreach (KeyValuePair<int, List<int>> pair in Main._ParametersCache.allFactionStructures)
									{
										if (pair.Key == objectToFind.Tile) siteType = pair.Value[0];
									}

									Main._ParametersCache.focusedTile = objectToFind.Tile;
									Main._ParametersCache.focusedCaravan = __instance;
									Find.WindowStack.Add(new Dialog_MPFactionSiteBuilt(siteType));
								}
							};

							Command_Action Command_DemolishSite = new Command_Action
							{
								defaultLabel = "Demolish Site",
								defaultDesc = "Demolish this site",
								icon = ContentFinder<Texture2D>.Get("UI/Commands/AbandonHome"),
								action = delegate
								{
									Main._ParametersCache.focusedTile = objectToFind.Tile;
									Find.WindowStack.Add(new Dialog_MPFactionSiteDemolish());
								}
							};

							gizmoList.Add(Command_AttackSite);
							if (objectToFind.Faction == Main._ParametersCache.onlineAllyFaction) gizmoList.Add(Command_AccessSite);
							if (objectToFind.Faction == Main._ParametersCache.onlineAllyFaction) gizmoList.Add(Command_DemolishSite);
							__result = gizmoList;
							return;
						}
					}

					Command_Action Command_BuildOnlineSite = new Command_Action
					{
						defaultLabel = "Build Faction Site",
						defaultDesc = "Build an utility site for your faction",
						icon = ContentFinder<Texture2D>.Get("UI/Commands/Install"),
						action = delegate
						{
							Main._ParametersCache.focusedTile = __instance.Tile;
							Main._ParametersCache.focusedCaravan = __instance;
							Find.WindowStack.Add(new Dialog_MPFactionSiteBuilding());
						}
					};

					gizmoList.Add(Command_BuildOnlineSite);
					__result = gizmoList;
				}

				else return;
			}
		}
	}
}
