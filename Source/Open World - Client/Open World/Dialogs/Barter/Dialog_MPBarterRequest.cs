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
    public class Dialog_MPBarterRequest : Window
    {
        public override Vector2 InitialSize => new Vector2(350f, 310f);

        private Vector2 scrollPosition = Vector2.zero;

        private float buttonX = 150f;
        private float buttonY = 38f;

        string windowTitle = "Barter Request";
        string silverText = "For: Your Items";

        string invokerID;
        bool rebarter;

        public Dialog_MPBarterRequest(string invoker, List<string> items, bool rebarter)
        {
            Main._ParametersCache.__MPBarterRequest = this;
            Main._ParametersCache.cachedItems = items;

            invokerID = invoker;
            this.rebarter = rebarter;

            Main._ParametersCache.inTrade = true;

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
            float horizontalLineDif = Text.CalcSize(windowTitle).y + (StandardMargin / 4) + 3f;
            float silverDif = horizontalLineDif + StandardMargin / 2;
            float itemListDif = silverDif + StandardMargin + 10f;

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(centeredX - (Text.CalcSize(windowTitle).x / 2), rect.y, Text.CalcSize(windowTitle).x, Text.CalcSize(windowTitle).y), windowTitle);
            Widgets.DrawLineHorizontal(rect.x, horizontalLineDif, rect.width);

            Widgets.Label(new Rect(centeredX - (Text.CalcSize(silverText).x / 2), silverDif, Text.CalcSize(silverText).x, Text.CalcSize(silverText).y), silverText);

            Text.Font = GameFont.Small;

            GenerateList(new Rect(rect.x, itemListDif, rect.width, 160f));

            Text.Font = GameFont.Medium;
            if (Widgets.ButtonText(new Rect(new Vector2(rect.x, rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "Accept"))
            {
                if (rebarter)
                {
                    Networking.SendData("BarterStatus│Deal│" + invokerID);
                    BarterHandler.ReceiveBarterToCaravan(Main._ParametersCache.cachedItems);

                    Main._ParametersCache.__MPWaiting.Close();
                    Close();

                    Main._ParametersCache.letterTitle = "Successful Trade";
                    Main._ParametersCache.letterDescription = "You have traded with another settlement! \n\nTraded items have been deposited in your caravan.";
                    Main._ParametersCache.letterType = LetterDefOf.PositiveEvent;
                    Injections.thingsToDoInUpdate.Add(RimworldHandler.GenerateLetter);
                }

                else
                {
                    if (RimworldHandler.CheckIfAnySocialPawn(1)) Find.WindowStack.Add(new Dialog_MPBarter(true, invokerID));
                    else
                    {
                        Networking.SendData("BarterStatus│Reject│" + invokerID);

                        BarterHandler.ResetBarterVariables();
                        Find.WindowStack.Add(new OW_ErrorDialog("Pawn does not have enough social to trade"));
                        Close();
                    }
                }
            }

            if (Widgets.ButtonText(new Rect(new Vector2(rect.xMax - buttonX, rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "Reject"))
            {
                Networking.SendData("BarterStatus│Reject│" + invokerID);

                if (rebarter)
                {
                    BarterHandler.ReturnBarterToCaravan();
                    Main._ParametersCache.__MPWaiting.Close();
                }

                BarterHandler.ResetBarterVariables();

                Close();
            }
        }

        private void GenerateList(Rect mainRect)
        {
            float height = 6f + (float)Main._ParametersCache.cachedItems.Count() * 21f;

            Rect viewRect = new Rect(mainRect.x, mainRect.y, mainRect.width - 16f, height);

            Widgets.BeginScrollView(mainRect, ref scrollPosition, viewRect);

            float num = 0;
            float num2 = scrollPosition.y - 30f;
            float num3 = scrollPosition.y + mainRect.height;
            int num4 = 0;

            int index = 0;

            var orderedList = Main._ParametersCache.cachedItems.OrderBy(x => x.ToString());

            foreach (string str in orderedList)
            {
                if (num > num2 && num < num3)
                {
                    Rect rect = new Rect(0f, mainRect.y + num, viewRect.width, 21f);
                    DrawCustomRow(rect, str, index);
                    index++;
                }

                num += 21f;
                num4++;
            }

            Widgets.EndScrollView();
        }

        private void DrawCustomRow(Rect rect, string itemDetails, int index)
        {
            Text.Font = GameFont.Small;
            Rect fixedRect = new Rect(new Vector2(rect.x + 10f, rect.y + 5f), new Vector2(rect.width - 36f, rect.height));
            if (index % 2 == 0) Widgets.DrawHighlight(fixedRect);

            string itemName = "Silver";

            try
            {
                if (itemDetails.Split('┼')[0] == "pawn")
                {
                    itemName = "Human";
                    Widgets.Label(fixedRect, itemName + ", " + itemDetails.Split('┼')[1]);
                    return;
                }

                else
                {
                    foreach (ThingDef item in DefDatabase<ThingDef>.AllDefs)
                    {
                        if (item.defName == itemDetails.Split('┼')[0].Replace("minified-", ""))
                        {
                            itemName = item.label;
                            if (itemName.Length > 1) itemName = char.ToUpper(itemName[0]) + itemName.Substring(1);
                            else itemName = itemName.ToUpper();
                            break;
                        }

                        else if (item.defName == itemDetails.Split('┼')[0])
                        {
                            itemName = item.label;
                            if (itemName.Length > 1) itemName = char.ToUpper(itemName[0]) + itemName.Substring(1);
                            else itemName = itemName.ToUpper();
                            break;
                        }
                    }
                }
            }

            catch { }

            Widgets.Label(fixedRect, itemName + " x" + itemDetails.Split('┼')[1]);
        }
    }
}
