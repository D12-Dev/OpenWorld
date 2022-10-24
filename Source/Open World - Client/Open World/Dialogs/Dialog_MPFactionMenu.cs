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
    public class Dialog_MPFactionMenu : Window
    {
        public override Vector2 InitialSize => new Vector2(400f, 434f);

        private string windowTitle = "Black Market";
        private string windowDescription = "Change The Outcomes... For A Price...";

        private float buttonX = 150f;
        private float buttonY = 38f;

        public Dialog_MPFactionMenu()
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

            if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2), rect.yMax - buttonY * 3 - 20), new Vector2(buttonX, buttonY)), "Create Faction"))
            {
                this.Close();
            }

            if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2), rect.yMax - buttonY * 2 - 10), new Vector2(buttonX, buttonY)), "Refresh"))
            {
                this.Close();
            }

            if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2), rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "Close"))
            {
                this.Close();
            }
        }
    }
}
