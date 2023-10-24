//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using RimWorld;
//using RimWorld.Planet;
//using RuntimeAudioClipLoader;
//using Verse;

//namespace LucianRoyalty
//{
//    public class LucianRoyalty_GameComponent_DynastyTracker : GameComponent
//    {
//        public HashSet<string> ExistingDynasties = new HashSet<string>(); // All current dynasties
//        public Dictionary<Pawn, string> DynastyHeads = new Dictionary<Pawn, string>(); // Dynasty heads
//        public Dictionary<Pawn, string> PawnDynasties = new Dictionary<Pawn, string>(); // Pawn and Dynasty // Primary checker for pawn's dynasty
//        public Dictionary<Pawn, int> OrderOfSuccession = new Dictionary<Pawn, int>(); // Pawn and Position in succession
//        public Dictionary<int, string> RulerHistory = new Dictionary<int, string>(); // nth Ruler and Name // Update during important events (coronation?)
//        public string RulingDynasty;
//        public Pawn RulingPawn;

//        public int tickCounter = 0;
//        public int tickInterval = 300;

//        public LucianRoyalty_GameComponent_DynastyTracker(Game game)
//        {
//        }

//        public override void FinalizeInit()
//        {
//            // Clear order of succession
//            OrderOfSuccession.Clear();
//            base.FinalizeInit();
//        }

//        public override void GameComponentTick()
//        {
//            this.tickCounter++;
//            bool updateTick = this.tickCounter > this.tickInterval;
//            if (updateTick)
//            {
//                foreach (Pawn pawn in PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_Colonists)
//                {
//                    LucianRoyalty_Core.Log(this.GetDynasty(pawn, isNoble(pawn)));
//                }

//                LucianRoyalty_Core.Log(this.GetRulingDynasty());

//                // Primogeniture test
//                Precept_Role precept_Role = Current.Game.World.factionManager.OfPlayer.ideos.PrimaryIdeo.GetPrecept(PreceptDefOf.IdeoRole_Leader) as Precept_Role;
//                Pawn currentMonarch = precept_Role.ChosenPawnSingle();

//                List<Pawn> BloodRelatives = new List<Pawn>(currentMonarch.relations.FamilyByBlood);
//                foreach (Pawn pawn in BloodRelatives)
//                {
//                    LucianRoyalty_Core.Log(pawn.ToString());
//                }

//                if (!DynastyHeads.ContainsKey(currentMonarch))
//                {
//                    DynastyHeads.Add(currentMonarch, PawnNamingUtility.GetLastName(currentMonarch));
//                }
//                GetPrimogenitureSuccession(currentMonarch);
//                LucianRoyalty_Core.Log(string.Format("The order of succession is as follows:"));

//                foreach (Pawn pawn in OrderOfSuccession.Keys)
//                {
//                    LucianRoyalty_Core.Log(pawn + " " + OrderOfSuccession[pawn].ToString());
//                }

//                this.tickCounter = 0;
//            }
//        }

//        private string GetRulingDynasty()
//        {
//            if (RulingDynasty != null)
//            {
//                // Check if not already part of existing dynasties
//                if (!ExistingDynasties.Contains(RulingDynasty))
//                {
//                    ExistingDynasties.Add(RulingDynasty);
//                }
//                return RulingDynasty;
//            }
//            else
//            {
//                return "LucianRoyalty_Translate_DynastyNoRuling".Translate();
//            }
//        }

//        private Pawn GetRulingPawn()
//        {
//            if (RulingPawn != null)
//            {
//                // Check if not already part of existing dynasties
//                if (!PawnDynasties.ContainsKey(RulingPawn))
//                {
//                    // Check
//                    if (!ExistingDynasties.Contains(GetDynasty(RulingPawn, isNoble(RulingPawn))))
//                    {

//                    }
//                    //PawnDynasties.Add(RulingDynasty);
//                }
//                return RulingPawn;
//            }
//            else
//            {
//                return null;
//            }
//        }

//        private void GetPrimogenitureSuccession(Pawn pawn)
//        {
//            // Identify if dynasty head and find relatives regardless of dynasty
//            if (DynastyHeads.ContainsKey(pawn))
//            {
//                List<Pawn> BloodRelatives = new List<Pawn>(pawn.relations.FamilyByBlood);
//                Dictionary<Pawn, float> Children = new Dictionary<Pawn, float>(); // Child and Age in ticks
//                Dictionary<Pawn, float> Siblings = new Dictionary<Pawn, float>(); // Siblings and Age in ticks
//                Dictionary<Pawn, float> FatherDynasticSiblings = new Dictionary<Pawn, float>(); // Siblings of father and Age in ticks
//                Dictionary<Pawn, float> MotherDynasticSiblings = new Dictionary<Pawn, float>(); // Siblings of mother and Age in ticks
//                Dictionary<Pawn, float> FatherDynasticParents = new Dictionary<Pawn, float>(); // Parents of mother and Age in ticks
//                Dictionary<Pawn, float> MotherDynasticParents = new Dictionary<Pawn, float>(); // Parents of mother and Age in ticks
//                Dictionary<Pawn, float> DistantKin = new Dictionary<Pawn, float>(); // Distant kin and Age in ticks
//                Pawn father = null;
//                Pawn grandfatherFatherSide = null;
//                Pawn grandmotherFatherSide = null;
//                Pawn mother = null;
//                Pawn grandfatherMotherSide = null;
//                Pawn grandmotherMotherSide = null;

//                Pawn mainLine = null;
//                bool hasFather = pawn.GetFather() != null;
//                bool hasMother = pawn.GetMother() != null;
//                bool hasPaternalGrandfather = pawn.GetFather().GetFather != null;
//                bool hasPaternalGrandmother = pawn.GetFather().GetMother != null;
//                bool hasMaternalGrandfather = pawn.GetMother().GetFather != null;
//                bool hasMaternalGrandmother = pawn.GetMother().GetMother != null;
//                if (hasFather)
//                {
//                    father = pawn.GetFather();
//                    if (hasPaternalGrandfather)
//                    {
//                        grandfatherFatherSide = father.GetFather();
//                    }
//                    if (hasPaternalGrandmother)
//                    {
//                        grandmotherFatherSide = father.GetMother();
//                    }
//                }
//                if (hasMother)
//                {
//                    mother = pawn.GetMother();
//                    if (hasMaternalGrandfather)
//                    {
//                        grandfatherMotherSide = mother.GetFather();
//                    }
//                    if (hasMaternalGrandmother)
//                    {
//                        grandmotherMotherSide = mother.GetMother();
//                    }
//                }

//                //// Purge dictionaries
//                Children.Clear();
//                Siblings.Clear();
//                FatherDynasticSiblings.Clear();
//                MotherDynasticSiblings.Clear();
//                FatherDynasticParents.Clear();
//                MotherDynasticParents.Clear();
//                DistantKin.Clear();

//                // DETERMINING FIRST LINE
//                // The first or main line will be used to determine checks for issue, e.g. children of a dead eligible heir take precedence over siblings
//                // CURRENT ORDER:
//                // 1. Children (and issue)
//                // 2. Siblings (and issue)
//                // 3. Aunts/Uncles from main line (and issue)
//                // 4. Grandparents (assumed dead/skipped but eligible siblings may be alive)
//                // 5. Distant kin
//                // Add to dictionaries according to relations
//                foreach (Pawn bloodRelative in BloodRelatives)
//                {
//                    if (PawnRelationDefOf.Child.Worker.InRelation(pawn, bloodRelative))
//                    {
//                        Children.Add(bloodRelative, bloodRelative.ageTracker.AgeChronologicalYearsFloat);
//                    }
//                    else if (PawnRelationDefOf.Sibling.Worker.InRelation(pawn, bloodRelative) || PawnRelationDefOf.HalfSibling.Worker.InRelation(pawn, bloodRelative))
//                    {
//                        // TO-DO: Add criteria for checking if part of main line or not
//                        Siblings.Add(bloodRelative, bloodRelative.ageTracker.AgeChronologicalYearsFloat);
//                    }
//                    else if (PawnRelationDefOf.UncleOrAunt.Worker.InRelation(pawn, bloodRelative))
//                    {
//                        // Determine if father and mother-side aunts or uncles are of dynasty
//                        // TO-DO: Add criteria for checking if part of main line or not
//                        if (hasFather)
//                        {
//                            if (PawnDynasties.ContainsKey(father) && PawnDynasties.ContainsKey(pawn))
//                            {
//                                if (PawnDynasties[father] == PawnDynasties[pawn])
//                                {
//                                    FatherDynasticSiblings.Add(bloodRelative, bloodRelative.ageTracker.AgeChronologicalYearsFloat);
//                                }
//                            }
//                        }
//                        if (hasMother)
//                        {
//                            if (PawnDynasties.ContainsKey(mother) && PawnDynasties.ContainsKey(pawn))
//                            {
//                                if (PawnDynasties[mother] == PawnDynasties[pawn])
//                                {
//                                    MotherDynasticSiblings.Add(bloodRelative, bloodRelative.ageTracker.AgeChronologicalYearsFloat);
//                                }
//                            }
//                        }
//                    }
//                    else if (PawnRelationDefOf.Grandparent.Worker.InRelation(pawn, bloodRelative))
//                    {
//                        // Determine if father and mother-side grandparents are of dynasty
//                        // TO-DO: Add criteria for checking if part of main line or not, may especially streamline succession determination
//                        if (hasFather)
//                        {
//                            if (hasPaternalGrandfather)
//                            {
//                                if (PawnDynasties.ContainsKey(grandfatherFatherSide) && PawnDynasties.ContainsKey(pawn))
//                                {
//                                    if (PawnDynasties[grandfatherFatherSide] == PawnDynasties[pawn])
//                                    {
//                                        FatherDynasticParents.Add(bloodRelative, bloodRelative.ageTracker.AgeChronologicalYearsFloat);
//                                    }
//                                }
//                            }
//                            if (hasPaternalGrandmother)
//                            {
//                                if (PawnDynasties.ContainsKey(grandmotherFatherSide) && PawnDynasties.ContainsKey(pawn))
//                                {
//                                    if (PawnDynasties[grandmotherFatherSide] == PawnDynasties[pawn])
//                                    {
//                                        FatherDynasticParents.Add(bloodRelative, bloodRelative.ageTracker.AgeChronologicalYearsFloat);
//                                    }
//                                }
//                            }
//                        }
//                        if (hasMother)
//                        {
//                            if (hasMaternalGrandfather)
//                            {
//                                if (PawnDynasties.ContainsKey(grandfatherMotherSide) && PawnDynasties.ContainsKey(pawn))
//                                {
//                                    if (PawnDynasties[grandfatherMotherSide] == PawnDynasties[pawn])
//                                    {
//                                        MotherDynasticParents.Add(bloodRelative, bloodRelative.ageTracker.AgeChronologicalYearsFloat);
//                                    }
//                                }
//                            }
//                            if (hasMaternalGrandmother)
//                            {
//                                if (PawnDynasties.ContainsKey(grandmotherMotherSide) && PawnDynasties.ContainsKey(pawn))
//                                {
//                                    if (PawnDynasties[grandmotherMotherSide] == PawnDynasties[pawn])
//                                    {
//                                        MotherDynasticParents.Add(bloodRelative, bloodRelative.ageTracker.AgeChronologicalYearsFloat);
//                                    }
//                                }
//                            }
//                        }
//                    }
//                    else if (PawnRelationDefOf.Kin.Worker.InRelation(pawn, bloodRelative))
//                    {
//                        // Dynastic check for distant kin
//                        if (PawnDynasties.ContainsKey(bloodRelative) && PawnDynasties.ContainsKey(pawn))
//                        {
//                            if (PawnDynasties[bloodRelative] == PawnDynasties[pawn])
//                            {
//                                DistantKin.Add(bloodRelative, bloodRelative.ageTracker.AgeChronologicalYearsFloat);
//                            }
//                        }
//                    }
//                }

//                // Clear order of succession
//                OrderOfSuccession.Clear();

//                // Determine main line
//                // NOTE: Assumes that if no children, there are no grandchildren as checks should consider even dead pawns
//                int SuccessionOrder = -1;
//                if (Children.Any())
//                {
//                    var OldestChild = Children.OrderByDescending(x => x.Value).ElementAt(0).Key; // Firstborn
//                    mainLine = OldestChild;
//                    TryAddSuccession(OldestChild);


//                    //for (int i = 0; i < Children.Count; i++)
//                    //{
//                    //    var currentChild = Children.OrderByDescending(x => x.Value).ElementAt(0).Key;
//                    //    TryAddSuccession(currentChild);
//                    //    ExhaustChildren(currentChild);
//                    //}


//                }
//                else if (Siblings.Any())
//                {
//                    var OldestSibling = Siblings.OrderByDescending(x => x.Value).ElementAt(0).Key; // Next sibling
//                    mainLine = OldestSibling;
//                    TryAddSuccession(OldestSibling);
//                }
//                else if (FatherDynasticSiblings.Any())
//                {
//                    var OldestFatherSibling = FatherDynasticSiblings.OrderByDescending(x => x.Value).ElementAt(0).Key; // Next aunt/uncle
//                    mainLine = OldestFatherSibling;
//                    TryAddSuccession(OldestFatherSibling);
//                }
//                else if (MotherDynasticSiblings.Any())
//                {
//                    var OldestMotherSibling = MotherDynasticSiblings.OrderByDescending(x => x.Value).ElementAt(0).Key; // Next aunt/uncle
//                    mainLine = OldestMotherSibling;
//                    TryAddSuccession(OldestMotherSibling);
//                }
//                else if (FatherDynasticParents.Any())
//                {
//                    var OldestFatherParent = FatherDynasticParents.OrderByDescending(x => x.Value).ElementAt(0).Key; // Next grandaunt/granduncle
//                    mainLine = OldestFatherParent;
//                    TryAddSuccession(OldestFatherParent);
//                }
//                else if (MotherDynasticParents.Any())
//                {
//                    var OldestMotherParent = MotherDynasticParents.OrderByDescending(x => x.Value).ElementAt(0).Key; // Next grandaunt/granduncle
//                    mainLine = OldestMotherParent;
//                    TryAddSuccession(OldestMotherParent);
//                }
//                else if (DistantKin.Any())
//                {
//                    var OldestKin = DistantKin.OrderByDescending(x => x.Value).ElementAt(0).Key; // Next eligible distant kin
//                    mainLine = OldestKin;
//                    TryAddSuccession(OldestKin);
//                }
//                else
//                {
//                    // No eligible next of kin left of dynasty
//                    // TO-DO: Pass to next eligible dynasty with blood relations
//                    mainLine = null;
//                }

//                // Check for children of first line then determine succeeding heirs
//                //if (mainLine != null)
//                //{
//                //    LucianRoyalty_Core.Log("PROOF THIS WORKS");
//                //    LucianRoyalty_Core.Log(mainLine.NameFullColored);
//                //    ExhaustChildren(mainLine);
//                //}

//                void TryAddSuccession(Pawn eligibleHeir)
//                {
//                    if (!eligibleHeir.Dead)
//                    {
//                        // Designate as heir apparent if alive
//                        SuccessionOrder++;
//                        OrderOfSuccession.Add(eligibleHeir, SuccessionOrder);
//                    }
//                }

//                void ExhaustChildren(Pawn pawn)
//                {
//                    // Assume input pawn is heir apparent
//                    Dictionary<Pawn, float> currentChildren = new Dictionary<Pawn, float>(); // Child and Age in ticks
//                    Dictionary<Pawn, float> currentSiblings = new Dictionary<Pawn, float>(); // Siblings and Age in ticks
//                    Dictionary<Pawn, float> currentPaternalAuntsOrUncles = new Dictionary<Pawn, float>(); // Aunts or uncles and Age in ticks
//                    Dictionary<Pawn, float> currentMaternalAuntsOrUncles = new Dictionary<Pawn, float>(); // Aunts or uncles and Age in ticks

//                    foreach (Pawn familyMember in pawn.relations.FamilyByBlood)
//                    {
//                        if (familyMember.GetFather() == pawn || familyMember.GetMother() == pawn)
//                        {
//                            // Children of pawn
//                            currentChildren.Add(familyMember, familyMember.ageTracker.AgeChronologicalYearsFloat);
//                        }
//                        //else if (familyMember.GetFather() == pawn.GetFather() || familyMember.GetMother() == pawn.GetMother())
//                        //{
//                        //    // Sibling of pawn
//                        //    currentSiblings.Add(familyMember, familyMember.ageTracker.AgeChronologicalYearsFloat);
//                        //}
//                    }

//                    foreach (Pawn pawntest in currentChildren.Keys)
//                    {
//                        LucianRoyalty_Core.Log("CHILD " + pawntest);
//                    }

//                    foreach (Pawn pawntest in currentSiblings.Keys)
//                    {
//                        LucianRoyalty_Core.Log("SIBLING " + pawntest);
//                    }


//                    if (currentChildren.Any())
//                    {
//                        for (int i = 0; i < currentChildren.Count; i++)
//                        {
//                            var currentChild = currentChildren.OrderByDescending(x => x.Value).ElementAt(i).Key; // Firstborn
//                            TryAddSuccession(currentChild);
//                            // If existing, exhausts firstborn's own children
//                            bool hasChild = currentChild.relations.FamilyByBlood.Where(x => x.GetFather() == currentChild || x.GetMother() == currentChild).Any();
//                            if (hasChild)
//                            {
//                                ExhaustChildren(currentChild);
//                            }
//                        }
//                    }
//                    //if (currentSiblings.Any())
//                    //{
//                    //    for (int i = 0; i < currentSiblings.Count; i++)
//                    //    {
//                    //        var currentSibling = currentSiblings.OrderByDescending(x => x.Value).ElementAt(i).Key; // Next sibling
//                    //        TryAddSuccession(currentSibling);
//                    //        // If existing, exhausts sibling's own children
//                    //        bool hasChild = currentSibling.relations.FamilyByBlood.Where(x => x.GetFather() == currentSibling || x.GetMother() == currentSibling).Any();
//                    //        if (hasChild)
//                    //        {
//                    //            ExhaustChildren(currentSibling);
//                    //        }
//                    //    }
//                    //}

//                }

//            }
//        }

//        private void GetUltimogenitureSuccession()
//        {

//        }

//        private void InitializeRulingDynasty()
//        {
//            // TO-DO: Give player option to name new dynasty
//            Precept_Role precept_Role = Current.Game.World.factionManager.OfPlayer.ideos.PrimaryIdeo.GetPrecept(PreceptDefOf.IdeoRole_Leader) as Precept_Role;
//            Pawn currentRuler = precept_Role.ChosenPawnSingle();
//            this.RulingPawn = currentRuler;

//            // Check if not part of any dynasty
//            if (!PawnDynasties.ContainsKey(currentRuler))
//            {

//            }
//            else if (!ExistingDynasties.Contains(GetDynasty(currentRuler, isNoble(currentRuler))))
//            {
//                // Hashset does not contain
//            }

//        }

//        private void InitializeDynasties()
//        {
//            // Determine dynasty heads

//            // Form dynasties

//            // Sort pawns into dynasties
//            foreach (Pawn pawn in PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_Colonists)
//            {
//                GetDynasty(pawn, isNoble(pawn));
//            }
//        }

//        private bool isNoble(Pawn pawn)
//        {
//            // Check if pawn is ruler first then part of any existing dynasties
//            // TO-DO: Check if pawn fulfills other criteria for nobility
//            if (pawn == RulingPawn)
//            {
//                return true;
//            }
//            else if (PawnDynasties.ContainsKey(pawn))
//            {
//                return true;
//            }
//            return false;
//        }

//        private string GetDynasty(Pawn pawn, bool isNoble)
//        {
//            if (PawnDynasties.ContainsKey(pawn))
//            {
//                return PawnDynasties[pawn];
//            }
//            else
//            {
//                // Not in the list, so verify if eligible for existing dynasties
//                foreach (string dynasty in ExistingDynasties)
//                {
//                    // Last name matches with existing dynasty
//                    bool newDynasty = false;
//                    if (PawnNamingUtility.GetLastName(pawn) == dynasty)
//                    {
//                        // Verify relations exist
//                        var dynasticPawns = PawnDynasties.Where(x => x.Value.Contains(dynasty)).Select(x => x.Key);
//                        foreach (Pawn dynastyMember in dynasticPawns)
//                        {
//                            if (pawn.relations.FamilyByBlood.Contains(dynastyMember) || pawn.relations.RelatedPawns.Contains(dynastyMember))
//                            {
//                                PawnDynasties.Add(pawn, dynasty);
//                            }
//                            else
//                            {
//                                newDynasty = true;
//                            }
//                        }
//                    }
//                    else
//                    {
//                        newDynasty = true;
//                    }
//                    if (newDynasty && isNoble)
//                    {
//                        // Establish new dynasty
//                        this.GenerateDynasty(pawn);
//                    }
//                }
//                if (!isNoble)
//                {
//                    return "LucianRoyalty_Translate_DynastyLowborn".Translate();
//                }
//                return (PawnNamingUtility.GetLastName(pawn)); // TO-DO: INCLUDE CHECK FOR NEW DYNASTY (IF NEW NAMING SCHEME)
//            }
//        }

//        private void GenerateDynasty(Pawn pawn)
//        {
//            // Add to dictionaries and hashsets
//        }

//        private void TryAddDynasty(Pawn pawn)
//        {
//            bool? pawnFlag;
//            if (pawn == null)
//            {
//                pawnFlag = null;
//            }
//            else
//            {
//                pawnFlag = pawn.genes.HasGene(LucianRoyalty_DefOf.LucianRoyalty_Gene_RoyalBlood);
//            }
//            bool? hashFlag = pawnFlag;
//            if (hashFlag.GetValueOrDefault())
//            {
//                //this.PawnDynasties.Add(pawn);
//            }
//        }
//    }

//    public class LucianRoyalty_GameComponent_LucianMonarchTracker : GameComponent
//    {
//        public HashSet<Pawn> EligibleLucians = new HashSet<Pawn>();

//        public Pawn immediateHeir;
//        public Pawn secondHeir;
//        public Pawn chosenLucian;

//        public int tickCounter = 0;
//        public int tickInterval = 300;

//        public LucianRoyalty_GameComponent_LucianMonarchTracker(Game game)
//        {
//        }

//        public override void FinalizeInit()
//        {
//            base.FinalizeInit();
//        }

//        public override void GameComponentTick()
//        {
//            this.tickCounter++;
//            bool updateTick = this.tickCounter > this.tickInterval;
//            if (updateTick)
//            {

//                this.DetermineEligible();
//                this.CalculateSuccession();
//                this.tickCounter = 0;

//                //Replace with manual action or event about being monarchless
//                //bool vacantMonarch = GetCurrentMonarch() == null;
//                //if (vacantMonarch)
//                //{
//                //    this.ImplementInheritance();
//                //}

//                if (this.immediateHeir != null)
//                {
//                    Messages.Message("LucianRoyalty_Translate_MessageHeirApparent".Translate(immediateHeir.NameFullColored + " " + immediateHeir.ageTracker.AgeChronologicalYears), MessageTypeDefOf.PositiveEvent, true);
//                }
//                else
//                {
//                    Messages.Message("LucianRoyalty_Translate_MessageHeirNone".Translate(), MessageTypeDefOf.PositiveEvent, true);
//                }
//                if (this.secondHeir != null)
//                {
//                    Messages.Message("LucianRoyalty_Translate_MessageSecondHeir".Translate(secondHeir.NameFullColored + " " + secondHeir.ageTracker.AgeChronologicalYears), MessageTypeDefOf.PositiveEvent, true);
//                }
//            }

//            //Replace with manual action or event about being monarchless
//            bool vacantMonarch = GetCurrentMonarch() == null;
//            if (vacantMonarch)
//            {
//                this.ImplementInheritance();
//            }
//        }

//        private Pawn GetCurrentMonarch()
//        {
//            Precept_Role precept_Role = Current.Game.World.factionManager.OfPlayer.ideos.PrimaryIdeo.GetPrecept(PreceptDefOf.IdeoRole_Leader) as Precept_Role;
//            Pawn currentMonarch = precept_Role.ChosenPawnSingle();
//            return currentMonarch;
//        }

//        private void TryAddEligible(Pawn pawn)
//        {
//            bool? pawnFlag;
//            if (pawn == null)
//            {
//                pawnFlag = null;
//            }
//            else
//            {
//                pawnFlag = pawn.genes.HasGene(LucianRoyalty_DefOf.LucianRoyalty_Gene_RoyalBlood);
//            }
//            bool? hashFlag = pawnFlag;
//            if (hashFlag.GetValueOrDefault())
//            {
//                this.EligibleLucians.Add(pawn);
//            }
//        }

//        private void ClearIneligible()
//        {
//            HashSet<Pawn> hashSetEligible = this.EligibleLucians;
//            if (hashSetEligible != null)
//            {
//                hashSetEligible.RemoveWhere((Pawn p) => ThingUtility.DestroyedOrNull(p) || p.Dead || !p.genes.HasGene(LucianRoyalty_DefOf.LucianRoyalty_Gene_RoyalBlood));
//            }
//        }

//        public override void ExposeData()
//        {
//            base.ExposeData();
//            Scribe_Collections.Look<Pawn>(ref this.EligibleLucians, "EligibleLucians", LookMode.Reference);
//            if (this.EligibleLucians == null)
//            {
//                this.EligibleLucians = new HashSet<Pawn>();
//            }
//            this.ClearIneligible();
//        }

//        private void DetermineEligible()
//        {
//            this.ClearIneligible();

//            bool biotechActive = ModsConfig.BiotechActive;
//            if (biotechActive)
//            {
//                Ideo primaryIdeo = Current.Game.World.factionManager.OfPlayer.ideos.PrimaryIdeo;
//                bool preceptLucian = primaryIdeo.HasPrecept(DefDatabase<PreceptDef>.GetNamedSilentFail("LucianRoyalty_Precept_LeaderLucian"));
//                if (preceptLucian)
//                {
//                    foreach (Pawn pawn in PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_Colonists)
//                    {
//                        bool isLucian = pawn.genes.HasGene(LucianRoyalty_DefOf.LucianRoyalty_Gene_RoyalBlood) && pawn.ideo.Ideo == primaryIdeo && !pawn.IsSlave;
//                        if (isLucian)
//                        {
//                            bool isAgnatic = primaryIdeo.HasPrecept(DefDatabase<PreceptDef>.GetNamedSilentFail("LucianRoyalty_Precept_GenderAgnatic"));
//                            bool isMatrilineal = primaryIdeo.HasPrecept(DefDatabase<PreceptDef>.GetNamedSilentFail("LucianRoyalty_Precept_GenderMatrilineal"));
//                            bool isAbsolute = primaryIdeo.HasPrecept(DefDatabase<PreceptDef>.GetNamedSilentFail("LucianRoyalty_Precept_GenderAbsolute"));
//                            if ((isAgnatic && pawn.gender.Equals(Gender.Male)) || (isMatrilineal && pawn.gender.Equals(Gender.Female)) || (isAbsolute))
//                            {
//                                TryAddEligible(pawn);
//                            }
//                            else
//                            {
//                                return;
//                            }
//                        }
//                    }
//                }
//            }
//        }

//        //Revise succession for primogeniture and ultimogeniture
//        private void CalculateSuccession()
//        {
//            if (this.EligibleLucians == null)
//            {
//                return;
//            }
//            else
//            {
//                Dictionary<Pawn, int> HouseMembers = new Dictionary<Pawn, int>();

//                Dictionary<Pawn, int> RoyalChildren = new Dictionary<Pawn, int>();
//                //Dictionary<Pawn, int> RoyalGrandchildren = new Dictionary<Pawn, int>();
//                Dictionary<Pawn, int> RoyalSibling = new Dictionary<Pawn, int>();
//                Dictionary<Pawn, int> RoyalNephewOrNiece = new Dictionary<Pawn, int>();
//                Dictionary<Pawn, int> RoyalUncleOrAunt = new Dictionary<Pawn, int>();
//                Dictionary<Pawn, int> RoyalCousin = new Dictionary<Pawn, int>();
//                Dictionary<Pawn, int> RoyalKin = new Dictionary<Pawn, int>();

//                Ideo primaryIdeo = Current.Game.World.factionManager.OfPlayer.ideos.PrimaryIdeo;
//                foreach (Pawn pawn in this.EligibleLucians)
//                {
//                    bool notSeniority = !primaryIdeo.HasPrecept(DefDatabase<PreceptDef>.GetNamedSilentFail("LucianRoyalty_Precept_SuccessionSeniority"));
//                    if (notSeniority)
//                    {
//                        bool isRoyalChild = PawnRelationDefOf.Child.Worker.InRelation(GetCurrentMonarch(), pawn);
//                        //bool isRoyalGrandchild = pawn.relations.DirectRelationExists(PawnRelationDefOf.Grandchild, GetCurrentMonarch());
//                        bool isRoyalSibling = PawnRelationDefOf.Sibling.Worker.InRelation(GetCurrentMonarch(), pawn);
//                        bool isRoyalNephewOrNiece = PawnRelationDefOf.NephewOrNiece.Worker.InRelation(GetCurrentMonarch(), pawn);
//                        bool isRoyalUncleOrAunt = PawnRelationDefOf.UncleOrAunt.Worker.InRelation(GetCurrentMonarch(), pawn);
//                        bool isRoyalCousin = PawnRelationDefOf.Cousin.Worker.InRelation(GetCurrentMonarch(), pawn);
//                        //bool isRoyalKin = pawn.relations.DirectRelationExists(PawnRelationDefOf.Kin, GetCurrentMonarch());

//                        if (isRoyalChild)
//                        {
//                            RoyalChildren.Add(pawn, pawn.ageTracker.AgeBiologicalYears);
//                        }
//                        //else if (isRoyalGrandchild)
//                        //{
//                        //    RoyalGrandchildren.Add(pawn, pawn.ageTracker.AgeBiologicalYears);
//                        //}
//                        else if (isRoyalSibling)
//                        {
//                            RoyalSibling.Add(pawn, pawn.ageTracker.AgeBiologicalYears);
//                        }
//                        else if (isRoyalNephewOrNiece)
//                        {
//                            RoyalNephewOrNiece.Add(pawn, pawn.ageTracker.AgeBiologicalYears);
//                        }
//                        else if (isRoyalUncleOrAunt)
//                        {
//                            RoyalUncleOrAunt.Add(pawn, pawn.ageTracker.AgeBiologicalYears);
//                        }
//                        else if (isRoyalCousin)
//                        {
//                            RoyalCousin.Add(pawn, pawn.ageTracker.AgeBiologicalYears);
//                        }
//                        else
//                        {
//                            RoyalKin.Add(pawn, pawn.ageTracker.AgeBiologicalYears);
//                        }
//                    }
//                    else
//                    {
//                        HouseMembers.Add(pawn, pawn.ageTracker.AgeBiologicalYears);
//                    }
//                }

//                bool isPrimogeniture = primaryIdeo.HasPrecept(DefDatabase<PreceptDef>.GetNamedSilentFail("LucianRoyalty_Precept_SuccessionPrimogeniture"));
//                bool isUltimogeniture = primaryIdeo.HasPrecept(DefDatabase<PreceptDef>.GetNamedSilentFail("LucianRoyalty_Precept_SuccessionUltimogeniture"));
//                bool isSeniority = primaryIdeo.HasPrecept(DefDatabase<PreceptDef>.GetNamedSilentFail("LucianRoyalty_Precept_SuccessionSeniority"));
//                if (isPrimogeniture)
//                {
//                    //Revise in the future -- consider children of dead firstborn, siblings, etc
//                    if (RoyalChildren.Count != 0)
//                    {
//                        Dictionary<Pawn, int> Grandchildren = new Dictionary<Pawn, int>();
//                        var OldestChild = RoyalChildren.OrderBy(x => x.Value).ElementAt(0).Key; //Firstborn or eldest living
//                        this.immediateHeir = OldestChild;

//                        foreach (Pawn pawn in this.EligibleLucians)
//                        {
//                            bool hasChild = PawnRelationDefOf.Child.Worker.InRelation(OldestChild, pawn);
//                            if (hasChild)
//                            {
//                                Grandchildren.Add(pawn, pawn.ageTracker.AgeBiologicalYears);
//                            }
//                        }
//                        if (Grandchildren.Count != 0)
//                        {
//                            var OldestGrandchild = Grandchildren.OrderBy(x => x.Value).ElementAt(0).Key; //Child of firstborn or eldest
//                            this.secondHeir = OldestGrandchild;
//                        }
//                        else if (RoyalChildren.Count >= 2)
//                        {
//                            var SecondOldestChild = RoyalChildren.OrderBy(x => x.Value).ElementAt(1).Key; //Next child
//                            this.secondHeir = SecondOldestChild;
//                        }
//                        else if (RoyalSibling.Count != 0)
//                        {
//                            var OldestSibling = RoyalSibling.OrderBy(x => x.Value).ElementAt(0).Key; //Oldest/next sibling of monarch
//                            this.secondHeir = OldestSibling;
//                        }
//                        else if (RoyalUncleOrAunt.Count != 0)
//                        {
//                            var OldestUncleOrAunt = RoyalUncleOrAunt.OrderBy(x => x.Value).ElementAt(0).Key; //Oldest/next uncle or aunt
//                            this.secondHeir = OldestUncleOrAunt;
//                        }
//                        else if (RoyalKin.Count != 0)
//                        {
//                            var OldestKin = RoyalKin.OrderBy(x => x.Value).ElementAt(0).Key; //Distant kin
//                            this.secondHeir = OldestKin;
//                        }
//                    }
//                    else if (RoyalSibling.Count != 0)
//                    {
//                        Dictionary<Pawn, int> NephewOrNieces = new Dictionary<Pawn, int>();
//                        var OldestSibling = RoyalSibling.OrderBy(x => x.Value).ElementAt(0).Key; //Oldest/next sibling of monarch
//                        this.immediateHeir = OldestSibling;

//                        foreach (Pawn pawn in this.EligibleLucians)
//                        {
//                            bool hasChild = PawnRelationDefOf.Child.Worker.InRelation(OldestSibling, pawn);
//                            if (hasChild)
//                            {
//                                NephewOrNieces.Add(pawn, pawn.ageTracker.AgeBiologicalYears);
//                            }
//                        }
//                        if (NephewOrNieces.Count != 0)
//                        {
//                            var OldestNephewOrNiece = NephewOrNieces.OrderBy(x => x.Value).ElementAt(0).Key; //Nephew or niece
//                            this.secondHeir = OldestNephewOrNiece;
//                        }
//                        else if (RoyalSibling.Count >= 2)
//                        {
//                            var SecondOldestSibling = RoyalSibling.OrderBy(x => x.Value).ElementAt(1).Key; //Next sibling
//                            this.secondHeir = SecondOldestSibling;
//                        }
//                        else if (RoyalUncleOrAunt.Count != 0)
//                        {
//                            var OldestUncleOrAunt = RoyalUncleOrAunt.OrderBy(x => x.Value).ElementAt(0).Key; //Oldest/next uncle or aunt
//                            this.secondHeir = OldestUncleOrAunt;
//                        }
//                        else if (RoyalKin.Count != 0)
//                        {
//                            var OldestKin = RoyalKin.OrderBy(x => x.Value).ElementAt(0).Key; //Distant kin
//                            this.secondHeir = OldestKin;
//                        }
//                    }
//                    else if (RoyalNephewOrNiece.Count != 0)
//                    {
//                        Dictionary<Pawn, int> GrandNephewOrNieces = new Dictionary<Pawn, int>();
//                        var OldestNephewOrNiece = RoyalNephewOrNiece.OrderBy(x => x.Value).ElementAt(0).Key; //Nephew or niece
//                        this.immediateHeir = OldestNephewOrNiece;

//                        foreach (Pawn pawn in this.EligibleLucians)
//                        {
//                            bool hasChild = PawnRelationDefOf.Child.Worker.InRelation(OldestNephewOrNiece, pawn);
//                            if (hasChild)
//                            {
//                                GrandNephewOrNieces.Add(pawn, pawn.ageTracker.AgeBiologicalYears);
//                            }
//                        }
//                        if (GrandNephewOrNieces.Count != 0)
//                        {
//                            var OldestGrandNephewOrNiece = GrandNephewOrNieces.OrderBy(x => x.Value).ElementAt(0).Key; //Child of nephew or niece
//                            this.secondHeir = OldestGrandNephewOrNiece;
//                        }
//                        else if (RoyalNephewOrNiece.Count >= 2)
//                        {
//                            var SecondOldestNephewOrNiece = RoyalNephewOrNiece.OrderBy(x => x.Value).ElementAt(1).Key; //Next nephew or niece of monarch
//                            this.secondHeir = SecondOldestNephewOrNiece;
//                        }
//                        else if (RoyalUncleOrAunt.Count != 0)
//                        {
//                            var OldestUncleOrAunt = RoyalUncleOrAunt.OrderBy(x => x.Value).ElementAt(0).Key; //Oldest/next uncle or aunt
//                            this.secondHeir = OldestUncleOrAunt;
//                        }
//                        else if (RoyalKin.Count != 0)
//                        {
//                            var OldestKin = RoyalKin.OrderBy(x => x.Value).ElementAt(0).Key; //Distant kin
//                            this.secondHeir = OldestKin;
//                        }
//                    }
//                    else if (RoyalUncleOrAunt.Count != 0)
//                    {
//                        Dictionary<Pawn, int> Cousins = new Dictionary<Pawn, int>();
//                        var OldestUncleOrAunt = RoyalUncleOrAunt.OrderBy(x => x.Value).ElementAt(0).Key; //Oldest/next uncle or aunt
//                        this.immediateHeir = OldestUncleOrAunt;

//                        foreach (Pawn pawn in this.EligibleLucians)
//                        {
//                            bool hasChild = PawnRelationDefOf.Child.Worker.InRelation(OldestUncleOrAunt, pawn);
//                            if (hasChild)
//                            {
//                                Cousins.Add(pawn, pawn.ageTracker.AgeBiologicalYears);
//                            }
//                        }
//                        if (Cousins.Count != 0)
//                        {
//                            var OldestCousin = Cousins.OrderBy(x => x.Value).ElementAt(0).Key; //Cousin
//                            this.secondHeir = OldestCousin;
//                        }
//                        else if (RoyalUncleOrAunt.Count >= 2)
//                        {
//                            var SecondOldestUncleOrAunt = RoyalUncleOrAunt.OrderBy(x => x.Value).ElementAt(1).Key; //Next uncle or aunt
//                            this.secondHeir = SecondOldestUncleOrAunt;
//                        }
//                        else if (RoyalKin.Count != 0)
//                        {
//                            var OldestKin = RoyalKin.OrderBy(x => x.Value).ElementAt(0).Key; //Distant kin
//                            this.secondHeir = OldestKin;
//                        }
//                    }
//                    else if (RoyalCousin.Count != 0)
//                    {
//                        Dictionary<Pawn, int> CousinChildren = new Dictionary<Pawn, int>();
//                        var OldestCousin = RoyalCousin.OrderBy(x => x.Value).ElementAt(0).Key; //Oldest cousin
//                        this.immediateHeir = OldestCousin;

//                        foreach (Pawn pawn in this.EligibleLucians)
//                        {
//                            bool hasChild = PawnRelationDefOf.Child.Worker.InRelation(OldestCousin, pawn);
//                            if (hasChild)
//                            {
//                                CousinChildren.Add(pawn, pawn.ageTracker.AgeBiologicalYears);
//                            }
//                        }
//                        if (CousinChildren.Count != 0)
//                        {
//                            var OldestCousinChild = CousinChildren.OrderBy(x => x.Value).ElementAt(0).Key; //Cousin's oldest child
//                            this.secondHeir = OldestCousinChild;
//                        }
//                        else if (RoyalCousin.Count >= 2)
//                        {
//                            var SecondOldestCousin = RoyalCousin.OrderBy(x => x.Value).ElementAt(1).Key; //Next cousin
//                            this.secondHeir = SecondOldestCousin;
//                        }
//                        else if (RoyalUncleOrAunt.Count != 0)
//                        {
//                            var OldestUncleOrAunt = RoyalUncleOrAunt.OrderBy(x => x.Value).ElementAt(0).Key; //Next uncle or aunt
//                            this.secondHeir = OldestUncleOrAunt;
//                        }
//                        else if (RoyalKin.Count != 0)
//                        {
//                            var OldestKin = RoyalKin.OrderBy(x => x.Value).ElementAt(0).Key; //Distant kin
//                            this.secondHeir = OldestKin;
//                        }
//                    }
//                    else if (RoyalKin.Count != 0)
//                    {
//                        Dictionary<Pawn, int> KinChildren = new Dictionary<Pawn, int>();
//                        var OldestKin = RoyalKin.OrderBy(x => x.Value).ElementAt(0).Key; //Distant kin
//                        this.immediateHeir = OldestKin;

//                        foreach (Pawn pawn in this.EligibleLucians)
//                        {
//                            bool hasChild = PawnRelationDefOf.Child.Worker.InRelation(OldestKin, pawn);
//                            if (hasChild)
//                            {
//                                KinChildren.Add(pawn, pawn.ageTracker.AgeBiologicalYears);
//                            }
//                        }
//                        if (KinChildren.Count != 0)
//                        {
//                            var OldestKinChild = KinChildren.OrderBy(x => x.Value).ElementAt(0).Key; //Kin's child
//                            this.secondHeir = OldestKinChild;
//                        }
//                        else if (RoyalKin.Count >= 2)
//                        {
//                            var SecondOldestKin = RoyalKin.OrderBy(x => x.Value).ElementAt(1).Key; //Distant kin
//                            this.secondHeir = SecondOldestKin;
//                        }
//                        else
//                        {
//                            this.secondHeir = null;
//                        }
//                    }
//                    else
//                    {
//                        this.immediateHeir = null;
//                        this.secondHeir = null;
//                    }
//                }
//                else if (isUltimogeniture)
//                {
//                    if (RoyalChildren != null)
//                    {
//                        var YoungestChild = RoyalChildren.Aggregate((x, y) => x.Value < y.Value ? x : y).Key;
//                        this.immediateHeir = YoungestChild;

//                        RoyalChildren.Remove(YoungestChild);
//                        var SecondYoungestChild = RoyalChildren.Aggregate((x, y) => x.Value < y.Value ? x : y).Key;
//                        this.secondHeir = SecondYoungestChild;
//                    }
//                }
//                else if (isSeniority)
//                {
//                    if (HouseMembers.Count >= 3)
//                    {
//                        var OldestHeir = HouseMembers.OrderBy(x => x.Value).ElementAt(1).Key;
//                        this.immediateHeir = OldestHeir;

//                        var SecondOldestHeir = HouseMembers.OrderBy(x => x.Value).ElementAt(2).Key;
//                        this.secondHeir = SecondOldestHeir;
//                    }
//                    else if (HouseMembers.Count == 2)
//                    {
//                        var OldestHeir = HouseMembers.OrderBy(x => x.Value).ElementAt(1).Key;
//                        this.immediateHeir = OldestHeir;
//                        this.secondHeir = null;
//                    }
//                    else
//                    {
//                        this.immediateHeir = null;
//                        this.secondHeir = null;
//                    }
//                }
//                else
//                {
//                    LucianRoyalty_Core.Log(string.Format("No succession type specified. Seniority assumed."));
//                    var CurrentMonarch = HouseMembers.Aggregate((x, y) => x.Value < y.Value ? x : y).Key;
//                    HouseMembers.Remove(CurrentMonarch);

//                    var OldestHeir = HouseMembers.Aggregate((x, y) => x.Value < y.Value ? x : y).Key;
//                    this.immediateHeir = OldestHeir;

//                    HouseMembers.Remove(OldestHeir);
//                    var SecondOldestHeir = HouseMembers.Aggregate((x, y) => x.Value < y.Value ? x : y).Key;
//                    this.secondHeir = SecondOldestHeir;
//                }
//            }

//        }

//        private void ImplementInheritance()
//        {
//            Precept_Role precept_Role = Current.Game.World.factionManager.OfPlayer.ideos.PrimaryIdeo.GetPrecept(PreceptDefOf.IdeoRole_Leader) as Precept_Role;
//            Pawn currentMonarch = precept_Role.ChosenPawnSingle();

//            bool vacantMonarch = currentMonarch == null;
//            if (vacantMonarch)
//            {
//                if (immediateHeir != null)
//                {
//                    this.chosenLucian = this.immediateHeir;
//                    bool chosenMonarch = precept_Role.RequirementsMet(this.chosenLucian);
//                    if (chosenMonarch)
//                    {
//                        precept_Role.Assign(this.chosenLucian, true);
//                    }
//                }
//                //No monarch
//            }
//        }


//    }
//}
