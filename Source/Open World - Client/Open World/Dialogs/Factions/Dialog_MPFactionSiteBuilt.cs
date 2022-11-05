using RimWorld;
using RimWorld.Planet;
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
        public override Vector2 InitialSize => new Vector2(500f, 375f);

        private Vector2 scrollPosition;

        private string windowTitle = "";
        private string windowDescription = "";

        private float buttonX = 150f;
        private float buttonY = 38f;

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

            if (siteType == 0)
            {
                Networking.SendData("FactionManagement│Silo│Check" + "│" + Main._ParametersCache.focusedCaravan.Tile);
            }

            if (siteType == 4)
            {
                Networking.SendData("FactionManagement│Bank│Refresh");
            }
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

                Text.Font = GameFont.Small;
                Widgets.Label(new Rect(centeredX - Text.CalcSize(windowDescription).x / 2, windowDescriptionDif, Text.CalcSize(windowDescription).x, Text.CalcSize(windowDescription).y), windowDescription);
                Text.Font = GameFont.Medium;

                Widgets.DrawLineHorizontal(rect.x, descriptionLineDif, rect.width);

                Rect listRect = new Rect(new Vector2(rect.xMin, rect.yMin + 90), new Vector2(rect.width, 146));
                GenerateList(listRect, 0);

                Text.Font = GameFont.Medium;
                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2) * 2, rect.yMax - buttonY * 2 - 10), new Vector2(buttonX * 2, buttonY)), "Deposit"))
                {
                    //Need a fix for this, can't use the player settlement but can't depend on another player forever
                    Settlement settlementToUse = Find.WorldObjects.Settlements.First((Settlement x) => Main._ParametersCache.allFactions.Contains(x.Faction));

                    if (settlementToUse == null) return;
                    else Main._ParametersCache.focusedSettlement = settlementToUse;

                    if (RimworldHandler.CheckIfAnySocialPawn(0)) Find.WindowStack.Add(new Dialog_MPFactionSiloDeposit());
                    else Find.WindowStack.Add(new OW_ErrorDialog("Pawn does not have enough social to trade"));
                }
            }

            else if (siteType == 1)
            {
                windowTitle = "Marketplace Management Menu";
                windowDescription = "Trade over unlimited distances with all faction members";

                Text.Font = GameFont.Small;
                Widgets.Label(new Rect(centeredX - Text.CalcSize(windowDescription).x / 2, windowDescriptionDif, Text.CalcSize(windowDescription).x, Text.CalcSize(windowDescription).y), windowDescription);
                Text.Font = GameFont.Medium;

                Widgets.DrawLineHorizontal(rect.x, descriptionLineDif, rect.width);

                Rect listRect = new Rect(new Vector2(rect.xMin, rect.yMin + 90), new Vector2(rect.width, 194f));
                GenerateList(listRect, 1);
            }

            else if (siteType == 2)
            {
                windowTitle = "Production Site Management Menu";
                windowDescription = "Generate resources over time for all members";

                Text.Font = GameFont.Small;
                Widgets.Label(new Rect(centeredX - Text.CalcSize(windowDescription).x / 2, windowDescriptionDif, Text.CalcSize(windowDescription).x, Text.CalcSize(windowDescription).y), windowDescription);
                Text.Font = GameFont.Medium;

                Widgets.DrawLineHorizontal(rect.x, descriptionLineDif, rect.width);

                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2) * 2, rect.yMax - buttonY * 5 - 40), new Vector2(buttonX * 2, buttonY)), "Receive Food"))
                {
                    Main._ParametersCache.productionSiteProduct = 1;
                    Find.WindowStack.Add(new OW_InfoDialog("This material has been temporarily increased"));
                }

                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2) * 2, rect.yMax - buttonY * 4 - 30), new Vector2(buttonX * 2, buttonY)), "Receive Silver"))
                {
                    Main._ParametersCache.productionSiteProduct = 2;
                    Find.WindowStack.Add(new OW_InfoDialog("This material has been temporarily increased"));
                }

                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2) * 2, rect.yMax - buttonY * 3 - 20), new Vector2(buttonX * 2, buttonY)), "Receive Components"))
                {
                    Main._ParametersCache.productionSiteProduct = 3;
                    Find.WindowStack.Add(new OW_InfoDialog("This material has been temporarily increased"));
                }

                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2) * 2, rect.yMax - buttonY * 2 - 10), new Vector2(buttonX * 2, buttonY)), "Receive Fuel"))
                {
                    Main._ParametersCache.productionSiteProduct = 4;
                    Find.WindowStack.Add(new OW_InfoDialog("This material has been temporarily increased"));
                }
            }

            else if (siteType == 3)
            {
                windowTitle = "Wonder Structure Management Menu";
                windowDescription = "Assert superiority against the whole planet";

                Text.Font = GameFont.Small;
                Widgets.Label(new Rect(centeredX - Text.CalcSize(windowDescription).x / 2, windowDescriptionDif, Text.CalcSize(windowDescription).x, Text.CalcSize(windowDescription).y), windowDescription);
                Text.Font = GameFont.Medium;

                Widgets.DrawLineHorizontal(rect.x, descriptionLineDif, rect.width);

                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2) * 2, rect.yMax - buttonY * 2 - 10), new Vector2(buttonX * 2, buttonY)), "Utilize"))
                {
                    Find.WindowStack.Add(new OW_ErrorDialog("This action is not implemented yet"));
                }
            }

            else if (siteType == 4)
            {
                windowTitle = "Bank Management Menu";
                windowDescription = "Generate passive wealth over time";

                string windowMoneyText = "Current funds: " + Main._ParametersCache.bankSilver + " silver";

                Text.Font = GameFont.Small;
                Widgets.Label(new Rect(centeredX - Text.CalcSize(windowDescription).x / 2, windowDescriptionDif, Text.CalcSize(windowDescription).x, Text.CalcSize(windowDescription).y), windowDescription);
                Text.Font = GameFont.Medium;

                Widgets.DrawLineHorizontal(rect.x, descriptionLineDif, rect.width);

                Widgets.Label(new Rect(centeredX - Text.CalcSize(windowMoneyText).x / 2, windowDescriptionDif + 30, Text.CalcSize(windowMoneyText).x, Text.CalcSize(windowMoneyText).y), windowMoneyText);

                Text.Font = GameFont.Medium;
                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2) * 2, rect.yMax - buttonY * 3 - 20), new Vector2(buttonX * 2, buttonY)), "Deposit"))
                {
                    //Need a fix for this, can't use the player settlement but can't depend on another player forever
                    Settlement settlementToUse = Find.WorldObjects.Settlements.First((Settlement x) => Main._ParametersCache.allFactions.Contains(x.Faction));

                    if (settlementToUse == null) return;
                    else Main._ParametersCache.focusedSettlement = settlementToUse;

                    if (RimworldHandler.CheckIfAnySocialPawn(0)) Find.WindowStack.Add(new Dialog_MPFactionBankDeposit());
                    else Find.WindowStack.Add(new OW_ErrorDialog("Pawn does not have enough social to trade"));
                }

                Text.Font = GameFont.Medium;
                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2) * 2, rect.yMax - buttonY * 2 - 10), new Vector2(buttonX * 2, buttonY)), "Withdraw"))
                {
                    Action actionToDo = delegate
                    {
                        Networking.SendData("FactionManagement│Bank│Withdraw" + "│" + Main._ParametersCache.quantityChosenOnDialog);
                    };

                    Find.WindowStack.Add(new OW_QuantityDialog(actionToDo));
                }
            }

            else if (siteType == 5)
            {
                windowTitle = "Stable Management Menu";
                windowDescription = "Jump between stables from all the planet for a fee";

                Text.Font = GameFont.Small;
                Widgets.Label(new Rect(centeredX - Text.CalcSize(windowDescription).x / 2, windowDescriptionDif, Text.CalcSize(windowDescription).x, Text.CalcSize(windowDescription).y), windowDescription);
                Text.Font = GameFont.Medium;

                Widgets.DrawLineHorizontal(rect.x, descriptionLineDif, rect.width);

                Rect listRect = new Rect(new Vector2(rect.xMin, rect.yMin + 90), new Vector2(rect.width, 194f));
                GenerateList(listRect, 3);
            }

            else if (siteType == 6)
            {
                windowTitle = "Courier Station Management Menu";
                windowDescription = "Trade over unlimited distances with everyone for a fee";

                Text.Font = GameFont.Small;
                Widgets.Label(new Rect(centeredX - Text.CalcSize(windowDescription).x / 2, windowDescriptionDif, Text.CalcSize(windowDescription).x, Text.CalcSize(windowDescription).y), windowDescription);
                Text.Font = GameFont.Medium;

                Widgets.DrawLineHorizontal(rect.x, descriptionLineDif, rect.width);

                Rect listRect = new Rect(new Vector2(rect.xMin, rect.yMin + 90), new Vector2(rect.width, 194f));
                GenerateList(listRect, 2);
            }

            Text.Font = GameFont.Medium;
            if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2) * 1.5f, rect.yMax - buttonY), new Vector2(buttonX * 1.5f, buttonY)), "Close"))
            {
                Close();
            }
        }

        private void GenerateList(Rect mainRect, int listMode)
        {
            float num = 0;
            float num2 = 0;
            float num3 = 0;
            int num4 = 0;

            var orderedDictionary = Main._ParametersCache.siloContents;

            if (listMode == 0)
            {
                orderedDictionary = Main._ParametersCache.siloContents
                    .OrderBy(x => x.Key)
                    .ToDictionary(x => x.Key, x => x.Value);
            }

            else if (listMode == 1)
            {
                orderedDictionary = Main._ParametersCache.onlineAllySettlements
                    .OrderBy(x => x.Value[0])
                    .ToDictionary(x => x.Key, x => x.Value);
            }

            else if (listMode == 2)
            {
                orderedDictionary = Main._ParametersCache.allOnlineSettlements
                    .OrderBy(x => x.Value[0])
                    .ToDictionary(x => x.Key, x => x.Value);
            }

            else if (listMode == 3)
            {
                Dictionary<int, List<string>> parsedDictionary = new Dictionary<int, List<string>>();
                foreach(KeyValuePair<int, List<int>> pair in Main._ParametersCache.allFactionStructures)
                {
                    if (pair.Value[0] == 5)
                    {
                        if (pair.Key == Main._ParametersCache.focusedCaravan.Tile) continue;
                        else parsedDictionary.Add(pair.Key, new List<string> { pair.Value[0].ToString(), pair.Value[1].ToString() });
                    }
                }

                orderedDictionary = parsedDictionary
                    .OrderBy(x => x.Value[0])
                    .ToDictionary(x => x.Key, x => x.Value);
            }

            float height = 6f + orderedDictionary.Count() * 30f;
            Rect viewRect = new Rect(mainRect.x, mainRect.y, mainRect.width - 16f, height);

            Widgets.BeginScrollView(mainRect, ref scrollPosition, viewRect);

            num = 0;
            num2 = scrollPosition.y - 30f;
            num3 = scrollPosition.y + mainRect.height;
            num4 = 0;

            foreach (KeyValuePair<int, List<string>> pair in orderedDictionary)
            {
                if (num > num2 && num < num3)
                {
                    Rect rect = new Rect(0f, mainRect.y + num, viewRect.width, 30f);
                    DrawCustomRow(rect, pair, num4, listMode);
                }

                num += 30f;
                num4++;
            }

            Widgets.EndScrollView();
        }

        private void DrawCustomRow(Rect rect, KeyValuePair<int, List<string>> pair, int index, int rowType)
        {
            float buttonX = 47f;
            float buttonY = 30f;

            Text.Font = GameFont.Small;

            if (index % 2 == 0) Widgets.DrawLightHighlight(rect);
            Rect fixedRect = new Rect(new Vector2(rect.x + 10f, rect.y + 5f), new Vector2(rect.width - 52f, rect.height));

            if (rowType == 0)
            {
                foreach (ThingDef item in DefDatabase<ThingDef>.AllDefs)
                {
                    if (item.defName == pair.Value[0])
                    {
                        if (item.label.Count() > 1) item.label = char.ToUpper(item.label[0]) + item.label.Substring(1);
                        else item.label = item.label.ToUpper();

                        Widgets.Label(fixedRect, item.label + " x" + pair.Value[1]);
                    }
                }
                    
                if (Widgets.ButtonText(new Rect(new Vector2(rect.xMax - buttonX, rect.y), new Vector2(buttonX, buttonY)), "Take"))
                {
                    if (Main._ParametersCache.canWithdrawSilo)
                    {
                        Networking.SendData("FactionManagement│Silo│Withdraw" + "│" + Main._ParametersCache.focusedCaravan.Tile + "│" + pair.Key);
                        Main._ParametersCache.canWithdrawSilo = false;
                    }
                }
            }

            else if (rowType == 1 || rowType == 2)
            {
                if (pair.Value[0].Length > 1) pair.Value[0] = char.ToUpper(pair.Value[0][0]) + pair.Value[0].Substring(1);
                else pair.Value[0] = pair.Value[0].ToUpper();

                Widgets.Label(fixedRect, pair.Value[0]);
                if (Widgets.ButtonText(new Rect(new Vector2(rect.xMax - buttonX * 3 - 20, rect.y), new Vector2(buttonX, buttonY)), "Trade"))
                {
                    if (rowType == 2)
                    {
                        if (!MPCaravan.TakeFundsFromCaravan(100))
                        {
                            Find.WindowStack.Add(new OW_ErrorDialog("You need 100 silver to use the courier"));
                            Close();
                            return;
                        }
                    }

                    foreach (Settlement settlement in Find.World.worldObjects.Settlements)
                    {
                        if (settlement.Tile == pair.Key)
                        {
                            Main._ParametersCache.focusedSettlement = settlement;
                            Main._ParametersCache.focusedTile = settlement.Tile;

                            if (RimworldHandler.CheckIfAnySocialPawn(0)) Find.WindowStack.Add(new Dialog_MPTrade());
                            else Find.WindowStack.Add(new OW_ErrorDialog("Pawn does not have enough social to trade"));
                        }
                    }
                }
                if (Widgets.ButtonText(new Rect(new Vector2(rect.xMax - buttonX * 2 - 10, rect.y), new Vector2(buttonX, buttonY)), "Barter"))
                {
                    if (rowType == 2)
                    {
                        if (!MPCaravan.TakeFundsFromCaravan(100))
                        {
                            Find.WindowStack.Add(new OW_ErrorDialog("You need 100 silver to use the courier"));
                            Close();
                            return;
                        }
                    }

                    foreach (Settlement settlement in Find.World.worldObjects.Settlements)
                    {
                        if (settlement.Tile == pair.Key)
                        {
                            if (RimworldHandler.CheckIfAnySocialPawn(0)) Find.WindowStack.Add(new Dialog_MPBarter(false, null));
                            else Find.WindowStack.Add(new OW_ErrorDialog("Pawn does not have enough social to trade"));
                        }
                    }
                }
                if (Widgets.ButtonText(new Rect(new Vector2(rect.xMax - buttonX, rect.y), new Vector2(buttonX, buttonY)), "Gift"))
                {
                    if (rowType == 2)
                    {
                        if (!MPCaravan.TakeFundsFromCaravan(100))
                        {
                            Find.WindowStack.Add(new OW_ErrorDialog("You need 100 silver to use the courier"));
                            Close();
                            return;
                        }
                    }

                    foreach (Settlement settlement in Find.World.worldObjects.Settlements)
                    {
                        if (settlement.Tile == pair.Key)
                        {
                            Main._ParametersCache.focusedSettlement = settlement;
                            Main._ParametersCache.focusedTile = settlement.Tile;

                            if (RimworldHandler.CheckIfAnySocialPawn(0)) Find.WindowStack.Add(new Dialog_MPGift());
                            else Find.WindowStack.Add(new OW_ErrorDialog("Pawn does not have enough social to trade"));
                        }
                    }
                }
            }

            else if (rowType == 3)
            {
                Widgets.Label(fixedRect, "Stable - " + pair.Key);
                if (Widgets.ButtonText(new Rect(new Vector2(rect.xMax - buttonX, rect.y), new Vector2(buttonX, buttonY)), "Travel"))
                {
                    if (!MPCaravan.TakeFundsFromCaravan(100))
                    {
                        Find.WindowStack.Add(new OW_ErrorDialog("You need 100 silver to use the courier"));
                        Close();
                        return;
                    }

                    Main._ParametersCache.focusedCaravan.Tile = pair.Key;
                    Main._ParametersCache.focusedCaravan.pather.nextTile = pair.Key;

                    Close();
                }
            }
        }
    }
}
