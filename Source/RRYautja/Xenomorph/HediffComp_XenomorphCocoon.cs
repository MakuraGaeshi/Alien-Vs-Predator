using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RRYautja
{
    public class HediffCompProperties_XenomorphCocoon : HediffCompProperties
    {
        public HediffCompProperties_XenomorphCocoon()
        {
            this.compClass = typeof(HediffComp_XenomorphCocoon);
        }


    }

    public class HediffComp_XenomorphCocoon : HediffComp
    {
        public HediffCompProperties_XenomorphCocoon XenoProps
        {
            get
            {
                return this.props as HediffCompProperties_XenomorphCocoon;
            }
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look<float>(ref this.conversionProgress, "conversionProgress", 0f, false);
        }

        private List<Rot4> Rotlist = new List<Rot4>
        {
            Rot4.North,
            Rot4.South,
            Rot4.East,
            Rot4.West
        };

        public IntVec3 MyPos
        {
            get
            {
                return Pawn.Position != null ? Pawn.Position : Pawn.Position;
            }
        }

        public Map MyMap
        {
            get
            {
                return Pawn.Map ?? Pawn.MapHeld;
            }
        }

        public ThingDef MyCocoonDef
        {
            get
            {
                return Pawn.RaceProps.Humanlike ? XenomorphDefOf.RRY_Xenomorph_Humanoid_Cocoon : XenomorphDefOf.RRY_Xenomorph_Animal_Cocoon;
            }
        }

        public Building_XenomorphCocoon MyCocoon
        {
            get
            {
                if (Pawn.InBed()&&Pawn.CurrentBed() is Building_XenomorphCocoon mycocoon)
                {
                    return mycocoon;
                }
                return XenomorphUtil.ClosestReachableEmptyCocoon(Pawn, MyCocoonDef).Position == Pawn.Position ? (Building_XenomorphCocoon)XenomorphUtil.ClosestReachableEmptyCocoon(Pawn, MyCocoonDef): null;
            }
        }

        PawnKindDef RoyalKindDef = XenomorphDefOf.RRY_Xenomorph_RoyaleHugger;
        public bool RoyalPresent
        {
            get
            {
                bool selected = Find.Selector.SelectedObjects.Contains(Pawn) && Prefs.DevMode;
                Predicate<Pawn> validator = delegate (Pawn t)
                {
                    bool RoyalHugger = t.kindDef == RoyalKindDef;
                    bool RoyalHuggerInfection = (t.health.hediffSet.HasHediff(XenomorphDefOf.RRY_FaceHuggerInfection) && t.health.hediffSet.GetFirstHediffOfDef(XenomorphDefOf.RRY_FaceHuggerInfection).TryGetComp<HediffComp_XenoFacehugger>().RoyaleHugger);
                    bool RoyalImpregnation = (t.health.hediffSet.HasHediff(XenomorphDefOf.RRY_XenomorphImpregnation) && t.health.hediffSet.GetFirstHediffOfDef(XenomorphDefOf.RRY_XenomorphImpregnation).TryGetComp<HediffComp_XenoSpawner>().RoyaleHugger);
                    bool RoyalHiddenImpregnation = (t.health.hediffSet.HasHediff(XenomorphDefOf.RRY_HiddenXenomorphImpregnation) && t.health.hediffSet.GetFirstHediffOfDef(XenomorphDefOf.RRY_HiddenXenomorphImpregnation).TryGetComp<HediffComp_XenoSpawner>().RoyaleHugger);
#if DEBUG
                    if (this.conversionProgress >= 1f && selected && Prefs.DevMode) Log.Message(string.Format("RoyalHugger: {0}, RoyalHuggerInfection: {1}, RoyalImpregnation: {2}, RoyalHiddenImpregnation: {3}", RoyalHugger , RoyalHuggerInfection , RoyalImpregnation , RoyalHiddenImpregnation));
#endif
                    return RoyalHugger || RoyalHuggerInfection || RoyalImpregnation || RoyalHiddenImpregnation;
                };
                return MyMap.mapPawns.AllPawnsSpawned.Any(validator);
            }
        }

        public int cocoonedCount
        {
            get
            {
                List<Pawn> pawns = MyMap.mapPawns.AllPawnsSpawned.Where(x => x.health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Cocooned)).ToList();
                return pawns.Count;
            }
        }

        PawnKindDef QueenKindDef = XenomorphDefOf.RRY_Xenomorph_Queen;
        public bool QueenPresent
        {
            get
            {
                return MyMap.mapPawns.AllPawnsSpawned.Any(x => x.kindDef == QueenKindDef);
            }
        }

        PawnKindDef PredalienKindDef = XenomorphDefOf.RRY_Xenomorph_Predalien;
        public bool PredalienPresent
        {
            get
            {
                return MyMap.mapPawns.AllPawnsSpawned.Any(x => x.kindDef == PredalienKindDef);
            }
        }

        public bool RoyalEggPresent
        {
            get
            {
                return MyMap.listerThings.ThingsOfDef(XenomorphDefOf.RRY_EggXenomorphFertilized).Any(x => x is Building_XenoEgg egg && egg.xenoHatcher.royalProgress>=1f);
            }
        }
        ThingDef eggDef = XenomorphDefOf.RRY_EggXenomorphFertilized;

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);
            if (Pawn.CurrentBed() == null && MyPos != null && MyMap != null && Pawn.Spawned)
            {
                Rot4 rot = Rotlist.RandomElement();
                Thing thing = ThingMaker.MakeThing(MyCocoonDef);
                GenSpawn.Spawn(thing, MyPos, MyMap, rot, WipeMode.Vanish, false);
                this.Pawn.jobs.Notify_TuckedIntoBed((Building_XenomorphCocoon)thing);
                this.Pawn.mindState.Notify_TuckedIntoBed();
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            bool selected = Find.Selector.SelectedObjects.Contains(Pawn) && Prefs.DevMode;
            base.CompPostTick(ref severityAdjustment);
            if (!QueenPresent && !PredalienPresent)
            {
                float num = 1f / ((3* Pawn.BodySize) * 60000f);
                if (this.conversionProgress < 1f)
                {
                    this.conversionProgress += num;
                }
            }
            if (Find.TickManager.TicksGame % 300 == 0)
            {
                if (Pawn.CurrentBed() == null)
                {
                    Pawn.health.RemoveHediff(this.parent);
                }
#if DEBUG
                if (this.conversionProgress >= 1f && selected) Log.Message(string.Format("QueenPresent: {0}, PredalienPresent: {1}, RoyalEggPresent: {2}, RoyalPresent: {3}", QueenPresent, PredalienPresent, RoyalEggPresent, RoyalPresent));
#endif
                if (!QueenPresent && !PredalienPresent)
                {
                    if (this.conversionProgress >= 1f&& XenomorphUtil.TotalSpawnedEggCount(MyMap)<(cocoonedCount / 2) && !XenomorphUtil.IsInfectedPawn(Pawn))
                    {
                        if (Rand.Chance(0.05f))
                        {
                            Thing thing = ThingMaker.MakeThing(eggDef, null);
                            Building_XenoEgg _XenoEgg = (Building_XenoEgg)thing;
                            CompXenoHatcher xenoHatcher = _XenoEgg.TryGetComp<CompXenoHatcher>();
                            if (!RoyalEggPresent && !RoyalPresent) xenoHatcher.royalProgress = Pawn.BodySize;
                            MyCocoon.Destroy();
                            GenPlace.TryPlaceThing(thing, Pawn.Position != null ? Pawn.Position : Pawn.PositionHeld, Pawn.Map != null ? Pawn.Map : Pawn.MapHeld, ThingPlaceMode.Direct);
                        //    Pawn.health.RemoveHediff(this.parent);
                            Pawn.Destroy();
                        }
                        else
                        {
#if DEBUG
                            Log.Message(string.Format("{0}, failed convert chance", conversionProgress));
#endif
                        }
                    }
                }
            }
        }

        public float conversionProgress;
    }
}
