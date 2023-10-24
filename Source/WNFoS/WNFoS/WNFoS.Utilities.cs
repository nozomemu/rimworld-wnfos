using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace WNFoS
{
    public static class WNFoS_Utilities
    {
        public static void DoWindowContents(Rect fillRect, List<WNFoS_Order> orders, WNFoS_Order curOrder, ref Vector2 scrollPosition, ref float scrollViewHeight, Faction scrollToFaction = null)
        {
            Rect rect = new Rect(0f, 0f, fillRect.width, fillRect.height);
            Widgets.BeginGroup(rect);

            DrawOrdersList(rect, orders, ref scrollPosition, ref scrollViewHeight);
            DrawMainOrderView(rect, orders, curOrder);

            if (orders.Count > 0)
            {
                //Widgets.Label(new Rect(614f, 50f, 200f, 100f), "EnemyOf".Translate());
                //outRect.yMin += Text.LineHeight;
                
            }
            else
            {
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(rect, "NoFactions".Translate());
                Text.Anchor = TextAnchor.UpperLeft;
            }
            Widgets.EndGroup();
        }

        private static void DrawOrdersList(Rect fillRect, List<WNFoS_Order> orders, ref Vector2 scrollPosition, ref float scrollViewHeight)
        {
            float scrollViewProportion = (fillRect.width * 0.20f);
            Text.Font = GameFont.Small;
            GUI.color = Color.white;

            Rect rect = new Rect(0f, 0f, scrollViewProportion, fillRect.height);
            Rect outRect = new Rect(0f, 50f, scrollViewProportion, rect.height - 50f);
            Rect rect2 = new Rect(0f, 0f, scrollViewProportion - 16f, scrollViewHeight);

            Widgets.BeginScrollView(outRect, ref scrollPosition, rect2);
            float num = 0f;
            int num2 = 0;
            foreach (WNFoS_Order order in orders)
            {
                //if (visibleFaction == scrollToFaction)
                //{
                //    scrollPosition.y = num;
                //}
                if (num2 % 2 == 1)
                {
                    Widgets.DrawLightHighlight(new Rect(rect2.x, num, rect2.width, 80f));
                    
                }
                num += DrawOrderRow(order, num, rect2);
                num2++;
            }
            if (Event.current.type == EventType.Layout)
            {
                scrollViewHeight = num;
            }
            Widgets.EndScrollView();
        }

        private static void DrawMainOrderView(Rect fillRect, List<WNFoS_Order> orders, WNFoS_Order curOrder)
        {
            float mainOrderViewProportion = (fillRect.width * 0.80f);
            Text.Font = GameFont.Small;
            GUI.color = Color.white;

            Rect rect = new Rect((fillRect.width - mainOrderViewProportion), 0f, mainOrderViewProportion, fillRect.height);

            if (curOrder != null)
            {
                string label = "WNFoS.Key.Name".Translate() + ": " + curOrder.Pawn.Name + "\n" +
                            "WNFoS.Key.Order".Translate() + ": " + curOrder.Def.LabelCap + "\n";

                Widgets.Label(rect, label);
            }
            else
            {
                Widgets.Label(rect, "NoFactions".Translate());
            }
            
            //Rect outRect = new Rect(0f, 50f, mainOrderViewProportion, rect.height - 50f);
            //Rect rect2 = new Rect(0f, 0f, mainOrderViewProportion - 16f, scrollViewHeight);
        }

        private static float DrawOrderRow(WNFoS_Order order, float rowY, Rect fillRect)
        {
            float num = fillRect.width - 300f - 40f - 70f - 54f - 16f - 120f;

            //Faction[] array = Find.FactionManager.AllFactionsInViewOrder.Where((Faction f) => f != order && f.HostileTo(order) && ((!f.IsPlayer && !f.Hidden) || showAll)).ToArray();

            //Rect rect = new Rect(90f, rowY, 300f, 80f);
            Rect rect = new Rect(90f, rowY, fillRect.width - 90f, 80f);

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleLeft;
            Rect position = new Rect(24f, rowY + (rect.height - 42f) / 2f, 42f, 42f);
            GUI.color = order.Color;
            //GUI.DrawTexture(position, order.Pawn.Draw()); //DRAW THE PAWN
            //GUI.DrawTexture(position, order.Faction.def.FactionIcon);
            GUI.color = Color.white;

            //string label = order.Name.CapitalizeFirst() + "\n" + order.Def.LabelCap + "\n" + ((order.leader != null) ? (order.LeaderTitle.CapitalizeFirst() + ": " + order.leader.Name.ToStringFull) : "");
            //string label = order.Name.CapitalizeFirst() + "\n" + order.Def.LabelCap + "\n";
            string label = "WNFoS.Key.Name".Translate() + ": " + order.Pawn.Name + "\n" +
                            "WNFoS.Key.Order".Translate() + ": " + order.Def.LabelCap + "\n";

            Widgets.Label(rect, label);
            if (Widgets.ButtonText(rect, label))
            {

            };

            Rect rect2 = new Rect(0f, rowY, rect.xMax, 80f);
            if (Mouse.IsOver(rect2))
            {
                //TipSignal tip = new TipSignal(() => order.Name.Colorize(ColoredText.TipSectionTitleColor) + "\n" + order.Def.LabelCap.Resolve() + "\n\n" + order.Def.description, order.loadID ^ 0x738AC053);
                //TooltipHandler.TipRegion(rect2, tip);
                Widgets.DrawHighlight(rect2);
            }
            if (Widgets.ButtonInvisible(rect2, doMouseoverSound: false))
            {
                //Find.WindowStack.Add(new Dialog_InfoCard(order));
            }
            Rect rect3 = new Rect(rect.xMax, rowY, 40f, 80f);
            //Widgets.InfoCardButtonCentered(rect3, order);
            Rect rect4 = new Rect(rect3.xMax, rowY, 60f, 80f);
            //if (ModsConfig.IdeologyActive && !Find.IdeoManager.classicMode)
            //{
            //    if (order.ideos != null)
            //    {
            //        float num2 = rect4.x;
            //        float num3 = rect4.y;
            //        if (order.ideos.PrimaryIdeo != null)
            //        {
            //            if (num2 + 40f > rect4.xMax)
            //            {
            //                num2 = rect4.x;
            //                num3 += 45f;
            //            }
            //            Rect rect5 = new Rect(num2, num3 + (rect4.height - 40f) / 2f, 40f, 40f);
            //            IdeoUIUtility.DoIdeoIcon(rect5, order.ideos.PrimaryIdeo, doTooltip: true, delegate
            //            {
            //                IdeoUIUtility.OpenIdeoInfo(order.ideos.PrimaryIdeo);
            //            });
            //            num2 += rect5.width + 5f;
            //            num2 = rect4.x;
            //            num3 += 45f;
            //        }
            //        List<Ideo> minor = order.ideos.IdeosMinorListForReading;
            //        int i;
            //        for (i = 0; i < minor.Count; i++)
            //        {
            //            if (num2 + 22f > rect4.xMax)
            //            {
            //                num2 = rect4.x;
            //                num3 += 27f;
            //            }
            //            if (num3 + 22f > rect4.yMax)
            //            {
            //                break;
            //            }
            //            Rect rect6 = new Rect(num2, num3 + (rect4.height - 22f) / 2f, 22f, 22f);
            //            IdeoUIUtility.DoIdeoIcon(rect6, minor[i], doTooltip: true, delegate
            //            {
            //                IdeoUIUtility.OpenIdeoInfo(minor[i]);
            //            });
            //            num2 += rect6.width + 5f;
            //        }
            //    }
            //}
            //else
            //{
            //    rect4.width = 0f;
            //}
            Rect rect7 = new Rect(rect4.xMax, rowY, 70f, 80f);
            //if (!order.IsPlayer)
            //{
            //    string text = order.PlayerRelationKind.GetLabelCap();
            //    if (order.defeated)
            //    {
            //        text = text.Colorize(ColorLibrary.Grey);
            //    }
            //    GUI.color = order.PlayerRelationKind.GetColor();
            //    Text.Anchor = TextAnchor.MiddleCenter;
            //    if (order.HasGoodwill && !order.def.permanentEnemy)
            //    {
            //        Widgets.Label(new Rect(rect7.x, rect7.y - 10f, rect7.width, rect7.height), text);
            //        Text.Font = GameFont.Medium;
            //        Widgets.Label(new Rect(rect7.x, rect7.y + 10f, rect7.width, rect7.height), order.PlayerGoodwill.ToStringWithSign());
            //        Text.Font = GameFont.Small;
            //    }
            //    else
            //    {
            //        Widgets.Label(rect7, text);
            //    }
            //    GenUI.ResetLabelAlign();
            //    GUI.color = Color.white;
            //    if (Mouse.IsOver(rect7))
            //    {
            //        TaggedString taggedString = "";
            //        if (order.def.permanentEnemy)
            //        {
            //            taggedString = "CurrentGoodwillTip_PermanentEnemy".Translate();
            //        }
            //        else if (order.HasGoodwill)
            //        {
            //            taggedString = "Goodwill".Translate().Colorize(ColoredText.TipSectionTitleColor) + ": " + (order.PlayerGoodwill.ToStringWithSign() + ", " + order.PlayerRelationKind.GetLabel()).Colorize(order.PlayerRelationKind.GetColor());
            //            TaggedString ongoingEvents = GetOngoingEvents(order);
            //            if (!ongoingEvents.NullOrEmpty())
            //            {
            //                taggedString += "\n\n" + "OngoingEvents".Translate().Colorize(ColoredText.TipSectionTitleColor) + ":\n" + ongoingEvents;
            //            }
            //            TaggedString recentEvents = GetRecentEvents(order);
            //            if (!recentEvents.NullOrEmpty())
            //            {
            //                taggedString += "\n\n" + "RecentEvents".Translate().Colorize(ColoredText.TipSectionTitleColor) + ":\n" + recentEvents;
            //            }
            //            string s = "";
            //            switch (order.PlayerRelationKind)
            //            {
            //                case FactionRelationKind.Ally:
            //                    s = "CurrentGoodwillTip_Ally".Translate(0.ToString("F0"));
            //                    break;
            //                case FactionRelationKind.Neutral:
            //                    s = "CurrentGoodwillTip_Neutral".Translate((-75).ToString("F0"), 75.ToString("F0"));
            //                    break;
            //                case FactionRelationKind.Hostile:
            //                    s = "CurrentGoodwillTip_Hostile".Translate(0.ToString("F0"));
            //                    break;
            //            }
            //            taggedString += "\n\n" + s.Colorize(ColoredText.SubtleGrayColor);
            //        }
            //        if (taggedString != "")
            //        {
            //            TooltipHandler.TipRegion(rect7, taggedString);
            //        }
            //        Widgets.DrawHighlight(rect7);
            //    }
            //}
            Rect rect8 = new Rect(rect7.xMax, rowY, 54f, 80f);
            //if (!order.IsPlayer && order.HasGoodwill && !order.def.permanentEnemy)
            //{
            //    FactionRelationKind relationKindForGoodwill = GetRelationKindForGoodwill(order.NaturalGoodwill);
            //    GUI.color = relationKindForGoodwill.GetColor();
            //    Rect rect9 = rect8.ContractedBy(7f);
            //    rect9.y = rowY + 30f;
            //    rect9.height = 20f;
            //    Text.Anchor = TextAnchor.UpperCenter;
            //    Widgets.DrawRectFast(rect9, Color.black);
            //    Widgets.Label(rect9, order.NaturalGoodwill.ToStringWithSign());
            //    GenUI.ResetLabelAlign();
            //    GUI.color = Color.white;
            //    if (Mouse.IsOver(rect8))
            //    {
            //        TaggedString taggedString2 = "NaturalGoodwill".Translate().Colorize(ColoredText.TipSectionTitleColor) + ": " + order.NaturalGoodwill.ToStringWithSign().Colorize(relationKindForGoodwill.GetColor());
            //        int goodwill = Mathf.Clamp(order.NaturalGoodwill - 50, -100, 100);
            //        int goodwill2 = Mathf.Clamp(order.NaturalGoodwill + 50, -100, 100);
            //        taggedString2 += "\n" + "NaturalGoodwillRange".Translate().Colorize(ColoredText.TipSectionTitleColor) + ": " + goodwill.ToString().Colorize(GetRelationKindForGoodwill(goodwill).GetColor()) + " " + "RangeTo".Translate() + " " + goodwill2.ToString().Colorize(GetRelationKindForGoodwill(goodwill2).GetColor());
            //        TaggedString naturalGoodwillExplanation = GetNaturalGoodwillExplanation(order);
            //        if (!naturalGoodwillExplanation.NullOrEmpty())
            //        {
            //            taggedString2 += "\n\n" + "AffectedBy".Translate().Colorize(ColoredText.TipSectionTitleColor) + "\n" + naturalGoodwillExplanation;
            //        }
            //        taggedString2 += "\n\n" + "NaturalGoodwillDescription".Translate(1.25f.ToStringPercent()).Colorize(ColoredText.SubtleGrayColor);
            //        TooltipHandler.TipRegion(rect8, taggedString2);
            //        Widgets.DrawHighlight(rect8);
            //    }
            //}
            float num4 = rect8.xMax + 17f;
            //for (int j = 0; j < array.Length; j++)
            //{
            //    if (num4 >= rect8.xMax + num)
            //    {
            //        num4 = rect8.xMax;
            //        rowY += 27f;
            //    }
            //    //DrawOrderPawnWithTooltip(new Rect(num4, rowY + 29f, 22f, 22f), array[j]);
            //    num4 += 27f;
            //}
            Text.Anchor = TextAnchor.UpperLeft;
            return 80f;
        }

        public static void DrawOrderPawnWithTooltip(Rect r, WNFoS_Order order)
        {
            GUI.color = order.Color;
            //GUI.DrawTexture(r, order.def.FactionIcon);
            GUI.color = Color.white;
            if (Mouse.IsOver(r))
            {
                //TipSignal tip = new TipSignal(() => string.Concat(new string[]
                //{
                //order.Name.Colorize(ColoredText.TipSectionTitleColor),
                //"\n",
                //order.Def.LabelCap.Resolve(),
                //"\n\n",
                //order.Def.description,
                //}));
                //TooltipHandler.TipRegion(r, tip);
                Widgets.DrawHighlight(r);
            }
            if (Widgets.ButtonInvisible(r, doMouseoverSound: false))
            {
                //Find.WindowStack.Add(new Dialog_InfoCard(order));
            }
        }

        //public static void DoWindowContents(Rect inRect)
        //{
        //    GameFont font = Text.Font;
        //    TextAnchor anchor = Text.Anchor;
        //    //Rect inRect2 = UIUtility.TakeLeftPart(ref inRect, (float)UI.screenWidth * 0.25f);
        //    Rect inRect2 = new Rect(inRect.LeftPart(UI.screenWidth * 0.25f));
        //    inRect.xMin += 3f;
        //    //if (curTab.doDividerLine)
        //    //{
        //    //    Widgets.DrawLineVertical(inRect2.xMax, inRect.y + 20f, inRect.height - 40f);
        //    //}
        //    Text.Font = GameFont.Medium;
        //    Text.Anchor = TextAnchor.MiddleCenter;
        //    //Widgets.Label(new Rect(inRect2.TopPart(50f)), curTab.label);
        //    Text.Anchor = anchor;
        //    Text.Font = GameFont.Tiny;
        //    //Widgets.Label(UIUtility.TakeTopPart(ref inRect2, 40f), curTab.description.Colorize(ColoredText.SubtleGrayColor));
        //    inRect2.yMin += 5f;
        //    Text.Font = GameFont.Small;
        //    //Widgets.Label(UIUtility.TakeTopPart(ref inRect2, 20f), "VFEE.SeeAlso".Translate());
        //    inRect2.yMin += 10f;
        //    //foreach (RoyaltyTabDef allDef in DefDatabase<RoyaltyTabDef>.AllDefs)
        //    //{
        //    //    allDef.Worker.parent = this;
        //    //    if (Widgets.ButtonText(UIUtility.TakeTopPart(ref inRect2, 40f).LeftPartPixels(230f), allDef.label))
        //    //    {
        //    //        curTab = allDef;
        //    //        curTab.Worker.Notify_Open();
        //    //    }
        //    //    inRect2.yMin += 5f;
        //    //}
        //    inRect2.yMin += 10f;
        //    //if (curTab.hasSearch)
        //    //{
        //    //    SearchWidget.OnGUI(UIUtility.TakeBottomPart(ref inRect2, 30f), DoSearch);
        //    //}
        //    //if (Prefs.DevMode)
        //    //{
        //    //    Widgets.CheckboxLabeled(UIUtility.TakeBottomPart(ref inRect2, 30f), "VFEE.DevMode".Translate(), ref DevMode);
        //    //}
        //    //if (curTab.needsCharacter)
        //    //{
        //    //    DoCharacterSelection(ref inRect2);
        //    //}
        //    //curTab.Worker.DoLeftBottom(inRect2);
        //    //curTab.Worker.DoMainSection(inRect);
        //    Text.Font = font;
        //    Text.Anchor = anchor;
        //}
    }
}
