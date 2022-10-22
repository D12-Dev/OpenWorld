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
		public void AddSettlementRealtime()
		{
			if (string.IsNullOrWhiteSpace(Main._ParametersCache.addSettlementData)) return;

			try
			{
				Settlement settlement = (Settlement)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.Settlement);
				settlement.SetFaction(Main._ParametersCache.faction);
				settlement.Tile = int.Parse(Main._ParametersCache.addSettlementData.Split('│')[1]);
				settlement.Name = Main._ParametersCache.addSettlementData.Split('│')[2] + "'s Settlement";
				Find.WorldObjects.Add(settlement);

				Main._ParametersCache.onlineSettlements.Add(int.Parse(Main._ParametersCache.addSettlementData.Split('│')[1]), new List<string>() { Main._ParametersCache.addSettlementData.Split('│')[2] });

				Main._ParametersCache.addSettlementData = "";
			}

			catch { }
		}

		public void RemoveSettlementRealtime()
		{
			if (string.IsNullOrWhiteSpace(Main._ParametersCache.removeSettlementData)) return;

			try
			{
				List<Settlement> settlementList = Find.WorldObjects.Settlements;

				Settlement toDestroy = settlementList.Find(item => item.Tile == int.Parse(Main._ParametersCache.removeSettlementData.Split('│')[1]));

				if (toDestroy.Faction == Faction.OfPlayer)
                {
					Main._ParametersCache.removeSettlementData = "";
					return;
				}

				Find.WorldObjects.Remove(toDestroy);

				Main._ParametersCache.onlineSettlements.Remove(int.Parse(Main._ParametersCache.removeSettlementData.Split('│')[1]));

				Main._ParametersCache.removeSettlementData = "";
			}

			catch { }
		}

		public void FindOnlineFactionInWorld()
		{
			foreach (Faction f in Find.FactionManager.AllFactions)
			{
				if (f.Name == "Open World Settlements")
				{
					Main._ParametersCache.faction = f;
					break;
				}
			}
		}

		public void RenderPodInWorld(string data)
        {
			string originTile = data.Split('│')[2];
			string targetTile = data.Split('│')[3];

			//DoStuff
		}

        public void HandleSettlementsLocation()
        {
			List<Settlement> existingSettlements = new List<Settlement>();
			List<Settlement> serverSettlements = new List<Settlement>();

			//Get existing settlements
			foreach (Settlement st in Find.WorldObjects.Settlements)
			{
				if (st.Faction == Main._ParametersCache.faction)
				{
					Settlement dummySettlement = new Settlement();
					dummySettlement.Tile = st.Tile;
					dummySettlement.Name = st.Name;

					existingSettlements.Add(dummySettlement);
				}
			}

			//Get server settlements
			foreach (KeyValuePair<int, List<string>> pair in Main._ParametersCache.onlineSettlements)
			{
				Settlement dummySettlement = new Settlement();
				dummySettlement.Tile = pair.Key;
				dummySettlement.Name = pair.Value[0];

				serverSettlements.Add(dummySettlement);
			}

			//Remove old settlements
			foreach (Settlement settlement in existingSettlements)
			{
				Settlement dummySettlement = null;
				dummySettlement = serverSettlements.Find(fetch => fetch.Tile == settlement.Tile);

				if (dummySettlement != null)
				{
					Log.Message("Old settlement still valid");
					continue;
				}
				else
				{
					Find.WorldObjects.Remove(settlement);
					Log.Message("Removing old settlement");
				}
			}

			//Add new settlements
			foreach (Settlement settlement in serverSettlements)
			{
				Settlement dummySettlement = null;
				dummySettlement = existingSettlements.Find(fetch => fetch.Tile == settlement.Tile);

				if (dummySettlement != null)
				{
					Log.Message("New settlement already placed");
					continue;
				}
				else
				{
					Settlement newSettlement = (Settlement)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.Settlement);
					newSettlement.SetFaction(Main._ParametersCache.faction);
					newSettlement.Tile = settlement.Tile;
					newSettlement.Name = settlement.Name + "'s Settlement";
					Find.WorldObjects.Add(newSettlement);

					Log.Message("Adding new settlement");
				}
			}
		}

        public void HandleRoadGeneration()
        {
			if (Main._ParametersCache.roadMode != 0)
			{
				if (Main._ParametersCache.roadMode == 1)
				{
					List<WorldGenStepDef> GenStepsInOrder = DefDatabase<WorldGenStepDef>.AllDefs.ToList();
					WorldGenStepDef roadGenerator = GenStepsInOrder.Find(a => a.defName == "Roads");

					try { roadGenerator.worldGenStep.GenerateFresh(Find.World.info.seedString); }
					catch { }
				}

				else if (Main._ParametersCache.roadMode == 2)
				{
					List<WorldGenStepDef> GenStepsInOrder = DefDatabase<WorldGenStepDef>.AllDefs.ToList();
					WorldGenStepDef roadGenerator = GenStepsInOrder.Find(a => a.defName == "Roads");

					try { roadGenerator.worldGenStep.GenerateFromScribe(Find.World.info.seedString); }
					catch { }
				}
			}
		}
    }
}
