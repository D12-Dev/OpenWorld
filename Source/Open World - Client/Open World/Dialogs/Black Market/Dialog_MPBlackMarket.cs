using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace OpenWorld
{
    public class Dialog_MPBlackMarket : Window
    {
        public override Vector2 InitialSize => new Vector2(400f, 434f);

        private string windowTitle = "Black Market";
        private string windowDescription = "Change The Outcomes... For A Price...";

        private float buttonX = 150f;
        private float buttonY = 38f;

        public Dialog_MPBlackMarket()
        {
            Main._ParametersCache.__MPBlackMarket = this;

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

            float windowDescriptionDif = Text.CalcSize(windowDescription).y + StandardMargin;

            float descriptionLineDif = windowDescriptionDif + Text.CalcSize(windowDescription).y;

            float firstButtonsDif = descriptionLineDif + StandardMargin;

            float secondButtonsDif = firstButtonsDif + buttonY + StandardMargin;

            float thirdButtonsDif = secondButtonsDif + buttonY + StandardMargin;

            float fourthButtonsDif = thirdButtonsDif + buttonY + StandardMargin;

            float fifthButtonsDif = fourthButtonsDif + buttonY + StandardMargin;

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(centeredX - Text.CalcSize(windowTitle).x / 2, rect.y, Text.CalcSize(windowTitle).x, Text.CalcSize(windowTitle).y), windowTitle);

            Text.Font = GameFont.Small;
            Widgets.Label(new Rect(centeredX - Text.CalcSize(windowDescription).x / 2, windowDescriptionDif, Text.CalcSize(windowDescription).x, Text.CalcSize(windowDescription).y), windowDescription);

            Widgets.DrawLineHorizontal(rect.x, descriptionLineDif, rect.width);

            if (Widgets.ButtonText(new Rect(new Vector2(rect.x + StandardMargin, firstButtonsDif), new Vector2(buttonX, buttonY)), "Raid"))
            {
                Main._ParametersCache.blackEventType = "Raid";

                Find.WindowStack.Add(new Dialog_MPConfirmEvent());
            }

            if (Widgets.ButtonText(new Rect(new Vector2(rect.xMax - buttonX - StandardMargin, firstButtonsDif), new Vector2(buttonX, buttonY)), "Infestation"))
            {
                Main._ParametersCache.blackEventType = "Infestation";

                Find.WindowStack.Add(new Dialog_MPConfirmEvent());
            }

            if (Widgets.ButtonText(new Rect(new Vector2(rect.x + StandardMargin, secondButtonsDif), new Vector2(buttonX, buttonY)), "Mech Cluster"))
            {
                Main._ParametersCache.blackEventType = "MechCluster";

                Find.WindowStack.Add(new Dialog_MPConfirmEvent());
            }

            if (Widgets.ButtonText(new Rect(new Vector2(rect.xMax - buttonX - StandardMargin, secondButtonsDif), new Vector2(buttonX, buttonY)), "Toxic Fallout"))
            {
                Main._ParametersCache.blackEventType = "ToxicFallout";

                Find.WindowStack.Add(new Dialog_MPConfirmEvent());
            }

            if (Widgets.ButtonText(new Rect(new Vector2(rect.x + StandardMargin, thirdButtonsDif), new Vector2(buttonX, buttonY)), "Manhunter Pack"))
            {
                Main._ParametersCache.blackEventType = "Manhunter";

                Find.WindowStack.Add(new Dialog_MPConfirmEvent());
            }

            if (Widgets.ButtonText(new Rect(new Vector2(rect.xMax - buttonX - StandardMargin, thirdButtonsDif), new Vector2(buttonX, buttonY)), "Wanderer Join"))
            {
                Main._ParametersCache.blackEventType = "Wanderer";

                Find.WindowStack.Add(new Dialog_MPConfirmEvent());
            }

            if (Widgets.ButtonText(new Rect(new Vector2(rect.x + StandardMargin, fourthButtonsDif), new Vector2(buttonX, buttonY)), "Farm Animals Join"))
            {
                Main._ParametersCache.blackEventType = "FarmAnimals";

                Find.WindowStack.Add(new Dialog_MPConfirmEvent());
            }

            if (Widgets.ButtonText(new Rect(new Vector2(rect.xMax - buttonX - StandardMargin, fourthButtonsDif), new Vector2(buttonX, buttonY)), "Ship Chunk Drop"))
            {
                Main._ParametersCache.blackEventType = "ShipChunk";

                Find.WindowStack.Add(new Dialog_MPConfirmEvent());
            }

            if (Widgets.ButtonText(new Rect(new Vector2(rect.x + StandardMargin, fifthButtonsDif), new Vector2(buttonX, buttonY)), "Give Random Quest"))
            {
                Main._ParametersCache.blackEventType = "GiveQuest";

                Find.WindowStack.Add(new Dialog_MPConfirmEvent());
            }

            if (Widgets.ButtonText(new Rect(new Vector2(rect.xMax - buttonX - StandardMargin, fifthButtonsDif), new Vector2(buttonX, buttonY)), "Trader Caravan"))
            {
                Main._ParametersCache.blackEventType = "TraderCaravan";

                Find.WindowStack.Add(new Dialog_MPConfirmEvent());
            }

            if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2), rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "Cancel"))
            {
                this.Close();
            }
        }
    }
}
