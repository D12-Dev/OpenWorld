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
    public class Dialog_MPTradeRequest : Window
    {
        public override Vector2 InitialSize => new Vector2(350f, 310f);

        private Vector2 scrollPosition = Vector2.zero;

        private float buttonX = 150f;
        private float buttonY = 38f;

        string windowTitle = "Trade Request";
        string silverText = "For: ";

        string[] tradeableItems;
        string invokerID;
        int silver;

        public Dialog_MPTradeRequest(string invoker, string[] items, string silver)
        {
            invokerID = invoker;
            tradeableItems = items;
            this.silver = int.Parse(silver);
            silverText += this.silver + " silver";

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

            try { GenerateList(new Rect(rect.x, itemListDif, rect.width, 160f)); }
            catch { }

            Text.Font = GameFont.Medium;
            if (Widgets.ButtonText(new Rect(new Vector2(rect.x, rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "Accept"))
            {
                Map map = Find.AnyPlayerHomeMap;
                int quantityAvailable = 0;
                int quantityToRemove = silver;

                foreach (Zone zone in map.zoneManager.AllZones)
                {
                    foreach (Thing item in zone.AllContainedThings)
                    {
                        if (item.def == ThingDefOf.Silver)
                        {
                            quantityAvailable += item.stackCount;
                        }
                    }
                }

                if (quantityAvailable < quantityToRemove)
                {
                    Networking.SendData("TradeStatus│Reject│" + invokerID);
                    TradeHandler.ResetTradeVariables();
                    Find.WindowStack.Add(new OW_ErrorDialog("You don't have enough funds for this action"));
                    Close();
                    return;
                }

                foreach (Zone zone in map.zoneManager.AllZones)
                {
                    foreach (Thing item in zone.AllContainedThings)
                    {
                        if (item.def == ThingDefOf.Silver)
                        {
                            if (quantityToRemove == 0) break;

                            if (quantityToRemove - item.stackCount < 0)
                            {
                                int quantityDif = quantityToRemove - item.stackCount;
                                item.stackCount = -quantityDif;
                                break;
                            }
                            else
                            {
                                quantityToRemove -= item.stackCount;
                                item.Destroy();
                            }
                        }
                    }
                }

                Networking.SendData("TradeStatus│Deal│" + invokerID);
                TradeHandler.ReceiveTradesFromPlayer(tradeableItems);
                Close();
            }

            if (Widgets.ButtonText(new Rect(new Vector2(rect.xMax - buttonX, rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "Reject"))
            {
                Networking.SendData("TradeStatus│Reject│" + invokerID);
                TradeHandler.ResetTradeVariables();
                Close();
            }
        }

        private void GenerateList(Rect mainRect)
        {
            float height = 6f + (float)tradeableItems.Count() * 21f;

            Rect viewRect = new Rect(mainRect.x, mainRect.y, mainRect.width - 16f, height);

            Widgets.BeginScrollView(mainRect, ref scrollPosition, viewRect);

            float num = 0;
            float num2 = scrollPosition.y - 30f;
            float num3 = scrollPosition.y + mainRect.height;
            int num4 = 0;

            int index = 0;

            var orderedList = tradeableItems.OrderBy(x => x.ToString());

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

            string itemName = "";

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
