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
		public void AddSettlementInWorld()
		{
			if (string.IsNullOrWhiteSpace(Main._ParametersCache.addSettlementData)) return;

			try
			{
				Settlement settlement = (Settlement)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.Settlement);
				settlement.SetFaction(Main._ParametersCache.neutralFaction);
				settlement.Tile = int.Parse(Main._ParametersCache.addSettlementData.Split('│')[2]);
				settlement.Name = Main._ParametersCache.addSettlementData.Split('│')[3] + "'s Settlement";
				Find.WorldObjects.Add(settlement);

				Main._ParametersCache.onlineNeutralSettlements.Add(int.Parse(Main._ParametersCache.addSettlementData.Split('│')[1]), 
					new List<string>() { Main._ParametersCache.addSettlementData.Split('│')[2] });

				Main._ParametersCache.allSettlements.Add(int.Parse(Main._ParametersCache.addSettlementData.Split('│')[1]),
					new List<string>() { Main._ParametersCache.addSettlementData.Split('│')[2] });
			}

			catch { }
		}

		public void RemoveSettlementInWorld()
		{
			if (string.IsNullOrWhiteSpace(Main._ParametersCache.removeSettlementData)) return;

			try
			{
				List<Settlement> settlementList = Find.WorldObjects.Settlements;

				Settlement toDestroy = settlementList.Find(item => item.Tile == int.Parse(Main._ParametersCache.removeSettlementData.Split('│')[2]));

				int settlementTile = int.Parse(Main._ParametersCache.removeSettlementData.Split('│')[2]);

				if (toDestroy.Faction == Main._ParametersCache.neutralFaction) Main._ParametersCache.onlineNeutralSettlements.Remove(settlementTile);
				else if (toDestroy.Faction == Main._ParametersCache.allyFaction) Main._ParametersCache.onlineAllySettlements.Remove(settlementTile);
				else if (toDestroy.Faction == Main._ParametersCache.enemyFaction) Main._ParametersCache.onlineEnemySettlements.Remove(settlementTile);

				Main._ParametersCache.allSettlements.Remove(settlementTile);

				Find.WorldObjects.Remove(toDestroy);
			}

			catch { }
		}

        public void HandleSettlementsLocation()
        {
			List<Settlement> existingSettlements = new List<Settlement>();
			List<Settlement> serverSettlements = new List<Settlement>();

			//Get existing settlements
			foreach (Settlement st in Find.WorldObjects.Settlements)
			{
				if (st.Faction == Main._ParametersCache.neutralFaction ||
					st.Faction == Main._ParametersCache.allyFaction ||
					st.Faction == Main._ParametersCache.enemyFaction)
				{
					existingSettlements.Add(st);
				}
			}

			//Can probably split this into 3 separate loops
			//Get server settlements
			foreach (KeyValuePair<int, List<string>> pair in Main._ParametersCache.allSettlements)
			{
				Settlement dummySettlement = (Settlement)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.Settlement);
				dummySettlement.Tile = pair.Key;
				dummySettlement.Name = pair.Value[0];

				if (pair.Value[1] == "0") dummySettlement.SetFaction(Main._ParametersCache.neutralFaction);
				else if (pair.Value[1] == "1") dummySettlement.SetFaction(Main._ParametersCache.allyFaction);
				else if (pair.Value[1] == "2") dummySettlement.SetFaction(Main._ParametersCache.enemyFaction);

				serverSettlements.Add(dummySettlement);
			}

			foreach (Settlement settlement in existingSettlements)
            {
				Settlement settlementToRemove = Find.WorldObjects.Settlements.Find(fetch => fetch.Tile == settlement.Tile);
				Find.WorldObjects.Remove(settlementToRemove);
			}

			foreach (Settlement settlement in serverSettlements)
            {
                Find.WorldObjects.Add(settlement);
			}
		}

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
