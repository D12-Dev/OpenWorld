using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace OpenWorld
{
    class FloatingOptionInjections
    {
		//Get All World Map Gizmos For Online Settlements
		[HarmonyPatch(typeof(Settlement), "GetFloatMenuOptions")]
		public static class SetWorldMapGizmos
		{
			[HarmonyPostfix]
			public static void SetGizmos(ref IEnumerable<FloatMenuOption> __result, Caravan caravan, Settlement __instance)
			{
				if (Main._ParametersCache.allFactions.Contains(__instance.Faction))
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

		//Get All World Map Gizmos For Online Settlements
		[HarmonyPatch(typeof(Site), "GetFloatMenuOptions")]
		public static class SetSiteFloatingOptions
		{
			[HarmonyPostfix]
			public static void SetFloatingOptions(Site __instance, ref IEnumerable<FloatMenuOption> __result)
			{
				if (Main._ParametersCache.allFactions.Contains(__instance.Faction))
				{
					var gizmoList = __result.ToList();
					gizmoList.Clear();

					__result = gizmoList;
					return;
				}
			}
		}

		//Get Item Transfer From Drop Pod
		[HarmonyPatch(typeof(TransportPodsArrivalAction_GiveGift), "GetFloatMenuOptions")]
		public static class ChangeMenuOptionsOnPod
		{
			[HarmonyPostfix]
			public static void ChangeOptions(ref IEnumerable<FloatMenuOption> __result, Settlement settlement, CompLaunchable representative)
			{
				if (Main._ParametersCache.allFactions.Contains(settlement.Faction))
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
						GiftHandler.SendGiftedPodsToSettlement(representative.TransportersInGroup, settlement);
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

		//Forbid Attacking AI Via Drop Pod
		[HarmonyPatch(typeof(TransportPodsArrivalAction_AttackSettlement), "GetFloatMenuOptions")]
		public static class ForbidAttackAI
		{
			[HarmonyPostfix]
			public static void ForbidAttack(ref IEnumerable<FloatMenuOption> __result, Settlement settlement, CompLaunchable representative)
			{
				if (Main._ParametersCache.allFactions.Contains(settlement.Faction))
				{
					var floatMenuList = __result.ToList();
					floatMenuList.Clear();

					__result = floatMenuList;
					return;
				}

				else return;
			}
		}
	}
}
