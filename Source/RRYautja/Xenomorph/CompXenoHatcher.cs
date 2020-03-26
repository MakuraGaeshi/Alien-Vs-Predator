using System;
using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RRYautja
{
    // Token: 0x02000743 RID: 1859
    public class CompProperties_XenoHatcher : CompProperties
    {
        // Token: 0x060028EA RID: 10474 RVA: 0x00136AE7 File Offset: 0x00134EE7
        public CompProperties_XenoHatcher()
        {
            this.compClass = typeof(CompXenoHatcher);
        }

        // Token: 0x040016C3 RID: 5827
        public float hatcherDaystoHatch = 1f;
        public float triggerRadius = 10f;
        // Token: 0x040016C4 RID: 5828
        public PawnKindDef hatcherPawn;
    }

    // Token: 0x02000744 RID: 1860
    public class CompXenoHatcher : ThingComp
    {
        public CompProperties_XenoHatcher Props => (CompProperties_XenoHatcher)this.props;
        public CompTemperatureRuinable FreezerComp => this.parent.GetComp<CompTemperatureRuinable>();
        public Map MyMap => this.parent.Map ?? this.parent.MapHeld;
        public IntVec3 MyPos => this.parent.Position != null ? this.parent.Position : this.parent.PositionHeld;

        bool selected => Find.Selector.SelectedObjects.Contains(parent) && Prefs.DevMode && DebugSettings.godMode;
        public bool TemperatureDamaged => this.FreezerComp != null && this.FreezerComp.Ruined;
        public bool QueenPresent => MyMap.mapPawns.AllPawnsSpawned.Any(x => x.kindDef == XenomorphDefOf.RRY_Xenomorph_Queen) || (XenomorphUtil.HivelikesPresent(MyMap) && ((XenomorphUtil.SpawnedHivelikes(MyMap)).Any<HiveLike>((HiveLike y) => y.hasQueen)));
        public bool RoyaleEgg => this.eggState == EggState.Royal;
        public bool EggMutating => this.mutateProgress > 0f && this.mutateProgress < 1f;
        public bool RoyalEggPresent => MyMap.listerThings.ThingsOfDef(XenomorphDefOf.RRY_EggXenomorphFertilized).Any(x => x.TryGetComp<CompXenoHatcher>() != null && x.TryGetComp<CompXenoHatcher>() is CompXenoHatcher hatcher && hatcher.RoyaleEgg && x != this.parent);
        public bool RoyalEggsPresent(out List<Thing> eggList)
        {
            eggList = MyMap.listerThings.ThingsOfDef(XenomorphDefOf.RRY_EggXenomorphFertilized).FindAll(x => x.TryGetComp<CompXenoHatcher>() != null && x.TryGetComp<CompXenoHatcher>() is CompXenoHatcher hatcher && hatcher.RoyaleEgg && x != this.parent);
            bool result = !eggList.NullOrEmpty();
            return result;
        }
        public bool EggPresent(EggState state) => MyMap.listerThings.ThingsOfDef(XenomorphDefOf.RRY_EggXenomorphFertilized).Any(x => x.TryGetComp<CompXenoHatcher>() != null && x.TryGetComp<CompXenoHatcher>() is CompXenoHatcher hatcher && hatcher.eggState == state && x != this.parent);
        public bool EggsPresent(EggState state, out List<Thing> eggList)
        {
            eggList = MyMap.listerThings.ThingsOfDef(XenomorphDefOf.RRY_EggXenomorphFertilized).FindAll(x => x.TryGetComp<CompXenoHatcher>() != null && x.TryGetComp<CompXenoHatcher>() is CompXenoHatcher hatcher && hatcher.eggState == state && x != this.parent);
            bool result = !eggList.NullOrEmpty();
            return result;
        }
        public bool RoyalPresent
        {
            get
            {
                Predicate<Pawn> validator = delegate (Pawn t)
                {
                    bool RoyalHugger = t.kindDef == RoyalKindDef;
                    bool RoyalHuggerInfection = (t.health.hediffSet.HasHediff(XenomorphDefOf.RRY_FaceHuggerInfection) && t.health.hediffSet.GetFirstHediffOfDef(XenomorphDefOf.RRY_FaceHuggerInfection).TryGetComp<HediffComp_XenoFacehugger>().RoyaleHugger);
                    bool RoyalImpregnation = (t.health.hediffSet.HasHediff(XenomorphDefOf.RRY_XenomorphImpregnation) && t.health.hediffSet.GetFirstHediffOfDef(XenomorphDefOf.RRY_XenomorphImpregnation).TryGetComp<HediffComp_XenoSpawner>().RoyaleHugger);
                    bool RoyalHiddenImpregnation = (t.health.hediffSet.HasHediff(XenomorphDefOf.RRY_HiddenXenomorphImpregnation) && t.health.hediffSet.GetFirstHediffOfDef(XenomorphDefOf.RRY_HiddenXenomorphImpregnation).TryGetComp<HediffComp_XenoSpawner>().RoyaleHugger);
                    return RoyalHugger || RoyalHuggerInfection || RoyalImpregnation || RoyalHiddenImpregnation || QueenPresent;
                };
                return MyMap.mapPawns.AllPawnsSpawned.Any(validator);
            }
        }
        
        // Token: 0x060028F0 RID: 10480 RVA: 0x00136BB0 File Offset: 0x00134FB0
        public override void CompTick()
        {
            if (!this.TemperatureDamaged)
            {
                float ambientTemperature = this.parent.AmbientTemperature;
                float num = 1f / (this.Props.hatcherDaystoHatch * 60000f);

                if (ambientTemperature > -20f && parent.Map!=null)
                {
                    if (settings.SettingsHelper.latest.AllowXenoEggMetamorph)
                    {
                        if (this.mutateProgress < 1f && this.eggState != EggState.Normal)
                        {
                            this.mutateProgress += num;
                        }
                    }
                    else
                    {
                        /*
                        this.mutateProgress = 0;
                        this.eggState = EggState.Normal;
                        */
                    }
                    if (this.gestateProgress < 1f && (!this.EggMutating))
                    {
                        this.gestateProgress += num;
                    }
                    if (Find.TickManager.TicksGame % 250 == 0)
                    {
                        this.CompTickRare();

                        if (this.gestateProgress >= 1f)
                        {
                            if (this.canHatch && this.willHatch)
                            {
                                this.Hatch();
                            }
                        }
                    }
                }
            }
        }

        // Token: 0x0600295E RID: 10590 RVA: 0x00139BAC File Offset: 0x00137FAC
        public override void CompTickRare()
        {
            float ambientTemperature = this.parent.AmbientTemperature;
            flagRoyal = !(QueenPresent || RoyalPresent || RoyalEggPresent);
            flagHyper = XenomorphUtil.TotalSpawnedXenomorphPawnCount(MyMap) < 10 && EggsPresent(EggState.Hyperfertile, out List<Thing> HEggs) && HEggs.Count<3;
            flagPrae = false && EggsPresent(EggState.Praetorian, out List<Thing> PEggs) && PEggs.Count < 3;
            flagMutate = flagRoyal || flagHyper || flagPrae;
            if (flagMutate && this.gestateProgress > 0.25f && this.eggState == EggState.Normal)
            {
                TryMutate();
            }
            Thing thing = null;
            if (MyMap !=null && MyPos.InBounds(MyMap)) thing = GenClosest.ClosestThingReachable(MyPos, MyMap, ThingRequest.ForGroup(ThingRequestGroup.Pawn), PathEndMode.OnCell, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), Props.triggerRadius, x => XenomorphUtil.isInfectablePawn(((Pawn)x)), null, 0, -1, false, RegionType.Set_Passable, false);
            int huggercount = thing != null ? XenomorphUtil.TotalSpawnedFacehuggerPawnCount(MyMap, 10, (Pawn)thing) : 0;
            int hostcount = thing != null ? XenomorphUtil.TotalSpawnedInfectablePawnCount(MyMap, 10, this.MyPos) : 0;
            bool shouldHatch = huggercount < hostcount;
            if (thing != null && ambientTemperature > -20f)
            {
                Pawn pawn = (Pawn)thing;
                bool flag = XenomorphUtil.isInfectablePawn(pawn);
                if (selected)
                {
                //    Log.Message(string.Format("{0} isInfectable?: {1}", pawn.Label, flag));
                }
                if (flag)
                {
                    this.canHatch = true;
                }
                if (canHatch && shouldHatch && MyPos.DistanceTo(pawn.Position)<10f)
                {
                    float thingdist = MyPos.DistanceTo(pawn.Position);
                    float thingsize = pawn.BodySize;
                    float thingstealth = thing.GetStatValue(StatDefOf.HuntingStealth);
                    float thingmovespeed = thing.GetStatValue(StatDefOf.MoveSpeed);
                    if (selected)
                    {
                    //    Log.Message(string.Format("distance between {1} @{3} and {2} @ {4}: {0}, gestateProgress: {5}, mutateProgress: {6}", MyPos.DistanceTo(pawn.Position), this.parent.LabelShort, pawn.Label, MyPos, pawn.Position , gestateProgress, mutateProgress));
                    //    Log.Message(string.Format("{0} thingsize: {1}, thingstealth: {2}, thingmovespeed: {3}", pawn.Label, thingsize, thingstealth, thingmovespeed));
                    }

                    float hatchon = ((10*thingdist) - (thingsize * 5));
                    float roll = thingstealth > 0 ? (Rand.RangeInclusive(0, 100)* thingstealth): (Rand.RangeInclusive(0, 100));
                    if (roll>hatchon)
                    {
                        this.willHatch = true;
                    }
                    if (selected)
                    {
                    //    Log.Message(string.Format("{0} hatchon: {1}, roll: {2}, willHatch: {3}", pawn.Label, hatchon, roll, willHatch));
                    }
                }
            }
        }

        public void TryMutate()
        {
        //    Log.Message("TryMutate");
            float num = 1f / (this.Props.hatcherDaystoHatch * 60000f);
            float chance = 0.1f;
            EggState state = EggState.Normal;
            if (flagRoyal)
            {
                state = EggState.Royal;
                chance += 0.5f;
            }
            else if (flagPrae)
            {
                state = EggState.Praetorian;
                chance += 0.1f;
            }
            else if (flagHyper)
            {
                state = EggState.Hyperfertile;
            }
            if (flagMutate && Rand.Chance(chance))
            {
            //    Log.Message("Mutated");
                eggState = state;
                this.mutateProgress += num;
            }
            else
            {
            //    Log.Message("Mutation failed");
            }
        }

        // Token: 0x060028F1 RID: 10481 RVA: 0x00136C04 File Offset: 0x00135004
        public void Hatch()
        {
            try
            {
                PawnKindDef hatchKindDef = eggState==EggState.Royal ? XenomorphDefOf.RRY_Xenomorph_RoyaleHugger : this.Props.hatcherPawn;
                int spawncount = eggState == EggState.Hyperfertile ? Rand.RangeSeeded(1,5,AvPConstants.AvPSeed) : 1;
                PawnGenerationRequest request = new PawnGenerationRequest(hatchKindDef, this.hatcheeFaction, PawnGenerationContext.NonPlayer, -1, false, true, false, false, true, false, 1f, false, true, true, false, false, false, false);
                for (int i = 0; i < spawncount; i++)
                {
                    Pawn pawn = PawnGenerator.GeneratePawn(request);
                    if (PawnUtility.TrySpawnHatchedOrBornPawn(pawn, this.parent))
                    {
                        if (pawn != null)
                        {
                            if (this.hatcheeParent != null)
                            {
                                if (pawn.playerSettings != null && this.hatcheeParent.playerSettings != null && this.hatcheeParent.Faction == this.hatcheeFaction)
                                {
                                    pawn.playerSettings.AreaRestriction = this.hatcheeParent.playerSettings.AreaRestriction;
                                }
                                if (pawn.RaceProps.IsFlesh)
                                {
                                    pawn.relations.AddDirectRelation(PawnRelationDefOf.Parent, this.hatcheeParent);
                                }
                            }
                            if (this.otherParent != null && (this.hatcheeParent == null || this.hatcheeParent.gender != this.otherParent.gender) && pawn.RaceProps.IsFlesh)
                            {
                                pawn.relations.AddDirectRelation(PawnRelationDefOf.Parent, this.otherParent);
                            }
                        }
                        if (this.parent.Spawned)
                        {
                            FilthMaker.TryMakeFilth(MyPos, MyMap, ThingDefOf.Filth_AmnioticFluid, 1);
                        }
                    }
                    else
                    {
                        Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Discard);
                    }
                }
            }
            finally
            {
                this.parent.Destroy(DestroyMode.Vanish);
            }
        }
        /*
       // Token: 0x060028F2 RID: 10482 RVA: 0x00136DF8 File Offset: 0x001351F8
       public override void PreAbsorbStack(Thing otherStack, int count)
       {
           float t = (float)count / (float)(this.parent.stackCount + count);
           CompXenoHatcher comp = ((ThingWithComps)otherStack).GetComp<CompXenoHatcher>();
           float b = comp.gestateProgress;
           this.gestateProgress = Mathf.Lerp(this.gestateProgress, b, t);
       }

       // Token: 0x060028F3 RID: 10483 RVA: 0x00136E40 File Offset: 0x00135240
       public override void PostSplitOff(Thing piece)
       {
           CompXenoHatcher comp = ((ThingWithComps)piece).GetComp<CompXenoHatcher>();
           comp.gestateProgress = this.gestateProgress;
           comp.hatcheeParent = this.hatcheeParent;
           comp.otherParent = this.otherParent;
           comp.hatcheeFaction = this.hatcheeFaction;
       }

       // Token: 0x060028F4 RID: 10484 RVA: 0x00136E89 File Offset: 0x00135289
       public override void PrePreTraded(TradeAction action, Pawn playerNegotiator, ITrader trader)
       {
           base.PrePreTraded(action, playerNegotiator, trader);
           if (action == TradeAction.PlayerBuys)
           {
               this.hatcheeFaction = Faction.OfPlayer;
           }
           else if (action == TradeAction.PlayerSells)
           {
               this.hatcheeFaction = trader.Faction;
           }
       }
       */
        // Token: 0x060028F5 RID: 10485 RVA: 0x00136EBE File Offset: 0x001352BE
        public override void PostPostGeneratedForTrader(TraderKindDef trader, int forTile, Faction forFaction)
        {
            base.PostPostGeneratedForTrader(trader, forTile, forFaction);
            this.hatcheeFaction = forFaction;
        }

        // Token: 0x060028F6 RID: 10486 RVA: 0x00136ED0 File Offset: 0x001352D0
        public override string CompInspectStringExtra()
        {
            if (!this.TemperatureDamaged)
            {
                if (this.mutateProgress > 0f && !QueenPresent && Prefs.DevMode && DebugSettings.godMode)
                {
                    return "Xeno_Egg_Gestation_Progress".Translate() + ": " + this.gestateProgress.ToStringPercent() + "\n" + GetDescription(eggState)+ " " + "Xeno_Egg_Mutation_Progress".Translate() + ": " + this.mutateProgress.ToStringPercent();
                }
                return "Xeno_Egg_Gestation_Progress".Translate() + ": " + this.gestateProgress.ToStringPercent() + "\n" + GetDescription(eggState);
            }
            return null;
        }

        // Token: 0x17000630 RID: 1584
        // (get) Token: 0x060028EE RID: 10478 RVA: 0x00136B2C File Offset: 0x00134F2C
        public static string GetDescription(EggState category)
        {
            switch (category)
            {
                case EggState.Hyperfertile:
                    return "Xeno_Egg_Hyperfertile".Translate();
                case EggState.Praetorian:
                    return "Xeno_Egg_Praetorian".Translate();
                case EggState.Royal:
                    return "Xeno_Egg_Royal".Translate();
                default:
                    return "Xeno_Egg".Translate();
            }
        }

        public EggState eggState = EggState.Normal;
        public Pawn hatcheeParent;
        public Pawn otherParent;
        public Faction hatcheeFaction;
        public float gestateProgress;
        public float mutateProgress;
        public bool canHatch = false;
        public bool willHatch = false;
        public bool flagRoyal = false;
        public bool flagHyper = false;
        public bool flagPrae = false;
        public bool flagMutate = false;

        // Token: 0x060028EF RID: 10479 RVA: 0x00136B54 File Offset: 0x00134F54
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<float>(ref this.gestateProgress, "gestateProgress", 0f, false);
            Scribe_Values.Look<float>(ref this.mutateProgress, "royalProgress", 0f, false);
            Scribe_Values.Look<bool>(ref this.flagRoyal, "flagRoyal", false, false);
            Scribe_Values.Look<bool>(ref this.flagHyper, "flagHyper", false, false);
            Scribe_Values.Look<bool>(ref this.flagPrae, "flagPrae", false, false);
            Scribe_Values.Look<bool>(ref this.flagMutate, "flagMutate", false, false);
            Scribe_Values.Look<EggState>(ref this.eggState, "eggState", EggState.Normal, false);
            Scribe_References.Look<Pawn>(ref this.hatcheeParent, "hatcheeParent", false);
            Scribe_References.Look<Pawn>(ref this.otherParent, "otherParent", false);
            Scribe_References.Look<Faction>(ref this.hatcheeFaction, "hatcheeFaction", false);
        }

        public enum EggState : byte
        {
            // Token: 0x0400322C RID: 12844
            Normal,
            // Token: 0x0400322D RID: 12845
            Hyperfertile,
            // Token: 0x0400322E RID: 12846
            Praetorian,
            // Token: 0x0400322F RID: 12847
            Royal
        }
        private static PawnKindDef RoyalKindDef = XenomorphDefOf.RRY_Xenomorph_RoyaleHugger;
        private static PawnKindDef NormalKindDef = XenomorphDefOf.RRY_Xenomorph_FaceHugger;
    }
}
