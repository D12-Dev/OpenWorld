using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Profile;

namespace OpenWorld
{
    public class Dialog_MPSpy : Window
    {
        public override Vector2 InitialSize => new Vector2(450f, 300f);

        private string windowTitle = "Spy Menu";
        private string text1 = "Spy other player's settlements from here.";
        private string text2 = "Using this action will gather information about current settlement.";
        private string text3 = "The standard cost of hiring a spy is around 150 silver.";
        private string text4 = "Beware! There is a chance your actions will go noticed!";
        private string text5 = "Would you like to proceed anyways?";

        private float buttonX = 150f;
        private float buttonY = 38f;

        public Dialog_MPSpy()
        {
            soundAppear = SoundDefOf.CommsWindow_Open;
            soundClose = SoundDefOf.CommsWindow_Close;
            absorbInputAroundWindow = true;
            forcePause = true;
            closeOnAccept = false;
            closeOnCancel = true;
        }

        public override void DoWindowContents(Rect rect)
        {
            float centeredX = rect.width / 2;

            float horizontalLineDif = Text.CalcSize(windowTitle).y + 3f;
            float text1Dif = Text.CalcSize(text1).y + (StandardMargin / 2);
            float text2Dif = text1Dif + Text.CalcSize(text1).y + StandardMargin;
            float text3Dif = text2Dif + Text.CalcSize(text2).y + StandardMargin;
            float text4Dif = text3Dif + Text.CalcSize(text3).y + StandardMargin;
            float text5Dif = text4Dif + Text.CalcSize(text4).y + StandardMargin;

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(centeredX - Text.CalcSize(windowTitle).x / 2, rect.y, Text.CalcSize(windowTitle).x, Text.CalcSize(windowTitle).y), windowTitle);

            Widgets.DrawLineHorizontal(rect.x, horizontalLineDif, rect.width);

            Text.Font = GameFont.Small;
            Widgets.Label(new Rect(centeredX - Text.CalcSize(text1).x / 2, text1Dif, Text.CalcSize(text1).x, Text.CalcSize(text1).y), text1);
            Widgets.Label(new Rect(centeredX - Text.CalcSize(text2).x / 2, text2Dif, Text.CalcSize(text2).x, Text.CalcSize(text2).y), text2);
            Widgets.Label(new Rect(centeredX - Text.CalcSize(text3).x / 2, text3Dif, Text.CalcSize(text3).x, Text.CalcSize(text3).y), text3);
            Widgets.Label(new Rect(centeredX - Text.CalcSize(text4).x / 2, text4Dif, Text.CalcSize(text4).x, Text.CalcSize(text4).y), text4);
            Widgets.Label(new Rect(centeredX - Text.CalcSize(text5).x / 2, text5Dif, Text.CalcSize(text5).x, Text.CalcSize(text5).y), text5);

            if (Widgets.ButtonText(new Rect(new Vector2(rect.x, rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "Accept"))
            {
                if (CaravanInventoryUtility.HasThings(Main._ParametersCache.focusedCaravan, ThingDefOf.Silver, 150))
                {
                    Networking.SendData("GetSpyInfo│" + Main._ParametersCache.focusedSettlement.Tile);
                    Close();
                }

                else
                {
                    Find.WindowStack.Add(new OW_ErrorDialog("You don't have enough funds for this action"));
                    Close();
                }
            }

            if (Widgets.ButtonText(new Rect(new Vector2(rect.xMax - buttonX, rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "Cancel"))
            {
                Close();
            }
        }
    }
}
