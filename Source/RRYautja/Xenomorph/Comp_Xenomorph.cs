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
        public CompProperties_Xenomorph Props
        {
            get
            {
                return (CompProperties_Xenomorph)this.props;
            }
        }

        public override void CompTickRare()
        {
            base.CompTickRare();
            Pawn pawn = (Pawn)parent;
            bool selected = Find.Selector.SingleSelectedThing == pawn;
            Lord lord = pawn.GetLord();
            Faction xenos = Find.FactionManager.FirstFactionOfDef(XenomorphDefOf.RRY_Xenomorph);
            if (pawn!=null && !pawn.Dead && pawn.kindDef!=XenomorphDefOf.RRY_Xenomorph_FaceHugger)
            { //Find.FactionManager.FirstFactionOfDef(XenomorphDefOf.RRY_Xenomorph).
                LifeStageDef stage = pawn.ageTracker.CurLifeStage;
                if (stage == pawn.RaceProps.lifeStageAges[pawn.RaceProps.lifeStageAges.Count - 1].def)
                {
                    if (lord == null)
                    {
#if DEBUG
                        if (selected)
                        {
                            Log.Message(string.Format("{0} has no lord, looking for lord from {1}", pawn.LabelShort, xenos.Name));
                        }
#endif
                        IEnumerable<Lord> lords = pawn.Map.lordManager.lords.Where(x => x.faction.def == xenos.def);
                        if (lords.Count() != 0)
                        {
                            foreach (var l in lords)
                            {
                                if (l == null)
                                {
#if DEBUG
                                    if (selected)
                                    {
                                        Log.Message(string.Format("no lord of faction {0} for {1}", xenos.Name, pawn.LabelShort));
                                    }

#endif

                                }
                                else
                                {
#if DEBUG
                                    if (selected)
                                    {
                                        Log.Message(string.Format("{0}: added to Lord: {1}, Cur Duty: {2}", pawn.LabelShort, l, l.LordJob));
                                    }

#endif
                                    lord = l;
                                    lord.AddPawn(pawn);
                                    break;
                                }
                            }
                        }
                        else
                        {
#if DEBUG
                            if (selected)
                            {
                                Log.Message(string.Format("{2} lords of {0} for {1}", xenos.Name, pawn.LabelShort, lords.Count()));
                            }
#endif
                            if (pawn.kindDef == XenomorphDefOf.RRY_Xenomorph_Queen)
                            {
                                CreateNewLord(pawn);
                            }
                        }
                    }
                    else
                    {
#if DEBUG
                        if (selected)
                        {
                            Log.Message(string.Format("{0} has Lord: {1}, Cur Duty: {2}", pawn.LabelShort, lord, lord.LordJob));
                        }

#endif

                    }
                }
                else
                {
                    Thing thing = GenClosest.ClosestThingReachable(this.parent.Position, this.parent.Map, ThingRequest.ForGroup(ThingRequestGroup.Pawn), PathEndMode.Touch, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), 6f, x => ((Pawn)this.parent).HostileTo((Pawn)x), null, 0, -1, false, RegionType.Set_Passable, false);
                    if (!((Pawn)this.parent).health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Xenomorph_Hidden) && thing==null)
                    {
                        string text = TranslatorFormattedStringExtensions.Translate("Xeno_Chestburster_Hides");
                    //    Log.Message(text);
                        MoteMaker.ThrowText(base.parent.Position.ToVector3(), base.parent.Map, text, 3f);
                        ((Pawn)this.parent).health.AddHediff(XenomorphDefOf.RRY_Hediff_Xenomorph_Hidden);
                    }
                }
                /*
                  for (int i = 0; i < num2; i++)
                  {
                      MoteMaker.ThrowDustPuff(loc, base.parent.Map, Rand.Range(0.8f, 1.2f));
                  }
                  */
            }

        }

        private Lord CreateNewLord(Pawn pawn)
        {
            IntVec3 c;
            Thing thing = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(XenomorphDefOf.RRY_EggXenomorphFertilized), PathEndMode.OnCell, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), 9999f, null, null, 0, -1, false, RegionType.Set_Passable, false);
            if (thing == null)
            {
                c = RCellFinder.RandomWanderDestFor(pawn, pawn.Position, 5f, null, Danger.Some);
            }
            else
            {
                InfestationCellFinder.TryFindCell(out c, pawn.Map);
                if (pawn.CanReach(c, PathEndMode.OnCell, Danger.Deadly))
                {
                    c = RCellFinder.RandomWanderDestFor(pawn, c, 3f, null, Danger.Some);
                }
                else
                {
                    c = RCellFinder.RandomWanderDestFor(pawn, thing.Position, 3f, null, Danger.Some);
                }
            }
            return LordMaker.MakeNewLord(parent.Faction, new LordJob_DefendBase(parent.Faction, c), parent.Map, null);
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
            if (((Pawn)this.parent).health.hediffSet.HasHediff(YautjaDefOf.RRY_Hediff_Cloaked))
            {
                ((Pawn)this.parent).health.RemoveHediff(((Pawn)this.parent).health.hediffSet.GetFirstHediffOfDef(YautjaDefOf.RRY_Hediff_Cloaked));
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
                                        Log.Message("weapon immune to acid");
                                    }
                                }
                                else if (WeaponDef.IsWeapon&& WeaponDef.IsRangedWeapon)
                                {
                                    Log.Message("ranged weapon, immune to acid");
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

        public override void CompTickRare()
        {
            base.CompTickRare();


        }

        public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {

            Pawn other = dinfo.Instigator as Pawn;
            Pawn pawn = base.parent as Pawn;



            base.PostPostApplyDamage(dinfo, totalDamageDealt);

        }
    }
}
