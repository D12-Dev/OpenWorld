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
    public class Dialog_MPFactionSiteBuilt : Window
    {
        public override Vector2 InitialSize => new Vector2(500f, 310f);

        private string windowTitle = "";
        private string windowDescription = "";

        private float buttonX = 150f;
        private float buttonY = 38f;

        bool isUsing = false;
        int siteType = 0;

        public Dialog_MPFactionSiteBuilt(int siteType)
        {
            soundAppear = SoundDefOf.CommsWindow_Open;
            soundClose = SoundDefOf.CommsWindow_Close;
            absorbInputAroundWindow = true;
            forcePause = false;
            closeOnAccept = false;
            closeOnCancel = true;

            this.siteType = siteType;
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

            float windowDescriptionDif = Text.CalcSize(windowDescription).y + StandardMargin;

            float descriptionLineDif = windowDescriptionDif + Text.CalcSize(windowDescription).y;

            if (siteType == 0)
            {
                windowTitle = "Silo Management Menu";
                windowDescription = "Manage the resources available for all faction members";
            }

            else if (siteType == 1)
            {
                windowTitle = "Marketplace Management Menu";
                windowDescription = "Trade over unlimited distances with all faction members";
            }

            else if (siteType == 2)
            {
                windowTitle = "Production Site Management Menu";
                windowDescription = "Generate resources over tile for all members";
            }

            if (isUsing)
            {
                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2) * 1.5f, rect.yMax - buttonY), new Vector2(buttonX * 1.5f, buttonY)), "Back"))
                {
                    isUsing = false;
                }

                Text.Font = GameFont.Small;
                Widgets.Label(new Rect(centeredX - Text.CalcSize(windowDescription).x / 2, windowDescriptionDif, Text.CalcSize(windowDescription).x, Text.CalcSize(windowDescription).y), windowDescription);
                Text.Font = GameFont.Medium;

                Widgets.DrawLineHorizontal(rect.x, descriptionLineDif, rect.width);
            }

            else
            {
                Text.Font = GameFont.Small;
                Widgets.Label(new Rect(centeredX - Text.CalcSize(windowDescription).x / 2, windowDescriptionDif, Text.CalcSize(windowDescription).x, Text.CalcSize(windowDescription).y), windowDescription);
                Text.Font = GameFont.Medium;

                Widgets.DrawLineHorizontal(rect.x, descriptionLineDif, rect.width);

                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2) * 2, rect.yMax - buttonY * 2 - 10), new Vector2(buttonX * 2, buttonY)), "Utilize Site"))
                {
                    isUsing = true;
                }

                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2) * 1.5f, rect.yMax - buttonY), new Vector2(buttonX * 1.5f, buttonY)), "Close"))
                {
                    Close();
                }
            }
        }
    }
}
