using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWorld
{
    class EnforcementInjections
    {
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

		//Enforce Difficulty Tweaks
		[HarmonyPatch(typeof(Page_SelectStorytellerInGame), "DoWindowContents")]
		public static class EnforceDifficultyTweaks
		{
			[HarmonyPostfix]
			public static void EnforceDifficulty()
			{
				if (!Main._ParametersCache.isPlayingOnline) return;

				Main._MPGame.EnforceDificultyTweaks();
			}
		}
	}
}
