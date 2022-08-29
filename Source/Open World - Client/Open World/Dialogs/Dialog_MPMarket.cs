using RimWorld;
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
    public class Dialog_MPMarket : Window
    {
        public override Vector2 InitialSize => new Vector2(650f, 627f);

        private Vector2 scrollPosition = Vector2.zero;

        private string windowTitle = "Market Menu";
        private string buyingLine = "Want To Buy";
        private string sellingLine = "Want To Sell";
        private string offeringLine = "Your Trades";

        private float buttonX = 150f;
        private float buttonY = 38f;

        private float horizontalBarLenght = 100f;

        private int pageIndex = 0;

        public Dialog_MPMarket()
        {
            Main._ParametersCache.__MPMarket = this;

            soundAppear = SoundDefOf.CommsWindow_Open;
            soundClose = SoundDefOf.CommsWindow_Close;
            absorbInputAroundWindow = true;
            forcePause = false;
            closeOnAccept = false;
            closeOnCancel = true;
        }

        public override void DoWindowContents(Rect rect)
        {
            float centeredX = rect.width / 2;

            float horizontalLineDif = buttonY + (StandardMargin / 2);
            float listingDif = horizontalLineDif + StandardMargin;

            Text.Font = GameFont.Medium;

            if (Widgets.ButtonText(new Rect(new Vector2(rect.x, rect.y), new Vector2(buttonX, buttonY)), "<"))
            {
                if (pageIndex > 0)
                {
                    pageIndex--;
                    SoundDefOf.Click.PlayOneShotOnCamera();
                }

                else pageIndex = 0;
            }

            if (Widgets.ButtonText(new Rect(new Vector2(rect.xMax - buttonX, rect.y), new Vector2(buttonX, buttonY)), ">"))
            {
                if (pageIndex < 2)
                {
                    pageIndex++;
                    SoundDefOf.Click.PlayOneShotOnCamera();
                }

                else pageIndex = 2;
            }

            Widgets.Label(new Rect(centeredX - (Text.CalcSize(windowTitle).x / 2), rect.y + 16, Text.CalcSize(windowTitle).x, Text.CalcSize(windowTitle).y), windowTitle);

            Widgets.DrawLineHorizontal(rect.x, horizontalLineDif, rect.width);

            if (pageIndex == 0)
            {
                Widgets.DrawLineHorizontal(rect.x + horizontalBarLenght, listingDif + (Text.CalcSize(buyingLine).y / 2), horizontalBarLenght);
                Widgets.Label(new Rect(centeredX - (Text.CalcSize(buyingLine).x / 2), listingDif, Text.CalcSize(buyingLine).x, Text.CalcSize(buyingLine).y), buyingLine);
                Widgets.DrawLineHorizontal(rect.xMax - horizontalBarLenght * 2, listingDif + (Text.CalcSize(buyingLine).y / 2), horizontalBarLenght);

                //Stuff

                Widgets.DrawHighlight(new Rect(new Vector2(rect.x, listingDif), new Vector2(rect.width, rect.yMax - listingDif - buttonY - StandardMargin)));
                FillMainRect(new Rect(new Vector2(rect.x, listingDif), new Vector2(rect.width, rect.yMax - listingDif - buttonY - StandardMargin)));
            }

            else if (pageIndex == 1)
            {
                Widgets.DrawLineHorizontal(rect.x + horizontalBarLenght, listingDif + (Text.CalcSize(sellingLine).y / 2), horizontalBarLenght);
                Widgets.Label(new Rect(centeredX - (Text.CalcSize(buyingLine).x / 2), listingDif, Text.CalcSize(sellingLine).x, Text.CalcSize(sellingLine).y), sellingLine);
                Widgets.DrawLineHorizontal(rect.xMax - horizontalBarLenght * 2, listingDif + (Text.CalcSize(sellingLine).y / 2), horizontalBarLenght);

                //Stuff

                Widgets.DrawHighlight(new Rect(new Vector2(rect.x, listingDif), new Vector2(rect.width, rect.yMax - listingDif - buttonY - StandardMargin)));
                FillMainRect(new Rect(new Vector2(rect.x, listingDif), new Vector2(rect.width, rect.yMax - listingDif - buttonY - StandardMargin)));
            }

            else if (pageIndex == 2)
            {
                Widgets.DrawLineHorizontal(rect.x + horizontalBarLenght, listingDif + (Text.CalcSize(offeringLine).y / 2), horizontalBarLenght);
                Widgets.Label(new Rect(centeredX - (Text.CalcSize(buyingLine).x / 2), listingDif, Text.CalcSize(offeringLine).x, Text.CalcSize(offeringLine).y), offeringLine);
                Widgets.DrawLineHorizontal(rect.xMax - horizontalBarLenght * 2, listingDif + (Text.CalcSize(offeringLine).y / 2), horizontalBarLenght);

                //Stuff

                Widgets.DrawHighlight(new Rect(new Vector2(rect.x, listingDif), new Vector2(rect.width, rect.yMax - listingDif - buttonY - StandardMargin)));
                FillMainRect(new Rect(new Vector2(rect.x, listingDif), new Vector2(rect.width, rect.yMax - listingDif - buttonY - StandardMargin)));
            }

            if (Widgets.ButtonText(new Rect(new Vector2(rect.x, rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "New Offer"))
            {
                Find.WindowStack.Add(new Dialog_MPAddListing());
            }

            if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2), rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "Refresh"))
            {
                SoundDefOf.Click.PlayOneShotOnCamera();
            }

            if (Widgets.ButtonText(new Rect(new Vector2(rect.xMax - buttonX, rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "Close"))
            {
                Close();
            }
        }

        private void FillMainRect(Rect mainRect)
        {
            float height = 6f + (float)Main._ParametersCache.yourListings.Count * 30f;
            Rect viewRect = new Rect(0f, 0f, mainRect.width - 16f, height);
            Widgets.BeginScrollView(mainRect, ref scrollPosition, viewRect);
            float num = 0;
            float num2 = scrollPosition.y - 30f;
            float num3 = scrollPosition.y + mainRect.height;
            int num4 = 0;

            for (int i = 0; i < Main._ParametersCache.yourListings.Count; i++)
            {
                if (num > num2 && num < num3)
                {
                    Rect rect = new Rect(0f, num, viewRect.width, 30f);
                    if (pageIndex == 0) ;
                    else if (pageIndex == 1) ;
                    else if (pageIndex == 2) DrawCustomRow(rect, Main._ParametersCache.yourListings[i], num4);
                }

                num += 30f;
                num4++;
            }

            Widgets.EndScrollView();
        }

        private void DrawCustomRow(Rect rect, List<string> details, int index)
        {
            Text.Font = GameFont.Small;

            Widgets.Label(rect, details[0]);

            Text.Font = GameFont.Medium;

            Widgets.DrawLightHighlight(rect);
        }
    }
}
