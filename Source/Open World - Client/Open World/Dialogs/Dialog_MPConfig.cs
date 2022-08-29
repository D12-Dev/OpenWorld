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
    public class Dialog_MPConfig : Window
    {
        public override Vector2 InitialSize => new Vector2(350f, 310f);

        private float buttonX = 150f;
        private float buttonY = 38f;

        string windowTitle = "Multiplayer Parameters";

        public Dialog_MPConfig()
        {
            forcePause = true;
            absorbInputAroundWindow = true;
            closeOnAccept = false;
            closeOnCancel = true;
        }

        public override void DoWindowContents(Rect rect)
        {
            float centeredX = rect.width / 2;
            float horizontalLineDif = Text.CalcSize(windowTitle).y + (StandardMargin / 4) + 3f;
            float horizontalLineDif2 = rect.y + 225f;

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(centeredX - (Text.CalcSize(windowTitle).x / 2), rect.y, Text.CalcSize(windowTitle).x, Text.CalcSize(windowTitle).y), windowTitle);
            Widgets.DrawLineHorizontal(rect.x, horizontalLineDif, rect.width);

            Rect t1 = new Rect(new Vector2(rect.x, rect.y + 40f), new Vector2(InitialSize.x, 25f));
            Rect t2 = new Rect(new Vector2(rect.x, rect.y + 70f), new Vector2(InitialSize.x, 25f));
            Rect t3 = new Rect(new Vector2(rect.x, rect.y + 100f), new Vector2(InitialSize.x, 25f));
            Rect t4 = new Rect(new Vector2(rect.x, rect.y + 130f), new Vector2(InitialSize.x, 25f));
            Rect t5 = new Rect(new Vector2(rect.x, rect.y + 160f), new Vector2(InitialSize.x, 25f));
            Rect t6 = new Rect(new Vector2(rect.x, rect.y + 190f), new Vector2(InitialSize.x, 25f));

            Widgets.CheckboxLabeled(t1, "Auto Deny Trades", ref Main._ParametersCache.inTrade, placeCheckboxNearText: true);
            Widgets.CheckboxLabeled(t2, "Auto Deny Visits", ref Main._ParametersCache.visitFlag, placeCheckboxNearText: true);
            Widgets.CheckboxLabeled(t3, "Online Raids", ref Main._ParametersCache.pvpFlag, placeCheckboxNearText: true);
            Widgets.CheckboxLabeled(t4, "Offline Raids", ref Main._ParametersCache.offlinePvpFlag, placeCheckboxNearText: true);
            Widgets.CheckboxLabeled(t5, "Spy Warning", ref Main._ParametersCache.spyWarnFlag, placeCheckboxNearText: true);
            Widgets.CheckboxLabeled(t6, "Very Secret Stuff", ref Main._ParametersCache.secretFlag, placeCheckboxNearText: true);

            Widgets.DrawLineHorizontal(rect.x, horizontalLineDif2, rect.width);

            Rect buttonRect = new Rect(new Vector2(rect.x + (buttonX / 2), rect.y + 235f), new Vector2(buttonX, buttonY));
            if (Widgets.ButtonText(buttonRect, "Close")) Close();
        }
    }
}
