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
    public class MPWorld
    {
		public void HandleRoadGeneration()
        {
			if (Main._ParametersCache.roadMode == 0) return;

			List<WorldGenStepDef> GenStepsInOrder = DefDatabase<WorldGenStepDef>.AllDefs.ToList();
			WorldGenStepDef roadGenerator = GenStepsInOrder.Find(a => a.defName == "Roads");

			if (Main._ParametersCache.roadMode == 1)
			{
				roadGenerator.worldGenStep.GenerateFresh(Find.World.info.seedString);
			}

			else if (Main._ParametersCache.roadMode == 2)
			{
				roadGenerator.worldGenStep.GenerateFromScribe(Find.World.info.seedString);
			}
		}
    }
}
