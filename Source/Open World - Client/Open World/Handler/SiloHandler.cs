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

			Thing thingToAdd = RimworldHandler.GetItemFromData(itemID, itemQuantity, itemQuality, itemMaterial, false);
			caravan.AddPawnOrItem(thingToAdd, false);
		}
	}
}
