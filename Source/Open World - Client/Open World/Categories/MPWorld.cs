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

				Main._ParametersCache.onlineSettlements.Add(Main._ParametersCache.addSettlementData.Split('│')[1], new List<string>() { Main._ParametersCache.addSettlementData.Split('│')[2] });

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

				Main._ParametersCache.onlineSettlements.Remove(Main._ParametersCache.removeSettlementData.Split('│')[1]);

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
	}
}
