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
    public class OW_QuantityDialog : Window
    {
        public override Vector2 InitialSize => new Vector2(350f, 200f);

        private int startAcceptingInputAtFrame;
        private bool AcceptsInput => startAcceptingInputAtFrame <= Time.frameCount;

        private string windowTitle = "QUANTITY";
        private string windowDescription = "Select the quantity you want to act on";

        private float buttonX = 150f;
        private float buttonY = 38f;

        Action actionOnConfirm;

        public OW_QuantityDialog(Action actionOnConfirm)
        {
            soundAppear = SoundDefOf.CommsWindow_Open;
            soundClose = SoundDefOf.CommsWindow_Close;
            absorbInputAroundWindow = true;
            forcePause = true;
            closeOnAccept = false;
            closeOnCancel = false;

            this.actionOnConfirm = actionOnConfirm;
        }

        public override void DoWindowContents(Rect rect)
        {
            float centeredX = rect.width / 2;

            float horizontalLineDif = Text.CalcSize(windowDescription).y + StandardMargin / 2;

            float windowDescriptionDif = Text.CalcSize(windowDescription).y + StandardMargin;

            float quantityInputDif = Text.CalcSize(windowDescription).y + StandardMargin * 2 + 15;

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(centeredX - Text.CalcSize(windowTitle).x / 2, rect.y, Text.CalcSize(windowTitle).x, Text.CalcSize(windowTitle).y), windowTitle);

            Widgets.DrawLineHorizontal(rect.x, horizontalLineDif, rect.width);

            Text.Font = GameFont.Small;
            Widgets.Label(new Rect(centeredX - Text.CalcSize(windowDescription).x / 2, windowDescriptionDif, Text.CalcSize(windowDescription).x, Text.CalcSize(windowDescription).y), windowDescription);

            string quantityInput = Widgets.TextField(new Rect(centeredX - (200f / 2), quantityInputDif, 200f, 30f), Main._ParametersCache.quantityChosenOnDialog.ToString());
            if (AcceptsInput && quantityInput.Length <= 8 &&  quantityInput.All(character => Char.IsDigit(character))) Main._ParametersCache.quantityChosenOnDialog = int.Parse(quantityInput);

            if (Widgets.ButtonText(new Rect(new Vector2(rect.xMin, rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "Accept"))
            {
                actionOnConfirm.Invoke();

                Close();
            }

            if (Widgets.ButtonText(new Rect(new Vector2(rect.xMax - buttonX, rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "Cancel"))
            {
                Close();
            }
        }
    }
}
