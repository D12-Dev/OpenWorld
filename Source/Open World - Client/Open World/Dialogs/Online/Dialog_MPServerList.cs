using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Profile;

namespace OpenWorld
{
    public class Dialog_MPServerList : Window
    {
        public override Vector2 InitialSize => new Vector2(400f, 543f);
        private Vector2 scrollPosition = Vector2.zero;

        private string windowTitle = "Public Server List";

        private float buttonX = 150f;
        private float buttonY = 38f;

        private float moreButtonX = 100f;
        private float moreButtonY = 20f;

        List<List<string>> serversInfo = new List<List<string>>();

        public Dialog_MPServerList()
        {
            soundAppear = SoundDefOf.CommsWindow_Open;
            soundClose = SoundDefOf.CommsWindow_Close;
            absorbInputAroundWindow = true;
            forcePause = true;
            closeOnAccept = false;
            closeOnCancel = true;

            WebClient wc = new WebClient();
            List<string> info = wc.DownloadString("https://raw.githubusercontent.com/TastyLollipop/OpenWorld/main/Global%20Server%20Listings").Split('│').ToList();
            info.RemoveAt(0);
            foreach (string str in info) serversInfo.Add(str.Split('-').ToList());
        }

        public override void DoWindowContents(Rect rect)
        {
            float centeredX = rect.width / 2;
            float horizontalLineDif = Text.CalcSize(windowTitle).y + (StandardMargin / 4);
            float listingDif = Text.CalcSize(windowTitle).y + (StandardMargin / 2);

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(centeredX - Text.CalcSize(windowTitle).x / 2, rect.y, Text.CalcSize(windowTitle).x, Text.CalcSize(windowTitle).y), windowTitle);
            Widgets.DrawLineHorizontal(rect.x, horizontalLineDif, rect.width);
            Text.Font = GameFont.Small;

            FillMainRect(new Rect(new Vector2(rect.x, listingDif), new Vector2(rect.width, rect.yMax - listingDif - buttonY - StandardMargin)));

            if (Widgets.ButtonText(new Rect(new Vector2(centeredX - buttonX / 2, rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "Return"))
            {
                Close();
            }
        }

        private void FillMainRect(Rect mainRect)
        {
            float objectSize = 20f;
            float height = 6f + serversInfo.Count() * objectSize;
            Rect viewRect = new Rect(0f, 0f, mainRect.width - 16f, height);
            Widgets.BeginScrollView(mainRect, ref scrollPosition, viewRect);
            float num = 0;
            float num2 = scrollPosition.y - objectSize;
            float num3 = scrollPosition.y + mainRect.height;
            int num4 = 0;

            for (int i = 0; i < serversInfo.Count(); i++)
            {
                if (num > num2 && num < num3)
                {
                    Rect rect = new Rect(0f, num, viewRect.width, objectSize);
                    DrawCustomRow(rect, serversInfo[i], num4);
                }

                num += objectSize;
                num4++;
            }

            Widgets.EndScrollView();
        }

        private void DrawCustomRow(Rect rect, List<string> details, int index)
        {
            if (index % 2 == 0) Widgets.DrawHighlight(rect);
            string toShow = details[1].Replace("Name: ", "");

            Text.Font = GameFont.Small;

            Widgets.Label(rect, toShow);
            if (Widgets.ButtonText(new Rect(new Vector2(rect.xMax - moreButtonX, rect.y), new Vector2(moreButtonX, moreButtonY)), "Info"))
            {
                Find.WindowStack.Add(new Dialog_MPServerDescription(details, this));
            }

            Text.Font = GameFont.Medium;
        }
    }
}
