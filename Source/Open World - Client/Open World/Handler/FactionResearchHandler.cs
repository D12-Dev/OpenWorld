using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Verse;
using Verse.Noise;

namespace OpenWorld
{
    public static class FactionResearchHandler
    {

        private static bool activelyCompletingResearches = false;

        public static void UpdateResearchProgress()
        {
            string name = Find.ResearchManager.currentProj.defName;
            float progress = Find.ResearchManager.currentProj.ProgressReal;
            float cost = Find.ResearchManager.currentProj.baseCost;
            Networking.SendData("FactionResearch│SetCurrentProgress│" + name + "│" + progress + "│" + cost);
        }

        public static void SendFullResearchReport()
        {
            if (activelyCompletingResearches)
            {
                // Don't send research reports if we're actively completing researches dictated by the server
                return;
            }

            List<string> completedTechs = new List<string>();
            List<string> techProgresses = new List<string>();

            List <ResearchProjectDef> techs = DefDatabase<ResearchProjectDef>.AllDefsListForReading;
            
            foreach(ResearchProjectDef tech in techs)
            {
                if (tech.IsFinished)
                {
                    completedTechs.Add(tech.defName);
                } else
                {
                    techProgresses.Add(tech.defName + ";" + tech.ProgressReal + ";" + tech.baseCost);
                }
            }

            Networking.SendData("FactionResearch│FullResearchReport│" + string.Join(",", completedTechs) + "│" + string.Join(",", techProgresses));
        }

        public static void CompleteResearches(List<string> completedResearches)
        {
            activelyCompletingResearches = true;
            List<ResearchProjectDef> techsCompleted = new List<ResearchProjectDef>();

            foreach (string completedResearchName in completedResearches)
            {
                ResearchProjectDef completedResearch = DefDatabase<ResearchProjectDef>.GetNamed(completedResearchName, false);

                if (completedResearch != null && !completedResearch.IsFinished)
                {
                    Find.ResearchManager.FinishProject(completedResearch, false, null, false);
                    techsCompleted.Add(completedResearch);
                }
            }
            activelyCompletingResearches = false;

            // "ResearchFinished".Translate(this.currentProj.LabelCap) + "\n\n" + this.currentProj.description);
            if (techsCompleted.Count > 0)
            {
                StringBuilder completedTechsString = new StringBuilder();
                completedTechsString.Append("The following techs were completed by the faction: \n\n");
                techsCompleted.ForEach(tech => completedTechsString.Append("ResearchFinished".Translate(tech.LabelCap) + "\n"));

                DiaNode diaNode = new DiaNode(completedTechsString.ToString());
                diaNode.options.Add(DiaOption.DefaultOK);
                DiaOption diaOption = new DiaOption("ResearchScreen".Translate());
                diaOption.resolveTree = true;
                diaOption.action = delegate ()
                {
                    Find.MainTabsRoot.SetCurrentTab(MainButtonDefOf.Research, true);
                };
                diaNode.options.Add(diaOption);
                Find.WindowStack.Add(new Dialog_NodeTree(diaNode, true, false, null));
            }
        }
    }
}
