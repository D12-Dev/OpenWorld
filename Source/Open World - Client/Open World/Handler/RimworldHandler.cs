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
    public static class RimworldHandler
    {
		public static Thing GetItemFromData(string itemID, int itemQuantity, int itemQuality, string itemMaterial, bool minified)
		{
			Thing thing = ThingMaker.MakeThing(ThingDefOf.Silver);

			foreach (ThingDef item in DefDatabase<ThingDef>.AllDefs)
			{
				if (minified)
				{
					thing = ThingMaker.MakeThing(item);
					Thing miniThing = thing.TryMakeMinified();
					thing = miniThing;

					CompQuality compQuality = thing.TryGetComp<CompQuality>();
					if (compQuality != null)
					{
						QualityCategory q = (QualityCategory)itemQuality;
						compQuality.SetQuality(q, ArtGenerationContext.Colony);
					}

					foreach (ThingDef def in DefDatabase<ThingDef>.AllDefs)
					{
						if (def.defName == itemMaterial)
						{
							thing.SetStuffDirect(def);
						}
					}

					break;
				}
				else if (item.defName == itemID)
				{
					thing = ThingMaker.MakeThing(item);

					CompQuality compQuality = thing.TryGetComp<CompQuality>();
					if (compQuality != null)
					{
						QualityCategory q = (QualityCategory)itemQuality;
						compQuality.SetQuality(q, ArtGenerationContext.Colony);
					}

					foreach (ThingDef def in DefDatabase<ThingDef>.AllDefs)
					{
						if (def.defName == itemMaterial)
						{
							thing.SetStuffDirect(def);
						}
					}

					break;
				}
			}

			thing.stackCount = itemQuantity;

			return thing;
		}

		public static IntVec3 GetMapPositionForDrop(string zoneName)
        {
			Map map = Find.AnyPlayerHomeMap;
			IntVec3 positionToDrop = new IntVec3(map.Center.x, map.Center.y, map.Center.z);

			if (!string.IsNullOrWhiteSpace(zoneName))
            {
				try
				{
					Zone z = map.zoneManager.AllZones.Find(zone => zone.label == zoneName && zone.GetType() == typeof(Zone_Stockpile));
					positionToDrop = z.Position;
				}
				catch { }
			}

			return positionToDrop;
		}

		public static void GenerateLetter()
		{
			Find.LetterStack.ReceiveLetter(Main._ParametersCache.letterTitle, Main._ParametersCache.letterDescription, Main._ParametersCache.letterType);

			Main._ParametersCache.letterTitle = "";
			Main._ParametersCache.letterDescription = "";
			Main._ParametersCache.letterType = null;
		}

		public static bool CheckIfAnySocialPawn(int searchLocation)
        {
			if (searchLocation == 0)
            {
				Caravan caravan = Main._ParametersCache.focusedCaravan;
				Pawn playerNegotiator = caravan.PawnsListForReading.Find(fetch => fetch.IsColonist && !fetch.skills.skills[10].PermanentlyDisabled);
				if (playerNegotiator == null) return false;
				else return true;
			}

			else if (searchLocation == 1)
            {
				Map map = Find.AnyPlayerHomeMap;
				Pawn playerNegotiator = map.mapPawns.AllPawns.Find(fetch => fetch.IsColonist && !fetch.skills.skills[10].PermanentlyDisabled);
				if (playerNegotiator == null) return false;
				else return true;
			}

			return false;
        }
	}
}
