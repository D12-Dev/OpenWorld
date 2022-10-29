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
	public static class SiloHandler
	{
		public static void ResetSiloVariables()
		{
			Main._ParametersCache.depositItemsString = "";

			Main._ParametersCache.listToShowInSiloMenu.Clear();
		}

		public static void SendMaterialsToSilo()
		{
			if (Main._ParametersCache.depositItemsString.Count() == 0) return;

			string depositedString = "FactionManagement│Silo│Deposit" + "│" + Main._ParametersCache.focusedCaravan.Tile + "│" + Main._ParametersCache.depositItemsString;

			Networking.SendData(depositedString);

			ResetSiloVariables();
		}

		public static void ReceiveMaterialFromSilo(string itemID, int itemQuantity, int itemQuality, string itemMaterial)
		{
			Caravan caravan = Main._ParametersCache.focusedCaravan;

			Thing thingToAdd = GetItemFromData(itemID, itemQuantity, itemQuality, itemMaterial, false);
			caravan.AddPawnOrItem(thingToAdd, false);
		}

		//Not working right now, but really useful if fixed
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
	}
}
