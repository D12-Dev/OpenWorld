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
    public class MPRTSE
    {
		public void SetupRaidParameters()
        {
            List<Pawn> pawnList = Main._ParametersCache.focusedCaravan.PawnsListForReading.FindAll(pawn => pawn.IsColonist);

            string pawnDataToSend = "";
            foreach (Pawn pawn in pawnList) Main._ParametersCache.playerPawnData.Add(pawn);

			pawnDataToSend = pawnList.Count().ToString();

            Main._Networking.SendData("Raid│TryRaid│" + Main._ParametersCache.focusedSettlement.Tile + "│" + pawnDataToSend);

			Find.WindowStack.Add(new Dialog_MPWaiting());
		}

		public void TryRaid()
		{
			int pawnCount = int.Parse(Main._ParametersCache.rtsGenerationData.Split('│')[2]);

			Main._ParametersCache.isInRTS = true;

			Main._Threading.GenerateThreads(2);

			Map mapToLoad = GetOrGenerateMapUtility.GetOrGenerateMap(Main._ParametersCache.focusedSettlement.Tile, null);
			CaravanEnterMapUtility.Enter(Main._ParametersCache.focusedCaravan, mapToLoad, CaravanEnterMode.Center, CaravanDropInventoryMode.DoNotDrop, draftColonists: true);
			IntVec3 positionToDrop = mapToLoad.Center;

			for (int i = 0; i < pawnCount; i++)
			{
				Pawn sujetoDePrueba = PawnGenerator.GeneratePawn(PawnKindDefOf.Colonist, Faction.OfPirates);
				Main._ParametersCache.enemyPawnData.Add(sujetoDePrueba);

				try { GenPlace.TryPlaceThing(sujetoDePrueba, positionToDrop, mapToLoad, ThingPlaceMode.Near); }
				catch { }

				sujetoDePrueba.ClearMind();
				sujetoDePrueba.mindState.Active = false;
			}

			Main._Networking.SendData("Raid│Ready│");

			Find.TickManager.CurTimeSpeed = TimeSpeed.Normal;
		}

		public void TryPrepareHostForRaid()
		{
			int pawnCount = int.Parse(Main._ParametersCache.rtsGenerationData.Split('│')[2]);

			Find.WindowStack.Add(new Dialog_MPWaiting());

			Main._ParametersCache.isInRTS = true;

			Main._Threading.GenerateThreads(2);

			Map playerMap = Find.AnyPlayerHomeMap;
			IntVec3 positionToDrop = playerMap.Center;

			List<Pawn> pawnList = playerMap.mapPawns.AllPawns.FindAll(pawn => pawn.IsColonist);
			foreach (Pawn pawn in pawnList) Main._ParametersCache.playerPawnData.Add(pawn);

			for (int i = 0; i < pawnCount; i++)
			{
				Pawn sujetoDePrueba = PawnGenerator.GeneratePawn(PawnKindDefOf.Colonist, Faction.OfPirates);
				Main._ParametersCache.enemyPawnData.Add(sujetoDePrueba);

				try { GenPlace.TryPlaceThing(sujetoDePrueba, positionToDrop, playerMap, ThingPlaceMode.Near); }
				catch { }

				sujetoDePrueba.ClearMind();
				sujetoDePrueba.mindState.Active = false;
			}

			Find.TickManager.CurTimeSpeed = TimeSpeed.Normal;
		}
	}
}
