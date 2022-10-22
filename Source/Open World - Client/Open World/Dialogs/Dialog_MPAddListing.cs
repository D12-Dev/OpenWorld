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
    public class Dialog_MPAddListing : Window
    {
        public override Vector2 InitialSize => new Vector2(353f, 310f);

        private float buttonX = 150f;
        private float buttonY = 38f;

        string windowTitle = "New Offer";
        string wantToString = "Want to: Buy";
        string itemNameString = "Item: ";
        string quantityString = "Quantity: ";
        string willPayString = "Price: ";
        string confirmString = "Type 'Confirm': ";

        string itemNameText;
        string QuantityText;
        string willPayText;
        string confirmText;

        private int modeIndex = 0;

        private int startAcceptingInputAtFrame;
        private bool AcceptsInput => startAcceptingInputAtFrame <= Time.frameCount;

        public Dialog_MPAddListing()
        {
            soundAppear = SoundDefOf.CommsWindow_Open;
            soundClose = SoundDefOf.CommsWindow_Close;

            forcePause = true;
            absorbInputAroundWindow = true;
            closeOnAccept = false;
            closeOnCancel = true;
        }

        public override void DoWindowContents(Rect rect)
        {
            float centeredX = rect.width / 2;
            float horizontalLineDif = Text.CalcSize(windowTitle).y + (StandardMargin / 4);
            float wantToDif = horizontalLineDif + StandardMargin;
            float itemNameDif = wantToDif + (StandardMargin * 2);
            float quantityDif = itemNameDif + (StandardMargin * 2);
            float willPayDif = quantityDif + (StandardMargin * 2);
            float confirmDif = willPayDif + (StandardMargin * 2);

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(centeredX - (Text.CalcSize(windowTitle).x / 2), rect.y, Text.CalcSize(windowTitle).x, Text.CalcSize(windowTitle).y), windowTitle);
            Widgets.DrawLineHorizontal(rect.x, horizontalLineDif, rect.width);

            Widgets.Label(new Rect(rect.x, wantToDif, Text.CalcSize(wantToString).x, Text.CalcSize(wantToString).y), wantToString);
            if (Widgets.ButtonText(new Rect(new Vector2(rect.xMax - buttonX, wantToDif), new Vector2(buttonX, 30f)), "Change"))
            {
                SoundDefOf.Click.PlayOneShotOnCamera();

                if (modeIndex == 0)
                {
                    wantToString = "Want to: Sell";
                    modeIndex = 1;
                }

                else
                {
                    wantToString = "Want to: Buy";
                    modeIndex = 0;
                }
            }

            Widgets.Label(new Rect(rect.x, itemNameDif, Text.CalcSize(itemNameString).x, Text.CalcSize(itemNameString).y), itemNameString);
            string a = Widgets.TextField(new Rect(Text.CalcSize(itemNameString).x, itemNameDif, rect.width - Text.CalcSize(itemNameString).x, 30f), itemNameText);
            if (AcceptsInput && a.Length <= 32 && a.All(character => Char.IsLetter(character))) itemNameText = a;

            Widgets.Label(new Rect(rect.x, quantityDif, Text.CalcSize(quantityString).x, Text.CalcSize(quantityString).y), quantityString);
            string b = Widgets.TextField(new Rect(Text.CalcSize(quantityString).x, quantityDif, rect.width - Text.CalcSize(quantityString).x, 30f), QuantityText);
            if (AcceptsInput && b.Length <= 6 && b.All(character => Char.IsDigit(character))) QuantityText = b;

            Widgets.Label(new Rect(rect.x, willPayDif, Text.CalcSize(willPayString).x, Text.CalcSize(willPayString).y), willPayString);
            string c = Widgets.TextField(new Rect(Text.CalcSize(willPayString).x, willPayDif, rect.width - Text.CalcSize(willPayString).x, 30f), willPayText);
            if (AcceptsInput && c.Length <= 8 && c.All(character => Char.IsDigit(character))) willPayText = c;

            Widgets.Label(new Rect(rect.x, confirmDif, Text.CalcSize(confirmString).x, Text.CalcSize(confirmString).y), confirmString);
            string d = Widgets.TextField(new Rect(Text.CalcSize(confirmString).x, confirmDif, rect.width - Text.CalcSize(confirmString).x, 30f), confirmText);
            if (AcceptsInput && d.Length <= 7 && d.All(character => Char.IsLetter(character))) confirmText = d;

            Text.Font = GameFont.Medium;
            if (Widgets.ButtonText(new Rect(new Vector2(rect.x, rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "Accept"))
            {
                CheckForValues();
            }

            if (Widgets.ButtonText(new Rect(new Vector2(rect.xMax - buttonX, rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "Cancel"))
            {
                Close();
            }
        }

        private void CheckForValues()
        {
            bool foundItemInDatabase = false;

            if (string.IsNullOrWhiteSpace(itemNameString)) return;
            if (int.Parse(QuantityText) == 0 || string.IsNullOrWhiteSpace(QuantityText)) return;
            if (int.Parse(willPayText) == 0 || string.IsNullOrWhiteSpace(willPayText)) return;
            if (confirmText != "Confirm") return;

            //foreach (ThingDef item in DefDatabase<ThingDef>.AllDefs)
            //{
            //    if (item.defName == itemNameString)
            //    {
            //        foundItemInDatabase = true;
            //        break;
            //    }
            //}

            //if (!foundItemInDatabase) return;

            Networking.SendData("AddNewListing│" + itemNameText + "│" + QuantityText + "│" + willPayText + "│" + modeIndex);

            Main._ParametersCache.yourListings.Add(Main._ParametersCache.yourListings.Count, new List<string>() { itemNameText, QuantityText, willPayText, modeIndex.ToString() });

            Close();
        }
    }
}
