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
        public float triggerRadius = 1f;
        // Token: 0x040016C4 RID: 5828
        public PawnKindDef hatcherPawn;
    }

    // Token: 0x02000744 RID: 1860
    public class CompXenoHatcher : ThingComp
    {
        // Token: 0x1700062E RID: 1582
        // (get) Token: 0x060028EC RID: 10476 RVA: 0x00136B12 File Offset: 0x00134F12
        public CompProperties_XenoHatcher Props
        {
            get
            {
                return (CompProperties_XenoHatcher)this.props;
            }
        }
        public bool canHatch = false;
        public bool willHatch = false;

        // Token: 0x1700062F RID: 1583
        // (get) Token: 0x060028ED RID: 10477 RVA: 0x00136B1F File Offset: 0x00134F1F
        private CompTemperatureRuinable FreezerComp
        {
            get
            {
                return this.parent.GetComp<CompTemperatureRuinable>();
            }
        }

        // Token: 0x17000630 RID: 1584
        // (get) Token: 0x060028EE RID: 10478 RVA: 0x00136B2C File Offset: 0x00134F2C
        public bool TemperatureDamaged
        {
            get
            {
                CompTemperatureRuinable freezerComp = this.FreezerComp;
                return freezerComp != null && this.FreezerComp.Ruined;
            }
        }

        // Token: 0x060028EF RID: 10479 RVA: 0x00136B54 File Offset: 0x00134F54
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<float>(ref this.gestateProgress, "gestateProgress", 0f, false);
            Scribe_Values.Look<float>(ref this.royalProgress, "royalProgress", 0f, false);
            Scribe_References.Look<Pawn>(ref this.hatcheeParent, "hatcheeParent", false);
            Scribe_References.Look<Pawn>(ref this.otherParent, "otherParent", false);
            Scribe_References.Look<Faction>(ref this.hatcheeFaction, "hatcheeFaction", false);
        }

        PawnKindDef RoyalKindDef = XenomorphDefOf.RRY_Xenomorph_RoyaleHugger;
        public bool RoyalPresent
        {
            get
            {
                bool selected = Find.Selector.SelectedObjects.Contains(this.parent) && Prefs.DevMode;
                Predicate<Pawn> validator = delegate (Pawn t)
                {
                    bool RoyalHugger = t.kindDef == RoyalKindDef;
                    bool RoyalHuggerInfection = (t.health.hediffSet.HasHediff(XenomorphDefOf.RRY_FaceHuggerInfection) && t.health.hediffSet.GetFirstHediffOfDef(XenomorphDefOf.RRY_FaceHuggerInfection).TryGetComp<HediffComp_XenoFacehugger>().RoyaleHugger);
                    bool RoyalImpregnation = (t.health.hediffSet.HasHediff(XenomorphDefOf.RRY_XenomorphImpregnation) && t.health.hediffSet.GetFirstHediffOfDef(XenomorphDefOf.RRY_XenomorphImpregnation).TryGetComp<HediffComp_XenoSpawner>().RoyaleHugger);
                    bool RoyalHiddenImpregnation = (t.health.hediffSet.HasHediff(XenomorphDefOf.RRY_HiddenXenomorphImpregnation) && t.health.hediffSet.GetFirstHediffOfDef(XenomorphDefOf.RRY_HiddenXenomorphImpregnation).TryGetComp<HediffComp_XenoSpawner>().RoyaleHugger);
                    return RoyalHugger || RoyalHuggerInfection || RoyalImpregnation || RoyalHiddenImpregnation;
                };
                return MyMap.mapPawns.AllPawnsSpawned.Any(validator);
            }
        }

        public Map MyMap
        {
            get
            {
                if (this.parent.Map!=null)
                {
                    return this.parent.Map;
                }
                else
                {
                    return this.parent.MapHeld;
                }
            }
        }

        public IntVec3 MyPos
        {
            get
            {
                if (this.parent.Position != null)
                {
                    return this.parent.Position;
                }
                else
                {
                    return this.parent.PositionHeld;
                }
            }
        }

        public bool QueenPresent
        {
            get
            {
                bool queenPresent = false;
                foreach (var p in MyMap.mapPawns.AllPawnsSpawned)
                {
                    if (p.kindDef == XenomorphDefOf.RRY_Xenomorph_Queen)
                    {
#if DEBUG
                    //    Log.Message(string.Format("Queen found"));
#endif
                        queenPresent = true;
                        break;
                    }
                }
                return queenPresent;
            }
        }

        // Token: 0x060028F0 RID: 10480 RVA: 0x00136BB0 File Offset: 0x00134FB0
        public override void CompTick()
        {
            if (!this.TemperatureDamaged)
            {
                float ambientTemperature = this.parent.AmbientTemperature;
                float num = 1f / (this.Props.hatcherDaystoHatch * 60000f);
                if (ambientTemperature > -20 && (this.parent.Spawned || this.ParentHolder is Pawn))
                {
                    if (this.gestateProgress < 1f)
                    {
                        this.gestateProgress += num;
                    }
                    else if (this.royalProgress < 1f && !QueenPresent && !XenomorphUtil.HivelikesPresent(MyMap) && !RoyalPresent)
                    {
                        this.royalProgress += num;
                    }
                }

                if (Find.TickManager.TicksGame % 250+(Rand.RangeInclusive(0,10)*10) == 0)
                {
                    this.CompTickRare();
                   
                    if (this.gestateProgress >= 1f)
                    {
                        bool selected = Find.Selector.SingleSelectedThing == this.parent;
                        if (selected) Log.Message(string.Format("{0} @ {1}, Can hatch?: {2}, Will hatch?: {3}", this.parent.Label, MyPos, canHatch, willHatch));
                        if (this.canHatch && this.willHatch)
                        {
                            this.Hatch();
                        }
                    }
                }
            }
        }
        public float DistanceBetween(IntVec3 a, IntVec3 b)
        {
            double distance = GetDistance(a.x, a.z, b.x, b.z);
            return (float)distance;
        }
        private static double GetDistance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
        }
        // Token: 0x0600295E RID: 10590 RVA: 0x00139BAC File Offset: 0x00137FAC
        
        public override void CompTickRare()
        {
            
            bool selected = Find.Selector.SelectedObjects.Contains(this.parent);
            Thing thing = null;
            if (MyMap !=null && MyPos.InBounds(MyMap)) thing = GenClosest.ClosestThingReachable(MyPos, MyMap, ThingRequest.ForGroup(ThingRequestGroup.Pawn), PathEndMode.OnCell, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), Props.triggerRadius, x => XenomorphUtil.isInfectablePawn(((Pawn)x)), null, 0, -1, false, RegionType.Set_Passable, false);
            int huggercount = thing != null ? XenomorphUtil.TotalSpawnedFacehuggerPawnCount(MyMap, 10, (Pawn)thing) : 0;
            int hostcount = thing != null ? XenomorphUtil.TotalSpawnedInfectablePawnCount(MyMap, 10, this.MyPos) : 0;
            bool shouldHatch = huggercount < hostcount;
            if (thing != null)
            {
                Pawn pawn = (Pawn)thing;
                bool flag = XenomorphUtil.isInfectablePawn(pawn);
                if (selected)
                {
                    Log.Message(string.Format("{0} isInfectable?: {1}", pawn.Label, flag));
                }
                if (flag)
                {
                    this.canHatch = true;
                }
                if (canHatch && shouldHatch && DistanceBetween(MyPos, pawn.Position)<10f)
                {
                    float thingdist = DistanceBetween(MyPos, pawn.Position);
                    float thingsize = pawn.BodySize;
                    float thingstealth = thing.GetStatValue(StatDefOf.HuntingStealth);
                    float thingmovespeed = thing.GetStatValue(StatDefOf.MoveSpeed);
                    if (selected)
                    {
                        Log.Message(string.Format("distance between {1} @{3} and {2} @ {4}: {0}", DistanceBetween(MyPos, pawn.Position), this.parent.LabelShort, pawn.Label, MyPos, pawn.Position));
                        Log.Message(string.Format("{0} thingsize: {1}, thingstealth: {2}, thingmovespeed: {3}", pawn.Label, thingsize, thingstealth, thingmovespeed));
                    }

                    float hatchon = ((10*thingdist) / thingsize);
                    float roll = thingstealth > 0 ? (Rand.RangeInclusive(0, 100)* thingstealth): (Rand.RangeInclusive(0, 100));
                    if (selected)
                    {
                        Log.Message(string.Format("{0} hatchon: {1}, roll: {2}", pawn.Label, hatchon, roll));
                    }
                    if (roll>hatchon)
                    {
                        this.willHatch = true;
                    }
                }
            }
            if (thing == null)
            {
                if (selected) Log.Message(string.Format("{0} @ {1}, Cant hatch No suitable Host Found", this.parent.Label, MyPos, canHatch));
            }
        }


        // Token: 0x060028F1 RID: 10481 RVA: 0x00136C04 File Offset: 0x00135004
        public void Hatch()
        {
            try
            {
                PawnKindDef hatchKindDef = Rand.Chance(royalProgress) && !QueenPresent && !XenomorphUtil.HivelikesPresent(MyMap) && !RoyalPresent ? XenomorphDefOf.RRY_Xenomorph_RoyaleHugger : this.Props.hatcherPawn;
#if DEBUG
            //    Log.Message(string.Format("hatchKindDef: {0}", hatchKindDef));
#endif
                PawnGenerationRequest request = new PawnGenerationRequest(hatchKindDef, this.hatcheeFaction, PawnGenerationContext.NonPlayer, -1, false, true, false, false, true, false, 1f, false, true, true, false, false, false, false, null, null, null, null, null, null, null, null);
                for (int i = 0; i < this.parent.stackCount; i++)
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
                            FilthMaker.MakeFilth(MyPos, MyMap, ThingDefOf.Filth_AmnioticFluid, 1);
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
                if (this.royalProgress > 0f && !QueenPresent && Prefs.DevMode)
                {
                    return "EggProgress".Translate() + ": " + this.gestateProgress.ToStringPercent() + "\n" + "RoyalProgress".Translate() + ": " + this.royalProgress.ToStringPercent();
                }
                return "EggProgress".Translate() + ": " + this.gestateProgress.ToStringPercent();
            }
            return null;
        }

        public override void PostDraw()
        {
            base.PostDraw();
        }

        // Token: 0x040016C5 RID: 5829
        private float gestateProgress;
        public float royalProgress;

        // Token: 0x040016C6 RID: 5830
        public Pawn hatcheeParent;

        // Token: 0x040016C7 RID: 5831
        public Pawn otherParent;

        // Token: 0x040016C8 RID: 5832
        public Faction hatcheeFaction;
    }
}
