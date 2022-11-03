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
    public static class TradeHandler
    {
		public static void ResetTradeVariables()
        {
			Main._ParametersCache.tradedItemString = "";

			Main._ParametersCache.wantedSilver = "";

			Main._ParametersCache.listToShowInTradeMenu.Clear();

			Main._ParametersCache.inTrade = false;
		}

		public static void GiveTradeFundsToCaravan()
		{
			Caravan caravan = Main._ParametersCache.focusedCaravan;

			Thing thing = ThingMaker.MakeThing(ThingDefOf.Silver);
			thing.stackCount = int.Parse(Main._ParametersCache.wantedSilver);

			caravan.AddPawnOrItem(thing, false);

			Injections.thingsToDoInUpdate.Add(Main._MPGame.ForceSave);

			ResetTradeVariables();
		}

		public static void ReturnTradesToCaravan()
		{
			Caravan caravan = Main._ParametersCache.focusedCaravan;

			if (caravan == null) return;

			string tradedString = Main._ParametersCache.tradedItemString;
			string[] receivedItems = new string[0];
			if (tradedString.Contains('»')) receivedItems = tradedString.Split('»').ToArray();
			else receivedItems = new string[1] { tradedString };

			foreach (string str in receivedItems)
			{
				string itemDefName = str.Split('┼')[0];
				int itemCount = int.Parse(str.Split('┼')[1]);
				QualityCategory qc = (QualityCategory)int.Parse(str.Split('┼')[1]);

				Thing thing = ThingMaker.MakeThing(ThingDefOf.Silver);

				foreach (ThingDef item in DefDatabase<ThingDef>.AllDefs)
				{
					if (item.defName == itemDefName)
					{
						thing = ThingMaker.MakeThing(item);
						thing.stackCount = itemCount;
						break;
					}
				}

				caravan.AddPawnOrItem(thing, false);
			}

			ResetTradeVariables();
		}

		public static void SendTradesToSettlement()
		{
			if (Main._ParametersCache.tradedItemString.Count() > 0) Main._ParametersCache.tradedItemString = Main._ParametersCache.tradedItemString.Remove(Main._ParametersCache.tradedItemString.Count() - 1, 1);
			else return;

			string tradedString = "SendTradeTo│" + Main._ParametersCache.focusedSettlement.Tile + "│" + Main._ParametersCache.tradedItemString + "│" + Main._ParametersCache.wantedSilver;

			Networking.SendData(tradedString);
		}

		public static void ReceiveTradesFromPlayer(string[] items)
		{
			Map map = Find.AnyPlayerHomeMap;
			IntVec3 positionToDrop = new IntVec3(map.Center.x, map.Center.y, map.Center.z);

			try
			{
				Zone z = map.zoneManager.AllZones.Find(zone => zone.label == "Trading" && zone.GetType() == typeof(Zone_Stockpile));
				positionToDrop = z.Position;
			}
			catch { }

			foreach (string str in items)
			{
				string itemDefName = str.Split('┼')[0];

				if (itemDefName == "pawn")
				{
					Pawn newPawn = PawnGenerator.GeneratePawn(PawnKindDefOf.Colonist, Faction.OfPlayer);

					newPawn.ageTracker.AgeBiologicalTicks = int.Parse(str.Split('┼')[2]);
					newPawn.ageTracker.AgeChronologicalTicks = int.Parse(str.Split('┼')[3]);
					newPawn.gender = (Gender)int.Parse(str.Split('┼')[4]);

					foreach (var bs in DefDatabase<BackstoryDef>.AllDefs)
					{
						if (bs.defName == str.Split('‼')[1].Split('┼')[1])
						{
							newPawn.story.Childhood = bs;
							break;
						}
					}

					try
					{
						foreach (var bs in DefDatabase<BackstoryDef>.AllDefs)
						{
							if (bs.defName == str.Split('‼')[1].Split('┼')[1])
							{
								newPawn.story.Adulthood = bs;
								break;
							}
						}
					}
					catch { newPawn.story.Adulthood = null; }

					List<string> skillList = str.Split('‼')[2].Split('┼').ToList();
					for (int i = 0; i < 12; i++)
					{
						int level = int.Parse(skillList[i].Split('-')[0]);
						int passion = int.Parse(skillList[i].Split('-')[1]);

						newPawn.skills.skills[i].levelInt = level;
						newPawn.skills.skills[i].passion = (Passion)passion;
					}

					List<string> traitList = str.Split('‼')[3].Split('┼').ToList();
					newPawn.story.traits.allTraits.Clear();
					foreach (string traitS in traitList)
					{
						foreach (TraitDef def in DefDatabase<TraitDef>.AllDefs)
						{
							Trait trait = new Trait(def);

							if (trait.Label == traitS)
							{
								newPawn.story.traits.GainTrait(trait);
								break;
							}
						}
					}

					try { GenPlace.TryPlaceThing(newPawn, positionToDrop, map, ThingPlaceMode.Near); }
					catch { }
				}

				else
				{
					int itemCount = int.Parse(str.Split('┼')[1]);
					int itemQuality = int.Parse(str.Split('┼')[2]);
					string madeFromString = str.Split('┼')[3];

					Thing thing = ThingMaker.MakeThing(ThingDefOf.Silver);

					foreach (ThingDef item in DefDatabase<ThingDef>.AllDefs)
					{
						if (itemDefName.Contains("minified-") && item.defName == itemDefName.Replace("minified-", ""))
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
								if (def.defName == madeFromString)
								{
									thing.SetStuffDirect(def);
								}
							}

							break;
						}

						else if (item.defName == itemDefName)
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
								if (def.defName == madeFromString)
								{
									thing.SetStuffDirect(def);
								}
							}

							break;
						}
					}

					thing.stackCount = itemCount;

					try { GenPlace.TryPlaceThing(thing, positionToDrop, map, ThingPlaceMode.Near); }
					catch { }
				}
			}

			Main._ParametersCache.letterTitle = "Trade Received";
			Main._ParametersCache.letterDescription = "You have received a trade from another player! \n\nIf you have a zone called 'Trading' check it for the items, if not, search around the center of the map. \n\nTake a look at your objects! Some might have been swapped out by thieves!";
			Main._ParametersCache.letterType = LetterDefOf.PositiveEvent;

			ResetTradeVariables();

			Injections.thingsToDoInUpdate.Add(RimworldHandler.GenerateLetter);

			Injections.thingsToDoInUpdate.Add(Main._MPGame.ForceSave);
		}
	}
}
