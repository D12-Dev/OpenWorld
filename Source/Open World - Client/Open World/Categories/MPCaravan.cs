using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace OpenWorld
{
    public static class MPCaravan
    {
		public static bool TakeFundsFromCaravan(int silverNeeded)
		{
			List<Thing> caravanSilver = CaravanInventoryUtility.AllInventoryItems(Main._ParametersCache.focusedCaravan)
				.Where((Thing x) => x.def == ThingDefOf.Silver).ToList();

			if (silverNeeded == 0) return true;

			int silverInCaravan = 0;
			foreach(Thing silverStack in caravanSilver) silverInCaravan += silverStack.stackCount;

			if (silverInCaravan < silverNeeded) return false;
			else
            {
				int takenSilver = 0;
				foreach(Thing silverStack in caravanSilver)
                {
					if (takenSilver + silverStack.stackCount > silverNeeded)
                    {
						silverStack.holdingOwner.Take(silverStack, silverNeeded - takenSilver);
						break;
					}

					else if (takenSilver + silverStack.stackCount < silverNeeded)
                    {
						silverStack.holdingOwner.Take(silverStack, silverStack.stackCount);
						takenSilver += silverStack.stackCount;
					}

					if (takenSilver == silverNeeded)
                    {
						break;
					}
                }

				Injections.thingsToDoInUpdate.Add(Main._MPGame.ForceSave);
				return true;
			}
		}

		public static void TakeItemsFromCaravan(int takeMode)
		{
			//Gift
			if (takeMode == 0)
			{
				Action action = delegate
				{
					Main._ParametersCache.transferMode = 0;

					if (TradeSession.deal.TryExecute(out bool actuallyTraded))
					{
						SoundDefOf.ExecuteTrade.PlayOneShotOnCamera();
						TradeSession.playerNegotiator.GetCaravan()?.RecacheImmobilizedNow();
						Main._ParametersCache.__MPGift.Close(doCloseSound: false);
					}
				};

				action();
			}

			//Trade
			else if (takeMode == 1)
			{
				Action action = delegate
				{
					Main._ParametersCache.transferMode = 1;

					if (TradeSession.deal.TryExecute(out bool actuallyTraded))
					{
						TradeSession.playerNegotiator.GetCaravan()?.RecacheImmobilizedNow();
						Main._ParametersCache.__MPTrade.Close(doCloseSound: false);
					}
				};

				action();
			}

			//Barter
			else if (takeMode == 2)
			{
				Action action = delegate
				{
					Main._ParametersCache.transferMode = 2;

					if (TradeSession.deal.TryExecute(out bool actuallyTraded))
					{
						TradeSession.playerNegotiator.GetCaravan()?.RecacheImmobilizedNow();
						Main._ParametersCache.__MPBarter.Close(doCloseSound: false);
					}
				};

				action();
			}

			//Silo Deposit
			else if (takeMode == 3)
			{
				Action action = delegate
				{
					Main._ParametersCache.transferMode = 3;

					if (TradeSession.deal.TryExecute(out bool actuallyTraded))
					{
						SoundDefOf.ExecuteTrade.PlayOneShotOnCamera();
						TradeSession.playerNegotiator.GetCaravan()?.RecacheImmobilizedNow();
						Main._ParametersCache.__MPSiloDeposit.Close(doCloseSound: false);
					}
				};

				action();
			}

			Event.current.Use();
		}

		public static void TakeItemsFromSettlement(int takeMode)
		{
			//Trade
			if (takeMode == 0)
			{
				Action action = delegate
				{
					Main._ParametersCache.transferMode = 1;

					if (TradeSession.deal.TryExecute(out bool actuallyTraded))
					{
						Main._ParametersCache.__MPTrade.Close(doCloseSound: false);
					}
				};

				action();
			}

			//Barter
			else if (takeMode == 1)
			{
				Action action = delegate
				{
					Main._ParametersCache.transferMode = 2;

					if (TradeSession.deal.TryExecute(out bool actuallyTraded))
					{
						Main._ParametersCache.__MPBarter.Close(doCloseSound: false);
					}
				};

				action();
			}

			Event.current.Use();
		}
	}
}