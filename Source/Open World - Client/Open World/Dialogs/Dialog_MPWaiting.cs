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
    public class Dialog_MPWaiting : Window
    {
        public override Vector2 InitialSize => new Vector2(125f, 75f);

        private string windowTitle = "Waiting...";

        public Dialog_MPWaiting()
        {
            Main._ParametersCache.__MPWaiting = this;

            soundAppear = SoundDefOf.CommsWindow_Open;
            soundClose = SoundDefOf.CommsWindow_Close;

            forcePause = true;
            absorbInputAroundWindow = true;
            closeOnAccept = false;
            closeOnCancel = false;
        }

        public override void DoWindowContents(Rect rect)
        {
            float centeredX = rect.width / 2;
            float centeredY = rect.height / 2;

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(centeredX - (Text.CalcSize(windowTitle).x / 2), centeredY - (Text.CalcSize(windowTitle).y / 2), Text.CalcSize(windowTitle).x, Text.CalcSize(windowTitle).y), windowTitle);
        }
    }
}
