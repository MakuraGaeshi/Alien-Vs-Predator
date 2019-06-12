using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RRYautja
{
    public class CompProperties_Xenomorph : CompProperties
    {
        public CompProperties_Xenomorph()
        {
            this.compClass = typeof(Comp_Xenomorph);
        }

        public int healIntervalTicks = 60;
    }

    public class Comp_Xenomorph : ThingComp
    {
        public PawnKindDef HuggerKindDef = XenomorphDefOf.RRY_Xenomorph_FaceHugger;
        public PawnKindDef RoyaleKindDef = XenomorphDefOf.RRY_Xenomorph_RoyaleHugger;
        public PawnKindDef QueenDef = XenomorphDefOf.RRY_Xenomorph_Queen;
        public CompProperties_Xenomorph Props
        {
            get
            {
                return (CompProperties_Xenomorph)this.props;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<int>(ref this.ticksSinceHeal, "ticksSinceHeal");
            Scribe_Values.Look<IntVec3>(ref this.HiveLoc, "HiveLoc", IntVec3.Invalid);
            /*
            Scribe_Values.Look<int>(ref this.pawnKills, "pawnKills");
            Scribe_Deep.Look<Hediff>(ref this.unmarked, "bloodedUnmarked");
            Scribe_Defs.Look<HediffDef>(ref this.MarkedhediffDef, "MarkedhediffDef");
            Scribe_References.Look<Corpse>(ref this.corpse, "corpseRef", true);
            Scribe_References.Look<Pawn>(ref this.pawn, "pawnRef", true);
            Scribe_Values.Look<String>(ref this.MarkHedifftype, "thisMarktype");
            Scribe_Values.Look<String>(ref this.MarkHedifflabel, "thislabel");
            Scribe_Values.Look<bool>(ref this.predator, "thisPred");
            Scribe_Values.Look<float>(ref this.combatPower, "thiscombatPower");
            Scribe_Values.Look<float>(ref this.BodySize, "thisBodySize");
            Scribe_Values.Look<bool>(ref this.TurretIsOn, "thisTurretIsOn");
            Scribe_Values.Look<bool>(ref this.blooded, "thisblooded");
            */
        }

        public IntVec3 HiveLoc;

        public Pawn pawn
        {
            get
            {
                return (Pawn)parent;
            }
        }

        public Map map
        {
            get
            {
                return pawn.Map != null ? pawn.Map : pawn.MapHeld;
            }
        }
        public float MinHideDist
        {
            get
            {
                return (10 * pawn.BodySize) * pawn.Map.glowGrid.GameGlowAt(pawn.Position, false);
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
        }

        public void XenoLordTick()
        {
            if (map != null)
            {
                IntVec3 c = IntVec3.Invalid;
                Lord lord = null;
                List<Lord> Hivelords = new List<Lord>();
                Lord Hivelord = null;
                LordJob Hivejob = null;
                Pawn Hivequeen = null;
                IEnumerable<Lord> lords = pawn.Map.lordManager.lords.Where(x => x.faction == pawn.Faction);
                if (lords.Count() != 0)
                {
                    foreach (var l in lords)
                    {
                        if (l != null)
                        {
                            if (l.LordJob is LordJob_DefendAndExpandHiveLike j)
                            {
                                Hivelord = l;
                                Hivejob = j;
                                if (l.ownedPawns.Any(x => x.kindDef == QueenDef))
                                {
                                    Hivequeen = l.ownedPawns.Find(x => x.kindDef == QueenDef);
                                }
                                if (pawn.kindDef != XenomorphDefOf.RRY_Xenomorph_Queen || (pawn.kindDef == XenomorphDefOf.RRY_Xenomorph_Queen && Hivequeen != null))
                                {
                                    Hivelords.Add(l);
                                }
                            }
                        }
                        else
                        {
                            /*
                            lord = l;
                            lord.AddPawn(pawn);
                            pawn.mindState.duty = lord.ownedPawns.FindAll(x => x.mindState.duty != null && x != pawn).RandomElement().mindState.duty;
                            break;
                            */
                        }
                   //     Log.Message(string.Format("{0} found: {1} Lords of: {2}", pawn.Label, Hivelords.Count, pawn.Faction.Name));
                    }
                }
                if (pawn.GetLord() != null && pawn.GetLord().LordJob is LordJob_DefendPoint LordJob_DefendPoint)
                {
                    lord = pawn.GetLord();
                    if (lord.ownedPawns.Count == 0)
                    {
                   //     Log.Message(string.Format("got no pawns, wtf?"));
                    }
                    if (lord.ownedPawns.Count == 1)
                    {
                   //     Log.Message(string.Format("lords only pawn"));
                    }
                    if (c == IntVec3.Invalid && XenomorphUtil.HivelikesPresent(map))
                    {
                   //     Log.Message(string.Format("Checking hiveloc"));
                        c = XenomorphUtil.ClosestReachableHivelike(pawn).DestroyedOrNull() ? XenomorphUtil.ClosestReachableHivelike(pawn).Position : IntVec3.Invalid;
                   //     Log.Message(string.Format("hiveloc: {0}", c));
                    }
                    if (c == IntVec3.Invalid && XenomorphUtil.EggsPresent(map))
                    {
                   //     Log.Message(string.Format("Checking eggloc"));
                        c = XenomorphUtil.ClosestReachableEgg(pawn).DestroyedOrNull() ? XenomorphUtil.ClosestReachableEgg(pawn).Position : IntVec3.Invalid;
                   //     Log.Message(string.Format("eggloc: {0}", c));
                    }
                    if (c == IntVec3.Invalid && XenomorphUtil.CocoonsPresent(map, XenomorphDefOf.RRY_Xenomorph_Humanoid_Cocoon))
                    {
                   //     Log.Message(string.Format("Checking Hcocoonloc"));
                        c = XenomorphUtil.ClosestReachableCocoon(pawn, XenomorphDefOf.RRY_Xenomorph_Humanoid_Cocoon).DestroyedOrNull() ? XenomorphUtil.ClosestReachableCocoon(pawn, XenomorphDefOf.RRY_Xenomorph_Humanoid_Cocoon).Position : IntVec3.Invalid;
                   //     Log.Message(string.Format("Hcocoonloc: {0}", c));
                    }
                    if (c == IntVec3.Invalid && XenomorphUtil.CocoonsPresent(map, XenomorphDefOf.RRY_Xenomorph_Animal_Cocoon))
                    {
                   //     Log.Message(string.Format("Checking Acocoonloc"));
                        c = XenomorphUtil.ClosestReachableCocoon(pawn, XenomorphDefOf.RRY_Xenomorph_Animal_Cocoon).DestroyedOrNull() ? XenomorphUtil.ClosestReachableCocoon(pawn, XenomorphDefOf.RRY_Xenomorph_Animal_Cocoon).Position : IntVec3.Invalid;
                   //     Log.Message(string.Format("Acocoonloc: {0}", c));
                    }
                    if (c == IntVec3.Invalid)
                    {
                        if (InfestationLikeCellFinder.TryFindCell(out c, pawn.Map, false))
                        {
                       //     Log.Message(string.Format("Checking InfestationLikeCellFinder"));
                            if (Prefs.DevMode)
                            {
                                ThingDef td = XenomorphDefOf.RRY_Filth_Slime;
                                GenSpawn.Spawn(td, c, pawn.Map);
                                Find.LetterStack.ReceiveLetter(string.Format("Lord Created"), string.Format("@: {0} ", c), LetterDefOf.NegativeEvent, c.GetFirstThing(pawn.Map, td), null, null);
                            }
                       //     Log.Message(string.Format("InfestationLikeCellFinder: {0}", c));
                        }
                        if (pawn.CanReach(c, PathEndMode.OnCell, Danger.Deadly, true))
                        {
                       //     Log.Message(string.Format("CanReach"));
                            //    c = RCellFinder.RandomWanderDestFor(pawn, c, 3f, null, Danger.Some);
                       //     Log.Message(string.Format("InfestationLikeCellFinder: {0}", c));
                        }
                        else
                        {
                       //     Log.Message(string.Format("CantReach InfestationLikeCellFinder"));
                            c = RCellFinder.RandomWanderDestFor(pawn, pawn.Position, 3f, null, Danger.Some);
                       //     Log.Message(string.Format("RCellFinder: {0}", c));
                        }
                    }
                    if (c != IntVec3.Invalid)
                    {
                   //     Log.Message(string.Format("Target cell found: {0}", c));
                        LordJob newJob = new LordJob_DefendAndExpandHiveLike(false);
                        /*
                   //     Log.Message(string.Format("New job: {0}", newJob));
                        lord.SetJob(newJob);
                   //     Log.Message(string.Format("Set job: {0}", newJob));
                        */
                        if (pawn.GetLord().LordJob is LordJob_DefendPoint)
                        {
                       //     Log.Message(string.Format("New Lord Required: {0}", pawn.GetLord().LordJob));

                            if (!Hivelords.NullOrEmpty())
                            {
                                Hivelord = Hivelords.RandomElement();
                                SwitchToLord(Hivelord);
                                CreateNewLord(pawn, c, newJob);
                           //     Log.Message(string.Format("New Lord switched: {0}", pawn.GetLord().LordJob));
                            }
                            else
                            {
                                CreateNewLord(pawn, c, newJob);
                           //     Log.Message(string.Format("New Lord created: {0}", pawn.GetLord().LordJob));
                            }
                            if (HiveLoc == IntVec3.Invalid) HiveLoc = c;
                        }
                    }
                }
                else if (c == IntVec3.Invalid && (pawn.GetLord() != null && pawn.GetLord().LordJob is LordJob LordJob))
                {
                    lord = pawn.GetLord();
                    c = LordJob.lord.Graph.StartingToil.FlagLoc;
                    if (c == IntVec3.Invalid)
                    {
                        c = LordJob.lord.CurLordToil.FlagLoc;
                    }
                }
                else if (pawn.GetLord() == null)
                {
               //     Log.Message(string.Format("{0} has no lord!!", pawn.Label));
                    if (!Hivelords.NullOrEmpty())
                    {
                        Hivelord = Hivelords.RandomElement();
                        SwitchToLord(Hivelord);
                   //     Log.Message(string.Format("New Lord switched: {0}", pawn.GetLord().LordJob));
                    }
                }
                if (pawn.GetLord()!=null)
                {
                    List<Pawn> list = pawn.GetLord().ownedPawns.Where(x => x.mindState.duty != null).ToList();
                    if (pawn.GetLord() != null && pawn.mindState.duty == null && !list.NullOrEmpty())
                    {
                        pawn.mindState.duty = list.RandomElement().mindState.duty;
                    }
                }
            }
        }

        private Lord SwitchToLord(Lord lord)
        {
            if (pawn.GetLord() != null && pawn.GetLord() is Lord l)
            {
                if (l.ownedPawns.Count > 0)
                {
                    l.ownedPawns.Remove(pawn);
                }
                if (l.ownedPawns.Count == 0)
                {
                    l.lordManager.RemoveLord(l);
                }
            }
            lord.AddPawn(pawn);
            return lord;
        }
        public override void CompTickRare()
        {
            base.CompTickRare();
            bool selected = Find.Selector.SelectedObjects.Contains(pawn);
            Lord lord = pawn.GetLord();
            Faction xenos = Find.FactionManager.FirstFactionOfDef(XenomorphDefOf.RRY_Xenomorph);
            
            if (pawn != null && pawn.Map != null && !pawn.Dead && pawn.kindDef != XenomorphDefOf.RRY_Xenomorph_FaceHugger && pawn.kindDef != XenomorphDefOf.RRY_Xenomorph_RoyaleHugger)
            {
                LifeStageDef stage = pawn.ageTracker.CurLifeStage;
                if (stage == pawn.RaceProps.lifeStageAges[pawn.RaceProps.lifeStageAges.Count - 1].def)
                {
                    if (pawn.health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Xenomorph_Hidden))
                    {
                        string text = TranslatorFormattedStringExtensions.Translate("Xeno_Chestburster_Matures",pawn.LabelCap);
                        Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(XenomorphDefOf.RRY_Hediff_Xenomorph_Hidden);
                        MoteMaker.ThrowText(pawn.Position.ToVector3(), pawn.Map, text, 3f);
                        pawn.health.RemoveHediff(hediff);
                    }
                    XenoLordTick();
                }
                else
                {
                    Thing thing = GenClosest.ClosestThingReachable(this.parent.Position, this.parent.Map, ThingRequest.ForGroup(ThingRequestGroup.Pawn), PathEndMode.Touch, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), 6f, x => ((Pawn)this.parent).HostileTo((Pawn)x)&&!((Pawn)x).health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Cocooned), null, 0, -1, false, RegionType.Set_Passable, false);
                    if (!((Pawn)this.parent).health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Xenomorph_Hidden) && thing==null)
                    {
                        string text = TranslatorFormattedStringExtensions.Translate("Xeno_Chestburster_Hides");

                        MoteMaker.ThrowText(base.parent.Position.ToVector3(), base.parent.Map, text, 3f);
                        ((Pawn)this.parent).health.AddHediff(XenomorphDefOf.RRY_Hediff_Xenomorph_Hidden);
                    }
                }
            }

        }

        private Lord CreateNewLord(Pawn pawn)
        {
            IntVec3 c;
            Thing thing = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(XenomorphDefOf.RRY_EggXenomorphFertilized), PathEndMode.OnCell, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), 9999f, null, null, 0, -1, false, RegionType.Set_Passable, false);
            if (thing != null)
            {
                c = RCellFinder.RandomWanderDestFor(pawn, thing.Position, 5f, null, Danger.Some);
            }
            else
            {
                if (InfestationLikeCellFinder.TryFindCell(out c, pawn.Map, false))
                {
                    if (Prefs.DevMode && Find.Selector.SelectedObjects.Contains(pawn))
                    {
                        ThingDef td = XenomorphDefOf.RRY_Filth_Slime;
                        GenSpawn.Spawn(td, c, pawn.Map);
                        Find.LetterStack.ReceiveLetter(string.Format("Lord Created"), string.Format("@: {0} ", c), LetterDefOf.NegativeEvent, c.GetFirstThing(pawn.Map, td), null, null);
                    }
                }
                if (pawn.CanReach(c, PathEndMode.OnCell, Danger.Deadly, true))
                {
                //    c = RCellFinder.RandomWanderDestFor(pawn, c, 3f, null, Danger.Some);
                }
                else
                {
                    c = RCellFinder.RandomWanderDestFor(pawn, thing.Position, 3f, null, Danger.Some);
                }
            }
            if (pawn.GetLord() != null && pawn.GetLord() is Lord l)
            {
                if (l.ownedPawns.Count > 0)
                {
                    l.ownedPawns.Remove(pawn);
                }
                if (l.ownedPawns.Count == 0)
                {
                    l.lordManager.RemoveLord(l);
                }
            }
            Lord lord = LordMaker.MakeNewLord(parent.Faction, new LordJob_DefendAndExpandHiveLike(false), parent.Map, null);
            lord.AddPawn(pawn);
            return lord;
        }

        private Lord CreateNewLord(Pawn pawn, IntVec3 loc, LordJob lordJob)
        {
            IntVec3 c = loc;
            if (pawn.GetLord() != null && pawn.GetLord() is Lord l)
            {
                if (l.ownedPawns.Count > 0)
                {
                    l.ownedPawns.Remove(pawn);
                }
                if (l.ownedPawns.Count == 0)
                {
                    l.lordManager.RemoveLord(l);
                }
            }
            Lord lord = LordMaker.MakeNewLord(parent.Faction, lordJob, map, null);
            lord.AddPawn(pawn);
            return lord;
        }

        public int healIntervalTicks = 60;
        public override void CompTick()
        {
            base.CompTick();
            this.ticksSinceHeal++;
            bool flag = this.ticksSinceHeal > this.healIntervalTicks;
            if (flag)
            {
                bool flag2 = ((Pawn)base.parent).health.hediffSet.HasNaturallyHealingInjury();
                if (flag2)
                {
                    this.ticksSinceHeal = 0;
                    float num = 8f;
                    Hediff_Injury hediff_Injury = GenCollection.RandomElement<Hediff_Injury>(from x in ((Pawn)base.parent).health.hediffSet.GetHediffs<Hediff_Injury>()
                                                                                             where HediffUtility.CanHealNaturally(x)
                                                                                             select x);
                    hediff_Injury.Heal(num * ((Pawn)base.parent).HealthScale * 0.01f);
                    string text = string.Format("{0} healed.", ((Pawn)base.parent).LabelCap);
                }
            }
        }

        public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            Pawn other = dinfo.Instigator as Pawn;
            Pawn pawn = base.parent as Pawn;

            base.PostPostApplyDamage(dinfo, totalDamageDealt);
            if (((Pawn)this.parent).health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Xenomorph_Hidden))
            {
                ((Pawn)this.parent).health.RemoveHediff(((Pawn)this.parent).health.hediffSet.GetFirstHediffOfDef(XenomorphDefOf.RRY_Hediff_Xenomorph_Hidden));
            }


        }

        public override void PostPreApplyDamage(DamageInfo dinfo, out bool absorbed)
        {
            bool acidburns = true;
            if (base.parent is Pawn pawn && pawn != null)
            {
                bool selected = Find.Selector.SelectedObjects.Contains(pawn);
#if DEBUG
                if (selected) Log.Message(string.Format("CompXeno PPAD pawn: {0}", pawn.LabelShortCap));
#endif
                if (dinfo.Instigator is Pawn Instigator && Instigator!=null)
                {
                    selected = selected? selected : Find.Selector.SelectedObjects.Contains(Instigator);
#if DEBUG
                    if (selected) Log.Message(string.Format("CompXeno PPAD otherpawn: {0}", Instigator.LabelShortCap));
#endif
                    if (dinfo.Weapon is ThingDef WeaponDef && WeaponDef != null)
                    {
#if DEBUG
                        if (selected) Log.Message(string.Format("CompXeno PPAD WeaponDef: {0}", WeaponDef.defName));
#endif
                        if (WeaponDef.IsWeapon && WeaponDef.IsMeleeWeapon)
                        {

                            if (WeaponDef == Instigator.equipment.Primary.def && Instigator.equipment.Primary is ThingWithComps Weapon && Instigator.equipment.PrimaryEq is CompEquippable WeaponEQ)
                            {
                                WeaponDef = Weapon.def;
#if DEBUG
                                if (selected) Log.Message(string.Format("CompXeno PPAD WeaponDef: {0} matches other.equipment.Primary: {1}", WeaponDef.LabelCap, Weapon.LabelCap));
#endif
                                if (WeaponDef.IsMeleeWeapon)
                                {
#if DEBUG
                                    if (selected) Log.Message(string.Format("CompXeno PPAD WeaponDef.isMelee: {0}", WeaponDef.IsMeleeWeapon));
#endif
                                    if (dinfo.Weapon.MadeFromStuff && Weapon.Stuff is ThingDef WeaponStuff)
                                    {
#if DEBUG
                                        if (selected) Log.Message(string.Format("CompXeno PPAD WeaponDef: {0}, MadeFromStuff: {1}", WeaponDef.LabelCap, WeaponDef.MadeFromStuff));
#endif
                                        if (WeaponStuff.defName.Contains("RRY_Xeno"))
                                        {
#if DEBUG
                                            if (selected) Log.Message(string.Format("CompXeno PPAD WeaponDef: {0}, MadeFromStuff: {1}", WeaponDef.LabelCap, WeaponDef.MadeFromStuff));
#endif
                                            acidburns = false;

                                        }
                                    }
                                    else
                                    {
#if DEBUG
                                        if (selected) Log.Message(string.Format("CompXeno PPAD WeaponDef: {0}, Not MadeFromStuff: {1}", WeaponDef.LabelCap, WeaponDef.MadeFromStuff));
#endif
                                        foreach (var item in Weapon.def.costList)
                                        {
                                            if (item.thingDef == XenomorphDefOf.RRY_Xenomorph_TailSpike || item.thingDef == XenomorphDefOf.RRY_Xenomorph_HeadShell)
                                            {
                                                acidburns = false;
                                            }
                                        }
                                    }
                                    if (acidburns)
                                    {
                                        Weapon.HitPoints -= Rand.Range(0, 5);
                                        if (Weapon.HitPoints <= 0)
                                        {
                                            Weapon.Destroy();
                                        }
                                    }
                                    else
                                    {
                                    //    Log.Message("weapon immune to acid");
                                    }
                                }
                                else if (WeaponDef.IsWeapon&& WeaponDef.IsRangedWeapon)
                                {
                                //    Log.Message("ranged weapon, immune to acid");
                                }
                            }

                        }
                    }
                }
            }
#if DEBUG
#endif
            base.PostPreApplyDamage(dinfo, out absorbed);
        }
        public PawnKindDef host;
        public int ticksSinceHeal;
    }

    // --------------------------------------------------------------------------- //
    public class CompProperties_Facehugger : CompProperties
    {
        public CompProperties_Facehugger()
        {
            this.compClass = typeof(Comp_Facehugger);
        }
    }

    public class Comp_Facehugger : ThingComp
    {
        public CompProperties_Facehugger Props
        {
            get
            {
                return (CompProperties_Facehugger)this.props;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<int>(ref this.Impregnations, "Impregnations", 0, true);
            Scribe_Values.Look<int>(ref this.ticksSinceHeal, "ticksSinceHeal");
            /*
            Scribe_Values.Look<int>(ref this.pawnKills, "pawnKills");
            Scribe_Deep.Look<Hediff>(ref this.unmarked, "bloodedUnmarked");
            Scribe_Defs.Look<HediffDef>(ref this.MarkedhediffDef, "MarkedhediffDef");
            Scribe_References.Look<Corpse>(ref this.corpse, "corpseRef", true);
            Scribe_References.Look<Pawn>(ref this.pawn, "pawnRef", true);
            Scribe_Values.Look<String>(ref this.MarkHedifftype, "thisMarktype");
            Scribe_Values.Look<String>(ref this.MarkHedifflabel, "thislabel");
            Scribe_Values.Look<bool>(ref this.predator, "thisPred");
            Scribe_Values.Look<float>(ref this.combatPower, "thiscombatPower");
            Scribe_Values.Look<float>(ref this.BodySize, "thisBodySize");
            Scribe_Values.Look<bool>(ref this.TurretIsOn, "thisTurretIsOn");
            Scribe_Values.Look<bool>(ref this.blooded, "thisblooded");
            */
        }

        public Pawn Facehugger
        {
            get
            {
                return ((Pawn)this.parent);
            }
        }

        public bool RoyaleHugger
        {
            get
            {
                return Facehugger.kindDef == RoyaleKindDef;
            }
        }

        public int maxImpregnations
        {
            get
            {
                if (RoyaleHugger)
                {
                    return 2;
                }
                return 1;
            }
        }

        public PawnKindDef pawnKindDef
        {
            get
            {
                return RoyaleHugger ? RoyaleKindDef : HuggerKindDef;
            }
        }
        public int Impregnations;

        public PawnKindDef HuggerKindDef = XenomorphDefOf.RRY_Xenomorph_FaceHugger;
        public PawnKindDef RoyaleKindDef = XenomorphDefOf.RRY_Xenomorph_RoyaleHugger;

        public int healIntervalTicks = 100;
        public int deathIntervalTicks = 300 * Rand.RangeInclusive(1,5);
        public override void CompTick()
        {
            base.CompTick();
            this.ticksSinceHeal++;
            if (Impregnations >= maxImpregnations) this.ticksSinceImpregnation++;
            bool flag = this.ticksSinceHeal > this.healIntervalTicks;
            if (flag)
            {
                bool flag2 = Facehugger.health.hediffSet.HasNaturallyHealingInjury();
                if (flag2)
                {
                    float num = 8f;
                    Hediff_Injury hediff_Injury = GenCollection.RandomElement<Hediff_Injury>(from x in Facehugger.health.hediffSet.GetHediffs<Hediff_Injury>()
                                                                                             where HediffUtility.CanHealNaturally(x)
                                                                                             select x);
                    hediff_Injury.Heal(num * Facehugger.HealthScale * 0.05f);
                    string text = string.Format("{0} healed.", Facehugger.LabelCap);
                }
                if (Impregnations>=maxImpregnations)
                {
                    bool flag3 = this.ticksSinceImpregnation > this.deathIntervalTicks;
                    if (flag3)
                    {
                        this.ticksSinceImpregnation = 0;
                        if (Rand.Chance(0.5f))
                        {
                            Facehugger.Kill(null);
                        }
                    }
                }
            }
        }

        public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {

            Pawn other = dinfo.Instigator as Pawn;
            Pawn pawn = base.parent as Pawn;



            base.PostPostApplyDamage(dinfo, totalDamageDealt);

        }
        public PawnKindDef host;
        public int ticksSinceHeal;
        public int ticksSinceImpregnation;
    }
    // ---------------------------------------------------------------------------
    public class CompProperties_Neomorph : CompProperties
    {
        public CompProperties_Neomorph()
        {
            this.compClass = typeof(Comp_Neomorph);
        }

    }

    public class Comp_Neomorph : ThingComp
    {
        public CompProperties_Neomorph Props
        {
            get
            {
                return (CompProperties_Neomorph)this.props;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<int>(ref this.ticksSinceHeal, "ticksSinceHeal");
            /*
            Scribe_Values.Look<int>(ref this.pawnKills, "pawnKills");
            Scribe_Deep.Look<Hediff>(ref this.unmarked, "bloodedUnmarked");
            Scribe_Defs.Look<HediffDef>(ref this.MarkedhediffDef, "MarkedhediffDef");
            Scribe_References.Look<Corpse>(ref this.corpse, "corpseRef", true);
            Scribe_References.Look<Pawn>(ref this.pawn, "pawnRef", true);
            Scribe_Values.Look<String>(ref this.MarkHedifftype, "thisMarktype");
            Scribe_Values.Look<String>(ref this.MarkHedifflabel, "thislabel");
            Scribe_Values.Look<bool>(ref this.predator, "thisPred");
            Scribe_Values.Look<float>(ref this.combatPower, "thiscombatPower");
            Scribe_Values.Look<float>(ref this.BodySize, "thisBodySize");
            Scribe_Values.Look<bool>(ref this.TurretIsOn, "thisTurretIsOn");
            Scribe_Values.Look<bool>(ref this.blooded, "thisblooded");
            */
        }

        public override void CompTickRare()
        {
            base.CompTickRare();


        }

        public int healIntervalTicks = 60;
        public override void CompTick()
        {
            base.CompTick();
            this.ticksSinceHeal++;
            bool flag = this.ticksSinceHeal > this.healIntervalTicks;
            if (flag)
            {
                bool flag2 = ((Pawn)base.parent).health.hediffSet.HasNaturallyHealingInjury();
                if (flag2)
                {
                    this.ticksSinceHeal = 0;
                    float num = 8f;
                    Hediff_Injury hediff_Injury = GenCollection.RandomElement<Hediff_Injury>(from x in ((Pawn)base.parent).health.hediffSet.GetHediffs<Hediff_Injury>()
                                                                                             where HediffUtility.CanHealNaturally(x)
                                                                                             select x);
                    hediff_Injury.Heal(num * ((Pawn)base.parent).HealthScale * 0.01f);
                    string text = string.Format("{0} healed.", ((Pawn)base.parent).LabelCap);
                }
            }
        }

        public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {

            Pawn other = dinfo.Instigator as Pawn;
            Pawn pawn = base.parent as Pawn;



            base.PostPostApplyDamage(dinfo, totalDamageDealt);

        }

        public PawnKindDef host;
        public int ticksSinceHeal;
    }
}
