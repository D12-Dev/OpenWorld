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

        bool inRecruit;
        bool inRank;

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

            HandleWindowContents(rect);
        }

        private void HandleWindowContents(Rect rect)
        {
            float centeredX = rect.width / 2;

            if (inRecruit)
            {
                windowTitle = "Recruitment Options";

                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2) * 2, rect.yMax - buttonY * 3 - 20), new Vector2(buttonX * 2, buttonY)), "Recruit"))
                {
                    Find.WindowStack.Add(new Dialog_MPFactionRecruit(this));
                }

                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2) * 2, rect.yMax - buttonY * 2 - 10), new Vector2(buttonX * 2, buttonY)), "Kick"))
                {
                    Find.WindowStack.Add(new Dialog_MPFactionKick(this));
                }

                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2) * 1.5f, rect.yMax - buttonY), new Vector2(buttonX * 1.5f, buttonY)), "Back"))
                {
                    inRecruit = false;
                }
            }

            else if (inRank)
            {
                windowTitle = "Rank Options";

                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2) * 2, rect.yMax - buttonY * 3 - 20), new Vector2(buttonX * 2, buttonY)), "Promote"))
                {
                    Find.WindowStack.Add(new Dialog_MPFactionPromote(this));
                }

                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2) * 2, rect.yMax - buttonY * 2 - 10), new Vector2(buttonX * 2, buttonY)), "Demote"))
                {
                    Find.WindowStack.Add(new Dialog_MPFactionDemote(this));
                }

                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2) * 1.5f, rect.yMax - buttonY), new Vector2(buttonX * 1.5f, buttonY)), "Back"))
                {
                    inRank = false;
                }
            }

            else
            {
                windowTitle = "Faction Options";

                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2) * 2, rect.yMax - buttonY * 3 - 20), new Vector2(buttonX * 2, buttonY)), "Recruitment"))
                {
                    inRecruit = true;
                }

                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2) * 2, rect.yMax - buttonY * 2 - 10), new Vector2(buttonX * 2, buttonY)), "Rank"))
                {
                    inRank = true;
                }

                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2) * 1.5f, rect.yMax - buttonY), new Vector2(buttonX * 1.5f, buttonY)), "Close"))
                {
                    Close();
                }
            }
        }
    }
}
