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
    class ResearchInjections
    {
        [HarmonyPatch(typeof(ResearchManager), "ResearchPerformed")]
        public static class ResearchMonitor
        {

            private const int TICKS_BETWEEN_RUNS = GenDate.TicksPerHour / 2;
            private static int runAfterTick = 0;

            [HarmonyPostfix]
            public static void MonitorResearch()
            {
                if (Networking.isConnectedToServer && FactionResearchGameComponent.factionResearchEnabled)
                {
                    if (Find.TickManager.TicksGame > runAfterTick)
                    {
                        runAfterTick = Find.TickManager.TicksGame + TICKS_BETWEEN_RUNS;
                        FactionResearchHandler.UpdateResearchProgress();
                    }
                }
            }
        }

        [HarmonyPatch(typeof(ResearchManager), "FinishProject")]
        public static class ResearchFinishedMonitor
        {

            [HarmonyPostfix]
            public static void MonitorFinishProject()
            {
                if (Networking.isConnectedToServer && FactionResearchGameComponent.factionResearchEnabled)
                {
                    // Sync a full report every time a research is finished
                    FactionResearchHandler.SendFullResearchReport();
                }
            }
        }

        [HarmonyPatch(typeof(ResearchProjectDef), "CostFactor")]
        public static class ResearchCostMultiplier
        {

            [HarmonyPostfix]
            public static void CostMultiplier(ref float __result)
            {
                if (FactionResearchGameComponent.factionResearchEnabled)
                {
                    __result *= FactionResearchGameComponent.researchCostMultiplier;
                }
            }
        }
    }
}
