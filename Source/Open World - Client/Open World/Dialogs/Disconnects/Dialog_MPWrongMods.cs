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
    public class Dialog_MPWrongMods : Window
    {
        public override Vector2 InitialSize => new Vector2(400f, 299f);
        private Vector2 scrollPosition = Vector2.zero;

        private string windowTitle = "Error!";
        private string windowDescription = "Mods are [D]isallowed or [M]issing";

        private float buttonX = 150f;
        private float buttonY = 38f;

        string[] flaggedMods;
        string[] loadedMods = LoadedModManager.RunningMods.Select((ModContentPack mod) => mod.Name).ToArray();

        public Dialog_MPWrongMods(string[] flaggedMods)
        {
            this.flaggedMods = flaggedMods;

            soundAppear = SoundDefOf.CommsWindow_Open;
            soundClose = SoundDefOf.CommsWindow_Close;
            absorbInputAroundWindow = true;
            closeOnAccept = false;
            closeOnCancel = true;
        }

        public override void DoWindowContents(Rect rect)
        {
            float centeredX = rect.width / 2;
            float windowDescriptionDif = Text.CalcSize(windowDescription).y + (StandardMargin / 2);
            float listingDif = windowDescriptionDif + StandardMargin + 6f;

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(centeredX - Text.CalcSize(windowTitle).x / 2, rect.y, Text.CalcSize(windowTitle).x, Text.CalcSize(windowTitle).y), windowTitle);

            Text.Font = GameFont.Small;
            Widgets.Label(new Rect(centeredX - Text.CalcSize(windowDescription).x / 2, windowDescriptionDif, Text.CalcSize(windowDescription).x, Text.CalcSize(windowDescription).y), windowDescription);

            FillMainRect(new Rect(new Vector2(rect.x, listingDif), new Vector2(rect.width, rect.yMax - listingDif - buttonY - StandardMargin)));

            if (Widgets.ButtonText(new Rect(new Vector2(centeredX - buttonX / 2, rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "OK"))
            {
                Close();
            }
        }

        private void FillMainRect(Rect mainRect)
        {
            float objectSize = 23f;
            float height = 6f + flaggedMods.Count() * objectSize;
            Rect viewRect = new Rect(0f, 0f, mainRect.width - 16f, height);
            Widgets.BeginScrollView(mainRect, ref scrollPosition, viewRect);
            float num = 0;
            float num2 = scrollPosition.y - objectSize;
            float num3 = scrollPosition.y + mainRect.height;
            int num4 = 0;

            for (int i = 0; i < flaggedMods.Count(); i++)
            {
                if (num > num2 && num < num3)
                {
                    Rect rect = new Rect(0f, num, viewRect.width, objectSize);
                    DrawCustomRow(rect, flaggedMods[i], num4);
                }

                num += objectSize;
                num4++;
            }

            Widgets.EndScrollView();
        }

        private void DrawCustomRow(Rect rect, string details, int index)
        {
            if (index % 2 == 0) Widgets.DrawHighlight(rect);

            Text.Font = GameFont.Small;

            if (loadedMods.Contains(details)) details = "[D] " + details;
            else details = "[M] " + details;

            Widgets.Label(rect, details);

            Text.Font = GameFont.Medium;
        }
    }
}
