using RimWorld;
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
    public class Dialog_MPFactionOnPlayer : Window
    {
        public override Vector2 InitialSize => new Vector2(325f, 225f);

        private string windowTitle = "Faction Options";

        private float buttonX = 150f;
        private float buttonY = 38f;

        public Dialog_MPFactionOnPlayer()
        {
            soundAppear = SoundDefOf.CommsWindow_Open;
            soundClose = SoundDefOf.CommsWindow_Close;
            absorbInputAroundWindow = true;
            forcePause = false;
            closeOnAccept = true;
            closeOnCancel = true;
        }

        public override void DoWindowContents(Rect rect)
        {
            float centeredX = rect.width / 2;

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(centeredX - Text.CalcSize(windowTitle).x / 2, rect.y, Text.CalcSize(windowTitle).x, Text.CalcSize(windowTitle).y), windowTitle);

            Widgets.DrawLineHorizontal(rect.xMin, (rect.y + Text.CalcSize(windowTitle).y + 10), rect.width);

            if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2), rect.yMax - buttonY * 3 - 20), new Vector2(buttonX, buttonY)), "Recruit"))
            {
                Find.WindowStack.Add(new Dialog_MPFactionRecruit(this));
            }

            if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2), rect.yMax - buttonY * 2 - 10), new Vector2(buttonX, buttonY)), "Kick"))
            {
                Find.WindowStack.Add(new Dialog_MPFactionKick(this));
            }

            if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2), rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "Close"))
            {
                this.Close();
            }
        }
    }
}
