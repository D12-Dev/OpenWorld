﻿using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace OpenWorld
{
    public class Dialog_MPFactionSiteBuild : Window
    {
        public override Vector2 InitialSize => new Vector2(350f, 150f);

        private string windowTitle = "Warning!";
        private string windowDescription = "Are you sure you want to build this site?";

        private float buttonX = 137f;
        private float buttonY = 38f;

        private int buildingType;
        private int silverCost;

        public Dialog_MPFactionSiteBuild(int buildingType, int silverCost)
        {
            soundAppear = SoundDefOf.CommsWindow_Open;
            soundClose = SoundDefOf.CommsWindow_Close;
            absorbInputAroundWindow = true;
            closeOnAccept = false;
            closeOnCancel = true;

            this.buildingType = buildingType;
            this.silverCost = silverCost;
        }

        public override void DoWindowContents(Rect rect)
        {
            float centeredX = rect.width / 2;

            float windowDescriptionDif = Text.CalcSize(windowDescription).y + StandardMargin;

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(centeredX - Text.CalcSize(windowTitle).x / 2, rect.y, Text.CalcSize(windowTitle).x, Text.CalcSize(windowTitle).y), windowTitle);

            Text.Font = GameFont.Small;
            Widgets.Label(new Rect(centeredX - Text.CalcSize(windowDescription).x / 2, windowDescriptionDif, Text.CalcSize(windowDescription).x, Text.CalcSize(windowDescription).y), windowDescription);

            if (Widgets.ButtonText(new Rect(new Vector2(rect.x, rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "Yes"))
            {
                Close();

                foreach (KeyValuePair<int, List<int>> pair in Main._ParametersCache.allFactionStructures)
                {
                    if (pair.Value[0] == buildingType && pair.Value[1] == 1)
                    {
                        Find.WindowStack.Add(new Dialog_MPFactionStructureLimit());
                        return;
                    }
                }

                if (!MPCaravan.TakeFundsFromCaravan(silverCost))
                {
                    Find.WindowStack.Add(new Dialog_MPNoFunds());
                    return;
                }

                Networking.SendData("FactionManagement│BuildStructure" + "│" + Main._ParametersCache.focusedTile + "│" + buildingType);
            }

            if (Widgets.ButtonText(new Rect(new Vector2(rect.xMax - buttonX, rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "No"))
            {
                Close();
            }
        }
    }
}