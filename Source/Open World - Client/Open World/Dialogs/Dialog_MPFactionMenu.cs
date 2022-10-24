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

        private string windowTitle = "Faction Menu";

        private float buttonX = 150f;
        private float buttonY = 38f;

        public Dialog_MPFactionMenu()
        {
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

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(centeredX - Text.CalcSize(windowTitle).x / 2, rect.y, Text.CalcSize(windowTitle).x, Text.CalcSize(windowTitle).y), windowTitle);

            Widgets.DrawLineHorizontal(rect.xMin, (rect.y + Text.CalcSize(windowTitle).y + 10), rect.width);

            HandleWindowContents(rect);
        }

        private void HandleWindowContents(Rect rect)
        {
            float centeredX = rect.width / 2;

            if (!Main._ParametersCache.hasFaction)
            {
                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2), rect.yMax - buttonY * 3 - 20), new Vector2(buttonX, buttonY)), "Create Faction"))
                {
                    this.Close();
                }

                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2), rect.yMax - buttonY * 2 - 10), new Vector2(buttonX, buttonY)), "Refresh"))
                {
                    Networking.SendData("FactionManagement│Refresh");
                }

                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2), rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "Close"))
                {
                    this.Close();
                }
            }

            else
            {

            }
        }
    }
}
