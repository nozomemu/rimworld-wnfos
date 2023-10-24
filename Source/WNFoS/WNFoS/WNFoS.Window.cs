using NarutoMod;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaranMagicFramework;
using UnityEngine;
using UnityEngine.Profiling;
using Verse;
using Verse.Sound;
using AbilityDef = TaranMagicFramework.AbilityDef;

namespace WNFoS
{
    public class WNFoS_MainTabWindow_Orders : MainTabWindow
    {
        public List<WNFoS_Order> fosOrders
        {
            get
            {
                WNFoS_WorldComponent_FleeOnSightComponent fosComponent = Find.World.GetComponent<WNFoS_WorldComponent_FleeOnSightComponent>();
                return fosComponent?.fosOrders;
            }
        }
        public WNFoS_Order curOrder;

        public override Vector2 RequestedTabSize => new Vector2(1192f, UI.screenHeight * 0.8f);
        //Idk I hate aspect ratio stuff
        //private float aspectRatioFactor = (UI.screenWidth / UI.screenHeight) / (16/9);

        public QuickSearchWidget searchWidget = new QuickSearchWidget();

        private Vector2 scrollPositionOrdersList;
        private Vector2 scrollPositionMainOrderViewDetails;
        private Vector2 scrollPositionMainOrderViewReasons;
        private float scrollViewHeightOrdersList;
        private float scrollViewHeightReasonsList;

        public override void PreOpen()
        {
            base.PreOpen();
        }

        public override void DoWindowContents(Rect fillRect)
        {
            Rect rect = new Rect(0f, 0f, fillRect.width, fillRect.height);
            Widgets.BeginGroup(rect);

            float ordersListWidth = 400f;
            DrawOrdersList(rect, ordersListWidth, fosOrders, ref scrollPositionOrdersList, ref scrollViewHeightOrdersList);
            Widgets.DrawLineVertical(ordersListWidth, 0, fillRect.height);
            float mainOrderViewWidth = 768f;
            DrawMainOrderView(rect, mainOrderViewWidth, curOrder, ref scrollPositionMainOrderViewDetails, ref scrollPositionMainOrderViewReasons, ref scrollViewHeightReasonsList);
            
            Widgets.DrawLineHorizontal(ordersListWidth + 8f, fillRect.yMax - 50f, mainOrderViewWidth);
            if (Find.FactionManager.OfPlayer.leader != null)
            {
                if (Widgets.ButtonText(new Rect(ordersListWidth + 8f, fillRect.yMax - Text.LineHeight * 2, 180f, Text.LineHeight * 2), "WNFoS.Key.IssueOrder".Translate()))
                {
                    Find.WindowStack.Add(new WNFoS_Dialog_OrderCreation(null, this));
                }
            }
            
            Widgets.EndGroup();
        }

        private void DrawOrdersList(Rect fillRect, float ordersListWidth, List<WNFoS_Order> orders, ref Vector2 scrollPosition, ref float scrollViewHeight)
        {
            string query = searchWidget.filter.Text.ToLower();
            List<WNFoS_Order> orderQuery = orders.Where(x => x.Pawn.Name.ToStringFull.ToLower().Contains(query) || x.Faction.Name.ToLower().Contains(query) || x.Name.ToLower().Contains(query)).ToList();
            
            GUI.color = Color.white;
            Text.Font = GameFont.Medium;            
            Rect rect = new Rect(0f, 0f, ordersListWidth, fillRect.height);
            Rect outRect = new Rect(0f, 50f, ordersListWidth, rect.height - 50f);
            Rect rect2 = new Rect(0f, 0f, ordersListWidth, scrollViewHeight);

            Text.Font = GameFont.Small;
            searchWidget.OnGUI(new Rect(outRect)
            {
                y = 50f - (Text.LineHeight * 2),
                height = Text.LineHeight
            });
            Widgets.BeginScrollView(outRect, ref scrollPosition, rect2);
            float num = 0f;
            int num2 = 0;
            foreach (WNFoS_Order order in orderQuery)
            {
                if (num2 % 2 == 1)
                {
                    Widgets.DrawLightHighlight(new Rect(rect2.x, num, rect2.width, 80f));

                }
                if (curOrder == order)
                {
                    Widgets.DrawHighlightSelected(new Rect(rect2.x, num, rect2.width, 80f));
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

        private float DrawOrderRow(WNFoS_Order order, float rowY, Rect fillRect)
        {
            Rect rect = new Rect(90f, rowY, fillRect.width - 90f, 80f);

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleLeft;
            Rect position = new Rect(24f, rowY + (rect.height - 42f) / 2f, 42f, 42f);
            GUI.color = order.Color;
            GUI.DrawTexture(position, order.Faction.def.FactionIcon);
            GUI.color = Color.white;
            string label = "WNFoS.Key.Name".Translate() + ": " + order.Pawn.Name + "\n" +
                            "WNFoS.Key.IssuingFaction".Translate() + ": " + order.Faction.Name + "\n" +
                            "WNFoS.Key.Order".Translate() + ": " + order.Name + "\n";
            Widgets.Label(rect, label);
            Rect rect2 = new Rect(0f, rowY, rect.xMax, 80f);
            if (Mouse.IsOver(rect2))
            {
                StringBuilder stringBuilderOrder = new StringBuilder();
                stringBuilderOrder.AppendLine(order.Name.Colorize(ColoredText.TipSectionTitleColor));
                if (order.Name != order.Def.LabelCap)
                {
                    stringBuilderOrder.AppendLine(order.Def.LabelCap);
                }
                stringBuilderOrder.AppendLine("WNFoS.Key.Name".Translate() + ": " + order.Pawn.Name);
                stringBuilderOrder.AppendLine("WNFoS.Key.IssuingFaction".Translate() + ": " + order.Faction.Name + "\n");
                stringBuilderOrder.AppendLine(order.Def.description);
                TooltipHandler.TipRegion(rect2, stringBuilderOrder.ToTaggedString());
                Widgets.DrawHighlight(rect2);
            }
            if (Widgets.ButtonInvisible(rect2))
            {
                SoundDefOf.Click.PlayOneShotOnCamera();
                this.curOrder = order;
            }
            Text.Anchor = TextAnchor.UpperLeft;
            return 80f;
        }

        private void DrawMainOrderView(Rect fillRect, float mainOrderViewWidth, WNFoS_Order curOrder, ref Vector2 scrollPositionDetails, ref Vector2 scrollPositionReasons, ref float scrollViewHeight)
        {
            Text.Font = GameFont.Small;
            GUI.color = Color.white;

            Rect rect = new Rect(fillRect)
            {
                x = fillRect.width - mainOrderViewWidth,
                width = mainOrderViewWidth,
            };
            Rect mainRect = new Rect(rect)
            {
                x = rect.x + 24f,
                y = rect.y,
                height = rect.height - 100f
            };

            if (curOrder != null)
            {
                Text.Font = GameFont.Medium;
                Widgets.Label(mainRect, curOrder.Faction.Name);
                Widgets.DrawLineHorizontal(mainRect.x, mainRect.y + Text.LineHeight, mainRect.width);
                float margin = 25f;

                Rect firstColumnRect = new Rect(mainRect.x, mainRect.y + Text.LineHeight * 2, 360f, mainRect.height - Text.LineHeight);
                DrawMainFirstColumn(mainRect, firstColumnRect, margin);

                Widgets.DrawLineVertical(firstColumnRect.xMax + 12f, firstColumnRect.y, firstColumnRect.height);

                Rect secondColumnRect = new Rect(firstColumnRect.xMax + 24f, firstColumnRect.y, 360f, firstColumnRect.height);
                DrawMainSecondColumn(mainRect, secondColumnRect, margin, ref scrollPositionDetails, ref scrollPositionReasons, ref scrollViewHeight);

                Rect editButtonRect = new Rect(400f + (8f * 2) + 180f, fillRect.yMax - Text.LineHeight * 2, 180f, Text.LineHeight * 2);
                if (curOrder.Faction.IsPlayer)
                {
                    if (Widgets.ButtonText(editButtonRect, "WNFoS.Key.EditOrder".Translate()))
                    {
                        Find.WindowStack.Add(new WNFoS_Dialog_OrderCreation(curOrder, this));
                    }
                }
                else if (Prefs.DevMode)
                {
                    if (Widgets.ButtonText(editButtonRect, "DEV: " + "WNFoS.Key.EditOrder".Translate()))
                    {
                        Find.WindowStack.Add(new WNFoS_Dialog_OrderCreation(curOrder, this));
                    }
                }

                Rect deleteButtonRect = new Rect(400f + (8f * 3) + (180f * 2), fillRect.yMax - Text.LineHeight * 2, 180f, Text.LineHeight * 2);
                if (curOrder.Faction.IsPlayer)
                {
                    if (Widgets.ButtonText(deleteButtonRect, "WNFoS.Key.DeleteOrder".Translate()))
                    {
                        WNFoS_WorldComponent_FleeOnSightComponent fosComponent = Find.World.GetComponent<WNFoS_WorldComponent_FleeOnSightComponent>();
                        fosComponent?.fosOrders.Remove(curOrder);
                    }
                }
                else if (Prefs.DevMode)
                {
                    if (Widgets.ButtonText(deleteButtonRect, "DEV: " + "WNFoS.Key.DeleteOrder".Translate()))
                    {
                        WNFoS_WorldComponent_FleeOnSightComponent fosComponent = Find.World.GetComponent<WNFoS_WorldComponent_FleeOnSightComponent>();
                        fosComponent?.fosOrders.Remove(curOrder);
                    }
                }

                Text.Anchor = TextAnchor.UpperLeft;
            }
            else
            {
                Text.Font = GameFont.Medium;
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(mainRect, "WNFoS.Key.NoOrderSelected".Translate());
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.UpperLeft;
            }
        }

        public void DrawMainFirstColumn(Rect mainRect, Rect firstColumnRect, float margin)
        {
            Rect portraitsRect = new Rect(firstColumnRect)
            {
                height = 250f
            };
            Rect leftPortraitRect = new Rect(portraitsRect)
            {
                width = 250f
            };
            Rect rightUpperPortraitRect = new Rect(leftPortraitRect)
            {
                x = leftPortraitRect.xMax,
                width = leftPortraitRect.width - 140f,
                height = leftPortraitRect.height / 2
            };
            Rect rightLowerPortraitRect = new Rect(rightUpperPortraitRect)
            {
                y = rightUpperPortraitRect.yMax
            };

            Rect pawnRect = new Rect(leftPortraitRect)
            {
                width = leftPortraitRect.width - margin,
                height = leftPortraitRect.height - margin
            }.
            CenteredOnXIn(leftPortraitRect).CenteredOnYIn(leftPortraitRect);
            Rect sideProfileRect = new Rect(rightUpperPortraitRect)
            {
                width = rightUpperPortraitRect.width - margin,
                height = rightUpperPortraitRect.height - margin
            }.
            CenteredOnXIn(rightUpperPortraitRect).CenteredOnYIn(rightUpperPortraitRect);
            Rect backViewRect = new Rect(rightLowerPortraitRect)
            {
                width = rightLowerPortraitRect.width - margin,
                height = rightLowerPortraitRect.height - margin
            }.
            CenteredOnXIn(rightLowerPortraitRect).CenteredOnYIn(rightLowerPortraitRect);

            Widgets.DrawBoxSolid(pawnRect, Color.grey);
            Widgets.DrawBoxSolid(sideProfileRect, Color.grey);
            Widgets.DrawBoxSolid(backViewRect, Color.grey);

            Vector3 portraitOffset = new Vector3(0f, 0f, 0.4f);
            float zoomLevel = 2.0f;
            GUI.DrawTexture(pawnRect, PortraitsCache.Get(curOrder.Pawn, pawnRect.size, Rot4.South, portraitOffset, zoomLevel));
            GUI.DrawTexture(sideProfileRect, PortraitsCache.Get(curOrder.Pawn, pawnRect.size, Rot4.West, portraitOffset, zoomLevel));
            GUI.DrawTexture(backViewRect, PortraitsCache.Get(curOrder.Pawn, pawnRect.size, Rot4.North, portraitOffset, zoomLevel));

            Widgets.DrawBoxSolidWithOutline(pawnRect, Color.clear, Color.white);
            Widgets.DrawBoxSolidWithOutline(sideProfileRect, Color.clear, Color.white);
            Widgets.DrawBoxSolidWithOutline(backViewRect, Color.clear, Color.white);

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleCenter;
            Rect bottomRect = new Rect(portraitsRect.x, portraitsRect.yMax + 24f, portraitsRect.width, firstColumnRect.height - (portraitsRect.height + 24f));
            Widgets.DrawMenuSection(bottomRect);

            Rect infoTitleRect = new Rect(bottomRect.x, bottomRect.y + (margin / 2), bottomRect.width - margin, Text.LineHeight).CenteredOnXIn(bottomRect);
            Widgets.DrawBoxSolidWithOutline(infoTitleRect, Color.clear, Color.white);
            Widgets.Label(infoTitleRect, "WNFoS.Key.BasicInformation".Translate());

            Rect infoLogoRect = new Rect(bottomRect.x, infoTitleRect.yMax, infoTitleRect.width, 100f).CenteredOnXIn(bottomRect);
            Widgets.DrawLightHighlight(infoLogoRect);
            Rect infoLogoPosition = new Rect(0f, 0f, 42f, 42f).CenteredOnXIn(infoLogoRect).CenteredOnYIn(infoLogoRect);
            Widgets.Label(new Rect(0f, infoLogoPosition.y - Text.LineHeight, infoLogoRect.width, Text.LineHeight).CenteredOnXIn(infoLogoRect), curOrder.Pawn.story.TitleCap + " " + "WNFoS.Key.Of".Translate());
            GUI.color = curOrder.Pawn.Faction.Color;
            GUI.DrawTexture(infoLogoPosition, curOrder.Pawn.Faction.def.FactionIcon);
            GUI.color = Color.white;
            Widgets.Label(new Rect(0f, infoLogoPosition.yMax, infoLogoRect.width, Text.LineHeight).CenteredOnXIn(infoLogoRect), curOrder.Pawn.Faction.NameColored);

            Text.Anchor = TextAnchor.UpperLeft;
            Rect infoRect = new Rect(bottomRect.x, infoLogoRect.yMax, bottomRect.width - margin, bottomRect.height - infoTitleRect.height - infoLogoRect.height - margin).CenteredOnXIn(bottomRect);
            Widgets.DrawLineHorizontal(infoRect.x, infoRect.y + (Text.LineHeight / 2), infoRect.width);
            string orderPawnName = curOrder.Pawn.Name.ToStringFull;
            string orderPawnBirthdate = curOrder.Pawn.ageTracker.BirthYear + "/" + curOrder.Pawn.ageTracker.BirthQuadrum + "/" + (curOrder.Pawn.ageTracker.BirthDayOfSeasonZeroBased + 1).ToString();
            StringBuilder stringBuilderBasicInformation = new StringBuilder();
            stringBuilderBasicInformation.AppendLine("WNFoS.Key.Name".Translate() + ": " + orderPawnName);
            stringBuilderBasicInformation.AppendLine("WNFoS.Key.Gender".Translate() + ": " + curOrder.Pawn.gender);
            stringBuilderBasicInformation.AppendLine("WNFoS.Key.Age".Translate() + ": " + curOrder.Pawn.ageTracker.AgeChronologicalYears);
            stringBuilderBasicInformation.AppendLine("WNFoS.Key.Birthdate".Translate() + ": " + orderPawnBirthdate);
            stringBuilderBasicInformation.AppendLine("WNFoS.Key.RaceClan".Translate() + ": " + curOrder.Pawn.genes.XenotypeLabelCap);
            stringBuilderBasicInformation.AppendLine();
            if (curOrder.Pawn.story.traits.HasTrait(WN_DefOf.WN_Chakra))
            {
                foreach (AbilityClass abilityClass in curOrder.Pawn.GetUnlockedAbilityClasses().Where(x => x.def.usesLevelSystem))
                {
                    string abilityRank;
                    if (19 <= abilityClass.level && abilityClass.level <= 20)
                    {
                        abilityRank = "WNFoS.Key.Kage".Translate();
                    }
                    else if (15 <= abilityClass.level && abilityClass.level <= 18)
                    {
                        abilityRank = "WNFoS.Key.EliteJounin".Translate();
                    }
                    else if (11 <= abilityClass.level && abilityClass.level <= 14)
                    {
                        abilityRank = "WNFoS.Key.Jounin".Translate();
                    }
                    else if (8 <= abilityClass.level && abilityClass.level <= 10)
                    {
                        abilityRank = "WNFoS.Key.Chuunin".Translate();
                    }
                    else if (1 <= abilityClass.level && abilityClass.level <= 7)
                    {
                        abilityRank = "WNFoS.Key.Genin".Translate();
                    }
                    else
                    {
                        abilityRank = "WNFoS.Key.NoAbility".Translate();
                    }
                    stringBuilderBasicInformation.AppendLine(abilityClass.def.LabelCap + ": " + abilityRank);
                }
                stringBuilderBasicInformation.AppendLine();
            }
            stringBuilderBasicInformation.AppendLine("WNFoS.Key.ConfirmedKills".Translate() + ": " + curOrder.Pawn.records.GetValue(WNFoS_DefOf.KillsHumanlikes));
            stringBuilderBasicInformation.AppendLine("WNFoS.Key.PeopleDefeated".Translate() + ": " + curOrder.Pawn.records.GetValue(WNFoS_DefOf.PawnsDowned));
            stringBuilderBasicInformation.AppendLine("WNFoS.Key.PeopleCaptured".Translate() + ": " + curOrder.Pawn.records.GetValue(WNFoS_DefOf.PeopleCaptured));
            Widgets.Label(new Rect(infoRect.x, infoRect.y + Text.LineHeight, infoRect.width, infoRect.height), stringBuilderBasicInformation.ToString());
        }

        private void DrawMainSecondColumn(Rect mainRect, Rect secondColumnRect, float margin, ref Vector2 scrollPositionDetails, ref Vector2 scrollPositionReasons, ref float scrollViewHeight)
        {
            Rect officialRecordsRect = new Rect(secondColumnRect.x, secondColumnRect.y, secondColumnRect.width, 250f);

            // Issuing Faction Seal
            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.MiddleCenter;
            Rect sealHeaderRect = new Rect(0f, officialRecordsRect.y, officialRecordsRect.width, Text.LineHeight).CenteredOnXIn(officialRecordsRect);
            Widgets.DrawBoxSolidWithOutline(sealHeaderRect, Color.clear, Color.white);
            Widgets.Label(sealHeaderRect, "WNFoS.Key.OfficialOrder".Translate());

            Text.Font = GameFont.Small;
            Rect orderIssuerRect = new Rect(0f, sealHeaderRect.yMax, officialRecordsRect.width, Text.LineHeight).CenteredOnXIn(officialRecordsRect);
            Widgets.DrawLightHighlight(orderIssuerRect);
            Widgets.Label(orderIssuerRect, "WNFoS.Key.IssuedBy".Translate() + " " + curOrder.IssuingPawn.Name.ToStringFull);

            Rect sealLogoPosition = new Rect(0f, (officialRecordsRect.center.y) + sealHeaderRect.height + orderIssuerRect.height - 84f, 84f, 84f).CenteredOnXIn(officialRecordsRect);
            GUI.DrawTexture(sealLogoPosition, curOrder.Faction.def.FactionIcon);
            Rect sealLogoLabel = new Rect(sealLogoPosition)
            {
                y = sealLogoPosition.yMax,
                width = officialRecordsRect.width,
                height = Text.LineHeight
            }.CenteredOnXIn(sealLogoPosition);
            Widgets.Label(sealLogoLabel, "WNFoS.Key.SealOf".Translate() + " " + curOrder.Faction.Name);

            // Divider
            Widgets.DrawLineHorizontal(secondColumnRect.x, officialRecordsRect.yMax, secondColumnRect.width);
            Widgets.DrawLineHorizontal(secondColumnRect.x + (margin / 2), officialRecordsRect.yMax + 12f, secondColumnRect.width - margin);

            // Order Info
            Rect orderInfoRect = new Rect(officialRecordsRect.x, officialRecordsRect.yMax + 24f, secondColumnRect.width, secondColumnRect.height - (officialRecordsRect.height + 24f));
            Widgets.DrawBoxSolidWithOutline(orderInfoRect, Color.clear, Color.white);

            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.UpperLeft;
            Rect orderLabelNameRect = new Rect(0f, orderInfoRect.y + (margin / 2), orderInfoRect.width - margin, Text.LineHeight).CenteredOnXIn(orderInfoRect);
            Widgets.Label(orderLabelNameRect, "WNFoS.Key.Order".Translate());

            Text.Font = GameFont.Small;
            Rect orderNameRect = new Rect(orderLabelNameRect)
            {
                y = orderLabelNameRect.yMax
            };
            Widgets.DrawLightHighlight(orderNameRect);
            Rect orderNameText = new Rect(orderNameRect)
            {
                width = orderNameRect.width - (margin / 4),
                height = orderNameRect.height - (margin / 4),
                x = orderNameRect.x + (margin / 8),
                y = orderNameRect.y + (margin / 8),
            };
            Widgets.Label(orderNameText, curOrder.Name);

            Text.Font = GameFont.Medium;
            Rect orderLabelDetailsRect = new Rect(orderLabelNameRect)
            {
                y = orderNameRect.yMax
            };
            Widgets.Label(orderLabelDetailsRect, "WNFoS.Key.Details".Translate());

            Text.Font = GameFont.Small;
            Rect orderDetailsRect = new Rect(orderLabelDetailsRect)
            {
                height = Text.LineHeight * 6,
                y = orderLabelDetailsRect.yMax
            };
            Widgets.DrawLightHighlight(orderDetailsRect);
            Rect orderDetailsText = new Rect(orderDetailsRect)
            {
                width = orderDetailsRect.width - (margin / 4),
                height = orderDetailsRect.height - (margin / 4),
                x = orderDetailsRect.x + (margin / 8),
                y = orderDetailsRect.y + (margin / 8),
            };
            StringBuilder stringBuilderOrderDetails = new StringBuilder();
            stringBuilderOrderDetails.Append(curOrder.Details + " ");
            if (curOrder.Def.orderExceptions != null)
            {
                stringBuilderOrderDetails.AppendLine("\n");
                if (curOrder.Def.orderExceptions.requiresAllExceptions)
                {
                    stringBuilderOrderDetails.Append("WNFoS.Key.AllExceptions".Translate() + ": ");
                }
                else
                {
                    stringBuilderOrderDetails.Append("WNFoS.Key.SomeExceptions".Translate() + ": ");
                }
                if (curOrder.Def.orderExceptions.exceptedRoyalTitles != null)
                {
                    foreach (RoyalTitleDef royalTitle in curOrder.Def.orderExceptions.exceptedRoyalTitles)
                    {
                        stringBuilderOrderDetails.Append(royalTitle.LabelCap + " / ");
                    }
                }
                if (curOrder.Def.orderExceptions.exceptedMinimumLevels != null)
                {
                    foreach (AbilityClassLevelRequirement levelRequirement in curOrder.Def.orderExceptions.exceptedMinimumLevels)
                    {
                        stringBuilderOrderDetails.Append(levelRequirement.abilityClass.LabelCap + "(" + levelRequirement.minLevel + ") / ");
                    }
                }
                if (curOrder.Def.orderExceptions.exceptedAbilities != null)
                {
                    foreach (AbilityDef ability in curOrder.Def.orderExceptions.exceptedAbilities)
                    {
                        stringBuilderOrderDetails.Append(ability.LabelCap + " / ");
                    }
                }
                if (curOrder.Def.orderExceptions.exceptedGenes != null)
                {
                    foreach (GeneDef gene in curOrder.Def.orderExceptions.exceptedGenes)
                    {
                        stringBuilderOrderDetails.Append(gene.LabelCap + " / ");
                    }
                }
                if (curOrder.Def.orderExceptions.exceptedHediffs != null)
                {
                    foreach (HediffDef hediff in curOrder.Def.orderExceptions.exceptedHediffs)
                    {
                        stringBuilderOrderDetails.Append(hediff.LabelCap + " / ");
                    }
                }
                if (curOrder.Def.orderExceptions.exceptedTraits != null)
                {
                    foreach (TraitDef trait in curOrder.Def.orderExceptions.exceptedTraits)
                    {
                        stringBuilderOrderDetails.Append(trait.LabelCap + " / ");
                    }
                }
                stringBuilderOrderDetails.Length -= " / ".Length;
            }
            else
            {
                stringBuilderOrderDetails.AppendLine("\n");
                stringBuilderOrderDetails.Append("WNFoS.Key.NoExceptions".Translate());
            }
            Widgets.LabelScrollable(orderDetailsText, stringBuilderOrderDetails.ToString(), ref scrollPositionDetails);

            Text.Font = GameFont.Medium;
            Rect orderLabelReasonsRect = new Rect(orderDetailsRect)
            {
                y = orderDetailsRect.yMax,
                height = Text.LineHeight
            };
            Widgets.Label(orderLabelReasonsRect, "WNFoS.Key.Reasons".Translate());

            Text.Font = GameFont.Small;
            Rect orderReasonsRect = new Rect(orderLabelReasonsRect)
            {
                y = orderLabelReasonsRect.yMax,
                height = secondColumnRect.height - (orderLabelReasonsRect.yMax - secondColumnRect.y + 1) - (margin / 2)
            };
            Rect reasonsViewRect = new Rect(0f, 0f, orderReasonsRect.width - 16f, scrollViewHeight);

            Widgets.BeginScrollView(orderReasonsRect, ref scrollPositionReasons, reasonsViewRect);
            float num = 0f;
            int num2 = 0;
            foreach (WNFoS_OrderReason reason in curOrder.OrderReasons)
            {
                if (num2 % 2 == 1)
                {
                    Widgets.DrawLightHighlight(new Rect(reasonsViewRect.x, num, reasonsViewRect.width, 80f));
                }
                num += DrawReasonRow(reason, num, reasonsViewRect);
                num2++;
            }
            if (Event.current.type == EventType.Layout)
            {
                scrollViewHeight = num;
            }
            Widgets.EndScrollView();
        }

        private float DrawReasonRow(WNFoS_OrderReason reason, float rowY, Rect fillRect)
        {
            Rect rect = new Rect(90f, rowY, fillRect.width - 90f, 80f);

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleLeft;
            Rect position = new Rect(24f, rowY + (rect.height - 42f) / 2f, 42f, 42f);
            GUI.DrawTexture(position, reason.Def.ReasonIcon);
            Widgets.Label(rect, reason.Name);

            Rect rect2 = new Rect(0f, rowY, rect.xMax, 80f);
            if (Mouse.IsOver(rect2))
            {
                StringBuilder stringBuilderReason = new StringBuilder();
                stringBuilderReason.AppendLine(reason.Name.Colorize(ColoredText.TipSectionTitleColor));
                stringBuilderReason.AppendLine(reason.Def.LabelCap + "\n");
                stringBuilderReason.AppendLine(reason.Description + "\n");
                if (reason.Def.reasonRequirements != null)
                {
                    stringBuilderReason.AppendLine("WNFoS.Key.reasonRequirements".Translate().Colorize(ColoredText.TipSectionTitleColor));
                    if (reason.Def.reasonRequirements.requiredAbility != null)
                    {
                        stringBuilderReason.AppendLine("WNFoS.Key.requiredAbility".Translate() + ": " + reason.Def.reasonRequirements.requiredAbility.LabelCap);
                    }
                    if (reason.Def.reasonRequirements.requiredKills > 1)
                    {
                        stringBuilderReason.AppendLine("WNFoS.Key.requiredKills".Translate() + ": " + reason.Def.reasonRequirements.requiredKills);
                    }
                    if (reason.Def.reasonRequirements.requiredAbilityClassLevels != null)
                    {
                        string requiredAbilityClassName = reason.Def.reasonRequirements.requiredAbilityClassLevels.requiredAbilityClass.LabelCap;
                        int requiredAbilityClassLevel = reason.Def.reasonRequirements.requiredAbilityClassLevels.requiredLevel;
                        stringBuilderReason.AppendLine("WNFoS.Key.requiredAbilityClass".Translate(requiredAbilityClassName) + " " + requiredAbilityClassLevel);
                    }
                }
                TooltipHandler.TipRegion(rect2, stringBuilderReason.ToTaggedString());
                Widgets.DrawHighlight(rect2);
            }
            return 80f;
        }
    }

    public class WNFoS_Dialog_OrderCreation : Window
    {
        protected WNFoS_MainTabWindow_Orders parent;
        protected bool editMode = false;
        protected Faction issuingFaction = Find.FactionManager.OfPlayer;
        private WNFoS_Order oldOrder;

        private List<Pawn> pawnList = Find.WorldPawns.AllPawnsAlive.Where(x => x?.Name != null && x?.story != null).ToList();

        protected Pawn curPawn;
        protected Pawn previewPawn;
        protected WNFoS_OrderDef curOrderDef;
        protected string curOrderName;
        protected string curOrderDetails;
        protected List<WNFoS_OrderReason> curOrderReasons = new List<WNFoS_OrderReason>();

        WNFoS_OrderReasonDef curReasonDef = null;
        string curReasonName = string.Empty;
        string curReasonDescription = string.Empty;
        WNFoS_OrderReason selectedReason = null;

        protected bool pawnSelectionDone = false;
        private bool orderDefSelectionDone = false;
        private bool orderReasonsSelectionDone = false;

        public override Vector2 InitialSize => new Vector2(800f, 600f);
        private QuickSearchWidget searchWidget = new QuickSearchWidget();

        private Vector2 scrollPositionPawnsList;
        private Vector2 scrollPositionReasonsList;

        public WNFoS_Dialog_OrderCreation(WNFoS_Order order, WNFoS_MainTabWindow_Orders parent)
        {
            absorbInputAroundWindow = false;
            closeOnClickedOutside = false;
            doCloseButton = false;
            doCloseX = true;

            if (order != null)
            {
                this.editMode = true;
                this.issuingFaction = order.Faction;
                this.oldOrder = order;

                this.curPawn = order.Pawn;
                this.curOrderDef = order.Def;
                this.curOrderName = order.Name;
                this.curOrderDetails = order.Details;
                this.curOrderReasons = order.OrderReasons;

                this.pawnSelectionDone = true;
            }
        }

        public override void PostOpen()
        {
            base.PostOpen();
            if (editMode)
            {
                WNFoS_WorldComponent_FleeOnSightComponent fosComponent = Find.World.GetComponent<WNFoS_WorldComponent_FleeOnSightComponent>();
                fosComponent?.fosOrders.Remove(oldOrder);
            }
        }

        public override void PostClose()
        {
            base.PostClose();
            if (editMode && !orderReasonsSelectionDone)
            {
                WNFoS_WorldComponent_FleeOnSightComponent fosComponent = Find.World.GetComponent<WNFoS_WorldComponent_FleeOnSightComponent>();
                fosComponent?.fosOrders.Add(oldOrder);
            }
        }

        public override void DoWindowContents(Rect inRect)
        {
            Widgets.BeginGroup(inRect);
            if (!pawnSelectionDone) 
            {
                DrawPawnSelection(inRect);
            }
            else if (!orderDefSelectionDone)
            {
                DrawOrderDefSelection(inRect);
            }
            else if (!orderReasonsSelectionDone)
            {
                DrawOrderReasonsSelection(inRect);
            }
            Widgets.EndGroup();
        }

        private void DrawPawnSelection(Rect inRect)
        {
            DrawPawnsList(inRect, ref scrollPositionPawnsList);

            float margin = 25f;
            Rect portraitsRect = new Rect(inRect.xMax - 360f, 0f, 360f, 250f).CenteredOnYIn(inRect);
            Rect leftPortraitRect = new Rect(portraitsRect) { width = 250f };
            Rect rightUpperPortraitRect = new Rect(leftPortraitRect)
            {
                x = leftPortraitRect.xMax,
                width = leftPortraitRect.width - 140f,
                height = leftPortraitRect.height / 2
            };
            Rect rightLowerPortraitRect = new Rect(rightUpperPortraitRect) { y = rightUpperPortraitRect.yMax };

            Rect pawnRect = new Rect(leftPortraitRect)
            {
                width = leftPortraitRect.width - margin,
                height = leftPortraitRect.height - margin
            }.
            CenteredOnXIn(leftPortraitRect).CenteredOnYIn(leftPortraitRect);
            Rect sideProfileRect = new Rect(rightUpperPortraitRect)
            {
                width = rightUpperPortraitRect.width - margin,
                height = rightUpperPortraitRect.height - margin
            }.
            CenteredOnXIn(rightUpperPortraitRect).CenteredOnYIn(rightUpperPortraitRect);
            Rect backViewRect = new Rect(rightLowerPortraitRect)
            {
                width = rightLowerPortraitRect.width - margin,
                height = rightLowerPortraitRect.height - margin
            }.
            CenteredOnXIn(rightLowerPortraitRect).CenteredOnYIn(rightLowerPortraitRect);

            Widgets.DrawBoxSolid(pawnRect, Color.grey);
            Widgets.DrawBoxSolid(sideProfileRect, Color.grey);
            Widgets.DrawBoxSolid(backViewRect, Color.grey);

            Vector3 portraitOffset = new Vector3(0f, 0f, 0.4f);
            float zoomLevel = 2.0f;
            Pawn portraitPawn = null;
            if (previewPawn != null)
            {
                portraitPawn = previewPawn;
            }
            else if (curPawn != null)
            {
                portraitPawn = curPawn;
            }
            if (portraitPawn != null)
            {
                Text.Font = GameFont.Medium;
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(new Rect(0f, portraitsRect.y - Text.LineHeight - (Text.LineHeight / 2), portraitsRect.width, Text.LineHeight).CenteredOnXIn(portraitsRect), portraitPawn.Name.ToStringFull);
                Text.Font = GameFont.Small;
                Widgets.Label(new Rect(0f, portraitsRect.y - Text.LineHeight, portraitsRect.width, Text.LineHeight).CenteredOnXIn(portraitsRect), portraitPawn.story.TitleCap + " " + "WNFoS.Key.Of".Translate() + " " + portraitPawn.Faction.Name);
                Text.Anchor = TextAnchor.UpperLeft;

                GUI.DrawTexture(pawnRect, PortraitsCache.Get(portraitPawn, pawnRect.size, Rot4.South, portraitOffset, zoomLevel));
                GUI.DrawTexture(sideProfileRect, PortraitsCache.Get(portraitPawn, pawnRect.size, Rot4.West, portraitOffset, zoomLevel));
                GUI.DrawTexture(backViewRect, PortraitsCache.Get(portraitPawn, pawnRect.size, Rot4.North, portraitOffset, zoomLevel));

                if (curPawn != null)
                {
                    if (Widgets.ButtonText(new Rect(0f, inRect.yMax - Text.LineHeight * 2, 180f, Text.LineHeight * 2).CenteredOnXIn(portraitsRect), "WNFoS.Key.Select".Translate()))
                    {
                        SoundDefOf.Click.PlayOneShotOnCamera();
                        if (curPawn != null)
                        {
                            pawnSelectionDone = true;
                        }
                    }
                }
            }
            else
            {
                Text.Font = GameFont.Medium;
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(new Rect(0f, 50f, inRect.width, Text.LineHeight).CenteredOnXIn(portraitsRect), "WNFoS.Key.SelectTarget".Translate());
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.UpperLeft;
            }

            Widgets.DrawBoxSolidWithOutline(pawnRect, Color.clear, Color.white);
            Widgets.DrawBoxSolidWithOutline(sideProfileRect, Color.clear, Color.white);
            Widgets.DrawBoxSolidWithOutline(backViewRect, Color.clear, Color.white);
        }

        private void DrawPawnsList(Rect inRect, ref Vector2 scrollPosition)
        {
            string query = searchWidget.filter.Text.ToLower();
            List<Pawn> pawnQuery = pawnList.Where(x => x.Name.ToStringFull.ToLower().Contains(query) || x.Faction.Name.ToLower().Contains(query)).ToList();

            GUI.color = Color.white;
            Text.Font = GameFont.Medium;
            Rect rect = new Rect(0f, 0f, 400f, inRect.height);
            Rect outRect = new Rect(0f, 50f, 400f, rect.height - 50f);
            Rect rect2 = new Rect(0f, 0f, 400f - 16f, (float)pawnQuery.Count() * 80f);

            Text.Font = GameFont.Small;
            searchWidget.OnGUI(new Rect(outRect)
            {
                y = 50f - (Text.LineHeight * 2),
                height = Text.LineHeight
            });
            Widgets.BeginScrollView(outRect, ref scrollPosition, rect2);
            float num = 0f;
            int num2 = 0;
            foreach (Pawn pawn in pawnQuery)
            {
                if (num2 % 2 == 1)
                {
                    Widgets.DrawLightHighlight(new Rect(rect2.x, num, rect2.width, 80f));
                }
                num += DrawPawnRow(pawn, num, rect2);
                num2++;
            }
            Widgets.EndScrollView();
        }

        private float DrawPawnRow(Pawn pawn, float rowY, Rect fillRect)
        {
            Rect rect = new Rect(128f, rowY, fillRect.width - 128f, 80f);

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleLeft;
            Rect pawnGraphicRect = new Rect(4f, rowY + (rect.height - 72f), 72f, 72f);
            GUI.DrawTexture(pawnGraphicRect, PortraitsCache.Get(pawn, pawnGraphicRect.size, Rot4.South));
            Rect position = new Rect(72f, rowY + (rect.height - 32f) / 2f, 32f, 32f);
            GUI.color = pawn.Faction.Color;
            GUI.DrawTexture(position, pawn.Faction.def.FactionIcon);
            GUI.color = Color.white;

            string label = "WNFoS.Key.Name".Translate() + ": " + pawn.Name.ToStringFull + "\n" +
                            pawn.Faction.Name;
            Widgets.Label(rect, label);
            Rect rect2 = new Rect(0f, rowY, rect.xMax, 80f);
            if (Mouse.IsOver(rect2))
            {
                if (Mouse.IsOver(fillRect))
                {
                    if (previewPawn == null || previewPawn != pawn)
                    {
                        previewPawn = pawn;
                    }
                }
                else
                {
                    previewPawn = null;
                }
                Widgets.DrawHighlight(rect2);
            }
            else
            {
                if (!Mouse.IsOver(fillRect))
                {
                    previewPawn = null;
                }
            }
            if (Widgets.ButtonInvisible(rect2))
            {
                SoundDefOf.Click.PlayOneShotOnCamera();
                curPawn = pawn;
            }
            Text.Anchor = TextAnchor.UpperLeft;
            return 80;
        }

        private void DrawOrderDefSelection(Rect inRect)
        {
            List<WNFoS_OrderDef> orderDefs = DefDatabase<WNFoS_OrderDef>.AllDefsListForReading;

            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(new Rect(0f, 0f, inRect.width, Text.LineHeight).CenteredOnXIn(inRect), "WNFoS.Key.OrderInformation".Translate());
            Rect orderSpacingRect = new Rect(0f, Text.LineHeight * 2, inRect.width, Text.LineHeight);
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            Rect orderTypeLabelRect = new Rect(orderSpacingRect) { width = Text.CalcSize("WNFoS.Key.OrderType".Translate()).x, height = Text.LineHeight };
            Widgets.Label(orderTypeLabelRect, "WNFoS.Key.OrderType".Translate());
            string orderDefLabel = curOrderDef == null ? "WNFoS.Key.Select".Translate() : curOrderDef.LabelCap;
            if (Widgets.ButtonText(new Rect(orderTypeLabelRect) { x = orderTypeLabelRect.xMax + 12f, width = inRect.width / 4 }, orderDefLabel))
            {
                SoundDefOf.Click.PlayOneShotOnCamera();
                List<FloatMenuOption> list = new List<FloatMenuOption>();
                foreach (WNFoS_OrderDef orderDef in orderDefs)
                {
                    list.Add(new FloatMenuOption(orderDef.LabelCap, delegate
                    {
                        curOrderDef = orderDef;
                        curOrderName = orderDef.LabelCap;
                        curOrderDetails = orderDef.details;
                    }));
                }
                Find.WindowStack.Add(new FloatMenu(list));
            }
            if (curOrderDef != null)
            {
                Widgets.Label(new Rect(orderTypeLabelRect) { y = orderTypeLabelRect.yMax, width = (inRect.width / 2) - 12f, height = Text.LineHeight * 3 }, curOrderDef.orderTypeDescription);

                if (curOrderDef.orderExceptions != null)
                {
                    StringBuilder stringBuilderExceptions = new StringBuilder();
                    if (curOrderDef.orderExceptions.requiresAllExceptions)
                    {
                        stringBuilderExceptions.Append("WNFoS.Key.AllExceptions".Translate() + ": ");
                    }
                    else
                    {
                        stringBuilderExceptions.Append("WNFoS.Key.SomeExceptions".Translate() + ": ");
                    }
                    if (curOrderDef.orderExceptions.exceptedRoyalTitles != null)
                    {
                        foreach (RoyalTitleDef royalTitle in curOrderDef.orderExceptions.exceptedRoyalTitles)
                        {
                            stringBuilderExceptions.Append(royalTitle.LabelCap + " / ");
                        }
                    }
                    if (curOrderDef.orderExceptions.exceptedMinimumLevels != null)
                    {
                        foreach (AbilityClassLevelRequirement levelRequirement in curOrderDef.orderExceptions.exceptedMinimumLevels)
                        {
                            stringBuilderExceptions.Append(levelRequirement.abilityClass.LabelCap + "(" + levelRequirement.minLevel + ") / ");
                        }
                    }
                    if (curOrderDef.orderExceptions.exceptedAbilities != null)
                    {
                        foreach (AbilityDef ability in curOrderDef.orderExceptions.exceptedAbilities)
                        {
                            stringBuilderExceptions.Append(ability.LabelCap + " / ");
                        }
                    }
                    if (curOrderDef.orderExceptions.exceptedGenes != null)
                    {
                        foreach (GeneDef gene in curOrderDef.orderExceptions.exceptedGenes)
                        {
                            stringBuilderExceptions.Append(gene.LabelCap + " / ");
                        }
                    }
                    if (curOrderDef.orderExceptions.exceptedHediffs != null)
                    {
                        foreach (HediffDef hediff in curOrderDef.orderExceptions.exceptedHediffs)
                        {
                            stringBuilderExceptions.Append(hediff.LabelCap + " / ");
                        }
                    }
                    if (curOrderDef.orderExceptions.exceptedTraits != null)
                    {
                        foreach (TraitDef trait in curOrderDef.orderExceptions.exceptedTraits)
                        {
                            stringBuilderExceptions.Append(trait.LabelCap + " / ");
                        }
                    }
                    stringBuilderExceptions.Length -= " / ".Length;
                    Widgets.Label(new Rect(orderTypeLabelRect) { y = orderTypeLabelRect.yMax + Text.LineHeight * 12, width = (inRect.width / 2) - 12f, height = Text.LineHeight * 5 }, stringBuilderExceptions.ToString());
                }
                else
                {
                    Widgets.Label(new Rect(orderTypeLabelRect) { y = orderTypeLabelRect.yMax + Text.LineHeight * 12, width = (inRect.width / 2) - 12f, height = Text.LineHeight * 5 }, "WNFoS.Key.NoExceptions".Translate());
                }
            }
            Widgets.Label(new Rect(orderTypeLabelRect) { y = orderTypeLabelRect.yMax + Text.LineHeight * 3 }, "WNFoS.Key.Order".Translate());
            Rect orderNameTextRect = new Rect(orderTypeLabelRect) { y = orderTypeLabelRect.yMax + Text.LineHeight * 4, width = (inRect.width / 2) - 12f };
            curOrderName = Widgets.TextArea(orderNameTextRect, curOrderName);

            Widgets.Label(new Rect(orderTypeLabelRect) { y = orderTypeLabelRect.yMax + Text.LineHeight * 5 }, "WNFoS.Key.Details".Translate());
            Rect orderDetailsTextRect = new Rect(orderTypeLabelRect) { y = orderTypeLabelRect.yMax + Text.LineHeight * 6, width = (inRect.width / 2) - 12f, height = Text.LineHeight * 4 };
            curOrderDetails = Widgets.TextArea(orderDetailsTextRect, curOrderDetails);

            Widgets.Label(new Rect(orderTypeLabelRect) { y = orderTypeLabelRect.yMax + Text.LineHeight * 11 }, "WNFoS.Key.Exceptions".Translate());

            Text.Font = GameFont.Medium;
            Widgets.DrawLineVertical(inRect.width / 2, Text.LineHeight, inRect.height - (Text.LineHeight * 2) - Text.LineHeight);
            Text.Font = GameFont.Small;

            if (!editMode)
            {
                if (Widgets.ButtonText(new Rect((inRect.width / 3) - (180f / 2), inRect.yMax - Text.LineHeight * 2, 180f, Text.LineHeight * 2), "WNFoS.Key.Back".Translate()))
                {
                    SoundDefOf.Click.PlayOneShotOnCamera();
                    pawnSelectionDone = false;
                }
            }
            if (Widgets.ButtonText(new Rect((inRect.width * 2 / 3) - (180f / 2), inRect.yMax - Text.LineHeight * 2, 180f, Text.LineHeight * 2), "WNFoS.Key.Confirm".Translate()))
            {
                if (curOrderDef == null)
                {
                    SoundDefOf.ClickReject.PlayOneShotOnCamera();
                    Messages.Message("WNFoS.Key.NoOrderType".Translate(), MessageTypeDefOf.RejectInput);
                }
                else
                {
                    SoundDefOf.Click.PlayOneShotOnCamera();
                    orderDefSelectionDone = true;
                }
            }
        }

        private void DrawOrderReasonsSelection(Rect inRect)
        {
            List<WNFoS_OrderReasonDef> reasonDefs = DefDatabase<WNFoS_OrderReasonDef>.AllDefsListForReading.Where(x => x.selectable).ToList();

            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(new Rect(0f, 0f, inRect.width, Text.LineHeight).CenteredOnXIn(inRect), "WNFoS.Key.OrderReasons".Translate());
            Rect reasonSpacingRect = new Rect(0f, Text.LineHeight * 2, inRect.width, Text.LineHeight);
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            Rect reasonTypeLabelRect = new Rect(reasonSpacingRect) { width = Text.CalcSize("WNFoS.Key.ReasonType".Translate()).x, height = Text.LineHeight };
            Widgets.Label(reasonTypeLabelRect, "WNFoS.Key.ReasonType".Translate());
            string reasonDefLabel = curReasonDef == null ? "WNFoS.Key.Select".Translate() : curReasonDef.LabelCap;
            if (Widgets.ButtonText(new Rect(reasonTypeLabelRect) { x = reasonTypeLabelRect.xMax + 12f, width = inRect.width / 4 }, reasonDefLabel))
            {
                SoundDefOf.Click.PlayOneShotOnCamera();
                List<FloatMenuOption> list = new List<FloatMenuOption>();
                foreach (WNFoS_OrderReasonDef reasonDef in reasonDefs)
                {
                    list.Add(new FloatMenuOption(reasonDef.LabelCap, delegate
                    {
                        curReasonDef = reasonDef;
                        curReasonName = reasonDef.LabelCap;
                        curReasonDescription = reasonDef.description;
                    }));
                }
                Find.WindowStack.Add(new FloatMenu(list));
            }
            Widgets.Label(new Rect(reasonTypeLabelRect) { width = inRect.width, y = reasonTypeLabelRect.yMax + Text.LineHeight }, "WNFoS.Key.OrderReason".Translate());
            Rect reasonNameTextRect = new Rect(reasonTypeLabelRect) { y = reasonTypeLabelRect.yMax + Text.LineHeight * 2, width = (inRect.width / 2) - 12f };
            curReasonName = Widgets.TextArea(reasonNameTextRect, curReasonName);

            Widgets.Label(new Rect(reasonTypeLabelRect) { y = reasonTypeLabelRect.yMax + Text.LineHeight * 3 }, "WNFoS.Key.Description".Translate());
            Rect reasonDescriptionTextRect = new Rect(reasonTypeLabelRect) { y = reasonTypeLabelRect.yMax + Text.LineHeight * 4, width = (inRect.width / 2) - 12f, height = Text.LineHeight * 4 };
            curReasonDescription = Widgets.TextArea(reasonDescriptionTextRect, curReasonDescription);

            Text.Font = GameFont.Medium;
            Widgets.DrawLineVertical(inRect.width / 2, Text.LineHeight, inRect.height - (Text.LineHeight * 2) - Text.LineHeight);
            Text.Font = GameFont.Small;

            // Right side
            Rect reasonsListRect = new Rect((inRect.width / 2) + 12f, reasonSpacingRect.y, (inRect.width / 2) - 12f, Text.LineHeight * 16);
            Rect reasonsListViewRect = new Rect(reasonsListRect) { x = 0f, y = 0f, height = curOrderReasons.Count * 80f };
            if (curOrderReasons.Any())
            {
                Widgets.BeginScrollView(reasonsListRect, ref scrollPositionReasonsList, reasonsListViewRect);
                float num = 0f;
                int num2 = 0;
                foreach (WNFoS_OrderReason reason in curOrderReasons)
                {
                    if (num2 % 2 == 1)
                    {
                        Widgets.DrawLightHighlight(new Rect(reasonsListViewRect.x, num, reasonsListViewRect.width, 80f));
                    }
                    Rect rect = new Rect(90f, num, reasonsListRect.width - 90f, 80f);

                    Text.Font = GameFont.Small;
                    Text.Anchor = TextAnchor.MiddleLeft;
                    Rect position = new Rect(24f, num + (rect.height - 42f) / 2f, 42f, 42f);
                    GUI.DrawTexture(position, reason.Def.ReasonIcon);
                    Widgets.Label(rect, reason.Name);
                    Text.Anchor = TextAnchor.UpperLeft;

                    Rect rect2 = new Rect(0f, num, rect.width, 80f);
                    if (Mouse.IsOver(rect2))
                    {
                        StringBuilder stringBuilderReason = new StringBuilder();
                        stringBuilderReason.AppendLine(reason.Name.Colorize(ColoredText.TipSectionTitleColor));
                        stringBuilderReason.AppendLine(reason.Def.LabelCap + "\n");
                        stringBuilderReason.AppendLine(reason.Description + "\n");
                        if (reason.Def.reasonRequirements != null)
                        {
                            stringBuilderReason.AppendLine("WNFoS.Key.reasonRequirements".Translate().Colorize(ColoredText.TipSectionTitleColor));
                            if (reason.Def.reasonRequirements.requiredAbility != null)
                            {
                                stringBuilderReason.AppendLine("WNFoS.Key.requiredAbility".Translate() + ": " + reason.Def.reasonRequirements.requiredAbility.LabelCap);
                            }
                            if (reason.Def.reasonRequirements.requiredKills > 1)
                            {
                                stringBuilderReason.AppendLine("WNFoS.Key.requiredKills".Translate() + ": " + reason.Def.reasonRequirements.requiredKills);
                            }
                            if (reason.Def.reasonRequirements.requiredAbilityClassLevels != null)
                            {
                                string requiredAbilityClassName = reason.Def.reasonRequirements.requiredAbilityClassLevels.requiredAbilityClass.LabelCap;
                                int requiredAbilityClassLevel = reason.Def.reasonRequirements.requiredAbilityClassLevels.requiredLevel;
                                stringBuilderReason.AppendLine("WNFoS.Key.requiredAbilityClass".Translate(requiredAbilityClassName) + " " + requiredAbilityClassLevel);
                            }
                        }
                        TooltipHandler.TipRegion(rect2, stringBuilderReason.ToTaggedString());
                        Widgets.DrawHighlight(rect2);
                    }
                    if (Widgets.ButtonInvisible(rect2))
                    {
                        SoundDefOf.Click.PlayOneShotOnCamera();
                        selectedReason = reason;
                    }
                    if (reason == selectedReason)
                    {
                        Widgets.DrawHighlightSelected(rect2);
                    }
                    num += 80f;
                    num2++;
                }
                if (Event.current.type == EventType.Layout)
                {
                    reasonsListViewRect.height = num;
                }
                Widgets.EndScrollView();
            }
            else
            {
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(reasonsListRect, "WNFoS.Key.NoReasonsSpecified".Translate());
                Text.Anchor = TextAnchor.UpperLeft;
            }

            if (Widgets.ButtonText(new Rect((inRect.width / 3) - (180f / 2), inRect.yMax - Text.LineHeight * 6, 180f, Text.LineHeight * 2), "WNFoS.Key.Add".Translate()))
            {
                if (curReasonDef == null)
                {
                    SoundDefOf.ClickReject.PlayOneShotOnCamera();
                    Messages.Message("WNFoS.Key.NoReasonType".Translate(), MessageTypeDefOf.RejectInput);
                }
                else
                {
                    SoundDefOf.Click.PlayOneShotOnCamera();
                    if (curOrderReasons.Where(x => x.Def == curReasonDef).Any())
                    {
                        SoundDefOf.ClickReject.PlayOneShotOnCamera();
                        Messages.Message("WNFoS.Key.AlreadyAddedReasonType".Translate(), MessageTypeDefOf.RejectInput);
                    }
                    else
                    {
                        curOrderReasons.Add(new WNFoS_OrderReason()
                        {
                            Def = curReasonDef,
                            Name = curReasonName,
                            Description = curReasonDescription
                        });
                    }
                }
            }
            if (Widgets.ButtonText(new Rect((inRect.width * 2 / 3) - (180f / 2), inRect.yMax - Text.LineHeight * 6, 180f, Text.LineHeight * 2), "WNFoS.Key.Remove".Translate()))
            {
                
                if (selectedReason == null)
                {
                    SoundDefOf.ClickReject.PlayOneShotOnCamera();
                    Messages.Message("WNFoS.Key.NoReasonSelected".Translate(), MessageTypeDefOf.RejectInput);
                }
                else
                {
                    SoundDefOf.Click.PlayOneShotOnCamera();
                    curOrderReasons.Remove(selectedReason);
                    selectedReason = null;
                }
            }

            if (Widgets.ButtonText(new Rect((inRect.width / 3) - (180f / 2), inRect.yMax - Text.LineHeight * 2, 180f, Text.LineHeight * 2), "WNFoS.Key.Back".Translate()))
            {
                SoundDefOf.Click.PlayOneShotOnCamera();
                orderDefSelectionDone = false;
            }
            if (Widgets.ButtonText(new Rect((inRect.width * 2 / 3) - (180f / 2), inRect.yMax - Text.LineHeight * 2, 180f, Text.LineHeight * 2), "WNFoS.Key.Confirm".Translate()))
            {
                if (curOrderReasons.Count == 0)
                {
                    SoundDefOf.ClickReject.PlayOneShotOnCamera();
                    Messages.Message("WNFoS.Key.NoReasons".Translate(), MessageTypeDefOf.RejectInput);
                }
                else
                {
                    SoundDefOf.Click.PlayOneShotOnCamera();
                    WNFoS_Order newOrder = new WNFoS_Order();
                    newOrder.SetOrder(curPawn, issuingFaction, curOrderDef, curOrderReasons, curOrderName, curOrderDetails);
                    WNFoS_WorldComponent_FleeOnSightComponent fosComponent = Find.World.GetComponent<WNFoS_WorldComponent_FleeOnSightComponent>();
                    fosComponent?.fosOrders.Add(newOrder);
                    Messages.Message("WNFoS.Key.OrderIssued".Translate(), MessageTypeDefOf.TaskCompletion);
                    orderReasonsSelectionDone = true;
                    Close();
                }
            }
        }
    }
}
