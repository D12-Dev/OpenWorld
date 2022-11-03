using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace OpenWorld
{
    public class Dialog_MPConfirmEvent : Window
    {
        public override Vector2 InitialSize => new Vector2(350f, 150f);

        private string windowTitle = "Warning!";
        private string windowDescription = "Using the market costs 2500 silver. Continue?";

        private float buttonX = 137f;
        private float buttonY = 38f;

        public Dialog_MPConfirmEvent()
        {
            soundAppear = SoundDefOf.CommsWindow_Open;
            soundClose = SoundDefOf.CommsWindow_Close;
            absorbInputAroundWindow = true;
            closeOnAccept = false;
            closeOnCancel = true;
        }

        public override void DoWindowContents(Rect rect)
        {
            float centeredX = rect.width / 2;

            float windowDescriptionDif = Text.CalcSize(windowDescription).y + StandardMargin;

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(centeredX - Text.CalcSize(windowTitle).x / 2, rect.y, Text.CalcSize(windowTitle).x, Text.CalcSize(windowTitle).y), windowTitle);

            Text.Font = GameFont.Small;
            Widgets.Label(new Rect(centeredX - Text.CalcSize(windowDescription).x / 2, windowDescriptionDif, Text.CalcSize(windowDescription).x, Text.CalcSize(windowDescription).y), windowDescription);

            if (Widgets.ButtonText(new Rect(new Vector2(rect.x, rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "OK"))
            {
                if (CaravanInventoryUtility.HasThings(Main._ParametersCache.focusedCaravan, ThingDefOf.Silver, 2500))
                {
                    Networking.SendData("ForceEvent│" + Main._ParametersCache.blackEventType + "│" + Main._ParametersCache.focusedSettlement.Tile);
                    Main._ParametersCache.blackEventType = "";
                    Close();
                }

                else
                {
                    Main._ParametersCache.blackEventType = "";
                    Close();
                    Main._ParametersCache.__MPBlackMarket.Close();
                    Find.WindowStack.Add(new OW_ErrorDialog("You don't have enough funds for this action"));
                }
            }

            if (Widgets.ButtonText(new Rect(new Vector2(rect.xMax - buttonX, rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "Cancel"))
            {
                Main._ParametersCache.blackEventType = "";

                this.Close();
            }
        }
    }
}
