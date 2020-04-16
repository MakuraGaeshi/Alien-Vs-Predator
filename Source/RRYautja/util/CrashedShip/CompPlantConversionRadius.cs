using RRYautja.ExtensionMethods;
using System;
using Verse;

namespace RimWorld
{
    // Token: 0x02000751 RID: 1873
    public class CompProperties_PlantConversionRadius : CompProperties
    {
        // Token: 0x06002955 RID: 10581 RVA: 0x0013996B File Offset: 0x00137D6B
        public CompProperties_PlantConversionRadius()
        {
            this.compClass = typeof(CompPlantConversionRadius);
        }
        
        // Token: 0x040016EA RID: 5866
        public float harmFrequencyPerArea = 1f;
        // Token: 0x040004CC RID: 1228
        public float addAmount = 0.12f;

        // Token: 0x040016E9 RID: 5865
        public SimpleCurve radiusPerDayCurve;

        // Token: 0x040004CD RID: 1229
        public float maxRadius = 55f;
    }

    // Token: 0x02000752 RID: 1874
    public class CompPlantConversionRadius : ThingComp
    {
        // Token: 0x17000656 RID: 1622
        // (get) Token: 0x06002957 RID: 10583 RVA: 0x001399A1 File Offset: 0x00137DA1
        public CompProperties_PlantConversionRadius PropsPlantHarmRadius
        {
            get
            {
                return (CompProperties_PlantConversionRadius)this.props;
            }
        }

        public CompGooSpread PlantHarmRadius
        {
            get
            {
                return (CompGooSpread)this.parent.TryGetComp<CompGooSpread>();
            }
        }

        // Token: 0x06002958 RID: 10584 RVA: 0x001399AE File Offset: 0x00137DAE
        public override void PostExposeData()
        {
            Scribe_Values.Look<int>(ref this.plantHarmAge, "plantHarmAge", 0, false);
            Scribe_Values.Look<int>(ref this.ticksToPlantHarm, "ticksToPlantHarm", 0, false);
        }

        // Token: 0x06002959 RID: 10585 RVA: 0x001399D4 File Offset: 0x00137DD4
        public override void CompTick()
        {
            if (!this.parent.Spawned)
            {
                return;
            }
            this.plantHarmAge++;
            this.ticksToPlantHarm--;
            if (this.ticksToPlantHarm <= 0)
            {
                float x = (float)this.plantHarmAge / 60000f;
                float num = (this.PlantHarmRadius != null ? this.PlantHarmRadius.Props.radiusPerDayCurve.Evaluate(x) : this.PropsPlantHarmRadius.radiusPerDayCurve.Evaluate(x))-3;

            //    Log.Message(string.Format("PlantHarmRadius: {0}, {1}", num, this.PlantHarmRadius != null));
                float num2 = 3.14159274f * num * num;
                float num3 = num2 * this.PropsPlantHarmRadius.harmFrequencyPerArea;
                float num4 = 60f / num3;
                int num5;
                if (num4 >= 1f)
                {
                    this.ticksToPlantHarm = GenMath.RoundRandom(num4);
                    num5 = 1;
                }
                else
                {
                    this.ticksToPlantHarm = 1;
                    num5 = GenMath.RoundRandom(1f / num4);
                }
                for (int i = 0; i < num5; i++)
                {
                    this.HarmRandomPlantInRadius(num);
                }
            }
        }

        // Token: 0x0600295A RID: 10586 RVA: 0x00139AB0 File Offset: 0x00137EB0
        private void HarmRandomPlantInRadius(float radius)
        {
            IntVec3 c = this.parent.Position + (Rand.InsideUnitCircleVec3 * radius).ToIntVec3();
            if (!c.InBounds(this.parent.Map))
            {
                return;
            }
            Plant plant = c.GetPlant(this.parent.Map);
            bool flag = c.GetThingList(this.parent.Map).Any(x=> x.def.defName.Contains("RRY_Plant_Neomorph_Fungus"));
            if (plant != null && !flag)
            {
                if (Rand.Value < this.LeaflessPlantKillChance && this.parent.Map.GooGrid().GetDepth(c) >= 1f)
                {
                    if (RRYautja.settings.SettingsHelper.latest.AllowNeomorphs)
                    {
                        Thing thing2;

                        if (!PlayerKnowledgeDatabase.IsComplete(XenomorphConceptDefOf.RRY_Concept_SporeSac))
                        {
                            thing2 = ThingMaker.MakeThing(XenomorphDefOf.RRY_Plant_Neomorph_Fungus_Hidden);
                        }
                        else
                        {
                            thing2 = ThingMaker.MakeThing(XenomorphDefOf.RRY_Plant_Neomorph_Fungus);
                        }
                        IntVec3 vec3 = plant.Position;
                        GenSpawn.Spawn(thing2, vec3, plant.Map, WipeMode.Vanish);
                    }
                    else
                    {
                           plant.Destroy();
                    }
                    //    GenSpawn.Spawn(ThingMaker.MakeThing(this.def), vec3, this.Map);
                }
            }
        }

        // Token: 0x040016EB RID: 5867
        public int plantHarmAge;

        // Token: 0x040016EC RID: 5868
        private int ticksToPlantHarm;

        // Token: 0x040016ED RID: 5869
        private float LeaflessPlantKillChance = 0.05f;
    }
}
