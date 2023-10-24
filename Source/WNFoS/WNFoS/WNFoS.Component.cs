using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using NarutoMod;
using RimWorld;
using RimWorld.Planet;
using TaranMagicFramework;
using Verse;
using AbilityDef = TaranMagicFramework.AbilityDef;
using UnityEngine.Profiling;

namespace WNFoS
{
    public class WNFoS_WorldComponent_FleeOnSightComponent : WorldComponent
    {
        public List<WNFoS_Order> fosOrders;

        public WNFoS_WorldComponent_FleeOnSightComponent(World world) : base(world) { 
        
        }

        public List<WNFoS_Order> Orders
        {
            get
            {
                return fosOrders;
            }
        }

        // Test lmoa remove when done
        public void YeTestSharinganBro()
        {
            foreach (Faction faction in Find.FactionManager.AllFactionsListForReading.Where(x => (x.leader != null) && !x.IsPlayer))
                    //foreach (Faction faction in Find.FactionManager.AllFactionsListForReading.Where(x => x.def == WN_DefOf.WN_NarutoFaction))
            {
                foreach (Pawn pawn in PawnsFinder.AllMaps_FreeColonists)
                {
                    if (pawn.genes.HasEndogene(WN_DefOf.WN_Sharingan))
                    {
                        if (fosOrders.Where(x => x.Pawn == pawn && x.Faction == faction).Count() == 0)
                        {
                            WNFoS_Order newOrder = new WNFoS_Order();
                            List<WNFoS_OrderReason> orderReasons = new List<WNFoS_OrderReason>()
                            {
                                new WNFoS_OrderReason()
                                {
                                    //Name = "UwU Deadly Jutsu X3",
                                    //Description = "Nuzzles you notices your bulge owo what's this",
                                    Def = WNFoS_DefOf.WNFoS_OrderReason_DeadlyJutsu
                                }
                            };
                            newOrder.SetOrder(pawn, faction, WNFoS_DefOf.WNFoS_Order_FleeOnSight, orderReasons);
                            fosOrders.Add(newOrder);

                            Log.Error("New Flee On Sight Order made | Pawn: " + newOrder.Pawn + " | Faction: " + newOrder.Faction + " | Name: " + newOrder.Def.defName + " | Reasons: " + newOrder.OrderReasons.First().Name);
                        }
                        else
                        {
                            //foreach (WNFoS_Order order in fosOrders.Where(x => x.Pawn == pawn && x.Faction == faction))
                            //{
                            //    foreach (WNFoS_OrderReason reason in order.OrderReasons)
                            //    {
                            //        reason.ResetCustomInformation();
                            //    }
                            //}
                            UpdateOrders();
                            continue;
                        }
                    }
                }
            }
        }

        public override void FinalizeInit()
        {
            base.FinalizeInit();
            if (fosOrders == null)
            {
                fosOrders = new List<WNFoS_Order>();
            }
            else
            {
                //ResolveErrors();
            }
        }

        public override void WorldComponentTick()
        {
            base.WorldComponentTick();
            if (Find.TickManager.TicksGame % 2500 == 0)
            {
                //YeTestSharinganBro();
                //Resolve();
                UpdateOrders();
            }
        }

        private void UpdateOrders()
        {
            foreach (WNFoS_Order order in fosOrders)
            {
                order.TryUpdate();
            }
            ResolveErrors();
        }

        private void ResolveErrors()
        {
            List<WNFoS_Order> removeList = new List<WNFoS_Order>();
            foreach (WNFoS_Order order in fosOrders)
            {
                if (order.Pawn == null || order.Faction == null || order.Def == null || order.OrderReasons == null || order.IssuingPawn == null)
                {
                    removeList.Add(order);
                }
            }
            foreach (Faction faction in Find.FactionManager.AllFactionsListForReading.Where(x => x.leader != null))
            {
                Dictionary<Tuple<Pawn, WNFoS_OrderDef>, WNFoS_Order> pawnOrder = new Dictionary<Tuple<Pawn, WNFoS_OrderDef>, WNFoS_Order>();
                foreach (WNFoS_Order order in fosOrders.Where(x => x.Faction == faction && x.Def != WNFoS_DefOf.WNFoS_Order_Custom).OrderByDescending(x => x.TicksGameCreated))
                {
                    if (pawnOrder.TryGetValue(new Tuple<Pawn, WNFoS_OrderDef>(order.Pawn, order.Def)) == null)
                    {
                        pawnOrder.Add(new Tuple<Pawn, WNFoS_OrderDef>(order.Pawn, order.Def), order);
                    }
                    else if (order.Faction == faction)
                    {
                        removeList.Add(order);
                    }
                }
            }
            if (removeList.Count > 0)
            {
                foreach (WNFoS_Order order in removeList)
                {
                    fosOrders.Remove(order);
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look<WNFoS_Order>(ref fosOrders, "WNFoS_FoSOrders.Orders", LookMode.Deep);
        }
    }

    public class WNFoS_Order : IExposable
    {
        private Pawn pawn;
        private Faction faction;
        private WNFoS_OrderDef def;
        private List<WNFoS_OrderReason> orderReasons;

        private Pawn issuingPawn;
        private string name;
        private string details;

        private int tickGameCreated;

        public Pawn Pawn
        {
            get
            {
                return pawn;
            }
        }

        public Faction Faction
        {
            get
            {
                return faction;
            }
        }

        public WNFoS_OrderDef Def
        {
            get
            {
                return def;
            }
        }

        public List<WNFoS_OrderReason> OrderReasons
        {
            get
            {
                return orderReasons;
            }
        }

        public Pawn IssuingPawn
        {
            get
            {
                return issuingPawn;
            }
        }

        public string Name
		{
			get
			{
                if (name != null)
                {
                    return name;
                }
                return def.LabelCap;
			}
            set
            {
                name = value;
            }
        }

        public string Details
        {
            get
            {
                if (details != null)
                {
                    return details;
                }
                else if (def.details != null)
                {
                    return def.details;
                }
                return def.description;
            }
            set
            {
                details = value;
            }
        }

        public int TicksGameCreated
        {
            get
            {
                return tickGameCreated;
            }
        }

        public Color Color
        {
            get
            {
                if (faction != null)
                {
                    return faction.Color;
                }
                return Color.white;
            }
        }

        public void TryUpdate()
        {
            UpdateLethalityOrderReasons();
            UpdateAbilityClassesOrderReasons();
        }

        private void UpdateLethalityOrderReasons()
        {
            int killCount = (int)pawn.records.GetValue(WNFoS_DefOf.KillsHumanlikes);
            string deadlyNinjaReasonName = null;
            WNFoS_OrderReasonDef deadlyNinjaReasonDef = null;
            IEnumerable<WNFoS_OrderReasonDef> orderReasonDefList = DefDatabase<WNFoS_OrderReasonDef>.AllDefsListForReading.Where(x => x.defName.Contains("WNFoS_OrderReason_DeadlyNinja")).OrderByDescending(x => x.reasonRequirements.requiredKills);
            foreach (WNFoS_OrderReasonDef reasonDef in orderReasonDefList)
            {
                if (reasonDef.reasonRequirements.requiredKills <= killCount)
                {
                    deadlyNinjaReasonName = ("WNFoS.Key.OrderReason." + (string)reasonDef.defName.Replace("WNFoS_OrderReason_", "")).Translate();
                    deadlyNinjaReasonDef = reasonDef;
                    break;
                }
                else
                {
                    continue;
                }
            };
            if (deadlyNinjaReasonName == null | deadlyNinjaReasonName == null)
            {
                return;
            }
            if (orderReasons.Where(x => x.Def.defName.Contains("WNFoS_OrderReason_DeadlyNinja")).Where(x => x.Def.reasonRequirements.requiredKills > 1).Any())
            {
                WNFoS_OrderReason firstOrderReason = orderReasons.Where(x => x.Def.defName.Contains("WNFoS_OrderReason_DeadlyNinja")).Where(x => x.Def.reasonRequirements.requiredKills > 1).First();
                firstOrderReason.Name = deadlyNinjaReasonName;
                firstOrderReason.Def = deadlyNinjaReasonDef;
                List<WNFoS_OrderReason> currentList = orderReasons.Where(x => x.Def.defName.Contains("WNFoS_OrderReason_DeadlyNinja")).Where(x => x.Def.reasonRequirements.requiredKills > 1).ToList();
                foreach (WNFoS_OrderReason orderReason in currentList)
                {
                    if (currentList.First() == orderReason)
                    {
                        continue;
                    }
                    else
                    {
                        orderReasons.Remove(orderReason);
                    }
                }
            }
            else
            {
                orderReasons.Add(new WNFoS_OrderReason()
                {
                    Name = deadlyNinjaReasonName,
                    Def = deadlyNinjaReasonDef
                });
            }
        }

        private void UpdateAbilityClassesOrderReasons()
        {
            foreach (AbilityClass abilityClass in pawn.GetUnlockedAbilityClasses().Where(x => x.def.usesLevelSystem))
            {
                string abilityClassMasteryReasonName = null;
                WNFoS_OrderReasonDef abilityClassMasteryReasonDef = null;
                IEnumerable<WNFoS_OrderReasonDef> orderReasonDefList = DefDatabase<WNFoS_OrderReasonDef>.AllDefsListForReading.Where(x => x.defName.Contains("WNFoS_OrderReason_" + (string)abilityClass.def.LabelCap + "Mastery")).OrderByDescending(x => x.reasonRequirements.requiredAbilityClassLevels.requiredLevel);
                foreach (WNFoS_OrderReasonDef reasonDef in orderReasonDefList)
                {
                    if (reasonDef.reasonRequirements.requiredAbilityClassLevels.requiredLevel <= abilityClass.level)
                    {
                        abilityClassMasteryReasonName = ("WNFoS.Key.OrderReason." + (string)reasonDef.defName.Replace("WNFoS_OrderReason_", "")).Translate();
                        abilityClassMasteryReasonDef = reasonDef;
                        break;
                    }
                    else
                    {
                        continue;
                    }
                };
                if (abilityClassMasteryReasonName == null | abilityClassMasteryReasonDef == null)
                {
                    continue;
                }
                if (orderReasons.Where(x => x.Def.defName.Contains("WNFoS_OrderReason_" + (string)abilityClass.def.LabelCap + "Mastery")).Any())
                {
                    WNFoS_OrderReason firstOrderReason = orderReasons.Where(x => x.Def.defName.Contains("WNFoS_OrderReason_" + (string)abilityClass.def.LabelCap + "Mastery")).First();
                    firstOrderReason.Name = abilityClassMasteryReasonName;
                    firstOrderReason.Def = abilityClassMasteryReasonDef;
                    List<WNFoS_OrderReason> currentList = orderReasons.Where(x => x.Def.defName.Contains("WNFoS_OrderReason_" + (string)abilityClass.def.LabelCap + "Mastery")).ToList();
                    foreach (WNFoS_OrderReason orderReason in currentList)
                    {
                        if (currentList.First() == orderReason)
                        {
                            continue;
                        }
                        else
                        {
                            orderReasons.Remove(orderReason);
                        }
                    }
                }
                else
                {
                    orderReasons.Add(new WNFoS_OrderReason()
                    {
                        Name = abilityClassMasteryReasonName,
                        Def = abilityClassMasteryReasonDef
                    });
                }
            }
        }

        public void SetOrder(Pawn pawn, Faction faction, WNFoS_OrderDef def, List<WNFoS_OrderReason> orderReasons, string name = null, string details = null)
        {
            this.pawn = pawn;
            this.faction = faction;
            this.def = def;
            this.orderReasons = orderReasons;
            if (faction.leader != null)
            {
                issuingPawn = faction.leader;
            }
            this.name = name;
            this.details = details;
            this.tickGameCreated = Find.TickManager.TicksGame;
        }

        public void ExposeData()
        {
            Scribe_References.Look<Pawn>(ref pawn, "WNFoS_FoSOrder.Pawn");
            Scribe_References.Look<Faction>(ref faction, "WNFoS_FoSOrder.Faction");
            Scribe_Defs.Look<WNFoS_OrderDef>(ref def, "WNFoS_FoSOrder.Def");
            Scribe_Collections.Look<WNFoS_OrderReason>(ref orderReasons, "WNFoS_FoSOrder.OrderReasons", LookMode.Deep);
            Scribe_References.Look<Pawn>(ref issuingPawn, "WNFoS_FoSOrder.IssuingPawn");
            Scribe_Values.Look<string>(ref name, "WNFoS_FoSOrder.Name");
            Scribe_Values.Look<string>(ref details, "WNFoS_FoSOrder.Details");
            Scribe_Values.Look<int>(ref tickGameCreated, "WNFoS_FoSOrder.TickGameCreated");
        }
    }

    public class WNFoS_OrderDef : Def
    {
        public string details;
        public string orderTypeDescription;
        public WNFoS_OrderExceptions orderExceptions;
    }

    public class WNFoS_OrderExceptions
    {
        public bool requiresAllExceptions;
        public List<RoyalTitleDef> exceptedRoyalTitles;
        public List<AbilityClassLevelRequirement> exceptedMinimumLevels;
        public List<AbilityDef> exceptedAbilities;
        public List<GeneDef> exceptedGenes;
        public List<HediffDef> exceptedHediffs;
        public List<TraitDef> exceptedTraits;
    }

    public class WNFoS_OrderReason : IExposable
    {
        private string name;
        private string description;
        private WNFoS_OrderReasonDef def;

        public string Name
        {
            get
            {
                if (name != null)
                {
                    return name;
                }
                return Def.LabelCap;
            }
            set
            {
                name = value;
            }
        }

        public string Description
        {
            get
            {
                if (description != null)
                {
                    return description;
                }
                return Def.description;
            }
            set
            {
                description = value;
            }
        }

        public WNFoS_OrderReasonDef Def
        {
            get
            {
                return def;
            }
            set
            {
                def = value;
            }
        }

        public void ResetCustomInformation()
        {
            name = null;
            description = null;
        }

        public void ExposeData()
        {
            Scribe_Values.Look<string>(ref name, "WNFoS_FoSOrderReason.Name");
            Scribe_Values.Look<string>(ref description, "WNFoS_FoSOrderReason.Description");
            Scribe_Defs.Look<WNFoS_OrderReasonDef>(ref def, "WNFoS_FoSOrderReason.Def");
        }
    }

    public class WNFoS_OrderReasonDef : Def
    {
        public int priority;
        public bool selectable;

        [NoTranslate]
        public string reasonIconPath;
        [Unsaved(false)]
        private Texture2D reasonIcon;
        public bool drawAbilityIcon;

        public WNFoS_OrderReasonRequirements reasonRequirements;

        public Texture2D ReasonIcon
        {
            get
            {
                if (reasonIcon == null)
                {
                    if (!reasonIconPath.NullOrEmpty() && !drawAbilityIcon)
                    {
                        reasonIcon = ContentFinder<Texture2D>.Get(reasonIconPath);
                    }
                    else if (reasonRequirements.requiredAbility != null)
                    {
                        reasonIcon = reasonRequirements.requiredAbility.icon;
                    }
                    else
                    {
                        reasonIcon = BaseContent.BadTex;
                    }
                }
                return reasonIcon;
            }
        }
    }

    public class WNFoS_OrderReasonRequirements
    {
        public AbilityDef requiredAbility;
        public int requiredKills;
        public WNFoS_OrderReasonAbilityClassRequirements requiredAbilityClassLevels;
    }

    public class WNFoS_OrderReasonAbilityClassRequirements
    {
        public AbilityClassDef requiredAbilityClass;
        public int requiredLevel;
    }
}
