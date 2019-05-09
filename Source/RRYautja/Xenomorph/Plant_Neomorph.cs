using System;
using System.Collections.Generic;
using RimWorld;
using RRYautja;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RRYautja
{
    // Token: 0x02000B78 RID: 2936
    public class Plant_Neomorph : Plant
    {
        public override void TickLong()
        {
            base.TickLong();
            bool selected = Find.Selector.SingleSelectedThing == this;
            if (!this.IsBurning()&& !this.Destroyed && this.Map!=null)
            {
                Thing thing = GenClosest.ClosestThingReachable(this.Position, this.Map, ThingRequest.ForGroup(ThingRequestGroup.Pawn), PathEndMode.OnCell, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), 6f, x => XenomorphUtil.isInfectablePawn(((Pawn)x)), null, 0, -1, false, RegionType.Set_Passable, false);
                if (thing != null && this.Growth > 0.95f && !thing.Destroyed && !((Pawn)thing).Dead)
                {
                    List<Thing> thingList = GridsUtility.GetThingList(thing.Position, this.Map);
                    Thing thing2;
                    if (this.def == XenomorphDefOf.RRY_Plant_Neomorph_Fungus)
                    {
                        thing2 = ThingMaker.MakeThing(XenomorphDefOf.RRY_Neomorph_Spores);
                    }
                    else
                    {
                        thing2 = ThingMaker.MakeThing(XenomorphDefOf.RRY_Neomorph_Spores_Hidden);
                    }
                    float Chance = ((0.5f * Growth) * ((Pawn)thing).BodySize) / DistanceBetween(this.Position, thing.Position);
                    if (selected) Log.Message(string.Format("Chance: {0}", Chance));
                    if (Rand.Chance(Chance) && !thingList.Exists(x => x.def == XenomorphDefOf.RRY_Neomorph_Spores))
                    {
                        if (thing.Faction == Faction.OfPlayer)
                        {
                            string text = TranslatorFormattedStringExtensions.Translate("Xeno_Neospores_Trigger", thing.LabelShortCap, this.Label);
                            Log.Message(text);
                            MoteMaker.ThrowText(this.Position.ToVector3(), this.Map, text, 5f);
                        }
                        GenSpawn.Spawn(thing2, thing.Position, this.Map);
                    }
                }
                else
                {
                    Plant plant = (Plant)GenClosest.ClosestThingReachable(this.Position, this.Map, ThingRequest.ForGroup(ThingRequestGroup.Plant), PathEndMode.OnCell, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), 3f, x => ((Plant)x).Growth > 0.65f && x.def != XenomorphDefOf.RRY_Plant_Neomorph_Fungus && x.def != XenomorphDefOf.RRY_Plant_Neomorph_Fungus_Hidden && (x.def.defName.Contains("Grass") || x.def.defName.Contains("Moss")), null, 0, -1, false, RegionType.Set_Passable, false);
                    if (plant != null && !plant.Destroyed && !plant.IsBurning() && this.Growth > 0.95f)
                    {
                        if (selected) Log.Message(string.Format("plant: {0}", plant));
                        float Chance2 = ((0.5f * Growth) * plant.Growth / DistanceBetween(this.Position, plant.Position));
                        if (selected) Log.Message(string.Format("Chance2: {0}", Chance2));
                        if (Rand.Chance(Chance2))
                        {
                            Thing thing2;
                            if (this.def == XenomorphDefOf.RRY_Plant_Neomorph_Fungus)
                            {
                                thing2 = ThingMaker.MakeThing(XenomorphDefOf.RRY_Neomorph_Spores);
                            }
                            else
                            {
                                thing2 = ThingMaker.MakeThing(XenomorphDefOf.RRY_Neomorph_Spores_Hidden);
                            }
                            if (selected) Log.Message(string.Format("was: {0}", plant));
                            IntVec3 vec3 = plant.Position;
                            GenSpawn.Spawn(thing2, vec3, this.Map);
                            plant.Destroy();
                            GenSpawn.Spawn(ThingMaker.MakeThing(this.def), vec3, this.Map);

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
    }
}
