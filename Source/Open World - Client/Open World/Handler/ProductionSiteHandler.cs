using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace OpenWorld
{
    public static class ProductionSiteHandler
    {
		public static void GetProductsToReceive(string data)
        {
			if (!Main._ParametersCache.hasLoadedCorrectly) return;

			string itemID = "";
			int itemQuantity = 0;
			int itemQuality = 0;
			int itemMultiplier = int.Parse(data.Split('│')[3]);

			if (Main._ParametersCache.productionSiteProduct == 0)
            {
				Random rnd = new Random();
				int randomNumber = rnd.Next(0, 4);

				if (randomNumber == 0)
				{
					itemID = "RawRice";
					itemQuantity = 30;
				}

				else if (randomNumber == 1)
				{
					itemID = "Silver";
					itemQuantity = 30;
				}

				else if (randomNumber == 2)
				{
					itemID = "ComponentIndustrial";
					itemQuantity = 1;
				}

				else if (randomNumber == 3)
				{
					itemID = "Chemfuel";
					itemQuantity = 10;
				}
			}

			else if (Main._ParametersCache.productionSiteProduct == 1)
			{
				itemID = "RawRice";
				itemQuantity = 50;
			}

			else if (Main._ParametersCache.productionSiteProduct == 2)
			{
				itemID = "Silver";
				itemQuantity = 50;
			}

			else if (Main._ParametersCache.productionSiteProduct == 3)
			{
				itemID = "ComponentIndustrial";
				itemQuantity = 2;
			}

			else if (Main._ParametersCache.productionSiteProduct == 4)
			{
				itemID = "Chemfuel";
				itemQuantity = 20;
			}

			itemQuantity *= itemMultiplier;

			Thing thingToAdd = RimworldHandler.GetItemFromData(itemID, itemQuantity, itemQuality, null, false);

			ReceiveProductionItems(thingToAdd);
		}

        public static void ReceiveProductionItems(Thing thingToAdd)
        {
			Map map = Find.AnyPlayerHomeMap;
			IntVec3 positionToDrop = RimworldHandler.GetMapPositionForDrop("Production Site");

			GenPlace.TryPlaceThing(thingToAdd, positionToDrop, map, ThingPlaceMode.Near);

			Main._ParametersCache.letterTitle = "Products Received";
			Main._ParametersCache.letterDescription = "You have products from the production site! \n\nIf you have a zone called 'Production Site' check it for the items, if not, search around the center of the map.";
			Main._ParametersCache.letterType = LetterDefOf.PositiveEvent;

			Injections.thingsToDoInUpdate.Add(RimworldHandler.GenerateLetter);

			Injections.thingsToDoInUpdate.Add(Main._MPGame.ForceSave);
		}
    }
}
