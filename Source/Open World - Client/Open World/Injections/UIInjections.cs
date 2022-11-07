using HarmonyLib;
using HugsLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Profile;

namespace OpenWorld
{
    class UIInjections
    {
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
				if (Widgets.ButtonText(new Rect(buttonLocation.x, buttonLocation.y, buttonSize.x, buttonSize.y), ""))
				{
					Find.WindowStack.Add(new Dialog_MPMultiplayerType());
				}

				Vector2 buttonLocation2 = new Vector2(rect.x, rect.y + buttonSize.y + 7.495f);
				if (Widgets.ButtonText(new Rect(buttonLocation2.x, buttonLocation2.y, buttonSize.x, buttonSize.y), ""))
				{
					Main._ParametersCache.isPlayingOnline = false;
					Main._ParametersCache.isGeneratingNewOnlineGame = false;
					Main._ParametersCache.isLoadingExistingGame = false;

					Find.WindowStack.Add(new Page_SelectScenario());
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
			}
		}

		//Inject Multiplayer Joining At Create World Selection
		[HarmonyPatch(typeof(Page_CreateWorldParams), "DoWindowContents")]
		public static class InjectMultiplayerJoinAtWorldParams
		{
			[HarmonyPrefix]
			public static bool PreInjectToWorldParams(Rect rect, Page_CreateWorldParams __instance)
			{
				if (!Main._ParametersCache.isGeneratingNewOnlineGame) return true;
				if (Current.ProgramState != ProgramState.Entry) return true;

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
					__instance.Close();
				}

				return true;
			}

			[HarmonyPostfix]
			public static void PostInjectToWorldParams(Rect rect)
			{
				if (!Main._ParametersCache.isGeneratingNewOnlineGame) return;
				if (Current.ProgramState != ProgramState.Entry) return;

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

		//Modify Save Button On In-Game Menu
		[HarmonyPatch(typeof(MainMenuDrawer), "DoMainMenuControls")]
		public static class RenderStuffInEscMenu
		{
			[HarmonyPrefix]
			public static bool ModifySaveButton()
			{
				if (!Main._ParametersCache.isPlayingOnline) return true;
				if (Current.ProgramState == ProgramState.Entry) return true;

				Vector2 buttonSize = new Vector2(170f, 45f);
				if (Widgets.ButtonText(new Rect(0, (buttonSize.y + 7) * 2, buttonSize.x, buttonSize.y), ""))
				{
					LongEventHandler.QueueLongEvent(delegate
					{
						if (Networking.isConnectedToServer) Networking.DisconnectFromServer();

						Main._ParametersCache.isPlayingOnline = false;
						Main._ParametersCache.isGeneratingNewOnlineGame = false;
						Main._ParametersCache.isLoadingExistingGame = false;
						Main._ParametersCache.hasLoadedCorrectly = false;

						Find.GameInfo.permadeathModeUniqueName = Main._ParametersCache.onlineFileSaveName + " - " + Main._ParametersCache.connectedServerIdentifier + " - " + Main._ParametersCache.usernameText;
						GameDataSaveLoader.SaveGame(Find.GameInfo.permadeathModeUniqueName);
						MemoryUtility.ClearAllMapsAndWorld();
					}, "Entry", "SavingLongEvent", doAsynchronously: false, null, showExtraUIInfo: false);
				}

				return true;
			}
		}

		//Modify Back Button At World Screen
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
				if (Widgets.ButtonText(new Rect(num6, num7, 150f, 38f), "Back".Translate()) || KeyBindingDefOf.Cancel.KeyDownEvent)
				{
					if (Networking.isConnectedToServer) Networking.DisconnectFromServer();

					Main._ParametersCache.isPlayingOnline = false;
					Main._ParametersCache.isGeneratingNewOnlineGame = false;
					Main._ParametersCache.isLoadingExistingGame = false;
					Main._ParametersCache.hasLoadedCorrectly = false;

					if (__instance.prev != null)
					{
						Find.WindowStack.Add(__instance.prev);
						__instance.Close();
					}
				}

				return true;
			}
		}

		//Add Buttons To Planet View Side Panel
		[HarmonyPatch(typeof(WorldInspectPane), "SetInitialSizeAndPosition")]
		public static class AddFindButton
		{
			[HarmonyPrefix]
			public static bool AddButton(ref WITab[] ___TileTabs)
			{
				if (___TileTabs.Count() == 4) return true;

				if (Main._ParametersCache.isPlayingOnline)
				{
					___TileTabs = new WITab[4]
					{
					new MP_WITabOnlinePlayers(),
					new MP_WITabSettlementList(),
					new WITab_Terrain(),
					new WITab_Planet()
					};
				}

				return true;
			}
		}
	}
}
