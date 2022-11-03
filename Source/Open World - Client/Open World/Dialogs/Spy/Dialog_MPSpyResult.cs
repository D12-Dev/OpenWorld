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
    public class Dialog_MPSpyResult : Window
    {
        public override Vector2 InitialSize => new Vector2(300f, 338f);

        private string windowTitle = "Spy Results";
        private string text1 = "Settlement pawn count: ";
        private string text2 = "Settlement wealth: ";
        private string text3 = "Can be black marketed: ";
        private string text4 = "Can engage in PvP: ";
        private string text5 = "Gifts can be stolen: ";
        private string text6 = "Trades can be stolen: ";

        private float buttonX = 150f;
        private float buttonY = 38f;

        string pawnCount = "0";
        string wealthValue = "0";
        string eventShielded = "No";
        string inPvP = "No";
        string giftsInside = "No";
        string tradesInside = "No";

        public Dialog_MPSpyResult(string dataToFetch)
        {
            soundAppear = SoundDefOf.CommsWindow_Open;
            soundClose = SoundDefOf.CommsWindow_Close;
            absorbInputAroundWindow = true;
            forcePause = true;
            closeOnAccept = false;
            closeOnCancel = false;

            string[] data = dataToFetch.Split('»');

            pawnCount = data[0];
            wealthValue = data[1];
            if (data[2] == "False") eventShielded = "Yes";
            if (data[3] == "False") inPvP = "Yes";
            if (data[4] == "True") giftsInside = "Yes";
            if (data[5] == "True") tradesInside = "Yes";

            text1 += pawnCount;
            text2 += wealthValue + " silver";
            text3 += eventShielded;
            text4 += inPvP;
            text5 += giftsInside;
            text6 += tradesInside;
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
            float text6Dif = text5Dif + Text.CalcSize(text5).y + StandardMargin;

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(centeredX - Text.CalcSize(windowTitle).x / 2, rect.y, Text.CalcSize(windowTitle).x, Text.CalcSize(windowTitle).y), windowTitle);

            Widgets.DrawLineHorizontal(rect.x, horizontalLineDif, rect.width);

            Text.Font = GameFont.Small;
            Widgets.Label(new Rect(centeredX - Text.CalcSize(text1).x / 2, text1Dif, Text.CalcSize(text1).x, Text.CalcSize(text1).y), text1);
            Widgets.Label(new Rect(centeredX - Text.CalcSize(text2).x / 2, text2Dif, Text.CalcSize(text2).x, Text.CalcSize(text2).y), text2);
            Widgets.Label(new Rect(centeredX - Text.CalcSize(text3).x / 2, text3Dif, Text.CalcSize(text3).x, Text.CalcSize(text3).y), text3);
            Widgets.Label(new Rect(centeredX - Text.CalcSize(text4).x / 2, text4Dif, Text.CalcSize(text4).x, Text.CalcSize(text4).y), text4);
            Widgets.Label(new Rect(centeredX - Text.CalcSize(text5).x / 2, text5Dif, Text.CalcSize(text5).x, Text.CalcSize(text5).y), text5);
            Widgets.Label(new Rect(centeredX - Text.CalcSize(text6).x / 2, text6Dif, Text.CalcSize(text6).x, Text.CalcSize(text6).y), text6);

            if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2), rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "Close"))
            {
                Close();
            }
        }
    }
}
