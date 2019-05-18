using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace RRYautja
{
    public class Filth_AddAcidDamageDef : ThingDef
    {
        
    }

    public class Filth_AddAcidDamage : Filth
    {
        // Token: 0x17000003 RID: 3
        // (get) Token: 0x06000017 RID: 23 RVA: 0x000027A2 File Offset: 0x000009A2
        // (set) Token: 0x06000018 RID: 24 RVA: 0x000027AA File Offset: 0x000009AA
        public object cachedLabelMouseover { get; private set; }
        public bool active = true;
        // Token: 0x06000019 RID: 25 RVA: 0x000027B3 File Offset: 0x000009B3
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.RecalcPathsOnAndAroundMe(map);
            this.destroyTick = Find.TickManager.TicksGame + (Rand.Range(29, 121) * 100);
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            Map map = base.Map;
            base.DeSpawn(mode);
            this.RecalcPathsOnAndAroundMe(map);
        }

        private void RecalcPathsOnAndAroundMe(Map map)
        {
            IntVec3[] adjacentCellsAndInside = GenAdj.AdjacentCellsAndInside;
            for (int i = 0; i < adjacentCellsAndInside.Length; i++)
            {
                IntVec3 c = base.Position + adjacentCellsAndInside[i];
                if (c.InBounds(map))
                {
                    map.pathGrid.RecalculatePerceivedPathCostAt(c);
                }
            }
        }

        // Token: 0x0600001A RID: 26 RVA: 0x000027C0 File Offset: 0x000009C0
        public override void Tick()
        {
            bool destroyed = base.Destroyed;
            if (!destroyed && this.active)
            {
                bool flag = this.destroyTick <= Find.TickManager.TicksGame && !base.Destroyed;
                if (flag)
                {
                    this.active = false;
                }
                else
                {
                    this.Ticks--;
                    bool flag2 = this.Ticks <= 0;
                    if (flag2)
                    {
                        this.TickTack();
                        this.Ticks = this.TickRate;
                    }
                }
            }
        }

        // Token: 0x0600001B RID: 27 RVA: 0x00002854 File Offset: 0x00000A54
        public void TickTack()
        {
            bool destroyed = base.Destroyed;
            if (!destroyed)
            {
                List<Thing> thingList = GridsUtility.GetThingList(base.Position, base.Map);
                for (int i = 0; i < thingList.Count; i++)
                {
                    bool flag = thingList[i] != null;
                    if (flag)
                    {
                        Thing thing = thingList[i];
                        Pawn pawn = thingList[i] as Pawn;
                        bool flaga = thing.def.useHitPoints && !this.touchingThings.Contains(thing) && thing.def != XenomorphDefOf.RRY_FilthBloodXenomorph && thing.GetType() != typeof(Pawn);
                        bool flag2 = thing != null && !this.touchingThings.Contains(thing) && thing.def != XenomorphDefOf.RRY_FilthBloodXenomorph && thing.GetType() != typeof(Mote) && thing.GetType() != typeof(MoteThrown) && thing.GetType() != typeof(Bullet) && thing.GetType() != typeof(Pawn);
                        bool flag2a = !(thing is Corpse corpse && XenomorphUtil.IsXenoCorpse(corpse));
                        bool flag2b = !(thing is Pawn && XenomorphUtil.IsXenomorph((Pawn)thing));
                        if (flaga && flag2a && flag2b )
                        {
                            this.touchingThings.Add(thing);
                            this.damageEntities(thing, Mathf.RoundToInt((float)this.AcidDamage * Rand.Range(0.5f, 1.25f)));
                            MoteMaker.ThrowDustPuff(thing.Position, base.Map, 0.2f);
                        }
                        bool flag3 = pawn != null;
                        if (flag3 && flag2b)
                        {
                            this.touchingPawns.Add(pawn);
                            bool flag4 = !XenomorphUtil.IsXenomorph(pawn);
                            if (flag4)
                            {
                                this.addAcidDamage(pawn);
                                MoteMaker.ThrowDustPuff(pawn.Position, base.Map, 0.2f);
                            }
                        }
                    }
                }   
                /*
                for (int j = 0; j < this.touchingPawns.Count; j++)
                {
                    Pawn pawn2 = this.touchingPawns[j];
                    bool flag5 = !pawn2.Spawned || pawn2.Position != base.Position || XenomorphUtil.IsXenomorph(pawn2);
                    if (flag5)
                    {
                        this.touchingPawns.Remove(pawn2);
                    }
                    else
                    {
                        bool flag6 = !pawn2.RaceProps.Animal;
                        if (flag6)
                        {
                            this.addAcidDamage(pawn2);
                        }
                    }
                }
                for (int k = 0; k < this.touchingThings.Count; k++)
                {
                    Thing thing2 = this.touchingThings[k];
                    bool flag7 = !thing2.Spawned || thing2.Position != base.Position;
                    if (flag7)
                    {
                        this.touchingThings.Remove(thing2);
                    }
                    else
                    {
                        this.damageEntities(thing2, Mathf.RoundToInt((float)this.AcidDamage * Rand.Range(0.5f, 1.25f)));
                    }
                }
                this.damageBuildings(Mathf.RoundToInt((float)this.AcidDamage * Rand.Range(0.5f, 1.25f)));
                this.cachedLabelMouseover = null;
                */
            }
        }

        // Token: 0x0600001C RID: 28 RVA: 0x00002AD4 File Offset: 0x00000CD4
        public void damageEntities(Thing e, int amt)
        {
            if (e is Pawn)
            {
                return;
            }
            DamageInfo damageInfo;
            damageInfo = new DamageInfo(XenomorphDefOf.RRY_AcidBurn, (float)amt, 0f, -1f, null, null, null, 0, null);
            bool flag = e != null;
            if (!flag||e.Stuff!=null)
            {
                if (e.Stuff!=XenomorphDefOf.RRY_Leather_Xenomorph) return;
            }
            if (e.def.costList!=null)
            {
                foreach (var cost in e.def.costList)
                {
                    if (cost.thingDef == XenomorphDefOf.RRY_Xenomorph_TailSpike || cost.thingDef == XenomorphDefOf.RRY_Xenomorph_HeadShell)
                    {
                        return;
                    }
                }
            }
            if (flag)
            {
                e.TakeDamage(damageInfo);
                MoteMaker.ThrowDustPuff(e.Position, base.Map, 0.2f);
            }
        }

        // Token: 0x0600001D RID: 29 RVA: 0x00002B28 File Offset: 0x00000D28
        public void damageBuildings(int amt)
        {
            IntVec3 intVec = GenAdj.RandomAdjacentCell8Way(this);
            bool flag = GenGrid.InBounds(intVec, base.Map);
            bool flag2 = flag;
            if (flag2)
            {
                Building firstBuilding = GridsUtility.GetFirstBuilding(intVec, base.Map);
                DamageInfo damageInfo;
                damageInfo = new DamageInfo(XenomorphDefOf.RRY_AcidBurn, (float)amt, 0f, -1f, null, null, null, 0, null);
                bool flag3 = firstBuilding != null;
                bool flag4 = flag3;
                if (flag4)
                {
                    MoteMaker.ThrowDustPuff(firstBuilding.Position, base.Map, 0.2f);
                    firstBuilding.TakeDamage(damageInfo);
                }
            }
        }

        // Token: 0x0600001E RID: 30 RVA: 0x00002BAC File Offset: 0x00000DAC
        public void addAcidDamage(Pawn p)
        {
        //    Log.Message(string.Format("addAcidDamage: {0}", p.LabelShort));
            List<BodyPartRecord> list = new List<BodyPartRecord>();
        //    Log.Message(string.Format("0 "));
            List<Apparel> wornApparel = new List<Apparel>();
            if (p.RaceProps.Humanlike) wornApparel = p.apparel.WornApparel;
        //    Log.Message(string.Format("1 "));
            int num = Mathf.RoundToInt((float)this.AcidDamage * Rand.Range(0.5f, 1.25f));
        //    Log.Message(string.Format("2 "));
            DamageInfo damageInfo = default(DamageInfo);
        //    Log.Message(string.Format("3 "));
            MoteMaker.ThrowDustPuff(p.Position, base.Map, 0.2f);
        //    Log.Message(string.Format("4 "));
            BodyPartHeight bodyPartHeight = p.Downed ? BodyPartHeight.Undefined : BodyPartHeight.Bottom;
        //    Log.Message(string.Format("{0}", bodyPartHeight));
            foreach (BodyPartRecord bodyPartRecord in p.health.hediffSet.GetNotMissingParts(bodyPartHeight, BodyPartDepth.Outside, null, null))
            {
            //    Log.Message(string.Format("{0}", bodyPartRecord.Label));
                bool flag = wornApparel.Count > 0;
                if (flag)
                {
                    bool flag2 = false;
                    for (int i = 0; i < wornApparel.Count; i++)
                    {
                        bool flag3 = wornApparel[i].def.apparel.CoversBodyPart(bodyPartRecord);
                        if (flag3)
                        {
                            flag2 = true;
                        //    Log.Message(string.Format("is protected"));
                            break;
                        }
                    }
                    bool flag4 = !flag2;
                    if (flag4)
                    {
                    //    Log.Message(string.Format("{0}", bodyPartRecord));
                        list.Add(bodyPartRecord);
                    }
                }
                else
                {
                //    Log.Message(string.Format("{0}", bodyPartRecord));
                    list.Add(bodyPartRecord);
                }
            }
            for (int k = 0; k < list.Count; k++)
            {
                damageInfo = new DamageInfo(XenomorphDefOf.RRY_AcidBurn, (float)Mathf.RoundToInt(((float)num * list[k].coverage)*10), 0f, -1f, this, list[k], null, 0, null);
                //    Log.Message(string.Format("addAcidDamage TakeDamage: {0}, list[k].coverage: {1}, damageInfo: {2}", list[k].customLabel, list[k].coverage, damageInfo));
                if (Rand.Chance(list[k].coverage))
                {

                    for (int j = 0; j < wornApparel.Count; j++)
                    {
                        bool flag3 = wornApparel[j].def.apparel.CoversBodyPart(list[k]);
                        if (flag3)
                        {
                            this.damageEntities(wornApparel[j], num);
                        }
                    }
                    p.TakeDamage(damageInfo);
                    break;
                }
            }
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.destroyTick, "destroyTick", 0, false);
        }

        public int destroyTick;

        private List<Pawn> touchingPawns = new List<Pawn>();

        private List<Thing> touchingThings = new List<Thing>();

        private int Ticks = 100;

        private int TickRate = 100;

        private int AcidDamage = 3;
    }
}
